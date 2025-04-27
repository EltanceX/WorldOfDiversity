using Engine.Graphics;
using Game;
using GameEntitySystem;
using Engine;

namespace GlassMod
{
    public class ComponentBluePrint : Component, IDrawable, IUpdateable
    {
        public int[] DrawOrders => [1024];
        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public Vector3? FirstPosition;
        public Vector3? SecondPosition;
        public PrimitivesRenderer3D primitivesRenderer3D = new PrimitivesRenderer3D();

        public void Update(float dt)
        {

        }

        public void Use()
        {

        }
        public void Draw(Camera camera, int i)
        {
            var flatBatch3D = primitivesRenderer3D.FlatBatch();
            if (FirstPosition.HasValue)
            {
                if (SecondPosition.HasValue)
                {
                    float x = Math.Min(FirstPosition.Value.X, SecondPosition.Value.X);
                    float y = Math.Min(FirstPosition.Value.Y, SecondPosition.Value.Y);
                    float z = Math.Min(FirstPosition.Value.Z, SecondPosition.Value.Z);
                    Vector3 rvec = SecondPosition.Value - FirstPosition.Value;
                    float width = Math.Abs(rvec.X);
                    float height = Math.Abs(rvec.Y);
                    float length = Math.Abs(rvec.Z);
                    flatBatch3D.QueueBoundingBox(new BoundingBox(new Vector3(x, y, z), new Vector3(x + width, y + height, z + length) + new Vector3(1)), Color.Orange);
                }
                else flatBatch3D.QueueBoundingBox(new BoundingBox(FirstPosition.Value, FirstPosition.Value + new Vector3(1)), Color.Orange);
            }
            primitivesRenderer3D.Flush(camera.ViewProjectionMatrix);
        }
    }
}
