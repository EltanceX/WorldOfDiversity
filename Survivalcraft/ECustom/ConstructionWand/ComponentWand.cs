using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugMod;
using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using TemplatesDatabase;

namespace GlassMod
{
    public class ComponentWand : Component, IUpdateable, IDrawable
    {
        public UpdateOrder UpdateOrder => UpdateOrder.Default;
        public int[] drawOrder = new int[] { 1024 };
        public int[] DrawOrders => drawOrder;
        public ComponentPlayer player;
        public ComponentInventory inventory;
        public ComponentCreativeInventory creativeInventory;
        public SubsystemGameInfo gameInfo;
        public ComponentMiner miner;
        public SubsystemTerrain subsystemTerrain;
        public PrimitivesRenderer3D primitivesRenderer3D = new PrimitivesRenderer3D();
        public CellFace cellFace = new CellFace();
        public float dts = 0;
        public int constructionWandBlockIndex = 0;
        public enum PlaneType
        {
            XZ,
            XY,
            YZ,
            Unknown
        }
        public List<Vector3> NearestBlocks;
        public static Point3 FaceToPosPoint(int face)
        {
            return face switch
            {
                0 => new Point3(0, 0, 1),//z+
                1 => new Point3(1, 0, 0),//x+
                2 => new Point3(0, 0, -1),//z-
                3 => new Point3(-1, 0, 0),//x-
                4 => new Point3(0, 1, 0),//y+
                5 => new Point3(0, -1, 0),//y-
                _ => new Point3(0),
            };
        }
        public static Vector3 FaceToPosVector(int face)
        {
            return face switch
            {
                0 => new Vector3(0, 0, 1),//z+
                1 => new Vector3(1, 0, 0),//x+
                2 => new Vector3(0, 0, -1),//z-
                3 => new Vector3(-1, 0, 0),//x-
                4 => new Vector3(0, 1, 0),//y+
                5 => new Vector3(0, -1, 0),//y-
                _ => new Vector3(0),
            };
        }
        public bool IsWandInHand()
        {
            if (gameInfo.WorldSettings.GameMode == GameMode.Creative)
            {
                if (creativeInventory.GetSlotValue(creativeInventory.ActiveSlotIndex) == constructionWandBlockIndex) return true;
                return false;
            }
            if (inventory.GetSlotValue(inventory.ActiveSlotIndex) == constructionWandBlockIndex) return true;
            return false;

        }
        public void Update(float dt)
        {
            if (constructionWandBlockIndex == 0 || inventory == null || creativeInventory == null || gameInfo == null) return;

            dts += dt;
            if (dts < 0.2f)
            {
                return;
            }
            dts = 0;
            if (!IsWandInHand())
            {
                if (NearestBlocks != null && NearestBlocks.Count > 0)
                {
                    NearestBlocks.Clear();
                    cellFace = new CellFace();
                }
                return;
            }

            if (player == null || miner == null || subsystemTerrain == null) return;
            var pos = player.ComponentCreatureModel.EyePosition;
            var dir = player.ComponentCreatureModel.EyeRotation.GetForwardVector();
            var ray3 = new Ray3(pos, dir);
            TerrainRaycastResult? res = miner.Raycast<TerrainRaycastResult>(ray3, RaycastMode.Digging);
            if (res.HasValue)
            {
                var resCellFace = res.Value.CellFace;
                if (resCellFace.X == cellFace.X && resCellFace.Y == cellFace.Y && resCellFace.Z == cellFace.Z && resCellFace.Face == cellFace.Face) return;
                UpdateTargetBlocks(res.Value);
#if GDEBUG
                ScreenLog.Info($"TerrainRaycastResult: {res.Value.CellFace}");
#endif
                cellFace = res.Value.CellFace;
            }
            else if (NearestBlocks != null && NearestBlocks.Count > 0) NearestBlocks.Clear();
        }
        public void UpdateTargetBlocks(TerrainRaycastResult rayResult)
        {
            var cellface = rayResult.CellFace;
            int content = subsystemTerrain.Terrain.GetCellContentsFast(cellface.X, cellface.Y, cellface.Z);
            if (content != 0)
            {

                NearestBlocks = FindNearestBlocks(cellface.X, cellface.Y, cellface.Z, content, cellface.Face);
            }
            //int value = subsystemTerrain.Terrain.GetCellValueFast(cellface.X, cellface.Y, cellface.Z);
            //subsystemTerrain.ChangeCell
            //var block = BlocksManager.Blocks[content];


        }
        public PlaneType FaceToPlaneType(int face)
        {
            return face switch
            {
                0 => PlaneType.XY,//z+
                1 => PlaneType.YZ,//x+
                2 => PlaneType.XY,//z-
                3 => PlaneType.YZ,//x-
                4 => PlaneType.XZ,//y+
                5 => PlaneType.XZ,//y-
                _ => PlaneType.Unknown,
            };
        }
        public bool IsTargetBlock(int x, int y, int z, int target)
        {
            int content = subsystemTerrain.Terrain.GetCellContentsFast(x, y, z);
            return content == target;
        }
        public bool MatchedCondition(int x, int y, int z, int target, Point3 facePoint)
        {
            if (y < 0) return false;
            return IsTargetBlock(x, y, z, target) && IsTargetBlock(x + facePoint.X, y + facePoint.Y, z + facePoint.Z, 0);
        }
        public List<Vector3> FindNearestBlocks(int centerX, int centerY, int centerZ, int target, int face)
        {
            PlaneType planeType = FaceToPlaneType(face);
            Point3 facePoint = FaceToPosPoint(face);

            int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };
            Queue<(int, int)> queue = new Queue<(int, int)>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            switch (planeType)
            {
                case PlaneType.XZ:
                    queue.Enqueue((centerX, centerZ));
                    visited.Add((centerX, centerZ));
                    break;
                case PlaneType.XY:
                    queue.Enqueue((centerX, centerY));
                    visited.Add((centerX, centerY));
                    break;
                case PlaneType.YZ:
                    queue.Enqueue((centerY, centerZ));
                    visited.Add((centerY, centerZ));
                    break;
            }


