using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;
using System;

namespace Me.Aonodensetsu.Stitch {
  [AddComponentMenu("Stitch")]
  public class StitchMenu : MonoBehaviour, IEditorOnly {
    [SerializeReference]
    public List<Action> actions = new List<Action>();
    [SerializeField]
    public string id;

    private void OnValidate() {
      var ids = new HashSet<string>();
      foreach (var action in FindObjectsByType<StitchMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
        if (!ids.Add(action.id)) action.id = Guid.NewGuid().ToString("N")[..7];
      }
    }
  }
}

