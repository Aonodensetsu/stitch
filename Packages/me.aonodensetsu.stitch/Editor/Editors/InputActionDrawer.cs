using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(InputAction), true)]
  internal class InputActionDrawer : ActionDrawer {
    internal SerializedProperty menu;
    internal SerializedProperty param;

    internal override bool ResultValidate() => true;

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      menu = property.FindPropertyRelative("menu");
      param = property.FindPropertyRelative("param");

      var root = new VisualElement();
      var lineOne = new VisualElement {
        style = {
          flexDirection = FlexDirection.Row,
          alignItems = Align.Center,
          marginBottom = StitchMenuEditor.Margin
        }
      };
      root.Add(lineOne);

      lineOne.Add(new PropertyField(menu, "") {
        style = {
          flexGrow = 1,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });
      lineOne.Add(new PropertyField(param, "") {
        style = {
          flexGrow = 1
        }
      });

      root.Add(base.CreatePropertyGUI(property));
      lineOne.Insert(0, root.ElementAt(1).ElementAt(0));
      root.ElementAt(1).Insert(0, new Label(Strings.Get("general.inputPrefix")));
      return root;
    }
  }
}

