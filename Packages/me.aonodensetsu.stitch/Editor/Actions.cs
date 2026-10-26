using System.Collections.Generic;
using UnityEditor.Animations;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

namespace Me.Aonodensetsu.Stitch {
  internal class Actions {
    private readonly Dictionary<(string name, float value), AnimationClip> clips = new();
    internal GameObject Obj;
    internal AnimatorController Controller;
    internal bool hasTimer = false;
    internal bool hasFrametime = false;
    internal bool isDebug = false;
    internal readonly HashSet<string> globals = new HashSet<string>();
    internal readonly List<PromoteAction> promotions = new List<PromoteAction>();

    internal static readonly HashSet<string> VRCGlobals = new HashSet<string> {
      "IsLocal",
      "PreviewMode",
      "Viseme",
      "Voice",
      "GestureLeft",
      "GestureRight",
      "GestureLeftWeight",
      "GestureRightWeight",
      "AngularY",
      "VelocityX",
      "VelocityY",
      "VelocityZ",
      "VelocityMagnitude",
      "Upright",
      "Grounded",
      "Seated",
      "AFK",
      "TrackingType",
      "VRMode",
      "MuteSelf",
      "InStation",
      "Earmuffs",
      "IsOnFriendsList",
      "AvatarVersion",
      "IsAnimatorEnabled",
      "ScaleModified",
      "ScaleFactor",
      "ScaleFactorInverse",
      "EyeHeightAsMeters",
      "EyeHeightAsPercent"
    };

    internal Actions(GameObject obj, AnimatorController controller) {
      Obj = obj;
      Controller = controller;
    }

    internal string InternParam() => $"StitchInternal_{Guid.NewGuid().ToString("N")}";

    internal AnimationClip EmptyClip() {
      var key = ("", 0);
      if (clips.TryGetValue(key, out var clip)) return clip;
      clip = new AnimationClip { name = "Stitch_Empty", frameRate = 60 };
      clips.Add(key, clip);
      return clip;
    }

    internal AnimationClip AClip(string name, float value = 0) {
      var key = (name, value);
      if (clips.TryGetValue(key, out var clip)) return clip;
      clip = new AnimationClip { name = $"{name}_{value}", frameRate = 60 };
      AnimationUtility.SetEditorCurve(
        clip,
        EditorCurveBinding.FloatCurve("", typeof(Animator), name),
        AnimationCurve.Constant(0, 1f / 60, value)
      );
      clips.Add(key, clip);
      return clip;
    }

    internal void MakeParameters(string[] s) {
      foreach (var param in s) {
        if (!Controller.parameters.Any(p => p.name == param)) {
          Controller.AddParameter(new AnimatorControllerParameter {
            name = param,
            type = AnimatorControllerParameterType.Float,
            defaultFloat = float.TryParse(param, out float val) ? val : 0
          });
          if (VRCGlobals.Contains(param)) globals.Add(param);
        }
      }
    }

    internal BlendTree TreeGroup(Action<BlendTree> body, BlendTree parent) {
      var wrap = parent.CreateBlendTreeChild(0);
      wrap.blendType = BlendTreeType.Direct;
      body(wrap);
      wrap.children = wrap.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();
      return wrap;
    }

    internal BlendTree Stitch(AbsoluteAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.value });

      var abs = parent.CreateBlendTreeChild(0);
      abs.name = $"{a.result} = Absolute {a.value}";
      abs.blendParameter = a.value;
      abs.minThreshold = -100;
      abs.maxThreshold = 100;

