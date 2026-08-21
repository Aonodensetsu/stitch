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
    internal string flagPath = $"Assets/{Strings.Get("general.manualFlag")}.prefab";

    public GameObject FlagObj() {
      return AssetDatabase.LoadAssetAtPath<GameObject>(flagPath);
    }

    public void Publish(GameObject obj, AnimatorController controller, List<string> globals = null) {
      var flag = FlagObj();
      if (flag == null) {
        obj.tag = "EditorOnly";
        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(obj.transform, false);

        var component = obj.AddComponent<StitchPublishReminder>();
        component.controller = controller;
        var asset = PrefabUtility.SaveAsPrefabAsset(obj, flagPath);

        Debug.LogError($"Stitch: {Strings.Get("log.manualPublish")}", asset);
      }
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
      var mc = obj.AddComponent<ModularAvatarMergeAnimator>();
      mc.animator = controller;
      mc.matchAvatarWriteDefaults = true;
      mc.deleteAttachedAnimator = true;
    }
  }
  #endif
}

