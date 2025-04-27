using System.IO.Compression;
using System.Text;

namespace BluePrint;
public class BluePrintWriter
{
    public BluePrintData Data;
    public DeflateStream DeflateWriterStream;
    public BinaryWriter BpBinaryWriter;
    public bool WritingEnd = false;
    public BluePrintWriter(BluePrintData data)
    {
        this.Data = data;
    }
    public virtual void PrepareForWriting(uint width, uint height, uint depth, byte CompressMode = 0x01)
    {
        BinaryWriter HeaderWriter = new BinaryWriter(Data.SBPFileStream, Encoding.UTF8, leaveOpen: true);
        HeaderWriter.Write(BluePrintManager.Identifier);
        HeaderWriter.Write((byte)CompressMode);
        HeaderWriter.Dispose();
        if (CompressMode == 0x01)
        {
            var dws = new DeflateStream(Data.SBPFileStream, CompressionMode.Compress, leaveOpen: true);
            var dbw = new BinaryWriter(dws, Encoding.UTF8);
            DeflateWriterStream = dws;
            BpBinaryWriter = dbw;
        }
        else if (CompressMode == 0x00)
        {
            var dbw = new BinaryWriter(Data.SBPFileStream, Encoding.UTF8);
            DeflateWriterStream = null;
            BpBinaryWriter = dbw;
        }

        WriteHeaderIdentifier();
        WriteSBPVersion();
        WriteWidth(width);
        WriteHeight(height);
        WriteDepth(depth);
        WriteBodyIdentifier();
    }
    public virtual void EndWriting()
    {
        WritingEnd = true;
        BpBinaryWriter.Close();
        DeflateWriterStream?.Close();
        Data.SBPFileStream.Close();
        Data.SBPFileStream.Dispose();
    }
    public void WriteIdentifier(EBluePrint id)
    {
        BpBinaryWriter.Write((byte)id);
    }
    public void AutoCalcPosition(Int32 rx, Int32 ry, Int32 rz)
    {
        Int32 dx = rx - Data.X;
        Int32 dy = ry - Data.Y;
        Int32 dz = rz - Data.Z;
        AutoAddX(dx);
        AutoAddY(dy);
        AutoAddZ(dz);
    }
    public void AutoAddX(Int32 x)
    {
        if (x == 0) return;
        else if (x == 1) XPP();
        else if (x >= SByte.MinValue && x <= SByte.MaxValue) AddSmallX((SByte)x);
        else if (x >= Int16.MinValue && x <= Int16.MaxValue) AddX((Int16)x);
        else if (x >= Int32.MinValue && x <= Int32.MaxValue) AddBigX(x);
        else
        {
            AddBigX(Int32.MaxValue);
            AutoAddX(x - Int32.MaxValue);
        }
    }
    public void AutoAddY(Int32 y)
    {
        if (y == 0) return;
        else if (y == 1) YPP();
        else if (y >= SByte.MinValue && y <= SByte.MaxValue) AddSmallY((SByte)y);
        else if (y >= Int16.MinValue && y <= Int16.MaxValue) AddY((Int16)y);
        else if (y >= Int32.MinValue && y <= Int32.MaxValue) AddBigY(y);
        else
        {
            AddBigY(Int32.MaxValue);
            AutoAddY(y - Int32.MaxValue);
        }
    }
    public void AutoAddZ(Int32 z)
    {
        if (z == 0) return;
        else if (z == 1) ZPP();
        else if (z >= SByte.MinValue && z <= SByte.MaxValue) AddSmallZ((SByte)z);
        else if (z >= Int16.MinValue && z <= Int16.MaxValue) AddZ((Int16)z);
        else if (z >= Int32.MinValue && z <= Int32.MaxValue) AddBigZ(z);
        else
        {
            AddBigZ(Int32.MaxValue);
            AutoAddZ(z - Int32.MaxValue);
        }
    }
    public ushort GetIndexByCraftingID(string craftingID)
    {
        int index = Data.BlockPattle.IndexOf(craftingID);
        if (index == -1) throw new Exception($"Unknown CraftingID: {craftingID} at: {DeflateWriterStream?.Position}");
        if (index > ushort.MaxValue) throw new Exception($"uint16 Index:{index} overflow at:{DeflateWriterStream?.Position}");
        return (ushort)index;
    }


