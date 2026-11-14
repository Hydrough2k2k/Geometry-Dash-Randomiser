namespace Geometry_Dash_Randomiser {
      partial class ChangelogForm {
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
                  this.ChangeLogTextBox = new System.Windows.Forms.RichTextBox();
                  this.nextVersionButton = new System.Windows.Forms.Button();
                  this.previousVersionButton = new System.Windows.Forms.Button();
                  this.HeaderLabel = new System.Windows.Forms.Label();
                  this.VersionLabel = new System.Windows.Forms.Label();
                  this.ReleaseDateLabel = new System.Windows.Forms.Label();
                  this.LengthMeasurerLabel_Hidden = new System.Windows.Forms.Label();
                  this.SuspendLayout();
                  // 
                  // ChangeLogTextBox
                  // 
                  this.ChangeLogTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                  this.ChangeLogTextBox.Location = new System.Drawing.Point(12, 90);
                  this.ChangeLogTextBox.Name = "ChangeLogTextBox";
                  this.ChangeLogTextBox.ReadOnly = true;
                  this.ChangeLogTextBox.Size = new System.Drawing.Size(727, 10);
                  this.ChangeLogTextBox.TabIndex = 110;
                  this.ChangeLogTextBox.Text = "";
                  // 
                  // nextVersionButton
                  // 
                  this.nextVersionButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
                  this.nextVersionButton.Location = new System.Drawing.Point(647, 12);
                  this.nextVersionButton.Name = "nextVersionButton";
                  this.nextVersionButton.Size = new System.Drawing.Size(90, 50);
                  this.nextVersionButton.TabIndex = 116;
                  this.nextVersionButton.Text = "Next Version";
                  this.nextVersionButton.UseVisualStyleBackColor = true;
                  this.nextVersionButton.Click += new System.EventHandler(this.NextVersion);
                  // 
                  // previousVersionButton
                  // 
                  this.previousVersionButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
                  this.previousVersionButton.Location = new System.Drawing.Point(12, 12);
                  this.previousVersionButton.Name = "previousVersionButton";
                  this.previousVersionButton.Size = new System.Drawing.Size(90, 50);
                  this.previousVersionButton.TabIndex = 117;
                  this.previousVersionButton.Text = "Previous Version";
                  this.previousVersionButton.UseVisualStyleBackColor = true;
                  this.previousVersionButton.Click += new System.EventHandler(this.PreviousVersion);
                  // 
                  // HeaderLabel
                  // 
                  this.HeaderLabel.AutoSize = true;
                  this.HeaderLabel.Font = new System.Drawing.Font("Javanese Text", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                  this.HeaderLabel.Location = new System.Drawing.Point(213, 2);
                  this.HeaderLabel.Name = "HeaderLabel";
                  this.HeaderLabel.Size = new System.Drawing.Size(186, 73);
                  this.HeaderLabel.TabIndex = 200;
                  this.HeaderLabel.Text = "Changelog";
                  // 
                  // VersionLabel
                  // 
                  this.VersionLabel.AutoSize = true;
                  this.VersionLabel.Font = new System.Drawing.Font("Javanese Text", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                  this.VersionLabel.Location = new System.Drawing.Point(149, 47);
                  this.VersionLabel.Name = "VersionLabel";
                  this.VersionLabel.Size = new System.Drawing.Size(87, 43);
                  this.VersionLabel.TabIndex = 119;
                  this.VersionLabel.Text = "V_._._._";
                  // 
                  // ReleaseDateLabel
                  // 
                  this.ReleaseDateLabel.AutoSize = true;
                  this.ReleaseDateLabel.Font = new System.Drawing.Font("Javanese Text", 12.25F);
                  this.ReleaseDateLabel.Location = new System.Drawing.Point(329, 52);
                  this.ReleaseDateLabel.Name = "ReleaseDateLabel";
                  this.ReleaseDateLabel.Size = new System.Drawing.Size(143, 38);
                  this.ReleaseDateLabel.TabIndex = 122;
                  this.ReleaseDateLabel.Text = "DD MMM YYYY";
                  // 
                  // LengthMeasurerLabel_Hidden
                  // 
                  this.LengthMeasurerLabel_Hidden.AutoSize = true;
                  this.LengthMeasurerLabel_Hidden.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                  this.LengthMeasurerLabel_Hidden.Location = new System.Drawing.Point(497, 65);
                  this.LengthMeasurerLabel_Hidden.Name = "LengthMeasurerLabel_Hidden";
                  this.LengthMeasurerLabel_Hidden.Size = new System.Drawing.Size(208, 20);
                  this.LengthMeasurerLabel_Hidden.TabIndex = 123;
                  this.LengthMeasurerLabel_Hidden.Text = "Hidden Text Measurer Label";
                  this.LengthMeasurerLabel_Hidden.Visible = false;
                  // 
                  // ChangelogForm
                  // 
                  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                  this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                  this.ClientSize = new System.Drawing.Size(749, 334);
                  this.Controls.Add(this.LengthMeasurerLabel_Hidden);
                  this.Controls.Add(this.ReleaseDateLabel);
                  this.Controls.Add(this.ChangeLogTextBox);
                  this.Controls.Add(this.VersionLabel);
                  this.Controls.Add(this.HeaderLabel);
                  this.Controls.Add(this.previousVersionButton);
                  this.Controls.Add(this.nextVersionButton);
                  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                  this.Name = "ChangelogForm";
                  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                  this.Text = "ChangelogForm";
                  this.Activated += new System.EventHandler(this.On_Activated);
                  this.Deactivate += new System.EventHandler(this.On_Deactivate);
                  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.On_FormClosing);
                  this.ResumeLayout(false);
                  this.PerformLayout();

            }

            #endregion
            private System.Windows.Forms.RichTextBox ChangeLogTextBox;
            private System.Windows.Forms.Button nextVersionButton;
            private System.Windows.Forms.Button previousVersionButton;
            private System.Windows.Forms.Label HeaderLabel;
            private System.Windows.Forms.Label VersionLabel;
            private System.Windows.Forms.Label ReleaseDateLabel;
            private System.Windows.Forms.Label LengthMeasurerLabel_Hidden;
      }
}