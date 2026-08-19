using UnityEditor;

namespace Me.Aonodensetsu.Stitch {
  [InitializeOnLoad]
  internal class VRCFuryLoader {
    static VRCFuryLoader() {
      #if HAS_VF
      Hook.VRCFury = new VRCFuryUpstream();
      #endif
    }
  }
}

