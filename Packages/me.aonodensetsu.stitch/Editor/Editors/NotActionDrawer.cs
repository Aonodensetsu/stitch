using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(NotAction), true)]
  internal class NotActionDrawer : UnaryActionDrawer {
    internal override void InheritedHighlight(SerializedProperty value, Rect valueRect) {
      if (string.IsNullOrWhiteSpace(value.stringValue) || float.TryParse(value.stringValue, out _)) EditorGUI.DrawRect(new Rect(valueRect.x, valueRect.y, 1f, valueRect.height), Color.yellow);
    }
  }
}

