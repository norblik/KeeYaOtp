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
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace KeeYaOtp.Tests
{
    public class Base32EncoderTest
    {
        public static IEnumerable<object[]> Rfc4648Data => new object[][] {

            new object[] { "","" },
            new object[] { "f", "MY======" },
            new object[] { "fo","MZXQ====" },
            new object[] { "foo","MZXW6===" },
            new object[] { "foob","MZXW6YQ=" },
            new object[] { "fooba","MZXW6YTB" },
            new object[] { "foobar", "MZXW6YTBOI======" }
        };

        [Theory]
        [PropertyData(nameof(Rfc4648Data))]
        public void encoding(string input, string expectedResult)
        {
            var arrayInput = Encoding.ASCII.GetBytes(input);

            var result = Base32Encoder.Encode(arrayInput, true);

            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [PropertyData(nameof(Rfc4648Data))]
        public void decoding(string expectedResult, string input)
        {
            var arrayExpectedResult = Encoding.ASCII.GetBytes(expectedResult);

            var result = Base32Encoder.Decode(input, true);

            Assert.Equal(result, arrayExpectedResult);
        }
    }
}