    #region Part Header
    public void WriteHeaderIdentifier()
    {
        WriteIdentifier(EBluePrint.Header);
        //DeflateBinaryWriter.Write((ushort)(sizeof(byte) + sizeof(byte) * 4 + sizeof(uint) * 3));
    }
    public void WriteSBPVersion()
    {
        WriteIdentifier(EBluePrint.Version);
        BpBinaryWriter.Write((byte)0x01);
    }
    public void WriteWidth(uint width)
    {
        WriteIdentifier(EBluePrint.Width);
        BpBinaryWriter.Write(width);
    }
    public void WriteHeight(uint height)
    {
        WriteIdentifier(EBluePrint.Height);
        BpBinaryWriter.Write(height);
    }
    public void WriteDepth(uint depth)
    {
        WriteIdentifier(EBluePrint.Depth);
        BpBinaryWriter.Write(depth);
    }
    #endregion


    public void WriteBodyIdentifier()
    {
        WriteIdentifier(EBluePrint.Body);
    }
    public void AddBlockPattle(string craftingID)
    {
        if (Data.BlockPattle.Contains(craftingID)) return;
        byte[] bytes = Encoding.UTF8.GetBytes(craftingID);
        if (bytes.Length > byte.MaxValue) throw new Exception($"byte number {bytes.Length} overflow, at: {DeflateWriterStream?.Position}");
        byte length = (byte)bytes.Length;
        Data.BlockPattle.Add(craftingID);
        WriteIdentifier(EBluePrint.AddBlockPattle);
        BpBinaryWriter.Write(length);
        BpBinaryWriter.Write(bytes);
    }

    #region Part Add Position
    public void SetWorldOriginX(int x)
    {
        Data.WorldOrigin[0] = x;
        WriteIdentifier(EBluePrint.SetWorldOriginX);
        BpBinaryWriter.Write(x);
    }
    public void SetWorldOriginY(int y)
    {
        Data.WorldOrigin[1] = y;
        WriteIdentifier(EBluePrint.SetWorldOriginY);
        BpBinaryWriter.Write(y);
    }
    public void SetWorldOriginZ(int z)
    {
        Data.WorldOrigin[2] = z;
        WriteIdentifier(EBluePrint.SetWorldOriginZ);
        BpBinaryWriter.Write(z);
    }
    public void XPP()
    {
        Data.X++;
        WriteIdentifier(EBluePrint.XPP);
    }
    public void YPP()
    {
        Data.Y++;
        WriteIdentifier(EBluePrint.YPP);
    }
    public void ZPP()
    {
        Data.Z++;
        WriteIdentifier(EBluePrint.ZPP);
    }
    public void AddX(short x)
    {
        Data.X += x;
        WriteIdentifier(EBluePrint.AddX);
        BpBinaryWriter.Write(x);
    }
    public void AddY(short y)
    {
        Data.Y += y;
        WriteIdentifier(EBluePrint.AddY);
        BpBinaryWriter.Write(y);
    }
    public void AddZ(short z)
    {
        Data.Z += z;
        WriteIdentifier(EBluePrint.AddZ);
        BpBinaryWriter.Write(z);
    }
    public void AddSmallX(sbyte x)
    {
        Data.X += x;
        WriteIdentifier(EBluePrint.AddSmallX);
        BpBinaryWriter.Write(x);
    }
    public void AddSmallY(sbyte y)
    {
        Data.Y += y;
        WriteIdentifier(EBluePrint.AddSmallY);
        BpBinaryWriter.Write(y);
    }
    public void AddSmallZ(sbyte z)
    {
        Data.Z += z;
        WriteIdentifier(EBluePrint.AddSmallZ);
        BpBinaryWriter.Write(z);
    }
    public void AddBigX(int x)
    {
        Data.X += x;
        WriteIdentifier(EBluePrint.AddBigX);
        BpBinaryWriter.Write(x);
    }
    public void AddBigY(int y)
    {
        Data.Y += y;
        WriteIdentifier(EBluePrint.AddBigY);
        BpBinaryWriter.Write(y);
    }
    public void AddBigZ(int z)
    {
        Data.Z += z;
        WriteIdentifier(EBluePrint.AddBigZ);
        BpBinaryWriter.Write(z);
    }
    #endregion

