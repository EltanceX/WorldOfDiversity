using BluePrint;
using DebugMod;
using Engine;
using Game;
using System.Diagnostics;

namespace GlassMod
{
    public class BluePrintGameBridge
    {
        public static bool Initialized = false;
        public static string SBPDirectoryPath = "./ModData/BluePrints/";
        public static void Initialize()
        {
            if (Initialized) return;
            Initialized = true;

            var CraftingIdTable = BluePrintManager.CraftingIdTable;
            var CraftingIdTableReverse = BluePrintManager.CraftingIdTableReverse;
            CraftingIdTable.Clear();
            CraftingIdTableReverse.Clear();
            var blocks = BlocksManager.Blocks;
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blocks[i] is AirBlock) continue;
                if (blocks[i].CraftingId == string.Empty)
                {
                    //Debugger.Break();
                    blocks[i].CraftingId = blocks[i].GetType().Name;
                }
                var block = blocks[i];
                CraftingIdTable.TryAdd(i, block.CraftingId);
                CraftingIdTableReverse.TryAdd(block.CraftingId, i);
            }
        }
        public static Point3 GetFirstPos(Vector3 FirstPosition, Vector3 SecondPosition)
        {
            float x = Math.Min(FirstPosition.X, SecondPosition.X);
            float y = Math.Min(FirstPosition.Y, SecondPosition.Y);
            float z = Math.Min(FirstPosition.Z, SecondPosition.Z);
            return new Point3((int)x, (int)y, (int)z);
        }
        public static Point3 GetSize(Vector3 FirstPosition, Vector3 SecondPosition)
        {
            Vector3 rvec = SecondPosition - FirstPosition;
            float width = Math.Abs(rvec.X);
            float height = Math.Abs(rvec.Y);
            float depth = Math.Abs(rvec.Z);
            return new Point3((int)width, (int)height, (int)depth);
        }
        public static void Export(ComponentPlayer player, string fileName)
        {
            if (!Initialized) Initialize();

            var prepares = PrepareForExport(player, fileName);
            if (!prepares.HasValue) return;

            var bluePrint = prepares.Value.bluePrint;
            var subsystemTerrain = prepares.Value.subsystemTerrain;
            var terrain = subsystemTerrain.Terrain;

            var FirstPosition = bluePrint.FirstPosition.Value;
            var SecondPosition = bluePrint.SecondPosition.Value;

            Point3 StartPos = GetFirstPos(FirstPosition, SecondPosition);
            Point3 Size = GetSize(FirstPosition, SecondPosition);



            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            BluePrintData data = new BluePrintData(fs);
            BluePrintWriter writer = new BluePrintWriter(data);
            writer.PrepareForWriting((uint)Size.X + 1, (uint)Size.Y + 1, (uint)Size.Z + 1, CompressMode: 1);
            var CraftingIdTable = BluePrintManager.CraftingIdTable;
            try
            {
                foreach (var pos in BlockPosEx.IterateInOrder(new BlockPos(StartPos), new BlockPos(StartPos + Size)))
                {
                    int x = pos.x, y = pos.y, z = pos.z;
                    int blockID = terrain.GetCellContents(x, y, z);
                    int value = terrain.GetCellValue(x, y, z);
                    int blockData = Terrain.ExtractData(value);

                    if (blockID == 0) continue;
                    if (!CraftingIdTable.Keys.Contains(blockID)) continue;

                    string craftingId = CraftingIdTable[blockID];
                    writer.AutoCalcPosition(x - StartPos.X, y - StartPos.Y, z - StartPos.Z);
                    writer.AddBlockPattle(craftingId);
                    if (blockData == 0) writer.PlaceBlock(craftingId);
                    else writer.PlaceBlockWithData(craftingId, blockData);
                }
                writer.End();
            }
            catch (Exception ex)
            {
                writer.End();
                throw;
            }
        }
        public static (ComponentBluePrint bluePrint, SubsystemTerrain subsystemTerrain)? PrepareForExport(ComponentPlayer player, string fileName)
        {
            if (player == null) throw new Exception("Player Cannot be null!");
            var subsystemTerrain = player.Project.Subsystems.Where(x => x is SubsystemTerrain).FirstOrDefault() as SubsystemTerrain;
            if (subsystemTerrain == null) throw new Exception("SubsystemTerrain cannot be null!");
            var terrain = subsystemTerrain.Terrain;
            if (terrain == null) return null;

            var bluePrint = player.Entity.Components.Where(x => x is ComponentBluePrint).FirstOrDefault() as ComponentBluePrint;
            if (bluePrint == null) throw new Exception("Blue Print Key Component not found!");

            if (!(bluePrint.FirstPosition.HasValue && bluePrint.SecondPosition.HasValue))
            {
                //ScreenLog.Error("Please Select 2 Blocks First.");
                //player.ComponentGui.DisplaySmallMessage("Please Select 2 Blocks First.", Color.Olive, false, true);
                //return null;
                throw new Exception("Please Select 2 Blocks First.");
            }
            return (bluePrint, subsystemTerrain);
        }







        public static void Import(ComponentPlayer player, string fileName)
        {
            if (!Initialized) Initialize();

            var prepares = PrepareForImport(player);
            if (!prepares.HasValue) return;

            var bluePrint = prepares.Value.bluePrint;
            var subsystemTerrain = prepares.Value.subsystemTerrain;
            var terrain = subsystemTerrain.Terrain;

            var FirstPosition = bluePrint.FirstPosition.Value;
            BlockPos position = new BlockPos(FirstPosition);
            var CraftingIdTableReverse = BluePrintManager.CraftingIdTableReverse;



            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BluePrintData data = new BluePrintData(fs);
            BluePrintReader reader = new BluePrintReader(data);
            reader.PrepareForReading();
            reader.ReadFileHeader();
            reader.ReadFormatHeader();

            reader.OnPlaceBlockWithData_CraftingID = (string craftingId, int blockData) =>
            {
                if (!CraftingIdTableReverse.Keys.Contains(craftingId)) return;
                int blockId = CraftingIdTableReverse[craftingId];
                int value = Terrain.MakeBlockValue(blockId, 0, blockData);
                subsystemTerrain.ChangeCell(position.x + data.X, position.y + data.Y, position.z + data.Z, value);
            };
            reader.OnPlaceBlock_CraftingID = (string craftingId) =>
            {
                if (!CraftingIdTableReverse.Keys.Contains(craftingId)) return;
                int blockId = CraftingIdTableReverse[craftingId];
                subsystemTerrain.ChangeCell(position.x + data.X, position.y + data.Y, position.z + data.Z, Terrain.MakeBlockValue(blockId));
            };

            reader.ReadToEnd();
        }
        public static (ComponentBluePrint bluePrint, SubsystemTerrain subsystemTerrain)? PrepareForImport(ComponentPlayer player, bool byPosition = false)
        {
            if (player == null) throw new Exception("Player Cannot be null!");
            var subsystemTerrain = player.Project.Subsystems.Where(x => x is SubsystemTerrain).FirstOrDefault() as SubsystemTerrain;
            if (subsystemTerrain == null) throw new Exception("SubsystemTerrain cannot be null!");
            var terrain = subsystemTerrain.Terrain;
            if (terrain == null) return null;

            var bluePrint = player.Entity.Components.Where(x => x is ComponentBluePrint).FirstOrDefault() as ComponentBluePrint;
            if (bluePrint == null) throw new Exception("Blue Print Key Component not found!");

            if (!byPosition)
            {

                if (!(bluePrint.FirstPosition.HasValue))
                {
                    //ScreenLog.Error("Please Select 1 Block First.");
                    //player.ComponentGui.DisplaySmallMessage("Please Select 1 Block First.", Color.Olive, false, true);
                    //return null;
                    throw new Exception("Please Select 1 Block First.");
                }
                if (bluePrint.SecondPosition.HasValue)
                {
                    //ScreenLog.Error("Please Select Only 1 Block.");
                    //player.ComponentGui.DisplaySmallMessage("Please Select Only 1 Block.", Color.Olive, false, true);
                    //return null;
                    throw new Exception("Please Select Only 1 Block.");
                }
            }
            return (bluePrint, subsystemTerrain);
        }
    }
}
