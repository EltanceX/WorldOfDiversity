using Engine;
using Game;
using System.Runtime.CompilerServices;

namespace GlassMod
{
    public class SurvivalcraftEx
    {
        public class PlayerExObject
        {
            public bool SuperJumpFunctionEnabled = true;
            public int SuperJumpTimes = 1;
            public int SuperJumped = 0;
        }
        public static ConditionalWeakTable<ComponentPlayer, PlayerExObject> ExComponentPlayer = new ConditionalWeakTable<ComponentPlayer, PlayerExObject>();
        public static PlayerExObject BindPlayer(ComponentPlayer player)
        {
            var exobj = new PlayerExObject();
            ExComponentPlayer.AddOrUpdate(player, exobj);
            return exobj;
        }
        public static PlayerExObject? GetPlayerExData(ComponentPlayer player)
        {
            if (player == null) return null;
            PlayerExObject exobj = null;
            //var a = ExComponentPlayer.Count();
            ExComponentPlayer.TryGetValue(player, out exobj);
            return exobj;
        }
    }
    public struct BlockPos
    {
        public int x;
        public int y;
        public int z;
        public BlockPos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public BlockPos(float x, float y, float z)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
        }
        public BlockPos(Vector3 pos)
        {
            this.x = (int)pos.X;
            this.y = (int)pos.Y;
            this.z = (int)pos.Z;
        }
        public BlockPos(Point3 pos)
        {
            this.x = pos.X;
            this.y = pos.Y;
            this.z = pos.Z;
        }
    }
    public static class BlockPosEx
    {
        public static BlockPos Vec3ToBlockPos(Vector3 pos)
        {
            return new BlockPos((int)pos.X, (int)pos.Y, (int)pos.Z);
        }
        public static IEnumerable<BlockPos> IterateInOrder(BlockPos p1, BlockPos p2)
        {
            for (int x = p1.x; x <= p2.x; x++)
            {
                for (int y = p1.y; y <= p2.y; y++)
                {
                    for (int z = p1.z; z <= p2.z; z++)
                    {
                        yield return new BlockPos(x, y, z);
                    }
                }
            }
        }
    }
}
