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
            DemoTools oTool = new();
            LocalFrame oOrigin = new();
            Fastener oSHCS = new();
            LocalFrame[] oFastenerFrames = new LocalFrame[iNumofExamples];
            LocalFrame[] oHoleFrames = new LocalFrame[iNumofExamples];
            Voxels voxSampleBase = new Voxels();
            voxSampleBase = oTool.Hex(
                LocalFrame.oGetRotatedFrame(oOrigin, MathF.PI / 2, new Vector3(1, 0, 0)),
                8,
                60
            );

            for (int i = 0; i < iNumofExamples; i++)
            {
                LocalFrame oRotatedFrame = LocalFrame.oGetRotatedFrame(
                    oOrigin,
                    MathF.PI * (i + 1) * 2 / iNumofExamples,
                    new Vector3(0, 1, 0)
                );

                LocalFrame oRelativeFrame = LocalFrame.oGetRelativeFrame(
                    oRotatedFrame,
                    new Vector3(0, 0, 60)
                );
                LocalFrame oRelativeFrame2 = LocalFrame.oGetRelativeFrame(
                    oRotatedFrame,
                    new Vector3(0, 0, 30)
                );
                oFastenerFrames[i] = oRelativeFrame;
                oHoleFrames[i] = oRelativeFrame2;
            }
            voxSampleBase = voxSampleBase - oSHCS.HoleClearence(oHoleFrames[0]);
            voxSampleBase = voxSampleBase - oSHCS.HoleThreaded(oHoleFrames[1]);
            voxSampleBase = voxSampleBase - oSHCS.TapDrill(oHoleFrames[2]);
            voxSampleBase = voxSampleBase - oSHCS.HoleCounterbored(oHoleFrames[3], 3);
            voxSampleBase = voxSampleBase - oSHCS.HoleCountersunk(oHoleFrames[4]);
            voxSampleBase = voxSampleBase - oSHCS.HoleClearence(oHoleFrames[5], 0, 6);
            Sh.PreviewVoxels(voxSampleBase, Cp.clrGray, 1);

            Voxels voxDemo = oSHCS.ScrewBasic(oFastenerFrames[0]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.ScrewThreaded(oFastenerFrames[1]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.ScrewBasic(oFastenerFrames[2], true);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Washer(oFastenerFrames[2]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Washer(oFastenerFrames[3]);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Nut(oFastenerFrames[4], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
            voxDemo = oSHCS.Stack(oFastenerFrames[5], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrGray, 1);
        }
    }

    class DemoTools()
    {
        float m_fHexSize;

        public Voxels Hex(LocalFrame HolePosition, float fHeight, float fRadius)
        {
            BaseCylinder oShape = new BaseCylinder(HolePosition, fHeight);
            m_fHexSize = fRadius * (MathF.Sqrt(3) / 3);
            oShape.SetRadius(new SurfaceModulation(fGetHexModulation));
            Voxels oVoxels = oShape.voxConstruct();
            return oVoxels;
        }

        public float fGetHexModulation(float fPhi, float fLengthRatio)
        {
            float fRadius = Uf.fGetPolygonRadius(fPhi, Uf.EPolygon.HEX) * m_fHexSize;
            return fRadius;
        }
    }
}
