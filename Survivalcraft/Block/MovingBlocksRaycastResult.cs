using Engine;
using TemplatesDatabase;

namespace Game
{
	public struct MovingBlocksRaycastResult
	{
		public MovingBlocksRaycastResult() { }

		public Ray3 Ray;

		public IMovingBlockSet MovingBlockSet;

		public float Distance;
		public Vector3 HitPoint()
		{
			return Ray.Position + (Ray.Direction * Distance);
		}

		public MovingBlock MovingBlock;

		public int CollisionBoxIndex;

		public BoundingBox? BlockBoundingBox;

		public int BlockValue
		{
			get
			{
				return MovingBlock?.Value ?? -1;
			}
		}
		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
