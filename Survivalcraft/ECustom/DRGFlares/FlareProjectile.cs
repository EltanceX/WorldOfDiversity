using Engine;
using System;
using GameEntitySystem;
using TemplatesDatabase;
using System.Globalization;
using Acornima;
using Jint.Native;
using System.Diagnostics;
using Game;
using Random = Game.Random;
using GMLForAPI;
using DebugMod;

namespace GlassMod
{
    public class FlareProjectile : Projectile
    {
        public TerrainChunk LightChunk;
        public SubsystemSky subsystemSky;
        public ComponentPlayer componentPlayer;
        public SubsystemGameInfo gameInfo;
        public SubsystemGameWidgets gameWidgets;
        public bool TrajectoryUpdated = false;
        public List<int[]> Trajectory = new();
        public int MaxTrajectoryStorage = 5;
        public int RandomTextureIndex = 0;
        public FlareProjectile()
        {
            //Debugger.Break();
            TerrainKnockBack = 0.5f;
            RandomTextureIndex = GUtil.random.Next(FlareBlock.FlareTextures.Count);
            if (RandomTextureIndex >= FlareBlock.FlareTextures.Count) Debugger.Break();
            //Debugger.Break();
        }
        public void LoadSubsystemAndComponent(Project project)
        {
            if (project == null) return;

            subsystemSky = project.FindSubsystem<SubsystemSky>();
            foreach (var entity in project.Entities)
            {
                var player = entity.Components.Where(x => x is ComponentPlayer).FirstOrDefault() as ComponentPlayer;
                if (player != null)
                {
                    componentPlayer = player;
                    break;
                }
            }
            gameWidgets = project.FindSubsystem<SubsystemGameWidgets>();
            gameInfo = project.FindSubsystem<SubsystemGameInfo>();
        }
        public override void Initialize(int value, Vector3 position, Vector3 velocity, Vector3 angularVelocity, Entity owner)
        {
            //Debugger.Break();
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
            Value = value;
            Position = position;
            Velocity = velocity;
            Rotation = Vector3.Zero;
            AngularVelocity = angularVelocity;
            OwnerEntity = owner;
            Damping = block.GetProjectileDamping(value);
            ProjectileStoppedAction = ProjectileStoppedAction.TurnIntoPickable;
            MaxTimeExist = 90f;


            //subsystemSky = Owner.Project.FindSubsystem<SubsystemSky>();
            //componentPlayer = Owner.Entity.FindComponent<ComponentPlayer>();
            //gameWidgets = Owner.Project.FindSubsystem<SubsystemGameWidgets>();
            //gameInfo = Owner.Project.FindSubsystem<SubsystemGameInfo>();
            LoadSubsystemAndComponent(GameManager.Project);

        }
        public override void Initialize(int value, Vector3 position, Vector3 velocity, Vector3 angularVelocity, ComponentCreature owner)
        {
            Initialize(value, position, velocity, angularVelocity, owner?.Entity);
        }
        public override void Raycast(float dt, out BodyRaycastResult? bodyRaycastResult, out TerrainRaycastResult? terrainRaycastResult)
        {
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(Value)];
            Vector3 position = Position;
            Vector3 positionAtdt = position + (Velocity * dt);
            Vector3 v = block.ProjectileTipOffset * Vector3.Normalize(Velocity);
            if (TerrainCollidable)
                terrainRaycastResult = SubsystemTerrain.Raycast(position + v, positionAtdt + v, useInteractionBoxes: false, skipAirBlocks: true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable_(value));
            else
                terrainRaycastResult = null;
            if (BodyCollidable)
                bodyRaycastResult = SubsystemProjectiles.m_subsystemBodies.Raycast(position + v, positionAtdt + v, 0.2f, (ComponentBody body, float distance) =>
                {
                    if (BodiesToIgnore.Contains(body)) return false;
                    return true;
                });
            else
                bodyRaycastResult = null;
        }
        public override void Update(float dt)
        {

            if (gameInfo == null)
            {
                if (GameManager.Project != null) LoadSubsystemAndComponent(GameManager.Project);
                return;
            }
            if (EGlobal.WindowClosing) Debugger.Break();
            if (gameInfo.m_subsystemTime.GameTime < 3) return;
            double totalElapsedGameTime = SubsystemProjectiles.m_subsystemGameInfo.TotalElapsedGameTime;
            if (totalElapsedGameTime - CreationTime > (MaxTimeExist ?? 40f))
            {
                RemoveSelf();
                //ToRemove = true;
            }
            TerrainChunk chunkAtCell = SubsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Z));
            if (chunkAtCell == null || chunkAtCell.State <= TerrainChunkState.InvalidContents4)
            {
                NoChunk = true;
                if (TrailParticleSystem != null)
                {
                    TrailParticleSystem.IsStopped = true;
                }
                OnProjectileFlyOutOfLoadedChunks();
            }
            else
            {
                NoChunk = false;
                UpdateInChunk(dt);

                //Light
                UpdateTrajectoryState();
                UpdateInChunkLight(chunkAtCell);
            }
        }
        //public bool HasPositionUpdated()
        //{
        //    if ((int)Position.X == LastX && (int)Position.Y == LastY && (int)Position.Z == LastZ) return false;
        //    return true;
        //}
        public void UpdateTrajectoryState()
        {
            if (Trajectory.Count == 0)
            {
                TrajectoryUpdated = true;
                return;
            }
            var lastPos = Trajectory.Last();
            if (lastPos.Length < 3) return;
            if ((int)Position.X == (int)lastPos[0] && (int)Position.Y == (int)lastPos[1] && (int)Position.Z == (int)lastPos[2])
            {
                TrajectoryUpdated = false;
                return;
            }
            TrajectoryUpdated = true;

        }
        public void UpdateChunkState(TerrainChunk chunk, TerrainChunkState terrainChunkState)
        {
            int[][] cells = new int[9][] {
                    new int[]{1 , 0 },
                    new int[]{1 , 1 },//corner
                    new int[]{1 , -1 },//corner
                    new int[]{-1, 0 },
                    new int[]{-1, 1 },//corner
                    new int[]{-1, -1 },//corner
                    new int[]{0 , 0 },
                    new int[]{0 , 1 },
                    new int[]{0 , -1 },
                };
            foreach (int[] cell in cells)
            {
                TerrainChunk chunkTest = this.SubsystemTerrain.Terrain.GetChunkAtCoords(chunk.Coords.X + cell[0], chunk.Coords.Y + cell[1]);
                if (chunkTest == null) continue;
                chunkTest.ThreadState = terrainChunkState;
                chunkTest.State = terrainChunkState;
                if (subsystemSky == null) continue;
                this.SubsystemTerrain.TerrainUpdater.UpdateChunkSingleStep(chunk, subsystemSky.SkyLightValue);
            }
        }
        public void UpdateChunkState(int x, int y, int z, TerrainChunkState terrainChunkState)
        {
            TerrainChunk chunk = SubsystemTerrain.Terrain.GetChunkAtCell(x, y, z);
            if (chunk == null) return;
            UpdateChunkState(chunk, terrainChunkState);
        }
        public void UpdateInChunkLight(TerrainChunk chunk)
        {
            if (TrajectoryUpdated)
            {
                TrajectoryUpdated = false;
                int[] newpos = new int[3] {
                    (int)Position.X,
                    (int)Position.Y,
                    (int)Position.Z
                };
                if (Trajectory.Count < MaxTrajectoryStorage)
                {
                    Trajectory.Add(newpos);
                    LightChunk = chunk;
                    UpdateChunkState(chunk, TerrainChunkState.InvalidPropagatedLight);
                    this.SubsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
                }
                else
                {
                    var ForePos = Trajectory[0];
                    Trajectory.RemoveAt(0);
                    if (ForePos.Length < 3) return;
                    UpdateChunkState(ForePos[0], ForePos[1], ForePos[2], TerrainChunkState.InvalidLight);
                    this.SubsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
                    this.SubsystemTerrain.TerrainUpdater.m_pauseEvent.WaitOne(500);


                    Trajectory.Add(newpos);
                    LightChunk = chunk;
                    UpdateChunkState(chunk, TerrainChunkState.InvalidPropagatedLight);
                    this.SubsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
                }
                //UpdateChunkState(LastX, LastY, LastZ, TerrainChunkState.InvalidLight);
                //this.SubsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
                //this.SubsystemTerrain.TerrainUpdater.m_pauseEvent.WaitOne(500);


                //LightChunk = chunk;
                //UpdateChunkState(chunk, TerrainChunkState.InvalidPropagatedLight);
                //this.SubsystemTerrain.TerrainUpdater.UnpauseUpdateThread();

                //SubsystemTerrain.Draw(componentPlayer.GameWidget.ActiveCamera, 0);
                //if (gameWidgets == null) return;
                //foreach (var gameWidget in gameWidgets.GameWidgets)
                //{
                //    var context = Widget.m_drawContextsCache.FirstOrDefault();
                //    gameWidget.Draw(context ?? new Widget.DrawContext());
                //}
            }

        }
        public void RemoveSelf()
        {
            ToRemove = true;
            BeforeRemoved();
        }
        public void BeforeRemoved()
        {
            LightChunk = null;
            foreach (var t in Trajectory)
            {
                if (t.Length < 3) continue;
                UpdateChunkState(t[0], t[1], t[2], TerrainChunkState.InvalidLight);
                this.SubsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
                this.SubsystemTerrain.TerrainUpdater.m_pauseEvent.WaitOne(500);
            }
            Trajectory.Clear();
        }

        public override void Save(SubsystemProjectiles subsystemProjectiles, ValuesDictionary valuesDictionary)
        {
            //Debugger.Break();
            //return;
            base.Save(subsystemProjectiles, valuesDictionary);
        }




















        public override void OnProjectileFlyOutOfLoadedChunks()
        {
            ModsManager.HookAction("OnProjectileFlyOutOfLoadedChunks", loader =>
            {
                loader.OnProjectileFlyOutOfLoadedChunks(this);
                return false;
            });
        }
        public override bool ProcessOnHitAsProjectileBlockBehavior(CellFace? cellFace, ComponentBody componentBody, float dt)
        {
            bool flag = false;
            SubsystemBlockBehavior[] blockBehaviors = SubsystemProjectiles.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(Value));
            for (int i = 0; i < blockBehaviors.Length; i++)
            {
                flag |= blockBehaviors[i].OnHitAsProjectile(cellFace, componentBody, this);
            }
            return flag;
        }
        public override void HitBody(BodyRaycastResult bodyRaycastResult, ref Vector3 positionAtdt)
        {
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(Value)];
            float attackPower = (Velocity.Length() > 10f) ? AttackPower : 0;
            Vector3 velocityAfterAttack = Velocity * 0.05f + m_random.Vector3(0.33f * Velocity.Length());
            Vector3 angularVelocityAfterAttack = AngularVelocity * 0.05f;
            bool ignoreBody = false;
            Attackment attackment = new ProjectileAttackment(bodyRaycastResult.ComponentBody.Entity, OwnerEntity, bodyRaycastResult.HitPoint(), Vector3.Normalize(Velocity), attackPower, this);
            ModsManager.HookAction("OnProjectileHitBody", loader =>
            {
                loader.OnProjectileHitBody(this, bodyRaycastResult, ref attackment, ref velocityAfterAttack, ref angularVelocityAfterAttack, ref ignoreBody);
                return false;
            });
            if (ignoreBody)
            {
                BodiesToIgnore.Add(bodyRaycastResult.ComponentBody);
            }
            if (attackPower > 0f)
            {
                ComponentMiner.AttackBody(attackment);
                if (Owner != null && Owner.PlayerStats != null)
                {
                    Owner.PlayerStats.RangedHits++;
                }
            }
            if (IsIncendiary)
            {
                bodyRaycastResult.ComponentBody.Entity.FindComponent<ComponentOnFire>()?.SetOnFire(Owner, m_random.Float(6f, 8f));
            }
            if (!ignoreBody) positionAtdt = Position;
            Velocity = velocityAfterAttack;
            AngularVelocity = angularVelocityAfterAttack;
        }
        public override void HitTerrain(TerrainRaycastResult terrainRaycastResult, CellFace cellFace, ref Vector3 positionAtdt, ref Vector3? pickableStuckMatrix)
        {
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(Value)];
            int cellValue = SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
            Block blockHitted = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
            float velocityLength = Velocity.Length();
            Vector3 velocityAfterHit = Velocity;
            Vector3 angularVelocityAfterHit = AngularVelocity * -0.3f;
            Plane plane = cellFace.CalculatePlane();
            if (plane.Normal.X != 0f)
            {
                velocityAfterHit *= new Vector3(-TerrainKnockBack, TerrainKnockBack, TerrainKnockBack);
            }
            if (plane.Normal.Y != 0f)
            {
                velocityAfterHit *= new Vector3(TerrainKnockBack, -TerrainKnockBack, TerrainKnockBack);
            }
            if (plane.Normal.Z != 0f)
            {
                velocityAfterHit *= new Vector3(TerrainKnockBack, TerrainKnockBack, -TerrainKnockBack);
            }
            float num3 = velocityAfterHit.Length();
            velocityAfterHit = num3 * Vector3.Normalize(velocityAfterHit + m_random.Vector3(num3 / 6f, num3 / 3f));
            bool triggerBlocksBehavior = true;
            bool destroyCell = (velocityLength > 10f && m_random.Float(0f, 1f) > blockHitted.GetProjectileResilience(cellValue));
            float impactSoundLoudness = (velocityLength > 5f) ? 1f : 0f;
            bool projectileGetStuck = block.IsStickable_(Value) && velocityLength > 10f && m_random.Bool(blockHitted.GetProjectileStickProbability(Value));
            //ModsManager.HookAction("OnProjectileHitTerrain", loader =>
            //{
            //    loader.OnProjectileHitTerrain(this, terrainRaycastResult, ref triggerBlocksBehavior, ref destroyCell, ref impactSoundLoudness, ref projectileGetStuck, ref velocityAfterHit, ref angularVelocityAfterHit);
            //    return false;
            //});
            //以上为ModLoader接口和ref变量
            if (triggerBlocksBehavior)
            {
                SubsystemBlockBehavior[] blockBehaviors2 = SubsystemProjectiles.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(cellValue));
                for (int j = 0; j < blockBehaviors2.Length; j++)
                {
                    blockBehaviors2[j].OnHitByProjectile(cellFace, this);
                }
            }
            if (destroyCell)
            {
                SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, noDrop: true, noParticleSystem: false);
                SubsystemProjectiles.m_subsystemSoundMaterials.PlayImpactSound(cellValue, Position, 1f);
            }
            if (IsIncendiary)
            {
                SubsystemProjectiles.m_subsystemFireBlockBehavior.SetCellOnFire(cellFace.X, cellFace.Y, cellFace.Z, 1f);
                Vector3 vector3 = Position - (0.75f * Vector3.Normalize(Velocity));
                for (int k = 0; k < 8; k++)
                {
                    Vector3 v2 = (k == 0) ? Vector3.Normalize(Velocity) : m_random.Vector3(1.5f);
                    TerrainRaycastResult? terrainRaycastResult2 = SubsystemTerrain.Raycast(vector3, vector3 + v2, useInteractionBoxes: false, skipAirBlocks: true, (int value, float distance) => true);
                    if (terrainRaycastResult2.HasValue)
                    {
                        SubsystemProjectiles.m_subsystemFireBlockBehavior.SetCellOnFire(terrainRaycastResult2.Value.CellFace.X, terrainRaycastResult2.Value.CellFace.Y, terrainRaycastResult2.Value.CellFace.Z, 1f);
                    }
                }
            }
            if (impactSoundLoudness > 0)
            {
                //SubsystemProjectiles.m_subsystemSoundMaterials.PlayImpactSound(cellValue, Position, impactSoundLoudness);
                m_subsystemAudio.PlaySound("Audio/flare_bounce1", m_random.Float(1.8f, 2.4f), m_random.Float(-0.2f, 0.2f), Position, 2f, autoDelay: true);

            }
            if (projectileGetStuck)
            {
                var v3 = Vector3.Normalize(Velocity);
                float s = MathUtils.Lerp(0.1f, 0.2f, MathUtils.Saturate((velocityLength - 15f) / 20f));
                pickableStuckMatrix = Position + (terrainRaycastResult.Distance * Vector3.Normalize(Velocity)) + (v3 * s);
            }
            else
            {
                positionAtdt = Position;
                AngularVelocity = angularVelocityAfterHit;
                Velocity = velocityAfterHit;
            }
            MakeNoise();
        }

        public override void TurnIntoPickable(Vector3? pickableStuckMatrix)
        {
            //return;
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(Value)];
            int damagedBlockValue = BlocksManager.DamageItem(Value, DamageToPickable, OwnerEntity);
            if (TurnIntoPickableBlockValue.HasValue) damagedBlockValue = TurnIntoPickableBlockValue.Value;
            if (damagedBlockValue != 0)
            {
                Pickable pickable = null;
                if (pickableStuckMatrix.HasValue)
                {
                    SubsystemProjectiles.CalculateVelocityAlignMatrix(block, pickableStuckMatrix.Value, Velocity, out Matrix matrix);
                    pickable = m_subsystemPickables.CreatePickable(damagedBlockValue, 1, Position, Vector3.Zero, matrix, OwnerEntity);
                }
                else
                {
                    pickable = m_subsystemPickables.CreatePickable(damagedBlockValue, 1, Position, Vector3.Zero, null, OwnerEntity);
                }
                ModsManager.HookAction("OnProjectileTurnIntoPickable", loader =>
                {
                    loader.OnProjectileTurnIntoPickable(this, ref pickable);
                    return false;
                });
                if (pickable != null) m_subsystemPickables.AddPickable(pickable);
            }
            else
            {
                m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(SubsystemTerrain, Position, Value, 1f));
            }
            RemoveSelf();
            //ToRemove = true;
        }
        public override void UpdateInChunk(float dt)
        {
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(Value)];
            Vector3 position = Position;
            if (Position.Y < 0)
            {
                RemoveSelf();
                return;
            }
            Vector3 positionAtdt = position + (Velocity * dt);
            Vector3? pickableStuckMatrix = null;
            Raycast(dt, out BodyRaycastResult? bodyRaycastResult, out TerrainRaycastResult? terrainRaycastResult);
            CellFace? nullableCellFace = terrainRaycastResult.HasValue ? new CellFace?(terrainRaycastResult.Value.CellFace) : null;
            ComponentBody componentBody = bodyRaycastResult.HasValue ? bodyRaycastResult.Value.ComponentBody : null;
            //这里增加：忽略哪些Body、是否忽略地形
            bool disintegrate = block.DisintegratesOnHit;
            //执行各方块的OnHitAsProjectile。
            //if (terrainRaycastResult.HasValue || bodyRaycastResult.HasValue)
            //{
            //    disintegrate |= ProcessOnHitAsProjectileBlockBehavior(nullableCellFace, componentBody, dt);
            //    ToRemove |= disintegrate;
            //}
            //如果弹射物命中了Body，进行攻击，并改变速度。
            if (bodyRaycastResult.HasValue && (!terrainRaycastResult.HasValue || bodyRaycastResult.Value.Distance < terrainRaycastResult.Value.Distance))
            {
                HitBody(bodyRaycastResult.Value, ref positionAtdt);
            }
            //如果弹射物命中了地形，进行处理。破坏方块、点燃方块、撞到地形的移动效果。
            else if (terrainRaycastResult.HasValue)
            {
                CellFace cellFace = nullableCellFace.Value;
                HitTerrain(terrainRaycastResult.Value, cellFace, ref positionAtdt, ref pickableStuckMatrix);
            }
            //弹射物转化为掉落物
            //if (terrainRaycastResult.HasValue || bodyRaycastResult.HasValue)
            //{
            //    if (disintegrate)
            //    {
            //        m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(SubsystemTerrain, Position, Value, 1f));
            //    }
            //    else if (!ToRemove && (pickableStuckMatrix.HasValue || Velocity.Length() < 1f))
            //    {
            //        //TurnIntoPickable(pickableStuckMatrix);
            //        if (ProjectileStoppedAction == ProjectileStoppedAction.TurnIntoPickable)
            //        {
            //            TurnIntoPickable(pickableStuckMatrix);
            //        }
            //        else if (ProjectileStoppedAction == ProjectileStoppedAction.Disappear)
            //        {
            //            RemoveSelf();
            //            //ToRemove = true;
            //        }
            //    }
            //}
            UpdateMovement(dt, ref positionAtdt);
        }
        public override void UpdateMovement(float dt, ref Vector3 positionAtdt)
        {
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(Value)];
            if (Damping < 0f)
            {
                Damping = block.GetProjectileDamping(Value);
            }
            float friction = IsInFluid ? MathF.Pow(DampingInFluid, dt) : MathF.Pow(Damping, dt);
            int cellContents = SubsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Y), Terrain.ToCell(Position.Z));
            Block blockTheProjectileIn = BlocksManager.Blocks[cellContents];
            bool isProjectileInFluid = (blockTheProjectileIn is FluidBlock);
            Velocity.Y += -Gravity * dt;
            Velocity *= friction;
            AngularVelocity *= friction;
            Position = positionAtdt;
            Rotation += AngularVelocity * dt;
            if (TrailParticleSystem != null)
            {
                UpdateTrailParticleSystem(dt);
            }
            //if (isProjectileInFluid && !IsInFluid)
            //{
            //    if (DampingInFluid <= 0.001f)
            //    {
            //        float horizontalSpeed = new Vector2(Velocity.X + Velocity.Z).Length();
            //        if (horizontalSpeed > 6f && horizontalSpeed > 4f * MathF.Abs(Velocity.Y))
            //        {
            //            Velocity *= 0.5f;
            //            Velocity.Y *= -1f;
            //            isProjectileInFluid = false;
            //        }
            //        else
            //        {
            //            Velocity *= 0.2f;
            //        }
            //    }

            //    float? surfaceHeight = SubsystemProjectiles.m_subsystemFluidBlockBehavior.GetSurfaceHeight(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Y), Terrain.ToCell(Position.Z));
            //    if (surfaceHeight.HasValue)
            //    {
            //        if (blockTheProjectileIn is MagmaBlock)
            //        {
            //            m_subsystemParticles.AddParticleSystem(new MagmaSplashParticleSystem(SubsystemTerrain, Position, large: false));
            //            m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, m_random.Float(-0.2f, 0.2f), Position, 3f, autoDelay: true);
            //            if (!IsFireProof)
            //            {
            //                ToRemove = true;
            //                SubsystemProjectiles.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Y), Terrain.ToCell(Position.Z), Value);
            //            }
            //        }
            //        else
            //        {
            //            m_subsystemParticles.AddParticleSystem(new WaterSplashParticleSystem(SubsystemTerrain, new Vector3(Position.X, surfaceHeight.Value, Position.Z), large: false));
            //            m_subsystemAudio.PlayRandomSound("Audio/Splashes", 1f, m_random.Float(-0.2f, 0.2f), Position, 6f, autoDelay: true);
            //        }
            //        MakeNoise();
            //    }
            //}
            IsInFluid = isProjectileInFluid;
            //if (!IsFireProof && SubsystemProjectiles.m_subsystemTime.PeriodicGameTimeEvent(1.0, GetHashCode() % 100 / 100.0) && (SubsystemProjectiles.m_subsystemFireBlockBehavior.IsCellOnFire(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Y + 0.1f), Terrain.ToCell(Position.Z)) || SubsystemProjectiles.m_subsystemFireBlockBehavior.IsCellOnFire(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Y + 0.1f) - 1, Terrain.ToCell(Position.Z))))
            //{
            //    m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, m_random.Float(-0.2f, 0.2f), Position, 3f, autoDelay: true);
            //    ToRemove = true;
            //    SubsystemProjectiles.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Y), Terrain.ToCell(Position.Z), Value);
            //}
        }
        public override void UpdateTrailParticleSystem(float dt)
        {
            if (!m_subsystemParticles.ContainsParticleSystem((ParticleSystemBase)TrailParticleSystem))
            {
                m_subsystemParticles.AddParticleSystem((ParticleSystemBase)TrailParticleSystem);
            }
            Vector3 v4 = (TrailOffset != Vector3.Zero) ? Vector3.TransformNormal(TrailOffset, Matrix.CreateFromAxisAngle(Vector3.Normalize(Rotation), Rotation.Length())) : Vector3.Zero;
            TrailParticleSystem.Position = Position + v4;
            if (IsInFluid && StopTrailParticleInFluid)
            {
                TrailParticleSystem.IsStopped = true;
            }
        }
        public override void MakeNoise()
        {
            if (SubsystemProjectiles.m_subsystemTime.GameTime - LastNoiseTime > 0.5)
            {
                SubsystemProjectiles.m_subsystemNoise.MakeNoise(Position, 0.25f, 6f);
                LastNoiseTime = SubsystemProjectiles.m_subsystemTime.GameTime;
            }
        }
        public override void UnderExplosion(Vector3 impulse, float damage)
        {
            Velocity += (impulse + new Vector3(0f, 0.1f * impulse.Length(), 0f)) * m_random.Float(0.75f, 1f);
        }

        public override void Draw(Camera camera, int drawOrder)
        {
            float num = MathUtils.Sqr(SubsystemProjectiles.m_subsystemSky.VisibilityRange);
            Vector3 position = Position;
            if (!NoChunk && Vector3.DistanceSquared(camera.ViewPosition, position) < num && camera.ViewFrustum.Intersection(position))
            {
                int x = Terrain.ToCell(position.X);
                int num2 = Terrain.ToCell(position.Y);
                int z = Terrain.ToCell(position.Z);
                int num3 = Terrain.ExtractContents(Value);
                Block block = BlocksManager.Blocks[num3];
                TerrainChunk chunkAtCell = SubsystemProjectiles.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
                if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1 && num2 >= 0 && num2 < 255)
                {
                    SubsystemProjectiles.m_drawBlockEnvironmentData.Humidity = SubsystemProjectiles.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
                    SubsystemProjectiles.m_drawBlockEnvironmentData.Temperature = SubsystemProjectiles.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num2);
                    Light = SubsystemProjectiles.m_subsystemTerrain.Terrain.GetCellLightFast(x, num2, z);
                }
                SubsystemProjectiles.m_drawBlockEnvironmentData.Light = Light;
                SubsystemProjectiles.m_drawBlockEnvironmentData.BillboardDirection = block.GetAlignToVelocity(Value) ? null : new Vector3?(camera.ViewDirection);
                SubsystemProjectiles.m_drawBlockEnvironmentData.InWorldMatrix.Translation = position;
                Matrix matrix;
                if (block.GetAlignToVelocity(Value))
                {
                    SubsystemProjectiles.CalculateVelocityAlignMatrix(block, position, Velocity, out matrix);
                }
                else if (Rotation != Vector3.Zero)
                {
                    matrix = Matrix.CreateFromAxisAngle(Vector3.Normalize(Rotation), Rotation.Length());
                    matrix.Translation = Position;
                }
                else
                {
                    matrix = Matrix.CreateTranslation(Position);
                }
                bool shouldDrawBlock = true;
                float drawBlockSize = 0.3f;
                Color drawBlockColor = Color.MultiplyNotSaturated(Color.White, 1f - SubsystemProjectiles.m_subsystemSky.CalculateFog(camera.ViewPosition, Position));
                ModsManager.HookAction("OnProjectileDraw", loader =>
                {
                    loader.OnProjectileDraw(this, SubsystemProjectiles, camera, drawOrder, ref shouldDrawBlock, ref drawBlockSize, ref drawBlockColor);
                    return false;
                });
                if (shouldDrawBlock)
                    //block.DrawBlock(SubsystemProjectiles.m_primitivesRenderer, Value, drawBlockColor, drawBlockSize, ref matrix, SubsystemProjectiles.m_drawBlockEnvironmentData);
                    (block as FlareBlock).DrawFlareBlock(SubsystemProjectiles.m_primitivesRenderer, Value, drawBlockColor, drawBlockSize, ref matrix, SubsystemProjectiles.m_drawBlockEnvironmentData, RandomTextureIndex);
            }
        }
    }
}
