using Engine;
using Engine.Graphics;
using Game;

namespace GlassMod
{
    public abstract class ModelBlock : Block
    {
        public Texture2D texture2D;
        public BlockMesh blockMesh = new BlockMesh();
        public Color blockColor = Color.White;
        public BlockMesh standaloneBlockMesh = new BlockMesh();
        public Model model;

        public string TexturePath;
        public string ModelPath;

        public Matrix rotationX = Matrix.CreateRotationX(0);
        public Matrix rotationY = Matrix.CreateRotationY(0);
        public Matrix rotationZ = Matrix.CreateRotationZ(0);

        public int scale = 3;
        public string NodeMeshName = "Node";

        public ModelBlock(string TexturePath, string ModelPath)
        {
            this.TexturePath = TexturePath;
            this.ModelPath = ModelPath;
            this.texture2D = ContentManager.Get<Texture2D>(TexturePath);
            this.model = ContentManager.Get<Model>(ModelPath);
            FirstPersonOffset = new Vector3(0.5f, -0.5f, -0.6f);
            FirstPersonRotation = new Vector3(0, 40, 0);
            FirstPersonScale = 0.2f;
            InHandScale = 0.5f;
            InHandOffset = new Vector3(0, 0.1f, -0.2f);
            DefaultIconBlockOffset = new Vector3(0, -0.1f, 0);//平移
            DefaultIconViewOffset = new Vector3(1f, 0.8f, 1f);//旋转？
            this.DefaultIconViewScale = 0.8f;
            //DisplayOrder = 1;
            //DefaultCreativeData = 0;
            //IsPlacementTransparent = false;
            IsTransparent = true;
            //FirstPersonScale = 0.2f;
            //FirstPersonOffset = new Vector3(0.5f,-0.8f,-0.6f);
            //InHandScale = 0.1f;
            //InHandOffset = new Vector3(0f,-0.5f,0f);
            //DefaultIconBlockOffset = new Vector3(0f,-0.5f,0f);
            //DefaultIconViewOffset = new Vector3(1f,0f,1f);
            //DigResilience = 0.1f;
            //IsCollidable = false;
            //DefaultIconViewScale = 0.7f;
            //DigMethod = BlockDigMethod.Quarry;
            //Color = Color.White;
            DefaultIsInteractive = true;
            this.HasCollisionBehavior = true;
        }
        public override void Initialize()
        {
            //Matrix rotationY = Matrix.CreateRotationX((float)(Math.PI / 2));
            var rotation = rotationX * rotationY * rotationZ;
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(NodeMeshName).ParentBone);
            blockMesh.AppendModelMeshPart(model.FindMesh(NodeMeshName).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, blockColor);
            standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(NodeMeshName).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(scale) * Matrix.CreateTranslation(0f, 0f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, blockColor);

            base.Initialize();
        }

        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, blockColor, 123);
        }

        /// <summary>
        /// Execute While Block Placed
        /// </summary>
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            //generator.GenerateShadedMeshVertices(this,x,y,z,blockMesh,blockColor,null,null,geometry.GetGeometry(texture2D).SubsetOpaque);
            generator.GenerateMeshVertices(this, x, y, z, blockMesh, blockColor, null, geometry.GetGeometry(texture2D).SubsetOpaque);

            //Vector2 centerOffset = new Vector2(0.45f,0f);
            //generator.GenerateWireVertices(value,x,y,z,0,0.01f,centerOffset,geometry.SubsetOpaque);
            //generator.GenerateWireVertices(value,x,y,z,1,0.01f,centerOffset,geometry.SubsetOpaque);
            //generator.GenerateWireVertices(value,x,y,z,2,0.01f,centerOffset,geometry.SubsetOpaque);
            //generator.GenerateWireVertices(value,x,y,z,3,0.01f,centerOffset,geometry.SubsetOpaque);
            //generator.GenerateWireVertices(value,x,y,z,4,0.01f,centerOffset,geometry.SubsetOpaque);
            //generator.GenerateWireVertices(value,x,y,z,5,0.01f,centerOffset,geometry.SubsetOpaque);
        }

        /// <summary>
        /// Draw Item Block
        /// </summary>
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {

            BlocksManager.DrawMeshBlock(primitivesRenderer, standaloneBlockMesh, texture2D, blockColor, 2f * size, ref matrix, environmentData);

            //this.GetCustomCollisionBoxes
            //this.HasCollisionBehavior
            //this.IsCollapsable
        }
    }
}
