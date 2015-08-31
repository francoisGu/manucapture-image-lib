using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using AForge.Video;
using AForge.Video.DirectShow;

namespace WindowsFormsApplication1
{
    public partial class MainWindow : Form
    {
        private CameraHandler myCamHandler = null;
        private PictureModificator myPicAnalyzer = new PictureModificator();
        public MainWindow()
        {
            InitializeComponent();
            // timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {


        }

        public void setNewListBoxContent()
        {
            try
            {
                lstDeviceList.Items.Clear();
                FilterInfoCollection videoDevices = myCamHandler.refreshCameraList();
                foreach (FilterInfo device in videoDevices)
                {
                    lstDeviceList.Items.Add(device.Name);
                }
                lstDeviceList.SelectedIndex = 0; //make dafault to first cam
            }
            catch (Exception e)
            {

            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {

        }



        private void btnSnapshot_Click(object sender, EventArgs e)
        {

        }


        private void analyzePicture()
        {
            try
            {
                myPicAnalyzer.setCurrentImage((Bitmap)pbSnapshotView.Image);
                myPicAnalyzer.applyGrayscale();
                myPicAnalyzer.applySobelEdgeFilter();
                myPicAnalyzer.markKnownForms();
                pbSnapshotView.Image = myPicAnalyzer.getCurrentImage();
            }
            catch (Exception exc)
            {

            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {


        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //pbSnapshotView.Image = myCamHandler.getSnapshot();
            // analyzePicture();
        }

        private void btnAnalyze_Click_1(object sender, EventArgs e)
        {
            analyzePicture();
            label1.Text = myPicAnalyzer.rad.ToString();
            label2.Text = myPicAnalyzer.tri.ToString();
            label3.Text = myPicAnalyzer.cir.ToString();
        }

        private void btnSnapshot_Click_1(object sender, EventArgs e)
        {
            pbSnapshotView.Image = myCamHandler.getSnapshot();
            analyzePicture();
        }

        private void refresh_Click_1(object sender, EventArgs e)
        {
            setNewListBoxContent();
        }

        private void start_Click_1(object sender, EventArgs e)
        {
            if (myCamHandler.setVideoSourceByIndex(lstDeviceList.SelectedIndex))
            {
                myCamHandler.startCapture();
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // myCamHandler = new CameraHandler(pbCamPreview);
            // setNewListBoxContent();
            // if (myCamHandler.setVideoSourceByIndex(lstDeviceList.SelectedIndex))
            // {
            //  myCamHandler.startCapture();
            // }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // ProcessImage((Bitmap)Bitmap.FromFile(openFileDialog.FileName));
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pbSnapshotView.Image = Image.FromFile(openFileDialog1.FileName);

                }
                catch
                {
                    MessageBox.Show("Failed loading selected image file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

    }
}