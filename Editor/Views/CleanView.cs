using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTCG.Toolbox.Editor
{
    public class CleanView : VisualElement
    {
        public CleanView()
        {
            this.InitVisualTree();

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