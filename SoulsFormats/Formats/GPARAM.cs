using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

// TKGP's latest version of GPARAM
#nullable disable
namespace SoulsFormats
{
    public class GPARAM : SoulsFile<GPARAM>
    {
        public GPARAM.GparamVersion Version { get; set; }

        public bool Unk0d { get; set; }

        public int Count14 { get; set; }

        public List<GPARAM.Param> Params { get; set; }

        public byte[] Data30 { get; set; }

        public List<GPARAM.UnkParamExtra> UnkParamExtras { get; set; }

        public float Unk40 { get; set; }

        public float Unk50 { get; set; }

        public GPARAM()
        {
            this.Unk0d = true;
            this.Params = new List<GPARAM.Param>();
        }

        public GPARAM Clone()
        {
            return (GPARAM)MemberwiseClone();
        }

        protected override bool Is(BinaryReaderEx br)
        {
            return br.Length >= 4L && br.GetASCII(0L, 8) == "f\0i\0l\0t\0";
        }

        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("f\0i\0l\0t\0");
            this.Version = br.ReadEnum32<GPARAM.GparamVersion>();
            int num1 = (int)br.AssertByte(new byte[1]);
            this.Unk0d = br.ReadBoolean();
            int num2 = (int)br.AssertInt16(new short[1]);
            int num3 = br.ReadInt32();
            this.Count14 = br.ReadInt32();
            GPARAM.BaseOffsets baseOffsets;
            baseOffsets.ParamOffsets = br.ReadInt32();
            baseOffsets.Params = br.ReadInt32();
            baseOffsets.FieldOffsets = br.ReadInt32();
            baseOffsets.Fields = br.ReadInt32();
            baseOffsets.Values = br.ReadInt32();
            baseOffsets.ValueIds = br.ReadInt32();
            baseOffsets.Unk30 = br.ReadInt32();
            int capacity = br.ReadInt32();
            baseOffsets.ParamExtras = br.ReadInt32();
            baseOffsets.ParamExtraIds = br.ReadInt32();
            this.Unk40 = br.ReadSingle();
            baseOffsets.ParamCommentsOffsets = br.ReadInt32();
            baseOffsets.CommentOffsets = br.ReadInt32();
            baseOffsets.Comments = br.ReadInt32();
            if (this.Version >= GPARAM.GparamVersion.V5)
                this.Unk50 = br.ReadSingle();
            int[] int32s1 = br.GetInt32s((long)baseOffsets.ParamOffsets, num3);
            this.Params = new List<GPARAM.Param>(num3);
            foreach (int num4 in int32s1)
            {
                br.Position = (long)(baseOffsets.Params + num4);
                this.Params.Add(new GPARAM.Param(br, this.Version, baseOffsets));
            }
            br.Position = (long)baseOffsets.Unk30;
            this.Data30 = br.ReadBytes(baseOffsets.ParamExtras - baseOffsets.Unk30);
            br.Position = (long)baseOffsets.ParamExtras;
            this.UnkParamExtras = new List<GPARAM.UnkParamExtra>(capacity);
            for (int index = 0; index < capacity; ++index)
                this.UnkParamExtras.Add(new GPARAM.UnkParamExtra(br, this.Version, baseOffsets));
            int[] int32s2 = br.GetInt32s((long)baseOffsets.ParamCommentsOffsets, num3);
            for (int index = 0; index < num3; ++index)
            {
                int offset = baseOffsets.CommentOffsets + int32s2[index];
                int num5 = ((index >= num3 - 1 ? baseOffsets.Comments : baseOffsets.CommentOffsets + int32s2[index + 1]) - offset) / 4;
                int[] int32s3 = br.GetInt32s((long)offset, num5);
                List<string> stringList = new List<string>(num5);
                foreach (int num6 in int32s3)
                    stringList.Add(br.GetUTF16((long)(baseOffsets.Comments + num6)));
                this.Params[index].Comments = stringList;
            }
        }

        protected override void Write(BinaryWriterEx bw)
        {
            GPARAM.BaseOffsets baseOffsets = new GPARAM.BaseOffsets();
            bw.BigEndian = false;
            bw.WriteUTF16("filt");
            bw.WriteUInt32((uint)this.Version);
            bw.WriteByte((byte)0);
            bw.WriteBoolean(this.Unk0d);
            bw.WriteInt16((short)0);
            bw.WriteInt32(this.Params.Count);
            bw.WriteInt32(this.Count14);
            bw.ReserveInt32("ParamOffsetsBase");
            bw.ReserveInt32("ParamsBase");
            bw.ReserveInt32("FieldOffsetsBase");
            bw.ReserveInt32("FieldsBase");
            bw.ReserveInt32("ValuesBase");
            bw.ReserveInt32("ValueIdsBase");
            bw.ReserveInt32("Unk30Base");
            bw.WriteInt32(this.UnkParamExtras.Count);
            bw.ReserveInt32("ParamExtrasBase");
            bw.ReserveInt32("ParamExtraIdsBase");
            bw.WriteSingle(this.Unk40);
            bw.ReserveInt32("ParamCommentsOffsetsBase");
            bw.ReserveInt32("CommentOffsetsBase");
            bw.ReserveInt32("CommentsBase");
            if (this.Version >= GPARAM.GparamVersion.V5)
                bw.WriteSingle(this.Unk50);
            baseOffsets.ParamOffsets = (int)bw.Position;
            bw.FillInt32("ParamOffsetsBase", baseOffsets.ParamOffsets);
            DefaultInterpolatedStringHandler interpolatedStringHandler;
            for (int index = 0; index < this.Params.Count; ++index)
            {
                BinaryWriterEx binaryWriterEx = bw;
                interpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
                interpolatedStringHandler.AppendLiteral("ParamOffset[");
                interpolatedStringHandler.AppendFormatted<int>(index);
                interpolatedStringHandler.AppendLiteral("]");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                binaryWriterEx.ReserveInt32(stringAndClear);
            }
            baseOffsets.Params = (int)bw.Position;
            bw.FillInt32("ParamsBase", baseOffsets.Params);
            for (int index = 0; index < this.Params.Count; ++index)
            {
                BinaryWriterEx binaryWriterEx = bw;
                interpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
                interpolatedStringHandler.AppendLiteral("ParamOffset[");
                interpolatedStringHandler.AppendFormatted<int>(index);
                interpolatedStringHandler.AppendLiteral("]");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                int num = (int)bw.Position - baseOffsets.Params;
                binaryWriterEx.FillInt32(stringAndClear, num);
                this.Params[index].Write(bw, index);
                bw.Pad(4);
            }
            baseOffsets.FieldOffsets = (int)bw.Position;
            bw.FillInt32("FieldOffsetsBase", baseOffsets.FieldOffsets);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteFieldOffsets(bw, baseOffsets, index);
            baseOffsets.Fields = (int)bw.Position;
            bw.FillInt32("FieldsBase", baseOffsets.Fields);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteFields(bw, Version, baseOffsets, index);
            baseOffsets.Values = (int)bw.Position;
            bw.FillInt32("ValuesBase", baseOffsets.Values);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteValues(bw, baseOffsets, index);
            baseOffsets.ValueIds = (int)bw.Position;
            bw.FillInt32("ValueIdsBase", baseOffsets.ValueIds);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteValueIds(bw, this.Version, baseOffsets, index);
            baseOffsets.Unk30 = (int)bw.Position;
            bw.FillInt32("Unk30Base", baseOffsets.Unk30);
            bw.WriteBytes(this.Data30);
            bw.Pad(4);
            baseOffsets.ParamExtras = (int)bw.Position;
            bw.FillInt32("ParamExtrasBase", baseOffsets.ParamExtras);
            for (int index = 0; index < this.UnkParamExtras.Count; ++index)
                this.UnkParamExtras[index].Write(bw, this.Version, index);
            baseOffsets.ParamExtraIds = (int)bw.Position;
            bw.FillInt32("ParamExtraIdsBase", baseOffsets.ParamExtraIds);
            for (int index = 0; index < this.UnkParamExtras.Count; ++index)
                this.UnkParamExtras[index].WriteIds(bw, baseOffsets, index);
            baseOffsets.ParamCommentsOffsets = (int)bw.Position;
            bw.FillInt32("ParamCommentsOffsetsBase", baseOffsets.ParamCommentsOffsets);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteCommentOffsetsOffset(bw, index);
            baseOffsets.CommentOffsets = (int)bw.Position;
            bw.FillInt32("CommentOffsetsBase", baseOffsets.CommentOffsets);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteCommentOffsets(bw, baseOffsets, index);
            baseOffsets.Comments = (int)bw.Position;
            bw.FillInt32("CommentsBase", baseOffsets.Comments);
            for (int index = 0; index < this.Params.Count; ++index)
                this.Params[index].WriteComments(bw, baseOffsets, index);
        }

