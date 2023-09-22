using System;

namespace SoulsFormats
{
    public class MPObject
    {
        public string Key { get; set; }
        public string Group { get; set; }
        public string Texture { get; set; }
        public byte[] GodKnows { get; set; }

        public MPObject(BinaryReaderEx br, byte i)
        {
            int strOffset = br.ReadInt32(); //98
            br.StepIn(strOffset);
            {
                Key = br.ReadUTF16();
            }
            br.StepOut();
            br.AssertInt32(0);

            br.AssertByte(16);
            br.AssertByte(i);

            short UnkA1 = br.ReadInt16();
            br.AssertInt32(-1);

            int name1 = br.ReadInt32(); //str offset again
            br.StepIn(name1);
            {
                Texture = br.ReadUTF16();
            }
            br.StepOut();
            br.AssertInt32(0);

            int name2 = br.ReadInt32(); //str offset again
            br.StepIn(name2);
            {
                Group = br.ReadUTF16();
            }
            br.StepOut();
            br.AssertInt32(0);

            //var f = br.ReadDouble();
            GodKnows = br.ReadBytes(4);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
        }
    }


    public class METAPARAM : SoulsFile<METAPARAM>
    {
        public short TextureCount { get; set; }
        public MPObject[] Objects { get; set; }
        public short Unk10 { get; set; }
        public short Unk18 { get; set; }
        public short Unk20 { get; set; }
        public short Unk2C { get; set; }
        public short Unk30 { get; set; }
        public short Unk34 { get; set; }
        public int Unk48 { get; set; }

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
            TextureCount = br.ReadInt16(); //texture count ?
            Objects = new MPObject[TextureCount];
            br.AssertInt16(0);
            var offset1 = br.ReadInt16(); //could be 2 bytes, then a short being 0 always
            br.AssertInt16(0);
            br.AssertInt32(0);
            var offset2 = br.ReadInt16(); //could be 2 bytes, then a short being 0 always
            br.AssertInt16(0);
            br.AssertInt32(0);
            var offset3 = br.ReadInt16(); //could be 2 bytes, then a short being 0 always. all 3 seems to eb the same. maybe count values?
            br.AssertInt16(0);
            br.AssertInt32(0);
            br.AssertInt32(0); //28. those 3 repeating shit might be offsets? header length maybe.

            Unk2C = br.ReadInt16(); //some count
            br.AssertInt16(0);

            Unk30 = br.ReadInt16();
            br.AssertInt16(0);

            Unk34 = br.ReadInt16();
            br.AssertInt16(0);

            br.AssertInt32(0, 4096); //enum? 38
            br.AssertInt32(0);

            br.AssertInt32(2);
            br.AssertInt32(0);

            Unk48 = br.ReadInt32();
            br.AssertInt32(0);

            br.AssertInt32(17); //oddly specific
            br.AssertInt32(0);

            byte Unk58 = br.ReadByte();
            byte Unk59 = br.ReadByte();
            br.AssertByte(0);
            byte Unk5B = br.ReadByte();

            for (int i = 0; i < 15; i++)
            {
                br.AssertInt32(0);
            }

            for (byte i = 0; i < TextureCount; i++)
            {
                Objects[i] = new MPObject(br, i);
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

            br.StepIn(offset4);
            {

            }
            br.StepOut();
            ;

        }

        protected override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
