using DebugMod;
using Engine;
using Engine.Graphics;
using Game;

namespace GlassMod
{
    public class Konata2048 : ModelBlock
    {
        public const int Index = 903;
        public Texture2D flatTexture;
        public BoundingBox[][] m_collisionBoxesBySize = new BoundingBox[8][];

        public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];

        public Konata2048()
            : base("Textures/Konata2048", "Models/temp1fb")
        {
            DefaultDisplayName = "此方手办";
            DefaultDescription = "幸运星Lucky☆Star";
            DefaultCategory = "WebTV";
            CraftingId = "konata";
            DefaultDropContent = Index;
            this.LightAttenuation = 0;
            this.DefaultEmittedLightAmount = 10;
            this.NoAutoJump = true;
            this.Behaviors = typeof(KonataBlockBehavior).Name;

            this.DefaultCreativeData = 3;
            this.HasCollisionBehavior = false;
        }
        public override int GetTextureSlotCount(int value)
        {
            return 1;
        }
        public override void Initialize()
        {
            float scale = 0.5f;
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Node").ParentBone);
            blockMesh.AppendModelMeshPart(model.FindMesh("Node").MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(scale) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: true, flipNormals: false, blockColor);
            blockMesh.AppendModelMeshPart(model.FindMesh("Node").MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(scale) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: true, flipNormals: false, blockColor);
            for (int i = 1; i <= 28; i++)
            {
                try
                {
                    var name = "Node." + i.ToString().PadLeft(3, '0');
                    Matrix b2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name).ParentBone);
                    blockMesh.AppendModelMeshPart(model.FindMesh(name).MeshParts[0], b2 * Matrix.CreateScale(scale) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: true, flipNormals: false, blockColor);
                    blockMesh.AppendModelMeshPart(model.FindMesh(name).MeshParts[0], b2 * Matrix.CreateScale(scale) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: true, flipNormals: false, blockColor);
                }
                catch (Exception e)
                {
                    ScreenLog.Info(e);
                }
            }
            //standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Node").MeshParts[0],boneAbsoluteTransform * Matrix.CreateScale(scale) * Matrix.CreateTranslation(0f,0f,0f),makeEmissive: false,flipWindingOrder: false,doubleSided: false,flipNormals: false,blockColor);
            //Image standaloneImage = Image.Load(ContentManager.Resources["Textures/Blocks.webp"].ContentStream);
            flatTexture = ContentManager.Get<Texture2D>("Textures/otaku");
            //flatTexture = ContentManager.Get<Texture2D>("Textures/Blocks");
            //standaloneBlockMesh.AppendImageExtrusion(standaloneImage,new Rectangle(0,0,256,256),new Vector3(1),Color.White);
            //standaloneBlockMesh.SetColor(Color.Blue);
            //standaloneBlockMesh.ModulateColor(Color.Blue);

            for (int j = 0; j < 8; j++)
            {
                BoundingBox boundingBox = new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(0.5f, 0f, 0.5f));
                float num3 = boundingBox.Max.X - boundingBox.Min.X;
                if (num3 < 0.8f)
                {
                    float num4 = (0.6f - num3) / 2f;
                    boundingBox.Min.X -= num4;
                    boundingBox.Min.Z -= num4;
                    boundingBox.Max.X += num4;
                    boundingBox.Max.Y = 1.0f;
                    boundingBox.Max.Z += num4;
                }
                m_collisionBoxesBySize[j] = new BoundingBox[1]
                {
                    boundingBox
                };
            }
            //m_collisionBoxesBySize[1] = new BoundingBox[1]
            //	{
            //		new BoundingBox(new Vector3(-0.1f,0f,-0.1f),new Vector3(0.1f,0.4f,0.1f))
            //	};
            m_collisionBoxesBySize[1][0].Max.Y = 0.4f;

            //m_collisionBoxesByData[0] = new BoundingBox[1] { new BoundingBox(new Vector3(0f,0f,0.875f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[1] = new BoundingBox[1] { new BoundingBox(new Vector3(0.875f,0f,-2.98e-8f), new Vector3(1f,1f,1f)) };
            //m_collisionBoxesByData[2] = new BoundingBox[1] { new BoundingBox(new Vector3(2.98e-8f,0f,-4.47e-8f), new Vector3(1f,2f,0.125f)) };
            //m_collisionBoxesByData[3] = new BoundingBox[1] { new BoundingBox(new Vector3(-7.45e-9f,0f,0f), new Vector3(0.125f,2f,1f)) };
            //m_collisionBoxesByData[4] = new BoundingBox[1] { new BoundingBox(new Vector3(0.875f,0f,0f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[5] = new BoundingBox[1] { new BoundingBox(new Vector3(-2.98e-8f,0f,-2.98e-8f), new Vector3(1f,2f,0.125f)) };
            //m_collisionBoxesByData[6] = new BoundingBox[1] { new BoundingBox(new Vector3(-5.21e-8f,0f,0f), new Vector3(0.125f,2f,1f)) };
            //m_collisionBoxesByData[7] = new BoundingBox[1] { new BoundingBox(new Vector3(0f,0f,0.875f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[8] = new BoundingBox[1] { new BoundingBox(new Vector3(0f,0f,0.875f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[9] = new BoundingBox[1] { new BoundingBox(new Vector3(0.875f,0f,-2.98e-8f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[10] = new BoundingBox[1] { new BoundingBox(new Vector3(-2.98e-8f,0f,-4.47e-8f), new Vector3(1f,2f,0.125f)) };
            //m_collisionBoxesByData[11] = new BoundingBox[1] { new BoundingBox(new Vector3(-7.45e-9f,0f,0f), new Vector3(0.125f,2f,1f)) };
            //m_collisionBoxesByData[12] = new BoundingBox[1] { new BoundingBox(new Vector3(-5.21e-8f,0f,0f), new Vector3(0.125f,2f,1f)) };
            //m_collisionBoxesByData[13] = new BoundingBox[1] { new BoundingBox(new Vector3(0f,0f,0.875f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[14] = new BoundingBox[1] { new BoundingBox(new Vector3(0.875f,-2.98e-8f,0.5f), new Vector3(1f,2f,1f)) };
            //m_collisionBoxesByData[15] = new BoundingBox[1] { new BoundingBox(new Vector3(0f,0f,-5.96e-8f), new Vector3(1f,2f,0.125f)) };
            //m_collisionBoxesBySize[0] = new BoundingBox[1]
            //	{
            //		new BoundingBox(new Vector3(0f,0f,0f),new Vector3(0.5f,0f,0.5f))
            //	};
            base.Initialize();
        }
        public static int GetRotation(int data)
        {
            return data & 3;
        }

        /// <summary>
        /// 纯物品栏绘制
        /// </summary>
        /// <param name="primitivesRenderer"></param>
        /// <param name="value"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="matrix"></param>
        /// <param name="environmentData"></param>
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            //int ext1 = Terrain.ExtractData(value);

            //if(ext1!=0)Debugger.Break();
            //if(ext1 != 3)
            base.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
            BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, flatTexture, Color.White, true, environmentData);
            //if(ext1 == 3)
            //BlocksManager.DrawCubeBlock(primitivesRenderer,value,new Vector3(size),ref matrix,Color.Red,Color.Blue,environmentData);
        }
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            //if(IsBottomPart(terrain))
            int data = Terrain.ExtractData(value);
            if (data == 4) return; //只绘制下方方块
            base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
        }
        public static int GetSize(int data)
        {
            return 7 - (data & 7);
        }
        public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
        {
            //return m_collisionBoxesBySize[0];
            //int size = GetSize(Terrain.ExtractData(value));
            //return m_collisionBoxesBySize[size];
            return m_collisionBoxesBySize[Terrain.ExtractData(value) == 4 ? 1 : 0];


            //int num = Terrain.ExtractData(value);
            //if(num < m_collisionBoxesByData.Length)
            //{
            //	return m_collisionBoxesByData[num];
            //}
            //return null;
        }
        public static bool IsTopPart(Terrain terrain, int x, int y, int z)
        {
            return BlocksManager.Blocks[terrain.GetCellContents(x, y - 1, z)] is Konata2048;
        }

        public static bool IsBottomPart(Terrain terrain, int x, int y, int z)
        {
            return BlocksManager.Blocks[terrain.GetCellContents(x, y + 1, z)] is Konata2048;
        }
        public static int SetOpen(int data, bool open)
        {
            if (!open)
            {
                return data & -5;
            }
            return data | 4;
        }
        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            //return base.GetPlacementValue(subsystemTerrain,componentMiner,value,raycastResult);

            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            //float num = Vector3.Dot(forward,Vector3.UnitZ);
            //float num2 = Vector3.Dot(forward,Vector3.UnitX);
            //float num3 = Vector3.Dot(forward,-Vector3.UnitZ);
            //float num4 = Vector3.Dot(forward,-Vector3.UnitX);
            //int num5 = 2;
            //if(num == MathUtils.Max(num,num2,num3,num4))
            //{
            //	num5 = 2;
            //}
            //else if(num2 == MathUtils.Max(num,num2,num3,num4))
            //{
            //	num5 = 3;
            //}
            //else if(num3 == MathUtils.Max(num,num2,num3,num4))
            //{
            //	num5 = 0;
            //}
            //else if(num4 == MathUtils.Max(num,num2,num3,num4))
            //{
            //	num5 = 1;
            //}
            Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
            int num6 = raycastResult.CellFace.X + point.X;
            int y = raycastResult.CellFace.Y + point.Y;
            int num7 = raycastResult.CellFace.Z + point.Z;
            //bool rightHanded = true;
            //switch(num5)
            //{
            //	case 0:
            //	int cellValue = subsystemTerrain.Terrain.GetCellValue(num6 - 1,y,num7);
            //	rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsNonAttachable(cellValue);
            //	break;
            //	case 1:
            //	cellValue = subsystemTerrain.Terrain.GetCellValue(num6,y,num7 + 1);
            //	rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsNonAttachable(cellValue);
            //	break;
            //	case 2:
            //	cellValue = subsystemTerrain.Terrain.GetCellValue(num6 + 1,y,num7);
            //	rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsNonAttachable(cellValue);
            //	break;
            //	case 3:
            //	cellValue = subsystemTerrain.Terrain.GetCellValue(num6,y,num7 - 1);
            //	rightHanded = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsNonAttachable(cellValue);
            //	break;
            //}
            //int data = SetRightHanded(SetOpen(SetRotation(0,num5),open: false),rightHanded);
            int data = 3; //Bottom
                          //bool isTop = IsTopPart(subsystemTerrain.Terrain,num6,y,num7);
            BlockPlacementData result = default;
            result.Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, BlockIndex), data);
            result.CellFace = raycastResult.CellFace;
            return result;
        }
        public static int SetRotation(int data, int rotation)
        {
            return (data & -4) | (rotation & 3);
        }
        public static int SetRightHanded(int data, bool rightHanded)
        {
            if (rightHanded)
            {
                return data & -9;
            }
            return data | 8;
        }
        //public override void GenerateTerrainVertices(BlockGeometryGenerator generator,TerrainGeometry geometry,int value,int x,int y,int z)
        //{
        //	base.GenerateTerrainVertices(generator,geometry,value,x,y,z);
        //	generator.GenerateFlatVertices(this,value,x,y,z,0,Color.Orange,geometry.GetGeometry(texture2D).Subsets);

        //}

    }
}
