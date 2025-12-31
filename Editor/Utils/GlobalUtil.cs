using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using Codice.CM.Common.Serialization.Replication;

namespace HTCG.Toolbox.Editor
{
    public class GlobalUtil
    {

    }

    public static class ShowMsg
    {
        private static string Format(params object[] args) => string.Join(" ", args);

        private static bool Show(string message, string title, string ok, string cancel)
        {
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        }
        public static void Info(params object[] args) => Show(Format(args), "提示", "确认", null);

        public static void Warning(params object[] args) => Show(Format(args), "警告", "确认", null);

        public static void Error(params object[] args) => Show(Format(args), "错误", "确认", null);

        public static bool Query(params object[] args) => Show(Format(args), "确认操作", "确认", "取消");
    }

    public static class ViewExtensions
    {
        /// <summary>
        /// 路径缓存
        /// </summary>
        private static readonly Dictionary<System.Type, string> PathCache = new();
        /// <summary>
        /// 获取 UXML 文件路径
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetUxmlPath(this VisualElement obj)
        {
            var type = obj.GetType();
            if (PathCache.TryGetValue(type, out string cachedPath)) return cachedPath;

            var guids = AssetDatabase.FindAssets($"t:MonoScript {type.Name}");
            if (guids.Length > 0)
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var uxmlPath = Path.ChangeExtension(scriptPath, ".uxml");
                PathCache[type] = uxmlPath;
                return uxmlPath;
            }
            return null;
        }
        /// <summary>
        /// 初始化视觉树
        /// </summary>
        /// <param name="obj"></param>
        public static void InitVisualTree(this VisualElement obj)
        {
            var uxmlPath = obj.GetUxmlPath();
            var ussPath = Path.ChangeExtension(uxmlPath, ".uss");
            if (string.IsNullOrEmpty(uxmlPath)) return;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            var style = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            if (visualTree != null)
            {
                visualTree.CloneTree(obj);
                if (style != null) obj.styleSheets.Add(style);
            }
            else
            {
                throw new System.Exception($"未找到 UXML 文件: {uxmlPath}");
            }
        }
    }

    public static class UnityUtil
    {
        /// <summary>
        /// 获取当前资源窗口所在的目录
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentAssetPath()
        {
            string path = "Assets";

            var projectBrowserType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
            var projectBrowser = EditorWindow.GetWindow(projectBrowserType);

            if (projectBrowser != null)
            {
                var getActiveFolderPath = projectBrowserType.GetMethod("GetActiveFolderPath", BindingFlags.Instance | BindingFlags.NonPublic);
                path = getActiveFolderPath.Invoke(projectBrowser, null).ToString();
            }

            return path;
        }

        /// <summary>
        /// 把当前选择的物体转为预制件
        /// </summary>
        /// <returns></returns>
        public static int ToPrefab()
        {
            Object[] selection = Selection.objects;
            int count = 0;

            foreach (var obj in selection)
            {
                Debug.Log(obj);
                string assetPath = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(assetPath))
                {
                    // 场景
                    if (obj is GameObject sceneObj)
                    {
                        CreatePrefabFromSceneObj(sceneObj);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    // 资源
                    if (obj is Texture2D tex)
                    {
                        if (!CreatePrefabFromTexture(tex)) continue;
                    }
                    else
                    {
                        Debug.Log($"其他类型，未处理: {obj}");
                        continue;
                    }
                }

                count++;
            }

            // 刷新资源数据库（Project 视图）
            AssetDatabase.Refresh();

            return count;
        }

        /// <summary>
        /// 场景对象转预制件
        /// </summary>
        /// <param name="obj"></param>
        public static void CreatePrefabFromSceneObj(GameObject obj)
        {
            string localPath = $"{GetCurrentAssetPath()}/{obj.name}.prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            PrefabUtility.SaveAsPrefabAsset(obj, localPath);
        }

        //[MenuItem("Assets/HTCG ToolBox/将图片转为预制件", false, 1)]
        /// <summary>
        /// 纹理对象转预制件
        /// </summary>
        public static bool CreatePrefabFromTexture(Texture2D tex)
        {
            // 获取图片在 Assets 中的相对路径
            string assetPath = AssetDatabase.GetAssetPath(tex);
            // 获取指定路径第一个类型为 Sprite 的对象
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite == null) return false;

            // 在内存中创建一个 GameObject，以图片名命名
            GameObject go = new GameObject(tex.name);
            // 给对象添加 SpriteRenderer 组件，并赋值 Sprite
            go.AddComponent<SpriteRenderer>().sprite = sprite;

            // 获取图片所在的文件夹目录路径
            string dir = Path.GetDirectoryName(assetPath);
            // 生成一个唯一的资产路径（同名会自动加后缀）
            string prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{dir}/{tex.name}.prefab");

            // 创建预制件
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            // 销毁临时对象
            Object.DestroyImmediate(go);

            return true;
        }

        public static void ImageGridSplit()
        {
            Object[] selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            foreach (var obj in selection)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer is TextureImporter texImporter)
                {
                    texImporter.textureType = TextureImporterType.Sprite;
                    texImporter.spriteImportMode = SpriteImportMode.Multiple;

                    // 初始化
                    var factory = new SpriteDataProviderFactories();
                    factory.Init();

                    // 获取数据提供者
                    var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImporter);
                    dataProvider.InitSpriteEditorDataProvider();

                    // 计算并设置切片
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    float w = texture.width / 3f;
                    float h = texture.height / 3f;

                    var newRects = new SpriteRect[9];
                    for (int i = 0; i < 9; i++)
                    {
                        int x = i % 3;
                        int y = i / 3;
                        newRects[i] = new SpriteRect
                        {
                            name = $"{texture.name}_{i}",
                            rect = new Rect(x * w, (2 - y) * h, w, h),
                            alignment = SpriteAlignment.Center,
                            pivot = new Vector2(0.5f, 0.5f),
                            spriteID = GUID.Generate()
                        };
                    }

                    // 设置和应用
                    dataProvider.SetSpriteRects(newRects);
                    dataProvider.Apply();

                    // 写入并重导
                    texImporter.SaveAndReimport();
                }
            }
        }

        public static List<string> GetObjAllProperties()
        {
            List<string> list = new List<string>();

            var obj = Selection.activeObject;
            if (obj == null)
            {
                MainViewModel.Ins.StateInfo = "请选择一个对象";
                return list;
            }

            var tabStr = new string('-', 25);
            string path = AssetDatabase.GetAssetPath(obj);

            list.Add($"[Is GameObject] {obj is GameObject}");
            list.Add($"[AssetPath] {path}");

            // 对象
            list.Add($"{tabStr} [Object: {obj}]");
            GetReflectionInfo(obj, list);

            Debug.Log(EditorJsonUtility.ToJson(obj, true));

            // 场景对象
            if (obj is GameObject gObj)
            {
                // 遍历组件
                Component[] components = gObj.GetComponents<Component>();
                foreach (var comp in components)
                {
                    if (comp == null) continue;

                    list.Add($"{tabStr} [Component: {comp.GetType().Name}]");
                    GetReflectionInfo(comp, list);
                    list.Add("");

                    Debug.Log(EditorJsonUtility.ToJson(comp, true));
                }
            }

            // 资产对象
            if (!string.IsNullOrEmpty(path))
            {
                AssetImporter importer = AssetImporter.GetAtPath(path);
                if (importer != null && importer != obj)
                {
                    list.Add($"{tabStr} [AssetImporter: {importer.GetType().Name}]");
                    list.Add($"Path: {path}");
                    GetReflectionInfo(importer, list);

                    Debug.Log(EditorJsonUtility.ToJson(importer, true));
                }
            }

            return list;
        }

        private static void GetReflectionInfo(object obj, List<string> list, int depth = 0, string prefix = "")
        {
            try
            {
                // 限制递归深度
                if (obj == null || depth > 3) return;

                // 跳过基础类型
                System.Type type = obj.GetType();
                if (type.IsPrimitive || type == typeof(string) || type == typeof(Vector3) || type == typeof(Color)) return;

                PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in props)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(p.PropertyType)) continue;

                    if (p.DeclaringType == typeof(Matrix4x4))
                    {
                        list.Add($"{list.Count} | continue | {p}");
                        continue;
                    }

                    try
                    {
                        object value = p.GetValue(obj, null);
                        string indent = new string(' ', depth * 4); // 缩进

                        list.Add($"{list.Count} | {indent}{prefix}{p.Name} = {value ?? "null"} | [{p.PropertyType.Name}]");

                        GetReflectionInfo(value, list, depth + 1, p.Name + ".");
                    }
                    catch
                    {
                        
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log($"[Reflection] {ex}");
            }
        }

    }

    /// <summary>
    /// 包管理
    /// <para>[InitializeOnLoad] 在 Unity 加载时以及脚本重新编译时初始化类</para>
    /// </summary>
    [InitializeOnLoad]
    public class PackageManager
    {
        public const string RemotePackageUpdateUrl = "https://github.com/hotocg/Unity.HTCG.ToolBox.git";
        public const string RemotePackageUrl = "https://raw.githubusercontent.com/hotocg/Unity.HTCG.ToolBox/master/package.json";
        public const string PackageName = "com.htcg.toolbox";

        public static UnityEditor.PackageManager.PackageInfo LocalPackageInfo;
        private class RemotePackageInfo
        {
            public string name;
            public string version;
        }

        static PackageManager()
        {

        }

        /// <summary>
        /// 检测更新
        /// </summary>
        /// <param name="callback"></param>
        public static void CheckUpdate(System.Action<bool> callback = null)
        {
            // 非播放模式下才执行
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Debug.Log("Check Update ...");

            var request = UnityWebRequest.Get(RemotePackageUrl);
            var operation = request.SendWebRequest();
            
            LocalPackageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(PackageName);
            if (LocalPackageInfo == null)
            {
                Debug.Log("无法获取包信息");
                return;
            }

            operation.completed += _ =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("无法连接到 GitHub 检测更新: " + request.error);
                    return;
                }

                // 获取远程版本号
                string remoteJson = request.downloadHandler.text;
                string remoteVersion = ParseVersion(remoteJson);

                // 获取本地版本号
                string localVersion = LocalPackageInfo.version;
                MainViewModel.Ins.Version = localVersion;

                // 对比版本
                var isNewer = IsNewer(remoteVersion, localVersion);
                callback?.Invoke(isNewer);

                if (isNewer)
                {
                    MainViewModel.Ins.Version = "新版本！";
                    //if (PackageInfo.source == PackageSource.Local)
                    //{
                    //    Debug.Log(JsonUtility.ToJson(PackageInfo, true));
                    //    return; // 本地包不提示
                    //}

                    //if (EditorUtility.DisplayDialog("检测更新", $"发现新版本！{remoteVersion}\n是否现在更新？", "立即更新", "稍后再说"))
                    //{
                    //    UpdateToolbox(() =>
                    //    {
                    //        MainViewModel.Ins.Version = remoteVersion;
                    //    });
                    //}
                }
            };
        }

        /// <summary>
        /// 更新工具箱
        /// </summary>
        /// <param name="callback"></param>
        public static void UpdateToolbox(System.Action callback = null)
        {
            if (!EditorUtility.DisplayDialog("检测更新", "是否立即更新？", "确认", "取消")) return;
            var request = Client.Add(RemotePackageUpdateUrl);

            EditorApplication.CallbackFunction handler = null;
            handler = () =>
            {
                // 任务是否完成
                if (request.IsCompleted)
                {
                    EditorApplication.update -= handler;
                    // 是否成功
                    if (request.Status == StatusCode.Success)
                    {
                        Debug.Log($"更新完成 {request.Result.displayName} {request.Result.version}");
                        callback?.Invoke();
                    }
                }
            };

            EditorApplication.update += handler;
        }

        private static string ParseVersion(string json)
        {
            try 
            {
                var data = JsonUtility.FromJson<RemotePackageInfo>(json);
                return !string.IsNullOrEmpty(data.version) ? data.version : "0.0.0";
            }
            catch
            {
                return "0.0.0";
            }
        }

        private static bool IsNewer(string remote, string local)
        {
            System.Version vRemote = new System.Version(remote);
            System.Version vLocal = new System.Version(local);
            return vRemote > vLocal;
        }
    }
}