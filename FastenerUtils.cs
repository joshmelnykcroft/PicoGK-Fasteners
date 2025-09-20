using System.Numerics;
using Leap71.ShapeKernel;
using PicoGK;

namespace PicoGK_Fasteners
{
    public partial class Fastener
    {
        // this function defines a hex shaped modulation using the shape ShapeKernel polygon utility functions.
        private float fGetHexModulation(float fPhi, float fLengthRatio)
        {
            float fRadius = Uf.fGetPolygonRadius(fPhi, Uf.EPolygon.HEX) * m_fHexSize;
            return fRadius;
        }

        // This is a workaround method, will get removed if shapekernel gets updated with something that replaces it.
        public static LocalFrame FrameOffset(LocalFrame SourceFrame, Vector3 vecLocalOffset)
        {
            Vector3 vecRefX = SourceFrame.vecGetLocalX();
            Vector3 vecRefY = SourceFrame.vecGetLocalY();
            Vector3 vecRefZ = SourceFrame.vecGetLocalZ();

            Vector3 vecGlobalOffset =
                vecRefX * vecLocalOffset.X
                + vecRefY * vecLocalOffset.Y
                + vecRefZ * vecLocalOffset.Z;

            Vector3 vecNewPosition = SourceFrame.vecGetPosition() + vecGlobalOffset;

            return new LocalFrame(
                vecNewPosition,
                SourceFrame.vecGetLocalZ(),
                SourceFrame.vecGetLocalX()
            );
        }

        //This method flattens a sphere by a given ratio
        private float fGetSphereRadius(float fPhi, float fTheta)
        {
            float fRatio = 0.5f;
            float fRadius =
                (fRatio * (m_fFlattenedSphereDiameter / 2))
                / (
                    MathF.Sqrt(
                        (MathF.Pow(fRatio, 2) * (MathF.Pow(MathF.Sin(fTheta), 2)))
                            + MathF.Pow(MathF.Cos(fTheta), 2)
                    )
                );
            return fRadius;
        }

        private Voxels FlattenedSphere(LocalFrame HolePosition, float Diameter)
        {
            BaseSphere oShape = new BaseSphere(HolePosition, Diameter / 2);
            m_fFlattenedSphereDiameter = Diameter;
            oShape.SetRadius(new SurfaceModulation(fGetSphereRadius));
            Voxels oVoxels = oShape.voxConstruct();
            return oVoxels;
        }

        private Voxels DrillTip(LocalFrame HolePosition, float fDrillRadius)
        {
            BaseCone oDrillTip = new BaseCone(
                HolePosition,
                -fDrillRadius * (float)Math.Tan(41 * Math.PI / 180),
                fDrillRadius,
                0.0001f
            );
            return oDrillTip.voxConstruct();
        }

        private Voxels Hex(LocalFrame HolePosition, float fHeight, float fRadius)
        {
            BaseCylinder oShape = new BaseCylinder(HolePosition, fHeight);
            m_fHexSize = fRadius;
            oShape.SetRadius(new SurfaceModulation(fGetHexModulation));
            Voxels oVoxels = oShape.voxConstruct();
            return oVoxels;
        }

        private Voxels ScrewHead(LocalFrame HolePosition)
        {
            Voxels oScrewHead = new Voxels();
            switch (m_eHeadType)
            {
                case EHeadType.Hex:
                    oScrewHead = Hex(
                        HolePosition,
                        m_fHeadHeight,
                        (m_fHeadDiameter * MathF.Sqrt(3)) / 3
                    );
                    break;
                case EHeadType.Countersunk:
                    m_fHeadHeightModifier = -m_fHeadHeight;
                    float fHeight =
                        ((m_fHeadDiameter / 2) - (m_fHeadDiameter * .0001f))
                        * MathF.Tan(41 * MathF.PI / 180); // Currently defaults to an 82 degree countersink
                    BaseCone oCSHead = new BaseCone(
                        HolePosition,
                        -fHeight,
                        m_fHeadDiameter / 2,
                        m_fHeadDiameter * .0001f
                    );
                    oScrewHead = oCSHead.voxConstruct();
                    break;
                case EHeadType.Button:
                    float fEdgeHeight = 0.05f * m_fHeadDiameter;
                    m_fHeadHeightModifier = fEdgeHeight - m_fHeadHeight / 2;
                    BaseCylinder oHeadEdge = new(HolePosition, fEdgeHeight, m_fHeadDiameter / 2);
                    BaseCylinder oTrim = new(HolePosition, -m_fHeadDiameter, m_fHeadDiameter / 2);
                    LocalFrame oButtonFrame = FrameOffset(
                        HolePosition,
                        new Vector3(0, 0, fEdgeHeight)
                    );
                    oScrewHead =
                        oHeadEdge.voxConstruct()
                        + FlattenedSphere(oButtonFrame, m_fHeadDiameter)
                        - oTrim.voxConstruct();
                    break;
                case EHeadType.SHCS:
                    BaseCylinder oSHCSBody = new BaseCylinder(
                        HolePosition,
                        m_fHeadHeight,
                        m_fHeadDiameter / 2
                    );
                    oScrewHead = oSHCSBody.voxConstruct();

                    break;
            }
            return oScrewHead;
        }

