using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using AssetStudio;
using Object = System.Object;

namespace PkgDoctorMain
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var assetsManager = new AssetsManager();
            args = new[] {"F:\\Downloads\\Misc\\trunk_1.1.3585(13585)(07091207)_R_B_zh-cn.apk"};
            var paths = args;
            if (paths.Length == 1)
            {
                var path_0 = Path.GetFullPath(paths[0]);
                if ((Path.GetExtension(path_0) == ".apk" || Path.GetExtension(path_0) == ".ipa") && File.Exists(path_0))
                {
                    var path_apk = path_0;
                    if (Path.GetExtension(path_0) == ".apk")
                        path_apk = path_0.Replace(".apk", "");
                    else
                        path_apk = path_0.Replace(".ipa", "");

                    if (Directory.Exists(path_apk))
                        Directory.Delete(path_apk, true);

                    Console.WriteLine("Unzip {0}", path_0);

                    try
                    {
                        //这个zip库有问题，高版本的apk解压不了
                        System.IO.Compression.ZipFile.ExtractToDirectory(path_0, path_apk);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    path_0 = path_apk;
                }

                if (File.Exists(path_0) || Directory.Exists(path_0))
                {
                    if (Directory.Exists(path_0))
                    {
                        path_0 = Path.Combine(Path.GetDirectoryName(path_0), Path.GetFileName(path_0));
                        Console.WriteLine("LoadFolder {0}", path_0);
                        assetsManager.LoadFolder(path_0);
                    }
                    else
                    {
                        Console.WriteLine("LoadFile {0}", path_0);
                        var method = typeof(AssetsManager).GetMethod("LoadFile", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (null != method)
                        {
                            method.Invoke(assetsManager, new object[] {path_0});
                        }
                    }

                    List<object> toExportAssets = new List<object>();
                    foreach (var assetsFile in assetsManager.assetsFileList)
                    {
                        foreach (var asset in assetsFile.Objects)
                        {
                            toExportAssets.Add(asset);
                        }
                    }
                    Console.WriteLine($"资产数量: {toExportAssets.Count}");
                    var path_pkg = path_0 + "-pkg";
                    Console.WriteLine("ExportAssets {0}", path_pkg);
                    if (!Directory.Exists(path_pkg))
                        Directory.CreateDirectory(path_pkg);
                    ExportAssets2(path_pkg, toExportAssets);
                    
                    var script_exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.exe");
                    if (File.Exists(script_exe))
                    {
                        Console.WriteLine("script.exe {0}", path_pkg + "/pkg.tsv");
                        run_exe(script_exe, Path.Combine(path_pkg, "pkg.tsv"), "");
                    }
                    else
                    {
                        Console.WriteLine("pkg.py {0}", path_pkg + "/pkg.tsv");
                        run_exe("python.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pkg.py"), Path.Combine(path_pkg, "pkg.tsv"));
                    }
                    
                    Console.WriteLine("Analysis completed !!!");

//                     Console.WriteLine("BuildAssetData");
// // 加载程序集
//                     Assembly assetStudioAssembly = Assembly.Load("AssetStudio"); // 假设程序集文件名是AssetStudio.dll
//                     // 反射获取Studio类的类型信息
//                     
//                     Type studioType = assetStudioAssembly.GetType("AssetStudioGUI.Studio");
//                     var buildAssetDataMethod = studioType.GetMethod("BuildAssetData", BindingFlags.Public | BindingFlags.Static);
//                     buildAssetDataMethod.Invoke(null, null);
//                     // BuildAssetData();
//
//                     var path_pkg = path_0 + "-pkg";
//                     Console.WriteLine("ExportAssets {0}", path_pkg);
//                     if (!Directory.Exists(path_pkg))
//                         Directory.CreateDirectory(path_pkg);
//
//                     // 获取exportableAssets字段的信息
//                     FieldInfo exportableAssetsField = studioType.GetField("exportableAssets", BindingFlags.Public | BindingFlags.Static);
//                     List<Object> toExportAssets = (List<Object>) exportableAssetsField.GetValue(null);
//                     // List<Object> toExportAssets = Studio.exportableAssets;
//                     // 示例：使用字段的值，这里我们只是打印出其包含的资产数量
//                     Console.WriteLine($"资产数量: {toExportAssets.Count}");
//
//                     toExportAssets.Sort(CompareAssetByFileSize);
//                     ExportAssets2(path_pkg, toExportAssets);
//
//                     var script_exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.exe");
//                     if (File.Exists(script_exe))
//                     {
//                         Console.WriteLine("script.exe {0}", path_pkg + "/pkg.tsv");
//                         run_exe(script_exe, Path.Combine(path_pkg, "pkg.tsv"), "");
//                     }
//                     else
//                     {
//                         Console.WriteLine("pkg.py {0}", path_pkg + "/pkg.tsv");
//                         run_exe("python.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pkg.py"), Path.Combine(path_pkg, "pkg.tsv"));
//                     }
//
//                     // quit this app
//                     // Load += (s, e) => Close();
                }
            }
        }

        private static int CompareAssetByFileSize(Object x, Object y)
        {
            var xText = (string) GetFieldValue(x, "Text");
            var yText = (string) GetFieldValue(y, "Text");
            return String.CompareOrdinal(xText, yText);
        }

        public static void run_exe(string exeName, string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = exeName;
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        public static object GetFieldValue(object obj, string fieldName)
        {
            Type type = obj.GetType(); // 获取对象类型

            // 尝试获取指定名称的字段，包括非公共的
            FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                // 如果找到了字段，就返回它的值
                return field.GetValue(obj);
            }
            else
            {
                // 如果没有找到字段，你可以选择返回null或者抛出异常
                // return null;

                // 或者
                throw new ArgumentException($"Field '{fieldName}' not found in type '{type.FullName}'.");
            }
        }

        public static void ExportAssets2(string savePath, List<Object> toExportAssets)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            string csvFileName = Path.Combine(savePath, "pkg.tsv");
            StreamWriter csvFile = new StreamWriter(csvFileName);

            csvFile.Write("Name\tContainer\tType\tDimension\tFormat\tSize\tFileName\tHash\tOriginalFile\tWrapMode\n");
            int toExportCount = toExportAssets.Count;
            int exportedCount = 0;
            int i = 0;
            foreach (var asset in toExportAssets)
            {
                try
                {
                    if (ExportVizFile(asset, savePath, csvFile))
                    {
                        exportedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Progress.Report(++i, toExportCount);
            }

            Process.Start(savePath);
            csvFile.Close();
        }

        static string[] wrapModes = {"Repeat", "Clamp"};

        public static bool ExportVizFile(Object item, string savePath, StreamWriter csvFile)
        {
            bool result = true;
            string filename = "";
            string hash = "";
            string dimension = "";
            string format = "";
            string wrapMode = "";
            byte[] rawData = null;
            var sourcePath = savePath.Replace("-pkg", "\\");

            var classIDType = (ClassIDType) GetFieldValue(item, "type");
            var typeString = classIDType.ToString();
            var exportPath = Path.Combine(savePath, typeString);
            var asset = (AssetStudio.Object)item;
            switch (classIDType)
            {
                case ClassIDType.Texture2D:
                {
                    var texture2D = (Texture2D) asset;
                    if (texture2D.m_MipMap)
                        dimension = string.Format("{0}x{1} mips", texture2D.m_Width, texture2D.m_Height, texture2D.m_MipCount);
                    else
                        dimension = string.Format("{0}x{1}", texture2D.m_Width, texture2D.m_Height);
                    if (texture2D.m_Width >= 512 || texture2D.m_Height >= 512)
                    {
                        result = InvokeExportTexture2D(item, exportPath);
                        filename = filename.Replace(exportPath, "Texture2D/");
                    }
                    else
                    {
                        rawData = texture2D.image_data.GetData();
                    }

                    format = texture2D.m_TextureFormat.ToString();

                    wrapMode = wrapModes[texture2D.m_TextureSettings.m_WrapMode];
                    break;
                }
                case ClassIDType.Texture2DArray:
                {
                    rawData = asset.GetRawData();
                    //result = ExportRawFile(item, exportPath, out filename);
                    //filename = filename.Replace(exportPath, "Texture2DArray/");
                    break;
                }
                case ClassIDType.Shader:
                {
                    rawData = asset.GetRawData();
                    //result = ExportRawFile(item, exportPath, out filename);
                    var shader = (Shader) asset;
                    //filename = filename.Replace(exportPath, "Shader/");
                    break;
                }
                case ClassIDType.Font:
                {
                    //rawData = item.Asset.GetRawData();
                    //result = ExportFont(item, exportPath, out filename);
                    //filename = filename.Replace(exportPath, "Font/");
                    //result = ExportRawFile(item, exportPath, out filename);
                    var font = (Font) asset;
                    rawData = font.m_FontData;
                    //filename = filename.Replace(exportPath, "Font/");
                    break;
                }
                case ClassIDType.Mesh:
                {
                    var mesh = (Mesh) asset;
                    if (mesh.m_VertexCount > 1000)
                    {
                        result = InvokeExportMesh(item, exportPath);
                        filename = filename.Replace(exportPath, "Mesh/");
                    }
                    else
                    {
                        rawData = asset.GetRawData();
                    }

                    //PreviewAsset()
                    //result = ExportRawFile(item, exportPath, out filename);
                    dimension = string.Format("vtx:{0} idx:{1} uv:{2} n:{3}",
                        mesh.m_VertexCount, mesh.m_Indices.Count, mesh.m_UV0?.Length, mesh.m_Normals?.Length);
                    //filename = filename.Replace(exportPath, "Mesh/");
                    break;
                }
                case ClassIDType.TextAsset:
                {
                    rawData = asset.GetRawData();
                    break;
                }
                case ClassIDType.AudioClip:
                {
                    rawData = asset.GetRawData();
                    //result = ExportRawFile(item, exportPath, out filename);
                    var audioClip = (AudioClip) asset;
                    //filename = filename.Replace(exportPath, "AudioClip/");
                    break;
                }
                case ClassIDType.AnimationClip:
                {
                    rawData = asset.GetRawData();
                    //result = ExportRawFile(item, exportPath, out filename);
                    var animationClip = (AnimationClip) asset;
                    //filename = filename.Replace(exportPath, "AnimationClip/");
                    break;
                }

                default:
                    return false;
            }

            if (rawData != null)
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] retVal = md5.ComputeHash(rawData);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    hash = sb.ToString();
                }
            }

            var text = GetTypeString(asset);
            var sourceFile = asset.assetsFile;
            //csvFile.Write("Name,Container,Type,Dimension,Format,Size,FileName,Hash,OriginalFile\n");
            var originalFile = sourceFile.originalPath ?? sourceFile.fullName;
            originalFile = originalFile.Replace(sourcePath, "");
            originalFile = originalFile.Replace("\\", "/");
            var fullSize = GetFullSize(asset);
            csvFile.Write(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\n",
                text, "", typeString, dimension, format, fullSize, filename, hash, originalFile, wrapMode));

            return result;
        }

        public static long GetFullSize(AssetStudio.Object asset)
        {
            switch (asset)
            {
                case Texture2D m_Texture2D:
                    if (!string.IsNullOrEmpty(m_Texture2D.m_StreamData?.path))
                        return asset.byteSize + m_Texture2D.m_StreamData.size;
                    break;
                case AudioClip m_AudioClip:
                    if (!string.IsNullOrEmpty(m_AudioClip.m_Source))
                        return asset.byteSize + m_AudioClip.m_Size;
                    break;
                case VideoClip m_VideoClip:
                    if (!string.IsNullOrEmpty(m_VideoClip.m_OriginalPath))
                        return asset.byteSize + (long)m_VideoClip.m_ExternalResources.m_Size;
                    break;
            }

            return asset.byteSize;
        }

        public static string GetTypeString(AssetStudio.Object asset)
        {
            switch (asset)
            {
                case GameObject m_GameObject:
                    return m_GameObject.m_Name;
                    break;
                case Texture2D m_Texture2D:
                    return m_Texture2D.m_Name;
                    break;
                case AudioClip m_AudioClip:
                    return m_AudioClip.m_Name;
                    break;
                case VideoClip m_VideoClip:
                    return m_VideoClip.m_Name;
                    break;
                case Shader m_Shader:
                    return m_Shader.m_ParsedForm?.m_Name ?? m_Shader.m_Name;
                    break;
                case Mesh _:
                case TextAsset _:
                case AnimationClip _:
                case Font _:
                case MovieTexture _:
                case Sprite _:
                    return ((NamedObject) asset).m_Name;
                    break;
                case Animator m_Animator:
                    if (m_Animator.m_GameObject.TryGet(out var gameObject))
                    {
                        return gameObject.m_Name;
                    }
                    break;
                case MonoBehaviour m_MonoBehaviour:
                    if (m_MonoBehaviour.m_Name == "" && m_MonoBehaviour.m_Script.TryGet(out var m_Script))
                    {
                        return m_Script.m_ClassName;
                    }
                    else
                    {
                        return m_MonoBehaviour.m_Name;
                    }
                    break;
                case PlayerSettings m_PlayerSettings:
                    return m_PlayerSettings.productName;
                    break;
                case AssetBundle m_AssetBundle:
                    return m_AssetBundle.m_Name;
                    break;
                case ResourceManager m_ResourceManager:
                    break;
                case NamedObject m_NamedObject:
                    return m_NamedObject.m_Name;
                    break;
            }
            var classIDType = (ClassIDType) GetFieldValue(asset, "type");
            return classIDType.ToString();
        }

        public static bool InvokeExportTexture2D(object item, string exportPath)
        {
            // // 首先，获取AssetItem类型和Exporter类型的Type对象
            // Assembly targetAssembly = Assembly.Load("AssetStudio"); // 替换为实际包含AssetItem和Exporter的程序集名称
            // Type assetItemType = targetAssembly.GetType("AssetStudioGUI.AssetItem", true); // 使用完整的命名空间
            // Type exporterType = targetAssembly.GetType("AssetStudioGUI.Exporter", true);
            //
            // // 检查item是否为AssetItem类型
            // if (assetItemType.IsInstanceOfType(item))
            // {
            //     // 获取ExportTexture2D方法信息
            //     MethodInfo exportTexture2DMethod = exporterType.GetMethod("ExportTexture2D", BindingFlags.Public | BindingFlags.Static);
            //
            //     // 调用ExportTexture2D静态方法
            //     bool result = (bool) exportTexture2DMethod.Invoke(null, new object[] {item, exportPath});
            //
            //     Console.WriteLine($"Export operation was {(result ? "successful" : "unsuccessful")}");
            //     return result;
            // }
            // else
            // {
            //     Console.WriteLine("Provided item is not an AssetItem instance.");
            //     return false;
            // }
            return true;
        }

        public static bool InvokeExportMesh(object item, string exportPath)
        {
            return true;
            // var m_Texture2D = (Texture2D) item;
            // var image = m_Texture2D.ConvertToImage(true);
            // if (image == null)
            //     return false;
            // using (image)
            // {
            //     using (var file = File.OpenWrite(exportPath))
            //     {
            //         image.WriteToStream(file);
            //     }
            //     return true;
            // }
        }
    }
}