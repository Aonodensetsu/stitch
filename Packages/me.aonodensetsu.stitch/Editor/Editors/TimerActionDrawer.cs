using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(TimerAction), true)]
  [CustomPropertyDrawer(typeof(FrametimeAction), true)]
  internal class TimerActionDrawer : ActionDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      var root = base.CreatePropertyGUI(property);
      root.ElementAt(1).Q<PropertyField>().SetEnabled(false);
      return root;
    }
  }
}

