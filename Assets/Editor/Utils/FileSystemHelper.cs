using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Editor.Utils
{
    class FileSystemHelper
    {

        /// <summary>
        /// FileStream 写文件， StreamReader,StreamWriter, 字符串读写
        /// </summary>
        /// <param name="name"></param>
        public static void FileStreamWrite(string name)
        {
            //写文件流
            string strContent = "\r\nHello file stream";
            byte[] buff = new byte[1024];
            buff = Encoding.UTF8.GetBytes(strContent);

            FileStream fst = new FileStream(name, FileMode.Append, FileAccess.Write);

            fst.Write(buff, 0, buff.Length);
            fst.Close();
        }
    }

    
}
