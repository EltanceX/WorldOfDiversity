using BluePrint;
using Game;
using System.Diagnostics;
using WebSocketSharp.Net;
using WFC;

namespace GlassMod
{
    public class ScStructure
    {
        public BluePrintData BpData;
        public BluePrintReader BpReader;
        public ScStructure()
        {

        }
        public ScStructure(string fileName)
        {
            string path = Path.Combine(BluePrintGameBridge.SBPDirectoryPath, fileName + ".sbp");
            Load(path);
        }
        public virtual void LoadByPath(string path)
        {
            Load(path);
        }
        public virtual void Load(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BluePrintData bluePrintData = new BluePrintData(fs);
            this.BpData = bluePrintData;
            BluePrintReader reader = new BluePrintReader(bluePrintData, useCache: true);
            this.BpReader = reader;
            reader.PrepareForReading();
            reader.ReadFileHeader();
            reader.ReadFormatHeader();
            reader.Cache.Position = 0;
        }
        public virtual void AutoRead()
        {

        }
        public virtual void Dispose()
        {
            BpReader?.Dispose();
            BpData?.Dispose();
            BpData = null;
            BpReader = null;
        }
    }
    public class WFCData
    {
        //public BluePrintReader BluePrintReader;
        public ScStructure structure;
        public string FileName;
        public List<TileDirection> TileDirections;
        public bool CompleteBlockCollapse = false;
        public WFCData() { }
    }
    public class WFCGameBridge
    {
        public static List<Tile> Tiles = new();
        public static bool BlueprintLoaded = false;
        public static List<WFCData> BlueprintFiles = new()
        {
            new WFCData(){FileName = "fortain_urdl", TileDirections= new List<TileDirection>(){ TileDirection.Up,TileDirection.Down,TileDirection.Right,TileDirection.Left } },
            new WFCData(){FileName = "restaurant_ud", TileDirections= new List<TileDirection>(){ TileDirection.Up,TileDirection.Down } },
            new WFCData(){FileName = "tower_l", TileDirections= new List<TileDirection>(){ TileDirection.Left } },
            new WFCData(){FileName = "villa_d", TileDirections= new List<TileDirection>(){ TileDirection.Down } },
            new WFCData(){FileName = "windmill_l", TileDirections= new List<TileDirection>(){ TileDirection.Left } },

            new WFCData(){FileName = "road_urdl", CompleteBlockCollapse = true, TileDirections= new List<TileDirection>(){ TileDirection.Up, TileDirection.Down, TileDirection.Right, TileDirection.Left } },
            new WFCData(){FileName = "road_urd" , CompleteBlockCollapse = true, TileDirections= new List<TileDirection>(){ TileDirection.Up, TileDirection.Right, TileDirection.Down } },
            new WFCData(){FileName = "road_rl"  , CompleteBlockCollapse = true, TileDirections= new List<TileDirection>(){ TileDirection.Right, TileDirection.Left } },
            new WFCData(){FileName = "road_ur"  , CompleteBlockCollapse = true, TileDirections= new List<TileDirection>(){ TileDirection.Up, TileDirection.Right } },
        };

