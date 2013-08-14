﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace TraPutty
{
	public partial class SettingForm : Form
	{
		private ApplicationSetting setting;

		public SettingForm(ApplicationSetting setting) {
			this.setting = setting;
			InitializeComponent();
		}

		public ApplicationSetting GetSetting() {
			return this.setting;
		}

		private void SettingForm_Load(object sender, EventArgs e) {
			this.numTransparent.Value = (int)this.setting.alphaValue;
			this.chkNotTransparentPuttySetting.Checked = this.setting.notTransparentPuttySetting;
		}

		private void buttonCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void buttonOk_Click(object sender, EventArgs e) {
			this.setting.alphaValue = (byte)numTransparent.Value;
			this.setting.notTransparentPuttySetting = chkNotTransparentPuttySetting.Checked;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start("http://www.yuuan.net/");
		}
	}
}