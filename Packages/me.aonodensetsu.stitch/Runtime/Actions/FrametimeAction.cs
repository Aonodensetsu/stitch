using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.frametime")]
  public class FrametimeAction : Action {
    public FrametimeAction() {
      result = "Stitch_Frametime";
    }

    public override bool Validate() => result == "Stitch_Frametime";
  }
}

