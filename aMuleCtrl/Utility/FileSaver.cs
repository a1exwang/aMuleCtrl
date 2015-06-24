using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace aMuleCtrl
{
    class FileSaver : IDisposable
    {
        public FileSaver(String path = "list.txt")
        {
            mFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void AddLink(String link)
        {
            byte[] data = Encoding.UTF8.GetBytes(link + "\n");
            mFile.Write(data, 0, data.Length);
            mFile.Flush();
        }

        public void AddLinks(String[] links)
        {
            foreach (String l in links)
            {
                AddLink(l);
            }
        }

        public void Close()
        {
            mFile.Close();
        }

        public void Dispose()
        {
            Close();
        }

        FileStream mFile;
    }
}
