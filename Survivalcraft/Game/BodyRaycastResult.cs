using Engine;
using TemplatesDatabase;

namespace Game
{
	public struct BodyRaycastResult
	{
		public BodyRaycastResult() { }
		public Ray3 Ray;

		public ComponentBody ComponentBody;

		public float Distance;

		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
		public Vector3 HitPoint()
		{
			return Ray.Sample(Distance);
		}
	}
}
