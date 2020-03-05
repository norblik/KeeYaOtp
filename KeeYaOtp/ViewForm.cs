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
using KeePass.Util;
using KeePassLib;
using KeeYaOtp.Core;
using System;
using System.Windows.Forms;

namespace KeeYaOtp
{
    public partial class ViewForm : Form
    {
        public ViewForm(Yaotp yaotp, PwEntry entry, IPluginHost host)
        {
            _yaotp = yaotp ?? throw new ArgumentNullException(nameof(yaotp));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));

            InitializeComponent();

            this.Icon = host.MainWindow.Icon;
            timerUpdate.Tick += UpdateUI;
        }

        private Timer _timer = new Timer() { Interval = 250 };

        private readonly Yaotp _yaotp;
        private readonly IPluginHost _host;
        private readonly PwEntry _entry;

        private void UpdateUI(object sender, EventArgs e)
        {
            var remaining = $"00:{_yaotp.GetRemainingSeconds():D2}";
            if (labelRemaining.Text != remaining) labelRemaining.Text = remaining;
            var otp = _yaotp.ComputeOtp();
            if (labelOtp.Text != otp) labelOtp.Text = otp;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            var otp = _yaotp.ComputeOtp();
            if (ClipboardUtil.Copy(otp, false, true, _entry, _host.Database, IntPtr.Zero))
                _host.MainWindow.StartClipboardCountdown();
        }

        private void ViewForm_Shown(object sender, EventArgs e)
        {
            UpdateUI(null, null);
            timerUpdate.Enabled = true;
        }
    }
}
