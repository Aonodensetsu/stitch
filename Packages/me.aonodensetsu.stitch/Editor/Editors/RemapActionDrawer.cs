using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(RemapAction), true)]
  internal class RemapActionDrawer : BaseActionDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUIUtility.singleLineHeight * 2f + 2f;
    }

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
      if (string.IsNullOrWhiteSpace(value.stringValue) || float.TryParse(value.stringValue, out _)) EditorGUI.DrawRect(new Rect(valueRect.x, valueRect.y, 1f, valueRect.height), Color.yellow);

      // second line
      var lowIn = property.FindPropertyRelative("lowIn");
      var highIn = property.FindPropertyRelative("highIn");
      var lowOut = property.FindPropertyRelative("lowOut");
      var highOut = property.FindPropertyRelative("highOut");

      y += h + 2f;
      string dash = Strings.Get("general.remapDash");
      string to = Strings.Get("general.remapTo");

      float dashWidth = boldCenter.CalcSize(new GUIContent(dash)).x;
      float toWidth = boldCenter.CalcSize(new GUIContent(to)).x;

      float availableWidth2 = rect.width - dashWidth - dashWidth - toWidth - spacing * 6f;
      float fieldWidth2 = availableWidth2 / 4f;

      var lowInRect = new Rect(rect.x, y, fieldWidth2, h);
      var dashRect = new Rect(lowInRect.xMax + spacing, y, dashWidth, h);
      var highInRect = new Rect(dashRect.xMax + spacing, y, fieldWidth2, h);
      var toRect = new Rect(highInRect.xMax + spacing, y, toWidth, h);
      var lowOutRect = new Rect(toRect.xMax + spacing, y, fieldWidth2, h);
      var dashOutRect = new Rect(lowOutRect.xMax + spacing, y, dashWidth, h);
      var highOutRect = new Rect(dashOutRect.xMax + spacing, y, fieldWidth2, h);

      lowIn.floatValue = EditorGUI.FloatField(lowInRect, lowIn.floatValue);
      EditorGUI.LabelField(dashRect, dash, boldCenter);
      highIn.floatValue = EditorGUI.FloatField(highInRect, highIn.floatValue);
      EditorGUI.LabelField(toRect, to, boldCenter);
      lowOut.floatValue = EditorGUI.FloatField(lowOutRect, lowOut.floatValue);
      EditorGUI.LabelField(dashOutRect, dash, boldCenter);
      highOut.floatValue = EditorGUI.FloatField(highOutRect, highOut.floatValue);

      if (highIn.floatValue <= lowIn.floatValue) EditorGUI.DrawRect(new Rect(highInRect.x, highInRect.y, 1f, highInRect.height), Color.yellow);
      if (highOut.floatValue == lowOut.floatValue) EditorGUI.DrawRect(new Rect(highOutRect.x, highOutRect.y, 1f, highOutRect.height), Color.yellow);
    }
  }
}

