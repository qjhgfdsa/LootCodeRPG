using UnityEngine;

namespace SA.UI.Icons
{
    [CreateAssetMenu(fileName = "IconDisplayProfile", menuName = "SA/UI/Icon Display Profile")]
    public class IconDisplayProfile : ScriptableObject
    {
        [Tooltip("FitInside keeps the icon inside padded bounds. FillWithBleed scales past the mask for a cropped hero look.")]
        public IconFitMode fitMode = IconFitMode.FillWithBleed;

        [Range(0f, 0.25f)]
        public float padding = 0.08f;

        [Tooltip("Only used when fitMode is FillWithBleed.")]
        [Min(1f)]
        public float bleedScale = 1.18f;

        public Vector2 offset = Vector2.zero;
    }
}
