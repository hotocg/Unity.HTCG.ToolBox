using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTCG.Toolbox.Editor
{
    public class ImageView : VisualElement
    {
        public ImageView(string packagePath)
        {
            this.Bind(new SerializedObject(MainViewModel.Ins));

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{packagePath}/Editor/Views/ImageView.uxml");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
            }

            var bt_ImageToPrefab = this.Q<Button>("bt_ImageToPrefab");
            if (bt_ImageToPrefab != null)
            {
                bt_ImageToPrefab.clicked += () =>
                {
                    int result = UnityUtil.ImageToPrefab();
                    MainViewModel.Ins.StateInfo = result > 0 ? $"成功生成 {result} 个预制件" : "未选中有效图片";
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