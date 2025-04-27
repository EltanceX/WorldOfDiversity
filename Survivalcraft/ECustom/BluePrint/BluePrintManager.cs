using System.IO.Compression;
using System.Text;

namespace BluePrint
{
    public class BluePrintData
    {
        public int[] WorldOrigin = new int[3];
        public uint Width = 0;
        public uint Height = 0;
        public uint Depth = 0;
        public int X = 0;
        public int Y = 0;
        public int Z = 0;
        public byte FormatVersion = 0;
        public List<string> BlockPattle = new List<string>();
        public FileStream SBPFileStream;
        public BluePrintData(FileStream fs)
        {
            SBPFileStream = fs;
        }
        public void Clear()
        {
            WorldOrigin = new int[3];
            Width = 0;
            Height = 0;
            Depth = 0;
            X = 0;
            Y = 0;
            Z = 0;
            FormatVersion = 0;
            BlockPattle = new List<string>();
            SBPFileStream?.Dispose();
            SBPFileStream = null;
        }
        public virtual void Dispose()
        {
            SBPFileStream?.Dispose();
            SBPFileStream = null;
            BlockPattle?.Clear();
            BlockPattle = null;
        }
    }
    
    public class BluePrintManager
    {
        public static char[] Identifier = ['S', 'C', 'B', 'P', 'F'];
        public static Dictionary<int, string> CraftingIdTable = new Dictionary<int, string>();
        public static Dictionary<string, int> CraftingIdTableReverse = new Dictionary<string, int>();
    }
}
