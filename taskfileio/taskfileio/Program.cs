using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace taskfileio
{
    class Program
    {
        static readonly object _pickupfileLock = new object();

        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                ThreadPool.QueueUserWorkItem(state => fun("1"));
                ThreadPool.QueueUserWorkItem(state => fun("2"));
                ThreadPool.QueueUserWorkItem(state => fun("3"));
            }



            //string[] fileName = { @".\1123.txt", @".\456.txt" };

            //foreach (var path in fileName)
            //{
            //    if (!File.Exists(path))
            //    {
            //        using (var tw = new StreamWriter(path, true))
            //        {
            //            tw.Close();
            //        }
            //    }
            //}

            //for (int i = 0; i < 500; i++)
            //{
            //    ThreadPool.QueueUserWorkItem(state => fileio(i.ToString()));

            //}

            //for (int i = 0; i <500; i++)
            //{
            //    fileio();
            //}


            Console.Read();
        }


        static void fun(string msg)
        {

            Console.WriteLine(msg);
        }

        static void fileio(string str = "")
        {
            lock (_pickupfileLock)
            {
                //using (FileStream file = new FileStream(@".\test.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
                //using (StreamWriter writer = new StreamWriter(file, Encoding.Unicode))
                //{
                //    writer.Write(","+str);
                //}
                //if (str == "")
                //{
                //    string[] fileText;
                //    fileText = File.ReadAllLines(@".\test.txt");
                //    // 清空 file
                //    //File.WriteAllText(@".\test.txt", "");
                //}
                //else
                //{
                //    string fileName = @".\test.txt";
                //    StringBuilder ftsb = new StringBuilder(str);

                //    ftsb.AppendLine();
                //    File.AppendAllText(fileName, ftsb.ToString());
                //}

            }
        }
    }
}
