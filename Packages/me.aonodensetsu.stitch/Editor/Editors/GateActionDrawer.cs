using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(GateAction), true)]
  internal class GateActionDrawer : BaseActionDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUIUtility.singleLineHeight * 2f + 2f;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      base.OnGUI(rect, property, label);

      var result = property.FindPropertyRelative("result");
      string equalsName = Strings.Get("general.equal");
      string actionName = GetActionName(property);
      var left = property.FindPropertyRelative("left");
      var right = property.FindPropertyRelative("right");

      const float spacing = 4f;

      float equalsWidth = boldCenter.CalcSize(new GUIContent(equalsName)).x;
      float actionWidth = boldCenter.CalcSize(new GUIContent(actionName)).x;
      float availableWidth = rect.width - equalsWidth - actionWidth - spacing * 4f;
      float fieldWidth = availableWidth / 3f;
      float y = rect.y + 2f;
      float h = EditorGUIUtility.singleLineHeight;

      var resultRect = new Rect(rect.x, y, fieldWidth, h);
      var equalsRect = new Rect(resultRect.xMax + spacing, y, equalsWidth, h);
      var actionRect = new Rect(equalsRect.xMax + spacing, y, actionWidth, h);
      var leftRect = new Rect(actionRect.xMax + spacing, y, fieldWidth, h);
      var rightRect = new Rect(leftRect.xMax + spacing, y, fieldWidth, h);

      result.stringValue = EditorGUI.TextField(resultRect, result.stringValue);
      EditorGUI.LabelField(equalsRect, equalsName, boldCenter);
      EditorGUI.LabelField(actionRect, actionName, boldCenter);
      left.stringValue = EditorGUI.TextField(leftRect, left.stringValue);
      right.stringValue = EditorGUI.TextField(rightRect, right.stringValue);

      if (string.IsNullOrWhiteSpace(result.stringValue) || float.TryParse(result.stringValue, out _)) EditorGUI.DrawRect(new Rect(resultRect.x, resultRect.y, 1f, resultRect.height), Color.yellow);
      if (string.IsNullOrWhiteSpace(left.stringValue) || float.TryParse(left.stringValue, out _)) EditorGUI.DrawRect(new Rect(leftRect.x, leftRect.y, 1f, leftRect.height), Color.yellow);
      if (string.IsNullOrWhiteSpace(right.stringValue) || float.TryParse(right.stringValue, out _)) EditorGUI.DrawRect(new Rect(rightRect.x, rightRect.y, 1f, rightRect.height), Color.yellow);

      // second line
      var zeroZero = property.FindPropertyRelative("zeroZero");
      var zeroOne = property.FindPropertyRelative("zeroOne");
      var oneZero = property.FindPropertyRelative("oneZero");
      var oneOne = property.FindPropertyRelative("oneOne");

      y += h + 2f;
      string zeroZeroName = Strings.Get("general.gateZeroZero");
      string zeroOneName = Strings.Get("general.gateZeroOne");
      string oneZeroName = Strings.Get("general.gateOneZero");
      string oneOneName = Strings.Get("general.gateOneOne");

      float zeroZeroWidth = boldCenter.CalcSize(new GUIContent(zeroZeroName)).x;
      float zeroOneWidth = boldCenter.CalcSize(new GUIContent(zeroOneName)).x;
      float oneZeroWidth = boldCenter.CalcSize(new GUIContent(oneZeroName)).x;
      float oneOneWidth = boldCenter.CalcSize(new GUIContent(oneOneName)).x;

      float availableWidth2 = rect.width - zeroZeroWidth - zeroOneWidth - oneZeroWidth - oneOneWidth - spacing * 7f;
      float fieldWidth2 = availableWidth2 / 4f;

      var zeroZeroNameRect = new Rect(rect.x, y, zeroZeroWidth, h);
      var zeroZeroRect = new Rect(zeroZeroNameRect.xMax + spacing, y, fieldWidth2, h);
      var zeroOneNameRect = new Rect(zeroZeroRect.xMax + spacing, y, zeroOneWidth, h);
      var zeroOneRect = new Rect(zeroOneNameRect.xMax + spacing, y, fieldWidth2, h);
      var oneZeroNameRect = new Rect(zeroOneRect.xMax + spacing, y, oneZeroWidth, h);
      var oneZeroRect = new Rect(oneZeroNameRect.xMax + spacing, y, fieldWidth2, h);
      var oneOneNameRect = new Rect(oneZeroRect.xMax + spacing, y, oneOneWidth, h);
      var oneOneRect = new Rect(oneOneNameRect.xMax + spacing, y, fieldWidth2, h);

      EditorGUI.LabelField(zeroZeroNameRect, zeroZeroName, boldCenter);
      zeroZero.floatValue = EditorGUI.FloatField(zeroZeroRect, zeroZero.floatValue);
      EditorGUI.LabelField(zeroOneNameRect, zeroOneName, boldCenter);
      zeroOne.floatValue = EditorGUI.FloatField(zeroOneRect, zeroOne.floatValue);
      EditorGUI.LabelField(oneZeroNameRect, oneZeroName, boldCenter);
      oneZero.floatValue = EditorGUI.FloatField(oneZeroRect, oneZero.floatValue);
      EditorGUI.LabelField(oneOneNameRect, oneOneName, boldCenter);
      oneOne.floatValue = EditorGUI.FloatField(oneOneRect, oneOne.floatValue);
    }
  }
}

