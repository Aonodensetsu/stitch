using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.drive")]
  public class DriveAction : Action {
    public float min;
    public float max = 1;
    // NOTE: runtime checked to have at least 2 non-constant keyframes
    public AnimationClip clip;

    public override bool Validate() => base.Validate() && min < max && clip != null;
  }
}

