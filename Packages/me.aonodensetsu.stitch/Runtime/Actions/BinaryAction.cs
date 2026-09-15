using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class BinaryAction : Action {
    public string left;
    public string right;

    public override bool Validate() => base.Validate() && !string.IsNullOrEmpty(left) && !string.IsNullOrEmpty(right) && !(float.TryParse(left, out _) && float.TryParse(right, out _)) && (!float.TryParse(left, out var lf) || (lf >= -100 && lf <= 100)) && (!float.TryParse(right, out var rf) || (rf >= -100 && rf <= 100));
  }
}

