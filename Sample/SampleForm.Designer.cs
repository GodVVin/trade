namespace Sample
{
    partial class SampleForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this._buttonConnect = new System.Windows.Forms.Button();
            this._textLog = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._buttonDisconnect = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _buttonConnect
            // 
            this._buttonConnect.Location = new System.Drawing.Point(12, 12);
            this._buttonConnect.Name = "_buttonConnect";
            this._buttonConnect.Size = new System.Drawing.Size(75, 23);
            this._buttonConnect.TabIndex = 0;
            this._buttonConnect.Text = "Connect";
            this._buttonConnect.UseVisualStyleBackColor = true;
            this._buttonConnect.Click += new System.EventHandler(this.ButtonConnectClick);
            // 
            // _textLog
            // 
            this._textLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._textLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._textLog.Location = new System.Drawing.Point(0, 0);
            this._textLog.Multiline = true;
            this._textLog.Name = "_textLog";
            this._textLog.ReadOnly = true;
            this._textLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._textLog.Size = new System.Drawing.Size(837, 466);
            this._textLog.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.PaleGreen;
            this.splitContainer1.Panel1.Controls.Add(this._buttonDisconnect);
            this.splitContainer1.Panel1.Controls.Add(this._buttonConnect);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._textLog);
            this.splitContainer1.Size = new System.Drawing.Size(837, 516);
            this.splitContainer1.SplitterDistance = 46;
            this.splitContainer1.TabIndex = 2;
            // 
            // _buttonDisconnect
            // 
            this._buttonDisconnect.Location = new System.Drawing.Point(93, 12);
            this._buttonDisconnect.Name = "_buttonDisconnect";
            this._buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this._buttonDisconnect.TabIndex = 3;
            this._buttonDisconnect.Text = "Disconnect";
            this._buttonDisconnect.UseVisualStyleBackColor = true;
            this._buttonDisconnect.Click += new System.EventHandler(this.ButtonDisconnectClick);
            // 
            // SampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 516);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SampleForm";
            this.Text = "Sample";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SampleFormFormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonConnect;
        private System.Windows.Forms.TextBox _textLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button _buttonDisconnect;
    }
}

