using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.remap")]
  public class RemapAction : UnaryAction {
    public float lowIn;
    public float highIn;
    public float lowOut;
    public float highOut;

    public override bool Validate() {
      return !float.TryParse(result, out _) && !float.TryParse(value, out _) && highIn > lowIn;
    }
  }
}

