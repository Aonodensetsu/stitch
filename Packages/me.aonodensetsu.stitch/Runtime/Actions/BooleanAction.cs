using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class BooleanAction : BinaryAction {
    public override bool Validate() => base.Validate() && !float.TryParse(left, out _) && !float.TryParse(right, out _);
  }
}

