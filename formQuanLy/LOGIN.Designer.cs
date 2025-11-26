namespace formQuanLy
{
    partial class formLOGIN
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
            this.gbDangNhap = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.checkHienMK = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lbUser = new System.Windows.Forms.Label();
            this.btnThoat = new System.Windows.Forms.Button();
            this.pnLOGIN = new System.Windows.Forms.Panel();
            this.gbDangNhap.SuspendLayout();
            this.pnLOGIN.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDangNhap
            // 
            this.gbDangNhap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gbDangNhap.BackColor = System.Drawing.Color.Ivory;
            this.gbDangNhap.Controls.Add(this.btnCancel);
            this.gbDangNhap.Controls.Add(this.btnOK);
            this.gbDangNhap.Controls.Add(this.checkHienMK);
            this.gbDangNhap.Controls.Add(this.txtPassword);
            this.gbDangNhap.Controls.Add(this.lbPassword);
            this.gbDangNhap.Controls.Add(this.txtUser);
            this.gbDangNhap.Controls.Add(this.lbUser);
            this.gbDangNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbDangNhap.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDangNhap.Location = new System.Drawing.Point(230, 51);
            this.gbDangNhap.Name = "gbDangNhap";
            this.gbDangNhap.Padding = new System.Windows.Forms.Padding(10);
            this.gbDangNhap.Size = new System.Drawing.Size(507, 474);
            this.gbDangNhap.TabIndex = 0;
            this.gbDangNhap.TabStop = false;
            this.gbDangNhap.Text = "LOGIN";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.LightBlue;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(295, 361);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 30);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.LightBlue;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(144)))), ((int)(((byte)(204)))));
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(134)))), ((int)(((byte)(194)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(151, 361);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 30);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // checkHienMK
            // 
            this.checkHienMK.AutoSize = true;
            this.checkHienMK.FlatAppearance.BorderSize = 0;
            this.checkHienMK.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkHienMK.Location = new System.Drawing.Point(138, 275);
            this.checkHienMK.Name = "checkHienMK";
            this.checkHienMK.Size = new System.Drawing.Size(146, 25);
            this.checkHienMK.TabIndex = 4;
            this.checkHienMK.Text = "Show password";
            this.checkHienMK.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Location = new System.Drawing.Point(138, 214);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(261, 31);
            this.txtPassword.TabIndex = 3;
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Location = new System.Drawing.Point(134, 181);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(90, 25);
            this.lbPassword.TabIndex = 2;
            this.lbPassword.Text = "Password";
            // 
            // txtUser
            // 
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.Location = new System.Drawing.Point(138, 119);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(261, 31);
            this.txtUser.TabIndex = 1;
            // 
            // lbUser
            // 
            this.lbUser.AutoSize = true;
            this.lbUser.Location = new System.Drawing.Point(134, 86);
            this.lbUser.Name = "lbUser";
            this.lbUser.Size = new System.Drawing.Size(96, 25);
            this.lbUser.TabIndex = 0;
            this.lbUser.Text = "Username";
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThoat.BackColor = System.Drawing.Color.LightBlue;
            this.btnThoat.FlatAppearance.BorderSize = 0;
            this.btnThoat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnThoat.Location = new System.Drawing.Point(855, 528);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(100, 43);
            this.btnThoat.TabIndex = 1;
            this.btnThoat.Text = "Exit";
            this.btnThoat.UseVisualStyleBackColor = false;
            // 
            // pnLOGIN
            // 
            this.pnLOGIN.BackColor = System.Drawing.Color.LightYellow;
            this.pnLOGIN.Controls.Add(this.btnThoat);
            this.pnLOGIN.Controls.Add(this.gbDangNhap);
            this.pnLOGIN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLOGIN.Location = new System.Drawing.Point(0, 0);
            this.pnLOGIN.Name = "pnLOGIN";
            this.pnLOGIN.Padding = new System.Windows.Forms.Padding(20);
            this.pnLOGIN.Size = new System.Drawing.Size(978, 594);
            this.pnLOGIN.TabIndex = 1;
            // 
            // formLOGIN
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightYellow;
            this.ClientSize = new System.Drawing.Size(978, 594);
            this.Controls.Add(this.pnLOGIN);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "formLOGIN";
            this.Text = "LOGIN";
            this.gbDangNhap.ResumeLayout(false);
            this.gbDangNhap.PerformLayout();
            this.pnLOGIN.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDangNhap;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox checkHienMK;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lbUser;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Panel pnLOGIN;
    }
}