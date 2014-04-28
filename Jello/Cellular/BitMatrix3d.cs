#define SANITYCHECKS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Diagnostics.Contracts;

namespace Jello.Cellular
{
    /// <summary>
    /// A 3 dimensional bit matrix.
    /// </summary>
    class BitMatrix3d : IEnumerable
    {
        private int[] _data;

        public int SizeZ { get; private set; }
        public int SizeY { get; private set; }
        public int SizeX { get; private set; }

        public int FlatSize { get; private set; }

        public BitMatrix3d(int sizeZ, int sizeY, int sizeX)
        {
            SizeZ = sizeZ;
            SizeY = sizeY;
            SizeX = sizeX;
            FlatSize = SizeZ * SizeY * SizeX;

            _data = new int[sizeZ * sizeY * sizeX];
        }

        public bool this[int flatIndex]
        {
            get
            {
                int intLocation = flatIndex / 32;
                int bitOffset = flatIndex % 32;
                return (_data[intLocation] & (1 << bitOffset)) != 0;
            }
            set
            {
                int intLocation = flatIndex / 32;
                int bitOffset = flatIndex % 32;

                if (value)
                    _data[intLocation] |= (1 << bitOffset);
                else
                    _data[intLocation] &= (~(1 << bitOffset));
            }
        }

        public bool this[int zIndex, int yIndex, int xIndex]
        {
            get
            {
                return Get(zIndex, yIndex, xIndex);
            }

            set
            {
                Set(zIndex, yIndex, xIndex, value);
            }
        }

        public bool Get(int zIndex, int yIndex, int xIndex)
        {
#if SANITYCHECKS
            if (zIndex < 0 || zIndex >= SizeZ)
                throw new ArgumentOutOfRangeException("zIndex");

            if (yIndex < 0 || yIndex >= SizeY)
                throw new ArgumentOutOfRangeException("yIndex");

            if (xIndex < 0 || xIndex >= SizeX)
                throw new ArgumentOutOfRangeException("xIndex");
#endif

            int flatIndex = (zIndex * SizeY * SizeX) + (yIndex * SizeX) + xIndex;
            return this[flatIndex];
        }

        public void Set(int zIndex, int yIndex, int xIndex, bool value)
        {
#if SANITYCHECKS
            if (zIndex < 0 || zIndex >= SizeZ)
                throw new ArgumentOutOfRangeException("zIndex");

            if (yIndex < 0 || yIndex >= SizeY)
                throw new ArgumentOutOfRangeException("yIndex");

            if (xIndex < 0 || xIndex >= SizeX)
                throw new ArgumentOutOfRangeException("xIndex");
#endif

            int flatIndex = (zIndex * SizeY * SizeX) + (yIndex * SizeX) + xIndex;
            this[flatIndex] = value;
        }

        public IEnumerator GetEnumerator()
        {
            return new BitMatrix3dEnumerator(this);
        }

        private class BitMatrix3dEnumerator : IEnumerator<bool>
        {
            private BitMatrix3d _bitMatrix;
            private int _flatIndex;
            private bool _currentElement;

            internal BitMatrix3dEnumerator(BitMatrix3d bitMatrix)
            {
                _bitMatrix = bitMatrix;
                _flatIndex = -1;
            }

            public bool Current
            {
                get
                {
                    if (_flatIndex == -1)
                        throw new InvalidOperationException();
                    if (_flatIndex > _bitMatrix.FlatSize)
                        throw new InvalidOperationException();
                    return _currentElement;
                }
            }

            public void Dispose() { }

            object IEnumerator.Current { get { return Current; }  }

            public bool MoveNext()
            {
                if (_flatIndex < _bitMatrix.FlatSize - 1)
                {
                    _flatIndex++;
                    _currentElement = _bitMatrix[_flatIndex];
                    return true;
                }
                else
                {
                    _flatIndex = _bitMatrix.FlatSize;
                    return false;
                }
            }

            public void Reset()
            {
                _flatIndex = -1;
            }
        }

        public BitMatrix3d Clone()
        {
            var clone = new BitMatrix3d(SizeZ, SizeY, SizeX);
            _data.CopyTo(clone._data, 0);
            return clone;
        }
    }
}