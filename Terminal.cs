/*=============================================================================
 File:			Terminal.cs					Serial terminal/debugger Class. 

 Created:		1.00	10/13/03	TJK		Timothy J. Krell	(SAIC)

 Version:		1.00

 Revisions:		1.00	10/13/03	TJK		Initial Release.

 Copyright(c)	2003, Science Applications International Corporation (SAIC)
				All Rights Reserved. Sponsored by the U.S. Government under
				Contract #DAAH01-00-D-0013/10. This software may be reproduced
				by or for the US Government pursuant to the copyright license
				under clause at DFARS 252.227-7014 (June 1995).
=============================================================================*/

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using SerialPorts;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace SerialTerminal
{
	/// <summary>
	/// Summary description for Terminal.
	/// </summary>
	public class Terminal : System.Windows.Forms.Form
	{
		#region Members
		private System.Windows.Forms.Label LblPort;
		private System.Windows.Forms.Label LblTxData;
		private System.Windows.Forms.Label LblRxData;
		private System.Windows.Forms.Label LblSettings;
		private System.Windows.Forms.Label LblReadRate;

		private System.Windows.Forms.Button CmdOpen;
		private System.Windows.Forms.Button CmdConfig;
		private System.Windows.Forms.Button CmdClear;

		private System.Windows.Forms.CheckBox ChkRxEcho;
		private System.Windows.Forms.CheckBox ChkUpdate;
		private System.Windows.Forms.CheckBox ChkRxDec;
		private System.Windows.Forms.CheckBox ChkTxDec;

		internal System.Windows.Forms.TextBox TxData;
		internal System.Windows.Forms.TextBox RxData;
		internal System.Windows.Forms.TextBox Settings;
		internal System.Windows.Forms.TextBox RecvRate;

		private System.Windows.Forms.Timer RecvTimer;
		private System.Windows.Forms.ComboBox ComPort;
		private System.Windows.Forms.GroupBox groupBox83;

		private System.ComponentModel.IContainer components;

		private SerialPort Port;
		private Button button1;
		private Button button2;
		private Button button3;
		private Label label1;
		private Label label2;
		private Label label3;
		private NumericUpDown numericUpDown1;
		private NumericUpDown numericUpDown2;
		private NumericUpDown numericUpDown3;
		private ComboBox comboBox1;
		private ComboBox comboBox2;
		private Button button4;
		private Button button5;
		private Label label4;
		private Label label5;
		private TabControl tabVisca;
		private TabPage tabMenu;
		private TabPage tabTest;
		private GroupBox VISCABox;
		private Button VISCA1button;
		private Button VISCA2button;
		private Button VISCA3button;
		private Button VISCA4button;
		private Button VISCA5button;
		private Button VISCA6button;
		private Button VISCA7button;
		private Button VISCA8button;
		private TextBox VISCA1Box;
		private TextBox VISCA2Box;
		private TextBox VISCA3Box;
		private TextBox VISCA4Box;
		private TextBox VISCA5Box;
		private TextBox VISCA6Box;
		private TextBox VISCA7Box;
		private TextBox VISCA8Box;
		private Label LabelFWVersion;
		private GroupBox groupBox14;

		// Terminal functions that handle base class events.
		private WithEvents Func;

		// Terminal app constants.
		private const int MAX_PORTS = 32;
		private const int DEFAULT_SPEED_LEVEL = 100;

		// Terminal app locals.
		private int TxBytes = 0;
		private int RxBytes = 0;
		private string TxString = "";
		private string RxString = "";
		private byte countRxPacket = 0;
		private byte lengthRxPacket;

		private readonly byte[] RxPacket = new byte[16];
		private readonly byte[] localRxPacket = new byte[16];

		private GeneralCommand G_eGeneralCommand;
		private readonly ushort[] AngleArray = new ushort[100];
		private readonly double[] TimeArray = new double[100];
		private readonly ushort[] SpeedArray = new ushort[100];
		private readonly double[] SpeedTimeArray = new double[100];

		private GroupBox groupBox1;
		private GroupBox groupBox4;
		private Button ButtonStopShowSpeed;
		private Button ButtonShowSpeed;
		private Chart ChartSpeed;
		private Label LabelFurbo3PanSpd;
		private TextBox TextBoxFurbo3PanSpd;
		private Label LabelFurbo3PanTorque;
		private ComboBox ComboBoxFurbo3PanTorque;
		private Button ButtonFurbo3SetPanSpd;
		private Label LabelFurbo3TossingSpd;
		private TextBox TextBoxFurbo3TossingSpd;
		private Button ButtonFurbo3SetTossingSpd;
		private Label LabelFurbo3FeedingSpd;
		private TextBox TextBoxFurbo3FeedingSpd;
		private Label LabelFurbo3Torque;
		private ComboBox ComboBoxFurbo3FeedingTorque;
		private Button ButtonFurbo3SetFeedingSpd;
		private System.Windows.Forms.Timer TimerUpdateSpeed;

		#endregion

		enum GeneralCommand
		{
			SYS_NONE				= 0x000000,
			SYS_ReadVersion			= 0x090002,
		}

		#region Constructor
		/// <summary>
		/// Terminal constructor. Initialization.
		/// </summary>
		[Obsolete]
		public Terminal()
		{
			InitializeComponent();

			this.ChkTxDec.Checked = false;
			this.ChkRxDec.Checked = false;
			this.ChkRxEcho.Checked = false;

			TxBytes = 0;
			this.TxString = "";
			this.TxData.Text = "";

			RxBytes = 0;
			this.RxString = "";
			this.RxData.Text = "";

			this.Settings.Text = "";
			this.RecvRate.Text = this.RecvTimer.Interval.ToString();

			// Fill com port list.
			this.FillAvailable();

			// Instantiate base class event handlers.
			this.Func = new WithEvents
			{
				Error = new StrnFunc(OnError),
				RxChar = new ByteFunc(this.OnRecvI)
			};

			// Instantiate the terminal port.
			this.Port = new SerialPort(this.Func);
			return;
		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[Obsolete]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.ComPort = new System.Windows.Forms.ComboBox();
			this.Settings = new System.Windows.Forms.TextBox();
			this.LblSettings = new System.Windows.Forms.Label();
			this.LblPort = new System.Windows.Forms.Label();
			this.CmdOpen = new System.Windows.Forms.Button();
			this.TxData = new System.Windows.Forms.TextBox();
			this.ChkUpdate = new System.Windows.Forms.CheckBox();
			this.ChkTxDec = new System.Windows.Forms.CheckBox();
			this.LblTxData = new System.Windows.Forms.Label();
			this.RecvRate = new System.Windows.Forms.TextBox();
			this.LblReadRate = new System.Windows.Forms.Label();
			this.ChkRxEcho = new System.Windows.Forms.CheckBox();
			this.ChkRxDec = new System.Windows.Forms.CheckBox();
			this.RxData = new System.Windows.Forms.TextBox();
			this.LblRxData = new System.Windows.Forms.Label();
			this.CmdConfig = new System.Windows.Forms.Button();
			this.CmdClear = new System.Windows.Forms.Button();
			this.RecvTimer = new System.Windows.Forms.Timer(this.components);
			this.groupBox83 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tabVisca = new System.Windows.Forms.TabControl();
			this.tabMenu = new System.Windows.Forms.TabPage();
			this.ButtonStopShowSpeed = new System.Windows.Forms.Button();
			this.ButtonShowSpeed = new System.Windows.Forms.Button();
			this.ChartSpeed = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.tabTest = new System.Windows.Forms.TabPage();
			this.VISCABox = new System.Windows.Forms.GroupBox();
			this.VISCA1button = new System.Windows.Forms.Button();
			this.VISCA2button = new System.Windows.Forms.Button();
			this.VISCA3button = new System.Windows.Forms.Button();
			this.VISCA4button = new System.Windows.Forms.Button();
			this.VISCA5button = new System.Windows.Forms.Button();
			this.VISCA6button = new System.Windows.Forms.Button();
			this.VISCA7button = new System.Windows.Forms.Button();
			this.VISCA8button = new System.Windows.Forms.Button();
			this.VISCA1Box = new System.Windows.Forms.TextBox();
			this.VISCA2Box = new System.Windows.Forms.TextBox();
			this.VISCA3Box = new System.Windows.Forms.TextBox();
			this.VISCA4Box = new System.Windows.Forms.TextBox();
			this.VISCA5Box = new System.Windows.Forms.TextBox();
			this.VISCA6Box = new System.Windows.Forms.TextBox();
			this.VISCA7Box = new System.Windows.Forms.TextBox();
			this.VISCA8Box = new System.Windows.Forms.TextBox();
			this.LabelFurbo3PanSpd = new System.Windows.Forms.Label();
			this.TextBoxFurbo3PanSpd = new System.Windows.Forms.TextBox();
			this.LabelFurbo3PanTorque = new System.Windows.Forms.Label();
			this.ComboBoxFurbo3PanTorque = new System.Windows.Forms.ComboBox();
			this.ButtonFurbo3SetPanSpd = new System.Windows.Forms.Button();
			this.LabelFurbo3TossingSpd = new System.Windows.Forms.Label();
			this.TextBoxFurbo3TossingSpd = new System.Windows.Forms.TextBox();
			this.ButtonFurbo3SetTossingSpd = new System.Windows.Forms.Button();
			this.LabelFurbo3FeedingSpd = new System.Windows.Forms.Label();
			this.TextBoxFurbo3FeedingSpd = new System.Windows.Forms.TextBox();
			this.LabelFurbo3Torque = new System.Windows.Forms.Label();
			this.ComboBoxFurbo3FeedingTorque = new System.Windows.Forms.ComboBox();
			this.ButtonFurbo3SetFeedingSpd = new System.Windows.Forms.Button();
			this.LabelFWVersion = new System.Windows.Forms.Label();
			this.groupBox14 = new System.Windows.Forms.GroupBox();
			this.TimerUpdateSpeed = new System.Windows.Forms.Timer(this.components);
			this.groupBox83.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			this.tabVisca.SuspendLayout();
			this.tabMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ChartSpeed)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tabTest.SuspendLayout();
			this.VISCABox.SuspendLayout();
			this.groupBox14.SuspendLayout();
			this.SuspendLayout();
			// 
			// ComPort
			// 
			this.ComPort.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ComPort.Location = new System.Drawing.Point(38, 14);
			this.ComPort.MaxDropDownItems = 16;
			this.ComPort.Name = "ComPort";
			this.ComPort.Size = new System.Drawing.Size(60, 21);
			this.ComPort.TabIndex = 1;
			// 
			// Settings
			// 
			this.Settings.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Settings.Location = new System.Drawing.Point(176, 16);
			this.Settings.Name = "Settings";
			this.Settings.ReadOnly = true;
			this.Settings.Size = new System.Drawing.Size(93, 19);
			this.Settings.TabIndex = 2;
			this.Settings.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LblSettings
			// 
			this.LblSettings.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblSettings.ForeColor = System.Drawing.Color.Teal;
			this.LblSettings.Location = new System.Drawing.Point(119, 19);
			this.LblSettings.Name = "LblSettings";
			this.LblSettings.Size = new System.Drawing.Size(54, 13);
			this.LblSettings.TabIndex = 40;
			this.LblSettings.Text = "Settings";
			// 
			// LblPort
			// 
			this.LblPort.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblPort.ForeColor = System.Drawing.Color.Teal;
			this.LblPort.Location = new System.Drawing.Point(8, 19);
			this.LblPort.Name = "LblPort";
			this.LblPort.Size = new System.Drawing.Size(34, 19);
			this.LblPort.TabIndex = 39;
			this.LblPort.Text = "Port";
			// 
			// CmdOpen
			// 
			this.CmdOpen.Location = new System.Drawing.Point(8, 16);
			this.CmdOpen.Name = "CmdOpen";
			this.CmdOpen.Size = new System.Drawing.Size(62, 19);
			this.CmdOpen.TabIndex = 0;
			this.CmdOpen.Text = "OnLine";
			this.CmdOpen.Click += new System.EventHandler(this.CmdOpen_Click);
			// 
			// TxData
			// 
			this.TxData.AcceptsReturn = true;
			this.TxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TxData.Location = new System.Drawing.Point(16, 478);
			this.TxData.Multiline = true;
			this.TxData.Name = "TxData";
			this.TxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.TxData.Size = new System.Drawing.Size(340, 114);
			this.TxData.TabIndex = 39;
			// 
			// ChkUpdate
			// 
			this.ChkUpdate.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChkUpdate.Location = new System.Drawing.Point(39, 458);
			this.ChkUpdate.Name = "ChkUpdate";
			this.ChkUpdate.Size = new System.Drawing.Size(100, 16);
			this.ChkUpdate.TabIndex = 64;
			this.ChkUpdate.Text = "Pause Updates";
			this.ChkUpdate.CheckedChanged += new System.EventHandler(this.ChkUpdate_CheckedChanged);
			// 
			// ChkTxDec
			// 
			this.ChkTxDec.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChkTxDec.Location = new System.Drawing.Point(39, 438);
			this.ChkTxDec.Name = "ChkTxDec";
			this.ChkTxDec.Size = new System.Drawing.Size(67, 16);
			this.ChkTxDec.TabIndex = 42;
			this.ChkTxDec.Text = "Dec Disp";
			this.ChkTxDec.CheckedChanged += new System.EventHandler(this.ChkTxHex_CheckedChanged);
			// 
			// LblTxData
			// 
			this.LblTxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblTxData.ForeColor = System.Drawing.Color.Teal;
			this.LblTxData.Location = new System.Drawing.Point(13, 439);
			this.LblTxData.Name = "LblTxData";
			this.LblTxData.Size = new System.Drawing.Size(27, 13);
			this.LblTxData.TabIndex = 38;
			this.LblTxData.Text = "Tx";
			// 
			// RecvRate
			// 
			this.RecvRate.Font = new System.Drawing.Font("Courier New", 8.861538F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RecvRate.Location = new System.Drawing.Point(538, 453);
			this.RecvRate.Name = "RecvRate";
			this.RecvRate.Size = new System.Drawing.Size(66, 21);
			this.RecvRate.TabIndex = 50;
			this.RecvRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RecvRate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RecvRate_KeyUp);
			// 
			// LblReadRate
			// 
			this.LblReadRate.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblReadRate.Location = new System.Drawing.Point(537, 437);
			this.LblReadRate.Name = "LblReadRate";
			this.LblReadRate.Size = new System.Drawing.Size(67, 13);
			this.LblReadRate.TabIndex = 49;
			this.LblReadRate.Text = "Recv Rate:";
			// 
			// ChkRxEcho
			// 
			this.ChkRxEcho.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChkRxEcho.Location = new System.Drawing.Point(383, 458);
			this.ChkRxEcho.Name = "ChkRxEcho";
			this.ChkRxEcho.Size = new System.Drawing.Size(100, 16);
			this.ChkRxEcho.TabIndex = 44;
			this.ChkRxEcho.Text = "Enable Echo";
			// 
			// ChkRxDec
			// 
			this.ChkRxDec.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChkRxDec.Location = new System.Drawing.Point(383, 440);
			this.ChkRxDec.Name = "ChkRxDec";
			this.ChkRxDec.Size = new System.Drawing.Size(67, 16);
			this.ChkRxDec.TabIndex = 43;
			this.ChkRxDec.Text = "Dec Disp";
			this.ChkRxDec.CheckedChanged += new System.EventHandler(this.ChkRxHex_CheckedChanged);
			// 
			// RxData
			// 
			this.RxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RxData.Location = new System.Drawing.Point(362, 478);
			this.RxData.Multiline = true;
			this.RxData.Name = "RxData";
			this.RxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.RxData.Size = new System.Drawing.Size(340, 114);
			this.RxData.TabIndex = 39;
			// 
			// LblRxData
			// 
			this.LblRxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblRxData.ForeColor = System.Drawing.Color.Teal;
			this.LblRxData.Location = new System.Drawing.Point(357, 441);
			this.LblRxData.Name = "LblRxData";
			this.LblRxData.Size = new System.Drawing.Size(20, 13);
			this.LblRxData.TabIndex = 38;
			this.LblRxData.Text = "Rx";
			// 
			// CmdConfig
			// 
			this.CmdConfig.Location = new System.Drawing.Point(288, 13);
			this.CmdConfig.Name = "CmdConfig";
			this.CmdConfig.Size = new System.Drawing.Size(68, 22);
			this.CmdConfig.TabIndex = 4;
			this.CmdConfig.Text = "Configure";
			this.CmdConfig.Click += new System.EventHandler(this.CmdConfig_Click);
			// 
			// CmdClear
			// 
			this.CmdClear.Location = new System.Drawing.Point(468, 437);
			this.CmdClear.Name = "CmdClear";
			this.CmdClear.Size = new System.Drawing.Size(62, 37);
			this.CmdClear.TabIndex = 62;
			this.CmdClear.Text = "Clear";
			this.CmdClear.Click += new System.EventHandler(this.CmdClear_Click);
			// 
			// RecvTimer
			// 
			this.RecvTimer.Tick += new System.EventHandler(this.RecvTimer_Tick);
			// 
			// groupBox83
			// 
			this.groupBox83.Controls.Add(this.CmdConfig);
			this.groupBox83.Controls.Add(this.Settings);
			this.groupBox83.Controls.Add(this.LblSettings);
			this.groupBox83.Controls.Add(this.ComPort);
			this.groupBox83.Controls.Add(this.LblPort);
			this.groupBox83.Location = new System.Drawing.Point(103, 0);
			this.groupBox83.Name = "groupBox83";
			this.groupBox83.Size = new System.Drawing.Size(376, 48);
			this.groupBox83.TabIndex = 66;
			this.groupBox83.TabStop = false;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(19, 21);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(100, 30);
			this.button1.TabIndex = 6;
			this.button1.Text = "Set Default";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(388, 28);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 30);
			this.button2.TabIndex = 15;
			this.button2.Text = "Clear";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(307, 28);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 30);
			this.button3.TabIndex = 14;
			this.button3.Text = "Set";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(208, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "Function Number";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(113, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "Dome ID";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "Target Number";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(210, 36);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(75, 22);
			this.numericUpDown1.TabIndex = 13;
			this.numericUpDown1.Tag = "";
			this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(115, 36);
			this.numericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(75, 22);
			this.numericUpDown2.TabIndex = 12;
			this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Location = new System.Drawing.Point(19, 36);
			this.numericUpDown3.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(75, 22);
			this.numericUpDown3.TabIndex = 11;
			this.numericUpDown3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "000:Success Start",
            "001:Success Termination",
            "002:End",
            "003:Release",
            "004:Inspection",
            "005:Open Store",
            "006:Shut Store",
            "007:Call For Clerk",
            "008:Break Wire",
            "009:Abnormal Over-Output",
            "010:Abnormal Supply",
            "011:Abnormal Recovery",
            "012:Abnormal Base",
            "013:Abnormal Prize",
            "014:Abnormal Open-Door",
            "015:Glass Open",
            "016:Abnormal Communication/Station",
            "021:Abnormal Radio Wave",
            "022:Abnormal Magnet"});
			this.comboBox1.Location = new System.Drawing.Point(19, 36);
			this.comboBox1.MaxDropDownItems = 22;
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.comboBox1.Size = new System.Drawing.Size(155, 20);
			this.comboBox1.TabIndex = 7;
			this.comboBox1.Text = "000:Success Start";
			// 
			// comboBox2
			// 
			this.comboBox2.Items.AddRange(new object[] {
            "000:Preset",
            "255:None"});
			this.comboBox2.Location = new System.Drawing.Point(185, 36);
			this.comboBox2.MaxDropDownItems = 10;
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(100, 20);
			this.comboBox2.TabIndex = 8;
			this.comboBox2.Tag = "";
			this.comboBox2.Text = "000:Preset";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(377, 29);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 30);
			this.button4.TabIndex = 10;
			this.button4.Text = "Clear";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(296, 29);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 30);
			this.button5.TabIndex = 9;
			this.button5.Text = "Set";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(183, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "AlarmType";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(17, 18);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(75, 12);
			this.label5.TabIndex = 1;
			this.label5.Text = "Alarm Number";
			// 
			// tabVisca
			// 
			this.tabVisca.Controls.Add(this.tabMenu);
			this.tabVisca.Controls.Add(this.tabTest);
			this.tabVisca.Location = new System.Drawing.Point(8, 54);
			this.tabVisca.Name = "tabVisca";
			this.tabVisca.SelectedIndex = 0;
			this.tabVisca.Size = new System.Drawing.Size(1033, 382);
			this.tabVisca.TabIndex = 5;
			// 
			// tabMenu
			// 
			this.tabMenu.Controls.Add(this.ButtonStopShowSpeed);
			this.tabMenu.Controls.Add(this.ButtonShowSpeed);
			this.tabMenu.Controls.Add(this.ChartSpeed);
			this.tabMenu.Controls.Add(this.groupBox1);
			this.tabMenu.Location = new System.Drawing.Point(4, 22);
			this.tabMenu.Name = "tabMenu";
			this.tabMenu.Padding = new System.Windows.Forms.Padding(3);
			this.tabMenu.Size = new System.Drawing.Size(1025, 356);
			this.tabMenu.TabIndex = 1;
			this.tabMenu.Text = "Main";
			this.tabMenu.UseVisualStyleBackColor = true;
			// 
			// ButtonStopShowSpeed
			// 
			this.ButtonStopShowSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonStopShowSpeed.Location = new System.Drawing.Point(917, 275);
			this.ButtonStopShowSpeed.Name = "ButtonStopShowSpeed";
			this.ButtonStopShowSpeed.Size = new System.Drawing.Size(87, 22);
			this.ButtonStopShowSpeed.TabIndex = 32;
			this.ButtonStopShowSpeed.Text = "Stop";
			this.ButtonStopShowSpeed.Click += new System.EventHandler(this.ButtonStopShowSpeed_Click);
			// 
			// ButtonShowSpeed
			// 
			this.ButtonShowSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonShowSpeed.Location = new System.Drawing.Point(917, 247);
			this.ButtonShowSpeed.Name = "ButtonShowSpeed";
			this.ButtonShowSpeed.Size = new System.Drawing.Size(87, 22);
			this.ButtonShowSpeed.TabIndex = 31;
			this.ButtonShowSpeed.Text = "Show Speed";
			this.ButtonShowSpeed.Click += new System.EventHandler(this.ButtonShowSpeed_Click);
			// 
			// ChartSpeed
			// 
			chartArea4.AxisX.Interval = 1D;
			chartArea4.AxisX.LabelStyle.Format = "N0";
			chartArea4.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea4.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea4.AxisX.ScrollBar.BackColor = System.Drawing.Color.White;
			chartArea4.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.White;
			chartArea4.AxisX.Title = "s";
			chartArea4.AxisY.Minimum = 0D;
			chartArea4.Name = "ChartArea1";
			this.ChartSpeed.ChartAreas.Add(chartArea4);
			legend4.Name = "Legend1";
			this.ChartSpeed.Legends.Add(legend4);
			this.ChartSpeed.Location = new System.Drawing.Point(398, 3);
			this.ChartSpeed.Name = "ChartSpeed";
			series4.ChartArea = "ChartArea1";
			series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series4.Legend = "Legend1";
			series4.Name = "V (pps)";
			this.ChartSpeed.Series.Add(series4);
			this.ChartSpeed.Size = new System.Drawing.Size(624, 350);
			this.ChartSpeed.TabIndex = 30;
			this.ChartSpeed.Text = "chart1";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox4);
			this.groupBox1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(6, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(386, 350);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pan/Tilt Control";
			// 
			// groupBox4
			// 
			this.groupBox4.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox4.Location = new System.Drawing.Point(6, 14);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(189, 84);
			this.groupBox4.TabIndex = 34;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Continuous Move";
			// 
			// tabTest
			// 
			this.tabTest.BackColor = System.Drawing.SystemColors.Control;
			this.tabTest.Controls.Add(this.VISCABox);
			this.tabTest.Location = new System.Drawing.Point(4, 22);
			this.tabTest.Name = "tabTest";
			this.tabTest.Padding = new System.Windows.Forms.Padding(3);
			this.tabTest.Size = new System.Drawing.Size(1025, 356);
			this.tabTest.TabIndex = 4;
			this.tabTest.Text = "Test";
			// 
			// VISCABox
			// 
			this.VISCABox.Controls.Add(this.VISCA1button);
			this.VISCABox.Controls.Add(this.VISCA2button);
			this.VISCABox.Controls.Add(this.VISCA3button);
			this.VISCABox.Controls.Add(this.VISCA4button);
			this.VISCABox.Controls.Add(this.VISCA5button);
			this.VISCABox.Controls.Add(this.VISCA6button);
			this.VISCABox.Controls.Add(this.VISCA7button);
			this.VISCABox.Controls.Add(this.VISCA8button);
			this.VISCABox.Controls.Add(this.VISCA1Box);
			this.VISCABox.Controls.Add(this.VISCA2Box);
			this.VISCABox.Controls.Add(this.VISCA3Box);
			this.VISCABox.Controls.Add(this.VISCA4Box);
			this.VISCABox.Controls.Add(this.VISCA5Box);
			this.VISCABox.Controls.Add(this.VISCA6Box);
			this.VISCABox.Controls.Add(this.VISCA7Box);
			this.VISCABox.Controls.Add(this.VISCA8Box);
			this.VISCABox.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCABox.Location = new System.Drawing.Point(16, 16);
			this.VISCABox.Name = "VISCABox";
			this.VISCABox.Size = new System.Drawing.Size(263, 186);
			this.VISCABox.TabIndex = 67;
			this.VISCABox.TabStop = false;
			this.VISCABox.Text = "d";
			// 
			// VISCA1button
			// 
			this.VISCA1button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA1button.Location = new System.Drawing.Point(7, 16);
			this.VISCA1button.Name = "VISCA1button";
			this.VISCA1button.Size = new System.Drawing.Size(50, 20);
			this.VISCA1button.TabIndex = 1;
			this.VISCA1button.Text = "VISCA1";
			this.VISCA1button.Click += new System.EventHandler(this.VISCA1button_Click);
			// 
			// VISCA2button
			// 
			this.VISCA2button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA2button.Location = new System.Drawing.Point(7, 36);
			this.VISCA2button.Name = "VISCA2button";
			this.VISCA2button.Size = new System.Drawing.Size(50, 20);
			this.VISCA2button.TabIndex = 2;
			this.VISCA2button.Text = "VISCA2";
			this.VISCA2button.Click += new System.EventHandler(this.VISCA2button_Click);
			// 
			// VISCA3button
			// 
			this.VISCA3button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA3button.Location = new System.Drawing.Point(7, 56);
			this.VISCA3button.Name = "VISCA3button";
			this.VISCA3button.Size = new System.Drawing.Size(50, 20);
			this.VISCA3button.TabIndex = 3;
			this.VISCA3button.Text = "VISCA3";
			this.VISCA3button.Click += new System.EventHandler(this.VISCA3button_Click);
			// 
			// VISCA4button
			// 
			this.VISCA4button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA4button.Location = new System.Drawing.Point(7, 76);
			this.VISCA4button.Name = "VISCA4button";
			this.VISCA4button.Size = new System.Drawing.Size(50, 20);
			this.VISCA4button.TabIndex = 4;
			this.VISCA4button.Text = "VISCA4";
			this.VISCA4button.Click += new System.EventHandler(this.VISCA4button_Click);
			// 
			// VISCA5button
			// 
			this.VISCA5button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA5button.Location = new System.Drawing.Point(7, 96);
			this.VISCA5button.Name = "VISCA5button";
			this.VISCA5button.Size = new System.Drawing.Size(50, 20);
			this.VISCA5button.TabIndex = 5;
			this.VISCA5button.Text = "VISCA5";
			this.VISCA5button.Click += new System.EventHandler(this.VISCA5button_Click);
			// 
			// VISCA6button
			// 
			this.VISCA6button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA6button.Location = new System.Drawing.Point(7, 116);
			this.VISCA6button.Name = "VISCA6button";
			this.VISCA6button.Size = new System.Drawing.Size(50, 20);
			this.VISCA6button.TabIndex = 6;
			this.VISCA6button.Text = "VISCA6";
			this.VISCA6button.Click += new System.EventHandler(this.VISCA6button_Click);
			// 
			// VISCA7button
			// 
			this.VISCA7button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA7button.Location = new System.Drawing.Point(7, 136);
			this.VISCA7button.Name = "VISCA7button";
			this.VISCA7button.Size = new System.Drawing.Size(50, 20);
			this.VISCA7button.TabIndex = 7;
			this.VISCA7button.Text = "VISCA7";
			this.VISCA7button.Click += new System.EventHandler(this.VISCA7button_Click);
			// 
			// VISCA8button
			// 
			this.VISCA8button.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VISCA8button.Location = new System.Drawing.Point(7, 156);
			this.VISCA8button.Name = "VISCA8button";
			this.VISCA8button.Size = new System.Drawing.Size(50, 20);
			this.VISCA8button.TabIndex = 8;
			this.VISCA8button.Text = "VISCA8";
			this.VISCA8button.Click += new System.EventHandler(this.VISCA8button_Click);
			// 
			// VISCA1Box
			// 
			this.VISCA1Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA1Box.Location = new System.Drawing.Point(60, 16);
			this.VISCA1Box.Name = "VISCA1Box";
			this.VISCA1Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA1Box.TabIndex = 1;
			// 
			// VISCA2Box
			// 
			this.VISCA2Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA2Box.Location = new System.Drawing.Point(60, 36);
			this.VISCA2Box.Name = "VISCA2Box";
			this.VISCA2Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA2Box.TabIndex = 2;
			// 
			// VISCA3Box
			// 
			this.VISCA3Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA3Box.Location = new System.Drawing.Point(60, 56);
			this.VISCA3Box.Name = "VISCA3Box";
			this.VISCA3Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA3Box.TabIndex = 3;
			// 
			// VISCA4Box
			// 
			this.VISCA4Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA4Box.Location = new System.Drawing.Point(60, 76);
			this.VISCA4Box.Name = "VISCA4Box";
			this.VISCA4Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA4Box.TabIndex = 4;
			// 
			// VISCA5Box
			// 
			this.VISCA5Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA5Box.Location = new System.Drawing.Point(60, 96);
			this.VISCA5Box.Name = "VISCA5Box";
			this.VISCA5Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA5Box.TabIndex = 5;
			// 
			// VISCA6Box
			// 
			this.VISCA6Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA6Box.Location = new System.Drawing.Point(60, 116);
			this.VISCA6Box.Name = "VISCA6Box";
			this.VISCA6Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA6Box.TabIndex = 6;
			// 
			// VISCA7Box
			// 
			this.VISCA7Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA7Box.Location = new System.Drawing.Point(60, 136);
			this.VISCA7Box.Name = "VISCA7Box";
			this.VISCA7Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA7Box.TabIndex = 7;
			// 
			// VISCA8Box
			// 
			this.VISCA8Box.Font = new System.Drawing.Font("新細明體", 7.8F);
			this.VISCA8Box.Location = new System.Drawing.Point(60, 156);
			this.VISCA8Box.Name = "VISCA8Box";
			this.VISCA8Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA8Box.TabIndex = 8;
			// 
			// LabelFurbo3PanSpd
			// 
			this.LabelFurbo3PanSpd.Location = new System.Drawing.Point(0, 0);
			this.LabelFurbo3PanSpd.Name = "LabelFurbo3PanSpd";
			this.LabelFurbo3PanSpd.Size = new System.Drawing.Size(100, 23);
			this.LabelFurbo3PanSpd.TabIndex = 0;
			// 
			// TextBoxFurbo3PanSpd
			// 
			this.TextBoxFurbo3PanSpd.Location = new System.Drawing.Point(0, 0);
			this.TextBoxFurbo3PanSpd.Name = "TextBoxFurbo3PanSpd";
			this.TextBoxFurbo3PanSpd.Size = new System.Drawing.Size(100, 22);
			this.TextBoxFurbo3PanSpd.TabIndex = 0;
			// 
			// LabelFurbo3PanTorque
			// 
			this.LabelFurbo3PanTorque.Location = new System.Drawing.Point(0, 0);
			this.LabelFurbo3PanTorque.Name = "LabelFurbo3PanTorque";
			this.LabelFurbo3PanTorque.Size = new System.Drawing.Size(100, 23);
			this.LabelFurbo3PanTorque.TabIndex = 0;
			// 
			// ComboBoxFurbo3PanTorque
			// 
			this.ComboBoxFurbo3PanTorque.Location = new System.Drawing.Point(0, 0);
			this.ComboBoxFurbo3PanTorque.Name = "ComboBoxFurbo3PanTorque";
			this.ComboBoxFurbo3PanTorque.Size = new System.Drawing.Size(121, 20);
			this.ComboBoxFurbo3PanTorque.TabIndex = 0;
			// 
			// ButtonFurbo3SetPanSpd
			// 
			this.ButtonFurbo3SetPanSpd.Location = new System.Drawing.Point(0, 0);
			this.ButtonFurbo3SetPanSpd.Name = "ButtonFurbo3SetPanSpd";
			this.ButtonFurbo3SetPanSpd.Size = new System.Drawing.Size(75, 23);
			this.ButtonFurbo3SetPanSpd.TabIndex = 0;
			// 
			// LabelFurbo3TossingSpd
			// 
			this.LabelFurbo3TossingSpd.Location = new System.Drawing.Point(0, 0);
			this.LabelFurbo3TossingSpd.Name = "LabelFurbo3TossingSpd";
			this.LabelFurbo3TossingSpd.Size = new System.Drawing.Size(100, 23);
			this.LabelFurbo3TossingSpd.TabIndex = 0;
			// 
			// TextBoxFurbo3TossingSpd
			// 
			this.TextBoxFurbo3TossingSpd.Location = new System.Drawing.Point(0, 0);
			this.TextBoxFurbo3TossingSpd.Name = "TextBoxFurbo3TossingSpd";
			this.TextBoxFurbo3TossingSpd.Size = new System.Drawing.Size(100, 22);
			this.TextBoxFurbo3TossingSpd.TabIndex = 0;
			// 
			// ButtonFurbo3SetTossingSpd
			// 
			this.ButtonFurbo3SetTossingSpd.Location = new System.Drawing.Point(0, 0);
			this.ButtonFurbo3SetTossingSpd.Name = "ButtonFurbo3SetTossingSpd";
			this.ButtonFurbo3SetTossingSpd.Size = new System.Drawing.Size(75, 23);
			this.ButtonFurbo3SetTossingSpd.TabIndex = 0;
			// 
			// LabelFurbo3FeedingSpd
			// 
			this.LabelFurbo3FeedingSpd.Location = new System.Drawing.Point(0, 0);
			this.LabelFurbo3FeedingSpd.Name = "LabelFurbo3FeedingSpd";
			this.LabelFurbo3FeedingSpd.Size = new System.Drawing.Size(100, 23);
			this.LabelFurbo3FeedingSpd.TabIndex = 0;
			// 
			// TextBoxFurbo3FeedingSpd
			// 
			this.TextBoxFurbo3FeedingSpd.Location = new System.Drawing.Point(0, 0);
			this.TextBoxFurbo3FeedingSpd.Name = "TextBoxFurbo3FeedingSpd";
			this.TextBoxFurbo3FeedingSpd.Size = new System.Drawing.Size(100, 22);
			this.TextBoxFurbo3FeedingSpd.TabIndex = 0;
			// 
			// LabelFurbo3Torque
			// 
			this.LabelFurbo3Torque.Location = new System.Drawing.Point(0, 0);
			this.LabelFurbo3Torque.Name = "LabelFurbo3Torque";
			this.LabelFurbo3Torque.Size = new System.Drawing.Size(100, 23);
			this.LabelFurbo3Torque.TabIndex = 0;
			// 
			// ComboBoxFurbo3FeedingTorque
			// 
			this.ComboBoxFurbo3FeedingTorque.Location = new System.Drawing.Point(0, 0);
			this.ComboBoxFurbo3FeedingTorque.Name = "ComboBoxFurbo3FeedingTorque";
			this.ComboBoxFurbo3FeedingTorque.Size = new System.Drawing.Size(121, 20);
			this.ComboBoxFurbo3FeedingTorque.TabIndex = 0;
			// 
			// ButtonFurbo3SetFeedingSpd
			// 
			this.ButtonFurbo3SetFeedingSpd.Location = new System.Drawing.Point(0, 0);
			this.ButtonFurbo3SetFeedingSpd.Name = "ButtonFurbo3SetFeedingSpd";
			this.ButtonFurbo3SetFeedingSpd.Size = new System.Drawing.Size(75, 23);
			this.ButtonFurbo3SetFeedingSpd.TabIndex = 0;
			// 
			// LabelFWVersion
			// 
			this.LabelFWVersion.Location = new System.Drawing.Point(6, 17);
			this.LabelFWVersion.Name = "LabelFWVersion";
			this.LabelFWVersion.Size = new System.Drawing.Size(188, 22);
			this.LabelFWVersion.TabIndex = 67;
			this.LabelFWVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox14
			// 
			this.groupBox14.Controls.Add(this.LabelFWVersion);
			this.groupBox14.Location = new System.Drawing.Point(808, 9);
			this.groupBox14.Name = "groupBox14";
			this.groupBox14.Size = new System.Drawing.Size(200, 49);
			this.groupBox14.TabIndex = 68;
			this.groupBox14.TabStop = false;
			this.groupBox14.Text = "FW Version";
			// 
			// TimerUpdateSpeed
			// 
			this.TimerUpdateSpeed.Tick += new System.EventHandler(this.TimerUpdateSpeed_Tick);
			// 
			// Terminal
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(1044, 659);
			this.Controls.Add(this.groupBox14);
			this.Controls.Add(this.groupBox83);
			this.Controls.Add(this.tabVisca);
			this.Controls.Add(this.CmdClear);
			this.Controls.Add(this.TxData);
			this.Controls.Add(this.RxData);
			this.Controls.Add(this.RecvRate);
			this.Controls.Add(this.ChkUpdate);
			this.Controls.Add(this.ChkTxDec);
			this.Controls.Add(this.LblTxData);
			this.Controls.Add(this.ChkRxDec);
			this.Controls.Add(this.ChkRxEcho);
			this.Controls.Add(this.LblRxData);
			this.Controls.Add(this.LblReadRate);
			this.Controls.Add(this.CmdOpen);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Terminal";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "LensTester_20210512-01";
			this.Closed += new System.EventHandler(this.TermForm_Closed);
			this.groupBox83.ResumeLayout(false);
			this.groupBox83.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			this.tabVisca.ResumeLayout(false);
			this.tabMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ChartSpeed)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.tabTest.ResumeLayout(false);
			this.VISCABox.ResumeLayout(false);
			this.VISCABox.PerformLayout();
			this.groupBox14.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Methods
		/// <summary>
		/// Converts between ASCII and hex display.
		/// </summary>
		private void ChkTxHex_CheckedChanged(object sender, System.EventArgs e)
		{
			this.TxData.Text = (this.ChkTxDec.Checked) ?
				this.TxString : AtoX(this.TxString);
		}

		/// <summary>
		/// Converts between ASCII and hex display.
		/// </summary>
		private void ChkRxHex_CheckedChanged(object sender, System.EventArgs e)
		{
			this.RxData.Text = (this.ChkRxDec.Checked) ?
				this.RxString : AtoX(this.RxString);
		}

		/// <summary>
		/// Updates the TX/RX data text boxes if continuous is off.
		/// </summary>
		private void ChkUpdate_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.ChkUpdate.Checked == false)
			{
				this.TxDataUpdate("");
				this.RxDataUpdate("");
			}
		}

		/// <summary>
		/// Open/close the com port.
		/// </summary>
		[Obsolete]
		private void CmdOpen_Click(object sender, System.EventArgs e)
		{
			this.PortControl();
		}

		/// <summary>
		/// Clear display fields.
		/// </summary>
		private void CmdClear_Click(object sender, System.EventArgs e)
		{
			TxBytes = 0;
			this.TxString = "";
			this.TxData.Text = "";

			RxBytes = 0;
			this.RxString = "";
			this.RxData.Text = "";
		}

		/// <summary>
		/// Open the port settings dialog.
		/// </summary>
		private void CmdConfig_Click(object sender, System.EventArgs e)
		{
			Settings frm = new Settings(this.Port.ConfigFileName, this.Port.Cnfg);
			frm.ShowDialog();
		}

		/// <summary>
		/// Quit the terminal app.
		/// </summary>
		private void CmdQuit_Click(object sender, System.EventArgs e)
		{
			this.Port.Close();
			this.Port = null;
			Application.Exit();
		}

		/// <summary>
		/// Quit terminal app.
		/// </summary>
		private void TermForm_Closed(object sender, System.EventArgs e)
		{
			this.CmdQuit_Click(sender, e);
		}

		/// <summary>
		/// Changes RX data display rate on CR.
		/// </summary>
		private void RecvRate_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyValue == 13)
			{
				try
				{
					this.RecvTimer.Interval = Convert.ToInt32(this.RecvRate.Text, 10);
				}
				catch
				{
					this.RecvTimer.Interval = 100;
				}
			}
		}

		/// <summary>
		/// Fill the com port selector with a list of available ports.
		/// </summary>
		private void FillAvailable()
		{
			bool bPortAvailable = false;
			string s;
			SerialPort p = new SerialPort(this.Func);
			for (int i = 1; i <= MAX_PORTS; i++)
			{
				s = "COM" + i.ToString() + ":";
				if (p.Available(s))
				{
					bPortAvailable = true;
					this.ComPort.Items.Add(s);
				}
			}

			if (bPortAvailable == true)
				this.ComPort.SelectedIndex = 0;
			return;
		}

		/// <summary>
		/// Converts an ASCII string to hex formatted lines.
		/// </summary>
		private string AtoX(string asc)
		{
			int nLines;
			int nChars;
			int offset;
			string hex = "";

			//--- Compute number of hex lines.
			if ((asc.Length % 40) > 0)
				nLines = asc.Length / 40 + 1;
			else
				nLines = asc.Length / 40;

			//--- Convert into hex lines.
			for (int i = 0; i < nLines; i++)
			{
				offset = i * 40;
				if ((asc.Length - offset) > 40)
					nChars = 40;
				else
					nChars = asc.Length - offset;

				hex += this.HexLine(asc.Substring(offset, nChars)) + "\r\n";//"\r\n";
			}
			return hex;
		}

		/// <summary>
		/// Converts a 32 byte ASCII string into one hex formatted line.
		/// </summary>
		private string HexLine(string asc)
		{
			string hex = "";

			//--- Copy line to char buffer.
			char[] c = new char[64];
			asc.CopyTo(0, c, 0, asc.Length);

			//--- Convert chars to hex representation.
			for (int i = 0; i < asc.Length; i++)
			{
				if ((i != 0) && ((i % 2) == 0))
					hex += "-";

				hex += String.Format("{0:X1}", (int)c[i]);
			}

			//--- Add padding.
			hex += " ";

			return hex;     // Return hex dump line.
		}

		/// <summary>
		/// Updates the TX data info.
		/// </summary>
		private void TxDataUpdate(string s)
		{
			TxBytes += s.Length;

			if (TxBytes >= 20000)
			{
				TxBytes = 0;
				this.TxString = "";
				this.TxData.Text = "";
			}

			if (this.ChkUpdate.Checked == false)
			{
				TxString += s;

				if (this.ChkTxDec.Checked)
					this.TxData.AppendText(s);
				//this.TxData.AppendText( this.AtoX(s) );
				else
					this.TxData.AppendText(this.AtoX(s));
			}
			return;
		}

		/// <summary>
		/// Updates the RX data info.
		/// </summary>
		private void RxDataUpdate(string s)
		{
			RxBytes += s.Length;

			if (RxBytes >= 20000)
			{
				RxBytes = 0;
				this.RxString = "";
				this.RxData.Text = "";
			}

			if (this.ChkUpdate.Checked == false)
			{
				this.RxString += s;

				if (this.ChkRxDec.Checked)
					this.RxData.AppendText(s);
				else
					this.RxData.AppendText(this.AtoX(s));
			}
			return;
		}

		/// <summary>
		/// Gets the current port index.
		/// </summary>
		private int PortIndex
		{
			get
			{
				if (this.Port == null)
					return -1;
				string s = (string)this.ComPort.SelectedItem;
				return Convert.ToInt32(s.Substring(3, s.Length - 4), 10);
			}
		}

		/// <summary>
		/// Controls opening/closing the port.
		/// </summary>
		[Obsolete]
		private void PortControl()
		{
			if (this.Port.IsOpen == false)
			{
				if (this.Port.Open(this.PortIndex) == false)
				{
				}
				else
				{
					this.CmdOpen.Text = "OffLine";
					this.Settings.Text = PortSettings;
					this.RecvTimer.Enabled = true;
					this.ComPort.Enabled = false;

					GetVersion();
				}
			}
			else
			{
				if (this.Port.IsOpen)
				{
					this.Port.Close();
				}
				this.Settings.Text = "";
				this.CmdOpen.Text = "OnLine";
				this.RecvTimer.Enabled = false;
				this.TimerUpdateSpeed.Enabled = false;
				this.ComPort.Enabled = true;
				this.Port.Signals();
			}
			return;
		}

		/// <summary>
		/// Transmit a buffer.
		/// </summary>
		private uint SendBuf(byte[] b)
		{
			uint nSent = 0;

			byte[] buf;
			byte hiPart;
			byte loPart;
			int i, len;
			string newLine = "";

			if (b.Length > 0)
			{
				// Send this packet of data.
				nSent = this.Port.Send(b);

				//------
				string strTest = Encoding.ASCII.GetString(b);
				len = strTest.Length;
				buf = new Byte[len * 2];

				for (i = 0; i < len; i++)
				{
					hiPart = b[i];
					hiPart = (byte)(hiPart >> 4);
					loPart = b[i];
					loPart = (byte)(loPart & 0x0f);

					buf[i * 2] = hiPart;
					buf[i * 2 + 1] = loPart;

					newLine += hiPart;
					newLine += loPart;
				}

				string strBuf = Encoding.ASCII.GetString(buf);

				this.TxDataUpdate(strBuf);      //--- shiang
			}

			return nSent;
		}

		/// <summary>
		/// S_pqTo0p0q
		/// </summary>
		private byte AsciiToHex(byte Value)
		{
			byte rByte = 0;

			if ((Value >= 0x30) && (Value <= 0x39))
				rByte = (byte)(Value - 0x30);
			else if ((Value >= 0x41) && (Value <= 0x5A))
				rByte = (byte)(Value - 0x37);
			else if ((Value >= 0x61) && (Value <= 0x7A))
				rByte = (byte)(Value - 0x57);

			return rByte;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the current port settings.
		/// </summary>
		private string PortSettings
		{
			get
			{
				string temp = this.Port.Cnfg.BaudRate.ToString().Substring(5);
				temp = temp + ":" + this.Port.Cnfg.Parity.ToString().Substring(0, 1);
				temp = temp + ":" + ((int)this.Port.Cnfg.DataBits).ToString();
				if ((int)this.Port.Cnfg.StopBits == 0)
					temp += ":1";
				else if ((int)this.Port.Cnfg.StopBits == 1)
					temp += ":1.5";
				else if ((int)this.Port.Cnfg.StopBits == 2)
					temp += ":2";
				return temp;
			}
		}
		#endregion

		#region Hooks
		/// <summary>
		/// Handles error events.
		/// </summary>
		[Obsolete]
		internal void OnError(string fault)
		{
			this.PortControl();
		}

		/// <summary>
		/// Immediate byte received.
		/// </summary>
		internal void OnRecvI(byte[] b)
		{
			/*
			this.RxDataUpdate(Encoding.UTF8.GetString(b));      //--- shiang

			if (this.ChkRxEcho.Checked)
				this.SendBuf(b);
			*/

			/*
			int i;

            // Check if the counter is overflow.
            if (countRxPacket > 15)
                countRxPacket = 0;

            // Get the received data.
            RxPacket[countRxPacket++] = b[0];

            // Check if a packet is finished or not.
            if (b[0] == 0xFF)
            {
                // Save the packet length.
                lengthRxPacket = countRxPacket;

                // Clear the rx count.
                countRxPacket = 0;

                // Set the rx packet flag.
                FlagRxPacket = true;

                // Save the data to local buffer.
                for (i = 0; i < lengthRxPacket; i++)
                    localRxPacket[i] = RxPacket[i];
            }
			*/
		}
		#endregion

		#region Version
		private void GetVersion()
		{
			byte[] b = { 0x81, 0x09, 0x00, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.SYS_ReadVersion;
			this.SendBuf(b);
		}
		#endregion

		#region Function
		private void UpdateRxData()
		{
			byte i, hiPart, loPart;
			byte[] buf;
			string strBuf;

			HandleRxPacket();

			//--------------------------------------------------------
			//	Receive ASCII Input
			//--------------------------------------------------------
			if (this.ChkRxDec.Checked)
			{
				strBuf = "";

				for (i = 0; i < lengthRxPacket; i++)
				{
					if (localRxPacket[i] == 0x0d)
					{
						this.RxDataUpdate(strBuf);
						strBuf = "";

						this.RxData.Text += "\r\n";//"\r\n";
						continue;
					}

					strBuf += localRxPacket[i];
				}

				this.RxDataUpdate(strBuf);
			}
			//--------------------------------------------------------
			//	Receive HEX Input
			//--------------------------------------------------------
			else
			{
				buf = new Byte[lengthRxPacket * 2];

				for (i = 0; i < lengthRxPacket; i++)
				{
					hiPart = localRxPacket[i];
					hiPart = (byte)(hiPart >> 4);
					loPart = localRxPacket[i];
					loPart = (byte)(loPart & 0x0f);

					buf[i * 2] = hiPart;
					buf[i * 2 + 1] = loPart;
				}

				strBuf = Encoding.ASCII.GetString(buf);
				this.RxDataUpdate(strBuf);
			}
		}

		private void UpdateSpeedChart()
		{
			ChartSpeed.Series[0].Points.Clear();

			ChartSpeed.ChartAreas[0].AxisX.Minimum = SpeedTimeArray[0];
			ChartSpeed.ChartAreas[0].AxisX.Maximum = SpeedTimeArray[SpeedTimeArray.Length - 1];

			for (int i = 0; i < SpeedArray.Length - 1; ++i)
				ChartSpeed.Series[0].Points.AddXY(SpeedTimeArray[i], SpeedArray[i]);
		}

		private void HandleRxPacket()
		{
			int i = 0, iStartByte;

			//--- find the first non-zero byte
			while (i < lengthRxPacket)
			{
				if (localRxPacket[i] == 0)
					i++;
				else
					break;
			}

			//--- eliminate non-zero bytes
			if (i != 0)
			{
				iStartByte = i;

				for (i = iStartByte; i < lengthRxPacket; i++)
					localRxPacket[i - iStartByte] = localRxPacket[i];
			}

			//=============================================================================
			// Handle the received data.
			//=============================================================================
			if ((localRxPacket[1] == 0x50) ||
				(localRxPacket[1] == 0x53))
			{
				switch (G_eGeneralCommand)
				{
					case GeneralCommand.SYS_ReadVersion:
						this.LabelFWVersion.Text = (localRxPacket[4] + 2000).ToString() + "/";
						this.LabelFWVersion.Text += localRxPacket[5].ToString() + "/";
						this.LabelFWVersion.Text += localRxPacket[6].ToString() + "-";
						this.LabelFWVersion.Text += localRxPacket[7].ToString();
						break;
				}
			}

			//G_eGeneralCommand = GeneralCommand.SYS_NONE;
		}
		#endregion

		#region TimerTick
		/// <summary>
		/// Pulls data from the driver and updates the RX data text box.
		/// </summary>
		private void RecvTimer_Tick(object sender, System.EventArgs e)
		{
			int i, len;

			uint nBytes = this.Port.Recv(out byte[] b);

			if (nBytes == 0)
				return;

			for (i = 0; i < nBytes; i++)
			{
				RxPacket[countRxPacket++] = b[i];

				if (b[i] == 0xFF || countRxPacket == 16)
				{
					//--- save rx packet length
					lengthRxPacket = countRxPacket;

					//--- clear rx count
					countRxPacket = 0;

					//--- save rx data to local buffer
					for (len = 0; len < lengthRxPacket; len++)
						localRxPacket[len] = RxPacket[len];

					//--- update rx data to text box
					UpdateRxData();
				}
			}
		}

		private void TimerUpdateSpeed_Tick(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x06, 0x03, 0xFF };

			//G_eGeneralCommand = GeneralCommand.MD_ReadSpeed;
			this.SendBuf(b);
		}
		#endregion

		#region TestTab
		private void VISCA1button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA1Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA1Box.Text[i];
				tempByte1 = (byte)VISCA1Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA2button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA2Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA2Box.Text[i];
				tempByte1 = (byte)VISCA2Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA3button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA3Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA3Box.Text[i];
				tempByte1 = (byte)VISCA3Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA4button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA4Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA4Box.Text[i];
				tempByte1 = (byte)VISCA4Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA5button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA5Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA5Box.Text[i];
				tempByte1 = (byte)VISCA5Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA6button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA6Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA6Box.Text[i];
				tempByte1 = (byte)VISCA6Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA7button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA7Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA7Box.Text[i];
				tempByte1 = (byte)VISCA7Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}

		private void VISCA8button_Click(object sender, EventArgs e)
		{
			byte len;
			byte tempByte, tempByte1;
			len = (byte)(VISCA8Box.Text.Length);
			byte[] buffHex = new byte[len / 2];
			byte[] rBuffer = new byte[2];

			for (int i = 0; i < (len - 1); i += 2)
			{
				tempByte = (byte)VISCA8Box.Text[i];
				tempByte1 = (byte)VISCA8Box.Text[i + 1];
				rBuffer[0] = AsciiToHex(tempByte);
				rBuffer[1] = AsciiToHex(tempByte1);
				buffHex[i / 2] = (byte)(rBuffer[1] | (byte)rBuffer[0] << 4);

			}
			this.SendBuf(buffHex);
		}
		#endregion
		
		#region Motor
		
		private void ButtonShowSpeed_Click(object sender, EventArgs e)
		{
			TimerUpdateSpeed.Enabled = true;
		}

		private void ButtonStopShowSpeed_Click(object sender, EventArgs e)
		{
			TimerUpdateSpeed.Enabled = false;
		}		
		#endregion

		/// <summary>
		/// Used to launch a stand-alone terminal app.
		/// </summary>
		public class TerminalMain
		{
			/// <summary>
			/// The main entry point for the application.
			/// </summary>
			[STAThread]
			[Obsolete]
			static void Main()
			{
				Application.Run(new Terminal());
				return;
			}
		}
	}
}
