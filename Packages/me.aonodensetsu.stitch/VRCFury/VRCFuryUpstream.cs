using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

#if HAS_VF
using com.vrcfury.api;
#endif

namespace Me.Aonodensetsu.Stitch {
  internal class VRCFuryUpstream : Publisher {
    public void Publish(GameObject avatar, AnimatorController controller, List<string> globals) {
      #if HAS_VF
      var fc = FuryComponents.CreateFullController(avatar);
      foreach (var p in globals) fc.AddGlobalParam(p);
      fc.AddController(controller);
      #endif
    }
  }
}

