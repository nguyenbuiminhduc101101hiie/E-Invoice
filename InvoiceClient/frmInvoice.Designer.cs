namespace InvoiceClient
{
    partial class frmInvoice
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInvoice));
            this.btnCreateInvoice = new System.Windows.Forms.Button();
            this.btnGetInvoice = new System.Windows.Forms.Button();
            this.btnSavePDF = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCreateInvoiceDraft = new System.Windows.Forms.Button();
            this.btnUpdatePaymentStatus = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.chkExample = new System.Windows.Forms.CheckBox();
            this.chkProxy = new System.Windows.Forms.CheckBox();
            this.chkSSL = new System.Windows.Forms.CheckBox();
            this.txtProxy = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lậpHóaĐơnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lậpNhápHóaĐơnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdexit = new System.Windows.Forms.Button();
            this.cmdsearch = new System.Windows.Forms.Button();
            this.txtcustomerphone = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtjson = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbocustomername = new System.Windows.Forms.ComboBox();
            this.chkcusname = new System.Windows.Forms.CheckBox();
            this.chkempoyeename = new System.Windows.Forms.CheckBox();
            this.cboemployeename = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txttaxcode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtvat = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtmauso = new System.Windows.Forms.TextBox();
            this.txtinvseries = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkchuyenL = new System.Windows.Forms.CheckBox();
            this.chkcanhan = new System.Windows.Forms.CheckBox();
            this.chk_searchby_PaymentDate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreateInvoice
            // 
            this.btnCreateInvoice.BackColor = System.Drawing.Color.Maroon;
            this.btnCreateInvoice.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCreateInvoice.Location = new System.Drawing.Point(193, 68);
            this.btnCreateInvoice.Name = "btnCreateInvoice";
            this.btnCreateInvoice.Size = new System.Drawing.Size(149, 40);
            this.btnCreateInvoice.TabIndex = 0;
            this.btnCreateInvoice.Text = "Lập hóa đơn";
            this.btnCreateInvoice.UseVisualStyleBackColor = false;
            this.btnCreateInvoice.Click += new System.EventHandler(this.btnCreateInvoice_Click);
            // 
            // btnGetInvoice
            // 
            this.btnGetInvoice.Location = new System.Drawing.Point(273, 320);
            this.btnGetInvoice.Name = "btnGetInvoice";
            this.btnGetInvoice.Size = new System.Drawing.Size(149, 40);
            this.btnGetInvoice.TabIndex = 1;
            this.btnGetInvoice.Text = "Danh sách hóa đơn theo khoảng thời gian";
            this.btnGetInvoice.UseVisualStyleBackColor = true;
            this.btnGetInvoice.Visible = false;
            this.btnGetInvoice.Click += new System.EventHandler(this.btnGetInvoice_Click);
            // 
            // btnSavePDF
            // 
            this.btnSavePDF.Location = new System.Drawing.Point(347, 68);
            this.btnSavePDF.Name = "btnSavePDF";
            this.btnSavePDF.Size = new System.Drawing.Size(149, 40);
            this.btnSavePDF.TabIndex = 2;
            this.btnSavePDF.Text = "Tải file pdf hóa đơn";
            this.btnSavePDF.UseVisualStyleBackColor = true;
            this.btnSavePDF.Click += new System.EventHandler(this.btnSavePDF_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(122, 361);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(149, 40);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Hủy hóa đơn";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCreateInvoiceDraft
            // 
            this.btnCreateInvoiceDraft.BackColor = System.Drawing.Color.PeachPuff;
            this.btnCreateInvoiceDraft.Location = new System.Drawing.Point(40, 68);
            this.btnCreateInvoiceDraft.Name = "btnCreateInvoiceDraft";
            this.btnCreateInvoiceDraft.Size = new System.Drawing.Size(149, 40);
            this.btnCreateInvoiceDraft.TabIndex = 4;
            this.btnCreateInvoiceDraft.Text = "Lập hóa đơn nháp";
            this.btnCreateInvoiceDraft.UseVisualStyleBackColor = false;
            this.btnCreateInvoiceDraft.Click += new System.EventHandler(this.btnCreateInvoiceDraft_Click);
            // 
            // btnUpdatePaymentStatus
            // 
            this.btnUpdatePaymentStatus.Location = new System.Drawing.Point(273, 361);
            this.btnUpdatePaymentStatus.Name = "btnUpdatePaymentStatus";
            this.btnUpdatePaymentStatus.Size = new System.Drawing.Size(149, 40);
            this.btnUpdatePaymentStatus.TabIndex = 5;
            this.btnUpdatePaymentStatus.Text = "Cập nhật trạng thái thanh toán";
            this.btnUpdatePaymentStatus.UseVisualStyleBackColor = true;
            this.btnUpdatePaymentStatus.Visible = false;
            this.btnUpdatePaymentStatus.Click += new System.EventHandler(this.btnUpdatePaymentStatus_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnEdit.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnEdit.Location = new System.Drawing.Point(40, 114);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(149, 40);
            this.btnEdit.TabIndex = 54;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // txtBody
            // 
            this.txtBody.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBody.Location = new System.Drawing.Point(1028, 68);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBody.Size = new System.Drawing.Size(151, 103);
            this.txtBody.TabIndex = 7;
            this.txtBody.WordWrap = false;
            // 
            // chkExample
            // 
            this.chkExample.AutoSize = true;
            this.chkExample.Checked = true;
            this.chkExample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExample.Location = new System.Drawing.Point(312, 20);
            this.chkExample.Name = "chkExample";
            this.chkExample.Size = new System.Drawing.Size(143, 17);
            this.chkExample.TabIndex = 8;
            this.chkExample.Text = "Lấy giá trị Json mặc định";
            this.chkExample.UseVisualStyleBackColor = true;
            // 
            // chkProxy
            // 
            this.chkProxy.AutoSize = true;
            this.chkProxy.Location = new System.Drawing.Point(211, 19);
            this.chkProxy.Name = "chkProxy";
            this.chkProxy.Size = new System.Drawing.Size(95, 17);
            this.chkProxy.TabIndex = 9;
            this.chkProxy.Text = "Sử dụng Proxy";
            this.chkProxy.UseVisualStyleBackColor = true;
            this.chkProxy.Visible = false;
            this.chkProxy.CheckedChanged += new System.EventHandler(this.chkProxy_CheckedChanged);
            // 
            // chkSSL
            // 
            this.chkSSL.AutoSize = true;
            this.chkSSL.Checked = true;
            this.chkSSL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSSL.Location = new System.Drawing.Point(211, 41);
            this.chkSSL.Name = "chkSSL";
            this.chkSSL.Size = new System.Drawing.Size(91, 17);
            this.chkSSL.TabIndex = 10;
            this.chkSSL.Text = "Cấu hình SSL";
            this.chkSSL.UseVisualStyleBackColor = true;
            this.chkSSL.Visible = false;
            // 
            // txtProxy
            // 
            this.txtProxy.Location = new System.Drawing.Point(47, 16);
            this.txtProxy.Name = "txtProxy";
            this.txtProxy.Size = new System.Drawing.Size(88, 20);
            this.txtProxy.TabIndex = 11;
            this.txtProxy.Visible = false;
            this.txtProxy.Enter += new System.EventHandler(this.txtProxy_Enter);
            this.txtProxy.Leave += new System.EventHandler(this.txtProxy_Leave);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(149, 16);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(47, 20);
            this.txtPort.TabIndex = 12;
            this.txtPort.Visible = false;
            this.txtPort.Enter += new System.EventHandler(this.txtPort_Enter);
            this.txtPort.Leave += new System.EventHandler(this.txtPort_Leave);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Location = new System.Drawing.Point(15, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1146, 374);
            this.dataGridView1.TabIndex = 13;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lậpHóaĐơnToolStripMenuItem,
            this.lậpNhápHóaĐơnToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(171, 48);
            // 
            // lậpHóaĐơnToolStripMenuItem
            // 
            this.lậpHóaĐơnToolStripMenuItem.Name = "lậpHóaĐơnToolStripMenuItem";
            this.lậpHóaĐơnToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.lậpHóaĐơnToolStripMenuItem.Text = "Lập hóa đơn";
            this.lậpHóaĐơnToolStripMenuItem.Click += new System.EventHandler(this.lậpHóaĐơnToolStripMenuItem_Click);
            // 
            // lậpNhápHóaĐơnToolStripMenuItem
            // 
            this.lậpNhápHóaĐơnToolStripMenuItem.Name = "lậpNhápHóaĐơnToolStripMenuItem";
            this.lậpNhápHóaĐơnToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.lậpNhápHóaĐơnToolStripMenuItem.Text = "Lập nháp hóa đơn";
            this.lậpNhápHóaĐơnToolStripMenuItem.Click += new System.EventHandler(this.lậpNhápHóaĐơnToolStripMenuItem_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(532, 17);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(100, 20);
            this.dateTimePicker1.TabIndex = 14;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(705, 17);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(100, 20);
            this.dateTimePicker2.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(502, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Từ :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(673, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "đến :";
            // 
            // cmdexit
            // 
            this.cmdexit.Location = new System.Drawing.Point(947, 95);
            this.cmdexit.Name = "cmdexit";
            this.cmdexit.Size = new System.Drawing.Size(75, 23);
            this.cmdexit.TabIndex = 18;
            this.cmdexit.Text = "Exit";
            this.cmdexit.UseVisualStyleBackColor = true;
            this.cmdexit.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdsearch
            // 
            this.cmdsearch.Location = new System.Drawing.Point(870, 95);
            this.cmdsearch.Name = "cmdsearch";
            this.cmdsearch.Size = new System.Drawing.Size(75, 23);
            this.cmdsearch.TabIndex = 19;
            this.cmdsearch.Text = "Search";
            this.cmdsearch.UseVisualStyleBackColor = true;
            this.cmdsearch.Click += new System.EventHandler(this.cmdsearch_Click);
            // 
            // txtcustomerphone
            // 
            this.txtcustomerphone.Location = new System.Drawing.Point(947, 2);
            this.txtcustomerphone.Name = "txtcustomerphone";
            this.txtcustomerphone.Size = new System.Drawing.Size(239, 20);
            this.txtcustomerphone.TabIndex = 27;
            this.txtcustomerphone.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(854, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Customer Phone :";
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(532, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Customer Name:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(40, 163);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1178, 411);
            this.tabControl1.TabIndex = 34;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1170, 385);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Invoice";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtjson);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1170, 385);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Json";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtjson
            // 
            this.txtjson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtjson.Location = new System.Drawing.Point(15, 6);
            this.txtjson.Multiline = true;
            this.txtjson.Name = "txtjson";
            this.txtjson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtjson.Size = new System.Drawing.Size(1149, 368);
            this.txtjson.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(532, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Employee Name:";
            // 
            // cbocustomername
            // 
            this.cbocustomername.FormattingEnabled = true;
            this.cbocustomername.Location = new System.Drawing.Point(625, 44);
            this.cbocustomername.Name = "cbocustomername";
            this.cbocustomername.Size = new System.Drawing.Size(239, 21);
            this.cbocustomername.TabIndex = 40;
            // 
            // chkcusname
            // 
            this.chkcusname.AutoSize = true;
            this.chkcusname.Location = new System.Drawing.Point(874, 48);
            this.chkcusname.Name = "chkcusname";
            this.chkcusname.Size = new System.Drawing.Size(78, 17);
            this.chkcusname.TabIndex = 41;
            this.chkcusname.Text = "Cus Name ";
            this.chkcusname.UseVisualStyleBackColor = true;
            // 
            // chkempoyeename
            // 
            this.chkempoyeename.AutoSize = true;
            this.chkempoyeename.Location = new System.Drawing.Point(874, 68);
            this.chkempoyeename.Name = "chkempoyeename";
            this.chkempoyeename.Size = new System.Drawing.Size(103, 17);
            this.chkempoyeename.TabIndex = 42;
            this.chkempoyeename.Text = "Employee Name";
            this.chkempoyeename.UseVisualStyleBackColor = true;
            // 
            // cboemployeename
            // 
            this.cboemployeename.FormattingEnabled = true;
            this.cboemployeename.Location = new System.Drawing.Point(625, 68);
            this.cboemployeename.Name = "cboemployeename";
            this.cboemployeename.Size = new System.Drawing.Size(239, 21);
            this.cboemployeename.TabIndex = 43;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(532, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Tax Code:";
            // 
            // txttaxcode
            // 
            this.txttaxcode.Location = new System.Drawing.Point(625, 95);
            this.txttaxcode.Name = "txttaxcode";
            this.txttaxcode.Size = new System.Drawing.Size(77, 20);
            this.txttaxcode.TabIndex = 44;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(531, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 47;
            this.label4.Text = "Vat (%)";
            // 
            // txtvat
            // 
            this.txtvat.Location = new System.Drawing.Point(625, 121);
            this.txtvat.Name = "txtvat";
            this.txtvat.Size = new System.Drawing.Size(77, 20);
            this.txtvat.TabIndex = 46;
            this.txtvat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtvat_KeyPress);
            this.txtvat.Leave += new System.EventHandler(this.txtvat_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(708, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 48;
            this.label5.Text = "Mẫu số:";
            // 
            // txtmauso
            // 
            this.txtmauso.Location = new System.Drawing.Point(787, 95);
            this.txtmauso.Name = "txtmauso";
            this.txtmauso.Size = new System.Drawing.Size(77, 20);
            this.txtmauso.TabIndex = 49;
            this.txtmauso.TextChanged += new System.EventHandler(this.txtmauso_TextChanged);
            // 
            // txtinvseries
            // 
            this.txtinvseries.Location = new System.Drawing.Point(787, 120);
            this.txtinvseries.Name = "txtinvseries";
            this.txtinvseries.Size = new System.Drawing.Size(77, 20);
            this.txtinvseries.TabIndex = 51;
            this.txtinvseries.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(708, 123);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 50;
            this.label9.Text = "invoice Series";
            // 
            // chkchuyenL
            // 
            this.chkchuyenL.AutoSize = true;
            this.chkchuyenL.Location = new System.Drawing.Point(1080, 48);
            this.chkchuyenL.Name = "chkchuyenL";
            this.chkchuyenL.Size = new System.Drawing.Size(90, 17);
            this.chkchuyenL.TabIndex = 52;
            this.chkchuyenL.Text = "Chuyển -> Lít";
            this.chkchuyenL.UseVisualStyleBackColor = true;
            this.chkchuyenL.Visible = false;
            // 
            // chkcanhan
            // 
            this.chkcanhan.AutoSize = true;
            this.chkcanhan.Location = new System.Drawing.Point(1080, 25);
            this.chkcanhan.Name = "chkcanhan";
            this.chkcanhan.Size = new System.Drawing.Size(93, 17);
            this.chkcanhan.TabIndex = 53;
            this.chkcanhan.Text = "Xuất Cá Nhân";
            this.chkcanhan.UseVisualStyleBackColor = true;
            this.chkcanhan.Visible = false;
            // 
            // chk_searchby_PaymentDate
            // 
            this.chk_searchby_PaymentDate.AutoSize = true;
            this.chk_searchby_PaymentDate.Location = new System.Drawing.Point(811, 18);
            this.chk_searchby_PaymentDate.Name = "chk_searchby_PaymentDate";
            this.chk_searchby_PaymentDate.Size = new System.Drawing.Size(141, 17);
            this.chk_searchby_PaymentDate.TabIndex = 55;
            this.chk_searchby_PaymentDate.Text = "Lấy Theo Payment Date";
            this.chk_searchby_PaymentDate.UseVisualStyleBackColor = true;
            // 
            // frmInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 586);
            this.Controls.Add(this.chk_searchby_PaymentDate);
            this.Controls.Add(this.chkcanhan);
            this.Controls.Add(this.chkchuyenL);
            this.Controls.Add(this.txtinvseries);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtmauso);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtvat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txttaxcode);
            this.Controls.Add(this.cboemployeename);
            this.Controls.Add(this.chkempoyeename);
            this.Controls.Add(this.chkcusname);
            this.Controls.Add(this.cbocustomername);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtcustomerphone);
            this.Controls.Add(this.cmdsearch);
            this.Controls.Add(this.cmdexit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtProxy);
            this.Controls.Add(this.chkSSL);
            this.Controls.Add(this.chkProxy);
            this.Controls.Add(this.chkExample);
            this.Controls.Add(this.txtBody);
            this.Controls.Add(this.btnUpdatePaymentStatus);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnCreateInvoiceDraft);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSavePDF);
            this.Controls.Add(this.btnGetInvoice);
            this.Controls.Add(this.btnCreateInvoice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmInvoice";
            this.Text = "NPPBaoNgoc";
            this.Load += new System.EventHandler(this.frmInvoice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateInvoice;
        private System.Windows.Forms.Button btnGetInvoice;
        private System.Windows.Forms.Button btnSavePDF;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCreateInvoiceDraft;
        private System.Windows.Forms.Button btnUpdatePaymentStatus;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.CheckBox chkExample;
        private System.Windows.Forms.CheckBox chkProxy;
        private System.Windows.Forms.CheckBox chkSSL;
        private System.Windows.Forms.TextBox txtProxy;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdexit;
        private System.Windows.Forms.Button cmdsearch;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem lậpHóaĐơnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lậpNhápHóaĐơnToolStripMenuItem;
        private System.Windows.Forms.TextBox txtcustomerphone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtjson;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbocustomername;
        private System.Windows.Forms.CheckBox chkcusname;
        private System.Windows.Forms.CheckBox chkempoyeename;
        private System.Windows.Forms.ComboBox cboemployeename;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txttaxcode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtvat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtmauso;
        private System.Windows.Forms.TextBox txtinvseries;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkchuyenL;
        private System.Windows.Forms.CheckBox chkcanhan;
        private System.Windows.Forms.CheckBox chk_searchby_PaymentDate;
    }
}

