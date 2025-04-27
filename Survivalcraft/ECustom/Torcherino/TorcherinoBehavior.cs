using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using System.Diagnostics;
using TemplatesDatabase;
using GlassMod;
using AccessLib;
using DebugMod;
using Acornima;

namespace Game
{
    public class SubsystemTorcherinoBlockBehavior : SubsystemBlockBehavior
    {
        public SubsystemParticles m_subsystemParticles;

        public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = [];
        public Entity TorcherinoOnlyEntity;

        //public override int[] HandledBlocks => new int[3]
        //{
        //    31,
        //    17,
        //    132
        //};
        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            int cellValueFast = SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
            switch (Terrain.ExtractContents(cellValueFast))
            {
                case 905:
                    {
                        Point3 point = CellFace.FaceToPoint3(ExtractFaceData(Terrain.ExtractData(cellValueFast)));
                        int x2 = x - point.X;
                        int y2 = y - point.Y;
                        int z2 = z - point.Z;
                        int cellContents2 = SubsystemTerrain.Terrain.GetCellContents(x2, y2, z2);
                        if (!BlocksManager.Blocks[cellContents2].IsCollidable_(cellValueFast))
                        {
                            SubsystemTerrain.DestroyCell(0, x, y, z, 0, noDrop: false, noParticleSystem: false);
                        }
                        break;
                    }
                    //case 132:
                    //    {
                    //        int cellContents = SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
                    //        if (!BlocksManager.Blocks[cellContents].IsCollidable_(cellValueFast))
                    //        {
                    //            SubsystemTerrain.DestroyCell(0, x, y, z, 0, noDrop: false, noParticleSystem: false);
                    //        }
                    //        break;
                    //    }
            }
        }

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            //Debugger.Break();
            AddTorch(value, x, y, z);
            TryAddTorcherino(x, y, z, value);
        }

        public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
        {
            //Debugger.Break();
            RemoveTorch(x, y, z);
            TryRemoveTorcherino(x, y, z);
        }

        public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
        {
            //Debugger.Break();
            RemoveTorch(x, y, z);
            TryRemoveTorcherino(x, y, z);
            AddTorch(value, x, y, z);
            TryAddTorcherino(x, y, z, value);

        }

        public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
        {
            //Debugger.Break();
            AddTorch(value, x, y, z);
            //var objType = new DatabaseObjectType("TorcherinoObjType", "", "", 1, true, false, 100, true);
            //objType.InitializeRelations(new List<DatabaseObjectType>(), new List<DatabaseObjectType>(), objType);
            //var newList = new List<DatabaseObjectType>();
            //ModAccessUtil.SetFieldOnceSlow<DatabaseObjectType, List<DatabaseObjectType>>(objType, "m_allowedNestingParents", newList);
            //Project.FindEntity()
            TryAddTorcherino(x, y, z, value);

            //区块卸载
            //TerrainChunk chunkAtCell = SubsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(Position.X), Terrain.ToCell(Position.Z));
            //if (chunkAtCell == null || chunkAtCell.State <= TerrainChunkState.InvalidContents4)
            //{
            //    NoChunk = true;
            //    if (TrailParticleSystem != null)
            //    {
            //        TrailParticleSystem.IsStopped = true;
            //    }
            //    OnProjectileFlyOutOfLoadedChunks();
            //}

        }
        public void DisplayMessage(int speed = 3)
        {
            foreach (var entity in Project.Entities)
            {
                var player = (ComponentPlayer)entity.Components.Where(x => x is ComponentPlayer).FirstOrDefault();
                if (player == null) continue;
                player.ComponentGui.DisplaySmallMessage($"GlassMod.Torcherino: Area: 3x3x3, Speed: {speed}x", Color.Cyan, blinking: false, playNotificationSound: true);
            }
        }
        public void TryAddTorcherino(int x, int y, int z, int value)
        {
#if GDEBUG
            ScreenLog.Info($"Torch Generated {x}, {y}, {z}");
#endif
            //DisplayMessage();
            TryAddTorcherinoEntity();
            AddTorcherinoComponent(x, y, z, value);
        }
        public TorcherinoComponent? GetComponentByPos(int x, int y, int z)
        {
            if (TorcherinoOnlyEntity == null) return null;
            foreach (TorcherinoComponent component in TorcherinoOnlyEntity.Components)
            {
                if (component.x == x && component.y == y && component.z == z)
                {
                    return component;
                }
            }
            return null;
        }
        public void TryRemoveTorcherino(int x, int y, int z)
        {
            //ScreenLog.Info($"Torch Removed {x}, {y}, {z}");

            if (TorcherinoOnlyEntity == null) return;

            TorcherinoComponent Removed = null;
            foreach (TorcherinoComponent component in TorcherinoOnlyEntity.Components)
            {
                if (component.x == x && component.y == y && component.z == z)
                {
                    TorcherinoOnlyEntity.Components.Remove(component);
                    Removed = component;
                    break;
                }
            }
            SubsystemUpdate? update = (SubsystemUpdate)Project.Subsystems.Where(x => x is SubsystemUpdate).FirstOrDefault();
            if (update == null) throw new Exception("SubsystemUpdate Cannot be Null!");
            update.RemoveUpdateable(Removed);
        }
        public void TryAddTorcherinoEntity()
        {
            if (TorcherinoOnlyEntity == null)
            {
                var valuesDictionary = new ValuesDictionary();
                var objType = this.Project.m_gameDatabase.EntityTemplateType;
                valuesDictionary.DatabaseObject = new DatabaseObject(objType, "TorcherinoObj");
                Entity entity = Project.CreateEntity(valuesDictionary);
                Project.AddEntity(entity);
                TorcherinoOnlyEntity = entity;
                Project.FireEntityAddedEvents(TorcherinoOnlyEntity);
            }
        }
        public void AddTorcherinoComponent(int x, int y, int z, int value)
        {
            int strength = ExtractStrengthMode(Terrain.ExtractData(value));
            //ScreenLog.Info("Strength: " + strength);
            if (TorcherinoOnlyEntity == null) return;

            foreach (TorcherinoComponent component in TorcherinoOnlyEntity.Components)
            {
                if (component.x == x && component.y == y && component.z == z) return;
            }
            var comp = new TorcherinoComponent(TorcherinoOnlyEntity, x, y, z);
            comp.speed = strength + 2;
            TorcherinoOnlyEntity.Components.Add(comp);
            //Project.FireEntityAddedEvents(TorcherinoOnlyEntity);
            SubsystemUpdate? update = (SubsystemUpdate)Project.Subsystems.Where(x => x is SubsystemUpdate).FirstOrDefault();
            if (update == null) throw new Exception("SubsystemUpdate Cannot be Null!");
            update.AddUpdateable(comp);
        }

        public override void OnChunkDiscarding(TerrainChunk chunk)
        {
            var list = new List<Point3>();
            foreach (Point3 key in m_particleSystemsByCell.Keys)
            {
                if (key.X >= chunk.Origin.X && key.X < chunk.Origin.X + 16 && key.Z >= chunk.Origin.Y && key.Z < chunk.Origin.Y + 16)
                {
                    list.Add(key);
                }
            }
            foreach (Point3 item in list)
            {
                RemoveTorch(item.X, item.Y, item.Z);
            }
        }

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(throwOnError: true);
        }
        public static int ExtractFaceData(int data)
        {
            return data & 0b111;
        }
        public static int ExtractStrengthMode(int data)
        {
            int lpos = 3;
            return data >> lpos & 0b11;
        }
        public static int ReplaceStrengthMode(int data, int strength)
        {
            int lpos = 3;
            int mask = 0b11 << lpos;
            return (data & ~mask) | strength << lpos;
        }
        public enum StrengthMode
        {
            Double,
            Triple,
            FourTimes,
            FiveTimes
        }
        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            //Debugger.Break();

            CellFace cellFace = raycastResult.CellFace;
            int x = cellFace.X;
            int y = cellFace.Y;
            int z = cellFace.Z;
            int value = SubsystemTerrain.Terrain.GetCellValue(x, y, z);
            int index = Terrain.ExtractContents(value);
            int data = Terrain.ExtractData(value);

            int strength = ExtractStrengthMode(data);
            if (++strength > 3) strength = 0;
            int newdata = ReplaceStrengthMode(data, strength);
            int newvalue = Terrain.ReplaceData(value, newdata);
            SubsystemTerrain.ChangeCell(x, y, z, newvalue);

            return true;
        }
        public void SwitchStrength()
        {

        }
        public void AddTorch(int value, int x, int y, int z)
        {
            Vector3 v;
            float size;
            int data = Terrain.ExtractData(value);
            int strength = ExtractStrengthMode(data);
            switch (strength)
            {
                case 0:
                    DisplayMessage(2);
                    break;
                case 1:
                    DisplayMessage(3);
                    break;
                case 2:
                    DisplayMessage(4);
                    break;
                case 3:
                    DisplayMessage(5);
                    break;
            }
            switch (Terrain.ExtractContents(value))
            {
                case 905:
                    switch (ExtractFaceData(data))
                    {
                        case 0:
                            v = new Vector3(0.5f, 0.58f, 0.27f);
                            break;
                        case 1:
                            v = new Vector3(0.27f, 0.58f, 0.5f);
                            break;
                        case 2:
                            v = new Vector3(0.5f, 0.58f, 0.73f);
                            break;
                        case 3:
                            v = new Vector3(0.73f, 0.58f, 0.5f);
                            break;
                        default:
                            v = new Vector3(0.5f, 0.53f, 0.5f);
                            break;
                    }
                    size = 0.1f;
                    break;
                //case 132:
                //    v = new Vector3(0.5f, 0.1f, 0.5f);
                //    size = 0.1f;
                //    break;
                default:
                    v = new Vector3(0.5f, 0.2f, 0.5f);
                    size = 0.1f;
                    break;
            }
            var fireParticleSystem = new TorcherinoParticle(new Vector3(x, y, z) + v, size, 32f);
            m_subsystemParticles.AddParticleSystem(fireParticleSystem);
            m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
        }

        public void RemoveTorch(int x, int y, int z)
        {
            var key = new Point3(x, y, z);
            FireParticleSystem particleSystem = m_particleSystemsByCell[key];
            m_subsystemParticles.RemoveParticleSystem(particleSystem);
            m_particleSystemsByCell.Remove(key);
        }
    }
}
