
using UnityEngine;

namespace HTCG.Toolbox.Editor
{
    public enum PivotPosition
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, Center, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public static class Extensions
    {
        /// <summary>
        /// 锚点位置转为 Vector2
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this PivotPosition pivot)
        {
            return pivot switch
            {
                PivotPosition.TopLeft      => new Vector2(0f, 1f),
                PivotPosition.TopCenter    => new Vector2(0.5f, 1f),
                PivotPosition.TopRight     => new Vector2(1f, 1f),
            
                PivotPosition.MiddleLeft   => new Vector2(0f, 0.5f),
                PivotPosition.Center       => new Vector2(0.5f, 0.5f),
                PivotPosition.MiddleRight  => new Vector2(1f, 0.5f),
            
                PivotPosition.BottomLeft   => new Vector2(0f, 0f),
                PivotPosition.BottomCenter => new Vector2(0.5f, 0f),
                PivotPosition.BottomRight  => new Vector2(1f, 0f),
            
                _ => new Vector2(0.5f, 0.5f)
            };
        }
    }

    public class ImageViewModel : ScriptableObject
    {
        private static ImageViewModel _ins;
        public static ImageViewModel Ins => _ins ??= (_ins = CreateInstance<ImageViewModel>());

        /// <summary>
        /// 分割的格子数量
        /// </summary>
        [SerializeField]
        public Vector2Int SplitCellCount = new Vector2Int(3, 3);
        /// <summary>
        /// 选择的格子锚点位置
        /// </summary>
        [SerializeField]
        public PivotPosition SelectCellPivot = PivotPosition.Center;

        [SerializeField]
        public int PlayerScore = 999;

        [SerializeField]
        public string PlayerName = "abcdefg";


    }
}