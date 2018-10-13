using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace GenDS
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string sourceFile = @"d:\config";
            string destFile = @"d:\ds";
            FileStream fs1 = File.OpenRead(sourceFile);
            byte[] arr1 = new byte[(int)fs1.Length];
            fs1.Read(arr1, 0, (int)fs1.Length);
            for (int i = 0; i < arr1.Length; i++)
            {
                arr1[i] = (byte)(arr1[i] + i);
            }

            FileStream ms1 = File.Create(destFile);
            GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);

            gz.Write(arr1, 0, arr1.Length);


            gz.Close();
            ms1.Close();


            File.Delete(@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Debug\ds");
            File.Delete(@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Release\ds");
            File.Copy(@"d:\ds",@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Debug\ds");
            File.Copy(@"d:\ds",@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Release\ds");


        }
        public static string CreateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
