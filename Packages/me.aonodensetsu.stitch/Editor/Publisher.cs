using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;

#if HAS_VF
using com.vrcfury.api;
#endif

#if HAS_MA
using nadena.dev.modular_avatar.core;
#endif

namespace Me.Aonodensetsu.Stitch {
  internal interface Publisher {
    void Publish(GameObject obj, AnimatorController controller, List<string> globals);
  }

  internal class InstructionPublisher : Publisher {
    public void Publish(GameObject obj, AnimatorController controller = null, List<string> globals = null) {
      var flagPath = $"Assets/{Strings.Get("support.manualFlag")}.prefab";
      var flag = AssetDatabase.LoadAssetAtPath<GameObject>(flagPath);
      if (flag != null) {
        Debug.LogError($"Stitch: {Strings.Get("log.noRebuild")}", flag);
        return;
      }
      var msg = new GameObject("Stitch");
      msg.transform.parent = obj.transform;
      msg.tag = "EditorOnly";
      UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(msg.transform, false);
      msg.AddComponent<StitchInstruction>();

      var asset = PrefabUtility.SaveAsPrefabAsset(msg, flagPath);
      Debug.LogError($"Stitch: {Strings.Get("log.createInstruction")}", asset);
    }
  }

  #if HAS_VF
  internal class VRCFuryPublisher : Publisher {
    public void Publish(GameObject obj, AnimatorController controller, List<string> globals) {
      var fc = FuryComponents.CreateFullController(obj);
      foreach (var p in globals) fc.AddGlobalParam(p);
      fc.AddController(controller);
    }
  }
  #endif

  #if HAS_MA
  internal class ModularAvatarPublisher : Publisher {
    public void Publish(GameObject obj, AnimatorController controller, List<string> globals) {
      var mp = obj.AddComponent<ModularAvatarParameters>();
      mp.parameters.AddRange(
        controller.parameters.Select(p => new ParameterConfig {
          nameOrPrefix = p.name,
          isPrefix = false,
          defaultValue = p.defaultFloat,
          syncType = ParameterSyncType.NotSynced,
          internalParameter = !globals.Contains(p.name),
          localOnly = true,
          saved = false
        })
      );

      var mm = obj.AddComponent<ModularAvatarMergeBlendTree>();
      mm.Motion = controller.layers[0].stateMachine.states[0].state.motion;
    }
  }
  #endif
}

