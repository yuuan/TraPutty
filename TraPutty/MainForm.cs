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

namespace TraPutty
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 設定
        /// </summary>
        private ApplicationSetting settings = new ApplicationSetting();

        /// <summary>
        /// 設定を変更するためのフォーム
        /// </summary>
        private SettingForm settingForm = new SettingForm();

        /// <summary>
        /// 透過する PuTTY ウィンドウのリスト
        /// </summary>
        private WindowList putties = new WindowList();

        /// <summary>
        /// PuTTY ウィンドウを探してターゲットに追加する
        /// </summary>
        private void SearchPutty() {
            this.putties.RemoveAll(window => window.NotExists);

            foreach (var process in Process.GetProcesses()) {
                if (Window.IsPuttyProcess(process)) {
                    this.putties.FindOrCreate(process);
                }
            }
        }

        /// <summary>
        /// PuTTY ウィンドウに透過状態を適用する
        /// </summary>
        private void TransparentPutty() {
            foreach (var window in this.putties) {
                if (! window.IsPuttyConfig || ! this.settings.notTransparentPuttySetting) {
                    window.Transparency = this.settings.puttysAreTransparent ? Window.NOT_TRANSPARENT : this.settings.alpha;
                }
            }
        }

        /// <summary>
        /// PuTTY の透過状態を切り替える
        /// </summary>
        private void ToggleTransparencyOfPuttys() {
			this.settings.puttysAreTransparent = ! this.settings.puttysAreTransparent;

            this.TransparentPutty();
		}

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public MainForm() {
			InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
			this.Enabled = false;

            this.settings = ApplicationSetting.Load();

            this.SearchPutty();

            timer.Enabled = true;
        }

        /// <summary>
        /// 起動時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void MainForm_Load(object sender, EventArgs e) {
            this.Hide();
		}

        /// <summary>
        /// 終了前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			this.settings.Save();
		}

        /// <summary>
        /// タスクトレイからメニューを開いたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void contextMenuStrip_Opening(object sender, CancelEventArgs e) {
			this.Cursor = Cursors.WaitCursor;

            this.TopMost = false;

			var menuSep1 = new ToolStripSeparator();
			var menuSep2 = new ToolStripSeparator();
			var menuOption = new ToolStripMenuItem("設定(&S)...");
			menuOption.Click += new EventHandler(this.menuItemOption_Click);
			var menuExit = new ToolStripMenuItem("終了(&X)");
			menuExit.Click += new EventHandler(this.menuItemExit_Click);

			contextMenuStrip.Items.Clear();
			contextMenuStrip.ImageList = this.imageList;

			foreach (var process in Process.GetProcesses().OrderBy(p => p.ProcessName)) {
                if (process.MainWindowHandle != IntPtr.Zero) {
                    if (! this.settings.ngwords.Contains(process.ProcessName)) {
                        var window = new Window(process);

                        var item = new ToolStripMenuItem(process.ProcessName);
                        item.ToolTipText = process.MainWindowTitle;
                        item.Tag = process;
                        item.ImageIndex = window.IsTransparents ? 1 : 0;
                        item.Checked = window.IsTransparents;
                        item.Text += (window.IsTransparents) ? " *" : "";
                        item.Click += new EventHandler(this.menuItemProcess_Click);
                        contextMenuStrip.Items.Add(item);
                    }
                }
			}

			contextMenuStrip.Items.Add(menuSep1);
			contextMenuStrip.Items.Add(menuOption);
			contextMenuStrip.Items.Add(menuSep2);
			contextMenuStrip.Items.Add(menuExit);

			this.Cursor = Cursors.Default;
		}

        /// <summary>
        /// 設定メニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void menuItemOption_Click(object sender, EventArgs e) {
            settingForm.SetSetting(this.settings);

            if (settingForm.Visible == false) {
                var result = settingForm.ShowDialog(this);
			    if (result == DialogResult.OK) {
				    this.settings = settingForm.GetSetting();
				    this.settings.Save();
			    }
            }
		}

        /// <summary>
        /// 終了メニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void menuItemExit_Click(object sender, EventArgs e) {
			notifyIcon.Visible = false;
            Application.Exit();
		}

        /// <summary>
        /// メニューからプロセスを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void menuItemProcess_Click(object sender, EventArgs e) {
			var item = (ToolStripMenuItem)sender;
			var process = (Process)item.Tag;
            var window = new Window(process);

            if (window.IsTransparents) {
                window.Transparency = Window.NOT_TRANSPARENT;
            }
            else {
                window.Transparency = this.settings.alpha;
            }
		}

        /// <summary>
        /// ただちに PuTTY の透過状態を切り替える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
			this.ToggleTransparencyOfPuttys();
		}

        /// <summary>
        /// 定期的に
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void timer_Tick(object sender, EventArgs e) {
            this.SearchPutty();
            this.TransparentPutty();
		}

        /// <summary>
        /// メインフォームのラベルをクリックしたら
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void labelTitle_Click(object sender, EventArgs e) {
			this.Hide();
		}

        /// <summary>
        /// メインフォームをクリックしたら
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Click(object sender, EventArgs e) {
            this.Hide();
        }
    }
}
