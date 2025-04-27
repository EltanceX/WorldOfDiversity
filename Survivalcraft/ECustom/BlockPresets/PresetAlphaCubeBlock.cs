using Engine;
using Engine.Graphics;
using Game;

namespace GlassMod
{
    public abstract class PresetAlphaCubeBlock : AlphaTestCubeBlock
    {
        public Texture2D texture2D;
        public Texture2D handTexture;
        public PresetAlphaCubeBlock(string TexturePath)
        {
            FirstPersonOffset = new Vector3(0.5f, -0.5f, -0.6f);
            FirstPersonRotation = new Vector3(0, 40, 0);
            FirstPersonScale = 0.2f;
            InHandScale = 0.5f;
            InHandOffset = new Vector3(0, 0.1f, -0.2f);
            this.texture2D = ContentManager.Get<Texture2D>(TexturePath);
            IsTransparent = false;
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            color = Color.White;
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), 1, ref matrix, color, color, environmentData, handTexture ?? texture2D);
            //DevicesBlockManager.GetDevice(value).DrawBlock(primitivesRenderer,value,color,size,ref matrix,environmentData);

            //base.DrawBlock(primitivesRenderer,value,color,size,ref matrix,environmentData);
            //BlocksManager.DrawFlatBlock(primitivesRenderer,value,size,ref matrix,flatTexture,Color.White,true,environmentData);
        }
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            //base.GenerateTerrainVertices(generator,geometry,value,x,y,z);
            var subsets = geometry.GetGeometry(texture2D).TransparentSubsetsByFace;
            generator.GenerateCubeVertices(this, value, x, y, z, Color.White, subsets);
        }
        public override int GetTextureSlotCount(int value)
        {
            return 16;
        }
        //public override get
        public override int GetFaceTextureSlot(int face, int value)
        {
            switch (face)
            {
                case 0: return 0; //z+
                case 1: return 1; //x+
                case 2: return 2; //z-
                case 3: return 3; //x-
                case 4: return 4; //y+
                case 5: return 5; //y-
            }
            return 1;
        }
        //public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain,int face,int value)
        //{
        //	return true;
        //}
        //public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain,int face,int value,int neighborValue,int x,int y,int z)
        //{
        //	return true;

        //}
        public override bool GenerateFacesForSameNeighbors_(int value)
        {
            return base.GenerateFacesForSameNeighbors_(value);
        }
    }
}
