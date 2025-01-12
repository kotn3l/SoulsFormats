using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// FLVER implementation for Model Editor usage
// Credit to The12thAvenger
namespace SoulsFormats
{
    public partial class FLVER2
    {
        public enum ModelMask
        {
            None = -1,
            UpperFace = 0,
            Chin = 1,
            NoseAndCheeks = 2,
            EarsAndTopOfHead = 3,
            Neck = 4,
            LowerNeck = 5,
            Chest = 6,
            UpperElbows = 7,
            Shoulders = 8,
            LowerElbows = 9,
            LowerArms = 10,
            RightHand = 11,
            Waist = 12,
            LeftHand = 13,
            Knees = 14,
            LowerLegs = 15,
            Feet = 16,
            Eyepatch = 17,
            _18 = 18,
            _19 = 19,
            ArmsLongGlovesLowerArmOnly = 20,
            ArmsLongGlovesFullLength = 21,
            _22 = 22,
            BodyScarfHighCollarCompressed = 23,
            BodySleevesBunchedToElbow = 24,
            BodySleevesBunchedMidway = 25,
            BodySleevesFullSize = 26,
            BodyLargeSleevesBunchedToElbow = 27,
            BodyLargeSleevesFullSize = 28,
            BodySleevesWristBracelets = 29,
            BodyCouterElbowArmor = 30,
            BodyCouterElbowArmorHigherUp = 31,
            BodySmallCollar = 32,
            BodySmallCollarCompressed = 33,
            BodyScarfHighCollarFullSize = 34,
            BodyLowerAbdomenCover = 35,
            BodySmallHoodDown = 36,
            BodySmallHoodUp = 37,
            BodyLeftPauldronShoulder = 38,
            BodyRightPauldronShoulder = 39,
            BodyCowl = 40,
            BodyCowlLong = 41,
            BodyCowlMid = 42,
            BodyCowlCompressed = 43,
            HeadGorgetNeckpiece = 44,
            HeadGorgetNeckpieceCompressed = 45,
            HeadLongHoodPlumeLow = 46,
            HeadLongHoodPlumeMid = 47,
            HeadLongHoodPlumeHigh = 48,
            HeadLongHoodPlumeShort = 49,
            LegsHighWaistbelt = 50,
            LegsLeggings = 51,
            LegsLeggingsCompressed = 52,
            LegsKneepads = 53,
            LegsKneepadsCompressed = 54,
            LegsWaistbelt = 55,
            LegsWaistbeltCompressed = 56,
            LegsWaistcloth = 57,
            LegsWaistclothCompressed = 58,
            LegsPantsBigThighs = 59,
            HairFront = 60,
            HairForehead = 61,
            HairUnderHelmet = 62,
            HairOverHeadband = 63,
            HairFull = 64,
            HairBackOfHead = 65,
            LongHairBraidTailLow = 66,
            LongHairBraidTailHigh = 67,
            LongHairBraidTailHighest = 68,
            LongHairBraidTailLowestShortest = 69,
            HeadGorgetNeckpieceLarge = 70,
            BodyLongShirtBunchedUpIntoBelt = 71,
            BodyLongShirtFullLengthOverWaist = 72,
            BareTorsoAndUpperArms = 73,
            BodyGravekeeperCloakHoodDown = 74,
            BodyGravekeeperCloakHoodUp = 75,
            LowerNeckWrap = 76,
            _77 = 77,
            BeardJaw = 78,
            BeardChin = 79,
            BeardStubble = 80,
            _81 = 81,
            _82 = 82,
            _83 = 83,
            _84 = 84,
            _85 = 85,
            _86 = 86,
            _87 = 87,
            _88 = 88,
            _89 = 89,
            _90 = 90,
            _91 = 91,
            _92 = 92,
            _93 = 93,
            _94 = 94,
            _95 = 95
        }


        /// <summary>
        /// A reference to an MTD file, specifying textures to use.
        /// </summary>
        public class Material : IFlverMaterial
        {
            /// <summary>
            /// Identifies the mesh that uses this material, may include keywords that determine hideable parts.
            /// </summary>
            public string Name { get; set; }

            public ModelMask MaskId { get; set; }

            /// <summary>
            /// Virtual path to an MTD file or a Matxml file in games since ER.
            /// </summary>
            public string MTD { get; set; }

            /// <summary>
            /// Textures used by this material.
            /// </summary>
            public List<Texture> Textures { get; set; }
            IReadOnlyList<IFlverTexture> IFlverMaterial.Textures => Textures;

            /// <summary>
            /// Index to the flver's list of GX lists.
            /// </summary>
            public int GXIndex { get; set; }

            /// <summary>
            /// Index of the material in the material list. Used since Sekiro during cutscenes. 
            /// </summary>
            public int Index { get; set; }

            private int textureIndex, textureCount;

