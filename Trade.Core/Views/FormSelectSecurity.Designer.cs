namespace Trade.Views
{
    partial class FormSelectSecurity
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
            this._lbSecurities = new System.Windows.Forms.ListBox();
            this.bSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lbSecurities
            // 
            this._lbSecurities.FormattingEnabled = true;
            this._lbSecurities.Location = new System.Drawing.Point(4, 12);
            this._lbSecurities.Name = "_lbSecurities";
            this._lbSecurities.Size = new System.Drawing.Size(193, 342);
            this._lbSecurities.TabIndex = 1;
            this._lbSecurities.DoubleClick += new System.EventHandler(this.SecuritiesDoubleClick);
            // 
            // bSelect
            // 
            this.bSelect.Location = new System.Drawing.Point(4, 360);
            this.bSelect.Name = "bSelect";
            this.bSelect.Size = new System.Drawing.Size(193, 23);
            this.bSelect.TabIndex = 4;
            this.bSelect.Text = "Выбрать";
            this.bSelect.UseVisualStyleBackColor = true;
            this.bSelect.Click += new System.EventHandler(this.SelectClick);
            // 
            // FormSelectSecurity
            // 
            this.AcceptButton = this.bSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 389);
            this.Controls.Add(this.bSelect);
            this.Controls.Add(this._lbSecurities);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectSecurity";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Финансовые инструменты";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _lbSecurities;
        private System.Windows.Forms.Button bSelect;
    }
}