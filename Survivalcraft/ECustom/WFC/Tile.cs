using BluePrint;
using GlassMod;
using System.Numerics;

namespace WFC
{
    public class Tile
    {
        public int Id;
        public string Name;
        public float Weight = 1f;

        //public BluePrintReader Blueprint = null;
        public ScStructure structure = null;

        public float RotateDegree = 0f * MathF.PI; //弧度
        public Matrix4x4? Transform;
        //可相邻的瓦片ID列表
        //public Dictionary<TileDirection, List<int>> AdjacencyRules = new();
        public List<TileDirection> AvailableFace = new List<TileDirection>();
        public bool RestrictedID = false;
        public Dictionary<TileDirection, List<int>> NeighboursID = new();

        public bool CompleteBlockCollapse = false;




        public Tile(int id, string name, bool RestrictedID = false)
        {
            Id = id;
            Name = name;
            this.RestrictedID = RestrictedID;
        }
        public void UpdateTransform()
        {
            var Blueprint = structure.BpReader;
            Transform = GetTransform((int)Blueprint.Data.Width, (int)Blueprint.Data.Depth, RotateDegree);
        }
        public static Matrix4x4 GetTransform(int width, int depth, float degrees)
        {
            Vector3 center = new Vector3(width / 2f, 0, depth / 2f);
            //float radians = MathF.PI / 180f * degrees; // 顺时针为负角度
            float radians = degrees; // 顺时针为负角度

            // 步骤1：创建平移矩阵（将中心点移到原点）
            Matrix4x4 translateToOrigin = Matrix4x4.CreateTranslation(-center);

            // 步骤2：绕 Y 轴旋转
            Matrix4x4 rotationY = Matrix4x4.CreateRotationY(radians);

            // 步骤3：平移回中心点
            Matrix4x4 translateBack = Matrix4x4.CreateTranslation(center);

            // 步骤4：组合变换矩阵
            Matrix4x4 transform = translateToOrigin * rotationY * translateBack;
            return transform;
        }
        public static Matrix4x4 GetTransform(float width, float depth, float degree)
        {
            Vector3 center = new Vector3(width / 2, 0, depth / 2);

            Matrix4x4 translateToOrigin = Matrix4x4.CreateTranslation(-center);
            Matrix4x4 rotation = Matrix4x4.CreateRotationY(degree);//弧度
            Matrix4x4 translateBack = Matrix4x4.CreateTranslation(center);

            return translateBack * rotation * translateToOrigin;
        }
        public static TileDirection RotateFace(TileDirection direction)
        {
            return direction switch
            {
                TileDirection.Up => TileDirection.Right,
                TileDirection.Right => TileDirection.Down,
                TileDirection.Down => TileDirection.Left,
                TileDirection.Left => TileDirection.Up,
                _ => TileDirection.Up,
            };
        }
        public static TileDirection Reverse(TileDirection direction)
        {
            return direction switch
            {
                TileDirection.Up => TileDirection.Down,
                TileDirection.Down => TileDirection.Up,
                TileDirection.Left => TileDirection.Right,
                TileDirection.Right => TileDirection.Left,
                _ => TileDirection.Up,
            };
        }
        public void Rotate()
        {
            for (int i = 0; i < AvailableFace.Count; i++)
            {
                AvailableFace[i] = RotateFace(AvailableFace[i]);
            }
            RotateDegree += MathF.PI / 2;
            if (RotateDegree > MathF.PI * 2) RotateDegree = 0;
        }
        public Tile CopySelf()
        {
            Tile tile = new Tile(this.Id, this.Name, this.RestrictedID);
            //tile.Blueprint = this.Blueprint;
            tile.structure = this.structure;
            tile.RotateDegree = this.RotateDegree;
            tile.AvailableFace = new List<TileDirection>(this.AvailableFace);
            tile.CompleteBlockCollapse = this.CompleteBlockCollapse;
            foreach (var item in this.NeighboursID)
            {
                tile.NeighboursID.Add(item.Key, new List<int>(item.Value));
            }
            return tile;
        }
    }

    public enum TileDirection
    {
        Right,
        Left,
        Up,
        Down
    }
}
