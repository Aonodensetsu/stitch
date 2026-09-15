using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(TrinaryAction), true)]
  internal class TrinaryActionDrawer : BinaryActionDrawer {
    internal SerializedProperty middle;

    internal virtual bool MiddleValidate() => !string.IsNullOrEmpty(middle.stringValue) && (!float.TryParse(middle.stringValue, out var f) || (f >= -100 && f <= 100)) && !(float.TryParse(left.stringValue, out _) && float.TryParse(middle.stringValue, out _));
    internal override bool RightValidate() => !string.IsNullOrEmpty(right.stringValue) && (!float.TryParse(right.stringValue, out var f) || (f >= -100 & f <= 100)) && (new[] { left, middle, right }.Count(x => float.TryParse(x.stringValue, out _)) <= 1);

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      middle = property.FindPropertyRelative("middle");
      var root = base.CreatePropertyGUI(property);

      var leftField = root.ElementAt(2).Q<PropertyField>();
      var rightField = root.ElementAt(4).Q<PropertyField>();
      var middleField = new PropertyField(middle, "") {
        style = {
          flexGrow = 1,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      };
      root.Insert(4, middleField);

      ValidateProperty(middleField, MiddleValidate);
      CrossValidateProperty(leftField, middleField, MiddleValidate);
      CrossValidateProperty(middleField, rightField, RightValidate);
      return root;
    }
  }
}

