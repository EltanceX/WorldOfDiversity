using Engine;
using System.Xml.Linq;

namespace Game;
public class BrowserWidget : CanvasWidget
{
    public void RefreshPosition()
    {
        playerPosition = m_componentPlayer.ComponentBody.Position;
        ScreenPositionTextbox.Text = $"{Math.Floor(playerPosition.X)}, {Math.Floor(playerPosition.Y)}, {Math.Floor(playerPosition.Z)}";
    }
    public ButtonWidget ResolvingPowerBtn;
    public ButtonWidget ResizeBtn;
    public LabelWidget ResolvingPowerLabel;
    public TextBoxWidget ScreenPositionTextbox;
    public TextBoxWidget SiteAddrDialog;
    public ButtonWidget CreatePreviewButton;
    public ButtonWidget CreateBrowserButton;
    public ButtonWidget ClosePageButton;
    public ButtonWidget CloseBrowserButton;

    public RectangleWidget m_facing;
    public ButtonWidget FacingBtn;


    public ComponentPlayer m_componentPlayer;
    public IInventory m_inventory;

    public int ResolvingPower_Height = 600;

    public int ResolvingPower_Width = 1000;

    public float ScreenSize = 10.0f;

    public Vector3 playerPosition;

    public enum Facing
    {
        X, Y, Z, Eye
    }
    public Facing FacingState = Facing.Eye;

    public BrowserWidget(/*ComponentShop componentShop,*/ ComponentPlayer componentPlayer/*, SubsystemFinancial subsystemFinancial*/)
    {
        m_componentPlayer = componentPlayer;
        m_inventory = componentPlayer.ComponentMiner.Inventory;
        XElement node = ContentManager.Get<XElement>("Widgets/BrowserWidget");
        LoadContents(this, node);
        //m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
        //m_itemvalueicon = Children.Find<BlockIconWidget>("Icon");
        ResolvingPowerBtn = Children.Find<ButtonWidget>("ResolvingPowerBtn");
        ResizeBtn = Children.Find<ButtonWidget>("ResizeBtn");
        ResolvingPowerLabel = Children.Find<LabelWidget>("ResolvingPowerLabel");
        ScreenPositionTextbox = Children.Find<TextBoxWidget>("ScreenPositionTextbox");
        SiteAddrDialog = Children.Find<TextBoxWidget>("SiteAddrDialog");
        CreatePreviewButton = Children.Find<ButtonWidget>("CreatePreviewButton");
        CreateBrowserButton = Children.Find<ButtonWidget>("CreateBrowserButton");
        ClosePageButton = Children.Find<ButtonWidget>("ClosePageButton");
        CloseBrowserButton = Children.Find<ButtonWidget>("CloseBrowserButton");
        //m_soldSlot = Children.Find<InventorySlotWidget>("SoldSlot");
        //m_soldSlot.AssignInventorySlot(componentShop, componentShop.SoldSlotIndex);
        RefreshPosition();
        //SiteAddrDialog.Text = WebTV.settings.defaultLink;

        m_facing = Children.Find<RectangleWidget>("Facing");

        FacingBtn = Children.Find<ButtonWidget>("FacingBtn");
    }


    public class ResizeDialog : Dialog
    {
        public ButtonWidget m_confirmButton;

        public ButtonWidget m_addButton;

        public ButtonWidget m_reduceButton;

        public SliderWidget m_countSlider;


        public Action<float> m_handler;

        public float ScreenSize = 10f;

        public ResizeDialog(float Size, Action<float> handler)
        {
            this.ScreenSize = Size;
            XElement node = ContentManager.Get<XElement>("Dialogs/ResizeDialog");
            LoadContents(this, node);
            m_handler = handler;
            m_confirmButton = Children.Find<ButtonWidget>("ConfirmButton");
            m_countSlider = Children.Find<SliderWidget>("CountSlider");
            m_addButton = Children.Find<ButtonWidget>("AddButton");
            m_reduceButton = Children.Find<ButtonWidget>("ReduceButton");
            m_countSlider.MinValue = 0.1f;
            m_countSlider.MaxValue = 30f;
            m_countSlider.Granularity = .1f;
            m_countSlider.Value = Size;

        }

