using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HTCG.Toolbox.Editor
{
    /// <summary>
    /// 工具箱窗口类，继承自 EditorWindow，用于创建自定义编辑器窗口
    /// </summary>
    public class MainView : EditorWindow
    {
        /// <summary>
        /// 包路径
        /// </summary>
        public const string PackagePath = "Packages/com.htcg.toolbox";

        /// <summary>
        /// MenuItem 在主菜单栏创建菜单项，点击时调用此方法
        /// </summary>
        [MenuItem("HTCG/工具箱")]
        public static void ShowWindow()
        {
            // 获取窗口实例
            var window = GetWindow<MainView>();
            window.titleContent = new GUIContent($"HTCG 工具箱");
        }

        private void OnEnable()
        {

        }

        /// <summary>
        /// 当编辑器窗口创建 UI 时调用
        /// </summary>
        public void CreateGUI()
        {
            try
            {
                // 动态加载
                string uxmlPath = $"{PackagePath}/Editor/MainView.uxml";
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

                if (visualTree == null)
                {
                    throw new System.Exception($"未找到 UXML 文件\n{uxmlPath}");
                }

                // 实例化 UXML
                visualTree.CloneTree(rootVisualElement);

                rootVisualElement.Bind(new SerializedObject(MainViewModel.Ins));

                // ==========
                var tabControl = rootVisualElement.Q<UI.TabControl>("TabControl");
                tabControl.AddTab("图片", new ImageView());
                tabControl.AddTab("清理", new CleanView());
                tabControl.AddTab("开发", new DevView());


            }
            catch (System.Exception ex)
            {
                ShowMsg.Error(ex);
            }

        }

    }
}