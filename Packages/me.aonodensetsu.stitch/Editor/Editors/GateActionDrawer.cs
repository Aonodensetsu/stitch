using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(GateAction), true)]
  internal class GateActionDrawer : PrefixedBinaryActionDrawer {
    internal SerializedProperty zeroZero;
    internal SerializedProperty zeroOne;
    internal SerializedProperty oneZero;
    internal SerializedProperty oneOne;

    internal override bool LeftValidate() => base.LeftValidate() && !float.TryParse(left.stringValue, out _);
    internal override bool RightValidate() => base.RightValidate() && !float.TryParse(right.stringValue, out _);

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      zeroZero = property.FindPropertyRelative("zeroZero");
      zeroOne = property.FindPropertyRelative("zeroOne");
      oneZero = property.FindPropertyRelative("oneZero");
      oneOne = property.FindPropertyRelative("oneOne");

      var root = new VisualElement();

      var lineOne = base.CreatePropertyGUI(property);
      root.Add(lineOne);

      var lineTwo = new VisualElement {
        style = {
          flexDirection = FlexDirection.Row,
          alignItems = Align.Center,
          marginTop = StitchMenuEditor.Margin
        }
      };
      root.Add(lineTwo);

      lineTwo.Add(new Label(Strings.Get("general.gateZeroZero")));
      lineTwo.Add(new PropertyField(zeroZero, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });
      lineTwo.Add(new Label(Strings.Get("general.gateZeroOne")));
      lineTwo.Add(new PropertyField(zeroOne, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });
      lineTwo.Add(new Label(Strings.Get("general.gateOneZero")));
      lineTwo.Add(new PropertyField(oneZero, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });
      lineTwo.Add(new Label(Strings.Get("general.gateOneOne")));
      lineTwo.Add(new PropertyField(oneOne, "") {
        style = {
          flexGrow = 1,
          marginLeft = StitchMenuEditor.Margin
        }
      });

      return root;
    }
  }
}

