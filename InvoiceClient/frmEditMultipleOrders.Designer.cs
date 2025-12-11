namespace InvoiceClient
{
    partial class frmEditMultipleOrders
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControlOrders;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControlOrders = new System.Windows.Forms.TabControl();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tabControlOrders
            // 
            this.tabControlOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlOrders.Location = new System.Drawing.Point(0, 0);
            this.tabControlOrders.Name = "tabControlOrders";
            this.tabControlOrders.SelectedIndex = 0;
            this.tabControlOrders.Size = new System.Drawing.Size(800, 550);
            this.tabControlOrders.TabIndex = 0;
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.BackColor = System.Drawing.Color.Green;
            this.btnSaveAll.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSaveAll.Location = new System.Drawing.Point(632, 560);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAll.TabIndex = 1;
            this.btnSaveAll.Text = "Lưu Tất Cả";
            this.btnSaveAll.UseVisualStyleBackColor = false;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(713, 560);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmEditMultipleOrders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 590);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.tabControlOrders);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditMultipleOrders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chỉnh sửa nhiều Orders";
            this.Load += new System.EventHandler(this.frmEditMultipleOrders_Load);
            this.ResumeLayout(false);
        }
    }
}

