using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace GenDS
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("Src: ");
            string sourceFile = Console.ReadLine();
            Console.Write("Des: ");
            string destFile = Console.ReadLine();
            FileStream fs1 = File.OpenRead(sourceFile);
            byte[] arr1 = new byte[(int)fs1.Length];
            fs1.Read(arr1, 0, (int)fs1.Length);

            FileStream ms1 = File.Create(destFile);
            GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);
            gz.Write(arr1, 0, arr1.Length);
            gz.Close();
            ms1.Close();

        }
    }
}
