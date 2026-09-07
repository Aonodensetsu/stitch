using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(RemapAction), true)]
  internal class RemapActionDrawer : UnaryActionDrawer {
    internal SerializedProperty lowIn;
    internal SerializedProperty lowOut;
    internal SerializedProperty highIn;
    internal SerializedProperty highOut;

    internal override bool ValueValidate() => base.ValueValidate() && !float.TryParse(value.stringValue, out _);
    internal virtual bool HighInValidate() => highIn.floatValue > lowIn.floatValue;
    internal virtual bool HighOutValidate() => highOut.floatValue != lowOut.floatValue;

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      lowIn = property.FindPropertyRelative("lowIn");
      highIn = property.FindPropertyRelative("highIn");
      lowOut = property.FindPropertyRelative("lowOut");
      highOut = property.FindPropertyRelative("highOut");

      var root = new VisualElement();

      root.Add(base.CreatePropertyGUI(property));

      var lineTwo = new VisualElement {
        style = {
          flexDirection = FlexDirection.Row,
          alignItems = Align.Center,
          marginTop = StitchMenuEditor.Margin
        }
      };
      root.Add(lineTwo);

      lineTwo.Add(new PropertyField(lowIn, "") {
        style = {
          flexGrow = 1,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });
      lineTwo.Add(new Label(Strings.Get("general.remapDash")));

      var highInProp = new PropertyField(highIn, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      };
      lineTwo.Add(highInProp);

      lineTwo.Add(new Label(Strings.Get("general.remapTo")));

      lineTwo.Add(new PropertyField(lowOut, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });
      lineTwo.Add(new Label(Strings.Get("general.remapDash")));

      var highOutProp = new PropertyField(highOut, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin
        }
      };
      lineTwo.Add(highOutProp);

      ValidateProperty(highInProp, HighInValidate);
      ValidateProperty(highOutProp, HighOutValidate);
      return root;
    }
  }
}

