using System.Numerics;
using Leap71.ShapeKernel;
using PicoGK;

namespace PicoGK_Fasteners
{
    public class Fastener
    {
        //primary characteristics

        public enum EHeadType
        {
            Hex,
            Countersunk,
            Button,
            SHCS,
        };

        public enum EDriver
        {
            None,
            Hex,
            Philips,
            Robinson,
        };

        protected EHeadType m_eHeadType;
        protected EDriver m_eDriver;
        protected string m_sDescription;
        protected float m_fSize;
        protected float m_fLength;
        protected float m_fThreadPitch;

        // Derived characteristics
        protected float m_fTapSize;
        protected float m_fThreadMinor;
        protected float m_fLoosefit;
        protected float m_fClosefit;
        protected float m_fNormalfit;

        //generic characteristics
        protected float m_fHeadDiameter;
        protected float m_fHeadHeight;
        protected float m_fBoreDiameter;
        protected float m_fDriverDepth;
        protected float m_fDriverSize;
        protected float m_fWasherDiameter;
        protected float m_fWasherThickness;
        protected float m_fNutHeight;
        protected float m_fNutSize;

        //counts for BOM TODO:check if necessary
        protected int m_iCountFasteners;
        protected int m_iCountNuts;
        protected int m_iCountWashers;

        ///<summary>
        ///This method defines the fastener via custom inputs. Defaults to M5X10 SHCS .
        ///Use this constructor if you have a fastener that you can measure, or if you
        ///have accesses to the standards that your desired fastener is based on.
        ///Any measurements of a Hex is done across the flats.
        ///</summary>
        public Fastener(
            string sDiscription = "M5X10 SHCS, Example Fastener",
            EHeadType eHeadType = EHeadType.SHCS,
            EDriver eDriver = EDriver.Hex,
            float fLength = 10,
            float fThreadPitch = 0.8f,
            float fThreadMajor = 5,
            float fThreadMinor = 4.134f,
            float fHeadHeight = 5,
            float fHeadDiameter = 8.72f,
            float fDriverDepth = 2.5f,
            float fDriverSize = 4,
            float fTapSize = 5,
            float fLooseFit = 5.8f,
            float fCloseFit = 5.3f,
            float fNormalfit = 5.5f,
            float fBoreDiameter = 9.75f,
            float fWasherDiameter = 15,
            float fWasherThickness = 1.2f,
            float fNutHeight = 4,
            float fNutSize = 8.79f
        )
        {
            m_sDescription = sDiscription;
            m_eHeadType = eHeadType;
            m_eDriver = eDriver;
            m_fLength = fLength;
            m_fTapSize = fTapSize;
            m_fThreadPitch = fThreadPitch;
            m_fThreadMinor = fThreadMinor;
            m_fSize = fThreadMajor;
            m_fLoosefit = fLooseFit;
            m_fClosefit = fCloseFit;
            m_fNormalfit = fNormalfit;
            m_fHeadDiameter = fHeadDiameter;
            m_fHeadHeight = fHeadHeight;
            m_fBoreDiameter = fBoreDiameter;
            m_fDriverDepth = fDriverDepth;
            m_fDriverSize = fDriverSize;
            m_fWasherDiameter = fWasherDiameter;
            m_fWasherThickness = fWasherThickness;
            m_fNutHeight = fNutHeight;
            m_fNutSize = fNutSize;
        }