        public enum FieldType : byte
        {
            Sbyte = 1,
            Short = 2,
            Int = 3,
            Byte = 5,
            Uint = 7,
            Float = 9,
            Bool = 11, // 0x0B
            Vec2 = 12, // 0x0C
            Vec3 = 13, // 0x0D
            Vec4 = 14, // 0x0E
            Color = 15, // 0x0F

            // Treated as int for now
            Unk_0x4 = 4,
            Unk_0x6 = 6, 
            Unk_0x8 = 8, 
            Unk_0xA = 10, 
            Unk_0x14 = 20,

            // x10 - 16
            Unk_0x10 = 16,
            // x11 - 17
            Unk_0x11 = 17,
            // x12 - 18
            Unk_0x12 = 18,
            // x13 - 19
            Unk_0x13 = 19,
            // x15 - 21
            Unk_0x15 = 21,
            // x16 - 22
            Unk_0x16 = 22,
            // x17 - 23
            Unk_0x17 = 23,
            // x18 - 24
            Unk_0x18 = 24,
            // x19 - 25
            Unk_0x19 = 25,
            // x1A - 26
            Unk_0x1A = 26,
            // x1B - 27
            Unk_0x1B = 27,
            // x1C - 28
            Unk_0x1C = 28,
            // x1D - 29
            Unk_0x1D = 29,
            // x1E - 30
            Unk_0x1E = 30,
            // x1F - 31
            Unk_0x1F = 31,
            // x20 - 32
            Unk_0x20 = 32,
            // x20 - 32
            Unk_0x21 = 33,
            // x24 - 36
            Unk_0x22 = 34,
            // x24 - 36
            Unk_0x23 = 35,
            // x24 - 36
            Unk_0x24 = 36,
            // x28 - 40
            Unk_0x28 = 40,
            // x29 - 41
            Unk_0x29 = 41,
            // x2A - 42
            Unk_0x2A = 42,
            // x2C - 44
            Unk_0x2C = 44,
            // x2D - 45
            Unk_0x2D = 45,
            // x30 - 48
            Unk_0x30 = 48,
            // x31 - 49
            Unk_0x31 = 49,
            // x34 - 49
            Unk_0x34 = 52,
            // x35
            Unk_0x35 = 53,
            // x35
            Unk_0x36 = 54,
            // x35
            Unk_0x37 = 55,
            // x38
            Unk_0x38 = 56,
            // x38
            Unk_0x39 = 57,
            // x3A - 58
            Unk_0x3A = 58,
            // x3B - 59
            Unk_0x3B = 59,
            // x3C
            Unk_0x3C = 60,
            // x3D - 61
            Unk_0x3D = 61,
            // x3E - 62
            Unk_0x3E = 62,
            // x3F - 63
            Unk_0x3F = 63,
            // x40 - 64
            Unk_0x40 = 64,
            // x41 - 65
            Unk_0x41 = 65,
            // x42 - 66
            Unk_0x42 = 66,
            // x43 - 67
            Unk_0x43 = 67,
            // x44 - 68
            Unk_0x44 = 68,
            // x45 - 69
            Unk_0x45 = 69,
            // x46 - 70
            Unk_0x46 = 70,
            // x47 - 71
            Unk_0x47 = 71,
            // x48 - 72
            Unk_0x48 = 72,
            // x49 - 73
            Unk_0x49 = 73,
            // x4A - 74
            Unk_0x4A = 74,
            // x4B - 75
            Unk_0x4B = 75,
            // x4C - 76
            Unk_0x4C = 76,
            // x4D - 77
            Unk_0x4D = 77,
            // x4E - 78
            Unk_0x4E = 78,
            // x4F - 79
            Unk_0x4F = 79,
            // x69 - 105
            Unk_0x69 = 105,
            // x6C - 108
            Unk_0x6C = 108,
            // x78 - 124
            Unk_0x78 = 120,
            // x7C - 124
            Unk_0x7C = 124,
            // x7F - 127
            Unk_0x7F = 127,
            // x80 - 128
            Unk_0x80 = 128,
            // x81 - 129
            Unk_0x81 = 129,
            // x82 - 130
            Unk_0x82 = 130,
            // x83 - 131
            Unk_0x83 = 131,
            // x84 - 132
            Unk_0x84 = 132,
            // x85 - 133
            Unk_0x85 = 133,
            // x86 - 134
            Unk_0x86 = 134,
            // x87 - 135
            Unk_0x87 = 135,
            // x88 - 136
            Unk_0x88 = 136,
            // x89 - 137
            Unk_0x89 = 137,
            // x8A - 138
            Unk_0x8A = 138,
            // x8B - 139
            Unk_0x8B = 139,
            // x8C - 140
            Unk_0x8C = 140,
            // x8D - 141
            Unk_0x8D = 141,
            // x8E - 142
            Unk_0x8E = 142,
            // x8F - 143
            Unk_0x8F = 143,
            // x90 - 144
            Unk_0x90 = 144,
            // x91 - 157
            Unk_0x91 = 145,
            // x92 - 146
            Unk_0x92 = 146,
            // x93 - 147
            Unk_0x93 = 147,
            // x94 - 148
            Unk_0x94 = 148,
            // x95 - 149
            Unk_0x95 = 149,
            // x96 - 150
            Unk_0x96 = 150,
            // x97 - 151
            Unk_0x97 = 151,
            // x98 - 152
            Unk_0x98 = 152,
            // x99 - 153
            Unk_0x99 = 153,
            // x9A - 154
            Unk_0x9A = 154,
            // x9B - 155
            Unk_0x9B = 155,
            // x9C - 156
            Unk_0x9C = 156,
            // x9D - 157
            Unk_0x9D = 157,
            // x9E - 158
            Unk_0x9E = 158,
            // x9F - 159
            Unk_0x9F = 159,
            // xA0 - 160
            Unk_0xA0 = 160,
            // xA1 - 161
            Unk_0xA1 = 161,
            // xA2 - 162
            Unk_0xA2 = 162,
            // xA3 - 163
            Unk_0xA3 = 163,
            // xA4 - 164
            Unk_0xA4 = 164,
            // xA5 - 165
            Unk_0xA5 = 165,
            // xA6 - 166
            Unk_0xA6 = 166,
            // xA7 - 167
            Unk_0xA7 = 167,
            // xA8 - 168
            Unk_0xA8 = 168,
            // xA9 - 169
            Unk_0xA9 = 169,
            // xAA - 170
            Unk_0xAA = 170,
            // xAB - 171
            Unk_0xAB = 171,
            // xAC - 172
            Unk_0xAC = 172,
            // xAD - 173
            Unk_0xAD = 173,
            // xAE - 174
            Unk_0xAE = 174,
            // xAF - 175
            Unk_0xAF = 175,
            // xB0 - 176
            Unk_0xB0 = 176,
            // xB1 - 177
            Unk_0xB1 = 177,
            // xB2 - 178
            Unk_0xB2 = 178,
            // xB3 - 179
            Unk_0xB3 = 179
        }

