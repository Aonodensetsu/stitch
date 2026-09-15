using UnityEditor;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(CopyAction), true)]
  internal class CopyActionDrawer : UnaryActionDrawer {
    internal override bool ValueValidate() => base.ValueValidate() && !float.TryParse(value.stringValue, out _);
  }
}

