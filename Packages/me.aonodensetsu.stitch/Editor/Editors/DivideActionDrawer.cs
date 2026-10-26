using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(DivideAction), true)]
  internal class DivideActionDrawer : BinaryActionDrawer {
    internal override bool RightValidate() => base.RightValidate() && (!float.TryParse(right.stringValue, out var f) || (f >= 0.00999 && f <= 100));
  }
}

