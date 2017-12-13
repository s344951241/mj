using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ExecuteCommandResult
{
    public bool success;
    public List<string> msg;
    public ExecuteCommandResult(bool success, List<string> msg)
    {
        this.success = success;
        this.msg = msg;
    }
}
public class XGEditorUtils  {
    public static ExecuteCommandResult excute(string fileName, string arguments)
    {
        List<string> msg = new List<string>();
        var p = new System.Diagnostics.Process();
        p.StartInfo.FileName = fileName;
        p.StartInfo.Arguments = arguments;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;

        p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("gbk");
        p.StartInfo.StandardErrorEncoding = Encoding.GetEncoding("gbk");
        p.Start();

        string resultStr = p.StandardOutput.ReadToEnd();
        string errorStr = p.StandardError.ReadToEnd();
        p.WaitForExit();
        p.Close();
        foreach (string str in resultStr.Split(new char[] { '\r' }))
        {
            msg.Add(str.Trim());
        }

        bool success = true;
        if (errorStr != "")
        {
            success = false;
            foreach (string str in errorStr.Split(new char[] { '\r' }))
            {
                msg.Add(str.Trim());
            }
            UnityEngine.Debug.LogErrorFormat("result:{0},errorStr:{1}", resultStr, errorStr);
        }
        return new ExecuteCommandResult(success, msg);
    }
}
