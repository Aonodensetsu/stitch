using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.debug")]
  public class DebugAction : Action {
    public DebugAction() {
      result = "Stitch_Version";
    }

    public override bool Validate() => result == "Stitch_Version";
  }
}

