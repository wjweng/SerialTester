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

		private System.Windows.Forms.Button CmdOpen;
		private System.Windows.Forms.Button CmdConfig;
		private System.Windows.Forms.Button CmdClear;

		internal System.Windows.Forms.TextBox TxData;
		internal System.Windows.Forms.TextBox RxData;
		internal System.Windows.Forms.TextBox Settings;

		private System.Windows.Forms.Timer RecvTimer;
		private System.Windows.Forms.ComboBox ComPort;
		private System.Windows.Forms.GroupBox GroupBoxSettings;
		private System.Windows.Forms.DataVisualization.Charting.Chart chartAngle;

		private System.ComponentModel.IContainer components;

		private SerialPort Port;
		private TabControl tabVisca;
		private TabPage tabMenu;
		private TabPage tabAK7452;
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
		private GroupBox groupBoxAK7452NormalMode;
		private TextBox textBoxAngleData;
		private Button ButtonSetNormalMode;
		private Button ButtonAngleData;
		private TextBox textBoxErrorPin;
		private Button ButtonErrorPin;
		private Button ButtonShowAngle;
		private Button ButtonStopShowAngle;
		private System.Windows.Forms.Timer TimerUpdateAngle;
		private GroupBox groupBoxAK7452ManualMode;
		private TextBox TextBoxErrorMonitor;
		private Button ButtonErrorMonitor;
		private TextBox TextBoxASMode;
		private Button ButtonASMode;
		private TextBox TextBoxMagFlux;
		private Button ButtonMagFlux;
		private TextBox TextBoxAngleDataReg;
		private Button ButtonSetMaualMode;
		private Button ButtonAngleDataReg;
		private TextBox TextBoxZP;
		private Button ButtonSetZP;
		private Button ButtonGetZP;
		private GroupBox groupBoxAK7452RDABZ;
		private GroupBox groupBox19;
		private Button ButtonSetSDDIS;
		private Button ButtonGetSDDIS;
		private GroupBox groupBoxAK7452ABZResolution;
		private Button ButtonSetABZRes;
		private Button ButtonGetABZRes;
		private TextBox TextBoxABZRes;
		private TextBox TextBoxMemLock;
		private Button ButtonMemLock;
		private GroupBox groupBoxAK7452ZeroDegreePoint;
		private Button ButtonGetRDABZ;
		private Button ButtonSetRDABZ;
		private Label labelAK7452ABZHysteresis;
		private Label labelAK7452ABZEnable;
		private Label labelAK7452ZWidth;
		private Label labelAK7452RotationDir;
		private Label LabelFWVersion;
		private GroupBox groupBoxVersion;
		private ComboBox ComboBoxSDDIS;

		// Terminal functions that handle base class events.
		private WithEvents Func;

		// Terminal app constants.
		private const int MAX_PORTS = 32;
		private const int DEFAULT_SPEED_LEVEL = 100;

		// Terminal app locals.
		private int TxBytes = 0;
		private int RxBytes = 0;
		private byte countRxPacket = 0;
		private byte lengthRxPacket;

		private readonly byte[] RxPacket = new byte[16];
		private readonly byte[] localRxPacket = new byte[16];

		private GeneralCommand G_eGeneralCommand;
		private readonly ushort[] AngleArray = new ushort[200];
		private readonly double[] TimeArray = new double[200];
		private readonly ushort[] SpeedArray = new ushort[200];
		private readonly double[] SpeedTimeArray = new double[200];

		private uint TimerUpdateAngleCnt = 0;
		private uint TimerUpdateSpeedCnt = 0;

		private ComboBox ComboBoxRD;
		private ComboBox ComboBoxABZHysteresis;
		private ComboBox ComboBoxABZEnable;
		private ComboBox ComboBoxZWidth;
		private CheckBox CheckBoxSaveToEEPROM;
		private GroupBox groupBox1;
		private Button ButtonPanStop;
		private Button ButtonPanRight;
		private Button ButtonPanLeft;
		private GroupBox groupBox2;
		private Button ButtonGetAcceleration;
		private TextBox TextBoxAcceleration;
		private Button ButtonSetAcceleration;
		private GroupBox groupBox3;
		private TextBox TextBoxTargetSpeed;
		private Button ButtonGetCurrentSpeed;
		private TextBox TextBoxCurrentSpeed;
		private Button ButtonSetTargetSpeed;
		private GroupBox groupBox5;
		private Button ButtonTiltUp;
		private Button ButtonTiltDown;
		private GroupBox groupBox4;
		private Button ButtonStopShowSpeed;
		private Button ButtonShowSpeed;
		private Chart ChartSpeed;
		private CheckBox CheckBoxMoveStop;
		private GroupBox groupBoxMCU;
		private Label LabelMCUType;
		private TextBox TextBoxABSPos;
		private Button ButtonABS;
		private ComboBox ComboBoxPanMethod;
		private GroupBox groupBox8;
		private GroupBox groupBox9;
		private TextBox TextBoxMotorPosition;
		private Button ButtonGetPosition;
		private Label label6;
		private TextBox TextBoxSpeedLevel;
		private GroupBox groupBox10;
		private Label label11;
		private TextBox TextBoxSpeedInPPS;
		private GroupBox groupBox15;
		private TextBox TextBoxMotorAngle;
		private Button ButtonGetAngle;
		private TextBox textBoxZCount;
		private Button ButtonZCount;
		private TextBox textBoxABCount;
		private Button ButtonABCount;
		private TextBox TextBoxABS2Pos;
		private Button ButtonABS2;
		private ComboBox ComboBoxAccLevel;
		private Button ButtonGetAccLevel;
		private Button ButtonSetAccLevel;
		private TextBox TextBoxStopAt;
		private Button ButtonStopAt;
		private Button ButtonDataRenewal;
		private GroupBox groupBox16;
		private Button ButtonGetSpeedByZoomRatio;
		private TextBox TextBoxSpeedByZoomRatio;
		private Button ButtonSpeedByZoomOn;
		private Button ButtonSpeedByZoomOff;
		private GroupBox groupBox17;
		private Button ButtonRelDown;
		private Button ButtonRelUp;
		private TextBox TextBoxRelStep;
		private Button ButtonRelLeft;
		private Button ButtonRelStop;
		private Button ButtonRelRight;
		private Label label12;
		private GroupBox groupBox20;
		private Button ButtonImageFlipMaxAngleOn;
		private Button ButtonImageFlipMaxAngleOff;
		private GroupBox groupBox6;
		private TextBox TextBoxZPCalibration;
		private Button ButtonZPCaliStatus;
		private Button ButtonClearZPCali;
		private Button ButtonHome;
		private Button ButtonCalibration;
		private Button ButtonABSAngleStop;
		private TextBox TextBoxABSAngle2;
		private Button ButtonABSAngle2;
		private TextBox TextBoxABSAngle;
		private Button ButtonABSAngle;
		private TabPage tabLightSensor;
		private GroupBox groupBoxLSResult;
		private Label LabelLSR;
		private TextBox TextBoxLSR;
		private Label LabelLSE;
		private TextBox TextBoxLSE;
		private TextBox TextBoxLSLux;
		private Button ButtonLSGetLux;
		private GroupBox groupBoxLSConfiguration;
		private TextBox TextBoxLSFC;
		private Button ButtonLSGetCfg;
		private Label LabelLSLux;
		private Button ButtonSetCfg;
		private Label LabelLSME;
		private TextBox TextBoxLSME;
		private Label LabelLSFC;
		private Label LabelLSOVF;
		private Label LabelLSCRF;
		private TextBox TextBoxLSOVF;
		private TextBox TextBoxLSCRF;
		private Label LabelLSFH;
		private Label LabelLSFL;
		private TextBox TextBoxLSFH;
		private Label LabelLSRN;
		private TextBox TextBoxLSFL;
		private Label LabelLSCT;
		private TextBox TextBoxLSRN;
		private TextBox TextBoxLSCT;
		private Label LabelLSM;
		private TextBox TextBoxLSM;
		private Label LabelLSPOL;
		private TextBox TextBoxLSPOL;
		private Label LabelLSL;
		private TextBox TextBoxLSL;
		private GroupBox groupBoxLSLimit;
		private Label LabelLSLuxLimit;
		private CheckBox CheckBoxLSHighLimit;
		private Label LabelLSRLimit;
		private CheckBox CheckBoxLSLowLimit;
		private TextBox TextBoxLSRLimit;
		private Button ButtonSetLSLimit;
		private Label LabelLSELimit;
		private Button ButtonGetLSLimit;
		private TextBox TextBoxLSELimit;
		private TextBox TextBoxLSLuxLimit;
		private GroupBox groupBoxLSID;
		private TextBox TextBoxLSDID;
		private TextBox TextBoxLSID;
		private Button ButtonGetLSID;
		private Button ButtonLSDID;
		private TabPage tabAlarm;
		private GroupBox groupBoxAlarm;
		private Button ButtonAlarmOn;
		private Button ButtonAlarmOff;
		private GroupBox groupBoxAlarmInq;
		private TextBox TextBoxAlarmDI1;
		private Label LabelAlarmDI1;
		private TextBox TextBoxAlarmDI0;
		private Label LabelAlarmDI0;
		private Button ButtonAlarmIn;
		private TextBox TextBoxAlarmTrigLvl;
		private TextBox TextBoxAlarmStatus;
		private Button ButtonAlarmAutoStatus;
		private Button ButtonAlarmTrigLvl;
		private GroupBox groupBoxAlarmOut;
		private Button ButtonAlarmOutHigh;
		private Button ButtonAlarmOutLow;
		private GroupBox groupBoxAlarmAuto;
		private Label LabelAlarmTrigLvl;
		private ComboBox ComboBoxAlarmTrigLvl;
		private GroupBox groupBoxRS485;
		private GroupBox groupBoxRS485Test;
		private Label LabelRS485TestRx;
		private TextBox TextBoxRS485TestRx;
		private Label LabelRS485TestTX;
		private Button ButtonRS485TestTx;
		private Button ButtonRS485TestRx;
		private GroupBox groupBoxRS485TermR;
		private Button ButtonRS485GetTermR;
		private GroupBox groupBoxRS485TransceiverMode;
		private Button ButtonRS485GetMode;
		private GroupBox groupBoxNTC;
		private TextBox TextBoxNTC2;
		private Button ButtonGetNTC2;
		private TextBox TextBoxNTC1;
		private Button ButtonNTCSingleScan;
		private Button ButtonGetNTC1;
		private GroupBox groupBoxRS485Comm;
		private ComboBox ComboBoxRS485BaudRate;
		private Label LabelRS485BaudRate;
		private Button ButtonRS485GetComm;
		private ComboBox ComboBoxRS485StopBits;
		private Label LabelRS485StopBits;
		private ComboBox ComboBoxRS485TermR;
		private ComboBox ComboBoxRS485Mode;
		private GroupBox groupBoxRS485DI;
		private TextBox TextBoxRS485DI8;
		private TextBox TextBoxRS485DI7;
		private TextBox TextBoxRS485DI6;
		private TextBox TextBoxRS485DI5;
		private TextBox TextBoxRS485DI4;
		private TextBox TextBoxRS485DI3;
		private TextBox TextBoxRS485DI2;
		private TextBox TextBoxRS485DI1;
		private Label LabelRS485DI8;
		private Label LabelRS485DI7;
		private Label LabelRS485DI6;
		private Label LabelRS485DI5;
		private Label LabelRS485DI4;
		private Label LabelRS485DI3;
		private Label LabelRS485DI2;
		private Button ButtonRS485DI;
		private Label LabelRS485DI1;
		private GroupBox groupBoxRS485DO;
		private Label LabelRS485DO8;
		private Label LabelRS485DO7;
		private Label LabelRS485DO6;
		private Label LabelRS485DO5;
		private Label LabelRS485DO4;
		private Label LabelRS485DO3;
		private Label LabelRS485DO2;
		private Button ButtonRS485DO;
		private Label LabelRS485DO1;
		private CheckBox CheckBoxRS485DO1;
		private CheckBox CheckBoxRS485DO8;
		private CheckBox CheckBoxRS485DO7;
		private CheckBox CheckBoxRS485DO6;
		private CheckBox CheckBoxRS485DO5;
		private CheckBox CheckBoxRS485DO4;
		private CheckBox CheckBoxRS485DO3;
		private CheckBox CheckBoxRS485DO2;
		private GroupBox groupBoxSD700Set;
		private ComboBox ComboBoxSD700TrigLvl;
		private Label LabelSD700TrigLvl;
		private Label LabelSD700AutoScan;
		private ComboBox ComboBoxSD700AutoScan;
		private TextBox TextBoxSD700Addr;
		private Button ButtonSD700SetAddr;
		private TextBox TextBoxRS485TestTx;
		private ComboBox ComboBoxRS485Dev;
		private Button ButtonRS485GetDev;
		private Button ButtonRS485GetAddr;
		private Button ButtonPanType;
		private Button ButtonABSStop;
		private Button ButtonLockHome;
		private Button ButtonUnlockHome;
		private TextBox TextBoxLockStatus;
		private Button ButtonLockStatus;
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
		private TabPage tabTLE5012B;
		private GroupBox groupTleSTAT;
		private TextBox TextBoxTleStatError;
		private TextBox TextBoxTleStatRD_ST;
		private Button ButtonTleGetError;
		private TextBox TextBoxTleStatS_RST;
		private Button ButtonTleGetS_RST;
		private Button ButtonClearShowSpeed;
		private Button ButtonTleGetSlave;
		private Button ButtonTleGetRD_ST;
		private GroupBox GroupBoxTleACSTAT;
		private Button ButtonTleGetAS_VR;
		private Button ButtonTleGetAS_WD;
		private Button ButtonTleGetAS_RST;
		private ComboBox ComboBoxTleStatS_NR;
		private ComboBox ComboBoxTleAS_RST;
		private ComboBox ComboBoxTleAS_FUSE;
		private Button ButtonTleGetAcstatAS_FUSE;
		private ComboBox ComboBoxTleAS_VR;
		private ComboBox ComboBoxTleAS_WD;
		private ComboBox ComboBoxTleAS_FRST;
		private Button ButtonTleGetAS_FRST;
		private ComboBox ComboBoxTleAS_ADCT;
		private Button ButtonTleGetAS_ADCT;
		private ComboBox ComboBoxTleAS_VEC_MAG;
		private ComboBox ComboBoxTleAS_VEC_XY;
		private ComboBox ComboBoxTleAS_OV;
		private Button ButtonTleGetAS_VEC_MAG;
		private Button ButtonTleGetAS_VEC_XY;
		private Button ButtonTleGetAS_OV;
		private ComboBox ComboBoxTleAS_DSPU;
		private Button ButtonTleGetAS_DSPU;
		private GroupBox GroupBoxTleAVAL;
		private TextBox TextBoxTleRD_AV;
		private Button ButtonTleGetRD_AV;
		private TextBox TextBoxTleANG_VAL;
		private Button ButtonTleGetANG_VAL;
		private GroupBox GroupBoxTleASPD;
		private TextBox TextBoxTleRD_AS;
		private Button ButtonTleGetRD_AS;
		private TextBox TextBoxTleANG_SPD;
		private Button ButtonTleGetANG_SPD;
		private GroupBox GroupBoxTleAREV;
		private TextBox TextBoxTleRD_REV;
		private Button ButtonTleGetRD_REV;
		private TextBox TextBoxTleFCNT;
		private Button ButtonTleGetFCNT;
		private TextBox TextBoxTleREVOL;
		private Button ButtonTleGetRevol;
		private GroupBox GroupBoxTleFSYNC;
		private TextBox TextBoxTleFSYNC;
		private Button ButtonTleGetFSYNC;
		private TextBox TextBoxTleTEMPER;
		private Button ButtonTleGetTEMPER;
		private GroupBox GroupBoxTleMOD_1;
		private Button ButtonTleGetDSPU_HOLD;
		private Button ButtonTleGetIIF_MOD;
		private ComboBox ComboBoxTleIIF_MOD;
		private ComboBox ComboBoxTleFIR_MD;
		private Button ButtonTleGetFIR_MD;
		private ComboBox ComboBoxTleCLK_SEL;
		private Button ButtonTleGetCLK_SEL;
		private ComboBox ComboBoxTleDSPU_HOLD;
		private GroupBox GroupBoxTleSIL;
		private ComboBox ComboBoxTleFILT_PAR;
		private Button ButtonTleGetFILT_PAR;
		private ComboBox ComboBoxTleFILT_INV;
		private Button ButtonTleGetFILT_INV;
		private ComboBox ComboBoxTleFUSE_REL;
		private Button ButtonTleGetFUSE_REL;
		private ComboBox ComboBoxTleADCTV_EN;
		private Button ButtonTleGetADCTV_EN;
		private ComboBox ComboBoxTleADCTV_Y;
		private ComboBox ComboBoxTleADCTV_X;
		private Button ButtonTleGetADCTV_Y;
		private Button ButtonTleGetADCTV_X;
		private GroupBox GroupBoxTleMOD_2;
		private Button ButtonTleGetANG_RANGE;
		private ComboBox ComboBoxTleANG_DIR;
		private Button ButtonTleGetANG_DIR;
		private ComboBox ComboBoxTlePREDICT;
		private ComboBox ComboBoxTleAUTOCAL;
		private Button ButtonTleGetPREDICT;
		private Button ButtonTleGetAUTOCAL;
		private GroupBox GroupBoxTleMOD_3;
		private Button ButtonTleGetANG_BASE;
		private ComboBox ComboBoxTleSPIKEF;
		private Button ButtonTleGetSPIKEF;
		private ComboBox ComboBoxTleSSC_OD;
		private ComboBox ComboBoxTlePAD_DRV;
		private Button ButtonTleGetSSC_OD;
		private Button ButtonTleGetPAD_DRV;
		private TextBox TextBoxTleANG_RANGE;
		private Button ButtonTleSetANGLE_RANGE;
		private TextBox TextBoxTleANG_BASE;
		private Button ButtonTleSetANG_BASE;
		private GroupBox GroupBoxTleOFFX;
		private TextBox TextBoxTleOFFX;
		private Button ButtonTleGetOFFSET_X;
		private Button ButtonTleSetOFFSET_X;
		private GroupBox GroupBoxTleOFFSET_Y;
		private Button ButtonTleSetOFFSET_Y;
		private TextBox TextBoxTleOFFY;
		private Button ButtonTleGetOFFSET_Y;
		private GroupBox GroupBoxTleSYNCH;
		private Button ButtonTleSetSYNCH;
		private TextBox TextBoxTleSYNCH;
		private Button ButtonTleGetSYNCH;
		private GroupBox GroupBoxTleIFAB;
		private TextBox TextBoxTleORTHO;
		private Button ButtonTleSetORTHO;
		private Button ButtonTleGetORTHO;
		private ComboBox ComboBoxTleFIR_UDR;
		private Button ButtonTleGetFIR_UDR;
		private ComboBox ComboBoxTleIFAB_OD;
		private ComboBox ComboBoxTleIFAB_HYST;
		private Button ButtonTleGetIFAB_OD;
		private Button ButtonTleGetIFAB_HYST;
		private GroupBox GroupBoxTleMOD_4;
		private TextBox TextBoxTleTCO_X_T;
		private Button ButtonTleSetTCO_X_T;
		private Button ButtonTleGetTCO_X_T;
		private ComboBox ComboBoxTleHSM_PLP;
		private Button ButtonTleGetHSM_PLP;
		private ComboBox ComboBoxTleIFAB_RES;
		private ComboBox ComboBoxTleIF_MD;
		private Button ButtonTleGetIFAB_RES;
		private Button ButtonTleGetIF_MD;
		private GroupBox GroupBoxTleTCO_Y;
		private TextBox TextBoxTleCRC_PAR;
		private Button ButtonTleSetCRC_PAR;
		private TextBox TextBoxTleTCO_Y_T;
		private Button ButtonTleSetTCO_Y_T;
		private Button ButtonTleGetTCO_Y_T;
		private ComboBox ComboBoxTleSBIST;
		private Button ButtonTleGetSBIST;
		private Button ButtonTleGetCRC_PAR;
		private GroupBox GroupBoxTleT_RAW;
		private TextBox TextBoxTleT_RAW;
		private Button ButtonTleGetT_RAW;
		private GroupBox GroupBoxTleIIF_CNT;
		private TextBox TextBoxTleIIF_CNT;
		private Button ButtonTleGetIIF_CNT;
		private GroupBox GroupBoxTleT25O;
		private TextBox TextBoxTleT25O;
		private Button ButtonTleGetT25O;
		private GroupBox GroupBoxTleD_MAG;
		private TextBox TextBoxTleMAG;
		private Button ButtonTleGetMAG;
		private GroupBox GroupBoxTleADC_Y;
		private TextBox TextBoxTleADC_Y;
		private Button ButtonTleGetADC_Y;
		private GroupBox GroupBoxTleADC_X;
		private TextBox TextBoxTleADC_X;
		private Button ButtonTleGetADC_X;
		private TextBox TextBoxTleT_TGL;
		private Button ButtonTleGetT_TGL;
		private GroupBox groupBoxShowSpeed;
		private GroupBox groupBoxShowAngle;
		private Button ButtonClearShowAngle;
		private TabPage tabTMC2209;
		private GroupBox GroupBox_General;
		private GroupBox GroupBox_GCONF;
		private ComboBox ComboBox_I_scale_analog;
		private Button Button_I_scale_analog;
		private ComboBox ComboBox_test_mode;
		private Button Button_test_mode;
		private ComboBox ComboBox_multistep_filt;
		private Button Button_multistep_filt;
		private ComboBox ComboBox_mstep_reg_sel;
		private Button Button_mstep_reg_sel;
		private ComboBox ComboBox_pdn_disable;
		private Button Button_pdn_disable;
		private ComboBox ComboBox_index_step;
		private Button Button_index_step;
		private ComboBox ComboBox_index_otpw;
		private Button Button_index_otpw;
		private ComboBox ComboBox_shaft;
		private Button Button_shaft;
		private ComboBox ComboBox_en_SpreadCycle;
		private Button Button_en_SpreadCycle;
		private ComboBox ComboBox_internal_Rsense;
		private Button Button_internal_Rsense;
		private GroupBox GroupBoxGSTAT;
		private ComboBox ComboBox_uv_cp;
		private Button Button_uv_cp;
		private ComboBox ComboBox_drv_err;
		private Button Button_drv_err;
		private ComboBox ComboBox_reset;
		private Button Button_reset;
		private GroupBox GroupBox_IFCNT;
		private TextBox TextBox_IFCNT;
		private Button Button_IFCNT;
		private GroupBox GroupBox_SLAVECONF;
		private ComboBox ComboBox_SLAVECONF;
		private GroupBox GroupBox_OTP_READ;
		private TextBox TextBox_OTP_READ_0;
		private TextBox TextBox_OTP_READ_1;
		private TextBox TextBox_OTP_READ_2;
		private Button Button_OTP_READ;
		private GroupBox GroupBox_IOIN;
		private TextBox TextBox_VERSION;
		private TextBox TextBox_DIR;
		private TextBox TextBox_SPREAD_EN;
		private TextBox TextBox_STEP;
		private TextBox TextBox_PDN_UART;
		private TextBox TextBox_DIAG;
		private TextBox TextBox_MS2;
		private TextBox TextBox_MS1;
		private TextBox TextBox_ENN;
		private Button Button_VERSION;
		private Button Button_DIR;
		private Button Button_SPREAD_EN;
		private Button Button_STEP;
		private Button Button_PDN_UART;
		private Button Button_DIAG;
		private Button Button_MS2;
		private Button Button_MS1;
		private Button Button_ENN;
		private GroupBox GroupBox_FACTORY_CONF;
		private ComboBox ComboBox_OTTRIM;
		private Button Button_OTTRIM;
		private ComboBox ComboBox_FCLKTRIM;
		private Button Button_FCLKTRIM;
		private Button Button_SLAVECONF;
		private GroupBox GroupBox_Velocity_Dependent_Control;
		private GroupBox GroupBox_IHOLD_IRUN;
		private ComboBox ComboBox_IHOLDDELAY;
		private Button Button_IHOLDDELAY;
		private ComboBox ComboBox_IRUN;
		private Button Button_IRUN;
		private ComboBox ComboBox_IHOLD;
		private Button Button_IHOLD;
		private GroupBox GroupBox_TPOWERDOWN;
		private Button Button_TPOWERDOWN;
		private TextBox TextBox_TPOWERDOWN;
		private GroupBox GroupBox_TSTEP;
		private TextBox TextBox_TSTEP;
		private Button Button_TSTEP;
		private GroupBox GroupBox_VACTUAL;
		private TextBox TextBox_VACTUAL;
		private Button Button_VACTUAL;
		private GroupBox GroupBox_TPWMTHRS;
		private TextBox TextBox_TPWMTHRS;
		private Button Button_TPWMTHRS;
		private GroupBox GroupBox_Sequencer_Registers;
		private GroupBox GroupBox_MSCURACT;
		private TextBox TextBox_CUR_A;
		private Button Button_CUR_A;
		private GroupBox GroupBox_MSCNT;
		private TextBox TextBox_MSCNT;
		private Button Button_MSCNT;
		private GroupBox GroupBox_StallGuard_Control;
		private GroupBox GroupBox_SG_RESULT;
		private TextBox TextBox_SG_RESULT;
		private Button Button_SG_RESULT;
		private GroupBox GroupBox_SGTHRS;
		private Button Button_SGTHRS;
		private TextBox TextBox_SGTHRS;
		private GroupBox GroupBox_TCOOLTHRS;
		private TextBox TextBox_TCOOLTHRS;
		private Button Button_TCOOLTHRS;
		private TextBox TextBox_CUR_B;
		private Button Button_CUR_B;
		private GroupBox GroupBox_STALLGARD_COOLCONF;
		private GroupBox GroupBox_COOLCONF;
		private ComboBox ComboBox_seimin;
		private Button Button_seimin;
		private ComboBox ComboBox_sedn;
		private Button Button_sedn;
		private ComboBox ComboBox_semax;
		private Button Button_semax;
		private ComboBox ComboBox_seup;
		private Button Button_seup;
		private ComboBox ComboBox_semin;
		private Button Button_semin;
		private GroupBox GroupBox_Chopper_DRV_STATUS;
		private GroupBox GroupBox_DRV_STATUS;
		private TextBox TextBox_otpw;
		private Button Button_otpw;
		private GroupBox GroupBox_Chopper_CHOPCONF;
		private GroupBox GroupBox_CHOPCONF;
		private ComboBox ComboBox_diss2vs;
		private Button Button_diss2vs;
		private ComboBox ComboBox_diss2g;
		private Button Button_diss2g;
		private ComboBox ComboBox_dedge;
		private Button Button_dedge;
		private ComboBox ComboBox_intpol;
		private Button Button_intpol;
		private ComboBox ComboBox_MRES;
		private Button Button_MRES;
		private ComboBox ComboBox_vsense;
		private Button Button_vsense;
		private ComboBox ComboBox_TBL;
		private Button Button_TBL;
		private ComboBox ComboBox_HEND;
		private Button Button_HEND;
		private ComboBox ComboBox_HSTRT;
		private Button Button_HSTRT;
		private ComboBox ComboBox_TOFF;
		private Button Button_TOFF;
		private GroupBox GroupBox_Chopper_PWMCONF;
		private GroupBox GroupBox_PWMCONF;
		private ComboBox ComboBox_PWM_LIM;
		private Button Button_PWM_LIM;
		private ComboBox ComboBox_PWM_REG;
		private Button Button_PWM_REG;
		private ComboBox ComboBox_freewheel;
		private Button Button_freewheel;
		private ComboBox ComboBox_pwm_autograd;
		private Button Button_pwm_autograd;
		private ComboBox ComboBox_pwm_autoscale;
		private Button Button_pwm_autoscale;
		private ComboBox ComboBox_pwm_freq;
		private Button Button_pwm_freq;
		private Button Button_PWM_GRAD;
		private Button Button_PWM_OFS;
		private TextBox TextBox_PWM_OFS;
		private TextBox TextBox_PWM_GRAD;
		private TextBox TextBox_stst;
		private Button Button_stst;
		private TextBox TextBox_stealth;
		private Button Button_stealth;
		private TextBox TextBox_CS_ACTUAL;
		private Button Button_CS_ACTUAL;
		private TextBox TextBox_t157;
		private Button Button_t157;
		private TextBox TextBox_t150;
		private Button Button_t150;
		private TextBox TextBox_t143;
		private Button Button_t143;
		private TextBox TextBox_t120;
		private Button Button_t120;
		private TextBox TextBox_olb;
		private Button Button_olb;
		private TextBox TextBox_ola;
		private Button Button_ola;
		private TextBox TextBox_s2vsb;
		private Button Button_s2vsb;
		private TextBox TextBox_s2vsa;
		private Button Button_s2vsa;
		private TextBox TextBox_s2gb;
		private Button Button_s2gb;
		private TextBox TextBox_s2ga;
		private Button Button_s2ga;
		private TextBox TextBox_ot;
		private Button Button_ot;
		private GroupBox GroupBox_Chopper_Control_Register;
		private GroupBox GroupBox_PWM_SCALE_AUTO;
		private TextBox TextBox_PWM_GRAD_AUTO;
		private Button Button_PWM_GRAD_AUTO;
		private TextBox TextBox_PWM_OFS_AUTO;
		private Button Button_PWM_OFS_AUTO;
		private TextBox TextBox_PWM_SCALE_AUTO;
		private Button Button_PWM_SCALE_AUTO;
		private TextBox TextBox_PWM_SCALE_SUM;
		private Button Button_PWM_SCALE_SUM;
		private Button ButtonSpeedDryStart;
		private Button ButtonSpeedDryStop;
		private Label LabelZoom;
		private Button ButtonZoom1ResetTele;
		private Button ButtonFocusNear;
		private Button ButtonZoomTele;
		private Button ButtonZoomTeleWOTrack;
		private Button ButtonFocusFar;
		private Button ButtonZoomWide;
		private Button ButtonZoomWideWOTrack;
		private Button ButtonFocusStop;
		private Button ButtonZoomStop;
		private Label LabelZoom1Reset;
		private Label LabelFocus;
		private Button ButtonFBufNear;
		private Button ButtonZ2BufTele;
		private Button ButtonZ1BufTele;
		private Button ButtonFBufFar;
		private Button ButtonZ2BufWide;
		private Button ButtonZ1BufWide;
		private Label LabelFBuf;
		private Label LabelZ2Buf;
		private Label LabelZoom1Buf;
		private TextBox TextBoxZ1Pos;
		private Button ButtonSetZ1Pos;
		private GroupBox groupBox7;
		private Button ButtonMotorType0p9d;
		private Button ButtonMotorType1p8d;
		private Button ButtonReverseCalibration;
		private GroupBox groupBox11;
		private Button ButtonStallCaliOn;
		private Button ButtonStallCaliOff;
		private Button ButtonHome_2;
		private ComboBox ComboBoxPTType;
		private Button ButtonZoom1ResetWide;
		private Button ButtonZoom1ResetSensor;
		private Button ButtonZoomCtrlGroup1;
		private Label LabelZoomCtrlGroup;
		private Button ButtonZoomCtrlGroup2;
		private Button ButtonZoomCtrlModeTrack;
		private Label LabelZoomCtrlMode;
		private Button ButtonZoomCtrlModeInd;
		private Button ButtonFocusResetSensor;
		private Button ButtonFocusResetWide;
		private Button ButtonFocusResetTele;
		private Label LabelFocusReset;
		private Button ButtonGetZ1Pos;
		private Button ButtonGetFPos;
		private Button ButtonSetFPos;
		private TextBox TextBoxFPos;
		private Button ButtonGetZ2Pos;
		private Button ButtonSetZ2Pos;
		private TextBox TextBoxZ2Pos;
		private TextBox TextBoxZoomStep;
		private Button ButtonZoomStepTele;
		private Button ButtonZoomStepWide;
		private Label LabelZoomStep;
		private TextBox TextBoxFocusStep;
		private Button ButtonFocusStepNear;
		private Button ButtonFocusStepFar;
		private Label LabelFocusStep;
		private Button ButtonICRDefog;
		private Button ButtonICRNight;
		private Button ButtonICRNormal;
		private Button ButtonICRDay;
		private Label LabelICR;
		private System.Windows.Forms.Timer TimerUpdateSpeed;
		#endregion

		enum GeneralCommand
		{
			SYS_NONE				= 0x000000,
			SYS_ReadVersion			= 0x090002,
			SYS_ReadMCUType			= 0x090003,

			MD_ReadMotorPosition	= 0x090612,
			MD_ReadMotoeAccLevel	= 0x090631,
			MD_ReadSpeedByZoomRatio	= 0x0906A2,

			AS_ReadAngleReg			= 0xD90501,
			AS_ReadMagFlux			= 0xD90502,
			AS_ReadMode				= 0xD90503,
			AS_ReadErrorMonitor		= 0xD90504,
			AS_ReadZP				= 0xD90505,
			AS_ReadRDABZ			= 0xD90507,
			AS_ReadABZRes			= 0xD90508,
			AS_ReadMemLock			= 0xD90509,
			AS_ReadSDDIS			= 0xD9050A,

			AS_ReadAngle			= 0xD90551,
			AS_ReadAB				= 0xD90552,
			AS_ReadZ				= 0xD90553,
			AS_ReadErrorPin			= 0xD90554,
			AS_ReadZPCalibration	= 0xD90555,
			AS_ReadZPLock			= 0xD90556,

			MD_ReadAcceleration		= 0xD90601,
			MD_ReadPanType			= 0xD90602,
			MD_ReadSpeed			= 0xD90603,
			MD_ReadMotorSpeedInPPS	= 0xD90604,

			LS_ReadLux				= 0xD90800,
			LS_ReadConfiguration	= 0xD90801,
			LS_ReadLowLimit			= 0xD90802,
			LS_ReadHighLimit		= 0xD90803,
			LS_ReadManufacturerID	= 0xD9087E,
			LS_ReadDeviceID			= 0xD9087F,

			ALM_ReadStatus			= 0xD90901,
			ALM_ReadAlarmIn			= 0xD90902,
			ALM_ReadTrigLvl			= 0xD90903,

			RS_ReadMode				= 0xD90A01,
			RS_ReadTermR			= 0xD90A02,
			RS_ReadBaudRate			= 0xD90A03,
			RS_ReadStopBits			= 0xD90A04,
			RS_ReadRS485Dev			= 0xD90A05,
			RS_ReadRS485DevAddr		= 0xD90A06,
			RS_ReadDI				= 0xD90A07,
			RS_ReadDO				= 0xD90A0A,

			NTC_ReadNTC1			= 0xD90B01,
			NTC_ReadNTC2			= 0xD90B02,

			TLE_Read_S_RST			= 0x09050000,
			TLE_Read_Error			= 0x09050001,
			TLE_Read_S_NR			= 0x0905000D,
			TLE_Read_RD_ST			= 0x0905000F,
			TLE_Read_AS_RST			= 0x09050100,
			TLE_Read_AS_WD			= 0x09050101,
			TLE_Read_AS_VR			= 0x09050102,
			TLE_Read_AS_FUSE		= 0x09050103,
			TLE_Read_AS_DSPU		= 0x09050104,
			TLE_Read_AS_OV			= 0x09050105,
			TLE_Read_AS_VEC_XY		= 0x09050106,
			TLE_Read_AS_VEC_MAG		= 0x09050107,
			TLE_Read_AS_ADCT		= 0x09050109,
			TLE_Read_AS_FRST		= 0x0905010A,
			TLE_Read_ANG_VAL		= 0x09050200,
			TLE_Read_RD_AV			= 0x0905020F,
			TLE_Read_ANG_SPD		= 0x09050300,
			TLE_Read_RD_AS			= 0x0905030F,
			TLE_Read_REVOL			= 0x09050400,
			TLE_Read_FCNT			= 0x09050409,
			TLE_Read_RD_REV			= 0x0905040F,
			TLE_Read_TEMPER			= 0x09050500,
			TLE_Read_FSYNC			= 0x09050509,
			TLE_Read_IIF_MOD		= 0x09050600,
			TLE_Read_DSPU_HOLD		= 0x09050602,
			TLE_Read_CLK_SEL		= 0x09050604,
			TLE_Read_FIR_MD			= 0x0905060E,
			TLE_Read_ADCTV_X		= 0x09050700,
			TLE_Read_ADCTV_Y		= 0x09050703,
			TLE_Read_ADCTV_EN		= 0x09050706,
			TLE_Read_FUSE_REL		= 0x0905070A,
			TLE_Read_FILT_INV		= 0x0905070E,
			TLE_Read_FILT_PAR		= 0x0905070F,
			TLE_Read_AUTOCAL		= 0x09050800,
			TLE_Read_PREDICT		= 0x09050802,
			TLE_Read_ANG_DIR		= 0x09050803,
			TLE_Read_ANG_RANGE		= 0x09050804,
			TLE_Read_PAD_DRV		= 0x09050900,
			TLE_Read_SSC_OD			= 0x09050902,
			TLE_Read_SPIKEF			= 0x09050903,
			TLE_Read_ANG_BASE		= 0x09050904,
			TLE_Read_OFFX			= 0x09050A00,
			TLE_Read_OFFY			= 0x09050B00,
			TLE_Read_SYNCH			= 0x09050C00,
			TLE_Read_IFAB_HYST		= 0x09050D00,
			TLE_Read_IFAB_OD		= 0x09050D02,
			TLE_Read_FIR_UDR		= 0x09050D03,
			TLE_Read_ORTHO			= 0x09050D04,
			TLE_Read_IF_MD			= 0x09050E00,
			TLE_Read_IFAB_RES		= 0x09050E03,
			TLE_Read_HSM_PLP		= 0x09050E05,
			TLE_Read_TCO_X_T		= 0x09050E09,
			TLE_Read_CRC_PAR		= 0x09050F00,
			TLE_Read_SBIST			= 0x09050F08,
			TLE_Read_TCO_Y_T		= 0x09050F09,
			TLE_Read_ADC_X			= 0x09051000,
			TLE_Read_ADC_Y			= 0x09051100,
			TLE_Read_MAG			= 0x09051400,
			TLE_Read_T_RAW			= 0x09051500,
			TLE_Read_T_TGL			= 0x0905150F,
			TLE_Read_IIF_CNT		= 0x09052000,
			TLE_Read_T25O			= 0x09053000,

			TMC_Read_I_scale_analog		= 0x09070000,
			TMC_Read_internal_Rsense	= 0x09070001,
			TMC_Read_en_SpreadCycle		= 0x09070002,
			TMC_Read_shaft				= 0x09070003,
			TMC_Read_index_otpw			= 0x09070004,
			TMC_Read_index_step			= 0x09070005,
			TMC_Read_pdn_disable		= 0x09070006,
			TMC_Read_mstep_reg_select	= 0x09070007,
			TMC_Read_multistep_filt		= 0x09070008,
			TMC_Read_test_mode			= 0x09070009,
			TMC_Read_reset				= 0x09070100,
			TMC_Read_drv_err			= 0x09070101,
			TMC_Read_uv_cp				= 0x09070102,
			TMC_Read_IFCNT				= 0x09070200,
			TMC_Read_SLAVECONF			= 0x09070300,
			TMC_Read_OTP_READ			= 0x09070500,
			TMC_Read_ENN				= 0x09070600,
			TMC_Read_MS1				= 0x09070601,
			TMC_Read_MS2				= 0x09070602,
			TMC_Read_DIAG				= 0x09070604,
			TMC_Read_PDN_UART			= 0x09070606,
			TMC_Read_STEP				= 0x09070607,
			TMC_Read_SPREAD_EN			= 0x09070608,
			TMC_Read_DIR				= 0x09070609,
			TMC_Read_VERSION			= 0x09070618,
			TMC_Read_FCLKTRIM			= 0x09070700,
			TMC_Read_OTTRIM				= 0x09070708,
			TMC_Read_IHOLD				= 0x09071000,
			TMC_Read_IRUN				= 0x09071008,
			TMC_Read_IHOLDDELAY			= 0x09071010,
			TMC_Read_TPOWERDOWN			= 0x09071100,
			TMC_Read_TSTEP				= 0x09071200,
			TMC_Read_TPWMTHRS			= 0x09071300,
			TMC_Read_TCOOLTHRS			= 0x09071400,
			TMC_Read_VACTUAL			= 0x09072200,
			TMC_Read_SGTHRS				= 0x09074000,
			TMC_Read_SG_RESULT			= 0x09074100,
			TMC_Read_semin				= 0x09074200,
			TMC_Read_seup				= 0x09074205,
			TMC_Read_semax				= 0x09074208,
			TMC_Read_sedn				= 0x0907420D,
			TMC_Read_seimin				= 0x0907420F,
			TMC_Read_MSCNT				= 0x09076A00,
			TMC_Read_CUR_A				= 0x09076B00,
			TMC_Read_CUR_B				= 0x09076B10,
			TMC_Read_TOFF				= 0x09076C00,
			TMC_Read_HSTRT				= 0x09076C04,
			TMC_Read_HEND				= 0x09076C07,
			TMC_Read_TBL				= 0x09076C0F,
			TMC_Read_vsense				= 0x09076C11,
			TMC_Read_MRES				= 0x09076C18,
			TMC_Read_intpol				= 0x09076C1C,
			TMC_Read_dedge				= 0x09076C1D,
			TMC_Read_diss2g				= 0x09076C1E,
			TMC_Read_diss2vs			= 0x09076C1F,
			TMC_Read_otpw				= 0x09076F00,
			TMC_Read_ot					= 0x09076F01,
			TMC_Read_s2ga				= 0x09076F02,
			TMC_Read_s2gb				= 0x09076F03,
			TMC_Read_s2vsa				= 0x09076F04,
			TMC_Read_s2vsb				= 0x09076F05,
			TMC_Read_ola				= 0x09076F06,
			TMC_Read_olb				= 0x09076F07,
			TMC_Read_t120				= 0x09076F08,
			TMC_Read_t143				= 0x09076F09,
			TMC_Read_t150				= 0x09076F0A,
			TMC_Read_t157				= 0x09076F0B,
			TMC_Read_CS_ACTUAL			= 0x09076F10,
			TMC_Read_stealth			= 0x09076F1E,
			TMC_Read_stst				= 0x09076F1F,
			TMC_Read_PWM_OFS			= 0x09077000,
			TMC_Read_PWM_GRAD			= 0x09077008,
			TMC_Read_pwm_freq			= 0x09077010,
			TMC_Read_pwm_autoscale		= 0x09077012,
			TMC_Read_pwm_autograd		= 0x09077013,
			TMC_Read_freewheel			= 0x09077014,
			TMC_Read_PWM_REG			= 0x09077018,
			TMC_Read_PWM_LIM			= 0x0907701C,
			TMC_Read_PWM_SCALE_SUM		= 0x09077100,
			TMC_Read_PWM_SCALE_AUTO		= 0x09077110,
			TMC_Read_PWM_OFS_AUTO		= 0x09077200,
			TMC_Read_PWM_GRAD_AUTO		= 0x09077210,

			Lens_ReadZ1Position			= 0x090447,
			Lens_ReadFPosition			= 0x090448,
			Lens_ReadZ2Position			= 0x090457,
		}

		#region Constructor
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

		/// <summary>
		/// Terminal constructor. Initialization.
		/// </summary>
		[Obsolete]
		public Terminal()
		{
			InitializeComponent();

			TxBytes = 0;
			this.TxData.Text = "";

			RxBytes = 0;
			this.RxData.Text = "";

			this.Settings.Text = "";

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
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[Obsolete]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Terminal));
			this.ComPort = new System.Windows.Forms.ComboBox();
			this.Settings = new System.Windows.Forms.TextBox();
			this.LblSettings = new System.Windows.Forms.Label();
			this.LblPort = new System.Windows.Forms.Label();
			this.CmdOpen = new System.Windows.Forms.Button();
			this.TxData = new System.Windows.Forms.TextBox();
			this.LblTxData = new System.Windows.Forms.Label();
			this.RxData = new System.Windows.Forms.TextBox();
			this.LblRxData = new System.Windows.Forms.Label();
			this.CmdConfig = new System.Windows.Forms.Button();
			this.CmdClear = new System.Windows.Forms.Button();
			this.RecvTimer = new System.Windows.Forms.Timer(this.components);
			this.GroupBoxSettings = new System.Windows.Forms.GroupBox();
			this.tabVisca = new System.Windows.Forms.TabControl();
			this.tabMenu = new System.Windows.Forms.TabPage();
			this.groupBoxShowSpeed = new System.Windows.Forms.GroupBox();
			this.ButtonShowSpeed = new System.Windows.Forms.Button();
			this.ButtonClearShowSpeed = new System.Windows.Forms.Button();
			this.ButtonStopShowSpeed = new System.Windows.Forms.Button();
			this.ChartSpeed = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ButtonHome_2 = new System.Windows.Forms.Button();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.ButtonStallCaliOn = new System.Windows.Forms.Button();
			this.ButtonStallCaliOff = new System.Windows.Forms.Button();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.ButtonMotorType0p9d = new System.Windows.Forms.Button();
			this.ButtonMotorType1p8d = new System.Windows.Forms.Button();
			this.groupBox20 = new System.Windows.Forms.GroupBox();
			this.ButtonImageFlipMaxAngleOn = new System.Windows.Forms.Button();
			this.ButtonImageFlipMaxAngleOff = new System.Windows.Forms.Button();
			this.groupBox17 = new System.Windows.Forms.GroupBox();
			this.label12 = new System.Windows.Forms.Label();
			this.ButtonRelDown = new System.Windows.Forms.Button();
			this.ButtonRelUp = new System.Windows.Forms.Button();
			this.TextBoxRelStep = new System.Windows.Forms.TextBox();
			this.ButtonRelLeft = new System.Windows.Forms.Button();
			this.ButtonRelStop = new System.Windows.Forms.Button();
			this.ButtonRelRight = new System.Windows.Forms.Button();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.TextBoxMotorAngle = new System.Windows.Forms.TextBox();
			this.ButtonGetAngle = new System.Windows.Forms.Button();
			this.textBoxZCount = new System.Windows.Forms.TextBox();
			this.TextBoxMotorPosition = new System.Windows.Forms.TextBox();
			this.ButtonGetPosition = new System.Windows.Forms.Button();
			this.ButtonZCount = new System.Windows.Forms.Button();
			this.ButtonABCount = new System.Windows.Forms.Button();
			this.textBoxABCount = new System.Windows.Forms.TextBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.ButtonABSAngleStop = new System.Windows.Forms.Button();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.ButtonPanType = new System.Windows.Forms.Button();
			this.ComboBoxPanMethod = new System.Windows.Forms.ComboBox();
			this.TextBoxABSAngle2 = new System.Windows.Forms.TextBox();
			this.ButtonABSStop = new System.Windows.Forms.Button();
			this.ButtonABSAngle2 = new System.Windows.Forms.Button();
			this.TextBoxABS2Pos = new System.Windows.Forms.TextBox();
			this.TextBoxABSAngle = new System.Windows.Forms.TextBox();
			this.ButtonABS2 = new System.Windows.Forms.Button();
			this.ButtonABSAngle = new System.Windows.Forms.Button();
			this.TextBoxABSPos = new System.Windows.Forms.TextBox();
			this.ButtonABS = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.TextBoxStopAt = new System.Windows.Forms.TextBox();
			this.ButtonStopAt = new System.Windows.Forms.Button();
			this.CheckBoxMoveStop = new System.Windows.Forms.CheckBox();
			this.ButtonTiltDown = new System.Windows.Forms.Button();
			this.ButtonTiltUp = new System.Windows.Forms.Button();
			this.ButtonPanLeft = new System.Windows.Forms.Button();
			this.ButtonPanRight = new System.Windows.Forms.Button();
			this.ButtonPanStop = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox16 = new System.Windows.Forms.GroupBox();
			this.ButtonSpeedByZoomOff = new System.Windows.Forms.Button();
			this.ButtonGetSpeedByZoomRatio = new System.Windows.Forms.Button();
			this.TextBoxSpeedByZoomRatio = new System.Windows.Forms.TextBox();
			this.ButtonSpeedByZoomOn = new System.Windows.Forms.Button();
			this.groupBox15 = new System.Windows.Forms.GroupBox();
			this.ButtonGetCurrentSpeed = new System.Windows.Forms.Button();
			this.TextBoxTargetSpeed = new System.Windows.Forms.TextBox();
			this.ButtonSetTargetSpeed = new System.Windows.Forms.Button();
			this.TextBoxCurrentSpeed = new System.Windows.Forms.TextBox();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.TextBoxSpeedInPPS = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.TextBoxSpeedLevel = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.ComboBoxAccLevel = new System.Windows.Forms.ComboBox();
			this.ButtonGetAccLevel = new System.Windows.Forms.Button();
			this.ButtonSetAccLevel = new System.Windows.Forms.Button();
			this.ButtonGetAcceleration = new System.Windows.Forms.Button();
			this.TextBoxAcceleration = new System.Windows.Forms.TextBox();
			this.ButtonSetAcceleration = new System.Windows.Forms.Button();
			this.tabAK7452 = new System.Windows.Forms.TabPage();
			this.groupBoxShowAngle = new System.Windows.Forms.GroupBox();
			this.ButtonClearShowAngle = new System.Windows.Forms.Button();
			this.ButtonShowAngle = new System.Windows.Forms.Button();
			this.ButtonStopShowAngle = new System.Windows.Forms.Button();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.ButtonReverseCalibration = new System.Windows.Forms.Button();
			this.TextBoxLockStatus = new System.Windows.Forms.TextBox();
			this.ButtonLockStatus = new System.Windows.Forms.Button();
			this.ButtonUnlockHome = new System.Windows.Forms.Button();
			this.ButtonLockHome = new System.Windows.Forms.Button();
			this.TextBoxZPCalibration = new System.Windows.Forms.TextBox();
			this.ButtonZPCaliStatus = new System.Windows.Forms.Button();
			this.ButtonClearZPCali = new System.Windows.Forms.Button();
			this.ButtonHome = new System.Windows.Forms.Button();
			this.ButtonCalibration = new System.Windows.Forms.Button();
			this.groupBoxAK7452ManualMode = new System.Windows.Forms.GroupBox();
			this.ButtonDataRenewal = new System.Windows.Forms.Button();
			this.CheckBoxSaveToEEPROM = new System.Windows.Forms.CheckBox();
			this.groupBox19 = new System.Windows.Forms.GroupBox();
			this.ComboBoxSDDIS = new System.Windows.Forms.ComboBox();
			this.ButtonSetSDDIS = new System.Windows.Forms.Button();
			this.ButtonGetSDDIS = new System.Windows.Forms.Button();
			this.groupBoxAK7452ABZResolution = new System.Windows.Forms.GroupBox();
			this.ButtonSetABZRes = new System.Windows.Forms.Button();
			this.ButtonGetABZRes = new System.Windows.Forms.Button();
			this.TextBoxABZRes = new System.Windows.Forms.TextBox();
			this.TextBoxMemLock = new System.Windows.Forms.TextBox();
			this.ButtonMemLock = new System.Windows.Forms.Button();
			this.groupBoxAK7452ZeroDegreePoint = new System.Windows.Forms.GroupBox();
			this.ButtonGetZP = new System.Windows.Forms.Button();
			this.TextBoxZP = new System.Windows.Forms.TextBox();
			this.ButtonSetZP = new System.Windows.Forms.Button();
			this.groupBoxAK7452RDABZ = new System.Windows.Forms.GroupBox();
			this.ComboBoxABZHysteresis = new System.Windows.Forms.ComboBox();
			this.ComboBoxABZEnable = new System.Windows.Forms.ComboBox();
			this.ComboBoxZWidth = new System.Windows.Forms.ComboBox();
			this.ComboBoxRD = new System.Windows.Forms.ComboBox();
			this.labelAK7452ABZHysteresis = new System.Windows.Forms.Label();
			this.labelAK7452ABZEnable = new System.Windows.Forms.Label();
			this.labelAK7452ZWidth = new System.Windows.Forms.Label();
			this.labelAK7452RotationDir = new System.Windows.Forms.Label();
			this.ButtonSetRDABZ = new System.Windows.Forms.Button();
			this.ButtonGetRDABZ = new System.Windows.Forms.Button();
			this.TextBoxErrorMonitor = new System.Windows.Forms.TextBox();
			this.ButtonErrorMonitor = new System.Windows.Forms.Button();
			this.TextBoxASMode = new System.Windows.Forms.TextBox();
			this.ButtonASMode = new System.Windows.Forms.Button();
			this.TextBoxMagFlux = new System.Windows.Forms.TextBox();
			this.ButtonMagFlux = new System.Windows.Forms.Button();
			this.TextBoxAngleDataReg = new System.Windows.Forms.TextBox();
			this.ButtonSetMaualMode = new System.Windows.Forms.Button();
			this.ButtonAngleDataReg = new System.Windows.Forms.Button();
			this.chartAngle = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.groupBoxAK7452NormalMode = new System.Windows.Forms.GroupBox();
			this.textBoxErrorPin = new System.Windows.Forms.TextBox();
			this.ButtonErrorPin = new System.Windows.Forms.Button();
			this.textBoxAngleData = new System.Windows.Forms.TextBox();
			this.ButtonSetNormalMode = new System.Windows.Forms.Button();
			this.ButtonAngleData = new System.Windows.Forms.Button();
			this.tabTLE5012B = new System.Windows.Forms.TabPage();
			this.GroupBoxTleT_RAW = new System.Windows.Forms.GroupBox();
			this.TextBoxTleT_TGL = new System.Windows.Forms.TextBox();
			this.ButtonTleGetT_TGL = new System.Windows.Forms.Button();
			this.TextBoxTleT_RAW = new System.Windows.Forms.TextBox();
			this.ButtonTleGetT_RAW = new System.Windows.Forms.Button();
			this.GroupBoxTleIIF_CNT = new System.Windows.Forms.GroupBox();
			this.TextBoxTleIIF_CNT = new System.Windows.Forms.TextBox();
			this.ButtonTleGetIIF_CNT = new System.Windows.Forms.Button();
			this.GroupBoxTleD_MAG = new System.Windows.Forms.GroupBox();
			this.TextBoxTleMAG = new System.Windows.Forms.TextBox();
			this.ButtonTleGetMAG = new System.Windows.Forms.Button();
			this.GroupBoxTleADC_Y = new System.Windows.Forms.GroupBox();
			this.TextBoxTleADC_Y = new System.Windows.Forms.TextBox();
			this.ButtonTleGetADC_Y = new System.Windows.Forms.Button();
			this.GroupBoxTleT25O = new System.Windows.Forms.GroupBox();
			this.TextBoxTleT25O = new System.Windows.Forms.TextBox();
			this.ButtonTleGetT25O = new System.Windows.Forms.Button();
			this.GroupBoxTleADC_X = new System.Windows.Forms.GroupBox();
			this.TextBoxTleADC_X = new System.Windows.Forms.TextBox();
			this.ButtonTleGetADC_X = new System.Windows.Forms.Button();
			this.GroupBoxTleTCO_Y = new System.Windows.Forms.GroupBox();
			this.TextBoxTleCRC_PAR = new System.Windows.Forms.TextBox();
			this.ButtonTleSetCRC_PAR = new System.Windows.Forms.Button();
			this.TextBoxTleTCO_Y_T = new System.Windows.Forms.TextBox();
			this.ButtonTleSetTCO_Y_T = new System.Windows.Forms.Button();
			this.ButtonTleGetTCO_Y_T = new System.Windows.Forms.Button();
			this.ComboBoxTleSBIST = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetSBIST = new System.Windows.Forms.Button();
			this.ButtonTleGetCRC_PAR = new System.Windows.Forms.Button();
			this.GroupBoxTleMOD_4 = new System.Windows.Forms.GroupBox();
			this.TextBoxTleTCO_X_T = new System.Windows.Forms.TextBox();
			this.ButtonTleSetTCO_X_T = new System.Windows.Forms.Button();
			this.ButtonTleGetTCO_X_T = new System.Windows.Forms.Button();
			this.ComboBoxTleHSM_PLP = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetHSM_PLP = new System.Windows.Forms.Button();
			this.ComboBoxTleIFAB_RES = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleIF_MD = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetIFAB_RES = new System.Windows.Forms.Button();
			this.ButtonTleGetIF_MD = new System.Windows.Forms.Button();
			this.GroupBoxTleIFAB = new System.Windows.Forms.GroupBox();
			this.TextBoxTleORTHO = new System.Windows.Forms.TextBox();
			this.ButtonTleSetORTHO = new System.Windows.Forms.Button();
			this.ButtonTleGetORTHO = new System.Windows.Forms.Button();
			this.ComboBoxTleFIR_UDR = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetFIR_UDR = new System.Windows.Forms.Button();
			this.ComboBoxTleIFAB_OD = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleIFAB_HYST = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetIFAB_OD = new System.Windows.Forms.Button();
			this.ButtonTleGetIFAB_HYST = new System.Windows.Forms.Button();
			this.GroupBoxTleSYNCH = new System.Windows.Forms.GroupBox();
			this.ButtonTleSetSYNCH = new System.Windows.Forms.Button();
			this.TextBoxTleSYNCH = new System.Windows.Forms.TextBox();
			this.ButtonTleGetSYNCH = new System.Windows.Forms.Button();
			this.GroupBoxTleOFFSET_Y = new System.Windows.Forms.GroupBox();
			this.ButtonTleSetOFFSET_Y = new System.Windows.Forms.Button();
			this.TextBoxTleOFFY = new System.Windows.Forms.TextBox();
			this.ButtonTleGetOFFSET_Y = new System.Windows.Forms.Button();
			this.GroupBoxTleOFFX = new System.Windows.Forms.GroupBox();
			this.ButtonTleSetOFFSET_X = new System.Windows.Forms.Button();
			this.TextBoxTleOFFX = new System.Windows.Forms.TextBox();
			this.ButtonTleGetOFFSET_X = new System.Windows.Forms.Button();
			this.GroupBoxTleMOD_3 = new System.Windows.Forms.GroupBox();
			this.TextBoxTleANG_BASE = new System.Windows.Forms.TextBox();
			this.ButtonTleSetANG_BASE = new System.Windows.Forms.Button();
			this.ButtonTleGetANG_BASE = new System.Windows.Forms.Button();
			this.ComboBoxTleSPIKEF = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetSPIKEF = new System.Windows.Forms.Button();
			this.ComboBoxTleSSC_OD = new System.Windows.Forms.ComboBox();
			this.ComboBoxTlePAD_DRV = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetSSC_OD = new System.Windows.Forms.Button();
			this.ButtonTleGetPAD_DRV = new System.Windows.Forms.Button();
			this.GroupBoxTleMOD_2 = new System.Windows.Forms.GroupBox();
			this.TextBoxTleANG_RANGE = new System.Windows.Forms.TextBox();
			this.ButtonTleSetANGLE_RANGE = new System.Windows.Forms.Button();
			this.ButtonTleGetANG_RANGE = new System.Windows.Forms.Button();
			this.ComboBoxTleANG_DIR = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetANG_DIR = new System.Windows.Forms.Button();
			this.ComboBoxTlePREDICT = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleAUTOCAL = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetPREDICT = new System.Windows.Forms.Button();
			this.ButtonTleGetAUTOCAL = new System.Windows.Forms.Button();
			this.GroupBoxTleSIL = new System.Windows.Forms.GroupBox();
			this.ComboBoxTleFILT_PAR = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetFILT_PAR = new System.Windows.Forms.Button();
			this.ComboBoxTleFILT_INV = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetFILT_INV = new System.Windows.Forms.Button();
			this.ComboBoxTleFUSE_REL = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetFUSE_REL = new System.Windows.Forms.Button();
			this.ComboBoxTleADCTV_EN = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetADCTV_EN = new System.Windows.Forms.Button();
			this.ComboBoxTleADCTV_Y = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetADCTV_X = new System.Windows.Forms.Button();
			this.ComboBoxTleADCTV_X = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetADCTV_Y = new System.Windows.Forms.Button();
			this.GroupBoxTleMOD_1 = new System.Windows.Forms.GroupBox();
			this.ComboBoxTleFIR_MD = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetFIR_MD = new System.Windows.Forms.Button();
			this.ComboBoxTleCLK_SEL = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetCLK_SEL = new System.Windows.Forms.Button();
			this.ComboBoxTleDSPU_HOLD = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleIIF_MOD = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetDSPU_HOLD = new System.Windows.Forms.Button();
			this.ButtonTleGetIIF_MOD = new System.Windows.Forms.Button();
			this.GroupBoxTleFSYNC = new System.Windows.Forms.GroupBox();
			this.TextBoxTleFSYNC = new System.Windows.Forms.TextBox();
			this.ButtonTleGetFSYNC = new System.Windows.Forms.Button();
			this.TextBoxTleTEMPER = new System.Windows.Forms.TextBox();
			this.ButtonTleGetTEMPER = new System.Windows.Forms.Button();
			this.GroupBoxTleAREV = new System.Windows.Forms.GroupBox();
			this.TextBoxTleRD_REV = new System.Windows.Forms.TextBox();
			this.ButtonTleGetRD_REV = new System.Windows.Forms.Button();
			this.TextBoxTleFCNT = new System.Windows.Forms.TextBox();
			this.ButtonTleGetFCNT = new System.Windows.Forms.Button();
			this.TextBoxTleREVOL = new System.Windows.Forms.TextBox();
			this.ButtonTleGetRevol = new System.Windows.Forms.Button();
			this.GroupBoxTleASPD = new System.Windows.Forms.GroupBox();
			this.TextBoxTleRD_AS = new System.Windows.Forms.TextBox();
			this.ButtonTleGetRD_AS = new System.Windows.Forms.Button();
			this.TextBoxTleANG_SPD = new System.Windows.Forms.TextBox();
			this.ButtonTleGetANG_SPD = new System.Windows.Forms.Button();
			this.GroupBoxTleAVAL = new System.Windows.Forms.GroupBox();
			this.TextBoxTleRD_AV = new System.Windows.Forms.TextBox();
			this.ButtonTleGetRD_AV = new System.Windows.Forms.Button();
			this.TextBoxTleANG_VAL = new System.Windows.Forms.TextBox();
			this.ButtonTleGetANG_VAL = new System.Windows.Forms.Button();
			this.GroupBoxTleACSTAT = new System.Windows.Forms.GroupBox();
			this.ComboBoxTleAS_FRST = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetAS_FRST = new System.Windows.Forms.Button();
			this.ComboBoxTleAS_ADCT = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetAS_ADCT = new System.Windows.Forms.Button();
			this.ComboBoxTleAS_VEC_MAG = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleAS_VEC_XY = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleAS_OV = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetAS_VEC_MAG = new System.Windows.Forms.Button();
			this.ButtonTleGetAS_VEC_XY = new System.Windows.Forms.Button();
			this.ButtonTleGetAS_OV = new System.Windows.Forms.Button();
			this.ComboBoxTleAS_DSPU = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetAS_DSPU = new System.Windows.Forms.Button();
			this.ComboBoxTleAS_FUSE = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetAcstatAS_FUSE = new System.Windows.Forms.Button();
			this.ComboBoxTleAS_VR = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleAS_WD = new System.Windows.Forms.ComboBox();
			this.ComboBoxTleAS_RST = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetAS_VR = new System.Windows.Forms.Button();
			this.ButtonTleGetAS_WD = new System.Windows.Forms.Button();
			this.ButtonTleGetAS_RST = new System.Windows.Forms.Button();
			this.groupTleSTAT = new System.Windows.Forms.GroupBox();
			this.ComboBoxTleStatS_NR = new System.Windows.Forms.ComboBox();
			this.ButtonTleGetSlave = new System.Windows.Forms.Button();
			this.ButtonTleGetRD_ST = new System.Windows.Forms.Button();
			this.TextBoxTleStatError = new System.Windows.Forms.TextBox();
			this.TextBoxTleStatRD_ST = new System.Windows.Forms.TextBox();
			this.ButtonTleGetError = new System.Windows.Forms.Button();
			this.TextBoxTleStatS_RST = new System.Windows.Forms.TextBox();
			this.ButtonTleGetS_RST = new System.Windows.Forms.Button();
			this.tabLightSensor = new System.Windows.Forms.TabPage();
			this.groupBoxLSID = new System.Windows.Forms.GroupBox();
			this.ButtonLSDID = new System.Windows.Forms.Button();
			this.TextBoxLSDID = new System.Windows.Forms.TextBox();
			this.TextBoxLSID = new System.Windows.Forms.TextBox();
			this.ButtonGetLSID = new System.Windows.Forms.Button();
			this.groupBoxLSLimit = new System.Windows.Forms.GroupBox();
			this.LabelLSLuxLimit = new System.Windows.Forms.Label();
			this.CheckBoxLSHighLimit = new System.Windows.Forms.CheckBox();
			this.LabelLSRLimit = new System.Windows.Forms.Label();
			this.CheckBoxLSLowLimit = new System.Windows.Forms.CheckBox();
			this.TextBoxLSRLimit = new System.Windows.Forms.TextBox();
			this.ButtonSetLSLimit = new System.Windows.Forms.Button();
			this.LabelLSELimit = new System.Windows.Forms.Label();
			this.ButtonGetLSLimit = new System.Windows.Forms.Button();
			this.TextBoxLSELimit = new System.Windows.Forms.TextBox();
			this.TextBoxLSLuxLimit = new System.Windows.Forms.TextBox();
			this.groupBoxLSConfiguration = new System.Windows.Forms.GroupBox();
			this.LabelLSL = new System.Windows.Forms.Label();
			this.TextBoxLSL = new System.Windows.Forms.TextBox();
			this.LabelLSOVF = new System.Windows.Forms.Label();
			this.LabelLSCRF = new System.Windows.Forms.Label();
			this.TextBoxLSOVF = new System.Windows.Forms.TextBox();
			this.TextBoxLSCRF = new System.Windows.Forms.TextBox();
			this.LabelLSFH = new System.Windows.Forms.Label();
			this.LabelLSFL = new System.Windows.Forms.Label();
			this.TextBoxLSFH = new System.Windows.Forms.TextBox();
			this.LabelLSRN = new System.Windows.Forms.Label();
			this.TextBoxLSFL = new System.Windows.Forms.TextBox();
			this.LabelLSCT = new System.Windows.Forms.Label();
			this.TextBoxLSRN = new System.Windows.Forms.TextBox();
			this.TextBoxLSCT = new System.Windows.Forms.TextBox();
			this.LabelLSM = new System.Windows.Forms.Label();
			this.TextBoxLSM = new System.Windows.Forms.TextBox();
			this.LabelLSPOL = new System.Windows.Forms.Label();
			this.TextBoxLSPOL = new System.Windows.Forms.TextBox();
			this.LabelLSME = new System.Windows.Forms.Label();
			this.TextBoxLSME = new System.Windows.Forms.TextBox();
			this.LabelLSFC = new System.Windows.Forms.Label();
			this.ButtonSetCfg = new System.Windows.Forms.Button();
			this.TextBoxLSFC = new System.Windows.Forms.TextBox();
			this.ButtonLSGetCfg = new System.Windows.Forms.Button();
			this.groupBoxLSResult = new System.Windows.Forms.GroupBox();
			this.LabelLSLux = new System.Windows.Forms.Label();
			this.LabelLSR = new System.Windows.Forms.Label();
			this.TextBoxLSR = new System.Windows.Forms.TextBox();
			this.LabelLSE = new System.Windows.Forms.Label();
			this.TextBoxLSE = new System.Windows.Forms.TextBox();
			this.TextBoxLSLux = new System.Windows.Forms.TextBox();
			this.ButtonLSGetLux = new System.Windows.Forms.Button();
			this.tabAlarm = new System.Windows.Forms.TabPage();
			this.groupBoxNTC = new System.Windows.Forms.GroupBox();
			this.TextBoxNTC2 = new System.Windows.Forms.TextBox();
			this.ButtonGetNTC2 = new System.Windows.Forms.Button();
			this.TextBoxNTC1 = new System.Windows.Forms.TextBox();
			this.ButtonNTCSingleScan = new System.Windows.Forms.Button();
			this.ButtonGetNTC1 = new System.Windows.Forms.Button();
			this.groupBoxRS485 = new System.Windows.Forms.GroupBox();
			this.groupBoxSD700Set = new System.Windows.Forms.GroupBox();
			this.ButtonRS485GetAddr = new System.Windows.Forms.Button();
			this.ComboBoxRS485Dev = new System.Windows.Forms.ComboBox();
			this.ComboBoxSD700TrigLvl = new System.Windows.Forms.ComboBox();
			this.ButtonRS485GetDev = new System.Windows.Forms.Button();
			this.LabelSD700TrigLvl = new System.Windows.Forms.Label();
			this.LabelSD700AutoScan = new System.Windows.Forms.Label();
			this.ComboBoxSD700AutoScan = new System.Windows.Forms.ComboBox();
			this.TextBoxSD700Addr = new System.Windows.Forms.TextBox();
			this.ButtonSD700SetAddr = new System.Windows.Forms.Button();
			this.groupBoxRS485DO = new System.Windows.Forms.GroupBox();
			this.CheckBoxRS485DO8 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO7 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO6 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO5 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO4 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO3 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO2 = new System.Windows.Forms.CheckBox();
			this.CheckBoxRS485DO1 = new System.Windows.Forms.CheckBox();
			this.LabelRS485DO8 = new System.Windows.Forms.Label();
			this.LabelRS485DO7 = new System.Windows.Forms.Label();
			this.LabelRS485DO6 = new System.Windows.Forms.Label();
			this.LabelRS485DO5 = new System.Windows.Forms.Label();
			this.LabelRS485DO4 = new System.Windows.Forms.Label();
			this.LabelRS485DO3 = new System.Windows.Forms.Label();
			this.LabelRS485DO2 = new System.Windows.Forms.Label();
			this.ButtonRS485DO = new System.Windows.Forms.Button();
			this.LabelRS485DO1 = new System.Windows.Forms.Label();
			this.groupBoxRS485DI = new System.Windows.Forms.GroupBox();
			this.TextBoxRS485DI8 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI7 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI6 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI5 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI4 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI3 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI2 = new System.Windows.Forms.TextBox();
			this.TextBoxRS485DI1 = new System.Windows.Forms.TextBox();
			this.LabelRS485DI8 = new System.Windows.Forms.Label();
			this.LabelRS485DI7 = new System.Windows.Forms.Label();
			this.LabelRS485DI6 = new System.Windows.Forms.Label();
			this.LabelRS485DI5 = new System.Windows.Forms.Label();
			this.LabelRS485DI4 = new System.Windows.Forms.Label();
			this.LabelRS485DI3 = new System.Windows.Forms.Label();
			this.LabelRS485DI2 = new System.Windows.Forms.Label();
			this.ButtonRS485DI = new System.Windows.Forms.Button();
			this.LabelRS485DI1 = new System.Windows.Forms.Label();
			this.groupBoxRS485Comm = new System.Windows.Forms.GroupBox();
			this.ComboBoxRS485StopBits = new System.Windows.Forms.ComboBox();
			this.LabelRS485StopBits = new System.Windows.Forms.Label();
			this.ButtonRS485GetComm = new System.Windows.Forms.Button();
			this.ComboBoxRS485BaudRate = new System.Windows.Forms.ComboBox();
			this.LabelRS485BaudRate = new System.Windows.Forms.Label();
			this.groupBoxRS485TermR = new System.Windows.Forms.GroupBox();
			this.ComboBoxRS485TermR = new System.Windows.Forms.ComboBox();
			this.ButtonRS485GetTermR = new System.Windows.Forms.Button();
			this.groupBoxRS485TransceiverMode = new System.Windows.Forms.GroupBox();
			this.ComboBoxRS485Mode = new System.Windows.Forms.ComboBox();
			this.ButtonRS485GetMode = new System.Windows.Forms.Button();
			this.groupBoxAlarm = new System.Windows.Forms.GroupBox();
			this.groupBoxAlarmInq = new System.Windows.Forms.GroupBox();
			this.TextBoxAlarmDI1 = new System.Windows.Forms.TextBox();
			this.LabelAlarmDI1 = new System.Windows.Forms.Label();
			this.TextBoxAlarmDI0 = new System.Windows.Forms.TextBox();
			this.LabelAlarmDI0 = new System.Windows.Forms.Label();
			this.ButtonAlarmIn = new System.Windows.Forms.Button();
			this.TextBoxAlarmTrigLvl = new System.Windows.Forms.TextBox();
			this.TextBoxAlarmStatus = new System.Windows.Forms.TextBox();
			this.ButtonAlarmAutoStatus = new System.Windows.Forms.Button();
			this.ButtonAlarmTrigLvl = new System.Windows.Forms.Button();
			this.groupBoxAlarmOut = new System.Windows.Forms.GroupBox();
			this.ButtonAlarmOutHigh = new System.Windows.Forms.Button();
			this.ButtonAlarmOutLow = new System.Windows.Forms.Button();
			this.groupBoxAlarmAuto = new System.Windows.Forms.GroupBox();
			this.LabelAlarmTrigLvl = new System.Windows.Forms.Label();
			this.ComboBoxAlarmTrigLvl = new System.Windows.Forms.ComboBox();
			this.ButtonAlarmOn = new System.Windows.Forms.Button();
			this.ButtonAlarmOff = new System.Windows.Forms.Button();
			this.groupBoxRS485Test = new System.Windows.Forms.GroupBox();
			this.TextBoxRS485TestTx = new System.Windows.Forms.TextBox();
			this.LabelRS485TestRx = new System.Windows.Forms.Label();
			this.TextBoxRS485TestRx = new System.Windows.Forms.TextBox();
			this.LabelRS485TestTX = new System.Windows.Forms.Label();
			this.ButtonRS485TestTx = new System.Windows.Forms.Button();
			this.ButtonRS485TestRx = new System.Windows.Forms.Button();
			this.tabTMC2209 = new System.Windows.Forms.TabPage();
			this.GroupBox_Chopper_Control_Register = new System.Windows.Forms.GroupBox();
			this.GroupBox_PWM_SCALE_AUTO = new System.Windows.Forms.GroupBox();
			this.TextBox_PWM_GRAD_AUTO = new System.Windows.Forms.TextBox();
			this.Button_PWM_GRAD_AUTO = new System.Windows.Forms.Button();
			this.TextBox_PWM_OFS_AUTO = new System.Windows.Forms.TextBox();
			this.Button_PWM_OFS_AUTO = new System.Windows.Forms.Button();
			this.TextBox_PWM_SCALE_AUTO = new System.Windows.Forms.TextBox();
			this.Button_PWM_SCALE_AUTO = new System.Windows.Forms.Button();
			this.TextBox_PWM_SCALE_SUM = new System.Windows.Forms.TextBox();
			this.Button_PWM_SCALE_SUM = new System.Windows.Forms.Button();
			this.GroupBox_Chopper_PWMCONF = new System.Windows.Forms.GroupBox();
			this.GroupBox_PWMCONF = new System.Windows.Forms.GroupBox();
			this.TextBox_PWM_GRAD = new System.Windows.Forms.TextBox();
			this.TextBox_PWM_OFS = new System.Windows.Forms.TextBox();
			this.ComboBox_PWM_LIM = new System.Windows.Forms.ComboBox();
			this.Button_PWM_LIM = new System.Windows.Forms.Button();
			this.ComboBox_PWM_REG = new System.Windows.Forms.ComboBox();
			this.Button_PWM_REG = new System.Windows.Forms.Button();
			this.ComboBox_freewheel = new System.Windows.Forms.ComboBox();
			this.Button_freewheel = new System.Windows.Forms.Button();
			this.ComboBox_pwm_autograd = new System.Windows.Forms.ComboBox();
			this.Button_pwm_autograd = new System.Windows.Forms.Button();
			this.ComboBox_pwm_autoscale = new System.Windows.Forms.ComboBox();
			this.Button_pwm_autoscale = new System.Windows.Forms.Button();
			this.ComboBox_pwm_freq = new System.Windows.Forms.ComboBox();
			this.Button_pwm_freq = new System.Windows.Forms.Button();
			this.Button_PWM_GRAD = new System.Windows.Forms.Button();
			this.Button_PWM_OFS = new System.Windows.Forms.Button();
			this.GroupBox_Chopper_DRV_STATUS = new System.Windows.Forms.GroupBox();
			this.GroupBox_DRV_STATUS = new System.Windows.Forms.GroupBox();
			this.TextBox_stst = new System.Windows.Forms.TextBox();
			this.Button_stst = new System.Windows.Forms.Button();
			this.TextBox_stealth = new System.Windows.Forms.TextBox();
			this.Button_stealth = new System.Windows.Forms.Button();
			this.TextBox_CS_ACTUAL = new System.Windows.Forms.TextBox();
			this.Button_CS_ACTUAL = new System.Windows.Forms.Button();
			this.TextBox_t157 = new System.Windows.Forms.TextBox();
			this.Button_t157 = new System.Windows.Forms.Button();
			this.TextBox_t150 = new System.Windows.Forms.TextBox();
			this.Button_t150 = new System.Windows.Forms.Button();
			this.TextBox_t143 = new System.Windows.Forms.TextBox();
			this.Button_t143 = new System.Windows.Forms.Button();
			this.TextBox_t120 = new System.Windows.Forms.TextBox();
			this.Button_t120 = new System.Windows.Forms.Button();
			this.TextBox_olb = new System.Windows.Forms.TextBox();
			this.Button_olb = new System.Windows.Forms.Button();
			this.TextBox_ola = new System.Windows.Forms.TextBox();
			this.Button_ola = new System.Windows.Forms.Button();
			this.TextBox_s2vsb = new System.Windows.Forms.TextBox();
			this.Button_s2vsb = new System.Windows.Forms.Button();
			this.TextBox_s2vsa = new System.Windows.Forms.TextBox();
			this.Button_s2vsa = new System.Windows.Forms.Button();
			this.TextBox_s2gb = new System.Windows.Forms.TextBox();
			this.Button_s2gb = new System.Windows.Forms.Button();
			this.TextBox_s2ga = new System.Windows.Forms.TextBox();
			this.Button_s2ga = new System.Windows.Forms.Button();
			this.TextBox_ot = new System.Windows.Forms.TextBox();
			this.Button_ot = new System.Windows.Forms.Button();
			this.TextBox_otpw = new System.Windows.Forms.TextBox();
			this.Button_otpw = new System.Windows.Forms.Button();
			this.GroupBox_Chopper_CHOPCONF = new System.Windows.Forms.GroupBox();
			this.GroupBox_CHOPCONF = new System.Windows.Forms.GroupBox();
			this.ComboBox_diss2vs = new System.Windows.Forms.ComboBox();
			this.Button_diss2vs = new System.Windows.Forms.Button();
			this.ComboBox_diss2g = new System.Windows.Forms.ComboBox();
			this.Button_diss2g = new System.Windows.Forms.Button();
			this.ComboBox_dedge = new System.Windows.Forms.ComboBox();
			this.Button_dedge = new System.Windows.Forms.Button();
			this.ComboBox_intpol = new System.Windows.Forms.ComboBox();
			this.Button_intpol = new System.Windows.Forms.Button();
			this.ComboBox_MRES = new System.Windows.Forms.ComboBox();
			this.Button_MRES = new System.Windows.Forms.Button();
			this.ComboBox_vsense = new System.Windows.Forms.ComboBox();
			this.Button_vsense = new System.Windows.Forms.Button();
			this.ComboBox_TBL = new System.Windows.Forms.ComboBox();
			this.Button_TBL = new System.Windows.Forms.Button();
			this.ComboBox_HEND = new System.Windows.Forms.ComboBox();
			this.Button_HEND = new System.Windows.Forms.Button();
			this.ComboBox_HSTRT = new System.Windows.Forms.ComboBox();
			this.Button_HSTRT = new System.Windows.Forms.Button();
			this.ComboBox_TOFF = new System.Windows.Forms.ComboBox();
			this.Button_TOFF = new System.Windows.Forms.Button();
			this.GroupBox_STALLGARD_COOLCONF = new System.Windows.Forms.GroupBox();
			this.GroupBox_COOLCONF = new System.Windows.Forms.GroupBox();
			this.ComboBox_seimin = new System.Windows.Forms.ComboBox();
			this.Button_seimin = new System.Windows.Forms.Button();
			this.ComboBox_sedn = new System.Windows.Forms.ComboBox();
			this.Button_sedn = new System.Windows.Forms.Button();
			this.ComboBox_semax = new System.Windows.Forms.ComboBox();
			this.Button_semax = new System.Windows.Forms.Button();
			this.ComboBox_seup = new System.Windows.Forms.ComboBox();
			this.Button_seup = new System.Windows.Forms.Button();
			this.ComboBox_semin = new System.Windows.Forms.ComboBox();
			this.Button_semin = new System.Windows.Forms.Button();
			this.GroupBox_Sequencer_Registers = new System.Windows.Forms.GroupBox();
			this.GroupBox_MSCURACT = new System.Windows.Forms.GroupBox();
			this.Button_CUR_B = new System.Windows.Forms.Button();
			this.TextBox_CUR_B = new System.Windows.Forms.TextBox();
			this.TextBox_CUR_A = new System.Windows.Forms.TextBox();
			this.Button_CUR_A = new System.Windows.Forms.Button();
			this.GroupBox_MSCNT = new System.Windows.Forms.GroupBox();
			this.TextBox_MSCNT = new System.Windows.Forms.TextBox();
			this.Button_MSCNT = new System.Windows.Forms.Button();
			this.GroupBox_StallGuard_Control = new System.Windows.Forms.GroupBox();
			this.GroupBox_SG_RESULT = new System.Windows.Forms.GroupBox();
			this.TextBox_SG_RESULT = new System.Windows.Forms.TextBox();
			this.Button_SG_RESULT = new System.Windows.Forms.Button();
			this.GroupBox_SGTHRS = new System.Windows.Forms.GroupBox();
			this.Button_SGTHRS = new System.Windows.Forms.Button();
			this.TextBox_SGTHRS = new System.Windows.Forms.TextBox();
			this.GroupBox_TCOOLTHRS = new System.Windows.Forms.GroupBox();
			this.TextBox_TCOOLTHRS = new System.Windows.Forms.TextBox();
			this.Button_TCOOLTHRS = new System.Windows.Forms.Button();
			this.GroupBox_Velocity_Dependent_Control = new System.Windows.Forms.GroupBox();
			this.GroupBox_VACTUAL = new System.Windows.Forms.GroupBox();
			this.TextBox_VACTUAL = new System.Windows.Forms.TextBox();
			this.Button_VACTUAL = new System.Windows.Forms.Button();
			this.GroupBox_TPWMTHRS = new System.Windows.Forms.GroupBox();
			this.TextBox_TPWMTHRS = new System.Windows.Forms.TextBox();
			this.Button_TPWMTHRS = new System.Windows.Forms.Button();
			this.GroupBox_TSTEP = new System.Windows.Forms.GroupBox();
			this.TextBox_TSTEP = new System.Windows.Forms.TextBox();
			this.Button_TSTEP = new System.Windows.Forms.Button();
			this.GroupBox_TPOWERDOWN = new System.Windows.Forms.GroupBox();
			this.TextBox_TPOWERDOWN = new System.Windows.Forms.TextBox();
			this.Button_TPOWERDOWN = new System.Windows.Forms.Button();
			this.GroupBox_IHOLD_IRUN = new System.Windows.Forms.GroupBox();
			this.ComboBox_IHOLDDELAY = new System.Windows.Forms.ComboBox();
			this.Button_IHOLDDELAY = new System.Windows.Forms.Button();
			this.ComboBox_IRUN = new System.Windows.Forms.ComboBox();
			this.Button_IRUN = new System.Windows.Forms.Button();
			this.ComboBox_IHOLD = new System.Windows.Forms.ComboBox();
			this.Button_IHOLD = new System.Windows.Forms.Button();
			this.GroupBox_General = new System.Windows.Forms.GroupBox();
			this.GroupBox_FACTORY_CONF = new System.Windows.Forms.GroupBox();
			this.ComboBox_OTTRIM = new System.Windows.Forms.ComboBox();
			this.Button_OTTRIM = new System.Windows.Forms.Button();
			this.ComboBox_FCLKTRIM = new System.Windows.Forms.ComboBox();
			this.Button_FCLKTRIM = new System.Windows.Forms.Button();
			this.GroupBox_IOIN = new System.Windows.Forms.GroupBox();
			this.TextBox_VERSION = new System.Windows.Forms.TextBox();
			this.TextBox_DIR = new System.Windows.Forms.TextBox();
			this.TextBox_SPREAD_EN = new System.Windows.Forms.TextBox();
			this.TextBox_STEP = new System.Windows.Forms.TextBox();
			this.TextBox_PDN_UART = new System.Windows.Forms.TextBox();
			this.TextBox_DIAG = new System.Windows.Forms.TextBox();
			this.TextBox_MS2 = new System.Windows.Forms.TextBox();
			this.TextBox_MS1 = new System.Windows.Forms.TextBox();
			this.TextBox_ENN = new System.Windows.Forms.TextBox();
			this.Button_VERSION = new System.Windows.Forms.Button();
			this.Button_DIR = new System.Windows.Forms.Button();
			this.Button_SPREAD_EN = new System.Windows.Forms.Button();
			this.Button_STEP = new System.Windows.Forms.Button();
			this.Button_PDN_UART = new System.Windows.Forms.Button();
			this.Button_DIAG = new System.Windows.Forms.Button();
			this.Button_MS2 = new System.Windows.Forms.Button();
			this.Button_MS1 = new System.Windows.Forms.Button();
			this.Button_ENN = new System.Windows.Forms.Button();
			this.GroupBoxGSTAT = new System.Windows.Forms.GroupBox();
			this.ComboBox_uv_cp = new System.Windows.Forms.ComboBox();
			this.Button_uv_cp = new System.Windows.Forms.Button();
			this.ComboBox_drv_err = new System.Windows.Forms.ComboBox();
			this.Button_drv_err = new System.Windows.Forms.Button();
			this.ComboBox_reset = new System.Windows.Forms.ComboBox();
			this.Button_reset = new System.Windows.Forms.Button();
			this.GroupBox_OTP_READ = new System.Windows.Forms.GroupBox();
			this.TextBox_OTP_READ_0 = new System.Windows.Forms.TextBox();
			this.TextBox_OTP_READ_1 = new System.Windows.Forms.TextBox();
			this.TextBox_OTP_READ_2 = new System.Windows.Forms.TextBox();
			this.Button_OTP_READ = new System.Windows.Forms.Button();
			this.GroupBox_SLAVECONF = new System.Windows.Forms.GroupBox();
			this.Button_SLAVECONF = new System.Windows.Forms.Button();
			this.ComboBox_SLAVECONF = new System.Windows.Forms.ComboBox();
			this.GroupBox_IFCNT = new System.Windows.Forms.GroupBox();
			this.TextBox_IFCNT = new System.Windows.Forms.TextBox();
			this.Button_IFCNT = new System.Windows.Forms.Button();
			this.GroupBox_GCONF = new System.Windows.Forms.GroupBox();
			this.ComboBox_test_mode = new System.Windows.Forms.ComboBox();
			this.Button_test_mode = new System.Windows.Forms.Button();
			this.ComboBox_multistep_filt = new System.Windows.Forms.ComboBox();
			this.Button_multistep_filt = new System.Windows.Forms.Button();
			this.ComboBox_mstep_reg_sel = new System.Windows.Forms.ComboBox();
			this.Button_mstep_reg_sel = new System.Windows.Forms.Button();
			this.ComboBox_pdn_disable = new System.Windows.Forms.ComboBox();
			this.Button_pdn_disable = new System.Windows.Forms.Button();
			this.ComboBox_index_step = new System.Windows.Forms.ComboBox();
			this.Button_index_step = new System.Windows.Forms.Button();
			this.ComboBox_index_otpw = new System.Windows.Forms.ComboBox();
			this.Button_index_otpw = new System.Windows.Forms.Button();
			this.ComboBox_shaft = new System.Windows.Forms.ComboBox();
			this.Button_shaft = new System.Windows.Forms.Button();
			this.ComboBox_en_SpreadCycle = new System.Windows.Forms.ComboBox();
			this.Button_en_SpreadCycle = new System.Windows.Forms.Button();
			this.ComboBox_internal_Rsense = new System.Windows.Forms.ComboBox();
			this.Button_internal_Rsense = new System.Windows.Forms.Button();
			this.ComboBox_I_scale_analog = new System.Windows.Forms.ComboBox();
			this.Button_I_scale_analog = new System.Windows.Forms.Button();
			this.tabTest = new System.Windows.Forms.TabPage();
			this.ButtonICRDefog = new System.Windows.Forms.Button();
			this.ButtonICRNight = new System.Windows.Forms.Button();
			this.ButtonICRNormal = new System.Windows.Forms.Button();
			this.ButtonICRDay = new System.Windows.Forms.Button();
			this.LabelICR = new System.Windows.Forms.Label();
			this.TextBoxFocusStep = new System.Windows.Forms.TextBox();
			this.ButtonFocusStepNear = new System.Windows.Forms.Button();
			this.ButtonFocusStepFar = new System.Windows.Forms.Button();
			this.LabelFocusStep = new System.Windows.Forms.Label();
			this.TextBoxZoomStep = new System.Windows.Forms.TextBox();
			this.ButtonZoomStepTele = new System.Windows.Forms.Button();
			this.ButtonZoomStepWide = new System.Windows.Forms.Button();
			this.LabelZoomStep = new System.Windows.Forms.Label();
			this.ButtonGetFPos = new System.Windows.Forms.Button();
			this.ButtonSetFPos = new System.Windows.Forms.Button();
			this.TextBoxFPos = new System.Windows.Forms.TextBox();
			this.ButtonGetZ2Pos = new System.Windows.Forms.Button();
			this.ButtonSetZ2Pos = new System.Windows.Forms.Button();
			this.TextBoxZ2Pos = new System.Windows.Forms.TextBox();
			this.ButtonGetZ1Pos = new System.Windows.Forms.Button();
			this.ButtonFocusResetSensor = new System.Windows.Forms.Button();
			this.ButtonFocusResetWide = new System.Windows.Forms.Button();
			this.ButtonFocusResetTele = new System.Windows.Forms.Button();
			this.LabelFocusReset = new System.Windows.Forms.Label();
			this.ButtonZoomCtrlModeInd = new System.Windows.Forms.Button();
			this.ButtonZoomCtrlModeTrack = new System.Windows.Forms.Button();
			this.LabelZoomCtrlMode = new System.Windows.Forms.Label();
			this.ButtonZoomCtrlGroup2 = new System.Windows.Forms.Button();
			this.ButtonZoomCtrlGroup1 = new System.Windows.Forms.Button();
			this.LabelZoomCtrlGroup = new System.Windows.Forms.Label();
			this.ButtonZoom1ResetSensor = new System.Windows.Forms.Button();
			this.ButtonZoom1ResetWide = new System.Windows.Forms.Button();
			this.ButtonSetZ1Pos = new System.Windows.Forms.Button();
			this.TextBoxZ1Pos = new System.Windows.Forms.TextBox();
			this.ButtonFBufNear = new System.Windows.Forms.Button();
			this.ButtonZ2BufTele = new System.Windows.Forms.Button();
			this.ButtonZ1BufTele = new System.Windows.Forms.Button();
			this.ButtonFBufFar = new System.Windows.Forms.Button();
			this.ButtonZ2BufWide = new System.Windows.Forms.Button();
			this.ButtonZ1BufWide = new System.Windows.Forms.Button();
			this.LabelFBuf = new System.Windows.Forms.Label();
			this.LabelZ2Buf = new System.Windows.Forms.Label();
			this.LabelZoom1Buf = new System.Windows.Forms.Label();
			this.ButtonZoom1ResetTele = new System.Windows.Forms.Button();
			this.ButtonFocusNear = new System.Windows.Forms.Button();
			this.ButtonZoomTele = new System.Windows.Forms.Button();
			this.ButtonZoomTeleWOTrack = new System.Windows.Forms.Button();
			this.ButtonFocusFar = new System.Windows.Forms.Button();
			this.ButtonZoomWide = new System.Windows.Forms.Button();
			this.ButtonZoomWideWOTrack = new System.Windows.Forms.Button();
			this.ButtonFocusStop = new System.Windows.Forms.Button();
			this.ButtonZoomStop = new System.Windows.Forms.Button();
			this.LabelZoom1Reset = new System.Windows.Forms.Label();
			this.LabelFocus = new System.Windows.Forms.Label();
			this.LabelZoom = new System.Windows.Forms.Label();
			this.ButtonSpeedDryStop = new System.Windows.Forms.Button();
			this.ButtonSpeedDryStart = new System.Windows.Forms.Button();
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
			this.TimerUpdateAngle = new System.Windows.Forms.Timer(this.components);
			this.LabelFWVersion = new System.Windows.Forms.Label();
			this.groupBoxVersion = new System.Windows.Forms.GroupBox();
			this.TimerUpdateSpeed = new System.Windows.Forms.Timer(this.components);
			this.groupBoxMCU = new System.Windows.Forms.GroupBox();
			this.ComboBoxPTType = new System.Windows.Forms.ComboBox();
			this.LabelMCUType = new System.Windows.Forms.Label();
			this.GroupBoxSettings.SuspendLayout();
			this.tabVisca.SuspendLayout();
			this.tabMenu.SuspendLayout();
			this.groupBoxShowSpeed.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ChartSpeed)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox20.SuspendLayout();
			this.groupBox17.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox16.SuspendLayout();
			this.groupBox15.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabAK7452.SuspendLayout();
			this.groupBoxShowAngle.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBoxAK7452ManualMode.SuspendLayout();
			this.groupBox19.SuspendLayout();
			this.groupBoxAK7452ABZResolution.SuspendLayout();
			this.groupBoxAK7452ZeroDegreePoint.SuspendLayout();
			this.groupBoxAK7452RDABZ.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartAngle)).BeginInit();
			this.groupBoxAK7452NormalMode.SuspendLayout();
			this.tabTLE5012B.SuspendLayout();
			this.GroupBoxTleT_RAW.SuspendLayout();
			this.GroupBoxTleIIF_CNT.SuspendLayout();
			this.GroupBoxTleD_MAG.SuspendLayout();
			this.GroupBoxTleADC_Y.SuspendLayout();
			this.GroupBoxTleT25O.SuspendLayout();
			this.GroupBoxTleADC_X.SuspendLayout();
			this.GroupBoxTleTCO_Y.SuspendLayout();
			this.GroupBoxTleMOD_4.SuspendLayout();
			this.GroupBoxTleIFAB.SuspendLayout();
			this.GroupBoxTleSYNCH.SuspendLayout();
			this.GroupBoxTleOFFSET_Y.SuspendLayout();
			this.GroupBoxTleOFFX.SuspendLayout();
			this.GroupBoxTleMOD_3.SuspendLayout();
			this.GroupBoxTleMOD_2.SuspendLayout();
			this.GroupBoxTleSIL.SuspendLayout();
			this.GroupBoxTleMOD_1.SuspendLayout();
			this.GroupBoxTleFSYNC.SuspendLayout();
			this.GroupBoxTleAREV.SuspendLayout();
			this.GroupBoxTleASPD.SuspendLayout();
			this.GroupBoxTleAVAL.SuspendLayout();
			this.GroupBoxTleACSTAT.SuspendLayout();
			this.groupTleSTAT.SuspendLayout();
			this.tabLightSensor.SuspendLayout();
			this.groupBoxLSID.SuspendLayout();
			this.groupBoxLSLimit.SuspendLayout();
			this.groupBoxLSConfiguration.SuspendLayout();
			this.groupBoxLSResult.SuspendLayout();
			this.tabAlarm.SuspendLayout();
			this.groupBoxNTC.SuspendLayout();
			this.groupBoxRS485.SuspendLayout();
			this.groupBoxSD700Set.SuspendLayout();
			this.groupBoxRS485DO.SuspendLayout();
			this.groupBoxRS485DI.SuspendLayout();
			this.groupBoxRS485Comm.SuspendLayout();
			this.groupBoxRS485TermR.SuspendLayout();
			this.groupBoxRS485TransceiverMode.SuspendLayout();
			this.groupBoxAlarm.SuspendLayout();
			this.groupBoxAlarmInq.SuspendLayout();
			this.groupBoxAlarmOut.SuspendLayout();
			this.groupBoxAlarmAuto.SuspendLayout();
			this.groupBoxRS485Test.SuspendLayout();
			this.tabTMC2209.SuspendLayout();
			this.GroupBox_Chopper_Control_Register.SuspendLayout();
			this.GroupBox_PWM_SCALE_AUTO.SuspendLayout();
			this.GroupBox_Chopper_PWMCONF.SuspendLayout();
			this.GroupBox_PWMCONF.SuspendLayout();
			this.GroupBox_Chopper_DRV_STATUS.SuspendLayout();
			this.GroupBox_DRV_STATUS.SuspendLayout();
			this.GroupBox_Chopper_CHOPCONF.SuspendLayout();
			this.GroupBox_CHOPCONF.SuspendLayout();
			this.GroupBox_STALLGARD_COOLCONF.SuspendLayout();
			this.GroupBox_COOLCONF.SuspendLayout();
			this.GroupBox_Sequencer_Registers.SuspendLayout();
			this.GroupBox_MSCURACT.SuspendLayout();
			this.GroupBox_MSCNT.SuspendLayout();
			this.GroupBox_StallGuard_Control.SuspendLayout();
			this.GroupBox_SG_RESULT.SuspendLayout();
			this.GroupBox_SGTHRS.SuspendLayout();
			this.GroupBox_TCOOLTHRS.SuspendLayout();
			this.GroupBox_Velocity_Dependent_Control.SuspendLayout();
			this.GroupBox_VACTUAL.SuspendLayout();
			this.GroupBox_TPWMTHRS.SuspendLayout();
			this.GroupBox_TSTEP.SuspendLayout();
			this.GroupBox_TPOWERDOWN.SuspendLayout();
			this.GroupBox_IHOLD_IRUN.SuspendLayout();
			this.GroupBox_General.SuspendLayout();
			this.GroupBox_FACTORY_CONF.SuspendLayout();
			this.GroupBox_IOIN.SuspendLayout();
			this.GroupBoxGSTAT.SuspendLayout();
			this.GroupBox_OTP_READ.SuspendLayout();
			this.GroupBox_SLAVECONF.SuspendLayout();
			this.GroupBox_IFCNT.SuspendLayout();
			this.GroupBox_GCONF.SuspendLayout();
			this.tabTest.SuspendLayout();
			this.VISCABox.SuspendLayout();
			this.groupBoxVersion.SuspendLayout();
			this.groupBoxMCU.SuspendLayout();
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
			this.CmdOpen.Text = "On";
			this.CmdOpen.Click += new System.EventHandler(this.CmdOpen_Click);
			// 
			// TxData
			// 
			this.TxData.AcceptsReturn = true;
			this.TxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TxData.Location = new System.Drawing.Point(16, 482);
			this.TxData.Multiline = true;
			this.TxData.Name = "TxData";
			this.TxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.TxData.Size = new System.Drawing.Size(340, 114);
			this.TxData.TabIndex = 39;
			// 
			// LblTxData
			// 
			this.LblTxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LblTxData.ForeColor = System.Drawing.Color.Teal;
			this.LblTxData.Location = new System.Drawing.Point(15, 466);
			this.LblTxData.Name = "LblTxData";
			this.LblTxData.Size = new System.Drawing.Size(341, 13);
			this.LblTxData.TabIndex = 38;
			this.LblTxData.Text = "Tx";
			this.LblTxData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// RxData
			// 
			this.RxData.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RxData.Location = new System.Drawing.Point(362, 482);
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
			this.LblRxData.Location = new System.Drawing.Point(362, 465);
			this.LblRxData.Name = "LblRxData";
			this.LblRxData.Size = new System.Drawing.Size(340, 14);
			this.LblRxData.TabIndex = 38;
			this.LblRxData.Text = "Rx";
			this.LblRxData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CmdConfig
			// 
			this.CmdConfig.Location = new System.Drawing.Point(288, 13);
			this.CmdConfig.Name = "CmdConfig";
			this.CmdConfig.Size = new System.Drawing.Size(68, 22);
			this.CmdConfig.TabIndex = 4;
			this.CmdConfig.Text = "Configure";
			this.CmdConfig.Visible = false;
			this.CmdConfig.Click += new System.EventHandler(this.CmdConfig_Click);
			// 
			// CmdClear
			// 
			this.CmdClear.Location = new System.Drawing.Point(708, 481);
			this.CmdClear.Name = "CmdClear";
			this.CmdClear.Size = new System.Drawing.Size(62, 114);
			this.CmdClear.TabIndex = 62;
			this.CmdClear.Text = "Clear";
			this.CmdClear.Click += new System.EventHandler(this.CmdClear_Click);
			// 
			// RecvTimer
			// 
			this.RecvTimer.Tick += new System.EventHandler(this.RecvTimer_Tick);
			// 
			// GroupBoxSettings
			// 
			this.GroupBoxSettings.Controls.Add(this.CmdConfig);
			this.GroupBoxSettings.Controls.Add(this.Settings);
			this.GroupBoxSettings.Controls.Add(this.LblSettings);
			this.GroupBoxSettings.Controls.Add(this.ComPort);
			this.GroupBoxSettings.Controls.Add(this.LblPort);
			this.GroupBoxSettings.Location = new System.Drawing.Point(103, 0);
			this.GroupBoxSettings.Name = "GroupBoxSettings";
			this.GroupBoxSettings.Size = new System.Drawing.Size(283, 48);
			this.GroupBoxSettings.TabIndex = 66;
			this.GroupBoxSettings.TabStop = false;
			// 
			// tabVisca
			// 
			this.tabVisca.Controls.Add(this.tabMenu);
			this.tabVisca.Controls.Add(this.tabAK7452);
			this.tabVisca.Controls.Add(this.tabTLE5012B);
			this.tabVisca.Controls.Add(this.tabLightSensor);
			this.tabVisca.Controls.Add(this.tabAlarm);
			this.tabVisca.Controls.Add(this.tabTMC2209);
			this.tabVisca.Controls.Add(this.tabTest);
			this.tabVisca.Location = new System.Drawing.Point(8, 54);
			this.tabVisca.Name = "tabVisca";
			this.tabVisca.SelectedIndex = 0;
			this.tabVisca.Size = new System.Drawing.Size(1033, 408);
			this.tabVisca.TabIndex = 5;
			// 
			// tabMenu
			// 
			this.tabMenu.Controls.Add(this.groupBoxShowSpeed);
			this.tabMenu.Controls.Add(this.ChartSpeed);
			this.tabMenu.Controls.Add(this.groupBox1);
			this.tabMenu.Location = new System.Drawing.Point(4, 22);
			this.tabMenu.Name = "tabMenu";
			this.tabMenu.Padding = new System.Windows.Forms.Padding(3);
			this.tabMenu.Size = new System.Drawing.Size(1025, 382);
			this.tabMenu.TabIndex = 1;
			this.tabMenu.Text = "Main";
			this.tabMenu.UseVisualStyleBackColor = true;
			// 
			// groupBoxShowSpeed
			// 
			this.groupBoxShowSpeed.Controls.Add(this.ButtonShowSpeed);
			this.groupBoxShowSpeed.Controls.Add(this.ButtonClearShowSpeed);
			this.groupBoxShowSpeed.Controls.Add(this.ButtonStopShowSpeed);
			this.groupBoxShowSpeed.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxShowSpeed.Location = new System.Drawing.Point(910, 40);
			this.groupBoxShowSpeed.Name = "groupBoxShowSpeed";
			this.groupBoxShowSpeed.Size = new System.Drawing.Size(100, 86);
			this.groupBoxShowSpeed.TabIndex = 43;
			this.groupBoxShowSpeed.TabStop = false;
			this.groupBoxShowSpeed.Text = "Show Speed";
			// 
			// ButtonShowSpeed
			// 
			this.ButtonShowSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonShowSpeed.Location = new System.Drawing.Point(7, 16);
			this.ButtonShowSpeed.Name = "ButtonShowSpeed";
			this.ButtonShowSpeed.Size = new System.Drawing.Size(87, 22);
			this.ButtonShowSpeed.TabIndex = 31;
			this.ButtonShowSpeed.Text = "Start";
			this.ButtonShowSpeed.Click += new System.EventHandler(this.ButtonShowSpeed_Click);
			// 
			// ButtonClearShowSpeed
			// 
			this.ButtonClearShowSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonClearShowSpeed.Location = new System.Drawing.Point(7, 58);
			this.ButtonClearShowSpeed.Name = "ButtonClearShowSpeed";
			this.ButtonClearShowSpeed.Size = new System.Drawing.Size(87, 22);
			this.ButtonClearShowSpeed.TabIndex = 33;
			this.ButtonClearShowSpeed.Text = "Clear Chart";
			this.ButtonClearShowSpeed.Click += new System.EventHandler(this.ButtonClearShowSpeed_Click);
			// 
			// ButtonStopShowSpeed
			// 
			this.ButtonStopShowSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonStopShowSpeed.Location = new System.Drawing.Point(7, 37);
			this.ButtonStopShowSpeed.Name = "ButtonStopShowSpeed";
			this.ButtonStopShowSpeed.Size = new System.Drawing.Size(87, 22);
			this.ButtonStopShowSpeed.TabIndex = 32;
			this.ButtonStopShowSpeed.Text = "Stop";
			this.ButtonStopShowSpeed.Click += new System.EventHandler(this.ButtonStopShowSpeed_Click);
			// 
			// ChartSpeed
			// 
			chartArea1.AxisX.Interval = 1D;
			chartArea1.AxisX.LabelStyle.Format = "N0";
			chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisX.ScrollBar.BackColor = System.Drawing.Color.White;
			chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.White;
			chartArea1.AxisX.Title = "s";
			chartArea1.AxisY.Minimum = 0D;
			chartArea1.Name = "ChartArea1";
			this.ChartSpeed.ChartAreas.Add(chartArea1);
			legend1.Name = "Legend1";
			this.ChartSpeed.Legends.Add(legend1);
			this.ChartSpeed.Location = new System.Drawing.Point(398, 3);
			this.ChartSpeed.Name = "ChartSpeed";
			series1.BorderWidth = 3;
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.Legend = "Legend1";
			series1.Name = "V (pps)";
			this.ChartSpeed.Series.Add(series1);
			this.ChartSpeed.Size = new System.Drawing.Size(624, 350);
			this.ChartSpeed.TabIndex = 30;
			this.ChartSpeed.Text = "chart1";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ButtonHome_2);
			this.groupBox1.Controls.Add(this.groupBox11);
			this.groupBox1.Controls.Add(this.groupBox7);
			this.groupBox1.Controls.Add(this.groupBox20);
			this.groupBox1.Controls.Add(this.groupBox17);
			this.groupBox1.Controls.Add(this.groupBox9);
			this.groupBox1.Controls.Add(this.groupBox5);
			this.groupBox1.Controls.Add(this.groupBox4);
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(6, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(386, 352);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pan/Tilt Control";
			// 
			// ButtonHome_2
			// 
			this.ButtonHome_2.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonHome_2.Location = new System.Drawing.Point(112, 320);
			this.ButtonHome_2.Name = "ButtonHome_2";
			this.ButtonHome_2.Size = new System.Drawing.Size(68, 22);
			this.ButtonHome_2.TabIndex = 44;
			this.ButtonHome_2.Text = "Home";
			this.ButtonHome_2.Click += new System.EventHandler(this.ButtonHome_2_Click);
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.ButtonStallCaliOn);
			this.groupBox11.Controls.Add(this.ButtonStallCaliOff);
			this.groupBox11.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox11.Location = new System.Drawing.Point(6, 305);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(89, 43);
			this.groupBox11.TabIndex = 43;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Stall Cali.";
			// 
			// ButtonStallCaliOn
			// 
			this.ButtonStallCaliOn.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonStallCaliOn.Location = new System.Drawing.Point(2, 15);
			this.ButtonStallCaliOn.Name = "ButtonStallCaliOn";
			this.ButtonStallCaliOn.Size = new System.Drawing.Size(43, 22);
			this.ButtonStallCaliOn.TabIndex = 39;
			this.ButtonStallCaliOn.Text = "On";
			this.ButtonStallCaliOn.Click += new System.EventHandler(this.ButtonStallCaliOn_Click);
			// 
			// ButtonStallCaliOff
			// 
			this.ButtonStallCaliOff.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonStallCaliOff.Location = new System.Drawing.Point(44, 15);
			this.ButtonStallCaliOff.Name = "ButtonStallCaliOff";
			this.ButtonStallCaliOff.Size = new System.Drawing.Size(43, 22);
			this.ButtonStallCaliOff.TabIndex = 40;
			this.ButtonStallCaliOff.Text = "Off";
			this.ButtonStallCaliOff.Click += new System.EventHandler(this.ButtonStallCaliOff_Click);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.ButtonMotorType0p9d);
			this.groupBox7.Controls.Add(this.ButtonMotorType1p8d);
			this.groupBox7.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox7.Location = new System.Drawing.Point(292, 305);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(89, 43);
			this.groupBox7.TabIndex = 43;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Motor Type";
			// 
			// ButtonMotorType0p9d
			// 
			this.ButtonMotorType0p9d.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonMotorType0p9d.Location = new System.Drawing.Point(2, 15);
			this.ButtonMotorType0p9d.Name = "ButtonMotorType0p9d";
			this.ButtonMotorType0p9d.Size = new System.Drawing.Size(43, 22);
			this.ButtonMotorType0p9d.TabIndex = 39;
			this.ButtonMotorType0p9d.Text = "0.9¢X";
			this.ButtonMotorType0p9d.Click += new System.EventHandler(this.ButtonMotorType0p9d_Click);
			// 
			// ButtonMotorType1p8d
			// 
			this.ButtonMotorType1p8d.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonMotorType1p8d.Location = new System.Drawing.Point(44, 15);
			this.ButtonMotorType1p8d.Name = "ButtonMotorType1p8d";
			this.ButtonMotorType1p8d.Size = new System.Drawing.Size(43, 22);
			this.ButtonMotorType1p8d.TabIndex = 40;
			this.ButtonMotorType1p8d.Text = "1.8¢X";
			this.ButtonMotorType1p8d.Click += new System.EventHandler(this.ButtonMotorType1p8d_Click);
			// 
			// groupBox20
			// 
			this.groupBox20.Controls.Add(this.ButtonImageFlipMaxAngleOn);
			this.groupBox20.Controls.Add(this.ButtonImageFlipMaxAngleOff);
			this.groupBox20.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox20.Location = new System.Drawing.Point(201, 305);
			this.groupBox20.Name = "groupBox20";
			this.groupBox20.Size = new System.Drawing.Size(89, 43);
			this.groupBox20.TabIndex = 42;
			this.groupBox20.TabStop = false;
			this.groupBox20.Text = "Max Angle";
			// 
			// ButtonImageFlipMaxAngleOn
			// 
			this.ButtonImageFlipMaxAngleOn.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonImageFlipMaxAngleOn.Location = new System.Drawing.Point(2, 15);
			this.ButtonImageFlipMaxAngleOn.Name = "ButtonImageFlipMaxAngleOn";
			this.ButtonImageFlipMaxAngleOn.Size = new System.Drawing.Size(43, 22);
			this.ButtonImageFlipMaxAngleOn.TabIndex = 39;
			this.ButtonImageFlipMaxAngleOn.Text = "On";
			this.ButtonImageFlipMaxAngleOn.Click += new System.EventHandler(this.ButtonImageFlipMaxAngleOn_Click);
			// 
			// ButtonImageFlipMaxAngleOff
			// 
			this.ButtonImageFlipMaxAngleOff.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonImageFlipMaxAngleOff.Location = new System.Drawing.Point(44, 15);
			this.ButtonImageFlipMaxAngleOff.Name = "ButtonImageFlipMaxAngleOff";
			this.ButtonImageFlipMaxAngleOff.Size = new System.Drawing.Size(43, 22);
			this.ButtonImageFlipMaxAngleOff.TabIndex = 40;
			this.ButtonImageFlipMaxAngleOff.Text = "Off";
			this.ButtonImageFlipMaxAngleOff.Click += new System.EventHandler(this.ButtonImageFlipMaxAngleOff_Click);
			// 
			// groupBox17
			// 
			this.groupBox17.Controls.Add(this.label12);
			this.groupBox17.Controls.Add(this.ButtonRelDown);
			this.groupBox17.Controls.Add(this.ButtonRelUp);
			this.groupBox17.Controls.Add(this.TextBoxRelStep);
			this.groupBox17.Controls.Add(this.ButtonRelLeft);
			this.groupBox17.Controls.Add(this.ButtonRelStop);
			this.groupBox17.Controls.Add(this.ButtonRelRight);
			this.groupBox17.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox17.Location = new System.Drawing.Point(6, 222);
			this.groupBox17.Name = "groupBox17";
			this.groupBox17.Size = new System.Drawing.Size(189, 84);
			this.groupBox17.TabIndex = 43;
			this.groupBox17.TabStop = false;
			this.groupBox17.Text = "Relative Move";
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(131, 15);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(34, 25);
			this.label12.TabIndex = 39;
			this.label12.Text = "Step";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonRelDown
			// 
			this.ButtonRelDown.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRelDown.Location = new System.Drawing.Point(46, 58);
			this.ButtonRelDown.Name = "ButtonRelDown";
			this.ButtonRelDown.Size = new System.Drawing.Size(39, 22);
			this.ButtonRelDown.TabIndex = 24;
			this.ButtonRelDown.Text = "Down";
			this.ButtonRelDown.Click += new System.EventHandler(this.ButtonRelDown_Click);
			// 
			// ButtonRelUp
			// 
			this.ButtonRelUp.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRelUp.Location = new System.Drawing.Point(46, 16);
			this.ButtonRelUp.Name = "ButtonRelUp";
			this.ButtonRelUp.Size = new System.Drawing.Size(39, 22);
			this.ButtonRelUp.TabIndex = 22;
			this.ButtonRelUp.Text = "Up";
			this.ButtonRelUp.Click += new System.EventHandler(this.ButtonRelUp_Click);
			// 
			// TextBoxRelStep
			// 
			this.TextBoxRelStep.Location = new System.Drawing.Point(128, 40);
			this.TextBoxRelStep.MaxLength = 10;
			this.TextBoxRelStep.Name = "TextBoxRelStep";
			this.TextBoxRelStep.Size = new System.Drawing.Size(46, 21);
			this.TextBoxRelStep.TabIndex = 21;
			this.TextBoxRelStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonRelLeft
			// 
			this.ButtonRelLeft.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRelLeft.Location = new System.Drawing.Point(8, 37);
			this.ButtonRelLeft.Name = "ButtonRelLeft";
			this.ButtonRelLeft.Size = new System.Drawing.Size(39, 22);
			this.ButtonRelLeft.TabIndex = 23;
			this.ButtonRelLeft.Text = "Left";
			this.ButtonRelLeft.Click += new System.EventHandler(this.ButtonRelLeft_Click);
			// 
			// ButtonRelStop
			// 
			this.ButtonRelStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRelStop.Location = new System.Drawing.Point(46, 37);
			this.ButtonRelStop.Name = "ButtonRelStop";
			this.ButtonRelStop.Size = new System.Drawing.Size(39, 22);
			this.ButtonRelStop.TabIndex = 26;
			this.ButtonRelStop.Text = "Stop";
			this.ButtonRelStop.Click += new System.EventHandler(this.ButtonRelStop_Click);
			// 
			// ButtonRelRight
			// 
			this.ButtonRelRight.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRelRight.Location = new System.Drawing.Point(84, 37);
			this.ButtonRelRight.Name = "ButtonRelRight";
			this.ButtonRelRight.Size = new System.Drawing.Size(39, 22);
			this.ButtonRelRight.TabIndex = 25;
			this.ButtonRelRight.Text = "Right";
			this.ButtonRelRight.Click += new System.EventHandler(this.ButtonRelRight_Click);
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.TextBoxMotorAngle);
			this.groupBox9.Controls.Add(this.ButtonGetAngle);
			this.groupBox9.Controls.Add(this.textBoxZCount);
			this.groupBox9.Controls.Add(this.TextBoxMotorPosition);
			this.groupBox9.Controls.Add(this.ButtonGetPosition);
			this.groupBox9.Controls.Add(this.ButtonZCount);
			this.groupBox9.Controls.Add(this.ButtonABCount);
			this.groupBox9.Controls.Add(this.textBoxABCount);
			this.groupBox9.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox9.Location = new System.Drawing.Point(201, 240);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(180, 66);
			this.groupBox9.TabIndex = 35;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Position";
			// 
			// TextBoxMotorAngle
			// 
			this.TextBoxMotorAngle.Location = new System.Drawing.Point(59, 40);
			this.TextBoxMotorAngle.MaxLength = 10;
			this.TextBoxMotorAngle.Name = "TextBoxMotorAngle";
			this.TextBoxMotorAngle.ReadOnly = true;
			this.TextBoxMotorAngle.Size = new System.Drawing.Size(42, 21);
			this.TextBoxMotorAngle.TabIndex = 35;
			this.TextBoxMotorAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonGetAngle
			// 
			this.ButtonGetAngle.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetAngle.Location = new System.Drawing.Point(7, 39);
			this.ButtonGetAngle.Name = "ButtonGetAngle";
			this.ButtonGetAngle.Size = new System.Drawing.Size(51, 22);
			this.ButtonGetAngle.TabIndex = 36;
			this.ButtonGetAngle.Text = "Angle";
			this.ButtonGetAngle.Click += new System.EventHandler(this.ButtonGetAngle_Click);
			// 
			// textBoxZCount
			// 
			this.textBoxZCount.Location = new System.Drawing.Point(132, 40);
			this.textBoxZCount.MaxLength = 10;
			this.textBoxZCount.Name = "textBoxZCount";
			this.textBoxZCount.ReadOnly = true;
			this.textBoxZCount.Size = new System.Drawing.Size(43, 21);
			this.textBoxZCount.TabIndex = 40;
			this.textBoxZCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxMotorPosition
			// 
			this.TextBoxMotorPosition.Location = new System.Drawing.Point(59, 15);
			this.TextBoxMotorPosition.MaxLength = 10;
			this.TextBoxMotorPosition.Name = "TextBoxMotorPosition";
			this.TextBoxMotorPosition.ReadOnly = true;
			this.TextBoxMotorPosition.Size = new System.Drawing.Size(42, 21);
			this.TextBoxMotorPosition.TabIndex = 20;
			this.TextBoxMotorPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonGetPosition
			// 
			this.ButtonGetPosition.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetPosition.Location = new System.Drawing.Point(7, 15);
			this.ButtonGetPosition.Name = "ButtonGetPosition";
			this.ButtonGetPosition.Size = new System.Drawing.Size(51, 22);
			this.ButtonGetPosition.TabIndex = 34;
			this.ButtonGetPosition.Text = "Position";
			this.ButtonGetPosition.Click += new System.EventHandler(this.ButtonGetPosition_Click);
			// 
			// ButtonZCount
			// 
			this.ButtonZCount.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZCount.Location = new System.Drawing.Point(102, 39);
			this.ButtonZCount.Name = "ButtonZCount";
			this.ButtonZCount.Size = new System.Drawing.Size(29, 22);
			this.ButtonZCount.TabIndex = 39;
			this.ButtonZCount.Text = "Z";
			this.ButtonZCount.Click += new System.EventHandler(this.ButtonZCount_Click);
			// 
			// ButtonABCount
			// 
			this.ButtonABCount.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABCount.Location = new System.Drawing.Point(102, 15);
			this.ButtonABCount.Name = "ButtonABCount";
			this.ButtonABCount.Size = new System.Drawing.Size(29, 22);
			this.ButtonABCount.TabIndex = 37;
			this.ButtonABCount.Text = "AB";
			this.ButtonABCount.Click += new System.EventHandler(this.ButtonABCount_Click);
			// 
			// textBoxABCount
			// 
			this.textBoxABCount.Location = new System.Drawing.Point(132, 16);
			this.textBoxABCount.MaxLength = 10;
			this.textBoxABCount.Name = "textBoxABCount";
			this.textBoxABCount.ReadOnly = true;
			this.textBoxABCount.Size = new System.Drawing.Size(43, 21);
			this.textBoxABCount.TabIndex = 38;
			this.textBoxABCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.ButtonABSAngleStop);
			this.groupBox5.Controls.Add(this.groupBox8);
			this.groupBox5.Controls.Add(this.TextBoxABSAngle2);
			this.groupBox5.Controls.Add(this.ButtonABSStop);
			this.groupBox5.Controls.Add(this.ButtonABSAngle2);
			this.groupBox5.Controls.Add(this.TextBoxABS2Pos);
			this.groupBox5.Controls.Add(this.TextBoxABSAngle);
			this.groupBox5.Controls.Add(this.ButtonABS2);
			this.groupBox5.Controls.Add(this.ButtonABSAngle);
			this.groupBox5.Controls.Add(this.TextBoxABSPos);
			this.groupBox5.Controls.Add(this.ButtonABS);
			this.groupBox5.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox5.Location = new System.Drawing.Point(6, 95);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(189, 128);
			this.groupBox5.TabIndex = 35;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Absolute Move";
			// 
			// ButtonABSAngleStop
			// 
			this.ButtonABSAngleStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABSAngleStop.Location = new System.Drawing.Point(97, 100);
			this.ButtonABSAngleStop.Name = "ButtonABSAngleStop";
			this.ButtonABSAngleStop.Size = new System.Drawing.Size(86, 22);
			this.ButtonABSAngleStop.TabIndex = 40;
			this.ButtonABSAngleStop.Text = "ABS Angle Stop";
			this.ButtonABSAngleStop.Click += new System.EventHandler(this.ButtonABSAngleStop_Click);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.ButtonPanType);
			this.groupBox8.Controls.Add(this.ComboBoxPanMethod);
			this.groupBox8.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox8.Location = new System.Drawing.Point(97, 33);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(87, 66);
			this.groupBox8.TabIndex = 37;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Pan Direction";
			// 
			// ButtonPanType
			// 
			this.ButtonPanType.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonPanType.Location = new System.Drawing.Point(6, 16);
			this.ButtonPanType.Name = "ButtonPanType";
			this.ButtonPanType.Size = new System.Drawing.Size(78, 22);
			this.ButtonPanType.TabIndex = 41;
			this.ButtonPanType.Text = "Get Pan Type";
			this.ButtonPanType.Click += new System.EventHandler(this.ButtonPanType_Click);
			// 
			// ComboBoxPanMethod
			// 
			this.ComboBoxPanMethod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxPanMethod.FormattingEnabled = true;
			this.ComboBoxPanMethod.Items.AddRange(new object[] {
            "Shorter Path",
            "Forced CW",
            "Forced CCW"});
			this.ComboBoxPanMethod.Location = new System.Drawing.Point(6, 39);
			this.ComboBoxPanMethod.Name = "ComboBoxPanMethod";
			this.ComboBoxPanMethod.Size = new System.Drawing.Size(78, 24);
			this.ComboBoxPanMethod.TabIndex = 36;
			this.ComboBoxPanMethod.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPanMethod_SelectedIndexChanged);
			// 
			// TextBoxABSAngle2
			// 
			this.TextBoxABSAngle2.Location = new System.Drawing.Point(51, 102);
			this.TextBoxABSAngle2.MaxLength = 10;
			this.TextBoxABSAngle2.Name = "TextBoxABSAngle2";
			this.TextBoxABSAngle2.Size = new System.Drawing.Size(44, 21);
			this.TextBoxABSAngle2.TabIndex = 38;
			this.TextBoxABSAngle2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonABSStop
			// 
			this.ButtonABSStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABSStop.Location = new System.Drawing.Point(97, 11);
			this.ButtonABSStop.Name = "ButtonABSStop";
			this.ButtonABSStop.Size = new System.Drawing.Size(86, 22);
			this.ButtonABSStop.TabIndex = 40;
			this.ButtonABSStop.Text = "ABS Stop";
			this.ButtonABSStop.Click += new System.EventHandler(this.ButtonABSStop_Click);
			// 
			// ButtonABSAngle2
			// 
			this.ButtonABSAngle2.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABSAngle2.Location = new System.Drawing.Point(8, 102);
			this.ButtonABSAngle2.Name = "ButtonABSAngle2";
			this.ButtonABSAngle2.Size = new System.Drawing.Size(43, 22);
			this.ButtonABSAngle2.TabIndex = 39;
			this.ButtonABSAngle2.Text = "Angle2";
			this.ButtonABSAngle2.Click += new System.EventHandler(this.ButtonABSAngle2_Click);
			// 
			// TextBoxABS2Pos
			// 
			this.TextBoxABS2Pos.Location = new System.Drawing.Point(51, 47);
			this.TextBoxABS2Pos.MaxLength = 10;
			this.TextBoxABS2Pos.Name = "TextBoxABS2Pos";
			this.TextBoxABS2Pos.Size = new System.Drawing.Size(44, 21);
			this.TextBoxABS2Pos.TabIndex = 38;
			this.TextBoxABS2Pos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxABSAngle
			// 
			this.TextBoxABSAngle.Location = new System.Drawing.Point(51, 80);
			this.TextBoxABSAngle.MaxLength = 10;
			this.TextBoxABSAngle.Name = "TextBoxABSAngle";
			this.TextBoxABSAngle.Size = new System.Drawing.Size(44, 21);
			this.TextBoxABSAngle.TabIndex = 21;
			this.TextBoxABSAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonABS2
			// 
			this.ButtonABS2.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABS2.Location = new System.Drawing.Point(8, 47);
			this.ButtonABS2.Name = "ButtonABS2";
			this.ButtonABS2.Size = new System.Drawing.Size(43, 22);
			this.ButtonABS2.TabIndex = 39;
			this.ButtonABS2.Text = "ABS2";
			this.ButtonABS2.Click += new System.EventHandler(this.ButtonABS2_Click);
			// 
			// ButtonABSAngle
			// 
			this.ButtonABSAngle.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABSAngle.Location = new System.Drawing.Point(8, 79);
			this.ButtonABSAngle.Name = "ButtonABSAngle";
			this.ButtonABSAngle.Size = new System.Drawing.Size(43, 22);
			this.ButtonABSAngle.TabIndex = 21;
			this.ButtonABSAngle.Text = "Angle";
			this.ButtonABSAngle.Click += new System.EventHandler(this.ButtonABSAngle_Click);
			// 
			// TextBoxABSPos
			// 
			this.TextBoxABSPos.Location = new System.Drawing.Point(51, 25);
			this.TextBoxABSPos.MaxLength = 10;
			this.TextBoxABSPos.Name = "TextBoxABSPos";
			this.TextBoxABSPos.Size = new System.Drawing.Size(44, 21);
			this.TextBoxABSPos.TabIndex = 21;
			this.TextBoxABSPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonABS
			// 
			this.ButtonABS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonABS.Location = new System.Drawing.Point(8, 24);
			this.ButtonABS.Name = "ButtonABS";
			this.ButtonABS.Size = new System.Drawing.Size(43, 22);
			this.ButtonABS.TabIndex = 21;
			this.ButtonABS.Text = "ABS";
			this.ButtonABS.Click += new System.EventHandler(this.ButtonABS_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.TextBoxStopAt);
			this.groupBox4.Controls.Add(this.ButtonStopAt);
			this.groupBox4.Controls.Add(this.CheckBoxMoveStop);
			this.groupBox4.Controls.Add(this.ButtonTiltDown);
			this.groupBox4.Controls.Add(this.ButtonTiltUp);
			this.groupBox4.Controls.Add(this.ButtonPanLeft);
			this.groupBox4.Controls.Add(this.ButtonPanRight);
			this.groupBox4.Controls.Add(this.ButtonPanStop);
			this.groupBox4.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox4.Location = new System.Drawing.Point(6, 12);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(189, 84);
			this.groupBox4.TabIndex = 34;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Continuous Move";
			// 
			// TextBoxStopAt
			// 
			this.TextBoxStopAt.Location = new System.Drawing.Point(134, 57);
			this.TextBoxStopAt.MaxLength = 10;
			this.TextBoxStopAt.Name = "TextBoxStopAt";
			this.TextBoxStopAt.Size = new System.Drawing.Size(44, 21);
			this.TextBoxStopAt.TabIndex = 21;
			this.TextBoxStopAt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonStopAt
			// 
			this.ButtonStopAt.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonStopAt.Location = new System.Drawing.Point(134, 32);
			this.ButtonStopAt.Name = "ButtonStopAt";
			this.ButtonStopAt.Size = new System.Drawing.Size(44, 22);
			this.ButtonStopAt.TabIndex = 21;
			this.ButtonStopAt.Text = "Stop At";
			this.ButtonStopAt.Click += new System.EventHandler(this.ButtonStopAt_Click);
			// 
			// CheckBoxMoveStop
			// 
			this.CheckBoxMoveStop.AutoSize = true;
			this.CheckBoxMoveStop.Location = new System.Drawing.Point(101, 10);
			this.CheckBoxMoveStop.Name = "CheckBoxMoveStop";
			this.CheckBoxMoveStop.Size = new System.Drawing.Size(77, 20);
			this.CheckBoxMoveStop.TabIndex = 20;
			this.CheckBoxMoveStop.Text = "Move+Stop";
			this.CheckBoxMoveStop.UseVisualStyleBackColor = true;
			// 
			// ButtonTiltDown
			// 
			this.ButtonTiltDown.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTiltDown.Location = new System.Drawing.Point(46, 58);
			this.ButtonTiltDown.Name = "ButtonTiltDown";
			this.ButtonTiltDown.Size = new System.Drawing.Size(39, 22);
			this.ButtonTiltDown.TabIndex = 13;
			this.ButtonTiltDown.Text = "Down";
			this.ButtonTiltDown.Click += new System.EventHandler(this.ButtonTiltDown_Click);
			this.ButtonTiltDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonTiltDown_MouseDown);
			this.ButtonTiltDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonTiltDown_MouseUp);
			// 
			// ButtonTiltUp
			// 
			this.ButtonTiltUp.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTiltUp.Location = new System.Drawing.Point(46, 16);
			this.ButtonTiltUp.Name = "ButtonTiltUp";
			this.ButtonTiltUp.Size = new System.Drawing.Size(39, 22);
			this.ButtonTiltUp.TabIndex = 7;
			this.ButtonTiltUp.Text = "Up";
			this.ButtonTiltUp.Click += new System.EventHandler(this.ButtonTiltUp_Click);
			this.ButtonTiltUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonTiltUp_MouseDown);
			this.ButtonTiltUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonTiltUp_MouseUp);
			// 
			// ButtonPanLeft
			// 
			this.ButtonPanLeft.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonPanLeft.Location = new System.Drawing.Point(8, 37);
			this.ButtonPanLeft.Name = "ButtonPanLeft";
			this.ButtonPanLeft.Size = new System.Drawing.Size(39, 22);
			this.ButtonPanLeft.TabIndex = 7;
			this.ButtonPanLeft.Text = "Left";
			this.ButtonPanLeft.Click += new System.EventHandler(this.ButtonPanLeft_Click);
			this.ButtonPanLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonPanLeft_MouseDown);
			this.ButtonPanLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonPanLeft_MouseUp);
			// 
			// ButtonPanRight
			// 
			this.ButtonPanRight.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonPanRight.Location = new System.Drawing.Point(84, 37);
			this.ButtonPanRight.Name = "ButtonPanRight";
			this.ButtonPanRight.Size = new System.Drawing.Size(39, 22);
			this.ButtonPanRight.TabIndex = 13;
			this.ButtonPanRight.Text = "Right";
			this.ButtonPanRight.Click += new System.EventHandler(this.ButtonPanRight_Click);
			this.ButtonPanRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonPanRight_MouseDown);
			this.ButtonPanRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonPanRight_MouseUp);
			// 
			// ButtonPanStop
			// 
			this.ButtonPanStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonPanStop.Location = new System.Drawing.Point(46, 37);
			this.ButtonPanStop.Name = "ButtonPanStop";
			this.ButtonPanStop.Size = new System.Drawing.Size(39, 22);
			this.ButtonPanStop.TabIndex = 17;
			this.ButtonPanStop.Text = "Stop";
			this.ButtonPanStop.Click += new System.EventHandler(this.ButtonPanStop_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.groupBox16);
			this.groupBox3.Controls.Add(this.groupBox15);
			this.groupBox3.Controls.Add(this.groupBox10);
			this.groupBox3.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(201, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(180, 172);
			this.groupBox3.TabIndex = 33;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Speed Control";
			// 
			// groupBox16
			// 
			this.groupBox16.Controls.Add(this.ButtonSpeedByZoomOff);
			this.groupBox16.Controls.Add(this.ButtonGetSpeedByZoomRatio);
			this.groupBox16.Controls.Add(this.TextBoxSpeedByZoomRatio);
			this.groupBox16.Controls.Add(this.ButtonSpeedByZoomOn);
			this.groupBox16.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox16.Location = new System.Drawing.Point(6, 57);
			this.groupBox16.Name = "groupBox16";
			this.groupBox16.Size = new System.Drawing.Size(168, 43);
			this.groupBox16.TabIndex = 39;
			this.groupBox16.TabStop = false;
			this.groupBox16.Text = "Speed by Zoom Ratio";
			// 
			// ButtonSpeedByZoomOff
			// 
			this.ButtonSpeedByZoomOff.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSpeedByZoomOff.Location = new System.Drawing.Point(79, 16);
			this.ButtonSpeedByZoomOff.Name = "ButtonSpeedByZoomOff";
			this.ButtonSpeedByZoomOff.Size = new System.Drawing.Size(36, 22);
			this.ButtonSpeedByZoomOff.TabIndex = 41;
			this.ButtonSpeedByZoomOff.Text = "Off";
			this.ButtonSpeedByZoomOff.Click += new System.EventHandler(this.ButtonSpeedByZoomOff_Click);
			// 
			// ButtonGetSpeedByZoomRatio
			// 
			this.ButtonGetSpeedByZoomRatio.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetSpeedByZoomRatio.Location = new System.Drawing.Point(5, 16);
			this.ButtonGetSpeedByZoomRatio.Name = "ButtonGetSpeedByZoomRatio";
			this.ButtonGetSpeedByZoomRatio.Size = new System.Drawing.Size(36, 22);
			this.ButtonGetSpeedByZoomRatio.TabIndex = 39;
			this.ButtonGetSpeedByZoomRatio.Text = "Get";
			this.ButtonGetSpeedByZoomRatio.Click += new System.EventHandler(this.ButtonGetSpeedByZoomRatio_Click);
			// 
			// TextBoxSpeedByZoomRatio
			// 
			this.TextBoxSpeedByZoomRatio.Location = new System.Drawing.Point(118, 16);
			this.TextBoxSpeedByZoomRatio.MaxLength = 10;
			this.TextBoxSpeedByZoomRatio.Name = "TextBoxSpeedByZoomRatio";
			this.TextBoxSpeedByZoomRatio.Size = new System.Drawing.Size(46, 21);
			this.TextBoxSpeedByZoomRatio.TabIndex = 38;
			this.TextBoxSpeedByZoomRatio.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSpeedByZoomOn
			// 
			this.ButtonSpeedByZoomOn.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSpeedByZoomOn.Location = new System.Drawing.Point(42, 16);
			this.ButtonSpeedByZoomOn.Name = "ButtonSpeedByZoomOn";
			this.ButtonSpeedByZoomOn.Size = new System.Drawing.Size(36, 22);
			this.ButtonSpeedByZoomOn.TabIndex = 40;
			this.ButtonSpeedByZoomOn.Text = "On";
			this.ButtonSpeedByZoomOn.Click += new System.EventHandler(this.ButtonSpeedByZoomOn_Click);
			// 
			// groupBox15
			// 
			this.groupBox15.Controls.Add(this.ButtonGetCurrentSpeed);
			this.groupBox15.Controls.Add(this.TextBoxTargetSpeed);
			this.groupBox15.Controls.Add(this.ButtonSetTargetSpeed);
			this.groupBox15.Controls.Add(this.TextBoxCurrentSpeed);
			this.groupBox15.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox15.Location = new System.Drawing.Point(6, 98);
			this.groupBox15.Name = "groupBox15";
			this.groupBox15.Size = new System.Drawing.Size(168, 69);
			this.groupBox15.TabIndex = 39;
			this.groupBox15.TabStop = false;
			this.groupBox15.Text = "Speed Change On The Fly";
			// 
			// ButtonGetCurrentSpeed
			// 
			this.ButtonGetCurrentSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetCurrentSpeed.Location = new System.Drawing.Point(5, 18);
			this.ButtonGetCurrentSpeed.Name = "ButtonGetCurrentSpeed";
			this.ButtonGetCurrentSpeed.Size = new System.Drawing.Size(113, 22);
			this.ButtonGetCurrentSpeed.TabIndex = 17;
			this.ButtonGetCurrentSpeed.Text = "Get Current Speed";
			this.ButtonGetCurrentSpeed.Click += new System.EventHandler(this.ButtonGetCurrentSpeed_Click);
			// 
			// TextBoxTargetSpeed
			// 
			this.TextBoxTargetSpeed.Location = new System.Drawing.Point(122, 42);
			this.TextBoxTargetSpeed.MaxLength = 10;
			this.TextBoxTargetSpeed.Name = "TextBoxTargetSpeed";
			this.TextBoxTargetSpeed.Size = new System.Drawing.Size(39, 21);
			this.TextBoxTargetSpeed.TabIndex = 20;
			this.TextBoxTargetSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSetTargetSpeed
			// 
			this.ButtonSetTargetSpeed.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetTargetSpeed.Location = new System.Drawing.Point(5, 42);
			this.ButtonSetTargetSpeed.Name = "ButtonSetTargetSpeed";
			this.ButtonSetTargetSpeed.Size = new System.Drawing.Size(113, 22);
			this.ButtonSetTargetSpeed.TabIndex = 19;
			this.ButtonSetTargetSpeed.Text = "Set Target Speed";
			this.ButtonSetTargetSpeed.Click += new System.EventHandler(this.ButtonSetTargetSpeed_Click);
			// 
			// TextBoxCurrentSpeed
			// 
			this.TextBoxCurrentSpeed.Location = new System.Drawing.Point(122, 18);
			this.TextBoxCurrentSpeed.MaxLength = 10;
			this.TextBoxCurrentSpeed.Name = "TextBoxCurrentSpeed";
			this.TextBoxCurrentSpeed.ReadOnly = true;
			this.TextBoxCurrentSpeed.Size = new System.Drawing.Size(39, 21);
			this.TextBoxCurrentSpeed.TabIndex = 16;
			this.TextBoxCurrentSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.label11);
			this.groupBox10.Controls.Add(this.TextBoxSpeedInPPS);
			this.groupBox10.Controls.Add(this.label6);
			this.groupBox10.Controls.Add(this.TextBoxSpeedLevel);
			this.groupBox10.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox10.Location = new System.Drawing.Point(6, 14);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(168, 43);
			this.groupBox10.TabIndex = 36;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Speed Lv. (1~127, default=100)";
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(84, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(34, 25);
			this.label11.TabIndex = 38;
			this.label11.Text = "PPS";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxSpeedInPPS
			// 
			this.TextBoxSpeedInPPS.Location = new System.Drawing.Point(118, 18);
			this.TextBoxSpeedInPPS.MaxLength = 10;
			this.TextBoxSpeedInPPS.Name = "TextBoxSpeedInPPS";
			this.TextBoxSpeedInPPS.ReadOnly = true;
			this.TextBoxSpeedInPPS.Size = new System.Drawing.Size(46, 21);
			this.TextBoxSpeedInPPS.TabIndex = 37;
			this.TextBoxSpeedInPPS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(3, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(34, 25);
			this.label6.TabIndex = 36;
			this.label6.Text = "Level";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxSpeedLevel
			// 
			this.TextBoxSpeedLevel.Location = new System.Drawing.Point(37, 18);
			this.TextBoxSpeedLevel.MaxLength = 10;
			this.TextBoxSpeedLevel.Name = "TextBoxSpeedLevel";
			this.TextBoxSpeedLevel.Size = new System.Drawing.Size(46, 21);
			this.TextBoxSpeedLevel.TabIndex = 21;
			this.TextBoxSpeedLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBoxSpeedLevel.TextChanged += new System.EventHandler(this.TextBoxSpeedLevel_TextChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ComboBoxAccLevel);
			this.groupBox2.Controls.Add(this.ButtonGetAccLevel);
			this.groupBox2.Controls.Add(this.ButtonSetAccLevel);
			this.groupBox2.Controls.Add(this.ButtonGetAcceleration);
			this.groupBox2.Controls.Add(this.TextBoxAcceleration);
			this.groupBox2.Controls.Add(this.ButtonSetAcceleration);
			this.groupBox2.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(201, 176);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(180, 65);
			this.groupBox2.TabIndex = 32;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Acceleration (pps/s, default=6016)";
			// 
			// ComboBoxAccLevel
			// 
			this.ComboBoxAccLevel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxAccLevel.FormattingEnabled = true;
			this.ComboBoxAccLevel.Items.AddRange(new object[] {
            "Sharp",
            "Standard",
            "Gentle"});
			this.ComboBoxAccLevel.Location = new System.Drawing.Point(118, 39);
			this.ComboBoxAccLevel.Name = "ComboBoxAccLevel";
			this.ComboBoxAccLevel.Size = new System.Drawing.Size(56, 24);
			this.ComboBoxAccLevel.TabIndex = 37;
			// 
			// ButtonGetAccLevel
			// 
			this.ButtonGetAccLevel.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetAccLevel.Location = new System.Drawing.Point(6, 39);
			this.ButtonGetAccLevel.Name = "ButtonGetAccLevel";
			this.ButtonGetAccLevel.Size = new System.Drawing.Size(55, 22);
			this.ButtonGetAccLevel.TabIndex = 21;
			this.ButtonGetAccLevel.Text = "Get Level";
			this.ButtonGetAccLevel.Click += new System.EventHandler(this.ButtonGetAccLevel_Click);
			// 
			// ButtonSetAccLevel
			// 
			this.ButtonSetAccLevel.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetAccLevel.Location = new System.Drawing.Point(61, 39);
			this.ButtonSetAccLevel.Name = "ButtonSetAccLevel";
			this.ButtonSetAccLevel.Size = new System.Drawing.Size(55, 22);
			this.ButtonSetAccLevel.TabIndex = 22;
			this.ButtonSetAccLevel.Text = "Set Level";
			this.ButtonSetAccLevel.Click += new System.EventHandler(this.ButtonSetAccLevel_Click);
			// 
			// ButtonGetAcceleration
			// 
			this.ButtonGetAcceleration.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetAcceleration.Location = new System.Drawing.Point(6, 15);
			this.ButtonGetAcceleration.Name = "ButtonGetAcceleration";
			this.ButtonGetAcceleration.Size = new System.Drawing.Size(55, 22);
			this.ButtonGetAcceleration.TabIndex = 17;
			this.ButtonGetAcceleration.Text = "Get Value";
			this.ButtonGetAcceleration.Click += new System.EventHandler(this.ButtonGetAcceleration_Click);
			// 
			// TextBoxAcceleration
			// 
			this.TextBoxAcceleration.Location = new System.Drawing.Point(118, 16);
			this.TextBoxAcceleration.MaxLength = 10;
			this.TextBoxAcceleration.Name = "TextBoxAcceleration";
			this.TextBoxAcceleration.Size = new System.Drawing.Size(56, 21);
			this.TextBoxAcceleration.TabIndex = 16;
			this.TextBoxAcceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSetAcceleration
			// 
			this.ButtonSetAcceleration.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetAcceleration.Location = new System.Drawing.Point(61, 15);
			this.ButtonSetAcceleration.Name = "ButtonSetAcceleration";
			this.ButtonSetAcceleration.Size = new System.Drawing.Size(55, 22);
			this.ButtonSetAcceleration.TabIndex = 19;
			this.ButtonSetAcceleration.Text = "Set Value";
			this.ButtonSetAcceleration.Click += new System.EventHandler(this.ButtonSetAcceleration_Click);
			// 
			// tabAK7452
			// 
			this.tabAK7452.BackColor = System.Drawing.SystemColors.Control;
			this.tabAK7452.Controls.Add(this.groupBoxShowAngle);
			this.tabAK7452.Controls.Add(this.groupBox6);
			this.tabAK7452.Controls.Add(this.groupBoxAK7452ManualMode);
			this.tabAK7452.Controls.Add(this.chartAngle);
			this.tabAK7452.Controls.Add(this.groupBoxAK7452NormalMode);
			this.tabAK7452.Location = new System.Drawing.Point(4, 22);
			this.tabAK7452.Name = "tabAK7452";
			this.tabAK7452.Padding = new System.Windows.Forms.Padding(3);
			this.tabAK7452.Size = new System.Drawing.Size(1025, 382);
			this.tabAK7452.TabIndex = 3;
			this.tabAK7452.Text = "AK7452";
			// 
			// groupBoxShowAngle
			// 
			this.groupBoxShowAngle.Controls.Add(this.ButtonClearShowAngle);
			this.groupBoxShowAngle.Controls.Add(this.ButtonShowAngle);
			this.groupBoxShowAngle.Controls.Add(this.ButtonStopShowAngle);
			this.groupBoxShowAngle.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxShowAngle.Location = new System.Drawing.Point(910, 40);
			this.groupBoxShowAngle.Name = "groupBoxShowAngle";
			this.groupBoxShowAngle.Size = new System.Drawing.Size(100, 86);
			this.groupBoxShowAngle.TabIndex = 44;
			this.groupBoxShowAngle.TabStop = false;
			this.groupBoxShowAngle.Text = "Show Angle";
			// 
			// ButtonClearShowAngle
			// 
			this.ButtonClearShowAngle.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonClearShowAngle.Location = new System.Drawing.Point(7, 58);
			this.ButtonClearShowAngle.Name = "ButtonClearShowAngle";
			this.ButtonClearShowAngle.Size = new System.Drawing.Size(87, 22);
			this.ButtonClearShowAngle.TabIndex = 33;
			this.ButtonClearShowAngle.Text = "Clear Chart";
			this.ButtonClearShowAngle.Click += new System.EventHandler(this.ButtonClearShowAngle_Click);
			// 
			// ButtonShowAngle
			// 
			this.ButtonShowAngle.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonShowAngle.Location = new System.Drawing.Point(7, 16);
			this.ButtonShowAngle.Name = "ButtonShowAngle";
			this.ButtonShowAngle.Size = new System.Drawing.Size(87, 22);
			this.ButtonShowAngle.TabIndex = 15;
			this.ButtonShowAngle.Text = "Start";
			this.ButtonShowAngle.Click += new System.EventHandler(this.ButtonShowAngle_Click);
			// 
			// ButtonStopShowAngle
			// 
			this.ButtonStopShowAngle.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonStopShowAngle.Location = new System.Drawing.Point(7, 37);
			this.ButtonStopShowAngle.Name = "ButtonStopShowAngle";
			this.ButtonStopShowAngle.Size = new System.Drawing.Size(87, 22);
			this.ButtonStopShowAngle.TabIndex = 30;
			this.ButtonStopShowAngle.Text = "Stop";
			this.ButtonStopShowAngle.Click += new System.EventHandler(this.ButtonStopShowAngle_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.ButtonReverseCalibration);
			this.groupBox6.Controls.Add(this.TextBoxLockStatus);
			this.groupBox6.Controls.Add(this.ButtonLockStatus);
			this.groupBox6.Controls.Add(this.ButtonUnlockHome);
			this.groupBox6.Controls.Add(this.ButtonLockHome);
			this.groupBox6.Controls.Add(this.TextBoxZPCalibration);
			this.groupBox6.Controls.Add(this.ButtonZPCaliStatus);
			this.groupBox6.Controls.Add(this.ButtonClearZPCali);
			this.groupBox6.Controls.Add(this.ButtonHome);
			this.groupBox6.Controls.Add(this.ButtonCalibration);
			this.groupBox6.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox6.Location = new System.Drawing.Point(6, 286);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(386, 67);
			this.groupBox6.TabIndex = 35;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Zero-point Calibration";
			// 
			// ButtonReverseCalibration
			// 
			this.ButtonReverseCalibration.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonReverseCalibration.Location = new System.Drawing.Point(42, 18);
			this.ButtonReverseCalibration.Name = "ButtonReverseCalibration";
			this.ButtonReverseCalibration.Size = new System.Drawing.Size(34, 22);
			this.ButtonReverseCalibration.TabIndex = 47;
			this.ButtonReverseCalibration.Text = "Cali-";
			this.ButtonReverseCalibration.Click += new System.EventHandler(this.ButtonReverseCalibration_Click);
			// 
			// TextBoxLockStatus
			// 
			this.TextBoxLockStatus.Location = new System.Drawing.Point(223, 41);
			this.TextBoxLockStatus.MaxLength = 10;
			this.TextBoxLockStatus.Name = "TextBoxLockStatus";
			this.TextBoxLockStatus.ReadOnly = true;
			this.TextBoxLockStatus.Size = new System.Drawing.Size(68, 21);
			this.TextBoxLockStatus.TabIndex = 46;
			this.TextBoxLockStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonLockStatus
			// 
			this.ButtonLockStatus.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonLockStatus.Location = new System.Drawing.Point(222, 18);
			this.ButtonLockStatus.Name = "ButtonLockStatus";
			this.ButtonLockStatus.Size = new System.Drawing.Size(69, 22);
			this.ButtonLockStatus.TabIndex = 45;
			this.ButtonLockStatus.Text = "Lock Status";
			this.ButtonLockStatus.Click += new System.EventHandler(this.ButtonLockStatus_Click);
			// 
			// ButtonUnlockHome
			// 
			this.ButtonUnlockHome.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonUnlockHome.Location = new System.Drawing.Point(145, 41);
			this.ButtonUnlockHome.Name = "ButtonUnlockHome";
			this.ButtonUnlockHome.Size = new System.Drawing.Size(76, 22);
			this.ButtonUnlockHome.TabIndex = 44;
			this.ButtonUnlockHome.Text = "Unlock Home";
			this.ButtonUnlockHome.Click += new System.EventHandler(this.ButtonUnlockHome_Click);
			// 
			// ButtonLockHome
			// 
			this.ButtonLockHome.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonLockHome.Location = new System.Drawing.Point(145, 18);
			this.ButtonLockHome.Name = "ButtonLockHome";
			this.ButtonLockHome.Size = new System.Drawing.Size(76, 22);
			this.ButtonLockHome.TabIndex = 43;
			this.ButtonLockHome.Text = "Lock Home";
			this.ButtonLockHome.Click += new System.EventHandler(this.ButtonLockHome_Click);
			// 
			// TextBoxZPCalibration
			// 
			this.TextBoxZPCalibration.Location = new System.Drawing.Point(80, 42);
			this.TextBoxZPCalibration.MaxLength = 10;
			this.TextBoxZPCalibration.Name = "TextBoxZPCalibration";
			this.TextBoxZPCalibration.ReadOnly = true;
			this.TextBoxZPCalibration.Size = new System.Drawing.Size(58, 21);
			this.TextBoxZPCalibration.TabIndex = 42;
			this.TextBoxZPCalibration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonZPCaliStatus
			// 
			this.ButtonZPCaliStatus.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZPCaliStatus.Location = new System.Drawing.Point(80, 18);
			this.ButtonZPCaliStatus.Name = "ButtonZPCaliStatus";
			this.ButtonZPCaliStatus.Size = new System.Drawing.Size(58, 22);
			this.ButtonZPCaliStatus.TabIndex = 41;
			this.ButtonZPCaliStatus.Text = "Cali. Status";
			this.ButtonZPCaliStatus.Click += new System.EventHandler(this.ButtonZPCaliStatus_Click);
			// 
			// ButtonClearZPCali
			// 
			this.ButtonClearZPCali.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonClearZPCali.Location = new System.Drawing.Point(8, 42);
			this.ButtonClearZPCali.Name = "ButtonClearZPCali";
			this.ButtonClearZPCali.Size = new System.Drawing.Size(68, 22);
			this.ButtonClearZPCali.TabIndex = 36;
			this.ButtonClearZPCali.Text = "Clear Cali.";
			this.ButtonClearZPCali.Click += new System.EventHandler(this.ButtonClearZPCali_Click);
			// 
			// ButtonHome
			// 
			this.ButtonHome.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonHome.Location = new System.Drawing.Point(297, 29);
			this.ButtonHome.Name = "ButtonHome";
			this.ButtonHome.Size = new System.Drawing.Size(68, 22);
			this.ButtonHome.TabIndex = 35;
			this.ButtonHome.Text = "Home";
			this.ButtonHome.Click += new System.EventHandler(this.ButtonHome_Click);
			// 
			// ButtonCalibration
			// 
			this.ButtonCalibration.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonCalibration.Location = new System.Drawing.Point(8, 18);
			this.ButtonCalibration.Name = "ButtonCalibration";
			this.ButtonCalibration.Size = new System.Drawing.Size(34, 22);
			this.ButtonCalibration.TabIndex = 34;
			this.ButtonCalibration.Text = "Cali+";
			this.ButtonCalibration.Click += new System.EventHandler(this.ButtonCalibration_Click);
			// 
			// groupBoxAK7452ManualMode
			// 
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonDataRenewal);
			this.groupBoxAK7452ManualMode.Controls.Add(this.CheckBoxSaveToEEPROM);
			this.groupBoxAK7452ManualMode.Controls.Add(this.groupBox19);
			this.groupBoxAK7452ManualMode.Controls.Add(this.groupBoxAK7452ABZResolution);
			this.groupBoxAK7452ManualMode.Controls.Add(this.TextBoxMemLock);
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonMemLock);
			this.groupBoxAK7452ManualMode.Controls.Add(this.groupBoxAK7452ZeroDegreePoint);
			this.groupBoxAK7452ManualMode.Controls.Add(this.groupBoxAK7452RDABZ);
			this.groupBoxAK7452ManualMode.Controls.Add(this.TextBoxErrorMonitor);
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonErrorMonitor);
			this.groupBoxAK7452ManualMode.Controls.Add(this.TextBoxASMode);
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonASMode);
			this.groupBoxAK7452ManualMode.Controls.Add(this.TextBoxMagFlux);
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonMagFlux);
			this.groupBoxAK7452ManualMode.Controls.Add(this.TextBoxAngleDataReg);
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonSetMaualMode);
			this.groupBoxAK7452ManualMode.Controls.Add(this.ButtonAngleDataReg);
			this.groupBoxAK7452ManualMode.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAK7452ManualMode.Location = new System.Drawing.Point(6, 43);
			this.groupBoxAK7452ManualMode.Name = "groupBoxAK7452ManualMode";
			this.groupBoxAK7452ManualMode.Size = new System.Drawing.Size(386, 244);
			this.groupBoxAK7452ManualMode.TabIndex = 29;
			this.groupBoxAK7452ManualMode.TabStop = false;
			this.groupBoxAK7452ManualMode.Text = "Manual mode";
			// 
			// ButtonDataRenewal
			// 
			this.ButtonDataRenewal.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonDataRenewal.Location = new System.Drawing.Point(103, 14);
			this.ButtonDataRenewal.Name = "ButtonDataRenewal";
			this.ButtonDataRenewal.Size = new System.Drawing.Size(87, 22);
			this.ButtonDataRenewal.TabIndex = 70;
			this.ButtonDataRenewal.Text = "Data Renewal";
			this.ButtonDataRenewal.Click += new System.EventHandler(this.ButtonDataRenewal_Click);
			// 
			// CheckBoxSaveToEEPROM
			// 
			this.CheckBoxSaveToEEPROM.AutoSize = true;
			this.CheckBoxSaveToEEPROM.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CheckBoxSaveToEEPROM.Location = new System.Drawing.Point(262, 9);
			this.CheckBoxSaveToEEPROM.Name = "CheckBoxSaveToEEPROM";
			this.CheckBoxSaveToEEPROM.Size = new System.Drawing.Size(105, 19);
			this.CheckBoxSaveToEEPROM.TabIndex = 69;
			this.CheckBoxSaveToEEPROM.Text = "Save To EEPROM";
			// 
			// groupBox19
			// 
			this.groupBox19.Controls.Add(this.ComboBoxSDDIS);
			this.groupBox19.Controls.Add(this.ButtonSetSDDIS);
			this.groupBox19.Controls.Add(this.ButtonGetSDDIS);
			this.groupBox19.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox19.Location = new System.Drawing.Point(10, 170);
			this.groupBox19.Name = "groupBox19";
			this.groupBox19.Size = new System.Drawing.Size(180, 70);
			this.groupBox19.TabIndex = 31;
			this.groupBox19.TabStop = false;
			this.groupBox19.Text = "Self-Diagnostic ON/OFF";
			// 
			// ComboBoxSDDIS
			// 
			this.ComboBoxSDDIS.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxSDDIS.FormattingEnabled = true;
			this.ComboBoxSDDIS.Items.AddRange(new object[] {
            "All ON",
            "Loss of Tracking Detection OFF",
            "Low Mag. Field Detection OFF",
            "All OFF"});
			this.ComboBoxSDDIS.Location = new System.Drawing.Point(7, 39);
			this.ComboBoxSDDIS.Name = "ComboBoxSDDIS";
			this.ComboBoxSDDIS.Size = new System.Drawing.Size(167, 24);
			this.ComboBoxSDDIS.TabIndex = 27;
			// 
			// ButtonSetSDDIS
			// 
			this.ButtonSetSDDIS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetSDDIS.Location = new System.Drawing.Point(90, 16);
			this.ButtonSetSDDIS.Name = "ButtonSetSDDIS";
			this.ButtonSetSDDIS.Size = new System.Drawing.Size(85, 22);
			this.ButtonSetSDDIS.TabIndex = 26;
			this.ButtonSetSDDIS.Text = "Set";
			this.ButtonSetSDDIS.Click += new System.EventHandler(this.ButtonSetSDDIS_Click);
			// 
			// ButtonGetSDDIS
			// 
			this.ButtonGetSDDIS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetSDDIS.Location = new System.Drawing.Point(6, 16);
			this.ButtonGetSDDIS.Name = "ButtonGetSDDIS";
			this.ButtonGetSDDIS.Size = new System.Drawing.Size(85, 22);
			this.ButtonGetSDDIS.TabIndex = 26;
			this.ButtonGetSDDIS.Text = "Get";
			this.ButtonGetSDDIS.Click += new System.EventHandler(this.ButtonGetSDDIS_Click);
			// 
			// groupBoxAK7452ABZResolution
			// 
			this.groupBoxAK7452ABZResolution.Controls.Add(this.ButtonSetABZRes);
			this.groupBoxAK7452ABZResolution.Controls.Add(this.ButtonGetABZRes);
			this.groupBoxAK7452ABZResolution.Controls.Add(this.TextBoxABZRes);
			this.groupBoxAK7452ABZResolution.Location = new System.Drawing.Point(196, 201);
			this.groupBoxAK7452ABZResolution.Name = "groupBoxAK7452ABZResolution";
			this.groupBoxAK7452ABZResolution.Size = new System.Drawing.Size(180, 39);
			this.groupBoxAK7452ABZResolution.TabIndex = 30;
			this.groupBoxAK7452ABZResolution.TabStop = false;
			this.groupBoxAK7452ABZResolution.Text = "ABZ Resolution";
			// 
			// ButtonSetABZRes
			// 
			this.ButtonSetABZRes.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetABZRes.Location = new System.Drawing.Point(66, 15);
			this.ButtonSetABZRes.Name = "ButtonSetABZRes";
			this.ButtonSetABZRes.Size = new System.Drawing.Size(60, 22);
			this.ButtonSetABZRes.TabIndex = 26;
			this.ButtonSetABZRes.Text = "Set";
			this.ButtonSetABZRes.Click += new System.EventHandler(this.ButtonSetABZRes_Click);
			// 
			// ButtonGetABZRes
			// 
			this.ButtonGetABZRes.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetABZRes.Location = new System.Drawing.Point(6, 15);
			this.ButtonGetABZRes.Name = "ButtonGetABZRes";
			this.ButtonGetABZRes.Size = new System.Drawing.Size(60, 22);
			this.ButtonGetABZRes.TabIndex = 26;
			this.ButtonGetABZRes.Text = "Get";
			this.ButtonGetABZRes.Click += new System.EventHandler(this.ButtonGetABZRes_Click);
			// 
			// TextBoxABZRes
			// 
			this.TextBoxABZRes.Location = new System.Drawing.Point(128, 15);
			this.TextBoxABZRes.MaxLength = 10;
			this.TextBoxABZRes.Name = "TextBoxABZRes";
			this.TextBoxABZRes.Size = new System.Drawing.Size(46, 21);
			this.TextBoxABZRes.TabIndex = 26;
			this.TextBoxABZRes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxMemLock
			// 
			this.TextBoxMemLock.Location = new System.Drawing.Point(103, 148);
			this.TextBoxMemLock.MaxLength = 10;
			this.TextBoxMemLock.Name = "TextBoxMemLock";
			this.TextBoxMemLock.ReadOnly = true;
			this.TextBoxMemLock.Size = new System.Drawing.Size(87, 21);
			this.TextBoxMemLock.TabIndex = 29;
			this.TextBoxMemLock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonMemLock
			// 
			this.ButtonMemLock.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonMemLock.Location = new System.Drawing.Point(10, 147);
			this.ButtonMemLock.Name = "ButtonMemLock";
			this.ButtonMemLock.Size = new System.Drawing.Size(87, 22);
			this.ButtonMemLock.TabIndex = 28;
			this.ButtonMemLock.Text = "Mem. Lock";
			this.ButtonMemLock.Click += new System.EventHandler(this.ButtonMemLock_Click);
			// 
			// groupBoxAK7452ZeroDegreePoint
			// 
			this.groupBoxAK7452ZeroDegreePoint.Controls.Add(this.ButtonGetZP);
			this.groupBoxAK7452ZeroDegreePoint.Controls.Add(this.TextBoxZP);
			this.groupBoxAK7452ZeroDegreePoint.Controls.Add(this.ButtonSetZP);
			this.groupBoxAK7452ZeroDegreePoint.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAK7452ZeroDegreePoint.Location = new System.Drawing.Point(196, 161);
			this.groupBoxAK7452ZeroDegreePoint.Name = "groupBoxAK7452ZeroDegreePoint";
			this.groupBoxAK7452ZeroDegreePoint.Size = new System.Drawing.Size(180, 39);
			this.groupBoxAK7452ZeroDegreePoint.TabIndex = 27;
			this.groupBoxAK7452ZeroDegreePoint.TabStop = false;
			this.groupBoxAK7452ZeroDegreePoint.Text = "Zero Degree Point";
			// 
			// ButtonGetZP
			// 
			this.ButtonGetZP.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetZP.Location = new System.Drawing.Point(6, 15);
			this.ButtonGetZP.Name = "ButtonGetZP";
			this.ButtonGetZP.Size = new System.Drawing.Size(60, 22);
			this.ButtonGetZP.TabIndex = 17;
			this.ButtonGetZP.Text = "Get";
			this.ButtonGetZP.Click += new System.EventHandler(this.ButtonGetZP_Click);
			// 
			// TextBoxZP
			// 
			this.TextBoxZP.Location = new System.Drawing.Point(128, 15);
			this.TextBoxZP.MaxLength = 10;
			this.TextBoxZP.Name = "TextBoxZP";
			this.TextBoxZP.Size = new System.Drawing.Size(46, 21);
			this.TextBoxZP.TabIndex = 16;
			this.TextBoxZP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSetZP
			// 
			this.ButtonSetZP.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetZP.Location = new System.Drawing.Point(66, 15);
			this.ButtonSetZP.Name = "ButtonSetZP";
			this.ButtonSetZP.Size = new System.Drawing.Size(60, 22);
			this.ButtonSetZP.TabIndex = 19;
			this.ButtonSetZP.Text = "Set";
			this.ButtonSetZP.Click += new System.EventHandler(this.ButtonSetZP_Click);
			// 
			// groupBoxAK7452RDABZ
			// 
			this.groupBoxAK7452RDABZ.Controls.Add(this.ComboBoxABZHysteresis);
			this.groupBoxAK7452RDABZ.Controls.Add(this.ComboBoxABZEnable);
			this.groupBoxAK7452RDABZ.Controls.Add(this.ComboBoxZWidth);
			this.groupBoxAK7452RDABZ.Controls.Add(this.ComboBoxRD);
			this.groupBoxAK7452RDABZ.Controls.Add(this.labelAK7452ABZHysteresis);
			this.groupBoxAK7452RDABZ.Controls.Add(this.labelAK7452ABZEnable);
			this.groupBoxAK7452RDABZ.Controls.Add(this.labelAK7452ZWidth);
			this.groupBoxAK7452RDABZ.Controls.Add(this.labelAK7452RotationDir);
			this.groupBoxAK7452RDABZ.Controls.Add(this.ButtonSetRDABZ);
			this.groupBoxAK7452RDABZ.Controls.Add(this.ButtonGetRDABZ);
			this.groupBoxAK7452RDABZ.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAK7452RDABZ.Location = new System.Drawing.Point(196, 19);
			this.groupBoxAK7452RDABZ.Name = "groupBoxAK7452RDABZ";
			this.groupBoxAK7452RDABZ.Size = new System.Drawing.Size(180, 141);
			this.groupBoxAK7452RDABZ.TabIndex = 25;
			this.groupBoxAK7452RDABZ.TabStop = false;
			this.groupBoxAK7452RDABZ.Text = "RDABZ";
			// 
			// ComboBoxABZHysteresis
			// 
			this.ComboBoxABZHysteresis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxABZHysteresis.FormattingEnabled = true;
			this.ComboBoxABZHysteresis.Items.AddRange(new object[] {
            "OFF",
            "0LSB",
            "1LSB",
            "2LSB",
            "3LSB",
            "4LSB",
            "5LSB",
            "6LSB",
            "7LSB",
            "8LSB",
            "9LSB",
            "10LSB",
            "11LSB",
            "12LSB",
            "OFF",
            "OFF"});
			this.ComboBoxABZHysteresis.Location = new System.Drawing.Point(93, 112);
			this.ComboBoxABZHysteresis.Name = "ComboBoxABZHysteresis";
			this.ComboBoxABZHysteresis.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxABZHysteresis.TabIndex = 37;
			// 
			// ComboBoxABZEnable
			// 
			this.ComboBoxABZEnable.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxABZEnable.FormattingEnabled = true;
			this.ComboBoxABZEnable.Items.AddRange(new object[] {
            "OFF",
            "ON"});
			this.ComboBoxABZEnable.Location = new System.Drawing.Point(93, 88);
			this.ComboBoxABZEnable.Name = "ComboBoxABZEnable";
			this.ComboBoxABZEnable.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxABZEnable.TabIndex = 36;
			// 
			// ComboBoxZWidth
			// 
			this.ComboBoxZWidth.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxZWidth.FormattingEnabled = true;
			this.ComboBoxZWidth.Items.AddRange(new object[] {
            "1 LSB",
            "180 deg.",
            "4 LSB",
            "180 deg."});
			this.ComboBoxZWidth.Location = new System.Drawing.Point(93, 64);
			this.ComboBoxZWidth.Name = "ComboBoxZWidth";
			this.ComboBoxZWidth.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxZWidth.TabIndex = 35;
			// 
			// ComboBoxRD
			// 
			this.ComboBoxRD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxRD.FormattingEnabled = true;
			this.ComboBoxRD.Items.AddRange(new object[] {
            "CCW",
            "CW"});
			this.ComboBoxRD.Location = new System.Drawing.Point(93, 40);
			this.ComboBoxRD.Name = "ComboBoxRD";
			this.ComboBoxRD.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxRD.TabIndex = 28;
			// 
			// labelAK7452ABZHysteresis
			// 
			this.labelAK7452ABZHysteresis.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAK7452ABZHysteresis.Location = new System.Drawing.Point(6, 112);
			this.labelAK7452ABZHysteresis.Name = "labelAK7452ABZHysteresis";
			this.labelAK7452ABZHysteresis.Size = new System.Drawing.Size(81, 22);
			this.labelAK7452ABZHysteresis.TabIndex = 34;
			this.labelAK7452ABZHysteresis.Text = "ABZ Hysteresis";
			this.labelAK7452ABZHysteresis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelAK7452ABZEnable
			// 
			this.labelAK7452ABZEnable.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAK7452ABZEnable.Location = new System.Drawing.Point(6, 87);
			this.labelAK7452ABZEnable.Name = "labelAK7452ABZEnable";
			this.labelAK7452ABZEnable.Size = new System.Drawing.Size(81, 22);
			this.labelAK7452ABZEnable.TabIndex = 32;
			this.labelAK7452ABZEnable.Text = "ABZ Enable";
			this.labelAK7452ABZEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelAK7452ZWidth
			// 
			this.labelAK7452ZWidth.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAK7452ZWidth.Location = new System.Drawing.Point(6, 62);
			this.labelAK7452ZWidth.Name = "labelAK7452ZWidth";
			this.labelAK7452ZWidth.Size = new System.Drawing.Size(81, 22);
			this.labelAK7452ZWidth.TabIndex = 30;
			this.labelAK7452ZWidth.Text = "Z Width";
			this.labelAK7452ZWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelAK7452RotationDir
			// 
			this.labelAK7452RotationDir.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAK7452RotationDir.Location = new System.Drawing.Point(6, 38);
			this.labelAK7452RotationDir.Name = "labelAK7452RotationDir";
			this.labelAK7452RotationDir.Size = new System.Drawing.Size(81, 22);
			this.labelAK7452RotationDir.TabIndex = 28;
			this.labelAK7452RotationDir.Text = "Rotation Dir.";
			this.labelAK7452RotationDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonSetRDABZ
			// 
			this.ButtonSetRDABZ.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetRDABZ.Location = new System.Drawing.Point(93, 13);
			this.ButtonSetRDABZ.Name = "ButtonSetRDABZ";
			this.ButtonSetRDABZ.Size = new System.Drawing.Size(81, 22);
			this.ButtonSetRDABZ.TabIndex = 27;
			this.ButtonSetRDABZ.Text = "Set";
			this.ButtonSetRDABZ.Click += new System.EventHandler(this.ButtonSetRDABZ_Click);
			// 
			// ButtonGetRDABZ
			// 
			this.ButtonGetRDABZ.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetRDABZ.Location = new System.Drawing.Point(6, 13);
			this.ButtonGetRDABZ.Name = "ButtonGetRDABZ";
			this.ButtonGetRDABZ.Size = new System.Drawing.Size(81, 22);
			this.ButtonGetRDABZ.TabIndex = 26;
			this.ButtonGetRDABZ.Text = "Get ";
			this.ButtonGetRDABZ.Click += new System.EventHandler(this.ButtonGetRDABZ_Click);
			// 
			// TextBoxErrorMonitor
			// 
			this.TextBoxErrorMonitor.Location = new System.Drawing.Point(103, 121);
			this.TextBoxErrorMonitor.MaxLength = 10;
			this.TextBoxErrorMonitor.Name = "TextBoxErrorMonitor";
			this.TextBoxErrorMonitor.ReadOnly = true;
			this.TextBoxErrorMonitor.Size = new System.Drawing.Size(87, 21);
			this.TextBoxErrorMonitor.TabIndex = 14;
			this.TextBoxErrorMonitor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonErrorMonitor
			// 
			this.ButtonErrorMonitor.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonErrorMonitor.Location = new System.Drawing.Point(10, 120);
			this.ButtonErrorMonitor.Name = "ButtonErrorMonitor";
			this.ButtonErrorMonitor.Size = new System.Drawing.Size(87, 22);
			this.ButtonErrorMonitor.TabIndex = 13;
			this.ButtonErrorMonitor.Text = "ERROR Monitor";
			this.ButtonErrorMonitor.Click += new System.EventHandler(this.ButtonErrorMonitor_Click);
			// 
			// TextBoxASMode
			// 
			this.TextBoxASMode.Location = new System.Drawing.Point(103, 94);
			this.TextBoxASMode.MaxLength = 10;
			this.TextBoxASMode.Name = "TextBoxASMode";
			this.TextBoxASMode.ReadOnly = true;
			this.TextBoxASMode.Size = new System.Drawing.Size(87, 21);
			this.TextBoxASMode.TabIndex = 12;
			this.TextBoxASMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonASMode
			// 
			this.ButtonASMode.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonASMode.Location = new System.Drawing.Point(10, 93);
			this.ButtonASMode.Name = "ButtonASMode";
			this.ButtonASMode.Size = new System.Drawing.Size(87, 22);
			this.ButtonASMode.TabIndex = 11;
			this.ButtonASMode.Text = "Mode";
			this.ButtonASMode.Click += new System.EventHandler(this.ButtonASMode_Click);
			// 
			// TextBoxMagFlux
			// 
			this.TextBoxMagFlux.Location = new System.Drawing.Point(103, 67);
			this.TextBoxMagFlux.MaxLength = 10;
			this.TextBoxMagFlux.Name = "TextBoxMagFlux";
			this.TextBoxMagFlux.ReadOnly = true;
			this.TextBoxMagFlux.Size = new System.Drawing.Size(87, 21);
			this.TextBoxMagFlux.TabIndex = 10;
			this.TextBoxMagFlux.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonMagFlux
			// 
			this.ButtonMagFlux.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonMagFlux.Location = new System.Drawing.Point(10, 66);
			this.ButtonMagFlux.Name = "ButtonMagFlux";
			this.ButtonMagFlux.Size = new System.Drawing.Size(87, 22);
			this.ButtonMagFlux.TabIndex = 9;
			this.ButtonMagFlux.Text = "Mag. Flux";
			this.ButtonMagFlux.Click += new System.EventHandler(this.ButtonMagFlux_Click);
			// 
			// TextBoxAngleDataReg
			// 
			this.TextBoxAngleDataReg.Location = new System.Drawing.Point(103, 40);
			this.TextBoxAngleDataReg.MaxLength = 10;
			this.TextBoxAngleDataReg.Name = "TextBoxAngleDataReg";
			this.TextBoxAngleDataReg.ReadOnly = true;
			this.TextBoxAngleDataReg.Size = new System.Drawing.Size(87, 21);
			this.TextBoxAngleDataReg.TabIndex = 8;
			this.TextBoxAngleDataReg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSetMaualMode
			// 
			this.ButtonSetMaualMode.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetMaualMode.Location = new System.Drawing.Point(10, 14);
			this.ButtonSetMaualMode.Name = "ButtonSetMaualMode";
			this.ButtonSetMaualMode.Size = new System.Drawing.Size(87, 22);
			this.ButtonSetMaualMode.TabIndex = 6;
			this.ButtonSetMaualMode.Text = "Set Manual Mode";
			this.ButtonSetMaualMode.Click += new System.EventHandler(this.ButtonSetMaualMode_Click);
			// 
			// ButtonAngleDataReg
			// 
			this.ButtonAngleDataReg.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAngleDataReg.Location = new System.Drawing.Point(10, 39);
			this.ButtonAngleDataReg.Name = "ButtonAngleDataReg";
			this.ButtonAngleDataReg.Size = new System.Drawing.Size(87, 22);
			this.ButtonAngleDataReg.TabIndex = 7;
			this.ButtonAngleDataReg.Text = "Angle Data (reg.)";
			this.ButtonAngleDataReg.Click += new System.EventHandler(this.ButtonAngleDataReg_Click);
			// 
			// chartAngle
			// 
			chartArea2.AxisX.Interval = 1D;
			chartArea2.AxisX.LabelStyle.Format = "N0";
			chartArea2.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea2.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea2.AxisX.ScrollBar.BackColor = System.Drawing.Color.White;
			chartArea2.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.White;
			chartArea2.AxisX.Title = "s";
			chartArea2.AxisY.Maximum = 20000D;
			chartArea2.AxisY.Minimum = 0D;
			chartArea2.Name = "ChartArea1";
			this.chartAngle.ChartAreas.Add(chartArea2);
			legend2.Name = "Legend1";
			this.chartAngle.Legends.Add(legend2);
			this.chartAngle.Location = new System.Drawing.Point(398, 3);
			this.chartAngle.Name = "chartAngle";
			series2.ChartArea = "ChartArea1";
			series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series2.Legend = "Legend1";
			series2.Name = "Angle";
			this.chartAngle.Series.Add(series2);
			this.chartAngle.Size = new System.Drawing.Size(624, 350);
			this.chartAngle.TabIndex = 29;
			this.chartAngle.Text = "chart1";
			// 
			// groupBoxAK7452NormalMode
			// 
			this.groupBoxAK7452NormalMode.Controls.Add(this.textBoxErrorPin);
			this.groupBoxAK7452NormalMode.Controls.Add(this.ButtonErrorPin);
			this.groupBoxAK7452NormalMode.Controls.Add(this.textBoxAngleData);
			this.groupBoxAK7452NormalMode.Controls.Add(this.ButtonSetNormalMode);
			this.groupBoxAK7452NormalMode.Controls.Add(this.ButtonAngleData);
			this.groupBoxAK7452NormalMode.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAK7452NormalMode.Location = new System.Drawing.Point(6, 3);
			this.groupBoxAK7452NormalMode.Name = "groupBoxAK7452NormalMode";
			this.groupBoxAK7452NormalMode.Size = new System.Drawing.Size(386, 41);
			this.groupBoxAK7452NormalMode.TabIndex = 28;
			this.groupBoxAK7452NormalMode.TabStop = false;
			this.groupBoxAK7452NormalMode.Text = "Normal mode";
			// 
			// textBoxErrorPin
			// 
			this.textBoxErrorPin.Location = new System.Drawing.Point(319, 15);
			this.textBoxErrorPin.MaxLength = 10;
			this.textBoxErrorPin.Name = "textBoxErrorPin";
			this.textBoxErrorPin.ReadOnly = true;
			this.textBoxErrorPin.Size = new System.Drawing.Size(57, 21);
			this.textBoxErrorPin.TabIndex = 14;
			this.textBoxErrorPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonErrorPin
			// 
			this.ButtonErrorPin.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonErrorPin.Location = new System.Drawing.Point(250, 14);
			this.ButtonErrorPin.Name = "ButtonErrorPin";
			this.ButtonErrorPin.Size = new System.Drawing.Size(63, 22);
			this.ButtonErrorPin.TabIndex = 13;
			this.ButtonErrorPin.Text = "ERROR";
			this.ButtonErrorPin.Click += new System.EventHandler(this.ButtonErrorPin_Click);
			// 
			// textBoxAngleData
			// 
			this.textBoxAngleData.Location = new System.Drawing.Point(187, 15);
			this.textBoxAngleData.MaxLength = 10;
			this.textBoxAngleData.Name = "textBoxAngleData";
			this.textBoxAngleData.ReadOnly = true;
			this.textBoxAngleData.Size = new System.Drawing.Size(57, 21);
			this.textBoxAngleData.TabIndex = 8;
			this.textBoxAngleData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSetNormalMode
			// 
			this.ButtonSetNormalMode.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetNormalMode.Location = new System.Drawing.Point(10, 14);
			this.ButtonSetNormalMode.Name = "ButtonSetNormalMode";
			this.ButtonSetNormalMode.Size = new System.Drawing.Size(87, 22);
			this.ButtonSetNormalMode.TabIndex = 6;
			this.ButtonSetNormalMode.Text = "Set Normal Mode";
			this.ButtonSetNormalMode.Click += new System.EventHandler(this.ButtonSetNormalMode_Click);
			// 
			// ButtonAngleData
			// 
			this.ButtonAngleData.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAngleData.Location = new System.Drawing.Point(118, 14);
			this.ButtonAngleData.Name = "ButtonAngleData";
			this.ButtonAngleData.Size = new System.Drawing.Size(63, 22);
			this.ButtonAngleData.TabIndex = 7;
			this.ButtonAngleData.Text = "Angle Data";
			this.ButtonAngleData.Click += new System.EventHandler(this.ButtonAngleData_Click);
			// 
			// tabTLE5012B
			// 
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleT_RAW);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleIIF_CNT);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleD_MAG);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleADC_Y);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleT25O);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleADC_X);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleTCO_Y);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleMOD_4);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleIFAB);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleSYNCH);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleOFFSET_Y);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleOFFX);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleMOD_3);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleMOD_2);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleSIL);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleMOD_1);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleFSYNC);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleAREV);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleASPD);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleAVAL);
			this.tabTLE5012B.Controls.Add(this.GroupBoxTleACSTAT);
			this.tabTLE5012B.Controls.Add(this.groupTleSTAT);
			this.tabTLE5012B.Location = new System.Drawing.Point(4, 22);
			this.tabTLE5012B.Name = "tabTLE5012B";
			this.tabTLE5012B.Size = new System.Drawing.Size(1025, 382);
			this.tabTLE5012B.TabIndex = 7;
			this.tabTLE5012B.Text = "TLE5012B";
			this.tabTLE5012B.UseVisualStyleBackColor = true;
			// 
			// GroupBoxTleT_RAW
			// 
			this.GroupBoxTleT_RAW.Controls.Add(this.TextBoxTleT_TGL);
			this.GroupBoxTleT_RAW.Controls.Add(this.ButtonTleGetT_TGL);
			this.GroupBoxTleT_RAW.Controls.Add(this.TextBoxTleT_RAW);
			this.GroupBoxTleT_RAW.Controls.Add(this.ButtonTleGetT_RAW);
			this.GroupBoxTleT_RAW.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleT_RAW.Location = new System.Drawing.Point(437, 318);
			this.GroupBoxTleT_RAW.Name = "GroupBoxTleT_RAW";
			this.GroupBoxTleT_RAW.Size = new System.Drawing.Size(256, 40);
			this.GroupBoxTleT_RAW.TabIndex = 56;
			this.GroupBoxTleT_RAW.TabStop = false;
			this.GroupBoxTleT_RAW.Text = "T_RAW T_RAW Register";
			// 
			// TextBoxTleT_TGL
			// 
			this.TextBoxTleT_TGL.Location = new System.Drawing.Point(208, 15);
			this.TextBoxTleT_TGL.MaxLength = 10;
			this.TextBoxTleT_TGL.Name = "TextBoxTleT_TGL";
			this.TextBoxTleT_TGL.ReadOnly = true;
			this.TextBoxTleT_TGL.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleT_TGL.TabIndex = 10;
			this.TextBoxTleT_TGL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetT_TGL
			// 
			this.ButtonTleGetT_TGL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetT_TGL.Location = new System.Drawing.Point(128, 14);
			this.ButtonTleGetT_TGL.Name = "ButtonTleGetT_TGL";
			this.ButtonTleGetT_TGL.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetT_TGL.TabIndex = 9;
			this.ButtonTleGetT_TGL.Text = "T_TGL";
			this.ButtonTleGetT_TGL.Click += new System.EventHandler(this.ButtonTleGetT_TGL_Click);
			// 
			// TextBoxTleT_RAW
			// 
			this.TextBoxTleT_RAW.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleT_RAW.MaxLength = 10;
			this.TextBoxTleT_RAW.Name = "TextBoxTleT_RAW";
			this.TextBoxTleT_RAW.ReadOnly = true;
			this.TextBoxTleT_RAW.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleT_RAW.TabIndex = 8;
			this.TextBoxTleT_RAW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetT_RAW
			// 
			this.ButtonTleGetT_RAW.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetT_RAW.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetT_RAW.Name = "ButtonTleGetT_RAW";
			this.ButtonTleGetT_RAW.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetT_RAW.TabIndex = 7;
			this.ButtonTleGetT_RAW.Text = "T_RAW";
			this.ButtonTleGetT_RAW.Click += new System.EventHandler(this.ButtonTleGetT_RAW_Click);
			// 
			// GroupBoxTleIIF_CNT
			// 
			this.GroupBoxTleIIF_CNT.Controls.Add(this.TextBoxTleIIF_CNT);
			this.GroupBoxTleIIF_CNT.Controls.Add(this.ButtonTleGetIIF_CNT);
			this.GroupBoxTleIIF_CNT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleIIF_CNT.Location = new System.Drawing.Point(694, 318);
			this.GroupBoxTleIIF_CNT.Name = "GroupBoxTleIIF_CNT";
			this.GroupBoxTleIIF_CNT.Size = new System.Drawing.Size(135, 40);
			this.GroupBoxTleIIF_CNT.TabIndex = 56;
			this.GroupBoxTleIIF_CNT.TabStop = false;
			this.GroupBoxTleIIF_CNT.Text = "IIF_CNT - IIF Counter";
			// 
			// TextBoxTleIIF_CNT
			// 
			this.TextBoxTleIIF_CNT.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleIIF_CNT.MaxLength = 10;
			this.TextBoxTleIIF_CNT.Name = "TextBoxTleIIF_CNT";
			this.TextBoxTleIIF_CNT.ReadOnly = true;
			this.TextBoxTleIIF_CNT.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleIIF_CNT.TabIndex = 8;
			this.TextBoxTleIIF_CNT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetIIF_CNT
			// 
			this.ButtonTleGetIIF_CNT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetIIF_CNT.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetIIF_CNT.Name = "ButtonTleGetIIF_CNT";
			this.ButtonTleGetIIF_CNT.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetIIF_CNT.TabIndex = 7;
			this.ButtonTleGetIIF_CNT.Text = "IIF_CNT";
			this.ButtonTleGetIIF_CNT.Click += new System.EventHandler(this.ButtonTleGetIIF_CNT_Click);
			// 
			// GroupBoxTleD_MAG
			// 
			this.GroupBoxTleD_MAG.Controls.Add(this.TextBoxTleMAG);
			this.GroupBoxTleD_MAG.Controls.Add(this.ButtonTleGetMAG);
			this.GroupBoxTleD_MAG.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleD_MAG.Location = new System.Drawing.Point(275, 313);
			this.GroupBoxTleD_MAG.Name = "GroupBoxTleD_MAG";
			this.GroupBoxTleD_MAG.Size = new System.Drawing.Size(135, 40);
			this.GroupBoxTleD_MAG.TabIndex = 54;
			this.GroupBoxTleD_MAG.TabStop = false;
			this.GroupBoxTleD_MAG.Text = "D_MAG";
			// 
			// TextBoxTleMAG
			// 
			this.TextBoxTleMAG.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleMAG.MaxLength = 10;
			this.TextBoxTleMAG.Name = "TextBoxTleMAG";
			this.TextBoxTleMAG.ReadOnly = true;
			this.TextBoxTleMAG.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleMAG.TabIndex = 8;
			this.TextBoxTleMAG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetMAG
			// 
			this.ButtonTleGetMAG.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetMAG.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetMAG.Name = "ButtonTleGetMAG";
			this.ButtonTleGetMAG.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetMAG.TabIndex = 7;
			this.ButtonTleGetMAG.Text = "MAG";
			this.ButtonTleGetMAG.Click += new System.EventHandler(this.ButtonTleGetMAG_Click);
			// 
			// GroupBoxTleADC_Y
			// 
			this.GroupBoxTleADC_Y.Controls.Add(this.TextBoxTleADC_Y);
			this.GroupBoxTleADC_Y.Controls.Add(this.ButtonTleGetADC_Y);
			this.GroupBoxTleADC_Y.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleADC_Y.Location = new System.Drawing.Point(275, 273);
			this.GroupBoxTleADC_Y.Name = "GroupBoxTleADC_Y";
			this.GroupBoxTleADC_Y.Size = new System.Drawing.Size(135, 40);
			this.GroupBoxTleADC_Y.TabIndex = 53;
			this.GroupBoxTleADC_Y.TabStop = false;
			this.GroupBoxTleADC_Y.Text = "ADC_Y - Y-raw value";
			// 
			// TextBoxTleADC_Y
			// 
			this.TextBoxTleADC_Y.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleADC_Y.MaxLength = 10;
			this.TextBoxTleADC_Y.Name = "TextBoxTleADC_Y";
			this.TextBoxTleADC_Y.ReadOnly = true;
			this.TextBoxTleADC_Y.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleADC_Y.TabIndex = 8;
			this.TextBoxTleADC_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetADC_Y
			// 
			this.ButtonTleGetADC_Y.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetADC_Y.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetADC_Y.Name = "ButtonTleGetADC_Y";
			this.ButtonTleGetADC_Y.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetADC_Y.TabIndex = 7;
			this.ButtonTleGetADC_Y.Text = "ADC_Y";
			this.ButtonTleGetADC_Y.Click += new System.EventHandler(this.ButtonTleGetADC_Y_Click);
			// 
			// GroupBoxTleT25O
			// 
			this.GroupBoxTleT25O.Controls.Add(this.TextBoxTleT25O);
			this.GroupBoxTleT25O.Controls.Add(this.ButtonTleGetT25O);
			this.GroupBoxTleT25O.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleT25O.Location = new System.Drawing.Point(830, 318);
			this.GroupBoxTleT25O.Name = "GroupBoxTleT25O";
			this.GroupBoxTleT25O.Size = new System.Drawing.Size(135, 40);
			this.GroupBoxTleT25O.TabIndex = 55;
			this.GroupBoxTleT25O.TabStop = false;
			this.GroupBoxTleT25O.Text = "T25O - 25¢XC offset value";
			// 
			// TextBoxTleT25O
			// 
			this.TextBoxTleT25O.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleT25O.MaxLength = 10;
			this.TextBoxTleT25O.Name = "TextBoxTleT25O";
			this.TextBoxTleT25O.ReadOnly = true;
			this.TextBoxTleT25O.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleT25O.TabIndex = 8;
			this.TextBoxTleT25O.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetT25O
			// 
			this.ButtonTleGetT25O.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetT25O.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetT25O.Name = "ButtonTleGetT25O";
			this.ButtonTleGetT25O.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetT25O.TabIndex = 7;
			this.ButtonTleGetT25O.Text = "T25O";
			this.ButtonTleGetT25O.Click += new System.EventHandler(this.ButtonTleGetT25O_Click);
			// 
			// GroupBoxTleADC_X
			// 
			this.GroupBoxTleADC_X.Controls.Add(this.TextBoxTleADC_X);
			this.GroupBoxTleADC_X.Controls.Add(this.ButtonTleGetADC_X);
			this.GroupBoxTleADC_X.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleADC_X.Location = new System.Drawing.Point(275, 233);
			this.GroupBoxTleADC_X.Name = "GroupBoxTleADC_X";
			this.GroupBoxTleADC_X.Size = new System.Drawing.Size(135, 41);
			this.GroupBoxTleADC_X.TabIndex = 52;
			this.GroupBoxTleADC_X.TabStop = false;
			this.GroupBoxTleADC_X.Text = "ADC_X - X-raw value";
			// 
			// TextBoxTleADC_X
			// 
			this.TextBoxTleADC_X.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleADC_X.MaxLength = 10;
			this.TextBoxTleADC_X.Name = "TextBoxTleADC_X";
			this.TextBoxTleADC_X.ReadOnly = true;
			this.TextBoxTleADC_X.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleADC_X.TabIndex = 8;
			this.TextBoxTleADC_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetADC_X
			// 
			this.ButtonTleGetADC_X.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetADC_X.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetADC_X.Name = "ButtonTleGetADC_X";
			this.ButtonTleGetADC_X.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetADC_X.TabIndex = 7;
			this.ButtonTleGetADC_X.Text = "ADC_X";
			this.ButtonTleGetADC_X.Click += new System.EventHandler(this.ButtonTleGetADC_X_Click);
			// 
			// GroupBoxTleTCO_Y
			// 
			this.GroupBoxTleTCO_Y.Controls.Add(this.TextBoxTleCRC_PAR);
			this.GroupBoxTleTCO_Y.Controls.Add(this.ButtonTleSetCRC_PAR);
			this.GroupBoxTleTCO_Y.Controls.Add(this.TextBoxTleTCO_Y_T);
			this.GroupBoxTleTCO_Y.Controls.Add(this.ButtonTleSetTCO_Y_T);
			this.GroupBoxTleTCO_Y.Controls.Add(this.ButtonTleGetTCO_Y_T);
			this.GroupBoxTleTCO_Y.Controls.Add(this.ComboBoxTleSBIST);
			this.GroupBoxTleTCO_Y.Controls.Add(this.ButtonTleGetSBIST);
			this.GroupBoxTleTCO_Y.Controls.Add(this.ButtonTleGetCRC_PAR);
			this.GroupBoxTleTCO_Y.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleTCO_Y.Location = new System.Drawing.Point(437, 235);
			this.GroupBoxTleTCO_Y.Name = "GroupBoxTleTCO_Y";
			this.GroupBoxTleTCO_Y.Size = new System.Drawing.Size(291, 84);
			this.GroupBoxTleTCO_Y.TabIndex = 71;
			this.GroupBoxTleTCO_Y.TabStop = false;
			this.GroupBoxTleTCO_Y.Text = "TCO_Y - Temperature coefficient Register";
			// 
			// TextBoxTleCRC_PAR
			// 
			this.TextBoxTleCRC_PAR.Location = new System.Drawing.Point(185, 15);
			this.TextBoxTleCRC_PAR.MaxLength = 10;
			this.TextBoxTleCRC_PAR.Name = "TextBoxTleCRC_PAR";
			this.TextBoxTleCRC_PAR.Size = new System.Drawing.Size(102, 21);
			this.TextBoxTleCRC_PAR.TabIndex = 69;
			this.TextBoxTleCRC_PAR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleSetCRC_PAR
			// 
			this.ButtonTleSetCRC_PAR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetCRC_PAR.Location = new System.Drawing.Point(95, 14);
			this.ButtonTleSetCRC_PAR.Name = "ButtonTleSetCRC_PAR";
			this.ButtonTleSetCRC_PAR.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleSetCRC_PAR.TabIndex = 68;
			this.ButtonTleSetCRC_PAR.Text = "Set CRC_PAR";
			this.ButtonTleSetCRC_PAR.Click += new System.EventHandler(this.ButtonTleSetCRC_PAR_Click);
			// 
			// TextBoxTleTCO_Y_T
			// 
			this.TextBoxTleTCO_Y_T.Location = new System.Drawing.Point(185, 59);
			this.TextBoxTleTCO_Y_T.MaxLength = 10;
			this.TextBoxTleTCO_Y_T.Name = "TextBoxTleTCO_Y_T";
			this.TextBoxTleTCO_Y_T.Size = new System.Drawing.Size(102, 21);
			this.TextBoxTleTCO_Y_T.TabIndex = 67;
			this.TextBoxTleTCO_Y_T.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleSetTCO_Y_T
			// 
			this.ButtonTleSetTCO_Y_T.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetTCO_Y_T.Location = new System.Drawing.Point(95, 58);
			this.ButtonTleSetTCO_Y_T.Name = "ButtonTleSetTCO_Y_T";
			this.ButtonTleSetTCO_Y_T.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleSetTCO_Y_T.TabIndex = 66;
			this.ButtonTleSetTCO_Y_T.Text = "Set TCO_Y_T";
			this.ButtonTleSetTCO_Y_T.Click += new System.EventHandler(this.ButtonTleSetTCO_Y_T_Click);
			// 
			// ButtonTleGetTCO_Y_T
			// 
			this.ButtonTleGetTCO_Y_T.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetTCO_Y_T.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetTCO_Y_T.Name = "ButtonTleGetTCO_Y_T";
			this.ButtonTleGetTCO_Y_T.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetTCO_Y_T.TabIndex = 65;
			this.ButtonTleGetTCO_Y_T.Text = "Get TCO_Y_T";
			this.ButtonTleGetTCO_Y_T.Click += new System.EventHandler(this.ButtonTleGetTCO_Y_T_Click);
			// 
			// ComboBoxTleSBIST
			// 
			this.ComboBoxTleSBIST.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleSBIST.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleSBIST.FormattingEnabled = true;
			this.ComboBoxTleSBIST.Items.AddRange(new object[] {
            "Startup-BIST disabled",
            "Startup-BIST enabled"});
			this.ComboBoxTleSBIST.Location = new System.Drawing.Point(98, 35);
			this.ComboBoxTleSBIST.Name = "ComboBoxTleSBIST";
			this.ComboBoxTleSBIST.Size = new System.Drawing.Size(189, 24);
			this.ComboBoxTleSBIST.TabIndex = 62;
			this.ComboBoxTleSBIST.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleSBIST_SelectionChangeCommitted);
			// 
			// ButtonTleGetSBIST
			// 
			this.ButtonTleGetSBIST.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetSBIST.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetSBIST.Name = "ButtonTleGetSBIST";
			this.ButtonTleGetSBIST.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetSBIST.TabIndex = 44;
			this.ButtonTleGetSBIST.Text = "SBIST";
			this.ButtonTleGetSBIST.Click += new System.EventHandler(this.ButtonTleGetSBIST_Click);
			// 
			// ButtonTleGetCRC_PAR
			// 
			this.ButtonTleGetCRC_PAR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetCRC_PAR.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetCRC_PAR.Name = "ButtonTleGetCRC_PAR";
			this.ButtonTleGetCRC_PAR.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetCRC_PAR.TabIndex = 7;
			this.ButtonTleGetCRC_PAR.Text = "Get CRC_PAR";
			this.ButtonTleGetCRC_PAR.Click += new System.EventHandler(this.ButtonTleGetCRC_PAR_Click);
			// 
			// GroupBoxTleMOD_4
			// 
			this.GroupBoxTleMOD_4.Controls.Add(this.TextBoxTleTCO_X_T);
			this.GroupBoxTleMOD_4.Controls.Add(this.ButtonTleSetTCO_X_T);
			this.GroupBoxTleMOD_4.Controls.Add(this.ButtonTleGetTCO_X_T);
			this.GroupBoxTleMOD_4.Controls.Add(this.ComboBoxTleHSM_PLP);
			this.GroupBoxTleMOD_4.Controls.Add(this.ButtonTleGetHSM_PLP);
			this.GroupBoxTleMOD_4.Controls.Add(this.ComboBoxTleIFAB_RES);
			this.GroupBoxTleMOD_4.Controls.Add(this.ComboBoxTleIF_MD);
			this.GroupBoxTleMOD_4.Controls.Add(this.ButtonTleGetIFAB_RES);
			this.GroupBoxTleMOD_4.Controls.Add(this.ButtonTleGetIF_MD);
			this.GroupBoxTleMOD_4.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleMOD_4.Location = new System.Drawing.Point(730, 213);
			this.GroupBoxTleMOD_4.Name = "GroupBoxTleMOD_4";
			this.GroupBoxTleMOD_4.Size = new System.Drawing.Size(292, 106);
			this.GroupBoxTleMOD_4.TabIndex = 70;
			this.GroupBoxTleMOD_4.TabStop = false;
			this.GroupBoxTleMOD_4.Text = "MOD_4 - Interface Mode4 Register";
			// 
			// TextBoxTleTCO_X_T
			// 
			this.TextBoxTleTCO_X_T.Location = new System.Drawing.Point(185, 81);
			this.TextBoxTleTCO_X_T.MaxLength = 10;
			this.TextBoxTleTCO_X_T.Name = "TextBoxTleTCO_X_T";
			this.TextBoxTleTCO_X_T.Size = new System.Drawing.Size(102, 21);
			this.TextBoxTleTCO_X_T.TabIndex = 67;
			this.TextBoxTleTCO_X_T.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleSetTCO_X_T
			// 
			this.ButtonTleSetTCO_X_T.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetTCO_X_T.Location = new System.Drawing.Point(95, 80);
			this.ButtonTleSetTCO_X_T.Name = "ButtonTleSetTCO_X_T";
			this.ButtonTleSetTCO_X_T.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleSetTCO_X_T.TabIndex = 66;
			this.ButtonTleSetTCO_X_T.Text = "Set TCO_X_T";
			this.ButtonTleSetTCO_X_T.Click += new System.EventHandler(this.ButtonTleSetTCO_X_T_Click);
			// 
			// ButtonTleGetTCO_X_T
			// 
			this.ButtonTleGetTCO_X_T.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetTCO_X_T.Location = new System.Drawing.Point(6, 80);
			this.ButtonTleGetTCO_X_T.Name = "ButtonTleGetTCO_X_T";
			this.ButtonTleGetTCO_X_T.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetTCO_X_T.TabIndex = 65;
			this.ButtonTleGetTCO_X_T.Text = "Get TCO_X_T";
			this.ButtonTleGetTCO_X_T.Click += new System.EventHandler(this.ButtonTleGetTCO_X_T_Click);
			// 
			// ComboBoxTleHSM_PLP
			// 
			this.ComboBoxTleHSM_PLP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleHSM_PLP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleHSM_PLP.FormattingEnabled = true;
			this.ComboBoxTleHSM_PLP.Items.AddRange(new object[] {
            "absolute count disabled",
            "absolute count enabled"});
			this.ComboBoxTleHSM_PLP.Location = new System.Drawing.Point(88, 56);
			this.ComboBoxTleHSM_PLP.Name = "ComboBoxTleHSM_PLP";
			this.ComboBoxTleHSM_PLP.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleHSM_PLP.TabIndex = 64;
			this.ComboBoxTleHSM_PLP.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleHSM_PLP_SelectionChangeCommitted);
			// 
			// ButtonTleGetHSM_PLP
			// 
			this.ButtonTleGetHSM_PLP.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetHSM_PLP.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetHSM_PLP.Name = "ButtonTleGetHSM_PLP";
			this.ButtonTleGetHSM_PLP.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetHSM_PLP.TabIndex = 63;
			this.ButtonTleGetHSM_PLP.Text = "HSM_PLP";
			this.ButtonTleGetHSM_PLP.Click += new System.EventHandler(this.ButtonTleGetHSM_PLP_Click);
			// 
			// ComboBoxTleIFAB_RES
			// 
			this.ComboBoxTleIFAB_RES.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleIFAB_RES.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleIFAB_RES.FormattingEnabled = true;
			this.ComboBoxTleIFAB_RES.Items.AddRange(new object[] {
            "12 bit, 0.088¢X step",
            "11 bit, 0.176¢X step",
            "10 bit, 0.352¢X step",
            "9 bit, 0.703¢X step"});
			this.ComboBoxTleIFAB_RES.Location = new System.Drawing.Point(88, 34);
			this.ComboBoxTleIFAB_RES.Name = "ComboBoxTleIFAB_RES";
			this.ComboBoxTleIFAB_RES.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleIFAB_RES.TabIndex = 62;
			this.ComboBoxTleIFAB_RES.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleIFAB_RES_SelectionChangeCommitted);
			// 
			// ComboBoxTleIF_MD
			// 
			this.ComboBoxTleIF_MD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleIF_MD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleIF_MD.FormattingEnabled = true;
			this.ComboBoxTleIF_MD.Items.AddRange(new object[] {
            "IIF",
            "PWM",
            "HSM",
            "SPC"});
			this.ComboBoxTleIF_MD.Location = new System.Drawing.Point(88, 13);
			this.ComboBoxTleIF_MD.Name = "ComboBoxTleIF_MD";
			this.ComboBoxTleIF_MD.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleIF_MD.TabIndex = 61;
			this.ComboBoxTleIF_MD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleIF_MD_SelectionChangeCommitted);
			// 
			// ButtonTleGetIFAB_RES
			// 
			this.ButtonTleGetIFAB_RES.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetIFAB_RES.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetIFAB_RES.Name = "ButtonTleGetIFAB_RES";
			this.ButtonTleGetIFAB_RES.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetIFAB_RES.TabIndex = 44;
			this.ButtonTleGetIFAB_RES.Text = "IFAB_RES";
			this.ButtonTleGetIFAB_RES.Click += new System.EventHandler(this.ButtonTleGetIFAB_RES_Click);
			// 
			// ButtonTleGetIF_MD
			// 
			this.ButtonTleGetIF_MD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetIF_MD.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetIF_MD.Name = "ButtonTleGetIF_MD";
			this.ButtonTleGetIF_MD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetIF_MD.TabIndex = 7;
			this.ButtonTleGetIF_MD.Text = "IF_MD";
			this.ButtonTleGetIF_MD.Click += new System.EventHandler(this.ButtonTleGetIF_MD_Click);
			// 
			// GroupBoxTleIFAB
			// 
			this.GroupBoxTleIFAB.Controls.Add(this.TextBoxTleORTHO);
			this.GroupBoxTleIFAB.Controls.Add(this.ButtonTleSetORTHO);
			this.GroupBoxTleIFAB.Controls.Add(this.ButtonTleGetORTHO);
			this.GroupBoxTleIFAB.Controls.Add(this.ComboBoxTleFIR_UDR);
			this.GroupBoxTleIFAB.Controls.Add(this.ButtonTleGetFIR_UDR);
			this.GroupBoxTleIFAB.Controls.Add(this.ComboBoxTleIFAB_OD);
			this.GroupBoxTleIFAB.Controls.Add(this.ComboBoxTleIFAB_HYST);
			this.GroupBoxTleIFAB.Controls.Add(this.ButtonTleGetIFAB_OD);
			this.GroupBoxTleIFAB.Controls.Add(this.ButtonTleGetIFAB_HYST);
			this.GroupBoxTleIFAB.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleIFAB.Location = new System.Drawing.Point(437, 129);
			this.GroupBoxTleIFAB.Name = "GroupBoxTleIFAB";
			this.GroupBoxTleIFAB.Size = new System.Drawing.Size(292, 106);
			this.GroupBoxTleIFAB.TabIndex = 69;
			this.GroupBoxTleIFAB.TabStop = false;
			this.GroupBoxTleIFAB.Text = "IFAB - IFAB Register";
			// 
			// TextBoxTleORTHO
			// 
			this.TextBoxTleORTHO.Location = new System.Drawing.Point(185, 81);
			this.TextBoxTleORTHO.MaxLength = 10;
			this.TextBoxTleORTHO.Name = "TextBoxTleORTHO";
			this.TextBoxTleORTHO.Size = new System.Drawing.Size(102, 21);
			this.TextBoxTleORTHO.TabIndex = 67;
			this.TextBoxTleORTHO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleSetORTHO
			// 
			this.ButtonTleSetORTHO.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetORTHO.Location = new System.Drawing.Point(95, 80);
			this.ButtonTleSetORTHO.Name = "ButtonTleSetORTHO";
			this.ButtonTleSetORTHO.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleSetORTHO.TabIndex = 66;
			this.ButtonTleSetORTHO.Text = "Set ORTHO";
			this.ButtonTleSetORTHO.Click += new System.EventHandler(this.ButtonTleSetORTHO_Click);
			// 
			// ButtonTleGetORTHO
			// 
			this.ButtonTleGetORTHO.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetORTHO.Location = new System.Drawing.Point(6, 80);
			this.ButtonTleGetORTHO.Name = "ButtonTleGetORTHO";
			this.ButtonTleGetORTHO.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetORTHO.TabIndex = 65;
			this.ButtonTleGetORTHO.Text = "Get ORTHO";
			this.ButtonTleGetORTHO.Click += new System.EventHandler(this.ButtonTleGetORTHO_Click);
			// 
			// ComboBoxTleFIR_UDR
			// 
			this.ComboBoxTleFIR_UDR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleFIR_UDR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleFIR_UDR.FormattingEnabled = true;
			this.ComboBoxTleFIR_UDR.Items.AddRange(new object[] {
            "FIR_MD = \'10\'",
            "FIR_MD = \'01\'"});
			this.ComboBoxTleFIR_UDR.Location = new System.Drawing.Point(88, 56);
			this.ComboBoxTleFIR_UDR.Name = "ComboBoxTleFIR_UDR";
			this.ComboBoxTleFIR_UDR.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleFIR_UDR.TabIndex = 64;
			this.ComboBoxTleFIR_UDR.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleFIR_UDR_SelectionChangeCommitted);
			// 
			// ButtonTleGetFIR_UDR
			// 
			this.ButtonTleGetFIR_UDR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFIR_UDR.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetFIR_UDR.Name = "ButtonTleGetFIR_UDR";
			this.ButtonTleGetFIR_UDR.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetFIR_UDR.TabIndex = 63;
			this.ButtonTleGetFIR_UDR.Text = "FIR_UDR";
			this.ButtonTleGetFIR_UDR.Click += new System.EventHandler(this.ButtonTleGetFIR_UDR_Click);
			// 
			// ComboBoxTleIFAB_OD
			// 
			this.ComboBoxTleIFAB_OD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleIFAB_OD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleIFAB_OD.FormattingEnabled = true;
			this.ComboBoxTleIFAB_OD.Items.AddRange(new object[] {
            "Push-Pull",
            "Open Drain"});
			this.ComboBoxTleIFAB_OD.Location = new System.Drawing.Point(88, 34);
			this.ComboBoxTleIFAB_OD.Name = "ComboBoxTleIFAB_OD";
			this.ComboBoxTleIFAB_OD.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleIFAB_OD.TabIndex = 62;
			this.ComboBoxTleIFAB_OD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleIFAB_OD_SelectionChangeCommitted);
			// 
			// ComboBoxTleIFAB_HYST
			// 
			this.ComboBoxTleIFAB_HYST.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleIFAB_HYST.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleIFAB_HYST.FormattingEnabled = true;
			this.ComboBoxTleIFAB_HYST.Items.AddRange(new object[] {
            "0¢X",
            "0.175¢X",
            "0.35¢X",
            "0.70¢X"});
			this.ComboBoxTleIFAB_HYST.Location = new System.Drawing.Point(88, 13);
			this.ComboBoxTleIFAB_HYST.Name = "ComboBoxTleIFAB_HYST";
			this.ComboBoxTleIFAB_HYST.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleIFAB_HYST.TabIndex = 61;
			this.ComboBoxTleIFAB_HYST.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleIFAB_HYST_SelectionChangeCommitted);
			// 
			// ButtonTleGetIFAB_OD
			// 
			this.ButtonTleGetIFAB_OD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetIFAB_OD.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetIFAB_OD.Name = "ButtonTleGetIFAB_OD";
			this.ButtonTleGetIFAB_OD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetIFAB_OD.TabIndex = 44;
			this.ButtonTleGetIFAB_OD.Text = "IFAB_OD";
			this.ButtonTleGetIFAB_OD.Click += new System.EventHandler(this.ButtonTleGetIFAB_OD_Click);
			// 
			// ButtonTleGetIFAB_HYST
			// 
			this.ButtonTleGetIFAB_HYST.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetIFAB_HYST.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetIFAB_HYST.Name = "ButtonTleGetIFAB_HYST";
			this.ButtonTleGetIFAB_HYST.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetIFAB_HYST.TabIndex = 7;
			this.ButtonTleGetIFAB_HYST.Text = "IFAB_HYST";
			this.ButtonTleGetIFAB_HYST.Click += new System.EventHandler(this.ButtonTleGetIFAB_HYST_Click);
			// 
			// GroupBoxTleSYNCH
			// 
			this.GroupBoxTleSYNCH.Controls.Add(this.ButtonTleSetSYNCH);
			this.GroupBoxTleSYNCH.Controls.Add(this.TextBoxTleSYNCH);
			this.GroupBoxTleSYNCH.Controls.Add(this.ButtonTleGetSYNCH);
			this.GroupBoxTleSYNCH.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleSYNCH.Location = new System.Drawing.Point(275, 193);
			this.GroupBoxTleSYNCH.Name = "GroupBoxTleSYNCH";
			this.GroupBoxTleSYNCH.Size = new System.Drawing.Size(135, 41);
			this.GroupBoxTleSYNCH.TabIndex = 54;
			this.GroupBoxTleSYNCH.TabStop = false;
			this.GroupBoxTleSYNCH.Text = "SYNCH - Synchronicity";
			// 
			// ButtonTleSetSYNCH
			// 
			this.ButtonTleSetSYNCH.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetSYNCH.Location = new System.Drawing.Point(45, 15);
			this.ButtonTleSetSYNCH.Name = "ButtonTleSetSYNCH";
			this.ButtonTleSetSYNCH.Size = new System.Drawing.Size(40, 22);
			this.ButtonTleSetSYNCH.TabIndex = 9;
			this.ButtonTleSetSYNCH.Text = "Set";
			this.ButtonTleSetSYNCH.Click += new System.EventHandler(this.ButtonTleSetSYNCH_Click);
			// 
			// TextBoxTleSYNCH
			// 
			this.TextBoxTleSYNCH.Location = new System.Drawing.Point(86, 16);
			this.TextBoxTleSYNCH.MaxLength = 10;
			this.TextBoxTleSYNCH.Name = "TextBoxTleSYNCH";
			this.TextBoxTleSYNCH.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleSYNCH.TabIndex = 8;
			this.TextBoxTleSYNCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetSYNCH
			// 
			this.ButtonTleGetSYNCH.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetSYNCH.Location = new System.Drawing.Point(6, 15);
			this.ButtonTleGetSYNCH.Name = "ButtonTleGetSYNCH";
			this.ButtonTleGetSYNCH.Size = new System.Drawing.Size(40, 22);
			this.ButtonTleGetSYNCH.TabIndex = 7;
			this.ButtonTleGetSYNCH.Text = "Get";
			this.ButtonTleGetSYNCH.Click += new System.EventHandler(this.ButtonTleGetSYNCH_Click);
			// 
			// GroupBoxTleOFFSET_Y
			// 
			this.GroupBoxTleOFFSET_Y.Controls.Add(this.ButtonTleSetOFFSET_Y);
			this.GroupBoxTleOFFSET_Y.Controls.Add(this.TextBoxTleOFFY);
			this.GroupBoxTleOFFSET_Y.Controls.Add(this.ButtonTleGetOFFSET_Y);
			this.GroupBoxTleOFFSET_Y.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleOFFSET_Y.Location = new System.Drawing.Point(275, 151);
			this.GroupBoxTleOFFSET_Y.Name = "GroupBoxTleOFFSET_Y";
			this.GroupBoxTleOFFSET_Y.Size = new System.Drawing.Size(135, 41);
			this.GroupBoxTleOFFSET_Y.TabIndex = 53;
			this.GroupBoxTleOFFSET_Y.TabStop = false;
			this.GroupBoxTleOFFSET_Y.Text = "OFFY - Offset Y";
			// 
			// ButtonTleSetOFFSET_Y
			// 
			this.ButtonTleSetOFFSET_Y.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetOFFSET_Y.Location = new System.Drawing.Point(45, 15);
			this.ButtonTleSetOFFSET_Y.Name = "ButtonTleSetOFFSET_Y";
			this.ButtonTleSetOFFSET_Y.Size = new System.Drawing.Size(40, 22);
			this.ButtonTleSetOFFSET_Y.TabIndex = 9;
			this.ButtonTleSetOFFSET_Y.Text = "Set";
			this.ButtonTleSetOFFSET_Y.Click += new System.EventHandler(this.ButtonTleSetOFFSET_Y_Click);
			// 
			// TextBoxTleOFFY
			// 
			this.TextBoxTleOFFY.Location = new System.Drawing.Point(86, 16);
			this.TextBoxTleOFFY.MaxLength = 10;
			this.TextBoxTleOFFY.Name = "TextBoxTleOFFY";
			this.TextBoxTleOFFY.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleOFFY.TabIndex = 8;
			this.TextBoxTleOFFY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetOFFSET_Y
			// 
			this.ButtonTleGetOFFSET_Y.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetOFFSET_Y.Location = new System.Drawing.Point(6, 15);
			this.ButtonTleGetOFFSET_Y.Name = "ButtonTleGetOFFSET_Y";
			this.ButtonTleGetOFFSET_Y.Size = new System.Drawing.Size(40, 22);
			this.ButtonTleGetOFFSET_Y.TabIndex = 7;
			this.ButtonTleGetOFFSET_Y.Text = "Get";
			this.ButtonTleGetOFFSET_Y.Click += new System.EventHandler(this.ButtonTleGetOFFSET_Y_Click);
			// 
			// GroupBoxTleOFFX
			// 
			this.GroupBoxTleOFFX.Controls.Add(this.ButtonTleSetOFFSET_X);
			this.GroupBoxTleOFFX.Controls.Add(this.TextBoxTleOFFX);
			this.GroupBoxTleOFFX.Controls.Add(this.ButtonTleGetOFFSET_X);
			this.GroupBoxTleOFFX.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleOFFX.Location = new System.Drawing.Point(275, 108);
			this.GroupBoxTleOFFX.Name = "GroupBoxTleOFFX";
			this.GroupBoxTleOFFX.Size = new System.Drawing.Size(135, 41);
			this.GroupBoxTleOFFX.TabIndex = 52;
			this.GroupBoxTleOFFX.TabStop = false;
			this.GroupBoxTleOFFX.Text = "OFFX - Offset X";
			// 
			// ButtonTleSetOFFSET_X
			// 
			this.ButtonTleSetOFFSET_X.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetOFFSET_X.Location = new System.Drawing.Point(45, 15);
			this.ButtonTleSetOFFSET_X.Name = "ButtonTleSetOFFSET_X";
			this.ButtonTleSetOFFSET_X.Size = new System.Drawing.Size(40, 22);
			this.ButtonTleSetOFFSET_X.TabIndex = 9;
			this.ButtonTleSetOFFSET_X.Text = "Set";
			this.ButtonTleSetOFFSET_X.Click += new System.EventHandler(this.ButtonTleSetOFFSET_X_Click);
			// 
			// TextBoxTleOFFX
			// 
			this.TextBoxTleOFFX.Location = new System.Drawing.Point(86, 16);
			this.TextBoxTleOFFX.MaxLength = 10;
			this.TextBoxTleOFFX.Name = "TextBoxTleOFFX";
			this.TextBoxTleOFFX.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleOFFX.TabIndex = 8;
			this.TextBoxTleOFFX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetOFFSET_X
			// 
			this.ButtonTleGetOFFSET_X.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetOFFSET_X.Location = new System.Drawing.Point(6, 15);
			this.ButtonTleGetOFFSET_X.Name = "ButtonTleGetOFFSET_X";
			this.ButtonTleGetOFFSET_X.Size = new System.Drawing.Size(40, 22);
			this.ButtonTleGetOFFSET_X.TabIndex = 7;
			this.ButtonTleGetOFFSET_X.Text = "Get";
			this.ButtonTleGetOFFSET_X.Click += new System.EventHandler(this.ButtonTleGetOFFSET_X_Click);
			// 
			// GroupBoxTleMOD_3
			// 
			this.GroupBoxTleMOD_3.Controls.Add(this.TextBoxTleANG_BASE);
			this.GroupBoxTleMOD_3.Controls.Add(this.ButtonTleSetANG_BASE);
			this.GroupBoxTleMOD_3.Controls.Add(this.ButtonTleGetANG_BASE);
			this.GroupBoxTleMOD_3.Controls.Add(this.ComboBoxTleSPIKEF);
			this.GroupBoxTleMOD_3.Controls.Add(this.ButtonTleGetSPIKEF);
			this.GroupBoxTleMOD_3.Controls.Add(this.ComboBoxTleSSC_OD);
			this.GroupBoxTleMOD_3.Controls.Add(this.ComboBoxTlePAD_DRV);
			this.GroupBoxTleMOD_3.Controls.Add(this.ButtonTleGetSSC_OD);
			this.GroupBoxTleMOD_3.Controls.Add(this.ButtonTleGetPAD_DRV);
			this.GroupBoxTleMOD_3.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleMOD_3.Location = new System.Drawing.Point(730, 108);
			this.GroupBoxTleMOD_3.Name = "GroupBoxTleMOD_3";
			this.GroupBoxTleMOD_3.Size = new System.Drawing.Size(292, 106);
			this.GroupBoxTleMOD_3.TabIndex = 68;
			this.GroupBoxTleMOD_3.TabStop = false;
			this.GroupBoxTleMOD_3.Text = "MOD_3 - Interface Mode3 Register";
			// 
			// TextBoxTleANG_BASE
			// 
			this.TextBoxTleANG_BASE.Location = new System.Drawing.Point(185, 81);
			this.TextBoxTleANG_BASE.MaxLength = 10;
			this.TextBoxTleANG_BASE.Name = "TextBoxTleANG_BASE";
			this.TextBoxTleANG_BASE.Size = new System.Drawing.Size(102, 21);
			this.TextBoxTleANG_BASE.TabIndex = 67;
			this.TextBoxTleANG_BASE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleSetANG_BASE
			// 
			this.ButtonTleSetANG_BASE.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetANG_BASE.Location = new System.Drawing.Point(95, 80);
			this.ButtonTleSetANG_BASE.Name = "ButtonTleSetANG_BASE";
			this.ButtonTleSetANG_BASE.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleSetANG_BASE.TabIndex = 66;
			this.ButtonTleSetANG_BASE.Text = "Set ANG_BASE";
			this.ButtonTleSetANG_BASE.Click += new System.EventHandler(this.ButtonTleSetANG_BASE_Click);
			// 
			// ButtonTleGetANG_BASE
			// 
			this.ButtonTleGetANG_BASE.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetANG_BASE.Location = new System.Drawing.Point(6, 80);
			this.ButtonTleGetANG_BASE.Name = "ButtonTleGetANG_BASE";
			this.ButtonTleGetANG_BASE.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetANG_BASE.TabIndex = 65;
			this.ButtonTleGetANG_BASE.Text = "Get ANG_BASE";
			this.ButtonTleGetANG_BASE.Click += new System.EventHandler(this.ButtonTleGetANG_BASE_Click);
			// 
			// ComboBoxTleSPIKEF
			// 
			this.ComboBoxTleSPIKEF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleSPIKEF.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleSPIKEF.FormattingEnabled = true;
			this.ComboBoxTleSPIKEF.Items.AddRange(new object[] {
            "spike filter disabled",
            "spike filter enabled"});
			this.ComboBoxTleSPIKEF.Location = new System.Drawing.Point(88, 56);
			this.ComboBoxTleSPIKEF.Name = "ComboBoxTleSPIKEF";
			this.ComboBoxTleSPIKEF.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleSPIKEF.TabIndex = 64;
			this.ComboBoxTleSPIKEF.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxSPIKEF_SelectionChangeCommitted);
			// 
			// ButtonTleGetSPIKEF
			// 
			this.ButtonTleGetSPIKEF.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetSPIKEF.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetSPIKEF.Name = "ButtonTleGetSPIKEF";
			this.ButtonTleGetSPIKEF.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetSPIKEF.TabIndex = 63;
			this.ButtonTleGetSPIKEF.Text = "SPIKEF";
			this.ButtonTleGetSPIKEF.Click += new System.EventHandler(this.ButtonTleGetSPIKEF_Click);
			// 
			// ComboBoxTleSSC_OD
			// 
			this.ComboBoxTleSSC_OD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleSSC_OD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleSSC_OD.FormattingEnabled = true;
			this.ComboBoxTleSSC_OD.Items.AddRange(new object[] {
            "Push-Pull",
            "Open Drain"});
			this.ComboBoxTleSSC_OD.Location = new System.Drawing.Point(88, 34);
			this.ComboBoxTleSSC_OD.Name = "ComboBoxTleSSC_OD";
			this.ComboBoxTleSSC_OD.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleSSC_OD.TabIndex = 62;
			this.ComboBoxTleSSC_OD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleSSC_OD_SelectionChangeCommitted);
			// 
			// ComboBoxTlePAD_DRV
			// 
			this.ComboBoxTlePAD_DRV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTlePAD_DRV.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTlePAD_DRV.FormattingEnabled = true;
			this.ComboBoxTlePAD_DRV.Items.AddRange(new object[] {
            "IFA/B/B: strong, DATA: strong, fast",
            "IFA/B/B: strong, DATA: strong, slow",
            "IFA/B/B: weak, DATA: medium, fast",
            "IFA/B/B: weak, DATA: weak, slow"});
			this.ComboBoxTlePAD_DRV.Location = new System.Drawing.Point(88, 13);
			this.ComboBoxTlePAD_DRV.Name = "ComboBoxTlePAD_DRV";
			this.ComboBoxTlePAD_DRV.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTlePAD_DRV.TabIndex = 61;
			this.ComboBoxTlePAD_DRV.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTlePAD_DRV_SelectionChangeCommitted);
			// 
			// ButtonTleGetSSC_OD
			// 
			this.ButtonTleGetSSC_OD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetSSC_OD.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetSSC_OD.Name = "ButtonTleGetSSC_OD";
			this.ButtonTleGetSSC_OD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetSSC_OD.TabIndex = 44;
			this.ButtonTleGetSSC_OD.Text = "SSC_OD";
			this.ButtonTleGetSSC_OD.Click += new System.EventHandler(this.ButtonTleGetSSC_OD_Click);
			// 
			// ButtonTleGetPAD_DRV
			// 
			this.ButtonTleGetPAD_DRV.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetPAD_DRV.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetPAD_DRV.Name = "ButtonTleGetPAD_DRV";
			this.ButtonTleGetPAD_DRV.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetPAD_DRV.TabIndex = 7;
			this.ButtonTleGetPAD_DRV.Text = "PAD_DRV";
			this.ButtonTleGetPAD_DRV.Click += new System.EventHandler(this.ButtonTleGetPAD_DRV_Click);
			// 
			// GroupBoxTleMOD_2
			// 
			this.GroupBoxTleMOD_2.Controls.Add(this.TextBoxTleANG_RANGE);
			this.GroupBoxTleMOD_2.Controls.Add(this.ButtonTleSetANGLE_RANGE);
			this.GroupBoxTleMOD_2.Controls.Add(this.ButtonTleGetANG_RANGE);
			this.GroupBoxTleMOD_2.Controls.Add(this.ComboBoxTleANG_DIR);
			this.GroupBoxTleMOD_2.Controls.Add(this.ButtonTleGetANG_DIR);
			this.GroupBoxTleMOD_2.Controls.Add(this.ComboBoxTlePREDICT);
			this.GroupBoxTleMOD_2.Controls.Add(this.ComboBoxTleAUTOCAL);
			this.GroupBoxTleMOD_2.Controls.Add(this.ButtonTleGetPREDICT);
			this.GroupBoxTleMOD_2.Controls.Add(this.ButtonTleGetAUTOCAL);
			this.GroupBoxTleMOD_2.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleMOD_2.Location = new System.Drawing.Point(730, 3);
			this.GroupBoxTleMOD_2.Name = "GroupBoxTleMOD_2";
			this.GroupBoxTleMOD_2.Size = new System.Drawing.Size(292, 106);
			this.GroupBoxTleMOD_2.TabIndex = 67;
			this.GroupBoxTleMOD_2.TabStop = false;
			this.GroupBoxTleMOD_2.Text = "MOD_2 - Interface Mode2 Register";
			// 
			// TextBoxTleANG_RANGE
			// 
			this.TextBoxTleANG_RANGE.Location = new System.Drawing.Point(185, 81);
			this.TextBoxTleANG_RANGE.MaxLength = 10;
			this.TextBoxTleANG_RANGE.Name = "TextBoxTleANG_RANGE";
			this.TextBoxTleANG_RANGE.Size = new System.Drawing.Size(102, 21);
			this.TextBoxTleANG_RANGE.TabIndex = 45;
			this.TextBoxTleANG_RANGE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleSetANGLE_RANGE
			// 
			this.ButtonTleSetANGLE_RANGE.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleSetANGLE_RANGE.Location = new System.Drawing.Point(95, 80);
			this.ButtonTleSetANGLE_RANGE.Name = "ButtonTleSetANGLE_RANGE";
			this.ButtonTleSetANGLE_RANGE.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleSetANGLE_RANGE.TabIndex = 66;
			this.ButtonTleSetANGLE_RANGE.Text = "Set ANG_RANGE";
			this.ButtonTleSetANGLE_RANGE.Click += new System.EventHandler(this.ButtonTleSetANGLE_RANGE_Click);
			// 
			// ButtonTleGetANG_RANGE
			// 
			this.ButtonTleGetANG_RANGE.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetANG_RANGE.Location = new System.Drawing.Point(6, 80);
			this.ButtonTleGetANG_RANGE.Name = "ButtonTleGetANG_RANGE";
			this.ButtonTleGetANG_RANGE.Size = new System.Drawing.Size(90, 22);
			this.ButtonTleGetANG_RANGE.TabIndex = 65;
			this.ButtonTleGetANG_RANGE.Text = "Get ANG_RANGE";
			this.ButtonTleGetANG_RANGE.Click += new System.EventHandler(this.ButtonTleGetANG_RANGE_Click);
			// 
			// ComboBoxTleANG_DIR
			// 
			this.ComboBoxTleANG_DIR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleANG_DIR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleANG_DIR.FormattingEnabled = true;
			this.ComboBoxTleANG_DIR.Items.AddRange(new object[] {
            "CCW rotation of magnet",
            "CW rotation of magnet"});
			this.ComboBoxTleANG_DIR.Location = new System.Drawing.Point(88, 56);
			this.ComboBoxTleANG_DIR.Name = "ComboBoxTleANG_DIR";
			this.ComboBoxTleANG_DIR.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleANG_DIR.TabIndex = 64;
			this.ComboBoxTleANG_DIR.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleANG_DIR_SelectionChangeCommitted);
			// 
			// ButtonTleGetANG_DIR
			// 
			this.ButtonTleGetANG_DIR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetANG_DIR.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetANG_DIR.Name = "ButtonTleGetANG_DIR";
			this.ButtonTleGetANG_DIR.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetANG_DIR.TabIndex = 63;
			this.ButtonTleGetANG_DIR.Text = "ANG_DIR";
			this.ButtonTleGetANG_DIR.Click += new System.EventHandler(this.ButtonTleGetANG_DIR_Click);
			// 
			// ComboBoxTlePREDICT
			// 
			this.ComboBoxTlePREDICT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTlePREDICT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTlePREDICT.FormattingEnabled = true;
			this.ComboBoxTlePREDICT.Items.AddRange(new object[] {
            "prediction disabled",
            "prediction enabled"});
			this.ComboBoxTlePREDICT.Location = new System.Drawing.Point(88, 34);
			this.ComboBoxTlePREDICT.Name = "ComboBoxTlePREDICT";
			this.ComboBoxTlePREDICT.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTlePREDICT.TabIndex = 62;
			this.ComboBoxTlePREDICT.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTlePREDICT_SelectionChangeCommitted);
			// 
			// ComboBoxTleAUTOCAL
			// 
			this.ComboBoxTleAUTOCAL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAUTOCAL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAUTOCAL.FormattingEnabled = true;
			this.ComboBoxTleAUTOCAL.Items.AddRange(new object[] {
            "no auto-calibration",
            "update every angle update cycle",
            "update every 1.5 revolution",
            "update every 11.25 degree"});
			this.ComboBoxTleAUTOCAL.Location = new System.Drawing.Point(88, 13);
			this.ComboBoxTleAUTOCAL.Name = "ComboBoxTleAUTOCAL";
			this.ComboBoxTleAUTOCAL.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleAUTOCAL.TabIndex = 61;
			this.ComboBoxTleAUTOCAL.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAUTOCAL_SelectionChangeCommitted);
			// 
			// ButtonTleGetPREDICT
			// 
			this.ButtonTleGetPREDICT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetPREDICT.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetPREDICT.Name = "ButtonTleGetPREDICT";
			this.ButtonTleGetPREDICT.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetPREDICT.TabIndex = 44;
			this.ButtonTleGetPREDICT.Text = "PREDICT";
			this.ButtonTleGetPREDICT.Click += new System.EventHandler(this.ButtonTleGetPREDICT_Click);
			// 
			// ButtonTleGetAUTOCAL
			// 
			this.ButtonTleGetAUTOCAL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAUTOCAL.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetAUTOCAL.Name = "ButtonTleGetAUTOCAL";
			this.ButtonTleGetAUTOCAL.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAUTOCAL.TabIndex = 7;
			this.ButtonTleGetAUTOCAL.Text = "AUTOCAL";
			this.ButtonTleGetAUTOCAL.Click += new System.EventHandler(this.ButtonTleGetAUTOCAL_Click);
			// 
			// GroupBoxTleSIL
			// 
			this.GroupBoxTleSIL.Controls.Add(this.ComboBoxTleFILT_PAR);
			this.GroupBoxTleSIL.Controls.Add(this.ButtonTleGetFILT_PAR);
			this.GroupBoxTleSIL.Controls.Add(this.ComboBoxTleFILT_INV);
			this.GroupBoxTleSIL.Controls.Add(this.ButtonTleGetFILT_INV);
			this.GroupBoxTleSIL.Controls.Add(this.ComboBoxTleFUSE_REL);
			this.GroupBoxTleSIL.Controls.Add(this.ButtonTleGetFUSE_REL);
			this.GroupBoxTleSIL.Controls.Add(this.ComboBoxTleADCTV_EN);
			this.GroupBoxTleSIL.Controls.Add(this.ButtonTleGetADCTV_EN);
			this.GroupBoxTleSIL.Controls.Add(this.ComboBoxTleADCTV_Y);
			this.GroupBoxTleSIL.Controls.Add(this.ButtonTleGetADCTV_X);
			this.GroupBoxTleSIL.Controls.Add(this.ComboBoxTleADCTV_X);
			this.GroupBoxTleSIL.Controls.Add(this.ButtonTleGetADCTV_Y);
			this.GroupBoxTleSIL.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleSIL.Location = new System.Drawing.Point(437, 3);
			this.GroupBoxTleSIL.Name = "GroupBoxTleSIL";
			this.GroupBoxTleSIL.Size = new System.Drawing.Size(292, 126);
			this.GroupBoxTleSIL.TabIndex = 67;
			this.GroupBoxTleSIL.TabStop = false;
			this.GroupBoxTleSIL.Text = "SIL - SIL Register";
			// 
			// ComboBoxTleFILT_PAR
			// 
			this.ComboBoxTleFILT_PAR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleFILT_PAR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleFILT_PAR.FormattingEnabled = true;
			this.ComboBoxTleFILT_PAR.Items.AddRange(new object[] {
            "filter parallel disabled",
            "filter parallel enabled"});
			this.ComboBoxTleFILT_PAR.Location = new System.Drawing.Point(88, 96);
			this.ComboBoxTleFILT_PAR.Name = "ComboBoxTleFILT_PAR";
			this.ComboBoxTleFILT_PAR.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleFILT_PAR.TabIndex = 70;
			this.ComboBoxTleFILT_PAR.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleFILT_PAR_SelectionChangeCommitted);
			// 
			// ButtonTleGetFILT_PAR
			// 
			this.ButtonTleGetFILT_PAR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFILT_PAR.Location = new System.Drawing.Point(6, 100);
			this.ButtonTleGetFILT_PAR.Name = "ButtonTleGetFILT_PAR";
			this.ButtonTleGetFILT_PAR.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetFILT_PAR.TabIndex = 69;
			this.ButtonTleGetFILT_PAR.Text = "FUSE_PAR";
			this.ButtonTleGetFILT_PAR.Click += new System.EventHandler(this.ButtonTleGetFILT_PAR_Click);
			// 
			// ComboBoxTleFILT_INV
			// 
			this.ComboBoxTleFILT_INV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleFILT_INV.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleFILT_INV.FormattingEnabled = true;
			this.ComboBoxTleFILT_INV.Items.AddRange(new object[] {
            "filter inverted disabled",
            "filter inverted enabled"});
			this.ComboBoxTleFILT_INV.Location = new System.Drawing.Point(88, 76);
			this.ComboBoxTleFILT_INV.Name = "ComboBoxTleFILT_INV";
			this.ComboBoxTleFILT_INV.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleFILT_INV.TabIndex = 68;
			this.ComboBoxTleFILT_INV.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleFILT_INV_SelectionChangeCommitted);
			// 
			// ButtonTleGetFILT_INV
			// 
			this.ButtonTleGetFILT_INV.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFILT_INV.Location = new System.Drawing.Point(6, 78);
			this.ButtonTleGetFILT_INV.Name = "ButtonTleGetFILT_INV";
			this.ButtonTleGetFILT_INV.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetFILT_INV.TabIndex = 67;
			this.ButtonTleGetFILT_INV.Text = "FILT_INV";
			this.ButtonTleGetFILT_INV.Click += new System.EventHandler(this.ButtonTleGetFILT_INV_Click);
			// 
			// ComboBoxTleFUSE_REL
			// 
			this.ComboBoxTleFUSE_REL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleFUSE_REL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleFUSE_REL.FormattingEnabled = true;
			this.ComboBoxTleFUSE_REL.Items.AddRange(new object[] {
            "normal operation",
            "reload of registers with fuse values immed."});
			this.ComboBoxTleFUSE_REL.Location = new System.Drawing.Point(88, 56);
			this.ComboBoxTleFUSE_REL.Name = "ComboBoxTleFUSE_REL";
			this.ComboBoxTleFUSE_REL.Size = new System.Drawing.Size(199, 24);
			this.ComboBoxTleFUSE_REL.TabIndex = 66;
			this.ComboBoxTleFUSE_REL.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleFUSE_REL_SelectionChangeCommitted);
			// 
			// ButtonTleGetFUSE_REL
			// 
			this.ButtonTleGetFUSE_REL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFUSE_REL.Location = new System.Drawing.Point(6, 56);
			this.ButtonTleGetFUSE_REL.Name = "ButtonTleGetFUSE_REL";
			this.ButtonTleGetFUSE_REL.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetFUSE_REL.TabIndex = 65;
			this.ButtonTleGetFUSE_REL.Text = "FUSE_REL";
			this.ButtonTleGetFUSE_REL.Click += new System.EventHandler(this.ButtonTleGetFUSE_REL_Click);
			// 
			// ComboBoxTleADCTV_EN
			// 
			this.ComboBoxTleADCTV_EN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleADCTV_EN.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleADCTV_EN.FormattingEnabled = true;
			this.ComboBoxTleADCTV_EN.Items.AddRange(new object[] {
            "ADC-Test Vectors disabled",
            "ADC-Test Vectors enabled"});
			this.ComboBoxTleADCTV_EN.Location = new System.Drawing.Point(7, 36);
			this.ComboBoxTleADCTV_EN.Name = "ComboBoxTleADCTV_EN";
			this.ComboBoxTleADCTV_EN.Size = new System.Drawing.Size(142, 24);
			this.ComboBoxTleADCTV_EN.TabIndex = 64;
			this.ComboBoxTleADCTV_EN.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleADCTV_EN_SelectionChangeCommitted);
			// 
			// ButtonTleGetADCTV_EN
			// 
			this.ButtonTleGetADCTV_EN.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetADCTV_EN.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetADCTV_EN.Name = "ButtonTleGetADCTV_EN";
			this.ButtonTleGetADCTV_EN.Size = new System.Drawing.Size(143, 22);
			this.ButtonTleGetADCTV_EN.TabIndex = 63;
			this.ButtonTleGetADCTV_EN.Text = "ADCTV_EN";
			this.ButtonTleGetADCTV_EN.Click += new System.EventHandler(this.ButtonTleGetADCTV_EN_Click);
			// 
			// ComboBoxTleADCTV_Y
			// 
			this.ComboBoxTleADCTV_Y.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleADCTV_Y.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleADCTV_Y.FormattingEnabled = true;
			this.ComboBoxTleADCTV_Y.Items.AddRange(new object[] {
            "0V",
            "+70%",
            "+100%",
            "+Overflow",
            "NA",
            "-70%",
            "-100%",
            "-Overflow"});
			this.ComboBoxTleADCTV_Y.Location = new System.Drawing.Point(217, 36);
			this.ComboBoxTleADCTV_Y.Name = "ComboBoxTleADCTV_Y";
			this.ComboBoxTleADCTV_Y.Size = new System.Drawing.Size(70, 24);
			this.ComboBoxTleADCTV_Y.TabIndex = 62;
			// 
			// ButtonTleGetADCTV_X
			// 
			this.ButtonTleGetADCTV_X.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetADCTV_X.Location = new System.Drawing.Point(148, 14);
			this.ButtonTleGetADCTV_X.Name = "ButtonTleGetADCTV_X";
			this.ButtonTleGetADCTV_X.Size = new System.Drawing.Size(70, 22);
			this.ButtonTleGetADCTV_X.TabIndex = 7;
			this.ButtonTleGetADCTV_X.Text = "ADCTV_X";
			this.ButtonTleGetADCTV_X.Click += new System.EventHandler(this.ButtonTleGetADCTV_X_Click);
			// 
			// ComboBoxTleADCTV_X
			// 
			this.ComboBoxTleADCTV_X.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleADCTV_X.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleADCTV_X.FormattingEnabled = true;
			this.ComboBoxTleADCTV_X.Items.AddRange(new object[] {
            "0V",
            "+70%",
            "+100%",
            "+Overflow",
            "NA",
            "-70%",
            "-100%",
            "-Overflow"});
			this.ComboBoxTleADCTV_X.Location = new System.Drawing.Point(148, 36);
			this.ComboBoxTleADCTV_X.Name = "ComboBoxTleADCTV_X";
			this.ComboBoxTleADCTV_X.Size = new System.Drawing.Size(70, 24);
			this.ComboBoxTleADCTV_X.TabIndex = 61;
			// 
			// ButtonTleGetADCTV_Y
			// 
			this.ButtonTleGetADCTV_Y.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetADCTV_Y.Location = new System.Drawing.Point(217, 14);
			this.ButtonTleGetADCTV_Y.Name = "ButtonTleGetADCTV_Y";
			this.ButtonTleGetADCTV_Y.Size = new System.Drawing.Size(70, 22);
			this.ButtonTleGetADCTV_Y.TabIndex = 44;
			this.ButtonTleGetADCTV_Y.Text = "ADCTV_Y";
			this.ButtonTleGetADCTV_Y.Click += new System.EventHandler(this.ButtonTleGetADCTV_Y_Click);
			// 
			// GroupBoxTleMOD_1
			// 
			this.GroupBoxTleMOD_1.Controls.Add(this.ComboBoxTleFIR_MD);
			this.GroupBoxTleMOD_1.Controls.Add(this.ButtonTleGetFIR_MD);
			this.GroupBoxTleMOD_1.Controls.Add(this.ComboBoxTleCLK_SEL);
			this.GroupBoxTleMOD_1.Controls.Add(this.ButtonTleGetCLK_SEL);
			this.GroupBoxTleMOD_1.Controls.Add(this.ComboBoxTleDSPU_HOLD);
			this.GroupBoxTleMOD_1.Controls.Add(this.ComboBoxTleIIF_MOD);
			this.GroupBoxTleMOD_1.Controls.Add(this.ButtonTleGetDSPU_HOLD);
			this.GroupBoxTleMOD_1.Controls.Add(this.ButtonTleGetIIF_MOD);
			this.GroupBoxTleMOD_1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleMOD_1.Location = new System.Drawing.Point(4, 273);
			this.GroupBoxTleMOD_1.Name = "GroupBoxTleMOD_1";
			this.GroupBoxTleMOD_1.Size = new System.Drawing.Size(270, 106);
			this.GroupBoxTleMOD_1.TabIndex = 52;
			this.GroupBoxTleMOD_1.TabStop = false;
			this.GroupBoxTleMOD_1.Text = "MOD_1 - Interface Mode1 Register";
			// 
			// ComboBoxTleFIR_MD
			// 
			this.ComboBoxTleFIR_MD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleFIR_MD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleFIR_MD.FormattingEnabled = true;
			this.ComboBoxTleFIR_MD.Items.AddRange(new object[] {
            "NA",
            "42.7 us",
            "85.3 us",
            "170.6 us"});
			this.ComboBoxTleFIR_MD.Location = new System.Drawing.Point(88, 78);
			this.ComboBoxTleFIR_MD.Name = "ComboBoxTleFIR_MD";
			this.ComboBoxTleFIR_MD.Size = new System.Drawing.Size(175, 24);
			this.ComboBoxTleFIR_MD.TabIndex = 66;
			this.ComboBoxTleFIR_MD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleFIR_MD_SelectionChangeCommitted);
			// 
			// ButtonTleGetFIR_MD
			// 
			this.ButtonTleGetFIR_MD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFIR_MD.Location = new System.Drawing.Point(6, 80);
			this.ButtonTleGetFIR_MD.Name = "ButtonTleGetFIR_MD";
			this.ButtonTleGetFIR_MD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetFIR_MD.TabIndex = 65;
			this.ButtonTleGetFIR_MD.Text = "FIR_MD";
			this.ButtonTleGetFIR_MD.Click += new System.EventHandler(this.ButtonTleGetFIR_MD_Click);
			// 
			// ComboBoxTleCLK_SEL
			// 
			this.ComboBoxTleCLK_SEL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleCLK_SEL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleCLK_SEL.FormattingEnabled = true;
			this.ComboBoxTleCLK_SEL.Items.AddRange(new object[] {
            "internal oscillator",
            "external 4-MHz clock"});
			this.ComboBoxTleCLK_SEL.Location = new System.Drawing.Point(88, 56);
			this.ComboBoxTleCLK_SEL.Name = "ComboBoxTleCLK_SEL";
			this.ComboBoxTleCLK_SEL.Size = new System.Drawing.Size(175, 24);
			this.ComboBoxTleCLK_SEL.TabIndex = 64;
			this.ComboBoxTleCLK_SEL.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleCLK_SEL_SelectionChangeCommitted);
			// 
			// ButtonTleGetCLK_SEL
			// 
			this.ButtonTleGetCLK_SEL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetCLK_SEL.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetCLK_SEL.Name = "ButtonTleGetCLK_SEL";
			this.ButtonTleGetCLK_SEL.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetCLK_SEL.TabIndex = 63;
			this.ButtonTleGetCLK_SEL.Text = "CLK_SEL";
			this.ButtonTleGetCLK_SEL.Click += new System.EventHandler(this.ButtonTleGetCLK_SEL_Click);
			// 
			// ComboBoxTleDSPU_HOLD
			// 
			this.ComboBoxTleDSPU_HOLD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleDSPU_HOLD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleDSPU_HOLD.FormattingEnabled = true;
			this.ComboBoxTleDSPU_HOLD.Items.AddRange(new object[] {
            "DSPU in normal schedule operation",
            "DSPU is on hold"});
			this.ComboBoxTleDSPU_HOLD.Location = new System.Drawing.Point(88, 34);
			this.ComboBoxTleDSPU_HOLD.Name = "ComboBoxTleDSPU_HOLD";
			this.ComboBoxTleDSPU_HOLD.Size = new System.Drawing.Size(175, 24);
			this.ComboBoxTleDSPU_HOLD.TabIndex = 62;
			this.ComboBoxTleDSPU_HOLD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleDSPU_HOLD_SelectionChangeCommitted);
			// 
			// ComboBoxTleIIF_MOD
			// 
			this.ComboBoxTleIIF_MOD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleIIF_MOD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleIIF_MOD.FormattingEnabled = true;
			this.ComboBoxTleIIF_MOD.Items.AddRange(new object[] {
            "IIF disabled",
            "A/B operation with index on IFC pin",
            "Step/Dir. operation with index on IFC pin"});
			this.ComboBoxTleIIF_MOD.Location = new System.Drawing.Point(88, 13);
			this.ComboBoxTleIIF_MOD.Name = "ComboBoxTleIIF_MOD";
			this.ComboBoxTleIIF_MOD.Size = new System.Drawing.Size(175, 24);
			this.ComboBoxTleIIF_MOD.TabIndex = 61;
			this.ComboBoxTleIIF_MOD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleIIF_MOD_SelectionChangeCommitted);
			// 
			// ButtonTleGetDSPU_HOLD
			// 
			this.ButtonTleGetDSPU_HOLD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetDSPU_HOLD.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetDSPU_HOLD.Name = "ButtonTleGetDSPU_HOLD";
			this.ButtonTleGetDSPU_HOLD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetDSPU_HOLD.TabIndex = 44;
			this.ButtonTleGetDSPU_HOLD.Text = "DSPU_HOLD";
			this.ButtonTleGetDSPU_HOLD.Click += new System.EventHandler(this.ButtonTleGetDSPU_HOLD_Click);
			// 
			// ButtonTleGetIIF_MOD
			// 
			this.ButtonTleGetIIF_MOD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetIIF_MOD.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetIIF_MOD.Name = "ButtonTleGetIIF_MOD";
			this.ButtonTleGetIIF_MOD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetIIF_MOD.TabIndex = 7;
			this.ButtonTleGetIIF_MOD.Text = "IIF_MOD";
			this.ButtonTleGetIIF_MOD.Click += new System.EventHandler(this.ButtonTleGetIIF_MOD_Click);
			// 
			// GroupBoxTleFSYNC
			// 
			this.GroupBoxTleFSYNC.Controls.Add(this.TextBoxTleFSYNC);
			this.GroupBoxTleFSYNC.Controls.Add(this.ButtonTleGetFSYNC);
			this.GroupBoxTleFSYNC.Controls.Add(this.TextBoxTleTEMPER);
			this.GroupBoxTleFSYNC.Controls.Add(this.ButtonTleGetTEMPER);
			this.GroupBoxTleFSYNC.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleFSYNC.Location = new System.Drawing.Point(4, 233);
			this.GroupBoxTleFSYNC.Name = "GroupBoxTleFSYNC";
			this.GroupBoxTleFSYNC.Size = new System.Drawing.Size(270, 41);
			this.GroupBoxTleFSYNC.TabIndex = 51;
			this.GroupBoxTleFSYNC.TabStop = false;
			this.GroupBoxTleFSYNC.Text = "FSYNC - Frame Synchronization Register";
			// 
			// TextBoxTleFSYNC
			// 
			this.TextBoxTleFSYNC.Location = new System.Drawing.Point(220, 16);
			this.TextBoxTleFSYNC.MaxLength = 10;
			this.TextBoxTleFSYNC.Name = "TextBoxTleFSYNC";
			this.TextBoxTleFSYNC.ReadOnly = true;
			this.TextBoxTleFSYNC.Size = new System.Drawing.Size(43, 21);
			this.TextBoxTleFSYNC.TabIndex = 44;
			this.TextBoxTleFSYNC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetFSYNC
			// 
			this.ButtonTleGetFSYNC.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFSYNC.Location = new System.Drawing.Point(140, 15);
			this.ButtonTleGetFSYNC.Name = "ButtonTleGetFSYNC";
			this.ButtonTleGetFSYNC.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetFSYNC.TabIndex = 44;
			this.ButtonTleGetFSYNC.Text = "FSYNC";
			this.ButtonTleGetFSYNC.Click += new System.EventHandler(this.ButtonTleGetFSYNC_Click);
			// 
			// TextBoxTleTEMPER
			// 
			this.TextBoxTleTEMPER.Location = new System.Drawing.Point(86, 16);
			this.TextBoxTleTEMPER.MaxLength = 10;
			this.TextBoxTleTEMPER.Name = "TextBoxTleTEMPER";
			this.TextBoxTleTEMPER.ReadOnly = true;
			this.TextBoxTleTEMPER.Size = new System.Drawing.Size(43, 21);
			this.TextBoxTleTEMPER.TabIndex = 8;
			this.TextBoxTleTEMPER.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetTEMPER
			// 
			this.ButtonTleGetTEMPER.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetTEMPER.Location = new System.Drawing.Point(6, 15);
			this.ButtonTleGetTEMPER.Name = "ButtonTleGetTEMPER";
			this.ButtonTleGetTEMPER.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetTEMPER.TabIndex = 7;
			this.ButtonTleGetTEMPER.Text = "TEMPER";
			this.ButtonTleGetTEMPER.Click += new System.EventHandler(this.ButtonTleGetTEMPER_Click);
			// 
			// GroupBoxTleAREV
			// 
			this.GroupBoxTleAREV.Controls.Add(this.TextBoxTleRD_REV);
			this.GroupBoxTleAREV.Controls.Add(this.ButtonTleGetRD_REV);
			this.GroupBoxTleAREV.Controls.Add(this.TextBoxTleFCNT);
			this.GroupBoxTleAREV.Controls.Add(this.ButtonTleGetFCNT);
			this.GroupBoxTleAREV.Controls.Add(this.TextBoxTleREVOL);
			this.GroupBoxTleAREV.Controls.Add(this.ButtonTleGetRevol);
			this.GroupBoxTleAREV.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleAREV.Location = new System.Drawing.Point(139, 68);
			this.GroupBoxTleAREV.Name = "GroupBoxTleAREV";
			this.GroupBoxTleAREV.Size = new System.Drawing.Size(271, 41);
			this.GroupBoxTleAREV.TabIndex = 52;
			this.GroupBoxTleAREV.TabStop = false;
			this.GroupBoxTleAREV.Text = "AREV - Angle Revolution Register";
			// 
			// TextBoxTleRD_REV
			// 
			this.TextBoxTleRD_REV.Location = new System.Drawing.Point(230, 16);
			this.TextBoxTleRD_REV.MaxLength = 10;
			this.TextBoxTleRD_REV.Name = "TextBoxTleRD_REV";
			this.TextBoxTleRD_REV.ReadOnly = true;
			this.TextBoxTleRD_REV.Size = new System.Drawing.Size(34, 21);
			this.TextBoxTleRD_REV.TabIndex = 45;
			this.TextBoxTleRD_REV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetRD_REV
			// 
			this.ButtonTleGetRD_REV.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetRD_REV.Location = new System.Drawing.Point(178, 15);
			this.ButtonTleGetRD_REV.Name = "ButtonTleGetRD_REV";
			this.ButtonTleGetRD_REV.Size = new System.Drawing.Size(52, 22);
			this.ButtonTleGetRD_REV.TabIndex = 46;
			this.ButtonTleGetRD_REV.Text = "RD_REV";
			this.ButtonTleGetRD_REV.Click += new System.EventHandler(this.ButtonTleGetRD_REV_Click);
			// 
			// TextBoxTleFCNT
			// 
			this.TextBoxTleFCNT.Location = new System.Drawing.Point(144, 16);
			this.TextBoxTleFCNT.MaxLength = 10;
			this.TextBoxTleFCNT.Name = "TextBoxTleFCNT";
			this.TextBoxTleFCNT.ReadOnly = true;
			this.TextBoxTleFCNT.Size = new System.Drawing.Size(34, 21);
			this.TextBoxTleFCNT.TabIndex = 44;
			this.TextBoxTleFCNT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetFCNT
			// 
			this.ButtonTleGetFCNT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetFCNT.Location = new System.Drawing.Point(92, 15);
			this.ButtonTleGetFCNT.Name = "ButtonTleGetFCNT";
			this.ButtonTleGetFCNT.Size = new System.Drawing.Size(52, 22);
			this.ButtonTleGetFCNT.TabIndex = 44;
			this.ButtonTleGetFCNT.Text = "FCNT";
			this.ButtonTleGetFCNT.Click += new System.EventHandler(this.ButtonTleGetFCNT_Click);
			// 
			// TextBoxTleREVOL
			// 
			this.TextBoxTleREVOL.Location = new System.Drawing.Point(58, 16);
			this.TextBoxTleREVOL.MaxLength = 10;
			this.TextBoxTleREVOL.Name = "TextBoxTleREVOL";
			this.TextBoxTleREVOL.ReadOnly = true;
			this.TextBoxTleREVOL.Size = new System.Drawing.Size(34, 21);
			this.TextBoxTleREVOL.TabIndex = 8;
			this.TextBoxTleREVOL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetRevol
			// 
			this.ButtonTleGetRevol.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetRevol.Location = new System.Drawing.Point(6, 15);
			this.ButtonTleGetRevol.Name = "ButtonTleGetRevol";
			this.ButtonTleGetRevol.Size = new System.Drawing.Size(52, 22);
			this.ButtonTleGetRevol.TabIndex = 7;
			this.ButtonTleGetRevol.Text = "REVOL";
			this.ButtonTleGetRevol.Click += new System.EventHandler(this.ButtonTleGetRevol_Click);
			// 
			// GroupBoxTleASPD
			// 
			this.GroupBoxTleASPD.Controls.Add(this.TextBoxTleRD_AS);
			this.GroupBoxTleASPD.Controls.Add(this.ButtonTleGetRD_AS);
			this.GroupBoxTleASPD.Controls.Add(this.TextBoxTleANG_SPD);
			this.GroupBoxTleASPD.Controls.Add(this.ButtonTleGetANG_SPD);
			this.GroupBoxTleASPD.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleASPD.Location = new System.Drawing.Point(275, 3);
			this.GroupBoxTleASPD.Name = "GroupBoxTleASPD";
			this.GroupBoxTleASPD.Size = new System.Drawing.Size(135, 61);
			this.GroupBoxTleASPD.TabIndex = 51;
			this.GroupBoxTleASPD.TabStop = false;
			this.GroupBoxTleASPD.Text = "ASPD - Angle Speed Register";
			// 
			// TextBoxTleRD_AS
			// 
			this.TextBoxTleRD_AS.Location = new System.Drawing.Point(86, 37);
			this.TextBoxTleRD_AS.MaxLength = 10;
			this.TextBoxTleRD_AS.Name = "TextBoxTleRD_AS";
			this.TextBoxTleRD_AS.ReadOnly = true;
			this.TextBoxTleRD_AS.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleRD_AS.TabIndex = 44;
			this.TextBoxTleRD_AS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetRD_AS
			// 
			this.ButtonTleGetRD_AS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetRD_AS.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetRD_AS.Name = "ButtonTleGetRD_AS";
			this.ButtonTleGetRD_AS.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetRD_AS.TabIndex = 44;
			this.ButtonTleGetRD_AS.Text = "RD_AS";
			this.ButtonTleGetRD_AS.Click += new System.EventHandler(this.ButtonTleGetRD_AS_Click);
			// 
			// TextBoxTleANG_SPD
			// 
			this.TextBoxTleANG_SPD.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleANG_SPD.MaxLength = 10;
			this.TextBoxTleANG_SPD.Name = "TextBoxTleANG_SPD";
			this.TextBoxTleANG_SPD.ReadOnly = true;
			this.TextBoxTleANG_SPD.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleANG_SPD.TabIndex = 8;
			this.TextBoxTleANG_SPD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetANG_SPD
			// 
			this.ButtonTleGetANG_SPD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetANG_SPD.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetANG_SPD.Name = "ButtonTleGetANG_SPD";
			this.ButtonTleGetANG_SPD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetANG_SPD.TabIndex = 7;
			this.ButtonTleGetANG_SPD.Text = "ANG_SPD";
			this.ButtonTleGetANG_SPD.Click += new System.EventHandler(this.ButtonTleGetANG_SPD_Click);
			// 
			// GroupBoxTleAVAL
			// 
			this.GroupBoxTleAVAL.Controls.Add(this.TextBoxTleRD_AV);
			this.GroupBoxTleAVAL.Controls.Add(this.ButtonTleGetRD_AV);
			this.GroupBoxTleAVAL.Controls.Add(this.TextBoxTleANG_VAL);
			this.GroupBoxTleAVAL.Controls.Add(this.ButtonTleGetANG_VAL);
			this.GroupBoxTleAVAL.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleAVAL.Location = new System.Drawing.Point(139, 3);
			this.GroupBoxTleAVAL.Name = "GroupBoxTleAVAL";
			this.GroupBoxTleAVAL.Size = new System.Drawing.Size(135, 61);
			this.GroupBoxTleAVAL.TabIndex = 50;
			this.GroupBoxTleAVAL.TabStop = false;
			this.GroupBoxTleAVAL.Text = "AVAL - Angle Value Register";
			// 
			// TextBoxTleRD_AV
			// 
			this.TextBoxTleRD_AV.Location = new System.Drawing.Point(86, 37);
			this.TextBoxTleRD_AV.MaxLength = 10;
			this.TextBoxTleRD_AV.Name = "TextBoxTleRD_AV";
			this.TextBoxTleRD_AV.ReadOnly = true;
			this.TextBoxTleRD_AV.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleRD_AV.TabIndex = 44;
			this.TextBoxTleRD_AV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetRD_AV
			// 
			this.ButtonTleGetRD_AV.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetRD_AV.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetRD_AV.Name = "ButtonTleGetRD_AV";
			this.ButtonTleGetRD_AV.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetRD_AV.TabIndex = 44;
			this.ButtonTleGetRD_AV.Text = "RD_AV";
			this.ButtonTleGetRD_AV.Click += new System.EventHandler(this.ButtonTleGetRD_AV_Click);
			// 
			// TextBoxTleANG_VAL
			// 
			this.TextBoxTleANG_VAL.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleANG_VAL.MaxLength = 10;
			this.TextBoxTleANG_VAL.Name = "TextBoxTleANG_VAL";
			this.TextBoxTleANG_VAL.ReadOnly = true;
			this.TextBoxTleANG_VAL.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleANG_VAL.TabIndex = 8;
			this.TextBoxTleANG_VAL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetANG_VAL
			// 
			this.ButtonTleGetANG_VAL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetANG_VAL.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetANG_VAL.Name = "ButtonTleGetANG_VAL";
			this.ButtonTleGetANG_VAL.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetANG_VAL.TabIndex = 7;
			this.ButtonTleGetANG_VAL.Text = "ANG_VAL";
			this.ButtonTleGetANG_VAL.Click += new System.EventHandler(this.ButtonTleGetANG_VAL_Click);
			// 
			// GroupBoxTleACSTAT
			// 
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_FRST);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_FRST);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_ADCT);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_ADCT);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_VEC_MAG);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_VEC_XY);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_OV);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_VEC_MAG);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_VEC_XY);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_OV);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_DSPU);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_DSPU);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_FUSE);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAcstatAS_FUSE);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_VR);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_WD);
			this.GroupBoxTleACSTAT.Controls.Add(this.ComboBoxTleAS_RST);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_VR);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_WD);
			this.GroupBoxTleACSTAT.Controls.Add(this.ButtonTleGetAS_RST);
			this.GroupBoxTleACSTAT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxTleACSTAT.Location = new System.Drawing.Point(4, 108);
			this.GroupBoxTleACSTAT.Name = "GroupBoxTleACSTAT";
			this.GroupBoxTleACSTAT.Size = new System.Drawing.Size(270, 126);
			this.GroupBoxTleACSTAT.TabIndex = 50;
			this.GroupBoxTleACSTAT.TabStop = false;
			this.GroupBoxTleACSTAT.Text = "ACSTAT - Activation Status Register";
			// 
			// ComboBoxTleAS_FRST
			// 
			this.ComboBoxTleAS_FRST.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_FRST.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_FRST.FormattingEnabled = true;
			this.ComboBoxTleAS_FRST.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_FRST.Location = new System.Drawing.Point(222, 100);
			this.ComboBoxTleAS_FRST.Name = "ComboBoxTleAS_FRST";
			this.ComboBoxTleAS_FRST.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_FRST.TabIndex = 65;
			this.ComboBoxTleAS_FRST.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_FRST_SelectionChangeCommitted);
			// 
			// ButtonTleGetAS_FRST
			// 
			this.ButtonTleGetAS_FRST.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_FRST.Location = new System.Drawing.Point(140, 101);
			this.ButtonTleGetAS_FRST.Name = "ButtonTleGetAS_FRST";
			this.ButtonTleGetAS_FRST.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_FRST.TabIndex = 64;
			this.ButtonTleGetAS_FRST.Text = "AS_FRST";
			this.ButtonTleGetAS_FRST.Click += new System.EventHandler(this.ButtonTleGetAS_FRST_Click);
			// 
			// ComboBoxTleAS_ADCT
			// 
			this.ComboBoxTleAS_ADCT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_ADCT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_ADCT.FormattingEnabled = true;
			this.ComboBoxTleAS_ADCT.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_ADCT.Location = new System.Drawing.Point(222, 77);
			this.ComboBoxTleAS_ADCT.Name = "ComboBoxTleAS_ADCT";
			this.ComboBoxTleAS_ADCT.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_ADCT.TabIndex = 63;
			this.ComboBoxTleAS_ADCT.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_ADCT_SelectionChangeCommitted);
			// 
			// ButtonTleGetAS_ADCT
			// 
			this.ButtonTleGetAS_ADCT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_ADCT.Location = new System.Drawing.Point(140, 80);
			this.ButtonTleGetAS_ADCT.Name = "ButtonTleGetAS_ADCT";
			this.ButtonTleGetAS_ADCT.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_ADCT.TabIndex = 62;
			this.ButtonTleGetAS_ADCT.Text = "AS_ADCT";
			this.ButtonTleGetAS_ADCT.Click += new System.EventHandler(this.ButtonTleGetAS_ADCT_Click);
			// 
			// ComboBoxTleAS_VEC_MAG
			// 
			this.ComboBoxTleAS_VEC_MAG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_VEC_MAG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_VEC_MAG.FormattingEnabled = true;
			this.ComboBoxTleAS_VEC_MAG.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_VEC_MAG.Location = new System.Drawing.Point(222, 58);
			this.ComboBoxTleAS_VEC_MAG.Name = "ComboBoxTleAS_VEC_MAG";
			this.ComboBoxTleAS_VEC_MAG.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_VEC_MAG.TabIndex = 60;
			this.ComboBoxTleAS_VEC_MAG.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_VEC_MAG_SelectionChangeCommitted);
			// 
			// ComboBoxTleAS_VEC_XY
			// 
			this.ComboBoxTleAS_VEC_XY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_VEC_XY.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_VEC_XY.FormattingEnabled = true;
			this.ComboBoxTleAS_VEC_XY.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_VEC_XY.Location = new System.Drawing.Point(222, 36);
			this.ComboBoxTleAS_VEC_XY.Name = "ComboBoxTleAS_VEC_XY";
			this.ComboBoxTleAS_VEC_XY.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_VEC_XY.TabIndex = 61;
			this.ComboBoxTleAS_VEC_XY.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_VEC_XY_SelectionChangeCommitted);
			// 
			// ComboBoxTleAS_OV
			// 
			this.ComboBoxTleAS_OV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_OV.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_OV.FormattingEnabled = true;
			this.ComboBoxTleAS_OV.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_OV.Location = new System.Drawing.Point(222, 16);
			this.ComboBoxTleAS_OV.Name = "ComboBoxTleAS_OV";
			this.ComboBoxTleAS_OV.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_OV.TabIndex = 59;
			this.ComboBoxTleAS_OV.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_OV_SelectionChangeCommitted);
			// 
			// ButtonTleGetAS_VEC_MAG
			// 
			this.ButtonTleGetAS_VEC_MAG.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_VEC_MAG.Location = new System.Drawing.Point(140, 58);
			this.ButtonTleGetAS_VEC_MAG.Name = "ButtonTleGetAS_VEC_MAG";
			this.ButtonTleGetAS_VEC_MAG.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_VEC_MAG.TabIndex = 58;
			this.ButtonTleGetAS_VEC_MAG.Text = "AS_VEC_MAG";
			this.ButtonTleGetAS_VEC_MAG.Click += new System.EventHandler(this.ButtonTleGetAS_VEC_MAG_Click);
			// 
			// ButtonTleGetAS_VEC_XY
			// 
			this.ButtonTleGetAS_VEC_XY.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_VEC_XY.Location = new System.Drawing.Point(140, 38);
			this.ButtonTleGetAS_VEC_XY.Name = "ButtonTleGetAS_VEC_XY";
			this.ButtonTleGetAS_VEC_XY.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_VEC_XY.TabIndex = 57;
			this.ButtonTleGetAS_VEC_XY.Text = "AS_VEC_XY";
			this.ButtonTleGetAS_VEC_XY.Click += new System.EventHandler(this.ButtonTleGetAS_VEC_XY_Click);
			// 
			// ButtonTleGetAS_OV
			// 
			this.ButtonTleGetAS_OV.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_OV.Location = new System.Drawing.Point(140, 16);
			this.ButtonTleGetAS_OV.Name = "ButtonTleGetAS_OV";
			this.ButtonTleGetAS_OV.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_OV.TabIndex = 56;
			this.ButtonTleGetAS_OV.Text = "AS_OV";
			this.ButtonTleGetAS_OV.Click += new System.EventHandler(this.ButtonTleGetAS_OV_Click);
			// 
			// ComboBoxTleAS_DSPU
			// 
			this.ComboBoxTleAS_DSPU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_DSPU.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_DSPU.FormattingEnabled = true;
			this.ComboBoxTleAS_DSPU.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_DSPU.Location = new System.Drawing.Point(86, 100);
			this.ComboBoxTleAS_DSPU.Name = "ComboBoxTleAS_DSPU";
			this.ComboBoxTleAS_DSPU.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_DSPU.TabIndex = 55;
			this.ComboBoxTleAS_DSPU.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_DSPU_SelectionChangeCommitted);
			// 
			// ButtonTleGetAS_DSPU
			// 
			this.ButtonTleGetAS_DSPU.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_DSPU.Location = new System.Drawing.Point(6, 100);
			this.ButtonTleGetAS_DSPU.Name = "ButtonTleGetAS_DSPU";
			this.ButtonTleGetAS_DSPU.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_DSPU.TabIndex = 54;
			this.ButtonTleGetAS_DSPU.Text = "AS_DSPU";
			this.ButtonTleGetAS_DSPU.Click += new System.EventHandler(this.ButtonTleGetAS_DSPU_Click);
			// 
			// ComboBoxTleAS_FUSE
			// 
			this.ComboBoxTleAS_FUSE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_FUSE.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_FUSE.FormattingEnabled = true;
			this.ComboBoxTleAS_FUSE.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_FUSE.Location = new System.Drawing.Point(86, 79);
			this.ComboBoxTleAS_FUSE.Name = "ComboBoxTleAS_FUSE";
			this.ComboBoxTleAS_FUSE.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_FUSE.TabIndex = 53;
			this.ComboBoxTleAS_FUSE.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_FUSE_SelectionChangeCommitted);
			// 
			// ButtonTleGetAcstatAS_FUSE
			// 
			this.ButtonTleGetAcstatAS_FUSE.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAcstatAS_FUSE.Location = new System.Drawing.Point(6, 79);
			this.ButtonTleGetAcstatAS_FUSE.Name = "ButtonTleGetAcstatAS_FUSE";
			this.ButtonTleGetAcstatAS_FUSE.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAcstatAS_FUSE.TabIndex = 52;
			this.ButtonTleGetAcstatAS_FUSE.Text = "AS_FUSE";
			this.ButtonTleGetAcstatAS_FUSE.Click += new System.EventHandler(this.ButtonTleGetAcstatAS_FUSE_Click);
			// 
			// ComboBoxTleAS_VR
			// 
			this.ComboBoxTleAS_VR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_VR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_VR.FormattingEnabled = true;
			this.ComboBoxTleAS_VR.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_VR.Location = new System.Drawing.Point(86, 58);
			this.ComboBoxTleAS_VR.Name = "ComboBoxTleAS_VR";
			this.ComboBoxTleAS_VR.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_VR.TabIndex = 51;
			this.ComboBoxTleAS_VR.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_VR_SelectionChangeCommitted);
			// 
			// ComboBoxTleAS_WD
			// 
			this.ComboBoxTleAS_WD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_WD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_WD.FormattingEnabled = true;
			this.ComboBoxTleAS_WD.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_WD.Location = new System.Drawing.Point(86, 35);
			this.ComboBoxTleAS_WD.Name = "ComboBoxTleAS_WD";
			this.ComboBoxTleAS_WD.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_WD.TabIndex = 51;
			this.ComboBoxTleAS_WD.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_WD_SelectionChangeCommitted);
			// 
			// ComboBoxTleAS_RST
			// 
			this.ComboBoxTleAS_RST.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleAS_RST.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleAS_RST.FormattingEnabled = true;
			this.ComboBoxTleAS_RST.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBoxTleAS_RST.Location = new System.Drawing.Point(86, 14);
			this.ComboBoxTleAS_RST.Name = "ComboBoxTleAS_RST";
			this.ComboBoxTleAS_RST.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleAS_RST.TabIndex = 50;
			this.ComboBoxTleAS_RST.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleAS_RST_SelectionChangeCommitted);
			// 
			// ButtonTleGetAS_VR
			// 
			this.ButtonTleGetAS_VR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_VR.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetAS_VR.Name = "ButtonTleGetAS_VR";
			this.ButtonTleGetAS_VR.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_VR.TabIndex = 46;
			this.ButtonTleGetAS_VR.Text = "AS_VR";
			this.ButtonTleGetAS_VR.Click += new System.EventHandler(this.ButtonTleGetAS_VR_Click);
			// 
			// ButtonTleGetAS_WD
			// 
			this.ButtonTleGetAS_WD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_WD.Location = new System.Drawing.Point(6, 37);
			this.ButtonTleGetAS_WD.Name = "ButtonTleGetAS_WD";
			this.ButtonTleGetAS_WD.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_WD.TabIndex = 44;
			this.ButtonTleGetAS_WD.Text = "AS_WD";
			this.ButtonTleGetAS_WD.Click += new System.EventHandler(this.ButtonTleGetAS_WD_Click);
			// 
			// ButtonTleGetAS_RST
			// 
			this.ButtonTleGetAS_RST.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetAS_RST.Location = new System.Drawing.Point(6, 16);
			this.ButtonTleGetAS_RST.Name = "ButtonTleGetAS_RST";
			this.ButtonTleGetAS_RST.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetAS_RST.TabIndex = 7;
			this.ButtonTleGetAS_RST.Text = "AS_RST";
			this.ButtonTleGetAS_RST.Click += new System.EventHandler(this.ButtonTleGetAS_RST_Click);
			// 
			// groupTleSTAT
			// 
			this.groupTleSTAT.Controls.Add(this.ComboBoxTleStatS_NR);
			this.groupTleSTAT.Controls.Add(this.ButtonTleGetSlave);
			this.groupTleSTAT.Controls.Add(this.ButtonTleGetRD_ST);
			this.groupTleSTAT.Controls.Add(this.TextBoxTleStatError);
			this.groupTleSTAT.Controls.Add(this.TextBoxTleStatRD_ST);
			this.groupTleSTAT.Controls.Add(this.ButtonTleGetError);
			this.groupTleSTAT.Controls.Add(this.TextBoxTleStatS_RST);
			this.groupTleSTAT.Controls.Add(this.ButtonTleGetS_RST);
			this.groupTleSTAT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupTleSTAT.Location = new System.Drawing.Point(4, 3);
			this.groupTleSTAT.Name = "groupTleSTAT";
			this.groupTleSTAT.Size = new System.Drawing.Size(134, 106);
			this.groupTleSTAT.TabIndex = 30;
			this.groupTleSTAT.TabStop = false;
			this.groupTleSTAT.Text = "STAT - Status Register";
			// 
			// ComboBoxTleStatS_NR
			// 
			this.ComboBoxTleStatS_NR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxTleStatS_NR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxTleStatS_NR.FormattingEnabled = true;
			this.ComboBoxTleStatS_NR.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
			this.ComboBoxTleStatS_NR.Location = new System.Drawing.Point(86, 80);
			this.ComboBoxTleStatS_NR.Name = "ComboBoxTleStatS_NR";
			this.ComboBoxTleStatS_NR.Size = new System.Drawing.Size(42, 24);
			this.ComboBoxTleStatS_NR.TabIndex = 49;
			this.ComboBoxTleStatS_NR.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxTleStatS_NR_SelectionChangeCommitted);
			// 
			// ButtonTleGetSlave
			// 
			this.ButtonTleGetSlave.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetSlave.Location = new System.Drawing.Point(6, 80);
			this.ButtonTleGetSlave.Name = "ButtonTleGetSlave";
			this.ButtonTleGetSlave.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetSlave.TabIndex = 47;
			this.ButtonTleGetSlave.Text = "S_NR";
			this.ButtonTleGetSlave.Click += new System.EventHandler(this.ButtonTleGetSlave_Click);
			// 
			// ButtonTleGetRD_ST
			// 
			this.ButtonTleGetRD_ST.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetRD_ST.Location = new System.Drawing.Point(6, 58);
			this.ButtonTleGetRD_ST.Name = "ButtonTleGetRD_ST";
			this.ButtonTleGetRD_ST.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetRD_ST.TabIndex = 46;
			this.ButtonTleGetRD_ST.Text = "RD_ST";
			this.ButtonTleGetRD_ST.Click += new System.EventHandler(this.ButtonTleGetRD_ST_Click);
			// 
			// TextBoxTleStatError
			// 
			this.TextBoxTleStatError.Location = new System.Drawing.Point(86, 37);
			this.TextBoxTleStatError.MaxLength = 10;
			this.TextBoxTleStatError.Name = "TextBoxTleStatError";
			this.TextBoxTleStatError.ReadOnly = true;
			this.TextBoxTleStatError.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleStatError.TabIndex = 44;
			this.TextBoxTleStatError.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxTleStatRD_ST
			// 
			this.TextBoxTleStatRD_ST.Location = new System.Drawing.Point(86, 59);
			this.TextBoxTleStatRD_ST.MaxLength = 10;
			this.TextBoxTleStatRD_ST.Name = "TextBoxTleStatRD_ST";
			this.TextBoxTleStatRD_ST.ReadOnly = true;
			this.TextBoxTleStatRD_ST.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleStatRD_ST.TabIndex = 45;
			this.TextBoxTleStatRD_ST.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetError
			// 
			this.ButtonTleGetError.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetError.Location = new System.Drawing.Point(6, 36);
			this.ButtonTleGetError.Name = "ButtonTleGetError";
			this.ButtonTleGetError.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetError.TabIndex = 44;
			this.ButtonTleGetError.Text = "Error";
			this.ButtonTleGetError.Click += new System.EventHandler(this.ButtonTleGetError_Click);
			// 
			// TextBoxTleStatS_RST
			// 
			this.TextBoxTleStatS_RST.Location = new System.Drawing.Point(86, 15);
			this.TextBoxTleStatS_RST.MaxLength = 10;
			this.TextBoxTleStatS_RST.Name = "TextBoxTleStatS_RST";
			this.TextBoxTleStatS_RST.ReadOnly = true;
			this.TextBoxTleStatS_RST.Size = new System.Drawing.Size(42, 21);
			this.TextBoxTleStatS_RST.TabIndex = 8;
			this.TextBoxTleStatS_RST.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonTleGetS_RST
			// 
			this.ButtonTleGetS_RST.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonTleGetS_RST.Location = new System.Drawing.Point(6, 14);
			this.ButtonTleGetS_RST.Name = "ButtonTleGetS_RST";
			this.ButtonTleGetS_RST.Size = new System.Drawing.Size(80, 22);
			this.ButtonTleGetS_RST.TabIndex = 7;
			this.ButtonTleGetS_RST.Text = "S_RST";
			this.ButtonTleGetS_RST.Click += new System.EventHandler(this.ButtonTleGetS_RST_Click);
			// 
			// tabLightSensor
			// 
			this.tabLightSensor.Controls.Add(this.groupBoxLSID);
			this.tabLightSensor.Controls.Add(this.groupBoxLSLimit);
			this.tabLightSensor.Controls.Add(this.groupBoxLSConfiguration);
			this.tabLightSensor.Controls.Add(this.groupBoxLSResult);
			this.tabLightSensor.Location = new System.Drawing.Point(4, 22);
			this.tabLightSensor.Name = "tabLightSensor";
			this.tabLightSensor.Padding = new System.Windows.Forms.Padding(3);
			this.tabLightSensor.Size = new System.Drawing.Size(1025, 382);
			this.tabLightSensor.TabIndex = 5;
			this.tabLightSensor.Text = "LightSensor";
			this.tabLightSensor.UseVisualStyleBackColor = true;
			// 
			// groupBoxLSID
			// 
			this.groupBoxLSID.Controls.Add(this.ButtonLSDID);
			this.groupBoxLSID.Controls.Add(this.TextBoxLSDID);
			this.groupBoxLSID.Controls.Add(this.TextBoxLSID);
			this.groupBoxLSID.Controls.Add(this.ButtonGetLSID);
			this.groupBoxLSID.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxLSID.Location = new System.Drawing.Point(6, 231);
			this.groupBoxLSID.Name = "groupBoxLSID";
			this.groupBoxLSID.Size = new System.Drawing.Size(386, 43);
			this.groupBoxLSID.TabIndex = 44;
			this.groupBoxLSID.TabStop = false;
			this.groupBoxLSID.Text = "Manufacturer / Device ID";
			// 
			// ButtonLSDID
			// 
			this.ButtonLSDID.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonLSDID.Location = new System.Drawing.Point(196, 15);
			this.ButtonLSDID.Name = "ButtonLSDID";
			this.ButtonLSDID.Size = new System.Drawing.Size(93, 22);
			this.ButtonLSDID.TabIndex = 15;
			this.ButtonLSDID.Text = "Device ID";
			this.ButtonLSDID.Click += new System.EventHandler(this.ButtonLSDID_Click);
			// 
			// TextBoxLSDID
			// 
			this.TextBoxLSDID.Location = new System.Drawing.Point(293, 15);
			this.TextBoxLSDID.MaxLength = 10;
			this.TextBoxLSDID.Name = "TextBoxLSDID";
			this.TextBoxLSDID.ReadOnly = true;
			this.TextBoxLSDID.Size = new System.Drawing.Size(80, 21);
			this.TextBoxLSDID.TabIndex = 14;
			this.TextBoxLSDID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxLSID
			// 
			this.TextBoxLSID.Location = new System.Drawing.Point(103, 15);
			this.TextBoxLSID.MaxLength = 10;
			this.TextBoxLSID.Name = "TextBoxLSID";
			this.TextBoxLSID.ReadOnly = true;
			this.TextBoxLSID.Size = new System.Drawing.Size(80, 21);
			this.TextBoxLSID.TabIndex = 8;
			this.TextBoxLSID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonGetLSID
			// 
			this.ButtonGetLSID.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetLSID.Location = new System.Drawing.Point(6, 14);
			this.ButtonGetLSID.Name = "ButtonGetLSID";
			this.ButtonGetLSID.Size = new System.Drawing.Size(93, 22);
			this.ButtonGetLSID.TabIndex = 7;
			this.ButtonGetLSID.Text = "Manufacturer ID";
			this.ButtonGetLSID.Click += new System.EventHandler(this.ButtonGetLSID_Click);
			// 
			// groupBoxLSLimit
			// 
			this.groupBoxLSLimit.Controls.Add(this.LabelLSLuxLimit);
			this.groupBoxLSLimit.Controls.Add(this.CheckBoxLSHighLimit);
			this.groupBoxLSLimit.Controls.Add(this.LabelLSRLimit);
			this.groupBoxLSLimit.Controls.Add(this.CheckBoxLSLowLimit);
			this.groupBoxLSLimit.Controls.Add(this.TextBoxLSRLimit);
			this.groupBoxLSLimit.Controls.Add(this.ButtonSetLSLimit);
			this.groupBoxLSLimit.Controls.Add(this.LabelLSELimit);
			this.groupBoxLSLimit.Controls.Add(this.ButtonGetLSLimit);
			this.groupBoxLSLimit.Controls.Add(this.TextBoxLSELimit);
			this.groupBoxLSLimit.Controls.Add(this.TextBoxLSLuxLimit);
			this.groupBoxLSLimit.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxLSLimit.Location = new System.Drawing.Point(6, 165);
			this.groupBoxLSLimit.Name = "groupBoxLSLimit";
			this.groupBoxLSLimit.Size = new System.Drawing.Size(386, 67);
			this.groupBoxLSLimit.TabIndex = 59;
			this.groupBoxLSLimit.TabStop = false;
			this.groupBoxLSLimit.Text = "High / Low Limit";
			// 
			// LabelLSLuxLimit
			// 
			this.LabelLSLuxLimit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSLuxLimit.Location = new System.Drawing.Point(100, 39);
			this.LabelLSLuxLimit.Name = "LabelLSLuxLimit";
			this.LabelLSLuxLimit.Size = new System.Drawing.Size(34, 25);
			this.LabelLSLuxLimit.TabIndex = 49;
			this.LabelLSLuxLimit.Text = "Lux";
			this.LabelLSLuxLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CheckBoxLSHighLimit
			// 
			this.CheckBoxLSHighLimit.AutoSize = true;
			this.CheckBoxLSHighLimit.Location = new System.Drawing.Point(184, 16);
			this.CheckBoxLSHighLimit.Name = "CheckBoxLSHighLimit";
			this.CheckBoxLSHighLimit.Size = new System.Drawing.Size(69, 20);
			this.CheckBoxLSHighLimit.TabIndex = 48;
			this.CheckBoxLSHighLimit.Text = "High Limit";
			this.CheckBoxLSHighLimit.UseVisualStyleBackColor = true;
			this.CheckBoxLSHighLimit.CheckedChanged += new System.EventHandler(this.CheckBoxLSHighLimit_CheckedChanged);
			// 
			// LabelLSRLimit
			// 
			this.LabelLSRLimit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSRLimit.Location = new System.Drawing.Point(282, 39);
			this.LabelLSRLimit.Name = "LabelLSRLimit";
			this.LabelLSRLimit.Size = new System.Drawing.Size(34, 25);
			this.LabelLSRLimit.TabIndex = 48;
			this.LabelLSRLimit.Text = "R";
			this.LabelLSRLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CheckBoxLSLowLimit
			// 
			this.CheckBoxLSLowLimit.AutoSize = true;
			this.CheckBoxLSLowLimit.Checked = true;
			this.CheckBoxLSLowLimit.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxLSLowLimit.Location = new System.Drawing.Point(111, 15);
			this.CheckBoxLSLowLimit.Name = "CheckBoxLSLowLimit";
			this.CheckBoxLSLowLimit.Size = new System.Drawing.Size(67, 20);
			this.CheckBoxLSLowLimit.TabIndex = 47;
			this.CheckBoxLSLowLimit.Text = "Low Limit";
			this.CheckBoxLSLowLimit.UseVisualStyleBackColor = true;
			this.CheckBoxLSLowLimit.CheckedChanged += new System.EventHandler(this.CheckBoxLSLowLimit_CheckedChanged);
			// 
			// TextBoxLSRLimit
			// 
			this.TextBoxLSRLimit.Location = new System.Drawing.Point(316, 39);
			this.TextBoxLSRLimit.MaxLength = 10;
			this.TextBoxLSRLimit.Name = "TextBoxLSRLimit";
			this.TextBoxLSRLimit.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSRLimit.TabIndex = 47;
			this.TextBoxLSRLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSetLSLimit
			// 
			this.ButtonSetLSLimit.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetLSLimit.Location = new System.Drawing.Point(6, 39);
			this.ButtonSetLSLimit.Name = "ButtonSetLSLimit";
			this.ButtonSetLSLimit.Size = new System.Drawing.Size(93, 22);
			this.ButtonSetLSLimit.TabIndex = 9;
			this.ButtonSetLSLimit.Text = "Set Limit";
			this.ButtonSetLSLimit.Click += new System.EventHandler(this.ButtonSetLSLimit_Click);
			// 
			// LabelLSELimit
			// 
			this.LabelLSELimit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSELimit.Location = new System.Drawing.Point(191, 39);
			this.LabelLSELimit.Name = "LabelLSELimit";
			this.LabelLSELimit.Size = new System.Drawing.Size(34, 25);
			this.LabelLSELimit.TabIndex = 46;
			this.LabelLSELimit.Text = "E";
			this.LabelLSELimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonGetLSLimit
			// 
			this.ButtonGetLSLimit.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetLSLimit.Location = new System.Drawing.Point(6, 14);
			this.ButtonGetLSLimit.Name = "ButtonGetLSLimit";
			this.ButtonGetLSLimit.Size = new System.Drawing.Size(93, 22);
			this.ButtonGetLSLimit.TabIndex = 7;
			this.ButtonGetLSLimit.Text = "Get Limit";
			this.ButtonGetLSLimit.Click += new System.EventHandler(this.ButtonGetLSLimit_Click);
			// 
			// TextBoxLSELimit
			// 
			this.TextBoxLSELimit.Location = new System.Drawing.Point(225, 39);
			this.TextBoxLSELimit.MaxLength = 10;
			this.TextBoxLSELimit.Name = "TextBoxLSELimit";
			this.TextBoxLSELimit.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSELimit.TabIndex = 45;
			this.TextBoxLSELimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxLSLuxLimit
			// 
			this.TextBoxLSLuxLimit.Location = new System.Drawing.Point(134, 39);
			this.TextBoxLSLuxLimit.MaxLength = 10;
			this.TextBoxLSLuxLimit.Name = "TextBoxLSLuxLimit";
			this.TextBoxLSLuxLimit.ReadOnly = true;
			this.TextBoxLSLuxLimit.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSLuxLimit.TabIndex = 44;
			this.TextBoxLSLuxLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// groupBoxLSConfiguration
			// 
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSL);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSL);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSOVF);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSCRF);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSOVF);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSCRF);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSFH);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSFL);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSFH);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSRN);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSFL);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSCT);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSRN);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSCT);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSM);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSM);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSPOL);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSPOL);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSME);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSME);
			this.groupBoxLSConfiguration.Controls.Add(this.LabelLSFC);
			this.groupBoxLSConfiguration.Controls.Add(this.ButtonSetCfg);
			this.groupBoxLSConfiguration.Controls.Add(this.TextBoxLSFC);
			this.groupBoxLSConfiguration.Controls.Add(this.ButtonLSGetCfg);
			this.groupBoxLSConfiguration.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxLSConfiguration.Location = new System.Drawing.Point(6, 42);
			this.groupBoxLSConfiguration.Name = "groupBoxLSConfiguration";
			this.groupBoxLSConfiguration.Size = new System.Drawing.Size(386, 124);
			this.groupBoxLSConfiguration.TabIndex = 43;
			this.groupBoxLSConfiguration.TabStop = false;
			this.groupBoxLSConfiguration.Text = "Configuration";
			// 
			// LabelLSL
			// 
			this.LabelLSL.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSL.Location = new System.Drawing.Point(282, 41);
			this.LabelLSL.Name = "LabelLSL";
			this.LabelLSL.Size = new System.Drawing.Size(34, 25);
			this.LabelLSL.TabIndex = 58;
			this.LabelLSL.Text = "L";
			this.LabelLSL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSL
			// 
			this.TextBoxLSL.Location = new System.Drawing.Point(316, 41);
			this.TextBoxLSL.MaxLength = 10;
			this.TextBoxLSL.Name = "TextBoxLSL";
			this.TextBoxLSL.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSL.TabIndex = 57;
			this.TextBoxLSL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSOVF
			// 
			this.LabelLSOVF.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSOVF.Location = new System.Drawing.Point(282, 68);
			this.LabelLSOVF.Name = "LabelLSOVF";
			this.LabelLSOVF.Size = new System.Drawing.Size(34, 25);
			this.LabelLSOVF.TabIndex = 55;
			this.LabelLSOVF.Text = "OVF";
			this.LabelLSOVF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelLSCRF
			// 
			this.LabelLSCRF.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSCRF.Location = new System.Drawing.Point(188, 68);
			this.LabelLSCRF.Name = "LabelLSCRF";
			this.LabelLSCRF.Size = new System.Drawing.Size(34, 25);
			this.LabelLSCRF.TabIndex = 56;
			this.LabelLSCRF.Text = "CRF";
			this.LabelLSCRF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSOVF
			// 
			this.TextBoxLSOVF.Location = new System.Drawing.Point(316, 68);
			this.TextBoxLSOVF.MaxLength = 10;
			this.TextBoxLSOVF.Name = "TextBoxLSOVF";
			this.TextBoxLSOVF.ReadOnly = true;
			this.TextBoxLSOVF.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSOVF.TabIndex = 53;
			this.TextBoxLSOVF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxLSCRF
			// 
			this.TextBoxLSCRF.Location = new System.Drawing.Point(222, 68);
			this.TextBoxLSCRF.MaxLength = 10;
			this.TextBoxLSCRF.Name = "TextBoxLSCRF";
			this.TextBoxLSCRF.ReadOnly = true;
			this.TextBoxLSCRF.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSCRF.TabIndex = 54;
			this.TextBoxLSCRF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSFH
			// 
			this.LabelLSFH.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSFH.Location = new System.Drawing.Point(97, 68);
			this.LabelLSFH.Name = "LabelLSFH";
			this.LabelLSFH.Size = new System.Drawing.Size(34, 25);
			this.LabelLSFH.TabIndex = 52;
			this.LabelLSFH.Text = "FH";
			this.LabelLSFH.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelLSFL
			// 
			this.LabelLSFL.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSFL.Location = new System.Drawing.Point(6, 68);
			this.LabelLSFL.Name = "LabelLSFL";
			this.LabelLSFL.Size = new System.Drawing.Size(34, 25);
			this.LabelLSFL.TabIndex = 52;
			this.LabelLSFL.Text = "FL";
			this.LabelLSFL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSFH
			// 
			this.TextBoxLSFH.Location = new System.Drawing.Point(131, 68);
			this.TextBoxLSFH.MaxLength = 10;
			this.TextBoxLSFH.Name = "TextBoxLSFH";
			this.TextBoxLSFH.ReadOnly = true;
			this.TextBoxLSFH.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSFH.TabIndex = 51;
			this.TextBoxLSFH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSRN
			// 
			this.LabelLSRN.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSRN.Location = new System.Drawing.Point(188, 95);
			this.LabelLSRN.Name = "LabelLSRN";
			this.LabelLSRN.Size = new System.Drawing.Size(34, 25);
			this.LabelLSRN.TabIndex = 52;
			this.LabelLSRN.Text = "RN";
			this.LabelLSRN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSFL
			// 
			this.TextBoxLSFL.Location = new System.Drawing.Point(40, 68);
			this.TextBoxLSFL.MaxLength = 10;
			this.TextBoxLSFL.Name = "TextBoxLSFL";
			this.TextBoxLSFL.ReadOnly = true;
			this.TextBoxLSFL.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSFL.TabIndex = 51;
			this.TextBoxLSFL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSCT
			// 
			this.LabelLSCT.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSCT.Location = new System.Drawing.Point(97, 95);
			this.LabelLSCT.Name = "LabelLSCT";
			this.LabelLSCT.Size = new System.Drawing.Size(34, 25);
			this.LabelLSCT.TabIndex = 52;
			this.LabelLSCT.Text = "CT";
			this.LabelLSCT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSRN
			// 
			this.TextBoxLSRN.Location = new System.Drawing.Point(222, 95);
			this.TextBoxLSRN.MaxLength = 10;
			this.TextBoxLSRN.Name = "TextBoxLSRN";
			this.TextBoxLSRN.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSRN.TabIndex = 51;
			this.TextBoxLSRN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxLSCT
			// 
			this.TextBoxLSCT.Location = new System.Drawing.Point(131, 95);
			this.TextBoxLSCT.MaxLength = 10;
			this.TextBoxLSCT.Name = "TextBoxLSCT";
			this.TextBoxLSCT.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSCT.TabIndex = 51;
			this.TextBoxLSCT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSM
			// 
			this.LabelLSM.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSM.Location = new System.Drawing.Point(6, 95);
			this.LabelLSM.Name = "LabelLSM";
			this.LabelLSM.Size = new System.Drawing.Size(34, 25);
			this.LabelLSM.TabIndex = 50;
			this.LabelLSM.Text = "M";
			this.LabelLSM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSM
			// 
			this.TextBoxLSM.Location = new System.Drawing.Point(40, 95);
			this.TextBoxLSM.MaxLength = 10;
			this.TextBoxLSM.Name = "TextBoxLSM";
			this.TextBoxLSM.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSM.TabIndex = 49;
			this.TextBoxLSM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSPOL
			// 
			this.LabelLSPOL.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSPOL.Location = new System.Drawing.Point(188, 41);
			this.LabelLSPOL.Name = "LabelLSPOL";
			this.LabelLSPOL.Size = new System.Drawing.Size(34, 25);
			this.LabelLSPOL.TabIndex = 48;
			this.LabelLSPOL.Text = "POL";
			this.LabelLSPOL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSPOL
			// 
			this.TextBoxLSPOL.Location = new System.Drawing.Point(222, 41);
			this.TextBoxLSPOL.MaxLength = 10;
			this.TextBoxLSPOL.Name = "TextBoxLSPOL";
			this.TextBoxLSPOL.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSPOL.TabIndex = 47;
			this.TextBoxLSPOL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSME
			// 
			this.LabelLSME.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSME.Location = new System.Drawing.Point(97, 41);
			this.LabelLSME.Name = "LabelLSME";
			this.LabelLSME.Size = new System.Drawing.Size(34, 25);
			this.LabelLSME.TabIndex = 46;
			this.LabelLSME.Text = "ME";
			this.LabelLSME.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSME
			// 
			this.TextBoxLSME.Location = new System.Drawing.Point(131, 41);
			this.TextBoxLSME.MaxLength = 10;
			this.TextBoxLSME.Name = "TextBoxLSME";
			this.TextBoxLSME.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSME.TabIndex = 45;
			this.TextBoxLSME.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSFC
			// 
			this.LabelLSFC.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSFC.Location = new System.Drawing.Point(6, 41);
			this.LabelLSFC.Name = "LabelLSFC";
			this.LabelLSFC.Size = new System.Drawing.Size(34, 25);
			this.LabelLSFC.TabIndex = 44;
			this.LabelLSFC.Text = "FC";
			this.LabelLSFC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonSetCfg
			// 
			this.ButtonSetCfg.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetCfg.Location = new System.Drawing.Point(103, 14);
			this.ButtonSetCfg.Name = "ButtonSetCfg";
			this.ButtonSetCfg.Size = new System.Drawing.Size(93, 22);
			this.ButtonSetCfg.TabIndex = 9;
			this.ButtonSetCfg.Text = "Set Configuration";
			this.ButtonSetCfg.Click += new System.EventHandler(this.ButtonSetCfg_Click);
			// 
			// TextBoxLSFC
			// 
			this.TextBoxLSFC.Location = new System.Drawing.Point(40, 41);
			this.TextBoxLSFC.MaxLength = 10;
			this.TextBoxLSFC.Name = "TextBoxLSFC";
			this.TextBoxLSFC.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSFC.TabIndex = 8;
			this.TextBoxLSFC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonLSGetCfg
			// 
			this.ButtonLSGetCfg.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonLSGetCfg.Location = new System.Drawing.Point(6, 14);
			this.ButtonLSGetCfg.Name = "ButtonLSGetCfg";
			this.ButtonLSGetCfg.Size = new System.Drawing.Size(93, 22);
			this.ButtonLSGetCfg.TabIndex = 7;
			this.ButtonLSGetCfg.Text = "Get Configuration";
			this.ButtonLSGetCfg.Click += new System.EventHandler(this.ButtonLSGetCfg_Click);
			// 
			// groupBoxLSResult
			// 
			this.groupBoxLSResult.Controls.Add(this.LabelLSLux);
			this.groupBoxLSResult.Controls.Add(this.LabelLSR);
			this.groupBoxLSResult.Controls.Add(this.TextBoxLSR);
			this.groupBoxLSResult.Controls.Add(this.LabelLSE);
			this.groupBoxLSResult.Controls.Add(this.TextBoxLSE);
			this.groupBoxLSResult.Controls.Add(this.TextBoxLSLux);
			this.groupBoxLSResult.Controls.Add(this.ButtonLSGetLux);
			this.groupBoxLSResult.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxLSResult.Location = new System.Drawing.Point(6, 3);
			this.groupBoxLSResult.Name = "groupBoxLSResult";
			this.groupBoxLSResult.Size = new System.Drawing.Size(386, 40);
			this.groupBoxLSResult.TabIndex = 29;
			this.groupBoxLSResult.TabStop = false;
			this.groupBoxLSResult.Text = "Result";
			// 
			// LabelLSLux
			// 
			this.LabelLSLux.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSLux.Location = new System.Drawing.Point(100, 14);
			this.LabelLSLux.Name = "LabelLSLux";
			this.LabelLSLux.Size = new System.Drawing.Size(34, 25);
			this.LabelLSLux.TabIndex = 43;
			this.LabelLSLux.Text = "Lux";
			this.LabelLSLux.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelLSR
			// 
			this.LabelLSR.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSR.Location = new System.Drawing.Point(282, 14);
			this.LabelLSR.Name = "LabelLSR";
			this.LabelLSR.Size = new System.Drawing.Size(34, 25);
			this.LabelLSR.TabIndex = 42;
			this.LabelLSR.Text = "R";
			this.LabelLSR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSR
			// 
			this.TextBoxLSR.Location = new System.Drawing.Point(316, 14);
			this.TextBoxLSR.MaxLength = 10;
			this.TextBoxLSR.Name = "TextBoxLSR";
			this.TextBoxLSR.ReadOnly = true;
			this.TextBoxLSR.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSR.TabIndex = 41;
			this.TextBoxLSR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelLSE
			// 
			this.LabelLSE.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelLSE.Location = new System.Drawing.Point(191, 14);
			this.LabelLSE.Name = "LabelLSE";
			this.LabelLSE.Size = new System.Drawing.Size(34, 25);
			this.LabelLSE.TabIndex = 40;
			this.LabelLSE.Text = "E";
			this.LabelLSE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxLSE
			// 
			this.TextBoxLSE.Location = new System.Drawing.Point(225, 14);
			this.TextBoxLSE.MaxLength = 10;
			this.TextBoxLSE.Name = "TextBoxLSE";
			this.TextBoxLSE.ReadOnly = true;
			this.TextBoxLSE.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSE.TabIndex = 14;
			this.TextBoxLSE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxLSLux
			// 
			this.TextBoxLSLux.Location = new System.Drawing.Point(134, 14);
			this.TextBoxLSLux.MaxLength = 10;
			this.TextBoxLSLux.Name = "TextBoxLSLux";
			this.TextBoxLSLux.ReadOnly = true;
			this.TextBoxLSLux.Size = new System.Drawing.Size(57, 21);
			this.TextBoxLSLux.TabIndex = 8;
			this.TextBoxLSLux.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonLSGetLux
			// 
			this.ButtonLSGetLux.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonLSGetLux.Location = new System.Drawing.Point(6, 14);
			this.ButtonLSGetLux.Name = "ButtonLSGetLux";
			this.ButtonLSGetLux.Size = new System.Drawing.Size(93, 22);
			this.ButtonLSGetLux.TabIndex = 7;
			this.ButtonLSGetLux.Text = "Get Lux";
			this.ButtonLSGetLux.Click += new System.EventHandler(this.ButtonLSGetLux_Click);
			// 
			// tabAlarm
			// 
			this.tabAlarm.Controls.Add(this.groupBoxNTC);
			this.tabAlarm.Controls.Add(this.groupBoxRS485);
			this.tabAlarm.Controls.Add(this.groupBoxAlarm);
			this.tabAlarm.Controls.Add(this.groupBoxRS485Test);
			this.tabAlarm.Location = new System.Drawing.Point(4, 22);
			this.tabAlarm.Name = "tabAlarm";
			this.tabAlarm.Size = new System.Drawing.Size(1025, 382);
			this.tabAlarm.TabIndex = 6;
			this.tabAlarm.Text = "Alarm/RS485";
			this.tabAlarm.UseVisualStyleBackColor = true;
			// 
			// groupBoxNTC
			// 
			this.groupBoxNTC.Controls.Add(this.TextBoxNTC2);
			this.groupBoxNTC.Controls.Add(this.ButtonGetNTC2);
			this.groupBoxNTC.Controls.Add(this.TextBoxNTC1);
			this.groupBoxNTC.Controls.Add(this.ButtonNTCSingleScan);
			this.groupBoxNTC.Controls.Add(this.ButtonGetNTC1);
			this.groupBoxNTC.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxNTC.Location = new System.Drawing.Point(6, 173);
			this.groupBoxNTC.Name = "groupBoxNTC";
			this.groupBoxNTC.Size = new System.Drawing.Size(386, 43);
			this.groupBoxNTC.TabIndex = 34;
			this.groupBoxNTC.TabStop = false;
			this.groupBoxNTC.Text = "NTC";
			// 
			// TextBoxNTC2
			// 
			this.TextBoxNTC2.Location = new System.Drawing.Point(299, 17);
			this.TextBoxNTC2.MaxLength = 10;
			this.TextBoxNTC2.Name = "TextBoxNTC2";
			this.TextBoxNTC2.ReadOnly = true;
			this.TextBoxNTC2.Size = new System.Drawing.Size(81, 21);
			this.TextBoxNTC2.TabIndex = 47;
			this.TextBoxNTC2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonGetNTC2
			// 
			this.ButtonGetNTC2.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetNTC2.Location = new System.Drawing.Point(241, 17);
			this.ButtonGetNTC2.Name = "ButtonGetNTC2";
			this.ButtonGetNTC2.Size = new System.Drawing.Size(57, 22);
			this.ButtonGetNTC2.TabIndex = 46;
			this.ButtonGetNTC2.Text = "NTC2";
			this.ButtonGetNTC2.Click += new System.EventHandler(this.ButtonGetNTC2_Click);
			// 
			// TextBoxNTC1
			// 
			this.TextBoxNTC1.Location = new System.Drawing.Point(159, 17);
			this.TextBoxNTC1.MaxLength = 10;
			this.TextBoxNTC1.Name = "TextBoxNTC1";
			this.TextBoxNTC1.ReadOnly = true;
			this.TextBoxNTC1.Size = new System.Drawing.Size(81, 21);
			this.TextBoxNTC1.TabIndex = 45;
			this.TextBoxNTC1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonNTCSingleScan
			// 
			this.ButtonNTCSingleScan.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonNTCSingleScan.Location = new System.Drawing.Point(6, 17);
			this.ButtonNTCSingleScan.Name = "ButtonNTCSingleScan";
			this.ButtonNTCSingleScan.Size = new System.Drawing.Size(93, 22);
			this.ButtonNTCSingleScan.TabIndex = 7;
			this.ButtonNTCSingleScan.Text = "Single Scan";
			this.ButtonNTCSingleScan.Click += new System.EventHandler(this.ButtonNTCSingleScan_Click);
			// 
			// ButtonGetNTC1
			// 
			this.ButtonGetNTC1.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetNTC1.Location = new System.Drawing.Point(101, 17);
			this.ButtonGetNTC1.Name = "ButtonGetNTC1";
			this.ButtonGetNTC1.Size = new System.Drawing.Size(57, 22);
			this.ButtonGetNTC1.TabIndex = 8;
			this.ButtonGetNTC1.Text = "NTC1";
			this.ButtonGetNTC1.Click += new System.EventHandler(this.ButtonGetNTC1_Click);
			// 
			// groupBoxRS485
			// 
			this.groupBoxRS485.Controls.Add(this.groupBoxSD700Set);
			this.groupBoxRS485.Controls.Add(this.groupBoxRS485DO);
			this.groupBoxRS485.Controls.Add(this.groupBoxRS485DI);
			this.groupBoxRS485.Controls.Add(this.groupBoxRS485Comm);
			this.groupBoxRS485.Controls.Add(this.groupBoxRS485TermR);
			this.groupBoxRS485.Controls.Add(this.groupBoxRS485TransceiverMode);
			this.groupBoxRS485.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485.Location = new System.Drawing.Point(398, 3);
			this.groupBoxRS485.Name = "groupBoxRS485";
			this.groupBoxRS485.Size = new System.Drawing.Size(386, 308);
			this.groupBoxRS485.TabIndex = 33;
			this.groupBoxRS485.TabStop = false;
			this.groupBoxRS485.Text = "RS485";
			// 
			// groupBoxSD700Set
			// 
			this.groupBoxSD700Set.Controls.Add(this.ButtonRS485GetAddr);
			this.groupBoxSD700Set.Controls.Add(this.ComboBoxRS485Dev);
			this.groupBoxSD700Set.Controls.Add(this.ComboBoxSD700TrigLvl);
			this.groupBoxSD700Set.Controls.Add(this.ButtonRS485GetDev);
			this.groupBoxSD700Set.Controls.Add(this.LabelSD700TrigLvl);
			this.groupBoxSD700Set.Controls.Add(this.LabelSD700AutoScan);
			this.groupBoxSD700Set.Controls.Add(this.ComboBoxSD700AutoScan);
			this.groupBoxSD700Set.Controls.Add(this.TextBoxSD700Addr);
			this.groupBoxSD700Set.Controls.Add(this.ButtonSD700SetAddr);
			this.groupBoxSD700Set.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxSD700Set.Location = new System.Drawing.Point(6, 102);
			this.groupBoxSD700Set.Name = "groupBoxSD700Set";
			this.groupBoxSD700Set.Size = new System.Drawing.Size(374, 77);
			this.groupBoxSD700Set.TabIndex = 49;
			this.groupBoxSD700Set.TabStop = false;
			this.groupBoxSD700Set.Text = "Alarm Extension Settings";
			// 
			// ButtonRS485GetAddr
			// 
			this.ButtonRS485GetAddr.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485GetAddr.Location = new System.Drawing.Point(186, 17);
			this.ButtonRS485GetAddr.Name = "ButtonRS485GetAddr";
			this.ButtonRS485GetAddr.Size = new System.Drawing.Size(71, 22);
			this.ButtonRS485GetAddr.TabIndex = 63;
			this.ButtonRS485GetAddr.Text = "Get Address";
			this.ButtonRS485GetAddr.Click += new System.EventHandler(this.ButtonRS485GetAddr_Click);
			// 
			// ComboBoxRS485Dev
			// 
			this.ComboBoxRS485Dev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxRS485Dev.FormattingEnabled = true;
			this.ComboBoxRS485Dev.Items.AddRange(new object[] {
            "None",
            "SD700",
            "ADAM-4150"});
			this.ComboBoxRS485Dev.Location = new System.Drawing.Point(93, 15);
			this.ComboBoxRS485Dev.Name = "ComboBoxRS485Dev";
			this.ComboBoxRS485Dev.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxRS485Dev.TabIndex = 51;
			this.ComboBoxRS485Dev.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRS485Dev_SelectedIndexChanged);
			// 
			// ComboBoxSD700TrigLvl
			// 
			this.ComboBoxSD700TrigLvl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxSD700TrigLvl.FormattingEnabled = true;
			this.ComboBoxSD700TrigLvl.Items.AddRange(new object[] {
            "Low",
            "High"});
			this.ComboBoxSD700TrigLvl.Location = new System.Drawing.Point(195, 44);
			this.ComboBoxSD700TrigLvl.Name = "ComboBoxSD700TrigLvl";
			this.ComboBoxSD700TrigLvl.Size = new System.Drawing.Size(69, 24);
			this.ComboBoxSD700TrigLvl.TabIndex = 62;
			this.ComboBoxSD700TrigLvl.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSD700TrigLvl_SelectedIndexChanged);
			// 
			// ButtonRS485GetDev
			// 
			this.ButtonRS485GetDev.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485GetDev.Location = new System.Drawing.Point(6, 17);
			this.ButtonRS485GetDev.Name = "ButtonRS485GetDev";
			this.ButtonRS485GetDev.Size = new System.Drawing.Size(77, 22);
			this.ButtonRS485GetDev.TabIndex = 50;
			this.ButtonRS485GetDev.Text = "Get Device";
			this.ButtonRS485GetDev.Click += new System.EventHandler(this.ButtonRS485GetDev_Click);
			// 
			// LabelSD700TrigLvl
			// 
			this.LabelSD700TrigLvl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelSD700TrigLvl.Location = new System.Drawing.Point(137, 47);
			this.LabelSD700TrigLvl.Name = "LabelSD700TrigLvl";
			this.LabelSD700TrigLvl.Size = new System.Drawing.Size(62, 22);
			this.LabelSD700TrigLvl.TabIndex = 61;
			this.LabelSD700TrigLvl.Text = "Trig. Lvl.";
			this.LabelSD700TrigLvl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelSD700AutoScan
			// 
			this.LabelSD700AutoScan.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelSD700AutoScan.Location = new System.Drawing.Point(6, 47);
			this.LabelSD700AutoScan.Name = "LabelSD700AutoScan";
			this.LabelSD700AutoScan.Size = new System.Drawing.Size(62, 22);
			this.LabelSD700AutoScan.TabIndex = 49;
			this.LabelSD700AutoScan.Text = "Auto Scan";
			this.LabelSD700AutoScan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ComboBoxSD700AutoScan
			// 
			this.ComboBoxSD700AutoScan.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxSD700AutoScan.FormattingEnabled = true;
			this.ComboBoxSD700AutoScan.Items.AddRange(new object[] {
            "Off",
            "On"});
			this.ComboBoxSD700AutoScan.Location = new System.Drawing.Point(68, 44);
			this.ComboBoxSD700AutoScan.Name = "ComboBoxSD700AutoScan";
			this.ComboBoxSD700AutoScan.Size = new System.Drawing.Size(69, 24);
			this.ComboBoxSD700AutoScan.TabIndex = 50;
			this.ComboBoxSD700AutoScan.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSD700AutoScan_SelectedIndexChanged);
			// 
			// TextBoxSD700Addr
			// 
			this.TextBoxSD700Addr.Location = new System.Drawing.Point(329, 17);
			this.TextBoxSD700Addr.MaxLength = 10;
			this.TextBoxSD700Addr.Name = "TextBoxSD700Addr";
			this.TextBoxSD700Addr.Size = new System.Drawing.Size(33, 21);
			this.TextBoxSD700Addr.TabIndex = 60;
			this.TextBoxSD700Addr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonSD700SetAddr
			// 
			this.ButtonSD700SetAddr.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSD700SetAddr.Location = new System.Drawing.Point(257, 17);
			this.ButtonSD700SetAddr.Name = "ButtonSD700SetAddr";
			this.ButtonSD700SetAddr.Size = new System.Drawing.Size(71, 22);
			this.ButtonSD700SetAddr.TabIndex = 46;
			this.ButtonSD700SetAddr.Text = "Set Address";
			this.ButtonSD700SetAddr.Click += new System.EventHandler(this.ButtonSD700SetAddr_Click);
			// 
			// groupBoxRS485DO
			// 
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO8);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO7);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO6);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO5);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO4);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO3);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO2);
			this.groupBoxRS485DO.Controls.Add(this.CheckBoxRS485DO1);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO8);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO7);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO6);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO5);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO4);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO3);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO2);
			this.groupBoxRS485DO.Controls.Add(this.ButtonRS485DO);
			this.groupBoxRS485DO.Controls.Add(this.LabelRS485DO1);
			this.groupBoxRS485DO.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485DO.Location = new System.Drawing.Point(6, 240);
			this.groupBoxRS485DO.Name = "groupBoxRS485DO";
			this.groupBoxRS485DO.Size = new System.Drawing.Size(374, 63);
			this.groupBoxRS485DO.TabIndex = 60;
			this.groupBoxRS485DO.TabStop = false;
			this.groupBoxRS485DO.Text = "DO";
			// 
			// CheckBoxRS485DO8
			// 
			this.CheckBoxRS485DO8.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO8.Location = new System.Drawing.Point(330, 30);
			this.CheckBoxRS485DO8.Name = "CheckBoxRS485DO8";
			this.CheckBoxRS485DO8.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO8.TabIndex = 68;
			this.CheckBoxRS485DO8.Text = "Off";
			this.CheckBoxRS485DO8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO8.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO8.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO8_CheckedChanged);
			// 
			// CheckBoxRS485DO7
			// 
			this.CheckBoxRS485DO7.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO7.Location = new System.Drawing.Point(290, 30);
			this.CheckBoxRS485DO7.Name = "CheckBoxRS485DO7";
			this.CheckBoxRS485DO7.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO7.TabIndex = 67;
			this.CheckBoxRS485DO7.Text = "Off";
			this.CheckBoxRS485DO7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO7.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO7.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO7_CheckedChanged);
			// 
			// CheckBoxRS485DO6
			// 
			this.CheckBoxRS485DO6.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO6.Location = new System.Drawing.Point(250, 30);
			this.CheckBoxRS485DO6.Name = "CheckBoxRS485DO6";
			this.CheckBoxRS485DO6.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO6.TabIndex = 66;
			this.CheckBoxRS485DO6.Text = "Off";
			this.CheckBoxRS485DO6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO6.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO6.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO6_CheckedChanged);
			// 
			// CheckBoxRS485DO5
			// 
			this.CheckBoxRS485DO5.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO5.Location = new System.Drawing.Point(210, 30);
			this.CheckBoxRS485DO5.Name = "CheckBoxRS485DO5";
			this.CheckBoxRS485DO5.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO5.TabIndex = 65;
			this.CheckBoxRS485DO5.Text = "Off";
			this.CheckBoxRS485DO5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO5.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO5.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO5_CheckedChanged);
			// 
			// CheckBoxRS485DO4
			// 
			this.CheckBoxRS485DO4.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO4.Location = new System.Drawing.Point(170, 30);
			this.CheckBoxRS485DO4.Name = "CheckBoxRS485DO4";
			this.CheckBoxRS485DO4.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO4.TabIndex = 64;
			this.CheckBoxRS485DO4.Text = "Off";
			this.CheckBoxRS485DO4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO4.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO4.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO4_CheckedChanged);
			// 
			// CheckBoxRS485DO3
			// 
			this.CheckBoxRS485DO3.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO3.Location = new System.Drawing.Point(130, 30);
			this.CheckBoxRS485DO3.Name = "CheckBoxRS485DO3";
			this.CheckBoxRS485DO3.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO3.TabIndex = 63;
			this.CheckBoxRS485DO3.Text = "Off";
			this.CheckBoxRS485DO3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO3.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO3.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO3_CheckedChanged);
			// 
			// CheckBoxRS485DO2
			// 
			this.CheckBoxRS485DO2.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO2.Location = new System.Drawing.Point(90, 30);
			this.CheckBoxRS485DO2.Name = "CheckBoxRS485DO2";
			this.CheckBoxRS485DO2.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO2.TabIndex = 62;
			this.CheckBoxRS485DO2.Text = "Off";
			this.CheckBoxRS485DO2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO2.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO2.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO2_CheckedChanged);
			// 
			// CheckBoxRS485DO1
			// 
			this.CheckBoxRS485DO1.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxRS485DO1.Location = new System.Drawing.Point(50, 30);
			this.CheckBoxRS485DO1.Name = "CheckBoxRS485DO1";
			this.CheckBoxRS485DO1.Size = new System.Drawing.Size(33, 26);
			this.CheckBoxRS485DO1.TabIndex = 61;
			this.CheckBoxRS485DO1.Text = "Off";
			this.CheckBoxRS485DO1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxRS485DO1.UseVisualStyleBackColor = true;
			this.CheckBoxRS485DO1.CheckedChanged += new System.EventHandler(this.CheckBoxRS485DO1_CheckedChanged);
			// 
			// LabelRS485DO8
			// 
			this.LabelRS485DO8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO8.Location = new System.Drawing.Point(330, 9);
			this.LabelRS485DO8.Name = "LabelRS485DO8";
			this.LabelRS485DO8.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO8.TabIndex = 53;
			this.LabelRS485DO8.Text = "DO8";
			this.LabelRS485DO8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DO7
			// 
			this.LabelRS485DO7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO7.Location = new System.Drawing.Point(290, 9);
			this.LabelRS485DO7.Name = "LabelRS485DO7";
			this.LabelRS485DO7.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO7.TabIndex = 52;
			this.LabelRS485DO7.Text = "DO7";
			this.LabelRS485DO7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DO6
			// 
			this.LabelRS485DO6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO6.Location = new System.Drawing.Point(250, 9);
			this.LabelRS485DO6.Name = "LabelRS485DO6";
			this.LabelRS485DO6.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO6.TabIndex = 51;
			this.LabelRS485DO6.Text = "DO6";
			this.LabelRS485DO6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DO5
			// 
			this.LabelRS485DO5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO5.Location = new System.Drawing.Point(210, 9);
			this.LabelRS485DO5.Name = "LabelRS485DO5";
			this.LabelRS485DO5.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO5.TabIndex = 50;
			this.LabelRS485DO5.Text = "DO5";
			this.LabelRS485DO5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DO4
			// 
			this.LabelRS485DO4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO4.Location = new System.Drawing.Point(170, 9);
			this.LabelRS485DO4.Name = "LabelRS485DO4";
			this.LabelRS485DO4.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO4.TabIndex = 49;
			this.LabelRS485DO4.Text = "DO4";
			this.LabelRS485DO4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DO3
			// 
			this.LabelRS485DO3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO3.Location = new System.Drawing.Point(130, 9);
			this.LabelRS485DO3.Name = "LabelRS485DO3";
			this.LabelRS485DO3.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO3.TabIndex = 48;
			this.LabelRS485DO3.Text = "DO3";
			this.LabelRS485DO3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DO2
			// 
			this.LabelRS485DO2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO2.Location = new System.Drawing.Point(90, 9);
			this.LabelRS485DO2.Name = "LabelRS485DO2";
			this.LabelRS485DO2.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO2.TabIndex = 47;
			this.LabelRS485DO2.Text = "DO2";
			this.LabelRS485DO2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonRS485DO
			// 
			this.ButtonRS485DO.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485DO.Location = new System.Drawing.Point(6, 24);
			this.ButtonRS485DO.Name = "ButtonRS485DO";
			this.ButtonRS485DO.Size = new System.Drawing.Size(38, 22);
			this.ButtonRS485DO.TabIndex = 46;
			this.ButtonRS485DO.Text = "Read";
			this.ButtonRS485DO.Click += new System.EventHandler(this.ButtonRS485DO_Click);
			// 
			// LabelRS485DO1
			// 
			this.LabelRS485DO1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DO1.Location = new System.Drawing.Point(50, 9);
			this.LabelRS485DO1.Name = "LabelRS485DO1";
			this.LabelRS485DO1.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DO1.TabIndex = 30;
			this.LabelRS485DO1.Text = "DO1";
			this.LabelRS485DO1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBoxRS485DI
			// 
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI8);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI7);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI6);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI5);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI4);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI3);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI2);
			this.groupBoxRS485DI.Controls.Add(this.TextBoxRS485DI1);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI8);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI7);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI6);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI5);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI4);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI3);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI2);
			this.groupBoxRS485DI.Controls.Add(this.ButtonRS485DI);
			this.groupBoxRS485DI.Controls.Add(this.LabelRS485DI1);
			this.groupBoxRS485DI.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485DI.Location = new System.Drawing.Point(6, 178);
			this.groupBoxRS485DI.Name = "groupBoxRS485DI";
			this.groupBoxRS485DI.Size = new System.Drawing.Size(374, 63);
			this.groupBoxRS485DI.TabIndex = 49;
			this.groupBoxRS485DI.TabStop = false;
			this.groupBoxRS485DI.Text = "DI";
			// 
			// TextBoxRS485DI8
			// 
			this.TextBoxRS485DI8.Location = new System.Drawing.Point(330, 33);
			this.TextBoxRS485DI8.MaxLength = 10;
			this.TextBoxRS485DI8.Name = "TextBoxRS485DI8";
			this.TextBoxRS485DI8.ReadOnly = true;
			this.TextBoxRS485DI8.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI8.TabIndex = 59;
			this.TextBoxRS485DI8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI7
			// 
			this.TextBoxRS485DI7.Location = new System.Drawing.Point(290, 33);
			this.TextBoxRS485DI7.MaxLength = 10;
			this.TextBoxRS485DI7.Name = "TextBoxRS485DI7";
			this.TextBoxRS485DI7.ReadOnly = true;
			this.TextBoxRS485DI7.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI7.TabIndex = 58;
			this.TextBoxRS485DI7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI6
			// 
			this.TextBoxRS485DI6.Location = new System.Drawing.Point(250, 33);
			this.TextBoxRS485DI6.MaxLength = 10;
			this.TextBoxRS485DI6.Name = "TextBoxRS485DI6";
			this.TextBoxRS485DI6.ReadOnly = true;
			this.TextBoxRS485DI6.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI6.TabIndex = 57;
			this.TextBoxRS485DI6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI5
			// 
			this.TextBoxRS485DI5.Location = new System.Drawing.Point(210, 33);
			this.TextBoxRS485DI5.MaxLength = 10;
			this.TextBoxRS485DI5.Name = "TextBoxRS485DI5";
			this.TextBoxRS485DI5.ReadOnly = true;
			this.TextBoxRS485DI5.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI5.TabIndex = 56;
			this.TextBoxRS485DI5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI4
			// 
			this.TextBoxRS485DI4.Location = new System.Drawing.Point(170, 33);
			this.TextBoxRS485DI4.MaxLength = 10;
			this.TextBoxRS485DI4.Name = "TextBoxRS485DI4";
			this.TextBoxRS485DI4.ReadOnly = true;
			this.TextBoxRS485DI4.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI4.TabIndex = 55;
			this.TextBoxRS485DI4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI3
			// 
			this.TextBoxRS485DI3.Location = new System.Drawing.Point(130, 33);
			this.TextBoxRS485DI3.MaxLength = 10;
			this.TextBoxRS485DI3.Name = "TextBoxRS485DI3";
			this.TextBoxRS485DI3.ReadOnly = true;
			this.TextBoxRS485DI3.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI3.TabIndex = 46;
			this.TextBoxRS485DI3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI2
			// 
			this.TextBoxRS485DI2.Location = new System.Drawing.Point(90, 33);
			this.TextBoxRS485DI2.MaxLength = 10;
			this.TextBoxRS485DI2.Name = "TextBoxRS485DI2";
			this.TextBoxRS485DI2.ReadOnly = true;
			this.TextBoxRS485DI2.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI2.TabIndex = 54;
			this.TextBoxRS485DI2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxRS485DI1
			// 
			this.TextBoxRS485DI1.Location = new System.Drawing.Point(50, 33);
			this.TextBoxRS485DI1.MaxLength = 10;
			this.TextBoxRS485DI1.Name = "TextBoxRS485DI1";
			this.TextBoxRS485DI1.ReadOnly = true;
			this.TextBoxRS485DI1.Size = new System.Drawing.Size(33, 21);
			this.TextBoxRS485DI1.TabIndex = 45;
			this.TextBoxRS485DI1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelRS485DI8
			// 
			this.LabelRS485DI8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI8.Location = new System.Drawing.Point(330, 9);
			this.LabelRS485DI8.Name = "LabelRS485DI8";
			this.LabelRS485DI8.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI8.TabIndex = 53;
			this.LabelRS485DI8.Text = "DI8";
			this.LabelRS485DI8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DI7
			// 
			this.LabelRS485DI7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI7.Location = new System.Drawing.Point(290, 9);
			this.LabelRS485DI7.Name = "LabelRS485DI7";
			this.LabelRS485DI7.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI7.TabIndex = 52;
			this.LabelRS485DI7.Text = "DI7";
			this.LabelRS485DI7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DI6
			// 
			this.LabelRS485DI6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI6.Location = new System.Drawing.Point(250, 9);
			this.LabelRS485DI6.Name = "LabelRS485DI6";
			this.LabelRS485DI6.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI6.TabIndex = 51;
			this.LabelRS485DI6.Text = "DI6";
			this.LabelRS485DI6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DI5
			// 
			this.LabelRS485DI5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI5.Location = new System.Drawing.Point(210, 9);
			this.LabelRS485DI5.Name = "LabelRS485DI5";
			this.LabelRS485DI5.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI5.TabIndex = 50;
			this.LabelRS485DI5.Text = "DI5";
			this.LabelRS485DI5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DI4
			// 
			this.LabelRS485DI4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI4.Location = new System.Drawing.Point(170, 9);
			this.LabelRS485DI4.Name = "LabelRS485DI4";
			this.LabelRS485DI4.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI4.TabIndex = 49;
			this.LabelRS485DI4.Text = "DI4";
			this.LabelRS485DI4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DI3
			// 
			this.LabelRS485DI3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI3.Location = new System.Drawing.Point(130, 9);
			this.LabelRS485DI3.Name = "LabelRS485DI3";
			this.LabelRS485DI3.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI3.TabIndex = 48;
			this.LabelRS485DI3.Text = "DI3";
			this.LabelRS485DI3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelRS485DI2
			// 
			this.LabelRS485DI2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI2.Location = new System.Drawing.Point(90, 9);
			this.LabelRS485DI2.Name = "LabelRS485DI2";
			this.LabelRS485DI2.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI2.TabIndex = 47;
			this.LabelRS485DI2.Text = "DI2";
			this.LabelRS485DI2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonRS485DI
			// 
			this.ButtonRS485DI.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485DI.Location = new System.Drawing.Point(6, 24);
			this.ButtonRS485DI.Name = "ButtonRS485DI";
			this.ButtonRS485DI.Size = new System.Drawing.Size(38, 22);
			this.ButtonRS485DI.TabIndex = 46;
			this.ButtonRS485DI.Text = "Read";
			this.ButtonRS485DI.Click += new System.EventHandler(this.ButtonRS485DI_Click);
			// 
			// LabelRS485DI1
			// 
			this.LabelRS485DI1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485DI1.Location = new System.Drawing.Point(50, 9);
			this.LabelRS485DI1.Name = "LabelRS485DI1";
			this.LabelRS485DI1.Size = new System.Drawing.Size(35, 22);
			this.LabelRS485DI1.TabIndex = 30;
			this.LabelRS485DI1.Text = "DI1";
			this.LabelRS485DI1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBoxRS485Comm
			// 
			this.groupBoxRS485Comm.Controls.Add(this.ComboBoxRS485StopBits);
			this.groupBoxRS485Comm.Controls.Add(this.LabelRS485StopBits);
			this.groupBoxRS485Comm.Controls.Add(this.ButtonRS485GetComm);
			this.groupBoxRS485Comm.Controls.Add(this.ComboBoxRS485BaudRate);
			this.groupBoxRS485Comm.Controls.Add(this.LabelRS485BaudRate);
			this.groupBoxRS485Comm.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485Comm.Location = new System.Drawing.Point(6, 14);
			this.groupBoxRS485Comm.Name = "groupBoxRS485Comm";
			this.groupBoxRS485Comm.Size = new System.Drawing.Size(374, 45);
			this.groupBoxRS485Comm.TabIndex = 46;
			this.groupBoxRS485Comm.TabStop = false;
			this.groupBoxRS485Comm.Text = "Communication Settings";
			// 
			// ComboBoxRS485StopBits
			// 
			this.ComboBoxRS485StopBits.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxRS485StopBits.FormattingEnabled = true;
			this.ComboBoxRS485StopBits.Items.AddRange(new object[] {
            "N 8 1",
            "N 8 2"});
			this.ComboBoxRS485StopBits.Location = new System.Drawing.Point(287, 14);
			this.ComboBoxRS485StopBits.Name = "ComboBoxRS485StopBits";
			this.ComboBoxRS485StopBits.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxRS485StopBits.TabIndex = 47;
			this.ComboBoxRS485StopBits.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRS485StopBits_SelectedIndexChanged);
			// 
			// LabelRS485StopBits
			// 
			this.LabelRS485StopBits.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485StopBits.Location = new System.Drawing.Point(225, 17);
			this.LabelRS485StopBits.Name = "LabelRS485StopBits";
			this.LabelRS485StopBits.Size = new System.Drawing.Size(68, 22);
			this.LabelRS485StopBits.TabIndex = 48;
			this.LabelRS485StopBits.Text = "Stop Bits";
			this.LabelRS485StopBits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonRS485GetComm
			// 
			this.ButtonRS485GetComm.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485GetComm.Location = new System.Drawing.Point(6, 17);
			this.ButtonRS485GetComm.Name = "ButtonRS485GetComm";
			this.ButtonRS485GetComm.Size = new System.Drawing.Size(77, 22);
			this.ButtonRS485GetComm.TabIndex = 46;
			this.ButtonRS485GetComm.Text = "Get";
			this.ButtonRS485GetComm.Click += new System.EventHandler(this.ButtonRS485GetComm_Click);
			// 
			// ComboBoxRS485BaudRate
			// 
			this.ComboBoxRS485BaudRate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxRS485BaudRate.FormattingEnabled = true;
			this.ComboBoxRS485BaudRate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
			this.ComboBoxRS485BaudRate.Location = new System.Drawing.Point(144, 14);
			this.ComboBoxRS485BaudRate.Name = "ComboBoxRS485BaudRate";
			this.ComboBoxRS485BaudRate.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxRS485BaudRate.TabIndex = 29;
			this.ComboBoxRS485BaudRate.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRS485BaudRate_SelectedIndexChanged);
			// 
			// LabelRS485BaudRate
			// 
			this.LabelRS485BaudRate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485BaudRate.Location = new System.Drawing.Point(80, 17);
			this.LabelRS485BaudRate.Name = "LabelRS485BaudRate";
			this.LabelRS485BaudRate.Size = new System.Drawing.Size(68, 22);
			this.LabelRS485BaudRate.TabIndex = 30;
			this.LabelRS485BaudRate.Text = "Baud Rate";
			this.LabelRS485BaudRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBoxRS485TermR
			// 
			this.groupBoxRS485TermR.Controls.Add(this.ComboBoxRS485TermR);
			this.groupBoxRS485TermR.Controls.Add(this.ButtonRS485GetTermR);
			this.groupBoxRS485TermR.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485TermR.Location = new System.Drawing.Point(200, 59);
			this.groupBoxRS485TermR.Name = "groupBoxRS485TermR";
			this.groupBoxRS485TermR.Size = new System.Drawing.Size(180, 43);
			this.groupBoxRS485TermR.TabIndex = 46;
			this.groupBoxRS485TermR.TabStop = false;
			this.groupBoxRS485TermR.Text = "Terminal Resistor";
			// 
			// ComboBoxRS485TermR
			// 
			this.ComboBoxRS485TermR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxRS485TermR.FormattingEnabled = true;
			this.ComboBoxRS485TermR.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
			this.ComboBoxRS485TermR.Location = new System.Drawing.Point(93, 12);
			this.ComboBoxRS485TermR.Name = "ComboBoxRS485TermR";
			this.ComboBoxRS485TermR.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxRS485TermR.TabIndex = 50;
			this.ComboBoxRS485TermR.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRS485TermR_SelectedIndexChanged);
			// 
			// ButtonRS485GetTermR
			// 
			this.ButtonRS485GetTermR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485GetTermR.Location = new System.Drawing.Point(6, 15);
			this.ButtonRS485GetTermR.Name = "ButtonRS485GetTermR";
			this.ButtonRS485GetTermR.Size = new System.Drawing.Size(77, 22);
			this.ButtonRS485GetTermR.TabIndex = 34;
			this.ButtonRS485GetTermR.Text = "Get Status";
			this.ButtonRS485GetTermR.Click += new System.EventHandler(this.ButtonRS485GetTermR_Click);
			// 
			// groupBoxRS485TransceiverMode
			// 
			this.groupBoxRS485TransceiverMode.Controls.Add(this.ComboBoxRS485Mode);
			this.groupBoxRS485TransceiverMode.Controls.Add(this.ButtonRS485GetMode);
			this.groupBoxRS485TransceiverMode.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485TransceiverMode.Location = new System.Drawing.Point(6, 59);
			this.groupBoxRS485TransceiverMode.Name = "groupBoxRS485TransceiverMode";
			this.groupBoxRS485TransceiverMode.Size = new System.Drawing.Size(180, 43);
			this.groupBoxRS485TransceiverMode.TabIndex = 33;
			this.groupBoxRS485TransceiverMode.TabStop = false;
			this.groupBoxRS485TransceiverMode.Text = "Transceiver Mode";
			// 
			// ComboBoxRS485Mode
			// 
			this.ComboBoxRS485Mode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxRS485Mode.FormattingEnabled = true;
			this.ComboBoxRS485Mode.Items.AddRange(new object[] {
            "Driver",
            "Receiver"});
			this.ComboBoxRS485Mode.Location = new System.Drawing.Point(93, 13);
			this.ComboBoxRS485Mode.Name = "ComboBoxRS485Mode";
			this.ComboBoxRS485Mode.Size = new System.Drawing.Size(81, 24);
			this.ComboBoxRS485Mode.TabIndex = 49;
			this.ComboBoxRS485Mode.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRS485Mode_SelectedIndexChanged);
			// 
			// ButtonRS485GetMode
			// 
			this.ButtonRS485GetMode.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485GetMode.Location = new System.Drawing.Point(6, 16);
			this.ButtonRS485GetMode.Name = "ButtonRS485GetMode";
			this.ButtonRS485GetMode.Size = new System.Drawing.Size(77, 22);
			this.ButtonRS485GetMode.TabIndex = 34;
			this.ButtonRS485GetMode.Text = "Get Mode";
			this.ButtonRS485GetMode.Click += new System.EventHandler(this.ButtonRS485GetMode_Click);
			// 
			// groupBoxAlarm
			// 
			this.groupBoxAlarm.Controls.Add(this.groupBoxAlarmInq);
			this.groupBoxAlarm.Controls.Add(this.groupBoxAlarmOut);
			this.groupBoxAlarm.Controls.Add(this.groupBoxAlarmAuto);
			this.groupBoxAlarm.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAlarm.Location = new System.Drawing.Point(6, 3);
			this.groupBoxAlarm.Name = "groupBoxAlarm";
			this.groupBoxAlarm.Size = new System.Drawing.Size(386, 171);
			this.groupBoxAlarm.TabIndex = 30;
			this.groupBoxAlarm.TabStop = false;
			this.groupBoxAlarm.Text = "Alarm";
			// 
			// groupBoxAlarmInq
			// 
			this.groupBoxAlarmInq.Controls.Add(this.TextBoxAlarmDI1);
			this.groupBoxAlarmInq.Controls.Add(this.LabelAlarmDI1);
			this.groupBoxAlarmInq.Controls.Add(this.TextBoxAlarmDI0);
			this.groupBoxAlarmInq.Controls.Add(this.LabelAlarmDI0);
			this.groupBoxAlarmInq.Controls.Add(this.ButtonAlarmIn);
			this.groupBoxAlarmInq.Controls.Add(this.TextBoxAlarmTrigLvl);
			this.groupBoxAlarmInq.Controls.Add(this.TextBoxAlarmStatus);
			this.groupBoxAlarmInq.Controls.Add(this.ButtonAlarmAutoStatus);
			this.groupBoxAlarmInq.Controls.Add(this.ButtonAlarmTrigLvl);
			this.groupBoxAlarmInq.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAlarmInq.Location = new System.Drawing.Point(6, 98);
			this.groupBoxAlarmInq.Name = "groupBoxAlarmInq";
			this.groupBoxAlarmInq.Size = new System.Drawing.Size(374, 68);
			this.groupBoxAlarmInq.TabIndex = 33;
			this.groupBoxAlarmInq.TabStop = false;
			this.groupBoxAlarmInq.Text = "Alarm Inquiry";
			// 
			// TextBoxAlarmDI1
			// 
			this.TextBoxAlarmDI1.Location = new System.Drawing.Point(205, 40);
			this.TextBoxAlarmDI1.MaxLength = 10;
			this.TextBoxAlarmDI1.Name = "TextBoxAlarmDI1";
			this.TextBoxAlarmDI1.ReadOnly = true;
			this.TextBoxAlarmDI1.Size = new System.Drawing.Size(57, 21);
			this.TextBoxAlarmDI1.TabIndex = 44;
			this.TextBoxAlarmDI1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelAlarmDI1
			// 
			this.LabelAlarmDI1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelAlarmDI1.Location = new System.Drawing.Point(182, 38);
			this.LabelAlarmDI1.Name = "LabelAlarmDI1";
			this.LabelAlarmDI1.Size = new System.Drawing.Size(26, 25);
			this.LabelAlarmDI1.TabIndex = 43;
			this.LabelAlarmDI1.Text = "DI1";
			this.LabelAlarmDI1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxAlarmDI0
			// 
			this.TextBoxAlarmDI0.Location = new System.Drawing.Point(122, 40);
			this.TextBoxAlarmDI0.MaxLength = 10;
			this.TextBoxAlarmDI0.Name = "TextBoxAlarmDI0";
			this.TextBoxAlarmDI0.ReadOnly = true;
			this.TextBoxAlarmDI0.Size = new System.Drawing.Size(57, 21);
			this.TextBoxAlarmDI0.TabIndex = 42;
			this.TextBoxAlarmDI0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelAlarmDI0
			// 
			this.LabelAlarmDI0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelAlarmDI0.Location = new System.Drawing.Point(98, 38);
			this.LabelAlarmDI0.Name = "LabelAlarmDI0";
			this.LabelAlarmDI0.Size = new System.Drawing.Size(26, 25);
			this.LabelAlarmDI0.TabIndex = 41;
			this.LabelAlarmDI0.Text = "DI0";
			this.LabelAlarmDI0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonAlarmIn
			// 
			this.ButtonAlarmIn.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmIn.Location = new System.Drawing.Point(6, 40);
			this.ButtonAlarmIn.Name = "ButtonAlarmIn";
			this.ButtonAlarmIn.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmIn.TabIndex = 33;
			this.ButtonAlarmIn.Text = "Alarm In";
			this.ButtonAlarmIn.Click += new System.EventHandler(this.ButtonAlarmIn_Click);
			// 
			// TextBoxAlarmTrigLvl
			// 
			this.TextBoxAlarmTrigLvl.Location = new System.Drawing.Point(283, 15);
			this.TextBoxAlarmTrigLvl.MaxLength = 10;
			this.TextBoxAlarmTrigLvl.Name = "TextBoxAlarmTrigLvl";
			this.TextBoxAlarmTrigLvl.ReadOnly = true;
			this.TextBoxAlarmTrigLvl.Size = new System.Drawing.Size(85, 21);
			this.TextBoxAlarmTrigLvl.TabIndex = 32;
			this.TextBoxAlarmTrigLvl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBoxAlarmStatus
			// 
			this.TextBoxAlarmStatus.Location = new System.Drawing.Point(100, 15);
			this.TextBoxAlarmStatus.MaxLength = 10;
			this.TextBoxAlarmStatus.Name = "TextBoxAlarmStatus";
			this.TextBoxAlarmStatus.ReadOnly = true;
			this.TextBoxAlarmStatus.Size = new System.Drawing.Size(85, 21);
			this.TextBoxAlarmStatus.TabIndex = 31;
			this.TextBoxAlarmStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonAlarmAutoStatus
			// 
			this.ButtonAlarmAutoStatus.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmAutoStatus.Location = new System.Drawing.Point(6, 15);
			this.ButtonAlarmAutoStatus.Name = "ButtonAlarmAutoStatus";
			this.ButtonAlarmAutoStatus.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmAutoStatus.TabIndex = 7;
			this.ButtonAlarmAutoStatus.Text = "Status";
			this.ButtonAlarmAutoStatus.Click += new System.EventHandler(this.ButtonAlarmAutoStatus_Click);
			// 
			// ButtonAlarmTrigLvl
			// 
			this.ButtonAlarmTrigLvl.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmTrigLvl.Location = new System.Drawing.Point(188, 15);
			this.ButtonAlarmTrigLvl.Name = "ButtonAlarmTrigLvl";
			this.ButtonAlarmTrigLvl.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmTrigLvl.TabIndex = 8;
			this.ButtonAlarmTrigLvl.Text = "Trigger Level";
			this.ButtonAlarmTrigLvl.Click += new System.EventHandler(this.ButtonAlarmTrigLvl_Click);
			// 
			// groupBoxAlarmOut
			// 
			this.groupBoxAlarmOut.Controls.Add(this.ButtonAlarmOutHigh);
			this.groupBoxAlarmOut.Controls.Add(this.ButtonAlarmOutLow);
			this.groupBoxAlarmOut.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAlarmOut.Location = new System.Drawing.Point(6, 56);
			this.groupBoxAlarmOut.Name = "groupBoxAlarmOut";
			this.groupBoxAlarmOut.Size = new System.Drawing.Size(197, 43);
			this.groupBoxAlarmOut.TabIndex = 32;
			this.groupBoxAlarmOut.TabStop = false;
			this.groupBoxAlarmOut.Text = "Alarm Out";
			// 
			// ButtonAlarmOutHigh
			// 
			this.ButtonAlarmOutHigh.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmOutHigh.Location = new System.Drawing.Point(6, 15);
			this.ButtonAlarmOutHigh.Name = "ButtonAlarmOutHigh";
			this.ButtonAlarmOutHigh.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmOutHigh.TabIndex = 7;
			this.ButtonAlarmOutHigh.Text = "Alarm Out High";
			this.ButtonAlarmOutHigh.Click += new System.EventHandler(this.ButtonAlarmOutHigh_Click);
			// 
			// ButtonAlarmOutLow
			// 
			this.ButtonAlarmOutLow.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmOutLow.Location = new System.Drawing.Point(99, 15);
			this.ButtonAlarmOutLow.Name = "ButtonAlarmOutLow";
			this.ButtonAlarmOutLow.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmOutLow.TabIndex = 8;
			this.ButtonAlarmOutLow.Text = "Alarm Out Low";
			this.ButtonAlarmOutLow.Click += new System.EventHandler(this.ButtonAlarmOutLow_Click);
			// 
			// groupBoxAlarmAuto
			// 
			this.groupBoxAlarmAuto.Controls.Add(this.LabelAlarmTrigLvl);
			this.groupBoxAlarmAuto.Controls.Add(this.ComboBoxAlarmTrigLvl);
			this.groupBoxAlarmAuto.Controls.Add(this.ButtonAlarmOn);
			this.groupBoxAlarmAuto.Controls.Add(this.ButtonAlarmOff);
			this.groupBoxAlarmAuto.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxAlarmAuto.Location = new System.Drawing.Point(6, 14);
			this.groupBoxAlarmAuto.Name = "groupBoxAlarmAuto";
			this.groupBoxAlarmAuto.Size = new System.Drawing.Size(374, 43);
			this.groupBoxAlarmAuto.TabIndex = 31;
			this.groupBoxAlarmAuto.TabStop = false;
			this.groupBoxAlarmAuto.Text = "Alarm Auto";
			// 
			// LabelAlarmTrigLvl
			// 
			this.LabelAlarmTrigLvl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelAlarmTrigLvl.Location = new System.Drawing.Point(198, 15);
			this.LabelAlarmTrigLvl.Name = "LabelAlarmTrigLvl";
			this.LabelAlarmTrigLvl.Size = new System.Drawing.Size(86, 25);
			this.LabelAlarmTrigLvl.TabIndex = 40;
			this.LabelAlarmTrigLvl.Text = "Trigger Level";
			this.LabelAlarmTrigLvl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ComboBoxAlarmTrigLvl
			// 
			this.ComboBoxAlarmTrigLvl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxAlarmTrigLvl.FormattingEnabled = true;
			this.ComboBoxAlarmTrigLvl.Items.AddRange(new object[] {
            "Low",
            "High"});
			this.ComboBoxAlarmTrigLvl.Location = new System.Drawing.Point(287, 15);
			this.ComboBoxAlarmTrigLvl.Name = "ComboBoxAlarmTrigLvl";
			this.ComboBoxAlarmTrigLvl.Size = new System.Drawing.Size(78, 24);
			this.ComboBoxAlarmTrigLvl.TabIndex = 37;
			this.ComboBoxAlarmTrigLvl.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAlarmTrigLvl_SelectedIndexChanged);
			// 
			// ButtonAlarmOn
			// 
			this.ButtonAlarmOn.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmOn.Location = new System.Drawing.Point(6, 15);
			this.ButtonAlarmOn.Name = "ButtonAlarmOn";
			this.ButtonAlarmOn.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmOn.TabIndex = 7;
			this.ButtonAlarmOn.Text = "Auto On";
			this.ButtonAlarmOn.Click += new System.EventHandler(this.ButtonAlarmOn_Click);
			// 
			// ButtonAlarmOff
			// 
			this.ButtonAlarmOff.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonAlarmOff.Location = new System.Drawing.Point(99, 15);
			this.ButtonAlarmOff.Name = "ButtonAlarmOff";
			this.ButtonAlarmOff.Size = new System.Drawing.Size(93, 22);
			this.ButtonAlarmOff.TabIndex = 8;
			this.ButtonAlarmOff.Text = "Auto Off";
			this.ButtonAlarmOff.Click += new System.EventHandler(this.ButtonAlarmOff_Click);
			// 
			// groupBoxRS485Test
			// 
			this.groupBoxRS485Test.Controls.Add(this.TextBoxRS485TestTx);
			this.groupBoxRS485Test.Controls.Add(this.LabelRS485TestRx);
			this.groupBoxRS485Test.Controls.Add(this.TextBoxRS485TestRx);
			this.groupBoxRS485Test.Controls.Add(this.LabelRS485TestTX);
			this.groupBoxRS485Test.Controls.Add(this.ButtonRS485TestTx);
			this.groupBoxRS485Test.Controls.Add(this.ButtonRS485TestRx);
			this.groupBoxRS485Test.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxRS485Test.Location = new System.Drawing.Point(6, 215);
			this.groupBoxRS485Test.Name = "groupBoxRS485Test";
			this.groupBoxRS485Test.Size = new System.Drawing.Size(374, 65);
			this.groupBoxRS485Test.TabIndex = 47;
			this.groupBoxRS485Test.TabStop = false;
			this.groupBoxRS485Test.Text = "RS485 Test Packet";
			// 
			// TextBoxRS485TestTx
			// 
			this.TextBoxRS485TestTx.Location = new System.Drawing.Point(106, 15);
			this.TextBoxRS485TestTx.MaxLength = 10;
			this.TextBoxRS485TestTx.Name = "TextBoxRS485TestTx";
			this.TextBoxRS485TestTx.Size = new System.Drawing.Size(57, 21);
			this.TextBoxRS485TestTx.TabIndex = 47;
			this.TextBoxRS485TestTx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelRS485TestRx
			// 
			this.LabelRS485TestRx.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485TestRx.Location = new System.Drawing.Point(163, 36);
			this.LabelRS485TestRx.Name = "LabelRS485TestRx";
			this.LabelRS485TestRx.Size = new System.Drawing.Size(205, 25);
			this.LabelRS485TestRx.TabIndex = 46;
			this.LabelRS485TestRx.Text = "(0~255) Test byte -> RS485 -> UART";
			this.LabelRS485TestRx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxRS485TestRx
			// 
			this.TextBoxRS485TestRx.Location = new System.Drawing.Point(106, 38);
			this.TextBoxRS485TestRx.MaxLength = 10;
			this.TextBoxRS485TestRx.Name = "TextBoxRS485TestRx";
			this.TextBoxRS485TestRx.Size = new System.Drawing.Size(57, 21);
			this.TextBoxRS485TestRx.TabIndex = 45;
			this.TextBoxRS485TestRx.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelRS485TestTX
			// 
			this.LabelRS485TestTX.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelRS485TestTX.Location = new System.Drawing.Point(163, 13);
			this.LabelRS485TestTX.Name = "LabelRS485TestTX";
			this.LabelRS485TestTX.Size = new System.Drawing.Size(205, 25);
			this.LabelRS485TestTX.TabIndex = 45;
			this.LabelRS485TestTX.Text = "(0~255) Test byte -> UART -> RS485";
			this.LabelRS485TestTX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonRS485TestTx
			// 
			this.ButtonRS485TestTx.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485TestTx.Location = new System.Drawing.Point(6, 15);
			this.ButtonRS485TestTx.Name = "ButtonRS485TestTx";
			this.ButtonRS485TestTx.Size = new System.Drawing.Size(99, 22);
			this.ButtonRS485TestTx.TabIndex = 7;
			this.ButtonRS485TestTx.Text = "Test Tx";
			this.ButtonRS485TestTx.Click += new System.EventHandler(this.ButtonRS485TestTx_Click);
			// 
			// ButtonRS485TestRx
			// 
			this.ButtonRS485TestRx.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonRS485TestRx.Location = new System.Drawing.Point(6, 38);
			this.ButtonRS485TestRx.Name = "ButtonRS485TestRx";
			this.ButtonRS485TestRx.Size = new System.Drawing.Size(99, 22);
			this.ButtonRS485TestRx.TabIndex = 8;
			this.ButtonRS485TestRx.Text = "Test Rx";
			this.ButtonRS485TestRx.Click += new System.EventHandler(this.ButtonRS485TestRx_Click);
			// 
			// tabTMC2209
			// 
			this.tabTMC2209.Controls.Add(this.GroupBox_Chopper_Control_Register);
			this.tabTMC2209.Controls.Add(this.GroupBox_Chopper_PWMCONF);
			this.tabTMC2209.Controls.Add(this.GroupBox_Chopper_DRV_STATUS);
			this.tabTMC2209.Controls.Add(this.GroupBox_Chopper_CHOPCONF);
			this.tabTMC2209.Controls.Add(this.GroupBox_STALLGARD_COOLCONF);
			this.tabTMC2209.Controls.Add(this.GroupBox_Sequencer_Registers);
			this.tabTMC2209.Controls.Add(this.GroupBox_StallGuard_Control);
			this.tabTMC2209.Controls.Add(this.GroupBox_Velocity_Dependent_Control);
			this.tabTMC2209.Controls.Add(this.GroupBox_General);
			this.tabTMC2209.Location = new System.Drawing.Point(4, 22);
			this.tabTMC2209.Name = "tabTMC2209";
			this.tabTMC2209.Padding = new System.Windows.Forms.Padding(3);
			this.tabTMC2209.Size = new System.Drawing.Size(1025, 382);
			this.tabTMC2209.TabIndex = 8;
			this.tabTMC2209.Text = "TMC2209";
			this.tabTMC2209.UseVisualStyleBackColor = true;
			// 
			// GroupBox_Chopper_Control_Register
			// 
			this.GroupBox_Chopper_Control_Register.Controls.Add(this.GroupBox_PWM_SCALE_AUTO);
			this.GroupBox_Chopper_Control_Register.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_Chopper_Control_Register.Location = new System.Drawing.Point(714, 250);
			this.GroupBox_Chopper_Control_Register.Name = "GroupBox_Chopper_Control_Register";
			this.GroupBox_Chopper_Control_Register.Size = new System.Drawing.Size(169, 122);
			this.GroupBox_Chopper_Control_Register.TabIndex = 62;
			this.GroupBox_Chopper_Control_Register.TabStop = false;
			this.GroupBox_Chopper_Control_Register.Text = "Chopper Control Register";
			// 
			// GroupBox_PWM_SCALE_AUTO
			// 
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.TextBox_PWM_GRAD_AUTO);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.Button_PWM_GRAD_AUTO);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.TextBox_PWM_OFS_AUTO);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.Button_PWM_OFS_AUTO);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.TextBox_PWM_SCALE_AUTO);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.Button_PWM_SCALE_AUTO);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.TextBox_PWM_SCALE_SUM);
			this.GroupBox_PWM_SCALE_AUTO.Controls.Add(this.Button_PWM_SCALE_SUM);
			this.GroupBox_PWM_SCALE_AUTO.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_PWM_SCALE_AUTO.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_PWM_SCALE_AUTO.Name = "GroupBox_PWM_SCALE_AUTO";
			this.GroupBox_PWM_SCALE_AUTO.Size = new System.Drawing.Size(164, 104);
			this.GroupBox_PWM_SCALE_AUTO.TabIndex = 56;
			this.GroupBox_PWM_SCALE_AUTO.TabStop = false;
			this.GroupBox_PWM_SCALE_AUTO.Text = "PWM_SCALE / PWM_AUTO";
			// 
			// TextBox_PWM_GRAD_AUTO
			// 
			this.TextBox_PWM_GRAD_AUTO.Location = new System.Drawing.Point(86, 79);
			this.TextBox_PWM_GRAD_AUTO.MaxLength = 10;
			this.TextBox_PWM_GRAD_AUTO.Name = "TextBox_PWM_GRAD_AUTO";
			this.TextBox_PWM_GRAD_AUTO.ReadOnly = true;
			this.TextBox_PWM_GRAD_AUTO.Size = new System.Drawing.Size(76, 21);
			this.TextBox_PWM_GRAD_AUTO.TabIndex = 63;
			this.TextBox_PWM_GRAD_AUTO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_PWM_GRAD_AUTO
			// 
			this.Button_PWM_GRAD_AUTO.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_GRAD_AUTO.Location = new System.Drawing.Point(2, 79);
			this.Button_PWM_GRAD_AUTO.Name = "Button_PWM_GRAD_AUTO";
			this.Button_PWM_GRAD_AUTO.Size = new System.Drawing.Size(83, 22);
			this.Button_PWM_GRAD_AUTO.TabIndex = 62;
			this.Button_PWM_GRAD_AUTO.Text = "GRAD AUTO";
			this.Button_PWM_GRAD_AUTO.Click += new System.EventHandler(this.Button_PWM_GRAD_AUTO_Click);
			// 
			// TextBox_PWM_OFS_AUTO
			// 
			this.TextBox_PWM_OFS_AUTO.Location = new System.Drawing.Point(86, 58);
			this.TextBox_PWM_OFS_AUTO.MaxLength = 10;
			this.TextBox_PWM_OFS_AUTO.Name = "TextBox_PWM_OFS_AUTO";
			this.TextBox_PWM_OFS_AUTO.ReadOnly = true;
			this.TextBox_PWM_OFS_AUTO.Size = new System.Drawing.Size(76, 21);
			this.TextBox_PWM_OFS_AUTO.TabIndex = 61;
			this.TextBox_PWM_OFS_AUTO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_PWM_OFS_AUTO
			// 
			this.Button_PWM_OFS_AUTO.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_OFS_AUTO.Location = new System.Drawing.Point(2, 58);
			this.Button_PWM_OFS_AUTO.Name = "Button_PWM_OFS_AUTO";
			this.Button_PWM_OFS_AUTO.Size = new System.Drawing.Size(83, 22);
			this.Button_PWM_OFS_AUTO.TabIndex = 60;
			this.Button_PWM_OFS_AUTO.Text = "OFS AUTO";
			this.Button_PWM_OFS_AUTO.Click += new System.EventHandler(this.Button_PWM_OFS_AUTO_Click);
			// 
			// TextBox_PWM_SCALE_AUTO
			// 
			this.TextBox_PWM_SCALE_AUTO.Location = new System.Drawing.Point(86, 37);
			this.TextBox_PWM_SCALE_AUTO.MaxLength = 10;
			this.TextBox_PWM_SCALE_AUTO.Name = "TextBox_PWM_SCALE_AUTO";
			this.TextBox_PWM_SCALE_AUTO.ReadOnly = true;
			this.TextBox_PWM_SCALE_AUTO.Size = new System.Drawing.Size(76, 21);
			this.TextBox_PWM_SCALE_AUTO.TabIndex = 59;
			this.TextBox_PWM_SCALE_AUTO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_PWM_SCALE_AUTO
			// 
			this.Button_PWM_SCALE_AUTO.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_SCALE_AUTO.Location = new System.Drawing.Point(2, 37);
			this.Button_PWM_SCALE_AUTO.Name = "Button_PWM_SCALE_AUTO";
			this.Button_PWM_SCALE_AUTO.Size = new System.Drawing.Size(83, 22);
			this.Button_PWM_SCALE_AUTO.TabIndex = 58;
			this.Button_PWM_SCALE_AUTO.Text = "SCALE AUTO";
			this.Button_PWM_SCALE_AUTO.Click += new System.EventHandler(this.Button_PWM_SCALE_AUTO_Click);
			// 
			// TextBox_PWM_SCALE_SUM
			// 
			this.TextBox_PWM_SCALE_SUM.Location = new System.Drawing.Point(86, 16);
			this.TextBox_PWM_SCALE_SUM.MaxLength = 10;
			this.TextBox_PWM_SCALE_SUM.Name = "TextBox_PWM_SCALE_SUM";
			this.TextBox_PWM_SCALE_SUM.ReadOnly = true;
			this.TextBox_PWM_SCALE_SUM.Size = new System.Drawing.Size(76, 21);
			this.TextBox_PWM_SCALE_SUM.TabIndex = 57;
			this.TextBox_PWM_SCALE_SUM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_PWM_SCALE_SUM
			// 
			this.Button_PWM_SCALE_SUM.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_SCALE_SUM.Location = new System.Drawing.Point(2, 16);
			this.Button_PWM_SCALE_SUM.Name = "Button_PWM_SCALE_SUM";
			this.Button_PWM_SCALE_SUM.Size = new System.Drawing.Size(83, 22);
			this.Button_PWM_SCALE_SUM.TabIndex = 50;
			this.Button_PWM_SCALE_SUM.Text = "SCALE SUM";
			this.Button_PWM_SCALE_SUM.Click += new System.EventHandler(this.Button_PWM_SCALE_SUM_Click);
			// 
			// GroupBox_Chopper_PWMCONF
			// 
			this.GroupBox_Chopper_PWMCONF.Controls.Add(this.GroupBox_PWMCONF);
			this.GroupBox_Chopper_PWMCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_Chopper_PWMCONF.Location = new System.Drawing.Point(544, 143);
			this.GroupBox_Chopper_PWMCONF.Name = "GroupBox_Chopper_PWMCONF";
			this.GroupBox_Chopper_PWMCONF.Size = new System.Drawing.Size(169, 206);
			this.GroupBox_Chopper_PWMCONF.TabIndex = 63;
			this.GroupBox_Chopper_PWMCONF.TabStop = false;
			this.GroupBox_Chopper_PWMCONF.Text = "Chopper - PWMCONF";
			// 
			// GroupBox_PWMCONF
			// 
			this.GroupBox_PWMCONF.Controls.Add(this.TextBox_PWM_GRAD);
			this.GroupBox_PWMCONF.Controls.Add(this.TextBox_PWM_OFS);
			this.GroupBox_PWMCONF.Controls.Add(this.ComboBox_PWM_LIM);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_PWM_LIM);
			this.GroupBox_PWMCONF.Controls.Add(this.ComboBox_PWM_REG);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_PWM_REG);
			this.GroupBox_PWMCONF.Controls.Add(this.ComboBox_freewheel);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_freewheel);
			this.GroupBox_PWMCONF.Controls.Add(this.ComboBox_pwm_autograd);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_pwm_autograd);
			this.GroupBox_PWMCONF.Controls.Add(this.ComboBox_pwm_autoscale);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_pwm_autoscale);
			this.GroupBox_PWMCONF.Controls.Add(this.ComboBox_pwm_freq);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_pwm_freq);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_PWM_GRAD);
			this.GroupBox_PWMCONF.Controls.Add(this.Button_PWM_OFS);
			this.GroupBox_PWMCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_PWMCONF.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_PWMCONF.Name = "GroupBox_PWMCONF";
			this.GroupBox_PWMCONF.Size = new System.Drawing.Size(164, 188);
			this.GroupBox_PWMCONF.TabIndex = 52;
			this.GroupBox_PWMCONF.TabStop = false;
			this.GroupBox_PWMCONF.Text = "PWMCONF";
			// 
			// TextBox_PWM_GRAD
			// 
			this.TextBox_PWM_GRAD.Location = new System.Drawing.Point(87, 36);
			this.TextBox_PWM_GRAD.MaxLength = 10;
			this.TextBox_PWM_GRAD.Name = "TextBox_PWM_GRAD";
			this.TextBox_PWM_GRAD.Size = new System.Drawing.Size(75, 21);
			this.TextBox_PWM_GRAD.TabIndex = 66;
			this.TextBox_PWM_GRAD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_PWM_GRAD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_PWM_GRAD_KeyDown);
			// 
			// TextBox_PWM_OFS
			// 
			this.TextBox_PWM_OFS.Location = new System.Drawing.Point(87, 14);
			this.TextBox_PWM_OFS.MaxLength = 10;
			this.TextBox_PWM_OFS.Name = "TextBox_PWM_OFS";
			this.TextBox_PWM_OFS.Size = new System.Drawing.Size(75, 21);
			this.TextBox_PWM_OFS.TabIndex = 59;
			this.TextBox_PWM_OFS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_PWM_OFS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_PWM_OFS_KeyDown);
			// 
			// ComboBox_PWM_LIM
			// 
			this.ComboBox_PWM_LIM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_PWM_LIM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_PWM_LIM.FormattingEnabled = true;
			this.ComboBox_PWM_LIM.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
			this.ComboBox_PWM_LIM.Location = new System.Drawing.Point(88, 161);
			this.ComboBox_PWM_LIM.Name = "ComboBox_PWM_LIM";
			this.ComboBox_PWM_LIM.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_PWM_LIM.TabIndex = 65;
			this.ComboBox_PWM_LIM.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_PWM_LIM_SelectionChangeCommitted);
			// 
			// Button_PWM_LIM
			// 
			this.Button_PWM_LIM.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_LIM.Location = new System.Drawing.Point(2, 161);
			this.Button_PWM_LIM.Name = "Button_PWM_LIM";
			this.Button_PWM_LIM.Size = new System.Drawing.Size(84, 22);
			this.Button_PWM_LIM.TabIndex = 64;
			this.Button_PWM_LIM.Text = "PWM_LIM";
			this.Button_PWM_LIM.Click += new System.EventHandler(this.Button_PWM_LIM_Click);
			// 
			// ComboBox_PWM_REG
			// 
			this.ComboBox_PWM_REG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_PWM_REG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_PWM_REG.FormattingEnabled = true;
			this.ComboBox_PWM_REG.Items.AddRange(new object[] {
            "0.5",
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "3.5",
            "4",
            "4.5",
            "5",
            "5.5",
            "6",
            "6.5",
            "7",
            "7.5"});
			this.ComboBox_PWM_REG.Location = new System.Drawing.Point(88, 140);
			this.ComboBox_PWM_REG.Name = "ComboBox_PWM_REG";
			this.ComboBox_PWM_REG.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_PWM_REG.TabIndex = 63;
			this.ComboBox_PWM_REG.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_PWM_REG_SelectionChangeCommitted);
			// 
			// Button_PWM_REG
			// 
			this.Button_PWM_REG.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_REG.Location = new System.Drawing.Point(2, 140);
			this.Button_PWM_REG.Name = "Button_PWM_REG";
			this.Button_PWM_REG.Size = new System.Drawing.Size(84, 22);
			this.Button_PWM_REG.TabIndex = 62;
			this.Button_PWM_REG.Text = "PWM_REG";
			this.Button_PWM_REG.Click += new System.EventHandler(this.Button_PWM_REG_Click);
			// 
			// ComboBox_freewheel
			// 
			this.ComboBox_freewheel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_freewheel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_freewheel.FormattingEnabled = true;
			this.ComboBox_freewheel.Items.AddRange(new object[] {
            "Normal",
            "Freewheeling",
            "LS drivers short",
            "HS drivers short"});
			this.ComboBox_freewheel.Location = new System.Drawing.Point(88, 119);
			this.ComboBox_freewheel.Name = "ComboBox_freewheel";
			this.ComboBox_freewheel.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_freewheel.TabIndex = 61;
			this.ComboBox_freewheel.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_freewheel_SelectionChangeCommitted);
			// 
			// Button_freewheel
			// 
			this.Button_freewheel.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_freewheel.Location = new System.Drawing.Point(2, 119);
			this.Button_freewheel.Name = "Button_freewheel";
			this.Button_freewheel.Size = new System.Drawing.Size(84, 22);
			this.Button_freewheel.TabIndex = 60;
			this.Button_freewheel.Text = "freewheel";
			this.Button_freewheel.Click += new System.EventHandler(this.Button_freewheel_Click);
			// 
			// ComboBox_pwm_autograd
			// 
			this.ComboBox_pwm_autograd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_pwm_autograd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_pwm_autograd.FormattingEnabled = true;
			this.ComboBox_pwm_autograd.Items.AddRange(new object[] {
            "Fixed",
            "Auto"});
			this.ComboBox_pwm_autograd.Location = new System.Drawing.Point(88, 98);
			this.ComboBox_pwm_autograd.Name = "ComboBox_pwm_autograd";
			this.ComboBox_pwm_autograd.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_pwm_autograd.TabIndex = 59;
			this.ComboBox_pwm_autograd.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_pwm_autograd_SelectionChangeCommitted);
			// 
			// Button_pwm_autograd
			// 
			this.Button_pwm_autograd.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_pwm_autograd.Location = new System.Drawing.Point(2, 98);
			this.Button_pwm_autograd.Name = "Button_pwm_autograd";
			this.Button_pwm_autograd.Size = new System.Drawing.Size(84, 22);
			this.Button_pwm_autograd.TabIndex = 58;
			this.Button_pwm_autograd.Text = "pwm_autograd";
			this.Button_pwm_autograd.Click += new System.EventHandler(this.Button_pwm_autograd_Click);
			// 
			// ComboBox_pwm_autoscale
			// 
			this.ComboBox_pwm_autoscale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_pwm_autoscale.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_pwm_autoscale.FormattingEnabled = true;
			this.ComboBox_pwm_autoscale.Items.AddRange(new object[] {
            "User defined",
            "Auto"});
			this.ComboBox_pwm_autoscale.Location = new System.Drawing.Point(88, 77);
			this.ComboBox_pwm_autoscale.Name = "ComboBox_pwm_autoscale";
			this.ComboBox_pwm_autoscale.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_pwm_autoscale.TabIndex = 57;
			this.ComboBox_pwm_autoscale.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_pwm_autoscale_SelectionChangeCommitted);
			// 
			// Button_pwm_autoscale
			// 
			this.Button_pwm_autoscale.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_pwm_autoscale.Location = new System.Drawing.Point(2, 77);
			this.Button_pwm_autoscale.Name = "Button_pwm_autoscale";
			this.Button_pwm_autoscale.Size = new System.Drawing.Size(84, 22);
			this.Button_pwm_autoscale.TabIndex = 56;
			this.Button_pwm_autoscale.Text = "pwm_autoscale";
			this.Button_pwm_autoscale.Click += new System.EventHandler(this.Button_pwm_autoscale_Click);
			// 
			// ComboBox_pwm_freq
			// 
			this.ComboBox_pwm_freq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_pwm_freq.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_pwm_freq.FormattingEnabled = true;
			this.ComboBox_pwm_freq.Items.AddRange(new object[] {
            "2/1024 fclk",
            "2/683 fclk",
            "2/512 fclk",
            "2/410 fclk"});
			this.ComboBox_pwm_freq.Location = new System.Drawing.Point(88, 56);
			this.ComboBox_pwm_freq.Name = "ComboBox_pwm_freq";
			this.ComboBox_pwm_freq.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_pwm_freq.TabIndex = 55;
			this.ComboBox_pwm_freq.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_pwm_freq_SelectionChangeCommitted);
			// 
			// Button_pwm_freq
			// 
			this.Button_pwm_freq.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_pwm_freq.Location = new System.Drawing.Point(2, 56);
			this.Button_pwm_freq.Name = "Button_pwm_freq";
			this.Button_pwm_freq.Size = new System.Drawing.Size(84, 22);
			this.Button_pwm_freq.TabIndex = 54;
			this.Button_pwm_freq.Text = "pwm_freq";
			this.Button_pwm_freq.Click += new System.EventHandler(this.Button_pwm_freq_Click);
			// 
			// Button_PWM_GRAD
			// 
			this.Button_PWM_GRAD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_GRAD.Location = new System.Drawing.Point(2, 35);
			this.Button_PWM_GRAD.Name = "Button_PWM_GRAD";
			this.Button_PWM_GRAD.Size = new System.Drawing.Size(84, 22);
			this.Button_PWM_GRAD.TabIndex = 52;
			this.Button_PWM_GRAD.Text = "PWM_GRAD";
			this.Button_PWM_GRAD.Click += new System.EventHandler(this.Button_PWM_GRAD_Click);
			// 
			// Button_PWM_OFS
			// 
			this.Button_PWM_OFS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PWM_OFS.Location = new System.Drawing.Point(2, 14);
			this.Button_PWM_OFS.Name = "Button_PWM_OFS";
			this.Button_PWM_OFS.Size = new System.Drawing.Size(84, 22);
			this.Button_PWM_OFS.TabIndex = 50;
			this.Button_PWM_OFS.Text = "PWM_OFS";
			this.Button_PWM_OFS.Click += new System.EventHandler(this.Button_PWM_OFS_Click);
			// 
			// GroupBox_Chopper_DRV_STATUS
			// 
			this.GroupBox_Chopper_DRV_STATUS.Controls.Add(this.GroupBox_DRV_STATUS);
			this.GroupBox_Chopper_DRV_STATUS.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_Chopper_DRV_STATUS.Location = new System.Drawing.Point(884, 3);
			this.GroupBox_Chopper_DRV_STATUS.Name = "GroupBox_Chopper_DRV_STATUS";
			this.GroupBox_Chopper_DRV_STATUS.Size = new System.Drawing.Size(138, 352);
			this.GroupBox_Chopper_DRV_STATUS.TabIndex = 61;
			this.GroupBox_Chopper_DRV_STATUS.TabStop = false;
			this.GroupBox_Chopper_DRV_STATUS.Text = "Chopper - DRV_STATUS";
			// 
			// GroupBox_DRV_STATUS
			// 
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_stst);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_stst);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_stealth);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_stealth);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_CS_ACTUAL);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_CS_ACTUAL);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_t157);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_t157);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_t150);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_t150);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_t143);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_t143);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_t120);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_t120);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_olb);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_olb);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_ola);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_ola);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_s2vsb);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_s2vsb);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_s2vsa);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_s2vsa);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_s2gb);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_s2gb);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_s2ga);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_s2ga);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_ot);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_ot);
			this.GroupBox_DRV_STATUS.Controls.Add(this.TextBox_otpw);
			this.GroupBox_DRV_STATUS.Controls.Add(this.Button_otpw);
			this.GroupBox_DRV_STATUS.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_DRV_STATUS.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_DRV_STATUS.Name = "GroupBox_DRV_STATUS";
			this.GroupBox_DRV_STATUS.Size = new System.Drawing.Size(132, 334);
			this.GroupBox_DRV_STATUS.TabIndex = 59;
			this.GroupBox_DRV_STATUS.TabStop = false;
			this.GroupBox_DRV_STATUS.Text = "DRV_STATUS";
			// 
			// TextBox_stst
			// 
			this.TextBox_stst.Location = new System.Drawing.Point(87, 309);
			this.TextBox_stst.MaxLength = 10;
			this.TextBox_stst.Name = "TextBox_stst";
			this.TextBox_stst.ReadOnly = true;
			this.TextBox_stst.Size = new System.Drawing.Size(42, 21);
			this.TextBox_stst.TabIndex = 81;
			this.TextBox_stst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_stst
			// 
			this.Button_stst.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_stst.Location = new System.Drawing.Point(2, 309);
			this.Button_stst.Name = "Button_stst";
			this.Button_stst.Size = new System.Drawing.Size(84, 22);
			this.Button_stst.TabIndex = 80;
			this.Button_stst.Text = "stst";
			this.Button_stst.Click += new System.EventHandler(this.Button_stst_Click);
			// 
			// TextBox_stealth
			// 
			this.TextBox_stealth.Location = new System.Drawing.Point(87, 288);
			this.TextBox_stealth.MaxLength = 10;
			this.TextBox_stealth.Name = "TextBox_stealth";
			this.TextBox_stealth.ReadOnly = true;
			this.TextBox_stealth.Size = new System.Drawing.Size(42, 21);
			this.TextBox_stealth.TabIndex = 79;
			this.TextBox_stealth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_stealth
			// 
			this.Button_stealth.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_stealth.Location = new System.Drawing.Point(2, 288);
			this.Button_stealth.Name = "Button_stealth";
			this.Button_stealth.Size = new System.Drawing.Size(84, 22);
			this.Button_stealth.TabIndex = 78;
			this.Button_stealth.Text = "stealth";
			this.Button_stealth.Click += new System.EventHandler(this.Button_stealth_Click);
			// 
			// TextBox_CS_ACTUAL
			// 
			this.TextBox_CS_ACTUAL.Location = new System.Drawing.Point(87, 267);
			this.TextBox_CS_ACTUAL.MaxLength = 10;
			this.TextBox_CS_ACTUAL.Name = "TextBox_CS_ACTUAL";
			this.TextBox_CS_ACTUAL.ReadOnly = true;
			this.TextBox_CS_ACTUAL.Size = new System.Drawing.Size(42, 21);
			this.TextBox_CS_ACTUAL.TabIndex = 77;
			this.TextBox_CS_ACTUAL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_CS_ACTUAL
			// 
			this.Button_CS_ACTUAL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_CS_ACTUAL.Location = new System.Drawing.Point(2, 267);
			this.Button_CS_ACTUAL.Name = "Button_CS_ACTUAL";
			this.Button_CS_ACTUAL.Size = new System.Drawing.Size(84, 22);
			this.Button_CS_ACTUAL.TabIndex = 76;
			this.Button_CS_ACTUAL.Text = "CS_ACTUAL";
			this.Button_CS_ACTUAL.Click += new System.EventHandler(this.Button_CS_ACTUAL_Click);
			// 
			// TextBox_t157
			// 
			this.TextBox_t157.Location = new System.Drawing.Point(87, 246);
			this.TextBox_t157.MaxLength = 10;
			this.TextBox_t157.Name = "TextBox_t157";
			this.TextBox_t157.ReadOnly = true;
			this.TextBox_t157.Size = new System.Drawing.Size(42, 21);
			this.TextBox_t157.TabIndex = 75;
			this.TextBox_t157.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_t157
			// 
			this.Button_t157.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_t157.Location = new System.Drawing.Point(2, 246);
			this.Button_t157.Name = "Button_t157";
			this.Button_t157.Size = new System.Drawing.Size(84, 22);
			this.Button_t157.TabIndex = 74;
			this.Button_t157.Text = "t157";
			this.Button_t157.Click += new System.EventHandler(this.Button_t157_Click);
			// 
			// TextBox_t150
			// 
			this.TextBox_t150.Location = new System.Drawing.Point(87, 225);
			this.TextBox_t150.MaxLength = 10;
			this.TextBox_t150.Name = "TextBox_t150";
			this.TextBox_t150.ReadOnly = true;
			this.TextBox_t150.Size = new System.Drawing.Size(42, 21);
			this.TextBox_t150.TabIndex = 73;
			this.TextBox_t150.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_t150
			// 
			this.Button_t150.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_t150.Location = new System.Drawing.Point(2, 225);
			this.Button_t150.Name = "Button_t150";
			this.Button_t150.Size = new System.Drawing.Size(84, 22);
			this.Button_t150.TabIndex = 72;
			this.Button_t150.Text = "t150";
			this.Button_t150.Click += new System.EventHandler(this.Button_t150_Click);
			// 
			// TextBox_t143
			// 
			this.TextBox_t143.Location = new System.Drawing.Point(87, 204);
			this.TextBox_t143.MaxLength = 10;
			this.TextBox_t143.Name = "TextBox_t143";
			this.TextBox_t143.ReadOnly = true;
			this.TextBox_t143.Size = new System.Drawing.Size(42, 21);
			this.TextBox_t143.TabIndex = 71;
			this.TextBox_t143.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_t143
			// 
			this.Button_t143.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_t143.Location = new System.Drawing.Point(2, 204);
			this.Button_t143.Name = "Button_t143";
			this.Button_t143.Size = new System.Drawing.Size(84, 22);
			this.Button_t143.TabIndex = 70;
			this.Button_t143.Text = "t143";
			this.Button_t143.Click += new System.EventHandler(this.Button_t143_Click);
			// 
			// TextBox_t120
			// 
			this.TextBox_t120.Location = new System.Drawing.Point(87, 183);
			this.TextBox_t120.MaxLength = 10;
			this.TextBox_t120.Name = "TextBox_t120";
			this.TextBox_t120.ReadOnly = true;
			this.TextBox_t120.Size = new System.Drawing.Size(42, 21);
			this.TextBox_t120.TabIndex = 69;
			this.TextBox_t120.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_t120
			// 
			this.Button_t120.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_t120.Location = new System.Drawing.Point(2, 183);
			this.Button_t120.Name = "Button_t120";
			this.Button_t120.Size = new System.Drawing.Size(84, 22);
			this.Button_t120.TabIndex = 68;
			this.Button_t120.Text = "t120";
			this.Button_t120.Click += new System.EventHandler(this.Button_t120_Click);
			// 
			// TextBox_olb
			// 
			this.TextBox_olb.Location = new System.Drawing.Point(87, 162);
			this.TextBox_olb.MaxLength = 10;
			this.TextBox_olb.Name = "TextBox_olb";
			this.TextBox_olb.ReadOnly = true;
			this.TextBox_olb.Size = new System.Drawing.Size(42, 21);
			this.TextBox_olb.TabIndex = 67;
			this.TextBox_olb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_olb
			// 
			this.Button_olb.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_olb.Location = new System.Drawing.Point(2, 162);
			this.Button_olb.Name = "Button_olb";
			this.Button_olb.Size = new System.Drawing.Size(84, 22);
			this.Button_olb.TabIndex = 66;
			this.Button_olb.Text = "olb";
			this.Button_olb.Click += new System.EventHandler(this.Button_olb_Click);
			// 
			// TextBox_ola
			// 
			this.TextBox_ola.Location = new System.Drawing.Point(87, 141);
			this.TextBox_ola.MaxLength = 10;
			this.TextBox_ola.Name = "TextBox_ola";
			this.TextBox_ola.ReadOnly = true;
			this.TextBox_ola.Size = new System.Drawing.Size(42, 21);
			this.TextBox_ola.TabIndex = 65;
			this.TextBox_ola.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_ola
			// 
			this.Button_ola.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_ola.Location = new System.Drawing.Point(2, 141);
			this.Button_ola.Name = "Button_ola";
			this.Button_ola.Size = new System.Drawing.Size(84, 22);
			this.Button_ola.TabIndex = 64;
			this.Button_ola.Text = "ola";
			this.Button_ola.Click += new System.EventHandler(this.Button_ola_Click);
			// 
			// TextBox_s2vsb
			// 
			this.TextBox_s2vsb.Location = new System.Drawing.Point(87, 120);
			this.TextBox_s2vsb.MaxLength = 10;
			this.TextBox_s2vsb.Name = "TextBox_s2vsb";
			this.TextBox_s2vsb.ReadOnly = true;
			this.TextBox_s2vsb.Size = new System.Drawing.Size(42, 21);
			this.TextBox_s2vsb.TabIndex = 63;
			this.TextBox_s2vsb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_s2vsb
			// 
			this.Button_s2vsb.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_s2vsb.Location = new System.Drawing.Point(2, 120);
			this.Button_s2vsb.Name = "Button_s2vsb";
			this.Button_s2vsb.Size = new System.Drawing.Size(84, 22);
			this.Button_s2vsb.TabIndex = 62;
			this.Button_s2vsb.Text = "s2vsb";
			this.Button_s2vsb.Click += new System.EventHandler(this.Button_s2vsb_Click);
			// 
			// TextBox_s2vsa
			// 
			this.TextBox_s2vsa.Location = new System.Drawing.Point(87, 99);
			this.TextBox_s2vsa.MaxLength = 10;
			this.TextBox_s2vsa.Name = "TextBox_s2vsa";
			this.TextBox_s2vsa.ReadOnly = true;
			this.TextBox_s2vsa.Size = new System.Drawing.Size(42, 21);
			this.TextBox_s2vsa.TabIndex = 61;
			this.TextBox_s2vsa.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_s2vsa
			// 
			this.Button_s2vsa.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_s2vsa.Location = new System.Drawing.Point(2, 99);
			this.Button_s2vsa.Name = "Button_s2vsa";
			this.Button_s2vsa.Size = new System.Drawing.Size(84, 22);
			this.Button_s2vsa.TabIndex = 60;
			this.Button_s2vsa.Text = "s2vsa";
			this.Button_s2vsa.Click += new System.EventHandler(this.Button_s2vsa_Click);
			// 
			// TextBox_s2gb
			// 
			this.TextBox_s2gb.Location = new System.Drawing.Point(87, 78);
			this.TextBox_s2gb.MaxLength = 10;
			this.TextBox_s2gb.Name = "TextBox_s2gb";
			this.TextBox_s2gb.ReadOnly = true;
			this.TextBox_s2gb.Size = new System.Drawing.Size(42, 21);
			this.TextBox_s2gb.TabIndex = 59;
			this.TextBox_s2gb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_s2gb
			// 
			this.Button_s2gb.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_s2gb.Location = new System.Drawing.Point(2, 78);
			this.Button_s2gb.Name = "Button_s2gb";
			this.Button_s2gb.Size = new System.Drawing.Size(84, 22);
			this.Button_s2gb.TabIndex = 58;
			this.Button_s2gb.Text = "s2gb";
			this.Button_s2gb.Click += new System.EventHandler(this.Button_s2gb_Click);
			// 
			// TextBox_s2ga
			// 
			this.TextBox_s2ga.Location = new System.Drawing.Point(87, 57);
			this.TextBox_s2ga.MaxLength = 10;
			this.TextBox_s2ga.Name = "TextBox_s2ga";
			this.TextBox_s2ga.ReadOnly = true;
			this.TextBox_s2ga.Size = new System.Drawing.Size(42, 21);
			this.TextBox_s2ga.TabIndex = 57;
			this.TextBox_s2ga.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_s2ga
			// 
			this.Button_s2ga.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_s2ga.Location = new System.Drawing.Point(2, 57);
			this.Button_s2ga.Name = "Button_s2ga";
			this.Button_s2ga.Size = new System.Drawing.Size(84, 22);
			this.Button_s2ga.TabIndex = 56;
			this.Button_s2ga.Text = "s2ga";
			this.Button_s2ga.Click += new System.EventHandler(this.Button_s2ga_Click);
			// 
			// TextBox_ot
			// 
			this.TextBox_ot.Location = new System.Drawing.Point(87, 36);
			this.TextBox_ot.MaxLength = 10;
			this.TextBox_ot.Name = "TextBox_ot";
			this.TextBox_ot.ReadOnly = true;
			this.TextBox_ot.Size = new System.Drawing.Size(42, 21);
			this.TextBox_ot.TabIndex = 55;
			this.TextBox_ot.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_ot
			// 
			this.Button_ot.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_ot.Location = new System.Drawing.Point(2, 36);
			this.Button_ot.Name = "Button_ot";
			this.Button_ot.Size = new System.Drawing.Size(84, 22);
			this.Button_ot.TabIndex = 54;
			this.Button_ot.Text = "ot";
			this.Button_ot.Click += new System.EventHandler(this.Button_ot_Click);
			// 
			// TextBox_otpw
			// 
			this.TextBox_otpw.Location = new System.Drawing.Point(87, 15);
			this.TextBox_otpw.MaxLength = 10;
			this.TextBox_otpw.Name = "TextBox_otpw";
			this.TextBox_otpw.ReadOnly = true;
			this.TextBox_otpw.Size = new System.Drawing.Size(42, 21);
			this.TextBox_otpw.TabIndex = 53;
			this.TextBox_otpw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_otpw
			// 
			this.Button_otpw.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_otpw.Location = new System.Drawing.Point(2, 15);
			this.Button_otpw.Name = "Button_otpw";
			this.Button_otpw.Size = new System.Drawing.Size(84, 22);
			this.Button_otpw.TabIndex = 50;
			this.Button_otpw.Text = "otpw";
			this.Button_otpw.Click += new System.EventHandler(this.Button_otpw_Click);
			// 
			// GroupBox_Chopper_CHOPCONF
			// 
			this.GroupBox_Chopper_CHOPCONF.Controls.Add(this.GroupBox_CHOPCONF);
			this.GroupBox_Chopper_CHOPCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_Chopper_CHOPCONF.Location = new System.Drawing.Point(714, 3);
			this.GroupBox_Chopper_CHOPCONF.Name = "GroupBox_Chopper_CHOPCONF";
			this.GroupBox_Chopper_CHOPCONF.Size = new System.Drawing.Size(169, 248);
			this.GroupBox_Chopper_CHOPCONF.TabIndex = 62;
			this.GroupBox_Chopper_CHOPCONF.TabStop = false;
			this.GroupBox_Chopper_CHOPCONF.Text = "Chopper - CHOPCONF";
			// 
			// GroupBox_CHOPCONF
			// 
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_diss2vs);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_diss2vs);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_diss2g);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_diss2g);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_dedge);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_dedge);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_intpol);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_intpol);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_MRES);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_MRES);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_vsense);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_vsense);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_TBL);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_TBL);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_HEND);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_HEND);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_HSTRT);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_HSTRT);
			this.GroupBox_CHOPCONF.Controls.Add(this.ComboBox_TOFF);
			this.GroupBox_CHOPCONF.Controls.Add(this.Button_TOFF);
			this.GroupBox_CHOPCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_CHOPCONF.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_CHOPCONF.Name = "GroupBox_CHOPCONF";
			this.GroupBox_CHOPCONF.Size = new System.Drawing.Size(164, 229);
			this.GroupBox_CHOPCONF.TabIndex = 52;
			this.GroupBox_CHOPCONF.TabStop = false;
			this.GroupBox_CHOPCONF.Text = "CHOPCONF";
			// 
			// ComboBox_diss2vs
			// 
			this.ComboBox_diss2vs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_diss2vs.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_diss2vs.FormattingEnabled = true;
			this.ComboBox_diss2vs.Items.AddRange(new object[] {
            "Enable",
            "Disable"});
			this.ComboBox_diss2vs.Location = new System.Drawing.Point(88, 203);
			this.ComboBox_diss2vs.Name = "ComboBox_diss2vs";
			this.ComboBox_diss2vs.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_diss2vs.TabIndex = 69;
			this.ComboBox_diss2vs.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_diss2vs_SelectionChangeCommitted);
			// 
			// Button_diss2vs
			// 
			this.Button_diss2vs.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_diss2vs.Location = new System.Drawing.Point(2, 203);
			this.Button_diss2vs.Name = "Button_diss2vs";
			this.Button_diss2vs.Size = new System.Drawing.Size(84, 22);
			this.Button_diss2vs.TabIndex = 68;
			this.Button_diss2vs.Text = "diss2vs";
			this.Button_diss2vs.Click += new System.EventHandler(this.Button_diss2vs_Click);
			// 
			// ComboBox_diss2g
			// 
			this.ComboBox_diss2g.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_diss2g.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_diss2g.FormattingEnabled = true;
			this.ComboBox_diss2g.Items.AddRange(new object[] {
            "Enable",
            "Disable"});
			this.ComboBox_diss2g.Location = new System.Drawing.Point(88, 182);
			this.ComboBox_diss2g.Name = "ComboBox_diss2g";
			this.ComboBox_diss2g.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_diss2g.TabIndex = 67;
			this.ComboBox_diss2g.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_diss2g_SelectionChangeCommitted);
			// 
			// Button_diss2g
			// 
			this.Button_diss2g.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_diss2g.Location = new System.Drawing.Point(2, 182);
			this.Button_diss2g.Name = "Button_diss2g";
			this.Button_diss2g.Size = new System.Drawing.Size(84, 22);
			this.Button_diss2g.TabIndex = 66;
			this.Button_diss2g.Text = "diss2g";
			this.Button_diss2g.Click += new System.EventHandler(this.Button_diss2g_Click);
			// 
			// ComboBox_dedge
			// 
			this.ComboBox_dedge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_dedge.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_dedge.FormattingEnabled = true;
			this.ComboBox_dedge.Items.AddRange(new object[] {
            "Disable",
            "Enable"});
			this.ComboBox_dedge.Location = new System.Drawing.Point(88, 161);
			this.ComboBox_dedge.Name = "ComboBox_dedge";
			this.ComboBox_dedge.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_dedge.TabIndex = 65;
			this.ComboBox_dedge.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_dedge_SelectionChangeCommitted);
			// 
			// Button_dedge
			// 
			this.Button_dedge.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_dedge.Location = new System.Drawing.Point(2, 161);
			this.Button_dedge.Name = "Button_dedge";
			this.Button_dedge.Size = new System.Drawing.Size(84, 22);
			this.Button_dedge.TabIndex = 64;
			this.Button_dedge.Text = "dedge";
			this.Button_dedge.Click += new System.EventHandler(this.Button_dedge_Click);
			// 
			// ComboBox_intpol
			// 
			this.ComboBox_intpol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_intpol.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_intpol.FormattingEnabled = true;
			this.ComboBox_intpol.Items.AddRange(new object[] {
            "No intpol",
            "256 msteps"});
			this.ComboBox_intpol.Location = new System.Drawing.Point(88, 140);
			this.ComboBox_intpol.Name = "ComboBox_intpol";
			this.ComboBox_intpol.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_intpol.TabIndex = 63;
			this.ComboBox_intpol.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_intpol_SelectionChangeCommitted);
			// 
			// Button_intpol
			// 
			this.Button_intpol.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_intpol.Location = new System.Drawing.Point(2, 140);
			this.Button_intpol.Name = "Button_intpol";
			this.Button_intpol.Size = new System.Drawing.Size(84, 22);
			this.Button_intpol.TabIndex = 62;
			this.Button_intpol.Text = "intpol";
			this.Button_intpol.Click += new System.EventHandler(this.Button_intpol_Click);
			// 
			// ComboBox_MRES
			// 
			this.ComboBox_MRES.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_MRES.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_MRES.FormattingEnabled = true;
			this.ComboBox_MRES.Items.AddRange(new object[] {
            "256",
            "128",
            "64",
            "32",
            "16",
            "8",
            "4",
            "2",
            "FULLSTEP"});
			this.ComboBox_MRES.Location = new System.Drawing.Point(88, 119);
			this.ComboBox_MRES.Name = "ComboBox_MRES";
			this.ComboBox_MRES.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_MRES.TabIndex = 61;
			this.ComboBox_MRES.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_MRES_SelectionChangeCommitted);
			// 
			// Button_MRES
			// 
			this.Button_MRES.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_MRES.Location = new System.Drawing.Point(2, 119);
			this.Button_MRES.Name = "Button_MRES";
			this.Button_MRES.Size = new System.Drawing.Size(84, 22);
			this.Button_MRES.TabIndex = 60;
			this.Button_MRES.Text = "MRES";
			this.Button_MRES.Click += new System.EventHandler(this.Button_MRES_Click);
			// 
			// ComboBox_vsense
			// 
			this.ComboBox_vsense.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_vsense.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_vsense.FormattingEnabled = true;
			this.ComboBox_vsense.Items.AddRange(new object[] {
            "Low sensitivity",
            "High sensitivity"});
			this.ComboBox_vsense.Location = new System.Drawing.Point(88, 98);
			this.ComboBox_vsense.Name = "ComboBox_vsense";
			this.ComboBox_vsense.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_vsense.TabIndex = 59;
			this.ComboBox_vsense.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_vsense_SelectionChangeCommitted);
			// 
			// Button_vsense
			// 
			this.Button_vsense.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_vsense.Location = new System.Drawing.Point(2, 98);
			this.Button_vsense.Name = "Button_vsense";
			this.Button_vsense.Size = new System.Drawing.Size(84, 22);
			this.Button_vsense.TabIndex = 58;
			this.Button_vsense.Text = "vsense";
			this.Button_vsense.Click += new System.EventHandler(this.Button_vsense_Click);
			// 
			// ComboBox_TBL
			// 
			this.ComboBox_TBL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_TBL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_TBL.FormattingEnabled = true;
			this.ComboBox_TBL.Items.AddRange(new object[] {
            "16 clocks",
            "24 clocks",
            "32 clocks",
            "40 clocks"});
			this.ComboBox_TBL.Location = new System.Drawing.Point(88, 77);
			this.ComboBox_TBL.Name = "ComboBox_TBL";
			this.ComboBox_TBL.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_TBL.TabIndex = 57;
			this.ComboBox_TBL.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_TBL_SelectionChangeCommitted);
			// 
			// Button_TBL
			// 
			this.Button_TBL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_TBL.Location = new System.Drawing.Point(2, 77);
			this.Button_TBL.Name = "Button_TBL";
			this.Button_TBL.Size = new System.Drawing.Size(84, 22);
			this.Button_TBL.TabIndex = 56;
			this.Button_TBL.Text = "TBL";
			this.Button_TBL.Click += new System.EventHandler(this.Button_TBL_Click);
			// 
			// ComboBox_HEND
			// 
			this.ComboBox_HEND.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_HEND.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_HEND.FormattingEnabled = true;
			this.ComboBox_HEND.Items.AddRange(new object[] {
            "-3",
            "-2",
            "-1",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
			this.ComboBox_HEND.Location = new System.Drawing.Point(88, 56);
			this.ComboBox_HEND.Name = "ComboBox_HEND";
			this.ComboBox_HEND.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_HEND.TabIndex = 55;
			this.ComboBox_HEND.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_HEND_SelectionChangeCommitted);
			// 
			// Button_HEND
			// 
			this.Button_HEND.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_HEND.Location = new System.Drawing.Point(2, 56);
			this.Button_HEND.Name = "Button_HEND";
			this.Button_HEND.Size = new System.Drawing.Size(84, 22);
			this.Button_HEND.TabIndex = 54;
			this.Button_HEND.Text = "HEND";
			this.Button_HEND.Click += new System.EventHandler(this.Button_HEND_Click);
			// 
			// ComboBox_HSTRT
			// 
			this.ComboBox_HSTRT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_HSTRT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_HSTRT.FormattingEnabled = true;
			this.ComboBox_HSTRT.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
			this.ComboBox_HSTRT.Location = new System.Drawing.Point(88, 35);
			this.ComboBox_HSTRT.Name = "ComboBox_HSTRT";
			this.ComboBox_HSTRT.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_HSTRT.TabIndex = 53;
			this.ComboBox_HSTRT.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_HSTRT_SelectionChangeCommitted);
			// 
			// Button_HSTRT
			// 
			this.Button_HSTRT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_HSTRT.Location = new System.Drawing.Point(2, 35);
			this.Button_HSTRT.Name = "Button_HSTRT";
			this.Button_HSTRT.Size = new System.Drawing.Size(84, 22);
			this.Button_HSTRT.TabIndex = 52;
			this.Button_HSTRT.Text = "HSTRT";
			this.Button_HSTRT.Click += new System.EventHandler(this.Button_HSTRT_Click);
			// 
			// ComboBox_TOFF
			// 
			this.ComboBox_TOFF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_TOFF.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_TOFF.FormattingEnabled = true;
			this.ComboBox_TOFF.Items.AddRange(new object[] {
            "Off",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
			this.ComboBox_TOFF.Location = new System.Drawing.Point(88, 14);
			this.ComboBox_TOFF.Name = "ComboBox_TOFF";
			this.ComboBox_TOFF.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_TOFF.TabIndex = 51;
			this.ComboBox_TOFF.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_TOFF_SelectionChangeCommitted);
			// 
			// Button_TOFF
			// 
			this.Button_TOFF.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_TOFF.Location = new System.Drawing.Point(2, 14);
			this.Button_TOFF.Name = "Button_TOFF";
			this.Button_TOFF.Size = new System.Drawing.Size(84, 22);
			this.Button_TOFF.TabIndex = 50;
			this.Button_TOFF.Text = "TOFF";
			this.Button_TOFF.Click += new System.EventHandler(this.Button_TOFF_Click);
			// 
			// GroupBox_STALLGARD_COOLCONF
			// 
			this.GroupBox_STALLGARD_COOLCONF.Controls.Add(this.GroupBox_COOLCONF);
			this.GroupBox_STALLGARD_COOLCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_STALLGARD_COOLCONF.Location = new System.Drawing.Point(544, 3);
			this.GroupBox_STALLGARD_COOLCONF.Name = "GroupBox_STALLGARD_COOLCONF";
			this.GroupBox_STALLGARD_COOLCONF.Size = new System.Drawing.Size(169, 142);
			this.GroupBox_STALLGARD_COOLCONF.TabIndex = 61;
			this.GroupBox_STALLGARD_COOLCONF.TabStop = false;
			this.GroupBox_STALLGARD_COOLCONF.Text = "StallGuard - COOLCONF";
			// 
			// GroupBox_COOLCONF
			// 
			this.GroupBox_COOLCONF.Controls.Add(this.ComboBox_seimin);
			this.GroupBox_COOLCONF.Controls.Add(this.Button_seimin);
			this.GroupBox_COOLCONF.Controls.Add(this.ComboBox_sedn);
			this.GroupBox_COOLCONF.Controls.Add(this.Button_sedn);
			this.GroupBox_COOLCONF.Controls.Add(this.ComboBox_semax);
			this.GroupBox_COOLCONF.Controls.Add(this.Button_semax);
			this.GroupBox_COOLCONF.Controls.Add(this.ComboBox_seup);
			this.GroupBox_COOLCONF.Controls.Add(this.Button_seup);
			this.GroupBox_COOLCONF.Controls.Add(this.ComboBox_semin);
			this.GroupBox_COOLCONF.Controls.Add(this.Button_semin);
			this.GroupBox_COOLCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_COOLCONF.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_COOLCONF.Name = "GroupBox_COOLCONF";
			this.GroupBox_COOLCONF.Size = new System.Drawing.Size(164, 126);
			this.GroupBox_COOLCONF.TabIndex = 52;
			this.GroupBox_COOLCONF.TabStop = false;
			this.GroupBox_COOLCONF.Text = "COOLCONF";
			// 
			// ComboBox_seimin
			// 
			this.ComboBox_seimin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_seimin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_seimin.FormattingEnabled = true;
			this.ComboBox_seimin.Items.AddRange(new object[] {
            "1/2 IRUN",
            "1/4 IRUN"});
			this.ComboBox_seimin.Location = new System.Drawing.Point(88, 98);
			this.ComboBox_seimin.Name = "ComboBox_seimin";
			this.ComboBox_seimin.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_seimin.TabIndex = 59;
			this.ComboBox_seimin.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_seimin_SelectionChangeCommitted);
			// 
			// Button_seimin
			// 
			this.Button_seimin.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_seimin.Location = new System.Drawing.Point(2, 98);
			this.Button_seimin.Name = "Button_seimin";
			this.Button_seimin.Size = new System.Drawing.Size(84, 22);
			this.Button_seimin.TabIndex = 58;
			this.Button_seimin.Text = "seimin";
			this.Button_seimin.Click += new System.EventHandler(this.Button_seimin_Click);
			// 
			// ComboBox_sedn
			// 
			this.ComboBox_sedn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_sedn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_sedn.FormattingEnabled = true;
			this.ComboBox_sedn.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
			this.ComboBox_sedn.Location = new System.Drawing.Point(88, 77);
			this.ComboBox_sedn.Name = "ComboBox_sedn";
			this.ComboBox_sedn.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_sedn.TabIndex = 57;
			this.ComboBox_sedn.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_sedn_SelectionChangeCommitted);
			// 
			// Button_sedn
			// 
			this.Button_sedn.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_sedn.Location = new System.Drawing.Point(2, 77);
			this.Button_sedn.Name = "Button_sedn";
			this.Button_sedn.Size = new System.Drawing.Size(84, 22);
			this.Button_sedn.TabIndex = 56;
			this.Button_sedn.Text = "sedn";
			this.Button_sedn.Click += new System.EventHandler(this.Button_sedn_Click);
			// 
			// ComboBox_semax
			// 
			this.ComboBox_semax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_semax.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_semax.FormattingEnabled = true;
			this.ComboBox_semax.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
			this.ComboBox_semax.Location = new System.Drawing.Point(88, 56);
			this.ComboBox_semax.Name = "ComboBox_semax";
			this.ComboBox_semax.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_semax.TabIndex = 55;
			this.ComboBox_semax.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_semax_SelectionChangeCommitted);
			// 
			// Button_semax
			// 
			this.Button_semax.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_semax.Location = new System.Drawing.Point(2, 56);
			this.Button_semax.Name = "Button_semax";
			this.Button_semax.Size = new System.Drawing.Size(84, 22);
			this.Button_semax.TabIndex = 54;
			this.Button_semax.Text = "semax";
			this.Button_semax.Click += new System.EventHandler(this.Button_semax_Click);
			// 
			// ComboBox_seup
			// 
			this.ComboBox_seup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_seup.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_seup.FormattingEnabled = true;
			this.ComboBox_seup.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8"});
			this.ComboBox_seup.Location = new System.Drawing.Point(88, 35);
			this.ComboBox_seup.Name = "ComboBox_seup";
			this.ComboBox_seup.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_seup.TabIndex = 53;
			this.ComboBox_seup.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_seup_SelectionChangeCommitted);
			// 
			// Button_seup
			// 
			this.Button_seup.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_seup.Location = new System.Drawing.Point(2, 35);
			this.Button_seup.Name = "Button_seup";
			this.Button_seup.Size = new System.Drawing.Size(84, 22);
			this.Button_seup.TabIndex = 52;
			this.Button_seup.Text = "seup";
			this.Button_seup.Click += new System.EventHandler(this.Button_seup_Click);
			// 
			// ComboBox_semin
			// 
			this.ComboBox_semin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_semin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_semin.FormattingEnabled = true;
			this.ComboBox_semin.Items.AddRange(new object[] {
            "CoolStep off",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
			this.ComboBox_semin.Location = new System.Drawing.Point(88, 14);
			this.ComboBox_semin.Name = "ComboBox_semin";
			this.ComboBox_semin.Size = new System.Drawing.Size(74, 24);
			this.ComboBox_semin.TabIndex = 51;
			this.ComboBox_semin.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_semin_SelectionChangeCommitted);
			// 
			// Button_semin
			// 
			this.Button_semin.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_semin.Location = new System.Drawing.Point(2, 14);
			this.Button_semin.Name = "Button_semin";
			this.Button_semin.Size = new System.Drawing.Size(84, 22);
			this.Button_semin.TabIndex = 50;
			this.Button_semin.Text = "semin";
			this.Button_semin.Click += new System.EventHandler(this.Button_semin_Click);
			// 
			// GroupBox_Sequencer_Registers
			// 
			this.GroupBox_Sequencer_Registers.Controls.Add(this.GroupBox_MSCURACT);
			this.GroupBox_Sequencer_Registers.Controls.Add(this.GroupBox_MSCNT);
			this.GroupBox_Sequencer_Registers.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_Sequencer_Registers.Location = new System.Drawing.Point(405, 259);
			this.GroupBox_Sequencer_Registers.Name = "GroupBox_Sequencer_Registers";
			this.GroupBox_Sequencer_Registers.Size = new System.Drawing.Size(138, 117);
			this.GroupBox_Sequencer_Registers.TabIndex = 61;
			this.GroupBox_Sequencer_Registers.TabStop = false;
			this.GroupBox_Sequencer_Registers.Text = "Sequencer Registers";
			// 
			// GroupBox_MSCURACT
			// 
			this.GroupBox_MSCURACT.Controls.Add(this.Button_CUR_B);
			this.GroupBox_MSCURACT.Controls.Add(this.TextBox_CUR_B);
			this.GroupBox_MSCURACT.Controls.Add(this.TextBox_CUR_A);
			this.GroupBox_MSCURACT.Controls.Add(this.Button_CUR_A);
			this.GroupBox_MSCURACT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_MSCURACT.Location = new System.Drawing.Point(3, 54);
			this.GroupBox_MSCURACT.Name = "GroupBox_MSCURACT";
			this.GroupBox_MSCURACT.Size = new System.Drawing.Size(132, 60);
			this.GroupBox_MSCURACT.TabIndex = 59;
			this.GroupBox_MSCURACT.TabStop = false;
			this.GroupBox_MSCURACT.Text = "MSCURACT";
			// 
			// Button_CUR_B
			// 
			this.Button_CUR_B.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_CUR_B.Location = new System.Drawing.Point(2, 35);
			this.Button_CUR_B.Name = "Button_CUR_B";
			this.Button_CUR_B.Size = new System.Drawing.Size(84, 22);
			this.Button_CUR_B.TabIndex = 59;
			this.Button_CUR_B.Text = "CUR_B";
			this.Button_CUR_B.Click += new System.EventHandler(this.Button_CUR_B_Click);
			// 
			// TextBox_CUR_B
			// 
			this.TextBox_CUR_B.Location = new System.Drawing.Point(87, 35);
			this.TextBox_CUR_B.MaxLength = 10;
			this.TextBox_CUR_B.Name = "TextBox_CUR_B";
			this.TextBox_CUR_B.ReadOnly = true;
			this.TextBox_CUR_B.Size = new System.Drawing.Size(42, 21);
			this.TextBox_CUR_B.TabIndex = 58;
			this.TextBox_CUR_B.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_CUR_A
			// 
			this.TextBox_CUR_A.Location = new System.Drawing.Point(87, 14);
			this.TextBox_CUR_A.MaxLength = 10;
			this.TextBox_CUR_A.Name = "TextBox_CUR_A";
			this.TextBox_CUR_A.ReadOnly = true;
			this.TextBox_CUR_A.Size = new System.Drawing.Size(42, 21);
			this.TextBox_CUR_A.TabIndex = 57;
			this.TextBox_CUR_A.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_CUR_A
			// 
			this.Button_CUR_A.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_CUR_A.Location = new System.Drawing.Point(2, 14);
			this.Button_CUR_A.Name = "Button_CUR_A";
			this.Button_CUR_A.Size = new System.Drawing.Size(84, 22);
			this.Button_CUR_A.TabIndex = 50;
			this.Button_CUR_A.Text = "CUR_A";
			this.Button_CUR_A.Click += new System.EventHandler(this.Button_CUR_A_Click);
			// 
			// GroupBox_MSCNT
			// 
			this.GroupBox_MSCNT.Controls.Add(this.TextBox_MSCNT);
			this.GroupBox_MSCNT.Controls.Add(this.Button_MSCNT);
			this.GroupBox_MSCNT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_MSCNT.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_MSCNT.Name = "GroupBox_MSCNT";
			this.GroupBox_MSCNT.Size = new System.Drawing.Size(132, 40);
			this.GroupBox_MSCNT.TabIndex = 56;
			this.GroupBox_MSCNT.TabStop = false;
			this.GroupBox_MSCNT.Text = "MSCNT";
			// 
			// TextBox_MSCNT
			// 
			this.TextBox_MSCNT.Location = new System.Drawing.Point(87, 14);
			this.TextBox_MSCNT.MaxLength = 10;
			this.TextBox_MSCNT.Name = "TextBox_MSCNT";
			this.TextBox_MSCNT.ReadOnly = true;
			this.TextBox_MSCNT.Size = new System.Drawing.Size(42, 21);
			this.TextBox_MSCNT.TabIndex = 57;
			this.TextBox_MSCNT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_MSCNT
			// 
			this.Button_MSCNT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_MSCNT.Location = new System.Drawing.Point(2, 14);
			this.Button_MSCNT.Name = "Button_MSCNT";
			this.Button_MSCNT.Size = new System.Drawing.Size(84, 22);
			this.Button_MSCNT.TabIndex = 50;
			this.Button_MSCNT.Text = "MSCNT";
			this.Button_MSCNT.Click += new System.EventHandler(this.Button_MSCNT_Click);
			// 
			// GroupBox_StallGuard_Control
			// 
			this.GroupBox_StallGuard_Control.Controls.Add(this.GroupBox_SG_RESULT);
			this.GroupBox_StallGuard_Control.Controls.Add(this.GroupBox_SGTHRS);
			this.GroupBox_StallGuard_Control.Controls.Add(this.GroupBox_TCOOLTHRS);
			this.GroupBox_StallGuard_Control.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_StallGuard_Control.Location = new System.Drawing.Point(4, 307);
			this.GroupBox_StallGuard_Control.Name = "GroupBox_StallGuard_Control";
			this.GroupBox_StallGuard_Control.Size = new System.Drawing.Size(400, 59);
			this.GroupBox_StallGuard_Control.TabIndex = 61;
			this.GroupBox_StallGuard_Control.TabStop = false;
			this.GroupBox_StallGuard_Control.Text = "StallGuard Control";
			// 
			// GroupBox_SG_RESULT
			// 
			this.GroupBox_SG_RESULT.Controls.Add(this.TextBox_SG_RESULT);
			this.GroupBox_SG_RESULT.Controls.Add(this.Button_SG_RESULT);
			this.GroupBox_SG_RESULT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_SG_RESULT.Location = new System.Drawing.Point(266, 15);
			this.GroupBox_SG_RESULT.Name = "GroupBox_SG_RESULT";
			this.GroupBox_SG_RESULT.Size = new System.Drawing.Size(130, 41);
			this.GroupBox_SG_RESULT.TabIndex = 60;
			this.GroupBox_SG_RESULT.TabStop = false;
			this.GroupBox_SG_RESULT.Text = "SG_RESULT";
			// 
			// TextBox_SG_RESULT
			// 
			this.TextBox_SG_RESULT.Location = new System.Drawing.Point(86, 15);
			this.TextBox_SG_RESULT.MaxLength = 10;
			this.TextBox_SG_RESULT.Name = "TextBox_SG_RESULT";
			this.TextBox_SG_RESULT.ReadOnly = true;
			this.TextBox_SG_RESULT.Size = new System.Drawing.Size(42, 21);
			this.TextBox_SG_RESULT.TabIndex = 53;
			this.TextBox_SG_RESULT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_SG_RESULT
			// 
			this.Button_SG_RESULT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_SG_RESULT.Location = new System.Drawing.Point(2, 15);
			this.Button_SG_RESULT.Name = "Button_SG_RESULT";
			this.Button_SG_RESULT.Size = new System.Drawing.Size(84, 22);
			this.Button_SG_RESULT.TabIndex = 50;
			this.Button_SG_RESULT.Text = "SG_RESULT";
			this.Button_SG_RESULT.Click += new System.EventHandler(this.Button_SG_RESULT_Click);
			// 
			// GroupBox_SGTHRS
			// 
			this.GroupBox_SGTHRS.Controls.Add(this.Button_SGTHRS);
			this.GroupBox_SGTHRS.Controls.Add(this.TextBox_SGTHRS);
			this.GroupBox_SGTHRS.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_SGTHRS.Location = new System.Drawing.Point(135, 15);
			this.GroupBox_SGTHRS.Name = "GroupBox_SGTHRS";
			this.GroupBox_SGTHRS.Size = new System.Drawing.Size(130, 41);
			this.GroupBox_SGTHRS.TabIndex = 59;
			this.GroupBox_SGTHRS.TabStop = false;
			this.GroupBox_SGTHRS.Text = "SGTHRS";
			// 
			// Button_SGTHRS
			// 
			this.Button_SGTHRS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_SGTHRS.Location = new System.Drawing.Point(2, 15);
			this.Button_SGTHRS.Name = "Button_SGTHRS";
			this.Button_SGTHRS.Size = new System.Drawing.Size(84, 22);
			this.Button_SGTHRS.TabIndex = 59;
			this.Button_SGTHRS.Text = "SGTHRS";
			this.Button_SGTHRS.Click += new System.EventHandler(this.Button_SGTHRS_Click);
			// 
			// TextBox_SGTHRS
			// 
			this.TextBox_SGTHRS.Location = new System.Drawing.Point(86, 15);
			this.TextBox_SGTHRS.MaxLength = 10;
			this.TextBox_SGTHRS.Name = "TextBox_SGTHRS";
			this.TextBox_SGTHRS.Size = new System.Drawing.Size(42, 21);
			this.TextBox_SGTHRS.TabIndex = 53;
			this.TextBox_SGTHRS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_SGTHRS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_SGTHRS_KeyDown);
			// 
			// GroupBox_TCOOLTHRS
			// 
			this.GroupBox_TCOOLTHRS.Controls.Add(this.TextBox_TCOOLTHRS);
			this.GroupBox_TCOOLTHRS.Controls.Add(this.Button_TCOOLTHRS);
			this.GroupBox_TCOOLTHRS.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_TCOOLTHRS.Location = new System.Drawing.Point(4, 15);
			this.GroupBox_TCOOLTHRS.Name = "GroupBox_TCOOLTHRS";
			this.GroupBox_TCOOLTHRS.Size = new System.Drawing.Size(130, 41);
			this.GroupBox_TCOOLTHRS.TabIndex = 56;
			this.GroupBox_TCOOLTHRS.TabStop = false;
			this.GroupBox_TCOOLTHRS.Text = "TCOOLTHRS";
			// 
			// TextBox_TCOOLTHRS
			// 
			this.TextBox_TCOOLTHRS.Location = new System.Drawing.Point(86, 15);
			this.TextBox_TCOOLTHRS.MaxLength = 10;
			this.TextBox_TCOOLTHRS.Name = "TextBox_TCOOLTHRS";
			this.TextBox_TCOOLTHRS.Size = new System.Drawing.Size(42, 21);
			this.TextBox_TCOOLTHRS.TabIndex = 57;
			this.TextBox_TCOOLTHRS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_TCOOLTHRS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_TCOOLTHRS_KeyDown);
			// 
			// Button_TCOOLTHRS
			// 
			this.Button_TCOOLTHRS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_TCOOLTHRS.Location = new System.Drawing.Point(2, 15);
			this.Button_TCOOLTHRS.Name = "Button_TCOOLTHRS";
			this.Button_TCOOLTHRS.Size = new System.Drawing.Size(84, 22);
			this.Button_TCOOLTHRS.TabIndex = 50;
			this.Button_TCOOLTHRS.Text = "TCOOLTHRS";
			this.Button_TCOOLTHRS.Click += new System.EventHandler(this.Button_TCOOLTHRS_Click);
			// 
			// GroupBox_Velocity_Dependent_Control
			// 
			this.GroupBox_Velocity_Dependent_Control.Controls.Add(this.GroupBox_VACTUAL);
			this.GroupBox_Velocity_Dependent_Control.Controls.Add(this.GroupBox_TPWMTHRS);
			this.GroupBox_Velocity_Dependent_Control.Controls.Add(this.GroupBox_TSTEP);
			this.GroupBox_Velocity_Dependent_Control.Controls.Add(this.GroupBox_TPOWERDOWN);
			this.GroupBox_Velocity_Dependent_Control.Controls.Add(this.GroupBox_IHOLD_IRUN);
			this.GroupBox_Velocity_Dependent_Control.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_Velocity_Dependent_Control.Location = new System.Drawing.Point(405, 3);
			this.GroupBox_Velocity_Dependent_Control.Name = "GroupBox_Velocity_Dependent_Control";
			this.GroupBox_Velocity_Dependent_Control.Size = new System.Drawing.Size(138, 257);
			this.GroupBox_Velocity_Dependent_Control.TabIndex = 32;
			this.GroupBox_Velocity_Dependent_Control.TabStop = false;
			this.GroupBox_Velocity_Dependent_Control.Text = "Velocity Dependent Control";
			// 
			// GroupBox_VACTUAL
			// 
			this.GroupBox_VACTUAL.Controls.Add(this.TextBox_VACTUAL);
			this.GroupBox_VACTUAL.Controls.Add(this.Button_VACTUAL);
			this.GroupBox_VACTUAL.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_VACTUAL.Location = new System.Drawing.Point(3, 213);
			this.GroupBox_VACTUAL.Name = "GroupBox_VACTUAL";
			this.GroupBox_VACTUAL.Size = new System.Drawing.Size(132, 40);
			this.GroupBox_VACTUAL.TabIndex = 60;
			this.GroupBox_VACTUAL.TabStop = false;
			this.GroupBox_VACTUAL.Text = "VACTUAL";
			// 
			// TextBox_VACTUAL
			// 
			this.TextBox_VACTUAL.Location = new System.Drawing.Point(87, 14);
			this.TextBox_VACTUAL.MaxLength = 10;
			this.TextBox_VACTUAL.Name = "TextBox_VACTUAL";
			this.TextBox_VACTUAL.Size = new System.Drawing.Size(42, 21);
			this.TextBox_VACTUAL.TabIndex = 57;
			this.TextBox_VACTUAL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_VACTUAL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_VACTUAL_KeyDown);
			// 
			// Button_VACTUAL
			// 
			this.Button_VACTUAL.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_VACTUAL.Location = new System.Drawing.Point(2, 14);
			this.Button_VACTUAL.Name = "Button_VACTUAL";
			this.Button_VACTUAL.Size = new System.Drawing.Size(84, 22);
			this.Button_VACTUAL.TabIndex = 50;
			this.Button_VACTUAL.Text = "VACTUAL";
			this.Button_VACTUAL.Click += new System.EventHandler(this.Button_VACTUAL_Click);
			// 
			// GroupBox_TPWMTHRS
			// 
			this.GroupBox_TPWMTHRS.Controls.Add(this.TextBox_TPWMTHRS);
			this.GroupBox_TPWMTHRS.Controls.Add(this.Button_TPWMTHRS);
			this.GroupBox_TPWMTHRS.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_TPWMTHRS.Location = new System.Drawing.Point(3, 174);
			this.GroupBox_TPWMTHRS.Name = "GroupBox_TPWMTHRS";
			this.GroupBox_TPWMTHRS.Size = new System.Drawing.Size(132, 40);
			this.GroupBox_TPWMTHRS.TabIndex = 59;
			this.GroupBox_TPWMTHRS.TabStop = false;
			this.GroupBox_TPWMTHRS.Text = "TPWMTHRS";
			// 
			// TextBox_TPWMTHRS
			// 
			this.TextBox_TPWMTHRS.Location = new System.Drawing.Point(87, 14);
			this.TextBox_TPWMTHRS.MaxLength = 10;
			this.TextBox_TPWMTHRS.Name = "TextBox_TPWMTHRS";
			this.TextBox_TPWMTHRS.Size = new System.Drawing.Size(42, 21);
			this.TextBox_TPWMTHRS.TabIndex = 57;
			this.TextBox_TPWMTHRS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_TPWMTHRS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_TPWMTHRS_KeyDown);
			// 
			// Button_TPWMTHRS
			// 
			this.Button_TPWMTHRS.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_TPWMTHRS.Location = new System.Drawing.Point(2, 14);
			this.Button_TPWMTHRS.Name = "Button_TPWMTHRS";
			this.Button_TPWMTHRS.Size = new System.Drawing.Size(84, 22);
			this.Button_TPWMTHRS.TabIndex = 50;
			this.Button_TPWMTHRS.Text = "TPWMTHRS";
			this.Button_TPWMTHRS.Click += new System.EventHandler(this.Button_TPWMTHRS_Click);
			// 
			// GroupBox_TSTEP
			// 
			this.GroupBox_TSTEP.Controls.Add(this.TextBox_TSTEP);
			this.GroupBox_TSTEP.Controls.Add(this.Button_TSTEP);
			this.GroupBox_TSTEP.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_TSTEP.Location = new System.Drawing.Point(3, 135);
			this.GroupBox_TSTEP.Name = "GroupBox_TSTEP";
			this.GroupBox_TSTEP.Size = new System.Drawing.Size(132, 40);
			this.GroupBox_TSTEP.TabIndex = 59;
			this.GroupBox_TSTEP.TabStop = false;
			this.GroupBox_TSTEP.Text = "TSTEP";
			// 
			// TextBox_TSTEP
			// 
			this.TextBox_TSTEP.Location = new System.Drawing.Point(87, 14);
			this.TextBox_TSTEP.MaxLength = 10;
			this.TextBox_TSTEP.Name = "TextBox_TSTEP";
			this.TextBox_TSTEP.ReadOnly = true;
			this.TextBox_TSTEP.Size = new System.Drawing.Size(42, 21);
			this.TextBox_TSTEP.TabIndex = 53;
			this.TextBox_TSTEP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_TSTEP
			// 
			this.Button_TSTEP.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_TSTEP.Location = new System.Drawing.Point(2, 14);
			this.Button_TSTEP.Name = "Button_TSTEP";
			this.Button_TSTEP.Size = new System.Drawing.Size(84, 22);
			this.Button_TSTEP.TabIndex = 50;
			this.Button_TSTEP.Text = "TSTEP";
			this.Button_TSTEP.Click += new System.EventHandler(this.Button_TSTEP_Click);
			// 
			// GroupBox_TPOWERDOWN
			// 
			this.GroupBox_TPOWERDOWN.Controls.Add(this.TextBox_TPOWERDOWN);
			this.GroupBox_TPOWERDOWN.Controls.Add(this.Button_TPOWERDOWN);
			this.GroupBox_TPOWERDOWN.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_TPOWERDOWN.Location = new System.Drawing.Point(3, 96);
			this.GroupBox_TPOWERDOWN.Name = "GroupBox_TPOWERDOWN";
			this.GroupBox_TPOWERDOWN.Size = new System.Drawing.Size(132, 40);
			this.GroupBox_TPOWERDOWN.TabIndex = 56;
			this.GroupBox_TPOWERDOWN.TabStop = false;
			this.GroupBox_TPOWERDOWN.Text = "TPOWERDOWN";
			// 
			// TextBox_TPOWERDOWN
			// 
			this.TextBox_TPOWERDOWN.Location = new System.Drawing.Point(87, 14);
			this.TextBox_TPOWERDOWN.MaxLength = 10;
			this.TextBox_TPOWERDOWN.Name = "TextBox_TPOWERDOWN";
			this.TextBox_TPOWERDOWN.Size = new System.Drawing.Size(42, 21);
			this.TextBox_TPOWERDOWN.TabIndex = 57;
			this.TextBox_TPOWERDOWN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TextBox_TPOWERDOWN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_TPOWERDOWN_KeyDown);
			// 
			// Button_TPOWERDOWN
			// 
			this.Button_TPOWERDOWN.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_TPOWERDOWN.Location = new System.Drawing.Point(2, 14);
			this.Button_TPOWERDOWN.Name = "Button_TPOWERDOWN";
			this.Button_TPOWERDOWN.Size = new System.Drawing.Size(84, 22);
			this.Button_TPOWERDOWN.TabIndex = 50;
			this.Button_TPOWERDOWN.Text = "TPOWERDOWN";
			this.Button_TPOWERDOWN.Click += new System.EventHandler(this.Button_TPOWERDOWN_Click);
			// 
			// GroupBox_IHOLD_IRUN
			// 
			this.GroupBox_IHOLD_IRUN.Controls.Add(this.ComboBox_IHOLDDELAY);
			this.GroupBox_IHOLD_IRUN.Controls.Add(this.Button_IHOLDDELAY);
			this.GroupBox_IHOLD_IRUN.Controls.Add(this.ComboBox_IRUN);
			this.GroupBox_IHOLD_IRUN.Controls.Add(this.Button_IRUN);
			this.GroupBox_IHOLD_IRUN.Controls.Add(this.ComboBox_IHOLD);
			this.GroupBox_IHOLD_IRUN.Controls.Add(this.Button_IHOLD);
			this.GroupBox_IHOLD_IRUN.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_IHOLD_IRUN.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_IHOLD_IRUN.Name = "GroupBox_IHOLD_IRUN";
			this.GroupBox_IHOLD_IRUN.Size = new System.Drawing.Size(132, 82);
			this.GroupBox_IHOLD_IRUN.TabIndex = 52;
			this.GroupBox_IHOLD_IRUN.TabStop = false;
			this.GroupBox_IHOLD_IRUN.Text = "IHOLD_IRUN";
			// 
			// ComboBox_IHOLDDELAY
			// 
			this.ComboBox_IHOLDDELAY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_IHOLDDELAY.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_IHOLDDELAY.FormattingEnabled = true;
			this.ComboBox_IHOLDDELAY.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
			this.ComboBox_IHOLDDELAY.Location = new System.Drawing.Point(88, 56);
			this.ComboBox_IHOLDDELAY.Name = "ComboBox_IHOLDDELAY";
			this.ComboBox_IHOLDDELAY.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_IHOLDDELAY.TabIndex = 55;
			this.ComboBox_IHOLDDELAY.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_IHOLDDELAY_SelectionChangeCommitted);
			// 
			// Button_IHOLDDELAY
			// 
			this.Button_IHOLDDELAY.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_IHOLDDELAY.Location = new System.Drawing.Point(2, 56);
			this.Button_IHOLDDELAY.Name = "Button_IHOLDDELAY";
			this.Button_IHOLDDELAY.Size = new System.Drawing.Size(84, 22);
			this.Button_IHOLDDELAY.TabIndex = 54;
			this.Button_IHOLDDELAY.Text = "IHOLDDELAY";
			this.Button_IHOLDDELAY.Click += new System.EventHandler(this.Button_IHOLDDELAY_Click);
			// 
			// ComboBox_IRUN
			// 
			this.ComboBox_IRUN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_IRUN.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_IRUN.FormattingEnabled = true;
			this.ComboBox_IRUN.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31"});
			this.ComboBox_IRUN.Location = new System.Drawing.Point(88, 35);
			this.ComboBox_IRUN.Name = "ComboBox_IRUN";
			this.ComboBox_IRUN.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_IRUN.TabIndex = 53;
			this.ComboBox_IRUN.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_IRUN_SelectionChangeCommitted);
			// 
			// Button_IRUN
			// 
			this.Button_IRUN.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_IRUN.Location = new System.Drawing.Point(2, 35);
			this.Button_IRUN.Name = "Button_IRUN";
			this.Button_IRUN.Size = new System.Drawing.Size(84, 22);
			this.Button_IRUN.TabIndex = 52;
			this.Button_IRUN.Text = "IRUN";
			this.Button_IRUN.Click += new System.EventHandler(this.Button_IRUN_Click);
			// 
			// ComboBox_IHOLD
			// 
			this.ComboBox_IHOLD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_IHOLD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_IHOLD.FormattingEnabled = true;
			this.ComboBox_IHOLD.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31"});
			this.ComboBox_IHOLD.Location = new System.Drawing.Point(88, 14);
			this.ComboBox_IHOLD.Name = "ComboBox_IHOLD";
			this.ComboBox_IHOLD.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_IHOLD.TabIndex = 51;
			this.ComboBox_IHOLD.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_IHOLD_SelectionChangeCommitted);
			// 
			// Button_IHOLD
			// 
			this.Button_IHOLD.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_IHOLD.Location = new System.Drawing.Point(2, 14);
			this.Button_IHOLD.Name = "Button_IHOLD";
			this.Button_IHOLD.Size = new System.Drawing.Size(84, 22);
			this.Button_IHOLD.TabIndex = 50;
			this.Button_IHOLD.Text = "IHOLD";
			this.Button_IHOLD.Click += new System.EventHandler(this.Button_IHOLD_Click);
			// 
			// GroupBox_General
			// 
			this.GroupBox_General.Controls.Add(this.GroupBox_FACTORY_CONF);
			this.GroupBox_General.Controls.Add(this.GroupBox_IOIN);
			this.GroupBox_General.Controls.Add(this.GroupBoxGSTAT);
			this.GroupBox_General.Controls.Add(this.GroupBox_OTP_READ);
			this.GroupBox_General.Controls.Add(this.GroupBox_SLAVECONF);
			this.GroupBox_General.Controls.Add(this.GroupBox_IFCNT);
			this.GroupBox_General.Controls.Add(this.GroupBox_GCONF);
			this.GroupBox_General.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_General.Location = new System.Drawing.Point(4, 3);
			this.GroupBox_General.Name = "GroupBox_General";
			this.GroupBox_General.Size = new System.Drawing.Size(400, 304);
			this.GroupBox_General.TabIndex = 31;
			this.GroupBox_General.TabStop = false;
			this.GroupBox_General.Text = "General Registers";
			// 
			// GroupBox_FACTORY_CONF
			// 
			this.GroupBox_FACTORY_CONF.Controls.Add(this.ComboBox_OTTRIM);
			this.GroupBox_FACTORY_CONF.Controls.Add(this.Button_OTTRIM);
			this.GroupBox_FACTORY_CONF.Controls.Add(this.ComboBox_FCLKTRIM);
			this.GroupBox_FACTORY_CONF.Controls.Add(this.Button_FCLKTRIM);
			this.GroupBox_FACTORY_CONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_FACTORY_CONF.Location = new System.Drawing.Point(265, 198);
			this.GroupBox_FACTORY_CONF.Name = "GroupBox_FACTORY_CONF";
			this.GroupBox_FACTORY_CONF.Size = new System.Drawing.Size(132, 62);
			this.GroupBox_FACTORY_CONF.TabIndex = 71;
			this.GroupBox_FACTORY_CONF.TabStop = false;
			this.GroupBox_FACTORY_CONF.Text = "FACTORY_CONF";
			// 
			// ComboBox_OTTRIM
			// 
			this.ComboBox_OTTRIM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_OTTRIM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_OTTRIM.FormattingEnabled = true;
			this.ComboBox_OTTRIM.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
			this.ComboBox_OTTRIM.Location = new System.Drawing.Point(88, 36);
			this.ComboBox_OTTRIM.Name = "ComboBox_OTTRIM";
			this.ComboBox_OTTRIM.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_OTTRIM.TabIndex = 53;
			this.ComboBox_OTTRIM.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_OTTRIM_SelectionChangeCommitted);
			// 
			// Button_OTTRIM
			// 
			this.Button_OTTRIM.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_OTTRIM.Location = new System.Drawing.Point(2, 36);
			this.Button_OTTRIM.Name = "Button_OTTRIM";
			this.Button_OTTRIM.Size = new System.Drawing.Size(84, 22);
			this.Button_OTTRIM.TabIndex = 52;
			this.Button_OTTRIM.Text = "OTTRIM";
			this.Button_OTTRIM.Click += new System.EventHandler(this.Button_OTTRIM_Click);
			// 
			// ComboBox_FCLKTRIM
			// 
			this.ComboBox_FCLKTRIM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_FCLKTRIM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_FCLKTRIM.FormattingEnabled = true;
			this.ComboBox_FCLKTRIM.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31"});
			this.ComboBox_FCLKTRIM.Location = new System.Drawing.Point(88, 15);
			this.ComboBox_FCLKTRIM.Name = "ComboBox_FCLKTRIM";
			this.ComboBox_FCLKTRIM.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_FCLKTRIM.TabIndex = 51;
			this.ComboBox_FCLKTRIM.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_FCLKTRIM_SelectionChangeCommitted);
			// 
			// Button_FCLKTRIM
			// 
			this.Button_FCLKTRIM.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_FCLKTRIM.Location = new System.Drawing.Point(2, 15);
			this.Button_FCLKTRIM.Name = "Button_FCLKTRIM";
			this.Button_FCLKTRIM.Size = new System.Drawing.Size(84, 22);
			this.Button_FCLKTRIM.TabIndex = 50;
			this.Button_FCLKTRIM.Text = "FCLKTRIM";
			this.Button_FCLKTRIM.Click += new System.EventHandler(this.Button_FCLKTRIM_Click);
			// 
			// GroupBox_IOIN
			// 
			this.GroupBox_IOIN.Controls.Add(this.TextBox_VERSION);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_DIR);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_SPREAD_EN);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_STEP);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_PDN_UART);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_DIAG);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_MS2);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_MS1);
			this.GroupBox_IOIN.Controls.Add(this.TextBox_ENN);
			this.GroupBox_IOIN.Controls.Add(this.Button_VERSION);
			this.GroupBox_IOIN.Controls.Add(this.Button_DIR);
			this.GroupBox_IOIN.Controls.Add(this.Button_SPREAD_EN);
			this.GroupBox_IOIN.Controls.Add(this.Button_STEP);
			this.GroupBox_IOIN.Controls.Add(this.Button_PDN_UART);
			this.GroupBox_IOIN.Controls.Add(this.Button_DIAG);
			this.GroupBox_IOIN.Controls.Add(this.Button_MS2);
			this.GroupBox_IOIN.Controls.Add(this.Button_MS1);
			this.GroupBox_IOIN.Controls.Add(this.Button_ENN);
			this.GroupBox_IOIN.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_IOIN.Location = new System.Drawing.Point(3, 178);
			this.GroupBox_IOIN.Name = "GroupBox_IOIN";
			this.GroupBox_IOIN.Size = new System.Drawing.Size(261, 124);
			this.GroupBox_IOIN.TabIndex = 70;
			this.GroupBox_IOIN.TabStop = false;
			this.GroupBox_IOIN.Text = "IOIN";
			// 
			// TextBox_VERSION
			// 
			this.TextBox_VERSION.Location = new System.Drawing.Point(214, 77);
			this.TextBox_VERSION.MaxLength = 10;
			this.TextBox_VERSION.Name = "TextBox_VERSION";
			this.TextBox_VERSION.ReadOnly = true;
			this.TextBox_VERSION.Size = new System.Drawing.Size(42, 21);
			this.TextBox_VERSION.TabIndex = 75;
			this.TextBox_VERSION.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_DIR
			// 
			this.TextBox_DIR.Location = new System.Drawing.Point(214, 56);
			this.TextBox_DIR.MaxLength = 10;
			this.TextBox_DIR.Name = "TextBox_DIR";
			this.TextBox_DIR.ReadOnly = true;
			this.TextBox_DIR.Size = new System.Drawing.Size(42, 21);
			this.TextBox_DIR.TabIndex = 74;
			this.TextBox_DIR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_SPREAD_EN
			// 
			this.TextBox_SPREAD_EN.Location = new System.Drawing.Point(214, 35);
			this.TextBox_SPREAD_EN.MaxLength = 10;
			this.TextBox_SPREAD_EN.Name = "TextBox_SPREAD_EN";
			this.TextBox_SPREAD_EN.ReadOnly = true;
			this.TextBox_SPREAD_EN.Size = new System.Drawing.Size(42, 21);
			this.TextBox_SPREAD_EN.TabIndex = 71;
			this.TextBox_SPREAD_EN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_STEP
			// 
			this.TextBox_STEP.Location = new System.Drawing.Point(214, 14);
			this.TextBox_STEP.MaxLength = 10;
			this.TextBox_STEP.Name = "TextBox_STEP";
			this.TextBox_STEP.ReadOnly = true;
			this.TextBox_STEP.Size = new System.Drawing.Size(42, 21);
			this.TextBox_STEP.TabIndex = 73;
			this.TextBox_STEP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_PDN_UART
			// 
			this.TextBox_PDN_UART.Location = new System.Drawing.Point(86, 98);
			this.TextBox_PDN_UART.MaxLength = 10;
			this.TextBox_PDN_UART.Name = "TextBox_PDN_UART";
			this.TextBox_PDN_UART.ReadOnly = true;
			this.TextBox_PDN_UART.Size = new System.Drawing.Size(42, 21);
			this.TextBox_PDN_UART.TabIndex = 72;
			this.TextBox_PDN_UART.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_DIAG
			// 
			this.TextBox_DIAG.Location = new System.Drawing.Point(86, 77);
			this.TextBox_DIAG.MaxLength = 10;
			this.TextBox_DIAG.Name = "TextBox_DIAG";
			this.TextBox_DIAG.ReadOnly = true;
			this.TextBox_DIAG.Size = new System.Drawing.Size(42, 21);
			this.TextBox_DIAG.TabIndex = 71;
			this.TextBox_DIAG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_MS2
			// 
			this.TextBox_MS2.Location = new System.Drawing.Point(86, 56);
			this.TextBox_MS2.MaxLength = 10;
			this.TextBox_MS2.Name = "TextBox_MS2";
			this.TextBox_MS2.ReadOnly = true;
			this.TextBox_MS2.Size = new System.Drawing.Size(42, 21);
			this.TextBox_MS2.TabIndex = 71;
			this.TextBox_MS2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_MS1
			// 
			this.TextBox_MS1.Location = new System.Drawing.Point(86, 35);
			this.TextBox_MS1.MaxLength = 10;
			this.TextBox_MS1.Name = "TextBox_MS1";
			this.TextBox_MS1.ReadOnly = true;
			this.TextBox_MS1.Size = new System.Drawing.Size(42, 21);
			this.TextBox_MS1.TabIndex = 69;
			this.TextBox_MS1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_ENN
			// 
			this.TextBox_ENN.Location = new System.Drawing.Point(86, 14);
			this.TextBox_ENN.MaxLength = 10;
			this.TextBox_ENN.Name = "TextBox_ENN";
			this.TextBox_ENN.ReadOnly = true;
			this.TextBox_ENN.Size = new System.Drawing.Size(42, 21);
			this.TextBox_ENN.TabIndex = 53;
			this.TextBox_ENN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_VERSION
			// 
			this.Button_VERSION.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_VERSION.Location = new System.Drawing.Point(130, 77);
			this.Button_VERSION.Name = "Button_VERSION";
			this.Button_VERSION.Size = new System.Drawing.Size(84, 22);
			this.Button_VERSION.TabIndex = 66;
			this.Button_VERSION.Text = "VERSION";
			this.Button_VERSION.Click += new System.EventHandler(this.Button_VERSION_Click);
			// 
			// Button_DIR
			// 
			this.Button_DIR.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_DIR.Location = new System.Drawing.Point(130, 56);
			this.Button_DIR.Name = "Button_DIR";
			this.Button_DIR.Size = new System.Drawing.Size(84, 22);
			this.Button_DIR.TabIndex = 64;
			this.Button_DIR.Text = "DIR";
			this.Button_DIR.Click += new System.EventHandler(this.Button_DIR_Click);
			// 
			// Button_SPREAD_EN
			// 
			this.Button_SPREAD_EN.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_SPREAD_EN.Location = new System.Drawing.Point(130, 35);
			this.Button_SPREAD_EN.Name = "Button_SPREAD_EN";
			this.Button_SPREAD_EN.Size = new System.Drawing.Size(84, 22);
			this.Button_SPREAD_EN.TabIndex = 62;
			this.Button_SPREAD_EN.Text = "SPREAD_EN";
			this.Button_SPREAD_EN.Click += new System.EventHandler(this.Button_SPREAD_EN_Click);
			// 
			// Button_STEP
			// 
			this.Button_STEP.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_STEP.Location = new System.Drawing.Point(130, 14);
			this.Button_STEP.Name = "Button_STEP";
			this.Button_STEP.Size = new System.Drawing.Size(84, 22);
			this.Button_STEP.TabIndex = 60;
			this.Button_STEP.Text = "STEP";
			this.Button_STEP.Click += new System.EventHandler(this.Button_STEP_Click);
			// 
			// Button_PDN_UART
			// 
			this.Button_PDN_UART.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_PDN_UART.Location = new System.Drawing.Point(2, 98);
			this.Button_PDN_UART.Name = "Button_PDN_UART";
			this.Button_PDN_UART.Size = new System.Drawing.Size(84, 22);
			this.Button_PDN_UART.TabIndex = 58;
			this.Button_PDN_UART.Text = "PDN_UART";
			this.Button_PDN_UART.Click += new System.EventHandler(this.Button_PDN_UART_Click);
			// 
			// Button_DIAG
			// 
			this.Button_DIAG.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_DIAG.Location = new System.Drawing.Point(2, 77);
			this.Button_DIAG.Name = "Button_DIAG";
			this.Button_DIAG.Size = new System.Drawing.Size(84, 22);
			this.Button_DIAG.TabIndex = 56;
			this.Button_DIAG.Text = "DIAG";
			this.Button_DIAG.Click += new System.EventHandler(this.Button_DIAG_Click);
			// 
			// Button_MS2
			// 
			this.Button_MS2.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_MS2.Location = new System.Drawing.Point(2, 56);
			this.Button_MS2.Name = "Button_MS2";
			this.Button_MS2.Size = new System.Drawing.Size(84, 22);
			this.Button_MS2.TabIndex = 54;
			this.Button_MS2.Text = "MS2";
			this.Button_MS2.Click += new System.EventHandler(this.Button_MS2_Click);
			// 
			// Button_MS1
			// 
			this.Button_MS1.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_MS1.Location = new System.Drawing.Point(2, 35);
			this.Button_MS1.Name = "Button_MS1";
			this.Button_MS1.Size = new System.Drawing.Size(84, 22);
			this.Button_MS1.TabIndex = 52;
			this.Button_MS1.Text = "MS1";
			this.Button_MS1.Click += new System.EventHandler(this.Button_MS1_Click);
			// 
			// Button_ENN
			// 
			this.Button_ENN.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_ENN.Location = new System.Drawing.Point(2, 14);
			this.Button_ENN.Name = "Button_ENN";
			this.Button_ENN.Size = new System.Drawing.Size(84, 22);
			this.Button_ENN.TabIndex = 50;
			this.Button_ENN.Text = "ENN";
			this.Button_ENN.Click += new System.EventHandler(this.Button_ENN_Click);
			// 
			// GroupBoxGSTAT
			// 
			this.GroupBoxGSTAT.Controls.Add(this.ComboBox_uv_cp);
			this.GroupBoxGSTAT.Controls.Add(this.Button_uv_cp);
			this.GroupBoxGSTAT.Controls.Add(this.ComboBox_drv_err);
			this.GroupBoxGSTAT.Controls.Add(this.Button_drv_err);
			this.GroupBoxGSTAT.Controls.Add(this.ComboBox_reset);
			this.GroupBoxGSTAT.Controls.Add(this.Button_reset);
			this.GroupBoxGSTAT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBoxGSTAT.Location = new System.Drawing.Point(265, 15);
			this.GroupBoxGSTAT.Name = "GroupBoxGSTAT";
			this.GroupBoxGSTAT.Size = new System.Drawing.Size(132, 82);
			this.GroupBoxGSTAT.TabIndex = 70;
			this.GroupBoxGSTAT.TabStop = false;
			this.GroupBoxGSTAT.Text = "GSTAT";
			// 
			// ComboBox_uv_cp
			// 
			this.ComboBox_uv_cp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_uv_cp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_uv_cp.FormattingEnabled = true;
			this.ComboBox_uv_cp.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_uv_cp.Location = new System.Drawing.Point(88, 56);
			this.ComboBox_uv_cp.Name = "ComboBox_uv_cp";
			this.ComboBox_uv_cp.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_uv_cp.TabIndex = 61;
			this.ComboBox_uv_cp.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_uv_cp_SelectionChangeCommitted);
			// 
			// Button_uv_cp
			// 
			this.Button_uv_cp.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_uv_cp.Location = new System.Drawing.Point(2, 56);
			this.Button_uv_cp.Name = "Button_uv_cp";
			this.Button_uv_cp.Size = new System.Drawing.Size(84, 22);
			this.Button_uv_cp.TabIndex = 60;
			this.Button_uv_cp.Text = "uv_cp";
			this.Button_uv_cp.Click += new System.EventHandler(this.Button_uv_cp_Click);
			// 
			// ComboBox_drv_err
			// 
			this.ComboBox_drv_err.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_drv_err.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_drv_err.FormattingEnabled = true;
			this.ComboBox_drv_err.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_drv_err.Location = new System.Drawing.Point(88, 35);
			this.ComboBox_drv_err.Name = "ComboBox_drv_err";
			this.ComboBox_drv_err.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_drv_err.TabIndex = 53;
			this.ComboBox_drv_err.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_drv_err_SelectionChangeCommitted);
			// 
			// Button_drv_err
			// 
			this.Button_drv_err.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_drv_err.Location = new System.Drawing.Point(2, 35);
			this.Button_drv_err.Name = "Button_drv_err";
			this.Button_drv_err.Size = new System.Drawing.Size(84, 22);
			this.Button_drv_err.TabIndex = 52;
			this.Button_drv_err.Text = "drv_err";
			this.Button_drv_err.Click += new System.EventHandler(this.Button_drv_err_Click);
			// 
			// ComboBox_reset
			// 
			this.ComboBox_reset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_reset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_reset.FormattingEnabled = true;
			this.ComboBox_reset.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_reset.Location = new System.Drawing.Point(88, 14);
			this.ComboBox_reset.Name = "ComboBox_reset";
			this.ComboBox_reset.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_reset.TabIndex = 51;
			this.ComboBox_reset.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_reset_SelectionChangeCommitted);
			// 
			// Button_reset
			// 
			this.Button_reset.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_reset.Location = new System.Drawing.Point(2, 14);
			this.Button_reset.Name = "Button_reset";
			this.Button_reset.Size = new System.Drawing.Size(84, 22);
			this.Button_reset.TabIndex = 50;
			this.Button_reset.Text = "reset";
			this.Button_reset.Click += new System.EventHandler(this.Button_reset_Click);
			// 
			// GroupBox_OTP_READ
			// 
			this.GroupBox_OTP_READ.Controls.Add(this.TextBox_OTP_READ_0);
			this.GroupBox_OTP_READ.Controls.Add(this.TextBox_OTP_READ_1);
			this.GroupBox_OTP_READ.Controls.Add(this.TextBox_OTP_READ_2);
			this.GroupBox_OTP_READ.Controls.Add(this.Button_OTP_READ);
			this.GroupBox_OTP_READ.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_OTP_READ.Location = new System.Drawing.Point(3, 138);
			this.GroupBox_OTP_READ.Name = "GroupBox_OTP_READ";
			this.GroupBox_OTP_READ.Size = new System.Drawing.Size(261, 41);
			this.GroupBox_OTP_READ.TabIndex = 72;
			this.GroupBox_OTP_READ.TabStop = false;
			this.GroupBox_OTP_READ.Text = "OTP_READ";
			// 
			// TextBox_OTP_READ_0
			// 
			this.TextBox_OTP_READ_0.Location = new System.Drawing.Point(203, 15);
			this.TextBox_OTP_READ_0.MaxLength = 10;
			this.TextBox_OTP_READ_0.Name = "TextBox_OTP_READ_0";
			this.TextBox_OTP_READ_0.ReadOnly = true;
			this.TextBox_OTP_READ_0.Size = new System.Drawing.Size(54, 21);
			this.TextBox_OTP_READ_0.TabIndex = 53;
			this.TextBox_OTP_READ_0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_OTP_READ_1
			// 
			this.TextBox_OTP_READ_1.Location = new System.Drawing.Point(146, 15);
			this.TextBox_OTP_READ_1.MaxLength = 10;
			this.TextBox_OTP_READ_1.Name = "TextBox_OTP_READ_1";
			this.TextBox_OTP_READ_1.ReadOnly = true;
			this.TextBox_OTP_READ_1.Size = new System.Drawing.Size(54, 21);
			this.TextBox_OTP_READ_1.TabIndex = 53;
			this.TextBox_OTP_READ_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TextBox_OTP_READ_2
			// 
			this.TextBox_OTP_READ_2.Location = new System.Drawing.Point(89, 15);
			this.TextBox_OTP_READ_2.MaxLength = 10;
			this.TextBox_OTP_READ_2.Name = "TextBox_OTP_READ_2";
			this.TextBox_OTP_READ_2.ReadOnly = true;
			this.TextBox_OTP_READ_2.Size = new System.Drawing.Size(54, 21);
			this.TextBox_OTP_READ_2.TabIndex = 52;
			this.TextBox_OTP_READ_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_OTP_READ
			// 
			this.Button_OTP_READ.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_OTP_READ.Location = new System.Drawing.Point(2, 15);
			this.Button_OTP_READ.Name = "Button_OTP_READ";
			this.Button_OTP_READ.Size = new System.Drawing.Size(84, 22);
			this.Button_OTP_READ.TabIndex = 50;
			this.Button_OTP_READ.Text = "OTP_READ";
			this.Button_OTP_READ.Click += new System.EventHandler(this.Button_OTP_READ_Click);
			// 
			// GroupBox_SLAVECONF
			// 
			this.GroupBox_SLAVECONF.Controls.Add(this.Button_SLAVECONF);
			this.GroupBox_SLAVECONF.Controls.Add(this.ComboBox_SLAVECONF);
			this.GroupBox_SLAVECONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_SLAVECONF.Location = new System.Drawing.Point(265, 135);
			this.GroupBox_SLAVECONF.Name = "GroupBox_SLAVECONF";
			this.GroupBox_SLAVECONF.Size = new System.Drawing.Size(132, 64);
			this.GroupBox_SLAVECONF.TabIndex = 71;
			this.GroupBox_SLAVECONF.TabStop = false;
			this.GroupBox_SLAVECONF.Text = "SLAVECONF";
			// 
			// Button_SLAVECONF
			// 
			this.Button_SLAVECONF.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_SLAVECONF.Location = new System.Drawing.Point(2, 14);
			this.Button_SLAVECONF.Name = "Button_SLAVECONF";
			this.Button_SLAVECONF.Size = new System.Drawing.Size(84, 22);
			this.Button_SLAVECONF.TabIndex = 53;
			this.Button_SLAVECONF.Text = "SLAVECONF";
			this.Button_SLAVECONF.Click += new System.EventHandler(this.Button_SLAVECONF_Click);
			// 
			// ComboBox_SLAVECONF
			// 
			this.ComboBox_SLAVECONF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_SLAVECONF.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_SLAVECONF.FormattingEnabled = true;
			this.ComboBox_SLAVECONF.Items.AddRange(new object[] {
            "8 bit times",
            "3*8 bit times",
            "5*8 bit times",
            "7*8 bit times",
            "9*8 bit times",
            "11*8 bit times",
            "13*8 bit times",
            "15*8 bit times"});
			this.ComboBox_SLAVECONF.Location = new System.Drawing.Point(5, 37);
			this.ComboBox_SLAVECONF.Name = "ComboBox_SLAVECONF";
			this.ComboBox_SLAVECONF.Size = new System.Drawing.Size(124, 24);
			this.ComboBox_SLAVECONF.TabIndex = 51;
			this.ComboBox_SLAVECONF.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_SLAVECONF_SelectionChangeCommitted);
			// 
			// GroupBox_IFCNT
			// 
			this.GroupBox_IFCNT.Controls.Add(this.TextBox_IFCNT);
			this.GroupBox_IFCNT.Controls.Add(this.Button_IFCNT);
			this.GroupBox_IFCNT.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_IFCNT.Location = new System.Drawing.Point(265, 96);
			this.GroupBox_IFCNT.Name = "GroupBox_IFCNT";
			this.GroupBox_IFCNT.Size = new System.Drawing.Size(132, 40);
			this.GroupBox_IFCNT.TabIndex = 71;
			this.GroupBox_IFCNT.TabStop = false;
			this.GroupBox_IFCNT.Text = "IFCNT";
			// 
			// TextBox_IFCNT
			// 
			this.TextBox_IFCNT.Location = new System.Drawing.Point(87, 14);
			this.TextBox_IFCNT.MaxLength = 10;
			this.TextBox_IFCNT.Name = "TextBox_IFCNT";
			this.TextBox_IFCNT.ReadOnly = true;
			this.TextBox_IFCNT.Size = new System.Drawing.Size(42, 21);
			this.TextBox_IFCNT.TabIndex = 52;
			this.TextBox_IFCNT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Button_IFCNT
			// 
			this.Button_IFCNT.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_IFCNT.Location = new System.Drawing.Point(2, 14);
			this.Button_IFCNT.Name = "Button_IFCNT";
			this.Button_IFCNT.Size = new System.Drawing.Size(84, 22);
			this.Button_IFCNT.TabIndex = 50;
			this.Button_IFCNT.Text = "IFCNT";
			this.Button_IFCNT.Click += new System.EventHandler(this.Button_IFCNT_Click);
			// 
			// GroupBox_GCONF
			// 
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_test_mode);
			this.GroupBox_GCONF.Controls.Add(this.Button_test_mode);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_multistep_filt);
			this.GroupBox_GCONF.Controls.Add(this.Button_multistep_filt);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_mstep_reg_sel);
			this.GroupBox_GCONF.Controls.Add(this.Button_mstep_reg_sel);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_pdn_disable);
			this.GroupBox_GCONF.Controls.Add(this.Button_pdn_disable);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_index_step);
			this.GroupBox_GCONF.Controls.Add(this.Button_index_step);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_index_otpw);
			this.GroupBox_GCONF.Controls.Add(this.Button_index_otpw);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_shaft);
			this.GroupBox_GCONF.Controls.Add(this.Button_shaft);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_en_SpreadCycle);
			this.GroupBox_GCONF.Controls.Add(this.Button_en_SpreadCycle);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_internal_Rsense);
			this.GroupBox_GCONF.Controls.Add(this.Button_internal_Rsense);
			this.GroupBox_GCONF.Controls.Add(this.ComboBox_I_scale_analog);
			this.GroupBox_GCONF.Controls.Add(this.Button_I_scale_analog);
			this.GroupBox_GCONF.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GroupBox_GCONF.Location = new System.Drawing.Point(3, 15);
			this.GroupBox_GCONF.Name = "GroupBox_GCONF";
			this.GroupBox_GCONF.Size = new System.Drawing.Size(261, 124);
			this.GroupBox_GCONF.TabIndex = 52;
			this.GroupBox_GCONF.TabStop = false;
			this.GroupBox_GCONF.Text = "GCONF";
			// 
			// ComboBox_test_mode
			// 
			this.ComboBox_test_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_test_mode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_test_mode.FormattingEnabled = true;
			this.ComboBox_test_mode.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_test_mode.Location = new System.Drawing.Point(216, 98);
			this.ComboBox_test_mode.Name = "ComboBox_test_mode";
			this.ComboBox_test_mode.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_test_mode.TabIndex = 69;
			this.ComboBox_test_mode.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_test_mode_SelectionChangeCommitted);
			// 
			// Button_test_mode
			// 
			this.Button_test_mode.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_test_mode.Location = new System.Drawing.Point(130, 98);
			this.Button_test_mode.Name = "Button_test_mode";
			this.Button_test_mode.Size = new System.Drawing.Size(84, 22);
			this.Button_test_mode.TabIndex = 68;
			this.Button_test_mode.Text = "test_mode";
			this.Button_test_mode.Click += new System.EventHandler(this.Button_test_mode_Click);
			// 
			// ComboBox_multistep_filt
			// 
			this.ComboBox_multistep_filt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_multistep_filt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_multistep_filt.FormattingEnabled = true;
			this.ComboBox_multistep_filt.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_multistep_filt.Location = new System.Drawing.Point(216, 77);
			this.ComboBox_multistep_filt.Name = "ComboBox_multistep_filt";
			this.ComboBox_multistep_filt.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_multistep_filt.TabIndex = 67;
			this.ComboBox_multistep_filt.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_multistep_filt_SelectionChangeCommitted);
			// 
			// Button_multistep_filt
			// 
			this.Button_multistep_filt.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_multistep_filt.Location = new System.Drawing.Point(130, 77);
			this.Button_multistep_filt.Name = "Button_multistep_filt";
			this.Button_multistep_filt.Size = new System.Drawing.Size(84, 22);
			this.Button_multistep_filt.TabIndex = 66;
			this.Button_multistep_filt.Text = "multistep_filt";
			this.Button_multistep_filt.Click += new System.EventHandler(this.Button_multistep_filt_Click);
			// 
			// ComboBox_mstep_reg_sel
			// 
			this.ComboBox_mstep_reg_sel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_mstep_reg_sel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_mstep_reg_sel.FormattingEnabled = true;
			this.ComboBox_mstep_reg_sel.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_mstep_reg_sel.Location = new System.Drawing.Point(216, 56);
			this.ComboBox_mstep_reg_sel.Name = "ComboBox_mstep_reg_sel";
			this.ComboBox_mstep_reg_sel.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_mstep_reg_sel.TabIndex = 65;
			this.ComboBox_mstep_reg_sel.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_mstep_reg_sel_SelectionChangeCommitted);
			// 
			// Button_mstep_reg_sel
			// 
			this.Button_mstep_reg_sel.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_mstep_reg_sel.Location = new System.Drawing.Point(130, 56);
			this.Button_mstep_reg_sel.Name = "Button_mstep_reg_sel";
			this.Button_mstep_reg_sel.Size = new System.Drawing.Size(84, 22);
			this.Button_mstep_reg_sel.TabIndex = 64;
			this.Button_mstep_reg_sel.Text = "mstep_reg_sel";
			this.Button_mstep_reg_sel.Click += new System.EventHandler(this.Button_mstep_reg_sel_Click);
			// 
			// ComboBox_pdn_disable
			// 
			this.ComboBox_pdn_disable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_pdn_disable.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_pdn_disable.FormattingEnabled = true;
			this.ComboBox_pdn_disable.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_pdn_disable.Location = new System.Drawing.Point(216, 35);
			this.ComboBox_pdn_disable.Name = "ComboBox_pdn_disable";
			this.ComboBox_pdn_disable.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_pdn_disable.TabIndex = 63;
			this.ComboBox_pdn_disable.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_pdn_disable_SelectionChangeCommitted);
			// 
			// Button_pdn_disable
			// 
			this.Button_pdn_disable.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_pdn_disable.Location = new System.Drawing.Point(130, 35);
			this.Button_pdn_disable.Name = "Button_pdn_disable";
			this.Button_pdn_disable.Size = new System.Drawing.Size(84, 22);
			this.Button_pdn_disable.TabIndex = 62;
			this.Button_pdn_disable.Text = "pdn_disable";
			this.Button_pdn_disable.Click += new System.EventHandler(this.Button_pdn_disable_Click);
			// 
			// ComboBox_index_step
			// 
			this.ComboBox_index_step.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_index_step.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_index_step.FormattingEnabled = true;
			this.ComboBox_index_step.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_index_step.Location = new System.Drawing.Point(216, 14);
			this.ComboBox_index_step.Name = "ComboBox_index_step";
			this.ComboBox_index_step.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_index_step.TabIndex = 61;
			this.ComboBox_index_step.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_index_step_SelectionChangeCommitted);
			// 
			// Button_index_step
			// 
			this.Button_index_step.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_index_step.Location = new System.Drawing.Point(130, 14);
			this.Button_index_step.Name = "Button_index_step";
			this.Button_index_step.Size = new System.Drawing.Size(84, 22);
			this.Button_index_step.TabIndex = 60;
			this.Button_index_step.Text = "index_step";
			this.Button_index_step.Click += new System.EventHandler(this.Button_index_step_Click);
			// 
			// ComboBox_index_otpw
			// 
			this.ComboBox_index_otpw.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_index_otpw.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_index_otpw.FormattingEnabled = true;
			this.ComboBox_index_otpw.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_index_otpw.Location = new System.Drawing.Point(88, 98);
			this.ComboBox_index_otpw.Name = "ComboBox_index_otpw";
			this.ComboBox_index_otpw.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_index_otpw.TabIndex = 59;
			this.ComboBox_index_otpw.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_index_otpw_SelectionChangeCommitted);
			// 
			// Button_index_otpw
			// 
			this.Button_index_otpw.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_index_otpw.Location = new System.Drawing.Point(2, 98);
			this.Button_index_otpw.Name = "Button_index_otpw";
			this.Button_index_otpw.Size = new System.Drawing.Size(84, 22);
			this.Button_index_otpw.TabIndex = 58;
			this.Button_index_otpw.Text = "index_otpw";
			this.Button_index_otpw.Click += new System.EventHandler(this.Button_index_otpw_Click);
			// 
			// ComboBox_shaft
			// 
			this.ComboBox_shaft.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_shaft.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_shaft.FormattingEnabled = true;
			this.ComboBox_shaft.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_shaft.Location = new System.Drawing.Point(88, 77);
			this.ComboBox_shaft.Name = "ComboBox_shaft";
			this.ComboBox_shaft.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_shaft.TabIndex = 57;
			this.ComboBox_shaft.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_shaft_SelectionChangeCommitted);
			// 
			// Button_shaft
			// 
			this.Button_shaft.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_shaft.Location = new System.Drawing.Point(2, 77);
			this.Button_shaft.Name = "Button_shaft";
			this.Button_shaft.Size = new System.Drawing.Size(84, 22);
			this.Button_shaft.TabIndex = 56;
			this.Button_shaft.Text = "shaft";
			this.Button_shaft.Click += new System.EventHandler(this.Button_shaft_Click);
			// 
			// ComboBox_en_SpreadCycle
			// 
			this.ComboBox_en_SpreadCycle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_en_SpreadCycle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_en_SpreadCycle.FormattingEnabled = true;
			this.ComboBox_en_SpreadCycle.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_en_SpreadCycle.Location = new System.Drawing.Point(88, 56);
			this.ComboBox_en_SpreadCycle.Name = "ComboBox_en_SpreadCycle";
			this.ComboBox_en_SpreadCycle.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_en_SpreadCycle.TabIndex = 55;
			this.ComboBox_en_SpreadCycle.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_en_SpreadCycle_SelectionChangeCommitted);
			// 
			// Button_en_SpreadCycle
			// 
			this.Button_en_SpreadCycle.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_en_SpreadCycle.Location = new System.Drawing.Point(2, 56);
			this.Button_en_SpreadCycle.Name = "Button_en_SpreadCycle";
			this.Button_en_SpreadCycle.Size = new System.Drawing.Size(84, 22);
			this.Button_en_SpreadCycle.TabIndex = 54;
			this.Button_en_SpreadCycle.Text = "en_SpreadCycle";
			this.Button_en_SpreadCycle.Click += new System.EventHandler(this.Button_en_SpreadCycle_Click);
			// 
			// ComboBox_internal_Rsense
			// 
			this.ComboBox_internal_Rsense.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_internal_Rsense.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_internal_Rsense.FormattingEnabled = true;
			this.ComboBox_internal_Rsense.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_internal_Rsense.Location = new System.Drawing.Point(88, 35);
			this.ComboBox_internal_Rsense.Name = "ComboBox_internal_Rsense";
			this.ComboBox_internal_Rsense.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_internal_Rsense.TabIndex = 53;
			this.ComboBox_internal_Rsense.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_internal_Rsense_SelectionChangeCommitted);
			// 
			// Button_internal_Rsense
			// 
			this.Button_internal_Rsense.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_internal_Rsense.Location = new System.Drawing.Point(2, 35);
			this.Button_internal_Rsense.Name = "Button_internal_Rsense";
			this.Button_internal_Rsense.Size = new System.Drawing.Size(84, 22);
			this.Button_internal_Rsense.TabIndex = 52;
			this.Button_internal_Rsense.Text = "internal_Rsense";
			this.Button_internal_Rsense.Click += new System.EventHandler(this.Button_internal_Rsense_Click);
			// 
			// ComboBox_I_scale_analog
			// 
			this.ComboBox_I_scale_analog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_I_scale_analog.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBox_I_scale_analog.FormattingEnabled = true;
			this.ComboBox_I_scale_analog.Items.AddRange(new object[] {
            "0",
            "1"});
			this.ComboBox_I_scale_analog.Location = new System.Drawing.Point(88, 14);
			this.ComboBox_I_scale_analog.Name = "ComboBox_I_scale_analog";
			this.ComboBox_I_scale_analog.Size = new System.Drawing.Size(42, 24);
			this.ComboBox_I_scale_analog.TabIndex = 51;
			this.ComboBox_I_scale_analog.SelectionChangeCommitted += new System.EventHandler(this.ComboBox_I_scale_analog_SelectionChangeCommitted);
			// 
			// Button_I_scale_analog
			// 
			this.Button_I_scale_analog.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Button_I_scale_analog.Location = new System.Drawing.Point(2, 14);
			this.Button_I_scale_analog.Name = "Button_I_scale_analog";
			this.Button_I_scale_analog.Size = new System.Drawing.Size(84, 22);
			this.Button_I_scale_analog.TabIndex = 50;
			this.Button_I_scale_analog.Text = "I_scale_analog";
			this.Button_I_scale_analog.Click += new System.EventHandler(this.Button_I_scale_analog_Click);
			// 
			// tabTest
			// 
			this.tabTest.BackColor = System.Drawing.SystemColors.Control;
			this.tabTest.Controls.Add(this.ButtonICRDefog);
			this.tabTest.Controls.Add(this.ButtonICRNight);
			this.tabTest.Controls.Add(this.ButtonICRNormal);
			this.tabTest.Controls.Add(this.ButtonICRDay);
			this.tabTest.Controls.Add(this.LabelICR);
			this.tabTest.Controls.Add(this.TextBoxFocusStep);
			this.tabTest.Controls.Add(this.ButtonFocusStepNear);
			this.tabTest.Controls.Add(this.ButtonFocusStepFar);
			this.tabTest.Controls.Add(this.LabelFocusStep);
			this.tabTest.Controls.Add(this.TextBoxZoomStep);
			this.tabTest.Controls.Add(this.ButtonZoomStepTele);
			this.tabTest.Controls.Add(this.ButtonZoomStepWide);
			this.tabTest.Controls.Add(this.LabelZoomStep);
			this.tabTest.Controls.Add(this.ButtonGetFPos);
			this.tabTest.Controls.Add(this.ButtonSetFPos);
			this.tabTest.Controls.Add(this.TextBoxFPos);
			this.tabTest.Controls.Add(this.ButtonGetZ2Pos);
			this.tabTest.Controls.Add(this.ButtonSetZ2Pos);
			this.tabTest.Controls.Add(this.TextBoxZ2Pos);
			this.tabTest.Controls.Add(this.ButtonGetZ1Pos);
			this.tabTest.Controls.Add(this.ButtonFocusResetSensor);
			this.tabTest.Controls.Add(this.ButtonFocusResetWide);
			this.tabTest.Controls.Add(this.ButtonFocusResetTele);
			this.tabTest.Controls.Add(this.LabelFocusReset);
			this.tabTest.Controls.Add(this.ButtonZoomCtrlModeInd);
			this.tabTest.Controls.Add(this.ButtonZoomCtrlModeTrack);
			this.tabTest.Controls.Add(this.LabelZoomCtrlMode);
			this.tabTest.Controls.Add(this.ButtonZoomCtrlGroup2);
			this.tabTest.Controls.Add(this.ButtonZoomCtrlGroup1);
			this.tabTest.Controls.Add(this.LabelZoomCtrlGroup);
			this.tabTest.Controls.Add(this.ButtonZoom1ResetSensor);
			this.tabTest.Controls.Add(this.ButtonZoom1ResetWide);
			this.tabTest.Controls.Add(this.ButtonSetZ1Pos);
			this.tabTest.Controls.Add(this.TextBoxZ1Pos);
			this.tabTest.Controls.Add(this.ButtonFBufNear);
			this.tabTest.Controls.Add(this.ButtonZ2BufTele);
			this.tabTest.Controls.Add(this.ButtonZ1BufTele);
			this.tabTest.Controls.Add(this.ButtonFBufFar);
			this.tabTest.Controls.Add(this.ButtonZ2BufWide);
			this.tabTest.Controls.Add(this.ButtonZ1BufWide);
			this.tabTest.Controls.Add(this.LabelFBuf);
			this.tabTest.Controls.Add(this.LabelZ2Buf);
			this.tabTest.Controls.Add(this.LabelZoom1Buf);
			this.tabTest.Controls.Add(this.ButtonZoom1ResetTele);
			this.tabTest.Controls.Add(this.ButtonFocusNear);
			this.tabTest.Controls.Add(this.ButtonZoomTele);
			this.tabTest.Controls.Add(this.ButtonZoomTeleWOTrack);
			this.tabTest.Controls.Add(this.ButtonFocusFar);
			this.tabTest.Controls.Add(this.ButtonZoomWide);
			this.tabTest.Controls.Add(this.ButtonZoomWideWOTrack);
			this.tabTest.Controls.Add(this.ButtonFocusStop);
			this.tabTest.Controls.Add(this.ButtonZoomStop);
			this.tabTest.Controls.Add(this.LabelZoom1Reset);
			this.tabTest.Controls.Add(this.LabelFocus);
			this.tabTest.Controls.Add(this.LabelZoom);
			this.tabTest.Controls.Add(this.ButtonSpeedDryStop);
			this.tabTest.Controls.Add(this.ButtonSpeedDryStart);
			this.tabTest.Controls.Add(this.VISCABox);
			this.tabTest.Location = new System.Drawing.Point(4, 22);
			this.tabTest.Name = "tabTest";
			this.tabTest.Padding = new System.Windows.Forms.Padding(3);
			this.tabTest.Size = new System.Drawing.Size(1025, 382);
			this.tabTest.TabIndex = 4;
			this.tabTest.Text = "Test";
			// 
			// ButtonICRDefog
			// 
			this.ButtonICRDefog.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonICRDefog.Location = new System.Drawing.Point(635, 325);
			this.ButtonICRDefog.Name = "ButtonICRDefog";
			this.ButtonICRDefog.Size = new System.Drawing.Size(57, 22);
			this.ButtonICRDefog.TabIndex = 125;
			this.ButtonICRDefog.Text = "Defog";
			this.ButtonICRDefog.Click += new System.EventHandler(this.ButtonICRDefog_Click);
			// 
			// ButtonICRNight
			// 
			this.ButtonICRNight.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonICRNight.Location = new System.Drawing.Point(572, 325);
			this.ButtonICRNight.Name = "ButtonICRNight";
			this.ButtonICRNight.Size = new System.Drawing.Size(57, 22);
			this.ButtonICRNight.TabIndex = 124;
			this.ButtonICRNight.Text = "Night";
			this.ButtonICRNight.Click += new System.EventHandler(this.ButtonICRNight_Click);
			// 
			// ButtonICRNormal
			// 
			this.ButtonICRNormal.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonICRNormal.Location = new System.Drawing.Point(698, 325);
			this.ButtonICRNormal.Name = "ButtonICRNormal";
			this.ButtonICRNormal.Size = new System.Drawing.Size(57, 22);
			this.ButtonICRNormal.TabIndex = 123;
			this.ButtonICRNormal.Text = "Normal";
			this.ButtonICRNormal.Click += new System.EventHandler(this.ButtonICRNormal_Click);
			// 
			// ButtonICRDay
			// 
			this.ButtonICRDay.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonICRDay.Location = new System.Drawing.Point(509, 325);
			this.ButtonICRDay.Name = "ButtonICRDay";
			this.ButtonICRDay.Size = new System.Drawing.Size(57, 22);
			this.ButtonICRDay.TabIndex = 122;
			this.ButtonICRDay.Text = "Day";
			this.ButtonICRDay.Click += new System.EventHandler(this.ButtonICRDay_Click);
			// 
			// LabelICR
			// 
			this.LabelICR.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelICR.Location = new System.Drawing.Point(456, 323);
			this.LabelICR.Name = "LabelICR";
			this.LabelICR.Size = new System.Drawing.Size(47, 25);
			this.LabelICR.TabIndex = 121;
			this.LabelICR.Text = "ICR";
			this.LabelICR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxFocusStep
			// 
			this.TextBoxFocusStep.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.TextBoxFocusStep.Location = new System.Drawing.Point(635, 202);
			this.TextBoxFocusStep.Name = "TextBoxFocusStep";
			this.TextBoxFocusStep.Size = new System.Drawing.Size(57, 20);
			this.TextBoxFocusStep.TabIndex = 120;
			this.TextBoxFocusStep.Text = "0";
			this.TextBoxFocusStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonFocusStepNear
			// 
			this.ButtonFocusStepNear.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusStepNear.Location = new System.Drawing.Point(572, 201);
			this.ButtonFocusStepNear.Name = "ButtonFocusStepNear";
			this.ButtonFocusStepNear.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusStepNear.TabIndex = 119;
			this.ButtonFocusStepNear.Text = "Near";
			this.ButtonFocusStepNear.Click += new System.EventHandler(this.ButtonFocusStepNear_Click);
			// 
			// ButtonFocusStepFar
			// 
			this.ButtonFocusStepFar.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusStepFar.Location = new System.Drawing.Point(509, 201);
			this.ButtonFocusStepFar.Name = "ButtonFocusStepFar";
			this.ButtonFocusStepFar.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusStepFar.TabIndex = 118;
			this.ButtonFocusStepFar.Text = "Far";
			this.ButtonFocusStepFar.Click += new System.EventHandler(this.ButtonFocusStepFar_Click);
			// 
			// LabelFocusStep
			// 
			this.LabelFocusStep.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelFocusStep.Location = new System.Drawing.Point(436, 199);
			this.LabelFocusStep.Name = "LabelFocusStep";
			this.LabelFocusStep.Size = new System.Drawing.Size(67, 25);
			this.LabelFocusStep.TabIndex = 117;
			this.LabelFocusStep.Text = "Focus Step";
			this.LabelFocusStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxZoomStep
			// 
			this.TextBoxZoomStep.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.TextBoxZoomStep.Location = new System.Drawing.Point(635, 121);
			this.TextBoxZoomStep.Name = "TextBoxZoomStep";
			this.TextBoxZoomStep.Size = new System.Drawing.Size(57, 20);
			this.TextBoxZoomStep.TabIndex = 116;
			this.TextBoxZoomStep.Text = "0";
			this.TextBoxZoomStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonZoomStepTele
			// 
			this.ButtonZoomStepTele.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomStepTele.Location = new System.Drawing.Point(572, 120);
			this.ButtonZoomStepTele.Name = "ButtonZoomStepTele";
			this.ButtonZoomStepTele.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomStepTele.TabIndex = 115;
			this.ButtonZoomStepTele.Text = "Tele";
			this.ButtonZoomStepTele.Click += new System.EventHandler(this.ButtonZoomStepTele_Click);
			// 
			// ButtonZoomStepWide
			// 
			this.ButtonZoomStepWide.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomStepWide.Location = new System.Drawing.Point(509, 120);
			this.ButtonZoomStepWide.Name = "ButtonZoomStepWide";
			this.ButtonZoomStepWide.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomStepWide.TabIndex = 114;
			this.ButtonZoomStepWide.Text = "Wide";
			this.ButtonZoomStepWide.Click += new System.EventHandler(this.ButtonZoomStepWide_Click);
			// 
			// LabelZoomStep
			// 
			this.LabelZoomStep.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZoomStep.Location = new System.Drawing.Point(436, 118);
			this.LabelZoomStep.Name = "LabelZoomStep";
			this.LabelZoomStep.Size = new System.Drawing.Size(67, 25);
			this.LabelZoomStep.TabIndex = 113;
			this.LabelZoomStep.Text = "Zoom Step";
			this.LabelZoomStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonGetFPos
			// 
			this.ButtonGetFPos.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetFPos.Location = new System.Drawing.Point(489, 289);
			this.ButtonGetFPos.Name = "ButtonGetFPos";
			this.ButtonGetFPos.Size = new System.Drawing.Size(44, 22);
			this.ButtonGetFPos.TabIndex = 112;
			this.ButtonGetFPos.Text = "Get F";
			this.ButtonGetFPos.Click += new System.EventHandler(this.ButtonGetFPos_Click);
			// 
			// ButtonSetFPos
			// 
			this.ButtonSetFPos.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetFPos.Location = new System.Drawing.Point(439, 289);
			this.ButtonSetFPos.Name = "ButtonSetFPos";
			this.ButtonSetFPos.Size = new System.Drawing.Size(44, 22);
			this.ButtonSetFPos.TabIndex = 111;
			this.ButtonSetFPos.Text = "Set F";
			this.ButtonSetFPos.Click += new System.EventHandler(this.ButtonSetFPos_Click);
			// 
			// TextBoxFPos
			// 
			this.TextBoxFPos.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.TextBoxFPos.Location = new System.Drawing.Point(539, 290);
			this.TextBoxFPos.Name = "TextBoxFPos";
			this.TextBoxFPos.Size = new System.Drawing.Size(57, 20);
			this.TextBoxFPos.TabIndex = 110;
			this.TextBoxFPos.Text = "0";
			this.TextBoxFPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonGetZ2Pos
			// 
			this.ButtonGetZ2Pos.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetZ2Pos.Location = new System.Drawing.Point(489, 264);
			this.ButtonGetZ2Pos.Name = "ButtonGetZ2Pos";
			this.ButtonGetZ2Pos.Size = new System.Drawing.Size(44, 22);
			this.ButtonGetZ2Pos.TabIndex = 109;
			this.ButtonGetZ2Pos.Text = "Get Z2";
			this.ButtonGetZ2Pos.Click += new System.EventHandler(this.ButtonGetZ2Pos_Click);
			// 
			// ButtonSetZ2Pos
			// 
			this.ButtonSetZ2Pos.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetZ2Pos.Location = new System.Drawing.Point(439, 264);
			this.ButtonSetZ2Pos.Name = "ButtonSetZ2Pos";
			this.ButtonSetZ2Pos.Size = new System.Drawing.Size(44, 22);
			this.ButtonSetZ2Pos.TabIndex = 108;
			this.ButtonSetZ2Pos.Text = "Set Z2";
			this.ButtonSetZ2Pos.Click += new System.EventHandler(this.ButtonSetZ2Pos_Click);
			// 
			// TextBoxZ2Pos
			// 
			this.TextBoxZ2Pos.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.TextBoxZ2Pos.Location = new System.Drawing.Point(539, 265);
			this.TextBoxZ2Pos.Name = "TextBoxZ2Pos";
			this.TextBoxZ2Pos.Size = new System.Drawing.Size(57, 20);
			this.TextBoxZ2Pos.TabIndex = 107;
			this.TextBoxZ2Pos.Text = "0";
			this.TextBoxZ2Pos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonGetZ1Pos
			// 
			this.ButtonGetZ1Pos.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonGetZ1Pos.Location = new System.Drawing.Point(489, 240);
			this.ButtonGetZ1Pos.Name = "ButtonGetZ1Pos";
			this.ButtonGetZ1Pos.Size = new System.Drawing.Size(44, 22);
			this.ButtonGetZ1Pos.TabIndex = 106;
			this.ButtonGetZ1Pos.Text = "Get Z1";
			this.ButtonGetZ1Pos.Click += new System.EventHandler(this.ButtonGetZ1Pos_Click);
			// 
			// ButtonFocusResetSensor
			// 
			this.ButtonFocusResetSensor.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusResetSensor.Location = new System.Drawing.Point(572, 177);
			this.ButtonFocusResetSensor.Name = "ButtonFocusResetSensor";
			this.ButtonFocusResetSensor.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusResetSensor.TabIndex = 105;
			this.ButtonFocusResetSensor.Text = "Sensor";
			this.ButtonFocusResetSensor.Click += new System.EventHandler(this.ButtonFocusResetSensor_Click);
			// 
			// ButtonFocusResetWide
			// 
			this.ButtonFocusResetWide.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusResetWide.Location = new System.Drawing.Point(509, 177);
			this.ButtonFocusResetWide.Name = "ButtonFocusResetWide";
			this.ButtonFocusResetWide.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusResetWide.TabIndex = 104;
			this.ButtonFocusResetWide.Text = "Wide";
			this.ButtonFocusResetWide.Click += new System.EventHandler(this.ButtonFocusResetWide_Click);
			// 
			// ButtonFocusResetTele
			// 
			this.ButtonFocusResetTele.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusResetTele.Location = new System.Drawing.Point(635, 177);
			this.ButtonFocusResetTele.Name = "ButtonFocusResetTele";
			this.ButtonFocusResetTele.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusResetTele.TabIndex = 103;
			this.ButtonFocusResetTele.Text = "Tele";
			this.ButtonFocusResetTele.Click += new System.EventHandler(this.ButtonFocusResetTele_Click);
			// 
			// LabelFocusReset
			// 
			this.LabelFocusReset.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelFocusReset.Location = new System.Drawing.Point(452, 174);
			this.LabelFocusReset.Name = "LabelFocusReset";
			this.LabelFocusReset.Size = new System.Drawing.Size(51, 25);
			this.LabelFocusReset.TabIndex = 102;
			this.LabelFocusReset.Text = "F Reset";
			this.LabelFocusReset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonZoomCtrlModeInd
			// 
			this.ButtonZoomCtrlModeInd.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomCtrlModeInd.Location = new System.Drawing.Point(572, 42);
			this.ButtonZoomCtrlModeInd.Name = "ButtonZoomCtrlModeInd";
			this.ButtonZoomCtrlModeInd.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomCtrlModeInd.TabIndex = 101;
			this.ButtonZoomCtrlModeInd.Text = "Independ";
			this.ButtonZoomCtrlModeInd.Click += new System.EventHandler(this.ButtonZoomCtrlModeInd_Click);
			// 
			// ButtonZoomCtrlModeTrack
			// 
			this.ButtonZoomCtrlModeTrack.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomCtrlModeTrack.Location = new System.Drawing.Point(509, 42);
			this.ButtonZoomCtrlModeTrack.Name = "ButtonZoomCtrlModeTrack";
			this.ButtonZoomCtrlModeTrack.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomCtrlModeTrack.TabIndex = 100;
			this.ButtonZoomCtrlModeTrack.Text = "Track";
			this.ButtonZoomCtrlModeTrack.Click += new System.EventHandler(this.ButtonZoomCtrlModeTrack_Click);
			// 
			// LabelZoomCtrlMode
			// 
			this.LabelZoomCtrlMode.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZoomCtrlMode.Location = new System.Drawing.Point(436, 41);
			this.LabelZoomCtrlMode.Name = "LabelZoomCtrlMode";
			this.LabelZoomCtrlMode.Size = new System.Drawing.Size(67, 25);
			this.LabelZoomCtrlMode.TabIndex = 99;
			this.LabelZoomCtrlMode.Text = "Zoom Mode";
			this.LabelZoomCtrlMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonZoomCtrlGroup2
			// 
			this.ButtonZoomCtrlGroup2.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomCtrlGroup2.Location = new System.Drawing.Point(572, 18);
			this.ButtonZoomCtrlGroup2.Name = "ButtonZoomCtrlGroup2";
			this.ButtonZoomCtrlGroup2.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomCtrlGroup2.TabIndex = 98;
			this.ButtonZoomCtrlGroup2.Text = "Zoom 2";
			this.ButtonZoomCtrlGroup2.Click += new System.EventHandler(this.ButtonZoomCtrlGroup2_Click);
			// 
			// ButtonZoomCtrlGroup1
			// 
			this.ButtonZoomCtrlGroup1.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomCtrlGroup1.Location = new System.Drawing.Point(509, 18);
			this.ButtonZoomCtrlGroup1.Name = "ButtonZoomCtrlGroup1";
			this.ButtonZoomCtrlGroup1.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomCtrlGroup1.TabIndex = 97;
			this.ButtonZoomCtrlGroup1.Text = "Zoom 1";
			this.ButtonZoomCtrlGroup1.Click += new System.EventHandler(this.ButtonZoomCtrlGroup1_Click);
			// 
			// LabelZoomCtrlGroup
			// 
			this.LabelZoomCtrlGroup.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZoomCtrlGroup.Location = new System.Drawing.Point(436, 16);
			this.LabelZoomCtrlGroup.Name = "LabelZoomCtrlGroup";
			this.LabelZoomCtrlGroup.Size = new System.Drawing.Size(67, 25);
			this.LabelZoomCtrlGroup.TabIndex = 96;
			this.LabelZoomCtrlGroup.Text = "Zoom Group";
			this.LabelZoomCtrlGroup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonZoom1ResetSensor
			// 
			this.ButtonZoom1ResetSensor.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoom1ResetSensor.Location = new System.Drawing.Point(572, 96);
			this.ButtonZoom1ResetSensor.Name = "ButtonZoom1ResetSensor";
			this.ButtonZoom1ResetSensor.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoom1ResetSensor.TabIndex = 95;
			this.ButtonZoom1ResetSensor.Text = "Sensor";
			this.ButtonZoom1ResetSensor.Click += new System.EventHandler(this.ButtonZoom1ResetSensor_Click);
			// 
			// ButtonZoom1ResetWide
			// 
			this.ButtonZoom1ResetWide.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoom1ResetWide.Location = new System.Drawing.Point(509, 96);
			this.ButtonZoom1ResetWide.Name = "ButtonZoom1ResetWide";
			this.ButtonZoom1ResetWide.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoom1ResetWide.TabIndex = 94;
			this.ButtonZoom1ResetWide.Text = "Wide";
			this.ButtonZoom1ResetWide.Click += new System.EventHandler(this.ButtonZoom1ResetWide_Click);
			// 
			// ButtonSetZ1Pos
			// 
			this.ButtonSetZ1Pos.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSetZ1Pos.Location = new System.Drawing.Point(439, 240);
			this.ButtonSetZ1Pos.Name = "ButtonSetZ1Pos";
			this.ButtonSetZ1Pos.Size = new System.Drawing.Size(44, 22);
			this.ButtonSetZ1Pos.TabIndex = 93;
			this.ButtonSetZ1Pos.Text = "Set Z1";
			this.ButtonSetZ1Pos.Click += new System.EventHandler(this.ButtonSetZ1Pos_Click);
			// 
			// TextBoxZ1Pos
			// 
			this.TextBoxZ1Pos.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.TextBoxZ1Pos.Location = new System.Drawing.Point(539, 241);
			this.TextBoxZ1Pos.Name = "TextBoxZ1Pos";
			this.TextBoxZ1Pos.Size = new System.Drawing.Size(57, 20);
			this.TextBoxZ1Pos.TabIndex = 9;
			this.TextBoxZ1Pos.Text = "0";
			this.TextBoxZ1Pos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// ButtonFBufNear
			// 
			this.ButtonFBufNear.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFBufNear.Location = new System.Drawing.Point(734, 288);
			this.ButtonFBufNear.Name = "ButtonFBufNear";
			this.ButtonFBufNear.Size = new System.Drawing.Size(57, 22);
			this.ButtonFBufNear.TabIndex = 89;
			this.ButtonFBufNear.Text = "Near";
			this.ButtonFBufNear.Click += new System.EventHandler(this.ButtonFBufNear_Click);
			// 
			// ButtonZ2BufTele
			// 
			this.ButtonZ2BufTele.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZ2BufTele.Location = new System.Drawing.Point(734, 263);
			this.ButtonZ2BufTele.Name = "ButtonZ2BufTele";
			this.ButtonZ2BufTele.Size = new System.Drawing.Size(57, 22);
			this.ButtonZ2BufTele.TabIndex = 88;
			this.ButtonZ2BufTele.Text = "Tele";
			this.ButtonZ2BufTele.Click += new System.EventHandler(this.ButtonZ2BufTele_Click);
			// 
			// ButtonZ1BufTele
			// 
			this.ButtonZ1BufTele.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZ1BufTele.Location = new System.Drawing.Point(734, 238);
			this.ButtonZ1BufTele.Name = "ButtonZ1BufTele";
			this.ButtonZ1BufTele.Size = new System.Drawing.Size(57, 22);
			this.ButtonZ1BufTele.TabIndex = 87;
			this.ButtonZ1BufTele.Text = "Tele";
			this.ButtonZ1BufTele.Click += new System.EventHandler(this.ButtonZ1BufTele_Click);
			// 
			// ButtonFBufFar
			// 
			this.ButtonFBufFar.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFBufFar.Location = new System.Drawing.Point(671, 288);
			this.ButtonFBufFar.Name = "ButtonFBufFar";
			this.ButtonFBufFar.Size = new System.Drawing.Size(57, 22);
			this.ButtonFBufFar.TabIndex = 86;
			this.ButtonFBufFar.Text = "Far";
			this.ButtonFBufFar.Click += new System.EventHandler(this.ButtonFBufFar_Click);
			// 
			// ButtonZ2BufWide
			// 
			this.ButtonZ2BufWide.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZ2BufWide.Location = new System.Drawing.Point(671, 263);
			this.ButtonZ2BufWide.Name = "ButtonZ2BufWide";
			this.ButtonZ2BufWide.Size = new System.Drawing.Size(57, 22);
			this.ButtonZ2BufWide.TabIndex = 85;
			this.ButtonZ2BufWide.Text = "Wide";
			this.ButtonZ2BufWide.Click += new System.EventHandler(this.ButtonZ2BufWide_Click);
			// 
			// ButtonZ1BufWide
			// 
			this.ButtonZ1BufWide.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZ1BufWide.Location = new System.Drawing.Point(671, 238);
			this.ButtonZ1BufWide.Name = "ButtonZ1BufWide";
			this.ButtonZ1BufWide.Size = new System.Drawing.Size(57, 22);
			this.ButtonZ1BufWide.TabIndex = 84;
			this.ButtonZ1BufWide.Text = "Wide";
			this.ButtonZ1BufWide.Click += new System.EventHandler(this.ButtonZ1BufWide_Click);
			// 
			// LabelFBuf
			// 
			this.LabelFBuf.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelFBuf.Location = new System.Drawing.Point(618, 285);
			this.LabelFBuf.Name = "LabelFBuf";
			this.LabelFBuf.Size = new System.Drawing.Size(47, 25);
			this.LabelFBuf.TabIndex = 83;
			this.LabelFBuf.Text = "F Buf";
			this.LabelFBuf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelZ2Buf
			// 
			this.LabelZ2Buf.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZ2Buf.Location = new System.Drawing.Point(618, 260);
			this.LabelZ2Buf.Name = "LabelZ2Buf";
			this.LabelZ2Buf.Size = new System.Drawing.Size(47, 25);
			this.LabelZ2Buf.TabIndex = 82;
			this.LabelZ2Buf.Text = "Z2 Buf";
			this.LabelZ2Buf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelZoom1Buf
			// 
			this.LabelZoom1Buf.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZoom1Buf.Location = new System.Drawing.Point(618, 236);
			this.LabelZoom1Buf.Name = "LabelZoom1Buf";
			this.LabelZoom1Buf.Size = new System.Drawing.Size(47, 25);
			this.LabelZoom1Buf.TabIndex = 81;
			this.LabelZoom1Buf.Text = "Z1 Buf";
			this.LabelZoom1Buf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonZoom1ResetTele
			// 
			this.ButtonZoom1ResetTele.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoom1ResetTele.Location = new System.Drawing.Point(635, 96);
			this.ButtonZoom1ResetTele.Name = "ButtonZoom1ResetTele";
			this.ButtonZoom1ResetTele.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoom1ResetTele.TabIndex = 80;
			this.ButtonZoom1ResetTele.Text = "Tele";
			this.ButtonZoom1ResetTele.Click += new System.EventHandler(this.ButtonZoom1ResetTele_Click);
			// 
			// ButtonFocusNear
			// 
			this.ButtonFocusNear.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusNear.Location = new System.Drawing.Point(635, 152);
			this.ButtonFocusNear.Name = "ButtonFocusNear";
			this.ButtonFocusNear.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusNear.TabIndex = 79;
			this.ButtonFocusNear.Text = "Near";
			this.ButtonFocusNear.Click += new System.EventHandler(this.ButtonFocusNear_Click);
			// 
			// ButtonZoomTele
			// 
			this.ButtonZoomTele.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomTele.Location = new System.Drawing.Point(635, 72);
			this.ButtonZoomTele.Name = "ButtonZoomTele";
			this.ButtonZoomTele.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomTele.TabIndex = 78;
			this.ButtonZoomTele.Text = "Tele";
			this.ButtonZoomTele.Click += new System.EventHandler(this.ButtonZoomTele_Click);
			// 
			// ButtonZoomTeleWOTrack
			// 
			this.ButtonZoomTeleWOTrack.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomTeleWOTrack.Location = new System.Drawing.Point(761, 72);
			this.ButtonZoomTeleWOTrack.Name = "ButtonZoomTeleWOTrack";
			this.ButtonZoomTeleWOTrack.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomTeleWOTrack.TabIndex = 77;
			this.ButtonZoomTeleWOTrack.Text = "Tele_ind";
			this.ButtonZoomTeleWOTrack.Click += new System.EventHandler(this.ButtonZoomTeleInd_Click);
			// 
			// ButtonFocusFar
			// 
			this.ButtonFocusFar.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusFar.Location = new System.Drawing.Point(572, 152);
			this.ButtonFocusFar.Name = "ButtonFocusFar";
			this.ButtonFocusFar.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusFar.TabIndex = 76;
			this.ButtonFocusFar.Text = "Far";
			this.ButtonFocusFar.Click += new System.EventHandler(this.ButtonFocusFar_Click);
			// 
			// ButtonZoomWide
			// 
			this.ButtonZoomWide.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomWide.Location = new System.Drawing.Point(572, 72);
			this.ButtonZoomWide.Name = "ButtonZoomWide";
			this.ButtonZoomWide.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomWide.TabIndex = 75;
			this.ButtonZoomWide.Text = "Wide";
			this.ButtonZoomWide.Click += new System.EventHandler(this.ButtonZoomWide_Click);
			// 
			// ButtonZoomWideWOTrack
			// 
			this.ButtonZoomWideWOTrack.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomWideWOTrack.Location = new System.Drawing.Point(698, 72);
			this.ButtonZoomWideWOTrack.Name = "ButtonZoomWideWOTrack";
			this.ButtonZoomWideWOTrack.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomWideWOTrack.TabIndex = 74;
			this.ButtonZoomWideWOTrack.Text = "Wide_ind";
			this.ButtonZoomWideWOTrack.Click += new System.EventHandler(this.ButtonZoomWideInd_Click);
			// 
			// ButtonFocusStop
			// 
			this.ButtonFocusStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonFocusStop.Location = new System.Drawing.Point(509, 152);
			this.ButtonFocusStop.Name = "ButtonFocusStop";
			this.ButtonFocusStop.Size = new System.Drawing.Size(57, 22);
			this.ButtonFocusStop.TabIndex = 73;
			this.ButtonFocusStop.Text = "Stop";
			this.ButtonFocusStop.Click += new System.EventHandler(this.ButtonFocusStop_Click);
			// 
			// ButtonZoomStop
			// 
			this.ButtonZoomStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonZoomStop.Location = new System.Drawing.Point(509, 72);
			this.ButtonZoomStop.Name = "ButtonZoomStop";
			this.ButtonZoomStop.Size = new System.Drawing.Size(57, 22);
			this.ButtonZoomStop.TabIndex = 72;
			this.ButtonZoomStop.Text = "Stop";
			this.ButtonZoomStop.Click += new System.EventHandler(this.ButtonZoomStop_Click);
			// 
			// LabelZoom1Reset
			// 
			this.LabelZoom1Reset.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZoom1Reset.Location = new System.Drawing.Point(452, 93);
			this.LabelZoom1Reset.Name = "LabelZoom1Reset";
			this.LabelZoom1Reset.Size = new System.Drawing.Size(51, 25);
			this.LabelZoom1Reset.TabIndex = 71;
			this.LabelZoom1Reset.Text = "Z1 Reset";
			this.LabelZoom1Reset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelFocus
			// 
			this.LabelFocus.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelFocus.Location = new System.Drawing.Point(456, 149);
			this.LabelFocus.Name = "LabelFocus";
			this.LabelFocus.Size = new System.Drawing.Size(47, 25);
			this.LabelFocus.TabIndex = 70;
			this.LabelFocus.Text = "Focus";
			this.LabelFocus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelZoom
			// 
			this.LabelZoom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LabelZoom.Location = new System.Drawing.Point(456, 70);
			this.LabelZoom.Name = "LabelZoom";
			this.LabelZoom.Size = new System.Drawing.Size(47, 25);
			this.LabelZoom.TabIndex = 69;
			this.LabelZoom.Text = "Zoom";
			this.LabelZoom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonSpeedDryStop
			// 
			this.ButtonSpeedDryStop.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSpeedDryStop.Location = new System.Drawing.Point(286, 52);
			this.ButtonSpeedDryStop.Name = "ButtonSpeedDryStop";
			this.ButtonSpeedDryStop.Size = new System.Drawing.Size(88, 22);
			this.ButtonSpeedDryStop.TabIndex = 68;
			this.ButtonSpeedDryStop.Text = "Speed Dry Stop";
			this.ButtonSpeedDryStop.Click += new System.EventHandler(this.ButtonSpeedDryStop_Click);
			// 
			// ButtonSpeedDryStart
			// 
			this.ButtonSpeedDryStart.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonSpeedDryStart.Location = new System.Drawing.Point(285, 29);
			this.ButtonSpeedDryStart.Name = "ButtonSpeedDryStart";
			this.ButtonSpeedDryStart.Size = new System.Drawing.Size(88, 22);
			this.ButtonSpeedDryStart.TabIndex = 9;
			this.ButtonSpeedDryStart.Text = "Speed Dry Start";
			this.ButtonSpeedDryStart.Click += new System.EventHandler(this.ButtonSpeedDryStart_Click);
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
			this.VISCABox.Text = "General Visca Command";
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
			this.VISCA1Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA1Box.Location = new System.Drawing.Point(60, 16);
			this.VISCA1Box.Name = "VISCA1Box";
			this.VISCA1Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA1Box.TabIndex = 1;
			// 
			// VISCA2Box
			// 
			this.VISCA2Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA2Box.Location = new System.Drawing.Point(60, 36);
			this.VISCA2Box.Name = "VISCA2Box";
			this.VISCA2Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA2Box.TabIndex = 2;
			// 
			// VISCA3Box
			// 
			this.VISCA3Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA3Box.Location = new System.Drawing.Point(60, 56);
			this.VISCA3Box.Name = "VISCA3Box";
			this.VISCA3Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA3Box.TabIndex = 3;
			// 
			// VISCA4Box
			// 
			this.VISCA4Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA4Box.Location = new System.Drawing.Point(60, 76);
			this.VISCA4Box.Name = "VISCA4Box";
			this.VISCA4Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA4Box.TabIndex = 4;
			// 
			// VISCA5Box
			// 
			this.VISCA5Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA5Box.Location = new System.Drawing.Point(60, 96);
			this.VISCA5Box.Name = "VISCA5Box";
			this.VISCA5Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA5Box.TabIndex = 5;
			// 
			// VISCA6Box
			// 
			this.VISCA6Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA6Box.Location = new System.Drawing.Point(60, 116);
			this.VISCA6Box.Name = "VISCA6Box";
			this.VISCA6Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA6Box.TabIndex = 6;
			// 
			// VISCA7Box
			// 
			this.VISCA7Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
			this.VISCA7Box.Location = new System.Drawing.Point(60, 136);
			this.VISCA7Box.Name = "VISCA7Box";
			this.VISCA7Box.Size = new System.Drawing.Size(200, 20);
			this.VISCA7Box.TabIndex = 7;
			// 
			// VISCA8Box
			// 
			this.VISCA8Box.Font = new System.Drawing.Font("·s²Ó©úÅé", 7.8F);
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
			// TimerUpdateAngle
			// 
			this.TimerUpdateAngle.Tick += new System.EventHandler(this.TimerUpdateAngle_Tick);
			// 
			// LabelFWVersion
			// 
			this.LabelFWVersion.Location = new System.Drawing.Point(6, 17);
			this.LabelFWVersion.Name = "LabelFWVersion";
			this.LabelFWVersion.Size = new System.Drawing.Size(188, 22);
			this.LabelFWVersion.TabIndex = 67;
			this.LabelFWVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBoxVersion
			// 
			this.groupBoxVersion.Controls.Add(this.LabelFWVersion);
			this.groupBoxVersion.Location = new System.Drawing.Point(808, 9);
			this.groupBoxVersion.Name = "groupBoxVersion";
			this.groupBoxVersion.Size = new System.Drawing.Size(200, 49);
			this.groupBoxVersion.TabIndex = 68;
			this.groupBoxVersion.TabStop = false;
			this.groupBoxVersion.Text = "FW Version";
			// 
			// TimerUpdateSpeed
			// 
			this.TimerUpdateSpeed.Interval = 50;
			this.TimerUpdateSpeed.Tick += new System.EventHandler(this.TimerUpdateSpeed_Tick);
			// 
			// groupBoxMCU
			// 
			this.groupBoxMCU.Controls.Add(this.ComboBoxPTType);
			this.groupBoxMCU.Controls.Add(this.LabelMCUType);
			this.groupBoxMCU.Location = new System.Drawing.Point(602, 9);
			this.groupBoxMCU.Name = "groupBoxMCU";
			this.groupBoxMCU.Size = new System.Drawing.Size(200, 49);
			this.groupBoxMCU.TabIndex = 69;
			this.groupBoxMCU.TabStop = false;
			this.groupBoxMCU.Text = "MCU";
			// 
			// ComboBoxPTType
			// 
			this.ComboBoxPTType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ComboBoxPTType.FormattingEnabled = true;
			this.ComboBoxPTType.Items.AddRange(new object[] {
            "Pan",
            "Tilt"});
			this.ComboBoxPTType.Location = new System.Drawing.Point(6, 19);
			this.ComboBoxPTType.Name = "ComboBoxPTType";
			this.ComboBoxPTType.Size = new System.Drawing.Size(85, 20);
			this.ComboBoxPTType.TabIndex = 38;
			this.ComboBoxPTType.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxPTType_SelectionChangeCommitted);
			// 
			// LabelMCUType
			// 
			this.LabelMCUType.Location = new System.Drawing.Point(97, 17);
			this.LabelMCUType.Name = "LabelMCUType";
			this.LabelMCUType.Size = new System.Drawing.Size(97, 22);
			this.LabelMCUType.TabIndex = 67;
			this.LabelMCUType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Terminal
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(1246, 720);
			this.Controls.Add(this.groupBoxMCU);
			this.Controls.Add(this.groupBoxVersion);
			this.Controls.Add(this.GroupBoxSettings);
			this.Controls.Add(this.tabVisca);
			this.Controls.Add(this.CmdClear);
			this.Controls.Add(this.TxData);
			this.Controls.Add(this.RxData);
			this.Controls.Add(this.LblTxData);
			this.Controls.Add(this.LblRxData);
			this.Controls.Add(this.CmdOpen);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Terminal";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PTTester_20210525-01";
			this.Closed += new System.EventHandler(this.TermForm_Closed);
			this.GroupBoxSettings.ResumeLayout(false);
			this.GroupBoxSettings.PerformLayout();
			this.tabVisca.ResumeLayout(false);
			this.tabMenu.ResumeLayout(false);
			this.groupBoxShowSpeed.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ChartSpeed)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox20.ResumeLayout(false);
			this.groupBox17.ResumeLayout(false);
			this.groupBox17.PerformLayout();
			this.groupBox9.ResumeLayout(false);
			this.groupBox9.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox16.ResumeLayout(false);
			this.groupBox16.PerformLayout();
			this.groupBox15.ResumeLayout(false);
			this.groupBox15.PerformLayout();
			this.groupBox10.ResumeLayout(false);
			this.groupBox10.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabAK7452.ResumeLayout(false);
			this.groupBoxShowAngle.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBoxAK7452ManualMode.ResumeLayout(false);
			this.groupBoxAK7452ManualMode.PerformLayout();
			this.groupBox19.ResumeLayout(false);
			this.groupBoxAK7452ABZResolution.ResumeLayout(false);
			this.groupBoxAK7452ABZResolution.PerformLayout();
			this.groupBoxAK7452ZeroDegreePoint.ResumeLayout(false);
			this.groupBoxAK7452ZeroDegreePoint.PerformLayout();
			this.groupBoxAK7452RDABZ.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartAngle)).EndInit();
			this.groupBoxAK7452NormalMode.ResumeLayout(false);
			this.groupBoxAK7452NormalMode.PerformLayout();
			this.tabTLE5012B.ResumeLayout(false);
			this.GroupBoxTleT_RAW.ResumeLayout(false);
			this.GroupBoxTleT_RAW.PerformLayout();
			this.GroupBoxTleIIF_CNT.ResumeLayout(false);
			this.GroupBoxTleIIF_CNT.PerformLayout();
			this.GroupBoxTleD_MAG.ResumeLayout(false);
			this.GroupBoxTleD_MAG.PerformLayout();
			this.GroupBoxTleADC_Y.ResumeLayout(false);
			this.GroupBoxTleADC_Y.PerformLayout();
			this.GroupBoxTleT25O.ResumeLayout(false);
			this.GroupBoxTleT25O.PerformLayout();
			this.GroupBoxTleADC_X.ResumeLayout(false);
			this.GroupBoxTleADC_X.PerformLayout();
			this.GroupBoxTleTCO_Y.ResumeLayout(false);
			this.GroupBoxTleTCO_Y.PerformLayout();
			this.GroupBoxTleMOD_4.ResumeLayout(false);
			this.GroupBoxTleMOD_4.PerformLayout();
			this.GroupBoxTleIFAB.ResumeLayout(false);
			this.GroupBoxTleIFAB.PerformLayout();
			this.GroupBoxTleSYNCH.ResumeLayout(false);
			this.GroupBoxTleSYNCH.PerformLayout();
			this.GroupBoxTleOFFSET_Y.ResumeLayout(false);
			this.GroupBoxTleOFFSET_Y.PerformLayout();
			this.GroupBoxTleOFFX.ResumeLayout(false);
			this.GroupBoxTleOFFX.PerformLayout();
			this.GroupBoxTleMOD_3.ResumeLayout(false);
			this.GroupBoxTleMOD_3.PerformLayout();
			this.GroupBoxTleMOD_2.ResumeLayout(false);
			this.GroupBoxTleMOD_2.PerformLayout();
			this.GroupBoxTleSIL.ResumeLayout(false);
			this.GroupBoxTleMOD_1.ResumeLayout(false);
			this.GroupBoxTleFSYNC.ResumeLayout(false);
			this.GroupBoxTleFSYNC.PerformLayout();
			this.GroupBoxTleAREV.ResumeLayout(false);
			this.GroupBoxTleAREV.PerformLayout();
			this.GroupBoxTleASPD.ResumeLayout(false);
			this.GroupBoxTleASPD.PerformLayout();
			this.GroupBoxTleAVAL.ResumeLayout(false);
			this.GroupBoxTleAVAL.PerformLayout();
			this.GroupBoxTleACSTAT.ResumeLayout(false);
			this.groupTleSTAT.ResumeLayout(false);
			this.groupTleSTAT.PerformLayout();
			this.tabLightSensor.ResumeLayout(false);
			this.groupBoxLSID.ResumeLayout(false);
			this.groupBoxLSID.PerformLayout();
			this.groupBoxLSLimit.ResumeLayout(false);
			this.groupBoxLSLimit.PerformLayout();
			this.groupBoxLSConfiguration.ResumeLayout(false);
			this.groupBoxLSConfiguration.PerformLayout();
			this.groupBoxLSResult.ResumeLayout(false);
			this.groupBoxLSResult.PerformLayout();
			this.tabAlarm.ResumeLayout(false);
			this.groupBoxNTC.ResumeLayout(false);
			this.groupBoxNTC.PerformLayout();
			this.groupBoxRS485.ResumeLayout(false);
			this.groupBoxSD700Set.ResumeLayout(false);
			this.groupBoxSD700Set.PerformLayout();
			this.groupBoxRS485DO.ResumeLayout(false);
			this.groupBoxRS485DI.ResumeLayout(false);
			this.groupBoxRS485DI.PerformLayout();
			this.groupBoxRS485Comm.ResumeLayout(false);
			this.groupBoxRS485TermR.ResumeLayout(false);
			this.groupBoxRS485TransceiverMode.ResumeLayout(false);
			this.groupBoxAlarm.ResumeLayout(false);
			this.groupBoxAlarmInq.ResumeLayout(false);
			this.groupBoxAlarmInq.PerformLayout();
			this.groupBoxAlarmOut.ResumeLayout(false);
			this.groupBoxAlarmAuto.ResumeLayout(false);
			this.groupBoxRS485Test.ResumeLayout(false);
			this.groupBoxRS485Test.PerformLayout();
			this.tabTMC2209.ResumeLayout(false);
			this.GroupBox_Chopper_Control_Register.ResumeLayout(false);
			this.GroupBox_PWM_SCALE_AUTO.ResumeLayout(false);
			this.GroupBox_PWM_SCALE_AUTO.PerformLayout();
			this.GroupBox_Chopper_PWMCONF.ResumeLayout(false);
			this.GroupBox_PWMCONF.ResumeLayout(false);
			this.GroupBox_PWMCONF.PerformLayout();
			this.GroupBox_Chopper_DRV_STATUS.ResumeLayout(false);
			this.GroupBox_DRV_STATUS.ResumeLayout(false);
			this.GroupBox_DRV_STATUS.PerformLayout();
			this.GroupBox_Chopper_CHOPCONF.ResumeLayout(false);
			this.GroupBox_CHOPCONF.ResumeLayout(false);
			this.GroupBox_STALLGARD_COOLCONF.ResumeLayout(false);
			this.GroupBox_COOLCONF.ResumeLayout(false);
			this.GroupBox_Sequencer_Registers.ResumeLayout(false);
			this.GroupBox_MSCURACT.ResumeLayout(false);
			this.GroupBox_MSCURACT.PerformLayout();
			this.GroupBox_MSCNT.ResumeLayout(false);
			this.GroupBox_MSCNT.PerformLayout();
			this.GroupBox_StallGuard_Control.ResumeLayout(false);
			this.GroupBox_SG_RESULT.ResumeLayout(false);
			this.GroupBox_SG_RESULT.PerformLayout();
			this.GroupBox_SGTHRS.ResumeLayout(false);
			this.GroupBox_SGTHRS.PerformLayout();
			this.GroupBox_TCOOLTHRS.ResumeLayout(false);
			this.GroupBox_TCOOLTHRS.PerformLayout();
			this.GroupBox_Velocity_Dependent_Control.ResumeLayout(false);
			this.GroupBox_VACTUAL.ResumeLayout(false);
			this.GroupBox_VACTUAL.PerformLayout();
			this.GroupBox_TPWMTHRS.ResumeLayout(false);
			this.GroupBox_TPWMTHRS.PerformLayout();
			this.GroupBox_TSTEP.ResumeLayout(false);
			this.GroupBox_TSTEP.PerformLayout();
			this.GroupBox_TPOWERDOWN.ResumeLayout(false);
			this.GroupBox_TPOWERDOWN.PerformLayout();
			this.GroupBox_IHOLD_IRUN.ResumeLayout(false);
			this.GroupBox_General.ResumeLayout(false);
			this.GroupBox_FACTORY_CONF.ResumeLayout(false);
			this.GroupBox_IOIN.ResumeLayout(false);
			this.GroupBox_IOIN.PerformLayout();
			this.GroupBoxGSTAT.ResumeLayout(false);
			this.GroupBox_OTP_READ.ResumeLayout(false);
			this.GroupBox_OTP_READ.PerformLayout();
			this.GroupBox_SLAVECONF.ResumeLayout(false);
			this.GroupBox_IFCNT.ResumeLayout(false);
			this.GroupBox_IFCNT.PerformLayout();
			this.GroupBox_GCONF.ResumeLayout(false);
			this.tabTest.ResumeLayout(false);
			this.tabTest.PerformLayout();
			this.VISCABox.ResumeLayout(false);
			this.VISCABox.PerformLayout();
			this.groupBoxVersion.ResumeLayout(false);
			this.groupBoxMCU.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Methods
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
			this.TxData.Text = "";

			RxBytes = 0;
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
				this.TxData.Text = "";
			}

			this.TxData.AppendText(this.AtoX(s));
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
				this.RxData.Text = "";
			}

			this.RxData.AppendText(this.AtoX(s));
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
					this.CmdOpen.Text = "Off";
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
				this.CmdOpen.Text = "On";
				this.RecvTimer.Enabled = false;
				this.TimerUpdateAngle.Enabled = false;
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

			if (b.Length > 0)
			{
				// Send this packet of data.
				nSent = this.Port.Send(b);

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
				}

				string strBuf = Encoding.ASCII.GetString(buf);
				this.TxDataUpdate(strBuf);
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

	/// <summary>
	/// S_pqTo0p0q
	/// </summary>
	//private byte[] StringTo_pqTo0p0q( string strText )
	//{
	//	byte	len;
	//	byte	tempByte;
	//	byte[]	buffHex = new byte[2];
	//	byte[]	rBuffer = new byte[2];

	//	len = (byte)(strText.Length);

	//	for( int i=0;i<=(len-1); i++ )
	//	{
	//		tempByte =(byte)strText[i];
	//		buffHex[i] = AsciiToHex( tempByte);
	//	}

	//	switch( len )
	//	{
	//		case 1:		rBuffer[0] = 0;	
	//					rBuffer[1] = (byte)buffHex[0];
	//					break;
	//		case 2:		rBuffer[0] = (byte)buffHex[0];
	//					rBuffer[1] = (byte)buffHex[1];
	//					break;
	//		default:	rBuffer[0] = (byte)buffHex[0];
	//					rBuffer[1] = (byte)buffHex[1];
	//					break;
	//	}

	//	return rBuffer;
	//}

	/// <summary>
	/// S_pqrsTo0p0q0r0s
	/// </summary>
	//private byte[] StringTo_pqrsTo0p0q0r0s( string strText )
	//{

	//	byte	len;
	//	byte	tempByte;
	//	byte[]	buffHex = new byte[4];
	//	byte[]	rBuffer = new byte[4];

	//	len = (byte)(strText.Length);

	//	for( int i=0;i<=(len-1); i++ )
	//	{
	//		tempByte =(byte)strText[i];
	//		buffHex[i] = AsciiToHex( tempByte);
	//	}

	//	switch( len )
	//	{
	//		case 1:	rBuffer[0] = 0;	
	//				rBuffer[1] = 0;
	//				rBuffer[2] = 0;
	//				rBuffer[3] = (byte)buffHex[0];
	//				break;
	//		case 2:	rBuffer[0] = 0;	
	//				rBuffer[1] = 0;	
	//				rBuffer[2] = (byte)buffHex[0];	
	//				rBuffer[3] = (byte)buffHex[1];
	//				break;
	//		case 3:	rBuffer[0] = 0;	
	//				rBuffer[1] = (byte)buffHex[0];
	//				rBuffer[2] = (byte)buffHex[1];
	//				rBuffer[3] = (byte)buffHex[2];
	//				break;
	//		case 4:	rBuffer[0] = (byte)buffHex[0];	
	//				rBuffer[1] = (byte)buffHex[1];
	//				rBuffer[2] = (byte)buffHex[2];	
	//				rBuffer[3] = (byte)buffHex[3];
	//				break;
	//		default:
	//				rBuffer[0] = (byte)buffHex[0];	
	//				rBuffer[1] = (byte)buffHex[1];
	//				rBuffer[2] = (byte)buffHex[2];	
	//				rBuffer[3] = (byte)buffHex[3];
	//				break;
	//	}

	//	return rBuffer;

	//}
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
		}
		#endregion

		#region Version
		private void GetVersion()
		{
			byte[] b = { 0x81, 0x09, 0x00, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.SYS_ReadVersion;
			this.SendBuf(b);
		}

		private void GetMCUType()
		{
			byte[] b = { 0x81, 0x09, 0x00, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.SYS_ReadMCUType;
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

		private void UpdateAngleChart()
		{
			chartAngle.Series["Angle"].Points.Clear();

			chartAngle.ChartAreas[0].AxisX.Minimum = TimeArray[0];
			chartAngle.ChartAreas[0].AxisX.Maximum = TimeArray[TimeArray.Length - 1];

			for (int i = 0; i < AngleArray.Length - 1; ++i)
				chartAngle.Series["Angle"].Points.AddXY(TimeArray[i], AngleArray[i]);
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
			short data_signed;
			ushort data;
			byte bData;

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
						GetMCUType();
						break;

					case GeneralCommand.SYS_ReadMCUType:
						if (localRxPacket[2] == 0x00)
							this.LabelMCUType.Text = "Pan";
						else if (localRxPacket[2] == 0x01)
							this.LabelMCUType.Text = "Tilt";
						G_eGeneralCommand = GeneralCommand.SYS_NONE;
						break;

					case GeneralCommand.MD_ReadMotorPosition:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxMotorPosition.Text = data.ToString();
						G_eGeneralCommand = GeneralCommand.SYS_NONE;
						break;

					case GeneralCommand.MD_ReadMotoeAccLevel:
						data = (ushort)(localRxPacket[2] & 0x0f);
						this.ComboBoxAccLevel.SelectedIndex = data - 1;
						break;

					case GeneralCommand.MD_ReadSpeedByZoomRatio:
						data = (ushort)(localRxPacket[2] & 0xff);
						this.TextBoxSpeedByZoomRatio.Text = data.ToString();
						break;

					case GeneralCommand.AS_ReadAngleReg:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxAngleDataReg.Text = data.ToString();
						break;

					case GeneralCommand.AS_ReadMagFlux:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxMagFlux.Text = ((float)data / 100).ToString() + " mT";
						break;

					case GeneralCommand.AS_ReadMode:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						if (data == 0x000)
							this.TextBoxASMode.Text = "Normal Mode";
						else if (data == 0x50F)
							this.TextBoxASMode.Text = "User Mode";
						else if (data == 0xAF0)
							this.TextBoxASMode.Text = "User Mode2";
						break;

					case GeneralCommand.AS_ReadErrorMonitor:
						data = (ushort)(localRxPacket[5] & 0x03);
						if ((data == 0) || (data == 1) || (data == 2))
							this.TextBoxErrorMonitor.Text = "Error(" + data.ToString() + ")";
						else if (data == 3)
							this.TextBoxErrorMonitor.Text = "Normal";
						break;

					case GeneralCommand.AS_ReadZP:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxZP.Text = data.ToString();
						break;

					case GeneralCommand.AS_ReadRDABZ:
						data = (ushort)(localRxPacket[2] & 0x0f);
						this.ComboBoxABZHysteresis.SelectedIndex = data;
						data = (ushort)(localRxPacket[3] & 0x01);
						this.ComboBoxABZEnable.SelectedIndex = data;
						data = (ushort)(localRxPacket[4] & 0x03);
						this.ComboBoxZWidth.SelectedIndex = data;
						data = (ushort)(localRxPacket[5] & 0x01);
						this.ComboBoxRD.SelectedIndex = data;
						break;

					case GeneralCommand.AS_ReadABZRes:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxABZRes.Text = data.ToString();
						break;

					case GeneralCommand.AS_ReadMemLock:
						data = (ushort)(localRxPacket[5] & 0x03);
						if ((data == 0) || (data == 1) || (data == 2))
							this.TextBoxMemLock.Text = "Locked";
						else if (data == 3)
							this.TextBoxMemLock.Text = "Unlocked";
						break;

					case GeneralCommand.AS_ReadSDDIS:
						data = (ushort)(localRxPacket[5] & 0x03);
						this.ComboBoxSDDIS.SelectedIndex = data;
						break;

					case GeneralCommand.AS_ReadAngle:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.textBoxAngleData.Text = data.ToString();
						this.TextBoxMotorAngle.Text = data.ToString();

						AngleArray[AngleArray.Length - 1] = data;
						TimeArray[TimeArray.Length - 1] = TimerUpdateAngleCnt * 0.05;

						Array.Copy(AngleArray, 1, AngleArray, 0, AngleArray.Length - 1);
						Array.Copy(TimeArray, 1, TimeArray, 0, TimeArray.Length - 1);

						UpdateAngleChart();
						TimerUpdateAngleCnt++;
						G_eGeneralCommand = GeneralCommand.SYS_NONE;
						break;

					case GeneralCommand.AS_ReadAB:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.textBoxABCount.Text = data.ToString();
						break;

					case GeneralCommand.AS_ReadZ:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.textBoxZCount.Text = data.ToString();
						break;

					case GeneralCommand.AS_ReadErrorPin:
						data = (ushort)(localRxPacket[5] & 0x01);
						if (data == 0)
							this.textBoxErrorPin.Text = "Normal";
						else if (data == 1)
							this.textBoxErrorPin.Text = "Error";
						break;

					case GeneralCommand.AS_ReadZPCalibration:
						data = (ushort)(localRxPacket[5] & 0x01);
						if (data == 1)
							this.TextBoxZPCalibration.Text = "Done";
						else if (data == 0)
							this.TextBoxZPCalibration.Text = "Not Done";
						break;

					case GeneralCommand.AS_ReadZPLock:
						data = (ushort)(localRxPacket[5] & 0x01);
						if (data == 1)
							this.TextBoxLockStatus.Text = "Locked";
						else if (data == 0)
							this.TextBoxLockStatus.Text = "Unlocked";
						break;

					case GeneralCommand.MD_ReadAcceleration:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxAcceleration.Text = data.ToString();
						break;

					case GeneralCommand.MD_ReadPanType:
						this.ComboBoxPanMethod.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.MD_ReadSpeed:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxCurrentSpeed.Text = data.ToString();

						SpeedArray[SpeedArray.Length - 1] = data;
						SpeedTimeArray[SpeedTimeArray.Length - 1] = TimerUpdateSpeedCnt * 0.05;

						Array.Copy(SpeedArray, 1, SpeedArray, 0, SpeedArray.Length - 1);
						Array.Copy(SpeedTimeArray, 1, SpeedTimeArray, 0, SpeedTimeArray.Length - 1);

						UpdateSpeedChart();
						TimerUpdateSpeedCnt++;
						break;

					case GeneralCommand.MD_ReadMotorSpeedInPPS:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxSpeedInPPS.Text = data.ToString();
						break;

					case GeneralCommand.LS_ReadLux:
						bData = localRxPacket[2];
						this.TextBoxLSE.Text = bData.ToString();

						data = (ushort)((localRxPacket[3] & 0x0f) << 8);
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxLSR.Text = data.ToString();
						this.TextBoxLSLux.Text = (0.01 * (1 << bData) * data).ToString();
						break;

					case GeneralCommand.LS_ReadConfiguration:
						this.TextBoxLSRN.Text = ((localRxPacket[2] & 0x0f) >> 0).ToString();
						this.TextBoxLSCT.Text = ((localRxPacket[3] & 0x08) >> 3).ToString();
						this.TextBoxLSM.Text = ((localRxPacket[3] & 0x06) >> 1).ToString();
						this.TextBoxLSOVF.Text = ((localRxPacket[3] & 0x01) >> 0).ToString();
						this.TextBoxLSCRF.Text = ((localRxPacket[4] & 0x08) >> 3).ToString();
						this.TextBoxLSFH.Text = ((localRxPacket[4] & 0x04) >> 2).ToString();
						this.TextBoxLSFL.Text = ((localRxPacket[4] & 0x02) >> 1).ToString();
						this.TextBoxLSL.Text = ((localRxPacket[4] & 0x01) >> 0).ToString();
						this.TextBoxLSPOL.Text = ((localRxPacket[5] & 0x08) >> 3).ToString();
						this.TextBoxLSME.Text = ((localRxPacket[5] & 0x04) >> 2).ToString();
						this.TextBoxLSFC.Text = ((localRxPacket[5] & 0x03) >> 0).ToString();
						break;

					case GeneralCommand.LS_ReadLowLimit:
					case GeneralCommand.LS_ReadHighLimit:
						bData = localRxPacket[2];
						this.TextBoxLSELimit.Text = bData.ToString();

						data = (ushort)((localRxPacket[3] & 0x0f) << 8);
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxLSRLimit.Text = data.ToString();
						this.TextBoxLSLuxLimit.Text = (0.01 * (1 << bData) * data).ToString();
						break;

					case GeneralCommand.LS_ReadManufacturerID:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxLSID.Text = Convert.ToString(data, 16) + "h";
						break;

					case GeneralCommand.LS_ReadDeviceID:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxLSDID.Text = Convert.ToString(data, 16) + "h";
						break;

					case GeneralCommand.ALM_ReadStatus:
						data = (ushort)(localRxPacket[2] & 0x03);
						this.TextBoxAlarmStatus.Text = (data == 2) ? "On" : "Off";
						break;

					case GeneralCommand.ALM_ReadAlarmIn:
						data = (ushort)(localRxPacket[2] & 0x03);
						this.TextBoxAlarmDI1.Text = ((data & 0x2) == 0x2) ? "1" : "0";
						this.TextBoxAlarmDI0.Text = ((data & 0x1) == 0x1) ? "1" : "0";
						break;

					case GeneralCommand.ALM_ReadTrigLvl:
						data = (ushort)(localRxPacket[2] & 0x03);
						this.TextBoxAlarmTrigLvl.Text = (data == 2) ? "High" : "Low";
						break;

					case GeneralCommand.RS_ReadMode:
						data = (ushort)(localRxPacket[2] & 0x03);
						if (data == 0x02)
							this.ComboBoxRS485Mode.SelectedIndex = 0;
						else if (data == 0x03)
							this.ComboBoxRS485Mode.SelectedIndex = 1;
						break;

					case GeneralCommand.RS_ReadTermR:
						data = (ushort)(localRxPacket[2] & 0x03);
						if (data == 0x02)
							this.ComboBoxRS485TermR.SelectedIndex = 1;
						else if (data == 0x03)
							this.ComboBoxRS485TermR.SelectedIndex = 0;
						break;

					case GeneralCommand.RS_ReadBaudRate:
						data = (ushort)(localRxPacket[2] & 0x0f);
						this.ComboBoxRS485BaudRate.SelectedIndex = data - 1;
						GetRS485StopBits();
						break;

					case GeneralCommand.RS_ReadStopBits:
						if (localRxPacket[2] == 1)
							this.ComboBoxRS485StopBits.SelectedIndex = 0;
						else if (localRxPacket[2] == 2)
							this.ComboBoxRS485StopBits.SelectedIndex = 1;
						break;

					case GeneralCommand.RS_ReadRS485Dev:
						if (localRxPacket[2] == 0)
							this.ComboBoxRS485Dev.SelectedIndex = 0;
						else if (localRxPacket[2] == 1)
							this.ComboBoxRS485Dev.SelectedIndex = 1;
						else if (localRxPacket[2] == 2)
							this.ComboBoxRS485Dev.SelectedIndex = 2;
						break;

					case GeneralCommand.RS_ReadRS485DevAddr:
						this.TextBoxSD700Addr.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.RS_ReadDI:
						this.TextBoxRS485DI1.Text = ((localRxPacket[3] & 0x1) == 0x1) ? "On" : "Off";
						this.TextBoxRS485DI2.Text = ((localRxPacket[3] & 0x2) == 0x2) ? "On" : "Off";
						this.TextBoxRS485DI3.Text = ((localRxPacket[3] & 0x4) == 0x4) ? "On" : "Off";
						this.TextBoxRS485DI4.Text = ((localRxPacket[3] & 0x8) == 0x8) ? "On" : "Off";
						this.TextBoxRS485DI5.Text = ((localRxPacket[2] & 0x1) == 0x1) ? "On" : "Off";
						this.TextBoxRS485DI6.Text = ((localRxPacket[2] & 0x2) == 0x2) ? "On" : "Off";
						this.TextBoxRS485DI7.Text = ((localRxPacket[2] & 0x4) == 0x4) ? "On" : "Off";
						this.TextBoxRS485DI8.Text = ((localRxPacket[2] & 0x8) == 0x8) ? "On" : "Off";
						break;

					case GeneralCommand.RS_ReadDO:
						this.CheckBoxRS485DO1.Checked = ((localRxPacket[3] & 0x1) == 0x1);
						this.CheckBoxRS485DO2.Checked = ((localRxPacket[3] & 0x2) == 0x2);
						this.CheckBoxRS485DO3.Checked = ((localRxPacket[3] & 0x4) == 0x4);
						this.CheckBoxRS485DO4.Checked = ((localRxPacket[3] & 0x8) == 0x8);
						this.CheckBoxRS485DO5.Checked = ((localRxPacket[2] & 0x1) == 0x1);
						this.CheckBoxRS485DO6.Checked = ((localRxPacket[2] & 0x2) == 0x2);
						this.CheckBoxRS485DO7.Checked = ((localRxPacket[2] & 0x4) == 0x4);
						this.CheckBoxRS485DO8.Checked = ((localRxPacket[2] & 0x8) == 0x8);

						this.CheckBoxRS485DO1.Text = ((localRxPacket[3] & 0x1) == 0x1) ? "On" : "Off";
						this.CheckBoxRS485DO2.Text = ((localRxPacket[3] & 0x2) == 0x2) ? "On" : "Off";
						this.CheckBoxRS485DO3.Text = ((localRxPacket[3] & 0x4) == 0x4) ? "On" : "Off";
						this.CheckBoxRS485DO4.Text = ((localRxPacket[3] & 0x8) == 0x8) ? "On" : "Off";
						this.CheckBoxRS485DO5.Text = ((localRxPacket[2] & 0x1) == 0x1) ? "On" : "Off";
						this.CheckBoxRS485DO6.Text = ((localRxPacket[2] & 0x2) == 0x2) ? "On" : "Off";
						this.CheckBoxRS485DO7.Text = ((localRxPacket[2] & 0x4) == 0x4) ? "On" : "Off";
						this.CheckBoxRS485DO8.Text = ((localRxPacket[2] & 0x8) == 0x8) ? "On" : "Off";
						break;

					case GeneralCommand.NTC_ReadNTC1:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						data_signed = (short)data;
						this.TextBoxNTC1.Text = data_signed.ToString();
						break;

					case GeneralCommand.NTC_ReadNTC2:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						data_signed = (short)data;
						this.TextBoxNTC2.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_S_RST:
						this.TextBoxTleStatS_RST.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_Error:
						this.TextBoxTleStatError.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_S_NR:
						this.ComboBoxTleStatS_NR.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_RD_ST:
						this.TextBoxTleStatRD_ST.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_AS_RST:
						this.ComboBoxTleAS_RST.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_WD:
						this.ComboBoxTleAS_WD.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_VR:
						this.ComboBoxTleAS_VR.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_FUSE:
						this.ComboBoxTleAS_FUSE.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_DSPU:
						this.ComboBoxTleAS_DSPU.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_OV:
						this.ComboBoxTleAS_OV.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_VEC_XY:
						this.ComboBoxTleAS_VEC_XY.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_VEC_MAG:
						this.ComboBoxTleAS_VEC_MAG.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_ADCT:
						this.ComboBoxTleAS_ADCT.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AS_FRST:
						this.ComboBoxTleAS_FRST.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_ANG_VAL:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleANG_VAL.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_RD_AV:
						this.TextBoxTleRD_AV.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_ANG_SPD:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 16383)
							data_signed = (short)(data - 32768);
						else
							data_signed = (short)data;
						this.TextBoxTleANG_SPD.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_RD_AS:
						this.TextBoxTleRD_AS.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_REVOL:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));						

						if (data > 255)
							data_signed = (short)(data - 512);
						else
							data_signed = (short)data;						
						this.TextBoxTleREVOL.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_FCNT:
						this.TextBoxTleFCNT.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_RD_REV:
						this.TextBoxTleRD_REV.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_TEMPER:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 255)
							data_signed = (short)(data - 512);
						else
							data_signed = (short)data;
						this.TextBoxTleTEMPER.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_FSYNC:
						this.TextBoxTleFSYNC.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_IIF_MOD:
						this.ComboBoxTleIIF_MOD.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_DSPU_HOLD:
						this.ComboBoxTleDSPU_HOLD.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_CLK_SEL:
						this.ComboBoxTleCLK_SEL.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_FIR_MD:
						this.ComboBoxTleFIR_MD.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_ADCTV_X:
						this.ComboBoxTleADCTV_X.SelectedIndex = localRxPacket[2] & 0x07;
						break;

					case GeneralCommand.TLE_Read_ADCTV_Y:
						this.ComboBoxTleADCTV_Y.SelectedIndex = localRxPacket[2] & 0x07;
						break;

					case GeneralCommand.TLE_Read_ADCTV_EN:
						this.ComboBoxTleADCTV_EN.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_FUSE_REL:
						this.ComboBoxTleFUSE_REL.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_FILT_INV:
						this.ComboBoxTleFILT_INV.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_FILT_PAR:
						this.ComboBoxTleFILT_PAR.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_AUTOCAL:
						this.ComboBoxTleAUTOCAL.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_PREDICT:
						this.ComboBoxTlePREDICT.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_ANG_DIR:
						this.ComboBoxTleANG_DIR.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_ANG_RANGE:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleANG_RANGE.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_PAD_DRV:
						this.ComboBoxTlePAD_DRV.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_SSC_OD:
						this.ComboBoxTleSSC_OD.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_SPIKEF:
						this.ComboBoxTleSPIKEF.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_ANG_BASE:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleANG_BASE.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_OFFX:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleOFFX.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_OFFY:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleOFFY.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_SYNCH:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleSYNCH.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_IFAB_HYST:
						this.ComboBoxTleIFAB_HYST.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_IFAB_OD:
						this.ComboBoxTleIFAB_OD.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_FIR_UDR:
						this.ComboBoxTleFIR_UDR.SelectedIndex = localRxPacket[2] & 0x01;
						break;

					case GeneralCommand.TLE_Read_ORTHO:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 2047)
							data_signed = (short)(data - 4096);
						else
							data_signed = (short)data;
						this.TextBoxTleORTHO.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_IF_MD:
						this.ComboBoxTleIF_MD.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_IFAB_RES:
						this.ComboBoxTleIFAB_RES.SelectedIndex = localRxPacket[2] & 0x03;
						break;

					case GeneralCommand.TLE_Read_HSM_PLP:
						if ((localRxPacket[2] & 0x4) == 0x0)
							this.ComboBoxTleHSM_PLP.SelectedIndex = 1;
						else if ((localRxPacket[2] & 0x4) == 0x4)
							this.ComboBoxTleHSM_PLP.SelectedIndex = 0;
						break;

					case GeneralCommand.TLE_Read_TCO_X_T:
						this.TextBoxTleTCO_X_T.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_CRC_PAR:
						this.TextBoxTleCRC_PAR.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_SBIST:
						this.ComboBoxTleSBIST.SelectedIndex = localRxPacket[2] & 0x1;
						break;

					case GeneralCommand.TLE_Read_TCO_Y_T:
						this.TextBoxTleTCO_Y_T.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_ADC_X:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 32767)
							data_signed = (short)(data - 65536);
						else
							data_signed = (short)data;
						this.TextBoxTleADC_X.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_ADC_Y:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 32767)
							data_signed = (short)(data - 65536);
						else
							data_signed = (short)data;
						this.TextBoxTleADC_Y.Text = data_signed.ToString();
						break;

					case GeneralCommand.TLE_Read_MAG:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleMAG.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_T_RAW:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleT_RAW.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_T_TGL:
						this.TextBoxTleT_TGL.Text = localRxPacket[2].ToString();
						break;

					case GeneralCommand.TLE_Read_IIF_CNT:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxTleIIF_CNT.Text = data.ToString();
						break;

					case GeneralCommand.TLE_Read_T25O:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 63)
							data_signed = (short)(data - 128);
						else
							data_signed = (short)data;
						this.TextBoxTleT25O.Text = data_signed.ToString();
						break;

					case GeneralCommand.TMC_Read_I_scale_analog:
						this.ComboBox_I_scale_analog.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_internal_Rsense:
						this.ComboBox_internal_Rsense.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_en_SpreadCycle:
						this.ComboBox_en_SpreadCycle.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_shaft:
						this.ComboBox_shaft.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_index_otpw:
						this.ComboBox_index_otpw.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_index_step:
						this.ComboBox_index_step.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_pdn_disable:
						this.ComboBox_pdn_disable.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_mstep_reg_select:
						this.ComboBox_mstep_reg_sel.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_multistep_filt:
						this.ComboBox_multistep_filt.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_test_mode:
						this.ComboBox_test_mode.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_reset:
						this.ComboBox_reset.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_drv_err:
						this.ComboBox_drv_err.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_uv_cp:
						this.ComboBox_uv_cp.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_IFCNT:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_IFCNT.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_SLAVECONF:
						this.ComboBox_SLAVECONF.SelectedIndex = (localRxPacket[3] & 0x0F) / 2;
						break;

					case GeneralCommand.TMC_Read_OTP_READ:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_OTP_READ_2.Text = data.ToString();
						data = (ushort)((localRxPacket[4] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[5] & 0x0f) << 0));
						this.TextBox_OTP_READ_1.Text = data.ToString();
						data = (ushort)((localRxPacket[6] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[7] & 0x0f) << 0));
						this.TextBox_OTP_READ_0.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_ENN:
						this.TextBox_ENN.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_MS1:
						this.TextBox_MS1.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_MS2:
						this.TextBox_MS2.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_DIAG:
						this.TextBox_DIAG.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_PDN_UART:
						this.TextBox_PDN_UART.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_STEP:
						this.TextBox_STEP.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_SPREAD_EN:
						this.TextBox_SPREAD_EN.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_DIR:
						this.TextBox_DIR.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_VERSION:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_VERSION.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_FCLKTRIM:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.ComboBox_FCLKTRIM.SelectedIndex = data & 0x1F;
						break;

					case GeneralCommand.TMC_Read_OTTRIM:
						this.ComboBox_OTTRIM.SelectedIndex = localRxPacket[3] & 0x03;
						break;

					case GeneralCommand.TMC_Read_IHOLD:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.ComboBox_IHOLD.SelectedIndex = data & 0x1F;
						break;

					case GeneralCommand.TMC_Read_IRUN:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.ComboBox_IRUN.SelectedIndex = data & 0x1F;
						break;

					case GeneralCommand.TMC_Read_IHOLDDELAY:
						this.ComboBox_IHOLDDELAY.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_TPOWERDOWN:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_TPOWERDOWN.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_TSTEP:
						data = (ushort)((localRxPacket[2] & 0x0f) << 20);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 16));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 12));
						data = (ushort)(data | ((localRxPacket[5] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[6] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[7] & 0x0f));
						this.TextBox_TSTEP.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_TPWMTHRS:
						data = (ushort)((localRxPacket[2] & 0x0f) << 20);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 16));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 12));
						data = (ushort)(data | ((localRxPacket[5] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[6] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[7] & 0x0f));
						this.TextBox_TPWMTHRS.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_VACTUAL:
						data = (ushort)((localRxPacket[2] & 0x0f) << 20);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 16));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 12));
						data = (ushort)(data | ((localRxPacket[5] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[6] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[7] & 0x0f));
						this.TextBox_VACTUAL.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_TCOOLTHRS:
						data = (ushort)((localRxPacket[2] & 0x0f) << 20);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 16));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 12));
						data = (ushort)(data | ((localRxPacket[5] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[6] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[7] & 0x0f));
						this.TextBox_TCOOLTHRS.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_SGTHRS:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_SGTHRS.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_SG_RESULT:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBox_SG_RESULT.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_MSCNT:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBox_MSCNT.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_CUR_A:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 255)
							data_signed = (short)(data - 512);
						else
							data_signed = (short)data;
						this.TextBox_CUR_A.Text = data_signed.ToString();
						break;

					case GeneralCommand.TMC_Read_CUR_B:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 255)
							data_signed = (short)(data - 512);
						else
							data_signed = (short)data;
						this.TextBox_CUR_B.Text = data_signed.ToString();
						break;

					case GeneralCommand.TMC_Read_semin:
						this.ComboBox_semin.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_seup:
						this.ComboBox_seup.SelectedIndex = localRxPacket[3] & 0x03;
						break;

					case GeneralCommand.TMC_Read_semax:
						this.ComboBox_semax.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_sedn:
						this.ComboBox_sedn.SelectedIndex = localRxPacket[3] & 0x03;
						break;

					case GeneralCommand.TMC_Read_seimin:
						this.ComboBox_seimin.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_TOFF:
						this.ComboBox_TOFF.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_HSTRT:
						this.ComboBox_HSTRT.SelectedIndex = localRxPacket[3] & 0x07;
						break;

					case GeneralCommand.TMC_Read_HEND:
						this.ComboBox_HEND.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_TBL:
						this.ComboBox_TBL.SelectedIndex = localRxPacket[3] & 0x03;
						break;

					case GeneralCommand.TMC_Read_vsense:
						this.ComboBox_vsense.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_MRES:
						this.ComboBox_MRES.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_intpol:
						this.ComboBox_intpol.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_dedge:
						this.ComboBox_dedge.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_diss2g:
						this.ComboBox_diss2g.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_diss2vs:
						this.ComboBox_diss2vs.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_otpw:
						this.TextBox_otpw.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_ot:
						this.TextBox_ot.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_s2ga:
						this.TextBox_s2ga.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_s2gb:
						this.TextBox_s2gb.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_s2vsa:
						this.TextBox_s2vsa.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_s2vsb:
						this.TextBox_s2vsb.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_ola:
						this.TextBox_ola.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_olb:
						this.TextBox_olb.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_t120:
						this.TextBox_t120.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_t143:
						this.TextBox_t143.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_t150:
						this.TextBox_t150.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_t157:
						this.TextBox_t157.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_CS_ACTUAL:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_CS_ACTUAL.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_stealth:
						this.TextBox_stealth.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_stst:
						this.TextBox_stst.Text = localRxPacket[3].ToString();
						break;

					case GeneralCommand.TMC_Read_PWM_OFS:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_PWM_OFS.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_PWM_GRAD:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_PWM_GRAD.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_pwm_freq:
						this.ComboBox_pwm_freq.SelectedIndex = localRxPacket[3] & 0x03;
						break;

					case GeneralCommand.TMC_Read_pwm_autoscale:
						this.ComboBox_pwm_autoscale.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_pwm_autograd:
						this.ComboBox_pwm_autograd.SelectedIndex = localRxPacket[3] & 0x01;
						break;

					case GeneralCommand.TMC_Read_freewheel:
						this.ComboBox_freewheel.SelectedIndex = localRxPacket[3] & 0x03;
						break;

					case GeneralCommand.TMC_Read_PWM_REG:
						this.ComboBox_PWM_REG.SelectedIndex = (localRxPacket[3] & 0x0F) - 1;
						break;

					case GeneralCommand.TMC_Read_PWM_LIM:
						this.ComboBox_PWM_LIM.SelectedIndex = localRxPacket[3] & 0x0F;
						break;

					case GeneralCommand.TMC_Read_PWM_SCALE_SUM:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_PWM_SCALE_SUM.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_PWM_SCALE_AUTO:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 255)
							data_signed = (short)(data - 512);
						else
							data_signed = (short)data;
						this.TextBox_PWM_SCALE_AUTO.Text = data_signed.ToString();
						break;

					case GeneralCommand.TMC_Read_PWM_OFS_AUTO:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_PWM_OFS_AUTO.Text = data.ToString();
						break;

					case GeneralCommand.TMC_Read_PWM_GRAD_AUTO:
						data = (ushort)((localRxPacket[2] & 0x0f) << 4);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 0));
						this.TextBox_PWM_GRAD_AUTO.Text = data.ToString();
						break;

					case GeneralCommand.Lens_ReadZ1Position:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxZ1Pos.Text = data.ToString();
						G_eGeneralCommand = GeneralCommand.SYS_NONE;
						break;

					case GeneralCommand.Lens_ReadFPosition:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));

						if (data > 0xFFF)
							data_signed = (short)((-1)*(data - 0x1000));
						else
							data_signed = (short)data;
						this.TextBoxFPos.Text = data_signed.ToString();
						G_eGeneralCommand = GeneralCommand.SYS_NONE;
						break;

					case GeneralCommand.Lens_ReadZ2Position:
						data = (ushort)((localRxPacket[2] & 0x0f) << 12);
						data = (ushort)(data | ((localRxPacket[3] & 0x0f) << 8));
						data = (ushort)(data | ((localRxPacket[4] & 0x0f) << 4));
						data = (ushort)(data | (localRxPacket[5] & 0x0f));
						this.TextBoxZ2Pos.Text = data.ToString();
						G_eGeneralCommand = GeneralCommand.SYS_NONE;
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

		private void TimerUpdateAngle_Tick(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x51, 0xFF };

			G_eGeneralCommand = GeneralCommand.AS_ReadAngle;
			this.SendBuf(b);
		}

		private void TimerUpdateSpeed_Tick(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x06, 0x03, 0xFF };

			G_eGeneralCommand = GeneralCommand.MD_ReadSpeed;
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

		#region AngleSensorTab
		private void ButtonSetNormalMode_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x03, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonAngleData_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x51, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadAngle;
			this.SendBuf(b);
		}

		private void ButtonErrorPin_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x54, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadErrorPin;
			this.SendBuf(b);
		}

		private void ButtonShowAngle_Click(object sender, EventArgs e)
		{
			TimerUpdateAngle.Enabled = true;
		}

		private void ButtonStopShowAngle_Click(object sender, EventArgs e)
		{
			TimerUpdateAngle.Enabled = false;
		}

		private void ButtonClearShowAngle_Click(object sender, EventArgs e)
		{
			chartAngle.Series[0].Points.Clear();

			Array.Clear(TimeArray, 0, TimeArray.Length);
			Array.Clear(AngleArray, 0, AngleArray.Length);

			TimerUpdateAngleCnt = 0;
		}

		private void ButtonSetMaualMode_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x03, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonAngleDataReg_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadAngleReg;
			this.SendBuf(b);
		}

		private void ButtonMagFlux_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadMagFlux;
			this.SendBuf(b);
		}

		private void ButtonASMode_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadMode;
			this.SendBuf(b);
		}

		private void ButtonErrorMonitor_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadErrorMonitor;
			this.SendBuf(b);
		}

		private void ButtonGetZP_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadZP;
			this.SendBuf(b);
		}

		private void ButtonMemLock_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadMemLock;
			this.SendBuf(b);
		}

		private void ButtonGetABZRes_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadABZRes;
			this.SendBuf(b);
		}

		private void ButtonSetABZRes_Click(object sender, EventArgs e)
		{
			uint data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00, 0xFF };

			data = (uint)(Convert.ToUInt16(this.TextBoxABZRes.Text));

			b[4] = (byte)((data >> 12) & 0x0f);
			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);

			if (CheckBoxSaveToEEPROM.Checked == true)
			{
				b[3] = 0x20;
				this.SendBuf(b);
			}
		}

		private void ButtonGetSDDIS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0A, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadSDDIS;
			this.SendBuf(b);
		}

		private void ButtonSetSDDIS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0A, 0x00, 0xFF };

			if (this.ComboBoxSDDIS.SelectedIndex == -1) this.ComboBoxSDDIS.SelectedIndex = 0;

			b[4] = (byte)(this.ComboBoxSDDIS.SelectedIndex & 0x03);
			this.SendBuf(b);

			if (CheckBoxSaveToEEPROM.Checked == true)
			{
				b[3] = 0x28;
				this.SendBuf(b);
			}
		}

		private void ButtonGetRDABZ_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadRDABZ;
			this.SendBuf(b);
		}

		private void ButtonSetRDABZ_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x07, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.ComboBoxRD.SelectedIndex == -1) this.ComboBoxRD.SelectedIndex = 0;
			if (this.ComboBoxZWidth.SelectedIndex == -1) this.ComboBoxZWidth.SelectedIndex = 0;
			if (this.ComboBoxABZEnable.SelectedIndex == -1) this.ComboBoxABZEnable.SelectedIndex = 0;
			if (this.ComboBoxABZHysteresis.SelectedIndex == -1) this.ComboBoxABZHysteresis.SelectedIndex = 0;

			b[4] = (byte)(this.ComboBoxABZHysteresis.SelectedIndex & 0x0f);
			b[5] = (byte)(this.ComboBoxABZEnable.SelectedIndex & 0x01);
			b[6] = (byte)(this.ComboBoxZWidth.SelectedIndex & 0x03);
			b[7] = (byte)(this.ComboBoxRD.SelectedIndex & 0x01);

			this.SendBuf(b);

			if (CheckBoxSaveToEEPROM.Checked == true)
			{
				b[3] = 0x1C;
				this.SendBuf(b);
			}
		}

		private void ButtonSetZP_Click(object sender, EventArgs e)
		{
			uint data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x05, 0x00, 0x00, 0x00, 0x00, 0xFF };

			data = Convert.ToUInt16(this.TextBoxZP.Text);

			b[4] = (byte)((data >> 12) & 0x0f);
			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);

			if (CheckBoxSaveToEEPROM.Checked == true)
			{
				b[3] = 0x14;
				this.SendBuf(b);
			}
		}

		private void ButtonDataRenewal_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonLockHome_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x04, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonUnlockHome_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x04, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonLockStatus_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x56, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadZPLock;
			this.SendBuf(b);
		}
		#endregion

		#region Motor
		private void ButtonPanLeft_Click(object sender, EventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x01, 0x03, 0xFF };

			if (CheckBoxMoveStop.Checked == true)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[4] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonPanRight_Click(object sender, EventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x02, 0x03, 0xFF };

			if (CheckBoxMoveStop.Checked == true)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[4] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonPanStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonGetAcceleration_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x06, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.MD_ReadAcceleration;
			this.SendBuf(b);
		}

		private void ButtonSetAcceleration_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x06, 0x01, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxAcceleration.Text == "")
				data = 100;
			else
				data = Convert.ToUInt16(this.TextBoxAcceleration.Text);

			b[4] = (byte)((data >> 12) & 0x0f);
			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonGetCurrentSpeed_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x06, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.MD_ReadSpeed;
			this.SendBuf(b);
		}

		private void ButtonSetTargetSpeed_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0xFF };

			data = Convert.ToUInt16(this.TextBoxTargetSpeed.Text);

			b[4] = (byte)((data >> 12) & 0x0f);
			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonCalibration_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x05, 0x01, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonReverseCalibration_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x05, 0x01, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonTiltUp_Click(object sender, EventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x01, 0xFF };

			if (CheckBoxMoveStop.Checked == true)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[5] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonTiltDown_Click(object sender, EventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x02, 0xFF };

			if (CheckBoxMoveStop.Checked == true)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[5] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonShowSpeed_Click(object sender, EventArgs e)
		{
			TimerUpdateSpeed.Enabled = true;
		}

		private void ButtonStopShowSpeed_Click(object sender, EventArgs e)
		{
			TimerUpdateSpeed.Enabled = false;
			G_eGeneralCommand = GeneralCommand.SYS_NONE;
		}

		private void ButtonClearShowSpeed_Click(object sender, EventArgs e)
		{
			ChartSpeed.Series[0].Points.Clear();

			Array.Clear(SpeedTimeArray, 0, SpeedTimeArray.Length);
			Array.Clear(SpeedArray, 0, SpeedArray.Length);

			TimerUpdateSpeedCnt = 0;
		}

		private void ButtonTiltUp_MouseDown(object sender, MouseEventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x01, 0xFF };

			if (CheckBoxMoveStop.Checked == false)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[5] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonTiltUp_MouseUp(object sender, MouseEventArgs e)
		{
			if (CheckBoxMoveStop.Checked == false)
				return;

			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonTiltDown_MouseDown(object sender, MouseEventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x02, 0xFF };

			if (CheckBoxMoveStop.Checked == false)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[5] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonTiltDown_MouseUp(object sender, MouseEventArgs e)
		{
			if (CheckBoxMoveStop.Checked == false)
				return;

			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonPanLeft_MouseDown(object sender, MouseEventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x01, 0x03, 0xFF };

			if (CheckBoxMoveStop.Checked == false)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[4] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonPanLeft_MouseUp(object sender, MouseEventArgs e)
		{
			if (CheckBoxMoveStop.Checked == false)
				return;

			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonPanRight_MouseDown(object sender, MouseEventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x02, 0x03, 0xFF };

			if (CheckBoxMoveStop.Checked == false)
				return;

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			//if (data > 127) data = 127;
			if (data < 1) data = 1;

			b[4] = data;

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonPanRight_MouseUp(object sender, MouseEventArgs e)
		{
			if (CheckBoxMoveStop.Checked == false)
				return;

			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonABS_Click(object sender, EventArgs e)
		{
			byte data;
			Int32 position;
			byte[] b = { 0x81, 0x01, 0x06, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };
			byte[] b2 = { 0x81, 0xD1, 0x06, 0x02, 0x00, 0xFF };

			if (this.ComboBoxPanMethod.SelectedIndex == -1)
			{
				this.ComboBoxPanMethod.SelectedIndex = 0;
				this.SendBuf(b2);
			}

			if (this.TextBoxABSPos.Text == "")
				this.TextBoxABSPos.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			position = Convert.ToInt32(this.TextBoxABSPos.Text);

			if (this.LabelMCUType.Text == "Pan")
			{
				b[4] = data;
				b[6] = (byte)((position >> 12) & 0x0f);
				b[7] = (byte)((position >> 8) & 0x0f);
				b[8] = (byte)((position >> 4) & 0x0f);
				b[9] = (byte)(position & 0x0f);
			}
			else if (this.LabelMCUType.Text == "Tilt")
			{
				b[5] = data;
				b[10] = (byte)((position >> 12) & 0x0f);
				b[11] = (byte)((position >> 8) & 0x0f);
				b[12] = (byte)((position >> 4) & 0x0f);
				b[13] = (byte)(position & 0x0f);
			}

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonPanType_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x06, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.MD_ReadPanType;
			this.SendBuf(b);
		}

		private void ComboBoxPanMethod_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x06, 0x02, 0x00, 0xFF };

			if (this.ComboBoxPanMethod.SelectedIndex == -1) this.ComboBoxPanMethod.SelectedIndex = 0;

			b[4] = (byte)(this.ComboBoxPanMethod.SelectedIndex & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonGetPosition_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x09, 0x06, 0x12, 0xFF };
			G_eGeneralCommand = GeneralCommand.MD_ReadMotorPosition;
			this.SendBuf(b);
		}

		private void TextBoxSpeedLevel_TextChanged(object sender, EventArgs e)
		{
			byte data;
			byte[] b = { 0x81, 0xD9, 0x06, 0x04, 0x00, 0xFF };

			if (this.TextBoxSpeedLevel.Text == "")
				return;

			data = Convert.ToByte(this.TextBoxSpeedLevel.Text);
			//if (data > 0x7F) data = 0x7F;
			if (data < 0x01) data = 0x01;

			G_eGeneralCommand = GeneralCommand.MD_ReadMotorSpeedInPPS;
			b[4] = data;

			this.SendBuf(b);
		}

		private void ButtonClearZPCali_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x05, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonHome_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x04, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonGetAngle_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x51, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadAngle;
			this.SendBuf(b);
		}

		private void ButtonABCount_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x52, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadAB;
			this.SendBuf(b);
		}

		private void ButtonZCount_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x53, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadZ;
			this.SendBuf(b);
		}

		private void ButtonZPCaliStatus_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x55, 0xFF };
			G_eGeneralCommand = GeneralCommand.AS_ReadZPCalibration;
			this.SendBuf(b);
		}

		private void ButtonABS2_Click(object sender, EventArgs e)
		{
			byte data;
			Int32 position;
			byte[] b = { 0x81, 0x01, 0x06, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };
			byte[] b2 = { 0x81, 0xD1, 0x06, 0x02, 0x00, 0xFF };

			if (this.ComboBoxPanMethod.SelectedIndex == -1)
			{
				this.ComboBoxPanMethod.SelectedIndex = 0;
				this.SendBuf(b2);
			}

			if (this.TextBoxABS2Pos.Text == "")
				this.TextBoxABS2Pos.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			position = Convert.ToInt32(this.TextBoxABS2Pos.Text);

			if (this.LabelMCUType.Text == "Pan")
			{
				b[4] = data;
				b[6] = (byte)((position >> 12) & 0x0f);
				b[7] = (byte)((position >> 8) & 0x0f);
				b[8] = (byte)((position >> 4) & 0x0f);
				b[9] = (byte)(position & 0x0f);
			}
			else if (this.LabelMCUType.Text == "Tilt")
			{
				b[5] = data;
				b[10] = (byte)((position >> 12) & 0x0f);
				b[11] = (byte)((position >> 8) & 0x0f);
				b[12] = (byte)((position >> 4) & 0x0f);
				b[13] = (byte)(position & 0x0f);
			}

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonABSStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x02, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonSetAccLevel_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x31, 0x00, 0xFF };

			if (this.ComboBoxAccLevel.SelectedIndex == -1) this.ComboBoxPanMethod.SelectedIndex = 1;

			b[4] = (byte)((this.ComboBoxAccLevel.SelectedIndex & 0x0f) + 1);

			this.SendBuf(b);
		}

		private void ButtonGetAccLevel_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x09, 0x06, 0x31, 0xFF };
			G_eGeneralCommand = GeneralCommand.MD_ReadMotoeAccLevel;
			this.SendBuf(b);
		}

		private void ButtonStopAt_Click(object sender, EventArgs e)
		{
			Int32 position;
			byte[] b = { 0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxStopAt.Text == "") this.TextBoxStopAt.Text = "0";

			position = Convert.ToInt32(this.TextBoxStopAt.Text);

			if (this.LabelMCUType.Text == "Pan")
			{
				b[8] = (byte)((position >> 12) & 0x0f);
				b[9] = (byte)((position >> 8) & 0x0f);
				b[10] = (byte)((position >> 4) & 0x0f);
				b[11] = (byte)(position & 0x0f);
			}
			else if (this.LabelMCUType.Text == "Tilt")
			{
				b[12] = (byte)((position >> 12) & 0x0f);
				b[13] = (byte)((position >> 8) & 0x0f);
				b[14] = (byte)((position >> 4) & 0x0f);
				b[15] = (byte)(position & 0x0f);
			}

			this.SendBuf(b);
		}

		private void ButtonGetSpeedByZoomRatio_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x09, 0x06, 0xA2, 0xFF };
			G_eGeneralCommand = GeneralCommand.MD_ReadSpeedByZoomRatio;
			this.SendBuf(b);
		}

		private void ButtonSpeedByZoomOn_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0xA2, 0x02, 0x00, 0xFF };

			if (this.TextBoxSpeedByZoomRatio.Text == "")
				this.TextBoxSpeedByZoomRatio.Text = "1";

			b[5] = Convert.ToByte(this.TextBoxSpeedByZoomRatio.Text);

			this.SendBuf(b);
		}

		private void ButtonSpeedByZoomOff_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0xA2, 0x03, 0xFF };

			this.TextBoxSpeedByZoomRatio.Text = "1";
			this.SendBuf(b);
		}

		private void ButtonRelStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x03, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonRelLeft_Click(object sender, EventArgs e)
		{
			byte data;
			UInt16 step;
			byte[] b = { 0x81, 0x01, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxRelStep.Text == "")
				this.TextBoxRelStep.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			step = Convert.ToUInt16(this.TextBoxRelStep.Text);

			b[4] = data;
			b[6] = 0x00;
			b[7] = (byte)((step >> 12) & 0x0f);
			b[8] = (byte)((step >> 8) & 0x0f);
			b[9] = (byte)((step >> 4) & 0x0f);
			b[10] = (byte)(step & 0x0f);

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonRelRight_Click(object sender, EventArgs e)
		{
			byte data;
			UInt16 step;
			byte[] b = { 0x81, 0x01, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxRelStep.Text == "")
				this.TextBoxRelStep.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			step = Convert.ToUInt16(this.TextBoxRelStep.Text);

			b[4] = data;
			b[6] = 0x01;
			b[7] = (byte)((step >> 12) & 0x0f);
			b[8] = (byte)((step >> 8) & 0x0f);
			b[9] = (byte)((step >> 4) & 0x0f);
			b[10] = (byte)(step & 0x0f);

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonRelUp_Click(object sender, EventArgs e)
		{
			byte data;
			UInt16 step;
			byte[] b = { 0x81, 0x01, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxRelStep.Text == "")
				this.TextBoxRelStep.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			step = Convert.ToUInt16(this.TextBoxRelStep.Text);

			b[5] = data;
			b[11] = 0x00;
			b[12] = (byte)((step >> 12) & 0x0f);
			b[13] = (byte)((step >> 8) & 0x0f);
			b[14] = (byte)((step >> 4) & 0x0f);
			b[15] = (byte)(step & 0x0f);

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonRelDown_Click(object sender, EventArgs e)
		{
			byte data;
			UInt16 step;
			byte[] b = { 0x81, 0x01, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxRelStep.Text == "")
				this.TextBoxRelStep.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			step = Convert.ToUInt16(this.TextBoxRelStep.Text);

			b[5] = data;
			b[11] = 0x01;
			b[12] = (byte)((step >> 12) & 0x0f);
			b[13] = (byte)((step >> 8) & 0x0f);
			b[14] = (byte)((step >> 4) & 0x0f);
			b[15] = (byte)(step & 0x0f);

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonImageFlipMaxAngleOn_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x66, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonImageFlipMaxAngleOff_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x66, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonABSAngle_Click(object sender, EventArgs e)
		{
			byte data;
			Int32 position;
			byte[] b = { 0x81, 0x01, 0x06, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };
			byte[] b2 = { 0x81, 0xD1, 0x06, 0x02, 0x00, 0xFF };

			if (this.ComboBoxPanMethod.SelectedIndex == -1)
			{
				this.ComboBoxPanMethod.SelectedIndex = 0;
				this.SendBuf(b2);
			}

			if (this.TextBoxABSAngle.Text == "")
				this.TextBoxABSAngle.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			position = Convert.ToInt32(this.TextBoxABSAngle.Text);

			if (this.LabelMCUType.Text == "Pan")
			{
				b[4] = data;
				b[6] = (byte)((position >> 12) & 0x0f);
				b[7] = (byte)((position >> 8) & 0x0f);
				b[8] = (byte)((position >> 4) & 0x0f);
				b[9] = (byte)(position & 0x0f);
			}
			else if (this.LabelMCUType.Text == "Tilt")
			{
				b[5] = data;
				b[10] = (byte)((position >> 12) & 0x0f);
				b[11] = (byte)((position >> 8) & 0x0f);
				b[12] = (byte)((position >> 4) & 0x0f);
				b[13] = (byte)(position & 0x0f);
			}

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonABSAngle2_Click(object sender, EventArgs e)
		{
			byte data;
			Int32 position;
			byte[] b = { 0x81, 0x01, 0x06, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };
			byte[] b2 = { 0x81, 0xD1, 0x06, 0x02, 0x00, 0xFF };

			if (this.ComboBoxPanMethod.SelectedIndex == -1)
			{
				this.ComboBoxPanMethod.SelectedIndex = 0;
				this.SendBuf(b2);
			}

			if (this.TextBoxABSAngle2.Text == "")
				this.TextBoxABSAngle2.Text = "0";

			if (this.TextBoxSpeedLevel.Text == "")
				data = DEFAULT_SPEED_LEVEL;
			else
				data = Convert.ToByte(this.TextBoxSpeedLevel.Text);

			if (data > 200) data = 200;
			if (data < 1) data = 1;

			position = Convert.ToInt32(this.TextBoxABSAngle2.Text);

			if (this.LabelMCUType.Text == "Pan")
			{
				b[4] = data;
				b[6] = (byte)((position >> 12) & 0x0f);
				b[7] = (byte)((position >> 8) & 0x0f);
				b[8] = (byte)((position >> 4) & 0x0f);
				b[9] = (byte)(position & 0x0f);
			}
			else if (this.LabelMCUType.Text == "Tilt")
			{
				b[5] = data;
				b[10] = (byte)((position >> 12) & 0x0f);
				b[11] = (byte)((position >> 8) & 0x0f);
				b[12] = (byte)((position >> 4) & 0x0f);
				b[13] = (byte)(position & 0x0f);
			}

			this.SendBuf(b);
			this.TextBoxSpeedLevel.Text = data.ToString();
		}

		private void ButtonABSAngleStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x06, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonMotorType0p9d_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x00, 0x03, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonMotorType1p8d_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x00, 0x03, 0x01, 0xFF };
			this.SendBuf(b);
		}
		#endregion

		#region LightSensor
		private void ButtonLSGetLux_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x08, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.LS_ReadLux;
			this.SendBuf(b);
		}

		private void ButtonLSGetCfg_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x08, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.LS_ReadConfiguration;
			this.SendBuf(b);
		}

		private void ButtonSetCfg_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0xFF };

			b[4] = Convert.ToByte(this.TextBoxLSRN.Text);
			b[5] = (byte)(((Convert.ToByte(this.TextBoxLSCT.Text) & 0x1) << 3) |
					((Convert.ToByte(this.TextBoxLSM.Text) & 0x3) << 1));
			b[6] = Convert.ToByte(this.TextBoxLSL.Text);
			b[7] = (byte)(((Convert.ToByte(this.TextBoxLSPOL.Text) & 0x1) << 3) |
					((Convert.ToByte(this.TextBoxLSME.Text) & 0x1) << 2) |
					((Convert.ToByte(this.TextBoxLSFC.Text) & 0x3) << 0));

			this.SendBuf(b);
		}

		private void ButtonGetLSLimit_Click(object sender, EventArgs e)
		{
			if (CheckBoxLSHighLimit.Checked == true)
			{
				byte[] b = { 0x81, 0xD9, 0x08, 0x03, 0xFF };
				G_eGeneralCommand = GeneralCommand.LS_ReadHighLimit;
				this.SendBuf(b);
			}
			else
			{
				CheckBoxLSLowLimit.Checked = true;

				byte[] b = { 0x81, 0xD9, 0x08, 0x02, 0xFF };
				G_eGeneralCommand = GeneralCommand.LS_ReadLowLimit;
				this.SendBuf(b);
			}
		}

		private void ButtonSetLSLimit_Click(object sender, EventArgs e)
		{
			byte ucELimit;
			UInt16 usRLimit;

			ucELimit = Convert.ToByte(this.TextBoxLSELimit.Text);
			usRLimit = Convert.ToUInt16(this.TextBoxLSRLimit.Text);

			//--- boundary protection for E
			if (ucELimit > 0xF)
			{
				ucELimit = 0xF;
				this.TextBoxLSELimit.Text = ucELimit.ToString();
			}

			//--- boundary protection for R
			if (usRLimit > 0xFFF)
			{
				usRLimit = 0xFFF;
				this.TextBoxLSRLimit.Text = usRLimit.ToString();
			}

			if (CheckBoxLSHighLimit.Checked == true)
			{
				byte[] b = { 0x81, 0xD1, 0x08, 0x03, 0x00, 0x00, 0x00, 0x00, 0xFF };
				b[4] = ucELimit;
				b[5] = (byte)((usRLimit & 0xF00) >> 8);
				b[6] = (byte)((usRLimit & 0x0F0) >> 4);
				b[7] = (byte)((usRLimit & 0x00F) >> 0);

				this.TextBoxLSLuxLimit.Text = (0.01 * (1 << ucELimit) * usRLimit).ToString();
				this.SendBuf(b);
			}
			else
			{
				CheckBoxLSLowLimit.Checked = true;

				byte[] b = { 0x81, 0xD1, 0x08, 0x02, 0x00, 0x00, 0x00, 0x00, 0xFF };
				b[4] = ucELimit;
				b[5] = (byte)((usRLimit & 0xF00) >> 8);
				b[6] = (byte)((usRLimit & 0x0F0) >> 4);
				b[7] = (byte)((usRLimit & 0x00F) >> 0);

				this.TextBoxLSLuxLimit.Text = (0.01 * (1 << ucELimit) * usRLimit).ToString();
				this.SendBuf(b);
			}
		}

		private void CheckBoxLSLowLimit_CheckedChanged(object sender, EventArgs e)
		{
			if (CheckBoxLSLowLimit.Checked == true)
				CheckBoxLSHighLimit.Checked = false;
		}

		private void CheckBoxLSHighLimit_CheckedChanged(object sender, EventArgs e)
		{
			if (CheckBoxLSHighLimit.Checked == true)
				CheckBoxLSLowLimit.Checked = false;
		}

		private void ButtonGetLSID_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x08, 0x7E, 0xFF };
			G_eGeneralCommand = GeneralCommand.LS_ReadManufacturerID;
			this.SendBuf(b);
		}

		private void ButtonLSDID_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x08, 0x7F, 0xFF };
			G_eGeneralCommand = GeneralCommand.LS_ReadDeviceID;
			this.SendBuf(b);
		}
#endregion

#region Alarm
		private void ButtonAlarmOn_Click(object sender, EventArgs e)
		{
			byte[] b1 = { 0x81, 0xD1, 0x09, 0x01, 0x02, 0xFF };
			byte[] b2 = { 0x81, 0xD1, 0x09, 0x03, 0x03, 0xFF };

			if (this.ComboBoxAlarmTrigLvl.SelectedIndex == -1)
			{
				this.ComboBoxAlarmTrigLvl.SelectedIndex = 0;
				this.SendBuf(b2);
			}

			this.SendBuf(b1);
		}

		private void ButtonAlarmOff_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x09, 0x01, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ComboBoxAlarmTrigLvl_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x09, 0x03, 0x00, 0xFF };

			if (this.ComboBoxAlarmTrigLvl.SelectedIndex == -1)
				this.ComboBoxAlarmTrigLvl.SelectedIndex = 0;

			b[4] = (byte)((this.ComboBoxAlarmTrigLvl.SelectedIndex == 1) ? 0x02 : 0x03);
			this.SendBuf(b);
		}

		private void ButtonAlarmOutHigh_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x09, 0x02, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonAlarmOutLow_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x09, 0x02, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonAlarmAutoStatus_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x09, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.ALM_ReadStatus;
			this.SendBuf(b);
		}

		private void ButtonAlarmIn_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x09, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.ALM_ReadAlarmIn;
			this.SendBuf(b);
		}

		private void ButtonAlarmTrigLvl_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x09, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.ALM_ReadTrigLvl;
			this.SendBuf(b);
		}

#endregion

#region NTC
		private void ButtonNTCSingleScan_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0B, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonGetNTC1_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0B, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.NTC_ReadNTC1;
			this.SendBuf(b);
		}

		private void ButtonGetNTC2_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0B, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.NTC_ReadNTC2;
			this.SendBuf(b);
		}
#endregion

#region RS485
		private void ButtonRS485GetMode_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadMode;
			this.SendBuf(b);
		}

		private void ComboBoxRS485Mode_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x01, 0x00, 0xFF };

			if (this.ComboBoxRS485Mode.SelectedIndex == 0)
				b[4] = 0x02;
			else if (this.ComboBoxRS485Mode.SelectedIndex == 1)
				b[4] = 0x03;

			this.SendBuf(b);
		}

		private void ButtonRS485GetTermR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadTermR;
			this.SendBuf(b);
		}

		private void ComboBoxRS485TermR_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x02, 0x00, 0xFF };

			if (this.ComboBoxRS485TermR.SelectedIndex == 0)
				b[4] = 0x03;
			else if (this.ComboBoxRS485TermR.SelectedIndex == 1)
				b[4] = 0x02;

			this.SendBuf(b);
		}

		private void ButtonRS485TestTx_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x0A, 0x00, 0xFF };

			if (TextBoxRS485TestTx.Text == "")
				TextBoxRS485TestTx.Text = "0";
			b[4] = Convert.ToByte(TextBoxRS485TestTx.Text);

			this.SendBuf(b);
		}

		private void ButtonRS485TestRx_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x00 };

			if (TextBoxRS485TestRx.Text == "")
				TextBoxRS485TestRx.Text = "0";
			b[0] = Convert.ToByte(TextBoxRS485TestRx.Text);

			this.SendBuf(b);
		}

		private void ButtonRS485GetComm_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadBaudRate;
			this.SendBuf(b);
		}

		private void GetRS485StopBits()
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadStopBits;
			this.SendBuf(b);
		}

		private void ComboBoxRS485BaudRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x03, 0x00, 0xFF };
			b[4] = (byte)(this.ComboBoxRS485BaudRate.SelectedIndex + 1);
			this.SendBuf(b);
		}

		private void ComboBoxRS485StopBits_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x04, 0x00, 0xFF };
			b[4] = (byte)(this.ComboBoxRS485StopBits.SelectedIndex + 1);
			this.SendBuf(b);
		}

		private void ButtonRS485DI_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x07, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadDI;
			this.SendBuf(b);
		}

		private void ButtonRS485DO_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x0A, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadDO;
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO1_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x01, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO1.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO1.Text = ((this.CheckBoxRS485DO1.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO2_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x02, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO2.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO2.Text = ((this.CheckBoxRS485DO2.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO3_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x03, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO3.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO3.Text = ((this.CheckBoxRS485DO3.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO4_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x04, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO4.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO4.Text = ((this.CheckBoxRS485DO4.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO5_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x05, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO5.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO5.Text = ((this.CheckBoxRS485DO5.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO6_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x06, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO6.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO6.Text = ((this.CheckBoxRS485DO6.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO7_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x07, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO7.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO7.Text = ((this.CheckBoxRS485DO7.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void CheckBoxRS485DO8_CheckedChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x07, 0x08, 0x00, 0xFF };
			b[5] = (byte)((this.CheckBoxRS485DO8.Checked == false) ? 0x00 : 0x01);
			this.CheckBoxRS485DO8.Text = ((this.CheckBoxRS485DO8.Checked == false) ? "Off" : "On");
			this.SendBuf(b);
		}

		private void ButtonSD700SetAddr_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x06, 0x00, 0xFF };

			if (this.TextBoxSD700Addr.Text == "")
				this.TextBoxSD700Addr.Text = "0";

			b[4] = Convert.ToByte(this.TextBoxSD700Addr.Text);
			this.SendBuf(b);
		}

		private void ComboBoxSD700AutoScan_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x08, 0x00, 0xFF };

			if (this.ComboBoxSD700AutoScan.SelectedIndex == 0)
				b[4] = 0x03;
			else if (this.ComboBoxSD700AutoScan.SelectedIndex == 1)
				b[4] = 0x02;

			this.SendBuf(b);
		}

		private void ComboBoxSD700TrigLvl_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x09, 0x00, 0xFF };

			if (this.ComboBoxSD700TrigLvl.SelectedIndex == 0)
				b[4] = 0x03;
			else if (this.ComboBoxSD700TrigLvl.SelectedIndex == 1)
				b[4] = 0x02;

			this.SendBuf(b);
		}

		private void ButtonRS485GetDev_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadRS485Dev;
			this.SendBuf(b);
		}

		private void ComboBoxRS485Dev_SelectedIndexChanged(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x0A, 0x05, 0x00, 0xFF };
			b[4] = (byte)this.ComboBoxRS485Dev.SelectedIndex;
			this.SendBuf(b);
		}

		private void ButtonRS485GetAddr_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x0A, 0x06, 0xFF };
			G_eGeneralCommand = GeneralCommand.RS_ReadRS485DevAddr;
			this.SendBuf(b);
		}
#endregion

#region TLE5012B
		private void ButtonTleGetS_RST_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x00, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_S_RST;
			this.SendBuf(b);
		}

		private void ButtonTleGetError_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x00, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_Error;
			this.SendBuf(b);

		}

		private void ButtonTleGetRD_ST_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x00, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_RD_ST;
			this.SendBuf(b);
		}

		private void ButtonTleGetSlave_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x00, 0x0D, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_S_NR;
			this.SendBuf(b);
		}

		private void ComboBoxTleStatS_NR_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleStatS_NR.SelectedIndex == -1)
				return;
			else
				b[4] = (byte)this.ComboBoxTleStatS_NR.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_RST_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_RST;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_RST_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleAS_RST.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_RST.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_WD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_WD;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_WD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x01, 0x00, 0xFF };

			if (this.ComboBoxTleAS_WD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_WD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_VR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_VR;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_VR_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x02, 0x00, 0xFF };

			if (this.ComboBoxTleAS_VR.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_VR.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAcstatAS_FUSE_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_FUSE;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_FUSE_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x03, 0x00, 0xFF };

			if (this.ComboBoxTleAS_FUSE.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_FUSE.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_DSPU_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_DSPU;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_DSPU_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x04, 0x00, 0xFF };

			if (this.ComboBoxTleAS_DSPU.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_DSPU.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_OV_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_OV;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_OV_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x05, 0x00, 0xFF };

			if (this.ComboBoxTleAS_OV.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_OV.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_VEC_XY_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x06, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_VEC_XY;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_VEC_XY_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x06, 0x00, 0xFF };

			if (this.ComboBoxTleAS_VEC_XY.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_VEC_XY.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_VEC_MAG_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x07, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_VEC_MAG;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_VEC_MAG_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x07, 0x00, 0xFF };

			if (this.ComboBoxTleAS_VEC_MAG.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_VEC_MAG.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_ADCT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_ADCT;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_ADCT_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x09, 0x00, 0xFF };

			if (this.ComboBoxTleAS_ADCT.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_ADCT.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAS_FRST_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x01, 0x0A, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AS_FRST;
			this.SendBuf(b);
		}

		private void ComboBoxTleAS_FRST_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x01, 0x0A, 0x00, 0xFF };

			if (this.ComboBoxTleAS_FRST.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAS_FRST.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetANG_VAL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x02, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ANG_VAL;
			this.SendBuf(b);
		}

		private void ButtonTleGetRD_AV_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x02, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_RD_AV;
			this.SendBuf(b);
		}

		private void ButtonTleGetANG_SPD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x03, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ANG_SPD;
			this.SendBuf(b);
		}

		private void ButtonTleGetRD_AS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x03, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_RD_AS;
			this.SendBuf(b);
		}

		private void ButtonTleGetRevol_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x04, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_REVOL;
			this.SendBuf(b);
		}

		private void ButtonTleGetFCNT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x04, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FCNT;
			this.SendBuf(b);
		}

		private void ButtonTleGetRD_REV_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x04, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_RD_REV;
			this.SendBuf(b);
		}

		private void ButtonTleGetTEMPER_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x05, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_TEMPER;
			this.SendBuf(b);
		}

		private void ButtonTleGetFSYNC_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x05, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FSYNC;
			this.SendBuf(b);
		}

		private void ButtonTleGetIIF_MOD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x06, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_IIF_MOD;
			this.SendBuf(b);
		}

		private void ButtonTleGetDSPU_HOLD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x06, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_DSPU_HOLD;
			this.SendBuf(b);
		}

		private void ButtonTleGetCLK_SEL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x06, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_CLK_SEL;
			this.SendBuf(b);
		}

		private void ButtonTleGetFIR_MD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x06, 0x0E, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FIR_MD;
			this.SendBuf(b);
		}

		private void ComboBoxTleIIF_MOD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x06, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleIIF_MOD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleIIF_MOD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleDSPU_HOLD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x06, 0x02, 0x00, 0xFF };

			if (this.ComboBoxTleDSPU_HOLD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleDSPU_HOLD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleCLK_SEL_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x06, 0x04, 0x00, 0xFF };

			if (this.ComboBoxTleCLK_SEL.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleCLK_SEL.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleFIR_MD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x06, 0x0E, 0x00, 0xFF };

			if ((this.ComboBoxTleFIR_MD.SelectedIndex == -1) ||
				(this.ComboBoxTleFIR_MD.SelectedIndex == 0))
				return;
			else
				b[5] = (byte)this.ComboBoxTleFIR_MD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetADCTV_EN_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0x06, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ADCTV_EN;
			this.SendBuf(b);
		}

		private void ButtonTleGetADCTV_X_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ADCTV_X;
			this.SendBuf(b);
		}

		private void ButtonTleGetADCTV_Y_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ADCTV_Y;
			this.SendBuf(b);
		}

		private void ButtonTleGetFUSE_REL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0x0A, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FUSE_REL;
			this.SendBuf(b);
		}

		private void ButtonTleGetFILT_INV_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0x0E, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FILT_INV;
			this.SendBuf(b);
		}

		private void ButtonTleGetFILT_PAR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x07, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FILT_PAR;
			this.SendBuf(b);
		}

		private void ComboBoxTleADCTV_EN_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x07, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleADCTV_EN.SelectedIndex == -1)
				return;
			else
			{
				if (this.ComboBoxTleADCTV_X.SelectedIndex == -1)
					this.ComboBoxTleADCTV_X.SelectedIndex = 0;

				if (this.ComboBoxTleADCTV_Y.SelectedIndex == -1)
					this.ComboBoxTleADCTV_Y.SelectedIndex = 0;

				b[5] = (byte)this.ComboBoxTleADCTV_X.SelectedIndex;
				b[6] = (byte)this.ComboBoxTleADCTV_Y.SelectedIndex;
				b[7] = (byte)this.ComboBoxTleADCTV_EN.SelectedIndex;
			}

			this.SendBuf(b);
		}

		private void ComboBoxTleFUSE_REL_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x07, 0x0A, 0x00, 0xFF };

			if (this.ComboBoxTleFUSE_REL.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleFUSE_REL.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleFILT_INV_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x07, 0x0E, 0x00, 0xFF };

			if (this.ComboBoxTleFILT_INV.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleFILT_INV.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleFILT_PAR_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x07, 0x0F, 0x00, 0xFF };

			if (this.ComboBoxTleFILT_PAR.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleFILT_PAR.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleGetAUTOCAL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x08, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_AUTOCAL;
			this.SendBuf(b);
		}

		private void ButtonTleGetPREDICT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x08, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_PREDICT;
			this.SendBuf(b);
		}

		private void ButtonTleGetANG_DIR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x08, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ANG_DIR;
			this.SendBuf(b);
		}

		private void ButtonTleGetANG_RANGE_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x08, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ANG_RANGE;
			this.SendBuf(b);
		}

		private void ComboBoxTleAUTOCAL_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x08, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleAUTOCAL.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleAUTOCAL.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTlePREDICT_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x08, 0x02, 0x00, 0xFF };

			if (this.ComboBoxTlePREDICT.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTlePREDICT.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleANG_DIR_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x08, 0x03, 0x00, 0xFF };

			if (this.ComboBoxTleANG_DIR.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleANG_DIR.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleSetANGLE_RANGE_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x08, 0x04, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleANG_RANGE.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleANG_RANGE.Text);

			if (data > 0xFFF)
			{
				data = 0xFFF;
				TextBoxTleANG_RANGE.Text = data.ToString();
			}

			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetPAD_DRV_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x09, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_PAD_DRV;
			this.SendBuf(b);
		}

		private void ButtonTleGetSSC_OD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x09, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_SSC_OD;
			this.SendBuf(b);
		}

		private void ButtonTleGetSPIKEF_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x09, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_SPIKEF;
			this.SendBuf(b);
		}

		private void ButtonTleGetANG_BASE_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x09, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ANG_BASE;
			this.SendBuf(b);
		}

		private void ComboBoxTlePAD_DRV_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x09, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTlePAD_DRV.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTlePAD_DRV.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleSSC_OD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x09, 0x02, 0x00, 0xFF };

			if (this.ComboBoxTleSSC_OD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleSSC_OD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxSPIKEF_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x09, 0x03, 0x00, 0xFF };

			if (this.ComboBoxTleSPIKEF.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleSPIKEF.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleSetANG_BASE_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x09, 0x04, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleANG_BASE.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleANG_BASE.Text);

			if (data > 0xFFF)
			{
				data = 0xFFF;
				TextBoxTleANG_BASE.Text = data.ToString();
			}

			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetOFFSET_X_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0A, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_OFFX;
			this.SendBuf(b);
		}

		private void ButtonTleSetOFFSET_X_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0A, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleOFFX.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleOFFX.Text);

			if (data > 0xFFF)
			{
				data = 0xFFF;
				TextBoxTleOFFX.Text = data.ToString();
			}

			b[4] = (byte)((data >> 8) & 0x0f);
			b[5] = (byte)((data >> 4) & 0x0f);
			b[6] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetOFFSET_Y_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0B, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_OFFY;
			this.SendBuf(b);
		}

		private void ButtonTleSetOFFSET_Y_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0B, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleOFFY.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleOFFY.Text);

			if (data > 0xFFF)
			{
				data = 0xFFF;
				TextBoxTleOFFY.Text = data.ToString();
			}

			b[4] = (byte)((data >> 8) & 0x0f);
			b[5] = (byte)((data >> 4) & 0x0f);
			b[6] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetSYNCH_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0C, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_SYNCH;
			this.SendBuf(b);
		}

		private void ButtonTleSetSYNCH_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0C, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleSYNCH.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleSYNCH.Text);

			if (data > 0xFFF)
			{
				data = 0xFFF;
				TextBoxTleSYNCH.Text = data.ToString();
			}

			b[4] = (byte)((data >> 8) & 0x0f);
			b[5] = (byte)((data >> 4) & 0x0f);
			b[6] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetIFAB_HYST_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0D, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_IFAB_HYST;
			this.SendBuf(b);
		}

		private void ButtonTleGetIFAB_OD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0D, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_IFAB_OD;
			this.SendBuf(b);
		}

		private void ButtonTleGetFIR_UDR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0D, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_FIR_UDR;
			this.SendBuf(b);
		}

		private void ButtonTleGetORTHO_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0D, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ORTHO;
			this.SendBuf(b);
		}

		private void ComboBoxTleIFAB_HYST_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0D, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleIFAB_HYST.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleIFAB_HYST.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleIFAB_OD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0D, 0x02, 0x00, 0xFF };

			if (this.ComboBoxTleIFAB_OD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleIFAB_OD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleFIR_UDR_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0D, 0x03, 0x00, 0xFF };

			if (this.ComboBoxTleFIR_UDR.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleFIR_UDR.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleSetORTHO_Click(object sender, EventArgs e)
		{
			Int16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0D, 0x04, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleORTHO.Text == "")
				return;

			data = Convert.ToInt16(this.TextBoxTleORTHO.Text);

			if ((data > 2047) || (data < -2048))
				return;

			if (data < 0)
				data = (Int16)(data + 4096);

			b[5] = (byte)((data >> 8) & 0x0f);
			b[6] = (byte)((data >> 4) & 0x0f);
			b[7] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetIF_MD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0E, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_IF_MD;
			this.SendBuf(b);
		}

		private void ButtonTleGetIFAB_RES_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0E, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_IFAB_RES;
			this.SendBuf(b);
		}

		private void ButtonTleGetHSM_PLP_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0E, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_HSM_PLP;
			this.SendBuf(b);
		}

		private void ButtonTleGetTCO_X_T_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0E, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_TCO_X_T;
			this.SendBuf(b);
		}

		private void ComboBoxTleIF_MD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0E, 0x00, 0x00, 0xFF };

			if (this.ComboBoxTleIF_MD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleIF_MD.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleIFAB_RES_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0E, 0x03, 0x00, 0xFF };

			if (this.ComboBoxTleIFAB_RES.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleIFAB_RES.SelectedIndex;

			this.SendBuf(b);
		}

		private void ComboBoxTleHSM_PLP_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0E, 0x05, 0x00, 0xFF };

			if (this.ComboBoxTleHSM_PLP.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleHSM_PLP.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleSetTCO_X_T_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0E, 0x09, 0x00, 0x00, 0xFF };

			if (TextBoxTleTCO_X_T.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleTCO_X_T.Text);

			if (data > 0x7F)
				return;

			b[5] = (byte)((data >> 4) & 0x0f);
			b[6] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetCRC_PAR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0F, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_CRC_PAR;
			this.SendBuf(b);
		}

		private void ButtonTleGetSBIST_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0F, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_SBIST;
			this.SendBuf(b);
		}

		private void ButtonTleGetTCO_Y_T_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x0F, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_TCO_Y_T;
			this.SendBuf(b);
		}

		private void ButtonTleSetCRC_PAR_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0F, 0x00, 0x00, 0x00, 0xFF };

			if (TextBoxTleCRC_PAR.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleCRC_PAR.Text);

			if (data > 0xFF)
				return;

			b[5] = (byte)((data >> 4) & 0x0f);
			b[6] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ComboBoxTleSBIST_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x05, 0x0F, 0x08, 0x00, 0xFF };

			if (this.ComboBoxTleSBIST.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBoxTleSBIST.SelectedIndex;

			this.SendBuf(b);
		}

		private void ButtonTleSetTCO_Y_T_Click(object sender, EventArgs e)
		{
			UInt16 data;
			byte[] b = { 0x81, 0xD1, 0x05, 0x0F, 0x09, 0x00, 0x00, 0xFF };

			if (TextBoxTleTCO_Y_T.Text == "")
				return;

			data = Convert.ToUInt16(this.TextBoxTleTCO_Y_T.Text);

			if (data > 0x7F)
				return;

			b[5] = (byte)((data >> 4) & 0x0f);
			b[6] = (byte)(data & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonTleGetADC_X_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ADC_X;
			this.SendBuf(b);
		}

		private void ButtonTleGetADC_Y_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x11, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_ADC_Y;
			this.SendBuf(b);
		}

		private void ButtonTleGetMAG_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x14, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_MAG;
			this.SendBuf(b);
		}

		private void ButtonTleGetT_RAW_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x15, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_T_RAW;
			this.SendBuf(b);
		}

		private void ButtonTleGetT_TGL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x15, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_T_TGL;
			this.SendBuf(b);
		}

		private void ButtonTleGetIIF_CNT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x20, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_IIF_CNT;
			this.SendBuf(b);
		}

		private void ButtonTleGetT25O_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x05, 0x30, 0xFF };
			G_eGeneralCommand = GeneralCommand.TLE_Read_T25O;
			this.SendBuf(b);
		}
#endregion

#region TMC2209
		private void Button_I_scale_analog_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_I_scale_analog;
			this.SendBuf(b);
		}

		private void ComboBox_I_scale_analog_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x00, 0x00, 0xFF };

			if (this.ComboBox_I_scale_analog.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_I_scale_analog.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_internal_Rsense_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_internal_Rsense;
			this.SendBuf(b);
		}

		private void ComboBox_internal_Rsense_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x01, 0x00, 0xFF };

			if (this.ComboBox_internal_Rsense.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_internal_Rsense.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_en_SpreadCycle_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_en_SpreadCycle;
			this.SendBuf(b);
		}

		private void ComboBox_en_SpreadCycle_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x02, 0x00, 0xFF };

			if (this.ComboBox_en_SpreadCycle.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_en_SpreadCycle.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_shaft_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_shaft;
			this.SendBuf(b);
		}

		private void ComboBox_shaft_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x03, 0x00, 0xFF };

			if (this.ComboBox_shaft.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_shaft.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_index_otpw_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_index_otpw;
			this.SendBuf(b);
		}

		private void ComboBox_index_otpw_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x04, 0x00, 0xFF };

			if (this.ComboBox_index_otpw.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_index_otpw.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_index_step_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_index_step;
			this.SendBuf(b);
		}

		private void ComboBox_index_step_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x05, 0x00, 0xFF };

			if (this.ComboBox_index_step.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_index_step.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_pdn_disable_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x06, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_pdn_disable;
			this.SendBuf(b);
		}

		private void ComboBox_pdn_disable_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x06, 0x00, 0xFF };

			if (this.ComboBox_pdn_disable.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_pdn_disable.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_mstep_reg_sel_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x07, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_mstep_reg_select;
			this.SendBuf(b);
		}

		private void ComboBox_mstep_reg_sel_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x07, 0x00, 0xFF };

			if (this.ComboBox_mstep_reg_sel.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_mstep_reg_sel.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_multistep_filt_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_multistep_filt;
			this.SendBuf(b);
		}

		private void ComboBox_multistep_filt_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x08, 0x00, 0xFF };

			if (this.ComboBox_multistep_filt.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_multistep_filt.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_test_mode_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x00, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_test_mode;
			this.SendBuf(b);
		}

		private void ComboBox_test_mode_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x00, 0x09, 0x00, 0xFF };

			if (this.ComboBox_test_mode.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_test_mode.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_reset_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x01, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_reset;
			this.SendBuf(b);
		}

		private void ComboBox_reset_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x01, 0x00, 0x00, 0xFF };

			if (this.ComboBox_reset.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_reset.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_drv_err_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x01, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_drv_err;
			this.SendBuf(b);
		}

		private void ComboBox_drv_err_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x01, 0x01, 0x00, 0xFF };

			if (this.ComboBox_drv_err.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_drv_err.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_uv_cp_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x01, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_uv_cp;
			this.SendBuf(b);
		}

		private void ComboBox_uv_cp_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x01, 0x02, 0x00, 0xFF };

			if (this.ComboBox_uv_cp.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_uv_cp.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_IFCNT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x02, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_IFCNT;
			this.SendBuf(b);
		}

		private void Button_SLAVECONF_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x03, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_SLAVECONF;
			this.SendBuf(b);
		}

		private void ComboBox_SLAVECONF_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x03, 0x00, 0x00, 0xFF };

			if (this.ComboBox_SLAVECONF.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_SLAVECONF.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_OTP_READ_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x05, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_OTP_READ;
			this.SendBuf(b);
		}

		private void Button_ENN_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_ENN;
			this.SendBuf(b);
		}

		private void Button_MS1_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_MS1;
			this.SendBuf(b);
		}

		private void Button_MS2_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_MS2;
			this.SendBuf(b);
		}

		private void Button_DIAG_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_DIAG;
			this.SendBuf(b);
		}

		private void Button_PDN_UART_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x06, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PDN_UART;
			this.SendBuf(b);
		}

		private void Button_STEP_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x07, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_STEP;
			this.SendBuf(b);
		}

		private void Button_SPREAD_EN_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_SPREAD_EN;
			this.SendBuf(b);
		}

		private void Button_DIR_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_DIR;
			this.SendBuf(b);
		}

		private void Button_VERSION_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x06, 0x18, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_VERSION;
			this.SendBuf(b);
		}

		private void Button_FCLKTRIM_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x07, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_FCLKTRIM;
			this.SendBuf(b);
		}

		private void ComboBox_FCLKTRIM_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x07, 0x00, 0x00, 0x00, 0xFF };

			if (this.ComboBox_FCLKTRIM.SelectedIndex == -1)
				return;
			else
			{
				b[5] = (byte)((this.ComboBox_FCLKTRIM.SelectedIndex >> 4) & 0x0F);
				b[6] = (byte)(this.ComboBox_FCLKTRIM.SelectedIndex & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_OTTRIM_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x07, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_OTTRIM;
			this.SendBuf(b);
		}

		private void ComboBox_OTTRIM_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x07, 0x08, 0x00, 0xFF };

			if (this.ComboBox_OTTRIM.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_OTTRIM.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_IHOLD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x10, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_IHOLD;
			this.SendBuf(b);
		}

		private void ComboBox_IHOLD_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x10, 0x00, 0x00, 0xFF };

			if (this.ComboBox_IHOLD.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_IHOLD.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_IRUN_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x10, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_IRUN;
			this.SendBuf(b);
		}

		private void ComboBox_IRUN_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x10, 0x08, 0x00, 0xFF };

			if (this.ComboBox_IRUN.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_IRUN.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_IHOLDDELAY_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x10, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_IHOLDDELAY;
			this.SendBuf(b);
		}

		private void ComboBox_IHOLDDELAY_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x10, 0x10, 0x00, 0xFF };

			if (this.ComboBox_IHOLDDELAY.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_IHOLDDELAY.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_TPOWERDOWN_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x11, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_TPOWERDOWN;
			this.SendBuf(b);
		}

		private void TextBox_TPOWERDOWN_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x11, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_TPOWERDOWN.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_TPOWERDOWN.Text) >> 4) & 0x0F);
				b[6] = (byte)(Convert.ToByte(this.TextBox_TPOWERDOWN.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_TSTEP_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x12, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_TSTEP;
			this.SendBuf(b);
		}

		private void Button_TPWMTHRS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x13, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_TPWMTHRS;
			this.SendBuf(b);
		}

		private void TextBox_TPWMTHRS_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_TPWMTHRS.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_TPWMTHRS.Text) >> 20) & 0x0F);
				b[6] = (byte)((Convert.ToByte(this.TextBox_TPWMTHRS.Text) >> 16) & 0x0F);
				b[7] = (byte)((Convert.ToByte(this.TextBox_TPWMTHRS.Text) >> 12) & 0x0F);
				b[8] = (byte)((Convert.ToByte(this.TextBox_TPWMTHRS.Text) >> 8) & 0x0F);
				b[9] = (byte)((Convert.ToByte(this.TextBox_TPWMTHRS.Text) >> 4) & 0x0F);
				b[10] = (byte)(Convert.ToByte(this.TextBox_TPWMTHRS.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_VACTUAL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x22, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_VACTUAL;
			this.SendBuf(b);
		}

		private void TextBox_VACTUAL_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_VACTUAL.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 20) & 0x0F);
				b[6] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 16) & 0x0F);
				b[7] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 12) & 0x0F);
				b[8] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 8) & 0x0F);
				b[9] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 4) & 0x0F);
				b[10] = (byte)(Convert.ToByte(this.TextBox_VACTUAL.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_VACTUAL_set_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_VACTUAL.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 20) & 0x0F);
				b[6] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 16) & 0x0F);
				b[7] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 12) & 0x0F);
				b[8] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 8) & 0x0F);
				b[9] = (byte)((Convert.ToByte(this.TextBox_VACTUAL.Text) >> 4) & 0x0F);
				b[10] = (byte)(Convert.ToByte(this.TextBox_VACTUAL.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_TCOOLTHRS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x14, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_TCOOLTHRS;
			this.SendBuf(b);
		}

		private void TextBox_TCOOLTHRS_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_TCOOLTHRS.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_TCOOLTHRS.Text) >> 20) & 0x0F);
				b[6] = (byte)((Convert.ToByte(this.TextBox_TCOOLTHRS.Text) >> 16) & 0x0F);
				b[7] = (byte)((Convert.ToByte(this.TextBox_TCOOLTHRS.Text) >> 12) & 0x0F);
				b[8] = (byte)((Convert.ToByte(this.TextBox_TCOOLTHRS.Text) >> 8) & 0x0F);
				b[9] = (byte)((Convert.ToByte(this.TextBox_TCOOLTHRS.Text) >> 4) & 0x0F);
				b[10] = (byte)(Convert.ToByte(this.TextBox_TCOOLTHRS.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_SGTHRS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x40, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_SGTHRS;
			this.SendBuf(b);
		}

		private void TextBox_SGTHRS_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x40, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_SGTHRS.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_SGTHRS.Text) >> 4) & 0x0F);
				b[6] = (byte)(Convert.ToByte(this.TextBox_SGTHRS.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_SG_RESULT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x41, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_SG_RESULT;
			this.SendBuf(b);
		}

		private void Button_MSCNT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6A, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_MSCNT;
			this.SendBuf(b);
		}

		private void Button_CUR_A_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6B, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_CUR_A;
			this.SendBuf(b);
		}

		private void Button_CUR_B_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6B, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_CUR_B;
			this.SendBuf(b);
		}

		private void Button_semin_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x42, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_semin;
			this.SendBuf(b);
		}

		private void ComboBox_semin_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x42, 0x00, 0x00, 0xFF };

			if (this.ComboBox_semin.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_semin.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_seup_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x42, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_seup;
			this.SendBuf(b);
		}

		private void ComboBox_seup_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x42, 0x05, 0x00, 0xFF };

			if (this.ComboBox_seup.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_seup.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_semax_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x42, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_semax;
			this.SendBuf(b);
		}

		private void ComboBox_semax_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x42, 0x08, 0x00, 0xFF };

			if (this.ComboBox_semax.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_semax.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_sedn_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x42, 0x0D, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_sedn;
			this.SendBuf(b);
		}

		private void ComboBox_sedn_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x42, 0x0D, 0x00, 0xFF };

			if (this.ComboBox_sedn.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_sedn.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_seimin_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x42, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_seimin;
			this.SendBuf(b);
		}

		private void ComboBox_seimin_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x42, 0x0F, 0x00, 0xFF };

			if (this.ComboBox_seimin.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_seimin.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_TOFF_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_TOFF;
			this.SendBuf(b);
		}

		private void ComboBox_TOFF_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x00, 0x00, 0xFF };

			if (this.ComboBox_TOFF.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_TOFF.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_HSTRT_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_HSTRT;
			this.SendBuf(b);
		}

		private void ComboBox_HSTRT_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x04, 0x00, 0xFF };

			if (this.ComboBox_HSTRT.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_HSTRT.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_HEND_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x07, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_HEND;
			this.SendBuf(b);
		}

		private void ComboBox_HEND_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x07, 0x00, 0xFF };

			if (this.ComboBox_HEND.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_HEND.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_TBL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x0F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_TBL;
			this.SendBuf(b);
		}

		private void ComboBox_TBL_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x0F, 0x00, 0xFF };

			if (this.ComboBox_TBL.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_TBL.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_vsense_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x11, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_vsense;
			this.SendBuf(b);
		}

		private void ComboBox_vsense_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x11, 0x00, 0xFF };

			if (this.ComboBox_vsense.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_vsense.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_MRES_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x18, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_MRES;
			this.SendBuf(b);
		}

		private void ComboBox_MRES_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x18, 0x00, 0xFF };

			if (this.ComboBox_MRES.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_MRES.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_intpol_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x1C, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_intpol;
			this.SendBuf(b);
		}

		private void ComboBox_intpol_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x1C, 0x00, 0xFF };

			if (this.ComboBox_intpol.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_intpol.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_dedge_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x1D, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_dedge;
			this.SendBuf(b);
		}

		private void ComboBox_dedge_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x1D, 0x00, 0xFF };

			if (this.ComboBox_dedge.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_dedge.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_diss2g_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x1E, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_diss2g;
			this.SendBuf(b);
		}

		private void ComboBox_diss2g_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x1E, 0x00, 0xFF };

			if (this.ComboBox_diss2g.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_diss2g.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_diss2vs_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6C, 0x1F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_diss2vs;
			this.SendBuf(b);
		}

		private void ComboBox_diss2vs_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x6C, 0x1F, 0x00, 0xFF };

			if (this.ComboBox_diss2vs.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_diss2vs.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_PWM_OFS_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_OFS;
			this.SendBuf(b);
		}

		private void TextBox_PWM_OFS_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBox_PWM_OFS.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_PWM_OFS.Text) >> 4) & 0x0F);
				b[6] = (byte)(Convert.ToByte(this.TextBox_PWM_OFS.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_PWM_GRAD_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_GRAD;
			this.SendBuf(b);
		}

		private void TextBox_PWM_GRAD_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;

			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x08, 0x00, 0x00, 0xFF };

			if (this.TextBox_PWM_GRAD.Text == "")
				return;
			else
			{
				b[5] = (byte)((Convert.ToByte(this.TextBox_PWM_GRAD.Text) >> 4) & 0x0F);
				b[6] = (byte)(Convert.ToByte(this.TextBox_PWM_GRAD.Text) & 0x0F);
			}

			this.SendBuf(b);
		}

		private void Button_pwm_freq_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_pwm_freq;
			this.SendBuf(b);
		}

		private void ComboBox_pwm_freq_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x10, 0x00, 0xFF };

			if (this.ComboBox_pwm_freq.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_pwm_freq.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_pwm_autoscale_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x12, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_pwm_autoscale;
			this.SendBuf(b);
		}

		private void ComboBox_pwm_autoscale_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x12, 0x00, 0xFF };

			if (this.ComboBox_pwm_autoscale.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_pwm_autoscale.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_pwm_autograd_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x13, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_pwm_autograd;
			this.SendBuf(b);
		}

		private void ComboBox_pwm_autograd_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x13, 0x00, 0xFF };

			if (this.ComboBox_pwm_autograd.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_pwm_autograd.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_freewheel_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x14, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_freewheel;
			this.SendBuf(b);
		}

		private void ComboBox_freewheel_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x14, 0x00, 0xFF };

			if (this.ComboBox_freewheel.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_freewheel.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_PWM_REG_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x18, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_REG;
			this.SendBuf(b);
		}

		private void ComboBox_PWM_REG_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x18, 0x00, 0xFF };

			if (this.ComboBox_PWM_REG.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)(this.ComboBox_PWM_REG.SelectedIndex + 1);

			this.SendBuf(b);
		}

		private void Button_PWM_LIM_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x70, 0x1C, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_LIM;
			this.SendBuf(b);
		}

		private void ComboBox_PWM_LIM_SelectionChangeCommitted(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x07, 0x70, 0x1C, 0x00, 0xFF };

			if (this.ComboBox_PWM_LIM.SelectedIndex == -1)
				return;
			else
				b[5] = (byte)this.ComboBox_PWM_LIM.SelectedIndex;

			this.SendBuf(b);
		}

		private void Button_PWM_SCALE_SUM_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x71, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_SCALE_SUM;
			this.SendBuf(b);
		}

		private void Button_PWM_SCALE_AUTO_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x71, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_SCALE_AUTO;
			this.SendBuf(b);
		}

		private void Button_PWM_OFS_AUTO_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x72, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_OFS_AUTO;
			this.SendBuf(b);
		}

		private void Button_PWM_GRAD_AUTO_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x72, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_PWM_GRAD_AUTO;
			this.SendBuf(b);
		}

		private void Button_otpw_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x00, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_otpw;
			this.SendBuf(b);
		}

		private void Button_ot_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x01, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_ot;
			this.SendBuf(b);
		}

		private void Button_s2ga_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x02, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_s2ga;
			this.SendBuf(b);
		}

		private void Button_s2gb_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x03, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_s2gb;
			this.SendBuf(b);
		}

		private void Button_s2vsa_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x04, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_s2vsa;
			this.SendBuf(b);
		}

		private void Button_s2vsb_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x05, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_s2vsb;
			this.SendBuf(b);
		}

		private void Button_ola_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x06, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_ola;
			this.SendBuf(b);
		}

		private void Button_olb_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x07, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_olb;
			this.SendBuf(b);
		}

		private void Button_t120_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x08, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_t120;
			this.SendBuf(b);
		}

		private void Button_t143_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x09, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_t143;
			this.SendBuf(b);
		}

		private void Button_t150_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x0A, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_t150;
			this.SendBuf(b);
		}

		private void Button_t157_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x0B, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_t157;
			this.SendBuf(b);
		}

		private void Button_CS_ACTUAL_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x10, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_CS_ACTUAL;
			this.SendBuf(b);
		}

		private void Button_stealth_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x1E, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_stealth;
			this.SendBuf(b);
		}

		private void Button_stst_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD9, 0x07, 0x6F, 0x1F, 0xFF };
			G_eGeneralCommand = GeneralCommand.TMC_Read_stst;
			this.SendBuf(b);
		}
		#endregion

#region SpeedDry
		private void ButtonSpeedDryStart_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x06, 0x07, 0x1E, 0x01, 0x03, 0x08, 0x08, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonSpeedDryStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x06, 0x07, 0x00, 0xFF };
			this.SendBuf(b);
		}

#endregion

		private void ButtonStallCaliOn_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x06, 0x05, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonStallCaliOff_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x06, 0x05, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonHome_2_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x06, 0x04, 0xFF };
			this.SendBuf(b);
		}

		private void ComboBoxPTType_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (this.ComboBoxPTType.SelectedIndex == 0)
				this.LabelMCUType.Text = "Pan";
			else if (this.ComboBoxPTType.SelectedIndex == 1)
				this.LabelMCUType.Text = "Tilt";
		}

#region Lens

		private void ButtonZoomStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x07, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomWide_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x07, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomTele_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x07, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomWideInd_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x07, 0xD3, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomTeleInd_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x07, 0xD2, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFocusStop_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x08, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFocusFar_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x08, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFocusNear_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x08, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoom1ResetWide_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x67, 0x02, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoom1ResetSensor_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x67, 0x02, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoom1ResetTele_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x67, 0x02, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomCtrlGroup1_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x07, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomCtrlGroup2_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x07, 0x00, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomCtrlModeTrack_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x07, 0x01, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZoomCtrlModeInd_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x07, 0x01, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFocusResetWide_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x68, 0x02, 0x01, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFocusResetSensor_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x68, 0x02, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFocusResetTele_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x68, 0x02, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZ1BufWide_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x73, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZ1BufTele_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x72, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZ2BufWide_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x73, 0x01, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonZ2BufTele_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x72, 0x01, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFBufFar_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x82, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonFBufNear_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0xD1, 0x04, 0x83, 0x00, 0x00, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonSetZ1Pos_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxZ1Pos.Text == "")
				this.TextBoxZ1Pos.Text = "0";

			position = Convert.ToUInt16(this.TextBoxZ1Pos.Text);

			b[4] = (byte)((position >> 12) & 0x0f);
			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonGetZ1Pos_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x09, 0x04, 0x47, 0xFF };
			G_eGeneralCommand = GeneralCommand.Lens_ReadZ1Position;
			this.SendBuf(b);
		}

		private void ButtonSetZ2Pos_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x57, 0x00, 0x00, 0x00, 0x00, 0x01, 0xFF };

			if (this.TextBoxZ2Pos.Text == "")
				this.TextBoxZ2Pos.Text = "0";

			position = Convert.ToUInt16(this.TextBoxZ2Pos.Text);

			b[4] = (byte)((position >> 12) & 0x0f);
			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonGetZ2Pos_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x09, 0x04, 0x57, 0xFF };
			G_eGeneralCommand = GeneralCommand.Lens_ReadZ2Position;
			this.SendBuf(b);
		}

		private void ButtonSetFPos_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x58, 0x00, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxFPos.Text == "")
				this.TextBoxFPos.Text = "0";

			position = Convert.ToUInt16(this.TextBoxFPos.Text);

			b[4] = (byte)((position >> 12) & 0x0f);
			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonGetFPos_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x09, 0x04, 0x48, 0xFF };
			G_eGeneralCommand = GeneralCommand.Lens_ReadFPosition;
			this.SendBuf(b);
		}

		private void ButtonZoomStepWide_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x47, 0x03, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxZoomStep.Text == "")
				this.TextBoxZoomStep.Text = "0";

			position = Convert.ToUInt16(this.TextBoxZoomStep.Text);

			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonZoomStepTele_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x47, 0x02, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxZoomStep.Text == "")
				this.TextBoxZoomStep.Text = "0";

			position = Convert.ToUInt16(this.TextBoxZoomStep.Text);

			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonFocusStepFar_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x48, 0x02, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxFocusStep.Text == "")
				this.TextBoxFocusStep.Text = "0";

			position = Convert.ToUInt16(this.TextBoxFocusStep.Text);

			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonFocusStepNear_Click(object sender, EventArgs e)
		{
			UInt16 position;
			byte[] b = { 0x81, 0x01, 0x04, 0x48, 0x03, 0x00, 0x00, 0x00, 0xFF };

			if (this.TextBoxFocusStep.Text == "")
				this.TextBoxFocusStep.Text = "0";

			position = Convert.ToUInt16(this.TextBoxFocusStep.Text);

			b[5] = (byte)((position >> 8) & 0x0f);
			b[6] = (byte)((position >> 4) & 0x0f);
			b[7] = (byte)(position & 0x0f);

			this.SendBuf(b);
		}

		private void ButtonICRDay_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x01, 0x02, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonICRNight_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x01, 0x03, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonICRDefog_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x01, 0x05, 0xFF };
			this.SendBuf(b);
		}

		private void ButtonICRNormal_Click(object sender, EventArgs e)
		{
			byte[] b = { 0x81, 0x01, 0x04, 0x01, 0x06, 0xFF };
			this.SendBuf(b);
		}
	}
}

#endregion
