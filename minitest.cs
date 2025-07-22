using Leap71.ShapeKernel;
using PicoGK;

namespace CarinaLabs
{
    using PicoGK_Fasteners;

    class MiniTest()
    {
        public static void Task()
        {
            LocalFrame oOrigin = new();
            Fastener ExampleFastener = new(
                Fastener.EHeadType.Button,
                Fastener.EDriver.Philips,
                4,
                10,
                1,
                "Generic test Fastener"
            );
            Voxels voxDemo = new();
            voxDemo = ExampleFastener.ScrewBasic(oOrigin);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
        }
    }
}
