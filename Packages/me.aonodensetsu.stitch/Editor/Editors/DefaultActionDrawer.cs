using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(DefaultAction), true)]
  internal class DefaultActionDrawer : UnaryActionDrawer {
    internal override bool ValueValidate() => float.TryParse(value.stringValue, out _);
  }
}

