namespace VideoUploader
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSelect = new System.Windows.Forms.Button();
            this.listBoxSuccess = new System.Windows.Forms.ListBox();
            this.listBoxFailed = new System.Windows.Forms.ListBox();
            this.listBoxAnmeldung = new System.Windows.Forms.ListBox();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.textBoxCustomer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxReady = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBoxUpload = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(6, 160);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(123, 23);
            this.buttonSelect.TabIndex = 0;
            this.buttonSelect.Text = "Select";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // listBoxSuccess
            // 
            this.listBoxSuccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxSuccess.FormattingEnabled = true;
            this.listBoxSuccess.HorizontalScrollbar = true;
            this.listBoxSuccess.Location = new System.Drawing.Point(6, 315);
            this.listBoxSuccess.Name = "listBoxSuccess";
            this.listBoxSuccess.Size = new System.Drawing.Size(696, 108);
            this.listBoxSuccess.TabIndex = 2;
            // 
            // listBoxFailed
            // 
            this.listBoxFailed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxFailed.FormattingEnabled = true;
            this.listBoxFailed.HorizontalScrollbar = true;
            this.listBoxFailed.Location = new System.Drawing.Point(6, 442);
            this.listBoxFailed.Name = "listBoxFailed";
            this.listBoxFailed.Size = new System.Drawing.Size(696, 108);
            this.listBoxFailed.TabIndex = 3;
            // 
            // listBoxAnmeldung
            // 
            this.listBoxAnmeldung.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAnmeldung.FormattingEnabled = true;
            this.listBoxAnmeldung.HorizontalScrollbar = true;
            this.listBoxAnmeldung.Location = new System.Drawing.Point(6, 19);
            this.listBoxAnmeldung.Name = "listBoxAnmeldung";
            this.listBoxAnmeldung.Size = new System.Drawing.Size(696, 108);
            this.listBoxAnmeldung.TabIndex = 4;
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(6, 146);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(123, 23);
            this.buttonUpload.TabIndex = 6;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // textBoxCustomer
            // 
            this.textBoxCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCustomer.Location = new System.Drawing.Point(131, 133);
            this.textBoxCustomer.Name = "textBoxCustomer";
            this.textBoxCustomer.ReadOnly = true;
            this.textBoxCustomer.Size = new System.Drawing.Size(571, 20);
            this.textBoxCustomer.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Selected Customer:";
            // 
            // listBoxReady
            // 
            this.listBoxReady.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxReady.FormattingEnabled = true;
            this.listBoxReady.Location = new System.Drawing.Point(6, 32);
            this.listBoxReady.Name = "listBoxReady";
            this.listBoxReady.Size = new System.Drawing.Size(696, 108);
            this.listBoxReady.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxAnmeldung);
            this.groupBox1.Controls.Add(this.textBoxCustomer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonSelect);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(712, 191);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Neuanmeldung";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBoxUpload);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.buttonUpload);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.listBoxReady);
            this.groupBox2.Controls.Add(this.listBoxSuccess);
            this.groupBox2.Controls.Add(this.listBoxFailed);
            this.groupBox2.Location = new System.Drawing.Point(12, 209);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(713, 562);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Upload";
            // 
            // listBoxUpload
            // 
            this.listBoxUpload.FormattingEnabled = true;
            this.listBoxUpload.Location = new System.Drawing.Point(6, 188);
            this.listBoxUpload.Name = "listBoxUpload";
            this.listBoxUpload.Size = new System.Drawing.Size(696, 108);
            this.listBoxUpload.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Im Upload:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 426);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Fehlgeschlagen:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Erledigt:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Bereit zum Upload:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 784);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.ListBox listBoxSuccess;
        private System.Windows.Forms.ListBox listBoxFailed;
        private System.Windows.Forms.ListBox listBoxAnmeldung;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.TextBox textBoxCustomer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxReady;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBoxUpload;
        private System.Windows.Forms.Label label5;
    }
}

