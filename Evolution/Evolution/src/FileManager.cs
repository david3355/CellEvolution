using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Evolution
{
    public class FileManager
    {
        public FileManager()
        {

        }

        public String ReadFile(String FileName)
        {            
            try
            {
                using (IsolatedStorageFile filestream = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (StreamReader streamreader = new StreamReader(filestream.OpenFile(FileName, FileMode.OpenOrCreate)))
                    {
                        return streamreader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        public List<String> ReadAllLines(String FileName)
        {
            List<String> lines = new List<string>();
            try
            {
                using (IsolatedStorageFile filestream = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (StreamReader streamreader = new StreamReader(filestream.OpenFile(FileName, FileMode.OpenOrCreate)))
                    {
                        while (!streamreader.EndOfStream)
                        {
                            lines.Add(streamreader.ReadLine());
                        }
                    }
                }
                return lines;
            }
            catch (Exception e)
            {
                return lines;
            }
        }

        public void WriteFile(String FileName, String Data)
        {
            try
            {
                using (IsolatedStorageFile filestream = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (StreamWriter streamwriter = new StreamWriter(filestream.OpenFile(FileName, FileMode.OpenOrCreate)))
                    {
                        streamwriter.Write(Data);
                    }
                }
            }
            catch (Exception e)
            {
                String msg = e.Message;
            }
        }
    }
}
