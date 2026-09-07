using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  public abstract class Action {
    public string result;

    public virtual bool Validate() {
      return !string.IsNullOrEmpty(result) && !float.TryParse(result, out _);
    }
  }
}

