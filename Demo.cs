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
            int iNumofExamples = 6;
            LocalFrame oOrigin = new();
            Fastener oSHCS = new();
            Sh.PreviewVoxels(oSHCS.Nut(oOrigin, 0), Cp.clrRed, 1);
            LocalFrame[] oDemoFrames = new LocalFrame[iNumofExamples];

            for (int i = 0; i < iNumofExamples; i++)
            {
                LocalFrame oRotatedFrame = LocalFrame.oGetRotatedFrame(
                    oOrigin,
                    MathF.PI * (i + 1) * 2 / iNumofExamples,
                    new Vector3(0, 1, 0)
                );
                LocalFrame oRelativeFrame = LocalFrame.oGetRelativeFrame(
                    oRotatedFrame,
                    new Vector3(0, 0, 30)
                );
                oDemoFrames[i] = oRelativeFrame;
            }
            Voxels voxDemo = oSHCS.ScrewThreaded(oDemoFrames[0]);
            voxDemo = voxDemo + oSHCS.ScrewBasic(oDemoFrames[1]);
            voxDemo = voxDemo + oSHCS.ScrewBasic(oDemoFrames[2], true);
            voxDemo = voxDemo + oSHCS.Washer(oDemoFrames[2]);
            voxDemo = voxDemo + oSHCS.Washer(oDemoFrames[3]);
            voxDemo = voxDemo + oSHCS.Nut(oDemoFrames[4], 5);
            voxDemo = voxDemo + oSHCS.Stack(oDemoFrames[5], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
        }
    }
}
