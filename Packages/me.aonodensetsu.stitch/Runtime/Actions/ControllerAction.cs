using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.controller")]
  public class ControllerAction : Action {
    public RuntimeAnimatorController c;

    public override bool Validate() => c != null;
  }
}