        public interface IField
        {
            string Key { get; set; }

            string Name { get; set; }

            IReadOnlyList<GPARAM.IFieldValue> Values { get; }

            internal static GPARAM.IField Read(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
            {
                GPARAM.FieldType enum8 = br.GetEnum8<GPARAM.FieldType>(br.Position + 8L);
                if (version < GparamVersion.V6)
                {
                    enum8 = br.GetEnum8<GPARAM.FieldType>(br.Position + 8L);
                }
                else
                {
                    enum8 = br.GetEnum8<GPARAM.FieldType>(br.Position + 10L);
                }
                switch (enum8)
                    {
                        case GPARAM.FieldType.Sbyte:
                            return (GPARAM.IField)new GPARAM.SbyteField(br, version, baseOffsets);
                        case GPARAM.FieldType.Short:
                            return (GPARAM.IField)new GPARAM.ShortField(br, version, baseOffsets);
                        case GPARAM.FieldType.Int:
                            return (GPARAM.IField)new GPARAM.IntField(br, version, baseOffsets);
                        case GPARAM.FieldType.Byte:
                            return (GPARAM.IField)new GPARAM.ByteField(br, version, baseOffsets);
                        case GPARAM.FieldType.Uint:
                            return (GPARAM.IField)new GPARAM.UintField(br, version, baseOffsets);
                        case GPARAM.FieldType.Float:
                            return (GPARAM.IField)new GPARAM.FloatField(br, version, baseOffsets);
                        case GPARAM.FieldType.Bool:
                            return (GPARAM.IField)new GPARAM.BoolField(br, version, baseOffsets);
                        case GPARAM.FieldType.Vec2:
                            return (GPARAM.IField)new GPARAM.Vector2Field(br, version, baseOffsets);
                        case GPARAM.FieldType.Vec3:
                            return (GPARAM.IField)new GPARAM.Vector3Field(br, version, baseOffsets);
                        case GPARAM.FieldType.Vec4:
                            return (GPARAM.IField)new GPARAM.Vector4Field(br, version, baseOffsets);
                        case GPARAM.FieldType.Color:
                            return (GPARAM.IField)new GPARAM.ColorField(br, version, baseOffsets);
                        default:
                            /*DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
                            interpolatedStringHandler.AppendLiteral("Unknown field type: ");
                            interpolatedStringHandler.AppendFormatted<GPARAM.FieldType>(enum8);
                            throw new NotImplementedException(interpolatedStringHandler.ToStringAndClear());*/
                            break;
                    }

                GPARAM.IField field;
                switch (enum8)
                {
                    case GPARAM.FieldType.Unk_0x4:
                        field = (GPARAM.IField)new GPARAM.Unk_4_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x6:
                        field = (GPARAM.IField)new GPARAM.Unk_6_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8:
                        field = (GPARAM.IField)new GPARAM.Unk_8_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA:
                        field = (GPARAM.IField)new GPARAM.Unk_A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x10:
                        field = (GPARAM.IField)new GPARAM.Unk_10_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x11:
                        field = (GPARAM.IField)new GPARAM.Unk_11_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x12:
                        field = (GPARAM.IField)new GPARAM.Unk_12_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x13:
                        field = (GPARAM.IField)new GPARAM.Unk_13_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x14:
                        field = (GPARAM.IField)new GPARAM.Unk_14_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x15:
                        field = (GPARAM.IField)new GPARAM.Unk_15_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x16:
                        field = (GPARAM.IField)new GPARAM.Unk_16_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x17:
                        field = (GPARAM.IField)new GPARAM.Unk_17_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x18:
                        field = (GPARAM.IField)new GPARAM.Unk_18_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x19:
                        field = (GPARAM.IField)new GPARAM.Unk_19_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x1A:
                        field = (GPARAM.IField)new GPARAM.Unk_1A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x1B:
                        field = (GPARAM.IField)new GPARAM.Unk_1B_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x1C:
                        field = (GPARAM.IField)new GPARAM.Unk_1C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x1D:
                        field = (GPARAM.IField)new GPARAM.Unk_1D_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x1E:
                        field = (GPARAM.IField)new GPARAM.Unk_1E_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x1F:
                        field = (GPARAM.IField)new GPARAM.Unk_1F_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x20:
                        field = (GPARAM.IField)new GPARAM.Unk_20_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x21:
                        field = (GPARAM.IField)new GPARAM.Unk_21_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x22:
                        field = (GPARAM.IField)new GPARAM.Unk_22_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x23:
                        field = (GPARAM.IField)new GPARAM.Unk_23_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x24:
                        field = (GPARAM.IField)new GPARAM.Unk_24_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x28:
                        field = (GPARAM.IField)new GPARAM.Unk_28_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x29:
                        field = (GPARAM.IField)new GPARAM.Unk_29_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x2A:
                        field = (GPARAM.IField)new GPARAM.Unk_2A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x2C:
                        field = (GPARAM.IField)new GPARAM.Unk_2C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x2D:
                        field = (GPARAM.IField)new GPARAM.Unk_2D_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x30:
                        field = (GPARAM.IField)new GPARAM.Unk_30_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x31:
                        field = (GPARAM.IField)new GPARAM.Unk_31_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x34:
                        field = (GPARAM.IField)new GPARAM.Unk_34_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x35:
                        field = (GPARAM.IField)new GPARAM.Unk_35_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x36:
                        field = (GPARAM.IField)new GPARAM.Unk_36_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x37:
                        field = (GPARAM.IField)new GPARAM.Unk_37_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x38:
                        field = (GPARAM.IField)new GPARAM.Unk_38_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x39:
                        field = (GPARAM.IField)new GPARAM.Unk_39_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x3A:
                        field = (GPARAM.IField)new GPARAM.Unk_3A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x3B:
                        field = (GPARAM.IField)new GPARAM.Unk_3B_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x3C:
                        field = (GPARAM.IField)new GPARAM.Unk_3C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x3D:
                        field = (GPARAM.IField)new GPARAM.Unk_3D_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x3E:
                        field = (GPARAM.IField)new GPARAM.Unk_3E_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x3F:
                        field = (GPARAM.IField)new GPARAM.Unk_3F_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x40:
                        field = (GPARAM.IField)new GPARAM.Unk_40_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x41:
                        field = (GPARAM.IField)new GPARAM.Unk_41_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x42:
                        field = (GPARAM.IField)new GPARAM.Unk_42_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x43:
                        field = (GPARAM.IField)new GPARAM.Unk_43_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x44:
                        field = (GPARAM.IField)new GPARAM.Unk_44_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x45:
                        field = (GPARAM.IField)new GPARAM.Unk_45_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x46:
                        field = (GPARAM.IField)new GPARAM.Unk_46_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x47:
                        field = (GPARAM.IField)new GPARAM.Unk_47_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x48:
                        field = (GPARAM.IField)new GPARAM.Unk_48_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x49:
                        field = (GPARAM.IField)new GPARAM.Unk_49_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x4A:
                        field = (GPARAM.IField)new GPARAM.Unk_4A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x4B:
                        field = (GPARAM.IField)new GPARAM.Unk_4B_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x4C:
                        field = (GPARAM.IField)new GPARAM.Unk_4C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x4D:
                        field = (GPARAM.IField)new GPARAM.Unk_4D_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x4E:
                        field = (GPARAM.IField)new GPARAM.Unk_4E_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x4F:
                        field = (GPARAM.IField)new GPARAM.Unk_4F_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x69:
                        field = (GPARAM.IField)new GPARAM.Unk_69_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x6C:
                        field = (GPARAM.IField)new GPARAM.Unk_6C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x78:
                        field = (GPARAM.IField)new GPARAM.Unk_78_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x7C:
                        field = (GPARAM.IField)new GPARAM.Unk_7C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x7F:
                        field = (GPARAM.IField)new GPARAM.Unk_7F_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x80:
                        field = (GPARAM.IField)new GPARAM.Unk_80_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x81:
                        field = (GPARAM.IField)new GPARAM.Unk_81_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x82:
                        field = (GPARAM.IField)new GPARAM.Unk_82_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x83:
                        field = (GPARAM.IField)new GPARAM.Unk_83_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x84:
                        field = (GPARAM.IField)new GPARAM.Unk_84_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x85:
                        field = (GPARAM.IField)new GPARAM.Unk_85_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x86:
                        field = (GPARAM.IField)new GPARAM.Unk_86_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x87:
                        field = (GPARAM.IField)new GPARAM.Unk_87_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x88:
                        field = (GPARAM.IField)new GPARAM.Unk_88_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x89:
                        field = (GPARAM.IField)new GPARAM.Unk_89_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8A:
                        field = (GPARAM.IField)new GPARAM.Unk_8A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8B:
                        field = (GPARAM.IField)new GPARAM.Unk_8B_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8C:
                        field = (GPARAM.IField)new GPARAM.Unk_8C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8D:
                        field = (GPARAM.IField)new GPARAM.Unk_8D_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8E:
                        field = (GPARAM.IField)new GPARAM.Unk_8E_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x8F:
                        field = (GPARAM.IField)new GPARAM.Unk_8F_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x90:
                        field = (GPARAM.IField)new GPARAM.Unk_90_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x91:
                        field = (GPARAM.IField)new GPARAM.Unk_91_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x92:
                        field = (GPARAM.IField)new GPARAM.Unk_92_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x93:
                        field = (GPARAM.IField)new GPARAM.Unk_93_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x94:
                        field = (GPARAM.IField)new GPARAM.Unk_94_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x95:
                        field = (GPARAM.IField)new GPARAM.Unk_95_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x96:
                        field = (GPARAM.IField)new GPARAM.Unk_96_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x97:
                        field = (GPARAM.IField)new GPARAM.Unk_97_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x98:
                        field = (GPARAM.IField)new GPARAM.Unk_98_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x99:
                        field = (GPARAM.IField)new GPARAM.Unk_99_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x9A:
                        field = (GPARAM.IField)new GPARAM.Unk_9A_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x9B:
                        field = (GPARAM.IField)new GPARAM.Unk_9B_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x9C:
                        field = (GPARAM.IField)new GPARAM.Unk_9C_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x9D:
                        field = (GPARAM.IField)new GPARAM.Unk_9D_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x9E:
                        field = (GPARAM.IField)new GPARAM.Unk_9E_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0x9F:
                        field = (GPARAM.IField)new GPARAM.Unk_9F_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA0:
                        field = (GPARAM.IField)new GPARAM.Unk_A0_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA1:
                        field = (GPARAM.IField)new GPARAM.Unk_A1_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA2:
                        field = (GPARAM.IField)new GPARAM.Unk_A2_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA3:
                        field = (GPARAM.IField)new GPARAM.Unk_A3_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA4:
                        field = (GPARAM.IField)new GPARAM.Unk_A4_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA5:
                        field = (GPARAM.IField)new GPARAM.Unk_A5_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA6:
                        field = (GPARAM.IField)new GPARAM.Unk_A6_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA7:
                        field = (GPARAM.IField)new GPARAM.Unk_A7_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA8:
                        field = (GPARAM.IField)new GPARAM.Unk_A8_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xA9:
                        field = (GPARAM.IField)new GPARAM.Unk_A9_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xAA:
                        field = (GPARAM.IField)new GPARAM.Unk_AA_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xAB:
                        field = (GPARAM.IField)new GPARAM.Unk_AB_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xAC:
                        field = (GPARAM.IField)new GPARAM.Unk_AC_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xAD:
                        field = (GPARAM.IField)new GPARAM.Unk_AD_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xAE:
                        field = (GPARAM.IField)new GPARAM.Unk_AE_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xAF:
                        field = (GPARAM.IField)new GPARAM.Unk_AF_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xB0:
                        field = (GPARAM.IField)new GPARAM.Unk_B0_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xB1:
                        field = (GPARAM.IField)new GPARAM.Unk_B1_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xB2:
                        field = (GPARAM.IField)new GPARAM.Unk_B2_Field(br, version, baseOffsets);
                        break;
                    case GPARAM.FieldType.Unk_0xB3:
                        field = (GPARAM.IField)new GPARAM.Unk_B3_Field(br, version, baseOffsets);
                        break;
                    default:
                        throw new Exception();
                }
                Console.WriteLine(enum8 + " " + field.Key);

                return field;
            }
        }