        public override void Update()
        {
            m_countSlider.Text = "Size (m):" + ScreenSize;
            ScreenSize = Math.Clamp(m_countSlider.Value, 0f, 30f);
            m_addButton.IsEnabled = m_countSlider.Value < 30f;
            m_reduceButton.IsEnabled = m_countSlider.Value > 0.1f;
            if (m_reduceButton.IsClicked) m_countSlider.Value -= .1f;
            else if (m_addButton.IsClicked) m_countSlider.Value += .1f;
            else if (m_confirmButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
                if (m_handler != null)
                {
                    m_handler(ScreenSize);
                }
            }
        }
    }

    public class ResolvingPowerDialog : Dialog
    {
        public ButtonWidget m_confirmButton;

        public ButtonWidget m_addButton;
        public ButtonWidget m_addButton2;

        public ButtonWidget m_reduceButton;
        public ButtonWidget m_reduceButton2;

        public SliderWidget m_countSlider;
        public SliderWidget m_countSlider2;


        public Action<int, int> m_handler;

        public int Width = 1000;
        public int Height = 600;

        public ResolvingPowerDialog(int ResolvingPower_Width, int ResolvingPower_Height, Action<int, int> handler)
        {
            Width = ResolvingPower_Width;
            Height = ResolvingPower_Height;
            XElement node = ContentManager.Get<XElement>("Dialogs/ResolvingPowerDialog");
            LoadContents(this, node);
            m_handler = handler;
            m_confirmButton = Children.Find<ButtonWidget>("ConfirmButton");
            m_countSlider = Children.Find<SliderWidget>("CountSlider");
            m_addButton = Children.Find<ButtonWidget>("AddButton");
            m_reduceButton = Children.Find<ButtonWidget>("ReduceButton");
            m_countSlider.MinValue = 100f;
            m_countSlider.MaxValue = 2000f;
            m_countSlider.Granularity = 100f;
            m_countSlider.Value = Width;

            m_countSlider2 = Children.Find<SliderWidget>("CountSlider2");
            m_addButton2 = Children.Find<ButtonWidget>("AddButton2");
            m_reduceButton2 = Children.Find<ButtonWidget>("ReduceButton2");
            m_countSlider2.MinValue = 100f;
            m_countSlider2.MaxValue = 2000f;
            m_countSlider2.Granularity = 100f;
            m_countSlider2.Value = Height;

        }

        public override void Update()
        {
            m_countSlider2.Text = "Height:" + Height;
            Height = Math.Clamp((int)m_countSlider2.Value, 100, 2000);
            m_addButton2.IsEnabled = m_countSlider2.Value < 2000f;
            m_reduceButton2.IsEnabled = m_countSlider2.Value > 100f;
            if (m_reduceButton2.IsClicked) m_countSlider2.Value -= 100;
            else if (m_addButton2.IsClicked) m_countSlider2.Value += 100;

            m_countSlider.Text = "Width:" + Width;
            Width = Math.Clamp((int)m_countSlider.Value, 100, 2000);
            m_addButton.IsEnabled = m_countSlider.Value < 2000f;
            m_reduceButton.IsEnabled = m_countSlider.Value > 100f;
            if (m_reduceButton.IsClicked) m_countSlider.Value -= 100;
            else if (m_addButton.IsClicked) m_countSlider.Value += 100;
            else if (m_confirmButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
                if (m_handler != null)
                {
                    m_handler(Width, Height);
                }
            }
        }
    }
    public override void Update()
    {
        //DialogsManager.ShowDialog(null, new ChooseItemsDialog(m_subsystemFinancial, delegate (int value)
        //{
        //    ItemValue = value;
        //}));



        ResolvingPowerLabel.Text = $"分辨率: {ResolvingPower_Width}*{ResolvingPower_Height}";

        if (ResolvingPowerBtn.IsClicked)
        {
            DialogsManager.ShowDialog(null, new ResolvingPowerDialog(ResolvingPower_Width, ResolvingPower_Height, delegate (int width, int height)
            {
                ResolvingPower_Height = height;
                ResolvingPower_Width = width;
            }));
        }
        else if (ResizeBtn.IsClicked)
        {
            DialogsManager.ShowDialog(null, new ResizeDialog(ScreenSize, delegate (float ScreenSize)
            {
                this.ScreenSize = ScreenSize;
            }));
        }
    }
}
