namespace KafeOtomasyonu
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbPersonel = new System.Windows.Forms.ComboBox();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn9 = new System.Windows.Forms.Button();
            this.btn0 = new System.Windows.Forms.Button();
            this.txtPin = new System.Windows.Forms.TextBox();
            this.btnSil = new System.Windows.Forms.Button();
            this.btnGiris = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPersonelSec = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbPersonel
            // 
            this.cmbPersonel.FormattingEnabled = true;
            this.cmbPersonel.Location = new System.Drawing.Point(17, 101);
            this.cmbPersonel.Name = "cmbPersonel";
            this.cmbPersonel.Size = new System.Drawing.Size(168, 28);
            this.cmbPersonel.TabIndex = 0;
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(259, 101);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(65, 65);
            this.btn1.TabIndex = 1;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn2
            // 
            this.btn2.Location = new System.Drawing.Point(352, 101);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(65, 65);
            this.btn2.TabIndex = 1;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn3
            // 
            this.btn3.Location = new System.Drawing.Point(447, 101);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(65, 65);
            this.btn3.TabIndex = 1;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn4
            // 
            this.btn4.Location = new System.Drawing.Point(259, 192);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(65, 65);
            this.btn4.TabIndex = 1;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn5
            // 
            this.btn5.Location = new System.Drawing.Point(352, 192);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(65, 65);
            this.btn5.TabIndex = 1;
            this.btn5.Text = "5";
            this.btn5.UseVisualStyleBackColor = true;
            this.btn5.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn6
            // 
            this.btn6.Location = new System.Drawing.Point(447, 192);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(65, 65);
            this.btn6.TabIndex = 1;
            this.btn6.Text = "6";
            this.btn6.UseVisualStyleBackColor = true;
            this.btn6.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn7
            // 
            this.btn7.Location = new System.Drawing.Point(259, 283);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(65, 65);
            this.btn7.TabIndex = 1;
            this.btn7.Text = "7";
            this.btn7.UseVisualStyleBackColor = true;
            this.btn7.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn8
            // 
            this.btn8.Location = new System.Drawing.Point(352, 283);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(65, 65);
            this.btn8.TabIndex = 1;
            this.btn8.Text = "8";
            this.btn8.UseVisualStyleBackColor = true;
            this.btn8.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn9
            // 
            this.btn9.Location = new System.Drawing.Point(447, 283);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(65, 65);
            this.btn9.TabIndex = 1;
            this.btn9.Text = "9";
            this.btn9.UseVisualStyleBackColor = true;
            this.btn9.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // btn0
            // 
            this.btn0.Location = new System.Drawing.Point(352, 381);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(65, 65);
            this.btn0.TabIndex = 1;
            this.btn0.Text = "0";
            this.btn0.UseVisualStyleBackColor = true;
            this.btn0.Click += new System.EventHandler(this.NumpadTus_Click);
            // 
            // txtPin
            // 
            this.txtPin.Location = new System.Drawing.Point(259, 21);
            this.txtPin.MaxLength = 4;
            this.txtPin.Multiline = true;
            this.txtPin.Name = "txtPin";
            this.txtPin.PasswordChar = '*';
            this.txtPin.Size = new System.Drawing.Size(253, 59);
            this.txtPin.TabIndex = 2;
            this.txtPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPin_KeyPress);
            // 
            // btnSil
            // 
            this.btnSil.Location = new System.Drawing.Point(259, 381);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(65, 65);
            this.btnSil.TabIndex = 1;
            this.btnSil.Text = "SİL";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            // 
            // btnGiris
            // 
            this.btnGiris.Location = new System.Drawing.Point(447, 381);
            this.btnGiris.Name = "btnGiris";
            this.btnGiris.Size = new System.Drawing.Size(65, 65);
            this.btnGiris.TabIndex = 1;
            this.btnGiris.Text = "GİRİŞ";
            this.btnGiris.UseVisualStyleBackColor = true;
            this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "HIZLI KASA GİRİŞİ";
            // 
            // lblPersonelSec
            // 
            this.lblPersonelSec.AutoSize = true;
            this.lblPersonelSec.Location = new System.Drawing.Point(17, 64);
            this.lblPersonelSec.Name = "lblPersonelSec";
            this.lblPersonelSec.Size = new System.Drawing.Size(130, 20);
            this.lblPersonelSec.TabIndex = 4;
            this.lblPersonelSec.Text = "Personel Seçiniz:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-2, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(255, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "\"Lütfen 4 haneli şifrenizi tuşlayınız.\"";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 468);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblPersonelSec);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPin);
            this.Controls.Add(this.btn0);
            this.Controls.Add(this.btnSil);
            this.Controls.Add(this.btnGiris);
            this.Controls.Add(this.btn9);
            this.Controls.Add(this.btn8);
            this.Controls.Add(this.btn7);
            this.Controls.Add(this.btn6);
            this.Controls.Add(this.btn5);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.cmbPersonel);
            this.Name = "Form1";
            this.Text = "GİRİŞ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPersonel;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Button btn6;
        private System.Windows.Forms.Button btn7;
        private System.Windows.Forms.Button btn8;
        private System.Windows.Forms.Button btn9;
        private System.Windows.Forms.Button btn0;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.Button btnSil;
        private System.Windows.Forms.Button btnGiris;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPersonelSec;
        private System.Windows.Forms.Label label2;
    }
}

