using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class MulticoloredLedBlock : MountedElectricElementBlock
	{
		public static int Index = 254;

		public BlockMesh m_standaloneBlockMesh;

		public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Leds");
			ModelMesh modelMesh = model.FindMesh("Led");
			ModelMesh modelMesh2 = model.FindMesh("LedBulb");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(modelMesh2.ParentBone);
			Matrix m = Matrix.CreateRotationY(-(float)Math.PI / 2f) * Matrix.CreateRotationZ((float)Math.PI / 2f);
			m_standaloneBlockMesh = new BlockMesh();
			m_standaloneBlockMesh.AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
			m_standaloneBlockMesh.AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, new Color(48, 48, 48));
			for (int i = 0; i < 6; i++)
			{
				int num = SetMountingFace(0, i);
				Matrix m2 = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX((float)Math.PI) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(i * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				m_blockMeshesByData[num] = new BlockMesh();
				m_blockMeshesByData[num].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m2, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
				m_blockMeshesByData[num].AppendModelMeshPart(modelMesh2.MeshParts[0], boneAbsoluteTransform2 * m2, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, new Color(48, 48, 48));
				m_collisionBoxesByData[num] = new BoundingBox[1]
				{
					m_blockMeshesByData[num].CalculateBoundingBox()
				};
			}
		}

		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			var craftingRecipe = new CraftingRecipe
			{
				ResultCount = 4,
				ResultValue = Terrain.MakeBlockValue(254, 0, 0),
				RequiredHeatLevel = 0f,
				Description = LanguageControl.Get(GetType().Name, 1)
			};
			craftingRecipe.Ingredients[1] = "glass";
			craftingRecipe.Ingredients[4] = "wire";
			craftingRecipe.Ingredients[6] = "copperingot";
			craftingRecipe.Ingredients[7] = "copperingot";
			craftingRecipe.Ingredients[8] = "copperingot";
			yield return craftingRecipe;
		}

		public override int GetFace(int value)
		{
			return GetMountingFace(Terrain.ExtractData(value));
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(254, 0, 0);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			BlockPlacementData result = default;
			result.Value = value2;
			result.CellFace = raycastResult.CellFace;
			return result;
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= m_collisionBoxesByData.Length)
			{
				return null;
			}
			return m_collisionBoxesByData[num];
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, GetFace(value), 0.25f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MulticoloredLedElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));
		}

		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace).HasValue)
			{
				return ElectricConnectorType.Input;
			}
			return null;
		}

		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}
	}
}
