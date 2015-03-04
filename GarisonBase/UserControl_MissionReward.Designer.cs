namespace Herbfunk.GarrisonBase
{
    partial class UserControl_MissionReward
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.numericUpDown_Priority = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_SuccessRate = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Priority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SuccessRate)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown_Priority
            // 
            this.numericUpDown_Priority.Dock = System.Windows.Forms.DockStyle.Left;
            this.numericUpDown_Priority.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_Priority.Location = new System.Drawing.Point(0, 0);
            this.numericUpDown_Priority.Name = "numericUpDown_Priority";
            this.numericUpDown_Priority.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_Priority.TabIndex = 0;
            // 
            // numericUpDown_SuccessRate
            // 
            this.numericUpDown_SuccessRate.Dock = System.Windows.Forms.DockStyle.Right;
            this.numericUpDown_SuccessRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_SuccessRate.Location = new System.Drawing.Point(268, 0);
            this.numericUpDown_SuccessRate.Name = "numericUpDown_SuccessRate";
            this.numericUpDown_SuccessRate.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_SuccessRate.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(48, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UserControl_MissionReward
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown_SuccessRate);
            this.Controls.Add(this.numericUpDown_Priority);
            this.Name = "UserControl_MissionReward";
            this.Size = new System.Drawing.Size(316, 24);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Priority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SuccessRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_Priority;
        private System.Windows.Forms.NumericUpDown numericUpDown_SuccessRate;
        private System.Windows.Forms.Label label1;
    }
}
