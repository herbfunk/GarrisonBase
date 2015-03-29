namespace Herbfunk.GarrisonBase.Config
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
            this.components = new System.ComponentModel.Container();
            this.numericUpDown_Priority = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_SuccessRate = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.numericUpDown_MinimumLevel = new System.Windows.Forms.NumericUpDown();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Priority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SuccessRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinimumLevel)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericUpDown_Priority
            // 
            this.numericUpDown_Priority.BackColor = System.Drawing.Color.Lime;
            this.numericUpDown_Priority.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown_Priority.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_Priority.Location = new System.Drawing.Point(3, 3);
            this.numericUpDown_Priority.Name = "numericUpDown_Priority";
            this.numericUpDown_Priority.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_Priority.TabIndex = 0;
            this.toolTip1.SetToolTip(this.numericUpDown_Priority, "The Priority of this type of mission compared to other types. Highest Priority ty" +
        "pes will be evaluated and started first. Setting this value to zero will ignore " +
        "it.");
            // 
            // numericUpDown_SuccessRate
            // 
            this.numericUpDown_SuccessRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.numericUpDown_SuccessRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown_SuccessRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_SuccessRate.Location = new System.Drawing.Point(57, 3);
            this.numericUpDown_SuccessRate.Name = "numericUpDown_SuccessRate";
            this.numericUpDown_SuccessRate.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_SuccessRate.TabIndex = 1;
            this.toolTip1.SetToolTip(this.numericUpDown_SuccessRate, "The minimum success for bonus reward.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown_MinimumLevel
            // 
            this.numericUpDown_MinimumLevel.BackColor = System.Drawing.Color.LightSkyBlue;
            this.numericUpDown_MinimumLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown_MinimumLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_MinimumLevel.Location = new System.Drawing.Point(111, 3);
            this.numericUpDown_MinimumLevel.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.numericUpDown_MinimumLevel.Name = "numericUpDown_MinimumLevel";
            this.numericUpDown_MinimumLevel.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_MinimumLevel.TabIndex = 3;
            this.toolTip1.SetToolTip(this.numericUpDown_MinimumLevel, "The minimum level required.");
            this.numericUpDown_MinimumLevel.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_Priority);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_SuccessRate);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_MinimumLevel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 20);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(295, 30);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // UserControl_MissionReward
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Name = "UserControl_MissionReward";
            this.Size = new System.Drawing.Size(295, 50);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Priority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SuccessRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinimumLevel)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_Priority;
        private System.Windows.Forms.NumericUpDown numericUpDown_SuccessRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown numericUpDown_MinimumLevel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
