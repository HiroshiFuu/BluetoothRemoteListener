using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Net.Sockets;

namespace ThirtyTwoFeet
{
	/// <summary>
	/// Processes incoming commands
	/// </summary>
	public class RemoteListenerForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private BluetoothListener bl;

		//unique service identifier
		private Guid service = new Guid("{4c9e60b8-007a-4476-b8f0-fb4e73f0eade}");

        private delegate void setButtonEnabledHandler(bool enabled);
        private setButtonEnabledHandler setButtonEnabled;
        private delegate void setTextHandler(string text);
        private setTextHandler setRcvText;
        private delegate void setLabelStatusHandler(string text, Color c);
        private setLabelStatusHandler setLabelStatus;

        private System.IO.Stream ns;
        private Button btnListen;
        private bool running = true;

        private System.Windows.Forms.Label lblVer;
		private System.Windows.Forms.NotifyIcon trayIcon;
		private System.Windows.Forms.PictureBox pictureBox1;
        private BackgroundWorker bgworkerRunning;
        private RichTextBox rtbMsg;
        private Label lbStatus;
        private BackgroundWorker bgworkerSend;
        private Timer timerSendMsg;

        public RemoteListenerForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteListenerForm));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblVer = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnListen = new System.Windows.Forms.Button();
            this.bgworkerRunning = new System.ComponentModel.BackgroundWorker();
            this.rtbMsg = new System.Windows.Forms.RichTextBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.timerSendMsg = new System.Windows.Forms.Timer(this.components);
            this.bgworkerSend = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Bluetooth Remote";
            this.trayIcon.Visible = true;
            this.trayIcon.Click += new System.EventHandler(this.trayIcon_Click);
            // 
            // lblVer
            // 
            this.lblVer.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVer.Location = new System.Drawing.Point(68, 12);
            this.lblVer.Name = "lblVer";
            this.lblVer.Size = new System.Drawing.Size(232, 24);
            this.lblVer.TabIndex = 0;
            this.lblVer.Text = "32feet.NET Bluetooth Remote v3.0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnListen
            // 
            this.btnListen.AutoSize = true;
            this.btnListen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnListen.Location = new System.Drawing.Point(71, 39);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(82, 23);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "Start to Listen";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // bgworkerRunning
            // 
            this.bgworkerRunning.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerListen_DoWork);
            this.bgworkerRunning.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerListen_RunWorkerCompleted);
            // 
            // rtbMsg
            // 
            this.rtbMsg.Location = new System.Drawing.Point(12, 68);
            this.rtbMsg.Name = "rtbMsg";
            this.rtbMsg.Size = new System.Drawing.Size(609, 368);
            this.rtbMsg.TabIndex = 3;
            this.rtbMsg.Text = "";
            this.rtbMsg.DoubleClick += new System.EventHandler(this.rtbMsg_DoubleClick);
            // 
            // lbStatus
            // 
            this.lbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbStatus.Location = new System.Drawing.Point(159, 36);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(335, 24);
            this.lbStatus.TabIndex = 4;
            this.lbStatus.Text = "Disconnected";
            // 
            // timerSendMsg
            // 
            this.timerSendMsg.Interval = 2000;
            this.timerSendMsg.Tick += new System.EventHandler(this.timerSendMsg_Tick);
            // 
            // bgworkerSend
            // 
            this.bgworkerSend.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerSend_DoWork);
            this.bgworkerSend.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerSend_RunWorkerCompleted);
            // 
            // RemoteListenerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(633, 448);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.rtbMsg);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblVer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RemoteListenerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bluetooth Remote";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.RemoteListenerForm_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new RemoteListenerForm());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
            setButtonEnabled = new setButtonEnabledHandler(setButtonEnabledMethod);
            setRcvText = new setTextHandler(setRcvTextMethod);
            setLabelStatus = new setLabelStatusHandler(setLabelStatusMethod);
            BluetoothRadio br = BluetoothRadio.PrimaryRadio;
            if (br == null) {
                MessageBox.Show("No supported Bluetooth radio/stack found.");
                btnListen.Enabled = false;
            } else if (br.Mode != InTheHand.Net.Bluetooth.RadioMode.Discoverable) {
                DialogResult rslt = MessageBox.Show("Make BluetoothRadio Discoverable?", "Bluetooth Remote Listener", MessageBoxButtons.YesNo);
                if (rslt == DialogResult.Yes) {
                    br.Mode = RadioMode.Discoverable;
                }
                else btnListen.Enabled = false;
            }
            bl = new BluetoothListener(service);
        }

		private void RemoteListenerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			running = false;
			bl.Stop();
		}

        public void setButtonEnabledMethod(bool enabled)
        {
            btnListen.Enabled = enabled;
        }

        public void setRcvTextMethod(string text)
        {
            rtbMsg.Text += text + "\n";
        }

        public void setLabelStatusMethod(string text, Color c)
        {
            lbStatus.Text = text;
            lbStatus.ForeColor = c;
        }

        private AsyncCallback AsyncReceiveCallback = new AsyncCallback(ProcessReceiveResults);

        static void ProcessReceiveResults(IAsyncResult ar)
        {
            ns.EndRead(ar);
            this.Invoke(setRcvText, BitConverter.ToString(buffer));
        }

        private void RunningLoop()
		{
			byte[] buffer = new byte[20];
			int received = 0;

			while (running)
			{
                
                while (running)
				{
					try
					{
                        ns.BeginRead(buffer, 0, 20, AsyncReceiveCallback, this);
						//received = ns.Read(buffer, 0, 20);
					}
					catch (Exception e)
                    {
                        //Console.WriteLine(e.ToString());
                        continue;
					}

                    if (received > 0)
					{
                        //System.Diagnostics.Trace.WriteLine(BitConverter.ToString(buffer));
                        //this.Invoke(setRcvText, BitConverter.ToString(buffer));
                        if (buffer[0] == 0x01)
                        {
                            string text = System.Text.Encoding.UTF8.GetString(buffer, 2, buffer.Length - 2) + " id:" + buffer[1] + " connected";
                            this.Invoke(setLabelStatus, new object[] { text, Color.Lime });
                        }
                        //string command = "";

                        //Keys keycode = (Keys)BitConverter.ToInt16(buffer, 0);

                        //switch(keycode)
                        //{
                        //	case Keys.Up:
                        //		command = "{UP}";
                        //		break;
                        //	case Keys.Down:
                        //		command = "{DOWN}";
                        //		break;
                        //	case Keys.Left:
                        //		command = "{LEFT}";
                        //		break;
                        //	case Keys.Right:
                        //		command = "{RIGHT}";
                        //		break;
                        //	case Keys.Enter:
                        //		command = "{ENTER}";
                        //		break;
                        //System.Diagnostics.Trace.WriteLine(@"SendWait(""" + command + @""")");
                        //System.Windows.Forms.SendKeys.SendWait(command);
                    }
					else
					{
                        Console.WriteLine("connection lost");
                        running = false;
                        break;
					}
				}

				try
				{
					bc.Close();
                    Console.WriteLine("closed");
                    running = false;
                    break;
                }
				catch
                {
                    running = false;
                    break;
                }
			}
        }

		private void trayIcon_Click(object sender, System.EventArgs e)
		{
			this.WindowState = FormWindowState.Normal;
		}

        private void btnListen_Click(object sender, EventArgs e)
        {
            btnListen.Enabled = false;

            bl.Start();
            running = true;

            bgworkerRunning.RunWorkerAsync();
        }

        private void bgWorkerListen_DoWork(object sender, DoWorkEventArgs e)
        {
            BluetoothClient bc;
            try
            {
                bc = bl.AcceptBluetoothClient();
                ns = bc.GetStream();
                Console.WriteLine("connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            RunningLoop();
        }

        private void bgWorkerListen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setButtonEnabled(true);
            bl.Stop();
            this.Invoke(setLabelStatus, new object[] { "Disconnected", Color.Red });
            ns = null;
        }

        private void rtbMsg_DoubleClick(object sender, EventArgs e)
        {
            rtbMsg.Clear();
        }

        private void timerSendMsg_Tick(object sender, EventArgs e)
        {
            byte[] data = new byte[20];
            for (byte i = 0; i < 20; i++)
                data[i] = i;
            if (running && ns != null)
                try
                {
                    ns.Write(data, 0, 20);
                    Console.WriteLine("data sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
        }

        private void bgworkerSend_DoWork(object sender, DoWorkEventArgs e)
        {
            timerSendMsg.Start();
            while (running) { }
        }

        private void bgworkerSend_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timerSendMsg.Stop();
        }
    }
}
