using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class BinaryAction : Action {
    public string left;
    public string right;

    public override bool Validate() {
      return base.Validate() && !string.IsNullOrEmpty(left) && !string.IsNullOrEmpty(right) && !(float.TryParse(left, out _) && float.TryParse(right, out _));
    }
  }
}

