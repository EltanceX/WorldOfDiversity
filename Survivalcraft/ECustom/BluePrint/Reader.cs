using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace BluePrint
{
    public class BluePrintReader
    {
        public BluePrintData Data;
        public MemoryStream Cache;
        public bool UseCache = false;

        public Func<Stream>? StreamFactory;
        public FileStream TEMP_FileStream;
        public Stream RawDataStream; //To fileStream or Custome Stream
        public DeflateStream? TEMP_DeflateReadStream;

        public BinaryReader BpBinaryReader;


        public Action<UInt16>? OnPlaceBlock_BlockID;
        public Action<string>? OnPlaceBlock_CraftingID;

        public Action<Int32>? OnSetBlockData;
        public Action<Int32>? OnSetBlockValue;
        public Action<SByte>? OnSetBlockLight;

        public Action<string, Int32>? OnPlaceBlockWithData_CraftingID;
        public Action<UInt16, Int32>? OnPlaceBlockWithData_BlockID;
        public Action<UInt16, Int32>? OnSetBlockWithData_BlockID;
        public Action<Int32>? OnPlaceBlockWithValue;

        public BluePrintReader(BluePrintData data, bool useCache = false)
        {
            this.UseCache = useCache;
            //if (useCache) Cache = new MemoryStream();
            if (data == null) throw new Exception("BluePrintData data Cannot be null!");
            Data = data;
            this.TEMP_FileStream = Data.SBPFileStream;
        }
        public virtual void Dispose()
        {
            if (UseCache)
            {
                Cache?.Dispose();
                Cache = null;
            }
            BpBinaryReader?.Dispose();
            RawDataStream?.Dispose();
            BpBinaryReader = null;
            RawDataStream = null;
        }
        public virtual void PrepareForReading()
        {
            if (TEMP_FileStream != null) RawDataStream = TEMP_FileStream;
            else if (StreamFactory != null) RawDataStream = StreamFactory.Invoke();
            //TEMP_FileStream = null;

            //if (UseCache)
            //{
            //    RawDataStream.CopyTo(Cache);
            //    RawDataStream.Close();
            //    RawDataStream = Cache;
            //    Cache = null;
            //    RawDataStream.Position = 0;
            //}

            if (RawDataStream == null) throw new Exception("Stream Cannot Be Null!");
        }
        public virtual void ReadToEnd()
        {
            try
            {
                while (true)
                {
                    EBluePrint id = ReadID();
                    ProcessID(id);
                    if (id == EBluePrint.End) break;

                    if (UseCache)
                    {
                        if (Cache == null) break;
                        if (!Cache.CanRead) break;
                        if (BpBinaryReader.PeekChar() == -1) break;
                        continue;
                    }
                    if (RawDataStream == null) break;
                    if (!RawDataStream.CanRead) break;
                    if (Data.FormatVersion == 0 && BpBinaryReader.PeekChar() == -1) break;
                }
            }
            catch (Exception ex)
            {
                if (ex is EndOfStreamException) End();
                else throw;
            }
        }


        //仅调用一次!
        public virtual void ReadFileHeader()
        {
            BinaryReader HeaderReader = new BinaryReader(RawDataStream, Encoding.UTF8, leaveOpen: true);
            byte[] bytes = HeaderReader.ReadBytes(BluePrintManager.Identifier.Length);
            string header = Encoding.UTF8.GetString(bytes);
            string targetHeader = new string(BluePrintManager.Identifier);
            if (header != targetHeader) throw new Exception("Unknown Header: " + header.ToString());
            byte compressionMode = HeaderReader.ReadByte();
            HeaderReader.Dispose();

            switch ((BlueprintCompressionMode)compressionMode)
            {
                case BlueprintCompressionMode.None:
                    if (UseCache)
                    {
                        Cache = new MemoryStream();
                        RawDataStream.CopyTo(Cache);
                        Cache.Position = 0;
                        BpBinaryReader = new BinaryReader(Cache, Encoding.UTF8, leaveOpen: true);
                    }
                    else BpBinaryReader = new BinaryReader(RawDataStream, Encoding.UTF8);
                    break;
                case BlueprintCompressionMode.Deflate:
                    var deflate = new DeflateStream(RawDataStream, CompressionMode.Decompress);
                    TEMP_DeflateReadStream = deflate;
                    if (UseCache)
                    {
                        Cache = new MemoryStream();
                        deflate.CopyTo(Cache);
                        deflate.Close();
                        deflate = null;
                        TEMP_DeflateReadStream = null;
                        Cache.Position = 0;
                        BpBinaryReader = new BinaryReader(Cache, Encoding.UTF8, leaveOpen: true);
                    }
                    else BpBinaryReader = new BinaryReader(deflate, Encoding.UTF8);
                    break;
                default:
                    throw new Exception("Unsupported Compression Mode: " + compressionMode);
            }
        }

        public virtual void ReadFormatHeader()
        {
            while (true)
            {
                EBluePrint id = ReadID();
                if (id != EBluePrint.Header && id != EBluePrint.Version && id != EBluePrint.Width && id != EBluePrint.Height && id != EBluePrint.Depth) return;
                ProcessID(id);
            }
        }

        public virtual void CloseStream()
        {
            BpBinaryReader.Close();
            TEMP_DeflateReadStream?.Close();
            Cache?.Close();
        }
        public virtual void EndReading()
        {
            if (UseCache)
            {
                //RawDataStream.Position = 0;
                Cache.Position = 0;
                Data.Clear();
            }
            else CloseStream();

        }


        public virtual EBluePrint ReadID()
        {
            Byte id = BpBinaryReader.ReadByte();
            EBluePrint command = (EBluePrint)id;
            return command;
        }
        public virtual void ProcessID(EBluePrint command)
        {
            switch (command)
            {
                case EBluePrint.Header:
                    Header();
                    break;
                case EBluePrint.Version:
                    ReadVersion();
                    break;
                case EBluePrint.Width:
                    ReadWidth();
                    break;
                case EBluePrint.Height:
                    ReadHeight();
                    break;
                case EBluePrint.Depth:
                    ReadDepth();
                    break;

                case EBluePrint.Body:
                    ReadBody();
                    break;
                case EBluePrint.AddBlockPattle:
                    AddBlockPattle();
                    break;
                case EBluePrint.SetWorldOriginX:
                    SetWorldOriginX();
                    break;
                case EBluePrint.SetWorldOriginY:
                    SetWorldOriginY();
                    break;
                case EBluePrint.SetWorldOriginZ:
                    SetWorldOriginZ();
                    break;
                case EBluePrint.XPP:
                    XPP();
                    break;
                case EBluePrint.YPP:
                    YPP();
                    break;
                case EBluePrint.ZPP:
                    ZPP();
                    break;
                case EBluePrint.AddX:
                    AddX();
                    break;
                case EBluePrint.AddY:
                    AddY();
                    break;
                case EBluePrint.AddZ:
                    AddZ();
                    break;
                case EBluePrint.AddSmallX:
                    AddSmallX();
                    break;
                case EBluePrint.AddSmallY:
                    AddSmallY();
                    break;
                case EBluePrint.AddSmallZ:
                    AddSmallZ();
                    break;
                case EBluePrint.AddBigX:
                    AddBigX();
                    break;
                case EBluePrint.AddBigY:
                    AddBigY();
                    break;
                case EBluePrint.AddBigZ:
                    AddBigZ();
                    break;

                case EBluePrint.SetX:
                    SetX();
                    break;
                case EBluePrint.SetY:
                    SetY();
                    break;
                case EBluePrint.SetZ:
                    SetZ();
                    break;
                case EBluePrint.SetSmallX:
                    SetSmallX();
                    break;
                case EBluePrint.SetSmallY:
                    SetSmallY();
                    break;
                case EBluePrint.SetSmallZ:
                    SetSmallZ();
                    break;
                case EBluePrint.SetBigX:
                    SetBigX();
                    break;
                case EBluePrint.SetBigY:
                    SetBigY();
                    break;
                case EBluePrint.SetBigZ:
                    SetBigZ();
                    break;

                case EBluePrint.PlaceBlock_CraftingID:
                    PlaceBlock_CraftingID();
                    break;
                case EBluePrint.PlaceBlock_BlockID:
                    PlaceBlock_BlockID();
                    break;
                case EBluePrint.SetBlockData:
                    SetBlockData();
                    break;
                case EBluePrint.SetBlockValue:
                    SetBlockValue();
                    break;
                case EBluePrint.SetBlockLight:
                    SetBlockLight();
                    break;
                case EBluePrint.PlaceBlockWithData_CraftingID:
                    PlaceBlockWithData_CraftingID();
                    break;
                case EBluePrint.PlaceBlockWithData_BlockID:
                    PlaceBlockWithData_BlockID();
                    break;
                case EBluePrint.PlaceBlockWithValue:
                    PlaceBlockWithValue();
                    break;
                case EBluePrint.BlockLabel:
                    ReadBlockLabel();
                    break;
                case EBluePrint.BlockLabelLarge:
                    ReadBlockLabelLarge();
                    break;
                case EBluePrint.None:
                    None();
                    break;
                case EBluePrint.End:
                    End();
                    break;

                default:
                    throw new NotImplementedException("Unknown Function ID: " + command);
            }
        }
        public virtual string GetCraftingIdInPattle(UInt16 PattleIndex)
        {
            if (PattleIndex >= Data.BlockPattle.Count) throw new Exception($"[BluePrintData.BlockPattle] Index out of bounds! {PattleIndex}/{Data.BlockPattle.Count}");
            string CraftingID = Data.BlockPattle[PattleIndex];
            return CraftingID;
        }





















        public void Header()
        {

        }
        public Byte ReadVersion()
        {
            Byte version = BpBinaryReader.ReadByte();
            Data.FormatVersion = version;
            return version;
        }

        public UInt32 ReadWidth()
        {
            UInt32 length = BpBinaryReader.ReadUInt32();
            Data.Width = length;
            return length;
        }
        public UInt32 ReadHeight()
        {
            UInt32 height = BpBinaryReader.ReadUInt32();
            Data.Height = height;
            return height;
        }
        public UInt32 ReadDepth()
        {
            UInt32 width = BpBinaryReader.ReadUInt32();
            Data.Depth = width;
            return width;
        }


        public void ReadBody()
        {

        }
        public string AddBlockPattle()
        {
            SByte length = BpBinaryReader.ReadSByte();
            byte[] bytes = BpBinaryReader.ReadBytes(length);
            string CraftingID = Encoding.UTF8.GetString(bytes);
            if (Data.BlockPattle.Contains(CraftingID)) throw new Exception($"Repeated CraftingID {CraftingID}!");
            Data.BlockPattle.Add(CraftingID);
            return CraftingID;
        }
        public Int32 SetWorldOriginX()
        {
            Int32 x = BpBinaryReader.ReadInt32();
            Data.WorldOrigin[0] = x;
            return x;
        }
        public Int32 SetWorldOriginY()
        {
            Int32 y = BpBinaryReader.ReadInt32();
            Data.WorldOrigin[1] = y;
            return y;
        }
        public Int32 SetWorldOriginZ()
        {
            Int32 z = BpBinaryReader.ReadInt32();
            Data.WorldOrigin[2] = z;
            return z;
        }

        #region Add Position
        public void XPP()
        {
            Data.X++;
        }
        public void YPP()
        {
            Data.Y++;
        }
        public void ZPP()
        {
            Data.Z++;
        }
        public Int16 AddX()
        {
            Int16 x = BpBinaryReader.ReadInt16();
            Data.X += x;
            return x;
        }
        public Int16 AddY()
        {
            Int16 y = BpBinaryReader.ReadInt16();
            Data.Y += y;
            return y;
        }
        public Int16 AddZ()
        {
            Int16 z = BpBinaryReader.ReadInt16();
            Data.Z += z;
            return z;
        }
        public SByte AddSmallX()
        {
            SByte x = BpBinaryReader.ReadSByte();
            Data.X += x;
            return x;
        }
        public SByte AddSmallY()
        {
            SByte y = BpBinaryReader.ReadSByte();
            Data.Y += y;
            return y;
        }
        public SByte AddSmallZ()
        {
            SByte z = BpBinaryReader.ReadSByte();
            Data.Z += z;
            return z;
        }
        public Int32 AddBigX()
        {
            Int32 x = BpBinaryReader.ReadInt32();
            Data.X += x;
            return x;
        }
        public Int32 AddBigY()
        {
            Int32 y = BpBinaryReader.ReadInt32();
            Data.Y += y;
            return y;
        }
        public Int32 AddBigZ()
        {
            Int32 z = BpBinaryReader.ReadInt32();
            Data.Z += z;
            return z;
        }
        #endregion


        #region Set Position
        public Int16 SetX()
        {
            Int16 x = BpBinaryReader.ReadInt16();
            Data.X = x;
            return x;
        }
        public Int16 SetY()
        {
            Int16 y = BpBinaryReader.ReadInt16();
            Data.Y = y;
            return y;
        }
        public Int16 SetZ()
        {
            Int16 z = BpBinaryReader.ReadInt16();
            Data.Z = z;
            return z;
        }
        public Byte SetSmallX()
        {
            Byte x = BpBinaryReader.ReadByte();
            Data.X = x;
            return x;
        }
        public Byte SetSmallY()
        {
            Byte y = BpBinaryReader.ReadByte();
            Data.Y = y;
            return y;
        }
        public Byte SetSmallZ()
        {
            Byte z = BpBinaryReader.ReadByte();
            Data.Z = z;
            return z;
        }
        public Int32 SetBigX()
        {
            Int32 x = BpBinaryReader.ReadInt32();
            Data.X = x;
            return x;
        }
        public Int32 SetBigY()
        {
            Int32 y = BpBinaryReader.ReadInt32();
            Data.Y = y;
            return y;
        }
        public Int32 SetBigZ()
        {
            Int32 z = BpBinaryReader.ReadInt32();
            Data.Z = z;
            return z;
        }
        #endregion


        #region Block Operation
        public UInt16 PlaceBlock_CraftingID()
        {
            UInt16 PattleIndex = BpBinaryReader.ReadUInt16();
            string CraftingID = GetCraftingIdInPattle(PattleIndex);
            OnPlaceBlock_CraftingID?.Invoke(CraftingID);
            return PattleIndex;
        }
        public UInt16 PlaceBlock_BlockID()
        {
            UInt16 BlockID = BpBinaryReader.ReadUInt16();
            OnPlaceBlock_BlockID?.Invoke(BlockID);
            return BlockID;
        }
        public Int32 SetBlockData()
        {
            Int32 BlockData = BpBinaryReader.ReadInt32();
            OnSetBlockData?.Invoke(BlockData);
            return BlockData;
        }
        public Int32 SetBlockValue()
        {
            Int32 Value = BpBinaryReader.ReadInt32();
            OnSetBlockValue?.Invoke(Value);
            return Value;
        }
        public SByte SetBlockLight()
        {
            SByte light = BpBinaryReader.ReadSByte();
            OnSetBlockLight?.Invoke(light);
            return light;
        }
        public (UInt16, Int32) PlaceBlockWithData_CraftingID()
        {
            UInt16 BlockPattleIndex = BpBinaryReader.ReadUInt16();
            Int32 BlockData = BpBinaryReader.ReadInt32();
            string CraftingID = GetCraftingIdInPattle(BlockPattleIndex);
            OnPlaceBlockWithData_CraftingID?.Invoke(CraftingID, BlockData);
            return (BlockPattleIndex, BlockData);
        }
        public (UInt16, Int32) PlaceBlockWithData_BlockID()
        {
            UInt16 BlockID = BpBinaryReader.ReadUInt16();
            Int32 BlockData = BpBinaryReader.ReadInt32();
            OnPlaceBlockWithData_BlockID?.Invoke(BlockID, BlockData);
            return (BlockID, BlockData);
        }
        public Int32 PlaceBlockWithValue()
        {
            Int32 Value = BpBinaryReader.ReadInt32();
            OnPlaceBlockWithValue?.Invoke(Value);
            return Value;
        }

        public string ReadBlockLabel()
        {
            UInt16 length = BpBinaryReader.ReadUInt16();
            byte[] bytes = BpBinaryReader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
        public string ReadBlockLabelLarge()
        {
            UInt32 length = BpBinaryReader.ReadUInt32();
            byte[] bytes = BpBinaryReader.ReadBytes((int)length);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion


        public void None()
        {

        }
        public void End()
        {
            EndReading();
        }
    }
}
