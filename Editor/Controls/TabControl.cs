using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace HTCG.Toolbox.Editor.UI
{
    public class TabControl : VisualElement
    {
        /// <summary>
        /// 公开到 uxml
        /// </summary>
        public class UxamlFactory : UxmlFactory<TabControl> { }

        private VisualElement TabItems = new VisualElement();
        private VisualElement TabContent = new VisualElement();

        /// <summary>
        /// 缓存视图
        /// </summary>
        private Dictionary<string, VisualElement> ViewCache = new Dictionary<string, VisualElement>();

        private Button CurrentButton;
        private VisualElement CurrentView;

        /// <summary>
        /// 最后一次打开的标签
        /// <para>注册表路径 HKEY_CURRENT_USER\SOFTWARE\Unity Technologies\Unity Editor 5.x</para>
        /// </summary>
        public string LastTab
        {
            get { return EditorPrefs.GetString("HTCG.Toolbox.LastTab"); }
            set { EditorPrefs.SetString("HTCG.Toolbox.LastTab", value); }
        }

        public TabControl()
        {
            this.style.flexDirection = FlexDirection.Row;
            this.style.flexGrow = 1;
            this.style.overflow = Overflow.Hidden;

            //TabItems.style.width = 100;
            TabContent.style.flexGrow = 1;

            TabItems.AddToClassList("tab-items");
            TabContent.AddToClassList("tab-content");
            this.Add(TabItems);
            this.Add(TabContent);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void AddTab(string name, VisualElement content)
        {
            try
            {
                var bt = new Button();
                bt.name = name;
                bt.text = name;
                bt.AddToClassList("tab-button");
                bt.clicked += () => SwitchTab(bt);

                content.style.display = DisplayStyle.None;

                TabItems.Add(bt);
                TabContent.Add(content);
                ViewCache.Add(name, content);

                if (bt.name == LastTab || (ViewCache.Count == 1 && string.IsNullOrWhiteSpace(LastTab))) SwitchTab(bt);
            }
            catch (System.Exception ex)
            {
                ShowMsg.Error(ex);
            }
        }

        /// <summary>
        /// 切换视图
        /// </summary>
        private void SwitchTab(Button bt)
        {
            if (bt == CurrentButton) return;

            // 按钮高亮
            CurrentButton?.RemoveFromClassList("active");
            bt.AddToClassList("active");
            CurrentButton = bt;

            if (ViewCache.TryGetValue(bt.name, out VisualElement nextView))
            {
                if (CurrentView != null) CurrentView.style.display = DisplayStyle.None;

                // 显示视图
                CurrentView = nextView;
                CurrentView.style.display = DisplayStyle.Flex;

                // 透明度动画
                CurrentView.style.opacity = 0;
                CurrentView.experimental.animation.Start(new StyleValues { opacity = 1f }, 250);
            }
            else
            {
                ShowMsg.Error($"未找到视图：{bt.name}\n{string.Join("\n", ViewCache.Select(x => $"{x.Key} {x.Value.GetType()}"))}");
            }

            LastTab = bt.name;
        }

    }
}