            /// <summary>
            /// Creates a new Material with null or default values.
            /// </summary>
            public Material()
            {
                Name = "Untitled";
                MTD = "";
                Textures = new List<Texture>();
                GXIndex = -1;
            }

            /// <summary>
            /// Creates a new Material with the given values and an empty texture list.
            /// </summary>
            public Material(string name, string mtd, int flags)
            {
                Name = name;
                MTD = mtd;
                Textures = new List<Texture>();
                GXIndex = -1;
                Index = 0;
            }

            /// <summary>
            /// Calculates the total number of bytes in the utf-16 null-terminated strings owned by this material
            /// </summary>
            private int CalculateNumStringBytes()
            {
                int numStringBytes = Name.Length + 1;
                numStringBytes += MTD.Length + 1;
                foreach (Texture texture in Textures)
                {
                    numStringBytes += texture.Type.Length + 1;
                    numStringBytes += texture.Path.Length + 1;
                }

                // 2-bytes per character
                numStringBytes *= 2;
                return numStringBytes;
            }

            internal Material(BinaryReaderEx br, FLVERHeader header, List<GXList> gxLists, Dictionary<int, int> gxListIndices)
            {
                int nameOffset = br.ReadInt32();
                int mtdOffset = br.ReadInt32();
                textureCount = br.ReadInt32();
                textureIndex = br.ReadInt32();
                // result of CalculateNumStringBytes
                br.ReadInt32();
                int gxOffset = br.ReadInt32();
                Index = br.ReadInt32();
                br.AssertInt32(0);

                string tempName;
                if (header.Unicode)
                {
                    tempName = br.GetUTF16(nameOffset);
                    MTD = br.GetUTF16(mtdOffset);
                }
                else
                {
                    tempName = br.GetShiftJIS(nameOffset);
                    MTD = br.GetShiftJIS(mtdOffset);
                }


                if (tempName.StartsWith('#'))
                {
                    Name = tempName[4..];
                    MaskId = (ModelMask)int.Parse(tempName[1..3]);
                }
                else
                {
                    Name = tempName;
                    MaskId = ModelMask.None;
                }

                if (gxOffset == 0)
                {
                    GXIndex = -1;
                }
                else
                {
                    if (!gxListIndices.ContainsKey(gxOffset))
                    {
                        br.StepIn(gxOffset);
                        {
                            gxListIndices[gxOffset] = gxLists.Count;
                            gxLists.Add(new GXList(br, header));
                        }
                        br.StepOut();
                    }
                    GXIndex = gxListIndices[gxOffset];
                }

            }

            internal void TakeTextures(Dictionary<int, Texture> textureDict)
            {
                Textures = new List<Texture>(textureCount);
                for (int i = textureIndex; i < textureIndex + textureCount; i++)
                {
                    if (!textureDict.ContainsKey(i))
                        throw new NotSupportedException("Texture not found or already taken: " + i);

                    Textures.Add(textureDict[i]);
                    textureDict.Remove(i);
                }

                textureIndex = -1;
                textureCount = -1;
            }

            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.ReserveInt32($"MaterialName{index}");
                bw.ReserveInt32($"MaterialMTD{index}");
                bw.WriteInt32(Textures.Count);
                bw.ReserveInt32($"TextureIndex{index}");
                bw.WriteInt32(CalculateNumStringBytes());
                bw.ReserveInt32($"GXOffset{index}");
                bw.WriteInt32(Index);
                bw.WriteInt32(0);
            }

            internal void FillGXOffset(BinaryWriterEx bw, int index, List<int> gxOffsets)
            {
                if (GXIndex == -1)
                    bw.FillInt32($"GXOffset{index}", 0);
                else
                    bw.FillInt32($"GXOffset{index}", gxOffsets[GXIndex]);
            }

            internal void WriteTextures(BinaryWriterEx bw, int index, int textureIndex)
            {
                bw.FillInt32($"TextureIndex{index}", textureIndex);
                for (int i = 0; i < Textures.Count; i++)
                    Textures[i].Write(bw, textureIndex + i);
            }

            internal void WriteStrings(BinaryWriterEx bw, FLVERHeader header, int index)
            {
                bw.FillInt32($"MaterialName{index}", (int)bw.Position);

                string tempName = Name;
                if (MaskId != ModelMask.None)
                {
                    tempName = $"#{MaskId:00}#{Name}";
                }

                if (header.Unicode)
                    bw.WriteUTF16(tempName, true);
                else
                    bw.WriteShiftJIS(tempName, true);

                bw.FillInt32($"MaterialMTD{index}", (int)bw.Position);
                if (header.Unicode)
                    bw.WriteUTF16(MTD, true);
                else
                    bw.WriteShiftJIS(MTD, true);
            }

            /// <summary>
            /// Returns the name and MTD path of the material.
            /// </summary>
            public override string ToString()
            {
                return $"{Name} | {MTD}";
            }
            public Material Clone()
            {
                return (Material)MemberwiseClone();
            }
        }
    }
}
