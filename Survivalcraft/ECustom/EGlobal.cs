using Engine.Input;
using System.Reflection;
using System.Text;
using System.Xml;
using Vector3 = Engine.Vector3;
using Game;

namespace DebugMod
{
    public class GamePadEx
    {

    }

    public class TickTimer
    {
        public double interval;
        public double LastUpdateTime = 0;

        public TickTimer(double interval = 100)
        {
            this.interval = interval;
        }
        public bool Next()
        {
            var tickCurrent = util.getTime();
            if (tickCurrent - LastUpdateTime >= interval)
            {
                LastUpdateTime = tickCurrent;
                return true;
            }
            return false;
        }
    }

    public class xml
    {
        public static string xmlToString(XmlDocument doc)
        {
            // 将 XML 转换为文本格式
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            doc.WriteTo(writer);
            writer.Flush();
            return sb.ToString();
        }
    }
    public class EGlobal
    {
        public static string Version = "1.6.0";
        public static string Date = "2025-4-25";
        public static string UpdateInfo = @"多样化世界之神秘冒险 整合包";
        public static bool SlopeEnabled = false;
        public static bool GenerateEnabled = true;
        public enum Platforms
        {
            Android,
            Windows
        }
        public static Platforms Platform = OperatingSystem.IsWindows() ? Platforms.Windows : Platforms.Android;
        public static double Android_LastMouseDown = 0;
        public static bool WindowClosing = false;
        //public static void AssemblyInit()
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //public static void place(int x, int y, int z, int value = 2)
        //{
        //    terrain?.ChangeCell(x, y, z, Terrain.MakeBlockValue(value));
        //}
        public static bool IsMouseDown(MouseButton mb)
        {
            switch (Platform)
            {
                case Platforms.Android:
                    if (mb == MouseButton.Right) return false;
                    if (Touch.TouchLocations.Count == 0) return false;
                    var location = Touch.TouchLocations[0];
                    double timenow = util.getTime();
                    if (location.State == TouchLocationState.Pressed) Android_LastMouseDown = timenow;
                    if (location.State == TouchLocationState.Released && timenow - Android_LastMouseDown <= 200) return true;
                    break;
                case Platforms.Windows:
                    return Mouse.IsMouseButtonDown(mb);
                    break;
            }
            return false;
        }
    }
    public class util
    {
        private static System.Random random = new();
        public static int randomI(int min, int max)
        {
            return random.Next(min, max);
        }
        public static double RandomD(double minimum, double maximum, int Len = 1)   //Len小数点保留位数
        {
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, Len);
        }
        public static float RandomF(float minimum, float maximum, int Len = 1)   //Len小数点保留位数
        {
            return (float)Math.Round(random.NextDouble() * (maximum - minimum) + minimum, Len);
        }
        public static string Progress(double per, int len)
        {
            if (per > 1) return per.ToString();
            return $"[{new String(' ', (int)(len * (1 - per))).PadLeft(len, '|')}]";
        }
        public static float VectorProjection(Vector3 vec, Vector3 target)
        {
            return (float)((vec.X * target.X + vec.Y * target.Y + vec.Z * target.Z) / Math.Sqrt(Math.Pow(target.X, 2) + Math.Pow(target.Y, 2) + Math.Pow(target.Z, 2)));
        }
        public static string uuid()
        {
            return Guid.NewGuid().ToString();
        }
        public static double getTime()
        {
            DateTime dateTime = DateTime.Now;
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (dateTime - startTime).TotalMilliseconds;

        }
        //public static Vector3 crossProduct(Vector3 a, Vector3 b)
        //{
        //    return new Vector3(
        //        a.Y * b.Z - a.Z * b.Y,
        //        a.Z * b.X - a.X * b.Z,
        //        a.X * b.Y - a.Y * b.X
        //    );
        //}
        public static Vector3[] GetUVByPlaneNormal(Vector3 planeNormal)
        {
            Vector3 u = new Vector3(1, 0, 0);
            //if (MathUtils.Abs(planeNormal.X) < 1e-10 && MathUtils.Abs(planeNormal.Y) < 1e-10)
            //{
            //    u = new Vector3(1, 0, 0);  // If b is close to (0, 0, 1), use x-axis
            //}
            //else
            //{
            //    u = new Vector3(0, 0, 1);  // Otherwise, use z-axis
            //}
            if (planeNormal.X >= 0 && planeNormal.Z <= 0 || planeNormal.X <= 0 && planeNormal.Z <= 0) u = new Vector3(1, 0, 0); //+- --
            else if (planeNormal.X >= 0 && planeNormal.Z >= 0 || planeNormal.X <= 0 && planeNormal.Z >= 0) u = new Vector3(-1, 0, 0); //++ -+
            //u = new Vector3(-1, 0, 0);  // Otherwise, use z-axis

            u = Vector3.Cross(u, planeNormal);

            u = Vector3.Normalize(u);


            Vector3 v = Vector3.Cross(u, planeNormal);
            v = Vector3.Normalize(v);
            //ScreenLog.Info($"U: {u.X} {u.Y} {u.Z} || V: {v.X} {v.Y} {v.Z}");
            return new Vector3[] { u, v };
        }
    }
}