      var hundred = AClip(a.result, 100);
      abs.AddChild(hundred);
      abs.AddChild(AClip(a.result));
      abs.AddChild(hundred);
      return abs;
    }

    internal BlendTree Stitch(AddAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var plus = parent.CreateBlendTreeChild(0);
      plus.name = $"{a.result} = {a.left} + {a.right}";
      plus.blendType = BlendTreeType.FreeformCartesian2D;
      plus.blendParameter = a.left;
      plus.blendParameterY = a.right;

      plus.AddChild(AClip(a.result, -200), new Vector2(-100, -100));
      plus.AddChild(AClip(a.result, 200), new Vector2(100, 100));
      return plus;
    }

    internal BlendTree Stitch(AndAction a, BlendTree parent) {
      var and = Stitch(new GateAction {
        result = a.result,
        left = a.left,
        right = a.right,
        zeroZero = 0,
        zeroOne = 0,
        oneZero = 0,
        oneOne = 1
      }, parent);
      and.name = $"{a.result} = {a.left} AND {a.right}";
      return and;
    }

    internal BlendTree Stitch(CompareAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var cmp = parent.CreateBlendTreeChild(0);
      cmp.blendType = BlendTreeType.FreeformCartesian2D;
      cmp.blendParameter = a.left;
      cmp.blendParameterY = a.right;

      var zero = AClip(a.result);
      var one = AClip(a.result, 1);
      switch (a.type) {
        case CompareAction.CompareType.MoreThan:
          cmp.name = $"{a.result} = {a.left} > {a.right}";
          cmp.AddChild(zero, new Vector2(0, 0));
          cmp.AddChild(one, new Vector2(1e-5f, -1e-5f));
          break;
        case CompareAction.CompareType.MoreEqual:
          cmp.name = $"{a.result} = {a.left} >= {a.right}";
          cmp.AddChild(zero, new Vector2(-1e-5f, 1e-5f));
          cmp.AddChild(one, new Vector2(0, 0));
          break;
        case CompareAction.CompareType.LessThan:
          cmp.name = $"{a.result} = {a.left} < {a.right}";
          cmp.AddChild(one, new Vector2(-1e-5f, 1e-5f));
          cmp.AddChild(zero, new Vector2(0, 0));
          break;
        case CompareAction.CompareType.LessEqual:
          cmp.name = $"{a.result} = {a.left} <= {a.right}";
          cmp.AddChild(one, new Vector2(0, 0));
          cmp.AddChild(zero, new Vector2(1e-5f, -1e-5f));
          break;
        case CompareAction.CompareType.Equal:
          cmp.name = $"{a.result} = {a.left} == {a.right}";
          cmp.AddChild(zero, new Vector2(-1e-5f, 1e-5f));
          cmp.AddChild(one, new Vector2(0, 0));
          cmp.AddChild(zero, new Vector2(1e-5f, -1e-5f));
          break;
        case CompareAction.CompareType.Inequal:
          cmp.name = $"{a.result} = {a.left} != {a.right}";
          cmp.AddChild(one, new Vector2(-1e-5f, 1e-5f));
          cmp.AddChild(zero, new Vector2(0, 0));
          cmp.AddChild(one, new Vector2(1e-5f, -1e-5f));
          break;
      }
      return cmp;
    }

    internal BlendTree Stitch(CopyAction a, BlendTree parent) {
      var cp = Stitch(new RemapAction {
        result = a.result,
        value = a.value,
        lowIn = -100,
        highIn = 100,
        lowOut = -100,
        highOut = 100
      }, parent);
      cp.name = $"{a.result} = Copy {a.value}";
      return cp;
    }

    internal BlendTree Stitch(DebugAction a, BlendTree parent) {
      Stitch(new DefaultAction { result = a.result, value = JObject.Parse(File.ReadAllText(Path.GetFullPath("Packages/me.aonodensetsu.stitch/package.json")))["version"].ToString().Replace(".", "") });
      Stitch(new GlobalAction { result = a.result });
      isDebug = true;
      return null;
    }

    internal BlendTree Stitch(DefaultAction a, BlendTree parent = null) {
      MakeParameters(new[] { a.result });
      var parameters = Controller.parameters;
      var parameter = parameters.FirstOrDefault(p => p.name == a.result);
      float.TryParse(a.value, out var af);
      parameter.defaultFloat = af;
      Controller.parameters = parameters;
      return null;
    }

    internal BlendTree Stitch(DivideAction a, BlendTree parent) {
      var rec = InternParam();
      return TreeGroup(t => {
        t.name = $"{a.result} = {a.left} / {a.right}";
        Stitch(new ReciprocalAction { result = rec, value = a.right }, t);
        Stitch(new MultiplyAction { result = a.result, left = a.left, right = rec }, t);
      }, parent);
    }

    internal BlendTree Stitch(DriveAction a, BlendTree parent) {
      if (DriveActionDrawer.IsConstantClip(a.clip)) return null;

      MakeParameters(new[] { a.result });

      var binds = AnimationUtility.GetCurveBindings(a.clip)
        .Select(bind => (
          bind,
          curve: AnimationUtility.GetEditorCurve(a.clip, bind)
        ))
        .Where(x => x.curve != null)
        .ToArray();
      var frames = binds.SelectMany(b => b.curve.keys)
        .Select(k => Mathf.RoundToInt(k.time * a.clip.frameRate))
        .Distinct()
        .OrderBy(f => f)
        .ToArray();

      var drv = parent.CreateBlendTreeChild(0);
      drv.name = $"{a.result} Drive {a.clip.name}";
      drv.blendParameter = a.result;
      drv.useAutomaticThresholds = false;
      drv.minThreshold = a.min;
      drv.maxThreshold = a.max;

      foreach (var frame in frames) {
        var clip = new AnimationClip { name = $"{a.clip.name}_{frame}", frameRate = 60 };
        foreach (var (bind, curve) in binds) AnimationUtility.SetEditorCurve(clip, bind, AnimationCurve.Constant(0, 1f / 60, curve.Evaluate(frame / a.clip.frameRate)));
        drv.AddChild(clip, a.min + frame * (a.max - a.min) / (a.clip.frameRate * a.clip.length));
      }

      return drv;
    }

    internal BlendTree Stitch(FrametimeAction a, BlendTree parent) {
      if (hasFrametime) return null;
      hasFrametime = true;
      Stitch(new GlobalAction { result = a.result });
      Stitch(new GlobalAction { result = "Stitch_Timer_Prev" });
      Stitch(new TimerAction());

      return TreeGroup(t => {
        t.name = "Stitch Frametime";
        Stitch(new CopyAction { result = "Stitch_Timer_Prev", value = "Stitch_Timer" }, t);
        Stitch(new SubtractAction {
          result = a.result,
          left = "Stitch_Timer",
          right = "Stitch_Timer_Prev"
        }, t);
      }, parent);
    }

    internal BlendTree Stitch(GateAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var gate = parent.CreateBlendTreeChild(0);
      gate.name = $"{a.result} = Gate {a.left} {a.right}";
      gate.blendType = BlendTreeType.FreeformCartesian2D;
      gate.blendParameter = a.left;
      gate.blendParameterY = a.right;

      gate.AddChild(AClip(a.result, a.zeroZero), new Vector2(0, 0));
      gate.AddChild(AClip(a.result, a.zeroOne), new Vector2(0, 1));
      gate.AddChild(AClip(a.result, a.oneZero), new Vector2(1, 0));
      gate.AddChild(AClip(a.result, a.oneOne), new Vector2(1, 1));
      return gate;
    }

    internal BlendTree Stitch(GlobalAction a, BlendTree parent = null) {
      MakeParameters(new[] { a.result });
      globals.Add(a.result);
      return null;
    }

    internal BlendTree Stitch(MaximumAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var negative = AClip(a.result, -100);
      var positive = AClip(a.result, 100);

      var max = parent.CreateBlendTreeChild(0);
      max.name = $"{a.result} = Maximum {a.left} {a.right}";
      max.blendType = BlendTreeType.FreeformCartesian2D;
      max.blendParameter = a.left;
      max.blendParameterY = a.right;

      Stitch(new CopyAction { result = a.result, value = a.right }, max);
      Stitch(new CopyAction { result = a.result, value = a.left }, max);

      var ch = max.children;
      ch[0].position = new Vector2(-1e-5f, 1e-5f);
      ch[1].position = new Vector2(0, 0);
      max.children = ch;

      return max;
    }

    internal BlendTree Stitch(MeanAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var plus = parent.CreateBlendTreeChild(0);
      plus.name = $"{a.result} = Mean {a.left} {a.right}";
      plus.blendType = BlendTreeType.FreeformCartesian2D;
      plus.blendParameter = a.left;
      plus.blendParameterY = a.right;

      plus.AddChild(AClip(a.result, -100), new Vector2(-100, -100));
      plus.AddChild(AClip(a.result, 100), new Vector2(100, 100));
      return plus;
    }

    internal BlendTree Stitch(MinimumAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var negative = AClip(a.result, -100);
      var positive = AClip(a.result, 100);

      var min = parent.CreateBlendTreeChild(0);
      min.name = $"{a.result} = Minimum {a.left} {a.right}";
      min.blendType = BlendTreeType.FreeformCartesian2D;
      min.blendParameter = a.left;
      min.blendParameterY = a.right;

      Stitch(new CopyAction { result = a.result, value = a.left }, min);
      Stitch(new CopyAction { result = a.result, value = a.right }, min);

      var ch = min.children;
      ch[0].position = new Vector2(-1e-5f, 1e-5f);
      ch[1].position = new Vector2(0, 0);
      min.children = ch;

      return min;
    }

    internal BlendTree Stitch(MonostableAction a, BlendTree parent) {
      return TreeGroup(t => {
        t.name = $"{a.result} = Monostable {a.value}";
        var prev = InternParam();
        Stitch(new CopyAction { result = prev, value = a.value }, t);
        var ms = Stitch(new CompareAction {
          result = a.result,
          left = a.value,
          right = prev,
          type = CompareAction.CompareType.Inequal
        }, t);
      }, parent);
    }

    internal BlendTree Stitch(MultiplyAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });
      var negative = AClip(a.result, -10000);
      var positive = AClip(a.result, 10000);

      var mul = parent.CreateBlendTreeChild(0);
      mul.name = $"{a.result} = {a.left} * {a.right}";
      mul.blendParameter = a.left;
      mul.minThreshold = -100;
      mul.maxThreshold = 100;

      var secondary = mul.CreateBlendTreeChild(0);
      secondary.blendType = BlendTreeType.FreeformCartesian2D;
      secondary.blendParameter = a.left;
      secondary.blendParameterY = a.right;
      secondary.AddChild(positive, new Vector2(-1e-5f, -100));
      secondary.AddChild(negative, new Vector2(-1e-5f, 100));
      secondary.AddChild(negative, new Vector2(0, -100));
      secondary.AddChild(positive, new Vector2(0, 100));

      mul.AddChild(AClip(a.result));
      mul.AddChild(secondary);

      return mul;
    }

    internal BlendTree Stitch(NotAction a, BlendTree parent) {
      var not = Stitch(new RemapAction {
        result = a.result,
        value = a.value,
        lowIn = 0,
        highIn = 1,
        lowOut = 1,
        highOut = 0
      }, parent);
      not.name = $"{a.result} = NOT {a.value}";
      return not;
    }

    internal BlendTree Stitch(OrAction a, BlendTree parent) {
      var or = Stitch(new GateAction {
        result = a.result,
        left = a.left,
        right = a.right,
        zeroZero = 0,
        zeroOne = 1,
        oneZero = 1,
        oneOne = 1
      }, parent);
      or.name = $"{a.result} = {a.left} OR {a.right}";
      return or;
    }

    internal BlendTree Stitch(PromoteAction a, BlendTree parent = null) {
      MakeParameters(new[] { a.result, a.value });
      Stitch(new GlobalAction { result = a.result });
      promotions.Add(a);
      return null;
    }

    internal BlendTree Stitch(ReciprocalAction a, BlendTree parent) {
      var af = InternParam();
      MakeParameters(new[] { a.result, a.value, af });

      return TreeGroup(t => {
        t.name = $"{a.result} = 1 / {a.value}";

        var aft = t.CreateBlendTreeChild(0);
        aft.name = $"{af} = 100 * {a.value} - 1";
        aft.blendParameter = a.value;
        aft.minThreshold = 0.01f;
        aft.maxThreshold = 100;
        aft.AddChild(AClip(af, 0));
        aft.AddChild(AClip(af, 9999));

        var r2 = parent.CreateBlendTreeChild(0);
        r2.name = $"{a.result} = 100 / (1 + {af})";
        r2.blendType = BlendTreeType.Direct;
        using (var so = new SerializedObject(r2)) {
          so.FindProperty("m_NormalizedBlendValues").boolValue = true;  // normally inaccessible
          so.ApplyModifiedProperties();
        }
        r2.AddChild(EmptyClip());
        r2.AddChild(AClip(a.result, 100));

        var ch = r2.children;
        ch[0].directBlendParameter = af;
        ch[1].directBlendParameter = "1";
        r2.children = ch;
      }, parent);
    }

    internal BlendTree Stitch(RemapAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.value });

      var remap = parent.CreateBlendTreeChild(0);
      remap.name = $"{a.result} = Remap {a.value} ({a.lowIn}-{a.highIn} -> {a.lowOut}-{a.highOut})";
      remap.blendParameter = a.value;
      remap.minThreshold = a.lowIn;
      remap.maxThreshold = a.highIn;
      remap.AddChild(AClip(a.result, a.lowOut));
      remap.AddChild(AClip(a.result, a.highOut));
      return remap;
    }

    internal BlendTree Stitch(SelectAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.middle, a.right });
      var negative = AClip(a.result, -100);
      var positive = AClip(a.result, 100);

      var sel = parent.CreateBlendTreeChild(0);
      sel.name = $"{a.result} = {a.left} Select {a.middle} {a.right}";
      sel.blendParameter = a.left;

      Stitch(new CopyAction { result = a.result, value = a.middle }, sel);
      Stitch(new CopyAction { result = a.result, value = a.right }, sel);

      return sel;
    }

    internal BlendTree Stitch(SmoothAction a, BlendTree parent) {
      // TODO: try to optimize
      var delta = a.delta.ToString();
      MakeParameters(new[] { a.result, a.value, delta });

      var negative = AClip(a.result, -100);
      var positive = AClip(a.result, 100);

      var smooth = parent.CreateBlendTreeChild(0);

      var secondary = smooth.CreateBlendTreeChild(0);
      secondary.minThreshold = -100;
      secondary.maxThreshold = 100;
      secondary.blendParameter = a.value;

      var tertiary = smooth.CreateBlendTreeChild(0);
      tertiary.minThreshold = -100;
      tertiary.maxThreshold = 100;
      tertiary.blendParameter = a.result;

      switch (a.type) {
        case SmoothAction.SmoothType.Exponential:
          smooth.name = $"{a.result} = Smooth {a.value} EXP {a.delta}";
          smooth.blendParameter = delta;

          secondary.AddChild(negative);
          secondary.AddChild(positive);

          tertiary.AddChild(negative);
          tertiary.AddChild(positive);
          break;
        case SmoothAction.SmoothType.Linear:
          smooth.name = $"{a.result} = Smooth {a.value} LIN {a.delta}";
          smooth.blendType = BlendTreeType.Direct;

          var r = InternParam();
          MakeParameters(new[] { r });
          var negativer = AClip(r, -100);
          var positiver = AClip(r, 100);

          secondary.AddChild(negativer);
          secondary.AddChild(positiver);

          tertiary.AddChild(positiver);
          tertiary.AddChild(negativer);

          var quarternary = smooth.CreateBlendTreeChild(0);
          quarternary.minThreshold = -100;
          quarternary.maxThreshold = 100;
          quarternary.blendParameter = a.result;
          quarternary.AddChild(negative);
          quarternary.AddChild(positive);

          var quinary = smooth.CreateBlendTreeChild(0);
          quinary.minThreshold = -0.1f;
          quinary.maxThreshold = 0.1f;
          quinary.blendParameter = r;
          quinary.AddChild(AClip(a.result, -1));
          quinary.AddChild(AClip(a.result, 0));
          quinary.AddChild(AClip(a.result, 1));

          smooth.children = smooth.children.Select((c, ix) => { c.directBlendParameter = ix == 3 ? delta : "1"; return c; }).ToArray();
          break;
      }
      return smooth;
    }

    internal BlendTree Stitch(SubtractAction a, BlendTree parent) {
      MakeParameters(new[] { a.result, a.left, a.right });

      var sub = parent.CreateBlendTreeChild(0);
      sub.name = $"{a.result} = {a.left} - {a.right}";
      sub.blendType = BlendTreeType.FreeformCartesian2D;
      sub.blendParameter = a.left;
      sub.blendParameterY = a.right;

      sub.AddChild(AClip(a.result, -200), new Vector2(-100, 100));
      sub.AddChild(AClip(a.result, 200), new Vector2(100, -100));
      return sub;
    }

    internal BlendTree Stitch(TimerAction a, BlendTree parent = null) {
      if (hasTimer) return null;
      hasTimer = true;
      Stitch(new GlobalAction { result = a.result });
      return null;
    }
  }
}

