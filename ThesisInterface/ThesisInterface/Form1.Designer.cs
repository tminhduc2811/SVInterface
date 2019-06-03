namespace ThesisInterface
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.autoUC1 = new ThesisInterface.UserControls.AutoUC();
            this.imuSetting1 = new ThesisInterface.UserControls.IMUSetting();
            this.manualUC1 = new ThesisInterface.UserControls.ManualUC();
            this.vehicleSetting1 = new ThesisInterface.UserControls.VehicleSetting();
            this.helperControls1 = new ThesisInterface.UserControls.HelperControls();
            this.progressUC1 = new ThesisInterface.UserControls.ProgressUC();
            this.autoSetting1 = new ThesisInterface.UserControls.AutoSetting();
            this.ConfigWaitForRespond = new System.Windows.Forms.Timer(this.components);
            this.IMUConfigWaitForRespond = new System.Windows.Forms.Timer(this.components);
            this.AutoTimer = new System.Windows.Forms.Timer(this.components);
            this.KcontrolTimer = new System.Windows.Forms.Timer(this.components);
            this.StartKctrlTimer = new System.Windows.Forms.Timer(this.components);
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.DefaultWaitForResponseTimer = new System.Windows.Forms.Timer(this.components);
            this.StopKctrlTimer = new System.Windows.Forms.Timer(this.components);
            this.TopPanel = new Bunifu.Framework.UI.BunifuGradientPanel();
            this.closebt = new Bunifu.Framework.UI.BunifuImageButton();
            this.maxbt = new Bunifu.Framework.UI.BunifuImageButton();
            this.minbt = new Bunifu.Framework.UI.BunifuImageButton();
            this.menubt = new Bunifu.Framework.UI.BunifuImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SendWithResTimer = new System.Windows.Forms.Timer(this.components);
            this.SidePanel = new Bunifu.Framework.UI.BunifuGradientPanel();
            this.bunifuFlatButton4 = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bunifuFlatButton3 = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bunifuFlatButton1 = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bunifuFlatButton2 = new Bunifu.Framework.UI.BunifuFlatButton();
            this.label2 = new System.Windows.Forms.Label();
            this.ControlPanelDrag = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.autoSettingDrag = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.TopDrag = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.bunifuFormFadeTransition1 = new Bunifu.Framework.UI.BunifuFormFadeTransition(this.components);
            this.panel1.SuspendLayout();
            this.TopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closebt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxbt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minbt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.menubt)).BeginInit();
            this.SidePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(11)))), ((int)(((byte)(42)))));
            this.panel1.Controls.Add(this.autoUC1);
            this.panel1.Controls.Add(this.imuSetting1);
            this.panel1.Controls.Add(this.manualUC1);
            this.panel1.Controls.Add(this.vehicleSetting1);
            this.panel1.Controls.Add(this.helperControls1);
            this.panel1.Controls.Add(this.progressUC1);
            this.panel1.Controls.Add(this.autoSetting1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(156, 47);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(931, 603);
            this.panel1.TabIndex = 11;
            // 
            // autoUC1
            // 
            this.autoUC1.BackColor = System.Drawing.Color.White;
            this.autoUC1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoUC1.Location = new System.Drawing.Point(0, 0);
            this.autoUC1.Name = "autoUC1";
            this.autoUC1.Size = new System.Drawing.Size(931, 603);
            this.autoUC1.TabIndex = 12;
            this.autoUC1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AutoUCControlByKeyDown);
            this.autoUC1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AutoUCControlByKeyUp);
            this.autoUC1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.autoUC1_MouseClick);
            // 
            // imuSetting1
            // 
            this.imuSetting1.BackColor = System.Drawing.Color.White;
            this.imuSetting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imuSetting1.Location = new System.Drawing.Point(0, 0);
            this.imuSetting1.Name = "imuSetting1";
            this.imuSetting1.Size = new System.Drawing.Size(931, 603);
            this.imuSetting1.TabIndex = 2;
            // 
            // manualUC1
            // 
            this.manualUC1.BackColor = System.Drawing.Color.White;
            this.manualUC1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manualUC1.Location = new System.Drawing.Point(0, 0);
            this.manualUC1.Name = "manualUC1";
            this.manualUC1.Size = new System.Drawing.Size(931, 603);
            this.manualUC1.TabIndex = 0;
            this.manualUC1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.manualUC1_KeyDown);
            this.manualUC1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.manualUC1_KeyUp);
            // 
            // vehicleSetting1
            // 
            this.vehicleSetting1.BackColor = System.Drawing.Color.White;
            this.vehicleSetting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vehicleSetting1.Location = new System.Drawing.Point(0, 0);
            this.vehicleSetting1.Name = "vehicleSetting1";
            this.vehicleSetting1.Size = new System.Drawing.Size(931, 603);
            this.vehicleSetting1.TabIndex = 13;
            // 
            // helperControls1
            // 
            this.helperControls1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.helperControls1.BackColor = System.Drawing.Color.White;
            this.helperControls1.Location = new System.Drawing.Point(166, 66);
            this.helperControls1.MaximumSize = new System.Drawing.Size(650, 650);
            this.helperControls1.Name = "helperControls1";
            this.helperControls1.Size = new System.Drawing.Size(533, 513);
            this.helperControls1.TabIndex = 14;
            // 
            // progressUC1
            // 
            this.progressUC1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressUC1.BackColor = System.Drawing.Color.White;
            this.progressUC1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.progressUC1.Location = new System.Drawing.Point(219, 118);
            this.progressUC1.MaximumSize = new System.Drawing.Size(480, 323);
            this.progressUC1.MinimumSize = new System.Drawing.Size(480, 323);
            this.progressUC1.Name = "progressUC1";
            this.progressUC1.Size = new System.Drawing.Size(480, 323);
            this.progressUC1.TabIndex = 15;
            // 
            // autoSetting1
            // 
            this.autoSetting1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.autoSetting1.BackColor = System.Drawing.Color.White;
            this.autoSetting1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.autoSetting1.Location = new System.Drawing.Point(292, 254);
            this.autoSetting1.MaximumSize = new System.Drawing.Size(344, 250);
            this.autoSetting1.MinimumSize = new System.Drawing.Size(344, 250);
            this.autoSetting1.Name = "autoSetting1";
            this.autoSetting1.Size = new System.Drawing.Size(344, 250);
            this.autoSetting1.TabIndex = 0;
            // 
            // ConfigWaitForRespond
            // 
            this.ConfigWaitForRespond.Interval = 700;
            this.ConfigWaitForRespond.Tick += new System.EventHandler(this.ConfigWaitForRespond_Tick);
            // 
            // IMUConfigWaitForRespond
            // 
            this.IMUConfigWaitForRespond.Interval = 85;
            this.IMUConfigWaitForRespond.Tick += new System.EventHandler(this.IMUConfigWaitForRespond_Tick);
            // 
            // AutoTimer
            // 
            this.AutoTimer.Tick += new System.EventHandler(this.AutoTimer_Tick);
            // 
            // KcontrolTimer
            // 
            this.KcontrolTimer.Interval = 80;
            this.KcontrolTimer.Tick += new System.EventHandler(this.KcontrolTimer_Tick);
            // 
            // StartKctrlTimer
            // 
            this.StartKctrlTimer.Interval = 45;
            this.StartKctrlTimer.Tick += new System.EventHandler(this.StartKctrlTimer_Tick);
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 25;
            this.bunifuElipse1.TargetControl = this;
            // 
            // DefaultWaitForResponseTimer
            // 
            this.DefaultWaitForResponseTimer.Interval = 20;
            this.DefaultWaitForResponseTimer.Tick += new System.EventHandler(this.DefaultWaitForResponseTimer_Tick);
            // 
            // StopKctrlTimer
            // 
            this.StopKctrlTimer.Interval = 45;
            this.StopKctrlTimer.Tick += new System.EventHandler(this.StopKctrlTimer_Tick);
            // 
            // TopPanel
            // 
            this.TopPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TopPanel.BackgroundImage")));
            this.TopPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TopPanel.Controls.Add(this.closebt);
            this.TopPanel.Controls.Add(this.maxbt);
            this.TopPanel.Controls.Add(this.minbt);
            this.TopPanel.Controls.Add(this.menubt);
            this.TopPanel.Controls.Add(this.label1);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.GradientBottomLeft = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.TopPanel.GradientBottomRight = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.TopPanel.GradientTopLeft = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(24)))), ((int)(((byte)(91)))));
            this.TopPanel.GradientTopRight = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(49)))), ((int)(((byte)(85)))));
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Quality = 10;
            this.TopPanel.Size = new System.Drawing.Size(1087, 47);
            this.TopPanel.TabIndex = 9;
            this.TopPanel.DoubleClick += new System.EventHandler(this.TopPanel_DoubleClick);
            // 
            // closebt
            // 
            this.closebt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closebt.BackColor = System.Drawing.Color.Transparent;
            this.closebt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closebt.Image = global::ThesisInterface.Properties.Resources.Close_Window_64px;
            this.closebt.ImageActive = null;
            this.closebt.Location = new System.Drawing.Point(1056, 7);
            this.closebt.Name = "closebt";
            this.closebt.Size = new System.Drawing.Size(25, 25);
            this.closebt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.closebt.TabIndex = 9;
            this.closebt.TabStop = false;
            this.closebt.Zoom = 10;
            this.closebt.Click += new System.EventHandler(this.closebt_Click);
            // 
            // maxbt
            // 
            this.maxbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxbt.BackColor = System.Drawing.Color.Transparent;
            this.maxbt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.maxbt.Image = global::ThesisInterface.Properties.Resources.Restore_Window_50px;
            this.maxbt.ImageActive = null;
            this.maxbt.Location = new System.Drawing.Point(1025, 7);
            this.maxbt.Name = "maxbt";
            this.maxbt.Size = new System.Drawing.Size(25, 25);
            this.maxbt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.maxbt.TabIndex = 8;
            this.maxbt.TabStop = false;
            this.maxbt.Zoom = 10;
            this.maxbt.Click += new System.EventHandler(this.maxbt_Click);
            // 
            // minbt
            // 
            this.minbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minbt.BackColor = System.Drawing.Color.Transparent;
            this.minbt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.minbt.Image = global::ThesisInterface.Properties.Resources.Minimize_Window_64px;
            this.minbt.ImageActive = null;
            this.minbt.Location = new System.Drawing.Point(994, 7);
            this.minbt.Name = "minbt";
            this.minbt.Size = new System.Drawing.Size(25, 25);
            this.minbt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.minbt.TabIndex = 7;
            this.minbt.TabStop = false;
            this.minbt.Zoom = 10;
            this.minbt.Click += new System.EventHandler(this.minbt_Click);
            // 
            // menubt
            // 
            this.menubt.BackColor = System.Drawing.Color.Transparent;
            this.menubt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.menubt.Image = global::ThesisInterface.Properties.Resources.Menu_64px;
            this.menubt.ImageActive = null;
            this.menubt.Location = new System.Drawing.Point(12, 7);
            this.menubt.Name = "menubt";
            this.menubt.Size = new System.Drawing.Size(30, 30);
            this.menubt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.menubt.TabIndex = 2;
            this.menubt.TabStop = false;
            this.menubt.Zoom = 10;
            this.menubt.Click += new System.EventHandler(this.menubt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Ailerons", 17.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.label1.Location = new System.Drawing.Point(58, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 28);
            this.label1.TabIndex = 1;
            this.label1.Text = "SELF-DRIVING VEHICLE";
            // 
            // SendWithResTimer
            // 
            this.SendWithResTimer.Interval = 20;
            this.SendWithResTimer.Tick += new System.EventHandler(this.SendWithResTimer_Tick);
            // 
            // SidePanel
            // 
            this.SidePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SidePanel.BackgroundImage")));
            this.SidePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SidePanel.Controls.Add(this.bunifuFlatButton4);
            this.SidePanel.Controls.Add(this.bunifuFlatButton3);
            this.SidePanel.Controls.Add(this.bunifuFlatButton1);
            this.SidePanel.Controls.Add(this.bunifuFlatButton2);
            this.SidePanel.Controls.Add(this.label2);
            this.SidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SidePanel.GradientBottomLeft = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.SidePanel.GradientBottomRight = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.SidePanel.GradientTopLeft = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(14)))), ((int)(((byte)(79)))));
            this.SidePanel.GradientTopRight = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(77)))), ((int)(((byte)(64)))));
            this.SidePanel.Location = new System.Drawing.Point(0, 47);
            this.SidePanel.Name = "SidePanel";
            this.SidePanel.Quality = 10;
            this.SidePanel.Size = new System.Drawing.Size(156, 603);
            this.SidePanel.TabIndex = 10;
            this.SidePanel.SizeChanged += new System.EventHandler(this.SidePanel_SizeChanged);
            // 
            // bunifuFlatButton4
            // 
            this.bunifuFlatButton4.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.bunifuFlatButton4.BackColor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bunifuFlatButton4.BorderRadius = 0;
            this.bunifuFlatButton4.ButtonText = "IMU Setting";
            this.bunifuFlatButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bunifuFlatButton4.DisabledColor = System.Drawing.Color.Gray;
            this.bunifuFlatButton4.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.bunifuFlatButton4.Iconcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton4.Iconimage = global::ThesisInterface.Properties.Resources.Settings_64px;
            this.bunifuFlatButton4.Iconimage_right = null;
            this.bunifuFlatButton4.Iconimage_right_Selected = null;
            this.bunifuFlatButton4.Iconimage_Selected = null;
            this.bunifuFlatButton4.IconMarginLeft = 0;
            this.bunifuFlatButton4.IconMarginRight = 0;
            this.bunifuFlatButton4.IconRightVisible = true;
            this.bunifuFlatButton4.IconRightZoom = 0D;
            this.bunifuFlatButton4.IconVisible = true;
            this.bunifuFlatButton4.IconZoom = 70D;
            this.bunifuFlatButton4.IsTab = false;
            this.bunifuFlatButton4.Location = new System.Drawing.Point(0, 211);
            this.bunifuFlatButton4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bunifuFlatButton4.Name = "bunifuFlatButton4";
            this.bunifuFlatButton4.Normalcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton4.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(14)))), ((int)(((byte)(79)))));
            this.bunifuFlatButton4.OnHoverTextColor = System.Drawing.Color.White;
            this.bunifuFlatButton4.selected = false;
            this.bunifuFlatButton4.Size = new System.Drawing.Size(156, 50);
            this.bunifuFlatButton4.TabIndex = 8;
            this.bunifuFlatButton4.Text = "IMU Setting";
            this.bunifuFlatButton4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bunifuFlatButton4.Textcolor = System.Drawing.Color.White;
            this.bunifuFlatButton4.TextFont = new System.Drawing.Font("Libel Suit Rg", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton4.Click += new System.EventHandler(this.bunifuFlatButton4_Click);
            // 
            // bunifuFlatButton3
            // 
            this.bunifuFlatButton3.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.bunifuFlatButton3.BackColor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bunifuFlatButton3.BorderRadius = 0;
            this.bunifuFlatButton3.ButtonText = "Auto";
            this.bunifuFlatButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bunifuFlatButton3.DisabledColor = System.Drawing.Color.Gray;
            this.bunifuFlatButton3.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.bunifuFlatButton3.Iconcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton3.Iconimage = global::ThesisInterface.Properties.Resources.auto;
            this.bunifuFlatButton3.Iconimage_right = null;
            this.bunifuFlatButton3.Iconimage_right_Selected = null;
            this.bunifuFlatButton3.Iconimage_Selected = null;
            this.bunifuFlatButton3.IconMarginLeft = 0;
            this.bunifuFlatButton3.IconMarginRight = 0;
            this.bunifuFlatButton3.IconRightVisible = true;
            this.bunifuFlatButton3.IconRightZoom = 0D;
            this.bunifuFlatButton3.IconVisible = true;
            this.bunifuFlatButton3.IconZoom = 70D;
            this.bunifuFlatButton3.IsTab = false;
            this.bunifuFlatButton3.Location = new System.Drawing.Point(0, 319);
            this.bunifuFlatButton3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bunifuFlatButton3.Name = "bunifuFlatButton3";
            this.bunifuFlatButton3.Normalcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton3.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(14)))), ((int)(((byte)(79)))));
            this.bunifuFlatButton3.OnHoverTextColor = System.Drawing.Color.White;
            this.bunifuFlatButton3.selected = false;
            this.bunifuFlatButton3.Size = new System.Drawing.Size(156, 50);
            this.bunifuFlatButton3.TabIndex = 8;
            this.bunifuFlatButton3.Text = "Auto";
            this.bunifuFlatButton3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bunifuFlatButton3.Textcolor = System.Drawing.Color.White;
            this.bunifuFlatButton3.TextFont = new System.Drawing.Font("Libel Suit Rg", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton3.Click += new System.EventHandler(this.bunifuFlatButton3_Click);
            // 
            // bunifuFlatButton1
            // 
            this.bunifuFlatButton1.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.bunifuFlatButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bunifuFlatButton1.BorderRadius = 0;
            this.bunifuFlatButton1.ButtonText = "Manual";
            this.bunifuFlatButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bunifuFlatButton1.DisabledColor = System.Drawing.Color.Gray;
            this.bunifuFlatButton1.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.bunifuFlatButton1.Iconcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton1.Iconimage = global::ThesisInterface.Properties.Resources.Manual_50px;
            this.bunifuFlatButton1.Iconimage_right = null;
            this.bunifuFlatButton1.Iconimage_right_Selected = null;
            this.bunifuFlatButton1.Iconimage_Selected = null;
            this.bunifuFlatButton1.IconMarginLeft = 0;
            this.bunifuFlatButton1.IconMarginRight = 0;
            this.bunifuFlatButton1.IconRightVisible = true;
            this.bunifuFlatButton1.IconRightZoom = 0D;
            this.bunifuFlatButton1.IconVisible = true;
            this.bunifuFlatButton1.IconZoom = 70D;
            this.bunifuFlatButton1.IsTab = false;
            this.bunifuFlatButton1.Location = new System.Drawing.Point(0, 265);
            this.bunifuFlatButton1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bunifuFlatButton1.Name = "bunifuFlatButton1";
            this.bunifuFlatButton1.Normalcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton1.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(14)))), ((int)(((byte)(79)))));
            this.bunifuFlatButton1.OnHoverTextColor = System.Drawing.Color.White;
            this.bunifuFlatButton1.selected = false;
            this.bunifuFlatButton1.Size = new System.Drawing.Size(156, 50);
            this.bunifuFlatButton1.TabIndex = 8;
            this.bunifuFlatButton1.Text = "Manual";
            this.bunifuFlatButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bunifuFlatButton1.Textcolor = System.Drawing.Color.White;
            this.bunifuFlatButton1.TextFont = new System.Drawing.Font("Libel Suit Rg", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton1.Click += new System.EventHandler(this.bunifuFlatButton1_Click);
            // 
            // bunifuFlatButton2
            // 
            this.bunifuFlatButton2.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.bunifuFlatButton2.BackColor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bunifuFlatButton2.BorderRadius = 0;
            this.bunifuFlatButton2.ButtonText = "Setting";
            this.bunifuFlatButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bunifuFlatButton2.DisabledColor = System.Drawing.Color.Gray;
            this.bunifuFlatButton2.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.bunifuFlatButton2.Iconcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton2.Iconimage = global::ThesisInterface.Properties.Resources.Settings_64px;
            this.bunifuFlatButton2.Iconimage_right = null;
            this.bunifuFlatButton2.Iconimage_right_Selected = null;
            this.bunifuFlatButton2.Iconimage_Selected = null;
            this.bunifuFlatButton2.IconMarginLeft = 0;
            this.bunifuFlatButton2.IconMarginRight = 0;
            this.bunifuFlatButton2.IconRightVisible = true;
            this.bunifuFlatButton2.IconRightZoom = 0D;
            this.bunifuFlatButton2.IconVisible = true;
            this.bunifuFlatButton2.IconZoom = 70D;
            this.bunifuFlatButton2.IsTab = false;
            this.bunifuFlatButton2.Location = new System.Drawing.Point(0, 159);
            this.bunifuFlatButton2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bunifuFlatButton2.Name = "bunifuFlatButton2";
            this.bunifuFlatButton2.Normalcolor = System.Drawing.Color.Transparent;
            this.bunifuFlatButton2.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(14)))), ((int)(((byte)(79)))));
            this.bunifuFlatButton2.OnHoverTextColor = System.Drawing.Color.White;
            this.bunifuFlatButton2.selected = false;
            this.bunifuFlatButton2.Size = new System.Drawing.Size(156, 50);
            this.bunifuFlatButton2.TabIndex = 8;
            this.bunifuFlatButton2.Text = "Setting";
            this.bunifuFlatButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bunifuFlatButton2.Textcolor = System.Drawing.Color.White;
            this.bunifuFlatButton2.TextFont = new System.Drawing.Font("Libel Suit Rg", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuFlatButton2.Click += new System.EventHandler(this.bunifuFlatButton2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Ailerons", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(42, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 36);
            this.label2.TabIndex = 9;
            this.label2.Text = "BKU";
            // 
            // ControlPanelDrag
            // 
            this.ControlPanelDrag.Fixed = true;
            this.ControlPanelDrag.Horizontal = true;
            this.ControlPanelDrag.TargetControl = this.SidePanel;
            this.ControlPanelDrag.Vertical = true;
            // 
            // autoSettingDrag
            // 
            this.autoSettingDrag.Fixed = true;
            this.autoSettingDrag.Horizontal = true;
            this.autoSettingDrag.TargetControl = this.autoSetting1;
            this.autoSettingDrag.Vertical = true;
            // 
            // TopDrag
            // 
            this.TopDrag.Fixed = true;
            this.TopDrag.Horizontal = true;
            this.TopDrag.TargetControl = this.TopPanel;
            this.TopDrag.Vertical = true;
            // 
            // bunifuFormFadeTransition1
            // 
            this.bunifuFormFadeTransition1.Delay = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 650);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SidePanel);
            this.Controls.Add(this.TopPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Form1";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closebt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxbt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minbt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.menubt)).EndInit();
            this.SidePanel.ResumeLayout(false);
            this.SidePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Bunifu.Framework.UI.BunifuGradientPanel SidePanel;
        private Bunifu.Framework.UI.BunifuFlatButton bunifuFlatButton4;
        private Bunifu.Framework.UI.BunifuFlatButton bunifuFlatButton3;
        private Bunifu.Framework.UI.BunifuFlatButton bunifuFlatButton1;
        private Bunifu.Framework.UI.BunifuFlatButton bunifuFlatButton2;
        private System.Windows.Forms.Label label2;
        private Bunifu.Framework.UI.BunifuGradientPanel TopPanel;
        private Bunifu.Framework.UI.BunifuImageButton closebt;
        private Bunifu.Framework.UI.BunifuImageButton maxbt;
        private Bunifu.Framework.UI.BunifuImageButton minbt;
        private Bunifu.Framework.UI.BunifuImageButton menubt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Panel panel1;
        private UserControls.ManualUC manualUC1;
        private System.Windows.Forms.Timer ConfigWaitForRespond;
        private UserControls.IMUSetting imuSetting1;
        private System.Windows.Forms.Timer IMUConfigWaitForRespond;
        private UserControls.AutoUC autoUC1;
        private System.Windows.Forms.Timer AutoTimer;
        private UserControls.VehicleSetting vehicleSetting1;
        private System.Windows.Forms.Timer KcontrolTimer;
        private System.Windows.Forms.Timer StartKctrlTimer;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private UserControls.HelperControls helperControls1;
        private Bunifu.Framework.UI.BunifuDragControl ControlPanelDrag;
        private System.Windows.Forms.Timer DefaultWaitForResponseTimer;
        private System.Windows.Forms.Timer StopKctrlTimer;
        private UserControls.AutoSetting autoSetting1;
        private Bunifu.Framework.UI.BunifuDragControl autoSettingDrag;
        private System.Windows.Forms.Timer SendWithResTimer;
        private UserControls.ProgressUC progressUC1;
        private Bunifu.Framework.UI.BunifuDragControl TopDrag;
        private Bunifu.Framework.UI.BunifuFormFadeTransition bunifuFormFadeTransition1;
    }
}

