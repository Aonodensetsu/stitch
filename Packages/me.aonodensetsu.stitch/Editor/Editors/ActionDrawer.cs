using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(Action), true)]
  internal class ActionDrawer : PropertyDrawer {
    internal SerializedProperty result;

    internal virtual bool ResultValidate() => !string.IsNullOrEmpty(result.stringValue) && !float.TryParse(result.stringValue, out _);

    internal static string GetActionName(SerializedProperty property) {
      var action = property.managedReferenceValue;
      var attribute = (ActionAttribute)System.Attribute.GetCustomAttribute(action.GetType(), typeof(ActionAttribute));

      return Strings.Get(attribute?.LocalizationKey).Split("/").Last() ?? action.GetType().Name;
    }

    internal void ValidateProperty(PropertyField prop, Func<bool> check) {
      prop.RegisterValueChangeCallback(e => {
        var t = prop.Q<VisualElement>("unity-text-input");
        t.style.borderLeftColor = check() ? StyleKeyword.Null : Color.yellow;
        t.style.borderLeftWidth = 1;
      });
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      result = property.FindPropertyRelative("result");

      var root = new VisualElement {
        style = {
          flexDirection = FlexDirection.Row,
          alignItems = Align.Center
        }
      };

      root.Add(new Label(GetActionName(property)) {
        style = {
          marginRight = StitchMenuEditor.Margin,
          unityFontStyleAndWeight = FontStyle.Bold,
          unityTextAlign = TextAnchor.MiddleCenter
        }
      });

      var resultField = new PropertyField(result, "") { style = { flexGrow = 1 } };
      root.Add(resultField);

      ValidateProperty(resultField, ResultValidate);
      return root;
    }
  }
}

