using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.Office.Interop.Word;
namespace GenDS
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //CreateDoc c = new CreateDoc();
            //c.CreateNewDocument(@"d:\temp.docx");
            //c.InsertText("Title", "甲钴胺片溶出度");
            //c.InsertText("Lot", "批号：\t1807001 1807002");
            //Table t = c.InsertTable("List", 15, 3, 1);
            //c.SaveDocument(@"d:\a.docx");

            FileStream srcFs = File.OpenRead(@"d:\ds");
            //XmlDocument dataFile = new XmlDocument();
            GZipStream g = new GZipStream(srcFs, CompressionMode.Decompress);             //compress stream
            MemoryStream ms = new MemoryStream();
            byte[] bytes = new byte[40960];
            int n;
            while ((n = g.Read(bytes, 0, bytes.Length)) > 0)
            {
                ms.Write(bytes, 0, n);
            }
            g.Close();
            ms.Position = 0;
            byte[] eData = ms.ToArray();
            for (int i = 0; i < eData.Length; i++)
            {
                eData[i] = (byte)(eData[i] - i);
            }
            ms = new MemoryStream(eData);
            FileStream fsw = File.Create(@"d:\1");
            fsw.Write(eData, 0, eData.Length);
            fsw.Close();




            //dataFile.Load(ms);
            srcFs.Close();













            #region compression info
            //string sourceFile = @"d:\config";
            //string destFile = @"d:\ds";
            //FileStream fs1 = File.OpenRead(sourceFile);
            //byte[] arr1 = new byte[(int)fs1.Length];
            //fs1.Read(arr1, 0, (int)fs1.Length);
            //for (int i = 0; i < arr1.Length; i++)
            //{
            //    arr1[i] = (byte)(arr1[i] + i);
            //}

            //FileStream ms1 = File.Create(destFile);
            //GZipStream gz = new GZipStream(ms1, CompressionMode.Compress);

            //gz.Write(arr1, 0, arr1.Length);


            //gz.Close();
            //ms1.Close();


            //File.Delete(@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Debug\ds");
            //File.Delete(@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Release\ds");
            //File.Copy(@"d:\ds",@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Debug\ds");
            //File.Copy(@"d:\ds",@"C:\Users\xulei\Source\Repos\LC-List\LC_List\Empower List\bin\Release\ds");
            #endregion

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
    class CreateDoc
    {
        private Microsoft.Office.Interop.Word.Application wordApp = null;
        private Document wordDoc = null;

        public Application Application
        {
            get
            {
                return wordApp;
            }

            set
            {
                wordApp = value;
            }
        }

        public Document Document
        {
            get
            {
                return wordDoc;
            }

            set
            {
                wordDoc = value;
            }
        }

        //通过模板创建新文档
        public void CreateNewDocument(string filePath)
        {
            killWinWordProcess();
            wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            wordApp.Visible = false;
            object missing = System.Reflection.Missing.Value;
            object templateName = filePath;
            wordDoc = wordApp.Documents.Open(ref templateName, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing);
        }

        //保存新文件
        public void SaveDocument(string filePath)
        {
            object fileName = filePath;
            object format = WdSaveFormat.wdFormatXMLDocument;  //保存格式
            object miss = System.Reflection.Missing.Value;
            wordDoc.SaveAs2(ref fileName, ref format, ref miss, ref miss, ref miss, ref miss, ref miss,
                ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

            object SaveChanges = WdSaveOptions.wdSaveChanges;
            object OriginalFormat = WdOriginalFormat.wdWordDocument;
            object RouteDocument = false;
            wordDoc.Close(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
            wordApp.Quit(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
        }

        //在书签处插入值
        public bool InsertValue(string bookmark, string value)
        {
            object bkObj = bookmark;
            if (wordApp.ActiveDocument.Bookmarks.Exists(bookmark))
            {
                wordApp.ActiveDocument.Bookmarks[bkObj].Select();
                wordApp.Selection.TypeText(value);
                return true;
            }

            return false;
        }

        //插入表格,bookmark书签
        public Table InsertTable(string bookmark, int rows, int columns, float width)
        {
            object miss = System.Reflection.Missing.Value;
            object toStart = bookmark;
            Range range = wordDoc.Bookmarks[toStart].Range;//表格插入位置
            Table newTable = wordDoc.Tables.Add(range, rows, columns, ref miss, ref miss);
            //设置表的格式
            newTable.Borders.Enable = 1; //允许有边框，默认没有边框(为0时报错，1为实线边框，2、3为虚线边框，以后的数字没试过)
            newTable.Borders.OutsideLineWidth = WdLineWidth.wdLineWidth050pt;//边框宽度
            if (width != 0)
            {
                newTable.PreferredWidth = width;//表格宽度
            }
            newTable.AllowPageBreaks = false;
            return newTable;
        }

        //合并单元格 表名,开始行号,开始列号,结束行号,结束列号
        public void MergeCell(Microsoft.Office.Interop.Word.Table table, int row1, int column1, int row2, int column2)
        {
            table.Cell(row1, column1).Merge(table.Cell(row2, column2));
        }

        //设置表格内容对齐方式Align水平方向，Vertical垂直方向(左对齐，居中对齐，右对齐分别对应Align和Vertical的值为-1,0,1)
        public void SetParagraph_Table(Microsoft.Office.Interop.Word.Table table, int Align, int Vertical)
        {
            switch (Align)
            {
                case -1: table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; break;//左对齐
                case 0: table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; break;//水平居中
                case 1: table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; break;//右对齐
            }
            switch (Vertical)
            {
                case -1: table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop; break;//顶端对齐
                case 0: table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter; break;//垂直居中
                case 1: table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom; break;//底端对齐
            }
        }

        //设置表格字体
        public void SetFont_Table(Microsoft.Office.Interop.Word.Table table, string fontName, double size)
        {
            if (size != 0)
            {
                table.Range.Font.Size = Convert.ToSingle(size);
            }
            if (fontName != "")
            {
                table.Range.Font.Name = fontName;
            }
        }

        //是否使用边框,n表格的序号,use是或否
        public void UseBorder(int n, bool use)
        {
            if (use)
            {
                wordDoc.Content.Tables[n].Borders.Enable = 1; //允许有边框，默认没有边框(为0时无边框，1为实线边框，2、3为虚线边框，以后的数字没试过)
            }
            else
            {
                wordDoc.Content.Tables[n].Borders.Enable = 2; //允许有边框，默认没有边框(为0时无边框，1为实线边框，2、3为虚线边框，以后的数字没试过)
            }
        }

        //给表格插入一行,n表格的序号从1开始记
        public void AddRow(int n)
        {
            object miss = System.Reflection.Missing.Value;
            wordDoc.Content.Tables[n].Rows.Add(ref miss);
        }

        //给表格添加一行
        public void AddRow(Microsoft.Office.Interop.Word.Table table)
        {
            object miss = System.Reflection.Missing.Value;
            table.Rows.Add(ref miss);
        }

        //给表格插入rows行,n为表格的序号
        public void AddRow(int n, int rows)
        {
            object miss = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Table table = wordDoc.Content.Tables[n];
            for (int i = 0; i < rows; i++)
            {
                table.Rows.Add(ref miss);
            }
        }

        //给表格中单元格插入元素，table所在表格，row行号，column列号，value插入的元素
        public void InsertCell(Microsoft.Office.Interop.Word.Table table, int row, int column, string value)
        {
            table.Cell(row, column).Range.Text = value;
        }

        //给表格中单元格插入元素，n表格的序号从1开始记，row行号，column列号，value插入的元素
        public void InsertCell(int n, int row, int column, string value)
        {
            wordDoc.Content.Tables[n].Cell(row, column).Range.Text = value;
        }

        //给表格插入一行数据，n为表格的序号，row行号，columns列数，values插入的值
        public void InsertCell(int n, int row, int columns, string[] values)
        {
            Microsoft.Office.Interop.Word.Table table = wordDoc.Content.Tables[n];
            for (int i = 0; i < columns; i++)
            {
                table.Cell(row, i + 1).Range.Text = values[i];
            }
        }

        //插入图片
        public void InsertPicture(string bookmark, string picturePath, float width, float hight)
        {
            object miss = System.Reflection.Missing.Value;
            object oStart = bookmark;
            Object linkToFile = false;    //图片是否为外部链接
            Object saveWithDocument = true; //图片是否随文档一起保存
            object range = wordDoc.Bookmarks.get_Item(ref oStart).Range;//图片插入位置
            wordDoc.InlineShapes.AddPicture(picturePath, ref linkToFile, ref saveWithDocument, ref range);
            wordDoc.Application.ActiveDocument.InlineShapes[1].Width = width; //设置图片宽度
            wordDoc.Application.ActiveDocument.InlineShapes[1].Height = hight; //设置图片高度
        }

        //插入一段文字,text为文字内容
        public void InsertText(string bookmark, string text)
        {
            object oStart = bookmark;
            object range = wordDoc.Bookmarks.get_Item(ref oStart).Range;
            Paragraph wp = wordDoc.Content.Paragraphs.Add(ref range);
            wp.Format.SpaceBefore = 6;
            wp.Range.Text = text;
            wp.Format.SpaceAfter = 24;
            wp.Range.InsertParagraphAfter();
            wordDoc.Paragraphs.Last.Range.Text = "\n";
        }

        //杀掉winword.exe进程
        public void killWinWordProcess()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (System.Diagnostics.Process process in processes)
            {
                bool b = process.MainWindowTitle == "";
                if (process.MainWindowTitle == "")
                {
                    process.Kill();
                }
            }
        }
    }
}
