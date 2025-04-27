using TemplatesDatabase;
namespace Game
{
	public struct BlockDropValue
	{
		public BlockDropValue() { }
		public BlockDropValue(int value, int count)
		{
			Value = value;
			Count = count;
		}
		public int Value;

		public int Count;

		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
