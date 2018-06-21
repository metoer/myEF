using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataGenerator
{
    public class FileCopy
    {
        public static Queue<FileItem> Sources
        {
            get;
            set;
        }

        public static bool IsCopy
        {
            get;
            set;
        }

        static FileCopy()
        {
            Sources = new Queue<FileItem>();
            Start();
        }


        private static void Start()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (Sources.Count == 0)
                        {
                            continue;
                        }

                        FileItem fileItem = null;
                        lock (((ICollection)Sources).SyncRoot)
                        {
                            fileItem = Sources.Dequeue();
                        }

                        File.Copy(fileItem.SourceFileName, fileItem.DestFileName);
                    }
                    catch
                    { }
                    finally
                    {
                        Thread.Sleep(1);
                    }
                }
            }) { IsBackground = true }.Start();
        }

        public static void Add(string sourceFileName, string destFileName)
        {
            if (!IsCopy)
            {
                return;
            }

            lock (((ICollection)Sources).SyncRoot)
            {
                Sources.Enqueue(new FileItem() { SourceFileName = sourceFileName, DestFileName = destFileName });
            }
        }

    }

    public class FileItem
    {
        public string SourceFileName
        {
            get;
            set;
        }
        public string DestFileName
        {
            get;
            set;
        }
    }
}
