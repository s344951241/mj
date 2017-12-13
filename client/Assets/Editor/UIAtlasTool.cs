using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class UIAtlasTool  {

    private static readonly bool MIPMAP_ENABLED = false;
    private static readonly SpriteImportMode SPRITE_IMPORTER_MODE = SpriteImportMode.Single;
    private static bool isCenterPivot = false;

    [MenuItem("Build/图集相关/生成静态图集")]
    public static void Test1()
    {
        BuildAtlas(true);
    }

    [MenuItem("Build/图集相关/生成动态图集")]

    public static void Test()
    {
        BuildAtlas(false);
    }

    public static void BuildAtlas(bool isCenter)
    {
        isCenterPivot = isCenter;
        if (Selection.activeObject == null)
            return;

        string localPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string[] arr = localPath.Split('/');
        string name = arr[arr.Length - 1];
        string parentPath = localPath.Substring(0, localPath.Length - name.Length - 1);
        string atlasPath = FileTools.getProjectPath(string.Format("{0}", localPath));
        string atlasOutputPath = FileTools.getProjectPath(string.Format("{0}/{1}_Atlas", parentPath, name));
        GenParticularAtlas(atlasPath, atlasOutputPath);
        SetAtlasAdvancedProps(string.Format("{0}.png", atlasOutputPath));
    }

    private static UIAtlasInfo GenParticularAtlas(string atlasDir, string atlasOutputDir, bool isSaveJsonData = false)
    {
        UIAtlasInfo atlasInfo = new UIAtlasInfo();
        Dictionary<string, string> pngPathDict = new Dictionary<string, string>();
        foreach (string filePath in FileTools.getAllFilesInFolder(atlasDir))
        {
            if (!filePath.EndsWith(".png", System.StringComparison.CurrentCultureIgnoreCase))
            {
                Debug.LogWarningFormat("[{0}]中含非png格式的文件：{1}", atlasDir, filePath);
                continue;
            }
            pngPathDict.Add(FileTools.getBaseNameWithoutExt(filePath), filePath);
            atlasInfo.AddPng(filePath);
        }
        if (pngPathDict.Keys.Count <= 0)
        {
            Debug.LogWarningFormat("目录中{0}中不含png,不生成图集", atlasDir);
            return atlasInfo;
        }

        string rgbaFilePath = string.Format("{0}.png", atlasOutputDir);
        string atlasDataFilePath = string.Format("{0}_data.json",atlasOutputDir);
        string tmpRGBAFilePath = string.Format("{0}_t.png", atlasOutputDir);
        string tmpAtlasDataFilePath = string.Format("{0}_data_t.json", atlasOutputDir);
        List<string> par = new List<string>() {
            "--opt RGBA8888",
            "--border-padding 0",
            "--shape-padding 0",
        };
        ExecuteCommandResult packRes = RunTexturePacker(atlasDir, tmpRGBAFilePath, tmpAtlasDataFilePath, par);
        if (packRes != null && !packRes.success)
        {
            Debug.LogErrorFormat("!!!散图合成图集失败，信息{}", packRes.msg);
            return atlasInfo;
        }
        string dataContent = FileTools.readFile(tmpAtlasDataFilePath);
        dataContent = ModifyContentLine(dataContent,dataContent.IndexOf("$TexturePacker:SmartUpdate:"),"$TexturePacker:SmartUpdate:111111111111111111111111111111$\"");
        FileTools.deleteFileOrFolder(tmpAtlasDataFilePath);
        FileTools.writeFile(tmpAtlasDataFilePath, dataContent);
        if (!File.Exists(rgbaFilePath) || !FileTools.getFileMd5(tmpRGBAFilePath).Equals(FileTools.getFileMd5(rgbaFilePath)) || !File.Exists(atlasDataFilePath) || !FileTools.getFileMd5(tmpAtlasDataFilePath).Equals(FileTools.getFileMd5(atlasDataFilePath)))
        {
            FileTools.deleteFileOrFolder(rgbaFilePath);
            FileTools.deleteFileOrFolder(atlasDataFilePath);
            File.Move(tmpRGBAFilePath, rgbaFilePath);
            File.Move(tmpAtlasDataFilePath, atlasDataFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.LogFormat("生成图集成功:{0}", rgbaFilePath);
        }
        else
        {
            FileTools.deleteFileOrFolder(tmpRGBAFilePath);
            FileTools.deleteFileOrFolder(tmpAtlasDataFilePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.LogFormat("图集不需要更新:{0}", rgbaFilePath);
        }

        atlasInfo.atlasFullPath = rgbaFilePath;
        atlasInfo.atlasDataFullPath = atlasDataFilePath;

        SetAtlasPngProps(rgbaFilePath, atlasDataFilePath, pngPathDict, ref atlasInfo);
        FileTools.deleteFileOrFolder(atlasDataFilePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return atlasInfo;
    }

    private static void SetAtlasAdvancedProps(string atlasPngPath)
    {
        AssetDatabase.StartAssetEditing();
        string assetPath = FileTools.getRelativePath(atlasPngPath);
        TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        TextureImporterSettings tis = new TextureImporterSettings();
        tis.spriteMeshType = SpriteMeshType.FullRect;
        tis.alphaIsTransparency = true;
        tis.spriteExtrude = 1;
        tis.spritePixelsPerUnit = 100f;
        ti.SetTextureSettings(tis);
        ti.wrapMode = TextureWrapMode.Clamp;
        ti.filterMode = FilterMode.Bilinear;
        ti.textureType = TextureImporterType.Advanced;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.mipmapEnabled = MIPMAP_ENABLED;
        ti.maxTextureSize = 2048;
        ti.textureFormat = TextureImporterFormat.AutomaticCompressed;
        ti.anisoLevel = 16;
        AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    private static ExecuteCommandResult RunTexturePacker(string pngFilesDir, string outputPngFile, string outputDataFile, List<string> arg)
    {
        List<string> defaultPar = new List<string>() {
            "--format unity",
            "--trim-mode Trim",
            "--disable-rotation",
            "--trim-sprite-names",
            "--reduce-border-artifacts",
            "--algorithm MaxRects",
        };

        foreach (string s in arg)
        {
            defaultPar.Add(s);
        }

#if UNITY_IOS
        string texturePackerExe = "/usr/local/bin/TexturePacker";
#else
        string texturePackerExe = FileTools.getProjectPath("CommonTool/TexturePacker/TexturePacker.exe");
#endif

        defaultPar.Add(string.Format("--sheet \"{0}\" --data \"{1}\"", outputPngFile, outputDataFile));
        defaultPar.Add(pngFilesDir);
        string argStr = string.Join(" ", defaultPar.ToArray());
        ExecuteCommandResult result = XGEditorUtils.excute(texturePackerExe, argStr);
        return result;
    }

    private static string ModifyContentLine(string content, int startIndex, string line)
    {
        int endIndex = content.IndexOf("\n", startIndex);
        return content.Substring(0, startIndex) + line + content.Substring(endIndex, content.Length - endIndex);
    }

    private static void SetAtlasPngProps(string atlasPngPath,string atlasDataFilePath,Dictionary<string,string> pngPathDict,ref UIAtlasInfo atlasInfo)
    {
        string assetPath = FileTools.getRelativePath(atlasPngPath);
        string assetMetaPath = assetPath + ".meta";
        string guidStr = FileTools.getGuidFromMetaFile(assetMetaPath);

        string content = FileTools.readFile(atlasDataFilePath);
        var jsonData = LitJson.JsonMapper.ToObject(content);
        int pngWidth = int.Parse(jsonData["meta"]["size"]["w"].ToString());
        int pngHeight = int.Parse(jsonData["meta"]["size"]["h"].ToString());

        List<string> upContentList = new List<string>();
        List<string> downContentList = new List<string>();
        int index = -1;
        foreach (string imageName in jsonData["frames"].Keys)
        {
            index++;
            string fileId = (21300000 + index * 2).ToString();
            SpriteForAtlasInfo sprInfo = atlasInfo.sprInfoDict[imageName];
            sprInfo.fileIDInAtlas = fileId;
            sprInfo.atlasGUID = guidStr;
            upContentList.Add(string.Format("    {0}: {1}", fileId, imageName));
            var imageInfo = jsonData["frames"][imageName]["frame"];
            int imageWidth = int.Parse(imageInfo["w"].ToString());
            int imageHeight = int.Parse(imageInfo["h"].ToString());
            int imageX = int.Parse(imageInfo["x"].ToString());
            int imageY = int.Parse(imageInfo["y"].ToString());

            var imageSrcInfo = jsonData["frames"][imageName]["spriteSourceSize"];
            var sourceSize = jsonData["frames"][imageName]["sourceSize"];
            int W_SIZE = int.Parse(sourceSize["w"].ToString());
            int H_SIZE = int.Parse(sourceSize["h"].ToString());

            int imageSrcX = int.Parse(imageSrcInfo["x"].ToString());
            int imageSrcY = int.Parse(imageSrcInfo["y"].ToString());

            float offsetX = (2 * imageSrcX + imageWidth - W_SIZE) / 2;
            float offsetY = (H_SIZE - (2 * imageSrcY + imageHeight)) / 2;

            float pivotX = 0.5f - offsetX / imageWidth;
            float pivotY = 0.5f - offsetY / imageHeight;
            string alignmentStr = "      alignment: 9";
            if (isCenterPivot == true)
            {
                alignmentStr = "      alignment: 0";
                pivotX = 0.5f;
                pivotY = 0.5f;
            }
            string ss = "      pivot: {x: " + pivotX.ToString() + ", y: " + pivotY.ToString() + "}";
            Debug.Log("imageName = " + imageName + " offsetx=" + offsetX.ToString() + "offsetY = " + offsetY.ToString());
            Debug.Log("imageName = " + imageName + " Width=" + imageWidth.ToString() + "Height = " + imageHeight.ToString() + "W_SIZE = " + W_SIZE.ToString() + "H_SIZE = " + H_SIZE.ToString());


            string borderInfoStr = GetBorderInfoStr(pngPathDict[imageName]);
            imageY = pngHeight - imageHeight - imageY;
            downContentList.Add(
                string.Join("\n", new string[] {
                    "    - name: "+imageName,
                    "      rect:",
                    "        serializedVersion: 2",
                    "        x: "+imageX,
                    "        y: "+imageY,
                    "        width: "+imageWidth,
                    "        height: "+imageHeight,
                    alignmentStr,
                    ss,
                    "      border: "+borderInfoStr,
                })
                );
        }

        string pngMetaTemplateFile = FileTools.getProjectPath("CommonTool/TexturePacker/PngMetaTemplate.txt");
        string metaTemplateContent = FileTools.readFile(pngMetaTemplateFile);
        FileTools.deleteFileOrFolder(assetMetaPath);
        var con2 = metaTemplateContent.Replace("#*0*#", guidStr).Replace("#*1*#", string.Join("\n", upContentList.ToArray())).Replace("#*2*#", string.Join("\n", downContentList.ToArray()));
#if UNITY_IOS
        con2 = con2.Replace("textureFormat: -1","textureFormat: 5");
#else
#endif

        FileTools.writeFile(assetMetaPath, con2);
    }

    private static string GetBorderInfoStr(string pngPath)
    {
        string metaContent = FileTools.readFile(pngPath + ".meta");
        int startIndex = metaContent.IndexOf("spriteBorder:") + "spriteBorder:".Length;
        string borderInfoStr = metaContent.Substring(startIndex, metaContent.IndexOf("\n", startIndex) - startIndex).Trim();
        return borderInfoStr;
    }
}

public class SpriteForAtlasInfo
{
    public string spriteName = "";//图片名称
    public string spriteGUID = "";//原散图的guid
    public string atlasGUID = "";//所属的图集的guid
    public string fileIDInAtlas = "";//在图集png中的fileID
}
public class UIAtlasInfo
{
    public Dictionary<string, SpriteForAtlasInfo> sprInfoDict = new Dictionary<string, SpriteForAtlasInfo>();
    public string atlasFullPath = "";
    public string atlasDataFullPath = "";
    public string tempPackPath = "";

    public void AddPng(string pngPath)
    {
        SpriteForAtlasInfo sprInfo = new SpriteForAtlasInfo();
        sprInfo.spriteName = FileTools.getBaseNameWithoutExt(pngPath);
        sprInfo.spriteGUID = FileTools.getGuidFromResFile(pngPath);
        sprInfoDict.Add(sprInfo.spriteName, sprInfo);
    }

    public bool IsEmpty()
    {
        return (string.IsNullOrEmpty(atlasFullPath) || string.IsNullOrEmpty(atlasDataFullPath));
    }
}