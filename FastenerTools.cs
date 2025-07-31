using System.Numerics;
using Leap71.ShapeKernel;
using PicoGK;

namespace PicoGK_Fasteners
{
    public partial class Fastener
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
        protected float m_fHexSize;
        protected float m_fHeadHeightModifier;
        protected float m_fFlattenedSphereDiameter;

        //counts for BOM TODO:check if necessary
        protected int m_iCountFasteners;
        protected int m_iCountNuts;
        protected int m_iCountWashers;

        ///<summary>
        /// This constuctor defines a generic fastener based on a few primary chararteristics.
        /// If you have a standard or measurements of the fastener that you want to use, use the other constructor.
        /// Any measurements of a Hex is done across the flats.
        /// </summary>
        public Fastener(
            EHeadType eHeadType,
            EDriver eDriver,
            float fSize,
            float fLength,
            float fThreadPitch,
            string sDescription
        )
        {
            m_sDescription = sDescription;
            m_fSize = fSize;
            m_fLength = fLength;
            m_fThreadPitch = fThreadPitch;
            m_eHeadType = eHeadType;
            m_eDriver = eDriver;

            //Derived properties
            m_fTapSize = fSize - fThreadPitch;
            m_fThreadMinor = fSize - (1.082532f * fThreadPitch);

            //Generic properties - these are for example only, these values are not based on specific standard only

            m_fLoosefit = fSize * 1.1f;
            m_fClosefit = fSize * 1.05f;
            m_fNormalfit = fSize * 1.075f;
            if (eHeadType == EHeadType.Hex)
            {
                m_fHeadDiameter = fSize * 1.75f;
            }
            else
            {
                m_fHeadDiameter = fSize * 1.5f;
            }
            m_fHeadHeight = fSize * 0.75f;
            m_fBoreDiameter = m_fHeadDiameter * 1.25f;
            m_fDriverDepth = m_fHeadHeight * 0.8f;
            m_fDriverSize = fSize * .75f;
            m_fWasherDiameter = fSize * 3;
            m_fWasherThickness = fSize * 0.125f;
            m_fNutHeight = fSize * 0.75f;
            m_fNutSize = fSize * 2;
        }

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
            float fTapSize = 4.2f,
            float fLooseFit = 5.8f,
            float fCloseFit = 5.3f,
            float fNormalfit = 5.5f,
            float fBoreDiameter = 9.75f,
            float fWasherDiameter = 15,
            float fWasherThickness = 1.2f,
            float fNutHeight = 4,
            float fNutSize = 7.79f
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
        ///This returns a cosmetically threaded screw with the size and head type specified by the object constructor. Set WithWasher to true if you want to lift the screw by the width of a washer.
        ///Takes a position from a LocalFrame.
        ///</summary>
        public Voxels ScrewThreaded(LocalFrame HolePosition, bool WithWasher = false)
        {
            LocalFrame oPosition = HolePosition;
            if (WithWasher)
            {
                oPosition = FrameOffset(HolePosition, new Vector3(0, 0, m_fWasherThickness));
            }
            Voxels oScrewThreaded =
                ScrewHead(oPosition)
                + MinorBody(oPosition)
                + Threads(oPosition, m_fLength)
                - EndChamfer(oPosition)
                - Driver(oPosition);
            return oScrewThreaded;
        }

        ///<summary>
        ///This returns a non-threaded screw with the size represented by the major diameter and end head type specified by the object constructor. Set WithWasher to true if you want to lift the screw by the width of a washer.
        ///Takes a position from a LocalFrame.
        ///</summary>
        public Voxels ScrewBasic(LocalFrame HolePosition, bool WithWasher = false)
        {
            LocalFrame oPosition = HolePosition;
            if (WithWasher)
            {
                oPosition = FrameOffset(HolePosition, new Vector3(0, 0, m_fWasherThickness));
            }
            Voxels oScrewBasic =
                ScrewHead(oPosition)
                + MajorBody(oPosition)
                - EndChamfer(oPosition)
                - Driver(oPosition);

            return oScrewBasic;
        }

