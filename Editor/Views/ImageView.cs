using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTCG.Toolbox.Editor
{
    public class ImageView : VisualElement
    {
        public ImageView()
        {
            this.InitVisualTree();

            this.Bind(new SerializedObject(MainViewModel.Ins));

            var bt_ImageToPrefab = this.Q<Button>("bt_ImageToPrefab");
            if (bt_ImageToPrefab != null)
            {
                bt_ImageToPrefab.clicked += () =>
                {
                    int result = UnityUtil.ToPrefab();
                    MainViewModel.Ins.StateInfo = result > 0 ? $"成功生成 {result} 个预制件" : "未选中有效对象";
                };
            }

            var bt_ImageGridSplit = this.Q<Button>("bt_ImageGridSplit");
            if (bt_ImageGridSplit != null)
            {
                bt_ImageGridSplit.clicked += () =>
                {
                    UnityUtil.ImageGridSplit();
                };
            }

            var bt_Test = this.Q<Button>("bt_Test");
            if (bt_Test != null)
            {
                bt_Test.clicked += () =>
                {
                    MainViewModel.Ins.PlayerName = GUID.Generate().ToString();
                    MainViewModel.Ins.PlayerScore = new System.Random().Next();
                };
            }
        }

        private void OnClicked()
        {

        }
    }
}