/*
 * User: Thibault MONTAUFRAY
 */
namespace Droid_Audio
{
	partial class Eject
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelDesc = new System.Windows.Forms.Label();
            this.comboBoxCDList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(126, 10);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(35, 21);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOKClick);
            // 
            // labelDesc
            // 
            this.labelDesc.Location = new System.Drawing.Point(12, 13);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(63, 23);
            this.labelDesc.TabIndex = 2;
            this.labelDesc.Text = "Eject :";
            // 
            // comboBoxCDList
            // 
            this.comboBoxCDList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCDList.FormattingEnabled = true;
            this.comboBoxCDList.Location = new System.Drawing.Point(61, 10);
            this.comboBoxCDList.Name = "comboBoxCDList";
            this.comboBoxCDList.Size = new System.Drawing.Size(59, 21);
            this.comboBoxCDList.TabIndex = 3;
            // 
            // Eject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(173, 42);
            this.Controls.Add(this.comboBoxCDList);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Eject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Eject";
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.ComboBox comboBoxCDList;
	}
}
