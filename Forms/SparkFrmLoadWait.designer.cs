namespace SparkControls.Forms
{
    partial class SparkFrmLoadWait<T>
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
            this.trtLabel1 = new SparkControls.Controls.SparkLabel();
            this.SuspendLayout();
            // 
            // trtLabel1
            // 
            this.trtLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trtLabel1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.trtLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.trtLabel1.Location = new System.Drawing.Point(1, 32);
            this.trtLabel1.Name = "trtLabel1";
            this.trtLabel1.Size = new System.Drawing.Size(474, 106);
            this.trtLabel1.TabIndex = 1;
            this.trtLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SparkFrmLoadWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 139);
            this.ControlBox = false;
            this.Controls.Add(this.trtLabel1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "SparkFrmLoadWait";
            this.ShowInTaskbar = false;
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "请稍等......";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.SparkLabel trtLabel1;
    }
}