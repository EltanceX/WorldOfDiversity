using Engine;
using Game;

namespace GlassMod
{
    public class TVKeyboardBlock : ModelBlock
    {
        public const int Index = 902;
        public BoundingBox[][] m_collisionBoxesBySize = new BoundingBox[8][];

        public TVKeyboardBlock()
            : base("Textures/IceOverlay", "Models/kb")
        {
            DefaultDisplayName = "键盘";
            DefaultDescription = "右键点击激活虚拟键盘";
            DefaultCategory = "WebTV";
            CraftingId = "Keyboard";
            DefaultDropContent = Index;
            this.Behaviors = typeof(TVKeyboardBlockBehavior).Name;
            //this.collision
        }
        public override void Initialize()
        {
            base.Initialize();
            BoundingBox boundingBox = new BoundingBox(new Vector3(-0.2f, 0f, 0.2f), new Vector3(1.2f, 0.2f, 0.8f));
            m_collisionBoxesBySize[0] = new BoundingBox[1]
            {
                    boundingBox
            };
        }
        public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
        {
            return m_collisionBoxesBySize[0];
        }
    }
}
