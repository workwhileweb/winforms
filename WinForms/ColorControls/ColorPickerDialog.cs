﻿using System;
using System.Drawing;
using System.Windows.Forms;
using AdamsLair.WinForms.Internal;

namespace AdamsLair.WinForms.ColorControls
{
	public partial class ColorPickerDialog : Form
	{
		public enum PrimaryAttrib
		{
			Hue,
			Saturation,
			Brightness,
			Red,
			Green,
			Blue
		}

		private struct InternalColor
		{
			public float h;
			public float s;
			public float v;
			public float a;

			public InternalColor(float h, float s, float v, float a)
			{
				this.h = h;
				this.s = s;
				this.v = v;
				this.a = a;
			}
			public InternalColor(Color c)
			{
				this.h = c.GetHSVHue();
				this.s = c.GetHSVSaturation();
				this.v = c.GetHSVBrightness();
				this.a = c.A / 255.0f;
			}

			public Color ToColor()
			{
				return Color.FromArgb((int)Math.Round(this.a * 255.0f), ExtMethodsColor.ColorFromHSV(this.h, this.s, this.v));
			}
		}

		private	bool			alphaEnabled		= true;
		private	InternalColor	oldColor			= new InternalColor(Color.Red);
		private	InternalColor	selColor			= new InternalColor(Color.Red);
		private	PrimaryAttrib	primAttrib			= PrimaryAttrib.Hue;
		private	bool			suspendTextEvents	= false;

		public event EventHandler ColorEdited = null;

		public bool AlphaEnabled
		{
			get { return this.alphaEnabled; }
			set 
			{ 
				this.alphaEnabled = value; 
				this.alphaSlider.Enabled = this.alphaEnabled;
				this.numAlpha.Enabled = this.alphaEnabled;
			}
		}
		public Color OldColor
		{
			get { return this.oldColor.ToColor(); }
			set { this.oldColor = new InternalColor(value); this.UpdateColorShowBox(); }
		}
		public Color SelectedColor
		{
			get { return this.selColor.ToColor(); }
			set { this.selColor = new InternalColor(value); this.UpdateColorControls(); }
		}
		public PrimaryAttrib PrimaryAttribute
		{
			get { return this.primAttrib; }
			set { this.primAttrib = value; this.UpdateColorControls(); }
		}

		public ColorPickerDialog()
		{
			this.InitializeComponent();
		}

