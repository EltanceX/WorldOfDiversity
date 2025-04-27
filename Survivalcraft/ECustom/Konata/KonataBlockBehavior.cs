using Game;

namespace GlassMod
{
    public class KonataBlockBehavior : SubsystemBlockBehavior
    {
        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            int cellValue = SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
            int cellValue2 = SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
            int ext1 = Terrain.ExtractContents(cellValue);
            int ext2 = Terrain.ExtractContents(cellValue2);
            if (!BlocksManager.Blocks[ext1].IsNonAttachable(cellValue) && ext2 == 0)
            {
                SubsystemTerrain.ChangeCell(x, y + 1, z, value);
                //int value2 = Terrain.ReplaceData(SubsystemTerrain.Terrain.GetCellValue(x,y + 1,z), 1);
                //SubsystemTerrain.ChangeCell(x,y + 1,z,10);
                //SubsystemTerrain.DestroyCell(10,x,y + 1,z,0,true,true);
                //int xxx = Terrain.ExtractData(value);
                //int xxx2 = Terrain.ExtractData(value2);
                //ScreenLog.Info();
            }

            int x1 = Terrain.ExtractData(value);
            int x2 = Terrain.ExtractData(cellValue);
            int x3 = Terrain.ExtractData(cellValue2);
        }
        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            //return;
            int cellValue = SubsystemTerrain.Terrain.GetCellValue(x, y, z);
            int num = Terrain.ExtractContents(cellValue);
            Block obj = BlocksManager.Blocks[num];
            int data = Terrain.ExtractData(cellValue);
            data = 4;
            if (!(obj is Konata2048))
            {
                return;
            }
            if (neighborX == x && neighborY == y && neighborZ == z)
            {
                //if(Konata2048.IsBottomPart(SubsystemTerrain.Terrain,x,y,z))
                //{
                //	int value = Terrain.ReplaceData(SubsystemTerrain.Terrain.GetCellValue(x,y + 1,z),data);
                //	//SubsystemTerrain.Terrain.
                //	SubsystemTerrain.ChangeCell(x,y + 1,z,value);
                //}
                if (Konata2048.IsTopPart(SubsystemTerrain.Terrain, x, y, z))
                {
                    //int value2 = Terrain.ReplaceData(SubsystemTerrain.Terrain.GetCellValue(x,y - 1,z),data);
                    //SubsystemTerrain.ChangeCell(x,y - 1,z,value2);
                    int value2 = Terrain.ReplaceData(SubsystemTerrain.Terrain.GetCellValue(x, y, z), data);
                    SubsystemTerrain.ChangeCell(x, y, z, value2);
                }
            }
            //return;

            if (Konata2048.IsBottomPart(SubsystemTerrain.Terrain, x, y, z))
            {
                int cellValue2 = SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z);
                if (BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)].IsNonAttachable(cellValue2))
                {
                    SubsystemTerrain.DestroyCell(0, x, y, z, 0, noDrop: false, noParticleSystem: false);
                }
            }
            if (!Konata2048.IsBottomPart(SubsystemTerrain.Terrain, x, y, z) && !Konata2048.IsTopPart(SubsystemTerrain.Terrain, x, y, z))
            {
                SubsystemTerrain.DestroyCell(0, x, y, z, 0, noDrop: true, noParticleSystem: false);
            }
        }
        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            CellFace cellFace = raycastResult.CellFace;
            int cellValue = SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
            int num = Terrain.ExtractContents(cellValue);
            int data = Terrain.ExtractData(cellValue);
            //bool open = Konata2048.GetOpen(data);
            //bool open = data == 2;
            int openData = data == 3 ? 4 : 3;
            return ChangeState(cellFace.X, cellFace.Y, cellFace.Z, openData);
        }
        public bool ChangeState(int x, int y, int z, int openData)
        {
            int cellValue = SubsystemTerrain.Terrain.GetCellValue(x, y, z);
            int num = Terrain.ExtractContents(cellValue);
            if (BlocksManager.Blocks[num] is Konata2048)
            {
                int extd = Terrain.ExtractData(cellValue);
                //int data = Konata2048.SetOpen(extd,open);
                int value = Terrain.ReplaceData(cellValue, openData);
                SubsystemTerrain.ChangeCell(x, y, z, value);
                //string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
                //SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(throwOnError: true).PlaySound(name,0.7f,m_random.Float(-0.1f,0.1f),new Vector3(x,y,z),4f,autoDelay: true);
                return true;
            }
            return false;
        }
    }
}
