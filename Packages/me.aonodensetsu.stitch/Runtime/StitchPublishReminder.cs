using UnityEngine;
using VRC.SDKBase;

namespace Me.Aonodensetsu.Stitch {
  [AddComponentMenu("")]
  public class StitchPublishReminder : MonoBehaviour, IEditorOnly {
    [SerializeReference]
    public RuntimeAnimatorController controller;
  }
}

