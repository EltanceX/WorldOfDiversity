using Game;
using TemplatesDatabase;
using TVKeyboardMenu;
using Engine;

namespace GlassMod
{
    public class TVKeyboardBlockBehavior : SubsystemBlockBehavior
    {
        public SubsystemPickables sub;

        //public SubsystemGunsBulletBlockBehavior SubsystemGunsBulletBlockBehavior;

        public override int[] HandledBlocks => new int[1] { 902 };

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            sub = base.Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
            //SubsystemGunsBulletBlockBehavior = base.Project.FindSubsystem<SubsystemGunsBulletBlockBehavior>(throwOnError: true);
        }

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            //Functions.AddTech(SubsystemGunsBulletBlockBehavior,"枪械台",100);
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            return true;
            bool visible = KeyboardMenu.Instance.Visible;
            KeyboardMenu.Instance.Visible = !visible;
            //if(visible) EntryHook.CancelPreventIngameKeyevent(KeyboardMenu.Instance.ClassUUID);
            //else EntryHook.PreventIngameKeyevent(KeyboardMenu.Instance.ClassUUID);

            return true;
        }
        public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
        {
            return base.OnUse(ray, componentMiner);
        }
    }

    //public class FirearmsTableBlock : AlphaTestCubeBlock
    //{
    //	public const int Index = 605;

    //	public BlockMesh m_blockMesh = new BlockMesh();

    //	public BlockMesh m_standaloneBlockMesh = new BlockMesh();

    //	public override void Initialize()
    //	{
    //		Model model = ContentManager.Get<Model>("Models/FirearmsTable");
    //		Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Table").ParentBone);
    //		m_blockMesh.AppendModelMeshPart(model.FindMesh("Table").MeshParts[0],boneAbsoluteTransform * Matrix.CreateTranslation(0f,0f,0f),makeEmissive: false,flipWindingOrder: false,doubleSided: false,flipNormals: false,Color.White);
    //		m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Table").MeshParts[0],boneAbsoluteTransform * Matrix.CreateTranslation(0f,-0.5f,0f),makeEmissive: false,flipWindingOrder: false,doubleSided: false,flipNormals: false,Color.White);
    //		base.Initialize();
    //	}

    //	public override void GenerateTerrainVertices(BlockGeometryGenerator generator,TerrainGeometry geometry,int value,int x,int y,int z)
    //	{
    //		generator.GenerateShadedMeshVertices(this,x,y,z,m_blockMesh,Color.White,null,null,geometry.SubsetOpaque);
    //	}

    //	public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer,int value,Color color,float size,ref Matrix matrix,DrawBlockEnvironmentData environmentData)
    //	{
    //		BlocksManager.DrawMeshBlock(primitivesRenderer,m_standaloneBlockMesh,color,size,ref matrix,environmentData);
    //	}
    //}
}
