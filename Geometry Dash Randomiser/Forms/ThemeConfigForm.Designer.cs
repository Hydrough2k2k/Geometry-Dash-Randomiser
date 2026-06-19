namespace Geometry_Dash_Randomiser.Forms {
      partial class ThemeConfigForm {
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
                  this.headerLabel = new System.Windows.Forms.Label();
                  this.RandomThemeCheckbox = new System.Windows.Forms.CheckBox();
                  this.SystemThemeCheckbox = new System.Windows.Forms.CheckBox();
                  this.SuspendLayout();
                  // 
                  // headerLabel
                  // 
                  this.headerLabel.AutoSize = true;
                  this.headerLabel.Font = new System.Drawing.Font("Javanese Text", 18F);
                  this.headerLabel.Location = new System.Drawing.Point(12, 9);
                  this.headerLabel.Name = "headerLabel";
                  this.headerLabel.Size = new System.Drawing.Size(187, 54);
                  this.headerLabel.TabIndex = 2;
                  this.headerLabel.Text = "Theme Settings";
                  // 
                  // RandomThemeCheckbox
                  // 
                  this.RandomThemeCheckbox.AutoSize = true;
                  this.RandomThemeCheckbox.BackColor = System.Drawing.Color.Transparent;
                  this.RandomThemeCheckbox.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                  this.RandomThemeCheckbox.Location = new System.Drawing.Point(21, 66);
                  this.RandomThemeCheckbox.Name = "RandomThemeCheckbox";
                  this.RandomThemeCheckbox.Size = new System.Drawing.Size(204, 27);
                  this.RandomThemeCheckbox.TabIndex = 15;
                  this.RandomThemeCheckbox.Text = "Enable Random Theme";
                  this.RandomThemeCheckbox.UseVisualStyleBackColor = false;
                  this.RandomThemeCheckbox.Click += new System.EventHandler(this.RandomThemeCheckbox_Click);
                  // 
                  // SystemThemeCheckbox
                  // 
                  this.SystemThemeCheckbox.AutoSize = true;
                  this.SystemThemeCheckbox.BackColor = System.Drawing.Color.Transparent;
                  this.SystemThemeCheckbox.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                  this.SystemThemeCheckbox.Location = new System.Drawing.Point(21, 99);
                  this.SystemThemeCheckbox.Name = "SystemThemeCheckbox";
                  this.SystemThemeCheckbox.Size = new System.Drawing.Size(196, 27);
                  this.SystemThemeCheckbox.TabIndex = 16;
                  this.SystemThemeCheckbox.Text = "Enable System Theme";
                  this.SystemThemeCheckbox.UseVisualStyleBackColor = false;
                  this.SystemThemeCheckbox.Click += new System.EventHandler(this.SystemThemeCheckbox_Click);
                  // 
                  // ThemeConfigForm
                  // 
                  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                  this.ClientSize = new System.Drawing.Size(311, 141);
                  this.Controls.Add(this.SystemThemeCheckbox);
                  this.Controls.Add(this.RandomThemeCheckbox);
                  this.Controls.Add(this.headerLabel);
                  this.Name = "ThemeConfigForm";
                  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                  this.Text = "ThemeConfigForm";
                  this.Activated += new System.EventHandler(this.On_Activated);
                  this.Deactivate += new System.EventHandler(this.On_Deactivate);
                  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.On_FormClosing);
                  this.ResumeLayout(false);
                  this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.Label headerLabel;
            private System.Windows.Forms.CheckBox RandomThemeCheckbox;
            private System.Windows.Forms.CheckBox SystemThemeCheckbox;
      }
}