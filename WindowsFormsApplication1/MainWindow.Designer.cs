namespace WindowsFormsApplication1
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.pbCamPreview = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lstDeviceList = new System.Windows.Forms.ComboBox();
            this.pbSnapshotView = new System.Windows.Forms.PictureBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.btnSnapshot = new System.Windows.Forms.Button();
            this.refresh = new System.Windows.Forms.Button();
            this.start = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbCamPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSnapshotView)).BeginInit();
            this.SuspendLayout();
            // 
            // pbCamPreview
            // 
            this.pbCamPreview.Location = new System.Drawing.Point(12, 12);
            this.pbCamPreview.Name = "pbCamPreview";
            this.pbCamPreview.Size = new System.Drawing.Size(216, 229);
            this.pbCamPreview.TabIndex = 0;
            this.pbCamPreview.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // lstDeviceList
            // 
            this.lstDeviceList.FormattingEnabled = true;
            this.lstDeviceList.Location = new System.Drawing.Point(12, 247);
            this.lstDeviceList.Name = "lstDeviceList";
            this.lstDeviceList.Size = new System.Drawing.Size(121, 21);
            this.lstDeviceList.TabIndex = 1;
            // 
            // pbSnapshotView
            // 
            this.pbSnapshotView.Location = new System.Drawing.Point(282, 12);
            this.pbSnapshotView.Name = "pbSnapshotView";
            this.pbSnapshotView.Size = new System.Drawing.Size(465, 377);
            this.pbSnapshotView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbSnapshotView.TabIndex = 2;
            this.pbSnapshotView.TabStop = false;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(102, 351);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyze.TabIndex = 3;
            this.btnAnalyze.Text = "Analyse";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click_1);
            // 
            // btnSnapshot
            // 
            this.btnSnapshot.Location = new System.Drawing.Point(12, 351);
            this.btnSnapshot.Name = "btnSnapshot";
            this.btnSnapshot.Size = new System.Drawing.Size(75, 23);
            this.btnSnapshot.TabIndex = 4;
            this.btnSnapshot.Text = "CreatSanpShot";
            this.btnSnapshot.UseVisualStyleBackColor = true;
            this.btnSnapshot.Click += new System.EventHandler(this.btnSnapshot_Click_1);
            // 
            // refresh
            // 
            this.refresh.Location = new System.Drawing.Point(12, 305);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(75, 23);
            this.refresh.TabIndex = 5;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click_1);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(102, 305);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 6;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click_1);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(183, 351);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "image upload";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(210, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(210, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "label3";
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(759, 449);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.start);
            this.Controls.Add(this.refresh);
            this.Controls.Add(this.btnSnapshot);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.pbSnapshotView);
            this.Controls.Add(this.lstDeviceList);
            this.Controls.Add(this.pbCamPreview);
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbCamPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSnapshotView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbCamPreview;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox lstDeviceList;
        private System.Windows.Forms.PictureBox pbSnapshotView;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Button btnSnapshot;
        private System.Windows.Forms.Button refresh;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}