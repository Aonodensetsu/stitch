using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.input")]
  public class InputAction : Action {
    public VRCExpressionsMenu menu;
    public VRCExpressionParameters param;

    public override bool Validate() => true;
  }
}

