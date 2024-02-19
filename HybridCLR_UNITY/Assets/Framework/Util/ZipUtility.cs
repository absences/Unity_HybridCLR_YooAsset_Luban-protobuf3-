using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using UnityEngine;

public static class ZipUtility
{
    /// <summary>
    /// 压缩文件和文件夹
    /// </summary>
    /// <param name="fileOrDirectoryArray">文件夹路径和文件名</param>
    /// <param name="outputPathName">压缩后的输出路径文件名</param>
    /// <param name="password">压缩密码</param>
    /// <returns></returns>
    public static bool Zip(string[] fileOrDirectoryArray, string outputPathName, string password = null)
    {
        if ((null == fileOrDirectoryArray) || string.IsNullOrEmpty(outputPathName))
        {
            return false;
        }

        ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(outputPathName));
        zipOutputStream.SetLevel(6);    // 压缩质量和压缩速度的平衡点
        if (!string.IsNullOrEmpty(password))
            zipOutputStream.Password = password;

        for (int index = 0; index < fileOrDirectoryArray.Length; ++index)
        {
            bool result = false;
            string fileOrDirectory = fileOrDirectoryArray[index];
            if (Directory.Exists(fileOrDirectory))
                result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream);
            else if (File.Exists(fileOrDirectory))
                result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream);

            if (!result)
            {
                return false;
            }
        }

        zipOutputStream.Finish();
        zipOutputStream.Close();


        return true;
    }

    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="filePathName">Zip包的文件路径名</param>
    /// <param name="outputPath">解压输出路径</param>
    /// <param name="password">解压密码</param>
    /// <returns></returns>
    public static bool UnzipFile(string filePathName, string outputPath, string password = null)
    {
        if (string.IsNullOrEmpty(filePathName) || string.IsNullOrEmpty(outputPath))
        {
            return false;
        }
        try
        {
            return UnzipFile(File.OpenRead(filePathName), outputPath, password);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[UnzipFile]: " + _e.ToString());
            return false;
        }
    }

    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="fileBytes">Zip包字节数组</param>
    /// <param name="outputPath">解压输出路径</param>
    /// <param name="password">解压密码</param>
    /// <returns></returns>
    public static bool UnzipFile(byte[] fileBytes, string outputPath, string password = null)
    {
        if ((null == fileBytes) || string.IsNullOrEmpty(outputPath))
        {
            return false;
        }

        bool result = UnzipFile(new MemoryStream(fileBytes), outputPath, password);
        if (!result) { }
        return result;
    }

    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="inputStream">Zip包输入流</param>
    /// <param name="outputPath">解压输出路径</param>
    /// <param name="password">解压密码</param>
    /// <returns></returns>
    public static bool UnzipFile(Stream inputStream, string outputPath, string password = null)
    {
        if ((null == inputStream) || string.IsNullOrEmpty(outputPath))
        {

            return false;
        }

        // 创建文件目录
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // 解压Zip包
        ZipEntry entry = null;
        using (ZipInputStream zipInputStream = new ZipInputStream(inputStream))
        {
            if (!string.IsNullOrEmpty(password))
                zipInputStream.Password = password;

            while (null != (entry = zipInputStream.GetNextEntry()))
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;


                string filePathName = Path.Combine(outputPath, entry.Name);
                filePathName = filePathName.Replace("\\", "/");
                //Debug.Log(filePathName+","+ entry.IsDirectory);
                // 创建文件目录
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(filePathName);
                    continue;
                }

                // 写入文件
                try
                {
                    //if (File.Exists(filePathName))
                    //{
                    //    File.Delete(filePathName);
                    //}
                    using (FileStream fileStream = File.Create(filePathName))
                    {
                        byte[] bytes = new byte[1024];
                        while (true)
                        {
                            int count = zipInputStream.Read(bytes, 0, bytes.Length);
                            if (count > 0)
                                fileStream.Write(bytes, 0, count);
                            else
                            {

                                break;
                            }
                        }
                    }
                }
                catch (System.Exception _e)
                {
                    Debug.LogError("[UnzipFile]: " + _e.ToString());
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 压缩文件
    /// </summary>
    /// <param name="filePathName">文件路径名</param>
    /// <param name="parentRelPath">要压缩的文件的父相对文件夹</param>
    /// <param name="zipOutputStream">压缩输出流</param>
    /// <returns></returns>
    private static bool ZipFile(string filePathName, string parentRelPath, ZipOutputStream zipOutputStream)
    {
        //Crc32 crc32 = new Crc32();
        ZipEntry entry = null;
        FileStream fileStream = null;
        try
        {
            string entryName = parentRelPath + '/' + Path.GetFileName(filePathName);
            entry = new ZipEntry(entryName);
            entry.DateTime = System.DateTime.Now;

            fileStream = File.OpenRead(filePathName);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Close();

            entry.Size = buffer.Length;

            zipOutputStream.PutNextEntry(entry);
            zipOutputStream.Write(buffer, 0, buffer.Length);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipFile]: " + _e.ToString());
            return false;
        }
        finally
        {
            if (null != fileStream)
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }
        return true;
    }

    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="path">要压缩的文件夹</param>
    /// <param name="parentRelPath">要压缩的文件夹的父相对文件夹</param>
    /// <param name="zipOutputStream">压缩输出流</param>
    /// <returns></returns>
    private static bool ZipDirectory(string path, string parentRelPath, ZipOutputStream zipOutputStream)
    {
        ZipEntry entry = null;
        try
        {
            string entryName = Path.Combine(parentRelPath, Path.GetFileName(path) + '/');
            entry = new ZipEntry(entryName);
            entry.DateTime = System.DateTime.Now;
            entry.Size = 0;

            zipOutputStream.PutNextEntry(entry);
            zipOutputStream.Flush();

            string[] files = Directory.GetFiles(path);
            for (int index = 0; index < files.Length; ++index)
                ZipFile(files[index], Path.Combine(parentRelPath, Path.GetFileName(path)), zipOutputStream);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipDirectory]: " + _e.ToString());
            return false;
        }

        string[] directories = Directory.GetDirectories(path);
        for (int index = 0; index < directories.Length; ++index)
        {
            if (!ZipDirectory(directories[index], Path.Combine(parentRelPath, Path.GetFileName(path)), zipOutputStream))
                return false;
        }

        return true;
    }
}
