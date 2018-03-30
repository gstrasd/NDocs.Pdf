using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf
{
    internal static class ArrayHelper
    {
        public static unsafe int Compare(byte[] left, byte[] right)
        {
            if ((object) left == null) return (object)right == null ? 0 : -1;
            if ((object) right == null) return 1;

            var index = 0;
            fixed (byte* leftData = left)
            {
                fixed (byte* rightData = right)
                {
                    if (left.Length <= right.Length)
                    {
                        while (index < left.Length)
                        {
                            var leftByte = *(leftData + index);
                            var rightByte = *(rightData + index);
                            if (leftByte < rightByte) return -1;
                            if (leftByte > rightByte) return 1;
                            index++;
                        }

                        return left.Length == right.Length ? 0 : -1;
                    }

                    while (index < right.Length)
                    {
                        var leftByte = *(leftData + index);
                        var rightByte = *(rightData + index);
                        if (leftByte < rightByte) return -1;
                        if (leftByte > rightByte) return 1;
                        index++;
                    }

                    return 1;
                }
            }
        }

        public static unsafe bool Equals(byte[] left, byte[] right)
        {
            if ((object) left == null) return (object) right == null;
            if ((object) right == null) return false;
            if (left.Length != right.Length) return false;

            var index = 0;
            fixed (byte* leftData = left)
            {
                fixed (byte* rightData = right)
                {
                    while (index < left.Length && *(leftData + index) == *(rightData + index)) index++;
                    return index == left.Length;
                }
            }
        }

        public static unsafe byte[] Copy(byte* data, long offset, long length)
        {
            var array = new byte[length];

            fixed (byte* arrayRef = array)
            {
                var sourceRef = data + offset;
                var lastByte = data + offset + length - 1;
                var destinationRef = arrayRef;

                while (sourceRef <= lastByte) *destinationRef++ = *sourceRef++;
            }
            
            return array;
        }

        public static unsafe bool Contains(this byte[] array, byte item)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            fixed (byte* arrayRef = array)
            {
                for (var index = 0; index < array.Length; index++)
                {
                    if (*(arrayRef + index) == item) return true;
                }
            }

            return false;
        }
    }
}