    #region Part Set Position
    public void SetX(short x)
    {
        Data.X = x;
        WriteIdentifier(EBluePrint.SetX);
        BpBinaryWriter.Write(x);
    }
    public void SetY(short y)
    {
        Data.Y = y;
        WriteIdentifier(EBluePrint.SetY);
        BpBinaryWriter.Write(y);
    }
    public void SetZ(short z)
    {
        Data.Z = z;
        WriteIdentifier(EBluePrint.SetZ);
        BpBinaryWriter.Write(z);
    }
    public void SetSmallX(sbyte x)
    {
        Data.X = x;
        WriteIdentifier(EBluePrint.SetSmallX);
        BpBinaryWriter.Write(x);
    }
    public void SetSmallY(sbyte y)
    {
        Data.Y = y;
        WriteIdentifier(EBluePrint.SetSmallY);
        BpBinaryWriter.Write(y);
    }
    public void SetSmallZ(sbyte z)
    {
        Data.Z = z;
        WriteIdentifier(EBluePrint.SetSmallZ);
        BpBinaryWriter.Write(z);
    }
    public void SetBigX(int x)
    {
        Data.X = x;
        WriteIdentifier(EBluePrint.SetBigX);
        BpBinaryWriter.Write(x);
    }
    public void SetBigY(int y)
    {
        Data.Y = y;
        WriteIdentifier(EBluePrint.SetBigY);
        BpBinaryWriter.Write(y);
    }
    public void SetBigZ(int z)
    {
        Data.Z = z;
        WriteIdentifier(EBluePrint.SetBigZ);
        BpBinaryWriter.Write(z);
    }
    #endregion


    #region Part Set Block
    public void PlaceBlock(string craftingID)
    {
        ushort index = GetIndexByCraftingID(craftingID);
        WriteIdentifier(EBluePrint.PlaceBlock_CraftingID);
        BpBinaryWriter.Write((ushort)index);
    }
    public void PlaceBlock(ushort blockID)
    {
        WriteIdentifier(EBluePrint.PlaceBlock_BlockID);
        BpBinaryWriter.Write(blockID);
    }
    public void SetBlockData(int data)
    {
        WriteIdentifier(EBluePrint.SetBlockData);
        BpBinaryWriter.Write(data);
    }
    public void SetBlockValue(int value)
    {
        WriteIdentifier(EBluePrint.SetBlockValue);
        BpBinaryWriter.Write(value);
    }
    public void SetBlockLight(byte light)
    {
        WriteIdentifier(EBluePrint.SetBlockLight);
        BpBinaryWriter.Write(light);
    }
    public void PlaceBlockWithData(string craftingID, int data)
    {
        ushort index = GetIndexByCraftingID(craftingID);
        WriteIdentifier(EBluePrint.PlaceBlockWithData_CraftingID);
        BpBinaryWriter.Write(index);
        BpBinaryWriter.Write(data);
    }
    public void PlaceBlockWithData(ushort blockID, int data)
    {
        WriteIdentifier(EBluePrint.PlaceBlockWithData_BlockID);
        BpBinaryWriter.Write(blockID);
        BpBinaryWriter.Write(data);
    }
    public void PlaceBlockWithValue(int value)
    {
        WriteIdentifier(EBluePrint.PlaceBlockWithValue);
        BpBinaryWriter.Write(value);
    }
    #endregion

    public void WriteBlockLabel(string label)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(label);
        if (bytes.Length > ushort.MaxValue) throw new Exception($"ushort number {bytes.Length} overflow, at: {DeflateWriterStream?.Position}");
        ushort length = (ushort)bytes.Length;
        WriteIdentifier(EBluePrint.BlockLabel);
        BpBinaryWriter.Write(length);
        BpBinaryWriter.Write(bytes);
    }
    public void WriteBlockLabelLarge(string label)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(label);
        int length = bytes.Length;
        WriteIdentifier(EBluePrint.BlockLabelLarge);
        BpBinaryWriter.Write(length);
        BpBinaryWriter.Write(bytes);
    }


    public void None()
    {
        WriteIdentifier(EBluePrint.None);
    }
    public void End(bool InvokeEndWriting = true)
    {
        WriteIdentifier(EBluePrint.End);
        if (InvokeEndWriting) EndWriting();
    }
}