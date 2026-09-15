using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class TrinaryAction : BinaryAction {
    public string middle;

    public override bool Validate() => base.Validate() && !string.IsNullOrEmpty(middle) && (!float.TryParse(middle, out var mf) || (mf >= -100 && mf <= 100)) && (new[] {left, middle, right}.Count(x => float.TryParse(x, out _)) <= 1);
  }
}