            List<Vector3> BlocksFound = new List<Vector3>();
            //BFS Search
            while (queue.Count > 0 && BlocksFound.Count < 32 && visited.Count < 256)
            {
                var (axis1, axis2) = queue.Dequeue();
                bool matched = false;
                switch (planeType)
                {
                    case PlaneType.XZ:
                        if (MatchedCondition(axis1, centerY, axis2, target, facePoint))
                        {
                            BlocksFound.Add(new Vector3(axis1, centerY, axis2));
                            matched = true;
                        }
                        break;
                    case PlaneType.XY:
                        if (MatchedCondition(axis1, axis2, centerZ, target, facePoint))
                        {
                            BlocksFound.Add(new Vector3(axis1, axis2, centerZ));
                            matched = true;
                        }
                        break;
                    case PlaneType.YZ:
                        if (MatchedCondition(centerX, axis1, axis2, target, facePoint))
                        {
                            BlocksFound.Add(new Vector3(centerX, axis1, axis2));
                            matched = true;
                        }
                        break;
                }
                if (!matched) continue;
                for (int i = 0; i < dx.Length; i++)
                {
                    int naxis1 = axis1 + dx[i];
                    int naxis2 = axis2 + dy[i];

                    if (!visited.Contains((naxis1, naxis2)))
                    {
                        visited.Add((naxis1, naxis2));
                        queue.Enqueue((naxis1, naxis2));
                    }
                }
            }
            return BlocksFound;


            //subsystemTerrain.ChangeCell()
            //BlocksManager
            //Terrain.MakeBlockValue()
            //subsystemTerrain.Terrain.SetCellValueFast
        }



        public void Use()
        {
            if (NearestBlocks == null || NearestBlocks.Count == 0) return;
            if (cellFace == null) return;
            var flatBatch3D = primitivesRenderer3D.FlatBatch();
            Vector3 facePos = FaceToPosVector(cellFace.Face);
            int cellFaceValue = subsystemTerrain.Terrain.GetCellValueFast(cellFace.X, cellFace.Y, cellFace.Z);
            if (cellFaceValue == 0) return;
            foreach (var pos in NearestBlocks)
            {
                Vector3 min = new Vector3(pos.X, pos.Y, pos.Z);
                min += facePos;
                subsystemTerrain.ChangeCell((int)min.X, (int)min.Y, (int)min.Z, cellFaceValue);
                //Terrain.MakeBlockValue
            }
            NearestBlocks.Clear();
        }

        public void Draw(Camera camera, int i)
        {
            if (NearestBlocks == null || NearestBlocks.Count == 0) return;
            if (cellFace == null) return;
            var flatBatch3D = primitivesRenderer3D.FlatBatch();
            Vector3 facePos = FaceToPosVector(cellFace.Face);
            foreach (var pos in NearestBlocks)
            {
                Vector3 min = new Vector3(pos.X, pos.Y, pos.Z);
                min += facePos;
                Vector3 max = min + new Vector3(1);
                flatBatch3D.QueueBoundingBox(new BoundingBox(min, max), Color.White);
            }

            primitivesRenderer3D.Flush(camera.ViewProjectionMatrix);
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            player = Entity.FindComponent<ComponentPlayer>();
            miner = Entity.FindComponent<ComponentMiner>();
            subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>();
            constructionWandBlockIndex = BlocksManager.GetBlockIndex<ConstructionWandBlock>();
            inventory = Entity.FindComponent<ComponentInventory>();
            creativeInventory = Entity.FindComponent<ComponentCreativeInventory>();
            gameInfo = Project.FindSubsystem<SubsystemGameInfo>();
        }
    }
}