        ///<summary>
        /// This constuctor defines a generic fastener based on a few primary chararteristics.
        /// If you have a standard or measurements of the fastener that you want to use, use the other constructor.
        /// </summary>
        public Fastener(
            EHeadType eHeadType,
            EDriver eDriver,
            float Size,
            float Length,
            float ThreadPitch,
            string Description
        ) //TODO: change these names so the match other constuctor
        {
            m_sDescription = Description;
            m_fSize = Size;
            m_fLength = Length;
            m_fThreadPitch = ThreadPitch;

            //Derived properties
            m_fTapSize = Size - ThreadPitch;
            m_fThreadMinor = Size - (1.082532f * ThreadPitch);

            //Generic properties - these are for example only, these values are not based on specific standard only

            m_fLoosefit = Size * 1.1f;
            m_fClosefit = Size * 1.05f;
            m_fNormalfit = Size * 1.075f;
            m_fHeadDiameter = Size * 1.5f;
            m_fHeadHeight = Size;
            m_fBoreDiameter = m_fHeadDiameter * 1.25f;
            m_fDriverDepth = m_fHeadHeight * 0.8f;
            m_fDriverSize = Size;
            m_fWasherDiameter = Size * 2;
            m_fWasherThickness = Size * 0.1f;
            m_fNutHeight = Size;
            m_fNutSize = Size * 2.5f;
        }

        // this function defines a hex shaped modulation using the shape ShapeKernel polygon utility functions.
        private float fGetHexModulation(float fPhi, float fLengthRatio)
        {
            float fRadius = Uf.fGetPolygonRadius(fPhi, Uf.EPolygon.HEX) * m_fDriverSize; // TODO: fix so that it will create any size of Hex
            return fRadius;
        }

        private static float fGetSphereRadius0(float fTheta, float fPhi)
        {
            float a = 2;
            float b = 1;
            float fRadius =
                1f
                / MathF.Sqrt(
                    (
                        (MathF.Pow(MathF.Sin(fPhi), 2) / MathF.Pow(a, 2))
                        + MathF.Pow(MathF.Cos(fPhi), 2) / MathF.Pow(b, 2)
                    )
                );
            return fRadius;
        }

        private Voxels FlattenedSphere(LocalFrame HolePosition) // TODO: add size controls
        {
            BaseSphere oShape = new BaseSphere(HolePosition, m_fHeadDiameter / 2);
            oShape.SetRadius(new SurfaceModulation(fGetSphereRadius0));
            Voxels oVoxels = oShape.voxConstruct();
            return oVoxels;
        }

        private Voxels DrillTip(LocalFrame HolePosition, float fDrillRadius)
        {
            BaseCone oDrillTip = new BaseCone(
                HolePosition,
                fDrillRadius * (float)Math.Tan(41 * Math.PI / 180),
                fDrillRadius,
                0.0001f
            );
            return oDrillTip.voxConstruct();
        }

        private Voxels Hex(LocalFrame HolePosition, float fHeight, float fRadius)
        {
            BaseCylinder oShape = new BaseCylinder(HolePosition, fHeight);
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
                    oScrewHead = Hex(HolePosition, m_fHeadHeight, m_fHeadDiameter / MathF.Sqrt(3)); // TODO:specify that for hexes size is measured accross flats
                    break;
                case EHeadType.Countersunk:
                    //beam with calculated angle, and subtracted body to make flat top, then subtract driver. TODO: add angle other than 82 degrees
                    float fHeight =
                        (m_fHeadDiameter - (m_fHeadDiameter * .0001f))
                        * (float)Math.Tan(41 * Math.PI / 180);
                    BaseCone oCSHead = new BaseCone(
                        HolePosition,
                        fHeight,
                        m_fHeadDiameter / 2,
                        m_fHeadDiameter * .0001f
                    );
                    // is this oFlatTop necessary?TODO:check
                    BaseCylinder oFlatTop = new BaseCylinder(
                        HolePosition,
                        m_fHeadHeight,
                        m_fHeadDiameter + 1
                    );
                    oScrewHead =
                        oCSHead.voxConstruct() - oFlatTop.voxConstruct() - Driver(HolePosition);
                    break;
                case EHeadType.Button:
                    //flattened sphere on top on top of short cylinder, then subtract driver.
                    float fEdgeHeight = 0.05f * m_fHeadDiameter; //TODO: correct with actual height
                    BaseCylinder oHeadEdge = new(HolePosition, fEdgeHeight, m_fHeadDiameter / 2);
                    LocalFrame oButtonFrame = LocalFrame.oGetRelativeFrame(
                        HolePosition,
                        new Vector3(0, 0, fEdgeHeight)
                    );
                    oScrewHead =
                        oHeadEdge.voxConstruct()
                        + FlattenedSphere(HolePosition)
                        - Driver(HolePosition);
                    break;
                case EHeadType.SHCS:
                    BaseCylinder oSHCSBody = new BaseCylinder(
                        HolePosition,
                        m_fHeadHeight,
                        m_fHeadDiameter / 2
                    );
                    oScrewHead = oSHCSBody.voxConstruct() - Driver(HolePosition);

                    break;
                default:
                    //throw exception?
                    break;
            }
            return oScrewHead;
        }

