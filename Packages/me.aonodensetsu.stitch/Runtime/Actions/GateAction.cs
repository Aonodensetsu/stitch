using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.gate")]
  public class GateAction : BooleanAction {
    public float zeroZero;
    public float zeroOne;
    public float oneZero;
    public float oneOne;
  }
}

