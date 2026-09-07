using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(UnaryAction), true)]
  internal class UnaryActionDrawer : ActionDrawer {
    internal SerializedProperty value;

    internal virtual bool ValueValidate() => !string.IsNullOrEmpty(value.stringValue);

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      value = property.FindPropertyRelative("value");

      var root = base.CreatePropertyGUI(property);
      root.ElementAt(1).style.marginRight = 6 + StitchMenuEditor.Margin;

      root.Add(new Label(Strings.Get("general.equal")) {
        style = {
          marginRight = StitchMenuEditor.Margin,
          unityTextAlign = TextAnchor.MiddleCenter
        }
      });

      root.Add(root.ElementAt(0));

      var valueField = new PropertyField(value, "") { style = { flexGrow = 1 } };
      root.Add(valueField);

      ValidateProperty(valueField, ValueValidate);
      return root;
    }
  }
}