        private Voxels Driver(LocalFrame HolePosition)
        {
            Voxels oDriver = new Voxels();
            LocalFrame oTopofHead = LocalFrame.oGetRelativeFrame(
                HolePosition,
                new Vector3(0, 0, m_fHeadHeight)
            );
            switch (m_eDriver)
            {
                case EDriver.Hex:
                {
                    oDriver = Hex(oTopofHead, -m_fDriverDepth, m_fDriverSize / MathF.Sqrt(3));
                    break;
                }
                case EDriver.Philips:
                {
                    //box, subtract flattened sphere. cross boxes, cut with cup.
                    LocalFrame oCupFrame = LocalFrame.oGetRelativeFrame(
                        oTopofHead,
                        new Vector3(0, 0, -m_fDriverDepth / 2)
                    );
                    BaseBox oCup = new BaseBox(
                        oCupFrame,
                        m_fDriverSize / 2,
                        m_fDriverSize / 2,
                        -m_fDriverDepth / 2
                    );
                    Voxels voxCup = oCup.voxConstruct() - FlattenedSphere(oCupFrame);

                    BaseBox oDriverCross1 = new BaseBox(
                        oTopofHead,
                        m_fDriverSize,
                        m_fDriverSize / 10,
                        -m_fDriverDepth
                    );
                    BaseBox oDriverCross2 = new BaseBox(
                        oTopofHead,
                        m_fDriverSize / 10,
                        m_fDriverSize,
                        -m_fDriverDepth
                    );
                    oDriver = oDriverCross2.voxConstruct() + oDriverCross1.voxConstruct() - voxCup;
                    break;
                }
                case EDriver.Robinson:
                {
                    BaseBox oRobinson = new BaseBox(
                        oTopofHead,
                        m_fDriverSize,
                        m_fDriverSize,
                        -m_fDriverDepth
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
        private Voxels Threads(LocalFrame HolePosition)
        {
            float fTurns = m_fLength / m_fThreadPitch;

            float fBeam1 = 0.5f * m_fThreadPitch;
            float fBeam2 = 0.1f;
            Lattice oLattice = new Lattice();
            for (float fPhi = 0; fPhi <= fTurns * 2f * MathF.PI; fPhi += 0.005f)
            {
                float dS = fPhi / (2f * MathF.PI) * -m_fThreadPitch;
                Vector3 vecRel1 = VecOperations.vecGetCylPoint(m_fThreadMinor / 2, fPhi, dS);
                Vector3 vecRel2 = VecOperations.vecGetCylPoint(m_fSize / 2, fPhi, dS);
                Vector3 vecPt1 = VecOperations.vecTranslatePointOntoFrame(HolePosition, vecRel1);
                Vector3 vecPt2 = VecOperations.vecTranslatePointOntoFrame(HolePosition, vecRel2);
                oLattice.AddBeam(vecPt1, fBeam1, vecPt2, fBeam2, false);
            }
            Voxels oThreads = new Voxels(oLattice);

            return oThreads;
        }

        private Voxels EndChamfer(LocalFrame HolePosition)
        {
            Vector3 vecOffsetFromEnd = new Vector3(0, 0, -m_fLength + (m_fSize / 8));
            LocalFrame oOffsetFromEnd = LocalFrame.oGetRelativeFrame(
                HolePosition,
                vecOffsetFromEnd
            );
            BaseCylinder oBody = new(oOffsetFromEnd, -m_fSize, m_fSize / 2);
            BaseCone oCone = new(oOffsetFromEnd, -m_fSize / 2, m_fSize / 2, .001f);
            return oBody.voxConstruct() - oCone.voxConstruct();
        }

        ///<summary>
        ///This returns a cosmetically threaded screw with the size and head type specified.
        ///Takes a position from a LocalFrame.
        ///</summary>
        public Voxels ScrewThreaded(LocalFrame HolePosition, bool WithWasher = false)
        {
            m_iCountFasteners++;
            LocalFrame oPosition = HolePosition;
            if (WithWasher)
            {
                oPosition = LocalFrame.oGetRelativeFrame(
                    HolePosition,
                    new Vector3(0, 0, m_fWasherThickness)
                );
            }
            Voxels oScrewThreaded =
                ScrewHead(oPosition)
                + MinorBody(oPosition)
                + Threads(oPosition)
                - EndChamfer(oPosition);
            return oScrewThreaded;
        }

        ///<summary>
        ///This returns a non-threaded screw with the size represented by the major diameter and end head type specified.
        ///Takes a position from a LocalFrame.
        ///</summary>
        public Voxels ScrewBasic(LocalFrame HolePosition, bool WithWasher = false)
        {
            m_iCountFasteners++;
            LocalFrame oPosition = HolePosition;
            if (WithWasher)
            {
                oPosition = LocalFrame.oGetRelativeFrame(
                    HolePosition,
                    new Vector3(0, 0, m_fWasherThickness)
                );
            }
            Voxels oScrewBasic =
                ScrewHead(oPosition) + MajorBody(oPosition) - EndChamfer(oPosition);

            return oScrewBasic;
        }

        ///<summary>
        ///This method provide a cylinder sized for a clearance fit for the parent fastener.
        ///Holefit =1 is a close fit, =2 is a normal fit, =3 is a loose fit, and =0 is a custom fit
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleClearence(LocalFrame HolePosition, int HoleFit = 2, float customFit = 0)
        {
            float m_fFit = 2;
            switch (HoleFit)
            {
                case 1:
                    m_fFit = m_fClosefit;
                    break;
                case 2:
                    m_fFit = m_fNormalfit;
                    break;
                case 3:
                    m_fFit = m_fLoosefit;
                    break;
                case 0:
                    m_fFit = customFit;
                    break;
            }

            float m_fHoleRadius = m_fFit / 2;
            BaseCylinder oHole = new BaseCylinder(HolePosition, -m_fLength, m_fHoleRadius);
            Voxels oHoleClearance = oHole.voxConstruct() + DrillTip(HolePosition, m_fHoleRadius);
            return oHoleClearance;
        }

        ///<summary>
        ///Returns a threaded cylinder for use in creating cosmetically threaded holes.
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleThreaded(LocalFrame HolePosition)
        {
            Voxels oTap = TapDrill(HolePosition) + MinorBody(HolePosition) + Threads(HolePosition);
            return oTap;
        }

        ///<summary>
        ///Returns a non-threaded cylinder for use in creating holes with the minor diameter of the chosen fastener..
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleThreadedBasic(LocalFrame HolePosition)
        {
            Voxels oTapMinor = TapDrill(HolePosition) + MinorBody(HolePosition);
            return oTapMinor;
        }

        ///<summary>
        ///Returns a countersunk clearance hole.
        ///Holefit =1 is a close fit, =2 is a normal fit, =3 is a loose fit, and =0 is a custom fit
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleCountersunk(LocalFrame HolePosition)
        {
            float fHeight =
                (m_fHeadDiameter / 2 - (m_fHeadDiameter * 0.0001f))
                * (float)Math.Tan(41 * Math.PI / 180); //WARN:the diameter for a cs hole might be a bit bigger
            BaseCone oCountersink = new BaseCone(
                HolePosition,
                -fHeight,
                m_fHeadDiameter / 2,
                m_fHeadDiameter * 0.0001f
            );
            Voxels oCSHole = HoleClearence(HolePosition) + oCountersink.voxConstruct();
            return oCSHole;
        }

        ///<summary>
        ///Returns a counterbored clearance hole.
        ///Holefit =1 is a close fit, =2 is a normal fit, =3 is a loose fit, and =0 is a custom fit
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleCounterbored(LocalFrame HolePosition, float BoreDepth)
        {
            BaseCylinder oCounterBore = new(HolePosition, BoreDepth, m_fBoreDiameter / 2);
            Voxels oCBHole = HoleClearence(HolePosition) + oCounterBore.voxConstruct();
            return oCBHole;
        }

        ///<summary>
        ///Returns a hole sized for a tap for the chosen fastener. Don't forget to account for tool access for your tap!
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels TapDrill(LocalFrame HolePosition)
        {
            float m_fExtra = m_fLength * 0.10f; //TODO: check to see if 10% is actually a standard, and write it in the summary
            BaseCylinder oTapDrill = new BaseCylinder(
                HolePosition,
                m_fLength + m_fExtra,
                m_fTapSize
            );
            return oTapDrill.voxConstruct() + DrillTip(HolePosition, m_fTapSize);
        }

        ///<summary>
        ///Returns a simple nut for the chosen fastener.
        ///Use by locating with a LocalFrame, specfiying the distance from the head with Thickness.
        ///</summary>
        public Voxels Nut(LocalFrame HolePosition, float Gap) //TODO: need to fix this, showing up without a hole, and wrong size
        {
            m_iCountNuts++;
            LocalFrame HolePositionTranslated = LocalFrame.oGetRelativeFrame(
                HolePosition,
                new Vector3(0, 0, -Gap)
            );
            Voxels oNut = Hex(HolePositionTranslated, -m_fNutHeight, m_fNutSize / MathF.Sqrt(3))
            //                - HoleThreadedBasic(HolePositionTranslated);
            ;
            return oNut;
        }

        ///<summary>
        ///Returns a simple washer for the chosen fastener.
        ///Use by locating with a LocalFrame, specfiying the distance from the head with Thickness.
        ///</summary>
        public Voxels Washer(LocalFrame HolePosition) //Add nonstandard washers in the future?
        {
            m_iCountWashers++;
            BaseLens oWasher = new BaseLens(
                HolePosition,
                m_fWasherThickness,
                m_fClosefit / 2,
                m_fWasherDiameter / 2
            );
            return oWasher.voxConstruct();
        }

        ///<summary>
        ///Returns a fastener-washer-gap-washer-nut stack for the chosen fastener.
        ///Use by locating with a LocalFrame, specfiying the distance to the bottom washer from the top washer with Gap.
        ///</summary>
        public Voxels Stack(LocalFrame HolePosition, float Gap)
        {
            Voxels oStack =
                ScrewBasic(HolePosition, true)
                + Washer(HolePosition)
                + Washer(
                    LocalFrame.oGetRelativeFrame(
                        HolePosition,
                        new Vector3(0, 0, -Gap - m_fWasherThickness)
                    )
                )
                + Nut(
                    LocalFrame.oGetRelativeFrame(
                        HolePosition,
                        new Vector3(0, 0, -m_fWasherThickness)
                    ),
                    Gap
                );
            return oStack;
        }

        ///<summary>
        ///Write to a csv the number of time a fastener, washer, or nut has been called for each fastener type.
        ///</summary>
        public void WritetoBOM(string BOMname)
        {
            //Test output
            Console.WriteLine();
        }
    }
}
