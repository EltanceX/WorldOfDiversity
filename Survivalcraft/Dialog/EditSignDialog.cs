using Engine;
using System.Xml.Linq;

namespace Game
{
	public class EditSignDialog : Dialog
	{
		public SubsystemSignBlockBehavior m_subsystemSignBlockBehavior;

		public Point3 m_signPoint;

		public ContainerWidget m_linesPage;

		public ContainerWidget m_urlPage;

		public TextBoxWidget m_textBox1;

		public TextBoxWidget m_textBox2;

		public TextBoxWidget m_textBox3;

		public TextBoxWidget m_textBox4;

		public ButtonWidget m_colorButton1;

		public ButtonWidget m_colorButton2;

		public ButtonWidget m_colorButton3;

		public ButtonWidget m_colorButton4;

		public TextBoxWidget m_urlTextBox;

		public ButtonWidget m_urlTestButton;

		public ButtonWidget m_okButton;

		public ButtonWidget m_cancelButton;

		public ButtonWidget m_urlButton;

		public ButtonWidget m_linesButton;

		public SubsystemSignBlockBehavior.TextData m_editingTextData;

		public Color[] m_colors = new Color[8]
		{
			new(0, 0, 0),
			new(140, 0, 0),
			new(0, 112, 0),
			new(0, 0, 96),
			new(160, 0, 128),
			new(0, 112, 112),
			new(160, 112, 0),
			new(180, 180, 180)
		};

		public EditSignDialog(SubsystemSignBlockBehavior subsystemSignBlockBehavior, Point3 signPoint)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditSignDialog");
			LoadContents(this, node);
			m_linesPage = Children.Find<ContainerWidget>("EditSignDialog.LinesPage");
			m_urlPage = Children.Find<ContainerWidget>("EditSignDialog.UrlPage");
			m_textBox1 = Children.Find<TextBoxWidget>("EditSignDialog.TextBox1");
			m_textBox2 = Children.Find<TextBoxWidget>("EditSignDialog.TextBox2");
			m_textBox3 = Children.Find<TextBoxWidget>("EditSignDialog.TextBox3");
			m_textBox4 = Children.Find<TextBoxWidget>("EditSignDialog.TextBox4");
			m_colorButton1 = Children.Find<ButtonWidget>("EditSignDialog.ColorButton1");
			m_colorButton2 = Children.Find<ButtonWidget>("EditSignDialog.ColorButton2");
			m_colorButton3 = Children.Find<ButtonWidget>("EditSignDialog.ColorButton3");
			m_colorButton4 = Children.Find<ButtonWidget>("EditSignDialog.ColorButton4");
			m_urlTextBox = Children.Find<TextBoxWidget>("EditSignDialog.UrlTextBox");
			m_urlTestButton = Children.Find<ButtonWidget>("EditSignDialog.UrlTestButton");
			m_okButton = Children.Find<ButtonWidget>("EditSignDialog.OkButton");
			m_cancelButton = Children.Find<ButtonWidget>("EditSignDialog.CancelButton");
			m_urlButton = Children.Find<ButtonWidget>("EditSignDialog.UrlButton");
			m_linesButton = Children.Find<ButtonWidget>("EditSignDialog.LinesButton");
			m_subsystemSignBlockBehavior = subsystemSignBlockBehavior;
			m_signPoint = signPoint;
			SignData signData = m_subsystemSignBlockBehavior.GetSignData(m_signPoint);
			m_editingTextData = m_subsystemSignBlockBehavior.m_textsByPoint.GetValueOrDefault(m_signPoint,null);
			if (signData != null && m_editingTextData != null)
			{
				m_textBox1.Text = signData.Lines[0];
				m_textBox2.Text = signData.Lines[1];
				m_textBox3.Text = signData.Lines[2];
				m_textBox4.Text = signData.Lines[3];
				m_colorButton1.Color = signData.Colors[0];
				m_colorButton2.Color = signData.Colors[1];
				m_colorButton3.Color = signData.Colors[2];
				m_colorButton4.Color = signData.Colors[3];
				m_urlTextBox.Text = signData.Url;
			}
			else
			{
				m_textBox1.Text = string.Empty;
				m_textBox2.Text = string.Empty;
				m_textBox3.Text = string.Empty;
				m_textBox4.Text = string.Empty;
				m_colorButton1.Color = Color.Black;
				m_colorButton2.Color = Color.Black;
				m_colorButton3.Color = Color.Black;
				m_colorButton4.Color = Color.Black;
				m_urlTextBox.Text = string.Empty;
			}
			m_linesPage.IsVisible = true;
			m_urlPage.IsVisible = false;
			UpdateControls();
		}

		public override void Update()
		{
			UpdateControls();
			if (m_okButton.IsClicked)
			{
				string[] lines = new string[4]
				{
					m_textBox1.Text,
					m_textBox2.Text,
					m_textBox3.Text,
					m_textBox4.Text
				};
				var colors = new Color[4]
				{
					m_colorButton1.Color,
					m_colorButton2.Color,
					m_colorButton3.Color,
					m_colorButton4.Color
				};
				m_subsystemSignBlockBehavior.SetSignData(m_signPoint, lines, colors, m_urlTextBox.Text);
				Dismiss();
			}
			if (m_urlButton.IsClicked)
			{
				m_urlPage.IsVisible = true;
				m_linesPage.IsVisible = false;
			}
			if (m_linesButton.IsClicked)
			{
				m_urlPage.IsVisible = false;
				m_linesPage.IsVisible = true;
			}
			if (m_urlTestButton.IsClicked)
			{
				WebBrowserManager.LaunchBrowser(m_urlTextBox.Text);
			}
			if (m_colorButton1.IsClicked)
			{
				m_colorButton1.Color = m_colors[(m_colors.FirstIndex(m_colorButton1.Color) + 1) % m_colors.Length];
			}
			if (m_colorButton2.IsClicked)
			{
				m_colorButton2.Color = m_colors[(m_colors.FirstIndex(m_colorButton2.Color) + 1) % m_colors.Length];
			}
			if (m_colorButton3.IsClicked)
			{
				m_colorButton3.Color = m_colors[(m_colors.FirstIndex(m_colorButton3.Color) + 1) % m_colors.Length];
			}
			if (m_colorButton4.IsClicked)
			{
				m_colorButton4.Color = m_colors[(m_colors.FirstIndex(m_colorButton4.Color) + 1) % m_colors.Length];
			}
			if (Input.Cancel || m_cancelButton.IsClicked)
			{
				Dismiss();
			}
			if(!MovingBlock.IsNullOrStopped(m_editingTextData?.MovingBlock))
			{
				Dismiss();
			}
		}

		public void UpdateControls()
		{
			bool flag = !string.IsNullOrEmpty(m_urlTextBox.Text);
			m_urlButton.IsVisible = m_linesPage.IsVisible;
			m_linesButton.IsVisible = !m_linesPage.IsVisible;
			m_colorButton1.IsEnabled = !flag;
			m_colorButton2.IsEnabled = !flag;
			m_colorButton3.IsEnabled = !flag;
			m_colorButton4.IsEnabled = !flag;
			m_urlTestButton.IsEnabled = flag;
		}

		public void Dismiss()
		{
			DialogsManager.HideDialog(this);
		}
	}
}
