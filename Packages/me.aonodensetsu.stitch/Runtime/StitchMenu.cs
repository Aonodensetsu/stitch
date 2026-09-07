using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;
using System;

namespace Me.Aonodensetsu.Stitch {
  [AddComponentMenu("Stitch")]
  public class StitchMenu : MonoBehaviour, IEditorOnly {
    [SerializeReference]
    public List<Action> actions = new List<Action>();
  }
}

