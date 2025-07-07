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
            Fastener ExampleFastener = new(
                Fastener.EHeadType.Button,
                Fastener.EDriver.Hex,
                4,
                10,
                1,
                "Generic test Fastener"
            );
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

                LocalFrame oRelativeFrame = Fastener.FrameOffset(
                    oRotatedFrame,
                    new Vector3(0, 0, 60)
                );
                LocalFrame oRelativeFrame2 = Fastener.FrameOffset(
                    oRotatedFrame,
                    new Vector3(0, 0, 30)
                );
                oFastenerFrames[i] = oRelativeFrame;
                oHoleFrames[i] = oRelativeFrame2;
            }
            voxSampleBase = voxSampleBase - ExampleFastener.HoleClearence(oHoleFrames[0]);
            voxSampleBase = voxSampleBase - ExampleFastener.HoleThreaded(oHoleFrames[1]);
            voxSampleBase = voxSampleBase - ExampleFastener.TapDrill(oHoleFrames[2]);
            voxSampleBase =
                voxSampleBase
                - ExampleFastener.HoleCounterbored(
                    Fastener.FrameOffset(oHoleFrames[3], new Vector3(0, 0, -3)),
                    3
                ); //offset so that counterbore can be shown
            voxSampleBase = voxSampleBase - ExampleFastener.HoleCountersunk(oHoleFrames[4]);
            voxSampleBase = voxSampleBase - ExampleFastener.HoleClearence(oHoleFrames[5], 0, 6);
            Sh.PreviewVoxels(voxSampleBase, Cp.clrRock, 1);

            Voxels voxDemo = ExampleFastener.ScrewBasic(oFastenerFrames[0]);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
            voxDemo = ExampleFastener.ScrewThreaded(oFastenerFrames[1]);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
            voxDemo = ExampleFastener.ScrewBasic(oFastenerFrames[2], true);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
            voxDemo = ExampleFastener.Washer(oFastenerFrames[2]);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
            voxDemo = ExampleFastener.Washer(oFastenerFrames[3]);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
            voxDemo = ExampleFastener.Nut(oFastenerFrames[4], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
            voxDemo = ExampleFastener.Stack(oFastenerFrames[5], 5);
            Sh.PreviewVoxels(voxDemo, Cp.clrBlue, 1);
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
