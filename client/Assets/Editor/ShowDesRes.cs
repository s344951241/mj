using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ShowDesRes : EditorWindow {

    private static GameObject _prefabs;
    public static ShowDesRes editor;

    public static string path ="";
    private static string _imagePath;
    private static Dictionary<string, Sprite> sps;

    [MenuItem("Game Tools/替换资源")]
    public static void Init()
    {
        editor = ShowDesRes.GetWindow<ShowDesRes>(true, "替换资源", true);
        editor.position = new Rect(400, 150, 540, 500);
        editor.Show();
       
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Image路径");
        _imagePath = getPathEditor(_imagePath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输入prefab");
        _prefabs = (GameObject)EditorGUILayout.ObjectField(_prefabs, typeof(Object), false);

        GUILayout.EndHorizontal();

        

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("确定", GUILayout.Width(350), GUILayout.Height(20)))
        {
            if (_prefabs != null)
            {
                sps = new Dictionary<string, Sprite>();
                if (_imagePath.EndsWith("png"))
                {
                    object[] text = AssetDatabase.LoadAllAssetsAtPath(_imagePath);
                    Sprite[] sprite = new Sprite[text.Length];
                   
                    for (int i = 0; i < text.Length; i++)
                    {
                        sprite[i] = text[i] as Sprite;
                    }

                    for (int i = 1; i < sprite.Length; i++)
                    {
                        sps.Add(sprite[i].name, sprite[i]);
                    }
                }
                else
                {
                    string[] filePaths = GetFileNames(_imagePath, "*.png|*.jpg", true);
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        Sprite sp = AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(Sprite)) as Sprite;
                        sps.Add(sp.name, sp);
                    }
                   
                }
               // object[] text = AssetDatabase.LoadAllAssetsAtPath(_imagePath);
               
                getPath(_prefabs);
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.TextArea(path,GUILayout.Height(300));
        GUILayout.EndHorizontal();
    }
    private void getPath(GameObject obj)
    {
        if (obj != null)
        {
            if (obj.transform.GetComponent<Image>()&& obj.transform.GetComponent<Image>().sprite)
            {
                //path = path + obj.transform.GetComponent<Image>().sprite.name + "\n";
                if (sps.ContainsKey(obj.transform.GetComponent<Image>().sprite.name))
                {
                    obj.transform.GetComponent<Image>().sprite  = sps[obj.transform.GetComponent<Image>().sprite.name];
                }
                
            }
            if (obj.transform.childCount > 0)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    getPath(obj.transform.GetChild(i).gameObject);
                }
            }
        }
        
    }

    private string getPathEditor(string path)
    {
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        path = EditorGUI.TextField(rect, path);
        if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                path = DragAndDrop.paths[0];
            }
        }
        return path;
    }

    #region 文件操作
    public static bool IsExistDirectory(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }
    public static bool IsExistFile(string filePath)
    {
        return File.Exists(filePath);
    }
    public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
    {
        int i;
        List<string> mList = new List<string>();
        string[] exts = searchPattern.Split('|');
        if (!IsExistDirectory(directoryPath))
        {
            throw new FileNotFoundException();
        }
        try
        {
            if (isSearchChild)
            {
                for (i = 0; i < exts.Length; i++)
                {
                    mList.AddRange(Directory.GetFiles(directoryPath, exts[i], SearchOption.AllDirectories));
                }

            }
            else
            {
                for (i = 0; i < exts.Length; i++)
                {
                    mList.AddRange(Directory.GetFiles(directoryPath, exts[i], SearchOption.TopDirectoryOnly));
                }
            }
            return mList.ToArray();
        }
        catch (IOException ex)
        {
            throw ex;
        }
    }
    public static string GetFileNameNoExtension(string filePath)
    {
        FileInfo file = new FileInfo(filePath);
        string name = file.Name;
        if (name.LastIndexOf('.') > -1)
        {
            name = name.Substring(0, name.LastIndexOf("."));
            return file.Name.Split('.')[0];
        }
        return file.Name;
    }
    public static string GetDirectoryPath(string filePath)
    {
        if (Directory.Exists(filePath))
        {
            return filePath;
        }
        else
        {
            filePath = filePath.Replace("\\", "/");
            return filePath.Substring(0, filePath.LastIndexOf("/"));
        }
    }
    public static void DeleteFile(string filePath)
    {
        if (IsExistFile(filePath))
        {
            File.Delete(filePath);
        }
    }
    #endregion
}
