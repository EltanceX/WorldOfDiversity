using Engine;
using Engine.Graphics;

namespace Game
{
	public class DiamondChunkBlock : Block
	{
		public static int Index = 111;

		public BlockMesh m_standaloneBlockMesh = new();

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Diamond");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Diamond").ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Diamond").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, 0f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
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
