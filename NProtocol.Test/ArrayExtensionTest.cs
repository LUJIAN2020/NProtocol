using NProtocol.Extensions;

namespace NProtocol.Test
{
    [TestClass]
    public class ArrayExtensionTest
    {
        [TestMethod]

        public void ArrayToHexString_Test()
        {
            byte[] buffer = [0x23, 0xAB];

            string expected = "23 AB";
            string actual = buffer.ToHexString(" ");
            Assert.AreEqual(actual, expected);

            expected = "23AB";
            actual = buffer.ToHexString("");
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Combine_Test()
        {
            byte[] buffer1 = [0x23, 0xAB];
            byte[] buffer2 = [0x34, 0xFB];

            byte[] expected = [0x23, 0xAB, 0x34, 0xFB];
            var actual = buffer1.Combine(buffer2);
            CollectionAssert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Combine1_Test()
        {
            byte[] buffer1 = [0x23, 0xAB];
            byte[] buffer2 = [0x34, 0xFB];
            byte[] buffer3 = [0x54, 0xF3];
            byte[] buffer4 = [0x56, 0x13];

            byte[] expected = [.. buffer1, .. buffer2, .. buffer3, .. buffer4];
            var actual = buffer1.Combine(buffer2, buffer3, buffer4);
            CollectionAssert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Slice_Test()
        {
            byte[] buffer = [1, 2, 3, 4, 5, 6, 7, 8];
            byte[] expected = [1, 2, 3, 4];
            var actual = buffer.Slice(0, 4);
            CollectionAssert.AreEqual(actual, expected);

            var actual1 = buffer.Slice(1, 0);
            CollectionAssert.AreEqual(actual1, Array.Empty<byte>());

            //开始索引必须>0
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                buffer.Slice(-1, 4);
            });

            //长度必须大于0
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                buffer.Slice(1, -1);
            });

            //开始索引不能大于数组长度
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                buffer.Slice(10, 1);
            });

            //长度不能大于数组长度
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                buffer.Slice(1, 9);
            });
        }

        [TestMethod]
        public void Slice1_Test()
        {
            byte[] buffer = [1, 2, 3, 4, 5, 6, 7, 8];
            byte[] expected = [4, 5, 6, 7, 8];
            var actual = buffer.Slice(3);
            CollectionAssert.AreEqual(actual, expected);

            //开始索引必须>0
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                buffer.Slice(-1);
            });

            //开始索引不能大于数组长度
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                buffer.Slice(10);
            });
        }

        [TestMethod]
        public void Flatten_Test()
        {
            var array = new object[] { 1, "A", new DateTime(2025, 10, 01, 10, 10, 10), TimeSpan.FromSeconds(1), new byte[] { 1, 2, 3, 4 } };
            var actual = array.Flatten();

            Assert.AreEqual(1, actual[0]);
            Assert.AreEqual("A", actual[1]);
            Assert.AreEqual(new DateTime(2025, 10, 01, 10, 10, 10), actual[2]);
            Assert.AreEqual(TimeSpan.FromSeconds(1), actual[3]);
            Assert.AreEqual((byte)1, actual[4]);
            Assert.AreEqual((byte)2, actual[5]);
            Assert.AreEqual((byte)3, actual[6]);
            Assert.AreEqual((byte)4, actual[7]);
        }

        [TestMethod]
        public void ToFlattenString_Test()
        {
            var array = new object[] { 1, 2, 3, "A", new DateTime(2025, 10, 01, 10, 10, 10), TimeSpan.FromSeconds(1), new byte[] { 1, 2, 3, 4 } };
            var actual = array.ToFlattenString();
            string expected = "[ 1, 2, 3, 'A', '2025/10/1 10:10:10', '00:00:01', 1, 2, 3, 4 ]";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ChunkBy_Test()
        {
            var array = new int[] { 1, 2, 4, 5, 6, 9, 10, 20 };
            var actual = array.ChunkBy(3);
            var expected = new List<int[]>()
            {
                new int[]{ 1,2,4},
                new int[]{ 5,6,9},
                new int[]{ 10,20},
            };
            CollectionAssert.AreEqual(expected, actual.ToList());
        }
    }
}
