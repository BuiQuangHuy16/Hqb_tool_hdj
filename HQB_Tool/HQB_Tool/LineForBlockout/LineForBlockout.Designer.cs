
namespace AlphaBIM.LineForBlockout
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
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbAllBlockOut = new System.Windows.Forms.RadioButton();
            this.rbPickPoint = new System.Windows.Forms.RadioButton();
            this.cbbLineStyles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(143, 124);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(97, 30);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(295, 124);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rbAllBlockOut
            // 
            this.rbAllBlockOut.AccessibleRole = System.Windows.Forms.AccessibleRole.Caret;
            this.rbAllBlockOut.AutoSize = true;
            this.rbAllBlockOut.Location = new System.Drawing.Point(29, 26);
            this.rbAllBlockOut.Name = "rbAllBlockOut";
            this.rbAllBlockOut.Size = new System.Drawing.Size(81, 17);
            this.rbAllBlockOut.TabIndex = 1;
            this.rbAllBlockOut.Text = "All Blockout";
            this.rbAllBlockOut.UseVisualStyleBackColor = true;
            this.rbAllBlockOut.CheckedChanged += new System.EventHandler(this.rbAllBlockOut_CheckedChanged);
            // 
            // rbPickPoint
            // 
            this.rbPickPoint.AutoSize = true;
            this.rbPickPoint.Location = new System.Drawing.Point(29, 61);
            this.rbPickPoint.Name = "rbPickPoint";
            this.rbPickPoint.Size = new System.Drawing.Size(72, 17);
            this.rbPickPoint.TabIndex = 1;
            this.rbPickPoint.Text = "Pick point";
            this.rbPickPoint.UseVisualStyleBackColor = true;
            this.rbPickPoint.CheckedChanged += new System.EventHandler(this.rbPickPoint_CheckedChanged);
            // 
            // cbbLineStyles
            // 
            this.cbbLineStyles.FormattingEnabled = true;
            this.cbbLineStyles.Location = new System.Drawing.Point(146, 26);
            this.cbbLineStyles.Name = "cbbLineStyles";
            this.cbbLineStyles.Size = new System.Drawing.Size(249, 21);
            this.cbbLineStyles.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 187);
            this.Controls.Add(this.cbbLineStyles);
            this.Controls.Add(this.rbPickPoint);
            this.Controls.Add(this.rbAllBlockOut);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Name = "Form1";
            this.Text = "LineForBlockout";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbAllBlockOut;
        private System.Windows.Forms.RadioButton rbPickPoint;
        private System.Windows.Forms.ComboBox cbbLineStyles;
    }
}