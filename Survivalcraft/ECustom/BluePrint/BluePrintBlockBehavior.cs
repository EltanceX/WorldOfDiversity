using Engine;
using Game;
using Silk.NET.Maths;
using System.Diagnostics;
using TemplatesDatabase;

namespace GlassMod
{
    public class BluePrintBlockBehavior : SubsystemBlockBehavior//,IDrawable, IUpdateable
    {
        //public int[] DrawOrders => [1024];
        //public UpdateOrder UpdateOrder => UpdateOrder.Default;
        //public void Update(float dt)
        //{

        //}
        //public void Draw(Camera camera, int i)
        //{

        //}

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
        }
        public override void Save(ValuesDictionary valuesDictionary)
        {
            base.Save(valuesDictionary);
        }
        public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
        {
            var bluePrint = componentMiner.Entity.Components.Where(x => x is ComponentBluePrint).FirstOrDefault() as ComponentBluePrint;
            if (bluePrint == null) return false;

            TerrainRaycastResult? res = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Digging);
            if (res == null) return false;

            var cellFace = res.Value.CellFace;
            if (!bluePrint.FirstPosition.HasValue)
            {
                bluePrint.FirstPosition = new Vector3(cellFace.Point.X, cellFace.Point.Y, cellFace.Point.Z);
            }
            else if (!bluePrint.SecondPosition.HasValue)
            {
                bluePrint.SecondPosition = new Vector3(cellFace.Point.X, cellFace.Point.Y, cellFace.Point.Z);
            }
            else
            {
                bluePrint.FirstPosition = null;
                bluePrint.SecondPosition = null;
            }
            return true;
        }
    }
}
