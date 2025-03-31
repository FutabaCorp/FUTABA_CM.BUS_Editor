namespace CM_BUS_Editor
{
    partial class EditorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PortSettingGroupBox = new System.Windows.Forms.GroupBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.BandRateComboBox = new System.Windows.Forms.ComboBox();
            this.BandRateLabel = new System.Windows.Forms.Label();
            this.ComPortComboBox = new System.Windows.Forms.ComboBox();
            this.ComPortlabel = new System.Windows.Forms.Label();
            this.OperationGroupBox = new System.Windows.Forms.GroupBox();
            this.IdSelect3RadioButton = new System.Windows.Forms.RadioButton();
            this.IdSelect2RadioButton = new System.Windows.Forms.RadioButton();
            this.IdSelect1RadioButton = new System.Windows.Forms.RadioButton();
            this.SleepButton = new System.Windows.Forms.Button();
            this.RebootButton = new System.Windows.Forms.Button();
            this.Set0Button = new System.Windows.Forms.Button();
            this.MoveSxButton = new System.Windows.Forms.Button();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.RepeatMoveSxButton = new System.Windows.Forms.CheckBox();
            this.RepeatSpeedButton = new System.Windows.Forms.CheckBox();
            this.TargetAngleNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TargetAngleMinLabel = new System.Windows.Forms.Label();
            this.TargetAngleMaxLabel = new System.Windows.Forms.Label();
            this.TargetAngle0Label = new System.Windows.Forms.Label();
            this.TargetAngleTrackBar = new System.Windows.Forms.TrackBar();
            this.WriteFlashRomButton = new System.Windows.Forms.Button();
            this.InitializeServoButton = new System.Windows.Forms.Button();
            this.GetParametersButton = new System.Windows.Forms.Button();
            this.AckResultLabel = new System.Windows.Forms.Label();
            this.AckButton = new System.Windows.Forms.Button();
            this.ServoIdNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ServoIdLabel = new System.Windows.Forms.Label();
            this.ServoParameterGroupBox = new System.Windows.Forms.GroupBox();
            this.LabelsPanel = new System.Windows.Forms.Panel();
            this.HeaderLabel4 = new System.Windows.Forms.Label();
            this.HeaderLabel3 = new System.Windows.Forms.Label();
            this.HeaderLabel2 = new System.Windows.Forms.Label();
            this.HeaderLabel1 = new System.Windows.Forms.Label();
            this.HeaderLabel0 = new System.Windows.Forms.Label();
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.SetRomButton = new System.Windows.Forms.Button();
            this.SetRamButton = new System.Windows.Forms.Button();
            this.StatusGroupBox = new System.Windows.Forms.GroupBox();
            this.Result5Label = new System.Windows.Forms.Label();
            this.Result4Label = new System.Windows.Forms.Label();
            this.Result3Label = new System.Windows.Forms.Label();
            this.Result2Label = new System.Windows.Forms.Label();
            this.Result1Label = new System.Windows.Forms.Label();
            this.ManufactureDateParameterLabel = new System.Windows.Forms.Label();
            this.CurrentControlFormLabel = new System.Windows.Forms.Label();
            this.UniqueNumberParameterLabel = new System.Windows.Forms.Label();
            this.FirmwareVersionParameterLabel = new System.Windows.Forms.Label();
            this.ModelNumberParameterLabel = new System.Windows.Forms.Label();
            this.ManufactureDateLabel = new System.Windows.Forms.Label();
            this.CurrentControlLabel = new System.Windows.Forms.Label();
            this.UniqueNumberLabel = new System.Windows.Forms.Label();
            this.FirmwareVersionLabel = new System.Windows.Forms.Label();
            this.ModelNumberLabel = new System.Windows.Forms.Label();
            this.Result6Label = new System.Windows.Forms.Label();
            this.FormBottomRightLabel = new System.Windows.Forms.Label();
            this.PortSettingGroupBox.SuspendLayout();
            this.OperationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetAngleNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetAngleTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServoIdNumericUpDown)).BeginInit();
            this.ServoParameterGroupBox.SuspendLayout();
            this.LabelsPanel.SuspendLayout();
            this.StatusGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PortSettingGroupBox
            // 
            this.PortSettingGroupBox.Controls.Add(this.SearchButton);
            this.PortSettingGroupBox.Controls.Add(this.BandRateComboBox);
            this.PortSettingGroupBox.Controls.Add(this.BandRateLabel);
            this.PortSettingGroupBox.Controls.Add(this.ComPortComboBox);
            this.PortSettingGroupBox.Controls.Add(this.ComPortlabel);
            this.PortSettingGroupBox.Location = new System.Drawing.Point(15, 8);
            this.PortSettingGroupBox.Margin = new System.Windows.Forms.Padding(51, 24, 51, 24);
            this.PortSettingGroupBox.Name = "PortSettingGroupBox";
            this.PortSettingGroupBox.Padding = new System.Windows.Forms.Padding(51, 24, 51, 24);
            this.PortSettingGroupBox.Size = new System.Drawing.Size(194, 165);
            this.PortSettingGroupBox.TabIndex = 1;
            this.PortSettingGroupBox.TabStop = false;
            this.PortSettingGroupBox.Text = "Port Setting";
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(8, 112);
            this.SearchButton.Margin = new System.Windows.Forms.Padding(51, 24, 51, 24);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(179, 35);
            this.SearchButton.TabIndex = 3;
            this.SearchButton.Text = "search ID/Band Rate";
            this.SearchButton.UseVisualStyleBackColor = true;
            // 
            // BandRateComboBox
            // 
            this.BandRateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BandRateComboBox.FormattingEnabled = true;
            this.BandRateComboBox.Location = new System.Drawing.Point(89, 70);
            this.BandRateComboBox.Margin = new System.Windows.Forms.Padding(51, 24, 51, 24);
            this.BandRateComboBox.Name = "BandRateComboBox";
            this.BandRateComboBox.Size = new System.Drawing.Size(96, 25);
            this.BandRateComboBox.TabIndex = 2;
            // 
            // BandRateLabel
            // 
            this.BandRateLabel.AutoSize = true;
            this.BandRateLabel.Location = new System.Drawing.Point(8, 74);
            this.BandRateLabel.Margin = new System.Windows.Forms.Padding(51, 0, 51, 0);
            this.BandRateLabel.Name = "BandRateLabel";
            this.BandRateLabel.Size = new System.Drawing.Size(77, 17);
            this.BandRateLabel.TabIndex = 0;
            this.BandRateLabel.Text = "Band Rate";
            // 
            // ComPortComboBox
            // 
            this.ComPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComPortComboBox.FormattingEnabled = true;
            this.ComPortComboBox.Location = new System.Drawing.Point(89, 26);
            this.ComPortComboBox.Margin = new System.Windows.Forms.Padding(51, 24, 51, 24);
            this.ComPortComboBox.Name = "ComPortComboBox";
            this.ComPortComboBox.Size = new System.Drawing.Size(96, 25);
            this.ComPortComboBox.TabIndex = 1;
            // 
            // ComPortlabel
            // 
            this.ComPortlabel.AutoSize = true;
            this.ComPortlabel.Location = new System.Drawing.Point(6, 31);
            this.ComPortlabel.Margin = new System.Windows.Forms.Padding(51, 0, 51, 0);
            this.ComPortlabel.Name = "ComPortlabel";
            this.ComPortlabel.Size = new System.Drawing.Size(73, 17);
            this.ComPortlabel.TabIndex = 0;
            this.ComPortlabel.Text = "COM Port";
            // 
            // OperationGroupBox
            // 
            this.OperationGroupBox.Controls.Add(this.IdSelect3RadioButton);
            this.OperationGroupBox.Controls.Add(this.IdSelect2RadioButton);
            this.OperationGroupBox.Controls.Add(this.IdSelect1RadioButton);
            this.OperationGroupBox.Controls.Add(this.SleepButton);
            this.OperationGroupBox.Controls.Add(this.RebootButton);
            this.OperationGroupBox.Controls.Add(this.Set0Button);
            this.OperationGroupBox.Controls.Add(this.MoveSxButton);
            this.OperationGroupBox.Controls.Add(this.numericUpDownInterval);
            this.OperationGroupBox.Controls.Add(this.RepeatMoveSxButton);
            this.OperationGroupBox.Controls.Add(this.RepeatSpeedButton);
            this.OperationGroupBox.Controls.Add(this.TargetAngleNumericUpDown);
            this.OperationGroupBox.Controls.Add(this.TargetAngleMinLabel);
            this.OperationGroupBox.Controls.Add(this.TargetAngleMaxLabel);
            this.OperationGroupBox.Controls.Add(this.TargetAngle0Label);
            this.OperationGroupBox.Controls.Add(this.TargetAngleTrackBar);
            this.OperationGroupBox.Controls.Add(this.WriteFlashRomButton);
            this.OperationGroupBox.Controls.Add(this.InitializeServoButton);
            this.OperationGroupBox.Controls.Add(this.GetParametersButton);
            this.OperationGroupBox.Controls.Add(this.AckResultLabel);
            this.OperationGroupBox.Controls.Add(this.AckButton);
            this.OperationGroupBox.Controls.Add(this.ServoIdNumericUpDown);
            this.OperationGroupBox.Controls.Add(this.ServoIdLabel);
            this.OperationGroupBox.Location = new System.Drawing.Point(215, 8);
            this.OperationGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.OperationGroupBox.Name = "OperationGroupBox";
            this.OperationGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.OperationGroupBox.Size = new System.Drawing.Size(962, 165);
            this.OperationGroupBox.TabIndex = 2;
            this.OperationGroupBox.TabStop = false;
            this.OperationGroupBox.Text = "Operation";
            // 
            // IdSelect3RadioButton
            // 
            this.IdSelect3RadioButton.AutoSize = true;
            this.IdSelect3RadioButton.Location = new System.Drawing.Point(14, 125);
            this.IdSelect3RadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.IdSelect3RadioButton.Name = "IdSelect3RadioButton";
            this.IdSelect3RadioButton.Size = new System.Drawing.Size(114, 21);
            this.IdSelect3RadioButton.TabIndex = 4;
            this.IdSelect3RadioButton.TabStop = true;
            this.IdSelect3RadioButton.Text = "Broadcast ID";
            this.IdSelect3RadioButton.UseVisualStyleBackColor = true;
            // 
            // IdSelect2RadioButton
            // 
            this.IdSelect2RadioButton.AutoSize = true;
            this.IdSelect2RadioButton.Location = new System.Drawing.Point(14, 96);
            this.IdSelect2RadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.IdSelect2RadioButton.Name = "IdSelect2RadioButton";
            this.IdSelect2RadioButton.Size = new System.Drawing.Size(87, 21);
            this.IdSelect2RadioButton.TabIndex = 3;
            this.IdSelect2RadioButton.TabStop = true;
            this.IdSelect2RadioButton.Text = "Group ID";
            this.IdSelect2RadioButton.UseVisualStyleBackColor = true;
            // 
            // IdSelect1RadioButton
            // 
            this.IdSelect1RadioButton.AutoSize = true;
            this.IdSelect1RadioButton.Location = new System.Drawing.Point(14, 68);
            this.IdSelect1RadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.IdSelect1RadioButton.Name = "IdSelect1RadioButton";
            this.IdSelect1RadioButton.Size = new System.Drawing.Size(92, 21);
            this.IdSelect1RadioButton.TabIndex = 2;
            this.IdSelect1RadioButton.TabStop = true;
            this.IdSelect1RadioButton.Text = "Unique ID";
            this.IdSelect1RadioButton.UseVisualStyleBackColor = true;
            // 
            // SleepButton
            // 
            this.SleepButton.Location = new System.Drawing.Point(340, 112);
            this.SleepButton.Margin = new System.Windows.Forms.Padding(2);
            this.SleepButton.Name = "SleepButton";
            this.SleepButton.Size = new System.Drawing.Size(156, 35);
            this.SleepButton.TabIndex = 10;
            this.SleepButton.Text = "Sleep";
            this.SleepButton.UseVisualStyleBackColor = true;
            // 
            // RebootButton
            // 
            this.RebootButton.Location = new System.Drawing.Point(340, 68);
            this.RebootButton.Margin = new System.Windows.Forms.Padding(2);
            this.RebootButton.Name = "RebootButton";
            this.RebootButton.Size = new System.Drawing.Size(156, 35);
            this.RebootButton.TabIndex = 9;
            this.RebootButton.Text = "Reboot";
            this.RebootButton.UseVisualStyleBackColor = true;
            // 
            // Set0Button
            // 
            this.Set0Button.Location = new System.Drawing.Point(841, 112);
            this.Set0Button.Margin = new System.Windows.Forms.Padding(2);
            this.Set0Button.Name = "Set0Button";
            this.Set0Button.Size = new System.Drawing.Size(104, 35);
            this.Set0Button.TabIndex = 14;
            this.Set0Button.Text = "Set 0 [deg]";
            this.Set0Button.UseVisualStyleBackColor = true;
            // 
            // MoveSxButton
            // 
            this.MoveSxButton.Location = new System.Drawing.Point(841, 68);
            this.MoveSxButton.Margin = new System.Windows.Forms.Padding(2);
            this.MoveSxButton.Name = "MoveSxButton";
            this.MoveSxButton.Size = new System.Drawing.Size(104, 35);
            this.MoveSxButton.TabIndex = 13;
            this.MoveSxButton.Text = "Move Sx";
            this.MoveSxButton.UseVisualStyleBackColor = true;
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.Location = new System.Drawing.Point(1640, 112);
            this.numericUpDownInterval.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownInterval.Name = "numericUpDownInterval";
            this.numericUpDownInterval.Size = new System.Drawing.Size(80, 25);
            this.numericUpDownInterval.TabIndex = 1;
            this.numericUpDownInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // RepeatMoveSxButton
            // 
            this.RepeatMoveSxButton.Location = new System.Drawing.Point(1730, 112);
            this.RepeatMoveSxButton.Margin = new System.Windows.Forms.Padding(2);
            this.RepeatMoveSxButton.Name = "RepeatMoveSxButton";
            this.RepeatMoveSxButton.Size = new System.Drawing.Size(104, 35);
            this.RepeatMoveSxButton.TabIndex = 13;
            this.RepeatMoveSxButton.Text = "R_MoveSx";
            this.RepeatMoveSxButton.UseVisualStyleBackColor = true;
            // 
            // RepeatSpeedButton
            // 
            this.RepeatSpeedButton.Location = new System.Drawing.Point(1730, 82);
            this.RepeatSpeedButton.Margin = new System.Windows.Forms.Padding(2);
            this.RepeatSpeedButton.Name = "RepeatSpeedButton";
            this.RepeatSpeedButton.Size = new System.Drawing.Size(104, 35);
            this.RepeatSpeedButton.TabIndex = 13;
            this.RepeatSpeedButton.Text = "R_Speed";
            this.RepeatSpeedButton.UseVisualStyleBackColor = true;
            // 
            // TargetAngleNumericUpDown
            // 
            this.TargetAngleNumericUpDown.Location = new System.Drawing.Point(841, 26);
            this.TargetAngleNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.TargetAngleNumericUpDown.Name = "TargetAngleNumericUpDown";
            this.TargetAngleNumericUpDown.Size = new System.Drawing.Size(102, 25);
            this.TargetAngleNumericUpDown.TabIndex = 12;
            this.TargetAngleNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TargetAngleMinLabel
            // 
            this.TargetAngleMinLabel.Location = new System.Drawing.Point(501, 60);
            this.TargetAngleMinLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TargetAngleMinLabel.Name = "TargetAngleMinLabel";
            this.TargetAngleMinLabel.Size = new System.Drawing.Size(80, 25);
            this.TargetAngleMinLabel.TabIndex = 0;
            this.TargetAngleMinLabel.Text = "MIN";
            this.TargetAngleMinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TargetAngleMaxLabel
            // 
            this.TargetAngleMaxLabel.Location = new System.Drawing.Point(762, 60);
            this.TargetAngleMaxLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TargetAngleMaxLabel.Name = "TargetAngleMaxLabel";
            this.TargetAngleMaxLabel.Size = new System.Drawing.Size(80, 25);
            this.TargetAngleMaxLabel.TabIndex = 0;
            this.TargetAngleMaxLabel.Text = "MAX";
            this.TargetAngleMaxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TargetAngle0Label
            // 
            this.TargetAngle0Label.Location = new System.Drawing.Point(550, 60);
            this.TargetAngle0Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TargetAngle0Label.Name = "TargetAngle0Label";
            this.TargetAngle0Label.Size = new System.Drawing.Size(294, 25);
            this.TargetAngle0Label.TabIndex = 0;
            this.TargetAngle0Label.Text = "0";
            this.TargetAngle0Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TargetAngleTrackBar
            // 
            this.TargetAngleTrackBar.Location = new System.Drawing.Point(520, 22);
            this.TargetAngleTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.TargetAngleTrackBar.Name = "TargetAngleTrackBar";
            this.TargetAngleTrackBar.Size = new System.Drawing.Size(294, 56);
            this.TargetAngleTrackBar.TabIndex = 11;
            // 
            // WriteFlashRomButton
            // 
            this.WriteFlashRomButton.Location = new System.Drawing.Point(340, 22);
            this.WriteFlashRomButton.Margin = new System.Windows.Forms.Padding(2);
            this.WriteFlashRomButton.Name = "WriteFlashRomButton";
            this.WriteFlashRomButton.Size = new System.Drawing.Size(156, 35);
            this.WriteFlashRomButton.TabIndex = 8;
            this.WriteFlashRomButton.Text = "Write Flash ROM";
            this.WriteFlashRomButton.UseVisualStyleBackColor = true;
            // 
            // InitializeServoButton
            // 
            this.InitializeServoButton.Location = new System.Drawing.Point(170, 112);
            this.InitializeServoButton.Margin = new System.Windows.Forms.Padding(2);
            this.InitializeServoButton.Name = "InitializeServoButton";
            this.InitializeServoButton.Size = new System.Drawing.Size(156, 35);
            this.InitializeServoButton.TabIndex = 7;
            this.InitializeServoButton.Text = "Initialize Servo";
            this.InitializeServoButton.UseVisualStyleBackColor = true;
            // 
            // GetParametersButton
            // 
            this.GetParametersButton.Location = new System.Drawing.Point(170, 68);
            this.GetParametersButton.Margin = new System.Windows.Forms.Padding(2);
            this.GetParametersButton.Name = "GetParametersButton";
            this.GetParametersButton.Size = new System.Drawing.Size(156, 35);
            this.GetParametersButton.TabIndex = 6;
            this.GetParametersButton.Text = "Get Parameters";
            this.GetParametersButton.UseVisualStyleBackColor = true;
            // 
            // AckResultLabel
            // 
            this.AckResultLabel.Location = new System.Drawing.Point(221, 28);
            this.AckResultLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.AckResultLabel.Name = "AckResultLabel";
            this.AckResultLabel.Size = new System.Drawing.Size(105, 25);
            this.AckResultLabel.TabIndex = 0;
            this.AckResultLabel.Text = "status";
            this.AckResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AckButton
            // 
            this.AckButton.Location = new System.Drawing.Point(170, 22);
            this.AckButton.Margin = new System.Windows.Forms.Padding(2);
            this.AckButton.Name = "AckButton";
            this.AckButton.Size = new System.Drawing.Size(50, 35);
            this.AckButton.TabIndex = 5;
            this.AckButton.Text = "ACK";
            this.AckButton.UseVisualStyleBackColor = true;
            // 
            // ServoIdNumericUpDown
            // 
            this.ServoIdNumericUpDown.Location = new System.Drawing.Point(82, 26);
            this.ServoIdNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.ServoIdNumericUpDown.Name = "ServoIdNumericUpDown";
            this.ServoIdNumericUpDown.Size = new System.Drawing.Size(75, 25);
            this.ServoIdNumericUpDown.TabIndex = 1;
            this.ServoIdNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ServoIdLabel
            // 
            this.ServoIdLabel.AutoSize = true;
            this.ServoIdLabel.Location = new System.Drawing.Point(14, 31);
            this.ServoIdLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ServoIdLabel.Name = "ServoIdLabel";
            this.ServoIdLabel.Size = new System.Drawing.Size(60, 17);
            this.ServoIdLabel.TabIndex = 0;
            this.ServoIdLabel.Text = "ServoID";
            // 
            // ServoParameterGroupBox
            // 
            this.ServoParameterGroupBox.Controls.Add(this.LabelsPanel);
            this.ServoParameterGroupBox.Controls.Add(this.tabCtrl);
            this.ServoParameterGroupBox.Controls.Add(this.SetRomButton);
            this.ServoParameterGroupBox.Controls.Add(this.SetRamButton);
            this.ServoParameterGroupBox.Location = new System.Drawing.Point(15, 179);
            this.ServoParameterGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.ServoParameterGroupBox.Name = "ServoParameterGroupBox";
            this.ServoParameterGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.ServoParameterGroupBox.Size = new System.Drawing.Size(1562, 789);
            this.ServoParameterGroupBox.TabIndex = 4;
            this.ServoParameterGroupBox.TabStop = false;
            this.ServoParameterGroupBox.Text = "Servo Parameter";
            // 
            // LabelsPanel
            // 
            this.LabelsPanel.Controls.Add(this.HeaderLabel4);
            this.LabelsPanel.Controls.Add(this.HeaderLabel3);
            this.LabelsPanel.Controls.Add(this.HeaderLabel2);
            this.LabelsPanel.Controls.Add(this.HeaderLabel1);
            this.LabelsPanel.Controls.Add(this.HeaderLabel0);
            this.LabelsPanel.Location = new System.Drawing.Point(81, 11);
            this.LabelsPanel.Margin = new System.Windows.Forms.Padding(2);
            this.LabelsPanel.Name = "LabelsPanel";
            this.LabelsPanel.Size = new System.Drawing.Size(656, 62);
            this.LabelsPanel.TabIndex = 0;
            this.LabelsPanel.Visible = false;
            // 
            // HeaderLabel4
            // 
            this.HeaderLabel4.Location = new System.Drawing.Point(546, 20);
            this.HeaderLabel4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeaderLabel4.Name = "HeaderLabel4";
            this.HeaderLabel4.Size = new System.Drawing.Size(54, 22);
            this.HeaderLabel4.TabIndex = 1;
            // 
            // HeaderLabel3
            // 
            this.HeaderLabel3.Location = new System.Drawing.Point(465, 20);
            this.HeaderLabel3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeaderLabel3.Name = "HeaderLabel3";
            this.HeaderLabel3.Size = new System.Drawing.Size(86, 22);
            this.HeaderLabel3.TabIndex = 2;
            // 
            // HeaderLabel2
            // 
            this.HeaderLabel2.Location = new System.Drawing.Point(359, 20);
            this.HeaderLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeaderLabel2.Name = "HeaderLabel2";
            this.HeaderLabel2.Size = new System.Drawing.Size(106, 22);
            this.HeaderLabel2.TabIndex = 3;
            this.HeaderLabel2.Text = "Parameter";
            // 
            // HeaderLabel1
            // 
            this.HeaderLabel1.Location = new System.Drawing.Point(146, 20);
            this.HeaderLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeaderLabel1.Name = "HeaderLabel1";
            this.HeaderLabel1.Size = new System.Drawing.Size(205, 22);
            this.HeaderLabel1.TabIndex = 4;
            this.HeaderLabel1.Text = "Name";
            // 
            // HeaderLabel0
            // 
            this.HeaderLabel0.Location = new System.Drawing.Point(48, 20);
            this.HeaderLabel0.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeaderLabel0.Name = "HeaderLabel0";
            this.HeaderLabel0.Size = new System.Drawing.Size(94, 22);
            this.HeaderLabel0.TabIndex = 5;
            this.HeaderLabel0.Text = "Address";
            // 
            // tabCtrl
            // 
            this.tabCtrl.Location = new System.Drawing.Point(19, 29);
            this.tabCtrl.Margin = new System.Windows.Forms.Padding(2);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(156, 125);
            this.tabCtrl.TabIndex = 0;
            // 
            // SetRomButton
            // 
            this.SetRomButton.Location = new System.Drawing.Point(1402, 12);
            this.SetRomButton.Margin = new System.Windows.Forms.Padding(2);
            this.SetRomButton.Name = "SetRomButton";
            this.SetRomButton.Size = new System.Drawing.Size(155, 35);
            this.SetRomButton.TabIndex = 1001;
            this.SetRomButton.Text = "ROM Write BTN";
            this.SetRomButton.UseVisualStyleBackColor = true;
            // 
            // SetRamButton
            // 
            this.SetRamButton.Location = new System.Drawing.Point(1228, 12);
            this.SetRamButton.Margin = new System.Windows.Forms.Padding(2);
            this.SetRamButton.Name = "SetRamButton";
            this.SetRamButton.Size = new System.Drawing.Size(155, 35);
            this.SetRamButton.TabIndex = 1000;
            this.SetRamButton.Text = "RAM Write BTN";
            this.SetRamButton.UseVisualStyleBackColor = true;
            // 
            // StatusGroupBox
            // 
            this.StatusGroupBox.Controls.Add(this.Result5Label);
            this.StatusGroupBox.Controls.Add(this.Result4Label);
            this.StatusGroupBox.Controls.Add(this.Result3Label);
            this.StatusGroupBox.Controls.Add(this.Result2Label);
            this.StatusGroupBox.Controls.Add(this.Result1Label);
            this.StatusGroupBox.Controls.Add(this.ManufactureDateParameterLabel);
            this.StatusGroupBox.Controls.Add(this.CurrentControlFormLabel);
            this.StatusGroupBox.Controls.Add(this.UniqueNumberParameterLabel);
            this.StatusGroupBox.Controls.Add(this.FirmwareVersionParameterLabel);
            this.StatusGroupBox.Controls.Add(this.ModelNumberParameterLabel);
            this.StatusGroupBox.Controls.Add(this.ManufactureDateLabel);
            this.StatusGroupBox.Controls.Add(this.CurrentControlLabel);
            this.StatusGroupBox.Controls.Add(this.UniqueNumberLabel);
            this.StatusGroupBox.Controls.Add(this.FirmwareVersionLabel);
            this.StatusGroupBox.Controls.Add(this.ModelNumberLabel);
            this.StatusGroupBox.Location = new System.Drawing.Point(1192, 8);
            this.StatusGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.StatusGroupBox.Name = "StatusGroupBox";
            this.StatusGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.StatusGroupBox.Size = new System.Drawing.Size(385, 165);
            this.StatusGroupBox.TabIndex = 3;
            this.StatusGroupBox.TabStop = false;
            this.StatusGroupBox.Text = "Status";
            // 
            // Result5Label
            // 
            this.Result5Label.AutoSize = true;
            this.Result5Label.Font = new System.Drawing.Font("Arial", 9F);
            this.Result5Label.Location = new System.Drawing.Point(281, 112);
            this.Result5Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result5Label.Name = "Result5Label";
            this.Result5Label.Size = new System.Drawing.Size(90, 17);
            this.Result5Label.TabIndex = 0;
            this.Result5Label.Text = "□ Com Error";
            // 
            // Result4Label
            // 
            this.Result4Label.AutoSize = true;
            this.Result4Label.Font = new System.Drawing.Font("Arial", 9F);
            this.Result4Label.Location = new System.Drawing.Point(281, 94);
            this.Result4Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result4Label.Name = "Result4Label";
            this.Result4Label.Size = new System.Drawing.Size(84, 17);
            this.Result4Label.TabIndex = 0;
            this.Result4Label.Text = "□ Soft Error";
            // 
            // Result3Label
            // 
            this.Result3Label.AutoSize = true;
            this.Result3Label.Font = new System.Drawing.Font("Arial", 9F);
            this.Result3Label.Location = new System.Drawing.Point(281, 75);
            this.Result3Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result3Label.Name = "Result3Label";
            this.Result3Label.Size = new System.Drawing.Size(89, 17);
            this.Result3Label.TabIndex = 0;
            this.Result3Label.Text = "□ Hard Error";
            // 
            // Result2Label
            // 
            this.Result2Label.AutoSize = true;
            this.Result2Label.Font = new System.Drawing.Font("Arial", 9F);
            this.Result2Label.Location = new System.Drawing.Point(281, 44);
            this.Result2Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result2Label.Name = "Result2Label";
            this.Result2Label.Size = new System.Drawing.Size(88, 17);
            this.Result2Label.TabIndex = 0;
            this.Result2Label.Text = "□ In Position";
            // 
            // Result1Label
            // 
            this.Result1Label.AutoSize = true;
            this.Result1Label.Font = new System.Drawing.Font("Arial", 9F);
            this.Result1Label.Location = new System.Drawing.Point(281, 25);
            this.Result1Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result1Label.Name = "Result1Label";
            this.Result1Label.Size = new System.Drawing.Size(60, 17);
            this.Result1Label.TabIndex = 0;
            this.Result1Label.Text = "□ Active";
            // 
            // ManufactureDateParameterLabel
            // 
            this.ManufactureDateParameterLabel.Location = new System.Drawing.Point(146, 104);
            this.ManufactureDateParameterLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ManufactureDateParameterLabel.Name = "ManufactureDateParameterLabel";
            this.ManufactureDateParameterLabel.Size = new System.Drawing.Size(125, 22);
            this.ManufactureDateParameterLabel.TabIndex = 0;
            // 
            // CurrentControlFormLabel
            // 
            this.CurrentControlFormLabel.Location = new System.Drawing.Point(146, 130);
            this.CurrentControlFormLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CurrentControlFormLabel.Name = "CurrentControlFormLabel";
            this.CurrentControlFormLabel.Size = new System.Drawing.Size(125, 22);
            this.CurrentControlFormLabel.TabIndex = 0;
            // 
            // UniqueNumberParameterLabel
            // 
            this.UniqueNumberParameterLabel.Location = new System.Drawing.Point(146, 78);
            this.UniqueNumberParameterLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.UniqueNumberParameterLabel.Name = "UniqueNumberParameterLabel";
            this.UniqueNumberParameterLabel.Size = new System.Drawing.Size(125, 22);
            this.UniqueNumberParameterLabel.TabIndex = 0;
            // 
            // FirmwareVersionParameterLabel
            // 
            this.FirmwareVersionParameterLabel.Location = new System.Drawing.Point(146, 52);
            this.FirmwareVersionParameterLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FirmwareVersionParameterLabel.Name = "FirmwareVersionParameterLabel";
            this.FirmwareVersionParameterLabel.Size = new System.Drawing.Size(125, 22);
            this.FirmwareVersionParameterLabel.TabIndex = 0;
            // 
            // ModelNumberParameterLabel
            // 
            this.ModelNumberParameterLabel.Location = new System.Drawing.Point(146, 26);
            this.ModelNumberParameterLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ModelNumberParameterLabel.Name = "ModelNumberParameterLabel";
            this.ModelNumberParameterLabel.Size = new System.Drawing.Size(125, 22);
            this.ModelNumberParameterLabel.TabIndex = 0;
            // 
            // ManufactureDateLabel
            // 
            this.ManufactureDateLabel.AutoSize = true;
            this.ManufactureDateLabel.Location = new System.Drawing.Point(10, 104);
            this.ManufactureDateLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ManufactureDateLabel.Name = "ManufactureDateLabel";
            this.ManufactureDateLabel.Size = new System.Drawing.Size(123, 17);
            this.ManufactureDateLabel.TabIndex = 0;
            this.ManufactureDateLabel.Text = "Manufacture Date";
            // 
            // CurrentControlLabel
            // 
            this.CurrentControlLabel.AutoSize = true;
            this.CurrentControlLabel.Location = new System.Drawing.Point(10, 130);
            this.CurrentControlLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CurrentControlLabel.Name = "CurrentControlLabel";
            this.CurrentControlLabel.Size = new System.Drawing.Size(94, 17);
            this.CurrentControlLabel.TabIndex = 0;
            this.CurrentControlLabel.Text = "Control Form";
            // 
            // UniqueNumberLabel
            // 
            this.UniqueNumberLabel.AutoSize = true;
            this.UniqueNumberLabel.Location = new System.Drawing.Point(10, 78);
            this.UniqueNumberLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.UniqueNumberLabel.Name = "UniqueNumberLabel";
            this.UniqueNumberLabel.Size = new System.Drawing.Size(109, 17);
            this.UniqueNumberLabel.TabIndex = 0;
            this.UniqueNumberLabel.Text = "Unique Number";
            // 
            // FirmwareVersionLabel
            // 
            this.FirmwareVersionLabel.AutoSize = true;
            this.FirmwareVersionLabel.Location = new System.Drawing.Point(10, 52);
            this.FirmwareVersionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FirmwareVersionLabel.Name = "FirmwareVersionLabel";
            this.FirmwareVersionLabel.Size = new System.Drawing.Size(122, 17);
            this.FirmwareVersionLabel.TabIndex = 0;
            this.FirmwareVersionLabel.Text = "Firmware Version";
            // 
            // ModelNumberLabel
            // 
            this.ModelNumberLabel.AutoSize = true;
            this.ModelNumberLabel.Location = new System.Drawing.Point(10, 26);
            this.ModelNumberLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ModelNumberLabel.Name = "ModelNumberLabel";
            this.ModelNumberLabel.Size = new System.Drawing.Size(102, 17);
            this.ModelNumberLabel.TabIndex = 0;
            this.ModelNumberLabel.Text = "Model Number";
            // 
            // Result6Label
            // 
            this.Result6Label.AutoSize = true;
            this.Result6Label.Font = new System.Drawing.Font("Arial", 9F);
            this.Result6Label.Location = new System.Drawing.Point(1474, 140);
            this.Result6Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result6Label.Name = "Result6Label";
            this.Result6Label.Size = new System.Drawing.Size(95, 17);
            this.Result6Label.TabIndex = 0;
            this.Result6Label.Text = "□ Other Error";
            // 
            // FormBottomRightLabel
            // 
            this.FormBottomRightLabel.Location = new System.Drawing.Point(1581, 939);
            this.FormBottomRightLabel.Margin = new System.Windows.Forms.Padding(51, 0, 51, 0);
            this.FormBottomRightLabel.Name = "FormBottomRightLabel";
            this.FormBottomRightLabel.Size = new System.Drawing.Size(12, 12);
            this.FormBottomRightLabel.TabIndex = 6;
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1592, 979);
            this.Controls.Add(this.Result6Label);
            this.Controls.Add(this.FormBottomRightLabel);
            this.Controls.Add(this.StatusGroupBox);
            this.Controls.Add(this.ServoParameterGroupBox);
            this.Controls.Add(this.OperationGroupBox);
            this.Controls.Add(this.PortSettingGroupBox);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(102, 48, 102, 48);
            this.MaximizeBox = false;
            this.Name = "EditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CM.BUS_Editor_BLA21-12R3-C01_Ver.1.01";
            this.Load += new System.EventHandler(this.EditorForm_Load);
            this.PortSettingGroupBox.ResumeLayout(false);
            this.PortSettingGroupBox.PerformLayout();
            this.OperationGroupBox.ResumeLayout(false);
            this.OperationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetAngleNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetAngleTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServoIdNumericUpDown)).EndInit();
            this.ServoParameterGroupBox.ResumeLayout(false);
            this.LabelsPanel.ResumeLayout(false);
            this.StatusGroupBox.ResumeLayout(false);
            this.StatusGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox PortSettingGroupBox;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.ComboBox BandRateComboBox;
        private System.Windows.Forms.Label BandRateLabel;
        private System.Windows.Forms.ComboBox ComPortComboBox;
        private System.Windows.Forms.Label ComPortlabel;
        private System.Windows.Forms.GroupBox OperationGroupBox;
        private System.Windows.Forms.Label TargetAngleMinLabel;
        private System.Windows.Forms.Label TargetAngleMaxLabel;
        private System.Windows.Forms.Label TargetAngle0Label;
        private System.Windows.Forms.TrackBar TargetAngleTrackBar;
        private System.Windows.Forms.Button WriteFlashRomButton;
        private System.Windows.Forms.Button InitializeServoButton;
        private System.Windows.Forms.Button GetParametersButton;
        private System.Windows.Forms.Label AckResultLabel;
        private System.Windows.Forms.Button AckButton;
        private System.Windows.Forms.NumericUpDown ServoIdNumericUpDown;
        private System.Windows.Forms.Label ServoIdLabel;
        private System.Windows.Forms.Button Set0Button;
        private System.Windows.Forms.Button MoveSxButton;
        private System.Windows.Forms.CheckBox RepeatMoveSxButton;
        private System.Windows.Forms.CheckBox RepeatSpeedButton;
        private System.Windows.Forms.NumericUpDown numericUpDownInterval;
        private System.Windows.Forms.NumericUpDown TargetAngleNumericUpDown;
        private System.Windows.Forms.GroupBox ServoParameterGroupBox;
        private System.Windows.Forms.GroupBox StatusGroupBox;
        private System.Windows.Forms.Button SleepButton;
        private System.Windows.Forms.Button RebootButton;
        private System.Windows.Forms.Label ManufactureDateLabel;
        private System.Windows.Forms.Label CurrentControlLabel;
        private System.Windows.Forms.Label UniqueNumberLabel;
        private System.Windows.Forms.Label FirmwareVersionLabel;
        private System.Windows.Forms.Label ModelNumberLabel;
        private System.Windows.Forms.RadioButton IdSelect3RadioButton;
        private System.Windows.Forms.RadioButton IdSelect2RadioButton;
        private System.Windows.Forms.RadioButton IdSelect1RadioButton;
        private System.Windows.Forms.Button SetRomButton;
        private System.Windows.Forms.Button SetRamButton;
        private System.Windows.Forms.Label FormBottomRightLabel;
        private System.Windows.Forms.Label ManufactureDateParameterLabel;
        private System.Windows.Forms.Label CurrentControlFormLabel;
        private System.Windows.Forms.Label UniqueNumberParameterLabel;
        private System.Windows.Forms.Label FirmwareVersionParameterLabel;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.Panel LabelsPanel;
        private System.Windows.Forms.Label HeaderLabel4;
        private System.Windows.Forms.Label HeaderLabel3;
        private System.Windows.Forms.Label HeaderLabel2;
        private System.Windows.Forms.Label HeaderLabel1;
        private System.Windows.Forms.Label HeaderLabel0;
        private System.Windows.Forms.Label ModelNumberParameterLabel;
        private System.Windows.Forms.Label Result1Label;
        private System.Windows.Forms.Label Result2Label;
        private System.Windows.Forms.Label Result3Label;
        private System.Windows.Forms.Label Result5Label;
        private System.Windows.Forms.Label Result4Label;
        private System.Windows.Forms.Label Result6Label;
    }
}

