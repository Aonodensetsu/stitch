using UnityEditor;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(BooleanAction), true)]
  internal class BooleanActionDrawer : BinaryActionDrawer {
    internal override bool LeftValidate() => base.LeftValidate() && !float.TryParse(left.stringValue, out _);
    internal override bool RightValidate() => base.RightValidate() && !float.TryParse(right.stringValue, out _);
  }
}

