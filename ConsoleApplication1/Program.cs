using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            String fileName = @"pic3.jpg";
            if (File.Exists(fileName))
            {
                ObjectDetect shapes = new ObjectDetect(fileName);
                shapes.SaveShapeImg("pic3_.jpg");
                shapes.SaveTextFile("3.txt");
            }
            else
            {
                Console.WriteLine("File not exists");
            }
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
