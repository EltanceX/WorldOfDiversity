using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;
using Game;
using DebugMod;
using GlassMod;

namespace GMenuMod
{
    public interface IDivValueComfirmable
    {
        public event Func<bool?> OnValueConfirm;
        public event Func<bool?> OnValueCancel;
        public void ConfirmValue();
        public void CancelValue();
    }
    public class PixelScreen : Component, IDrawable, IUpdateable
    {
        public static bool Debugging = false;
        public static float Scale = 1.0f;
        public static int MaxRecursionLevel = 16;
        public bool PreventIngameKey = false;
        public string ClassUUID = util.uuid();
        public bool MenuOpened { get => PreventIngameKey; }

        public ComponentPlayer Player;
        public int[] m_drawOrders = new int[1] { 2001 };

        UpdateOrder IUpdateable.UpdateOrder => UpdateOrder.Default;
        public int[] DrawOrders => m_drawOrders;


        public PrimitivesRenderer2D primitivesRenderer2D = new PrimitivesRenderer2D();
        //public Layout layout;
        public double LastCalcTime = 0;
        public Div MainLayout;
        public List<Div> DivList = new List<Div>();
        public static BitmapFont bmpFont;
        public static int[] WindowSize = [0, 0];
        public bool CalculateRequired = false;

        public static void GetRelativeDivs(List<Div> divs, Div div, int levels = 0)
        {
            divs.Add(div);
            if (levels >= MaxRecursionLevel) return;
            if (div.Next != null)
            {
                GetRelativeDivs(divs, div.Next, levels + 1);
            }
            foreach (var item in div.Childs)
            {
                GetRelativeDivs(divs, item, levels + 1);
            }
        }

        public static int[] ToCenter(int CenterBoxWidth, int CenterBoxHeight)
        {
            //修正分辨率
            var WindowSize = Window.Size;
            int Top = (int)((WindowSize.Y - CenterBoxHeight) / 2);
            int Left = (int)((WindowSize.X - CenterBoxWidth) / 2);
            return new int[] { Left, Top };
        }
        //public class Layout : Div
        //{
        //	public List<Div> OptionsBar;
        //	public List<Div> DataBar;
        //	public Layout(int width,int height) : base(width,height)
        //	{
        //		base.Width = width;
        //		base.Height = height;
        //	}
        //}
        public enum PositionMode
        {
            Relative,
            Absolute
        }
        public enum AnimationType
        {
            ColorTransform,
            PositionTransform,
            CustomTransform
        }
        public class DivAnimation
        {
            public double StartMillionSeconds;
            public int ColorTransitionTime = 500;
            public int PositionTransitionTime = 300;
            public float ColorProcess = 0;
            public PrimitivesRenderer2D Renderer2D;
            public AnimationType type;
            public bool HoverBegin = false;
            public Color SColor = new Color(128, 128, 128, 0);
            public Color ToColor = new Color(32, 32, 32, 128);
            public int AniX = 0;
            public int AniY = 0;
            public int ToAniX = 0;
            public int ToAniY = -6;
            public float PositionProcess = 0;
            public Div div;
            public delegate void Presets(DivAnimation divAnimation);
            public List<Presets> PresetList = new List<Presets>();

            public class AnimationFunction
            {
                public static float EaseOut(float t, float b, float c, float d)
                {
                    return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
                }
                public static float EaseOutQuad(float x)
                {
                    return 1 - (1 - x) * (1 - x);
                }
            }

            public static void ColorTransformPreset(DivAnimation divAnimation)
            {
                float process = divAnimation.ColorProcess;
                Color ToColor = divAnimation.ToColor;
                Color SColor = divAnimation.SColor;
                //int r = 128 - (int)(128 * process);
                int r = SColor.R + (int)((ToColor.R - SColor.R) * process);
                int g = SColor.G + (int)((ToColor.G - SColor.G) * process);
                int b = SColor.B + (int)((ToColor.B - SColor.B) * process);
                int a = SColor.A + (int)((ToColor.A - SColor.A) * process);

                var color = new Color(r, g, b, a);
                //divAnimation.color = color;
                PrimitivesRenderer2D Renderer2D = divAnimation.Renderer2D;
                Div div = divAnimation.div;

                FlatBatch2D flatBatch2D = divAnimation.Renderer2D.FlatBatch();
                flatBatch2D.QueueQuad(new Vector2(div.MCX, div.MCY), new Vector2(div.MCX + div.Width, div.MCY + div.Height), 0, color);
                Renderer2D.Flush();
            }
            public static void PositionTransformPreset(DivAnimation divAnimation)
            {
                Div div = divAnimation.div;
                float process = divAnimation.PositionProcess;
                int AniX = divAnimation.ToAniX;
                int AniY = divAnimation.ToAniY;
                div.AniX = (int)(process * AniX);
                div.AniY = (int)(process * AniY);
            }