        internal interface IFieldWriteable
        {
            void Write(BinaryWriterEx bw, GPARAM.GparamVersion version, int paramIndex, int fieldIndex);

            void WriteValues(
              BinaryWriterEx bw,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex,
              int fieldIndex);

            void WriteValueIds(
              BinaryWriterEx bw,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex,
              int fieldIndex);
        }

        public abstract class Field<T> : GPARAM.IField, GPARAM.IFieldWriteable
        {
            public string Key { get; set; }

            public string Name { get; set; }

            public List<GPARAM.FieldValue<T>> Values { get; set; }

            public short Capacity { get; set; }
            public short Unk { get; set; }

            IReadOnlyList<GPARAM.IFieldValue> GPARAM.IField.Values
            {
                get => (IReadOnlyList<GPARAM.IFieldValue>)this.Values;
            }

            public Field()
            {
                this.Key = "";
                this.Name = "";
                this.Values = new List<GPARAM.FieldValue<T>>();
            }

            public override string ToString()
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
                interpolatedStringHandler.AppendFormatted(this.Key);
                interpolatedStringHandler.AppendLiteral(" [");
                interpolatedStringHandler.AppendFormatted<int>(this.Values.Count);
                interpolatedStringHandler.AppendLiteral("]");
                return interpolatedStringHandler.ToStringAndClear();
            }

            private protected abstract GPARAM.FieldType Type { get; }

            private protected Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
            {
                int num1 = br.ReadInt32();
                int num2 = br.ReadInt32();

                int num3;
                short capacity;
                if (version < GparamVersion.V6) //kisebb
                {
                    num3 = (int)br.AssertByte((byte)this.Type);
                    Capacity = br.ReadSByte();
                    Unk = br.AssertInt16(0);

                }
                else
                {
                    Capacity = br.ReadInt16();
                    num3 = (int)br.AssertByte((byte)this.Type);
                    Unk = br.ReadByte();
                    ;
                    //UnusedType = br.ReadByte();
                    //br.AssertByte(0);
                }
                capacity = Capacity;
                this.Key = br.ReadUTF16();
                this.Name = br.ReadUTF16();
                br.Position = (long)(baseOffsets.Values + num1);
                T[] objArray = new T[(int)capacity];
                for (int index = 0; index < (int)capacity; ++index)
                    objArray[index] = this.ReadValue(br);
                br.Position = (long)(baseOffsets.ValueIds + num2);
                this.Values = new List<GPARAM.FieldValue<T>>((int)capacity);
                for (int index = 0; index < (int)capacity; ++index)
                    this.Values.Add(new GPARAM.FieldValue<T>(br, version, objArray[index]));
            }

            private protected abstract T ReadValue(BinaryReaderEx br);

