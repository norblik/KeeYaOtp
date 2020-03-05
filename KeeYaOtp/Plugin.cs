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
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Utility;
using KeeYaOtp.Core;
using System;
using System.Windows.Forms;

namespace KeeYaOtp
{
    sealed public class KeeYaOtpExt : Plugin
    {
        private const string yaotpPlaceholder = "{YAOTP}";
        private IPluginHost _host;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            _host = host;

            SprEngine.FilterCompile += SprEngine_FilterCompile;
            SprEngine.FilterPlaceholderHints.Add(yaotpPlaceholder);

            return true;
        }

        private void SprEngine_FilterCompile(object sender, SprEventArgs e)
        {
            if (((e.Context.Flags & SprCompileFlags.ExtActive) == SprCompileFlags.ExtActive) &&
                e.Text.Contains(yaotpPlaceholder) &&
                e.Context.Entry.Strings.Exists(OtpDataUtils.Key) &&
                OtpDataUtils.TryParseOtpData(e.Context.Entry.Strings.Get(OtpDataUtils.Key).ReadString(), out var secret, out var pin))
            {
                var otpString = (new Yaotp(secret, pin, () => DateTime.UtcNow)).ComputeOtp();
                e.Text = StrUtil.ReplaceCaseInsensitive(e.Text, yaotpPlaceholder, otpString);
            }
        }

        public override void Terminate()
        {
            SprEngine.FilterPlaceholderHints.Remove(yaotpPlaceholder);
        }

        private bool TryGetSingleSelectedEntry(out PwEntry entry)
        {
            var se = _host.MainWindow.GetSelectedEntries();
            if (se != null && se.Length == 1)
            {
                entry = se[0];
                return true;
            }

            entry = default;
            return false;
        }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType pmType)
        {
            if (pmType != PluginMenuType.Entry) return null;

            var tsCreate = new ToolStripMenuItem(Properties.Strings.Plugin_Menu_Create);
            tsCreate.Click += TsCreateEdit_Click;

            var tsEdit = new ToolStripMenuItem(Properties.Strings.Plugin_Menu_Edit);
            tsEdit.Click += TsCreateEdit_Click;

            var tsCopy = new ToolStripMenuItem(Properties.Strings.Plugin_Menu_Copy);
            tsCopy.Click += TsCopy_Click;

            var tsView = new ToolStripMenuItem(Properties.Strings.Plugin_Menu_View);
            tsView.Click += TsView_Click;

            var tsGroup = new ToolStripMenuItem("OTP for Yandex 2FA", null, new[] { tsCreate, tsCopy, tsView, tsEdit });
            tsGroup.OwnerChanged += (o, e) =>
            {
                if (tsGroup.Owner is ToolStripDropDown tsdd)
                {
                    tsdd.Opening += (oo, ee) => tsGroup.Enabled = _host.MainWindow.GetSelectedEntriesCount() == 1;
                }
            };
            tsGroup.DropDownOpening += (o, e) =>
            {
                var otpDataExists = (TryGetSingleSelectedEntry(out var entry) && entry.Strings.Exists(OtpDataUtils.Key));
                tsCopy.Visible = tsView.Visible = tsEdit.Visible = otpDataExists;
                tsCreate.Visible = !otpDataExists;
            };

            return tsGroup;
        }

        private delegate void OnSuccessDelegate(Yaotp yaotp);

        private void ProcessEntry(PwEntry entry, OnSuccessDelegate onSuccess)
        {
            if (entry.Strings.Exists(OtpDataUtils.Key))
            {
                var data = entry.Strings.Get(OtpDataUtils.Key).ReadString();
                if (OtpDataUtils.TryParseOtpData(data, out var secret, out var pin) && pin.Length == secret.PinLength)
                    onSuccess(new Yaotp(secret, pin, () => DateTime.UtcNow));
                else
                    MessageBox.Show(_host.MainWindow, string.Format(Properties.Strings.Plugin_InvalidData, OtpDataUtils.Key), Properties.Strings.Plugin_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show(_host.MainWindow, Properties.Strings.Plugin_DataNotFound, Properties.Strings.Plugin_Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowViewForm(PwEntry entry)
        {
            ProcessEntry(entry, yaotp =>
            {
                var viewForm = new ViewForm(yaotp, entry, _host);
                viewForm.ShowDialog(_host.MainWindow);
            });
        }

        private void TsCreateEdit_Click(object sender, EventArgs e)
        {
            if (TryGetSingleSelectedEntry(out var entry))
            {
                var createEditForm = new CreateEditForm(entry, _host);
                if (createEditForm.ShowDialog(_host.MainWindow) == DialogResult.OK)
                    ShowViewForm(entry);
            }
        }

        private void TsView_Click(object sender, EventArgs e)
        {
            if (TryGetSingleSelectedEntry(out var entry))
                ShowViewForm(entry);
        }

        private void TsCopy_Click(object sender, EventArgs e)
        {
            if (TryGetSingleSelectedEntry(out var entry))
            {
                ProcessEntry(entry, yaotp =>
                {
                    var otpString = yaotp.ComputeOtp();
                    if (ClipboardUtil.CopyAndMinimize(new KeePassLib.Security.ProtectedString(true, otpString), true, _host.MainWindow, entry, _host.Database))
                        _host.MainWindow.StartClipboardCountdown();
                });
            }
        }


    }
}
