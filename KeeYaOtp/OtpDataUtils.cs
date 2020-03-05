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

using KeeYaOtp.Core;
using System.Collections.Generic;

namespace KeeYaOtp
{
    public static class OtpDataUtils
    {
        public const string Key = "keeyandexotp";
        private const string PinKey = "pin";
        private const string SecretKey = "secret";

        private static IDictionary<string, string> GetKeyValuePairs(string source)
        {
            var dict = new Dictionary<string, string>(2);
            var pairs = source.Split('&');
            foreach (var pair in pairs)
            {
                var keyvalue = pair.Split('=');
                dict.Add(keyvalue[0].Trim(), keyvalue[1].Trim());
            }
            return dict;
        }

        public static bool TryGetOtpParts(string data, out string secretString, out string pinString)
        {
            var pairs = GetKeyValuePairs(data);
            return pairs.TryGetValue(PinKey, out pinString) & pairs.TryGetValue(SecretKey, out secretString);
        }

        public static bool TryParseOtpData(string data, out Secret secret, out Pin pin)
        {
            pin = null;
            secret = null;

            return (TryGetOtpParts(data, out var secretString, out var pinString)
                 && Pin.TryCreate(pinString, out pin)
                 && Secret.TryCreate(secretString, out secret));
        }

        public static bool TryCreateOtpData(string secret, string pin, out string data)
        {
            if (Secret.Verify(secret, out var validPinLength) &&
                Pin.Verify(pin, out var inputPinLength) &&
                validPinLength == inputPinLength)
            {
                data = $"{PinKey}={pin}&{SecretKey}={secret}";
                return true;
            }

            data = null;
            return false;
        }
    }
}
