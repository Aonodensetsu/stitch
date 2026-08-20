using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.smooth")]
  public class SmoothAction : UnaryAction {
    public enum SmoothType {
      Exponential = 0,
      Linear = 1
    };

    public float delta;
    public SmoothType type;

    public override bool Validate() {
      return !float.TryParse(result, out _) && !float.TryParse(value, out _) && delta > 0f && delta < 1f;
    }
  }
}

