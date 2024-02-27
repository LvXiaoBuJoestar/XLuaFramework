using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileUtil
{
    public static bool IsExisted(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }

    public static void WriteFile(string path, byte[] data)
    {
        path = PathUtil.GetStandardPath(path);
        string dir = path.Substring(0, path.LastIndexOf('/'));

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        FileInfo file = new FileInfo(path);
        if (file.Exists)
            file.Delete();

        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
}
