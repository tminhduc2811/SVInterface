namespace ThesisInterface.UserControls
{
    partial class ProgressUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressUC));
            this.ProgressBar = new Bunifu.Framework.UI.BunifuCircleProgressbar();
            this.TopPanel = new Bunifu.Framework.UI.BunifuGradientPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.HideBt = new Bunifu.Framework.UI.BunifuFlatButton();
            this.CancelBt = new Bunifu.Framework.UI.BunifuFlatButton();
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgressBar
            // 
            this.ProgressBar.animated = false;
            this.ProgressBar.animationIterval = 5;
            this.ProgressBar.animationSpeed = 300;
            this.ProgressBar.BackColor = System.Drawing.Color.White;
            this.ProgressBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ProgressBar.BackgroundImage")));
            this.ProgressBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F);
            this.ProgressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.ProgressBar.LabelVisible = true;
            this.ProgressBar.LineProgressThickness = 8;
            this.ProgressBar.LineThickness = 5;
            this.ProgressBar.Location = new System.Drawing.Point(128, 47);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.ProgressBar.MaxValue = 100;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.ProgressBackColor = System.Drawing.Color.Gainsboro;
            this.ProgressBar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.ProgressBar.Size = new System.Drawing.Size(214, 214);
            this.ProgressBar.TabIndex = 0;
            this.ProgressBar.Value = 0;
            // 
            // TopPanel
            // 
            this.TopPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TopPanel.BackgroundImage")));
            this.TopPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TopPanel.Controls.Add(this.label1);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.GradientBottomLeft = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.TopPanel.GradientBottomRight = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.TopPanel.GradientTopLeft = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(24)))), ((int)(((byte)(91)))));
            this.TopPanel.GradientTopRight = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(49)))), ((int)(((byte)(85)))));
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Quality = 10;
            this.TopPanel.Size = new System.Drawing.Size(480, 47);
            this.TopPanel.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Ailerons", 17.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(5)))), ((int)(((byte)(65)))));
            this.label1.Location = new System.Drawing.Point(79, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 28);
            this.label1.TabIndex = 1;
            this.label1.Text = "SENDING MAP TO VEHICLE";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HideBt
            // 
            this.HideBt.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.HideBt.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.HideBt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(24)))), ((int)(((byte)(91)))));
            this.HideBt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HideBt.BorderRadius = 0;
            this.HideBt.ButtonText = "Hide";
            this.HideBt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HideBt.DisabledColor = System.Drawing.Color.Gray;
            this.HideBt.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideBt.Iconcolor = System.Drawing.Color.Transparent;
            this.HideBt.Iconimage = ((System.Drawing.Image)(resources.GetObject("HideBt.Iconimage")));
            this.HideBt.Iconimage_right = null;
            this.HideBt.Iconimage_right_Selected = null;
            this.HideBt.Iconimage_Selected = null;
            this.HideBt.IconMarginLeft = 0;
            this.HideBt.IconMarginRight = 0;
            this.HideBt.IconRightVisible = true;
            this.HideBt.IconRightZoom = 0D;
            this.HideBt.IconVisible = true;
            this.HideBt.IconZoom = 90D;
            this.HideBt.IsTab = false;
            this.HideBt.Location = new System.Drawing.Point(86, 274);
            this.HideBt.Margin = new System.Windows.Forms.Padding(4);
            this.HideBt.Name = "HideBt";
            this.HideBt.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(24)))), ((int)(((byte)(91)))));
            this.HideBt.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(119)))), ((int)(((byte)(37)))));
            this.HideBt.OnHoverTextColor = System.Drawing.Color.White;
            this.HideBt.selected = false;
            this.HideBt.Size = new System.Drawing.Size(123, 40);
            this.HideBt.TabIndex = 11;
            this.HideBt.Text = "Hide";
            this.HideBt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HideBt.Textcolor = System.Drawing.Color.White;
            this.HideBt.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // CancelBt
            // 
            this.CancelBt.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(9)))), ((int)(((byte)(42)))));
            this.CancelBt.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelBt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(24)))), ((int)(((byte)(91)))));
            this.CancelBt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelBt.BorderRadius = 0;
            this.CancelBt.ButtonText = "Cancel";
            this.CancelBt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CancelBt.DisabledColor = System.Drawing.Color.Gray;
            this.CancelBt.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBt.Iconcolor = System.Drawing.Color.Transparent;
            this.CancelBt.Iconimage = ((System.Drawing.Image)(resources.GetObject("CancelBt.Iconimage")));
            this.CancelBt.Iconimage_right = null;
            this.CancelBt.Iconimage_right_Selected = null;
            this.CancelBt.Iconimage_Selected = null;
            this.CancelBt.IconMarginLeft = 0;
            this.CancelBt.IconMarginRight = 0;
            this.CancelBt.IconRightVisible = true;
            this.CancelBt.IconRightZoom = 0D;
            this.CancelBt.IconVisible = true;
            this.CancelBt.IconZoom = 90D;
            this.CancelBt.IsTab = false;
            this.CancelBt.Location = new System.Drawing.Point(259, 274);
            this.CancelBt.Margin = new System.Windows.Forms.Padding(4);
            this.CancelBt.Name = "CancelBt";
            this.CancelBt.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(24)))), ((int)(((byte)(91)))));
            this.CancelBt.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(119)))), ((int)(((byte)(37)))));
            this.CancelBt.OnHoverTextColor = System.Drawing.Color.White;
            this.CancelBt.selected = false;
            this.CancelBt.Size = new System.Drawing.Size(123, 40);
            this.CancelBt.TabIndex = 11;
            this.CancelBt.Text = "Cancel";
            this.CancelBt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CancelBt.Textcolor = System.Drawing.Color.White;
            this.CancelBt.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // ProgressUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.CancelBt);
            this.Controls.Add(this.HideBt);
            this.Controls.Add(this.TopPanel);
            this.Controls.Add(this.ProgressBar);
            this.MaximumSize = new System.Drawing.Size(480, 323);
            this.MinimumSize = new System.Drawing.Size(480, 323);
            this.Name = "ProgressUC";
            this.Size = new System.Drawing.Size(480, 323);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Bunifu.Framework.UI.BunifuGradientPanel TopPanel;
        private System.Windows.Forms.Label label1;
        public Bunifu.Framework.UI.BunifuFlatButton HideBt;
        public Bunifu.Framework.UI.BunifuFlatButton CancelBt;
        public Bunifu.Framework.UI.BunifuCircleProgressbar ProgressBar;
    }
}
