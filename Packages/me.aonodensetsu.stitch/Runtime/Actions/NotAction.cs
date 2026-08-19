using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.not")]
  public class NotAction : UnaryAction {
    public override bool Validate() {
      return !float.TryParse(result, out _) && !float.TryParse(value, out _);
    }
  }
}

