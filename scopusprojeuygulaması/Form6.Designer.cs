namespace scopusprojeuygulaması
{
    partial class Form6
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form6));
            this.genelDataView = new System.Windows.Forms.DataGridView();
            this.genelDataDupesiz = new System.Windows.Forms.DataGridView();
            this.genelDataMakale = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.genelDataView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genelDataDupesiz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genelDataMakale)).BeginInit();
            this.SuspendLayout();
            // 
            // genelDataView
            // 
            this.genelDataView.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.genelDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.genelDataView.Location = new System.Drawing.Point(16, 15);
            this.genelDataView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genelDataView.Name = "genelDataView";
            this.genelDataView.RowHeadersWidth = 51;
            this.genelDataView.Size = new System.Drawing.Size(697, 182);
            this.genelDataView.TabIndex = 0;
            // 
            // genelDataDupesiz
            // 
            this.genelDataDupesiz.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.genelDataDupesiz.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.genelDataDupesiz.Location = new System.Drawing.Point(16, 217);
            this.genelDataDupesiz.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genelDataDupesiz.Name = "genelDataDupesiz";
            this.genelDataDupesiz.RowHeadersWidth = 51;
            this.genelDataDupesiz.Size = new System.Drawing.Size(696, 182);
            this.genelDataDupesiz.TabIndex = 1;
            // 
            // genelDataMakale
            // 
            this.genelDataMakale.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.genelDataMakale.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.genelDataMakale.Location = new System.Drawing.Point(19, 427);
            this.genelDataMakale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genelDataMakale.Name = "genelDataMakale";
            this.genelDataMakale.RowHeadersWidth = 51;
            this.genelDataMakale.Size = new System.Drawing.Size(695, 169);
            this.genelDataMakale.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.DarkBlue;
            this.label1.Location = new System.Drawing.Point(721, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 28);
            this.label1.TabIndex = 3;
            this.label1.Text = "----> Dupeli";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.ForeColor = System.Drawing.Color.DarkBlue;
            this.label2.Location = new System.Drawing.Point(721, 284);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 28);
            this.label2.TabIndex = 4;
            this.label2.Text = "----> Dupesiz";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.DarkBlue;
            this.label3.Location = new System.Drawing.Point(721, 495);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 28);
            this.label3.TabIndex = 5;
            this.label3.Text = "----> Ham Makale Üzerinden";
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1000, 635);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.genelDataMakale);
            this.Controls.Add(this.genelDataDupesiz);
            this.Controls.Add(this.genelDataView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form6";
            this.Text = "Genel Rapor";
            this.Load += new System.EventHandler(this.Form6_Load);
            ((System.ComponentModel.ISupportInitialize)(this.genelDataView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genelDataDupesiz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genelDataMakale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView genelDataView;
        private System.Windows.Forms.DataGridView genelDataDupesiz;
        private System.Windows.Forms.DataGridView genelDataMakale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}