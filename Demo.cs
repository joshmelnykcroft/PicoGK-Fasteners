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
            Voxels voxDemo = oSHCS.ScrewBasic(oDemoFrames[0]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.ScrewThreaded(oDemoFrames[1]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.ScrewBasic(oDemoFrames[2], true);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Washer(oDemoFrames[2]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Washer(oDemoFrames[3]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Nut(oDemoFrames[4], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Stack(oDemoFrames[5], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
        }
    }
}
