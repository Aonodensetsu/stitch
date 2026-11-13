using UnityEditor.UIElements;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(ControllerAction), true)]
  internal class ControllerActionDrawer : ActionDrawer {
    internal SerializedProperty c;

    internal virtual bool CValidate() => c.objectReferenceValue != null;

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      c = property.FindPropertyRelative("c");

      var root = base.CreatePropertyGUI(property);
      root.RemoveAt(1);

      var cProp = new PropertyField(c, "") {
        style = {
          flexGrow = 1
        }
      };
      root.Add(cProp);

      cProp.RegisterValueChangeCallback(e => {
        var t = cProp.Q<ObjectField>();
        if (t == null) return;
        t.style.borderLeftColor = CValidate() ? StyleKeyword.Null : Color.yellow;
        t.style.borderLeftWidth = 1;
      });
      return root;
    }
  }
}

