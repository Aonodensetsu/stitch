using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  #if HAS_VF
  [Serializable]
  [Action("action.global")]
  public class GlobalAction : Action {}
  #endif
}

