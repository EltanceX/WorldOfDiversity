using TemplatesDatabase;
namespace Game
{
	public struct ScannedItemData
	{
		public ScannedItemData() { }
		public object Container;

		public int IndexInContainer;

		public int Value;

		public int Count;
		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
