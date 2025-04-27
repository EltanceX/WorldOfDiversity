using Engine;
using Engine.Graphics;
using Game;

namespace GlassMod
{
    public class BluePrintAxeBlock : Block
    {
        public static int Index = 908;

        public int m_handleTextureSlot;

        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public BluePrintAxeBlock()
        {
            m_handleTextureSlot = 47;
            m_headTextureSlot = 79;
        }

        public override void Initialize()
        {
            DefaultDisplayName = "BluePrint Axe";
            DefaultCategory = "Tools";
            DisplayOrder = 168;
            DefaultIconBlockOffset = new Vector3(0, 0, 0);
            DefaultIconViewOffset = new Vector3(-1, 0.5f, 0);
            DefaultIconViewScale = 0.85f;
            FirstPersonScale = 0.4f;
            FirstPersonOffset = new Vector3(0.5f, -0.5f, -0.6f);
            FirstPersonRotation = new Vector3(0, 40, 0);
            InHandScale = 0.35f;
            InHandOffset = new Vector3(0, 0.2f, 0);
            InHandRotation = new Vector3(0, 0, 0);
            CraftingId = "magicaxe";
            DefaultCreativeData = 0;
            IsCollidable = false;
            IsPlaceable = false;
            IsDiggingTransparent = false;
            IsPlacementTransparent = true;
            DefaultIsInteractive = false;
            IsEditable = false;
            IsNonDuplicable = false;
            IsGatherable = false;
            HasCollisionBehavior = false;
            KillsWhenStuck = false;
            IsFluidBlocker = false;
            IsTransparent = false;
            GenerateFacesForSameNeighbors = false;
            DefaultShadowStrength = 15;
            LightAttenuation = 0;
            DefaultEmittedLightAmount = 0;
            ObjectShadowStrength = 0;
            MaxStacking = 1;
            SleepSuitability = 0;
            FrictionFactor = 1;
            Density = 3;
            NoAutoJump = false;
            NoSmoothRise = false;
            FuelHeatLevel = 0;
            FuelFireDuration = 0;
            ShovelPower = 1.5f;
            QuarryPower = 1.5f;
            HackPower = 6;
            DefaultMeleePower = 6;
            DefaultMeleeHitProbability = 0.5f;
            DefaultProjectilePower = 1;
            ToolLevel = 3;
            Durability = 75;
            IsAimable = false;
            IsStickable = false;
            AlignToVelocity = false;
            ProjectileSpeed = 0;
            ProjectileTipOffset = 0;
            DisintegratesOnHit = false;
            ProjectileStickProbability = 0;
            FireDuration = 0;
            DefaultTextureSlot = 232;
            DestructionDebrisScale = 1;

            Behaviors = nameof(BluePrintBlockBehavior);




            Model model = ContentManager.Get<Model>("Models/Axe");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle").ParentBone);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Handle").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_handleTextureSlot % 16 / 16f, m_handleTextureSlot / 16 / 16f, 0f));
            var blockMesh2 = new BlockMesh();
            blockMesh2.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation(m_headTextureSlot % 16 / 16f, m_headTextureSlot / 16 / 16f, 0f));
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh2);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
        }
    }
}
