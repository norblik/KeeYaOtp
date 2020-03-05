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

using System.Text;

namespace KeeYaOtp.Core
{
    sealed public class Pin
    {
        private Pin(byte[] data, byte length)
        {
            Data = data;
            Length = length;
        }

        public byte[] Data { get; }

        public byte Length { get; }

        static public bool Verify(string rawPin, out int length)
        {
            length = default;
            if (rawPin == null || rawPin.Length < MinLength || rawPin.Length > MaxLength) return false;
            foreach (var c in rawPin)
                if (!(c >= '0' && c <= '9'))
                    return false;
            length = rawPin.Length;
            return true;
        }

        static public bool TryCreate(string pinString, out Pin pin)
        {
            pin = default;
            if (!Verify(pinString, out var length)) return false;
            pin = new Pin(Encoding.UTF8.GetBytes(pinString), (byte)length);
            return true;
        }

        public const int MinLength = 4;
        public const int MaxLength = 16;
    }


}
