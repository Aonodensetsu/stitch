using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(SmoothAction), true)]
  internal class SmoothActionDrawer : BaseActionDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      base.OnGUI(rect, property, label);

      var result = property.FindPropertyRelative("result");
      var value = property.FindPropertyRelative("value");
      var type = property.FindPropertyRelative("type");
      var delta = property.FindPropertyRelative("delta");
      string equalsName = Strings.Get("general.equal");
      string actionName = GetActionName(property);
      string[] enumNames = {
        Strings.Get("general.smoothExponential"),
        Strings.Get("general.smoothLinear")
      };

      const float spacing = 4f;

      float equalsWidth = boldCenter.CalcSize(new GUIContent(equalsName)).x;
      float actionWidth = boldCenter.CalcSize(new GUIContent(actionName)).x;
      float availableWidth = rect.width - equalsWidth - actionWidth - spacing * 5f;
      float fieldWidth = availableWidth / 4f;
      float y = rect.y + 2f;
      float h = EditorGUIUtility.singleLineHeight;

      var resultRect = new Rect(rect.x, y, fieldWidth, h);
      var equalsRect = new Rect(resultRect.xMax + spacing, y, equalsWidth, h);
      var actionRect = new Rect(equalsRect.xMax + spacing, y, actionWidth, h);
      var valueRect = new Rect(actionRect.xMax + spacing, y, fieldWidth, h);
      var typeRect = new Rect(valueRect.xMax + spacing, y, fieldWidth, h);
      var deltaRect = new Rect(typeRect.xMax + spacing, y, fieldWidth, h);

      result.stringValue = EditorGUI.TextField(resultRect, result.stringValue);
      EditorGUI.LabelField(equalsRect, equalsName, boldCenter);
      EditorGUI.LabelField(actionRect, actionName, boldCenter);
      value.stringValue = EditorGUI.TextField(valueRect, value.stringValue);
      type.enumValueIndex = EditorGUI.Popup(typeRect, type.enumValueIndex, enumNames);
      delta.floatValue = EditorGUI.FloatField(deltaRect, delta.floatValue);

      if (string.IsNullOrWhiteSpace(result.stringValue) || float.TryParse(result.stringValue, out _)) EditorGUI.DrawRect(new Rect(resultRect.x, resultRect.y, 1f, resultRect.height), Color.yellow);
      if (string.IsNullOrWhiteSpace(value.stringValue) || float.TryParse(value.stringValue, out _)) EditorGUI.DrawRect(new Rect(valueRect.x, valueRect.y, 1f, valueRect.height), Color.yellow);
      if (1f <= delta.floatValue || 0f >= delta.floatValue) EditorGUI.DrawRect(new Rect(deltaRect.x, deltaRect.y, 1f, deltaRect.height), Color.yellow);
    }
  }
}