            public void DrawOnHover(Div div)
            {
                if (!HoverBegin)
                {
                    HoverBegin = true;
                    StartMillionSeconds = util.getTime();
                    //color = new Color(0, 0, 0);
                }
                ColorProcess = (float)(util.getTime() - StartMillionSeconds) / ColorTransitionTime;
                //ColorProcess = AnimationFunction.EaseOut(ColorProcess, 0, 1, ColorTransitionTime / 1000);
                ColorProcess = Math.Clamp(ColorProcess, 0, 1);
                ColorProcess = AnimationFunction.EaseOutQuad(ColorProcess);

                PositionProcess = (float)(util.getTime() - StartMillionSeconds) / PositionTransitionTime;
                PositionProcess = Math.Clamp(PositionProcess, 0, 1);
                PositionProcess = AnimationFunction.EaseOutQuad(PositionProcess);
                //PositionProcess = AnimationFunction.EaseOut(PositionProcess, 0, 1, PositionTransitionTime / 1000);

                foreach (var preset in PresetList)
                {
                    preset.Invoke(this);
                }
                //color = new Color((int)(255 * process),0,0,128);
                //FlatBatch2D flatBatch2D = Renderer2D.FlatBatch();
                //flatBatch2D.QueueQuad(new Vector2(div.MCX,div.MCY),new Vector2(div.MCX + div.Width,div.MCY + div.Height),0,color);
                //Renderer2D.Flush();
            }
            public void OnMouseDown()
            {

            }
            public void OnLeave()
            {
                HoverBegin = false;
                StartMillionSeconds = 0;
                div.AniX = 0;
                div.AniY = 0;
            }
            public DivAnimation(PrimitivesRenderer2D renderer2D, AnimationType type, Div div)
            {
                this.Renderer2D = renderer2D;
                this.type = type;
                this.div = div;
            }
        }
        public class Div
        {
            public PositionMode positionMode = PositionMode.Relative;
            public string Name = "";
            public int m_height;
            public int m_width;
            //public float Scale = 1.0f;
            public bool ScaleEnabled = true;
            public int Height { get { return ScaleEnabled ? (int)(m_height * Scale) : m_height; } set { m_height = value; } }
            public int Width { get { return ScaleEnabled ? (int)(m_width * Scale) : m_width; } set { m_width = value; } }
            public int Left = 0;
            public int Top = 0;
            public int Margin_Left = 0;
            public int Margin_Right = 0;
            public int Margin_Top = 0;
            public int Margin_Bottom = 0;
            public bool AnimateEnabled = false;
            public int AniX = 0;
            public int AniY = 0;
            public bool m_visible = true;
            public int m_Right = 0;
            public int m_Bottom = 0;
            public int Right { get { return ScaleEnabled ? (int)(m_Right * Scale) : m_Right; } set { m_Right = value; } }
            public int Bottom { get { return ScaleEnabled ? (int)(m_Bottom * Scale) : m_Bottom; } set { m_Bottom = value; } }
            public object Value;


            /// <summary>
            /// Upload to Parent while SelectingTargets
            /// </summary>
            public bool ParentCalculateRequire = false;
            public bool Visible { get { return m_visible; } set { m_visible = value; ParentCalculateRequire = true; } }
            public Div m_Next;
            public Div Last;
            public Div Next { get { return m_Next; } set { m_Next = value; value.Last = this; } }
            public List<Div> Childs = new List<Div>();
            public Div Parent;
            public void AppendChild(Div child)
            {
                Childs.Add(child);
                child.Parent = this;
            }
            public string BasicText = "";
            public Vector2 m_BasicTextScale = new Vector2(1f);
            public Vector2 BasicTextScale { get { return ScaleEnabled ? m_BasicTextScale * Scale : m_BasicTextScale; } set { m_BasicTextScale = value; } }
            public int BasicTextLeft = 10;
            public int BasicTextTop = 6;
            public TextAnchor BasicTextAnchor = TextAnchor.Default;
            public Color BasicTextColor = Color.White;

            public int[] LastMouseMovePosition = { 0, 0 };
            public bool AudioEnabled = false;
            public string AudioPath = "Audio/McBtndown";
            public ComponentPlayer Player;

            public Color m_BackgroundColor;
            public bool BgColorEnabled = false;
            public Color BackgroundColor
            {
                get { return m_BackgroundColor; }
                set
                {
                    m_BackgroundColor = value;
                    BgColorEnabled = value.A != 0;
                }
            }

            public bool BgColorMouseDownEnabled = false;
            public Color m_BgColorDown;
            public Color BgColorDown
            {
                get { return m_BgColorDown; }
                set
                {
                    m_BgColorDown = value;
                    BgColorMouseDownEnabled = value.A != 0;
                }
            }


            public int CX = 0;
            public int CY = 0;
            public int ZIndex = 0;
            public bool Calculated = false;
            public bool Inline = false;
            public bool ClickEmit = false;
            public bool Focused = false;
            public bool MouseDown = false;



            public List<DivAnimation> m_animations = new List<DivAnimation>();
            public List<DivAnimation> Animations { get { return m_animations; } set { m_animations = value; } }
            public delegate void BeforeFrame();
            public List<BeforeFrame> beforeFrames = new List<BeforeFrame>();
            //public delegate void MouseDownListenerFunc(Div div);
            //public MouseDownListenerFunc MouseDownListener;
            public event Action<Div> MouseDownListener;


