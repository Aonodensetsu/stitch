using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.timer")]
  public class TimerAction : Action {
    public TimerAction() {
      result = "Stitch_Timer";
    }

    public override bool Validate() => result == "Stitch_Timer";
  }
}

