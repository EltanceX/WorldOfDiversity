using DebugMod;
using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using GMenuMod;
using System.Diagnostics;
using TemplatesDatabase;

namespace GlassMod
{
    public class GamePauseMenu : PixelScreen
    {
        public Dictionary<TopbarButton, Div> AssociatedDivs = new Dictionary<TopbarButton, Div>();
        public List<TopbarButton> TopbarButtons = new List<TopbarButton>();
        public bool m_modificationToSave = false;
        public bool ModificationToSave
        {
            get => m_modificationToSave;
            set
            {
                m_modificationToSave = value;
                if (value && ApplyBtn != null) ApplyBtn.BasicText = "Apply*";
                else ApplyBtn.BasicText = "Apply";
            }
        }
        public Div ApplyBtn;
        public void OnBackEvent()
        {
            if (PreventIngameKey)
            {
                EntryHook.CancelPreventIngameKeyevent(ClassUUID);
            }
            else EntryHook.PreventIngameKeyevent(ClassUUID);

            PreventIngameKey = !PreventIngameKey;
        }
        public void CloseMenu()
        {
            EntryHook.CancelPreventIngameKeyevent(ClassUUID);
            PreventIngameKey = false;
        }
        public static void InvokeConditionalDelegateChain(Delegate[] delegates)
        {
            foreach (Func<bool?> handler in delegates)
            {
                var res = handler();
                if (!res.HasValue || res.Value) continue;
                return;
            }
        }
        public static string RemoveModificationIdentifier(string str, char identifier = '*')
        {
            if (str.EndsWith(identifier)) str = str.Substring(0, str.Length - 1);
            return str;
        }
        public class TopbarButton : Div
        {
            public int Border_Bottom = 3;
            public bool ButtonOn = false;
            public List<TopbarButton> topbarButtons;
            public Vector2[] BorderBottomRect;
            public GamePauseMenu MainMenu;
            public TopbarButton(int Width, int Height, string BasicText, string Name, PrimitivesRenderer2D primitivesRenderer2D, ComponentPlayer player, List<TopbarButton> tb, GamePauseMenu gamePauseMenu = null) : base(Width, Height, Name)
            {
                //Div ScreenTerminal = new Div(80,50,"ScreenTerminal");
                topbarButtons = tb;
                tb.Add(this);
                this.BasicText = BasicText;
                this.Player = player;
                ClickEmit = true;
                Margin_Top = 10;
                Inline = true;
                //Margin_Left = 10;
                Margin_Right = 10;
                ApplyBasicAnimation(this, primitivesRenderer2D);
                ApplyBackground(this);
                ApplyClickBg(this);
                ApplyClickAudio(this, player);
                //this.AssociatedDivs = ads;
                this.MainMenu = gamePauseMenu;
            }
            public override void OnCalculatePosition()
            {
                base.OnCalculatePosition();
                BorderBottomRect = new Vector2[2] { new Vector2(0, Height - Border_Bottom), new Vector2(Width, Height) };
            }
            public override void OnMouseDown(int x, int y)
            {
                base.OnMouseDown(x, y);
                ButtonOn = true;
                foreach (var btn in topbarButtons)
                {
                    if (btn != this) btn.ButtonOn = false;
                }
                var AssociatedDivs = MainMenu.AssociatedDivs;
                if (AssociatedDivs != null)
                {
                    foreach (var item in AssociatedDivs)
                    {
                        if (item.Key == this)
                        {
                            if (!item.Value.Visible)
                            {
                                //此处存在问题 在关联元素被卸载后将无法获取关联元素的刷新计算请求，需要本节点向上层请求刷新
                                item.Key.ParentCalculateRequire = true;
                                item.Value.Visible = true;
                            }
                            continue;
                        }
                        if (item.Value.Visible) item.Value.Visible = false;
                    }
                }
            }
            public override void OnDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                base.OnDraw(primitivesRenderer2D);
                if (ButtonOn)
                {
                    FlatBatch2D flatBatch2D = primitivesRenderer2D.FlatBatch();
                    var p1 = BorderBottomRect[0];
                    var p2 = BorderBottomRect[1];
                    flatBatch2D.QueueQuad(ToAbsoluteRect(p1), ToAbsoluteRect(p2), 0, new Color(148, 228, 211, 255));
                    primitivesRenderer2D.Flush();
                }
            }
        }