        public static void LoadBlueprints()
        {
            if (BlueprintLoaded) return;
            BlueprintLoaded = true;

            foreach (var data in BlueprintFiles)
            {
                data.structure = new ScStructure(data.FileName);
            }
        }
        public static void LoadTiles()
        {
            Debugger.Break();
            LoadBlueprints();
            Tiles.Clear();

            int id = 0;
            int idcopy = 100;

            foreach (var data in BlueprintFiles)
            {
                Tile tile = new Tile(id++, data.FileName);
                tile.AvailableFace = data.TileDirections;
                //tile.Blueprint = data.structure.BpReader;
                tile.structure = data.structure;
                tile.CompleteBlockCollapse = data.CompleteBlockCollapse;
                tile.UpdateTransform();
                Tiles.Add(tile);
                switch (data.FileName)
                {
                    case "fortain_urdl":
                    case "road_urdl":
                        break;
                    case "restaurant_ud":
                    case "road_rl":
                    case "road_ur":
                        Tile t2 = tile.CopySelf();
                        t2.Rotate();
                        t2.Id = idcopy++;
                        t2.UpdateTransform();

                        Tiles.Add(t2);
                        break;
                    case "tower_l":
                    case "villa_d":
                    case "windmill_l":
                    case "road_urd":
                        Tile t3 = tile.CopySelf();
                        t3.Rotate();
                        t3.Id = idcopy++;
                        t3.UpdateTransform();


                        Tile t4 = t3.CopySelf();
                        t4.Rotate();
                        t4.Id = idcopy++;
                        t4.UpdateTransform();

                        Tile t5 = t4.CopySelf();
                        t5.Rotate();
                        t5.Id = idcopy++;
                        t5.UpdateTransform();

                        Tiles.Add(t3);
                        Tiles.Add(t4);
                        Tiles.Add(t5);
                        break;
                }
            }
        }
        public static (int[,] TargetChunkHeight, int[,] TargetChunkCollapse, int TopChunkHeight) UpdateHeight(SubsystemTerrain subsystemTerrain, Tile data, int width, int depth, int vx, int vz)
        {
            var TargetChunkHeight = new int[width, depth];
            var TopChunkHeight = 0;
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CheckAndUpdateChunk(subsystemTerrain, vx + x, vz + z);
                    int height = subsystemTerrain.Terrain.GetTopHeight(vx + x, vz + z);
                    if (height <= 0) height = subsystemTerrain.Terrain.CalculateTopmostCellHeight(vx + x, vz + z);
                    if (height < 0) throw new Exception("Height cannot be less than 0!");
                    if (height == 0)
                    {
                        TargetChunkHeight[x, z] = -1;
                        continue;
                    }
                    //if (height == 0)
                    //{
                    //var ck = subsystemTerrain.Terrain.GetChunkAtCell(vx + x, vz + z);
                    //var bl = ck.IsLoaded;
                    //}
                    height = CollapseAndRemoveBlock(subsystemTerrain, vx + x, height, vz + z);
                    if (height > TopChunkHeight) TopChunkHeight = height;
                    TargetChunkHeight[x, z] = height;
                }
            }


