using Game;

namespace DebugMod
{
    public class ScreenLog
    {
        public enum Levels
        {
            Debug,
            SC,
            Info,
            Warn,
            Error
        }
        public static int LogLevel = (int)Levels.Debug;
        public static LabelWidget label;
        public static bool Enabled
        {
            get => label == null ? false : label.IsVisible;
            set { if (label != null) label.IsVisible = value; }
        }
        public static int currentLine = -1;
        public static int maxDisplayLine = 20;
        public static List<string> logs = new();

        public static ComponentPlayer player;
        public static void UpdatePlayer(ComponentPlayer newPlayer)
        {
            player = newPlayer;
        }
        public static void Info(object obj = null)
        {
            Send(obj, Levels.Info);
            //if (!(LogLevel <= 1)) return;
            //logs.Add($"[Info]{DateTime.Now.ToString("[HH:mm:ss.fff]")} " + obj.ToString());
            //currentLine = logs.Count - 1;
            //Refresh();
        }
        public static void SCLOG(object obj = null)
        {
            Send(obj, Levels.SC);
        }
        public static void Error(object obj = null)
        {
            Send(obj, Levels.Error);
        }
        public static void Warn(object obj = null)
        {
            Send(obj, Levels.Warn);
        }
        public static void Debug(object obj = null)
        {
            Send(obj, Levels.Debug);
            //if (!(LogLevel <= 0)) return;
            //string str = obj.ToString();
            //if (str.Contains("\n"))
            //{
            //    string[] strs = str.Split('\n');
            //    logs.Add($"[Debug]{DateTime.Now.ToString("[HH:mm:ss.fff]")} " + strs[0]);
            //    for (int i = 1; i < strs.Length; i++)
            //    {
            //        logs.Add($"       " + strs[i]);
            //    }
            //    return;
            //}
            //logs.Add($"[Debug]{DateTime.Now.ToString("[HH:mm:ss.fff]")} " + str);
            //currentLine = logs.Count - 1;
            //Refresh();
        }
        private static void Send(object obj, Levels level)
        {
            if (!(LogLevel <= (int)level)) return;
            string str = obj.ToString();
            if (str.Contains("\n"))
            {
                string[] strs = str.Split('\n');
                logs.Add($"[{level}]{DateTime.Now.ToString("[HH:mm:ss.fff]")} " + strs[0]);
                for (int i = 1; i < strs.Length; i++)
                {
                    logs.Add($"  " + strs[i]);
                }
            }
            else
            {
                logs.Add($"[{level}]{DateTime.Now.ToString("[HH:mm:ss.fff]")} " + str);
            }

            if (logs.Count > 6000)
            {
                logs.RemoveRange(0, 1000);
            }

            currentLine = logs.Count - 1;
            Refresh();
        }
        public static void Refresh()
        {
            if (label != null)
            {
                var tolog = $"ELog [↑: UP, ↓: Down] [Line {currentLine} / {logs.Count - 1}]";
                for (var i = currentLine - maxDisplayLine; i <= currentLine; i++)
                {
                    tolog += "\n";
                    if (i >= 0 && i < logs.Count) tolog += /*$"{i} " + */logs[i];
                }
                label.Text = tolog + $"    [{currentLine}]";
            }
        }
        public static void up()
        {
            currentLine--;
            Refresh();
        }
        public static void down()
        {
            currentLine++;
            Refresh();
        }
        public static void RemoveLabel()
        {
            player.GuiWidget.Children.Remove(label);
            label = null;
        }
    }
}
