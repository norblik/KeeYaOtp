#region copyright
// KeeYaOtp, a KeePass plugin that generate one-time passwords for Yandex 2FA
// Copyright (C) 2020 norblik
//
// This plugin is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This plugin is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this plugin. If not, see <https://www.gnu.org/licenses/>.
//
// SPDX-License-Identifier: GPL-3.0-or-later
#endregion

using System;
using System.Text;

namespace KeeYaOtp.Core
{
    sealed public class Secret
    {
        public static bool IsValidChar(char input) => Base32Encoder.Alphabet.IndexOf(input) != -1;

        public static string RemoveInvalidChars(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (var c in input)
                if (IsValidChar(c))
                    sb.Append(c);
            return sb.ToString();
        }

        static private byte NumberOfLeadingZeros(ushort value)
        {
            if (value == 0) return 16;
            byte n = 0;
            if ((value & 0xFF00) == 0) { n += 8; value <<= 8; }
            if ((value & 0xF000) == 0) { n += 4; value <<= 4; }
            if ((value & 0xC000) == 0) { n += 2; value <<= 2; }
            if ((value & 0x8000) == 0) { n++; }
            return n;
        }

        static private bool ChecksumIsValid(byte[] input)
        {
            if (input.Length < 4) throw new ArgumentException("Too short input", nameof(input));

            const ushort poly = 0b1_1000_1111_0011;// 13 bits
            ushort checksumOrig = (ushort)((input[input.Length - 2] & 0x0F) << 8 | input[input.Length - 1]);// 12 tail bits

            ushort accum = 0;
            int accumBits = 0;

            int inputTotalBitsAvailable = input.Length * 8 - 12;
            int inputIndex = 0;
            int inputBitsAvailable = 8;

            while (inputTotalBitsAvailable > 0)
            {
                int requiredBits = 13 - accumBits;
                if (inputTotalBitsAvailable < requiredBits) requiredBits = inputTotalBitsAvailable;
                while (requiredBits > 0)
                {
                    byte curInput = (byte)(input[inputIndex] & (1 << inputBitsAvailable) - 1);
                    var bitsToRead = requiredBits > inputBitsAvailable ? inputBitsAvailable : requiredBits;
                    curInput >>= inputBitsAvailable - bitsToRead;
                    accum = (ushort)((uint)accum << bitsToRead | curInput);

                    inputTotalBitsAvailable -= bitsToRead;
                    requiredBits -= bitsToRead;
                    inputBitsAvailable -= bitsToRead;
                    accumBits += bitsToRead;
                    if (inputBitsAvailable == 0)
                    {
                        inputIndex++;
                        inputBitsAvailable = 8;
                    }
                }

                if (accumBits == 13) accum ^= poly;
                accumBits = 16 - NumberOfLeadingZeros(accum);
            }

            return accum == checksumOrig;
        }

        // pinLength 4 bits + checksum 12 bits, 2 bytes [decoded.Length - 2 ... decoded.Length - 1]
        // userId, 8 bytes  [decoded.Length - 10 ... decoded.Length - 3]
        // key, 16 bytes [decoded.Length - 26 ... decoded.Length - 11]

        static private bool TryDecodeAndVerify(string secretString, out byte[] decoded)
        {
            decoded = default;
            if (string.IsNullOrEmpty(secretString)) return false;

            try
            {
                var d = Base32Encoder.Decode(secretString);
                if (d.Length >= 26 && ChecksumIsValid(d))
                {
                    decoded = d;
                    return true;
                }
            }
            catch { }

            return false;
        }

        private static int GetPinLength(byte[] decoded)
        {
            return (byte)((decoded[decoded.Length - 2] >> 4) + 1);
        }

        static public bool Verify(string secretString, out int pinLength)
        {
            if (TryDecodeAndVerify(secretString, out var decoded))
            {
                pinLength = GetPinLength(decoded);
                return true;
            }

            pinLength = default;
            return false;
        }

        static public bool TryCreate(string secretString, out Secret secret)
        {
            if (TryDecodeAndVerify(secretString, out byte[] decoded))
            {
                var data = new byte[16];
                System.Array.Copy(decoded, decoded.Length - 26, data, 0, 16);
                secret = new Secret(data, (byte)GetPinLength(decoded));
                return true;
            }

            secret = default;
            return false;
        }

        private Secret(byte[] data, byte pinLength)
        {
            PinLength = pinLength;
            Data = data;
        }

        public byte PinLength { get; }
        public byte[] Data { get; }
    }


}
