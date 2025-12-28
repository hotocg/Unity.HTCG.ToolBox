using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTCG.Toolbox.Editor
{
    public class CleanView : VisualElement
    {
        public CleanView(string packagePath)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{packagePath}/Editor/Views/CleanView.uxml");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
            }

            var bt_Clean = this.Q<Button>("bt_Clean");
            if (bt_Clean != null)
            {
                bt_Clean.clicked += OnClicked;
            }
        }

        private void OnClicked()
        {
            MainViewModel.Ins.StateInfo = $"{DateTime.Now.ToString()} | Cleaning...";
        }
    }
}