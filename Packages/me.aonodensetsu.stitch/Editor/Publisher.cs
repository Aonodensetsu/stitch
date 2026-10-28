using VRC.SDK3.Avatars.ScriptableObjects;
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

      foreach (var i in actions.actions.OfType<InputAction>()) {
        if (i.menu) fc.AddMenu(i.menu, i.result);
        if (i.param) fc.AddParams(i.param);
      }

      var promotions = actions.actions.OfType<PromoteAction>();
      if (promotions.Any(a => a.type == PromoteAction.PromoteType.Synced)) fc.AddParams(new VRCExpressionParameters {
        parameters = promotions.Where(a => a.type == PromoteAction.PromoteType.Synced).Select(e => new VRCExpressionParameters.Parameter {
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
      var param = actions.Obj.AddComponent<ModularAvatarParameters>();

      var promotions = actions.actions.OfType<PromoteAction>();
      param.parameters.AddRange(
        actions.Controller.parameters.Select(p => new ParameterConfig {
          nameOrPrefix = p.name,
          isPrefix = false,
          defaultValue = p.defaultFloat,
          syncType = promotions.Where(a => a.type == PromoteAction.PromoteType.Synced).Select(e => e.result).Contains(p.name) ? ParameterSyncType.Float : ParameterSyncType.NotSynced,
          internalParameter = !actions.globals.Contains(p.name),
          localOnly = !promotions.Select(x => x.result).Contains(p.name),
          saved = false
        })
      );

      foreach (var i in actions.actions.OfType<InputAction>()) {
        if (i.menu) {
          var menu = i.menu;
          foreach (var path in i.result.Split("/").Reverse()) {
            menu = new VRCExpressionsMenu {
              controls = new List<VRCExpressionsMenu.Control> {
                new VRCExpressionsMenu.Control {
                  name = path,
                  type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                  subMenu = menu
                }
              }
            };
          }
          actions.Obj.AddComponent<ModularAvatarMenuInstaller>().menuToAppend = menu;
        }
        if (i.param) {
          param.parameters.AddRange(i.param.parameters.Select(p => new ParameterConfig {
            nameOrPrefix = p.name,
            isPrefix = false,
            defaultValue = p.defaultValue,
            syncType = p.networkSynced ? p.valueType switch {
              VRCExpressionParameters.ValueType.Int => ParameterSyncType.Int,
              VRCExpressionParameters.ValueType.Float => ParameterSyncType.Float,
              VRCExpressionParameters.ValueType.Bool => ParameterSyncType.Bool,
              _ => ParameterSyncType.NotSynced
            } : ParameterSyncType.NotSynced,
            internalParameter = !actions.globals.Contains(p.name),
            localOnly = !p.networkSynced,
            saved = p.saved
          }));
        }
      }

      if (!actions.isDebug) {
        var t = actions.Obj.AddComponent<ModularAvatarMergeBlendTree>();
        t.Motion = actions.Controller.layers[0].stateMachine.states[0].state.motion;
        t.PathMode = MergeAnimatorPathMode.Absolute;
      }
      if (actions.isDebug || actions.Controller.layers.Length > 1) {
        actions.Controller.layers = actions.Controller.layers.Skip(actions.isDebug ? 0 : 1).ToArray();
        var c = actions.Obj.AddComponent<ModularAvatarMergeAnimator>();
        c.animator = actions.Controller;
        c.pathMode = MergeAnimatorPathMode.Absolute;
      }
    }
  }
  #endif
}

