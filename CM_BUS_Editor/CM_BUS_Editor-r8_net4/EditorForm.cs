using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace CM_BUS_Editor
{
    public partial class EditorForm : System.Windows.Forms.Form
    {
        /*-----------------------*/
        /* 各種定義 */
        /*-----------------------*/

        /** 送信 Function Header 返信有 定義 (返信無は+0x08) */
        public enum SendFunction
        {
            RegWriteNoAck = 0xF0, /**< 単体書き込み返信なし */
            RegWrite = 0xF8, /**< 単体書き込み */
            RegRead = 0xF9   /**< 単体読み出し */
        }

        /** サーボからの戻り値を示すクラス */
        public class ReternValue
        {
            /** コンストラクタ 全パラメータの初期化を行う */
            public ReternValue()
            {
                this.data = new byte[256]; /* パケットの最大Lengthをバッファサイズとする */
                Init(); /* このクラスの初期化 */
            }

            /** このクラスのメンバへ初期値を与える */
            public void Init()
            {
                this.id = 0x00;
                this.f_Active = false;
                this.f_InPosition = false;
                this.f_Reserve1 = false;
                this.f_Reserve2 = false;
                this.f_HardError = false;
                this.f_SoftError = false;
                this.f_ComError = false;
                this.f_InternalError = false;
                this.addr = 0x00;
                this.dataLength = 0;
                for (int i = 0; i < this.data.Length; i++)
                {
                    data[i] = 0;
                }
                this.crcError = false;
            }

            public byte id;                 /**< 受信 サーボid */
            public bool f_Active;           /**< 受信 flags b0 true = 動作中 */
            public bool f_InPosition;       /**< 受信 flags b1 true = 目標位置に到達 */
            public bool f_Reserve1;         /**< 受信 flags b2 予約 */
            public bool f_Reserve2;         /**< 受信 flags b3 予約 */
            public bool f_HardError;        /**< 受信 flags b4 true = ハードウェアエラー */
            public bool f_SoftError;        /**< 受信 flags b5 true = ソフトウェアエラー */
            public bool f_ComError;         /**< 受信 flags b6 true = 通信エラー */
            public bool f_InternalError;    /**< 受信 flags b7 true = その他エラー */
            public byte addr;               /**< レジスタアドレス = 送信時の Address と同じ */
            public int dataLength;          /**< データ長 = 送信時の Length と同じ */
            public byte[] data;             /**< 受信 データ */
            public bool crcError;           /**< true = 受信CRCエラー発生 */
        }

        /*-----------------------*/
        /* 共有データ */
        /*-----------------------*/
        public static SerialPort Com = new SerialPort();      /**< シリアルポート */
        public static string ComPortName = "";                /**< COMポート名 */
        public static ReternValue Ret = new ReternValue();    /**< 結果 */
        public static byte ServoId = 0;                       /**< 選択されているサーボID */
        public static byte[] Servo = new byte[256];           /**< サーボとの送受信データバッファ[メモリマップ全体] */
        public static Thread Servo_Search = null;            /**< サーボ検索スレッド */
        public static bool Servo_Search_Enable = false;       /**< サーボ検索スレッドの実行状態 */
        public bool speedoff = false;

        /** Com Port Band List */
        public static string[] ComPortBands = {
            "9600",
            "19200",
            "57600",
            "115200",
            "230400",
            "460800",
            "1000000"
            //"2000000",
            //"4000000",
            //"8000000"
        };

        /* Initial Com Port Band */
        public static string ComPortBandIni = "115200";

        /* Current Com Port Band */
        public static string ComPortBand = ComPortBandIni;

        /** Tab Names */
        public static readonly string[] TabName = {
            "Command. State. Control. Error.",
            "Communication. Drive. Gain. ",
            "Limit. Protection. Other."
        };

        /** Sub Group Names */
        public static readonly string[] SubGroupName = {
            "Command",
            "Command  (Push \"GetParameters\")",
            "Error  (Push \"GetParameters\")",
            "Communication",
            "Drive",
            "Gain",
            "Limit",
            "Protection",
            "Other"
        };

        /** Servo Register Map (編集・閲覧する際はExcel等へCSV形式でインポートするなどして下さい)
         <pre>
         [0]Address, [1]Size, [2]Memory Position, [3]Name, [4]JP-ja Description, [5]R/W Specify,
         [6]Initiale Value, [7]Minimum Value, [8]Maximum Value, [9]Unit, [10]Base, [11]Tab No, [12]Sub Group No, [1]Select Items
         </pre>
         */
        public static readonly string[] RegMap = {
            "0x00,4,RAM,Target Angle,指令角度,R/W,0,-36000000,36000000,0.1°,10,0,0,,",
            "0x04,2,RAM,Target Speed,指令速度,R/W,0,-300,300,rpm,10,0,0,,",
            "0x06,2,RAM,Target Torque,指令トルク,R/W,0,-10000,10000,0.01%,10,0,0,,",
            "0x08,1,RAM,Enable Torque,トルクON/OFF,R/W,0,0,2,-,10,0,0,,",
            "0x0A,2,RAM,Travel Time,移動時間,R/W,10,0,300,10ms,10,0,0,,",
            /*"0x0C,1,RAM,Acceleration Time,加速時間,R/W,3,3,200,10ms,10,0,0,,",
            "0x0D,1,RAM,Deceleration Time,減速時間,R/W,3,3,200,10ms,10,0,0,,",*/
            "0x10,4,RAM,Present Angle,現在角度,R,0,-36000000,36000000,0.1°,10,0,1,,",
            "0x14,2,RAM,Present Speed,現在速度,R,0,-300,300,rpm,10,0,1,,",
            "0x16,2,RAM,Present Torque,現在トルク,R,0,-10000,10000,0.01%,10,0,1,,",
            "0x1A,2,RAM,Present Travel Time,現在移動時間,R,0,0,65535,10ms,10,0,1,,",
            "0x1C,1,RAM,Present Temperature,現在温度,R,0,-20,100,℃,10,0,1,,",
            "0x1E,2,RAM,Present Voltage,現在電圧,R,0,0,500,0.1V,10,0,1,,",
            "0x28,1,RAM,Status Flags,ステータス情報,R,0x00,0x00,0xFF,flags,16,0,2,,",
            "0x2A,2,RAM,Hard. Error Flags,エラー情報ハード,R,0x0000,0x0000,0xFFFF,flags,16,0,2,,",
            "0x2C,2,RAM,Soft. Error Flags,エラー情報ソフト,R,0x0000,0x0000,0xFFFF,flags,16,0,2,,",
            "0x2E,2,RAM,Comm. Error Flags,エラー情報通信,R,0x0000,0x0000,0xFFFF,flags,16,0,2,,",
            "0x40,1,ROM,ID,ID,R/W,1,1,127,-,10,1,3,,",
            "0x41,1,ROM,Group ID,グループID,R/W,128,128,254,-,10,1,3,,",
            "0x42,1,ROM,Baud Rate,通信速度,R/W,3,0,9,-,sel,1,3,9600/19200/57600/115200/230400/460800/1000000/2000000/4000000/8000000,",
            "0x43,1,ROM,Return Delay Time,返信遅延時間,R/W,1,1,100,25us,10,1,3,,",
            /*"0x44,1,ROM,Protocol Type,通信プロトコル,R/W,2,0,2,-,sel,1,3,0:CMD/1:S.BUS/2:New,",*/
            "0x46,1,ROM,Green LED indicate,LED(緑)表示,R/W,0x08,0x00,0x1F,-,16,1,4,,",
            "0x47,1,ROM,Red LED indicate,LED(赤)表示,R/W,0x13,0x00,0x1F,-,16,1,4,,",
            "0x48,1,ROM,Enable Smoothing,指令の平滑化,R/W,0,0,1,flag,sel,1,4,0:No/1:Yes,",
            "0x49,1,ROM,Enable Reverse,CW/CCW反転,R/W,0,0,1,flag,sel,1,4,0:No/1:Yes,",
            "0x4A,1,ROM,Enable MultiTurn,マルチターン,R/W,0,0,1,flag,sel,1,4,0:No/1:Yes,",
            /*"0x4B,1,ROM,Move Mode,制御モード,R/W,0,0,2,-,10,1,4,,",*/
            "0x4C,1,ROM,Relative Angle Mode,相対角度制御,R/W,1,0,1,flag,sel,1,4,0:No/1:Yes,",
            "0x4D,1,ROM,RollOver,ロールオーバー,R/W,1,0,1,flag,sel,1,4,0:No/1:Yes,",
            /*"0x4E,1,ROM,Accel-Decel Mode,加減速制御,R/W,1,0,1,flag,sel,1,4,0:No/1:Yes,",*/
            "0x50,1,ROM,Angle Prop Gain,角度 比例ゲイン,R/W,50,1,255,gain,10,1,5,,",
            "0x51,1,ROM,Angle Diff Gain,角度 微分ゲイン,R/W,50,0,255,gain,10,1,5,,",
            /*"0x52,1,ROM,Angle Boost,角度 ブースト,R/W,50,0,255,0.1%,10,1,5,,",*/
            "0x53,1,ROM,Angle Dead band,角度 不感帯,R/W,50,0,100,0.1°,10,1,5,,",
            "0x54,1,ROM,Speed Prop Gain,速度 比例ゲイン,R/W,50,1,255,gain,10,1,5,,",
            "0x55,1,ROM,Speed Intg Gain,速度 積分ゲイン,R/W,50,0,255,gain,10,1,5,,",
            "0x56,1,ROM,Speed Intg Limit,速度 積分上限,R/W,50,1,200,-,10,1,5,,",
            "0x57,1,ROM,Speed Dead band,速度 不感帯,R/W,50,0,100,rpm,10,1,5,,",
            "0x58,1,ROM,Current Prop Gain,トルク 比例ゲイン,R/W,38,1,255,gain,10,1,5,,",
            "0x59,1,ROM,Current Intg Gain,トルク 積分ゲイン,R/W,50,1,255,gain,10,1,5,,",
            /*"0x5A,1,ROM,Current Intg Limit,トルク 積分上限,R/W,50,1,200,-,10,1,5,,",
            "0x5B,1,ROM,Current Duty Limit,トルク 出力上限,R/W,100,1,100,%,10,1,5,,",
            "0x5C,1,ROM,Correct Dead Time,むだ時間補正,R/W,2,0,100,-,10,1,5,,",*/
            "0x5D,1,ROM,Angle error Correct,累積角度誤差補正,R/W,1,0,1,flag,sel,1,5,OFF/ON,",
            /*"0x5E,1,ROM,Velocity Smooth,速度平滑化,R/W,0,0,2,-,10,1,5,,",*/
            "0x70,4,ROM,Limit Angle CW,制限角度(CW),R/W,36000000,0,36000000,0.1°,10,2,6,,",
            "0x74,4,ROM,Limit Angle CCW,制限角度(CCW),R/W,-36000000,-36000000,0,0.1°,10,2,6,,",
            "0x78,2,ROM,Limit Speed CW,制限速度(CW),R/W,300,0,300,rpm,10,2,6,,",
            "0x7A,2,ROM,Limit Speed CCW,制限速度(CCW),R/W,-300,-300,0,rpm,10,2,6,,",
            "0x7C,2,ROM,Limit Torque CW,制限トルク(CW),R/W,10000,0,10000,0.01%,10,2,6,,",
            "0x7E,2,ROM,Limit Torque CCW,制限トルク(CW),R/W,-10000,-10000,0,0.01%,10,2,6,,",
            "0x80,1,ROM,Limit Temp. High,制限温度(上限),R/W,60,20,80,℃,10,2,6,,",
            "0x81,1,ROM,Limit Temp. Low,制限温度(下限),R/W,0,-20,20,℃,10,2,6,,",
            "0x82,2,ROM,Limit Voltage High,制限電圧(上限),R/W,300,120,500,0.1V,10,2,6,,",
            "0x84,2,ROM,Limit Voltage Low,制限電圧(下限),R/W,80,45,120,0.1V,10,2,6,,",
            "0x88,1,ROM,Timeout Operation,タイムアウト動作,R/W,1,0,2,-,sel,2,7,0:Free/1:Keep/2:Brake,",
            "0x89,1,ROM,Timeout Period,タイムアウト時間,R/W,100,1,255,10ms,10,2,7,,",
            "0x8A,1,ROM,OLP Torque,過負荷保護 閾値,R/W,80,1,100,%,10,2,7,,",
            "0x8B,1,ROM,OLP Time,過負荷保護 時間,R/W,150,1,255,10ms,10,2,7,,",
            "0x8C,1,ROM,OCP Current,過電流保護,R/W,70,10,120,0.1A,10,2,7,,",
            "0x90,1,ROM,Startup Torque,起動時トルク,R/W,0,0,2,-,10,2,8,,",
            "0x91,1,ROM,Enable Soft Start,起動時低速動作,R/W,0,0,1,flag,sel,2,8,0:No/1:Yes,",
            "0x92,1,ROM,Target Soft Speed,低速動作時速度,R/W,8,5,20,rpm,10,2,8,,",
            "0x93,1,ROM,Target Soft Torque,低速動作時トルク,R/W,30,20,40,%,10,2,8,,",
            "0x94,2,ROM,Origin Position,原点角度,R/W,0,-1800,1799,0.1°,10,2,8,,",
            "0x96,2,ROM,Enable boot mode key,ブートローダ起動,R/W,0x0000,0x0000,0xFFFF,-,16,2,8,,",
            "0xA0,2,ROM,PWMIN Neutral,PWM ニュートラル,R/W,1520,300,10000,us,10,2,8,,",
            "0xA2,2,ROM,PWMIN Range,PWM 入力パルス幅,R/W,700,100,5000,us,10,2,8,,",
            "0xA4,2,ROM,PWMIN Target,PWM 動作指令,R/W,700,10,36000,0.1°/rpm,10,2,8,,",
            "0xA6,1,ROM,PWMIN Target Mode,PWM 動作モード,R/W,0,0,1,-,sel,2,8,0:Angle/1:Speed,"
        };

        /*-----------------------*/
        /* 処理 */
        /*-----------------------*/

        /**
         * @brief フォーム上の全オブジェクトの初期状態を作成する
         * @note フォームにオブジェクトを配置します。EventHandler()内がサーボに対する処理です。
         */
        private void Ui_Make_Init()
        {
            /* フォームデザイナー側で指定したフォントが自動的にシステムデフォルトに変更されてしまわないように指定する */
            this.Font = new System.Drawing.Font("Arial", 9);

            /* フォームデザイナー側で指定したフォームサイズが自動調整されてしまわないように、右下に置いた透明ラベルの位置をフォームの大きさに指定する */
            this.ClientSize = new System.Drawing.Size(FormBottomRightLabel.Location.X + FormBottomRightLabel.Size.Width, FormBottomRightLabel.Location.Y + FormBottomRightLabel.Size.Height);

            /* ComPortComboBoxの初期化 */
            string[] ComPortNames = SerialPort.GetPortNames();
            if (ComPortNames.Length != 0)
            {
                ComPortComboBox.Items.AddRange(ComPortNames);
                ComPortComboBox.Text = ComPortNames[0];
                ComPortName = ComPortComboBox.Text;
            }
            ComPortComboBox.SelectedIndexChanged += new EventHandler(Port_Chenge);

            /* BandRateComboBoxの初期化 */
            BandRateComboBox.Items.AddRange(ComPortBands);
            BandRateComboBox.Text = ComPortBandIni;
            ComPortBand = BandRateComboBox.Text;
            BandRateComboBox.SelectedIndexChanged += new EventHandler(Port_Chenge);

            /* 初回COMオープン */
            this.Shown += new EventHandler(Port_Chenge);

            /* SearchButtonの初期化(非公開機能) */
            SearchButton.Click += new EventHandler(Servo_Search_Button_Click);
            SearchButton.Visible = false;

            /* ServoIdNumericUpDownの初期化 */
            ServoIdNumericUpDown.Value = (int)ServoId;
            ServoIdNumericUpDown.ValueChanged += new EventHandler(Servo_ID_Numeric_UpDown_Change);

            /* IdSelectRadioButtonの初期化 */
            IdSelect1RadioButton.Select();

            /* ServoIdNumericUpDownの初期化 */
            Servo_ID_Select_Click(IdSelect1RadioButton, null);
            IdSelect1RadioButton.Click += new EventHandler(Servo_ID_Select_Click);
            IdSelect2RadioButton.Click += new EventHandler(Servo_ID_Select_Click);
            IdSelect3RadioButton.Click += new EventHandler(Servo_ID_Select_Click);

            /* AckButtonの初期化 */
            AckButton.Click += new EventHandler(Servo_Ack_Button_Click);

            /* AckResultLabelの初期化 */
            AckResultLabel.Text = "---";

            /* GetParametersButtonの初期化 */
            GetParametersButton.Click += new EventHandler(Servo_Get_Parameters_Click);

            /* InitializeServoButtonの初期化 */
            InitializeServoButton.Click += new EventHandler(Servo_Initialize_Button_Click);

            /* WriteFlashRomButtonの初期化 */
            WriteFlashRomButton.Click += new EventHandler(Servo_Write_FlashRom_Button_Click);

            /* RebootButtonの初期化 */
            RebootButton.Click += new EventHandler(Servo_Reboot_Button_Click);

            /* SleepButtonの初期化(非公開機能) */
            SleepButton.Click += new EventHandler(Servo_Sleep_Button_Click);
            SleepButton.Visible = false;

            /* TargetAngleTrackBarの初期化 */
            TargetAngleTrackBar.SmallChange = 10;
            TargetAngleTrackBar.LargeChange = 100;
            TargetAngleTrackBar.Minimum = -36000;
            TargetAngleTrackBar.Maximum = 36000;
            TargetAngleTrackBar.Value = 0;
            TargetAngleTrackBar.TickFrequency = 100;
            TargetAngleTrackBar.ValueChanged += new EventHandler(Servo_Set_Target_Angle_Change);

            /* TargetAngleMinLabel, TargetAngleMaxLabelの初期化 */
            TargetAngleMinLabel.Text = (TargetAngleTrackBar.Minimum / 10).ToString() + ".0";
            TargetAngleMaxLabel.Text = (TargetAngleTrackBar.Maximum / 10).ToString() + ".0";

            /* TargetAngleNumericUpDownの初期化 */
            TargetAngleNumericUpDown.Minimum = TargetAngleTrackBar.Minimum;
            TargetAngleNumericUpDown.Maximum = TargetAngleTrackBar.Maximum;
            TargetAngleNumericUpDown.Value = 0;
            TargetAngleNumericUpDown.ValueChanged += new EventHandler(Servo_Set_Target_Angle_Change);

            /* MoveSxButtonの初期化 */
            MoveSxButton.Click += new EventHandler(Servo_MoveSx_Button_Click);

            /* RepeatMoveSxButtonの初期化 */
            RepeatMoveSxButton.CheckedChanged += new EventHandler(Servo_RepeatMoveSx_Button_Click);

            /* RepeatSpeedButtonの初期化 */
            RepeatSpeedButton.CheckedChanged += new EventHandler(Servo_RepeatSpeed_Button_Click);

            /* Set0Buttonの初期化 */
            Set0Button.Click += new EventHandler(Servo_Set0_Button_Click);

            /* Statusの初期化 */
            ModelNumberParameterLabel.Text = "";
            FirmwareVersionParameterLabel.Text = "";
            UniqueNumberParameterLabel.Text = "";
            ManufactureDateParameterLabel.Text = "";
            this.Shown += (sender, e) => Set_Result_Status();

            /* 各項目の初期位置 */
            double HeightScale = 0.9; /* 項目の縦のスケール調整 */
            double LabelMargin = 2.4; /* 項目ラベルの初期位置調整 */
            int ColumnHeight = (int)((BandRateComboBox.Location.Y - ComPortComboBox.Location.Y) * HeightScale); /* Y = Formの COM とBAND のComboBox間を基準とする */

            int AddrIniX = HeaderLabel0.Location.X, AddrIniY = (int)(HeaderLabel0.Location.Y * LabelMargin), AddrWidth = HeaderLabel0.Size.Width, AddrHeight = HeaderLabel0.Size.Height;
            int AddrX = AddrIniX, AddrY = AddrIniY;

            int NameIniX = HeaderLabel1.Location.X, NameIniY = (int)(HeaderLabel1.Location.Y * LabelMargin), NameWidth = HeaderLabel1.Size.Width, NameHeight = HeaderLabel1.Size.Height;
            int NameX = NameIniX, NameY = NameIniY;

            int RegIniX = HeaderLabel2.Location.X, RegIniY = (int)(HeaderLabel2.Location.Y * LabelMargin), RegWidth = HeaderLabel2.Size.Width, RegHeight = (int)(ComPortComboBox.Size.Height * HeightScale);
            RegIniY -= ((RegHeight - HeaderLabel2.Size.Height) / 2); /* LabelよりHeightが大きい分位置調整 */
            int RegX = RegIniX, RegY = RegIniY;

            int UnitIniX = HeaderLabel3.Location.X, UnitIniY = (int)(HeaderLabel3.Location.Y * LabelMargin), UnitWidth = HeaderLabel3.Size.Width, UnitHeight = HeaderLabel3.Size.Height;
            int UnitX = UnitIniX, UnitY = UnitIniY;

            int SetIniX = HeaderLabel4.Location.X, SetIniY = (int)(HeaderLabel4.Location.Y * LabelMargin), SetWidth = HeaderLabel4.Size.Width, SetHeight = (int)(SearchButton.Size.Height * HeightScale);
            SetIniY -= ((SetHeight - HeaderLabel4.Size.Height) / 2); /* LabelよりHeightが大きい分位置調整 */
            int SetX = SetIniX, SetY = SetIniY;

            int RamStart = -1, RamEnd = -1, RomStart = -1, RomEnd = -1;

            int TabIndex = 1;

            /* 項目のタブを初期化 */
            tabCtrl.Size = new System.Drawing.Size(ServoParameterGroupBox.Width - tabCtrl.Location.X * 2, ServoParameterGroupBox.Height - tabCtrl.Location.Y * 2);
            for (int i = 0; i < TabName.Length; i++)
            {
                TabPage tabPage = new TabPage();
                tabPage.Name = "tab" + i.ToString(); /* tab0 ... */
                tabPage.Text = "   " + TabName[i] + "   ";
                tabCtrl.Controls.Add(tabPage);
            }
            tabCtrl.TabIndex = 100;
            ServoParameterGroupBox.Controls.Add(tabCtrl);

            /* 項目のグループを作成 */
            int GroupIniHeight = (int)(HeaderLabel0.Size.Height * LabelMargin);
            int[] GroupHeight = new int[SubGroupName.Length];
            int[] GroupTab = new int[SubGroupName.Length];
            int GroupIniX = LabelsPanel.Location.X, GroupIniY = LabelsPanel.Location.Y;
            int GroupX = GroupIniX, GroupY = GroupIniY;
            int CurrentTab = 0;

            for (int i = 0; i < SubGroupName.Length; i++)
            {   /* グループの初期サイズ */
                GroupHeight[i] = GroupIniHeight;
            }
            for (int i = 0; i < RegMap.Length; i++)
            {   /* グループに属する項目の個数を数えてグループの高さを決める */
                string[] prm = RegMap[i].Split(',');
                int groupIndex = Convert.ToInt32(prm[12]);
                int tabIndex = Convert.ToInt32(prm[11]);
                GroupHeight[groupIndex] += ColumnHeight; /* グループごとの高さ */
                GroupTab[groupIndex] = tabIndex; /* グループが属するタブ番号 */
            }
            for (int i = 0; i < SubGroupName.Length; i++)
            {   /* グループを作成してタブページに配置する */
                GroupBox groupBox = new GroupBox();
                groupBox.Size = new System.Drawing.Size(LabelsPanel.Width, GroupHeight[i]); /* グループの幅はLabelsPanelに合わせる */
                if (CurrentTab != GroupTab[i])
                {   /* タブ移動したらX,Y座標を初期化する */
                    GroupX = GroupIniX;
                    GroupY = GroupIniY;
                    CurrentTab = GroupTab[i];
                }
                if (GroupY + groupBox.Height > (ServoParameterGroupBox.Height - GroupIniHeight * 2))
                {   /* 列移動 */
                    GroupX = LabelsPanel.Width + GroupIniX * 2;
                    GroupY = GroupIniY;
                }
                groupBox.Location = new System.Drawing.Point(GroupX, GroupY);
                groupBox.Name = "group" + i.ToString();
                groupBox.Text = SubGroupName[i];

                for (int j = 0; j < 5; j++)
                {   /* グループ内にラベルを配置する */
                    Control[] formLabel = LabelsPanel.Controls.Find($"HeaderLabel{j}", true);
                    Label headerLabel = formLabel[0] as Label;
                    if (headerLabel != null)
                    {
                        Label tabLabel = new Label()
                        {
                            Text = headerLabel.Text,
                            Size = new System.Drawing.Size(headerLabel.Width, headerLabel.Height),
                            Location = new System.Drawing.Point(headerLabel.Location.X, headerLabel.Location.Y),
                            AutoSize = false
                        };
                        groupBox.Controls.Add(tabLabel);
                    }
                }

                Control[] tab = tabCtrl.Controls.Find($"tab{GroupTab[i]}", true);
                TabPage tabPage = tab[0] as TabPage;
                if (tabPage != null)
                {
                    tabPage.Controls.Add(groupBox);
                }

                /* グループの下へ次のグループ追加 */
                GroupY += groupBox.Height;
            }

            int CurrentGroup = 0;

            for (int i = 0; i < RegMap.Length; i++)
            {   /* 各項目の作成 */
                string[] prm = RegMap[i].Split(',');
                for (int j = 0; j < prm.Length; j++)
                {   /* 16進数表現 0xを外す */
                    if (prm[j].StartsWith("0x") == true)
                    {
                        prm[j] = prm[j].Replace("0x", "");
                    }
                }

                int Addr = Convert.ToInt32(prm[0], 16);
                int Size = Convert.ToInt32(prm[1], 10);
                string Name = "_" + prm[0] + "_" + prm[1] + "_" + prm[10]; /* コントロールの名前 _Addr_Size_Base */
                int GroupNo = Convert.ToInt32(prm[12]);

                for (int j = 0; j < prm.Length; j++)
                {   /* TBDならば初期値を与える */
                    if (prm[7] == "TBD")
                    {
                        /* Minimum Value不定なら0とする */
                        if (prm[10] == "16")
                        {
                            prm[7] = new string('0', Size);
                        }
                        else
                        {
                            prm[7] = "0";
                        }
                    }
                    if (prm[6] == "TBD")
                    {
                        /* Initiale Value不定ならMinimum Valueとする */
                        prm[6] = prm[7];
                    }
                    if (prm[8] == "TBD")
                    {
                        /* Maximum Value不定なら最大サイズとする */
                        if (prm[10] == "16")
                        {
                            prm[8] = new string('F', Size);
                        }
                        else
                        {
                            if (Size == 1)
                            {
                                prm[8] = "255";
                            }
                            else if (Size == 2)
                            {
                                prm[8] = "65535";
                            }
                            else
                            {
                                prm[8] = "4294967295";
                            }
                        }
                    }

                }

                Control[] groupCtrl = ServoParameterGroupBox.Controls.Find($"group{GroupNo}", true);
                GroupBox group = groupCtrl[0] as GroupBox;
                if (group != null)
                {
                    if (CurrentGroup != GroupNo)
                    {   /* グループ移動したらX,Y座標を初期化する */
                        AddrY = AddrIniY;
                        NameY = NameIniY;
                        RegY = RegIniY;
                        UnitY = UnitIniY;
                        SetY = SetIniY;
                        CurrentGroup = GroupNo;
                    }

                    /* SetRAM, SetROMボタンの表示用 */
                    if (prm[2] == "RAM")
                    {
                        if (RamStart == -1)
                        {
                            RamStart = Addr;
                        }
                        if ((prm[5] == "R/W") || (prm[5] == "W"))
                        {
                            RamEnd = Addr + Size - 1;
                        }
                    }
                    if (prm[2] == "ROM")
                    {
                        if (RomStart == -1)
                        {
                            RomStart = Addr;
                        }
                        if ((prm[5] == "R/W") || (prm[5] == "W"))
                        {
                            RomEnd = Addr + Size - 1;
                        }
                    }

                    /* Address部へNo.表記を作る */
                    string NoText;
                    if (Size == 1)
                    {
                        NoText = "No." + Addr.ToString();
                    }
                    else
                    {
                        NoText = "No." + Addr.ToString() + "-" + (Addr + Size - 1).ToString();
                    }
                    Label NoLabel = new Label();
                    NoLabel.AutoSize = false;
                    NoLabel.Location = new System.Drawing.Point(AddrX, AddrY);
                    NoLabel.Size = new System.Drawing.Size(AddrWidth, AddrHeight);
                    NoLabel.Text = NoText;
                    group.Controls.Add(NoLabel);

                    /* Name部へレジスタ名表記を作る */
                    Label RegLabel = new Label();
                    RegLabel.AutoSize = false;
                    RegLabel.Location = new System.Drawing.Point(NameX, NameY);
                    RegLabel.Size = new System.Drawing.Size(NameWidth, NameHeight);
                    RegLabel.Text = prm[3];
                    group.Controls.Add(RegLabel);

                    /* Parameter部へ数値入力ボックス, テキストボックス, コンボボックスを作る */
                    if (prm[10] == "10")
                    {   /* 10進数ならば数値入力ボックス */

                        /* 数値入力コントロールだけWindowsの高DPI設定に追従できないバグがあるので強制する */
                        double parDpi = (double)DeviceDpi / 96.0;

                        NumericUpDown RegInput = new NumericUpDown();
                        RegInput.Name = Name;
                        RegInput.Location = new System.Drawing.Point((int)((double)RegX / parDpi), (int)((double)RegY / parDpi));
                        RegInput.Size = new System.Drawing.Size((int)((double)RegWidth / parDpi), RegHeight);
                        RegInput.Minimum = Convert.ToInt32(prm[7]);
                        RegInput.Maximum = Convert.ToInt32(prm[8]);
                        RegInput.Value = Convert.ToInt32(prm[6]);
                        if (prm[5] == "R")
                        {   /*リードオンリー */
                            RegInput.Enabled = false;
                        }
                        RegInput.TextAlign = HorizontalAlignment.Right;
                        RegInput.TabIndex = TabIndex;
                        RegInput.Leave += new EventHandler(Servo_Paramater_Leave);
                        if (prm[0] == "00")
                        {   /* Target Angle */
                            RegInput.ValueChanged += new EventHandler(Servo_Set_Target_Angle_Change);
                        }
                        group.Controls.Add(RegInput);
                        byte[] data = GetByteArray(Convert.ToInt32(prm[6]), Size);
                        Array.Copy(data, 0, Servo, Addr, Size);
                    }
                    else if (prm[10] == "16")
                    {   /* 16進数ならばテキストボックス */
                        TextBox RegInput = new TextBox();
                        RegInput.Name = Name;
                        RegInput.Location = new System.Drawing.Point(RegX, RegY);
                        RegInput.Size = new System.Drawing.Size(RegWidth, RegHeight);
                        RegInput.MaxLength = prm[8].Length;
                        RegInput.Text = prm[6];
                        if (prm[5] == "R")
                        {   /*リードオンリー */
                            RegInput.Enabled = false;
                        }
                        RegInput.TextAlign = HorizontalAlignment.Right;
                        RegInput.TabIndex = TabIndex;
                        RegInput.KeyDown += new KeyEventHandler(Servo_Paramater_16Base_KeyPress);
                        RegInput.Leave += new EventHandler(Servo_Paramater_Leave);
                        group.Controls.Add(RegInput);
                        byte[] data = GetByteArray(Convert.ToInt32(prm[6], 16), Size);
                        Array.Copy(data, 0, Servo, Addr, Size);
                    }
                    else if (prm[10] == "sel")
                    {   /* 選択肢ならばコンボボックス */
                        string[] items = prm[13].Split('/');
                        ComboBox RegInput = new ComboBox();
                        RegInput.Name = Name;
                        RegInput.Location = new System.Drawing.Point(RegX, RegY);
                        RegInput.Size = new System.Drawing.Size(RegWidth, RegHeight);
                        RegInput.DropDownStyle = ComboBoxStyle.DropDownList;
                        RegInput.Items.AddRange(items);
                        RegInput.SelectedIndex = Convert.ToInt32(prm[6]);
                        if (prm[5] == "R")
                        {   /*リードオンリー */
                            RegInput.Enabled = false;
                        }
                        RegInput.TabIndex = TabIndex;
                        RegInput.Leave += new EventHandler(Servo_Paramater_Leave);
                        group.Controls.Add(RegInput);
                        byte[] data = GetByteArray(Convert.ToInt32(prm[6]), Size);
                        Array.Copy(data, 0, Servo, Addr, Size);
                    }
                    else { }

                    TabIndex++;

                    /* Parameter部へ単位表記を作る */
                    Label UnitLabel = new Label();
                    UnitLabel.AutoSize = false;
                    UnitLabel.Location = new System.Drawing.Point(UnitX, UnitY);
                    UnitLabel.Size = new System.Drawing.Size(UnitWidth, UnitHeight);
                    UnitLabel.Text = prm[9];
                    group.Controls.Add(UnitLabel);

                    /* Setボタンを作る */
                    if ((prm[5] == "R/W") || (prm[5] == "W"))
                    {
                        Button SetBtn = new Button();
                        SetBtn.Name = Name + "_Button";
                        SetBtn.Location = new System.Drawing.Point(SetX, SetY);
                        SetBtn.Size = new System.Drawing.Size(SetWidth, SetHeight);
                        SetBtn.Text = "Set";
                        SetBtn.TabIndex = TabIndex;
                        SetBtn.Click += new EventHandler(Servo_Set_Click);
                        group.Controls.Add(SetBtn);

                        TabIndex++;
                    }

                    /* 次の項目へ移動 */
                    AddrY += ColumnHeight;
                    NameY += ColumnHeight;
                    RegY += ColumnHeight;
                    UnitY += ColumnHeight;
                    SetY += ColumnHeight;
                }
            }

            /* SetRamボタンの初期化 */
            SetRamButton.Text = "Set No." + RamStart.ToString() + " - No." + RamEnd.ToString();
            SetRamButton.Click += new EventHandler(Servo_Set_RAM_Click);

            /* SetRomボタンの初期化 */
            SetRomButton.Text = "Set No." + RomStart.ToString() + " - No." + RomEnd.ToString();
            SetRomButton.Click += new EventHandler(Servo_Set_ROM_Click);

            /* フォームを閉じるときにServo_Search_Threadも終了 */
            this.FormClosed += (sender, e) => { Servo_Search_Enable = false; };
        }

        /**
         * @brief サーボからの戻り値をステータス情報に反映する
         */
        private void Set_Result_Status()
        {
            Change_Status(Result1Label, Ret.f_Active, "Active", false);
            Change_Status(Result2Label, Ret.f_InPosition, "In Position", false);
            Change_Status(Result3Label, Ret.f_HardError, "Hard Error", true);
            Change_Status(Result4Label, Ret.f_SoftError, "Soft Error", true);
            Change_Status(Result5Label, Ret.f_ComError, "Com Error", true);
            Change_Status(Result6Label, Ret.f_InternalError, "Other Error", true);
        }

        /**
         * @brief ステータス内容を更新する
         * @param Status_Object [in]更新先のラベルオブジェクト
         * @param status [in]true/false
         * @param message [in]ラベルオブジェクトに表示する内容
         * @param error [in]true=エラー情報/true=正常情報
         * @note この処理はスレッド化されています。右上Status枠のサーボ状態を更新します。
         */
        private void Change_Status(Label Status_Object, bool status, string message, bool error)
        {
            try
            {
                Invoke(new Action<Label, bool, string, bool>((obj, sta, msg, err) =>
                {
                    if (sta == true)
                    {
                        obj.Text = "■ " + msg;
                        if (err == true)
                        {
                            obj.ForeColor = System.Drawing.Color.FromArgb(160, 0, 0);
                        }
                        else
                        {
                            obj.ForeColor = System.Drawing.Color.FromArgb(0, 160, 0);
                        }
                    }
                    else
                    {
                        obj.Text = "□ " + msg;
                        obj.ForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
                    }
                }), Status_Object, status, message, error);
            }
            catch { }
        }

        /**
         * @brief サーボからレジスタを読み込み共有データServo[]を更新する
         * @param id [in]サーボID
         * @param addr [in]読み込むレジスタの先頭アドレス
         * @param dataLength [in]読み込むレジスタ長
         * @retval true=成功
         * @retval false=失敗
         * @note サーボIDと先頭アドレスと読み込むレジスタ長を指定してサーボレジスタを読み込みます。レジスタのアドレス先頭を指定しなかった場合及び、レジスタ長に満たない範囲は読み込むことが出来ません。例えばTarget Angleは0x00～0x03に配置されていますが、0x03を指定して1byteのみを読み込むことはできません。レジスタは連続して読み込むことが出来ます。例えば0x00から16byteを指定して連続で読み込むことが出来ます。この場合、Target Angle、Target Speed、Target Torque、Enable Torque、Travel Time、Acceleration Timeが同時に読み込まれます。割り当ての無いアドレスのレジスタ値は不定です。
         */
        private bool Reg_Read(byte id, byte addr, byte dataLength)
        {
            bool result; /* 結果 */

            /* パケット送受信 */
            result = Comm(SendFunction.RegRead, id, addr, GetEmptyArray(), dataLength);
            if (result == false)
            {
                goto exit_func;
            }

            if (Ret.crcError == true)
            {
                AckResultLabelUpdate("CRC Error");
                goto exit_func;
            }

            result = true;
        exit_func:
            return result;
        }

        /**
         * @brief 共有データServo[]の値をサーボのレジスタに書き込む
         * @param id [in]サーボID
         * @param addr [in]書き込むレジスタの先頭アドレス
         * @param dataLength [in]書き込むレジスタ長
         * @retval true=成功
         * @retval false=失敗
         * @note サーボIDと先頭アドレスと書き込むレジスタ長を指定してサーボレジスタへ書き込みます。レジスタのアドレス先頭を指定しなかった場合及び、レジスタ長に満たない範囲の書き込みは出来ません。例えばTarget Angleは0x00～0x03に配置されていますが、0x03を指定して1byteのみを書き込むことはできません。レジスタは連続して書き込むことが出来ます。例えば0x00から16byteを指定して連続で書き込むことが出来ます。この場合、Target Angle、Target Speed、Target Torque、Enable Torque、Travel Time、Acceleration Timeへ同時に書き込まれます。割り当ての無いアドレスのレジスタへは書き込まれません。
         */
        private bool Reg_Write(byte id, byte addr, byte dataLength)
        {
            bool result; /* 結果 */
            byte[] data = new byte[dataLength]; /* 送信データバッファ */

            /* 送信するデータを用意 */
            Array.Copy(Servo, addr, data, 0, data.Length);

            /* コンソールに送信データを表示 */
            Console.WriteLine("サーボID: " + id);
            Console.WriteLine("データ長: " + data.Length);
            Console.WriteLine("アドレス: " + addr);
            for (int i = 0; i < data.Length; i++)
            {
                Console.WriteLine("データ[" + i + "]: " + data[i]);
            }

            /* パケット送受信 */
            if (addr >= 0x20 && addr <= 0x23)
            {
                result = Comm(SendFunction.RegWriteNoAck, id, addr, data, dataLength);
            }
            else
            {
                result = Comm(SendFunction.RegWrite, id, addr, data, dataLength);
            }
            if (result == false)
            {
                goto exit_func;
            }

            if (Ret.crcError == true)
            {
                AckResultLabelUpdate("CRC Error");
            }

            result = true;
        exit_func:
            return result;
        }

        /**
         * @brief サーボへパケットを送信し、応答ありパケットなら受信して共有データServo[]を更新する
         * @param func [in]RegWrite若しくはRegReadを指定
         * @param id [in]サーボID
         * @param addr [in]読み書きするレジスタの先頭アドレス
         * @param data [in]送信するデータ配列
         * @param dataLength [in]送信及び受信するデータ配列長
         * @retval true=成功
         * @retval false=失敗
         * @note id=0～127(Unique ID)は返信があります。id=129～254(Group ID)とid=255(Broadcast ID)は返信がありません。この処理は返信有りの場合func=0xf8(書き込み)とfunc=0xf9(読み込み)が出来ます。返信無しの場合func=0xf0のみが出来ます。
         */
        private bool Comm(SendFunction func, byte id, byte addr, byte[] data, byte dataLength)
        {
            bool result;                    /* 結果 */
            int i;                          /* カウンタ */

            /* 受信データ初期化 */
            Ret.Init();

            /* パケット送信 */
            result = Comm_Send(func, id, addr, data, dataLength);
            if (result == false)
            {
                goto exit_func;
            }

            if (id >= 0x80)
            {   /* 応答なしパケット種別の場合はここまで */
                result = true;
                goto exit_func;
            }

            if (addr >= 0x20 && addr <= 0x23)
            {   /* 返答無しコマンドの場合はここまで */
                result = true;
                goto exit_func;
            }

            /* パケット受信 */
            result = Comm_Recv(func, dataLength);
            if (result == false)
            {
                goto exit_func;
            }

            if (func == SendFunction.RegRead)
            {
                if (Ret.crcError == false)
                {
                    /* サーボの現在値を更新 */
                    for (i = 0; i < Ret.dataLength; i++)
                    {
                        Servo[addr + i] = Ret.data[i];
                    }
                }
            }

            result = true;
        exit_func:
            /* ステータス情報を更新する */
            Set_Result_Status();

            return result;
        }

        /**
         * @brief COMポートをオープンする
         * @retval true=成功
         * @retval false=失敗
         */
        private bool Comm_Open()
        {
            bool result = false; /* 結果 */

            /* COMポートオープン */
            if (Com.IsOpen == false)
            {
                try
                {
                    Com.PortName = ComPortName ?? throw new Exception();
                    Com.BaudRate = Convert.ToInt32(ComPortBand);
                    Com.DataBits = 8;
                    Com.StopBits = System.IO.Ports.StopBits.One;
                    Com.Parity = System.IO.Ports.Parity.None;
                    Com.ReadBufferSize = 4096;/*byte*/
                    Com.ReadTimeout = 500;/*ms*/
                    Com.WriteBufferSize = 2048;/*byte*/
                    Com.WriteTimeout = 500;/*ms*/
                    Com.RtsEnable = false;
                    Com.DtrEnable = false;
                    Com.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    goto exit_func;
                }
            }

            result = true;
        exit_func:
            return result;
        }

        /**
         * @brief COMポートをクローズする
         * @retval true=成功
         * @retval false=失敗
         */
        private bool Comm_Close()
        {
            bool result = false; /* 結果 */

            /* COMポートクローズ */
            if (Com.IsOpen == true)
            {
                try
                {
                    Com.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    goto exit_func;
                }
            }

            result = true;
        exit_func:
            return result;
        }

        /**
         * @brief サーボへパケットを送信する
         * @param func [in]RegWrite若しくはRegReadを指定
         * @param id [in]サーボID
         * @param addr [in]書き込むレジスタの先頭アドレス
         * @param data [in]書き込むデータ配列
         * @param dataLength [in]書き込むデータ配列長
         * @retval true=成功
         * @retval false=失敗
         */
        private bool Comm_Send(SendFunction func, byte id, byte addr, byte[] data, byte dataLength)
        {
            bool result = false;                /* 結果 */
            byte[] p = new byte[4 + 256 + 1];   /* パケットバッファ Header(1) ID(1) Address(1) Length(1) Data(MAX 256) CRC(1) */
            byte funcH;                         /* パケットヘッダー */
            int pLen = 0;                       /* パケットレングス */
            int i;                              /* カウンタ */

            funcH = (byte)func;    /* 返信ありID */
            if (id > 0x80)
            {
                funcH -= 8;        /* グループID/ブロードキャストID の場合は返信なしID */
            }

            switch (func)
            {
                case SendFunction.RegWrite:
                case SendFunction.RegWriteNoAck:
                    /* 書 Header(1) ID(1) Address(1) Length(1) Data(n)  CRC(1) */
                    p[pLen++] = funcH;
                    p[pLen++] = id;
                    p[pLen++] = dataLength;
                    p[pLen++] = addr;
                    for (i = 0; i < dataLength; i++)
                    {
                        p[pLen++] = data[i];
                    }
                    p[pLen] = Culc_Crc8(p, 0, pLen);
                    pLen++;
                    break;
                case SendFunction.RegRead:
                    /* 読 Header(1) ID(1) Address(1) Length(1) CRC(1) */
                    p[pLen++] = funcH;
                    p[pLen++] = id;
                    p[pLen++] = dataLength;
                    p[pLen++] = addr;
                    p[pLen] = Culc_Crc8(p, 0, pLen);
                    pLen++;
                    break;
                default:
                    goto exit_func;
            }

            /* COMポート送信 */
            if (Com.IsOpen == true)
            {
                try
                {
                    /* サーボ側は半二重通信なので、PC側が送信前に出している信号はノイズと思われるので破棄する */
                    Com.DiscardInBuffer();

                    Com.Write(p, 0, pLen);

                    /* サーボ側は半二重通信なので、PC側が送信直後の受信データは反射データと思われるので破棄する */
                    Com.DiscardInBuffer();

                    result = true;
                }
                catch
                {
                    AckResultLabelUpdate("COM Error");
                    goto exit_func;
                }
            }

            result = true;
        exit_func:
            return result;
        }

        /**
         * @brief サーボからパケットを受信する。
         * @param func [in]RegWrite若しくはRegReadを指定
         * @param id [in]サーボID
         * @param dataLength [in]読み込むデータ配列長
         * @retval true=成功
         * @retval false=失敗
         */
        private bool Comm_Recv(SendFunction func, byte dataLength)
        {
            bool result = false;                /* 結果 */
            byte[] p = new byte[2 + 256 + 1];   /* パケットバッファ ID(1) Flags(1) Data(MAX 256) CRC(1) */
            int pLen = 0;                       /* パケットレングス */
            byte crc8;                          /* crc8 */
            int pSz;                            /* 受信予定のパケットサイズ */
            bool isExistData;                   /* true = データ有り返信パケット */

            /* 返信なしFunctionHeaderで本処理を呼び出すことは無い */

            switch (func)
            {
                case SendFunction.RegWrite:
                case SendFunction.RegWriteNoAck:
                    /* ID(1) Flags(1) */
                    pSz = 1 + 1;
                    isExistData = false;
                    break;
                case SendFunction.RegRead:
                    /* ID(1) Flags(1) Data(n) CRC(1) */
                    pSz = 1 + 1 + dataLength + 1;
                    isExistData = true;
                    break;
                default:
                    goto exit_func;
            }

            try
            {   /* 既定のパケット長になるまでシリアルデータ読み取り */
                while (pLen < pSz)
                {
                    pLen += Com.Read(p, pLen, pSz - pLen);
                }
            }
            catch
            {
                /* 主にタイムアウト、他 */
                AckResultLabelUpdate("No response");
                goto exit_func;
            }

            if (isExistData == true)
            {   /* データ有り返信パケット */
                /* ID(1) Flags(1) Data(n) CRC(1) */
                crc8 = Culc_Crc8(p, 0, 2 + dataLength);
                if (crc8 == p[2 + dataLength])
                {
                    Ret.id = p[0];
                    Ret.f_Active = ((p[1] & 0x01) != 0);
                    Ret.f_InPosition = ((p[1] & 0x02) != 0);
                    Ret.f_Reserve1 = ((p[1] & 0x04) != 0);
                    Ret.f_Reserve2 = ((p[1] & 0x08) != 0);
                    Ret.f_HardError = ((p[1] & 0x10) != 0);
                    Ret.f_SoftError = ((p[1] & 0x20) != 0);
                    Ret.f_ComError = ((p[1] & 0x40) != 0);
                    Ret.f_InternalError = ((p[1] & 0x80) != 0);
                    Ret.dataLength = dataLength;
                    Array.Copy(p, 2, Ret.data, 0, Ret.dataLength);
                }
                else
                {
                    Ret.crcError = true;
                    goto exit_func;
                }
            }
            else
            {
                /* データ無し返信パケット */
                /* ID(1) Flags(1) */
                Ret.id = p[0];
                Ret.f_Active = ((p[1] & 0x01) != 0);
                Ret.f_InPosition = ((p[1] & 0x02) != 0);
                Ret.f_Reserve1 = ((p[1] & 0x04) != 0);
                Ret.f_Reserve2 = ((p[1] & 0x08) != 0);
                Ret.f_HardError = ((p[1] & 0x10) != 0);
                Ret.f_SoftError = ((p[1] & 0x20) != 0);
                Ret.f_ComError = ((p[1] & 0x40) != 0);
                Ret.f_InternalError = ((p[1] & 0x80) != 0);
            }

            result = true;
        exit_func:
            return result;
        }

        /**
         * @brief バイト配列からCRC8を計算する
         * @param data [in]計算するバイト配列データ
         * @param start [in]計算するバイト配列データの先頭インデックス
         * @param end [in]計算するバイト配列データの終端インデックス
         * @return byte CRC8
         * @note lookup_table CRC-8-Dallas/Maxim lookup table 多項式(x8 + x5 + x4 + 1)  反転 を使用してCRCを計算します。
         */
        private byte Culc_Crc8(byte[] data, int start, int end)
        {
            int i;
            byte crc8 = 0;
            byte[] lookup_table =
            {
#if false
            /* CRC-8-Dallas/Maxim lookup table 多項式(x8 + x5 + x4 + 1)  標準 */
            0x00,0x31,0x62,0x53,0xC4,0xF5,0xA6,0x97,0xB9,0x88,0xDB,0xEA,0x7D,0x4C,0x1F,0x2E,0x43,0x72,0x21,0x10,0x87,0xB6,0xE5,0xD4,0xFA,0xCB,0x98,0xA9,0x3E,0x0F,0x5C,0x6D,
            0x86,0xB7,0xE4,0xD5,0x42,0x73,0x20,0x11,0x3F,0x0E,0x5D,0x6C,0xFB,0xCA,0x99,0xA8,0xC5,0xF4,0xA7,0x96,0x01,0x30,0x63,0x52,0x7C,0x4D,0x1E,0x2F,0xB8,0x89,0xDA,0xEB,
            0x3D,0x0C,0x5F,0x6E,0xF9,0xC8,0x9B,0xAA,0x84,0xB5,0xE6,0xD7,0x40,0x71,0x22,0x13,0x7E,0x4F,0x1C,0x2D,0xBA,0x8B,0xD8,0xE9,0xC7,0xF6,0xA5,0x94,0x03,0x32,0x61,0x50,
            0xBB,0x8A,0xD9,0xE8,0x7F,0x4E,0x1D,0x2C,0x02,0x33,0x60,0x51,0xC6,0xF7,0xA4,0x95,0xF8,0xC9,0x9A,0xAB,0x3C,0x0D,0x5E,0x6F,0x41,0x70,0x23,0x12,0x85,0xB4,0xE7,0xD6,
            0x7A,0x4B,0x18,0x29,0xBE,0x8F,0xDC,0xED,0xC3,0xF2,0xA1,0x90,0x07,0x36,0x65,0x54,0x39,0x08,0x5B,0x6A,0xFD,0xCC,0x9F,0xAE,0x80,0xB1,0xE2,0xD3,0x44,0x75,0x26,0x17,
            0xFC,0xCD,0x9E,0xAF,0x38,0x09,0x5A,0x6B,0x45,0x74,0x27,0x16,0x81,0xB0,0xE3,0xD2,0xBF,0x8E,0xDD,0xEC,0x7B,0x4A,0x19,0x28,0x06,0x37,0x64,0x55,0xC2,0xF3,0xA0,0x91,
            0x47,0x76,0x25,0x14,0x83,0xB2,0xE1,0xD0,0xFE,0xCF,0x9C,0xAD,0x3A,0x0B,0x58,0x69,0x04,0x35,0x66,0x57,0xC0,0xF1,0xA2,0x93,0xBD,0x8C,0xDF,0xEE,0x79,0x48,0x1B,0x2A,
            0xC1,0xF0,0xA3,0x92,0x05,0x34,0x67,0x56,0x78,0x49,0x1A,0x2B,0xBC,0x8D,0xDE,0xEF,0x82,0xB3,0xE0,0xD1,0x46,0x77,0x24,0x15,0x3B,0x0A,0x59,0x68,0xFF,0xCE,0x9D,0xAC
#else
            /* CRC-8-Dallas/Maxim lookup table 多項式(x8 + x5 + x4 + 1)  反転 */
            0x00,0x5e,0xbc,0xe2,0x61,0x3f,0xdd,0x83,0xc2,0x9c,0x7e,0x20,0xa3,0xfd,0x1f,0x41,0x9d,0xc3,0x21,0x7f,0xfc,0xa2,0x40,0x1e,0x5f,0x01,0xe3,0xbd,0x3e,0x60,0x82,0xdc,
            0x23,0x7d,0x9f,0xc1,0x42,0x1c,0xfe,0xa0,0xe1,0xbf,0x5d,0x03,0x80,0xde,0x3c,0x62,0xbe,0xe0,0x02,0x5c,0xdf,0x81,0x63,0x3d,0x7c,0x22,0xc0,0x9e,0x1d,0x43,0xa1,0xff,
            0x46,0x18,0xfa,0xa4,0x27,0x79,0x9b,0xc5,0x84,0xda,0x38,0x66,0xe5,0xbb,0x59,0x07,0xdb,0x85,0x67,0x39,0xba,0xe4,0x06,0x58,0x19,0x47,0xa5,0xfb,0x78,0x26,0xc4,0x9a,
            0x65,0x3b,0xd9,0x87,0x04,0x5a,0xb8,0xe6,0xa7,0xf9,0x1b,0x45,0xc6,0x98,0x7a,0x24,0xf8,0xa6,0x44,0x1a,0x99,0xc7,0x25,0x7b,0x3a,0x64,0x86,0xd8,0x5b,0x05,0xe7,0xb9,
            0x8c,0xd2,0x30,0x6e,0xed,0xb3,0x51,0x0f,0x4e,0x10,0xf2,0xac,0x2f,0x71,0x93,0xcd,0x11,0x4f,0xad,0xf3,0x70,0x2e,0xcc,0x92,0xd3,0x8d,0x6f,0x31,0xb2,0xec,0x0e,0x50,
            0xaf,0xf1,0x13,0x4d,0xce,0x90,0x72,0x2c,0x6d,0x33,0xd1,0x8f,0x0c,0x52,0xb0,0xee,0x32,0x6c,0x8e,0xd0,0x53,0x0d,0xef,0xb1,0xf0,0xae,0x4c,0x12,0x91,0xcf,0x2d,0x73,
            0xca,0x94,0x76,0x28,0xab,0xf5,0x17,0x49,0x08,0x56,0xb4,0xea,0x69,0x37,0xd5,0x8b,0x57,0x09,0xeb,0xb5,0x36,0x68,0x8a,0xd4,0x95,0xcb,0x29,0x77,0xf4,0xaa,0x48,0x16,
            0xe9,0xb7,0x55,0x0b,0x88,0xd6,0x34,0x6a,0x2b,0x75,0x97,0xc9,0x4a,0x14,0xf6,0xa8,0x74,0x2a,0xc8,0x96,0x15,0x4b,0xa9,0xf7,0xb6,0xe8,0x0a,0x54,0xd7,0x89,0x6b,0x35
#endif
            };

            for (i = start; i < end; i++)
            {
                crc8 = lookup_table[crc8 ^ data[i]];
            }

            return crc8;
        }

        /**
         * @brief サイズ０のバイト配列を生成する
         * @return byte[] サイズ０のバイト配列
         */
        private byte[] GetEmptyArray()
        {
            return Array.Empty<byte>();
        }

        /**
         * @brief intの値をbyte[]配列に変換する
         * @param value [in]intの値
         * @param length [in]出力先のbyte[]配列の大きさ
         * @return byte[] リトルエンディアンであるバイト配列
         */
        private byte[] GetByteArray(int value, int length)
        {
            byte[] result = new byte[length];

            switch (length)
            {
                case 1:
                    result[0] = (byte)value;
                    break;
                case 2:
                    result = BitConverter.GetBytes((short)value);
                    break;
                case 4:
                    result = BitConverter.GetBytes(value);
                    break;
                default:
                    break;
            }

            return result;
        }

        /**
         * @brief byte[]配列をintに変換する
         * @param data [in]byte[]配列
         * @param length [in]byte[]配列の大きさ
         * @param Minus [in]true=signedデータに変換する / false=unsignedデータに変換する
         * @return int 値
         */
        private int GetIntValue(byte[] data, byte length, bool Minus)
        {
            int result = 0;

            switch (length)
            {
                case 1:
                    result = data[0];
                    if (Minus == true)
                    {
                        if (result >= 0x80)
                        {
                            result -= 0x100;
                        }
                    }
                    break;
                case 2:
                    result = (data[0]) | (data[1] << 8);
                    if (Minus == true)
                    {
                        if (result >= 0x8000)
                        {
                            result -= 0x10000;
                        }
                    }
                    break;
                case 4:
                    Int64 i64 = ((data[0]) | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
                    if (Minus == true)
                    {
                        if (i64 >= 0x80000000)
                        {
                            i64 -= 0x100000000;
                        }
                    }
                    result = (int)i64;
                    break;
                default:
                    break;
            }

            return result;
        }

        /** 
         * @brief ２つの配列データが同一かチェックする
         * @param s1 [in]チェックする１つ目の配列
         * @param s1Index [in]チェックする１つ目の配列の開始位置
         * @param s2 [in]チェックする２つ目の配列
         * @param s2Index [in]チェックする２つ目の配列の開始位置
         * @param length [in]チェックする配列長
         * @retval true=同じデータ
         * @retval false=異なるデータ
         */
        private bool ChkByteArray(byte[] s1, int s1Index, byte[] s2, int s2Index, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (s1[i + s1Index] != s2[i + s2Index])
                {
                    return false;
                }
            }

            return true;
        }

        /*-----------------------*/
        /* イベント */
        /*-----------------------*/

        /**
         * @brief コンストラクタ
         */
        public EditorForm()
        {
            InitializeComponent();

            /* EditorFormの表示内容を初期化 */
            Ui_Make_Init();
        }

        /**
         * @brief search COM Port, Band Rate の変更イベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Port_Chenge(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            /* COMポートクローズ */
            Comm_Close();

            ComPortName = ComPortComboBox.Text;
            ComPortBand = BandRateComboBox.Text;

            /* COMポートオープン */
            result = Comm_Open();
            if (result == true)
            {
                AckResultLabelUpdate("COM OK");
            }
            else
            {
                AckResultLabelUpdate("COM NG");
                goto exit_func;
            }

        exit_func:
            return;
        }

        /**
         * @brief AckResultLabel更新
         * @param value [in]string
         */
        private void AckResultLabelUpdate(string value)
        {
            try
            {
                Invoke(new Action<string>((v) =>
                {
                    AckResultLabel.Text = v;
                    AckResultLabel.Refresh();
                }), value);
            }
            catch { }
        }

        /**
         * @brief サーチ中のUI操作無効化
         * @param value [in]bool
         */
        private void UiUpdate(bool value)
        {
            try
            {
                Invoke(new Action<bool>((v) =>
                {
                    ComPortComboBox.Enabled = v;
                    BandRateComboBox.Enabled = v;
                    OperationGroupBox.Enabled = v;
                    ServoParameterGroupBox.Enabled = v;
                }), value);
            }
            catch { }
        }

        /**
         * @brief ServoIdNumericUpDown更新
         * @param value [in]decimal
         */
        private void ServoIdNumericUpDownUpdate(decimal value)
        {
            try
            {
                Invoke(new Action<decimal>((v) =>
                {
                    ServoIdNumericUpDown.Value = v;
                }), value);
            }
            catch { }
        }

        /**
         * @brief BandRateComboBox更新
         * @param value [in]string
         */
        private void BandRateComboBoxUpdate(string value)
        {
            try
            {
                Invoke(new Action<string>((v) =>
                {
                    BandRateComboBox.Text = v;
                    ComPortBand = v;
                }), value);
            }
            catch { }
        }

        /**
         * @brief search ID/Band Rateボタンのクリックイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         * @note 本機能は非標準機能であり標準では公開しません。十分な検証を行った上で実装検討を行ってください。
         */
        private void Servo_Search_Button_Click(object sender, EventArgs e)
        {
            if (Servo_Search == null)
            {
                Servo_Search = new Thread(new ThreadStart(Servo_Search_Thread));
            }
            if (Servo_Search != null)
            {
                if (Servo_Search.ThreadState == ThreadState.Unstarted)
                {
                    Servo_Search_Enable = true;
                    Servo_Search.Start();
                }
                else
                {
                    Servo_Search_Enable = false;
                }
            }
        }

        /**
         * @brief search ID/Band Rateを実行するスレッド
         */
        private void Servo_Search_Thread()
        {
            bool result;    /* 結果 */
            decimal ServoIdPreviousValue = ServoIdNumericUpDown.Value; /* サーボIDに設定されている現在値 */
            string[] ComPortSearchList = new string[ComPortBands.Length];

            /* search実行中、ボタン無効/パラメータ編集無効 */
            UiUpdate(false);

            /* サーチ中はPort＿Chengeイベントを停止しておく */
            ComPortComboBox.SelectedIndexChanged -= new EventHandler(Port_Chenge);
            BandRateComboBox.SelectedIndexChanged -= new EventHandler(Port_Chenge);

            /* 初回のBand Rate が 115200 のリスト作成 */
            {
                int count = 0;
                ComPortSearchList[count++] = ComPortBandIni;
                for (int i = 0; i < ComPortBands.Length; i++)
                {
                    if (ComPortBands[i] != ComPortBandIni)
                    {
                        ComPortSearchList[count++] = ComPortBands[i];
                    }
                }
            }

            /* 全Band Rate確認 */
            for (int BandRate = 0; BandRate < ComPortSearchList.Length; BandRate++)
            {
                /* Band Rateを変更し、COMを閉じて開きなおす */
                Comm_Close();
                BandRateComboBoxUpdate(ComPortSearchList[BandRate]);
                ComPortBand = ComPortSearchList[BandRate];
                Comm_Open();

                result = false;
                if (Com.IsOpen == true)
                {
                    /* サーボID全件確認 */
                    for (ServoId = (byte)ServoIdNumericUpDown.Minimum; ServoId <= (byte)ServoIdNumericUpDown.Maximum; ServoId++)
                    {
                        ServoIdNumericUpDownUpdate(ServoId);

                        /* No.64 Servo ID を確認する */
                        result = Reg_Read(ServoId, 64, 1);
                        if (result == true)
                        {
                            /* 読めた */
                            AckResultLabelUpdate("OK");
                            break;
                        }
                        if (Servo_Search_Enable == false)
                        {
                            /* キャンセルされた */
                            break;
                        }
                    }
                }

                if (result == true)
                {
                    break;
                }
                if (Servo_Search_Enable == false)
                {
                    ServoIdNumericUpDownUpdate(ServoIdPreviousValue);

                    /* Band Rate を115200に戻してCOMオープンし直し */
                    Comm_Close();
                    BandRateComboBoxUpdate(ComPortBandIni);
                    ComPortBand = ComPortBandIni;
                    Comm_Open();
                    AckResultLabelUpdate("Not Found");
                    break;
                }
            }
            if (ServoId > (byte)ServoIdNumericUpDown.Maximum)
            {
                /* 読めなかった */
                AckResultLabelUpdate("Not Found");
                ServoIdNumericUpDownUpdate(ServoIdPreviousValue);

                /* Band Rate を115200に戻してCOMオープンし直し */
                Comm_Close();
                BandRateComboBoxUpdate(ComPortBandIni);
                ComPortBand = ComPortBandIni;
                Comm_Open();
                goto exit_func;
            }

        exit_func:
            /* search完了 */
            UiUpdate(true);

            /* Port_Chengeイベント再開 */
            ComPortComboBox.SelectedIndexChanged += new EventHandler(Port_Chenge);
            BandRateComboBox.SelectedIndexChanged += new EventHandler(Port_Chenge);

            Servo_Search = null;
            return;
        }

        /**
         * @brief ServoIdNumericUpDownイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_ID_Numeric_UpDown_Change(object sender, EventArgs e)
        {
            NumericUpDown n = sender as NumericUpDown;
            if (n == null) return;

            ServoId = (byte)n.Value;
        }

        /**
         * @brief ServoIdNumericUpDownイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_ID_Select_Click(object sender, EventArgs e)
        {
            RadioButton b = sender as RadioButton;
            if (b == null) return;

            switch (b.Name)
            {
                case "IdSelect1RadioButton":
                    ServoIdNumericUpDown.Minimum = 1;
                    ServoIdNumericUpDown.Maximum = 127;
                    ServoIdNumericUpDown.Value = 1;
                    ServoIdNumericUpDown.Enabled = true;
                    SearchButton.Enabled = true;
                    ServoId = (byte)ServoIdNumericUpDown.Value;
                    AckButton.Enabled = true;
                    GetParametersButton.Enabled = true;
                    break;
                case "IdSelect2RadioButton":
                    ServoIdNumericUpDown.Minimum = 129;
                    ServoIdNumericUpDown.Maximum = 254;
                    ServoIdNumericUpDown.Value = 129;
                    ServoIdNumericUpDown.Enabled = true;
                    SearchButton.Enabled = false;
                    ServoId = (byte)ServoIdNumericUpDown.Value;
                    AckButton.Enabled = false;
                    GetParametersButton.Enabled = false;
                    break;
                case "IdSelect3RadioButton":
                    ServoIdNumericUpDown.Minimum = 255;
                    ServoIdNumericUpDown.Maximum = 255;
                    ServoIdNumericUpDown.Value = 255;
                    ServoIdNumericUpDown.Enabled = false;
                    SearchButton.Enabled = false;
                    ServoId = (byte)ServoIdNumericUpDown.Value;
                    AckButton.Enabled = false;
                    GetParametersButton.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        /**
         * @brief AckButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Ack_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            /* search実行中 */
            AckResultLabelUpdate("searching...");

            /* No.64 Servo ID を確認する */
            result = Reg_Read(ServoId, 64, 1);
            if (result == true)
            {
                /* 読めた */
                AckResultLabelUpdate("OK");
            }
            else
            {
                /* 読めなかった */
                AckResultLabelUpdate("Not Found");
            }
        }

        /**
         * @brief GetParametersButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Get_Parameters_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            /* サーボから全レジスタを取得しServo[]を更新する */

            /* サーボからの最大送信バイト数は、サーボ受信に合わせて127バイトまで */
            /* ID,フラグ，CRCを除くと最大124バイト */

            /* RAM 0x00～0x2F */
            result = Reg_Read(ServoId, 0x00, 48);
            if (result == false)
            {
                return;
            }

            /* ROM 0x40～0xA6 */
            result = Reg_Read(ServoId, 0x40, 103);
            if (result == false)
            {
                return;
            }

            /* ROM 0xB0～0xBF */
            result = Reg_Read(ServoId, 0xB0, 16);
            if (result == false)
            {
                return;
            }

            /* TargetAngleTrackBarの値を更新する */
            /*
            byte[] TrackBarData = new byte[4];
            Array.Copy(Servo, 0, TrackBarData, 0, 4);
            TargetAngleTrackBar.Value = GetIntValue(TrackBarData, 4, true);
            */

            /* ModelNumberParameterLabelの値を更新する */
            byte[] ModelData = new byte[5];
            Array.Copy(Servo, 176, ModelData, 0, 5);
            ModelNumberParameterLabel.Text = BitConverter.ToString(ModelData).Replace("-", string.Empty);

            /* FirmwareVersionParameterLabelの値を更新する */
            byte[] FirmData = new byte[2];
            Array.Copy(Servo, 181, FirmData, 0, 2);
            FirmwareVersionParameterLabel.Text = GetIntValue(FirmData, 2, false).ToString();

            /* UniqueNumberParameterLabelの値を更新する */
            byte[] UniqueData = new byte[4];
            Array.Copy(Servo, 183, UniqueData, 0, 4);
            UniqueNumberParameterLabel.Text = GetIntValue(UniqueData, 4, false).ToString();

            /* ManufactureDateParameterLabelの値を更新する */
            string MunufactureData = string.Format("{0:D2}/{1:D2}/{2:D2} {3:D2}:{4:D2}"
                , Servo[187], Servo[188], Servo[189], Servo[190], Servo[191]);
            ManufactureDateParameterLabel.Text = MunufactureData;

            /* CurrentControlFormLabelの値を更新する */
            if (Servo[69] == 1)
            {
                string CurrentControl = string.Format("Vector");
                CurrentControlFormLabel.Text = CurrentControl;
            }
            else
            {
                string CurrentControl = string.Format("Square Wave");
                CurrentControlFormLabel.Text = CurrentControl;
            }

            /* 個別コントロールの値を更新する */
            for (int i = 0; i < RegMap.Length; i++)
            {
                string[] prm = RegMap[i].Split(',');
                prm[0] = prm[0].Replace("0x", ""); /* 16進数表現 0xを外す */
                string RegName = "_" + prm[0] + "_" + prm[1] + "_" + prm[10]; /* コントロールの名前 _Addr_Size_Base */
                byte addr = Convert.ToByte(prm[0], 16);
                byte dataLength = Convert.ToByte(prm[1]);
                byte[] data = new byte[dataLength];
                Control[] RegControl = this.Controls.Find(RegName, true);

                switch (RegControl[0].GetType().Name)
                {
                    case "NumericUpDown":
                        NumericUpDown Reg10 = RegControl[0] as NumericUpDown;
                        if (Reg10 == null) break;
                        Array.Copy(Servo, addr, data, 0, dataLength);
                        Reg10.Value = GetIntValue(data, dataLength, Reg10.Minimum < 0);
                        break;
                    case "TextBox":
                        TextBox Reg16 = RegControl[0] as TextBox;
                        if (Reg16 == null) break;
                        if (dataLength == 1)
                        {
                            Array.Copy(Servo, addr, data, 0, dataLength);
                            Reg16.Text = BitConverter.ToString(data).Replace("-", string.Empty);
                        }
                        else if (dataLength == 2)
                        {
                            Array.Copy(Servo, addr + 0, data, 1, 1);
                            Array.Copy(Servo, addr + 1, data, 0, 1);
                            Reg16.Text = BitConverter.ToString(data).Replace("-", string.Empty);
                        }
                        else
                        {
                            Array.Copy(Servo, addr + 0, data, 3, 1);
                            Array.Copy(Servo, addr + 1, data, 2, 1);
                            Array.Copy(Servo, addr + 2, data, 1, 1);
                            Array.Copy(Servo, addr + 3, data, 0, 1);
                            Reg16.Text = BitConverter.ToString(data).Replace("-", string.Empty);
                        }
                        break;
                    case "ComboBox":
                        ComboBox RegSel = RegControl[0] as ComboBox;
                        if (RegSel == null) break;
                        Array.Copy(Servo, addr, data, 0, dataLength);
                        RegSel.SelectedIndex = GetIntValue(data, dataLength, false);
                        break;
                    default:
                        break;
                }
            }
        }

        /**
         * @brief InitializeServoButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Initialize_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            Servo[32] = 1;

            /* No.32 Initialize へ書き込む */
            result = Reg_Write(ServoId, 32, 1);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }

            Servo[32] = 0;
        }

        /**
         * @brief WriteFlashRomButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Write_FlashRom_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            Servo[34] = 1;

            /* No.34 Write FlashROM へ書き込む */
            result = Reg_Write(ServoId, 34, 1);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }

            Servo[34] = 0;
        }

        /**
         * @brief RebootButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Reboot_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            Servo[33] = 1;

            /* No.33 Reboot へ書き込む */
            result = Reg_Write(ServoId, 33, 1);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }

            Servo[33] = 0;
        }

        /**
         * @brief SleepButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         * @note 本機能は非標準機能であり標準では公開しません。十分な検証を行った上で実装検討を行ってください。
         */
        private void Servo_Sleep_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            Servo[35] = 1;

            /* No.35 Reboot へ書き込む */
            result = Reg_Write(ServoId, 35, 1);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }

            Servo[35] = 0;
        }

        /**
        * @brief TargetAngleTrackBarイベント
        * @param sender [in]object
        * @param e [in]EventArgs
        */
        private void Servo_Set_Target_Angle_Change(object sender, EventArgs e)
        {
            int value;                      /* 入力データの int表現 */
            byte[] targetAngleValue;        /* 入力データの byte[4]表現 */

            if (sender == null) return;

            Control[] RegControl = this.Controls.Find("_00_4_10", true); /* Reg No.0 */
            NumericUpDown Reg0 = RegControl[0] as NumericUpDown;

            switch (sender.GetType().Name)
            {
                case "TrackBar":
                    bool result;    /* 結果 */

                    /* 入力データを取得 */
                    value = TargetAngleTrackBar.Value;
                    targetAngleValue = GetByteArray(value, 4);

                    if (ChkByteArray(targetAngleValue, 0, Servo, 0, 4) == true)
                    {   /* トラックバーの変化なしならここで終了 */
                        break;
                    }

                    for (int i = 0; i < 4; i++)
                    {   /* 現在値更新 */
                        Servo[i + 0] = targetAngleValue[i];
                    }

                    /* 同じレジスタを参照するコントロールと同期 */
                    TargetAngleNumericUpDown.Value = value;
                    if (Reg0 != null)
                    {
                        Reg0.Value = value;
                    }

                    /* No.0 Target Angle 現在値を書き込む */
                    result = Reg_Write(ServoId, 0, 4);
                    if (result == true)
                    {
                        /* 書けた */
                    }
                    else
                    {
                        /* 書けなかった */
                    }

                    /* パケット間に1byte分以上のウェイト時間を設ける。このウェイト時間によりパケットの開始/終了を判断する。 */
                    Thread.Sleep(2);

                    break;
                case "NumericUpDown":
                    NumericUpDown ctrl = sender as NumericUpDown;
                    if (ctrl == null) break;
                    if (ctrl.Name == "TargetAngleNumericUpDown")
                    {
                        /* 入力データを取得 */
                        value = (int)TargetAngleNumericUpDown.Value;
                        targetAngleValue = GetByteArray(value, 4);

                        for (int i = 0; i < 4; i++)
                        {   /* 現在値更新 */
                            Servo[i + 0] = targetAngleValue[i];
                        }

                        /* 同じレジスタを参照するコントロールと同期 */
                        if ((value >= TargetAngleTrackBar.Minimum) && (value <= TargetAngleTrackBar.Maximum))
                        {
                            TargetAngleTrackBar.Value = value;
                        }
                        if (Reg0 != null)
                        {
                            Reg0.Value = value;
                        }
                    }
                    else if (ctrl.Name == "_00_4_10") /* Reg No.0 */
                    {
                        if (Reg0 != null)
                        {
                            /* 入力データを取得 */
                            value = (int)Reg0.Value;
                            targetAngleValue = GetByteArray(value, 4);

                            for (int i = 0; i < 4; i++)
                            {   /* 現在値更新 */
                                Servo[i + 0] = targetAngleValue[i];
                            }

                            /* 同じレジスタを参照するコントロールと同期 */
                            if ((value >= TargetAngleTrackBar.Minimum) && (value <= TargetAngleTrackBar.Maximum))
                            {
                                TargetAngleTrackBar.Value = value;
                                TargetAngleNumericUpDown.Value = value;
                            }
                        }
                    }
                    else { }
                    break;
                default:
                    break;
            }
        }

        /**
         * @brief MoveSxButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_MoveSx_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            /* No.0 Target Angle 現在値を書き込む */
            result = Reg_Write(ServoId, 0, 4);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }
        }

        /**
         * @brief RepeatMoveSxButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */

        private void Servo_RepeatMoveSx_Button_Click(object sender, EventArgs e)
        {
            if (RepeatMoveSxButton.Checked)
            {
                StartProcessing();
            }
            else
            {
                StopProcessing();
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private void StartProcessing()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            int interval = (int)numericUpDownInterval.Value * 100; // 秒単位からミリ秒単位に変換

            int value;                      /* 入力データの int表現 */
            byte[] targetAngleValue;        /* 入力データの byte[4]表現 */
            int value_minus;                      /* 入力データの int表現 */
            byte[] targetAngleValue_minus;        /* 入力データの byte[4]表現 */
            bool result;    /* 結果 */

            /* 入力データを取得 */
            value = TargetAngleTrackBar.Value;
            value_minus = -1 * TargetAngleTrackBar.Value;
            targetAngleValue = GetByteArray(value, 4);
            targetAngleValue_minus = GetByteArray(value_minus, 4);

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    // 特定の処理をここに記述
                    for (int i = 0; i < 4; i++)
                    {   /* 現在値更新 */
                        Servo[i + 0] = targetAngleValue[i];
                    }

                    /* No.0 Target Angle 現在値を書き込む */
                    result = Reg_Write(ServoId, 0, 4);
                    if (result == true)
                    {
                        /* 書けた */
                    }
                    else
                    {
                        /* 書けなかった */
                    }

                    Thread.Sleep(interval);

                    for (int i = 0; i < 4; i++)
                    {   /* 現在値更新 */
                        Servo[i + 0] = targetAngleValue_minus[i];
                    }

                    /* No.0 Target Angle 現在値を書き込む */
                    result = Reg_Write(ServoId, 0, 4);
                    if (result == true)
                    {
                        /* 書けた */
                    }
                    else
                    {
                        /* 書けなかった */
                    }

                    Thread.Sleep(interval);
                }
            }, token);
        }
        private void StopProcessing()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        /**
         * @brief RepeatMoveSxButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */

        private void Servo_RepeatSpeed_Button_Click(object sender, EventArgs e)
        {
            if (RepeatSpeedButton.Checked)
            {
                StartSpeedProcessing();
            }
            else
            {
                StopProcessing();
                if (speedoff==true)
                {
                    speedoff = false;
                    for (int i = 0; i < 2; i++)
                    {   /* 現在値更新 */
                        Servo[i + 4] = 0;
                    }
                    Reg_Write(ServoId, 4, 2);
                }
            }
        }

        private void StartSpeedProcessing()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            int interval = (int)numericUpDownInterval.Value * 100; // 秒単位からミリ秒単位に変換

            int value;                      /* 入力データの int表現 */
            byte[] targetAngleValue;        /* 入力データの byte[4]表現 */
            int value_minus;                      /* 入力データの int表現 */
            byte[] targetAngleValue_minus;        /* 入力データの byte[4]表現 */
            bool result;    /* 結果 */

            /* 入力データを取得 */
            value = TargetAngleTrackBar.Value;
            value_minus = -1 * TargetAngleTrackBar.Value;
            targetAngleValue = GetByteArray(value, 2);
            targetAngleValue_minus = GetByteArray(value_minus, 2);

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    speedoff = true;

                    // 特定の処理をここに記述
                    for (int i = 0; i < 2; i++)
                    {   /* 現在値更新 */
                        Servo[i + 4] = targetAngleValue[i];
                    }
                    result = Reg_Write(ServoId, 4, 2);
                    if (result == true)
                    {
                        /* 書けた */
                    }
                    else
                    {
                        /* 書けなかった */
                    }
                    Thread.Sleep(interval);

                    if (speedoff == true)
                    {
                        for (int i = 0; i < 2; i++)
                        {   /* 現在値更新 */
                            Servo[i + 4] = targetAngleValue_minus[i];
                        }
                        result = Reg_Write(ServoId, 4, 2);
                        if (result == true)
                        {
                            /* 書けた */
                        }
                        else
                        {
                            /* 書けなかった */
                        }
                        Thread.Sleep(interval);
                    }
                }
            }, token);
        }

        /**
         * @brief Set0Buttonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Set0_Button_Click(object sender, EventArgs e)
        {
            bool result;    /* 結果 */

            /* Set 0 deg */
            for (int i = 0; i < 4; i++)
            {
                Servo[i] = 0;
            }

            /* トラックバー */
            TargetAngleTrackBar.Value = 0;

            /* 数値入力 */
            TargetAngleNumericUpDown.Value = 0;

            /* Reg No.0 */
            Control[] RegControl = this.Controls.Find("_00_4_10", true);
            NumericUpDown Reg0 = RegControl[0] as NumericUpDown;
            if (Reg0 != null)
            {
                Reg0.Value = 0;
            }

            /* No.0 Target Angle へ書き込む */
            result = Reg_Write(ServoId, 0, 4);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }
        }

        /**
         * @brief サーボパラメータの各コントロールから離れたときに共有データServo[]へ値を取り込む
         * @param sender [in]このイベントを発生させたコントロール
         * @param length [in]引数
         */
        private void Servo_Paramater_Leave(object sender, EventArgs e)
        {
            string[] RegName;       /* コントロールの名前 _Addr_Size_Base */
            int addr;               /* レジスタアドレス */
            int size;               /* レジスタサイズ */
            int value;              /* レジスタの値 */
            byte[] reg;             /* レジスタの値をbyte配列に変換した結果 */
            string ini = "", min = "", max = "";

            if (sender == null) return;

            switch (sender.GetType().Name)
            {
                case "NumericUpDown":
                    NumericUpDown Reg10 = sender as NumericUpDown;
                    if (Reg10 == null) return;
                    RegName = Reg10.Name.Split('_');
                    addr = Convert.ToInt32(RegName[1], 16);
                    size = Convert.ToInt32(RegName[2]);

                    for (int i = 0; i < RegMap.Length; i++)
                    {   /* 設定を取得 */
                        string[] prm = RegMap[i].Split(',');
                        prm[0] = prm[0].Replace("0x", ""); /* 16進数表現 0xを外す */
                        if (RegName[1] == prm[0])
                        {
                            ini = prm[6]; /* 初期値 */
                            min = prm[7]; /* min */
                            max = prm[8]; /* max */
                            break;
                        }
                    }

                    if (Reg10.Text == "")
                    {   /* 値が設定されていない場合初期値を与える */
                        Reg10.Text = ini;
                    }
                    else if (Reg10.Value < Convert.ToInt32(min))
                    {   /* 最小値以下の場合 */
                        Reg10.Text = min;
                    }
                    else if (Reg10.Value > Convert.ToInt32(max))
                    {   /* 最大値以上の場合 */
                        Reg10.Text = max;
                    }
                    else { }

                    value = (int)Reg10.Value;

                    break;
                case "TextBox":
                    TextBox Reg16 = sender as TextBox;
                    if (Reg16 == null) return;
                    RegName = Reg16.Name.Split('_');
                    addr = Convert.ToInt32(RegName[1], 16);
                    size = Convert.ToInt32(RegName[2]);

                    for (int i = 0; i < RegMap.Length; i++)
                    {   /* 設定を取得 */
                        string[] prm = RegMap[i].Split(',');
                        prm[0] = prm[0].Replace("0x", ""); /* 16進数表現 0xを外す */
                        if (RegName[1] == prm[0])
                        {
                            ini = prm[6].Replace("0x", ""); /* 初期値 */
                            min = prm[7].Replace("0x", ""); /* min */
                            max = prm[8].Replace("0x", ""); /* max */
                            break;
                        }
                    }

                    if (Reg16.Text == "")
                    {   /* 値が設定されていない場合初期値を与える */
                        Reg16.Text = ini;
                    }
                    else if (Convert.ToInt32(Reg16.Text, 16) < Convert.ToInt32(min, 16))
                    {   /* 最小値以下の場合 */
                        Reg16.Text = min;
                    }
                    else if (Convert.ToInt32(Reg16.Text, 16) > Convert.ToInt32(max, 16))
                    {   /* 最大値以上の場合 */
                        Reg16.Text = max;
                    }
                    else { }

                    value = Convert.ToInt32(Reg16.Text, 16);

                    break;
                case "ComboBox":
                    ComboBox RegSel = sender as ComboBox;
                    if (RegSel == null) return;
                    RegName = RegSel.Name.Split('_');
                    addr = Convert.ToInt32(RegName[1], 16);
                    size = Convert.ToInt32(RegName[2]);
                    value = RegSel.SelectedIndex;
                    break;
                default:
                    return;
            }

            reg = GetByteArray(value, size);

            for (int i = 0; i < size; i++)
            {
                Servo[addr + i] = reg[i];
            }
        }
        /**
          * @brief テキストボックスを１６進数入力に都合の良いキーコードに限定する
          * @param sender [in]このイベントを発生させたコントロール
          * @param length [in]引数
          */
        private void Servo_Paramater_16Base_KeyPress(object sender, KeyEventArgs e)
        {
            if (((e.KeyValue >= '0') && (e.KeyValue <= '9'))
             || ((e.KeyValue >= 'A') && (e.KeyValue <= 'F'))
             || ((e.KeyValue >= 'a') && (e.KeyValue <= 'f'))
             || ((e.KeyValue >= (int)Keys.NumPad0) && (e.KeyValue <= (int)Keys.NumPad9))
             || ((e.KeyValue == (int)Keys.Left))
             || ((e.KeyValue == (int)Keys.Right))
             || ((e.KeyValue == (int)Keys.Up))
             || ((e.KeyValue == (int)Keys.Down))
             || ((e.KeyValue == (int)Keys.Home))
             || ((e.KeyValue == (int)Keys.End))
             || ((e.KeyValue == (int)Keys.Back))
             || ((e.KeyValue == (int)Keys.Delete)))
            {
                e.SuppressKeyPress = false; /* 許可する */
            }
            else
            {
                e.SuppressKeyPress = true; /* 許可しない */
            }
        }

        /**
         * @brief 各レジスタのSetボタンイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Set_Click(object sender, EventArgs e)
        {
            Button SetButton = sender as Button;
            if (SetButton == null) return;

            bool result;                                /* 結果 */
            string[] prm = SetButton.Name.Split('_');   /* _Addr_Size_Base */
            byte addr = Convert.ToByte(prm[1], 16);     /* Addr */
            byte dataLength = Convert.ToByte(prm[2]);   /* Size */

            /* 書き込む */
            result = Reg_Write(ServoId, addr, dataLength);
            if (result == true)
            {
                /* 書けた */
            }
            else
            {
                /* 書けなかった */
            }
        }

        /**
         * @brief SetRamButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Set_RAM_Click(object sender, EventArgs e)
        {
            bool result;                    /* 結果 */
            int RamStart = -1, RamEnd = -1; /* RAMのスタートアドレスとエンドアドレス */

            for (int i = 0; i < RegMap.Length; i++)
            {   /* RAMのスタートアドレスとエンドアドレスを取得 */
                string[] prm = RegMap[i].Split(',');
                prm[0] = prm[0].Replace("0x", ""); /* 16進数表現 0xを外す */
                int Addr = Convert.ToInt32(prm[0], 16);
                int Size = Convert.ToInt32(prm[1], 10);

                if ((prm[2] == "RAM") && ((prm[5] == "R/W") || (prm[5] == "W")))
                {
                    if (RamStart == -1)
                    {
                        RamStart = Addr;
                    }
                    RamEnd = Addr + Size - 1;
                }
            }

            if ((RamStart != -1) && (RamEnd != -1))
            {
                byte dataLength = (byte)(RamEnd - RamStart + 1); /* 書き込むデータ長 */

                /* 書き込む */
                result = Reg_Write(ServoId, (byte)RamStart, dataLength);
                if (result == true)
                {
                    /* 書けた */
                }
                else
                {
                    /* 書けなかった */
                }
            }
        }

        /**
         * @brief SetRomButtonイベント
         * @param sender [in]object
         * @param e [in]EventArgs
         */
        private void Servo_Set_ROM_Click(object sender, EventArgs e)
        {
            bool result;                    /* 結果 */
            int RomStart = -1, RomEnd = -1; /* ROMのスタートアドレスとエンドアドレス */

            for (int i = 0; i < RegMap.Length; i++)
            {   /* ROMのスタートアドレスとエンドアドレスを取得 */
                string[] prm = RegMap[i].Split(',');
                prm[0] = prm[0].Replace("0x", ""); /* 16進数表現 0xを外す */
                int Addr = Convert.ToInt32(prm[0], 16);
                int Size = Convert.ToInt32(prm[1], 10);

                if ((prm[2] == "ROM") && ((prm[5] == "R/W") || (prm[5] == "W")))
                {
                    if (RomStart == -1)
                    {
                        RomStart = Addr;
                    }
                    RomEnd = Addr + Size - 1;
                }
            }

            if ((RomStart != -1) && (RomEnd != -1))
            {
                byte dataLength = (byte)(RomEnd - RomStart + 1); /* 書き込むデータ長 */

                /* 書き込む */
                result = Reg_Write(ServoId, (byte)RomStart, dataLength);
                if (result == true)
                {
                    /* 書けた */
                }
                else
                {
                    /* 書けなかった */
                }
            }
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {

        }
    }
}

/**/