            public Texture2D Texture;
            public Div(int Width, int Height, string Name = "")
            {
                this.Width = Width;
                this.Height = Height;
                this.Name = Name;
            }
            public int MCX
            {
                get { return CX + Margin_Left + (AnimateEnabled ? AniX : 0); }
            }
            public int MCY
            {
                get { return CY + Margin_Top + (AnimateEnabled ? AniY : 0); }
            }
            public virtual void UpdateAnimation()
            {
                if (!AnimateEnabled) return;

            }
            public virtual void PlayAudio(ComponentPlayer player)
            {
                if (!AudioEnabled || Player == null) return;
                var subsystemAudio = player.ComponentBody.m_subsystemAudio;
                var componentBody = player.ComponentBody;
                try
                {
                    subsystemAudio.PlaySound(AudioPath, 1f, 0f, componentBody.Position, 4f, autoDelay: true);
                }
                catch (Exception e)
                {
                    ScreenLog.Error(e);
                }
            }
            public virtual void DrawAnimation(PrimitivesRenderer2D primitivesRenderer2D)
            {
                foreach (var animation in m_animations)
                {
                    if (Focused) animation.DrawOnHover(this);
                }
            }
            public virtual void DrawText(PrimitivesRenderer2D primitivesRenderer2D)
            {
                FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);

                //if(Focused)
                //fontBatch2D.Font = bmpFont;
                //fontBatch2D.
                //fontBatch2D.QueueText(Name, new Vector2(MCX,MCY), 0, Color.Orange, TextAnchor.Default, new Vector2(1,1));
                //fontBatch2D.
                if (BasicText != string.Empty)
                    fontBatch2D.QueueText(BasicText, new Vector2(MCX + BasicTextLeft, MCY + BasicTextTop), 0, BasicTextColor, BasicTextAnchor, BasicTextScale);
            }
            public virtual void DrawBackground(PrimitivesRenderer2D primitivesRenderer2D)
            {
                FlatBatch2D flatBatch2D = primitivesRenderer2D.FlatBatch();
                if (MouseDown && BgColorMouseDownEnabled)
                    flatBatch2D.QueueQuad(new Vector2(MCX, MCY), new Vector2(MCX + Width, MCY + Height), 0, BgColorDown);
                else if (BgColorEnabled) flatBatch2D.QueueQuad(new Vector2(MCX, MCY), new Vector2(MCX + Width, MCY + Height), 0, BackgroundColor);
            }
            public virtual void OnDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                if (AnimateEnabled) DrawAnimation(primitivesRenderer2D);
                DrawBackground(primitivesRenderer2D);
                FlatBatch2D flatBatch2D = primitivesRenderer2D.FlatBatch();
                Vector2[] RectToDraw = { new Vector2(MCX, MCY), new Vector2(MCX + Width, MCY + Height) };
                //flatBatch2D.QueueLine(new Vector2(0,0),new Vector2(100,100),1,Color.Yellow);
                //flatBatch2D.QueueRectangle(RectToDraw[0],RectToDraw[1],1,Color.Cyan);
                if (Debugging) flatBatch2D.QueueRectangle(RectToDraw[0], RectToDraw[1], 0, Color.Red);
                //flatBatch2D.QueueQuad(RectToDraw[0],RectToDraw[1],0,new Color(255,118,117,80));
                //var tb2d = primitivesRenderer2D.TexturedBatch(VKBD_Texture);//, true,1);
                //tb2d.QueueQuad(new Vector2(0 + Left,0 + Top),new Vector2(TextureSize.X + Left,TextureSize.Y + Top),0,new Vector2(0,0),new Vector2(1,1),Color.White);
                DrawText(primitivesRenderer2D);
                //tb2d.Flush();
                //fontBatch2D.Font = new BitmapFont()"77767qjanamms";
                //fontBatch2D.QueueBatch(fontBatch2D);
                primitivesRenderer2D.Flush();
            }

