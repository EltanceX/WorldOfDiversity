//using OpenTK.Input;
using DebugMod;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Game;
using GameEntitySystem;
using TemplatesDatabase;
using Key = Engine.Input.Key;
using Mouse = Engine.Input.Mouse;
using MouseButton = Engine.Input.MouseButton;
using GlassMod;

namespace TVKeyboardMenu
{
    public enum cKey
	{
		Back,
		Shift,
		Control,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		LeftArrow,
		RightArrow,
		UpArrow,
		DownArrow,
		Enter,
		Escape,
		Space,
		Tab,
		BackSpace,
		Insert,
		Delete,
		PageUp,
		PageDown,
		Home,
		End,
		CapsLock,
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K,
		L,
		M,
		N,
		O,
		P,
		Q,
		R,
		S,
		T,
		U,
		V,
		W,
		X,
		Y,
		Z,
		Number0,
		Number1,
		Number2,
		Number3,
		Number4,
		Number5,
		Number6,
		Number7,
		Number8,
		Number9,
		Tilde,
		Minus,
		Plus,
		LeftBracket,
		RightBracket,
		Semicolon,
		Quote,
		Comma,
		Period,
		Slash,

		Equals = 10070,

		WebUp = 20001,
		WebDown = 20002
	}
	public class KeyboardMenu : Component, IDrawable, IUpdateable
	{
		public string ClassUUID = util.uuid();
		public static KeyboardMenu Instance { get; private set; }
		public Texture2D VKBD_Texture;
		public SubsystemAudio subsystemAudio;
		public ComponentBody componentBody;
		public ComponentPlayer Player;
		public Camera camera;
		public int[] m_drawOrders = new int[1] { 2000 };
		public Vector2 RelativeStart = new Vector2(100, 100);
		public bool m_Visible = false;
		public bool Visible { get { return m_Visible; } set { 
				m_Visible = value;
				if(m_Visible) EntryHook.PreventIngameKeyevent(KeyboardMenu.Instance.ClassUUID);
				else EntryHook.CancelPreventIngameKeyevent(KeyboardMenu.Instance.ClassUUID);
			} }
		public int Top = 0;
		public int Left = 0;
		public Vector2 TextureSize;
		public enum KeyState
		{
			None,
			Down,
			Triggered
		}

		public KeyState State = KeyState.None;

		public PrimitivesRenderer2D primitivesRenderer2D = new PrimitivesRenderer2D();
		public Vector2[] RectToDraw = { new Vector2(0, 0), new Vector2(0, 0)};
		public class SingleKey
		{
			public cKey key;
			/// <summary>
			/// Left Top Relative to Keybd
			/// </summary>
			public Vector2 Position;
			public int Width = 48;
			public int Height = 48;
			public Action<SingleKey> OnKeyDown;
			public SingleKey(cKey key,Vector2 position,int width = 48,int height = 48)
			{
				this.key = key;
				Position = position;
				Width = width;
				Height = height;
			}
		}
		public List<SingleKey> KeyList = new List<SingleKey>();
		public int FindKey(Vector2 RPos)
		{
			for(int i = 0;i< KeyList.Count; i++)
			{
				SingleKey key = KeyList[i];
				if(RPos.X > key.Position.X + Left && RPos.Y > key.Position.Y + Top && RPos.X - (key.Position.X + Left) < key.Width && RPos.Y - (key.Position.Y + Top) < key.Height)
				{
					//键盘按下
					//ScreenLog.Info("键盘按下");
					//ScreenLog.Info("键盘按下KeyState "+State.ToString());
					RectToDraw[0].X = key.Position.X + Left;
					RectToDraw[0].Y = key.Position.Y + Top;
					RectToDraw[1].X = key.Position.X + key.Width + Left;
					RectToDraw[1].Y = key.Position.Y + key.Height + Top;
					if(State == KeyState.Down)
					{
						State = KeyState.Triggered;
						subsystemAudio.PlaySound("Audio/Keydown",1f,0f,componentBody.Position,4f,autoDelay: true);
						Player.ComponentGui.DisplaySmallMessage("按下了: "+key.key.ToString() ,Color.White,blinking: true,playNotificationSound: false);
						//if((int)key.key<20000) WebviewTV.Interact.KeybdSimulation.Add((int)key.key);
						key.OnKeyDown?.Invoke(key);
					}
					return (int)key.key;

				}
			}
			RectToDraw[0].X = 0;
			RectToDraw[0].Y = 0;
			RectToDraw[1].X = 0;
			RectToDraw[1].Y = 0;
			State = KeyState.None;
			return -1;

		}

