using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;

#if HAS_VF
using VRC.SDK3.Avatars.ScriptableObjects;
using com.vrcfury.api;
#endif

#if HAS_MA
using nadena.dev.modular_avatar.core;
#endif

namespace Me.Aonodensetsu.Stitch {
  internal interface Publisher {
    void Publish(Actions actions);
  }

  internal class InstructionPublisher : Publisher {
    public void Publish(Actions actions) {
      var flagPath = $"Assets/{Strings.Get("support.manualFlag")}.prefab";
      var flag = AssetDatabase.LoadAssetAtPath<GameObject>(flagPath);
      if (flag != null) {
        Debug.LogError($"Stitch: {Strings.Get("log.noRebuild")}", flag);
        return;
      }
      var msg = new GameObject("Stitch");
      msg.transform.parent = actions.Obj.transform;
      msg.tag = "EditorOnly";
      UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(msg.transform, false);
      msg.AddComponent<StitchInstruction>();

      var asset = PrefabUtility.SaveAsPrefabAsset(msg, flagPath);
      Debug.LogError($"Stitch: {Strings.Get("log.createInstruction")}", asset);
    }
  }

  #if HAS_VF
  internal class VRCFuryPublisher : Publisher {
    public void Publish(Actions actions) {
      var fc = FuryComponents.CreateFullController(actions.Obj);
      foreach (var p in actions.globals) fc.AddGlobalParam(p);
      if (actions.promotions.Any(a => a.type == PromoteAction.PromoteType.Synced)) fc.AddParams(new VRCExpressionParameters {
        parameters = actions.promotions.Where(a => a.type == PromoteAction.PromoteType.Synced).Select(e => new VRCExpressionParameters.Parameter {
          name = e.result,
          valueType = VRCExpressionParameters.ValueType.Float,
          saved = false
        }).ToArray()
      });
      fc.AddController(actions.Controller);
    }
  }
  #endif

  #if HAS_MA
  internal class ModularAvatarPublisher : Publisher {
    // NOTE: the Drive action doesn't work in debug mode for some reason
    public void Publish(Actions actions) {
      actions.Obj.AddComponent<ModularAvatarParameters>().parameters.AddRange(
        actions.Controller.parameters.Select(p => new ParameterConfig {
          nameOrPrefix = p.name,
          isPrefix = false,
          defaultValue = p.defaultFloat,
          syncType = actions.promotions.Where(a => a.type == PromoteAction.PromoteType.Synced).Select(e => e.result).Contains(p.name) ? ParameterSyncType.Float : ParameterSyncType.NotSynced,
          internalParameter = !actions.globals.Contains(p.name),
          localOnly = !actions.promotions.Select(x => x.result).Contains(p.name),
          saved = false
        })
      );

      if (!actions.isDebug) {
        var t = actions.Obj.AddComponent<ModularAvatarMergeBlendTree>();
        t.Motion = actions.Controller.layers[0].stateMachine.states[0].state.motion;
        t.PathMode = nadena.dev.modular_avatar.core.MergeAnimatorPathMode.Absolute;
      }
      if (actions.isDebug || actions.Controller.layers.Length > 1) {
        actions.Controller.layers = actions.Controller.layers.Skip(actions.isDebug ? 0 : 1).ToArray();
        var c = actions.Obj.AddComponent<ModularAvatarMergeAnimator>();
        c.animator = actions.Controller;
        c.pathMode = nadena.dev.modular_avatar.core.MergeAnimatorPathMode.Absolute;
      }
    }
  }
  #endif
}

