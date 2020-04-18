﻿//
//   Copyright (c) 2011-2014 Exxeleron GmbH
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

using System;
using System.IO;
using System.Text;

namespace qSharp
{
    /// <summary>
    ///     Partial reimplementation of the BinaryReader class. Allows to switch endianess in which data is interpreted.
    /// </summary>
    internal sealed class EndianBinaryReader
    {
        /// order of uid read/write
        private static readonly int[] GuidByteOrder = {3, 2, 1, 0, 5, 4, 7, 6, 8, 9, 10, 11, 12, 13, 14, 15};

        private readonly byte[] _rawData;
        private ByteConverter _byteConverter;
        private Endianess _endianess;

        public EndianBinaryReader(byte[] rawData, Endianess endianess = Endianess.LittleEndian)
        {
            _rawData = rawData;
            Position = 0;
            Endianess = endianess;
        }

        public Endianess Endianess
        {
            get { return _endianess; }

            set
            {
                _endianess = value;
                if (value == Endianess.LittleEndian)
                    _byteConverter = FromLittleEndian;
                else
                    _byteConverter = FromBigEndian;
            }
        }

        public int Position { get; private set; }

        public bool ReadBoolean()
        {
            return _rawData[Position++] == 1;
        }

        public byte ReadByte()
        {
            return _rawData[Position++];
        }

        public sbyte ReadSByte()
        {
            return unchecked((sbyte) _rawData[Position++]);
        }

        public short ReadInt16()
        {
            var ret = unchecked((short) (_byteConverter(_rawData, Position, 2)));
            Position += 2;
            return ret;
        }

        public int ReadInt32()
        {
            var ret = unchecked((int) (_byteConverter(_rawData, Position, 4)));
            Position += 4;
            return ret;
        }

        public long ReadInt64()
        {
            var ret = unchecked((_byteConverter(_rawData, Position, 8)));
            Position += 8;
            return ret;
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32()), 0);
        }

        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(ReadInt64());
        }

        public char ReadChar()
        {
            return (char) (ReadByte() & 0xFF);
        }

        public byte[] ReadBytes(int count)
        {
            var bytes = new byte[count];
            Array.Copy(_rawData, Position, bytes, 0, count);
            Position += count;
            return bytes;
        }

        public Guid ReadGuid()
        {
            var b = new byte[16];
            for (var j = 0; j < 16; j++)
            {
                b[GuidByteOrder[j]] = ReadByte();
            }
            return new Guid(b);
        }

        public string ReadString(int count, Encoding encoding)
        {
            Position += count;
            return encoding.GetString(_rawData, Position - count, count);
        }

        public string ReadSymbol(Encoding encoding)
        {
            var i = Position;
            for (; _rawData[i] != 0; ++i)
            {
            } //empty
            var count = i - Position;

            var symbol = encoding.GetString(_rawData, Position, count);
            Position += count + 1;
            return symbol;
        }

        public static long FromLittleEndian(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long ret = 0;
            for (var i = 0; i < bytesToConvert; i++)
            {
                ret = unchecked((ret << 8) | buffer[startIndex + bytesToConvert - 1 - i]);
            }
            return ret;
        }

        public static long FromBigEndian(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long ret = 0;
            for (var i = 0; i < bytesToConvert; i++)
            {
                ret = unchecked((ret << 8) | buffer[startIndex + i]);
            }
            return ret;
        }

        internal void Seek(int position, SeekOrigin seekOrigin)
        {
            switch (seekOrigin)
            {
                case SeekOrigin.Current:
                    Position += position;
                    break;
                case SeekOrigin.Begin:
                    Position = position;
                    break;
                case SeekOrigin.End:
                    Position = _rawData.Length - position;
                    break;
            }
        }

        private delegate long ByteConverter(byte[] buffer, int startIndex, int bytesToConvert);
    }
}