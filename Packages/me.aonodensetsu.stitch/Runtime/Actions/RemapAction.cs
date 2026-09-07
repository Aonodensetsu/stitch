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
      return base.Validate() && highIn > lowIn && highOut != lowOut;
    }
  }
}

