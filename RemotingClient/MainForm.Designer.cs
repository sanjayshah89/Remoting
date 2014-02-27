namespace RemotingClient
{
    partial class MainForm
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
            this.GVDeptDetails = new System.Windows.Forms.DataGridView();
            this.lblMainTitle = new System.Windows.Forms.Label();
            this.lblLoginMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.GVDeptDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // GVDeptDetails
            // 
            this.GVDeptDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GVDeptDetails.Location = new System.Drawing.Point(2, 98);
            this.GVDeptDetails.Name = "GVDeptDetails";
            this.GVDeptDetails.Size = new System.Drawing.Size(608, 342);
            this.GVDeptDetails.TabIndex = 0;
            // 
            // lblMainTitle
            // 
            this.lblMainTitle.AutoSize = true;
            this.lblMainTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainTitle.Location = new System.Drawing.Point(193, 42);
            this.lblMainTitle.Name = "lblMainTitle";
            this.lblMainTitle.Size = new System.Drawing.Size(213, 25);
            this.lblMainTitle.TabIndex = 1;
            this.lblMainTitle.Text = "Department Details";
            // 
            // lblLoginMessage
            // 
            this.lblLoginMessage.AutoSize = true;
            this.lblLoginMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoginMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblLoginMessage.Location = new System.Drawing.Point(13, 13);
            this.lblLoginMessage.Name = "lblLoginMessage";
            this.lblLoginMessage.Size = new System.Drawing.Size(0, 13);
            this.lblLoginMessage.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(612, 442);
            this.Controls.Add(this.lblLoginMessage);
            this.Controls.Add(this.lblMainTitle);
            this.Controls.Add(this.GVDeptDetails);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.GVDeptDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView GVDeptDetails;
        private System.Windows.Forms.Label lblMainTitle;
        private System.Windows.Forms.Label lblLoginMessage;
    }
}