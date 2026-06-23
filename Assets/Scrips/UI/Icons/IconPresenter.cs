using SA;
using UnityEngine;
using UnityEngine.UI;

namespace SA.UI.Icons
{
    public static class IconPresenter
    {
        public static void Show(Image target, IconId id, IconDisplayProfile profile)
        {
            if (target == null)
                return;

            if (profile == null)
            {
                Debug.LogWarning("IconPresenter: profile is null.");
                Clear(target);
                return;
            }

            IconRegistry registry = IconRegistry.Instance;
            if (registry == null)
            {
                Debug.LogWarning("IconPresenter: IconRegistry not found at Resources/SA.IconRegistry.");
                Clear(target);
                return;
            }

            Sprite sprite = registry.Resolve(id);
            if (sprite == null)
            {
                Clear(target);
                return;
            }

            Show(target, sprite, profile);
        }

        public static void Show(Image target, Sprite sprite, IconDisplayProfile profile)
        {
            if (target == null)
                return;

            if (profile == null)
            {
                Debug.LogWarning("IconPresenter: profile is null.");
                Clear(target);
                return;
            }

            if (sprite == null)
            {
                Clear(target);
                return;
            }

            EnsureMask(target);
            ApplyLayout(target, profile);

            target.preserveAspect = true;
            target.sprite = sprite;
            target.enabled = true;
            target.gameObject.SetActive(true);
        }

        public static void Clear(Image target)
        {
            if (target == null)
                return;

            target.sprite = null;
            target.enabled = false;
            target.gameObject.SetActive(false);
        }

        static void ApplyLayout(Image target, IconDisplayProfile profile)
        {
            RectTransform rect = target.rectTransform;
            RectTransform parent = rect.parent as RectTransform;
            if (parent == null)
                return;

            float pad = Mathf.Clamp(profile.padding, 0f, 0.25f);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = profile.offset;

            Vector2 parentSize = parent.rect.size;
            rect.offsetMin = new Vector2(parentSize.x * pad, parentSize.y * pad);
            rect.offsetMax = new Vector2(-parentSize.x * pad, -parentSize.y * pad);

            float scale = profile.fitMode == IconFitMode.FillWithBleed
                ? Mathf.Max(1f, profile.bleedScale)
                : 1f;
            rect.localScale = new Vector3(scale, scale, 1f);
        }

        static void EnsureMask(Image icon)
        {
            RectTransform iconRect = icon.rectTransform;
            if (iconRect == null || iconRect.parent == null)
                return;

            RectMask2D clipMask = iconRect.parent.GetComponent<RectMask2D>();
            if (clipMask == null)
                iconRect.parent.gameObject.AddComponent<RectMask2D>();

            icon.maskable = true;
        }
    }
}
