#if !HAS_VF && HAS_MA
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(Me.Aonodensetsu.Stitch.StitchMAPlugin))]

namespace Me.Aonodensetsu.Stitch {
  internal class StitchMAPlugin : Plugin<StitchMAPlugin> {
    public override string DisplayName => "Stitch";
    protected override void Configure() {
      InPhase(BuildPhase.Generating).Run("Stitch", ctx => {
        Hook.RunBuild(ctx.AvatarRootObject);
      });
    }
  }
}
#endif