            /// <summary>
            /// Must be used when cursor is in a element
            /// </summary>
            /// <param name="x">Mouse X</param>
            /// <param name="y">Mouse Y</param>
            /// <returns>Relative Pos</returns>
            public virtual int[] ToRelativePos(int x, int y)
            {
                return new int[2] { x - MCX, y - MCY };
            }
            public virtual Vector2 ToAbsoluteRect(Vector2 vec2)
            {
                return new Vector2(vec2.X + MCX, vec2.Y + MCY);
            }
            public virtual void OnMousePositionChanged(int x, int y)
            {
#if GDEBUG
                ScreenLog.Info($"Position: {Name} {x} {y}");
#endif
                if (!Focused) Focused = true;

            }
            public virtual void OnMouseDown(int x, int y)
            {
#if GDEBUG
                ScreenLog.Info($"MouseDown {Name} {x} {y}");
#endif
                int rx = x - MCX;
                int ry = y - MCY;
                MouseDown = true;

                MouseDownListener?.Invoke(this);
            }
            public virtual void OnMouseUp(int x, int y)
            {
#if GDEBUG
                ScreenLog.Info($"MouseUp {Name} {x} {y}");
#endif
                MouseDown = false;
            }
            public virtual void OnMouseLeave()
            {
#if GDEBUG
                ScreenLog.Info($"MouseLeave {Name}");
#endif
                if (AnimateEnabled)
                {
                    foreach (var animation in m_animations)
                    {
                        animation.OnLeave();
                    }
                }
            }
            public virtual void OnCalculatePosition()
            {

            }
        }
        public class SwitchBar : Div
        {

            public List<string> ValueKeys = new List<string>();
            //public Dictionary<string, int> ValueKeys2 = new Dictionary<string, int>();
            public int SelectedIndex = -1;
            public int sRight = 20;
            public int sTop = 4;
            public Action<int> OnValueChanged;
            public Vector2 TextPoint { get { return new Vector2(Width - sRight, sTop); } }
            public SwitchBar(ComponentPlayer Player, int Width = 580, int Height = 44, string Name = "SwitchBar") : base(Width, Height, Name)
            {
                ValueKeys.Add("Default");
                ValueKeys.Add("A");
                ValueKeys.Add("B");
                ValueKeys.Add("C");
                SelectedIndex = 0;
                ClickEmit = true;
                BasicText = "Switch";
                ApplyClickAudio(this, Player);
            }
            public override void OnMouseDown(int x, int y)
            {
                base.OnMouseDown(x, y);
                if (++SelectedIndex >= ValueKeys.Count)
                {
                    SelectedIndex = 0;
                }
                OnValueChanged?.Invoke(SelectedIndex);
            }
            public virtual void SwitchBarDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);
                fontBatch2D.QueueText(ValueKeys[SelectedIndex], ToAbsoluteRect(TextPoint), 0, Color.White, TextAnchor.Right, BasicTextScale);
                primitivesRenderer2D.Flush();
            }
            public override void OnDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                base.OnDraw(primitivesRenderer2D);
                SwitchBarDraw(primitivesRenderer2D);
            }
        }
        public class SelectionBar : Div
        {
            public int m_sHeight = 23;
            public int m_sWidth = 23;
            public int m_sRight = 20;
            public int m_BlockSmallSize = 11;
            public int m_BlockBorderWidth = 3;

            public int sHeight { get { return ScaleEnabled ? (int)(m_sHeight * Scale) : m_sHeight; } set { m_sHeight = value; } }
            public int sWidth{ get { return ScaleEnabled ? (int)(m_sWidth * Scale) : m_sWidth; } set { m_sWidth = value; } }
            public int sRight{ get { return ScaleEnabled ? (int)(m_sRight * Scale) : m_sRight; } set { m_sRight = value; } }
            public int BlockSmallSize{ get { return ScaleEnabled ? (int)(m_BlockSmallSize * Scale) : m_BlockSmallSize; } set { m_BlockSmallSize = value; } }
            public int BlockBorderWidth{ get { return ScaleEnabled ? (int)(m_BlockBorderWidth * Scale) : m_BlockBorderWidth; } set { m_BlockBorderWidth = value; } }

            public bool ButtonOn = false;
            public Vector2[] SquareRect;

            public int MistakeAllowed = 3;
            public event Action<bool> OnValueChanged;

            /// <summary>
            /// 相对于外方块左上角
            /// </summary>
            public int[] CenterPoint { get { return new int[2] { sWidth / 2, sHeight / 2 }; } }
            /// <summary>
            /// 相对于外方块左上角
            /// </summary>
            public int[] InnerLeftTopPoint { get { return new int[2] { CenterPoint[0] - BlockSmallSize / 2, CenterPoint[1] - BlockSmallSize / 2 }; } }

            public Vector2[] CreateSquareRect()
            {
                int rx = Width - sRight - sWidth;
                int ry = (Height - sHeight) / 2;
                return new Vector2[2] { new Vector2(rx, ry), new Vector2(rx + sWidth, ry + sHeight) };
            }
            public SelectionBar(int Width = 580, int Height = 44, string Name = "SelectionBar") : base(Width, Height, Name)
            {
                BasicText = "亮度";
                ClickEmit = true;
                AudioEnabled = true;
            }
            public bool WithinButtonArea(int rx, int ry)
            {
                if (rx > SquareRect[0].X - MistakeAllowed && ry > SquareRect[0].Y - MistakeAllowed && rx < SquareRect[1].X + MistakeAllowed && ry < SquareRect[1].Y + MistakeAllowed) return true;
                return false;
            }
            public override void OnCalculatePosition()
            {
                base.OnCalculatePosition();
                SquareRect = CreateSquareRect();
            }
            public virtual void SelectionBarDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                if (ButtonOn)
                {
                    DrawOutline(primitivesRenderer2D, new Color(148, 228, 211, 255));
                    DrawInnerBox(primitivesRenderer2D, new Color(148, 228, 211, 255));
                }
                else
                {
                    DrawOutline(primitivesRenderer2D, Color.White);
                }
            }
            public override void OnDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                base.OnDraw(primitivesRenderer2D);
                SelectionBarDraw(primitivesRenderer2D);
            }
            public void DrawOutline(PrimitivesRenderer2D primitivesRenderer2D, Color color)
            {
                FlatBatch2D flatBatch = primitivesRenderer2D.FlatBatch();
                //  a b          e f
                //  c d          g h
                //  
                //  
                //  i j          m n
                //  k l          o p
                var p1 = SquareRect[0]; //a
                var p2 = new Vector2(SquareRect[1].X, SquareRect[0].Y + BlockBorderWidth);//h
                var p22 = new Vector2(SquareRect[1].X - BlockBorderWidth, SquareRect[0].Y);//e
                var p3 = new Vector2(SquareRect[0].X + BlockBorderWidth, SquareRect[1].Y);//l
                var p33 = new Vector2(SquareRect[0].X, SquareRect[1].Y - BlockBorderWidth);//i
                var p4 = SquareRect[1];//p
                flatBatch.QueueQuad(ToAbsoluteRect(p1), ToAbsoluteRect(p2), 0, color);//ah
                flatBatch.QueueQuad(ToAbsoluteRect(p1), ToAbsoluteRect(p3), 0, color);//al
                flatBatch.QueueQuad(ToAbsoluteRect(p33), ToAbsoluteRect(p4), 0, color);//ip
                flatBatch.QueueQuad(ToAbsoluteRect(p22), ToAbsoluteRect(p4), 0, color);//ep
            }
            public void DrawInnerBox(PrimitivesRenderer2D primitivesRenderer2D, Color color)
            {
                FlatBatch2D flatBatch = primitivesRenderer2D.FlatBatch();

                int[] innerLeftTop = InnerLeftTopPoint;
                Vector2 p1 = new Vector2(SquareRect[0].X + innerLeftTop[0], SquareRect[0].Y + innerLeftTop[1]);
                Vector2 p2 = new Vector2(SquareRect[0].X + innerLeftTop[0] + BlockSmallSize, SquareRect[0].Y + innerLeftTop[1] + BlockSmallSize);
                flatBatch.QueueQuad(ToAbsoluteRect(p1), ToAbsoluteRect(p2), 0, color);
            }
            public override void OnMouseDown(int x, int y)
            {
                base.OnMouseDown(x, y);
                int[] rpos = ToRelativePos(x, y);
                if (WithinButtonArea(rpos[0], rpos[1]))
                {
                    ButtonOn = !ButtonOn;
                    PlayAudio(Player);
                    OnValueChanged.Invoke(ButtonOn);
                }
            }
        }
        public class SlidingBar : Div
        {
            public Vector2[] BarRects;
            public Vector2 BarBlockLeftTop;

            public int m_sHeight = 4;
            public int m_sWidth = 300;
            public int m_sRight = 20;
            public int m_sBlockWidth = 10;
            public int m_sBlockHeight = 20;
            public int m_sBlockLeft = 0;

            public int sHeight { get { return ScaleEnabled ? (int)(m_sHeight * Scale) : m_sHeight; } set { m_sHeight = value; } }
            public int sWidth { get { return ScaleEnabled ? (int)(m_sWidth * Scale) : m_sWidth; } set { m_sWidth = value; } }
            public int sRight { get { return ScaleEnabled ? (int)(m_sRight * Scale) : m_sRight; } set { m_sRight = value; } }
            public int sBlockWidth { get { return ScaleEnabled ? (int)(m_sBlockWidth * Scale) : m_sBlockWidth; } set { m_sBlockWidth = value; } }
            public int sBlockHeight { get { return ScaleEnabled ? (int)(m_sBlockHeight * Scale) : m_sBlockHeight; } set { m_sBlockHeight = value; } }
            public int sBlockLeft { get { return ScaleEnabled ? (int)(m_sBlockLeft * Scale) : m_sBlockLeft; } set { m_sBlockLeft = value; } }
            public int MistakeAllowed = 3;

            public float Data = 0;
            //public float DataProcess = 0;
            public float DataProcess { get { return (Data - DataMin) / (DataMax - DataMin); } }
            public float Granularity = 0.1f;
            //public float rGranularity { get { return Granularity / (DataMax - DataMin); } }
            //public float rData { get { return Data / (DataMax- DataMin); } }
            public float DataMax = 32;
            public float DataMin = 0;
            public string DataUnit = "个区块";

            public Vector2 FullTextPoint { get { return new Vector2(BarRects[1].X, 8); } }
            public Vector2 RoughTextPoint { get { return new Vector2(BarRects[0].X - MistakeAllowed - 10, 8); } }
            //public int sBlockTop = -10;
            public int sBlockTop { get { return -(sBlockHeight - sHeight) / 2; } }

            public event Action<float> OnValueChanged;

            public Vector2[] CreateSlideBarRect()
            {
                int rx = Width - (sWidth + sRight);
                int ry = (int)((Height - sHeight) / 2);
                return new Vector2[2] { new Vector2(rx, ry), new Vector2(rx + sWidth, ry + sHeight) };
            }
            public Vector2[] CreateBarBlockRect(float dataProcess)
            {
                int rx = (int)BarRects[0].X;
                int ry = (int)BarRects[0].Y;
                int processX = (int)(sWidth * dataProcess);
                return new Vector2[2] { new Vector2(rx + sBlockLeft + processX, ry + sBlockTop), new Vector2(rx + sBlockLeft + sBlockWidth + processX, ry + sBlockTop + sBlockHeight) };
            }
            public SlidingBar(int Width = 580, int Height = 44, string Name = "SlidingBar") : base(Width, Height, Name)
            {
                BasicText = "渲染距离";
                //BackgroundColor = Color.White;
                ClickEmit = true;
            }
            public override void OnCalculatePosition()
            {
                base.OnCalculatePosition();
                BarRects = CreateSlideBarRect();
                BarBlockLeftTop = BarRects[0];
            }
            public bool WithinSlidebarArea(int rx, int ry)
            {
                if (rx > BarRects[0].X - MistakeAllowed && ry > 0 && rx < BarRects[1].X + MistakeAllowed && ry < Height) return true;
                return false;
            }
            //public Vector2 GetBlockbarRectWithData()
            //{
            //}
            public virtual void SlidingBarDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                if (Focused)
                {
                    FlatBatch2D flatBatch2D = primitivesRenderer2D.FlatBatch();
                    var bRects = CreateBarBlockRect(DataProcess);
                    flatBatch2D.QueueQuad(ToAbsoluteRect(BarRects[0]), ToAbsoluteRect(BarRects[1]), 0, Color.White);
                    flatBatch2D.QueueQuad(ToAbsoluteRect(bRects[0]), ToAbsoluteRect(bRects[1]), 0, Color.White);

                    FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);
                    fontBatch2D.QueueText(Math.Round(Data, 2).ToString(), ToAbsoluteRect(RoughTextPoint), 0, Color.White, TextAnchor.Right);
                }
                else
                {
                    FontBatch2D fontBatch2D = primitivesRenderer2D.FontBatch(bmpFont);
                    fontBatch2D.QueueText(Math.Round(Data, 2) + DataUnit, ToAbsoluteRect(FullTextPoint), 0, Color.White, TextAnchor.Right);
                }
            }
            public override void OnDraw(PrimitivesRenderer2D primitivesRenderer2D)
            {
                base.OnDraw(primitivesRenderer2D);
                SlidingBarDraw(primitivesRenderer2D);
            }
            public void UpdateSlideData(int rx, int ry)
            {
                float w = rx - BarRects[0].X;
                float process = w / sWidth;
                process = Math.Clamp(process, 0f, 1f);
                Data = DataMin + process * (DataMax - DataMin);
#if GDEBUG
                ScreenLog.Info("Data " + Data);
#endif
                float DataOverflow = Data % Granularity;
                float DataLow = Data - Data % Granularity;
                if (DataOverflow > Granularity / 2) Data = DataLow + Granularity;
                else Data = DataLow;

                OnValueChanged.Invoke(Data);

                //DataProcess = Data / (DataMax - DataMin);
                //Data = Math.Clamp(Data,DataMin,DataMax);
            }
            public override void OnMousePositionChanged(int x, int y)
            {
                base.OnMousePositionChanged(x, y);
                //ScreenLog.Info($"PC "+MouseDown);
                if (MouseDown)
                {
                    int[] rpos = ToRelativePos(x, y);
                    if (WithinSlidebarArea(rpos[0], rpos[1]))
                    {
                        UpdateSlideData(rpos[0], rpos[1]);
                    }
                }
            }
            public override void OnMouseDown(int x, int y)
            {
                base.OnMouseDown(x, y);
                int[] rpos = ToRelativePos(x, y);
                if (WithinSlidebarArea(rpos[0], rpos[1]))
                {
                    UpdateSlideData(rpos[0], rpos[1]);
                }
            }
        }

        public static void CalculatePosition(Div div, List<Div> DivToDraw, int drawX = 0, int drawY = 0, int levels = 0)
        {
            if (div.Calculated) return;
            if (levels > MaxRecursionLevel) return;
            if (!div.Visible) return;
            div.OnCalculatePosition();
            if (div.Parent != null)
            {
                if (div.Right != 0) div.Left = div.Parent.Width - div.Right - div.Width;
                if (div.Bottom != 0) div.Top = div.Parent.Height - div.Bottom - div.Height;
            }
            div.CX = drawX + div.Left;
            div.CY = drawY + div.Top;
            int _x = drawX;
            int _y = drawY;
            drawX += div.Left + div.Margin_Left;
            drawY += div.Top + div.Margin_Top;
            DivToDraw.Add(div);
            div.ZIndex = levels;
            div.Calculated = true;
            div.ParentCalculateRequire = false;

            if (div.Next is Div)
            {
                //if(div.Next.positionMode == PositionMode.Relative)
                switch (div.Next.positionMode)
                {
                    case PositionMode.Relative:
                        //对下一个元素的影响
                        CalculatePosition(div.Next, DivToDraw, div.Next.Inline ? drawX + div.Width + div.Margin_Right : div.CX, div.Next.Inline ? div.CY : drawY + div.Height + div.Margin_Bottom, levels + 1);
                        break;
                    case PositionMode.Absolute:
                        CalculatePosition(div.Next, DivToDraw, drawX, drawY, levels + 1);
                        break;
                    default:
                        break;
                }
            }
            foreach (var child in div.Childs)
            {
                CalculatePosition(child, DivToDraw, div.MCX, div.MCY, levels + 1);
            }
        }
        public virtual void CalculateAnimation(Div div)
        {

        }
        public virtual void ProcessMouseMove(Div target, int x, int y)
        {
            if (target == null) return;
            //var mousePoint = GetMousePosition();
            var LastMouseMovePosition = target.LastMouseMovePosition;
            if (LastMouseMovePosition[0] == x && LastMouseMovePosition[1] == y) return;
            LastMouseMovePosition[0] = x;
            LastMouseMovePosition[1] = y;
            if (target != null) target.OnMousePositionChanged(x, y);
        }
        public virtual void ProcessMouseDown(Div target, int x, int y)
        {
            if (target != null) target.OnMouseDown(x, y);
        }
        public virtual void ProcessMouseUp(Div target, int x, int y)
        {
            if (target != null) target.OnMouseUp(x, y);
        }
        public virtual void ProcessUnfocus(Div target)
        {
            foreach (var div in DivList)
            {
                if (div != target && div.Focused)
                {
                    div.Focused = false;
                    div.OnMouseLeave();
                }
            }
        }
        public virtual void ProcessUnfocus(List<Div> targets)
        {
            foreach (var div in DivList)
            {
                if (!targets.Contains(div) && div.Focused)
                {
                    div.Focused = false;
                    div.MouseDown = false;
                    div.OnMouseLeave();
                }
            }
        }
        public virtual Div SelectTarget(int x, int y)
        {
            foreach (var div in DivList)
            {
                if (div.ClickEmit && x >= div.MCX && y >= div.MCY && x < div.MCX + div.Width && y < div.MCY + div.Height)
                {
                    return div;
                }
            }
            return null;
        }
        public virtual List<Div> SelectTargets(int x, int y)
        {
            List<Div> divs = new List<Div>();
            foreach (var div in DivList)
            {
                //是否存在刷新请求
                if (div.ParentCalculateRequire)
                {
                    CalculateRequired = true;
                    div.ParentCalculateRequire = false;
                }
                if (div.ClickEmit && x >= div.MCX && y >= div.MCY && x < div.MCX + div.Width && y < div.MCY + div.Height)
                {
                    divs.Add(div);
                }
            }
            return divs;
        }

        public virtual void Draw(Camera camera, int drawOrder)
        {
            if (!MenuOpened) return;
            //绘制更新

            foreach (var div in DivList)
            {
                if (div.Visible) div.OnDraw(primitivesRenderer2D);
            }
        }
        public virtual void DetectKeyboardOpenEvent()
        {
            if (Keyboard.IsKeyDownOnce(Key.Minus))
            {
                if (PreventIngameKey)
                {
                    EntryHook.CancelPreventIngameKeyevent(ClassUUID);
                }
                else EntryHook.PreventIngameKeyevent(ClassUUID);

                PreventIngameKey = !PreventIngameKey;
            }
        }
        public virtual void Update(float dt)
        {
            DetectKeyboardOpenEvent();


            double ms = util.getTime();
            //foreach(var item in DivList)
            //{

            //}
            if (CalculateRequired || ms - LastCalcTime > 1000 * 60 * 5)
            {
                CalculateRequired = false;
                LastCalcTime = ms;
                foreach (var div in DivList)
                {
                    div.Calculated = false;
                }
                DivList.Clear();
                CalculatePosition(MainLayout, DivList);
            }

            //窗口大小变化，更新DIV布局位置
            var wsize = Window.Size;
            if (WindowSize[0] != wsize.X || WindowSize[1] != wsize.Y)
            {
                ScreenLog.Info("Resizing...");
                WindowSize[0] = wsize.X;
                WindowSize[1] = wsize.Y;
                MainLayout.Width = wsize.X;
                MainLayout.Height = wsize.Y;
                //int[] layoutPos = ToCenter(MainLayout.Width,MainLayout.Height);
                //MainLayout.Left = layoutPos[0];
                //MainLayout.Top = layoutPos[1];
                foreach (var div in DivList)
                {
                    div.Calculated = false;
                }
                DivList.Clear();
                CalculatePosition(MainLayout, DivList);
            }

            var MousePosition = GetMousePosition();
            //if (Mouse.MousePosition == null || !MenuOpened) return;

            //int x = Mouse.MousePosition.Value.X;
            //int y = Mouse.MousePosition.Value.Y;

            if (MousePosition == null || !MenuOpened) return;
            int x = MousePosition.Value.X;
            int y = MousePosition.Value.Y;
            //ScreenLog.Info($"Stage 1 {x}, {y}");
            //Div selected = SelectTarget(x, y);
            List<Div> divs = SelectTargets(x, y);

            //鼠标更新
            //if(divs.Count >1)Debugger.Break();
            int i = 0;
            do
            {
                Div selected;
                if (divs.Count == 0) selected = null;
                else selected = divs[i];
                ProcessMouseMove(selected, x, y);
                //if (Mouse.IsMouseButtonDownOnce(MouseButton.Left))
                if (DetectMouseDown())
                {
                    ProcessMouseDown(selected, x, y);
                }
                //if (Mouse.IsMouseButtonUpOnce(MouseButton.Left))
                if (DetectMouseUp())
                {
                    ProcessMouseUp(selected, x, y);
                }
            } while (++i < divs.Count);
            ProcessUnfocus(divs);
        }
        public static Point2? GetMousePosition()
        {
            if (EGlobal.Platform == EGlobal.Platforms.Android)
            {
                if (Touch.TouchLocations.Count == 0) return null;
                var location = Touch.TouchLocations[0];
                //ScreenLog.Info($"Touch: {location.Position.X}, {location.Position.Y}");
                return new Point2((int)location.Position.X, (int)location.Position.Y);
            }
            if (Mouse.MousePosition == null) return null;
            return Mouse.MousePosition.Value;
        }
        public static bool DetectMouseDown()
        {
            if (EGlobal.Platform == EGlobal.Platforms.Android)
            {
                if (Touch.TouchLocations.Count == 0) return false;
                var location = Touch.TouchLocations[0];
                //ScreenLog.Info($"Down: {location.State}");
                if (location.State == TouchLocationState.Pressed) return true;
            }
            return Mouse.IsMouseButtonDownOnce(MouseButton.Left);
        }
        public static bool DetectMouseUp()
        {
            if (EGlobal.Platform == EGlobal.Platforms.Android)
            {
                if (Touch.TouchLocations.Count == 0) return false;
                var location = Touch.TouchLocations[0];
                //ScreenLog.Info($"Up: {location.State}");
                if (location.State == TouchLocationState.Released) return true;
            }
            return Mouse.IsMouseButtonUpOnce(MouseButton.Left);
        }
        public static DivAnimation ApplyBasicAnimation(Div div, PrimitivesRenderer2D primitivesRenderer2D)
        {
            div.AnimateEnabled = true;
            var BasicAnimation = new DivAnimation(primitivesRenderer2D, AnimationType.ColorTransform, div);
            BasicAnimation.SColor = new Color(64, 64, 64, 160);
            BasicAnimation.ToColor = new Color(100, 100, 100, 160);
            BasicAnimation.PresetList.Add(DivAnimation.ColorTransformPreset);
            BasicAnimation.PresetList.Add(DivAnimation.PositionTransformPreset);
            div.Animations.Add(BasicAnimation);
            return BasicAnimation;
        }
        public static DivAnimation ApplySettingsColorAnimation(Div div, PrimitivesRenderer2D primitivesRenderer2D)
        {
            div.AnimateEnabled = true;
            var BasicAnimation = new DivAnimation(primitivesRenderer2D, AnimationType.ColorTransform, div);
            BasicAnimation.SColor = new Color(32, 32, 32, 160);
            BasicAnimation.ToColor = new Color(16, 16, 16, 160);
            BasicAnimation.PresetList.Add(DivAnimation.ColorTransformPreset);
            div.Animations.Add(BasicAnimation);
            return BasicAnimation;
        }
        public static DivAnimation ApplyColorAnimation(Div div, PrimitivesRenderer2D primitivesRenderer2D, Color color1, Color color2)
        {
            div.AnimateEnabled = true;
            var BasicAnimation = new DivAnimation(primitivesRenderer2D, AnimationType.ColorTransform, div);
            BasicAnimation.SColor = color1;
            BasicAnimation.ToColor = color2;
            BasicAnimation.PresetList.Add(DivAnimation.ColorTransformPreset);
            div.Animations.Add(BasicAnimation);
            return BasicAnimation;
        }
        public static void ApplyBackground(Div div)
        {
            div.BackgroundColor = new Color(16, 16, 16, 160);
        }
        public static void ApplyBackground(Div div, Color color)
        {
            div.BackgroundColor = color;
        }
        public static void ApplyClickBg(Div div)
        {
            div.BgColorDown = new Color(0, 0, 0, 160);
        }
        public static void ApplyClickBg(Div div, Color color)
        {
            div.BgColorDown = color;
        }
        public static void ApplyClickAudio(Div div, ComponentPlayer Player)
        {
            div.Player = Player;
            div.AudioEnabled = true;
            div.MouseDownListener += (Div d) =>
            {
                d.PlayAudio(div.Player);
            };
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            //Initialize Menus....
        }
        public override void Dispose()
        {

            EntryHook.CancelPreventIngameKeyevent(ClassUUID);
            base.Dispose();
        }
    }
}
