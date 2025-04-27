using Engine;

namespace DebugMod
{
    public class ET_LogSink : ILogSink
    {
        public static Stream m_stream;

        public static StreamWriter m_writer;

        public ET_LogSink()
        {
        }

        public static string GetRecentLog(int bytesCount)
        {
            return string.Empty;
        }

        public static List<string> GetRecentLogLines(int bytesCount)
        {
            return new List<string>();
        }

        public void Log(LogType type, string message)
        {
            ScreenLog.SCLOG(message);
        }
    }
}