            void GPARAM.IFieldWriteable.Write(BinaryWriterEx bw, GPARAM.GparamVersion version, int paramIndex, int fieldIndex)
            {
                BinaryWriterEx binaryWriterEx1 = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]Field[");
                interpolatedStringHandler.AppendFormatted<int>(fieldIndex);
                interpolatedStringHandler.AppendLiteral("]ValuesOffset");
                string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
                binaryWriterEx1.ReserveInt32(stringAndClear1);
                BinaryWriterEx binaryWriterEx2 = bw;
                interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 2);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]Field[");
                interpolatedStringHandler.AppendFormatted<int>(fieldIndex);
                interpolatedStringHandler.AppendLiteral("]ValueIdsOffset");
                string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
                binaryWriterEx2.ReserveInt32(stringAndClear2);
                

                if (version < GparamVersion.V6) //kisebb
                {
                    bw.WriteByte((byte)this.Type);
                    bw.WriteSByte((sbyte)Capacity);
                    bw.WriteInt16(Unk);
                }
                else
                {
                    bw.WriteInt16(Capacity);
                    bw.WriteByte((byte)this.Type);
                    bw.WriteByte((byte)Unk);
                }

                bw.WriteUTF16(this.Key, true);
                bw.WriteUTF16(this.Name, true);
            }

            void GPARAM.IFieldWriteable.WriteValues(
              BinaryWriterEx bw,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex,
              int fieldIndex)
            {
                BinaryWriterEx binaryWriterEx = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]Field[");
                interpolatedStringHandler.AppendFormatted<int>(fieldIndex);
                interpolatedStringHandler.AppendLiteral("]ValuesOffset");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                int num = (int)bw.Position - baseOffsets.Values;
                binaryWriterEx.FillInt32(stringAndClear, num);
                foreach (GPARAM.FieldValue<T> fieldValue in this.Values)
                    this.WriteValue(bw, fieldValue.Value);
            }

            private protected abstract void WriteValue(BinaryWriterEx bw, T value);

