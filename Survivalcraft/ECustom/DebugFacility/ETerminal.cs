using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;
using Game;

namespace DebugMod
{
    public static class CrossHair
    {
        public static bool isDisplaying = true;
        public static Subtexture crosshairT;
        public static Subtexture crosshairF;

        public static bool SwitchState()
        {
            if (isDisplaying) TextureAtlasManager.m_subtextures["Textures/Atlas/Crosshair"] = crosshairF;
            else TextureAtlasManager.m_subtextures["Textures/Atlas/Crosshair"] = crosshairT;
            return isDisplaying = !isDisplaying;
        }
    }
    public class ETerminal : Component, IUpdateable
    {
        public ET_F3 et_F3;

        public TickTimer keyboardTickTimer = new(10);

        public ComponentPlayer Player;
        public static Dictionary<string, ButtonWidget> CustomButtons = new();
        public SubsystemSky subsystemSky;
        public SubsystemBodies m_subsystemBodies;
        public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();
        public ComponentPlayer componentPlayer;
        public SubsystemBlockEntities Entities;
        public ComponentOnFire componentOnFire;
        public ComponentMiner componentMiner;

        public SubsystemTime subsystemTime;

        public float m_frequency = 0.5f;
        public Entity entity;

        public static void HandleInput(string input, ComponentPlayer player)
        {
            if (input == null || input == string.Empty) return;//当输入为空: 确定:input=string.empty  取消:input=nulll

            CommandInput.Exec(input, player, false);
        }

        UpdateOrder IUpdateable.UpdateOrder => UpdateOrder.Default;

        public void AddButton(string Name,
                    string Text,
                    Vector2 Size,
                    BitmapFont Font = null,
                    bool isEnabled = true,
                    bool isVisible = true
                    )
        {

            if (Font == null) Font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
            if (Size == null) Size = new Engine.Vector2(110f, 60f);


            var btn = Player.ViewWidget.GameWidget.Children.Find<ButtonWidget>(Name, false);
            if (btn != null)
            {
                //XLog.Information("按钮已存在" + Name);
                CustomButtons[Name] = btn;
            }
            else
            {
                //XLog.Information("按钮不存在" + Name);
                var btn2 = new BevelledButtonWidget
                {
                    Name = Name,
                    Text = Text,
                    Font = Font,
                    Size = Size,
                    IsEnabled = isEnabled,
                    IsVisible = isVisible
                };
                Player.ViewWidget.GameWidget.Children.Find<StackPanelWidget>("RightControlsContainer").Children.Add(btn2);
                if (!CustomButtons.TryGetValue(Name, out ButtonWidget _)) CustomButtons.Add(Name, btn2);
                else CustomButtons[Name] = btn2;
            }


        }


        public static TextBoxDialog commandbox;
        public void Update(float dt)
        {


            //如果正在显示命令输入界面
            if (DialogsManager.HasDialogs(componentPlayer.GuiWidget))
            {
                if (Keyboard.IsKeyDownOnce(Key.Enter))
                {
                    commandbox.Dismiss(commandbox.m_textBoxWidget.Text);
                }
                else if (Keyboard.IsKeyDownOnce(Key.UpArrow))
                {

                }
                else if (Keyboard.IsKeyDownOnce(Key.DownArrow))
                {

                }
                //Has had dislog, do nothing.
            }
            //else if (Keyboard.IsKeyDownOnce(Key.K) || CustomButtons["WebTV"].IsClicked)
            //{
            //    try
            //    {
            //        var wid = new BrowserWidget(componentPlayer);
            //        wid.Update();
            //        bool flag = componentPlayer.ComponentGui.ModalPanelWidget is BrowserWidget;
            //        if (flag)
            //        {
            //            componentPlayer.ComponentGui.ModalPanelWidget = null;
            //        }
            //        else
            //        {
            //            //componentPlayer.ComponentGui.ModalPanelWidget = new BrowserWidget(componentPlayer);
            //            //WebTV.settings.KWidget.RefreshPosition();
            //            //componentPlayer.ComponentGui.ModalPanelWidget = WebTV.settings.KWidget;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        ScreenLog.Info(e);
            //    }
            //}
            else if (Keyboard.IsKeyDownOnce(Key.Slash) || CustomButtons["ChatButton"].IsClicked)
            {
                commandbox = new TextBoxDialog("ETerminal: Chat & Command", string.Empty, 256, (string input) =>
                    {
                        HandleInput(input, Player);
                    }
                    //if (input == null || input == string.Empty) return;//当输入为空: 确定:input=string.empty  取消:input=nulll
                );
                DialogsManager.ShowDialog(componentPlayer.GuiWidget, commandbox);
            }
            else if (Keyboard.IsKeyDown(Key.UpArrow) || CustomButtons["BButton"].IsClicked)
            {
                if (keyboardTickTimer.Next()) ScreenLog.up();
            }
            else if (Keyboard.IsKeyDown(Key.DownArrow) || CustomButtons["CButton"].IsClicked)
            {
                if (keyboardTickTimer.Next()) ScreenLog.down();
            }
            else if (Keyboard.IsKeyDownOnce(Key.F1) || CustomButtons["F1"].IsClicked)
            {
                bool visible = ScreenLog.label.IsVisible = !ScreenLog.label.IsVisible;
                CustomButtons["BButton"].IsVisible = visible;
                CustomButtons["CButton"].IsVisible = visible;
            }
            else if (Keyboard.IsKeyDownOnce(Key.F3) || CustomButtons["F3"].IsClicked)
            {
                et_F3.SwitchState();
            }



            et_F3.Update();
        }
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            base.Save(valuesDictionary, entityToIdMap);
        }
        public override void OnEntityAdded()
        {
            base.OnEntityAdded();
        }

