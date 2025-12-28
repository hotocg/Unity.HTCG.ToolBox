using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTCG.Toolbox.Editor
{
    //public partial class MainViewModel : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    public bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    //    {
    //        if (Equals(storage, value)) return false;
    //        storage = value;
    //        OnPropertyChanged(propertyName);
    //        return true;
    //    }
    //}

    public partial class MainViewModel : ScriptableObject
    {
        public static MainViewModel _ins;
        //public static MainViewModel Ins => _ins ??= ScriptableObject.CreateInstance<MainViewModel>();
        public static MainViewModel Ins
        {
            get
            {
                if (_ins == null)
                {
                    // 尝试从内存中获取
                    //_ins = Resources.FindObjectsOfTypeAll<MainViewModel>().FirstOrDefault();
                    if (_ins == null)
                    {
                        _ins = ScriptableObject.CreateInstance<MainViewModel>();
                        // 不显示在层级视图中，不保存到场景中，也不通过 Resources.UnloadUnusedAssets 卸载
                        //_ins.hideFlags = HideFlags.HideAndDontSave;

                        Debug.Log("创建 MainViewModel");
                    }
                    else
                    {
                        Debug.Log("从 Resources 中获取 MainViewModel");
                    }
                }
                return _ins;
            }
        }

        /// <summary>
        /// 状态信息
        /// </summary>
        [SerializeField]
        public string StateInfo = DateTime.Now.ToString();

        [SerializeField]
        public int PlayerScore = 999;

        [SerializeField]
        public string PlayerName = "123456";

    }
}