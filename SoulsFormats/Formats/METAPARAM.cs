using System;

namespace SoulsFormats
{
    public class METAPARAM : SoulsFile<METAPARAM>
    {
        public short TextureCount { get; set; }
        public short Unk10 { get; set; }
        public short Unk18 { get; set; }
        public short Unk20 { get; set; }
        public short Unk2C { get; set; }
        public short Unk30 { get; set; }
        public short Unk34 { get; set; }
        public short Unk48 { get; set; }

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
            br.AssertInt16(0);
            Unk10 = br.ReadInt16(); //could be 2 bytes, then a short being 0 always
            br.AssertInt16(0);
            br.AssertInt32(0);
            Unk18 = br.ReadInt16(); //could be 2 bytes, then a short being 0 always
            br.AssertInt16(0);
            br.AssertInt32(0);
            Unk20 = br.ReadInt16(); //could be 2 bytes, then a short being 0 always. all 3 seems to eb the same. maybe count values?
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

            Unk48 = br.ReadInt16();
            br.AssertInt16(0);
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

            short someOffset = br.ReadInt16(); //98
            br.AssertInt16(0);
            br.AssertInt32(0);

            br.AssertInt16(16);
            br.AssertByte(0);
            byte UnkA2 = br.ReadByte(); //some count
            ;

        }

        protected override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
