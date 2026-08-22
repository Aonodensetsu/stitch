using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(GlobalAction), true)]
  internal class GlobalActionDrawer : PropertyDrawer {
    private GUIStyle boldCenter;

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      boldCenter ??= new GUIStyle(EditorStyles.boldLabel) {
        alignment = TextAnchor.MiddleCenter
      };

      var result = property.FindPropertyRelative("result");
      string actionName = GetActionName(property);

      const float spacing = 4f;

      float actionWidth = boldCenter.CalcSize(new GUIContent(actionName)).x;
      float fieldWidth = rect.width - actionWidth - spacing * 1f;
      float y = rect.y + 2f;
      float h = EditorGUIUtility.singleLineHeight;

      var actionRect = new Rect(rect.x, y, actionWidth, h);
      var resultRect = new Rect(actionRect.xMax + spacing, y, fieldWidth, h);

      EditorGUI.LabelField(actionRect, actionName, boldCenter);
      result.stringValue = EditorGUI.TextField(resultRect, result.stringValue);

      if (string.IsNullOrWhiteSpace(result.stringValue) || float.TryParse(result.stringValue, out _)) EditorGUI.DrawRect(new Rect(resultRect.x, resultRect.y, 1f, resultRect.height), Color.yellow);
    }

    private static string GetActionName(SerializedProperty property) {
      var action = property.managedReferenceValue;
      var attribute = System.Attribute.GetCustomAttribute(action.GetType(), typeof(ActionAttribute)) as ActionAttribute;

      return Strings.Get(attribute?.LocalizationKey) ?? action.GetType().Name;
    }
  }
}

