using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class UnaryAction : Action {
    public string value;

    public override bool Validate() {
      return base.Validate() && !string.IsNullOrEmpty(value) && !float.TryParse(value, out _);
    }
  }
}

