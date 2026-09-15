using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(PrefixedBinaryAction), true)]
  internal class PrefixedBinaryActionDrawer : BinaryActionDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      var root = base.CreatePropertyGUI(property);
      root.Insert(2, root.ElementAt(3));
      return root;
    }
  }
}