        private Voxels Driver(LocalFrame HolePosition) //TODO: These need draft angles
        {
            Voxels oDriver = new Voxels();
            LocalFrame oTopofHead = FrameOffset(
                HolePosition,
                new Vector3(0, 0, m_fHeadHeight + m_fHeadHeightModifier)
            );
            switch (m_eDriver)
            {
                case EDriver.Hex:
                {
                    oDriver = Hex(oTopofHead, -m_fDriverDepth, (m_fDriverSize * MathF.Sqrt(3)) / 3);
                    break;
                }
                case EDriver.Philips:
                {
                    //box, subtract flattened sphere to make cup. cross boxes, cut with cup.
                    LocalFrame oCupFrame = FrameOffset(
                        oTopofHead,
                        new Vector3(0, 0, -m_fDriverDepth / 2)
                    );
                    BaseBox oCup = new BaseBox(
                        oCupFrame,
                        -m_fDriverDepth,
                        m_fDriverSize,
                        m_fDriverSize
                    );
                    Voxels voxCup = oCup.voxConstruct() - FlattenedSphere(oCupFrame, m_fDriverSize);

                    BaseBox oDriverCross1 = new BaseBox(
                        oTopofHead,
                        -m_fDriverDepth,
                        m_fDriverSize,
                        m_fDriverSize / 6
                    );
                    BaseBox oDriverCross2 = new BaseBox(
                        oTopofHead,
                        -m_fDriverDepth,
                        m_fDriverSize / 6,
                        m_fDriverSize
                    );
                    oDriver = oDriverCross2.voxConstruct() + oDriverCross1.voxConstruct() - voxCup;
                    break;
                }
                case EDriver.Robinson:
                {
                    BaseBox oRobinson = new BaseBox(
                        oTopofHead,
                        -m_fDriverDepth,
                        m_fDriverSize,
                        m_fDriverSize
                    );
                    oDriver = oRobinson.voxConstruct();
                    break;
                }
                case EDriver.None:
                {
                    return oDriver;
                }
            }
            return oDriver;
        }

        ///<summary>
        ///This creates body with the minor diameter of the thread chosen. Takes a position from a LocalFrame.
        ///</summary>
        private Voxels MinorBody(LocalFrame HolePosition)
        {
            BaseCylinder oMinorBody = new BaseCylinder(
                HolePosition,
                -m_fLength,
                m_fThreadMinor / 2
            );
            return oMinorBody.voxConstruct();
        }

        ///<summary>
        ///This creates body with the major diameter of the thread chosen. Takes a position from a LocalFrame.
        ///</summary>
        private Voxels MajorBody(LocalFrame HolePosition)
        {
            BaseCylinder oMajorBody = new BaseCylinder(HolePosition, -m_fLength, m_fSize / 2);
            return oMajorBody.voxConstruct();
        }

        ///<summary>
        ///This creates threads based on the size of the screw. should only be used for graphical/cosmetic
        ///representations, use a tap or die when manufacturing along with the tapdrill method.
        ///Takes a position from a LocalFrame.
        ///</summary>
        private Voxels Threads(LocalFrame HolePosition, float Length)
        {
            float fTurns = (Length / m_fThreadPitch) + m_fThreadPitch;

            float fBeam1 = 0.5f * m_fThreadPitch;
            float fBeam2 = 0.1f;
            Lattice oLattice = new Lattice();
            for (float fPhi = 0; fPhi <= fTurns * 2f * MathF.PI; fPhi += 0.005f)
            {
                float dS = fPhi / (2f * MathF.PI) * -m_fThreadPitch;
                Vector3 vecRel1 = VecOperations.vecGetCylPoint(m_fThreadMinor / 2, fPhi, dS);
                Vector3 vecRel2 = VecOperations.vecGetCylPoint(m_fSize / 2, fPhi, dS);
                Vector3 vecPt1 = VecOperations.vecTranslatePointOntoFrame(
                    FrameOffset(HolePosition, new Vector3(0, 0, m_fThreadPitch)),
                    vecRel1
                );
                Vector3 vecPt2 = VecOperations.vecTranslatePointOntoFrame(
                    FrameOffset(HolePosition, new Vector3(0, 0, m_fThreadPitch)),
                    vecRel2
                );
                oLattice.AddBeam(vecPt1, fBeam1, vecPt2, fBeam2, false);
            }
            Voxels oThreads = new Voxels(oLattice);

            BaseCylinder oTrimbody = new(HolePosition, 2 * m_fThreadPitch, m_fSize);

            return oThreads - oTrimbody.voxConstruct();
        }

        private Voxels EndChamfer(LocalFrame HolePosition)
        {
            Vector3 vecOffsetFromEnd = new Vector3(0, 0, -m_fLength + (m_fSize / 6));
            LocalFrame oOffsetFromEnd = FrameOffset(HolePosition, vecOffsetFromEnd);
            BaseCylinder oBody = new(oOffsetFromEnd, -m_fSize, m_fSize);
            BaseCone oCone = new(oOffsetFromEnd, -m_fSize / 2, m_fSize / 2, .001f);
            return oBody.voxConstruct() - oCone.voxConstruct();
        }
    }
}
