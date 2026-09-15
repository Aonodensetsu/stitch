using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.select")]
  public class SelectAction : TrinaryAction {
    public override bool Validate() => base.Validate() && (!float.TryParse(middle, out var mf) || (mf > 0 && mf < 1));
  }
}

