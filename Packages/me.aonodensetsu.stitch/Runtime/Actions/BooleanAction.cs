using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class BooleanAction : BinaryAction {
    public override bool Validate() {
      return !float.TryParse(result, out _) && !float.TryParse(left, out _) && !float.TryParse(right, out _);
    }
  }
}