        //玩家移除(死亡)
        public override void OnEntityRemoved()
        {
            base.OnEntityRemoved();
            ScreenLog.Info("\n\n");

            et_F3.Dispose();
            ScreenLog.RemoveLabel();
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);


            Player = base.Entity.FindComponent<ComponentPlayer>();
            componentPlayer = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            et_F3 = new ET_F3(base.Entity);

            var screenLogLabel = new LabelWidget()
            {
                FontScale = 0.6f,
                Color = Color.LightGray,
                Margin = new Vector2(300f, 0f),
                VerticalAlignment = WidgetAlignment.Near
            };
            componentPlayer.GuiWidget.Children.Add(screenLogLabel);
            ScreenLog.label = screenLogLabel;
            ScreenLog.label.IsVisible = false;
            ScreenLog.Info("Debug Log Screen View Loaded.\n");



            Player = base.Entity.FindComponent<ComponentPlayer>();
            entity = base.Entity;
            componentPlayer = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            componentMiner = base.Entity.FindComponent<ComponentMiner>(throwOnError: true);
            //componentLocomotion = base.Entity.FindComponent<ComponentLocomotion>(throwOnError: true);
            subsystemTime = base.Project.FindSubsystem<SubsystemTime>(throwOnError: true);
            //m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
            //m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(throwOnError: true);
            m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
            subsystemSky = base.Project.FindSubsystem<SubsystemSky>(throwOnError: true);
            var ButtonSize = new Vector2(60f, 50f);
            AddButton("F1", "F1", ButtonSize);
            AddButton("F3", "F3", ButtonSize);
            AddButton("BButton", "↑", ButtonSize, isVisible: false);
            AddButton("CButton", "↓", ButtonSize, isVisible: false);
            //AddButton("WebTV", "K", ButtonSize);


            StackPanelWidget stackPanelWidget = componentPlayer.GameWidget.Children.Find<StackPanelWidget>("MoreContents");
            var mapButton = new BevelledButtonWidget
            {
                Name = "ChatButton",
                Text = "/",
                Size = new Vector2(68f, 64f),
                Margin = new Vector2(4f, 0f),
                CenterColor = new Color(127, 127, 127, 180)
            };
            try
            {
                stackPanelWidget.Children.Find<ButtonWidget>(mapButton.Name);
            }
            catch
            {
                stackPanelWidget.Children.Add(mapButton);
                try
                {
                    CustomButtons[mapButton.Name] = mapButton;
                }
                catch
                {
                    CustomButtons.Add(mapButton.Name, mapButton);
                }
            }

            //WebTV.settings.KWidget = new BrowserWidget(componentPlayer);

            Texture2D AtlasTexture = TextureAtlasManager.AtlasTexture;
            CrossHair.crosshairT = TextureAtlasManager.m_subtextures["Textures/Atlas/Crosshair"];
            CrossHair.crosshairF = new Subtexture(AtlasTexture, new Vector2(0), new Vector2(0));

            //ScreenLog.Info("\n");
            ScreenLog.Info("[ETerminal]获取更新: 328170928");
            ScreenLog.Info("按 / 以打开控制台，按 esc 打开菜单");
            ScreenLog.Info("测试版本，若发现问题请及时反馈，感谢您的游玩体验");
            ScreenLog.Info(" ");
            ScreenLog.Info(" ");
            ScreenLog.Info("加载完成!");
            ScreenLog.Info("World of Diversity - Mysterious Adventures");
            componentPlayer.ComponentGui.DisplaySmallMessage("Welcome to World of Diversity - Mysterious Adventures", Color.White, false, false);
            ScreenLog.Info("欢迎来到 多样性世界之神秘冒险[Alpha 0.1]");
            ScreenLog.Info("SurvivalCraft2.4 API 1.81 [Modified]");
            //ScreenLog.Info("生存战争内置浏览器模组 安卓版本");
        }
    }
}