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

namespace KeeYaOtp
{
    partial class CreateEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateEditForm));
            this.textSecret = new System.Windows.Forms.TextBox();
            this.textPin = new System.Windows.Forms.TextBox();
            this.labelSecret = new System.Windows.Forms.Label();
            this.labelPin = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.errorProviderSecret = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderPin = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderSecret)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderPin)).BeginInit();
            this.SuspendLayout();
            // 
            // textSecret
            // 
            this.errorProviderSecret.SetIconAlignment(this.textSecret, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textSecret.IconAlignment"))));
            this.errorProviderSecret.SetIconPadding(this.textSecret, ((int)(resources.GetObject("textSecret.IconPadding"))));
            resources.ApplyResources(this.textSecret, "textSecret");
            this.textSecret.Name = "textSecret";
            // 
            // textPin
            // 
            this.errorProviderPin.SetIconAlignment(this.textPin, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textPin.IconAlignment"))));
            this.errorProviderPin.SetIconPadding(this.textPin, ((int)(resources.GetObject("textPin.IconPadding"))));
            resources.ApplyResources(this.textPin, "textPin");
            this.textPin.Name = "textPin";
            // 
            // labelSecret
            // 
            resources.ApplyResources(this.labelSecret, "labelSecret");
            this.labelSecret.Name = "labelSecret";
            // 
            // labelPin
            // 
            resources.ApplyResources(this.labelPin, "labelPin");
            this.labelPin.Name = "labelPin";
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // errorProviderSecret
            // 
            this.errorProviderSecret.ContainerControl = this;
            // 
            // errorProviderPin
            // 
            this.errorProviderPin.ContainerControl = this;
            // 
            // CreateEditForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelPin);
            this.Controls.Add(this.labelSecret);
            this.Controls.Add(this.textPin);
            this.Controls.Add(this.textSecret);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateEditForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateEditForm_FormClosing);
            this.Shown += new System.EventHandler(this.CreateEditForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderSecret)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textSecret;
        private System.Windows.Forms.TextBox textPin;
        private System.Windows.Forms.Label labelSecret;
        private System.Windows.Forms.Label labelPin;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ErrorProvider errorProviderSecret;
        private System.Windows.Forms.ErrorProvider errorProviderPin;
    }
}