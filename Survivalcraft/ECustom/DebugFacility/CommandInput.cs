using AccessLib;
using Acornima;
using Game;
using GlassMod;
using System.Collections.Generic;
using System.Diagnostics;
using WebSocketSharp.Net;
using Engine;
namespace DebugMod
{
    public class CommandParsed
    {
        public Dictionary<string, string> opts;
        public string origin;
        public string header;
        public string body;
        public ArrayList arr;
        public CommandParsed(string str)
        {
            this.origin = str;
        }
    }
    public class CommandInput
    {
        public static void PrintError(ComponentPlayer player, string message)
        {
            player.ComponentGui.DisplaySmallMessage(message, Color.Orange, false, true);
            ScreenLog.Error(message);
        }
        public static void PrintSuccess(ComponentPlayer player, string message)
        {
            player.ComponentGui.DisplaySmallMessage(message, Color.Green, false, true);
            ScreenLog.Info(message);
        }
        public static Dictionary<string, Action<CommandParsed, ComponentPlayer>> Commands = new();
        public static void Initialize(bool clearBeforeLoading = true)
        {
            if (clearBeforeLoading) Commands.Clear();
            Commands.Add("/wfc", (CommandParsed parsed, ComponentPlayer player) =>
            {
                //WFCGameBridge.LoadTiles();
                WFCGameBridge.Generate(player);
            });
            Commands.Add("/bp", (CommandParsed parsed, ComponentPlayer player) =>
            {
                //Debugger.Break();
                if (parsed.arr.Count < 2)
                {
                    PrintError(player, "Paramaters not matched (2)");
                    return;
                }
                switch (parsed.arr[0].ToString())
                {
                    case "export":
                        string fileName = parsed.arr[1].ToString();
                        fileName = BluePrintGameBridge.SBPDirectoryPath + fileName;
                        if (!fileName.EndsWith(".sbp")) fileName += ".sbp";
                        try
                        {
                            BluePrintGameBridge.Export(player, fileName);
                            PrintSuccess(player, "File exported successfully: " + fileName);
                        }
                        catch (Exception e) { PrintError(player, e.Message); }
                        break;
                    case "import":
                        string fileName2 = parsed.arr[1].ToString();
                        fileName2 = BluePrintGameBridge.SBPDirectoryPath + fileName2;
                        if (!fileName2.EndsWith(".sbp")) fileName2 += ".sbp";
                        try
                        {
                            BluePrintGameBridge.Import(player, fileName2);
                            PrintSuccess(player, "File imported successfully: " + fileName2);
                        }
                        catch (Exception e) { PrintError(player, e.Message); }
                        break;
                }
            });
            Commands.Add("/next", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var bgmusic = player.Project.Subsystems.Where(x => x is SubsystemBgMusic).FirstOrDefault() as SubsystemBgMusic;
                if (bgmusic == null) return;
                if (bgmusic.BgMusic != null)
                {
                    bgmusic.BgMusic.Stop();
                    bgmusic.BgMusic.Dispose();
                    bgmusic.BgMusic = null;
                }
                bgmusic.NextIndex++;
                if (bgmusic.NextIndex == bgmusic.musicDatas.Count) bgmusic.NextIndex = 0;
                bgmusic.BgMusic = BackgroundMusicManager.GetMusic(bgmusic.musicDatas[bgmusic.NextIndex].streamingSource, 0);
                bgmusic.BgMusic.Play();
                bgmusic.MusicBeginTime = util.getTime() / 1000;
                bgmusic.WaitBeginTime = 0;
            });
            Commands.Add("/rotate", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var block = BlocksManager.Blocks[906] as ConstructionWandBlock;
                if (block == null)
                {
                    ScreenLog.Error("Block not found!");
                    return;
                }
                float x = float.Parse(parsed.arr[0].ToString());
                float y = float.Parse(parsed.arr[1].ToString());
                float z = float.Parse(parsed.arr[2].ToString());
                block.DefaultIconBlockOffset = new Vector3(x, y, z);


            });
            Commands.Add("/updatelight", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var subsystemTerrain = player.Project.FindSubsystem<SubsystemTerrain>();
                var pos = player.ComponentBody.Position;
                TerrainChunk chunk = subsystemTerrain.Terrain.GetChunkAtCell((int)pos.X, (int)pos.Z);
                if (chunk == null)
                {
                    ScreenLog.Error("chunk equals null");
                    return;
                }
                int[][] cells = new int[5][] {
                    new int[]{1 , 0 },
                    new int[]{-1, 0 },
                    new int[]{0 , 0 },
                    new int[]{0 , 1 },
                    new int[]{0 , -1 },
                };
                foreach (int[] cell in cells)
                {
                    TerrainChunk chunkTest = subsystemTerrain.Terrain.GetChunkAtCoords(chunk.Coords.X + cell[0], chunk.Coords.Y + cell[1]);
                    if (chunkTest == null) continue;
                    chunkTest.ThreadState = TerrainChunkState.InvalidLight;
                    chunkTest.State = TerrainChunkState.InvalidLight;
                }
                subsystemTerrain.TerrainUpdater.UnpauseUpdateThread();
            });
            Commands.Add("/music", (CommandParsed parsed, ComponentPlayer player) =>
            {
                MusicManager.PlayMusic("Music/Background/winter_the_wind_can_be_still", 0);
                ScreenLog.Info("Music Playing");
                MusicManager.m_volume = 0.01f;
            });
            Commands.Add("/music2", (CommandParsed parsed, ComponentPlayer player) =>
            {
                BackgroundMusicManager.PlayMusic("Music/Background/winter_the_wind_can_be_still", 0);
                ScreenLog.Info("Music Playing");
                //MusicManager.m_volume = 0.01f;
            });
            Commands.Add("/help", (CommandParsed parsed, ComponentPlayer player) =>
            {
                int page = 1;
                try
                {
                    page = int.Parse(parsed.arr[0].ToString());
                }
                catch
                {
                    page = 1;
                }
                switch (page)
                {
                    case 1:
                        ScreenLog.Info("-- 显示帮助手册总 2 页中的第 1 页(/help [页码]) --\n" +
                            "/help                显示帮助页面\n" +
                            "/clear               清空聊天栏\n" +
                            "/title [Content]     向玩家展示标题\n" +
                            "/gamemode [Int]      切换游戏模式 [0]Creative [1]Harmless [2]Survival\n" +
                            "                                 [3]Challenging [4]Cruel [5]Adventure\n" +
                            "/heal [Health:Float] 治疗玩家\n" +
                            "/kill                杀死玩家\n" +
                            "/spawnpoint          设置玩家的出生点为当前位置\n" +
                            "/version             显示游戏版本\n" +
                            "/about               关于ETerminal\n" +
                            "/dll                 列出所有已加载的程序集\n" +
                            "-- 查看下一页：/help 2 --"
                        );
                        break;
                    case 2:
                        ScreenLog.Info("-- 显示帮助手册总 2 页中的第 2 页(/help [页码]) --\n" +
                            "/setblock [x: Int][y: Int][z: Int] [Value: Int] 在世界中放置方块\n" +
                            "/tp [x: Float][y: Float][z: Float]              在世界中传送\n" +
                            "/say [Message]              发送聊天信息\n" +
                            "/destroy [x: Int] [y:Int] [z: Int]              在世界中破坏方块\n" +
                            "/crosshair     -"
                        );
                        break;
                }
            });
            Commands.Add("/setblock", (CommandParsed parsed, ComponentPlayer player) =>
            {
                try
                {
                    int x = int.Parse(parsed.arr[0].ToString());
                    int y = int.Parse(parsed.arr[1].ToString());
                    int z = int.Parse(parsed.arr[2].ToString());
                    int value = int.Parse(parsed.arr[3].ToString());
                    player.m_subsystemTerrain.ChangeCell(x, y, z, value);
                    var sterrain = ModAccessUtil.GetFieldOnceSlow<ComponentPlayer, SubsystemTerrain>(player, "m_subsystemTerrain");
                    sterrain.ChangeCell(x, y, z, value);

                    ScreenLog.Info($"已将坐标位置为 {x}, {y}, {z} 的方块Value设置为: {value}");
                }
                catch (Exception e)
                {
                    ScreenLog.Info(e);
                    ScreenLog.Info("命令参数错误!");
                }
            });
            Commands.Add("/destroy", (CommandParsed parsed, ComponentPlayer player) =>
            {
                try
                {
                    int x = int.Parse(parsed.arr[0].ToString());
                    int y = int.Parse(parsed.arr[1].ToString());
                    int z = int.Parse(parsed.arr[2].ToString());
                    int value = int.Parse(parsed.arr[3].ToString());
                    player.m_subsystemTerrain.DestroyCell(10, x, y, z, 0, false, false);
                    ScreenLog.Info($"已将摧毁位于 {x}, {y}, {z} 的方块");
                }
                catch (Exception e)
                {
                    ScreenLog.Info(e);
                    ScreenLog.Info("命令参数错误!");
                }
            });
            Commands.Add("/say", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var playername = player.PlayerData.Name;
                ScreenLog.Info((false ? "[External] " : $"<{playername}> ") + parsed.body);

            });
            Commands.Add("/tp", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var playername = player.PlayerData.Name;
                try
                {
                    float x = float.Parse(parsed.arr[0].ToString());
                    float y = float.Parse(parsed.arr[1].ToString());
                    float z = float.Parse(parsed.arr[2].ToString());
                    player.ComponentBody.m_position.X = x;
                    player.ComponentBody.m_position.Y = y;
                    player.ComponentBody.m_position.Z = z;
                    ScreenLog.Info($"已将玩家 {playername} 传送至 {x} {y} {z}");
                }
                catch (Exception e)
                {
                    ScreenLog.Warn(e);
                    ScreenLog.Warn("无效的坐标参数，格式: /tp [x] [y] [z]");
                }
            });
            Commands.Add("/spawnpoint", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var playername = player.PlayerData.Name;

                var pos = player.ComponentBody.Position;
                player.PlayerData.SpawnPosition = pos;
                ScreenLog.Info($"将玩家 {playername} 的 spawnpoint 设置为: {pos.X} {pos.Y} {pos.Z}");
            });
            Commands.Add("/version", (CommandParsed parsed, ComponentPlayer player) =>
            {
                ScreenLog.Info($"Survival Craft: {Game.VersionsManager.Version}");
                ScreenLog.Info($"ETerminal: {EGlobal.Version} [{EGlobal.Date}]");
            });
            Commands.Add("/about", (CommandParsed parsed, ComponentPlayer player) =>
            {
                ScreenLog.Info(@"ETerminal by EltanceX 控制台模组
ETerminal 控制台组件
按键绑定: [F3]实时信息 [F1]日志 [UP]上滚 [Down]下滚 [/]聊天和命令
禁用字体： 删除FontTemplate.dll
使用钻石镐采集石头以触发3x3效果。
本次更新: " + EGlobal.UpdateInfo);
            });
            Commands.Add("/dll", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    ScreenLog.Info(assembly.FullName);
                }
                ScreenLog.Info("---- List of all loaded dependency files ----");
            });
            Commands.Add("/kill", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var playername = player.PlayerData.Name;
                //EGlobal.Player.Entity.FindComponent<ComponentHealth>(throwOnError: true).Health = -1;
                var health = player.Entity.FindComponent<ComponentHealth>(throwOnError: true);
                //health.Health = 0;
                ModAccessUtil.CallSetterOnceSlow<ComponentHealth, float>(health, "Health", -1f);
                player.ComponentGui.DisplaySmallMessage("Set Health to -1", Color.Cyan, true, true);

                ScreenLog.Info($"已清除: {playername}");
            });
            Commands.Add("/title", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var playername = player.PlayerData.Name;
                string content;
                try
                {
                    content = parsed.body;
                }
                catch { content = "null"; }
                ScreenLog.Info($"向玩家 {playername} 的 actionbar 展示: {content}");
                player.ComponentGui.DisplaySmallMessage(content, Color.White, true, true);
            });
            Commands.Add("/heal", (CommandParsed parsed, ComponentPlayer player) =>
            {
                var playername = player.PlayerData.Name;
                float heal = 1;
                if (parsed.arr.Count != 0)
                {
                    try
                    {
                        heal = float.Parse(parsed.arr[0].ToString());
                    }
                    catch (Exception e)
                    {
                        ScreenLog.Info(e);
                        ScreenLog.Info("错误: 不是一个浮点数");
                        return;
                    }
                }
                player.Entity.FindComponent<ComponentHealth>(throwOnError: true).Health += heal;
                ScreenLog.Info($"已治愈玩家 {playername} 的血量: {heal}");
            });
            Commands.Add("/gamemode", (CommandParsed parsed, ComponentPlayer player) =>
            {
                string mode;
                try { mode = parsed.arr[0].ToString(); }
                catch
                {
                    mode = "0";
                }
                switch (mode.ToLower())
                {
                    case "0":
                    case "creative":
                        player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Creative;
                        break;
                    case "1":
                    case "harmless":
                        player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Harmless;
                        break;
                    case "2":
                    case "survival":
                        player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Survival;
                        break;
                    case "3":
                    case "challenging":
                        player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Challenging;
                        break;
                    case "4":
                    case "Cruel":
                        player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Cruel;
                        break;
                    case "5":
                    case "Adventure":
                        player.m_subsystemGameInfo.WorldSettings.GameMode = GameMode.Adventure;
                        break;
                }
                ScreenLog.Info($"已将游戏模式设置为: {player.m_subsystemGameInfo.WorldSettings.GameMode}");
            });
            Commands.Add("/clear", (CommandParsed parsed, ComponentPlayer player) =>
            {
                ScreenLog.logs.Clear();
                ScreenLog.currentLine = 0;
                ScreenLog.Refresh();
                ScreenLog.Info("已清屏");
            });
            Commands.Add("/ccmd", (CommandParsed parsed, ComponentPlayer player) =>
            {
                Debugger.Break();
                var terrain = ModAccessUtil.GetFieldOnceSlow<ComponentPlayer, SubsystemTerrain>(player, "m_subsystemTerrain");
                var pos1 = player.ComponentBody.Position;
                //terrain.ChangeCell((int)Math.Floor(pos1.X), (int)Math.Floor(pos1.Y), (int)Math.Floor(pos1.Z), 1);//将当前位置替换为基岩

                float a = 5;
                for (float theta = 0; theta < 2 * Math.PI; theta += 0.04f)
                {
                    float r = a * theta;
                    float x = (float)Math.Floor(Math.Cos(theta) * r);
                    float z = (float)Math.Floor(Math.Sin(theta) * r);

                    terrain.ChangeCell((int)(pos1.X + x), (int)pos1.Y, (int)(pos1.Z + z), 1);
                }
            });
        }
        public static CommandParsed ParseCommand(string str)
        {
            var cmdp = new CommandParsed(str);
            cmdp.origin = str;
            var opts = new Dictionary<string, string>();
            ArrayList arr = new ArrayList();
            string optIndex = null;
            for (var i = 0; i < str.Length;)
            {
                int space = str.IndexOf(' ', i);
                if (space == -1) space = str.Length;
                string s = str.Substring(i, space - i);
                if (s == "")
                {
                    i++;
                    continue;
                }
                if (s[0] == '*')
                {
                    string optheader = s.Substring(1, s.Length - 1);
                    try
                    {
                        opts.Add(optheader, "true");
                    }
                    catch { }
                    optIndex = optheader;
                    i = space + 1;
                    continue;
                }
                else if (optIndex != null)
                {
                    opts[optIndex] = s;
                    optIndex = null;
                    i = space + 1;
                    continue;
                }
                if (cmdp.header == null)
                {
                    cmdp.header = s;
                    if (space + 1 > str.Length) cmdp.body = "";
                    else cmdp.body = str.Substring(space + 1, str.Length - space - 1);
                }
                else arr.Add(s);
                i = space + 1;
            }

            cmdp.arr = arr;
            cmdp.opts = opts;
            return cmdp;
        }
        public static void Exec(string cmd, ComponentPlayer player, bool isExternal = false)
        {
            var playername = player.PlayerData.Name;
            if (!isExternal) ScreenLog.Info($"<{playername}> {cmd}");
            if (cmd[0] != '/') return;
            CommandParsed parsed = ParseCommand(cmd);
            string header = parsed.header;
            string headerLower = header.ToLower();
            if (Commands.Keys.Contains(headerLower))
            {
                Commands[headerLower].Invoke(parsed, player);
                return;
            }
            else ScreenLog.Info($"未知的命令: {header}。\n请检查命令是否存在，执行/help以显示帮助菜单。");


        }
    }
}
