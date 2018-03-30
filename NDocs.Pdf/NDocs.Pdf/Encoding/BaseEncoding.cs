using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Encoding
{
    internal abstract class BaseEncoding : System.Text.Encoding
    {
        public override bool IsSingleByte => true;

        public override Encoder GetEncoder()
        {
            throw new NotSupportedException($"Encode operations should be performed using the {GetType().Name} class.");
        }

        public override Decoder GetDecoder()
        {
            throw new NotSupportedException($"Decode operations should be performed using the {GetType().Name} class.");
        }

        public override unsafe int GetByteCount(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            fixed (char* charRef = s)
            {
                return GetByteCountInternal(charRef, 0, s.Length);
            }
        }

        public override unsafe int GetByteCount(char[] chars)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));

            fixed (char* charRef = chars)
            {
                return GetByteCountInternal(charRef, 0, chars.Length);
            }
        }

        public override unsafe int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= chars.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (count > chars.Length - index) throw new ArgumentOutOfRangeException(nameof(count));

            fixed (char* charRef = chars)
            {
                return GetByteCountInternal(charRef, index, count);
            }
        }

        public override unsafe int GetByteCount(char* chars, int count)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return GetByteCountInternal(chars, 0, count);
        }

        protected abstract unsafe int GetByteCountInternal(char* chars, int offset, int count);

        public override int GetMaxByteCount(int charCount)
        {
            if (charCount < 0) throw new ArgumentOutOfRangeException(nameof(charCount));

            return GetMaxByteCountInternal(charCount);
        }

        protected abstract int GetMaxByteCountInternal(int charCount);

        public override unsafe byte[] GetBytes(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            fixed (char* charRef = s)
            {
                var byteCount = GetByteCountInternal(charRef, 0, s.Length);
                var bytes = new byte[byteCount];

                if (byteCount > 0)
                {
                    fixed (byte* byteRef = bytes)
                    {
                        GetBytesInternal(charRef, byteRef, s.Length, byteCount);
                    }
                }

                return bytes;
            }
        }

        //public virtual unsafe byte[] GetBytesAndFormattedString(string s)
        //{
        //    if (s == null) throw new ArgumentNullException(nameof(s));

        //    var (byteCount, formattedCharCount) = GetByteCountInternal(s);
        //    var bytes = new byte[byteCount];
        //    var formattedChars = new char[formattedCharCount];

        //    if (byteCount > 0 || formattedCharCount > 0)
        //    {
        //        fixed (char* charRef = s)
        //        {
        //            fixed (byte* byteRef = bytes)
        //            {
        //                fixed (char* formattedCharRef = formattedChars)
        //                {
        //                    GetBytesInternal(charRef, byteRef, s.Length, formattedCharRef);
        //                }
        //            }
        //        }
        //    }

        //    return (bytes, new string(formattedChars));
        //}

        public override unsafe int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (charIndex < 0) throw new ArgumentOutOfRangeException(nameof(charIndex));
            if (charIndex >= s.Length) throw new ArgumentOutOfRangeException(nameof(charIndex));
            if (charCount > s.Length - charIndex) throw new ArgumentOutOfRangeException(nameof(charCount));
            if (byteIndex < 0) throw new ArgumentOutOfRangeException(nameof(byteIndex));
            if (byteIndex >= bytes.Length) throw new ArgumentOutOfRangeException(nameof(byteIndex));

            fixed (char* charRef = s)
            {
                var byteCount = Math.Min(GetByteCountInternal(charRef, charIndex, charCount), bytes.Length - byteIndex);

                if (byteCount > 0)
                {
                    fixed (byte* byteRef = bytes)
                    {
                        GetBytesInternal(charRef + charIndex, byteRef + byteIndex, charCount, byteCount);
                    }
                }

                return byteCount;
            }
        }

        public override unsafe byte[] GetBytes(char[] chars)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));

            fixed (char* charRef = chars)
            {
                var byteCount = GetByteCountInternal(charRef, 0, chars.Length);
                var bytes = new byte[byteCount];

                if (byteCount > 0)
                {
                    fixed (byte* byteRef = bytes)
                    {
                        GetBytesInternal(charRef, byteRef, chars.Length, byteCount);
                    }
                }

                return bytes;
            }
        }

        //public virtual unsafe (byte[], char[]) GetBytesAndFormattedChars(char[] chars)
        //{
        //    if (chars == null) throw new ArgumentNullException(nameof(chars));

        //    var (byteCount, formattedCharCount) = GetByteCountInternal(chars);
        //    var bytes = new byte[byteCount];
        //    var formattedChars = new char[formattedCharCount];

        //    if (byteCount > 0 || formattedCharCount > 0)
        //    {
        //        fixed (char* charRef = chars)
        //        {
        //            fixed (byte* byteRef = bytes)
        //            {
        //                fixed (char* formattedCharRef = formattedChars)
        //                {
        //                    GetBytesInternal(charRef, byteRef, chars.Length, formattedCharRef);
        //                }
        //            }
        //        }
        //    }

        //    return (bytes, formattedChars);
        //}

        public override unsafe byte[] GetBytes(char[] chars, int index, int count)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (chars.Length < index + count) throw new ArgumentOutOfRangeException(nameof(chars));

            fixed (char* charRef = chars)
            {
                var byteCount = GetByteCountInternal(charRef, index, count);
                var bytes = new byte[byteCount];

                if (byteCount > 0)
                {
                    fixed (byte* byteRef = bytes)
                    {
                        GetBytesInternal(charRef + index, byteRef, count, byteCount);
                    }
                }

                return bytes;
            }
        }

        public override unsafe int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (charIndex < 0) throw new ArgumentOutOfRangeException(nameof(charIndex));
            if (charIndex >= chars.Length) throw new ArgumentOutOfRangeException(nameof(charIndex));
            if (byteIndex < 0) throw new ArgumentOutOfRangeException(nameof(byteIndex));
            if (byteIndex >= bytes.Length) throw new ArgumentOutOfRangeException(nameof(byteIndex));

            fixed (char* charRef = chars)
            {
                var byteCount = Math.Min(GetByteCountInternal(charRef, charIndex, charCount), bytes.Length - byteIndex);

                if (byteCount > 0)
                {
                    fixed (byte* byteRef = bytes)
                    {
                        GetBytesInternal(charRef + charIndex, byteRef + byteIndex, charCount, byteCount);
                    }
                }

                return byteCount;
            }
        }

        public override unsafe int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
        {
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (charCount < 0) throw new ArgumentOutOfRangeException(nameof(charCount));
            if (byteCount < 0) throw new ArgumentOutOfRangeException(nameof(byteCount));

            if (byteCount == 0) return 0;

            var count = Math.Min(GetByteCountInternal(chars, 0, charCount), byteCount);
            if (count == 0) return 0;

            GetBytesInternal(chars, bytes, charCount, count);
            
            return count;
        }

        protected abstract unsafe void GetBytesInternal(char* chars, byte* bytes, int charCount, int byteCount);
        
        public override unsafe int GetCharCount(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            fixed (byte* byteRef = bytes)
            {
                return GetCharCountInternal(byteRef, 0, bytes.Length);
            }
        }

        public override unsafe int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= bytes.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (count > bytes.Length - index) throw new ArgumentOutOfRangeException(nameof(count));

            fixed (byte* byteRef = bytes)
            {
                return GetCharCountInternal(byteRef, index, count);
            }
        }

        public override unsafe int GetCharCount(byte* bytes, int count)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return GetCharCountInternal(bytes, 0, count);
        }

        protected abstract unsafe int GetCharCountInternal(byte* bytes, int offset, int count);
        
        public override int GetMaxCharCount(int byteCount)
        {
            if (byteCount < 0) throw new ArgumentOutOfRangeException(nameof(byteCount));

            return GetMaxCharCountInternal(byteCount);
        }

        protected abstract int GetMaxCharCountInternal(int byteCount);

        public override unsafe char[] GetChars(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            fixed (byte* byteRef = bytes)
            {
                var charCount = GetCharCountInternal(byteRef, 0, bytes.Length);
                var chars = new char[charCount];

                if (charCount > 0)
                {
                    fixed (char* charRef = chars)
                    {
                        GetCharsInternal(byteRef, charRef, bytes.Length, charCount);
                    }
                }

                return chars;
            }
        }

        //public virtual unsafe (char[], char[]) GetCharsAndFormattedChars(byte[] bytes)
        //{
        //    if (bytes == null) throw new ArgumentNullException(nameof(bytes));

        //    var (byteCount, formattedCharCount) = GetCharCountInternal(bytes);
        //    var chars = new char[byteCount];
        //    var formattedChars = new char[formattedCharCount];

        //    if (byteCount > 0 || formattedCharCount > 0)
        //    {
        //        fixed (byte* byteRef = bytes)
        //        {
        //            fixed (char* charRef = chars)
        //            {
        //                fixed (char* formattedCharRef = formattedChars)
        //                {
        //                    GetCharsInternal(byteRef, charRef, bytes.Length, formattedCharRef);
        //                }
        //            }
        //        }
        //    }

        //    return (chars, formattedChars);
        //}

        public override unsafe char[] GetChars(byte[] bytes, int index, int count)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= bytes.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (count > bytes.Length - index) throw new ArgumentOutOfRangeException(nameof(count));

            fixed (byte* byteRef = bytes)
            {
                var charCount = GetCharCountInternal(byteRef, index, count);
                var chars = new char[charCount];

                if (charCount > 0)
                {
                    var byteCount = charCount;

                    fixed (char* charRef = chars)
                    {
                        GetCharsInternal(byteRef + index, charRef, byteCount, charCount);
                    }
                }

                return chars;
            }
        }

        public override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (byteIndex < 0) throw new ArgumentOutOfRangeException(nameof(byteIndex));
            if (byteIndex >= bytes.Length) throw new ArgumentOutOfRangeException(nameof(byteIndex));
            if (byteCount < 0) throw new ArgumentOutOfRangeException(nameof(byteCount));
            if (byteCount > bytes.Length - byteIndex) throw new ArgumentOutOfRangeException(nameof(byteCount));
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (charIndex < 0) throw new ArgumentOutOfRangeException(nameof(charIndex));
            if (charIndex >= chars.Length) throw new ArgumentOutOfRangeException(nameof(charIndex));

            fixed (byte* byteRef = bytes)
            {
                var charCount = GetCharCountInternal(byteRef, byteIndex, byteCount);
                if (charCount == 0) return 0;

                fixed (char* charRef = chars)
                {
                    GetCharsInternal(byteRef, charRef, byteCount, charCount);
                }

                return charCount;
            }
        }

        public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (byteCount < 0) throw new ArgumentOutOfRangeException(nameof(byteCount));
            if (chars == null) throw new ArgumentNullException(nameof(chars));
            if (charCount < 0) throw new ArgumentOutOfRangeException(nameof(charCount));

            var count = Math.Min(GetCharCountInternal(bytes, 0, byteCount), charCount);
            if (count == 0) return 0;

            GetCharsInternal(bytes, chars, byteCount, count);

            return count;
        }

        public override string GetString(byte[] bytes)
        {
            return new string(GetChars(bytes));
        }

        //public virtual (string, string) GetStringAndFormattedString(byte[] bytes)
        //{
        //    var (chars, formattedChars) = GetCharsAndFormattedChars(bytes);
        //    return (new string(chars), new string(formattedChars));
        //}

        public override string GetString(byte[] bytes, int index, int count)
        {
            return new string(GetChars(bytes, index, count));
        }

        protected abstract unsafe void GetCharsInternal(byte* bytes, char* chars, int byteCount, int charCount);
    }
}