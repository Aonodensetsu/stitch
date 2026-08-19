using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class BinaryAction : Action {
    public string left;
    public string right;
  }
}

