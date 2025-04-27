using Engine;
using TemplatesDatabase;

namespace Game
{
	public struct FluidInfo
	{
		public FluidInfo() { }
		public FluidBlock Block;

		public float SurfaceHeight;

		public Vector2 FlowSpeed;

		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
