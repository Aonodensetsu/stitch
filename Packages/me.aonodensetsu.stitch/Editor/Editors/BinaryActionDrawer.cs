using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(BinaryAction), true)]
  internal class BinaryActionDrawer : BaseActionDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      base.OnGUI(rect, property, label);

      var result = property.FindPropertyRelative("result");
      var left = property.FindPropertyRelative("left");
      var right = property.FindPropertyRelative("right");
      string equalsName = Strings.Get("general.equal");
      string actionName = GetActionName(property);

      const float spacing = 4f;

      float equalsWidth = boldCenter.CalcSize(new GUIContent(equalsName)).x;
      float actionWidth = boldCenter.CalcSize(new GUIContent(actionName)).x;
      float availableWidth = rect.width - equalsWidth - actionWidth - spacing * 4f;
      float fieldWidth = availableWidth / 3f;
      float y = rect.y + 2f;
      float h = EditorGUIUtility.singleLineHeight;

      var resultRect = new Rect(rect.x, y, fieldWidth, h);
      var equalsRect = new Rect(resultRect.xMax + spacing, y, equalsWidth, h);
      var leftRect = new Rect(equalsRect.xMax + spacing, y, fieldWidth, h);
      var actionRect = new Rect(leftRect.xMax + spacing, y, actionWidth, h);
      var rightRect = new Rect(actionRect.xMax + spacing, y, fieldWidth, h);

      result.stringValue = EditorGUI.TextField(resultRect, result.stringValue);
      EditorGUI.LabelField(equalsRect, equalsName, boldCenter);
      left.stringValue = EditorGUI.TextField(leftRect, left.stringValue);
      EditorGUI.LabelField(actionRect, actionName, boldCenter);
      right.stringValue = EditorGUI.TextField(rightRect, right.stringValue);

      if (string.IsNullOrWhiteSpace(result.stringValue) || float.TryParse(result.stringValue, out _)) EditorGUI.DrawRect(new Rect(resultRect.x, resultRect.y, 1f, resultRect.height), Color.yellow);
      InheritedHighlight(left, leftRect, right, rightRect);
    }

    internal virtual void InheritedHighlight(SerializedProperty left, Rect leftRect, SerializedProperty right, Rect rightRect) {
      if (string.IsNullOrWhiteSpace(left.stringValue)) EditorGUI.DrawRect(new Rect(leftRect.x, leftRect.y, 1f, leftRect.height), Color.yellow);
      if (string.IsNullOrWhiteSpace(right.stringValue)) EditorGUI.DrawRect(new Rect(rightRect.x, rightRect.y, 1f, rightRect.height), Color.yellow);
    }
  }
}

