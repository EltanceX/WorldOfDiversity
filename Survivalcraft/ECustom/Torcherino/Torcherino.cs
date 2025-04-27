using Engine;
using Engine.Graphics;
using GameEntitySystem;
using GlassMod;
using System;
using System.Diagnostics;

namespace Game
{
    public class Torcherino : Block
    {
        public static int Index = 905;

        public BlockMesh m_standaloneBlockMesh = new();

        public BlockMesh[] m_blockMeshesByVariant = new BlockMesh[5];

        public BoundingBox[][] m_collisionBoxes = new BoundingBox[5][];

        public Torcherino()
        {
            this.DefaultDisplayName = "Torcherino+";
            CanBeBuiltIntoFurniture = true;
            DefaultCategory = "Items";
            DisplayOrder = 114;
            DefaultIconBlockOffset = new Vector3(0, 0, 0);
            DefaultIconViewOffset = new Vector3(1, 1, 1);
            DefaultIconViewScale = 1;
            FirstPersonScale = 0.4f;
            FirstPersonOffset = new Vector3(0.5f, -0.5f, -0.6f);
            FirstPersonRotation = new Vector3(0, 40, 0);
            InHandScale = 0.5f;
            InHandOffset = new Vector3(0, 0.12f, 0);
            InHandRotation = new Vector3(0, 0, 0);
            CraftingId = "torcherino";
            DefaultCreativeData = 0;
            IsCollidable = false;
            IsPlaceable = true;
            IsDiggingTransparent = false;
            IsPlacementTransparent = true;
            DefaultIsInteractive = true;
            IsEditable = false;
            IsNonDuplicable = false;
            IsGatherable = false;
            HasCollisionBehavior = false;
            KillsWhenStuck = false;
            IsFluidBlocker = false;
            IsTransparent = true;
            GenerateFacesForSameNeighbors = false;
            DefaultShadowStrength = -99;
            LightAttenuation = 0;
            DefaultEmittedLightAmount = 15;
            ObjectShadowStrength = 0;
            DefaultDropContent = BlocksManager.GetBlockIndex<Torcherino>();
            DefaultDropCount = 1;
            RequiredToolLevel = 0;
            MaxStacking = 16;
            SleepSuitability = 0;
            FrictionFactor = 1;
            Density = 0.5f;
            NoAutoJump = false;
            NoSmoothRise = false;
            FuelHeatLevel = 0;
            FuelFireDuration = 0;
            DefaultSoundMaterialName = "Wood";
            ShovelPower = 1;
            QuarryPower = 1;
            HackPower = 1;
            DefaultMeleePower = 1;
            DefaultProjectilePower = 1;
            ToolLevel = 1;
            Durability = -1;
            IsAimable = false;
            IsStickable = false;
            AlignToVelocity = false;
            ProjectileSpeed = 0;
            ProjectileTipOffset = 0;
            DisintegratesOnHit = false;
            ProjectileStickProbability = 0;
            FireDuration = 0;
            ExplosionResilience = 5;
            DigMethod = BlockDigMethod.Hack;
            DigResilience = 0;
            ProjectileResilience = 0;
            DefaultTextureSlot = 215;
            DestructionDebrisScale = 1;
            Behaviors = nameof(SubsystemTorcherinoBlockBehavior);
        }
        public override void Initialize()
        {
            for (int i = 0; i < m_blockMeshesByVariant.Length; i++)
            {
                m_blockMeshesByVariant[i] = new BlockMesh();
            }
            Model model = ContentManager.Get<Model>("Models/Torch");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Torch").ParentBone);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Flame").ParentBone);
            Matrix m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(0f) * Matrix.CreateTranslation(0.5f, 0.15f, -0.05f);
            m_blockMeshesByVariant[0].AppendModelMeshPart(model.FindMesh("Torch").MeshParts[0], boneAbsoluteTransform * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_blockMeshesByVariant[0].AppendModelMeshPart(model.FindMesh("Flame").MeshParts[0], boneAbsoluteTransform2 * m, makeEmissive: true, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY((float)Math.PI / 2f) * Matrix.CreateTranslation(-0.05f, 0.15f, 0.5f);
            m_blockMeshesByVariant[1].AppendModelMeshPart(model.FindMesh("Torch").MeshParts[0], boneAbsoluteTransform * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_blockMeshesByVariant[1].AppendModelMeshPart(model.FindMesh("Flame").MeshParts[0], boneAbsoluteTransform2 * m, makeEmissive: true, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY((float)Math.PI) * Matrix.CreateTranslation(0.5f, 0.15f, 1.05f);
            m_blockMeshesByVariant[2].AppendModelMeshPart(model.FindMesh("Torch").MeshParts[0], boneAbsoluteTransform * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_blockMeshesByVariant[2].AppendModelMeshPart(model.FindMesh("Flame").MeshParts[0], boneAbsoluteTransform2 * m, makeEmissive: true, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m = Matrix.CreateRotationX(0.6f) * Matrix.CreateRotationY(4.712389f) * Matrix.CreateTranslation(1.05f, 0.15f, 0.5f);
            m_blockMeshesByVariant[3].AppendModelMeshPart(model.FindMesh("Torch").MeshParts[0], boneAbsoluteTransform * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_blockMeshesByVariant[3].AppendModelMeshPart(model.FindMesh("Flame").MeshParts[0], boneAbsoluteTransform2 * m, makeEmissive: true, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m = Matrix.CreateTranslation(0.5f, 0f, 0.5f);
            m_blockMeshesByVariant[4].AppendModelMeshPart(model.FindMesh("Torch").MeshParts[0], boneAbsoluteTransform * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_blockMeshesByVariant[4].AppendModelMeshPart(model.FindMesh("Flame").MeshParts[0], boneAbsoluteTransform2 * m, makeEmissive: true, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Torch").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.25f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            for (int j = 0; j < 5; j++)
            {
                m_collisionBoxes[j] = new BoundingBox[1]
                {
                    m_blockMeshesByVariant[j].CalculateBoundingBox()
                };
            }
            base.Initialize();
        }
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            //Debugger.Break();
            int data = Terrain.ExtractData(value);
            int num = SubsystemTorcherinoBlockBehavior.ExtractFaceData(data);
            int strength = SubsystemTorcherinoBlockBehavior.ExtractStrengthMode(data);
            Color color = Color.LightBlue;
            switch (strength)
            {
                case 0:
                    color = Color.Gray;
                    break;
                case 1:
                    color = Color.LightBlue;
                    break;
                case 2:
                    color = Color.DarkYellow;
                    break;
                case 3:
                    color = Color.Yellow;
                    break;
            }
            if (num < m_blockMeshesByVariant.Length)
            {
                generator.GenerateMeshVertices(this, x, y, z, m_blockMeshesByVariant[num], color, null, geometry.SubsetOpaque);
            }
        }

        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            int value2 = 0;
            if (raycastResult.CellFace.Face == 0)
            {
                value2 = Terrain.ReplaceData(Terrain.ReplaceContents(905), 0);
            }
            if (raycastResult.CellFace.Face == 1)
            {
                value2 = Terrain.ReplaceData(Terrain.ReplaceContents(905), 1);
            }
            if (raycastResult.CellFace.Face == 2)
            {
                value2 = Terrain.ReplaceData(Terrain.ReplaceContents(905), 2);
            }
            if (raycastResult.CellFace.Face == 3)
            {
                value2 = Terrain.ReplaceData(Terrain.ReplaceContents(905), 3);
            }
            if (raycastResult.CellFace.Face == 4)
            {
                value2 = Terrain.ReplaceData(Terrain.ReplaceContents(905), 4);
            }
            BlockPlacementData result = default;
            result.Value = value2;
            result.CellFace = raycastResult.CellFace;
            return result;
        }

        public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
        {
            int num = SubsystemTorcherinoBlockBehavior.ExtractFaceData(Terrain.ExtractData(value));
            if (num < m_collisionBoxes.Length)
            {
                return m_collisionBoxes[num];
            }
            return null;
        }
        public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
        {
            var craftingRecipe = new CraftingRecipe
            {
                ResultCount = 1,
                //ResultValue = Terrain.MakeBlockValue(182,0,SetColor(0,color)),
                ResultValue = BlocksManager.GetBlockIndex<Torcherino>(),
                RemainsCount = 0,
                //RemainsValue = Terrain.MakeBlockValue(90),
                RequiredHeatLevel = 0f,
                Description = "加速火把，对3x3x3范围内植物生长和熔炉烧炼有效"
            };
            craftingRecipe.Ingredients[0] = "copperingot";
            craftingRecipe.Ingredients[1] = "diamond";
            craftingRecipe.Ingredients[2] = "copperingot";

            craftingRecipe.Ingredients[3] = "ironblock";
            craftingRecipe.Ingredients[4] = "torch";
            craftingRecipe.Ingredients[5] = "ironblock";

            craftingRecipe.Ingredients[6] = "ironblock";
            craftingRecipe.Ingredients[7] = "ironblock";
            craftingRecipe.Ingredients[8] = "ironblock";
            //craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
            yield return craftingRecipe;
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.LightBlue, 2f * size, ref matrix, environmentData);
        }
    }
}
