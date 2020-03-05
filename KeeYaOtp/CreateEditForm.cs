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

using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;
using KeeYaOtp.Core;
using System;
using System.Windows.Forms;

namespace KeeYaOtp
{
    public partial class CreateEditForm : Form
    {
        private readonly PwEntry _entry;
        private readonly IPluginHost _host;

        public CreateEditForm(PwEntry entry, IPluginHost host)
        {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
            _host = host ?? throw new ArgumentNullException(nameof(host));

            InitializeComponent();

            this.Icon = host.MainWindow.Icon;

            if (entry.Strings.Exists(OtpDataUtils.Key))
            {
                var data = entry.Strings.Get(OtpDataUtils.Key).ReadString();
                if (OtpDataUtils.TryGetOtpParts(data, out var secretString, out var pinString))
                {
                    textPin.Text = pinString;
                    textSecret.Text = secretString;
                }
            }

            textPin.MaxLength = Pin.MaxLength;
            textPin.KeyPress += TextPin_KeyPress;

            textSecret.CharacterCasing = CharacterCasing.Upper;
            textSecret.TextChanged += TextSecret_TextChanged;
            textSecret.KeyPress += TextSecret_KeyPress;

            this.AutoValidate = AutoValidate.EnableAllowFocusChange;
            //textPin.Validating += textPin_Validating;
            //textSecret.Validating += textSecret_Validating;

            //
            textPin.Validated += TextPin_Validated;
            textSecret.Validated += TextSecret_Validated;
            //
        }


        private void TextPin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void TextSecret_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) && !Secret.IsValidChar(char.ToUpper(e.KeyChar))) e.Handled = true;
        }

        private void TextSecret_TextChanged(object sender, EventArgs e)
        {
            if (textSecret.Modified)
            {
                var correctText = Secret.RemoveInvalidChars(textSecret.Text);
                if (correctText != textSecret.Text)
                    textSecret.Text = correctText;
                textSecret.Select(correctText.Length, 0);
            }
        }


        private bool ValidateTextPin()
        {
            if (Secret.Verify(textSecret.Text, out var validPinLength))
            {
                if (!Pin.Verify(textPin.Text, out var inputPinLength) || validPinLength != inputPinLength)
                {
                    errorProviderPin.SetError(textPin, string.Format(Properties.Strings.CreateEditForm_InvalidPinForSecret, validPinLength));
                    return false;
                }
            }
            else
            {
                if (!Pin.Verify(textPin.Text, out var inputPinLength) || inputPinLength < Pin.MinLength || inputPinLength > Pin.MaxLength)
                {
                    errorProviderPin.SetError(textPin, string.Format(Properties.Strings.CreateEditForm_InvalidPinCommon, Pin.MinLength, Pin.MaxLength));
                    return false;
                }
            }

            errorProviderPin.SetError(textPin, string.Empty);
            return true;
        }

        //
        private void TextPin_Validated(object sender, EventArgs e)
        {
            ValidateTextPin();
        }
        //

        //private void textPin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = !ValidateTextPin();
        //}


        private bool ValidateTextSecret()
        {
            if (!Secret.Verify(textSecret.Text, out var _))
            {
                errorProviderSecret.SetError(textSecret, Properties.Strings.CreateEditForm_InvalidSecretKey);
                return false;
            }
            else
            {
                errorProviderSecret.SetError(textSecret, string.Empty);
                return true;
            }
        }

        //
        private void TextSecret_Validated(object sender, EventArgs e)
        {
            ValidateTextSecret();
            ValidateTextPin();
        }
        //

        //private void textSecret_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = !ValidateTextSecret();
        //    ValidateTextPin();
        //}


        private void CreateEditForm_Shown(object sender, EventArgs e)
        {
            this.ValidateChildren();
        }

        private void CreateEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK) return;

            if (OtpDataUtils.TryCreateOtpData(textSecret.Text, textPin.Text, out var dataString))
            {
                if (!_entry.Strings.Exists(OtpDataUtils.Key) || _entry.Strings.Get(OtpDataUtils.Key).ReadString() != dataString)
                {
                    _entry.Strings.Set(OtpDataUtils.Key, new ProtectedString(true, dataString));
                    _entry.Touch(true);
                    _host.MainWindow.UpdateUI(false, null, true, _host.Database.RootGroup, true, null, true);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
