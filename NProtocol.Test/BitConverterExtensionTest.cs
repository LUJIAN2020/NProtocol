using System.Collections;
using NProtocol.Extensions;

namespace NProtocol.Test
{
    [TestClass]
    public class BitConverterExtensionTest
    {
        [TestMethod]
        public void Byte_ToBooleans_Test()
        {
            byte b = 0xDD;

            bool[] expected = new bool[8];
            var bits = new BitArray(new byte[] { b });
            bits.CopyTo(expected, 0);

            bool[] actual = b.ToBooleans();

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));
        }

        [TestMethod]
        public void Bytes_ToBooleans_Test()
        {
            byte[] bytes = [0xDD, 0xDF, 0xAB];

            var expected = new bool[8 * bytes.Length];
            var bits1 = new BitArray(bytes);
            bits1.CopyTo(expected, 0);

            bool[] actual = bytes.ToBooleans();

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));
        }

        [TestMethod]
        public void Short_ToBooleans_Test()
        {
            short value = 0x6A_1F;
            bool[] expected = new bool[16];
            bool[] actual = new bool[16];

            //小端序
            byte[] bytes1 = [0X1F, 0x6A];
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(expected, 0);

            actual = value.ToBooleans(true);

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F];
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(expected, 0);

            actual = value.ToBooleans(false);

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));
        }

        [TestMethod]
        public void UShort_ToBooleans_Test()
        {
            ushort value = 0x6A_1F;
            bool[] expected = new bool[16];

            //小端序
            byte[] bytes1 = [0X1F, 0x6A];
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(expected, 0);

            bool[] actual = value.ToBooleans(true);

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F];
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(expected, 0);

            actual = value.ToBooleans(false);

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));
        }

        [TestMethod]
        public void Int_ToBooleans_Test()
        {
            int value = 0x6A_1F_CC_DF;
            bool[] expected = new bool[32];
            bool[] actual = [];

            //小端序
            byte[] bytes1 = [0xDF, 0xCC, 0X1F, 0x6A];
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(expected, 0);

            actual = value.ToBooleans(true);

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0xCC, 0xDF];
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(expected, 0);

            actual = value.ToBooleans(false);

            Assert.IsTrue(Enumerable.SequenceEqual(actual, expected));
        }

        [TestMethod]
        public void UInt_ToBooleans_Test()
        {
            uint value = 0x6A_1F_CC_DF;
            bool[] bits = new bool[32];

            //小端序
            byte[] bytes1 = [0xDF, 0xCC, 0X1F, 0x6A];
            bool[] result1 = value.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0xCC, 0xDF];
            bool[] result2 = value.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void Long_ToBooleans_Test()
        {
            long value = 0x6A_1F_CC_DF_CD_DF_AD_D1;
            bool[] bits = new bool[64];

            //小端序
            byte[] bytes1 = [0xD1, 0XAD, 0XDF, 0XCD, 0xDF, 0xCC, 0X1F, 0x6A];
            bool[] result1 = value.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0xCC, 0xDF, 0XCD, 0XDF, 0XAD, 0XD1];
            bool[] result2 = value.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void ULong_ToBooleans_Test()
        {
            ulong value = 0x6A_1F_CC_DF_CD_DF_AD_D1;
            bool[] bits = new bool[64];

            //小端序
            byte[] bytes1 = [0xD1, 0XAD, 0XDF, 0XCD, 0xDF, 0xCC, 0X1F, 0x6A];
            bool[] result1 = value.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0xCC, 0xDF, 0XCD, 0XDF, 0XAD, 0XD1];
            bool[] result2 = value.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void Shorts_ToBooleans_Test()
        {
            short[] values = [0x6A_1F, 0x7B_2C];
            bool[] bits = new bool[32];

            //小端序
            byte[] bytes1 = [0X1F, 0x6A, 0x2C, 0x7B];
            bool[] result1 = values.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0x7B, 0x2C];
            bool[] result2 = values.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void UShorts_ToBooleans_Test()
        {
            ushort[] values = [0x6A_1F, 0x7B_2C];
            bool[] bits = new bool[32];

            //小端序
            byte[] bytes1 = [0X1F, 0x6A, 0x2C, 0x7B];
            bool[] result1 = values.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0x7B, 0x2C];
            bool[] result2 = values.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void Ints_ToBooleans_Test()
        {
            int[] values = [0x6A_1F_2B_B2, 0x7B_2C_6A_A6];
            bool[] bits = new bool[32 * 2];

            //小端序
            byte[] bytes1 = [0xB2, 0x2B, 0X1F, 0x6A, 0xA6, 0x6A, 0x2C, 0x7B];
            bool[] result1 = values.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2B, 0xB2, 0x7B, 0x2C, 0x6A, 0xA6];
            bool[] result2 = values.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void UInts_ToBooleans_Test()
        {
            uint[] values = [0x6A_1F_2B_B2, 0x7B_2C_6A_A6];
            bool[] bits = new bool[32 * 2];

            //小端序
            byte[] bytes1 = [0xB2, 0x2B, 0X1F, 0x6A, 0xA6, 0x6A, 0x2C, 0x7B];
            bool[] result1 = values.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2B, 0xB2, 0x7B, 0x2C, 0x6A, 0xA6];
            bool[] result2 = values.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void Longs_ToBooleans_Test()
        {
            long[] values = [0x6A_1F_2B_B2_3C_C3_DF_3D, 0x7B_2C_6A_A6_1F_2B_B2_3C];
            bool[] bits = new bool[64 * 2];

            //小端序
            byte[] bytes1 = [0x3D, 0XDF, 0XC3, 0X3C, 0xB2, 0x2B, 0X1F, 0x6A, 0X3C, 0XB2, 0X2B, 0X1F, 0xA6, 0x6A, 0x2C, 0x7B,];
            bool[] result1 = values.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2B, 0xB2, 0x3C, 0xC3, 0xDF, 0x3D, 0x7B, 0x2C, 0x6A, 0xA6, 0x1F, 0x2B, 0xB2, 0x3C];
            bool[] result2 = values.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void ULongs_ToBooleans_Test()
        {
            ulong[] values = [0x6A_1F_2B_B2_3C_C3_DF_3D, 0x7B_2C_6A_A6_1F_2B_B2_3C];
            bool[] bits = new bool[64 * 2];

            //小端序
            byte[] bytes1 = [0x3D, 0XDF, 0XC3, 0X3C, 0xB2, 0x2B, 0X1F, 0x6A, 0X3C, 0XB2, 0X2B, 0X1F, 0xA6, 0x6A, 0x2C, 0x7B,];
            bool[] result1 = values.ToBooleans(true);
            var bits1 = new BitArray(bytes1);
            bits1.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bits));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2B, 0xB2, 0x3C, 0xC3, 0xDF, 0x3D, 0x7B, 0x2C, 0x6A, 0xA6, 0x1F, 0x2B, 0xB2, 0x3C];
            bool[] result2 = values.ToBooleans(false);
            var bits2 = new BitArray(bytes2);
            bits2.CopyTo(bits, 0);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bits));
        }

        [TestMethod]
        public void Booleans_ToByte_Test()
        {
            //位长度够8位
            byte real1 = 0x2B;
            bool[] bits1 = [true, true, false, true, false, true, false, false];
            byte b1 = bits1.ToByte();
            Assert.AreEqual(b1, real1);

            //位长度不够8位
            byte real2 = 0x0D;
            bool[] bits2 = [true, false, true, true];
            byte b2 = bits2.ToByte();
            Assert.AreEqual(b2, real2);

            //长度超范围报错
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                bool[] bits2 = [true, true, false, true, false, true, false, false, true, true];
                byte b2 = bits2.ToByte();
            });
        }

        [TestMethod]
        public void Booleans_ToBytes_Test()
        {
            //长度够8位
            byte[] real1 = [0x2B, 0x33];
            bool[] bits1 = [
                true,
                true,
                false,
                true,
                false,
                true,
                false,
                false,

                true,
                true,
                false,
                false,
                true,
                true,
                false,
                false
            ];
            byte[] bytes1 = bits1.ToBytes();
            Assert.IsTrue(Enumerable.SequenceEqual(bytes1, real1));

            //长度不够8位的数据
            byte[] real2 = [0x0D];
            bool[] bits2 = [true, false, true, true];
            byte[] bytes2 = bits2.ToBytes();
            Assert.IsTrue(Enumerable.SequenceEqual(bytes2, real2));
        }

        [TestMethod]
        public void Short_ToBytes_Test()
        {
            short value = 0x6A_1F;

            //小端序
            byte[] bytes1 = [0X1F, 0x6A];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void UShort_ToBytes_Test()
        {
            ushort value = 0x6A_1F;

            //小端序
            byte[] bytes1 = [0X1F, 0x6A];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Int_ToBytes_Test()
        {
            int value = 0x6A_1F_AF_BF;

            //小端序
            byte[] bytes1 = [0XBF, 0xAF, 0x1F, 0x6A];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0xAF, 0xBF];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void UInt_ToBytes_Test()
        {
            uint value = 0x6A_1F_AF_BF;

            //小端序
            byte[] bytes1 = [0XBF, 0xAF, 0x1F, 0x6A];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0xAF, 0xBF];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Long_ToBytes_Test()
        {
            long value = 0x6A_1F_AF_BF_CF_BC_F1_A4;

            //小端序
            byte[] bytes1 = [0xA4, 0xF1, 0xBC, 0xCF, 0xBF, 0xAF, 0x1F, 0x6A];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0xAF, 0xBF, 0xCF, 0xBC, 0xF1, 0xA4];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void ULong_ToBytes_Test()
        {
            ulong value = 0x6A_1F_AF_BF_CF_BC_F1_A4;

            //小端序
            byte[] bytes1 = [0xA4, 0xF1, 0xBC, 0xCF, 0xBF, 0xAF, 0x1F, 0x6A];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0xAF, 0xBF, 0xCF, 0xBC, 0xF1, 0xA4];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Float_ToBytes_Test()
        {
            float value = 3.14159f;

            //小端序
            byte[] bytes1 = [0xD0, 0x0F, 0x49, 0x40];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x40, 0x49, 0x0F, 0xD0];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Double_ToBytes_Test()
        {
            double value = 3.14159d;
            var buffer = BitConverter.GetBytes(value);

            //小端序
            byte[] bytes1 = [0x6E, 0x86, 0x1B, 0xF0, 0xF9, 0x21, 0x09, 0x40];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x40, 0x09, 0x21, 0xf9, 0xF0, 0x1B, 0x86, 0x6E];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Shorts_ToBytes_Test()
        {
            short[] value = [0x6A_1F, 0x6B_2F];

            //小端序
            byte[] bytes1 = [0X1F, 0x6A, 0x2F, 0x6B];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0x6B, 0x2F];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void UShorts_ToBytes_Test()
        {
            ushort[] value = [0x6A_1F, 0x6B_2F];

            //小端序
            byte[] bytes1 = [0X1F, 0x6A, 0x2F, 0x6B];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0X1F, 0x6B, 0x2F];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Ints_ToBytes_Test()
        {
            int[] value = [0x6A_1F_2F_3F, 0x6B_2F_1A_2A];

            //小端序
            byte[] bytes1 = [0X3F, 0X2F, 0X1F, 0x6A, 0x2A, 0X1A, 0x2F, 0x6B];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2F, 0x3F, 0x6B, 0x2F, 0x1A, 0x2A];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void UInts_ToBytes_Test()
        {
            uint[] value = [0x6A_1F_2F_3F, 0x6B_2F_1A_2A];

            //小端序
            byte[] bytes1 = [0X3F, 0X2F, 0X1F, 0x6A, 0x2A, 0X1A, 0x2F, 0x6B];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2F, 0x3F, 0x6B, 0x2F, 0x1A, 0x2A];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Longs_ToBytes_Test()
        {
            long[] value = [0x6A_1F_2F_3F_6B_2F_1A_2A, 0x6B_2F_1A_2A_6A_1F_2F_3F];

            //小端序
            byte[] bytes1 = [0x2A, 0X1A, 0x2F, 0x6B, 0X3F, 0X2F, 0X1F, 0x6A, 0X3F, 0X2F, 0X1F, 0x6A, 0x2A, 0X1A, 0x2F, 0x6B];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2F, 0x3F, 0x6B, 0x2F, 0x1A, 0x2A, 0x6B, 0x2F, 0x1A, 0x2A, 0x6A, 0x1F, 0x2F, 0x3F];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void ULongs_ToBytes_Test()
        {
            ulong[] value = [0x6A_1F_2F_3F_6B_2F_1A_2A, 0x6B_2F_1A_2A_6A_1F_2F_3F];

            //小端序
            byte[] bytes1 = [0x2A, 0X1A, 0x2F, 0x6B, 0X3F, 0X2F, 0X1F, 0x6A, 0X3F, 0X2F, 0X1F, 0x6A, 0x2A, 0X1A, 0x2F, 0x6B];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x6A, 0x1F, 0x2F, 0x3F, 0x6B, 0x2F, 0x1A, 0x2A, 0x6B, 0x2F, 0x1A, 0x2A, 0x6A, 0x1F, 0x2F, 0x3F];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Floats_ToBytes_Test()
        {
            float[] value = [3.14f, 374558.78f];

            var buffer1 = BitConverter.GetBytes(3.14f);
            var buffer2 = BitConverter.GetBytes(374558.78f);

            //小端序
            byte[] bytes1 = [0xC3, 0xF5, 0x48, 0x40, 0xD9, 0xE3, 0xB6, 0x48];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [0x40, 0x48, 0xF5, 0xC3, 0x48, 0xB6, 0xE3, 0xD9];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Doubles_ToBytes_Test()
        {
            double[] value = [3.14d, 374558.78d];

            var buffer1 = BitConverter.GetBytes(3.14d);
            var buffer2 = BitConverter.GetBytes(374558.78d);

            //小端序
            byte[] bytes1 = [
                0x1f,
                0x85,
                0xeb,
                0x51,
                0xb8,
                0x1e,
                0x09,
                0x40,

                0xec,
                0x51,
                0xb8,
                0x1e,
                0x7b,
                0xdc,
                0x16,
                0x41];
            var result1 = value.ToBytes(true);
            Assert.IsTrue(Enumerable.SequenceEqual(result1, bytes1));

            //大端序
            byte[] bytes2 = [
                0x40,
                0x09,
                0x1e,
                0xb8,
                0x51,
                0xeb,
                0x85,
                0x1f,

                0x41,
                0x16,
                0xdc,
                0x7b,
                0x1e,
                0xb8,
                0x51,
                0xec];
            var result2 = value.ToBytes(false);
            Assert.IsTrue(Enumerable.SequenceEqual(result2, bytes2));
        }

        [TestMethod]
        public void Bytes_ToInt16_Test()
        {
            byte[] value = [0xF4, 0XFA];

            //小端序
            var result1 = value.ToInt16(true);
            unchecked
            {
                Assert.AreEqual(result1, (short)0xF4FA);
            }

            //大端序
            var result2 = value.ToInt16(false);
            unchecked
            {
                Assert.AreEqual(result2, (short)0xF4FA);
            }

            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x1F, 0X3A, 0xDD];
                value.ToInt16();
            });
        }

        [TestMethod]
        public void Bytes_ToUInt16_Test()
        {
            byte[] value = [0xFE, 0XEF];

            //小端序
            var result1 = value.ToUInt16(true);
            Assert.AreEqual(result1, (ushort)0xFEEF);

            //大端序
            var result2 = value.ToUInt16(false);
            Assert.AreEqual(result2, (ushort)0xFEEF);

            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x1F, 0X3A, 0xDD];
                value.ToUInt16();
            });
        }

        [TestMethod]
        public void Bytes_ToInt32_Test()
        {
            byte[] value = [0xFE, 0XEF, 0xFF, 0x10];

            //小端序
            var result1 = value.ToInt32(true);
            unchecked
            {
                Assert.AreEqual(result1, (int)0xFEEFFF10);
            }

            //大端序
            var result2 = value.ToInt32(false);
            unchecked
            {
                Assert.AreEqual(result2, (int)0xFEEFFF10);
            }

            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0xFE, 0XEF, 0xFF, 0x10, 0x11];
                value.ToInt32(true);
                value.ToInt32(false);
            });
        }

        [TestMethod]
        public void Bytes_ToUInt32_Test()
        {
            byte[] value = [0xFE, 0XEF, 0xFF, 0x10];

            //小端序
            var result1 = value.ToUInt32(true);
            Assert.AreEqual<uint>(result1, 0xFEEFFF10);

            //大端序
            var result2 = value.ToUInt32(false);
            Assert.AreEqual<uint>(result2, 0xFEEFFF10);

            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0xFE, 0XEF, 0xFF, 0x10, 0x11];
                value.ToUInt32(true);
                value.ToUInt32(false);
            });
        }

        [TestMethod]
        public void Bytes_ToInt64_Test()
        {
            byte[] value = [0xFE, 0XEF, 0xFF, 0x10, 0xEE, 0x2F, 0x11, 0x33];

            //小端序
            var result1 = value.ToInt64(true);
            unchecked
            {
                Assert.AreEqual(result1, (long)0xFEEFFF10_EE2F1133);
            }

            //大端序
            var result2 = value.ToInt64(false);
            unchecked
            {
                Assert.AreEqual(result2, (long)0xFEEFFF10_EE2F1133);
            }


            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0xFE, 0XEF, 0xFF, 0x10, 0x11];
                value.ToUInt32(true);
                value.ToUInt32(false);
            });
        }

        [TestMethod]
        public void Bytes_ToFloat_Test()
        {
            //小端序
            byte[] value = [0x40, 0x49, 0x0F, 0xD0];

            //小端序
            var result1 = value.ToFloat(true);
            Assert.AreEqual(result1, 3.14159f);

            //大端序
            var result2 = value.ToFloat(false);
            Assert.AreEqual(result2, 3.14159f);

            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0xFE, 0XEF, 0xFF, 0x10, 0x11];
                value.ToFloat(true);
                value.ToFloat(false);
            });
        }

        [TestMethod]
        public void Bytes_ToDouble_Test()
        {
            //小端序
            byte[] value = [0x40, 0x09, 0x1e, 0xb8, 0x51, 0xeb, 0x85, 0x1f];

            //小端序
            var result1 = value.ToDouble(true);
            Assert.AreEqual(result1, 3.14d);

            //大端序
            var result2 = value.ToDouble(false);
            Assert.AreEqual(result2, 3.14d);

            //数组长度超限异常
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x09, 0x1e, 0xb8, 0x51, 0xeb, 0x85, 0x1f, 0x11];
                value.ToDouble(true);
                value.ToDouble(false);
            });
        }

        [TestMethod]
        public void Bytes_ToInt16Array_Test()
        {
            byte[] value = [0x1f, 0x14, 0x1b, 0x51, 0x18, 0x1e, 0x09, 0x40];

            //小端序
            short[] real = [0x1f14, 0x1b51, 0x181e, 0x0940];
            var result1 = value.ToInt16Array(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            real = [0x141f, 0x511b, 0x1e18, 0x4009];
            var result2 = value.ToInt16Array(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度>2
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToInt16Array(true);
                value.ToInt16Array(false);
            });

            //数组长度为2的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x33, 0x34];
                value.ToInt16Array(true);
                value.ToInt16Array(false);
            });
        }

        [TestMethod]
        public void Bytes_ToUInt16Array_Test()
        {
            byte[] value = [0x1f, 0x14, 0x1b, 0x51, 0x18, 0x1e, 0x09, 0x40];

            //小端序
            ushort[] real = [0x1f14, 0x1b51, 0x181e, 0x0940];
            var result1 = value.ToUInt16Array(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            real = [0x141f, 0x511b, 0x1e18, 0x4009];
            var result2 = value.ToUInt16Array(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度>2
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToUInt16Array(true);
                value.ToUInt16Array(false);
            });

            //数组长度为2的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x33, 0x34];
                value.ToUInt16Array(true);
                value.ToUInt16Array(false);
            });
        }

        [TestMethod]
        public void Bytes_ToInt32Array_Test()
        {
            byte[] value = [0x1f, 0x14, 0x1b, 0x51, 0x18, 0x1e, 0x09, 0x40];

            //小端序
            int[] real = [0x1f141b51, 0x181e0940];
            var result1 = value.ToInt32Array(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            real = [0x511B141F, 0x40091e18];
            var result2 = value.ToInt32Array(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度>4
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToInt32Array(true);
                value.ToInt32Array(false);
            });

            //数组长度为4的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x33, 0x34, 0X33, 0X23];
                value.ToInt32Array(true);
                value.ToInt32Array(false);
            });
        }

        [TestMethod]
        public void Bytes_ToUInt32Array_Test()
        {
            byte[] value = [0x1f, 0x14, 0x1b, 0x51, 0x18, 0x1e, 0x09, 0x40];

            //小端序
            uint[] real = [0x1f141b51, 0x181e0940];
            var result1 = value.ToUInt32Array(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            real = [0x511b141f, 0x40091e18];
            var result2 = value.ToUInt32Array(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度>4
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToUInt32Array(true);
                value.ToUInt32Array(false);
            });

            //数组长度为4的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x33, 0x34, 0X33, 0X23];
                value.ToUInt32Array(true);
                value.ToUInt32Array(false);
            });
        }

        [TestMethod]
        public void Bytes_ToFloatArray_Test()
        {
            byte[] value = [0x1f, 0x14, 0x1b, 0x51, 0x18, 0x1e, 0x09, 0x40];

            //小端序
            float f1 = BitConverter.ToSingle([0x51, 0x1b, 0x14, 0x1f], 0);
            float f2 = BitConverter.ToSingle([0x40, 0x09, 0x1e, 0x18], 0);

            float[] real = [f1, f2];
            var result1 = value.ToFloatArray(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            f1 = BitConverter.ToSingle([0x1f, 0x14, 0x1b, 0x51], 0);
            f2 = BitConverter.ToSingle([0x18, 0x1e, 0x09, 0x40], 0);
            real = [f1, f2];
            var result2 = value.ToFloatArray(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度>4
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToFloatArray(true);
                value.ToFloatArray(false);
            });

            //数组长度为4的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x33, 0x34, 0X33, 0X23];
                value.ToFloatArray(true);
                value.ToFloatArray(false);
            });
        }

        [TestMethod]
        public void Bytes_ToDoubleArray_Test()
        {
            byte[] value = [
                0x1f,
                0x14,
                0x1b,
                0x51,
                0x18,
                0x1e,
                0x09,
                0x40,

                0x2f,
                0x34,
                0x4b,
                0x51,
                0x68,
                0x7e,
                0x89,
                0x40
            ];

            //小端序
            double f1 = BitConverter.ToDouble([0x40, 0x09, 0x1e, 0x18, 0x51, 0x1b, 0x14, 0x1f], 0);
            double f2 = BitConverter.ToDouble([0x40, 0x89, 0x7e, 0x68, 0x51, 0x4b, 0x34, 0x2f], 0);

            double[] real = [f1, f2];
            var result1 = value.ToDoubleArray(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            f1 = BitConverter.ToDouble([
                0x1f,
                0x14,
                0x1b,
                0x51,
                0x18,
                0x1e,
                0x09,
                0x40], 0);
            f2 = BitConverter.ToDouble([
                0x2f,
                0x34,
                0x4b,
                0x51,
                0x68,
                0x7e,
                0x89,
                0x40], 0);
            real = [f1, f2];
            var result2 = value.ToDoubleArray(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度要求>4
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToDoubleArray(true);
                value.ToDoubleArray(false);
            });

            //数组长度为8的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x09, 0x1e, 0x18, 0x51, 0x1b, 0x14, 0x1f, 0x44];
                value.ToDoubleArray(true);
                value.ToDoubleArray(false);
            });
        }

        [TestMethod]
        public void Bytes_ToInt64Array_Test()
        {
            byte[] value = [
                0x1f,
                0x14,
                0x1b,
                0x51,
                0x18,
                0x1e,
                0x09,
                0x40,

                0x2f,
                0x34,
                0x4b,
                0x51,
                0x68,
                0x7e,
                0x89,
                0x40
            ];

            //小端序
            double f1 = BitConverter.ToInt64([0x40, 0x09, 0x1e, 0x18, 0x51, 0x1b, 0x14, 0x1f], 0);
            double f2 = BitConverter.ToInt64([0x40, 0x89, 0x7e, 0x68, 0x51, 0x4b, 0x34, 0x2f], 0);

            double[] real = [f1, f2];
            var result1 = value.ToInt64Array(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            f1 = BitConverter.ToInt64([
                0x1f,
                0x14,
                0x1b,
                0x51,
                0x18,
                0x1e,
                0x09,
                0x40], 0);
            f2 = BitConverter.ToInt64([
                0x2f,
                0x34,
                0x4b,
                0x51,
                0x68,
                0x7e,
                0x89,
                0x40], 0);
            real = [f1, f2];
            var result2 = value.ToInt64Array(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度要求>4
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToInt64Array(true);
                value.ToInt64Array(false);
            });

            //数组长度为8的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x09, 0x1e, 0x18, 0x51, 0x1b, 0x14, 0x1f, 0x44];
                value.ToInt64Array(true);
                value.ToInt64Array(false);
            });
        }

        [TestMethod]
        public void Bytes_ToUInt64Array_Test()
        {
            byte[] value = [
                0x1f,
                0x14,
                0x1b,
                0x51,
                0x18,
                0x1e,
                0x09,
                0x40,

                0x2f,
                0x34,
                0x4b,
                0x51,
                0x68,
                0x7e,
                0x89,
                0x40
            ];

            //小端序
            double f1 = BitConverter.ToUInt64([0x40, 0x09, 0x1e, 0x18, 0x51, 0x1b, 0x14, 0x1f], 0);
            double f2 = BitConverter.ToUInt64([0x40, 0x89, 0x7e, 0x68, 0x51, 0x4b, 0x34, 0x2f], 0);

            double[] real = [f1, f2];
            var result1 = value.ToUInt64Array(true);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result1[i]);
            }

            //大端序
            f1 = BitConverter.ToUInt64([
                0x1f,
                0x14,
                0x1b,
                0x51,
                0x18,
                0x1e,
                0x09,
                0x40], 0);
            f2 = BitConverter.ToUInt64([
                0x2f,
                0x34,
                0x4b,
                0x51,
                0x68,
                0x7e,
                0x89,
                0x40], 0);
            real = [f1, f2];
            var result2 = value.ToUInt64Array(false);
            for (int i = 0; i < real.Length; i++)
            {
                Assert.AreEqual(real[i], result2[i]);
            }

            //数组长度要求>4
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40];
                value.ToUInt64Array(true);
                value.ToUInt64Array(false);
            });

            //数组长度为8的倍数
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                byte[] value = [0x40, 0x09, 0x1e, 0x18, 0x51, 0x1b, 0x14, 0x1f, 0x44];
                value.ToUInt64Array(true);
                value.ToUInt64Array(false);
            });
        }

        [TestMethod]
        public void To_Short_ArrayFromBytes_Test()
        {
            var buf = new byte[] { 0x01, 0xFF };
            short[] actual = buf.ToValueArrayFromBytes<short>(true);
            var expected = new short[] { 0x01FF };
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void To_UShort_ArrayFromBytes_Test()
        {
            var buf = new byte[] { 0x01, 0xFF };
            ushort[] actual = buf.ToValueArrayFromBytes<ushort>(true);
            var expected = new ushort[] { 0x01FF };
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ToBooleansFromWord_Test()
        {
            var buf = new ushort[] { 0x0123, 0xEF51 };
            bool[] actual = buf.ToBooleansFromWord(true);
            var expected = new bool[] {
                true, true, false, false, false, true, false, false,
                true, false, false, false, false, false, false, false,

                true, false, false, false, true, false,true, false,
                true, true, true, true, false, true, true, true,
            };
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ToBooleansFromWord2_Test()
        {
            var buf = new byte[] { 0x01, 0x23, 0xEF, 0x51 };
            bool[] actual = buf.ToBooleansFromWord(true);
            var expected = new bool[] {
                true, true, false, false, false, true, false, false,
                true, false, false, false, false, false, false, false,

                true, false, false, false, true, false,true, false,
                true, true, true, true, false, true, true, true,
            };
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
