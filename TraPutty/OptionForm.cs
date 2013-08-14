using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace TransparentizePutty
{
    public partial class OptionForm : Form
    {
		private class ProcessInfo
		{
			public bool transparent = false;
			public IntPtr hWnd = IntPtr.Zero;
			public string name;
			public string title;
			public bool exists = true;

			public ProcessInfo(IntPtr hWnd, string name, string title) {
				this.hWnd = hWnd;
				this.name = name;
				this.title = title;
			}

			public ProcessInfo(Process process) {
				Update(process);
			}

			public void Update(Process process) {
				this.hWnd = process.MainWindowHandle;
				this.name = process.ProcessName;
				this.title = process.MainWindowTitle;
				this.exists = true;
			}
		}

		public class ApplicationSetting
		{
			[System.Xml.Serialization.XmlElement("alpha")]
			public byte alphaValue = 210;
			public bool puttysAreTransparent = false;
			public bool notTransparentPuttySetting = false;
			public string[] ngwords = new string[] {"sidebar", "Saezuri"};
		}

		private List<ProcessInfo> processList = new List<ProcessInfo>();
		private ApplicationSetting setting = new ApplicationSetting();
		private bool allowExit = false;

		private void SaveSetting() {
			try {
				XmlSerializer sirializer = new XmlSerializer(typeof(ApplicationSetting));
				FileStream stream = new FileStream("TraPutty.xml", FileMode.Create);
				sirializer.Serialize(stream, this.setting);
				stream.Close();
			}
			catch { }
		}

		private void LoadSettings() {
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(ApplicationSetting));
				FileStream stream = new FileStream("TraPutty.xml", FileMode.Open);
				this.setting = (ApplicationSetting)serializer.Deserialize(stream);
			}
			catch {
				this.setting = new ApplicationSetting();
			}
		}

		private void ResetProcessesInfo() {
			for (int i = 0; i < processList.Count; i++) {
				processList[i].exists = false;
			}

			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes) {
				if (process.MainWindowHandle != IntPtr.Zero) {
					bool ng = false;
					foreach (string ngword in this.setting.ngwords) {
						if (process.ProcessName.Equals(ngword, StringComparison.CurrentCultureIgnoreCase)) ng = true;
					}
					if (!ng) {
						bool exists = false;
						for (int i = 0; i < processList.Count; i++) {
							ProcessInfo processInfo = processList[i];
							if (processInfo.hWnd == process.MainWindowHandle) {
								int exStyle = Win32API.GetWindowLong(processInfo.hWnd, Win32API.GWL_EXSTYLE);
								if ((exStyle & (int)Win32API.WS_EX.LAYERED) != 0) {
									byte alpha;
									Win32API.GetLayeredWindowAttributes(processInfo.hWnd, 0, out alpha, 0);
									processInfo.transparent = (alpha != 255) ? true : false;
								}
								processInfo.Update(process);
								exists = true;
								break;
							}
						}
						if (!exists) {
							ProcessInfo pi = new ProcessInfo(process);
							this.processList.Add(pi);

							if (this.setting.puttysAreTransparent) {
								SetTransparentForPutty(pi, true);
							}
						}
					}
				}
			}

			processList.Sort(delegate(ProcessInfo x, ProcessInfo y) {
				return x.name.CompareTo(y.name);
			});

			for (int i = 0; i < processList.Count; i++ ) {
				if (!processList[i].exists) {
					processList.Remove(processList[i]);
				}
			}
		}

		private bool SetTransparent(IntPtr hWnd, bool transparent) {
			if (transparent) {
				int exStyle = Win32API.GetWindowLong(hWnd, Win32API.GWL_EXSTYLE);
				Win32API.SetWindowLong(hWnd, Win32API.GWL_EXSTYLE, exStyle | (int)Win32API.WS_EX.LAYERED);

				return Win32API.SetLayeredWindowAttributes(hWnd, 0, this.setting.alphaValue, Win32API.LWA_ALPHA);
			}
			else {
				return Win32API.SetLayeredWindowAttributes(hWnd, 0, 255, Win32API.LWA_ALPHA);
			}
		}

		private void SetTransparentForAllPutty() {
			this.setting.puttysAreTransparent = !this.setting.puttysAreTransparent;
			foreach (ProcessInfo process in processList) {
				this.SetTransparentForPutty(process, this.setting.puttysAreTransparent);
			}
		}

		private void SetTransparentForPutty(ProcessInfo process, bool transparent) {
			if (process.name.Equals("putty", StringComparison.CurrentCultureIgnoreCase)) {
				if (this.setting.notTransparentPuttySetting && process.title.Equals("PuTTY 設定"))
					transparent = false;
				SetTransparent(process.hWnd, transparent);
			}
		}

		public OptionForm() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			LoadSettings();
			ResetProcessesInfo();
			timer.Enabled = true;
		}

		private void Form1_Shown(object sender, EventArgs e) {
			this.Hide();
		}

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e) {
			this.Cursor = Cursors.WaitCursor;

			ToolStripSeparator menuSep1 = new ToolStripSeparator();
			ToolStripSeparator menuSep2 = new ToolStripSeparator();
			ToolStripMenuItem menuOption = new ToolStripMenuItem("設定(&S)");
			menuOption.Click += new EventHandler(this.menuItemOption_Click);
			ToolStripMenuItem menuExit = new ToolStripMenuItem("終了(&X)");
			menuExit.Click += new EventHandler(this.menuItemExit_Click);

			ResetProcessesInfo();
			contextMenuStrip.Items.Clear();
			contextMenuStrip.ImageList = this.imageList;

			foreach (ProcessInfo process in this.processList) {
				if (process.exists) {
					ToolStripMenuItem item = new ToolStripMenuItem(process.name);
					item.ToolTipText = process.title;
					item.Tag = process;
					item.ImageIndex = (process.transparent) ? 1 : 0;
					item.Checked = process.transparent;
					item.Text += (process.transparent) ? " *" : "";
					item.Click += new EventHandler(this.menuItemProcess_Click);
					contextMenuStrip.Items.Add(item);
				}
			}

			contextMenuStrip.Items.Add(menuSep1);
			contextMenuStrip.Items.Add(menuOption);
			contextMenuStrip.Items.Add(menuSep2);
			contextMenuStrip.Items.Add(menuExit);

			this.Cursor = Cursors.Default;
		}

		private void menuItemOption_Click(object sender, EventArgs e) {
			this.numTransparent.Value = (int)this.setting.alphaValue;
			this.chkNotTransparentPuttySetting.Checked = this.setting.notTransparentPuttySetting;
			this.Enabled = true;
			this.Show();
			this.Activate();
		}

		private void menuItemExit_Click(object sender, EventArgs e) {
			SaveSetting();
			this.allowExit = true;
			this.Close();
		}

		private void menuItemProcess_Click(object sender, EventArgs e) {
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			ProcessInfo processInfo = (ProcessInfo)item.Tag;

			bool transparent = !processInfo.transparent;
			if (SetTransparent(processInfo.hWnd, transparent)) {
				processInfo.transparent = transparent;
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e) {
			this.Hide();
		}

		private void buttonOk_Click(object sender, EventArgs e) {
			this.setting.alphaValue = (byte)numTransparent.Value;
			this.setting.notTransparentPuttySetting = chkNotTransparentPuttySetting.Checked;
			this.Hide();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			if (!this.allowExit) {
				e.Cancel = true;
				this.Hide();
			}
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
			SetTransparentForAllPutty();
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start("http://www.yuuan.net/blog/");
		}

		private void timer_Tick(object sender, EventArgs e) {
			ResetProcessesInfo();
		}
    }
}
