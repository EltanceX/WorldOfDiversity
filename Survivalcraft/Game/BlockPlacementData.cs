using TemplatesDatabase;
namespace Game
{
	public struct BlockPlacementData
	{
		public BlockPlacementData() { }
		public int Value;

		public CellFace CellFace;

		/// <summary>
		/// ģ�������Ҫ��ӻ�ʹ�ö�����Ϣ�����������ValuesDictionary��дԪ��
		/// </summary>
		public ValuesDictionary ValuesDictionaryForMods = new ValuesDictionary();
	}
}
