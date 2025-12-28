using System.IO;
using UnityEditor;
using UnityEngine;

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

    public class UnityUtil
    {
        [MenuItem("Assets/HTCG ToolBox/将图片转为预制件", false, 1)]
        /// <summary>
        /// 图片转预制件
        /// </summary>
        public static int ImageToPrefab()
        {
            // 从当前选中的资源中获取所有类型为 Texture2D 的资源
            Object[] selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            int count = 0;

            foreach (var obj in selection)
            {
                // 获取图片在 Assets 中的相对路径
                string assetPath = AssetDatabase.GetAssetPath(obj);
                // 获取指定路径第一个类型为 Sprite 的资产对象
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null) continue;

                // 在内存中创建一个 GameObject，以图片名命名
                GameObject go = new GameObject(obj.name);
                // 给对象添加 SpriteRenderer 组件，并赋值 Sprite
                go.AddComponent<SpriteRenderer>().sprite = sprite;

                // 获取图片所在的文件夹目录路径
                string dir = Path.GetDirectoryName(assetPath);
                // 生成一个唯一的资产路径（如果同名会自动加上 1, 2 等后缀）
                string prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{dir}/{obj.name}.prefab");

                // 创建预制件
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                // 销毁临时对象
                Object.DestroyImmediate(go);

                count++;
            }

            // 刷新资源数据库（Project 视图）
            AssetDatabase.Refresh();

            return count;
        }
    }
}