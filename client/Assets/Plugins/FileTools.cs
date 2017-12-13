using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileTools
{
	public static string NewLine = "";
	public static void AppendText (string filePath, string content)
	{
		File.AppendAllText (filePath, content);
	}

	public static Texture2D BytesToTexture (byte[] bytes, int width, int height)
	{
		Texture2D texture2D = new Texture2D (width, height);
		texture2D.LoadImage (bytes);
		return texture2D;
	}

	public static void ClearDirectory (string directoryPath,bool flag = false)
	{
		if (FileTools.IsExistDirectory (directoryPath)) {
			string[] fileNames = FileTools.GetFileNames (directoryPath);
			for (int i = 0; i < fileNames.Length; i++) {
				FileTools.DeleteFile (fileNames [i]);
			}
            if (flag)
            {
                string[] directories = FileTools.GetDirectories(directoryPath);
                for (int j = 0; j < directories.Length; j++)
                {
                    FileTools.DeleteDirectory(directories[j], null, true);
                }
		
			}
		}
	}
	public static void ClearFile (string filePath)
	{
		File.Delete (filePath);
		FileTools.CreateFile (filePath);
	}

	public static bool Contains (string directoryPath, string searchPattern)
	{
		bool result;
		try {
			string[] fileNames = FileTools.GetFileNames (directoryPath, searchPattern, false);
			if (fileNames.Length == 0) {
				result = false;
			}
			else {
				result = true;
			}
		}
		catch (Exception arg) {
			Debug.LogError ("FileUtil:Contains();Common" + arg);
			result = false;
		}
		return result;
	}

	public static bool Contains (string directoryPath, string searchPattern, bool isSearchChild)
	{
		bool result;
		try {
			string[] fileNames = FileTools.GetFileNames (directoryPath, searchPattern, true);
			if (fileNames.Length == 0) {
				result = false;
			}
			else {
				result = true;
			}
		}
		catch (Exception arg) {
			Debug.LogError ("FileUtil:Contains();Common" + arg);
			result = false;
		}
		return result;
	}

	public static void Copy (string sourceFilePath, string destFilePath, bool overwrite = true)
	{
		File.Copy (sourceFilePath, destFilePath, true);
	}

	public static void CopyDirectory (string from, string to)
	{
		try {
			if (to [to.Length - 1] != Path.DirectorySeparatorChar) {
				to += Path.DirectorySeparatorChar;
			}
			if (!Directory.Exists (to)) {
				Directory.CreateDirectory (to);
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries (from);
			for (int i = 0; i < fileSystemEntries.Length; i++) {
				string text = fileSystemEntries [i];
				if (Directory.Exists (text)) {
					FileTools.CopyDirectory (text, to + Path.GetFileName (text));
				}
				else {
					File.Copy (text, to + Path.GetFileName (text), true);
				}
			}
		}
		catch (Exception ex) {
			Debug.LogError ("拷贝文件夹出错" + ex.Message);
		}
	}

	public static void CreateDirectory (string directoryPath)
	{
		if (!FileTools.IsExistDirectory (directoryPath)) {
			Directory.CreateDirectory (directoryPath);
		}
	}

	public static void CreateFile (string filePath, byte[] buffer)
	{
		try {
			if (!FileTools.IsExistFile (filePath)) {
				FileInfo fileInfo = new FileInfo (filePath);
				FileStream fileStream = fileInfo.Create ();
				fileStream.Write (buffer, 0, buffer.Length);
				fileStream.Close ();
			}
		}
		catch (Exception ex) {
			throw ex;
		}
	}

	public static bool CreateFile (string filePath, string s, string encode)
	{
		bool result = true;
		Encoding encoding = Encoding.GetEncoding (encode);
		StreamWriter streamWriter = null;
		try {
			streamWriter = new StreamWriter (filePath, false, encoding);
			streamWriter.Write (s);
			streamWriter.Flush ();
		}
		catch (Exception ex) {
			result = false;
			throw ex;
		}
		finally {
			streamWriter.Close ();
		}
		return result;
	}

	public static void CreateFile (string filePath)
	{
		try {
			if (!FileTools.IsExistFile (filePath)) {
				FileInfo fileInfo = new FileInfo (filePath);
				FileStream fileStream = fileInfo.Create ();
				fileStream.Close ();
			}
		}
		catch (Exception ex) {
			throw ex;
		}
	}

	public static void DeleteDirectory (string directoryPath, Action deleteCallBack = null, bool delDir = true)
	{
		if (FileTools.IsExistDirectory (directoryPath)) {
			string[] files = Directory.GetFiles (directoryPath);
			string[] directories = Directory.GetDirectories (directoryPath);
			for (int i = 0; i < files.Length; i++) {
				string path = files [i];
				File.SetAttributes (path, FileAttributes.Normal);
				File.Delete (path);
				if (deleteCallBack != null) {
					deleteCallBack ();
				}
			}
			for (int i = 0; i < directories.Length; i++) {
				string text = directories [i];
				if (!text.Contains ("svn")) {
					FileTools.DeleteDirectory (text, deleteCallBack, true);
				}
			}
			if (delDir) {
				Directory.Delete (directoryPath, false);
			}
		}
	}

	public static void DeleteFile (string filePath)
	{
		if (FileTools.IsExistFile (filePath)) {
			File.Delete (filePath);
		}
	}

	public static byte[] FileToBytes (string filePath)
	{
		int fileSize = FileTools.GetFileSize (filePath);
		byte[] array = new byte[fileSize];
		FileInfo fileInfo = new FileInfo (filePath);
		FileStream fileStream = fileInfo.Open (FileMode.Open);
		byte[] result;
		try {
			fileStream.Read (array, 0, fileSize);
			result = array;
		}
		catch (IOException ex) {
			throw ex;
		}
		finally {
			fileStream.Close ();
		}
		return result;
	}

	public static string FileToString (string filePath, Encoding encoding)
	{
		StreamReader streamReader = new StreamReader (filePath, encoding);
		string result;
		try {
			result = streamReader.ReadToEnd ();
		}
		catch (Exception ex) {
			throw ex;
		}
		finally {
			streamReader.Close ();
		}
		return result;
	}

	public static string FileToString (string filePath)
	{
		return FileTools.FileToString (filePath, Encoding.Default);
	}

	public static string[] GetDirectories (string directoryPath)
	{
		string[] directories;
		try {
			directories = Directory.GetDirectories (directoryPath);
		}
		catch (IOException ex) {
			throw ex;
		}
		return directories;
	}

	public static string[] GetDirectories (string directoryPath, string searchPattern, bool isSearchChild)
	{
		string[] directories;
		try {
			if (isSearchChild) {
				directories = Directory.GetDirectories (directoryPath, searchPattern, SearchOption.AllDirectories);
			}
			else {
				directories = Directory.GetDirectories (directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
			}
		}
		catch (IOException ex) {
			throw ex;
		}
		return directories;
	}

	public static string GetDiretoryName (string filePath)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo (filePath);
		return directoryInfo.Name;
	}

	public static string GetDiretoryPath (string filePath)
	{
		if (Directory.Exists (filePath)) {
			return filePath;
		}
		filePath = filePath.Replace ("\\", "/");
		return filePath.Substring (0, filePath.LastIndexOf ("/"));
	}

	public static string GetExtension (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		return fileInfo.Extension;
	}

	public static string GetFileContent (string path)
	{
		FileStream stream = File.Open (path, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader (stream, Encoding.UTF8);
		return streamReader.ReadToEnd ();
	}

	public static string GetFileExtension (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		string text = fileInfo.Name;
		if (text.LastIndexOf ('.') > -1) {
			text = text.Substring (text.LastIndexOf ('.') + 1);
			return text.ToLower ();
		}
		return string.Empty;
	}

	public static string[] GetFileLinesContent (string filePath)
	{
		return File.ReadAllLines (filePath);
	}

	public static string GetFileName (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		return fileInfo.Name;
	}

	public static string GetFileNameNoExtension (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		string text = fileInfo.Name;
		if (text.LastIndexOf ('.') > -1) {
			text = text.Substring (0, text.LastIndexOf ('.'));
			return fileInfo.Name.Split (new char[] {
				'.'
			}) [0];
		}
		return fileInfo.Name;
	}

	public static string[] GetFileNames (string directoryPath, string searchPattern, bool isSearchChild)
	{
		List<string> list = new List<string> ();
		string[] array = searchPattern.Split (new char[] {
			'|'
		});
		if (!FileTools.IsExistDirectory (directoryPath)) {
			throw new FileNotFoundException ();
		}
		string[] result;
		try {
			if (isSearchChild) {
				for (int i = 0; i < array.Length; i++) {
					list.AddRange (Directory.GetFiles (directoryPath, array [i], SearchOption.AllDirectories));
				}
			}
			else {
				for (int i = 0; i < array.Length; i++) {
					list.AddRange (Directory.GetFiles (directoryPath, array [i], SearchOption.TopDirectoryOnly));
				}
			}
			result = list.ToArray ();
		}
		catch (IOException ex) {
			throw ex;
		}
		return result;
	}

	public static string[] GetFileNames (string directoryPath)
	{
		if (!FileTools.IsExistDirectory (directoryPath)) {
			throw new FileNotFoundException ();
		}
		return Directory.GetFiles (directoryPath);
	}

	public static int GetFilesCount (DirectoryInfo dirInfo)
	{
		int num = 0;
		num += dirInfo.GetFiles ().Length;
		DirectoryInfo[] directories = dirInfo.GetDirectories ();
		for (int i = 0; i < directories.Length; i++) {
			DirectoryInfo dirInfo2 = directories [i];
			num += FileTools.GetFilesCount (dirInfo2);
		}
		return num;
	}

	public static int GetFileSize (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		return (int)fileInfo.Length;
	}

	public static double GetFileSizeByKB (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		return Convert.ToDouble (Convert.ToDouble (fileInfo.Length) / 1024);
	}

	public static double GetFileSizeByMB (string filePath)
	{
		FileInfo fileInfo = new FileInfo (filePath);
		return Convert.ToDouble (Convert.ToDouble (fileInfo.Length) / 1024 / 1024);
	}

	public static int GetLineCount (string filePath)
	{
		string[] array = File.ReadAllLines (filePath);
		return array.Length;
	}

	public static bool IsEmptyDirectory (string directoryPath)
	{
		bool result;
		try {
			if (!FileTools.IsExistDirectory (directoryPath)) {
				result = true;
			}
			else {
				string[] fileNames = FileTools.GetFileNames (directoryPath);
				if (fileNames.Length > 0) {
					result = false;
				}
				else {
					string[] directories = FileTools.GetDirectories (directoryPath);
					if (directories.Length > 0) {
						result = false;
					}
					else {
						result = true;
					}
				}
			}
		}
		catch (Exception var_2_4C) {
			result = true;
		}
		return result;
	}

	public static bool IsExistDirectory (string directoryPath)
	{
		return Directory.Exists (directoryPath);
	}

	public static bool IsExistFile (string filePath)
	{
		return File.Exists (filePath);
	}

	public static void Move (string sourceFilePath, string descDirectoryPath)
	{
		string fileName = FileTools.GetFileName (sourceFilePath);
		if (FileTools.IsExistDirectory (descDirectoryPath)) {
			if (FileTools.IsExistFile (descDirectoryPath + "\\" + fileName)) {
				FileTools.DeleteFile (descDirectoryPath + "\\" + fileName);
			}
			File.Move (sourceFilePath, descDirectoryPath + "\\" + fileName);
		}
	}

	public static void MoveDirectory (string from, string to)
	{
		try {
			if (to [to.Length - 1] != Path.DirectorySeparatorChar) {
				to += Path.DirectorySeparatorChar;
			}
			if (!Directory.Exists (to)) {
				Directory.CreateDirectory (to);
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries (from);
			for (int i = 0; i < fileSystemEntries.Length; i++) {
				string text = fileSystemEntries [i];
				if (Directory.Exists (text)) {
					FileTools.MoveDirectory (text, to + Path.GetFileName (text));
				}
				else {
					string text2 = to + Path.GetFileName (text);
					if (!string.IsNullOrEmpty (FileTools.GetFileNameNoExtension (Path.GetFileName (text)))) {
						if (File.Exists (text2)) {
							File.Delete (text2);
						}
						File.Move (text, text2);
					}
				}
			}
		}
		catch (Exception ex) {
			Debug.LogError ("拷贝文件夹出错" + ex.Message);
		}
	}

	public static string ReadText (string filePath)
	{
		return File.ReadAllText (filePath);
	}

	public static bool Save (string path, byte[] bytes)
	{
		if (bytes == null) {
			if (File.Exists (path)) {
				File.Delete (path);
			}
			return true;
		}
		FileStream fileStream = null;
		try {
			string diretoryPath = FileTools.GetDiretoryPath (path);
			if (!FileTools.IsExistDirectory (diretoryPath)) {
				FileTools.CreateDirectory (diretoryPath);
			}
			fileStream = File.Create (path);
		}
		catch (Exception ex) {
			Debug.LogError (ex.Message);
			return false;
		}
		fileStream.Write (bytes, 0, bytes.Length);
		fileStream.Close ();
		return true;
	}

	public static byte[] StreamToBytes (Stream stream)
	{
		byte[] result;
		try {
			byte[] array = new byte[stream.Length];
			stream.Read (array, 0, Convert.ToInt32 (stream.Length));
			result = array;
		}
		catch (Exception ex) {
			throw ex;
		}
		finally {
			stream.Close ();
		}
		return result;
	}

	public static void TextureToPngFile (Texture2D texture, string path)
	{
		byte[] bytes = texture.EncodeToPNG ();
		File.WriteAllBytes (path, bytes);
	}

	public static byte[] ToBytes (string filePath)
	{
		return File.ReadAllBytes (filePath);
	}

	public static void ToFormat (string directoryPath, string ext, Encoding encoding)
	{
		string[] fileNames = FileTools.GetFileNames (directoryPath, ext, true);
		for (int i = 0; i < fileNames.Length; i++) {
			string path = fileNames [i];
			File.WriteAllText (path, File.ReadAllText (path, Encoding.Default), encoding);
		}
	}

	public static void WriteBytesToFile (byte[] bytes, string path)
	{
		string diretoryPath = FileTools.GetDiretoryPath (path);
		if (!Directory.Exists (diretoryPath)) {
			Directory.CreateDirectory (diretoryPath);
		}
		FileStream fileStream = new FileStream (path, FileMode.Create);
		fileStream.Write (bytes, 0, bytes.Length);
		fileStream.Close ();
	}

	public static void WriteText (string filePath, string content, bool withBom = true)
	{
		StreamWriter streamWriter = null;
		try {
			UTF8Encoding encoding = new UTF8Encoding (withBom);
			StreamWriter streamWriter2;
			streamWriter = (streamWriter2 = new StreamWriter (filePath, false, encoding));
			try {
				streamWriter.Write (content);
			}
			finally {
				if (streamWriter2 != null) {
					((IDisposable)streamWriter2).Dispose ();
				}
			}
		}
		catch {
		}
		finally {
			streamWriter.Close ();
		}
	}
    //获取父目录
    public static string getParentFolder(string folder)
    {
        return folder.Substring(0, folder.LastIndexOf("/"));
    }
    //获取项目路径
    private static string projectPath = getParentFolder(Application.dataPath);
    public static string getProjectPath(string path = "")
    {
        string result = projectPath;
        if (path.Equals("") == false)
        {
            result += "/" + path;
        }
        return result;
    }

    public static string getBaseNameWithoutExt(string file)
    {
        string baseName = getBaseName(file);
        return baseName.Substring(0, baseName.LastIndexOf("."));
    }

    public static string getBaseName(string file)
    {
        return file.Substring(file.LastIndexOf("/") + 1);
    }

    public static string getGuidFromResFile(string resFile)
    {
        string assetPath = getRelativePath(resFile);
        string assetMetaPath = assetPath + ".meta";
        string guidStr = getGuidFromMetaFile(assetMetaPath);
        return guidStr;
    }
    public static string getRelativePath(string path)
    {
        return path.Replace(Application.dataPath, "Assets");
    }

    public static string getGuidFromMetaFile(string metaFile)
    {
        string content = readFile(metaFile);
        foreach (string line in content.Split('\n'))
        {
            if (line.StartsWith("guid:"))
            {
                return line.Substring(5).Trim();
            }
        }
        throw new System.Exception("未能获取meta文件中的guid属性");

    }

    public static string readFile(string file)
    {
        FileStream fs = new FileStream(file, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string content = sr.ReadToEnd();
        sr.Close();
        fs.Close();
        return content;
    }

    public static List<string> ignoreFileEnds = new List<string>() { ".meta", ".svn" };
    public static List<string> getAllFilesInFolder(string folder, bool isFormatSeparator = true)
    {
        List<string> result = new List<string>(Directory.GetFiles(folder));
        List<string> subFolders = new List<string>(Directory.GetDirectories(folder));

        for (int i = 0; i < result.Count; i++)
        {
            foreach (string ignoreEnd in ignoreFileEnds)
            {
                if (result[i].EndsWith(ignoreEnd))
                {
                    result.RemoveAt(i);
                    i = i - 1;
                    break;
                }
            }
        }
        foreach (string subFolder in subFolders)
        {
            bool isIgnore = false;
            foreach (string ignoreEnd in ignoreFileEnds)
            {
                if (subFolder.EndsWith(ignoreEnd))
                {
                    isIgnore = true;
                    break;
                }
            }
            if (isIgnore) { continue; }
            result.AddRange(getAllFilesInFolder(subFolder, false));
        }

        if (isFormatSeparator)
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = result[i].Replace(@"\", "/");
            }
        }
        return result;
    }

    public static string deleteFileOrFolder(string path)
    {
        if (File.Exists(path))
        {
            new FileInfo(path).Delete();
        }
        else if (Directory.Exists(path))
        {
            new DirectoryInfo(path).Delete(true);
        }
        return path;
    }

    public static string getFileMd5(string file)
    {
        FileStream f = new FileStream(file, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(f);
        f.Close();
        md5.Clear();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            sb.Append(retVal[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public static bool writeFile(string file,string content)
    {

        createFolderForFile(file);
        FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(content);
        sw.Flush();
        sw.Close();
        fs.Close();
        return true;
    }

    public static string createFolderForFile(string file)
    {
        DirectoryInfo folder = Directory.GetParent(file);
        if (folder.Exists == false)
        {
            folder.Create();
        }
        return file;
    }
}
