using Engine;
using TemplatesDatabase;

namespace Game
{
	public struct TouchInput
	{
		public TouchInput() { }
		public TouchInputType InputType;

		public Vector2 Position;

		public Vector2 Move;

		public Vector2 TotalMove;

		public Vector2 TotalMoveLimited;

		public float Duration;

		public int DurationFrames;
		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
