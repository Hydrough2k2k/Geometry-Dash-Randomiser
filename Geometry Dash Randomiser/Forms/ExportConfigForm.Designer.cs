namespace Geometry_Dash_Randomiser {
      partial class ExportConfigForm {
            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing) {
                  if (disposing && (components != null)) {
                        components.Dispose();
                  }
                  base.Dispose(disposing);
            }

            #region Windows Form Designer generated code

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent() {
                  this.exportTextBox = new System.Windows.Forms.RichTextBox();
                  this.exportLabel = new System.Windows.Forms.Label();
                  this.copyToClipboardButton = new System.Windows.Forms.Button();
                  this.SuspendLayout();
                  // 
                  // exportTextBox
                  // 
                  this.exportTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                  this.exportTextBox.Location = new System.Drawing.Point(12, 46);
                  this.exportTextBox.Name = "exportTextBox";
                  this.exportTextBox.ReadOnly = true;
                  this.exportTextBox.Size = new System.Drawing.Size(310, 128);
                  this.exportTextBox.TabIndex = 0;
                  this.exportTextBox.Text = "";
                  // 
                  // exportLabel
                  // 
                  this.exportLabel.AutoSize = true;
                  this.exportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
                  this.exportLabel.Location = new System.Drawing.Point(32, 12);
                  this.exportLabel.Name = "exportLabel";
                  this.exportLabel.Size = new System.Drawing.Size(114, 22);
                  this.exportLabel.TabIndex = 107;
                  this.exportLabel.Text = "Export String";
                  // 
                  // copyToClipboardButton
                  // 
                  this.copyToClipboardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
                  this.copyToClipboardButton.Location = new System.Drawing.Point(174, 12);
                  this.copyToClipboardButton.Name = "copyToClipboardButton";
                  this.copyToClipboardButton.Size = new System.Drawing.Size(150, 28);
                  this.copyToClipboardButton.TabIndex = 113;
                  this.copyToClipboardButton.Text = "Copy to Clipboard";
                  this.copyToClipboardButton.UseVisualStyleBackColor = true;
                  this.copyToClipboardButton.Click += new System.EventHandler(this.copyToClipboardButton_Click);
                  // 
                  // ExportConfigForm
                  // 
                  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                  this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                  this.ClientSize = new System.Drawing.Size(334, 186);
                  this.Controls.Add(this.copyToClipboardButton);
                  this.Controls.Add(this.exportLabel);
                  this.Controls.Add(this.exportTextBox);
                  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                  this.MaximizeBox = false;
                  this.Name = "ExportConfigForm";
                  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                  this.Text = "ExportConfigForm";
                  this.Activated += new System.EventHandler(this._Activated);
                  this.Deactivate += new System.EventHandler(this._Deactivate);
                  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
                  this.ResumeLayout(false);
                  this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.RichTextBox exportTextBox;
            private System.Windows.Forms.Label exportLabel;
            private System.Windows.Forms.Button copyToClipboardButton;
      }
}