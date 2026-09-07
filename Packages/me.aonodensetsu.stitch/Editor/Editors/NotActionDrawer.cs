using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(NotAction), true)]
  internal class NotActionDrawer : UnaryActionDrawer {
    internal override bool ValueValidate() => base.ValueValidate() && !float.TryParse(value.stringValue, out _);
  }
}

