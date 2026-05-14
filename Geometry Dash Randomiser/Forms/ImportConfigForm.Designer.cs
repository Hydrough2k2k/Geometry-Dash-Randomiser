namespace Geometry_Dash_Randomiser {

      partial class ImportConfigForm {
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
                  this.importLabel = new System.Windows.Forms.Label();
                  this.importTextBox = new System.Windows.Forms.RichTextBox();
                  this.acceptButton = new System.Windows.Forms.Button();
                  this.cancelButton = new System.Windows.Forms.Button();
                  this.pasteFromClipboardButton = new System.Windows.Forms.Button();
                  this.SuspendLayout();
                  // 
                  // importLabel
                  // 
                  this.importLabel.AutoSize = true;
                  this.importLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
                  this.importLabel.Location = new System.Drawing.Point(32, 12);
                  this.importLabel.Name = "importLabel";
                  this.importLabel.Size = new System.Drawing.Size(116, 22);
                  this.importLabel.TabIndex = 109;
                  this.importLabel.Text = "Import Config";
                  // 
                  // importTextBox
                  // 
                  this.importTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                  this.importTextBox.Location = new System.Drawing.Point(12, 46);
                  this.importTextBox.Name = "importTextBox";
                  this.importTextBox.Size = new System.Drawing.Size(310, 91);
                  this.importTextBox.TabIndex = 108;
                  this.importTextBox.Text = "";
                  // 
                  // acceptButton
                  // 
                  this.acceptButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
                  this.acceptButton.Location = new System.Drawing.Point(171, 144);
                  this.acceptButton.Name = "acceptButton";
                  this.acceptButton.Size = new System.Drawing.Size(153, 30);
                  this.acceptButton.TabIndex = 110;
                  this.acceptButton.Text = "Accept";
                  this.acceptButton.UseVisualStyleBackColor = true;
                  this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
                  // 
                  // cancelButton
                  // 
                  this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
                  this.cancelButton.Location = new System.Drawing.Point(12, 144);
                  this.cancelButton.Name = "cancelButton";
                  this.cancelButton.Size = new System.Drawing.Size(153, 30);
                  this.cancelButton.TabIndex = 111;
                  this.cancelButton.Text = "Cancel";
                  this.cancelButton.UseVisualStyleBackColor = true;
                  this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
                  // 
                  // pasteFromClipboardButton
                  // 
                  this.pasteFromClipboardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
                  this.pasteFromClipboardButton.Location = new System.Drawing.Point(174, 12);
                  this.pasteFromClipboardButton.Name = "pasteFromClipboardButton";
                  this.pasteFromClipboardButton.Size = new System.Drawing.Size(150, 28);
                  this.pasteFromClipboardButton.TabIndex = 112;
                  this.pasteFromClipboardButton.Text = "Paste from Clipboard";
                  this.pasteFromClipboardButton.UseVisualStyleBackColor = true;
                  this.pasteFromClipboardButton.Click += new System.EventHandler(this.pasteFromClipboardButton_Click);
                  // 
                  // ImportConfigForm
                  // 
                  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                  this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                  this.ClientSize = new System.Drawing.Size(334, 186);
                  this.Controls.Add(this.pasteFromClipboardButton);
                  this.Controls.Add(this.cancelButton);
                  this.Controls.Add(this.acceptButton);
                  this.Controls.Add(this.importLabel);
                  this.Controls.Add(this.importTextBox);
                  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                  this.MaximizeBox = false;
                  this.Name = "ImportConfigForm";
                  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                  this.Text = "ImportConfigForm";
                  this.Activated += new System.EventHandler(this._Activated);
                  this.Deactivate += new System.EventHandler(this._Deactivate);
                  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
                  this.ResumeLayout(false);
                  this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.Label importLabel;
            private System.Windows.Forms.RichTextBox importTextBox;
            private System.Windows.Forms.Button acceptButton;
            private System.Windows.Forms.Button cancelButton;
            private System.Windows.Forms.Button pasteFromClipboardButton;
      }
}