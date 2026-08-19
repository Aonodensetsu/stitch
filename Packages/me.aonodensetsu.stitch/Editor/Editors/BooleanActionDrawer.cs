using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(BooleanAction), true)]
  internal class BooleanActionDrawer : BinaryActionDrawer {
    internal override void InheritedHighlight(SerializedProperty left, Rect leftRect, SerializedProperty right, Rect rightRect) {
      if (string.IsNullOrWhiteSpace(left.stringValue) || float.TryParse(left.stringValue, out _)) EditorGUI.DrawRect(new Rect(leftRect.x, leftRect.y, 1f, leftRect.height), Color.yellow);
      if (string.IsNullOrWhiteSpace(right.stringValue) || float.TryParse(right.stringValue, out _)) EditorGUI.DrawRect(new Rect(rightRect.x, rightRect.y, 1f, rightRect.height), Color.yellow);
    }
  }
}

