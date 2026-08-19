using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.gate")]
  public class GateAction : BinaryAction {
    public float zeroZero;
    public float zeroOne;
    public float oneZero;
    public float oneOne;

    public override bool Validate() {
      return !float.TryParse(result, out _) && !float.TryParse(left, out _) && !float.TryParse(right, out _);
    }
  }
}

