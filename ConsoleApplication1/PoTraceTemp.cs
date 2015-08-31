using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class PoTraceTemp
    {
        /// <summary>
        /// In this stage we just run the .exe file to convert the image to svg
        /// </summary>
        /// <param name="fileName"></param>
        public void CreateSVGFromImg(String fileName)
        {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            startInfo.Arguments = 'D:\potrace-1.12.win32\potrace.exe -s C:\Users\F\Desktop\1.bmp -o C:\Users\F\Desktop\1.svg';
            process.StartInfo = startInfo;
            process.Start();
        }

    }
}
