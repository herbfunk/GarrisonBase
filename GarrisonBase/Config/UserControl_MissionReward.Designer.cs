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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.numericUpDown_Priority = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_SuccessRate = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_MinimumLevel = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.numericUpDown_Priority.Location = new System.Drawing.Point(253, 4);
            this.numericUpDown_Priority.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.numericUpDown_Priority.Name = "numericUpDown_Priority";
            this.numericUpDown_Priority.Size = new System.Drawing.Size(36, 23);
            this.numericUpDown_Priority.TabIndex = 7;
            this.toolTip1.SetToolTip(this.numericUpDown_Priority, "The Priority of this type of mission compared to other types. Highest Priority ty" +
        "pes will be evaluated and started first. Setting this value to zero will ignore " +
        "it.");
            // 
            // numericUpDown_SuccessRate
            // 
            this.numericUpDown_SuccessRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.numericUpDown_SuccessRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown_SuccessRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_SuccessRate.Location = new System.Drawing.Point(156, 4);
            this.numericUpDown_SuccessRate.Margin = new System.Windows.Forms.Padding(0, 4, 3, 0);
            this.numericUpDown_SuccessRate.Name = "numericUpDown_SuccessRate";
            this.numericUpDown_SuccessRate.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_SuccessRate.TabIndex = 8;
            this.toolTip1.SetToolTip(this.numericUpDown_SuccessRate, "The minimum success for bonus reward.");
            this.numericUpDown_SuccessRate.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numericUpDown_MinimumLevel
            // 
            this.numericUpDown_MinimumLevel.BackColor = System.Drawing.Color.LightSkyBlue;
            this.numericUpDown_MinimumLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown_MinimumLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_MinimumLevel.Location = new System.Drawing.Point(46, 4);
            this.numericUpDown_MinimumLevel.Margin = new System.Windows.Forms.Padding(0, 4, 3, 0);
            this.numericUpDown_MinimumLevel.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.numericUpDown_MinimumLevel.Name = "numericUpDown_MinimumLevel";
            this.numericUpDown_MinimumLevel.Size = new System.Drawing.Size(48, 23);
            this.numericUpDown_MinimumLevel.TabIndex = 9;
            this.toolTip1.SetToolTip(this.numericUpDown_MinimumLevel, "The minimum level required.");
            this.numericUpDown_MinimumLevel.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 26);
            this.label3.TabIndex = 12;
            this.label3.Text = "Min Lvl";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label3, "The minimum level required.");
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Location = new System.Drawing.Point(97, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 26);
            this.label2.TabIndex = 13;
            this.label2.Text = "Success";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label2, "The minimum success for bonus reward.");
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label4.Location = new System.Drawing.Point(207, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 26);
            this.label4.TabIndex = 14;
            this.label4.Text = "Priority";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label4, "The Priority of this type of mission compared to other types. Highest Priority ty" +
        "pes will be evaluated and started first. Setting this value to zero will ignore " +
        "it.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label1.Size = new System.Drawing.Size(60, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_MinimumLevel);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_SuccessRate);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_Priority);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 23);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(312, 33);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // UserControl_MissionReward
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 6, 8);
            this.Name = "UserControl_MissionReward";
            this.Size = new System.Drawing.Size(312, 56);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Priority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SuccessRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MinimumLevel)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.NumericUpDown numericUpDown_MinimumLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown_SuccessRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_Priority;
        private System.Windows.Forms.Label label4;
    }
}
