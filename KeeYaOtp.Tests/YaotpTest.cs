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
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace KeeYaOtp.Tests
{
    public class YaotpTest
    {
        public static IEnumerable<object[]> Data => new object[][] {

            new object[] { "7586", "LA2V6KMCGYMWWVEW64RNP3JA3IAAAAAAHTSG4HRZPI", new DateTime(2020, 02, 07, 08, 27, 00, DateTimeKind.Utc), "oactmacq" },
            new object[] { "7586", "LA2V6KMCGYMWWVEW64RNP3JA3IAAAAAAHTSG4HRZPI", new DateTime(2020, 02, 07, 15, 53, 30, DateTimeKind.Utc), "wemdwrix" },
            new object[] { "5210481216086702", "JBGSAU4G7IEZG6OY4UAXX62JU4AAAAAAHTSG4HXU3M", new DateTime(2020, 02, 07, 16, 04, 29, DateTimeKind.Utc), "dfrpywob" },
            new object[] { "5210481216086702", "JBGSAU4G7IEZG6OY4UAXX62JU4AAAAAAHTSG4HXU3M", new DateTime(2020, 02, 07, 16, 30, 59, DateTimeKind.Utc), "vunyprpd" }
        };

        [Theory]
        [PropertyData(nameof(Data))]
        public void generating_otp(string pinString, string secretString, DateTime genTime, string expectedResult)
        {
            Secret.TryCreate(secretString, out var secret);
            Pin.TryCreate(pinString, out var pin);
            var yaotp = new Yaotp(secret, pin, () => genTime);

            var result = yaotp.ComputeOtp();

            Assert.Equal(result, expectedResult);
        }
    }
}
