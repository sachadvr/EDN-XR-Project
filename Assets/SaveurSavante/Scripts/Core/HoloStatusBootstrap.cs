using UnityEngine;
using TMPro;

namespace SaveurSavante.Core
{
    public static class HoloStatusBootstrap
    {
        public static TextMeshPro EnsureHoloText(Transform parent, string name, Vector3 localOffset, float fontSize = 1.2f)
        {
            if (parent == null) return null;

            Transform existing = parent.Find(name);
            TextMeshPro tmp;
            if (existing != null)
            {
                tmp = existing.GetComponent<TextMeshPro>();
                if (tmp == null) tmp = existing.gameObject.AddComponent<TextMeshPro>();
            }
            else
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.transform.localPosition = localOffset;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                tmp = go.AddComponent<TextMeshPro>();
            }

            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = Color.black;
            tmp.rectTransform.sizeDelta = new Vector2(4f, 2f);
            tmp.gameObject.SetActive(true);
            tmp.enabled = true;
            return tmp;
        }
    }
}
