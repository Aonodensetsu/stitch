using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(SelectAction), true)]
  internal class SelectActionDrawer : TrinaryActionDrawer {
    internal override bool LeftValidate() => !string.IsNullOrEmpty(left.stringValue) && (!float.TryParse(left.stringValue, out var f) || (f > 0 && f < 1));
  }
}

