using DebugMod;
using Engine;
using Game;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace GlassMod
{
    public class TorcherinoComponent : Component, IUpdateable
    {
        public int x;
        public int y;
        public int z;
        public int speed = 2;
        public TorcherinoComponent(Entity Torcherino, int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.m_entity = Torcherino;
        }
        public UpdateOrder UpdateOrder => UpdateOrder.Default;
        public void Update(float dt)
        {
            //ScreenLog.Info("TorcherinoComponent.Update");
            //return;
            foreach (var entity in this.Project.Entities)
            {
                //ComponentFurnace? furnace = (ComponentFurnace)entity.Components.Where(x => x is ComponentFurnace).FirstOrDefault();
                ComponentBlockEntity? blockEntity = (ComponentBlockEntity)entity.Components.Where(x => x is ComponentBlockEntity).FirstOrDefault();
                if (blockEntity == null) continue;
                IUpdateable updateable = (IUpdateable)entity.Components.Where(x => x is not ComponentBlockEntity && x is IUpdateable).FirstOrDefault();
                if (updateable == null) continue;
                if (!WithinArea(x, y, z, blockEntity.Coordinates)) continue;
                //blockEntity.Coordinates;
#if GDEBUG
                ScreenLog.Info($"TorcherinoComponent.Update.F {x}, {y}, {z}");
#endif

                updateable.Update(dt * speed);
                if (updateable is ComponentFurnace) (updateable as ComponentFurnace).m_fuelEndTime -= dt * speed;
            }
            UpdatePlant(x, y, z);
        }
        public void UpdatePlant(int x, int y, int z)
        {
            var subsystemTerrain = Entity.Project.FindSubsystem<SubsystemTerrain>();
            if (subsystemTerrain == null) return;
            var terrain = subsystemTerrain.Terrain;
            var subsystemBlocksScanner = Entity.Project.FindSubsystem<SubsystemBlocksScanner>();
            if (subsystemBlocksScanner == null) return;
            //int cellValueFast = terrainChunk.GetCellValueFast(num);
            for (int tx = -1; tx <= 1; tx++)
            {
                for (int ty = -1; ty <= 1; ty++)
                {
                    for (int tz = -1; tz <= 1; tz++)
                    {
                        int cx = x + tx;
                        int cy = y + ty;
                        int cz = z + tz;
                        int cellValueFast = terrain.GetCellValueFast(cx, cy, cz);
                        int num3 = Terrain.ExtractContents(cellValueFast);
                        if (num3 != 0)
                        {
                            SubsystemPollableBlockBehavior[] array = subsystemBlocksScanner.m_pollableBehaviorsByContents[num3];
                            for (int i = 0; i < array.Length; i++)
                            {
                                try
                                {
                                    array[i].OnPoll(cellValueFast, cx, cy, cz, 0);
                                    //array[i].OnPoll(cellValueFast, cx, cy, cz, m_pollPass);
                                }
                                catch (Exception e)
                                {
                                    Log.Error(array[i].ToString() + " Poll " + BlocksManager.Blocks[num3].GetType().Name + " " + cellValueFast + " at " + string.Format("({0},{1},{2}) ", x, y, z) + "\n" + e);
                                }
                            }
                        }
                    }
                }
            }

        }
        public static bool WithinArea(int x, int y, int z, Point3 point)
        {
            return WithinArea(x, y, z, point.X, point.Y, point.Z);
        }
        public static bool WithinArea(int x, int y, int z, int px, int py, int pz)
        {
            if (px >= x - 1 && py >= y - 1 && pz >= z - 1 &&
                px <= x + 1 && py <= y + 1 && pz <= z + 1
                ) return true;
            return false;
        }
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            return;
            //base.Save(valuesDictionary, entityToIdMap);
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
        }
    }
}
