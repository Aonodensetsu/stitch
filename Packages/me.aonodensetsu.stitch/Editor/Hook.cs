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

    public bool OnPreprocessAvatar(GameObject avatar) {
      if (publisher is InstructionPublisher p) {
        p.Publish(avatar);
        return true;
      }

      var hasTimer = false;
      var hasFrametime = false;
      foreach (var (obj, actions) in avatar
        .GetComponentsInChildren<StitchMenu>(true)
        .GroupBy(c => c.gameObject)
        .Select(g => ( obj: g.Key, actions: g.SelectMany(c => c.actions) ))
      ) {
        var id = Math.Abs(obj.GetInstanceID()).ToString("X");
        var f = obj.GetComponentsInChildren<StitchMenu>().First();
        var controller = new AnimatorController();
        var act = new Actions(controller);
        act.hasTimer = hasTimer;
        act.hasFrametime = hasFrametime;

        controller.AddLayer(new AnimatorControllerLayer {
          stateMachine = new AnimatorStateMachine(),
          name = $"Stitch_{id}"
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
        tree.children = tree.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();
        foreach (var param in controller.parameters) {
          if (Actions.VRCGlobals.Contains(param.name)) act.globals.Add(param.name);
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
            name = "Stitch_Timer"
          });
        }
        hasFrametime = act.hasFrametime;
        hasTimer = act.hasTimer;

        publisher.Publish(obj, controller, act.globals.Distinct().ToList());
      }
      return true;
    }
  }
}