            void GPARAM.IFieldWriteable.WriteValueIds(
              BinaryWriterEx bw,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex,
              int fieldIndex)
            {
                BinaryWriterEx binaryWriterEx = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 2);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]Field[");
                interpolatedStringHandler.AppendFormatted<int>(fieldIndex);
                interpolatedStringHandler.AppendLiteral("]ValueIdsOffset");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                int num = (int)bw.Position - baseOffsets.ValueIds;
                binaryWriterEx.FillInt32(stringAndClear, num);
                foreach (GPARAM.FieldValue<T> fieldValue in this.Values)
                    fieldValue.Write(bw, version);
            }
        }

        public class SbyteField : GPARAM.Field<sbyte>
        {
            public SbyteField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Sbyte;

            internal SbyteField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override sbyte ReadValue(BinaryReaderEx br) => br.ReadSByte();

            private protected override void WriteValue(BinaryWriterEx bw, sbyte value)
            {
                bw.WriteSByte(value);
            }
        }

        public class ShortField : GPARAM.Field<short>
        {
            public ShortField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Short;

            internal ShortField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override short ReadValue(BinaryReaderEx br) => br.ReadInt16();

            private protected override void WriteValue(BinaryWriterEx bw, short value)
            {
                bw.WriteInt16(value);
            }
        }

        public class IntField : GPARAM.Field<int>
        {
            public IntField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Int;

            internal IntField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class ByteField : GPARAM.Field<byte>
        {
            public ByteField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Byte;

            internal ByteField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override byte ReadValue(BinaryReaderEx br) => br.ReadByte();

            private protected override void WriteValue(BinaryWriterEx bw, byte value)
            {
                bw.WriteByte(value);
            }
        }

        public class UintField : GPARAM.Field<uint>
        {
            public UintField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Uint;

            internal UintField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override uint ReadValue(BinaryReaderEx br) => br.ReadUInt32();

            private protected override void WriteValue(BinaryWriterEx bw, uint value)
            {
                bw.WriteUInt32(value);
            }
        }

        public class FloatField : GPARAM.Field<float>
        {
            public FloatField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Float;

            internal FloatField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override float ReadValue(BinaryReaderEx br) => br.ReadSingle();

            private protected override void WriteValue(BinaryWriterEx bw, float value)
            {
                bw.WriteSingle(value);
            }
        }

        public class BoolField : GPARAM.Field<bool>
        {
            public BoolField()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Bool;

            internal BoolField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override bool ReadValue(BinaryReaderEx br) => br.ReadBoolean();

            private protected override void WriteValue(BinaryWriterEx bw, bool value)
            {
                bw.WriteBoolean(value);
            }
        }

        public class Vector2Field : GPARAM.Field<Vector2>
        {
            public Vector2Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Vec2;

            internal Vector2Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override Vector2 ReadValue(BinaryReaderEx br)
            {
                Vector2 vector2 = br.ReadVector2();
                br.AssertInt64(new long[1]);
                return vector2;
            }

            private protected override void WriteValue(BinaryWriterEx bw, Vector2 value)
            {
                bw.WriteVector2(value);
                bw.WriteInt64(0L);
            }
        }

        public class Vector3Field : GPARAM.Field<Vector3>
        {
            public Vector3Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Vec3;

            internal Vector3Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override Vector3 ReadValue(BinaryReaderEx br)
            {
                Vector3 vector3 = br.ReadVector3();
                br.AssertInt32(new int[1]);
                return vector3;
            }

            private protected override void WriteValue(BinaryWriterEx bw, Vector3 value)
            {
                bw.WriteVector3(value);
                bw.WriteInt32(0);
            }
        }

        public class Vector4Field : GPARAM.Field<Vector4>
        {
            public Vector4Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Vec4;

            internal Vector4Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override Vector4 ReadValue(BinaryReaderEx br) => br.ReadVector4();

            private protected override void WriteValue(BinaryWriterEx bw, Vector4 value)
            {
                bw.WriteVector4(value);
            }
        }

        public class ColorField : GPARAM.Field<Color>
        {
            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Color;

            public ColorField()
            {
            }

            internal ColorField(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override Color ReadValue(BinaryReaderEx br) => br.ReadRGBA();

            private protected override void WriteValue(BinaryWriterEx bw, Color value)
            {
                bw.WriteRGBA(value);
            }
        }
        public class Unk_4_Field : GPARAM.Field<Vector4>
        {
            public Unk_4_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4;

            internal Unk_4_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override Vector4 ReadValue(BinaryReaderEx br)
            {
                var gfg = br.ReadVector4();
                return gfg;
            }

            private protected override void WriteValue(BinaryWriterEx bw, Vector4 value)
            {
                bw.WriteVector4(value);
            }
        }

        public class Unk_6_Field : GPARAM.Field<float>
        {
            public Unk_6_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x6;

            internal Unk_6_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override float ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadSingle();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, float value)
            {
                bw.WriteSingle(value);
            }
        }

        public class Unk_8_Field : GPARAM.Field<Color>
        {
            public Unk_8_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8;

            internal Unk_8_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override Color ReadValue(BinaryReaderEx br)
            {
                var vv1 = br.ReadRGBA();
                return vv1;
            }
            private protected override void WriteValue(BinaryWriterEx bw, Color value)
            {
                bw.WriteRGBA(value);
            }
        }
        public class Unk_A_Field : GPARAM.Field<int>
        {
            public Unk_A_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA;

            internal Unk_A_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_10_Field : GPARAM.Field<int>
        {
            public Unk_10_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x10;

            internal Unk_10_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_11_Field : GPARAM.Field<int>
        {
            public Unk_11_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x11;

            internal Unk_11_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_12_Field : GPARAM.Field<int>
        {
            public Unk_12_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x12;

            internal Unk_12_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_13_Field : GPARAM.Field<int>
        {
            public Unk_13_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x13;

            internal Unk_13_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_14_Field : GPARAM.Field<int>
        {
            public Unk_14_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x14;

            internal Unk_14_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_15_Field : GPARAM.Field<int>
        {
            public Unk_15_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x15;

            internal Unk_15_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_16_Field : GPARAM.Field<int>
        {
            public Unk_16_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x16;

            internal Unk_16_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_17_Field : GPARAM.Field<int>
        {
            public Unk_17_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x17;

            internal Unk_17_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_18_Field : GPARAM.Field<int>
        {
            public Unk_18_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x18;

            internal Unk_18_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_19_Field : GPARAM.Field<int>
        {
            public Unk_19_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x19;

            internal Unk_19_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_1A_Field : GPARAM.Field<int>
        {
            public Unk_1A_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x1A;

            internal Unk_1A_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_1B_Field : GPARAM.Field<int>
        {
            public Unk_1B_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x1B;

            internal Unk_1B_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_1C_Field : GPARAM.Field<int>
        {
            public Unk_1C_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x1C;

            internal Unk_1C_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_1D_Field : GPARAM.Field<int>
        {
            public Unk_1D_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x1D;

            internal Unk_1D_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_1E_Field : GPARAM.Field<int>
        {
            public Unk_1E_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x1E;

            internal Unk_1E_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_1F_Field : GPARAM.Field<int>
        {
            public Unk_1F_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x1F;

            internal Unk_1F_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_20_Field : GPARAM.Field<int>
        {
            public Unk_20_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x20;

            internal Unk_20_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_21_Field : GPARAM.Field<int>
        {
            public Unk_21_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x21;

            internal Unk_21_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_22_Field : GPARAM.Field<int>
        {
            public Unk_22_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x22;

            internal Unk_22_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_23_Field : GPARAM.Field<int>
        {
            public Unk_23_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x23;

            internal Unk_23_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_24_Field : GPARAM.Field<int>
        {
            public Unk_24_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x24;

            internal Unk_24_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_28_Field : GPARAM.Field<int>
        {
            public Unk_28_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x28;

            internal Unk_28_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br)
            {
                var vv = br.ReadInt32();
                return vv;
            }

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_29_Field : GPARAM.Field<int>
        {
            public Unk_29_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x29;

            internal Unk_29_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_2A_Field : GPARAM.Field<int>
        {
            public Unk_2A_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x2A;

            internal Unk_2A_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_2C_Field : GPARAM.Field<int>
        {
            public Unk_2C_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x2C;

            internal Unk_2C_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_2D_Field : GPARAM.Field<int>
        {
            public Unk_2D_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x2D;

            internal Unk_2D_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_30_Field : GPARAM.Field<int>
        {
            public Unk_30_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x30;

            internal Unk_30_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_31_Field : GPARAM.Field<int>
        {
            public Unk_31_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x31;

            internal Unk_31_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_34_Field : GPARAM.Field<int>
        {
            public Unk_34_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x34;

            internal Unk_34_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_35_Field : GPARAM.Field<int>
        {
            public Unk_35_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x35;

            internal Unk_35_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_36_Field : GPARAM.Field<int>
        {
            public Unk_36_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x36;

            internal Unk_36_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_37_Field : GPARAM.Field<int>
        {
            public Unk_37_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x37;

            internal Unk_37_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_38_Field : GPARAM.Field<int>
        {
            public Unk_38_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x38;

            internal Unk_38_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_39_Field : GPARAM.Field<int>
        {
            public Unk_39_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x39;

            internal Unk_39_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_3A_Field : GPARAM.Field<int>
        {
            public Unk_3A_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x3A;

            internal Unk_3A_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_3B_Field : GPARAM.Field<int>
        {
            public Unk_3B_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x3B;

            internal Unk_3B_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_3C_Field : GPARAM.Field<int>
        {
            public Unk_3C_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x3C;

            internal Unk_3C_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_3D_Field : GPARAM.Field<int>
        {
            public Unk_3D_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x3D;

            internal Unk_3D_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_3E_Field : GPARAM.Field<int>
        {
            public Unk_3E_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x3E;

            internal Unk_3E_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_3F_Field : GPARAM.Field<int>
        {
            public Unk_3F_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x3F;

            internal Unk_3F_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_40_Field : GPARAM.Field<int>
        {
            public Unk_40_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x40;

            internal Unk_40_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_41_Field : GPARAM.Field<int>
        {
            public Unk_41_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x41;

            internal Unk_41_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_42_Field : GPARAM.Field<int>
        {
            public Unk_42_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x42;

            internal Unk_42_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_43_Field : GPARAM.Field<int>
        {
            public Unk_43_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x43;

            internal Unk_43_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_44_Field : GPARAM.Field<int>
        {
            public Unk_44_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x44;

            internal Unk_44_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_45_Field : GPARAM.Field<int>
        {
            public Unk_45_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x45;

            internal Unk_45_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }


        public class Unk_46_Field : GPARAM.Field<int>
        {
            public Unk_46_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x46;

            internal Unk_46_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_47_Field : GPARAM.Field<int>
        {
            public Unk_47_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x47;

            internal Unk_47_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_48_Field : GPARAM.Field<int>
        {
            public Unk_48_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x48;

            internal Unk_48_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_49_Field : GPARAM.Field<int>
        {
            public Unk_49_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x49;

            internal Unk_49_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_4A_Field : GPARAM.Field<int>
        {
            public Unk_4A_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4A;

            internal Unk_4A_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_4B_Field : GPARAM.Field<int>
        {
            public Unk_4B_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4B;

            internal Unk_4B_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_4C_Field : GPARAM.Field<int>
        {
            public Unk_4C_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4C;

            internal Unk_4C_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_4D_Field : GPARAM.Field<int>
        {
            public Unk_4D_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4D;

            internal Unk_4D_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_4E_Field : GPARAM.Field<int>
        {
            public Unk_4E_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4E;

            internal Unk_4E_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_4F_Field : GPARAM.Field<int>
        {
            public Unk_4F_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x4F;

            internal Unk_4F_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_69_Field : GPARAM.Field<int>
        {
            public Unk_69_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x69;

            internal Unk_69_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_6C_Field : GPARAM.Field<int>
        {
            public Unk_6C_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x6C;

            internal Unk_6C_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_78_Field : GPARAM.Field<int>
        {
            public Unk_78_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x78;

            internal Unk_78_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_7C_Field : GPARAM.Field<int>
        {
            public Unk_7C_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x7C;

            internal Unk_7C_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_7F_Field : GPARAM.Field<int>
        {
            public Unk_7F_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x7F;

            internal Unk_7F_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_80_Field : GPARAM.Field<int>
        {
            public Unk_80_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x80;

            internal Unk_80_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }
        public class Unk_82_Field : GPARAM.Field<int>
        {
            public Unk_82_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x82;

            internal Unk_82_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_8B_Field : GPARAM.Field<int>
        {
            public Unk_8B_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8B;

            internal Unk_8B_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_81_Field : GPARAM.Field<int>
        {
            public Unk_81_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x81;

            internal Unk_81_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_83_Field : GPARAM.Field<int>
        {
            public Unk_83_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x83;

            internal Unk_83_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_84_Field : GPARAM.Field<int>
        {
            public Unk_84_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x84;

            internal Unk_84_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_85_Field : GPARAM.Field<int>
        {
            public Unk_85_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x85;

            internal Unk_85_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_86_Field : GPARAM.Field<int>
        {
            public Unk_86_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x86;

            internal Unk_86_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_87_Field : GPARAM.Field<int>
        {
            public Unk_87_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x87;

            internal Unk_87_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_88_Field : GPARAM.Field<int>
        {
            public Unk_88_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x88;

            internal Unk_88_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_89_Field : GPARAM.Field<int>
        {
            public Unk_89_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x89;

            internal Unk_89_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_8A_Field : GPARAM.Field<int>
        {
            public Unk_8A_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8A;

            internal Unk_8A_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_8C_Field : GPARAM.Field<int>
        {
            public Unk_8C_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8C;

            internal Unk_8C_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_8D_Field : GPARAM.Field<int>
        {
            public Unk_8D_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8D;

            internal Unk_8D_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_8E_Field : GPARAM.Field<int>
        {
            public Unk_8E_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8E;

            internal Unk_8E_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_8F_Field : GPARAM.Field<int>
        {
            public Unk_8F_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x8F;

            internal Unk_8F_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_90_Field : GPARAM.Field<int>
        {
            public Unk_90_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x90;

            internal Unk_90_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_91_Field : GPARAM.Field<int>
        {
            public Unk_91_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x91;

            internal Unk_91_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_9D_Field : GPARAM.Field<int>
        {
            public Unk_9D_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x9D;

            internal Unk_9D_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }

        public class Unk_92_Field : GPARAM.Field<int>
        {
            public Unk_92_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x92;

            internal Unk_92_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_93_Field : GPARAM.Field<int>
        {
            public Unk_93_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x93;

            internal Unk_93_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_94_Field : GPARAM.Field<int>
        {
            public Unk_94_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x94;

            internal Unk_94_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_95_Field : GPARAM.Field<int>
        {
            public Unk_95_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x95;

            internal Unk_95_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_96_Field : GPARAM.Field<int>
        {
            public Unk_96_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x96;

            internal Unk_96_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_97_Field : GPARAM.Field<int>
        {
            public Unk_97_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x97;

            internal Unk_97_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_98_Field : GPARAM.Field<int>
        {
            public Unk_98_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x98;

            internal Unk_98_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_99_Field : GPARAM.Field<int>
        {
            public Unk_99_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x99;

            internal Unk_99_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_9A_Field : GPARAM.Field<int>
        {
            public Unk_9A_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x9A;

            internal Unk_9A_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_9B_Field : GPARAM.Field<int>
        {
            public Unk_9B_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x9B;

            internal Unk_9B_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_9C_Field : GPARAM.Field<int>
        {
            public Unk_9C_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x9C;

            internal Unk_9C_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_9E_Field : GPARAM.Field<int>
        {
            public Unk_9E_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x9E;

            internal Unk_9E_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_9F_Field : GPARAM.Field<int>
        {
            public Unk_9F_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0x9F;

            internal Unk_9F_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A0_Field : GPARAM.Field<int>
        {
            public Unk_A0_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA0;

            internal Unk_A0_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A1_Field : GPARAM.Field<int>
        {
            public Unk_A1_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA1;

            internal Unk_A1_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A2_Field : GPARAM.Field<int>
        {
            public Unk_A2_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA2;

            internal Unk_A2_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A3_Field : GPARAM.Field<int>
        {
            public Unk_A3_Field()
            {
            }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA3;

            internal Unk_A3_Field(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
              : base(br, version, baseOffsets)
            {
            }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value)
            {
                bw.WriteInt32(value);
            }
        }


        public class Unk_A4_Field : GPARAM.Field<int>
        {
            public Unk_A4_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA4;

            internal Unk_A4_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A5_Field : GPARAM.Field<int>
        {
            public Unk_A5_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA5;

            internal Unk_A5_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A6_Field : GPARAM.Field<int>
        {
            public Unk_A6_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA6;

            internal Unk_A6_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A7_Field : GPARAM.Field<int>
        {
            public Unk_A7_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA7;

            internal Unk_A7_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A8_Field : GPARAM.Field<int>
        {
            public Unk_A8_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA8;

            internal Unk_A8_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_A9_Field : GPARAM.Field<int>
        {
            public Unk_A9_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xA9;

            internal Unk_A9_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_AA_Field : GPARAM.Field<int>
        {
            public Unk_AA_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xAA;

            internal Unk_AA_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_AB_Field : GPARAM.Field<int>
        {
            public Unk_AB_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xAB;

            internal Unk_AB_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_AC_Field : GPARAM.Field<int>
        {
            public Unk_AC_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xAC;

            internal Unk_AC_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_AD_Field : GPARAM.Field<int>
        {
            public Unk_AD_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xAD;

            internal Unk_AD_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_AE_Field : GPARAM.Field<int>
        {
            public Unk_AE_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xAE;

            internal Unk_AE_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_AF_Field : GPARAM.Field<int>
        {
            public Unk_AF_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xAF;

            internal Unk_AF_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_B0_Field : GPARAM.Field<int>
        {
            public Unk_B0_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xB0;

            internal Unk_B0_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_B1_Field : GPARAM.Field<int>
        {
            public Unk_B1_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xB1;

            internal Unk_B1_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_B2_Field : GPARAM.Field<int>
        {
            public Unk_B2_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xB2;

            internal Unk_B2_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public class Unk_B3_Field : GPARAM.Field<int>
        {
            public Unk_B3_Field() { }

            private protected override GPARAM.FieldType Type => GPARAM.FieldType.Unk_0xB3;

            internal Unk_B3_Field(BinaryReaderEx br, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets)
                : base(br, version, baseOffsets) { }

            private protected override int ReadValue(BinaryReaderEx br) => br.ReadInt32();

            private protected override void WriteValue(BinaryWriterEx bw, int value) => bw.WriteInt32(value);
        }

        public interface IFieldValue
        {
            int Id { get; set; }

            float Unk04 { get; set; }

            object Value { get; set; }
        }

        public class FieldValue<T> : GPARAM.IFieldValue
        {
            public int Id { get; set; }

            public float Unk04 { get; set; }

            public T Value { get; set; }

            object GPARAM.IFieldValue.Value
            {
                get
                {
                   return (object)this.Value;
                }
                set
                {
                    this.Value = (T)value;
                }
            }

            public FieldValue()
            {
            }

            public override string ToString()
            {
                if ((double)this.Unk04 != 0.0)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
                    interpolatedStringHandler.AppendFormatted<int>(this.Id);
                    interpolatedStringHandler.AppendLiteral(" (");
                    interpolatedStringHandler.AppendFormatted<float>(this.Unk04);
                    interpolatedStringHandler.AppendLiteral(") = ");
                    interpolatedStringHandler.AppendFormatted<T>(this.Value);
                    return interpolatedStringHandler.ToStringAndClear();
                }
                DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(3, 2);
                interpolatedStringHandler1.AppendFormatted<int>(this.Id);
                interpolatedStringHandler1.AppendLiteral(" = ");
                interpolatedStringHandler1.AppendFormatted<T>(this.Value);
                return interpolatedStringHandler1.ToStringAndClear();
            }

            internal FieldValue(BinaryReaderEx br, GPARAM.GparamVersion version, T value)
            {
                this.Id = br.ReadInt32();
                if (version >= GPARAM.GparamVersion.V5)
                    this.Unk04 = br.ReadSingle();
                this.Value = value;
            }

            internal void Write(BinaryWriterEx bw, GPARAM.GparamVersion version)
            {
                bw.WriteInt32(this.Id);
                if (version < GPARAM.GparamVersion.V5)
                    return;
                bw.WriteSingle(this.Unk04);
            }
        }

        public enum GparamVersion : uint
        {
            V3 = 3,
            V5 = 5,
            V6 = 6
        }

        internal struct BaseOffsets
        {
            public int ParamOffsets;
            public int Params;
            public int FieldOffsets;
            public int Fields;
            public int Values;
            public int ValueIds;
            public int Unk30;
            public int ParamExtras;
            public int ParamExtraIds;
            public int ParamCommentsOffsets;
            public int CommentOffsets;
            public int Comments;
        }

        public class Param
        {
            public List<GPARAM.IField> Fields { get; set; }

            public string Key { get; set; }

            public string Name { get; set; }

            public List<string> Comments { get; set; }

            public Param()
            {
                this.Fields = new List<GPARAM.IField>();
                this.Key = "";
                this.Name = "";
                this.Comments = new List<string>();
            }

            public override string ToString()
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
                interpolatedStringHandler.AppendFormatted(this.Key);
                interpolatedStringHandler.AppendLiteral(" [");
                interpolatedStringHandler.AppendFormatted<int>(this.Fields.Count);
                interpolatedStringHandler.AppendLiteral("]");
                return interpolatedStringHandler.ToStringAndClear();
            }

            internal Param(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
            {
                int num1 = br.ReadInt32();
                int num2 = br.ReadInt32();
                this.Key = br.ReadUTF16();
                this.Name = br.ReadUTF16();
                int[] int32s = br.GetInt32s((long)(baseOffsets.FieldOffsets + num2), num1);
                this.Fields = new List<GPARAM.IField>(num1);
                foreach (int num3 in int32s)
                {
                    br.Position = (long)(baseOffsets.Fields + num3);
                    this.Fields.Add(GPARAM.IField.Read(br, version, baseOffsets));
                }
            }

            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt32(this.Fields.Count);
                BinaryWriterEx binaryWriterEx = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(index);
                interpolatedStringHandler.AppendLiteral("]FieldOffsetsOffset");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                binaryWriterEx.ReserveInt32(stringAndClear);
                bw.WriteUTF16(this.Key, true);
                bw.WriteUTF16(this.Name, true);
            }

            internal void WriteFieldOffsets(
              BinaryWriterEx bw,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex)
            {
                BinaryWriterEx binaryWriterEx1 = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]FieldOffsetsOffset");
                string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
                int num = (int)bw.Position - baseOffsets.FieldOffsets;
                binaryWriterEx1.FillInt32(stringAndClear1, num);
                for (int index = 0; index < this.Fields.Count; ++index)
                {
                    BinaryWriterEx binaryWriterEx2 = bw;
                    interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
                    interpolatedStringHandler.AppendLiteral("Param[");
                    interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                    interpolatedStringHandler.AppendLiteral("]Field[");
                    interpolatedStringHandler.AppendFormatted<int>(index);
                    interpolatedStringHandler.AppendLiteral("]Offset");
                    string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
                    binaryWriterEx2.ReserveInt32(stringAndClear2);
                }
            }

            internal void WriteFields(BinaryWriterEx bw, GPARAM.GparamVersion version, GPARAM.BaseOffsets baseOffsets, int paramIndex)
            {
                for (int index = 0; index < this.Fields.Count; ++index)
                {
                    BinaryWriterEx binaryWriterEx = bw;
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
                    interpolatedStringHandler.AppendLiteral("Param[");
                    interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                    interpolatedStringHandler.AppendLiteral("]Field[");
                    interpolatedStringHandler.AppendFormatted<int>(index);
                    interpolatedStringHandler.AppendLiteral("]Offset");
                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    int num = (int)bw.Position - baseOffsets.Fields;
                    binaryWriterEx.FillInt32(stringAndClear, num);
                    ((GPARAM.IFieldWriteable)this.Fields[index]).Write(bw, version, paramIndex, index);
                    bw.Pad(4);
                }
            }

            internal void WriteValues(BinaryWriterEx bw, GPARAM.BaseOffsets baseOffsets, int paramIndex)
            {
                for (int index = 0; index < this.Fields.Count; ++index)
                {
                    ((GPARAM.IFieldWriteable)this.Fields[index]).WriteValues(bw, baseOffsets, paramIndex, index);
                    bw.Pad(4);
                }
            }

            internal void WriteValueIds(
              BinaryWriterEx bw,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex)
            {
                for (int index = 0; index < this.Fields.Count; ++index)
                    ((GPARAM.IFieldWriteable)this.Fields[index]).WriteValueIds(bw, version, baseOffsets, paramIndex, index);
            }

            internal void WriteCommentOffsetsOffset(BinaryWriterEx bw, int paramIndex)
            {
                BinaryWriterEx binaryWriterEx = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]CommentOffsetsOffset");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                binaryWriterEx.ReserveInt32(stringAndClear);
            }

            internal void WriteCommentOffsets(
              BinaryWriterEx bw,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex)
            {
                BinaryWriterEx binaryWriterEx1 = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
                interpolatedStringHandler.AppendLiteral("Param[");
                interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                interpolatedStringHandler.AppendLiteral("]CommentOffsetsOffset");
                string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
                int num = (int)bw.Position - baseOffsets.CommentOffsets;
                binaryWriterEx1.FillInt32(stringAndClear1, num);
                for (int index = 0; index < this.Comments.Count; ++index)
                {
                    BinaryWriterEx binaryWriterEx2 = bw;
                    interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
                    interpolatedStringHandler.AppendLiteral("Param[");
                    interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                    interpolatedStringHandler.AppendLiteral("]Comment[");
                    interpolatedStringHandler.AppendFormatted<int>(index);
                    interpolatedStringHandler.AppendLiteral("]Offset");
                    string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
                    binaryWriterEx2.ReserveInt32(stringAndClear2);
                }
            }

            internal void WriteComments(
              BinaryWriterEx bw,
              GPARAM.BaseOffsets baseOffsets,
              int paramIndex)
            {
                for (int index = 0; index < this.Comments.Count; ++index)
                {
                    BinaryWriterEx binaryWriterEx = bw;
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
                    interpolatedStringHandler.AppendLiteral("Param[");
                    interpolatedStringHandler.AppendFormatted<int>(paramIndex);
                    interpolatedStringHandler.AppendLiteral("]Comment[");
                    interpolatedStringHandler.AppendFormatted<int>(index);
                    interpolatedStringHandler.AppendLiteral("]Offset");
                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    int num = (int)bw.Position - baseOffsets.Comments;
                    binaryWriterEx.FillInt32(stringAndClear, num);
                    bw.WriteUTF16(this.Comments[index], true);
                    bw.Pad(4);
                }
            }
        }

        public class UnkParamExtra
        {
            // group index
            public int Unk00 { get; set; }

            public List<int> Ids { get; set; }

            public int Unk0c { get; set; }

            public UnkParamExtra() => this.Ids = new List<int>();

            internal UnkParamExtra(
              BinaryReaderEx br,
              GPARAM.GparamVersion version,
              GPARAM.BaseOffsets baseOffsets)
            {
                this.Unk00 = br.ReadInt32();
                int count = br.ReadInt32();
                int num = br.ReadInt32();
                if (version >= GPARAM.GparamVersion.V5)
                    this.Unk0c = br.ReadInt32();
                this.Ids = Enumerable.ToList<int>((IEnumerable<int>)br.GetInt32s((long)(baseOffsets.ParamExtraIds + num), count));
            }

            internal void Write(BinaryWriterEx bw, GPARAM.GparamVersion version, int index)
            {
                bw.WriteInt32(this.Unk00);
                bw.WriteInt32(this.Ids.Count);
                BinaryWriterEx binaryWriterEx = bw;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
                interpolatedStringHandler.AppendLiteral("ParamExtra[");
                interpolatedStringHandler.AppendFormatted<int>(index);
                interpolatedStringHandler.AppendLiteral("]IdsOffset");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                binaryWriterEx.ReserveInt32(stringAndClear);
                if (version < GPARAM.GparamVersion.V5)
                    return;
                bw.WriteInt32(this.Unk0c);
            }

            internal void WriteIds(BinaryWriterEx bw, GPARAM.BaseOffsets baseOffsets, int index)
            {
                if (this.Ids.Count == 0)
                {
                    BinaryWriterEx binaryWriterEx = bw;
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
                    interpolatedStringHandler.AppendLiteral("ParamExtra[");
                    interpolatedStringHandler.AppendFormatted<int>(index);
                    interpolatedStringHandler.AppendLiteral("]IdsOffset");
                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    binaryWriterEx.FillInt32(stringAndClear, 0);
                }
                else
                {
                    BinaryWriterEx binaryWriterEx = bw;
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
                    interpolatedStringHandler.AppendLiteral("ParamExtra[");
                    interpolatedStringHandler.AppendFormatted<int>(index);
                    interpolatedStringHandler.AppendLiteral("]IdsOffset");
                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    int num = (int)bw.Position - baseOffsets.ParamExtraIds;
                    binaryWriterEx.FillInt32(stringAndClear, num);
                    bw.WriteInt32s((IList<int>)this.Ids);
                }
            }
        }
    }
}
