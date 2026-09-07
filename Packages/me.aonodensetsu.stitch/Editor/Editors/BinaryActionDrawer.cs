using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(BinaryAction), true)]
  internal class BinaryActionDrawer : ActionDrawer {
    internal SerializedProperty left;
    internal SerializedProperty right;

    internal virtual bool LeftValidate() => !string.IsNullOrEmpty(left.stringValue);
    internal virtual bool RightValidate() => !string.IsNullOrEmpty(right.stringValue) && !(float.TryParse(left.stringValue, out _) && float.TryParse(right.stringValue, out _));

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      left = property.FindPropertyRelative("left");
      right = property.FindPropertyRelative("right");

      var root = base.CreatePropertyGUI(property);

      root.ElementAt(1).style.marginRight = 6 + StitchMenuEditor.Margin;

      root.Add(new Label(Strings.Get("general.equal")) { style = { marginRight = StitchMenuEditor.Margin } });

      var leftField = new PropertyField(left, "") {
        style = {
          flexGrow = 1,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      };
      root.Add(leftField);

      root.Add(root.ElementAt(0));

      var rightField = new PropertyField(right, "") { style = { flexGrow = 1 }};
      root.Add(rightField);

      ValidateProperty(leftField, LeftValidate);
      ValidateProperty(rightField, RightValidate);

      leftField.RegisterValueChangeCallback(e => {
        var f = rightField.Q<VisualElement>("unity-text-input");
        if (f == null) return;
        f.style.borderLeftColor = RightValidate() ? StyleKeyword.Null : Color.yellow;
        f.style.borderLeftWidth = 1;
      });

      return root;
    }
  }
}

