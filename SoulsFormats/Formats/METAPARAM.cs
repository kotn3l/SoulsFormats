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

                br.AssertByte(16, 17);
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
                    case ParamType.Bool: Value = br.ReadBoolean(); br.AssertByte(0); br.AssertInt16(0); br.AssertInt64(0); br.AssertInt64(0); br.AssertInt64(0); break;
                    case ParamType.Float: Value = br.ReadSingle(); br.AssertInt64(0); br.AssertInt64(0); br.AssertInt64(0); break;
                    case ParamType.Float2: Value = br.ReadSingles(2); br.AssertInt64(0); br.AssertInt64(0); br.AssertInt32(0); break;
                    case ParamType.Float5: Value = br.ReadSingles(5); br.AssertInt64(0); break;

                    default:
                        throw new NotImplementedException($"Unimplemented value type: {Type}");
                }
            }
        }
        public class MPObject
        {
            public string Name { get; set; }
            public MPObject(BinaryReaderEx br)
            {
                Name = br.GetUTF16(br.ReadInt64());
                br.AssertPattern(28, 0);

                var unk1 = br.ReadInt32();
                var unk2 = br.ReadInt32();
                var unk3 = br.ReadInt32();

                var unk35 = br.ReadInt32();
                //br.AssertInt32(0, 1, 2, 3); //sometimes 1
                br.AssertInt16(0);

                var unk4 = br.ReadByte();
                var unk5 = br.ReadByte();
                var unk6 = br.ReadInt32();

                var godknows = br.ReadBytes(4);
                br.AssertPattern(32, 0);
            }
        }
        public Sampler[] Textures { get; set; }
        public Param[] Params { get; set; }
        public MPObject[] Objects { get; set; }
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
            var texCount = br.ReadUInt32();
            Textures = new Sampler[texCount];
            long textureEndOffset = br.ReadInt64(); 
            br.AssertInt64(textureEndOffset); //some other offset, it was always the same as the first one tho. params dont seem to have different offsets
            
            long objectOffset = br.ReadInt64(); //noidea object end ???
            var objectCount = br.ReadUInt32();
            Objects = new MPObject[objectCount];

            var paramCount = br.ReadUInt32(); 
            Params = new Param[paramCount];

            br.AssertInt32(1025); //1025 in all of them
            var unk34 = br.ReadInt32();

            long unk38 = br.ReadInt64(); //enum? 0, 4096, 4112. 
            br.AssertInt64(2);

            int unk48 = br.ReadInt32();
            int objectLength = br.ReadInt32();
            br.AssertInt32(17); //oddly specific
            int unk54 = br.ReadInt32();

            var Unk58 = br.ReadBytes(4);
            br.AssertPattern(60, 0);

            for (byte i = 0; i < texCount; i++)
            {
                Textures[i] = new Sampler(br, i);
            }

            long offset4 = br.ReadInt64();
            br.AssertInt32(5);
            br.AssertPattern(28, 0);

            if (br.Position != textureEndOffset)
            {
                throw new Exception();
            }

            for (int i = 0; i < objectCount; i++)
            {
                Objects[i] = new MPObject(br);
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
            short Unknew = br.ReadInt16();
            br.AssertInt32(0);
            int Unknew2 = br.ReadInt32();
            br.AssertInt32(0);
            br.AssertInt64(1);

            for (int i = 6; i >= 4; i--)
            {
                br.AssertByte(UnkEnd1);
                br.AssertByte(UnkEnd2);
                br.AssertInt16(Unknew);
                br.AssertInt32(0);
                br.AssertInt32(Unknew2);
                br.AssertInt32(0);
                br.AssertInt64(i);
            }
            br.AssertInt64(1, 0, UnkEnd1);
            br.AssertInt64(0, Unknew2);
            ;

        }

        protected override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
