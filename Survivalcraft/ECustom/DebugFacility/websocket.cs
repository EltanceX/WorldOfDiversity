using Game;
using System.Xml;
using WebSocketSharp;

namespace DebugMod
{

    public class GameWebsocket
    {
        public static websocket Opened;
    }
    public class websocket : WebSocketSharp.WebSocket
    {
        public ComponentPlayer player;


        //源码修改: Logger.cs 
        //         [+] websocket.m_log
        public delegate void cmdCallback(XmlDocument data, Exception e);
        public Dictionary<string, cmdCallback> cmdQueue = new();
        public string Address;
        public enum MessageType
        {
            Message,
            Connect,
            None,
            PlayerMessage
        }
        public static string uuidv4() { return System.Guid.NewGuid().ToString(); }

        public static void m_log(object o)
        {
            //Console.WriteLine(o);
            ScreenLog.Info(o);
        }
        public static XmlDocument getStandardFormat(string type, string data, string UUID = null)
        {
            if (UUID == null) UUID = uuidv4();
            var xml = new XmlDocument();
            XmlDeclaration declare = xml.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xml.AppendChild(declare);
            var el_root = xml.CreateElement("SCWS");
            xml.AppendChild(el_root);
            var el_type = xml.CreateElement("Type");
            el_type.InnerText = type;
            el_root.AppendChild(el_type);
            var el_UUID = xml.CreateElement("UUID");
            el_UUID.InnerText = UUID;
            el_root.AppendChild(el_UUID);
            var el_Data = xml.CreateElement("Data");
            el_Data.InnerText = data;
            //el_Data.
            el_root.AppendChild(el_Data);
            //xml.CreateAttribute("id", "7777");
            //el_root.SetAttribute("idd", "888");
            return xml;
        }

        public websocket(ComponentPlayer player, string url = "ws://127.0.0.1:3000") : base(url)
        {
            this.player = player;
            //new WebSocketSharp.WebSocket()
            Address = url;
            this.Connect();
            this.EmitOnPing = true;
            this.OnMessage += onMessage;
            this.OnError += onError;
            this.OnClose += onClose;
            //socket.OnError
            //raw_send("666");
            if (this.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                ScreenLog.Info($"已建立服务器连接: {this.Address}");
                send("Connection established. Hello, Server!", MessageType.Connect);
            }
        }
        //public override void Logger()
        //{

        //}
        public void onClose(object sender, CloseEventArgs e)
        {
            GameWebsocket.Opened = null;
            ScreenLog.Info($"Websocket {this.Address} Closed [Code {e.Code}] [R: {e.Reason}]");
        }
        public void onMessage(object sender, MessageEventArgs e)
        {
            //m_log(sender);
            //m_log(e);
            //m_log($"[{Address}] {e.Data}");
            if (e.IsPing)
            {
                m_log("Ping");
                return;
            }
            //var error = false;
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(e.Data);
                string uuid = doc.GetElementsByTagName("UUID")[0].InnerText;
                string type = doc.GetElementsByTagName("Type")[0].InnerText;
                string data = doc.GetElementsByTagName("Data")[0].InnerText;
                bool isret = type == "Return";
                m_log($"[{Address}] Type: {type}, UUID: {uuid}");
                switch (type)
                {
                    case "CommandRequest":
                        CommandInput.Exec(data, player, true);
                        break;
                }

                if (isret)
                {
                    cmdCallback func = cmdQueue[uuid];
                    func.Invoke(doc, null);
                    m_log($"Remove [{uuid}]: {cmdQueue.Remove(uuid)}");
                    m_log($"Queue Length: " + cmdQueue.Count);
                }
            }
            catch (Exception ex)
            {
                m_log(ex);
                return;
            }
        }
        public void onError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            m_log("<Error Event> Error!");
            m_log(e.Message);
        }
        public void raw_send(string data)
        {
            try
            {
                this.Send(data);
            }
            catch (Exception e)
            {
                m_log(e);
            }
        }
        public void send(string data, MessageType type = MessageType.None)
        {
            //m_log($"<send1> {type}, {data}");
            raw_send(xml.xmlToString(getStandardFormat(type.ToString(), data)));
        }
        public void send(string data, cmdCallback callback, MessageType type = MessageType.None)
        {
            var uuid = uuidv4();
            //m_log($"<send2> {type}, {data}, {uuid}");
            raw_send(xml.xmlToString(getStandardFormat(type.ToString(), data, uuid)));
            cmdQueue.Add(uuid, callback);
        }
    }

}
