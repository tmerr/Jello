using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jello.Cellular;

namespace JelloUnitTests.Cellular
{
    [TestClass]
    public class BitMatrix3dTests
    {
        [TestMethod]
        public void CheckSizes()
        {
            var bitMatrix = new BitMatrix3d(10, 5, 3);
            Assert.AreEqual(bitMatrix.SizeZ, 10);
            Assert.AreEqual(bitMatrix.SizeY, 5);
            Assert.AreEqual(bitMatrix.SizeX, 3);
        }

        [TestMethod]
        public void EnsureInitializesToZero()
        {
            var bitMatrix = new BitMatrix3d(3, 4, 5);
            foreach (bool bit in bitMatrix)
                Assert.IsFalse(bit);
        }

        [TestMethod]
        public void SetFirstIndex()
        {
            var bitMatrix = new BitMatrix3d(3, 4, 5);
            foreach (bool bit in bitMatrix)
                Assert.IsFalse(bit);
            bitMatrix[0, 0, 0] = true;
            bool first = true;
            foreach (bool bit in bitMatrix)
            {
                if (first)
                {
                    Assert.IsTrue(bit);
                    first = false;
                }
                else
                {
                    Assert.IsFalse(bit);
                }
            }
        }

        [TestMethod]
        public void SetMiddleIndex()
        {
            var bitMatrix = new BitMatrix3d(3, 4, 5);
            foreach (bool bit in bitMatrix)
                Assert.IsFalse(bit);
            bitMatrix[1, 2, 3] = true;
            Assert.IsTrue(bitMatrix[1, 2, 3]);
        }

        [TestMethod]
        public void FlatIndexingRWStripingTest()
        {
            var bitMatrix = new BitMatrix3d(2, 3, 4);

            for (int i = 0; i < bitMatrix.FlatSize; i++)
            {
                bitMatrix[i] = (i % 2 != 0);
            }

            for (int i = 0; i < bitMatrix.FlatSize; i++)
            {
                Assert.AreEqual((i % 2 != 0), bitMatrix[i]);
            }
        }

        [TestMethod]
        public void FlatIndexingWEnumeratorRStripingTest()
        {
            var bitMatrix = new BitMatrix3d(2, 3, 4);

            for (int i = 0; i < bitMatrix.FlatSize; i++)
            {
                bitMatrix[i] = (i % 2 != 0);
            }

            int j = 0;
            foreach (bool bit in bitMatrix)
            {
                Assert.AreEqual((j % 2 != 0), bit);
                j++;
            }
        }
    }
}
