using System;

namespace SoulsFormats
{
    public class METAPARAM : SoulsFile<METAPARAM>
    {
        /// <summary>
        /// Available types for param values.
        /// </summary>
        public enum ParamType : byte
        {
            /// <summary>
            /// (bool) A 1-byte boolean.
            /// </summary>
            Bool = 0,

            /// <summary>
            /// (float) A 32-bit float.
            /// </summary>
            Float = 1,

            /// <summary>
            /// (float[2]) Two 32-bit floats.
            /// </summary>
            Float2 = 2,

            /// <summary>
            /// (float[3]) Three 32-bit floats.
            /// </summary>
            Float3 = 10,

            /// <summary>
            /// (float[4]) Four 32-bit floats.
            /// </summary>
            Float4 = 11,

            /// <summary>
            /// (float[5]) Five 32-bit floats.
            /// </summary>
            Float5 = 13,
        }
        public class Sampler
        {
            public string Name { get; set; }
            public string Group { get; set; }
            public string Texture { get; set; }
            public byte[] GodKnows { get; set; }

            internal Sampler(BinaryReaderEx br, byte i)
            {
                Name = br.GetUTF16(br.ReadInt64());

                br.AssertByte(16);
                br.AssertByte(i);

                short unkA1 = br.ReadInt16();
                br.AssertInt32(-1);

                Texture = br.GetUTF16(br.ReadInt64());
                Group = br.GetUTF16(br.ReadInt64());

                GodKnows = br.ReadBytes(4);
                br.AssertInt64(0);
                br.AssertInt32(0);
            }
        }
        public class Param
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public byte[] GodKnows { get; set; }
            public ParamType Type { get; set; }

            internal Param(BinaryReaderEx br)
            {
                Name = br.GetUTF16(br.ReadInt64());
                br.AssertPattern(28, 0);

                int vmi1 = br.ReadInt32();
                int vmi2 = br.ReadInt32();
                //byte vmi3 = br.ReadByte(); //enum
                Type = br.ReadEnum8<ParamType>();
                br.AssertByte(0, 2);
                byte vmi4 = br.ReadByte();
                byte vmi5 = br.ReadByte();
                GodKnows = br.ReadBytes(4);

                switch (Type)
                {
                    case ParamType.Float: Value = br.ReadSingle(); br.AssertInt64(0); br.AssertInt64(0); br.AssertInt64(0); break;
                    case ParamType.Float2: Value = br.ReadSingles(2); br.AssertInt64(0); br.AssertInt64(0); br.AssertInt32(0); break;
                    case ParamType.Float5: Value = br.ReadSingles(5); br.AssertInt64(0); break;

                    default:
                        throw new NotImplementedException($"Unimplemented value type: {Type}");
                }
            }
        }
        public Sampler[] Textures { get; set; }
        public Param[] Params { get; set; }

        public METAPARAM()
        {

        }

        public override bool Validate(out Exception ex)
        {
            return base.Validate(out ex);
        }

        protected override bool Is(BinaryReaderEx br)
        {
            return base.Is(br);
        }

        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("SMD\0");
            br.AssertInt32(0);
            br.AssertInt32(6);
            var texCount = br.ReadInt32(); //texture count ?
            Textures = new Sampler[texCount];
            long offset1 = br.ReadInt64(); //could be 2 bytes, then a short being 0 always
            /*long offset2 = br.ReadInt64(); //could be 2 bytes, then a short being 0 always
            long offset3 = br.ReadInt64();*/ //could be 2 bytes, then a short being 0 always. all 3 seems to eb the same. maybe count values?
            br.AssertInt64(offset1);
            br.AssertInt64(offset1);
            br.AssertInt32(0);

            var paramCount = br.ReadInt32(); 
            Params = new Param[paramCount];

            var unk30 = br.ReadInt32(); //1025 in all of them
            var unk34 = br.ReadInt32();

            br.AssertInt64(0, 4096); //enum? 38
            br.AssertInt64(2);

            long colorLength = br.ReadInt64();
            br.AssertInt64(17); //oddly specific

            byte Unk58 = br.ReadByte();
            byte Unk59 = br.ReadByte();
            br.AssertByte(0);
            byte Unk5B = br.ReadByte();

            br.AssertPattern(60, 0);

            for (byte i = 0; i < texCount; i++)
            {
                Textures[i] = new Sampler(br, i);
            }

            int offset4 = br.ReadInt32();
            br.AssertInt32(0);
            br.AssertInt32(5);
            br.AssertPattern(28, 0);

            if (br.Position != offset1)
            {
                throw new Exception();
            }
            for (int i = 0; i < paramCount; i++)
            {
                Params[i] = new Param(br);
            }
            if (br.Position != offset4)
            {
                throw new Exception();
            }

            br.AssertInt64(2);

            byte UnkEnd1 = br.ReadByte();
            byte UnkEnd2 = br.ReadByte();
            br.AssertInt16(0);
            br.AssertInt64(0);
            br.AssertInt32(0);
            br.AssertInt64(1);

            for (int i = 6; i >= 4; i--)
            {
                br.AssertByte(UnkEnd1);
                br.AssertByte(UnkEnd2);
                br.AssertInt16(0);
                br.AssertInt64(0);
                br.AssertInt32(0);
                br.AssertInt64(i);
            }
            br.AssertInt64(0);
            br.AssertInt64(0);
            ;

        }

        protected override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
