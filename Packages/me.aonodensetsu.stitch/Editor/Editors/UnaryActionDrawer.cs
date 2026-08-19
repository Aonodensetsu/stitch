using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(UnaryAction), true)]
  internal class UnaryActionDrawer : BaseActionDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      base.OnGUI(rect, property, label);

      var result = property.FindPropertyRelative("result");
      var value = property.FindPropertyRelative("value");
      string equalsName = Strings.Get("general.equal");
      string actionName = GetActionName(property);

      const float spacing = 4f;

      float equalsWidth = boldCenter.CalcSize(new GUIContent(equalsName)).x;
      float actionWidth = boldCenter.CalcSize(new GUIContent(actionName)).x;
      float availableWidth = rect.width - equalsWidth - actionWidth - spacing * 3f;
      float fieldWidth = availableWidth / 2f;
      float y = rect.y + 2f;
      float h = EditorGUIUtility.singleLineHeight;

      var resultRect = new Rect(rect.x, y, fieldWidth, h);
      var equalsRect = new Rect(resultRect.xMax + spacing, y, equalsWidth, h);
      var actionRect = new Rect(equalsRect.xMax + spacing, y, actionWidth, h);
      var valueRect = new Rect(actionRect.xMax + spacing, y, fieldWidth, h);

      result.stringValue = EditorGUI.TextField(resultRect, result.stringValue);
      EditorGUI.LabelField(equalsRect, equalsName, boldCenter);
      EditorGUI.LabelField(actionRect, actionName, boldCenter);
      value.stringValue = EditorGUI.TextField(valueRect, value.stringValue);

      if (string.IsNullOrWhiteSpace(result.stringValue) || float.TryParse(result.stringValue, out _)) EditorGUI.DrawRect(new Rect(resultRect.x, resultRect.y, 1f, resultRect.height), Color.yellow);
      InheritedHighlight(value, valueRect);
    }

    internal virtual void InheritedHighlight(SerializedProperty value, Rect valueRect) {
      if (string.IsNullOrWhiteSpace(value.stringValue)) EditorGUI.DrawRect(new Rect(valueRect.x, valueRect.y, 1f, valueRect.height), Color.yellow);
    }
  }
}

