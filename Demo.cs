using System.Numerics;
using Leap71.ShapeKernel;
using PicoGK;

namespace CarinaLabs
{
    using PicoGK_Fasteners;

    class FastenerExample()
    {
        public static void Task()
        {
            LocalFrame oFrame = new();
            Fastener oBolt = new();
            Voxels voxBolt = oBolt.ScrewThreaded(oFrame);
            Sh.PreviewVoxels(voxBolt, Cp.clrLavender, 1);

            Fastener oHexBolt = new("Hex", 15);
            Voxels voxBolt2 = oHexBolt.ScrewBasic(
                LocalFrame.oGetTranslatedFrame(oFrame, new Vector3(10, 0, 0))
            );
            Sh.PreviewVoxels(voxBolt2, Cp.clrGreen, 1);
        }
    }
}
