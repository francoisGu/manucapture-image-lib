namespace AforgeTest
{
    partial class Aforge
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
            this.lineImageBox = new System.Windows.Forms.PictureBox();
            this.testAforge = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.lineImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lineImageBox
            // 
            this.lineImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lineImageBox.Location = new System.Drawing.Point(12, 39);
            this.lineImageBox.Name = "lineImageBox";
            this.lineImageBox.Size = new System.Drawing.Size(910, 601);
            this.lineImageBox.TabIndex = 4;
            this.lineImageBox.TabStop = false;
            // 
            // testAforge
            // 
            this.testAforge.Location = new System.Drawing.Point(12, 5);
            this.testAforge.Name = "testAforge";
            this.testAforge.Size = new System.Drawing.Size(84, 28);
            this.testAforge.TabIndex = 5;
            this.testAforge.Text = "Test - Aforge";
            this.testAforge.UseVisualStyleBackColor = true;
            this.testAforge.Click += new System.EventHandler(this.testAforge_Click);
            // 
            // Aforge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 652);
            this.Controls.Add(this.testAforge);
            this.Controls.Add(this.lineImageBox);
            this.Name = "Aforge";
            this.Text = "Aforge";
            ((System.ComponentModel.ISupportInitialize)(this.lineImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox lineImageBox;
        private System.Windows.Forms.Button testAforge;
    }
}

