using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(CompareAction), true)]
  internal class CompareActionDrawer : BinaryActionDrawer {
    internal SerializedProperty type;

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      type = property.FindPropertyRelative("type");

      var root = base.CreatePropertyGUI(property);
      root.Remove(root.ElementAt(3));

      root.Insert(3, new PropertyField(type, "") {
        style = {
          unityFontStyleAndWeight = FontStyle.Bold,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      });

      return root;
    }
  }
}

