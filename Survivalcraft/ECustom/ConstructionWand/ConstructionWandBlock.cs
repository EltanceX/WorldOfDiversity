using Engine;
using Engine.Graphics;
using Game;
using System.Diagnostics;

namespace GlassMod
{
    public class ConstructionWandBlock : Block
    {
        public static int Index = 907;
        public Texture2D Texture;
        public BlockMesh m_standaloneBlockMesh = new();
        public ConstructionWandBlock()
        {
            //Debugger.Break();
        }
        public override void Initialize()
        {
            //Model model = ContentManager.Get<Model>("Models/Whistle");
            //Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Whistle").ParentBone);
            //m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Whistle").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.04f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, new Color(255, 255, 255));
            //m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Whistle").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.04f, 0f), makeEmissive: false, flipWindingOrder: true, doubleSided: false, flipNormals: false, new Color(64, 64, 64));

            DefaultDisplayName = "Iron Construction Wand";
            DefaultCategory = "Tools";
            DisplayOrder = 191;
            //DefaultIconBlockOffset = new Vector3(0.3f,-0.25f, 0);
            //DefaultIconViewOffset = new Vector3(-1, 1, -1);
            DefaultIconViewOffset = new Vector3(1, 0.2f, 1);
            DefaultIconViewScale = 1.0f;
            FirstPersonScale = 1.0f;
            //FirstPersonOffset = new Vector3(0.5f, -0.5f, -0.6f);
            FirstPersonOffset = new Vector3(2f, -0.8f, -2f); //z向屏幕内部 y正上负下零中间 x正方向右侧
            FirstPersonRotation = new Vector3(0, 90, 40);//绕x/y/z轴旋转
            InHandScale = 0.3f;
            InHandOffset = new Vector3(0,0.05f,-0.2f);
            InHandRotation = new Vector3(-30, 0, 0);
            CraftingId = "ConstructionWandBlock";
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
            ShovelPower = 1;
            QuarryPower = 1;
            HackPower = 1;
            DefaultMeleePower = 1;
            DefaultProjectilePower = 1;
            ToolLevel = 1;
            Durability = 300;
            IsAimable = false;
            IsStickable = false;
            AlignToVelocity = false;
            ProjectileSpeed = 0;
            ProjectileTipOffset = 0;
            DisintegratesOnHit = false;
            ProjectileStickProbability = 0;
            FireDuration = 0;
            DefaultTextureSlot = 0;
            DestructionDebrisScale = 1;

            Texture = ContentManager.Get<Texture2D>("Textures/IronConstructionWand");
            Behaviors = nameof(ConstructionWandBehavior);
            base.Initialize();
        }
        //public override int GetShadowStrength(int value)
        //{
        //    return base.GetShadowStrength(value);
        //}
        public override int GetTextureSlotCount(int value)
        {
            return 1;
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            //BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 9f * size, ref matrix, environmentData);
            BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, Texture, Color.White, true, environmentData);
        }
    }
}
