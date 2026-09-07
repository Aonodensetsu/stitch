using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.default")]
  public class DefaultAction : UnaryAction {
    public override bool Validate() {
      return !string.IsNullOrEmpty(result) && !float.TryParse(result, out _) && float.TryParse(value, out _);
    }
  }
}

