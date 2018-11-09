using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System;
using System.Text;
/// <summary>
/// Upload: 上传一个文件
/// Download: 下载一个文件
/// Delete: 删除一个文件
/// RemoveDirectory: 删除一个文件夹，未成功
/// GetFilesDetailList：获得一个文件夹下的所有文件，文件夹
/// </summary>
public class FtpServerHelper {

    private string ftpServerIP;
    private string ftpRemotePath;
    private string ftpUserId;
    private string ftpPassword;
    private string ftpURI;

    public FtpServerHelper(string ip, string path, string userId, string passward) {
        this.ftpServerIP = ip;
        this.ftpRemotePath = path;
        this.ftpUserId = userId;
        this.ftpPassword = passward;
        ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
    }

    /// <summary>
    /// 将文件fileName上传到
    /// "'ftp://' + ftpServerIP + '/' + ftpRemotePath + '/' + filename"
    /// </summary>
    /// <param name="filename"></param>
    public void Upload(string filename)
    {
        FileInfo fileInf = new FileInfo(filename);
        string uri = ftpURI + fileInf.Name;
        FtpWebRequest reqFTP;

        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
        //设置上传参数
        reqFTP.Credentials = new NetworkCredential(this.ftpUserId, this.ftpPassword);
        reqFTP.KeepAlive = false;
        reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
        reqFTP.UseBinary = true;
        reqFTP.ContentLength = fileInf.Length;

        int buffLength = 2048;
        byte[] buff = new byte[buffLength];
        int contentLen;
        FileStream fs = fileInf.OpenRead();
        try
        {
            Stream strm = reqFTP.GetRequestStream();
            contentLen = fs.Read(buff, 0, buffLength);
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }
            strm.Close();
            fs.Close();
            Debug.LogError("file upload succeed!!!");
        }
        catch (Exception ex)
        {
            Debug.LogError("上传文件报错：" + ex.Message);
        }
        
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="filePath">下载文件的父路径</param>
    /// <param name="fileName">下载文件的名字</param>
    public void Download(string filePath, string fileName)
    {
        FtpWebRequest reqFTP;
        try
        {
            //输出文件流
            FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
            //创建ftp链接
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
            Debug.LogError(ftpURI + fileName);
            //此次连接参数
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(this.ftpUserId, this.ftpPassword);
            //输入流，从ftp到本地
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            //将Uri指定的文件输出到outputStream
            long cl = response.ContentLength;
            int bufferSize = 2048;
            int readCount;
            byte[] buffer = new byte[bufferSize];

            readCount = ftpStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                outputStream.Write(buffer, 0, readCount);
                readCount = ftpStream.Read(buffer, 0, bufferSize);
            }

            ftpStream.Close();
            outputStream.Close();
            response.Close();
            Debug.LogError("file download succeed!!!");
        }
        catch (Exception ex)
        {
            Debug.LogError("下载文件报错：" + ex.Message);
        }
        
    }
    /// <summary>
    /// s删除文件，基于URI路径下
    /// </summary>
    /// <param name="fileName"></param>
    public void Delete(string fileName)
    {
        try
        {
            //根据Uri创建一个链接
            string uri = ftpURI + fileName;
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            //设置远程链接参数
            reqFTP.Credentials = new NetworkCredential(this.ftpUserId, this.ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

            string result = String.Empty;
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

            long size = response.ContentLength;
            Stream datastream = response.GetResponseStream();
            StreamReader sr = new StreamReader(datastream);
            result = sr.ReadToEnd();
            sr.Close();
            datastream.Close();
            response.Close();
            Debug.LogError("文件删除成功");
        }
        catch (Exception ex)
        {
            Debug.LogError("删除文件出错：" + ex.Message);
        }
    }

    /// <summary>
    /// 删除文件夹, 当前删除文件夹操作并不能支持，换个ftp试下。
    /// </summary>
    /// <param name="folderName"></param>
    public void RemoveDirectory(string folderName)
    {
        try
        {
            //创建请求链接
            string uri = ftpURI + folderName;
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            
            //设置请求参数
            reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
            reqFTP.Credentials = new NetworkCredential(this.ftpUserId, this.ftpPassword);
            reqFTP.KeepAlive = false;
            

            string result = String.Empty;
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            long size = response.ContentLength;
            Stream datastream = response.GetResponseStream();
            StreamReader sr = new StreamReader(datastream);
            result = sr.ReadToEnd();
            sr.Close();
            datastream.Close();
            response.Close();
            Debug.LogError("directory remove succeess");
        }
        catch (Exception ex)
        {
            Debug.LogError("删除文件夹失败：" + ex.Message);
        }
    }

    /// <summary>
    /// 获取当前目录下明细(包含文件和文件夹)
    /// </summary>
    /// <returns></returns>
    public string[] GetFilesDetailList()
    {
        string[] downloadFiles;
        try
        {
            StringBuilder result = new StringBuilder();
            //创建远程链接
            FtpWebRequest ftp;
            ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));

            ftp.Credentials = new NetworkCredential(this.ftpUserId, this.ftpPassword);
            ftp.Method = WebRequestMethods.Ftp.ListDirectory;
            WebResponse response = ftp.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

            string line = reader.ReadLine();

            while (line != null)
            {
                result.Append(line);
                result.Append("\n");
                line = reader.ReadLine();
            }
            result.Remove(result.ToString().LastIndexOf("\n"), 1);
            reader.Close();
            response.Close();
            return result.ToString().Split('\n');
        }
        catch (Exception ex)
        {
            downloadFiles = null;
            Debug.LogError("获取文件列表失败!");
            return downloadFiles;
        }
    }
}
