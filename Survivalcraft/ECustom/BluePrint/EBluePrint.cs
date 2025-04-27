namespace BluePrint
{
    public enum BlueprintCompressionMode
    {
        None = 0,
        Deflate = 1
    }
    public enum EBluePrint
    {
        Header = 0x01,
        Version = 0x02,
        Width = 0x04,
        Height = 0x05,
        Depth = 0x06,



        Body = 0x10,
        AddBlockPattle = 0x11,

        SetWorldOriginX = 0x15,
        SetWorldOriginY = 0x16,
        SetWorldOriginZ = 0x17,
        XPP = 0x18,
        YPP = 0x19,
        ZPP = 0x1A,
        AddX = 0x1B,
        AddY = 0x1C,
        AddZ = 0x1D,
        AddSmallX = 0x1E,
        AddSmallY = 0x1F,
        AddSmallZ = 0x20,
        AddBigX = 0x21,
        AddBigY = 0x22,
        AddBigZ = 0x23,

        SetX = 0x27,
        SetY = 0x28,
        SetZ = 0x29,
        SetSmallX = 0x2A,
        SetSmallY = 0x2B,
        SetSmallZ = 0x2C,
        SetBigX = 0x2D,
        SetBigY = 0x2E,
        SetBigZ = 0x2F,

        PlaceBlock_CraftingID = 0x40,
        PlaceBlock_BlockID = 0x41,
        SetBlockData = 0x42,
        SetBlockValue = 0x43,
        SetBlockLight = 0x44,
        PlaceBlockWithData_CraftingID = 0x45,
        PlaceBlockWithData_BlockID = 0x46,
        PlaceBlockWithValue = 0x47,

        BlockLabel = 0x4A,
        BlockLabelLarge = 0x4B,



        None = 0xEE,
        End = 0xFF
    }
}
