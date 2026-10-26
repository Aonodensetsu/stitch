using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [Serializable]
  [Action("action.promote")]
  public class PromoteAction : UnaryAction {
    public enum PromoteType {
      Contact = 0,
      Synced = 1
    };

    public PromoteType type;
  }
}

