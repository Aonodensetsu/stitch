using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(PromoteAction), true)]
  internal class PromoteActionDrawer : UnaryActionDrawer {
    internal SerializedProperty type;

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      type = property.FindPropertyRelative("type");

      var root = base.CreatePropertyGUI(property);
      root.ElementAt(3).style.marginRight = 6 + StitchMenuEditor.Margin;
      root.Add(new PropertyField(type, ""));
      return root;
    }
  }
}

