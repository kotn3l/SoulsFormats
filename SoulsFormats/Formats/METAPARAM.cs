using System;

namespace SoulsFormats
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
    public class MPTexture
    {
        public string Key { get; set; }
        public string Group { get; set; }
        public string Texture { get; set; }
        public byte[] GodKnows { get; set; }

        public MPTexture(BinaryReaderEx br, byte i)
        {
            long strOffset = br.ReadInt64(); //98
            br.StepIn(strOffset);
            {
                Key = br.ReadUTF16();
            }
            br.StepOut();

            br.AssertByte(16);
            br.AssertByte(i);

            short UnkA1 = br.ReadInt16();
            br.AssertInt32(-1);

            long name1 = br.ReadInt64(); //str offset again
            br.StepIn(name1);
            {
                Texture = br.ReadUTF16();
            }
            br.StepOut();

            long name2 = br.ReadInt64(); //str offset again
            br.StepIn(name2);
            {
                Group = br.ReadUTF16();
            }
            br.StepOut();

            //var f = br.ReadDouble();
            GodKnows = br.ReadBytes(4);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
        }
    }
    public class MPParams
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public byte[] GodKnows { get; set; }
        public ParamType Type { get; set; }

        public MPParams(BinaryReaderEx br)
        {
            Key = br.GetUTF16(br.ReadInt64());
            for (int i = 0; i < 7; i++)
            {
                br.AssertInt32(0);
            }

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


            //Value = br.ReadSingles(5);
            
        }
    }

    public class METAPARAM : SoulsFile<METAPARAM>
    {
        public MPTexture[] Textures { get; set; }
        public MPParams[] Params { get; set; }
        public short Unk10 { get; set; }
        public short Unk18 { get; set; }
        public short Unk20 { get; set; }
        public short Unk2C { get; set; }
        public short Unk30 { get; set; }
        public short Unk34 { get; set; }
        public int ColorCount { get; set; }

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
            var texCount = br.ReadInt16(); //texture count ?
            Textures = new MPTexture[texCount];
            br.AssertInt16(0);
            long offset1 = br.ReadInt64(); //could be 2 bytes, then a short being 0 always
            long offset2 = br.ReadInt64(); //could be 2 bytes, then a short being 0 always
            long offset3 = br.ReadInt64(); //could be 2 bytes, then a short being 0 always. all 3 seems to eb the same. maybe count values?
            br.AssertInt32(0);

            var paramCount = br.ReadInt16(); 
            br.AssertInt16(0);

            Params = new MPParams[paramCount];

            Unk30 = br.ReadInt16();
            br.AssertInt16(0);

            Unk34 = br.ReadInt16();
            br.AssertInt16(0);

            br.AssertInt64(0, 4096); //enum? 38

            br.AssertInt64(2);

            long colorLength = br.ReadInt64();

            br.AssertInt64(17); //oddly specific

            byte Unk58 = br.ReadByte();
            byte Unk59 = br.ReadByte();
            br.AssertByte(0);
            byte Unk5B = br.ReadByte();

            for (int i = 0; i < 15; i++)
            {
                br.AssertInt32(0);
            }

            for (byte i = 0; i < texCount; i++)
            {
                Textures[i] = new MPTexture(br, i);
            }

            int offset4 = br.ReadInt32();
            br.AssertInt32(0);
            br.AssertInt32(5);
            for (int i = 0; i < 7; i++)
            {
                br.AssertInt32(0);
            }

            if (br.Position != offset1)
            {
                throw new Exception();
            }

            if (br.Position != offset4)
            {               
                for (int i = 0; i < paramCount; i++)
                {
                    Params[i] = new MPParams(br);
                }
                /*int c = 0;
                while (br.Position < offset1 + colorLength)
                {
                    Params[c] = new MPParams(br);
                    c++;
                }*/

                if (br.Position != offset4)
                {
                    throw new Exception();
                }

                ;
            }



            
            ;

        }

        protected override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
