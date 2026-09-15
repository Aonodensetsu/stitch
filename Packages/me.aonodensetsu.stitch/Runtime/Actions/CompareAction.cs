using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.compare")]
  public class CompareAction : BinaryAction {
    public enum CompareType {
      MoreThan  = 0,
      MoreEqual = 1,
      LessThan  = 2,
      LessEqual = 3,
      Equal     = 4,
      Inequal   = 5
    };

    public CompareType type;
  }
}