        public class SettingSwitchBar : SwitchBar, IDivValueComfirmable
        {
            public event Func<bool?> OnValueConfirm;
            public event Func<bool?> OnValueCancel;
            public bool ValueModified = false;
            public void ConfirmValue() => InvokeConditionalDelegateChain(OnValueConfirm.GetInvocationList());
            public void CancelValue() => InvokeConditionalDelegateChain(OnValueCancel.GetInvocationList());
            public object NewValue;
            //public Action OnModified;
            public GamePauseMenu MainMenu;
            public SettingSwitchBar(GamePauseMenu mainMenu, int defaultIndex = 0, int Width = 580, int Height = 44, string Name = "SwitchBar") : base(mainMenu.Player, Width, Height, Name)
            {
                OnValueConfirm += () =>
                {
                    if (!ValueModified) return false;
                    ValueModified = false;

                    Value = NewValue;
                    BasicText = RemoveModificationIdentifier(BasicText);
                    MainMenu.ModificationToSave = false;
                    return null;
                };
                OnValueCancel += () =>
                {
                    if (!ValueModified) return false;
                    ValueModified = false;

                    NewValue = Value;
                    SelectedIndex = (int)Value;
                    MainMenu.ModificationToSave = false;
                    BasicText = RemoveModificationIdentifier(BasicText);
                    return null;
                };
                OnValueChanged += (int selectedIndex) =>
                {
                    NewValue = selectedIndex;
                    if (!BasicText.EndsWith('*')) BasicText += "*";
                    ValueModified = true;
                    MainMenu.ModificationToSave = true;
                };
                SelectedIndex = defaultIndex;
                NewValue = SelectedIndex;
                Value = NewValue;
                MainMenu = mainMenu;
            }
            public override void SwitchBarDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                if (NewValue == null) return;
                FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);
                fontBatch2D.QueueText(ValueKeys[(int)NewValue], ToAbsoluteRect(TextPoint), 0, Color.White, TextAnchor.Right, BasicTextScale);
                primitivesRenderer2D.Flush();
            }
        }
        public class SettingSelectionBar : SelectionBar, IDivValueComfirmable
        {
            public event Func<bool?> OnValueConfirm;
            public event Func<bool?> OnValueCancel;
            public bool ValueModified = false;
            public void ConfirmValue() => InvokeConditionalDelegateChain(OnValueConfirm.GetInvocationList());
            public void CancelValue() => InvokeConditionalDelegateChain(OnValueCancel.GetInvocationList());
            public GamePauseMenu MainMenu;
            public object NewValue;
            public SettingSelectionBar(GamePauseMenu mainMenu, bool defaultValue = false, int Width = 580, int Height = 44, string Name = "SelectionBar") : base(Width, Height, Name)
            {
                OnValueConfirm += () =>
                {
                    if (!ValueModified) return false;
                    ValueModified = false;

                    Value = NewValue;
                    BasicText = RemoveModificationIdentifier(BasicText);
                    MainMenu.ModificationToSave = false;
                    return null;
                };
                OnValueCancel += () =>
                {
                    if (!ValueModified) return false;
                    ValueModified = false;

                    NewValue = Value;
                    ButtonOn = (bool)Value;
                    BasicText = RemoveModificationIdentifier(BasicText);
                    MainMenu.ModificationToSave = false;
                    return null;
                };
                OnValueChanged += (bool buttonOn) =>
                {
                    NewValue = buttonOn;
                    if (!BasicText.EndsWith('*')) BasicText += '*';
                    ValueModified = true;
                    MainMenu.ModificationToSave = true;
                };
                ButtonOn = defaultValue;
                NewValue = ButtonOn;
                Value = NewValue;
                MainMenu = mainMenu;
                ApplyClickAudio(this, mainMenu.Player);
            }
            public override void SelectionBarDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                if (NewValue == null) return;
                if ((bool)NewValue)
                {
                    DrawOutline(primitivesRenderer2D, new Color(148, 228, 211, 255));
                    DrawInnerBox(primitivesRenderer2D, new Color(148, 228, 211, 255));
                }
                else
                {
                    DrawOutline(primitivesRenderer2D, Color.White);
                }
            }
        }
        public class SettingSlidingBar : SlidingBar, IDivValueComfirmable
        {
            public event Func<bool?> OnValueConfirm;
            public event Func<bool?> OnValueCancel;
            public bool ValueModified = false;
            public void ConfirmValue() => InvokeConditionalDelegateChain(OnValueConfirm.GetInvocationList());
            public void CancelValue() => InvokeConditionalDelegateChain(OnValueCancel.GetInvocationList());
            public GamePauseMenu MainMenu;
            public object NewValue;
            public SettingSlidingBar(GamePauseMenu mainMenu, float defaultValue = 0f, int Width = 580, int Height = 44, string Name = "SlidingBar") : base(Width, Height, Name)
            {
                OnValueConfirm += () =>
                {
                    if (!ValueModified) return false;
                    ValueModified = false;

                    Value = NewValue;
                    BasicText = RemoveModificationIdentifier(BasicText);
                    MainMenu.ModificationToSave = false;
                    return null;
                };
                OnValueCancel += () =>
                {
                    if (!ValueModified) return false;
                    ValueModified = false;

                    NewValue = Value;
                    Data = (float)Value;
                    BasicText = RemoveModificationIdentifier(BasicText);
                    MainMenu.ModificationToSave = false;
                    return null;
                };
                OnValueChanged += (float data) =>
                {
                    NewValue = data;
                    if (!BasicText.EndsWith('*')) BasicText += "*";
                    ValueModified = true;
                    if (!MainMenu.ModificationToSave) MainMenu.ModificationToSave = true;
                };
                Data = defaultValue;
                NewValue = Data;
                Value = NewValue;
                MainMenu = mainMenu;
            }
            public override void SlidingBarDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                if (NewValue == null) return;
                float data = (float)NewValue;
                if (Focused)
                {
                    FlatBatch2D flatBatch2D = primitivesRenderer2D.FlatBatch();
                    var bRects = CreateBarBlockRect((data - DataMin) / (DataMax - DataMin));
                    flatBatch2D.QueueQuad(ToAbsoluteRect(BarRects[0]), ToAbsoluteRect(BarRects[1]), 0, Color.White);
                    flatBatch2D.QueueQuad(ToAbsoluteRect(bRects[0]), ToAbsoluteRect(bRects[1]), 0, Color.White);

                    FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);
                    fontBatch2D.QueueText(Math.Round(data, 2).ToString(), ToAbsoluteRect(RoughTextPoint), 0, Color.White, TextAnchor.Right, BasicTextScale);
                }
                else
                {
                    FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);
                    fontBatch2D.QueueText(Math.Round(data, 2) + DataUnit, ToAbsoluteRect(FullTextPoint), 0, Color.White, TextAnchor.Right, BasicTextScale);
                }
            }

        }
        public void SpreadModifiedEvents(bool cancel = false)
        {
            var divs = new List<Div>();
            GetRelativeDivs(divs, MainLayout);
            foreach (Div div in divs)
            {
                //if (div.BasicText == "视距") Debugger.Break();
                if (div is IDivValueComfirmable)
                {
                    if (cancel) (div as IDivValueComfirmable).CancelValue();
                    else (div as IDivValueComfirmable).ConfirmValue();
                }
            }
        }
        public void ShowVanillaMenu()
        {
            ComponentPlayer player = Entity.Components.Where(x => x is ComponentPlayer).FirstOrDefault() as ComponentPlayer;
            if (player == null) return;
            DialogsManager.ShowDialog(player.GuiWidget, new GameMenuDialog(player));
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            bmpFont = LabelWidget.BitmapFont;
            WindowSize[0] = Window.Size.X;
            WindowSize[1] = Window.Size.Y;
            Player = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            MainLayout = new Div(WindowSize[0], WindowSize[1], "Div_Main_Layout") { ScaleEnabled = false };
            //int[] layoutPos = ToCenter(MainLayout.Width, MainLayout.Height);
            //MainLayout.Left = layoutPos[0];
            //MainLayout.Top = layoutPos[1];
            //MainLayout.BackgroundColor = new Color(16, 16, 16, 160);

            LoadMainLayouts(MainLayout);


            Div OptionsBar = new Div(800, 80, "Options");
            OptionsBar.Margin_Left = 10;
            OptionsBar.Margin_Top = 10;
            OptionsBar.Margin_Right = 50;
            MainLayout.AppendChild(OptionsBar);

            Div SettingsArea = new Div(1000, 700, "SettingsArea");
            //SettingsArea.Margin_Top = 0;
            SettingsArea.Margin_Left = 20;
            OptionsBar.Next = SettingsArea;





            Div AreaGeneral = new Div(580, 700, "AreaGeneral");
            AreaGeneral.AnimateEnabled = true;
            AreaGeneral.Visible = false;
            //General.Margin_Top = 20;
            //General.Margin_Left = 20;
            //OptionsBar.Next = General;
            SettingsArea.AppendChild(AreaGeneral);
            Div Group1 = CreateGeneralGroup1(AreaGeneral);
            Div Group2 = CreateGeneralGroup2(Group1);
            Div Group3 = CreateGeneralGroup3(Group2);


            Div AreaModpack = new Div(580, 500, "AreaModpack");
            AreaModpack.AnimateEnabled = true;
            SettingsArea.AppendChild(AreaModpack);
            CreateModpackGroup1(AreaModpack);


            Div AreaGraphics = new Div(580, 500, "AreaGraphics");
            AreaGraphics.AnimateEnabled = true;
            AreaGraphics.Visible = false;
            SettingsArea.AppendChild(AreaGraphics);
            Div AreaGraphicsGroup1 = CreateGraphicsGroup1(AreaGraphics);
            Div AreaGraphicsGroup2 = CreateGraphicsGroup2(AreaGraphicsGroup1);


            Div AreaAudio = new Div(580, 500, "AreaAudio");
            AreaAudio.AnimateEnabled = true;
            AreaAudio.Visible = false;
            SettingsArea.AppendChild(AreaAudio);
            Div AreaAudioGroup1 = CreateAudioGroup1(AreaAudio);



            Div Quality = new Div(580, 500, "AreaQuality");
            Quality.AnimateEnabled = true;
            //Quality.Margin_Top = 20;
            //Quality.Margin_Left = 20;
            //Quality.BgColorDown = new Color(128,128,128,16);
            Quality.Visible = false;
            SettingsArea.AppendChild(Quality);

            Div QualityGroup1 = new Div(580, 200, "QualityGroup1");
            QualityGroup1.Margin_Bottom = 20;
            QualityGroup1.BasicText = "Quality List To Be Added";
            ApplyBackground(QualityGroup1);
            Quality.AppendChild(QualityGroup1);



            Div About = new Div(580, 500, "AreaAbout");
            About.AnimateEnabled = true;
            About.Visible = false;
            SettingsArea.AppendChild(About);
            Div AreaAboutGroup1 = CreateAboutGroup1(About);
            Div AreaAboutGroup2 = CreateAboutGroup2(AreaAboutGroup1);





            TopbarButton GlassModPack = new TopbarButton(80, 50, "整合包", "GlassModPackage", primitivesRenderer2D, Player, TopbarButtons, this);
            GlassModPack.Margin_Left = 10;
            GlassModPack.ButtonOn = true;
            AssociatedDivs.Add(GlassModPack, AreaModpack);
            OptionsBar.AppendChild(GlassModPack);

            TopbarButton GraphicsButton = new TopbarButton(70, 50, "画面", "GraphicsButton", primitivesRenderer2D, Player, TopbarButtons, this);
            GraphicsButton.Margin_Left = 4;
            AssociatedDivs.Add(GraphicsButton, AreaGraphics);
            GlassModPack.Next = GraphicsButton;

            TopbarButton AudioButton = new TopbarButton(70, 50, "音频", "AudioButton", primitivesRenderer2D, Player, TopbarButtons, this);
            AudioButton.Margin_Left = 4;
            AssociatedDivs.Add(AudioButton, AreaAudio);
            GraphicsButton.Next = AudioButton;


            //Div ScreenTerminal = new Div(80,50,"ScreenTerminal");
            TopbarButton ScreenTerminal = new TopbarButton(80, 50, "Debug", "ScreenTerminal", primitivesRenderer2D, Player, TopbarButtons, this);
            //ScreenTerminal.Margin_Left = 10;
            //ScreenTerminal.ButtonOn = true;
            //ScreenTerminal.AssociatedDiv = Group1;
            AssociatedDivs.Add(ScreenTerminal, AreaGeneral);
            //OptionsBar.AppendChild(ScreenTerminal);
            AudioButton.Next = ScreenTerminal;

            TopbarButton DisplaySetting = new TopbarButton(110, 50, "Quality", "DisplaySetting", primitivesRenderer2D, Player, TopbarButtons, this);
            AssociatedDivs.Add(DisplaySetting, Quality);
            ScreenTerminal.Next = DisplaySetting;

            TopbarButton DivGameSetting = new TopbarButton(170, 50, "Vanilla原版菜单", "DivGameSetting", primitivesRenderer2D, Player, TopbarButtons, this);
            DivGameSetting.MouseDownListener += (_) => ShowVanillaMenu();
            DisplaySetting.Next = DivGameSetting;

            TopbarButton DivAbout = new TopbarButton(120, 50, "关于模组", "DivAdvanced", primitivesRenderer2D, Player, TopbarButtons, this);
            AssociatedDivs.Add(DivAbout, About);
            DivGameSetting.Next = DivAbout;

            //TopbarButton DivPlayerSetting = new TopbarButton(130, 50, "WebTV Set", "DivPlayerSetting", primitivesRenderer2D, Player, TopbarButtons, AssociatedDivs);
            //DivAdvanced.Next = DivPlayerSetting;


        }











        public void LoadMainLayouts(Div MainLayout)
        {
            Div BuyUsACoffee = new Div(230, 44, "BuyUsACoffee");
            BuyUsACoffee.Right = 90;
            BuyUsACoffee.Top = 10;
            ApplyBackground(BuyUsACoffee);
            BuyUsACoffee.BasicText = "Buy us a coffee!";
            BuyUsACoffee.ClickEmit = true;
            BuyUsACoffee.MouseDownListener += (Div div) =>
            {
                string url = "https://github.com/EltanceX";
                try
                {
#if ANDROID
				Engine.Window.Activity.OpenLink(url);
#else
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
#endif
                }
                catch (Exception ex)
                {
                    ScreenLog.Info($"无法打开浏览器: {ex.Message}");
                }
            };
            ApplyClickBg(BuyUsACoffee);
            ApplyClickAudio(BuyUsACoffee, Player);
            MainLayout.AppendChild(BuyUsACoffee);


            Div CloseAd = new Div(60, 44, "CloseAd");
            CloseAd.Right = 10;
            CloseAd.Top = 10;
            ApplyBackground(CloseAd);
            CloseAd.BasicText = " X ";
            CloseAd.ClickEmit = true;
            ApplyClickBg(CloseAd);
            ApplyClickAudio(CloseAd, Player);
            MainLayout.AppendChild(CloseAd);


            Div DoneBtn = new Div(110, 50, "DoneBtn");
            DoneBtn.Right = 10;
            DoneBtn.Bottom = 20;
            ApplyBackground(DoneBtn);
            DoneBtn.BasicText = "Done";
            DoneBtn.BasicTextTop = 10;
            DoneBtn.BasicTextLeft = 30;
            DoneBtn.ClickEmit = true;
            DoneBtn.MouseDownListener += (_) =>
            {
                if (ModificationToSave)
                {
                    ModificationToSave = false;
                    SpreadModifiedEvents(cancel: true);
                    return;
                }
                CloseMenu();
            };
            ApplyClickBg(DoneBtn);
            ApplyClickAudio(DoneBtn, Player);
            MainLayout.AppendChild(DoneBtn);


            Div ApplyBtn = new Div(120, 50, "ApplyBtn");
            ApplyBtn.Right = 140;
            ApplyBtn.Bottom = 20;
            ApplyBackground(ApplyBtn);
            ApplyBtn.BasicText = "Apply";
            ApplyBtn.BasicTextTop = 10;
            ApplyBtn.BasicTextLeft = 30;
            ApplyBtn.ClickEmit = true;
            ApplyBtn.MouseDownListener += (_) =>
            {
                ModificationToSave = false;
                SpreadModifiedEvents();
            };
            this.ApplyBtn = ApplyBtn;

            ApplyClickBg(ApplyBtn);
            ApplyClickAudio(ApplyBtn, Player);
            MainLayout.AppendChild(ApplyBtn);
        }

        public Div CreateAudioGroup1(Div audio)
        {
            Div Group1 = new Div(580, 250, "GraphicsGroup1");
            Group1.Margin_Bottom = 20;
            ApplyBackground(Group1);
            audio.AppendChild(Group1);

            SettingSlidingBar SoundsVolume = new SettingSlidingBar(this, defaultValue: SettingsManager.SoundsVolume * 10);
            SoundsVolume.Margin_Bottom = 6;
            SoundsVolume.BasicText = "游戏音效";
            SoundsVolume.DataMax = 10;
            SoundsVolume.DataMin = 0;
            SoundsVolume.DataUnit = "";
            SoundsVolume.Granularity = 1;
            SoundsVolume.OnValueConfirm += () =>
            {
                SettingsManager.SoundsVolume = SoundsVolume.Data / 10;
                return null;
            };
            ApplySettingsColorAnimation(SoundsVolume, primitivesRenderer2D);
            Group1.AppendChild(SoundsVolume);

            SettingSlidingBar MusicVolume = new SettingSlidingBar(this, defaultValue: SettingsManager.MusicVolume * 10);
            MusicVolume.Margin_Bottom = 6;
            MusicVolume.BasicText = "游戏音乐";
            MusicVolume.DataMax = 10;
            MusicVolume.DataMin = 0;
            MusicVolume.DataUnit = "";
            MusicVolume.Granularity = 1;
            MusicVolume.OnValueConfirm += () =>
            {
                SettingsManager.MusicVolume = MusicVolume.Data / 10;
                return null;
            };
            ApplySettingsColorAnimation(MusicVolume, primitivesRenderer2D);
            SoundsVolume.Next = MusicVolume;


            return Group1;
        }

        public Div CreateGraphicsGroup1(Div graphics)
        {
            Div Group1 = new Div(580, 250, "GraphicsGroup1");
            Group1.Margin_Bottom = 20;
            ApplyBackground(Group1);
            graphics.AppendChild(Group1);

            SettingSlidingBar ViewDistance = new SettingSlidingBar(this, defaultValue: SettingsManager.VisibilityRange);
            ViewDistance.Margin_Bottom = 6;
            ViewDistance.BasicText = "视距";
            ViewDistance.DataMax = 1024;
            ViewDistance.DataMin = 32;
            ViewDistance.DataUnit = "方块";
            ViewDistance.Granularity = 16;
            ViewDistance.OnValueConfirm += () =>
            {
                SettingsManager.VisibilityRange = (int)ViewDistance.Data;
                return null;
            };
            ApplySettingsColorAnimation(ViewDistance, primitivesRenderer2D);
            Group1.AppendChild(ViewDistance);

            SettingSwitchBar FrameLimit = new SettingSwitchBar(this, defaultIndex: SettingsManager.PresentationInterval);
            FrameLimit.Margin_Bottom = 6;
            FrameLimit.BasicText = "帧率限制";
            FrameLimit.ValueKeys.Clear();
            FrameLimit.ValueKeys.Add("半垂直同步");
            FrameLimit.ValueKeys.Add("垂直同步");
            FrameLimit.ValueKeys.Add("不限制");
            FrameLimit.OnValueConfirm += () =>
            {
                SettingsManager.PresentationInterval = Math.Clamp(FrameLimit.SelectedIndex, 0, 2);
                Window.PresentationInterval = SettingsManager.PresentationInterval;
                return null;
            };
            ApplySettingsColorAnimation(FrameLimit, primitivesRenderer2D);
            ViewDistance.Next = FrameLimit;

            SettingSwitchBar Resolution = new SettingSwitchBar(this, defaultIndex: (int)SettingsManager.ResolutionMode);
            Resolution.Margin_Bottom = 6;
            Resolution.BasicText = "渲染分辨率";
            Resolution.ValueKeys.Clear();
            Resolution.ValueKeys.Add("低");
            Resolution.ValueKeys.Add("中");
            Resolution.ValueKeys.Add("高");
            Resolution.OnValueConfirm += () =>
            {
                SettingsManager.ResolutionMode = (ResolutionMode)Resolution.SelectedIndex;
                return null;
            };
            ApplySettingsColorAnimation(Resolution, primitivesRenderer2D);
            FrameLimit.Next = Resolution;

            SettingSelectionBar FPSCounter = new SettingSelectionBar(this, defaultValue: SettingsManager.DisplayFpsCounter);
            FPSCounter.Margin_Bottom = 6;
            FPSCounter.BasicText = "显示性能信息";
            FPSCounter.OnValueConfirm += () =>
            {
                SettingsManager.DisplayFpsCounter = FPSCounter.ButtonOn;
                return null;
            };
            ApplySettingsColorAnimation(FPSCounter, primitivesRenderer2D);
            Resolution.Next = FPSCounter;

            SettingSelectionBar FPSRibbon = new SettingSelectionBar(this, defaultValue: SettingsManager.DisplayFpsRibbon);
            FPSRibbon.Margin_Bottom = 6;
            FPSRibbon.BasicText = "显示性能图示";
            FPSRibbon.OnValueConfirm += () =>
            {
                SettingsManager.DisplayFpsRibbon = FPSRibbon.ButtonOn;
                return null;
            };
            ApplySettingsColorAnimation(FPSRibbon, primitivesRenderer2D);
            FPSCounter.Next = FPSRibbon;

            return Group1;
        }
        public Div CreateGraphicsGroup2(Div group1)
        {
            Div Group2 = new Div(580, 160, "GraphicsGroup2");
            Group2.Margin_Bottom = 20;
            ApplyBackground(Group2);
            group1.Next = Group2;


            SettingSlidingBar Brightness = new SettingSlidingBar(this, defaultValue: SettingsManager.Brightness);
            Brightness.Margin_Bottom = 6;
            Brightness.BasicText = "亮度";
            Brightness.DataMax = 1f;
            Brightness.DataMin = 0;
            Brightness.DataUnit = "光照";
            Brightness.Granularity = 0.1f;
            Brightness.OnValueConfirm += () =>
            {
                SettingsManager.Brightness = Brightness.Data;
                return null;
            };
            ApplySettingsColorAnimation(Brightness, primitivesRenderer2D);
            Group2.AppendChild(Brightness);

            SettingSlidingBar ViewAngle = new SettingSlidingBar(this, defaultValue: SettingsManager.ViewAngle * 100);
            ViewAngle.Margin_Bottom = 6;
            ViewAngle.BasicText = "视角";
            ViewAngle.DataMax = 200;
            ViewAngle.DataMin = 0;
            ViewAngle.DataUnit = "%";
            ViewAngle.Granularity = 1f;
            ViewAngle.OnValueConfirm += () =>
            {
                SettingsManager.ViewAngle = ViewAngle.Data / 100;
                return null;
            };
            ApplySettingsColorAnimation(ViewAngle, primitivesRenderer2D);
            Brightness.Next = ViewAngle;

            SettingSelectionBar SlopeDisplay = new SettingSelectionBar(this, defaultValue: EGlobal.SlopeEnabled);
            SlopeDisplay.Margin_Bottom = 6;
            SlopeDisplay.BasicText = "启用斜面显示[实验]";
            SlopeDisplay.OnValueConfirm += () =>
            {
                EGlobal.SlopeEnabled = SlopeDisplay.ButtonOn;
                return null;
            };
            ApplySettingsColorAnimation(SlopeDisplay, primitivesRenderer2D);
            ViewAngle.Next = SlopeDisplay;

            return Group2;
        }

        public Div CreateModpackGroup1(Div modpack)
        {
            Div Group1 = new Div(580, 400, "ModpackGroup1");
            Group1.Margin_Bottom = 20;
            ApplyBackground(Group1);
            modpack.AppendChild(Group1);

            Div ModpackIntroduce = new Div(580, 40, "ModpackIntroduce");
            ModpackIntroduce.Margin_Bottom = 6;
            ModpackIntroduce.BasicText = "World Of Diversity - Mystery World";
            Group1.AppendChild(ModpackIntroduce);


            SettingSlidingBar BgmusicVolume = new SettingSlidingBar(this, defaultValue: 1f);
            BgmusicVolume.Margin_Bottom = 6;
            BgmusicVolume.BasicText = "背景音乐音量";
            BgmusicVolume.DataMax = 2;
            BgmusicVolume.DataMin = 0;
            BgmusicVolume.DataUnit = "";
            BgmusicVolume.Granularity = 0.01f;
            BgmusicVolume.OnValueConfirm += () =>
            {
                var bgmusic = Project.Subsystems.Where(x => x is SubsystemBgMusic).FirstOrDefault() as SubsystemBgMusic;
                if (bgmusic == null) return null;
                bgmusic.SettingVolume = BgmusicVolume.Data;
                return null;
            };
            ApplySettingsColorAnimation(BgmusicVolume, primitivesRenderer2D);
            ModpackIntroduce.Next = BgmusicVolume;

            SettingSlidingBar GUIScale = new SettingSlidingBar(this, defaultValue: PixelScreen.Scale);
            GUIScale.Margin_Bottom = 6;
            GUIScale.BasicText = "GUI缩放";
            GUIScale.DataMax = 2.0f;
            GUIScale.DataMin = 0.6f;
            GUIScale.DataUnit = "";
            GUIScale.Granularity = 0.1f;
            GUIScale.OnValueConfirm += () =>
            {
                PixelScreen.Scale = GUIScale.Data;
                CalculateRequired = true;
                return null;
            };
            ApplySettingsColorAnimation(GUIScale, primitivesRenderer2D);
            BgmusicVolume.Next = GUIScale;


            var Terminal = Entity.Components.Where(x => x is ETerminal).FirstOrDefault() as ETerminal;
            SettingSelectionBar F3 = new SettingSelectionBar(this, defaultValue: Terminal == null ? false : Terminal.et_F3.enabled);
            F3.Margin_Bottom = 6;
            F3.BasicText = "游戏调试数据[F3]";
            F3.OnValueConfirm += () =>
            {
                var Terminal = Entity.Components.Where(x => x is ETerminal).FirstOrDefault() as ETerminal;
                if (Terminal != null)
                {
                    if (F3.ButtonOn) Terminal.et_F3.Enable();
                    else Terminal.et_F3.Disable();
                }
                return null;
            };
            ApplySettingsColorAnimation(F3, primitivesRenderer2D);
            GUIScale.Next = F3;

            SettingSelectionBar F1 = new SettingSelectionBar(this, defaultValue: ScreenLog.Enabled);
            F1.Margin_Bottom = 16;
            F1.BasicText = "游戏调试日志[F1]";
            F1.OnValueConfirm += () =>
            {
                ScreenLog.Enabled = F1.ButtonOn;
                return null;
            };
            ApplySettingsColorAnimation(F1, primitivesRenderer2D);
            F3.Next = F1;

            SettingSelectionBar ByTouch = new SettingSelectionBar(this, defaultValue: EGlobal.Platform == EGlobal.Platforms.Android);
            ByTouch.Margin_Bottom = 6;
            ByTouch.BasicText = "使用触摸控制";
            ByTouch.OnValueConfirm += () =>
            {
                Player.ComponentInput.IsControlledByTouch = ByTouch.ButtonOn;
                return null;
            };
            ApplySettingsColorAnimation(ByTouch, primitivesRenderer2D);
            F1.Next = ByTouch;

            SettingSlidingBar TouchOffsetX = new SettingSlidingBar(this, defaultValue: 0f);
            TouchOffsetX.Margin_Bottom = 6;
            TouchOffsetX.BasicText = "方向键X偏移[安卓]";
            TouchOffsetX.DataMax = 512;
            TouchOffsetX.DataMin = 0;
            TouchOffsetX.DataUnit = "";
            TouchOffsetX.Granularity = 1f;
            TouchOffsetX.OnValueConfirm += () =>
            {
                var gui = Entity.Components.Where(x => x is ComponentGui).FirstOrDefault() as ComponentGui;
                if (gui == null) return null;
                if (gui.MoveRoseWidget == null) return null;
                var TouchMoveRose = gui.MoveRoseWidget as TouchMoveRoseWidget;
                if (TouchMoveRose == null) return null;
                TouchMoveRose.Offset.X = TouchOffsetX.Data;
                return null;
            };
            ApplySettingsColorAnimation(TouchOffsetX, primitivesRenderer2D);
            ByTouch.Next = TouchOffsetX;

            SettingSlidingBar TouchOffsetY = new SettingSlidingBar(this, defaultValue: 0f);
            TouchOffsetY.Margin_Bottom = 6;
            TouchOffsetY.BasicText = "方向键Y偏移[安卓]";
            TouchOffsetY.DataMax = 512;
            TouchOffsetY.DataMin = 0;
            TouchOffsetY.DataUnit = "";
            TouchOffsetY.Granularity = 1f;
            TouchOffsetY.OnValueConfirm += () =>
            {
                var gui = Entity.Components.Where(x => x is ComponentGui).FirstOrDefault() as ComponentGui;
                if (gui == null) return null;
                if (gui.MoveRoseWidget == null) return null;
                var TouchMoveRose = gui.MoveRoseWidget as TouchMoveRoseWidget;
                if (TouchMoveRose == null) return null;
                TouchMoveRose.Offset.Y = -TouchOffsetY.Data;
                return null;
            };
            ApplySettingsColorAnimation(TouchOffsetY, primitivesRenderer2D);
            TouchOffsetX.Next = TouchOffsetY;


            return Group1;
        }

        public Div CreateGeneralGroup1(Div SettingsBar)
        {
            Div Group1 = new Div(580, 200, "Group1");
            Group1.Margin_Bottom = 20;
            ApplyBackground(Group1);
            SettingsBar.AppendChild(Group1);

            SettingSlidingBar RenderDistance = new SettingSlidingBar(this);
            RenderDistance.Margin_Bottom = 6;
            ApplySettingsColorAnimation(RenderDistance, primitivesRenderer2D);
            Group1.AppendChild(RenderDistance);

            SettingSlidingBar MaxShadowDistance = new SettingSlidingBar(this);
            MaxShadowDistance.Margin_Bottom = 6;
            MaxShadowDistance.BasicText = "最大阴影距离";
            ApplySettingsColorAnimation(MaxShadowDistance, primitivesRenderer2D);
            RenderDistance.Next = MaxShadowDistance;

            SettingSlidingBar SimulationDistance = new SettingSlidingBar(this);
            SimulationDistance.Margin_Bottom = 6;
            SimulationDistance.BasicText = "模拟距离";
            ApplySettingsColorAnimation(SimulationDistance, primitivesRenderer2D);
            MaxShadowDistance.Next = SimulationDistance;

            SettingSlidingBar Brightness = new SettingSlidingBar(this);
            Brightness.Margin_Bottom = 6;
            Brightness.BasicText = "亮度";
            Brightness.DataMax = 100;
            Brightness.Granularity = 1f;
            Brightness.DataUnit = "%";
            ApplySettingsColorAnimation(Brightness, primitivesRenderer2D);
            SimulationDistance.Next = Brightness;
            return Group1;
        }
        public Div CreateGeneralGroup2(Div Group1)
        {
            Div Group2 = new Div(580, 200, "Group2");
            Group2.Margin_Bottom = 20;
            ApplyBackground(Group2);
            Group1.Next = Group2;

            SettingSlidingBar GUIScale = new SettingSlidingBar(this);
            GUIScale.Margin_Bottom = 6;
            GUIScale.BasicText = "界面尺寸";
            GUIScale.DataMax = 4;
            GUIScale.Granularity = 1f;
            GUIScale.DataUnit = "x";
            ApplySettingsColorAnimation(GUIScale, primitivesRenderer2D);
            Group2.AppendChild(GUIScale);

            SettingSelectionBar FullScreen = new SettingSelectionBar(this);
            FullScreen.Margin_Bottom = 6;
            FullScreen.Player = Player;
            FullScreen.BasicText = "全屏";
            ApplySettingsColorAnimation(FullScreen, primitivesRenderer2D);
            GUIScale.Next = FullScreen;

            SettingSelectionBar VSync = new SettingSelectionBar(this);
            VSync.Margin_Bottom = 6;
            VSync.Player = Player;
            VSync.BasicText = "垂直同步";
            ApplySettingsColorAnimation(VSync, primitivesRenderer2D);
            FullScreen.Next = VSync;

            SettingSlidingBar MaxFramerate = new SettingSlidingBar(this);
            MaxFramerate.Margin_Bottom = 6;
            MaxFramerate.BasicText = "最大帧率";
            MaxFramerate.DataMax = 165;
            MaxFramerate.Granularity = 1f;
            MaxFramerate.DataUnit = " fps";
            ApplySettingsColorAnimation(MaxFramerate, primitivesRenderer2D);
            VSync.Next = MaxFramerate;
            return Group2;
        }
        public Div CreateGeneralGroup3(Div Group2)
        {
            Div Group3 = new Div(580, 140, "Group3");
            ApplyBackground(Group3);
            Group2.Next = Group3;

            SettingSelectionBar ViewBobbing = new SettingSelectionBar(this);
            ViewBobbing.Margin_Bottom = 6;
            ViewBobbing.Player = Player;
            ViewBobbing.BasicText = "视角摇晃";
            ApplySettingsColorAnimation(ViewBobbing, primitivesRenderer2D);
            Group3.AppendChild(ViewBobbing);

            SettingSwitchBar AttackIndicator = new SettingSwitchBar(this);
            ApplySettingsColorAnimation(AttackIndicator, primitivesRenderer2D);
            AttackIndicator.ValueKeys.Clear();
            AttackIndicator.ValueKeys.Add("十字准星");
            AttackIndicator.ValueKeys.Add("快捷栏");
            AttackIndicator.ValueKeys.Add("关");
            AttackIndicator.BasicText = "攻击指示器";
            ViewBobbing.Next = AttackIndicator;

            SettingSelectionBar AutosaveIndicator = new SettingSelectionBar(this);
            AutosaveIndicator.Margin_Bottom = 6;
            AutosaveIndicator.Player = Player;
            AutosaveIndicator.BasicText = "自动保存";
            ApplySettingsColorAnimation(AutosaveIndicator, primitivesRenderer2D);
            AttackIndicator.Next = AutosaveIndicator;

            //SelectionBar selb = new SelectionBar();
            //selb.Margin_Bottom = 6;
            //selb.Player = Player;
            //ApplySettingsColorAnimation(selb,primitivesRenderer2D);
            //SimulationDistance.Next = selb;

            //SwitchBar swb = new SwitchBar(Player);
            //ApplySettingsColorAnimation(swb,primitivesRenderer2D);
            //selb.Next = swb;
            return Group3;
        }


        public Div CreateAboutGroup1(Div about)
        {
            Div Group1 = new Div(580, 100, "AboutGroup1");
            Group1.Margin_Bottom = 20;
            ApplyBackground(Group1);
            about.AppendChild(Group1);

            Div ModpackIntroduce = new Div(580, 60, "ModpackIntroduce");
            ModpackIntroduce.Margin_Bottom = 6;
            ModpackIntroduce.BasicText = "欢迎来到 多样性世界之神秘冒险\n EltanceX 2025-4-25";
            Group1.AppendChild(ModpackIntroduce);

            return Group1;
        }
        public Div CreateAboutGroup2(Div Group1)
        {
            Div Group2 = new Div(580, 200, "Group2");
            Group2.Margin_Bottom = 20;
            ApplyBackground(Group2);
            Group1.Next = Group2;

            Div StructureContributor = new Div(580, 200, "StructureContributor");
            StructureContributor.Margin_Bottom = 6;
            StructureContributor.BasicText = "特别感谢\n\n遗迹、村落结构贡献者: \ntp_16b \n\n冒险家: YOU!";
            //StructureContributor.Animations.
            StructureContributor.ClickEmit = true;
            ApplyColorAnimation(StructureContributor, primitivesRenderer2D, new Color(30, 30, 30, 120), new Color(106, 168, 216, 160));
            ApplyClickBg(Group2, new Color(106, 168, 216, 0));
            Group2.AppendChild(StructureContributor);

            return Group2;
        }

    }
}