            var TargetChunkCollapse = new int[width, depth];
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (TargetChunkHeight[x, z] == -1)
                    {
                        TargetChunkCollapse[x, z] = -1;
                        continue;
                    }
                    if (data.CompleteBlockCollapse) TargetChunkCollapse[x, z] = TopChunkHeight - TargetChunkHeight[x, z];
                    else
                    {
                        FillBlank(subsystemTerrain, vx + x, TopChunkHeight - 1, vz + z);
                        TargetChunkCollapse[x, z] = 0;
                    }
                }
            }
            return (TargetChunkHeight, TargetChunkCollapse, TopChunkHeight);
        }
        public static bool CanCollpaseBlock(Block block, bool containsAir)
        {
            if (block is LeavesBlock) return true;
            else if (containsAir && block is AirBlock) return true;
            else if (block is WoodBlock) return true;
            else if (block is TallGrassBlock) return true;
            else if (block is CrossBlock) return true;
            else if (block is SnowBlock) return true;
            else if (block is IvyBlock) return true;
            else if (block is BasePumpkinBlock) return true;
            return false;
        }
        public static int CollapseAndRemoveBlock(SubsystemTerrain subsystemTerrain, int x, int top, int z, bool containsAir = true)
        {
            int y = top;
            int i = 0;
            while (++i <= 256 && y >= 0)
            {
                int id = subsystemTerrain.Terrain.GetCellContentsFast(x, y, z);
                Block block = BlocksManager.Blocks[id];
                if (CanCollpaseBlock(block, containsAir))
                {
                    subsystemTerrain.Terrain.SetCellValueFast(x, y, z, 0);
                    y--;
                    if (y == -1) Debugger.Break();
                }
                else break;
            }
            return y;
        }
        public static void FillBlank(SubsystemTerrain subsystemTerrain, int x, int top, int z)
        {
            DirtBlock dirt = BlocksManager.GetBlock<DirtBlock>();
            if (dirt == null) return;
            int dirtIndex = BlocksManager.GetBlockIndex<DirtBlock>();

            int y = top;
            int i = 0;
            while (++i <= 256 && y >= 0)
            {
                int id = subsystemTerrain.Terrain.GetCellContentsFast(x, y, z);
                Block block = BlocksManager.Blocks[id];
                if (block is AirBlock)
                {
                    subsystemTerrain.Terrain.SetCellValueFast(x, y, z, Terrain.MakeBlockValue(dirtIndex));
                    y--;
                }
                else break;
            }
        }

        //public static void LoadNotLoaded(SubsystemTerrain subsystemTerrain, TerrainChunk chunk)
        //{
        //    //double realTime19 = Time.RealTime;
        //    if (subsystemTerrain.TerrainSerializer.LoadChunk(chunk))
        //    {
        //        chunk.ThreadState = TerrainChunkState.InvalidLight;
        //        chunk.WasUpgraded = true;
        //        //double realTime20 = Time.RealTime;
        //        chunk.IsLoaded = true;
        //        //m_statistics.LoadingCount++;
        //        //m_statistics.LoadingTime += realTime20 - realTime19;
        //    }
        //    else
        //    {
        //        chunk.ThreadState = TerrainChunkState.InvalidContents1;
        //        chunk.WasUpgraded = true;
        //    }
        //}
        public static void UpdateChunkDisplay(SubsystemTerrain subsystemTerrain, BlockPos pos)
        {
            var terrain = subsystemTerrain.Terrain;
            var chunkStart = terrain.GetChunkAtCell(pos.x, pos.z);
            if (chunkStart == null) return;
            for (int x = 0; x < 6; x++)
            {
                for (int z = 0; z < 6; z++)
                {
                    var chunkat = terrain.GetChunkAtCoords(chunkStart.Coords.X + x, chunkStart.Coords.Y + z);
                    if (chunkat == null) continue;
                    chunkat.ModificationCounter = 1;
                    chunkat.State = TerrainChunkState.InvalidLight;
                }
            }

            subsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
            subsystemTerrain.TerrainUpdater.m_pauseEvent.WaitOne(500);
        }
        public static void CheckAndUpdateChunk(SubsystemTerrain subsystemTerrain, int x, int z)
        {
            var terrain = subsystemTerrain.Terrain;
            var chunk = terrain.GetChunkAtCell(x, z);
            if (chunk == null)
            {
                chunk = terrain.AllocateChunk(x >> 4, z >> 4);
            }
            //if (chunk.State == TerrainChunkState.NotLoaded)
            //{
            //LoadNotLoaded(subsystemTerrain, chunk);
            //}
            //if (!chunk.IsLoaded)
            //{
            //    var sky = subsystemTerrain.Project.Subsystems.Where(x => x is SubsystemSky).FirstOrDefault() as SubsystemSky;
            //    subsystemTerrain.TerrainUpdater.UpdateChunkSingleStep(chunk, sky == null ? 14 : sky.SkyLightValue);
            //    subsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
            //    subsystemTerrain.TerrainUpdater.m_pauseEvent.WaitOne(500);
            //}
            if (chunk == null) return;
            chunk.State = TerrainChunkState.InvalidLight;
            chunk.ModificationCounter = 1;
        }
        public static void Generate(ComponentPlayer player, BlockPos? pos = null)
        {
            Debugger.Break();
            if (!BluePrintGameBridge.Initialized) BluePrintGameBridge.Initialize();

            if (player == null) throw new Exception("ComponentPlayer cannot be null!");
            (ComponentBluePrint bluePrint, SubsystemTerrain subsystemTerrain) = BluePrintGameBridge.PrepareForImport(player, pos.HasValue).Value;
            BlockPos position = new();

            if (pos.HasValue) position = pos.Value;
            else position = new BlockPos(bluePrint.FirstPosition.Value);


            if (Tiles.Count == 0) LoadTiles();

            int AreaWidth = 5;
            int AreaDepth = 5;
            int unitSize = 20;

            var wfc = new WaveFunctionCollapse(Tiles.ToArray(), AreaWidth, AreaDepth, 233);
            wfc.Begin();

            var CraftingIdTableReverse = BluePrintManager.CraftingIdTableReverse;

            for (int z = 0; z < AreaDepth; z++)
            {
                for (int x = 0; x < AreaWidth; x++)
                {
                    int id = wfc.Result[x, z];
                    if (id == -1 || id == -1000)
                    {
                        continue;
                    }
                    var tile = wfc.GetTileById(id);
                    //var bp = tile.Blueprint;
                    var bp = tile.structure.BpReader;
                    var data = bp.Data;
                    bp.ReadFormatHeader();

#warning 去掉+1
                    int width = (int)data.Width + 1;
                    int depth = (int)data.Depth + 1;

                    int widthpos = width - 1;
                    int depthpos = depth - 1;

                    int UnitOffsetX = (int)(widthpos > unitSize ? -(widthpos - unitSize) / 2 : (unitSize - widthpos) / 2);
                    int UnitOffsetZ = (int)(depthpos > unitSize ? -(depthpos - unitSize) / 2 : (unitSize - depthpos) / 2);

                    int vx = position.x + x * unitSize + UnitOffsetX;//vertex x (start point)
                    int vz = position.z + z * unitSize + UnitOffsetZ;
                    try
                    {
                        //CheckAndUpdateChunk(subsystemTerrain, vx, vz);
                    }
                    catch (Exception ex) { continue; }

                    (int[,] ChunkHeight, int[,] ChunkCollapse, int TopChunkHeight) = UpdateHeight(subsystemTerrain, tile, width, depth, vx, vz);

                    bp.OnPlaceBlock_CraftingID = (string craftingId) => bp.OnPlaceBlockWithData_CraftingID(craftingId, 0);
                    bp.OnPlaceBlockWithData_CraftingID = (string craftingId, int blockData) =>
                    {
                        if (!CraftingIdTableReverse.Keys.Contains(craftingId)) return;
                        int blockId = CraftingIdTableReverse[craftingId];
                        int value = Terrain.MakeBlockValue(blockId, 0, blockData);
                        System.Numerics.Vector3 v = new(data.X, data.Y, data.Z);
                        System.Numerics.Vector3 vt = System.Numerics.Vector3.Transform(v, tile.Transform.Value);
                        vt.X = Math.Clamp(vt.X, 0, width - 1);
                        vt.Z = Math.Clamp(vt.Z, 0, depth - 1);

                        int setX = vx + (int)vt.X;
                        int setZ = vz + (int)vt.Z;
                        int collapesY = ChunkCollapse[(int)vt.X, (int)vt.Z];
                        if (collapesY == -1) return;
                        //var chunkAtCell = subsystemTerrain.Terrain.GetChunkAtCell(setX, setZ);
                        //if (chunkAtCell == null) CheckAndUpdateChunk(subsystemTerrain, setX, setZ);


                        subsystemTerrain.Terrain.SetCellValueFast(
                            setX,
                            //position.y + (int)vt.Y,
                            TopChunkHeight - collapesY + (int)vt.Y,
                            setZ,
                            value
                        );
                    };
                    bp.ReadToEnd();
                }
            }


            //subsystemTerrain.ForceSave(null);
            //UpdateChunkDisplay(subsystemTerrain, position);
            //if (chunk.State > TerrainChunkState.InvalidContents4 && chunk.ModificationCounter > 0)

        }
    }
}
