using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Net.Sockets;
using System.IO;

namespace ThirtyTwoFeet
{
	/// <summary>
	/// Processes incoming commands
	/// </summary>
	public class BluetoothCommForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private BluetoothListener bl;

        //unique service identifier
        private Guid service = new Guid("{00001101-0000-1000-8000-00805F9B34FB}");

        private delegate void setButtonEnabledHandler(bool enabled);
        private setButtonEnabledHandler setButtonEnabled;
        private delegate void setTextHandler(string text);
        private setTextHandler setRcvText;
        private setTextHandler setSendText;
        private delegate void setLabelStatusHandler(string text, Color c);
        private setLabelStatusHandler setLabelStatus;
        
        private BluetoothClient bc;
        private Stream ns;
        private Button btnListen;
        private bool running = true;
		private NotifyIcon trayIcon;
		private PictureBox pictureBox1;
        private BackgroundWorker bgworkerRunning;
        private RichTextBox rtbRcvMsg;
        private Label lbStatus;
        private Timer timerSendMsg;
        private Label label1;
        private Label label2;
        private RichTextBox rtbSendMsg;
        private int counter = 0;

        public BluetoothCommForm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BluetoothCommForm));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnListen = new System.Windows.Forms.Button();
            this.bgworkerRunning = new System.ComponentModel.BackgroundWorker();
            this.rtbRcvMsg = new System.Windows.Forms.RichTextBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.timerSendMsg = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rtbSendMsg = new System.Windows.Forms.RichTextBox();
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
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnListen
            // 
            this.btnListen.AutoSize = true;
            this.btnListen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnListen.Location = new System.Drawing.Point(50, 15);
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
            // rtbRcvMsg
            // 
            this.rtbRcvMsg.Location = new System.Drawing.Point(342, 78);
            this.rtbRcvMsg.Name = "rtbRcvMsg";
            this.rtbRcvMsg.Size = new System.Drawing.Size(321, 358);
            this.rtbRcvMsg.TabIndex = 3;
            this.rtbRcvMsg.Text = "";
            this.rtbRcvMsg.TextChanged += new System.EventHandler(this.rtbMsg_TextChanged);
            this.rtbRcvMsg.DoubleClick += new System.EventHandler(this.rtbMsg_DoubleClick);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbStatus.Location = new System.Drawing.Point(138, 15);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(126, 24);
            this.lbStatus.TabIndex = 4;
            this.lbStatus.Text = "Disconnected";
            // 
            // timerSendMsg
            // 
            this.timerSendMsg.Enabled = true;
            this.timerSendMsg.Interval = 1000;
            this.timerSendMsg.Tick += new System.EventHandler(this.timerSendMsg_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "Send Message:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(339, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 18);
            this.label2.TabIndex = 8;
            this.label2.Text = "Receving Message:";
            // 
            // rtbSendMsg
            // 
            this.rtbSendMsg.Location = new System.Drawing.Point(15, 78);
            this.rtbSendMsg.Name = "rtbSendMsg";
            this.rtbSendMsg.Size = new System.Drawing.Size(321, 358);
            this.rtbSendMsg.TabIndex = 7;
            this.rtbSendMsg.Text = "";
            this.rtbSendMsg.TextChanged += new System.EventHandler(this.rtbMsg_TextChanged);
            this.rtbSendMsg.DoubleClick += new System.EventHandler(this.rtbMsg_DoubleClick);
            // 
            // BluetoothCommForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(676, 448);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rtbSendMsg);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.rtbRcvMsg);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BluetoothCommForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bluetooth Communication";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.BluetoothCommForm_Closing);
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
			Application.Run(new BluetoothCommForm());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
            setButtonEnabled = new setButtonEnabledHandler(setButtonEnabledMethod);
            setRcvText = new setTextHandler(setRcvTextMethod);
            setSendText = new setTextHandler(setSendTextMethod);
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

		private void BluetoothCommForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			running = false;
			bl.Stop();
        }

        private void trayIcon_Click(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        public void setButtonEnabledMethod(bool enabled)
        {
            btnListen.Enabled = enabled;
        }

        public void setRcvTextMethod(string text)
        {
            rtbRcvMsg.Text += text + "\n";
        }

        public void setSendTextMethod(string text)
        {
            rtbSendMsg.Text += text + "\n";
        }

        public void setLabelStatusMethod(string text, Color c)
        {
            lbStatus.Text = text;
            lbStatus.ForeColor = c;
        }

        public class BluetoothStreamHandler
        {
            private byte[] buffer = new byte[20];
            private Stream BluetoothStream = null;
            private BluetoothCommForm bcFm = null;

            private AsyncCallback AsyncReceiveCallback = new AsyncCallback(ProcessReceiveResults);

            public BluetoothStreamHandler(BluetoothClient bc, BluetoothCommForm fm)
            {
                bcFm = fm;
                try
                {
                    BluetoothStream = bc.GetStream();
                    BluetoothStream.BeginRead(buffer, 0, buffer.Length, AsyncReceiveCallback, this);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            static void ProcessReceiveResults(IAsyncResult ar)
            {
                BluetoothStreamHandler BSH = (BluetoothStreamHandler)ar.AsyncState;
                try
                {
                    int bytesToRead = 0;
                    bytesToRead = BSH.BluetoothStream.EndRead(ar);
                    if (bytesToRead == 0)
                    {
                        BSH.BluetoothStream.Close();
                        return;
                    }
                    try
                    {
                        if (BSH.buffer[0] == 0xCA)
                        {
                            string text = System.Text.Encoding.UTF8.GetString(BSH.buffer, 2, BSH.buffer.Length - 2) + " id:" + BSH.buffer[1] + " connected";
                            text = text.Replace("\a", "");
                            BSH.bcFm.Invoke(BSH.bcFm.setLabelStatus, new object[] { text, Color.Green });
                        }
                        BSH.bcFm.Invoke(BSH.bcFm.setRcvText, (BitConverter.ToString(BSH.buffer)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    BSH.BluetoothStream.BeginRead(BSH.buffer, 0, BSH.buffer.Length, BSH.AsyncReceiveCallback, BSH);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to read from a bluetooth stream with error: " + e.Message);
                    BSH.BluetoothStream.Close();
                }
            }
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            btnListen.Enabled = false;
            bgworkerRunning.RunWorkerAsync();
        }

        private void bgWorkerListen_DoWork(object sender, DoWorkEventArgs e)
        {
            running = true;
            bl.Start();
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
            BluetoothStreamHandler BSH = new BluetoothStreamHandler(bc, this);
            while (running) { }
        }

        private void bgWorkerListen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                bc.Close();
                Console.WriteLine("closed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            bl.Stop();
            try
            {
                this.Invoke(setLabelStatus, new object[] { "Disconnected", Color.Red });
            }
            catch { }
            ns = null;
            setButtonEnabled(true);
        }

        private void rtbMsg_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)sender;
            rtb.SelectionStart = rtb.Text.Length;
            rtb.ScrollToCaret();
        }

        private void rtbMsg_DoubleClick(object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)sender;
            rtb.Clear();
        }

        private void timerSendMsg_Tick(object sender, EventArgs e)
        {
            if (running && ns != null)
            {
                int data_size = 16;
                byte[] data = new byte[data_size];
                data[0] = 0x00;     //
                data[1] = 0x53;     //Start Id 'S'
                Random rnd = new Random();
                for (int i = 2; i < 14; i += 4)
                {
                    int amp_freq = 100;
                    int amp_mag = 100;
                    int j = (i - 2) / 4;
                    int freq = rnd.Next(100 + 100 * j, 200 + 100 * j);
                    double mag = rnd.NextDouble();
                    int freq_val = freq * amp_freq;
                    int mag_val = (int)(mag * amp_mag);
                    data[i] = (byte)(freq_val / 256);
                    data[i + 1] = (byte)(freq_val % 256);
                    data[i + 2] = (byte)(mag_val / 256);
                    data[i + 3] = (byte)(mag_val % 256);
                    //Console.WriteLine(freq_val + " " + data[i].ToString("X") + " " + data[i + 1].ToString("X"));
                }
                data[14] = 0x00;     //
                data[15] = 0x46;     //End Id 'F'
                try
                {
                    ns.Write(data, 0, data_size);
                    this.Invoke(setSendText, BitConverter.ToString(data));
                    Console.WriteLine("data sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("connection lost");
                    running = false;
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
