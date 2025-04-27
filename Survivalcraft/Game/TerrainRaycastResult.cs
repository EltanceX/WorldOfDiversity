using Engine;
using TemplatesDatabase;

namespace Game
{
	public struct TerrainRaycastResult
	{
		public TerrainRaycastResult() { }

		public Ray3 Ray;

		public int Value;

		public CellFace CellFace;

		public int CollisionBoxIndex;

		public float Distance;

		public Vector3 HitPoint(float offsetFromSurface = 0f)
		{
			return Ray.Position + (Ray.Direction * Distance) + (CellFace.FaceToVector3(CellFace.Face) * offsetFromSurface);
		}

		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