        ///<summary>
        ///This method provide a cylinder sized for a clearance fit for the parent fastener.
        ///Holefit =1 is a close fit, =2 is a normal fit, =3 is a loose fit, and =0 is a
        ///custom fit where you define your desired hole diameter in the last value customFit.
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
            Voxels oHoleClearance =
                oHole.voxConstruct()
                + DrillTip(FrameOffset(HolePosition, new Vector3(0, 0, -m_fLength)), m_fHoleRadius);
            return oHoleClearance;
        }

        ///<summary>
        ///Returns a threaded cylinder for use in creating cosmetically threaded holes.
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///Depth of the tapped section of the hole is equal to the length of the fastener plus three threads.
        ///</summary>
        public Voxels HoleThreaded(LocalFrame HolePosition)
        {
            Voxels oTap =
                TapDrill(HolePosition)
                + MinorBody(HolePosition)
                + Threads(HolePosition, m_fLength + (3 * m_fThreadPitch));
            return oTap;
        }

        ///<summary>
        ///Returns a non-threaded cylinder for use in creating holes with the minor diameter of the chosen fastener.
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///Depth of the tapped section of the hole is equal to the length of the fastener plus three threads.
        ///</summary>
        /* Minor diameter will always be smaller than tap drill diameter, so this method is unnecessary?
        public Voxels HoleThreadedBasic(LocalFrame HolePosition)
        {
            Voxels oTapMinor = TapDrill(HolePosition) + MinorBody(HolePosition);
            return oTapMinor;
        }
        */

        ///<summary>
        ///Returns a countersunk clearance hole.
        ///Holefit =1 is a close fit, =2 is a normal fit, =3 is a loose fit, and =0 is a custom fit
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleCountersunk(LocalFrame HolePosition, int HoleFit = 2, float customFit = 0)
        {
            //TODO: Add a option for a height offset so that the head of the screw does not have to be flush with the surface.
            float fHeight =
                (m_fHeadDiameter / 2 - (m_fHeadDiameter * 0.0001f))
                * (float)Math.Tan(41 * Math.PI / 180);
            BaseCone oCountersink = new BaseCone(
                HolePosition,
                -fHeight,
                m_fHeadDiameter / 2,
                m_fHeadDiameter * 0.0001f
            );
            Voxels oCSHole =
                HoleClearence(HolePosition, HoleFit, customFit) + oCountersink.voxConstruct();
            return oCSHole;
        }

        ///<summary>
        ///Returns a counterbored clearance hole.
        ///Holefit =1 is a close fit, =2 is a normal fit, =3 is a loose fit, and =0 is a custom fit
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///</summary>
        public Voxels HoleCounterbored(
            LocalFrame HolePosition,
            float BoreDepth,
            int HoleFit = 2,
            float customFit = 0
        )
        {
            BaseCylinder oCounterBore = new(HolePosition, BoreDepth, m_fBoreDiameter / 2);
            Voxels oCBHole =
                HoleClearence(HolePosition, HoleFit, customFit) + oCounterBore.voxConstruct();
            return oCBHole;
        }

        ///<summary>
        ///Returns a hole sized for a tap for the chosen fastener.
        ///Use by locating with a LocalFrame, and subtracting the resulting voxel body from your object.
        ///Drills to a depth equal to the length specifed plus 1/2 the major diameter of the tap.
        ///</summary>
        public Voxels TapDrill(LocalFrame HolePosition)
        {
            float m_fTapDepth = m_fLength + (m_fSize / 2) + (3 * m_fThreadPitch);

            BaseCylinder oTapDrill = new BaseCylinder(HolePosition, -m_fTapDepth, m_fTapSize / 2);
            return oTapDrill.voxConstruct()
                + DrillTip(
                    FrameOffset(HolePosition, new Vector3(0, 0, -m_fTapDepth)),
                    m_fTapSize / 2
                );
        }

        ///<summary>
        ///Returns a simple hex nut for the chosen fastener.
        ///Use by locating with a LocalFrame, specfiying the distance from the head with Gap.
        ///</summary>
        public Voxels Nut(LocalFrame HolePosition, float Gap)
        {
            LocalFrame HolePositionTranslated = FrameOffset(HolePosition, new Vector3(0, 0, -Gap));
            Voxels oNut =
                Hex(HolePositionTranslated, -m_fNutHeight, (m_fNutSize * MathF.Sqrt(3)) / 3)
                - TapDrill(HolePositionTranslated);

            return oNut;
        }

        ///<summary>
        ///Returns a simple washer for the chosen fastener.
        ///Use by locating with a LocalFrame, specfiying the distance from the head with Thickness.
        ///</summary>
        public Voxels Washer(LocalFrame HolePosition) //Add nonstandard washers in the future?
        {
            BaseLens oWasher = new BaseLens(
                HolePosition,
                m_fWasherThickness,
                m_fClosefit / 2,
                m_fWasherDiameter / 2
            );
            return oWasher.voxConstruct();
        }

        ///<summary>
        ///Returns a Fastener-Washer-Gap-Washer-Nut stack for the chosen fastener.
        ///Use by locating with a LocalFrame, specfiying the distance to the bottom washer from the top washer with Gap.
        ///</summary>
        public Voxels Stack(LocalFrame HolePosition, float Gap)
        {
            Voxels oStack =
                ScrewBasic(HolePosition, true)
                + Washer(HolePosition)
                + Washer(FrameOffset(HolePosition, new Vector3(0, 0, -Gap - m_fWasherThickness)))
                + Nut(FrameOffset(HolePosition, new Vector3(0, 0, -m_fWasherThickness)), Gap);
            return oStack;
        }

        ///<summary>
        ///Write to a csv the number of time a fastener, washer, or nut has been called for each fastener type.
        ///</summary>
        public void WritetoBOM(string BOMname) //TODO: finish this
        {
            //Test output
            Console.WriteLine();
        }
    }
}