		private void UpdateColorControls()
		{
			this.UpdatePrimaryAttributeRadioBox();
			this.UpdateText();
			this.UpdateColorShowBox();
			this.UpdateColorPanelGradient();
			this.UpdateColorSliderGradient();
			this.UpdateAlphaSliderGradient();
			this.UpdateColorPanelValue();
			this.UpdateColorSliderValue();
			this.UpdateAlphaSliderValue();
		}
		private void UpdatePrimaryAttributeRadioBox()
		{
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.radioHue.Checked = true;
					break;
				case PrimaryAttrib.Saturation:
					this.radioSaturation.Checked = true;
					break;
				case PrimaryAttrib.Brightness:
					this.radioValue.Checked = true;
					break;
				case PrimaryAttrib.Red:
					this.radioRed.Checked = true;
					break;
				case PrimaryAttrib.Green:
					this.radioGreen.Checked = true;
					break;
				case PrimaryAttrib.Blue:
					this.radioBlue.Checked = true;
					break;
			}
		}
		private void UpdateText()
		{
			Color tmp = this.selColor.ToColor();
			this.suspendTextEvents = true;

			this.textBoxHex.Text = String.Format("{0:X}", tmp.ToArgb());

			this.numRed.Value = tmp.R;
			this.numGreen.Value = tmp.G;
			this.numBlue.Value = tmp.B;
			this.numAlpha.Value = tmp.A;

			this.numHue.Value = (decimal)(this.selColor.h * 360.0f);
			this.numSaturation.Value = (decimal)(this.selColor.s * 100.0f);
			this.numValue.Value = (decimal)(this.selColor.v * 100.0f);

			this.suspendTextEvents = false;
		}
		private void UpdateColorShowBox()
		{
			this.colorShowBox.UpperColor = this.alphaEnabled ? this.oldColor.ToColor() : Color.FromArgb(255, this.oldColor.ToColor());
			this.colorShowBox.LowerColor = this.alphaEnabled ? this.selColor.ToColor() : Color.FromArgb(255, this.selColor.ToColor());
		}
		private void UpdateColorPanelGradient()
		{
			Color tmp;
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.colorPanel.SetupXYGradient(
						Color.White,
						ExtMethodsColor.ColorFromHSV(this.selColor.h, 1.0f, 1.0f),
						Color.Black,
						Color.Transparent);
					break;
				case PrimaryAttrib.Saturation:
					this.colorPanel.SetupHueBrightnessGradient(this.selColor.s);
					break;
				case PrimaryAttrib.Brightness:
					this.colorPanel.SetupHueSaturationGradient(this.selColor.v);
					break;
				case PrimaryAttrib.Red:
					tmp = this.selColor.ToColor();
					this.colorPanel.SetupGradient(
						Color.FromArgb(255, tmp.R, 255, 0),
						Color.FromArgb(255, tmp.R, 255, 255),
						Color.FromArgb(255, tmp.R, 0, 0),
						Color.FromArgb(255, tmp.R, 0, 255),
						32);
					break;
				case PrimaryAttrib.Green:
					tmp = this.selColor.ToColor();
					this.colorPanel.SetupGradient(
						Color.FromArgb(255, 255, tmp.G, 0),
						Color.FromArgb(255, 255, tmp.G, 255),
						Color.FromArgb(255, 0, tmp.G, 0),
						Color.FromArgb(255, 0, tmp.G, 255),
						32);
					break;
				case PrimaryAttrib.Blue:
					tmp = this.selColor.ToColor();
					this.colorPanel.SetupGradient(
						Color.FromArgb(255, 255, 0, tmp.B),
						Color.FromArgb(255, 255, 255, tmp.B),
						Color.FromArgb(255, 0, 0, tmp.B),
						Color.FromArgb(255, 0, 255, tmp.B),
						32);
					break;
			}
		}
		private void UpdateColorPanelValue()
		{
			Color tmp;
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.colorPanel.ValuePercentual = new PointF(
						this.selColor.s,
						this.selColor.v);
					break;
				case PrimaryAttrib.Saturation:
					this.colorPanel.ValuePercentual = new PointF(
						this.selColor.h,
						this.selColor.v);
					break;
				case PrimaryAttrib.Brightness:
					this.colorPanel.ValuePercentual = new PointF(
						this.selColor.h,
						this.selColor.s);
					break;
				case PrimaryAttrib.Red:
					tmp = this.selColor.ToColor();
					this.colorPanel.ValuePercentual = new PointF(
						tmp.B / 255.0f,
						tmp.G / 255.0f);
					break;
				case PrimaryAttrib.Green:
					tmp = this.selColor.ToColor();
					this.colorPanel.ValuePercentual = new PointF(
						tmp.B / 255.0f,
						tmp.R / 255.0f);
					break;
				case PrimaryAttrib.Blue:
					tmp = this.selColor.ToColor();
					this.colorPanel.ValuePercentual = new PointF(
						tmp.G / 255.0f,
						tmp.R / 255.0f);
					break;
			}
		}
		private void UpdateColorSliderGradient()
		{
			Color tmp;
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.colorSlider.SetupHueGradient(/*this.selColor.GetHSVSaturation(), this.selColor.GetHSVBrightness()*/);
					break;
				case PrimaryAttrib.Saturation:
					this.colorSlider.SetupGradient(
						ExtMethodsColor.ColorFromHSV(this.selColor.h, 0.0f, this.selColor.v),
						ExtMethodsColor.ColorFromHSV(this.selColor.h, 1.0f, this.selColor.v));
					break;
				case PrimaryAttrib.Brightness:
					this.colorSlider.SetupGradient(
						ExtMethodsColor.ColorFromHSV(this.selColor.h, this.selColor.s, 0.0f),
						ExtMethodsColor.ColorFromHSV(this.selColor.h, this.selColor.s, 1.0f));
					break;
				case PrimaryAttrib.Red:
					tmp = this.selColor.ToColor();
					this.colorSlider.SetupGradient(
						Color.FromArgb(255, 0, tmp.G, tmp.B),
						Color.FromArgb(255, 255, tmp.G, tmp.B));
					break;
				case PrimaryAttrib.Green:
					tmp = this.selColor.ToColor();
					this.colorSlider.SetupGradient(
						Color.FromArgb(255, tmp.R, 0, tmp.B),
						Color.FromArgb(255, tmp.R, 255, tmp.B));
					break;
				case PrimaryAttrib.Blue:
					tmp = this.selColor.ToColor();
					this.colorSlider.SetupGradient(
						Color.FromArgb(255, tmp.R, tmp.G, 0),
						Color.FromArgb(255, tmp.R, tmp.G, 255));
					break;
			}
		}
		private void UpdateColorSliderValue()
		{
			Color tmp;
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.colorSlider.ValuePercentual = this.selColor.h;
					break;
				case PrimaryAttrib.Saturation:
					this.colorSlider.ValuePercentual = this.selColor.s;
					break;
				case PrimaryAttrib.Brightness:
					this.colorSlider.ValuePercentual = this.selColor.v;
					break;
				case PrimaryAttrib.Red:
					tmp = this.selColor.ToColor();
					this.colorSlider.ValuePercentual = tmp.R / 255.0f;
					break;
				case PrimaryAttrib.Green:
					tmp = this.selColor.ToColor();
					this.colorSlider.ValuePercentual = tmp.G / 255.0f;
					break;
				case PrimaryAttrib.Blue:
					tmp = this.selColor.ToColor();
					this.colorSlider.ValuePercentual = tmp.B / 255.0f;
					break;
			}
		}
		private void UpdateAlphaSliderGradient()
		{
			this.alphaSlider.SetupGradient(Color.Transparent, Color.FromArgb(255, this.selColor.ToColor()));
		}
		private void UpdateAlphaSliderValue()
		{
			this.alphaSlider.ValuePercentual = this.selColor.a;
		}

		private void UpdateSelectedColorFromSliderValue()
		{
			Color tmp;
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.selColor.h = this.colorSlider.ValuePercentual;
					break;
				case PrimaryAttrib.Saturation:
					this.selColor.s = this.colorSlider.ValuePercentual;
					break;
				case PrimaryAttrib.Brightness:
					this.selColor.v = this.colorSlider.ValuePercentual;
					break;
				case PrimaryAttrib.Red:
					tmp = this.selColor.ToColor();
					this.selColor = new InternalColor(Color.FromArgb(
						tmp.A, 
						(int)Math.Round(this.colorSlider.ValuePercentual * 255.0f), 
						tmp.G, 
						tmp.B));
					break;
				case PrimaryAttrib.Green:
					tmp = this.selColor.ToColor();
					this.selColor = new InternalColor(Color.FromArgb(
						tmp.A, 
						tmp.R, 
						(int)Math.Round(this.colorSlider.ValuePercentual * 255.0f), 
						tmp.B));
					break;
				case PrimaryAttrib.Blue:
					tmp = this.selColor.ToColor();
					this.selColor = new InternalColor(Color.FromArgb(
						tmp.A, 
						tmp.R, 
						tmp.G, 
						(int)Math.Round(this.colorSlider.ValuePercentual * 255.0f)));
					break;
			}
			this.OnColorEdited();
		}
		private void UpdateSelectedColorFromPanelValue()
		{
			Color tmp;
			switch (this.primAttrib)
			{
				default:
				case PrimaryAttrib.Hue:
					this.selColor.s = this.colorPanel.ValuePercentual.X;
					this.selColor.v = this.colorPanel.ValuePercentual.Y;
					break;
				case PrimaryAttrib.Saturation:
					this.selColor.h = this.colorPanel.ValuePercentual.X;
					this.selColor.v = this.colorPanel.ValuePercentual.Y;
					break;
				case PrimaryAttrib.Brightness:
					this.selColor.h = this.colorPanel.ValuePercentual.X;
					this.selColor.s = this.colorPanel.ValuePercentual.Y;
					break;
				case PrimaryAttrib.Red:
					tmp = this.selColor.ToColor();
					this.selColor = new InternalColor(Color.FromArgb(
						tmp.A, 
						tmp.R, 
						(int)Math.Round(this.colorPanel.ValuePercentual.Y * 255.0f), 
						(int)Math.Round(this.colorPanel.ValuePercentual.X * 255.0f)));
					break;
				case PrimaryAttrib.Green:
					tmp = this.selColor.ToColor();
					this.selColor = new InternalColor(Color.FromArgb(
						tmp.A, 
						(int)Math.Round(this.colorPanel.ValuePercentual.Y * 255.0f), 
						tmp.G, 
						(int)Math.Round(this.colorPanel.ValuePercentual.X * 255.0f)));
					break;
				case PrimaryAttrib.Blue:
					tmp = this.selColor.ToColor();
					this.selColor = new InternalColor(Color.FromArgb(
						tmp.A, 
						(int)Math.Round(this.colorPanel.ValuePercentual.Y * 255.0f), 
						(int)Math.Round(this.colorPanel.ValuePercentual.X * 255.0f),
						tmp.B));
					break;
			}
			this.OnColorEdited();
		}
		private void UpdateSelectedColorFromAlphaValue()
		{
			this.selColor.a = this.alphaSlider.ValuePercentual;
			this.OnColorEdited();
		}

		private void OnColorEdited()
		{
			if (this.ColorEdited != null)
				this.ColorEdited(this, EventArgs.Empty);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.selColor = this.oldColor;
			this.UpdateColorControls();
		}

		private void radioHue_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioHue.Checked) this.PrimaryAttribute = PrimaryAttrib.Hue;
		}
		private void radioSaturation_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioSaturation.Checked) this.PrimaryAttribute = PrimaryAttrib.Saturation;
		}
		private void radioValue_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioValue.Checked) this.PrimaryAttribute = PrimaryAttrib.Brightness;
		}
		private void radioRed_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioRed.Checked) this.PrimaryAttribute = PrimaryAttrib.Red;
		}
		private void radioGreen_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioGreen.Checked) this.PrimaryAttribute = PrimaryAttrib.Green;
		}
		private void radioBlue_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioBlue.Checked) this.PrimaryAttribute = PrimaryAttrib.Blue;
		}

		private void colorPanel_PercentualValueChanged(object sender, EventArgs e)
		{
			if (this.ContainsFocus) this.UpdateSelectedColorFromPanelValue();
			this.UpdateColorSliderGradient();
			this.UpdateAlphaSliderGradient();
			this.UpdateColorShowBox();
			this.UpdateText();
		}
		private void colorSlider_PercentualValueChanged(object sender, EventArgs e)
		{
			if (this.ContainsFocus) this.UpdateSelectedColorFromSliderValue();
			this.UpdateColorPanelGradient();
			this.UpdateAlphaSliderGradient();
			this.UpdateColorShowBox();
			this.UpdateText();
		}
		private void alphaSlider_PercentualValueChanged(object sender, EventArgs e)
		{
			if (this.ContainsFocus) this.UpdateSelectedColorFromAlphaValue();
			this.UpdateColorSliderGradient();
			this.UpdateColorPanelGradient();
			this.UpdateColorShowBox();
			this.UpdateText();
		}

		private void numHue_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			this.selColor.h = (float)this.numHue.Value / 360.0f;
			this.UpdateColorControls();
		}
		private void numSaturation_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			this.selColor.s = (float)this.numSaturation.Value / 100.0f;
			this.UpdateColorControls();
		}
		private void numValue_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			this.selColor.v = (float)this.numValue.Value / 100.0f;
			this.UpdateColorControls();
		}
		private void numRed_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			Color tmp = this.selColor.ToColor();
			this.selColor = new InternalColor(Color.FromArgb(
				tmp.A,
				(byte)this.numRed.Value,
				tmp.G,
				tmp.B));
			this.UpdateColorControls();
		}
		private void numGreen_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			Color tmp = this.selColor.ToColor();
			this.selColor = new InternalColor(Color.FromArgb(
				tmp.A,
				tmp.R,
				(byte)this.numGreen.Value,
				tmp.B));
			this.UpdateColorControls();
		}
		private void numBlue_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			Color tmp = this.selColor.ToColor();
			this.selColor = new InternalColor(Color.FromArgb(
				tmp.A,
				tmp.R,
				tmp.G,
				(byte)this.numBlue.Value));
			this.UpdateColorControls();
		}
		private void numAlpha_ValueChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			Color tmp = this.selColor.ToColor();
			this.selColor = new InternalColor(Color.FromArgb(
				(byte)this.numAlpha.Value,
				tmp.R,
				tmp.G,
				tmp.B));
			this.UpdateColorControls();
		}
		private void textBoxHex_TextChanged(object sender, EventArgs e)
		{
			if (this.suspendTextEvents) return;
			int argb;
			if (int.TryParse(this.textBoxHex.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentUICulture, out argb))
			{
				Color tmp = Color.FromArgb(argb);
				this.selColor = new InternalColor(tmp);
				this.UpdateColorControls();
			}
		}

		private void colorShowBox_UpperClick(object sender, EventArgs e)
		{
			this.selColor = this.oldColor;
			this.UpdateColorControls();
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Hide();
		}
		private void ColorPickerDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.DialogResult == DialogResult.Cancel)
				this.SelectedColor = this.OldColor;
			else
				this.OldColor = this.SelectedColor;
		}
	}
}
