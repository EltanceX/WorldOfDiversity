using DebugMod;
using Engine;
using Engine.Graphics;
using Game;
using Silk.NET.OpenGLES;

namespace GlassMod
{
    public class FlareBlock : Block
    {
        public static int Index = 906;
        public static List<Texture2D> FlareTextures = new List<Texture2D>();

        public BlockMesh m_standaloneBlockMesh = new();
        public Texture2D FlareTexture;
        public BlockMesh blockMesh = new BlockMesh();
        public BlockMesh standaloneBlockMesh = new BlockMesh();
        public Color blockColor = Color.White;
        public static void LoadTextures()
        {
            string[] TextureNames = new string[] {
                "Textures/Flares/drg_flare_black",
                "Textures/Flares/drg_flare_blue",
                "Textures/Flares/drg_flare_brown",
                "Textures/Flares/drg_flare_cyan",
                "Textures/Flares/drg_flare_gray",
                "Textures/Flares/drg_flare_green",
                "Textures/Flares/drg_flare_light_blue",
                "Textures/Flares/drg_flare_light_gray",
                "Textures/Flares/drg_flare_lime",
                "Textures/Flares/drg_flare_magenta",
                "Textures/Flares/drg_flare_orange",
                "Textures/Flares/drg_flare_pink",
                "Textures/Flares/drg_flare_purple",
                "Textures/Flares/drg_flare_red",
                "Textures/Flares/drg_flare_white",
                "Textures/Flares/drg_flare_yellow",
            };
            foreach (string TextureName in TextureNames)
            {
                var texture = ContentManager.Get<Texture2D>(TextureName);
                if (texture != null) FlareTextures.Add(texture);
            }

        }
        public override void Initialize()
        {
            DefaultDisplayName = "DRG Flares";
            DefaultCategory = "Tools";
            Behaviors = nameof(SubsystemFlareBlockBehavior);
            DisplayOrder = 209;
            this.DefaultIconBlockOffset = new Vector3(0f, -0.2f, 0f);//z表示左右，负右正左
            DefaultIconViewOffset = new Vector3(0.5f, 0.8f, 1f);
            DefaultIconViewScale = 1.4f;
            FirstPersonScale = 0.4f;
            FirstPersonOffset = new Vector3(0.7f, -0.5f, -0.6f);
            FirstPersonRotation = new Vector3(0, 40, 0);
            InHandScale = 0.25f;
            InHandOffset = new Vector3(0, 0, 0);
            InHandRotation = new Vector3(0, 0, 0);
            CraftingId = "flare_block";
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
            MaxStacking = 64;
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
            Durability = -1;
            IsAimable = true;
            IsStickable = false;
            AlignToVelocity = false;
            ProjectileSpeed = 14;
            ProjectileDamping = 0.9f;
            ProjectileTipOffset = 0;
            DisintegratesOnHit = false;
            ProjectileStickProbability = 0;
            FireDuration = 0;
            DefaultExplosionPressure = 50;
            DefaultExplosionIncendiary = true;
            DefaultTextureSlot = 227;
            DestructionDebrisScale = 1;
            FlareTexture = ContentManager.Get<Texture2D>("Textures/DRGFlaresGreen");
            LoadTextures();
            //FlareTexture.


            Model model = ContentManager.Get<Model>("Models/DRGFlares");
            Matrix boneTransPole = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pole").ParentBone);
            Matrix boneTransCubeTop = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CubeTop").ParentBone);
            Matrix boneTransCubeBottom = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CubeBottom").ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Pole").MeshParts[0], boneTransPole * Matrix.CreateTranslation(0f, -0.25f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CubeTop").MeshParts[0], boneTransCubeTop * Matrix.CreateTranslation(0f, -0.25f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CubeBottom").MeshParts[0], boneTransCubeBottom * Matrix.CreateTranslation(0f, -0.25f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateMeshVertices(this, x, y, z, blockMesh, blockColor, null, geometry.GetGeometry(FlareTexture).SubsetOpaque);
            //blockMesh.
        }
        public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
        {
            var craftingRecipe = new CraftingRecipe
            {
                ResultCount = 32,
                //ResultValue = Terrain.MakeBlockValue(182,0,SetColor(0,color)),
                ResultValue = BlocksManager.GetBlockIndex<FlareBlock>(),
                RemainsCount = 0,
                //RemainsValue = Terrain.MakeBlockValue(90),
                RequiredHeatLevel = 0f,
                Description = "照明弹，提供临时的光源"
            };
            craftingRecipe.Ingredients[0] = "glass";
            craftingRecipe.Ingredients[1] = "ironingot";
            craftingRecipe.Ingredients[2] = "glass";
            //craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
            yield return craftingRecipe;


            var craftingRecipe2 = new CraftingRecipe
            {
                ResultCount = 24,
                //ResultValue = Terrain.MakeBlockValue(182,0,SetColor(0,color)),
                ResultValue = BlocksManager.GetBlockIndex<FlareBlock>(),
                RemainsCount = 0,
                //RemainsValue = Terrain.MakeBlockValue(90),
                RequiredHeatLevel = 0f,
                Description = "照明弹，提供临时的光源"
            };
            craftingRecipe2.Ingredients[0] = "copperingot";
            craftingRecipe2.Ingredients[1] = "glass";
            craftingRecipe2.Ingredients[2] = "copperingot";
            //craftingRecipe2.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
            yield return craftingRecipe2;
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            //ScreenLog.Info("Draw Flare Block");
            //return;
            //BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
            //ScreenLog.Info(value);
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, FlareTexture, Color.White, size, ref matrix, environmentData);

            //BlocksManager.DrawCubeBlock()
        }
        public void DrawFlareBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData, int flareTextureIndex)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, FlareTextures[flareTextureIndex], Color.White, size * 1.6f, ref matrix, environmentData);

        }
    }
}
