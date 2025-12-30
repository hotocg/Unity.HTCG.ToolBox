using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTCG.Toolbox.Editor
{
    public class DevView : VisualElement
    {
        /// <summary>
        /// 属性数据
        /// </summary>
        private List<string> PropertyData = new List<string>();

        private ListView list_PropInfo;

        public DevView()
        {
            this.InitVisualTree();

            var bt_QueryProp = this.Q<Button>("bt_QueryProp");
            var bt_GoBack = this.Q<Button>("bt_GoBack");

            bt_QueryProp.clicked += OnClicked;

            list_PropInfo = this.Q<ListView>("list_PropInfo");
            list_PropInfo.itemsSource = PropertyData;
            list_PropInfo.makeItem = () => new Label(); // ListView Item 模板
            list_PropInfo.bindItem = (element, i) => {
                // Item 数据绑定
                var label = element as Label;
                label.text = PropertyData[i];

                label.style.whiteSpace = WhiteSpace.NoWrap;
                label.style.textOverflow = TextOverflow.Ellipsis;
                label.style.overflow = Overflow.Hidden;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                //label.tooltip = label.text;
            };

        }

        private void OnClicked()
        {
            PropertyData.Clear();
            PropertyData.AddRange(UnityUtil.GetObjAllProperties());
            //list_PropInfo.RefreshItems();
            list_PropInfo.Rebuild();
        }
    }
}