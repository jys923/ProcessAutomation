namespace SonoCap_PS
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.IDD_OPEN_BTN = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.IDC_VI_START_BTN = new System.Windows.Forms.Button();
            this.IDC_VI_END_BTN = new System.Windows.Forms.Button();
            this.imagePanel2 = new ImageQM.ImagePanel();
            this.imagePanel1 = new ImageQM.ImagePanel();
            this.SuspendLayout();
            // 
            // IDD_OPEN_BTN
            // 
            this.IDD_OPEN_BTN.Location = new System.Drawing.Point(354, 821);
            this.IDD_OPEN_BTN.Margin = new System.Windows.Forms.Padding(2);
            this.IDD_OPEN_BTN.Name = "IDD_OPEN_BTN";
            this.IDD_OPEN_BTN.Size = new System.Drawing.Size(118, 168);
            this.IDD_OPEN_BTN.TabIndex = 2;
            this.IDD_OPEN_BTN.Text = "영상 불러오기";
            this.IDD_OPEN_BTN.UseVisualStyleBackColor = true;
            this.IDD_OPEN_BTN.Click += new System.EventHandler(this.IDD_OPEN_BTN_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(526, 821);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 168);
            this.button1.TabIndex = 11;
            this.button1.Text = "검사";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(19, 925);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(132, 64);
            this.button3.TabIndex = 14;
            this.button3.Text = "MakeMaskImg";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(894, 809);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(802, 180);
            this.textBox1.TabIndex = 15;
            // 
            // IDC_VI_START_BTN
            // 
            this.IDC_VI_START_BTN.Location = new System.Drawing.Point(182, 821);
            this.IDC_VI_START_BTN.Name = "IDC_VI_START_BTN";
            this.IDC_VI_START_BTN.Size = new System.Drawing.Size(118, 168);
            this.IDC_VI_START_BTN.TabIndex = 16;
            this.IDC_VI_START_BTN.Text = "검사 시작";
            this.IDC_VI_START_BTN.UseVisualStyleBackColor = true;
            this.IDC_VI_START_BTN.Click += new System.EventHandler(this.IDC_VI_START_BTN_Click);
            // 
            // IDC_VI_END_BTN
            // 
            this.IDC_VI_END_BTN.Location = new System.Drawing.Point(698, 821);
            this.IDC_VI_END_BTN.Name = "IDC_VI_END_BTN";
            this.IDC_VI_END_BTN.Size = new System.Drawing.Size(118, 168);
            this.IDC_VI_END_BTN.TabIndex = 17;
            this.IDC_VI_END_BTN.Text = "검사 종료";
            this.IDC_VI_END_BTN.UseVisualStyleBackColor = true;
            this.IDC_VI_END_BTN.Click += new System.EventHandler(this.IDC_VI_END_BTN_Click);
            // 
            // imagePanel2
            // 
            this.imagePanel2.BackColor = System.Drawing.Color.Black;
            this.imagePanel2.Image = null;
            this.imagePanel2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.imagePanel2.Location = new System.Drawing.Point(894, 3);
            this.imagePanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.imagePanel2.Name = "imagePanel2";
            this.imagePanel2.Size = new System.Drawing.Size(800, 800);
            this.imagePanel2.TabIndex = 6;
            this.imagePanel2.Zoom = 1D;
            this.imagePanel2.ZoomStep = 0.2D;
            // 
            // imagePanel1
            // 
            this.imagePanel1.BackColor = System.Drawing.Color.Black;
            this.imagePanel1.Image = null;
            this.imagePanel1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.imagePanel1.Location = new System.Drawing.Point(17, 3);
            this.imagePanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.imagePanel1.Name = "imagePanel1";
            this.imagePanel1.Size = new System.Drawing.Size(800, 800);
            this.imagePanel1.TabIndex = 0;
            this.imagePanel1.Zoom = 1D;
            this.imagePanel1.ZoomStep = 0.2D;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1714, 1001);
            this.Controls.Add(this.IDC_VI_END_BTN);
            this.Controls.Add(this.IDC_VI_START_BTN);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.imagePanel2);
            this.Controls.Add(this.IDD_OPEN_BTN);
            this.Controls.Add(this.imagePanel1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageQM.ImagePanel imagePanel1;
        private System.Windows.Forms.Button IDD_OPEN_BTN;
        private ImageQM.ImagePanel imagePanel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button IDC_VI_START_BTN;
        private System.Windows.Forms.Button IDC_VI_END_BTN;
    }
}

