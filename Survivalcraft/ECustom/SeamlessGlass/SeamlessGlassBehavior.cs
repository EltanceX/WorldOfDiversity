using Game;

namespace GlassMod
{
    public class SeamlessGlassBehavior : SubsystemBlockBehavior
    {
        //public override  
        public static int[][] detect = {
            new int[]{0, 1, 0 , 0b100000}, //y+ 0
			new int[] { 0,-1,0, 0b010000 },//y- 1
			new int[] { 1,0,0,0b001000 },  //x+ 2
			new int[]{-1, 0, 0, 0b000100 },//x- 3
			new int[] {0, 0, 1,0b000010 }, //z+ 4
			new int[]{ 0, 0, -1,0b000001 } //z- 5
		};
        public int UpdateConnectValue(int x, int y, int z)
        {
            var terrain = SubsystemTerrain.Terrain;
            int value = 0;

            foreach (int[] toDetect in detect)
            {
                int cell = terrain.GetCellValue(x + toDetect[0], y + toDetect[1], z + toDetect[2]);
                int content = Terrain.ExtractContents(cell);
                var block = BlocksManager.Blocks[content];
                if (block is SeamlessGlass)
                {
                    value |= toDetect[3];
                }
            }
            return value;
        }
        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            base.OnBlockAdded(value, oldValue, x, y, z);
            var terrain = SubsystemTerrain.Terrain;
            var a = terrain.GetCellValue(x, y, z);
            var a2 = Terrain.ReplaceData(value, 0b000001);
            var b = terrain.GetCellContentsFast(x, y, z);
            SubsystemTerrain.ChangeCell(x, y, z, a2);
        }
        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
            int newValue = UpdateConnectValue(x, y, z);
            var terrain = SubsystemTerrain.Terrain;
            var v = terrain.GetCellValue(x, y, z);
            var a = Terrain.ExtractData(v);
            var a2 = Terrain.ReplaceData(v, newValue);
            if (a != a2) SubsystemTerrain.ChangeCell(x, y, z, a2);
        }
    }
}
