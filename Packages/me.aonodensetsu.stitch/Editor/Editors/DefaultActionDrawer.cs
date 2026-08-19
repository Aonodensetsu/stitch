using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(DefaultAction), true)]
  internal class DefaultActionDrawer : PropertyDrawer {
    private GUIStyle boldCenter;

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      boldCenter ??= new GUIStyle(EditorStyles.boldLabel) {
        alignment = TextAnchor.MiddleCenter
      };

      var result = property.FindPropertyRelative("result");
      var value = property.FindPropertyRelative("value");
      string actionName = GetActionName(property);
      string defaulttoName = Strings.Get("general.defaultto");

      const float spacing = 4f;

      float actionWidth = boldCenter.CalcSize(new GUIContent(actionName)).x;
      float defaulttoWidth = boldCenter.CalcSize(new GUIContent(defaulttoName)).x;
      float availableWidth = rect.width - defaulttoWidth - actionWidth - spacing * 3f;
      float fieldWidth = availableWidth / 2f;
      float y = rect.y;
      float h = EditorGUIUtility.singleLineHeight;

      var actionRect = new Rect(rect.x, y, actionWidth, h);
      var resultRect = new Rect(actionRect.xMax + spacing, y, fieldWidth, h);
      var defaulttoRect = new Rect(resultRect.xMax + spacing, y, defaulttoWidth, h);
      var valueRect = new Rect(defaulttoRect.xMax + spacing, y, fieldWidth, h);

      EditorGUI.LabelField(actionRect, actionName, boldCenter);
      result.stringValue = EditorGUI.TextField(resultRect, result.stringValue);
      EditorGUI.LabelField(defaulttoRect, defaulttoName, boldCenter);
      value.floatValue = EditorGUI.FloatField(valueRect, value.floatValue);

      if (string.IsNullOrWhiteSpace(result.stringValue)) EditorGUI.DrawRect(new Rect(resultRect.x, resultRect.y, 1f, resultRect.height), Color.yellow);
      if (float.IsNaN(value.floatValue)) EditorGUI.DrawRect(new Rect(valueRect.x, valueRect.y, 1f, valueRect.height), Color.yellow);
    }

    private static string GetActionName(SerializedProperty property) {
      var action = property.managedReferenceValue;
      var attribute = System.Attribute.GetCustomAttribute(action.GetType(), typeof(ActionAttribute)) as ActionAttribute;

      return Strings.Get(attribute?.LocalizationKey) ?? action.GetType().Name;
    }
  }
}

