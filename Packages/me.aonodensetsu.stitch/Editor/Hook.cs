using VRC.SDK3.Dynamics.Contact.Components;
using VRC.SDKBase.Editor.BuildPipeline;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

namespace Me.Aonodensetsu.Stitch {
  internal class Hook : IVRCSDKPreprocessAvatarCallback {
    #if HAS_VF
    internal static Publisher publisher = new VRCFuryPublisher();
    #elif HAS_MA
    internal static Publisher publisher = new ModularAvatarPublisher();
    #else
    internal static Publisher publisher = new InstructionPublisher();
    #endif

    public int callbackOrder => -19742;

    internal AnimationClip PromoteClip(string name, float value = 0) {
      var clip = new AnimationClip { name = $"{name}_{value}", frameRate = 60 };
      AnimationUtility.SetEditorCurve(
        clip,
        EditorCurveBinding.FloatCurve(name, typeof(Transform), "m_LocalPosition.y"),
        AnimationCurve.Constant(0, 1f / 60, value)
      );
      return clip;
    }

    public bool OnPreprocessAvatar(GameObject avatar) {
      if (publisher is InstructionPublisher p) {
        p.Publish(new Actions(avatar, null));
        return true;
      }

      var hasTimer = false;
      var hasFrametime = false;
      GameObject promoteReceiver = null;
      foreach (var (obj, actions) in avatar
        .GetComponentsInChildren<StitchMenu>(true)
        .GroupBy(c => c.gameObject)
        .Select(g => ( obj: g.Key, actions: g.SelectMany(c => c.actions) ))
      ) {
        var id = Math.Abs(obj.GetInstanceID()).ToString("X");
        var f = obj.GetComponentsInChildren<StitchMenu>().First();
        var controller = new AnimatorController();
        var act = new Actions(obj, controller);
        act.hasTimer = hasTimer;
        act.hasFrametime = hasFrametime;

        controller.AddLayer(new AnimatorControllerLayer {
          stateMachine = new AnimatorStateMachine(),
          name = $"Stitch_{id}",
          defaultWeight = 1f
        });
        act.Stitch(new DefaultAction { result = "1", value = "1" });

        controller.CreateBlendTreeInController($"Stitch_{id}", out var tree);
        tree.blendType = BlendTreeType.Direct;
        foreach (var action in actions) {
          if (!action.Validate()) {
            Debug.LogWarning($"Stitch: {Strings.Get("log.invalidAction")}", obj);
            continue;
          }
          act.Stitch((dynamic)action, tree);
        }

        if (act.hasTimer && !hasTimer) {
          var clip = new AnimationClip { name = "Stitch_Timer", frameRate = 60 };
          var curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1e5f, 1e5f));
          for (int i = 0; i < curve.length; i++) {
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
          }
          AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Animator), "Stitch_Timer"), curve);
          var sett = AnimationUtility.GetAnimationClipSettings(clip);
          sett.loopTime = true;
          AnimationUtility.SetAnimationClipSettings(clip, sett);

          var sm = new AnimatorStateMachine();
          sm.AddState("Stitch_Timer").motion = clip;
          controller.AddLayer(new AnimatorControllerLayer {
            stateMachine = sm,
            name = "Stitch_Timer",
            defaultWeight = 1f
          });
        }
        hasFrametime = act.hasFrametime;
        hasTimer = act.hasTimer;

        foreach (var action in act.promotions) {
          if (!promoteReceiver) {
            promoteReceiver = new GameObject { name = "Stitch_PromoteReceiver" };
            promoteReceiver.transform.SetParent(avatar.transform, false);
          }
          var promoteSender = new GameObject { name = $"Stitch_PromoteSender_{action.result}" };
          promoteSender.transform.SetParent(avatar.transform);
          promoteSender.transform.localPosition = new Vector3(0, 0.1f, 0);

          var pname = $"Stitch_Promote_{action.result}";
          var prm = tree.CreateBlendTreeChild(0);
          prm.blendParameter = action.value;
          prm.AddChild(PromoteClip(promoteSender.name, 0.1f));
          prm.AddChild(PromoteClip(promoteSender.name, 0.05f));

          var rec = promoteReceiver.AddComponent<VRCContactReceiver>();
          rec.radius = 0.05f;
          rec.UpdateContentTypes(VRC.Dynamics.DynamicsUsageFlags.Avatar);
          rec.allowOthers = false;
          rec.UpdateCollisionTags(new[] { pname });
          rec.receiverType = VRC.Dynamics.ContactReceiver.ReceiverType.Proximity;
          rec.parameter = action.result;

          var send = promoteSender.AddComponent<VRCContactSender>();
          send.radius = 0.05f;
          send.UpdateContentTypes(VRC.Dynamics.DynamicsUsageFlags.Avatar);
          send.UpdateCollisionTags(new[] { pname });

          if (action.type == PromoteAction.PromoteType.Synced) {
            rec.localOnly = true;
            send.localOnly = true;
          }
          rec.ApplyConfigurationChanges();
          send.ApplyConfigurationChanges();
        }

        tree.children = tree.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();

        if (act.isDebug) controller.layers[0].stateMachine.AddStateMachine($"DEBUG_prevent_DBT_merge");

        publisher.Publish(act);
      }
      return true;
    }
  }
}

