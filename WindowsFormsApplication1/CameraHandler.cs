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
    class CameraHandler
    {
        private FilterInfoCollection videoDevices = null;
        private VideoCaptureDevice videoSource = null;
        private PictureBox pbCamPreview = null;
        private Bitmap currentImage = null;


        // initialize with picturebox - used to load new frames later on
        public CameraHandler(PictureBox pbCamPreview)
        {
            this.pbCamPreview = pbCamPreview;
        }


        public Bitmap getSnapshot()
        {
            return currentImage;
        }

        // this function stops the image capturing 
        public void stopCapture()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }

        // starts the video capturering function
        public bool startCapture()
        {
            try
            {
                videoSource.Start();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        // sets a VideoSource to capture images
        public bool setVideoSourceByIndex(int index)
        {
            videoSource = null;
            try
            {
                videoSource = new VideoCaptureDevice(videoDevices[index].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(setNewFrame);
                return true;
            }
            catch
            {

                return false;
            }
        }

        /* refreshes the list of video sources for further processing and 
         * returns a list of all devices, found on the system */
        public FilterInfoCollection refreshCameraList()
        {
            videoDevices = null;
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                    throw new ApplicationException();
            }
            catch (ApplicationException)
            {

            }
            return videoDevices;
        }

        //eventhandler for every new frame
        private void setNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                currentImage = (Bitmap)eventArgs.Frame.Clone();

                //do show image in assigned picturebox
                pbCamPreview.Image = currentImage;
            }
            catch (Exception e)
            {

            }
        }

    }
}