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

    public SmoothType type;
    public float delta;

    public override bool Validate() => base.Validate() && delta > 0 && delta < 1;
  }
}