		UpdateOrder IUpdateable.UpdateOrder => ((IUpdateable)Player).UpdateOrder;
		public int[] DrawOrders => m_drawOrders;

		public KeyboardMenu()
		{

		}
		public void Draw(Camera camera,int drawOrder)
		{
			if(!Visible) return;
			FlatBatch2D flatBatch2D = primitivesRenderer2D.FlatBatch();
			//flatBatch2D.QueueLine(new Vector2(0,0),new Vector2(100,100),1,Color.Yellow);
			//flatBatch2D.QueueRectangle(RectToDraw[0],RectToDraw[1],1,Color.Cyan);
			flatBatch2D.QueueQuad(RectToDraw[0],RectToDraw[1],0,new Color(255,118,117,80));
			var tb2d = primitivesRenderer2D.TexturedBatch(VKBD_Texture);//, true,1);
			tb2d.QueueQuad(new Vector2(0 + Left,0+Top),new Vector2(TextureSize.X+Left, TextureSize.Y+Top),0,new Vector2(0,0),new Vector2(1,1),Color.White);
			tb2d.Flush();
			primitivesRenderer2D.Flush();
		}

		public void Update(float dt)
		{
			//if(Mouse.MousePosition == null) ScreenLog.Info("bbb null");
			//TouchLocationState.Pressed
			//ScreenLog.Info("Touch.TouchLocations.Length: " + Touch.TouchLocations.Count);
			//if(Touch.TouchLocations.Count > 0)
			//	ScreenLog.Info("TouchLocationState.Pressed: " + Touch.TouchLocations[0].State.ToString());

			if(!Visible) return;
			if(EGlobal.Platform == EGlobal.Platforms.Android && Touch.TouchLocations.Count == 0) return;
			else if(EGlobal.Platform == EGlobal.Platforms.Windows && Mouse.MousePosition == null) return;

			if(Mouse.IsMouseButtonDown(MouseButton.Left) || Touch.TouchLocations[0].State == TouchLocationState.Pressed || Touch.TouchLocations[0].State == TouchLocationState.Moved)
			{
				//Touch.TouchLocations[0].
				Vector2 Pos = new Vector2();
				switch(EGlobal.Platform)
				{
					case EGlobal.Platforms.Android:
						Pos = Touch.TouchLocations[0].Position;
					break;
					case EGlobal.Platforms.Windows:
						//if(Mouse.MousePosition == null) return;
						Pos = new Vector2(Mouse.MousePosition.Value.X,Mouse.MousePosition.Value.Y);
					break;
				}
				//var Pos = Mouse.MousePosition;
				var keyNumber = FindKey(Pos);
				//ScreenLog.Info("keyNumber: " + keyNumber);
				//ScreenLog.Info("KeyState: " + State.ToString());
				if(State == KeyState.None) State = KeyState.Down;
				if(keyNumber == (int)Key.End)
				{
					State = KeyState.None;
					Visible = false;
				}
				//else if(keyNumber == (int)cKey.UpArrow)
				//{
				//	ScreenLog.Info("aaa");
				//	WebviewTV.Interact.ScrollRequest = WebviewTV.WebviewTVScrollType.Up;
				//	WebviewTV.Interact.ScrollDegree = 10;
				//}

			} else State = KeyState.None;
		}
		public SingleKey FindKeyInList(cKey key)
		{
			foreach(var item in KeyList)
			{
				if(item.key == key) return item;
			}
			return null;
		}
		public void AddToKeyList()
		{
			cKey[] KLine1 = new cKey[] { cKey.Number1,cKey.Number2,cKey.Number3,cKey.Number4,cKey.Number5,cKey.Number6,cKey.Number7,cKey.Number8,cKey.Number9,cKey.Number0,cKey.Minus, cKey.Equals, cKey.Delete };
			cKey[] KLine2 = new cKey[] { cKey.Q,cKey.W,cKey.E,cKey.R,cKey.T,cKey.Y,cKey.U,cKey.I,cKey.O,cKey.P,cKey.LeftBracket,cKey.RightBracket };
			cKey[] KLine3 = new cKey[] { cKey.A,cKey.S,cKey.D,cKey.F,cKey.G,cKey.H,cKey.J,cKey.K,cKey.L,cKey.Semicolon,cKey.Quote };
			cKey[] KLine4 = new cKey[] { cKey.Space,cKey.Z,cKey.X,cKey.C,cKey.V,cKey.B,cKey.N,cKey.M,cKey.Comma,cKey.Period,cKey.Slash, cKey.End };
			cKey[] KLine5 = new cKey[] { cKey.UpArrow, cKey.DownArrow, cKey.WebUp, cKey.WebDown };
			for(int i = 0; i < KLine1.Length; i++)
			{
				if(i == KLine1.Length - 1)
				{
					KeyList.Add(new SingleKey(KLine1[i],new Vector2(16 + 48 * i + 16 * i,16 * 1), 96, 48));
					continue;
				}
				//                                             X偏移  每个键帽的偏移   Y偏移
				KeyList.Add(new SingleKey(KLine1[i],new Vector2(16 + 48 * i + 16 * i,16 * 1)));
			}
			for(int i = 0; i < KLine2.Length; i++)
			{
				KeyList.Add(new SingleKey(KLine2[i],new Vector2(48 + 48 * i + 16 * i,16 * 5)));
			}
			for(int i = 0; i < KLine3.Length; i++)
			{
				KeyList.Add(new SingleKey(KLine3[i],new Vector2(80 + 48 * i + 16 * i,16 * 9)));
			}
			for(int i = 0; i < KLine4.Length; i++)
			{
				KeyList.Add(new SingleKey(KLine4[i],new Vector2(48 + 48 * i + 16 * i,16 * 13)));
			}
			for(int i = 0; i < KLine5.Length; i++)
			{
				SingleKey sk = new SingleKey(KLine5[i],new Vector2(48 + 48 * i + 16 * i,16 * 17));
				KeyList.Add(sk);
				if(KLine5[i] == cKey.WebUp)
				{
					sk.OnKeyDown = delegate (SingleKey singleKey)
					{
						//WebviewTV.Interact.ScrollRequest = WebviewTV.WebviewTVScrollType.Up;
						//WebviewTV.Interact.ScrollDegree = 30;
					};
				}
				else if(KLine5[i] == cKey.WebDown)
				{
					sk.OnKeyDown = delegate (SingleKey singleKey)
					{
						//WebviewTV.Interact.ScrollRequest = WebviewTV.WebviewTVScrollType.Down;
						//WebviewTV.Interact.ScrollDegree = 30;
					};
				}
			}
		}
		public override void Load(ValuesDictionary valuesDictionary,IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary,idToEntityMap);
			Instance = this;
			VKBD_Texture = ContentManager.Get<Texture2D>("Textures/VKeyboard");

			//Player = base.Entity.FindComponent<ComponentPlayer>();
			Player = base.Entity.FindComponent<ComponentPlayer>(throwOnError: true);
			subsystemAudio = Player.ComponentBody.m_subsystemAudio;
			componentBody = Player.ComponentBody;
			primitivesRenderer2D = new PrimitivesRenderer2D();

			//修正分辨率
			var WindowSize = Window.Size;
			TextureSize = new Vector2(VKBD_Texture.Width, VKBD_Texture.Height);
			this.Top = (int)((WindowSize.Y - TextureSize.Y) / 2);
			this.Left = (int)((WindowSize.X - TextureSize.X) / 2);

			AddToKeyList();
		}
	}
}
