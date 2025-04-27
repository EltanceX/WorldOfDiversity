using System.Diagnostics;

namespace WFC
{
    public class WaveFunctionCollapse
    {
        public Tile[] Tiles;
        public Dictionary<int, Tile> TileIdMap = new(); //id tile

        public int Width;
        public int Height;
        public int[,] Wave;
        public int[,] Result;
        public Random random;
        public WaveFunctionCollapse(Tile[] tiles, int width = 20, int height = 20, int? seed = null)
        {
            this.Tiles = tiles;
            this.Width = width;
            this.Height = height;
            random = seed.HasValue ? new Random(seed.Value) : new Random();

            for (int i = 0; i < this.Tiles.Length; i++)
            {
                TileIdMap.Add(this.Tiles[i].Id, this.Tiles[i]);
            }

            InitializeWave();
        }

        public void InitializeWave()
        {
            Wave = new int[Width, Height];
            Result = new int[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Result[i, j] = -1;
                }
            }
        }
        public bool WithIn((int x, int z) pos)
        {
            if (pos.x >= Height || pos.x < 0 || pos.z >= Width || pos.z < 0) return false;
            return true;
        }
        public bool HasResult((int x, int z) pos)
        {
            if (!WithIn(pos)) return false;
            int value = Result[pos.x, pos.z];
            if (value == -1) return false;
            return true;
        }
        public static (int x, int z) DirectionToPos(TileDirection direction)
        {
            return direction switch
            {
                TileDirection.Up => (0, 1),
                TileDirection.Down => (0, -1),
                TileDirection.Right => (1, 0),
                TileDirection.Left => (-1, 0),
                _ => (0, 0),
            };
        }
        public void Print()
        {
            string[] str = new string[Height * 3];
            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int ResId = Result[x, z];
                    string a = "";
                    string b = "";
                    string c = "";


                    if (ResId == -1000)
                    {
                        a = "???";
                        b = "???";
                        c = "???";
                    }
                    else
                    {
                        var tile = GetTileById(ResId);
                        if (tile == null)
                        {
                            a = "***";
                            b = "***";
                            c = "***";
                        }
                        else
                        {
                            a = $" {(tile.AvailableFace.Contains(TileDirection.Up) ? "." : " ")} ";
                            b = $"{(tile.AvailableFace.Contains(TileDirection.Left) ? "." : " ")}.{(tile.AvailableFace.Contains(TileDirection.Right) ? "." : " ")}";
                            c = $" {(tile.AvailableFace.Contains(TileDirection.Down) ? "." : " ")} ";
                        }
                    }
                    str[str.Length - 1 - (z * 3 + 2)] += a + "|  ";
                    str[str.Length - 1 - (z * 3 + 1)] += b + "|  ";
                    str[str.Length - 1 - (z * 3 + 0)] += c + "|  ";
                }
                str[str.Length - 1 - (z * 3 + 0)] += "\n";
            }
            foreach (var x in str)
            {
                Console.WriteLine(x);
            }
        }

        public Tile? GetTileByPos((int x, int z) pos)
        {
            int ResId = Result[pos.x, pos.z];
            if (ResId == -1 || ResId < 0) return null;
            return GetTileById(ResId);
        }
        public Tile? GetTileById(int id)
        {
            if (!TileIdMap.ContainsKey(id)) return null;
            return TileIdMap[id];
        }
        public int GetTileIdByIndex(int index)
        {
            if (index < 0 || index >= Tiles.Length) return -1;
            return Tiles[index].Id;
        }
        public static float CalculateEntropy(List<Tile> tiles)
        {
            float entropy = 0;
            float weights = 0;
            foreach (var tile in tiles)
            {
                weights += tile.Weight;
            }
            foreach (var tile in tiles)
            {
                float probability = tile.Weight / weights;
                entropy += -1 * (probability * MathF.Log2(probability));
            }
            return entropy;
        }
        public void Begin()
        {
            List<(float e, int x, int z)> EntropyList = new();
            //Queue<(int x, int z)> queue = new();
            (int x, int z) StartPoint = (random.Next(Width), random.Next(Height));
            //queue.Enqueue(StartPoint);
            EntropyList.Add((1, StartPoint.x, StartPoint.z));
            Result[StartPoint.x, StartPoint.z] = GetTileIdByIndex(random.Next(Tiles.Length));


            //Print();
            //熵低优先 (递归，列表排序)
            //while (queue.Count > 0)
            while (EntropyList.Count > 0)
            {
                //var pos = queue.Dequeue();
                if (EntropyList.Count > 1) EntropyList.Sort((a, b) => a.e.CompareTo(b.e));
                var pos = EntropyList[0];
                EntropyList.RemoveAt(0);

                var CurrentTile = GetTileByPos((pos.x, pos.z));
                if (CurrentTile == null) continue;
                foreach (TileDirection direction in Enum.GetValues<TileDirection>())
                {
                    (int x, int z) = DirectionToPos(direction);
                    (int x, int z) PosWithDirection = (pos.x + x, pos.z + z);
                    if (WithIn(PosWithDirection) && !HasResult(PosWithDirection))
                    {
                        List<Tile> tiles = [];
                        if (CurrentTile.RestrictedID)
                        {
                            if (CurrentTile.NeighboursID.ContainsKey(direction)) tiles = GetTiles(CurrentTile.NeighboursID[direction]);
                        }
                        else tiles = GetAvailableTiles(CurrentTile, PosWithDirection);
                        Result[PosWithDirection.x, PosWithDirection.z] = tiles.Count != 0 ? tiles[random.Next(tiles.Count)].Id : -1000;
                        float entropy = CalculateEntropy(tiles);
                        //queue.Enqueue(PosWithDirection);
                        EntropyList.Add((entropy, PosWithDirection.x, PosWithDirection.z));
                    }
                }
                Console.WriteLine("----------------------------------------------------");
                //Print();
            }
            //Debugger.Break();
        }
        public static void ConditionalRemoveNot(List<Tile> tiles, TileDirection direction)
        {
            var ToRemove = tiles.Where(x => !x.AvailableFace.Contains(direction)).ToList();
            foreach (var x in ToRemove)
            {
                tiles.Remove(x);
            }
        }
        public static void ConditionalRemove(List<Tile> tiles, TileDirection direction)
        {
            var ToRemove = tiles.Where(x => x.AvailableFace.Contains(direction)).ToList();
            foreach (var x in ToRemove)
            {
                tiles.Remove(x);
            }
        }
        public List<Tile> GetTiles(List<int> ids)
        {
            var result = new List<Tile>();

            foreach (var id in ids)
            {
                if (TileIdMap.ContainsKey(id)) result.Add(TileIdMap[id]);
            }

            return result;
        }
        public static List<Tile> CopyArrayToList(Tile[] tiles)
        {
            List<Tile> tiles1 = new List<Tile>();
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles1.Add(tiles[i]);
            }
            return tiles1;
        }
        public List<Tile> GetAvailableTiles(Tile lastTile, (int x, int z) pos)
        {

            List<Tile> tiles = null;
            tiles = CopyArrayToList(Tiles);

            Dictionary<TileDirection, bool> ToDetect = new() {//是否不超出边界
                {TileDirection.Up, !(pos.z + 1 >= Height) }, //if (pos.z + 1 >= Height) Up = false;
                {TileDirection.Down, !(pos.z - 1 < 0) }, //if (pos.z - 1 < 0) Down = false;
                {TileDirection.Right, !(pos.x + 1 >= Width) },//if (pos.x + 1 >= Width) Right = false;
                {TileDirection.Left, !(pos.x - 1 < 0) },//if (pos.x - 1 < 0) Left = false;
            };

            foreach (TileDirection direction in Enum.GetValues<TileDirection>())
            {
                if (!ToDetect[direction]) continue;

                (int x, int z) = DirectionToPos(direction);
                int TileID = Result[pos.x + x, pos.z + z];
                if (TileID >= 0)
                {
                    //Tile tile = Tiles[TileID];
                    Tile tile = GetTileById(TileID);
                    if (tile == null) continue;
                    //若下面的瓦片含向上通道，则当前检测瓦片必须含向下通道与之链接
                    bool hasUp = tile.AvailableFace.Contains(Tile.Reverse(direction));
                    if (hasUp)
                    {
                        ConditionalRemoveNot(tiles, direction);
                    }
                    //若下面的瓦片不含向上通道，则当前检测瓦片不能含向下的通道
                    else ToDetect[direction] = false;
                }
            }


            //若向上为空，则清除含向上通道的瓦片
            if (!ToDetect[TileDirection.Up]) ConditionalRemove(tiles, TileDirection.Up);
            if (!ToDetect[TileDirection.Down]) ConditionalRemove(tiles, TileDirection.Down);
            if (!ToDetect[TileDirection.Right]) ConditionalRemove(tiles, TileDirection.Right);
            if (!ToDetect[TileDirection.Left]) ConditionalRemove(tiles, TileDirection.Left);
            return tiles;
        }


    }
}
