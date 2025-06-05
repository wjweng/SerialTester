namespace SerialPorts
{
    partial class Alarm
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
            this.buttonAlarmDialogCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAlarmDialogCancel
            // 
            this.buttonAlarmDialogCancel.Location = new System.Drawing.Point(542, 42);
            this.buttonAlarmDialogCancel.Name = "buttonAlarmDialogCancel";
            this.buttonAlarmDialogCancel.Size = new System.Drawing.Size(78, 37);
            this.buttonAlarmDialogCancel.TabIndex = 0;
            this.buttonAlarmDialogCancel.Text = "Cancel";
            this.buttonAlarmDialogCancel.UseVisualStyleBackColor = true;
            this.buttonAlarmDialogCancel.Click += new System.EventHandler(this.buttonAlarmDialogCancel_Click);
            // 
            // Alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.buttonAlarmDialogCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Alarm";
            this.Text = "Alarm Configuration";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAlarmDialogCancel;
    }
}