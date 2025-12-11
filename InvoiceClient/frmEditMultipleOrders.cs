using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace InvoiceClient
{
    public partial class frmEditMultipleOrders : Form
    {
        private List<string> orderIds;
        private SqlConnection cnn = new SqlConnection(@"Data Source=115.78.95.245;Initial Catalog=NPPBaoNgoc@;Integrated Security=false;user id=sa;password=qwerQWER1234!@#$");
        private DataTable dtServices;

        public frmEditMultipleOrders(List<string> orderIds)
        {
            InitializeComponent();
            this.orderIds = orderIds;
        }

        private void frmEditMultipleOrders_Load(object sender, EventArgs e)
        {
            LoadServices();
            LoadOrders();
        }

        private void LoadServices()
        {
            try
            {
                cnn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT id, Name FROM Services ORDER BY Name", cnn);
                dtServices = new DataTable();
                da.Fill(dtServices);
                cnn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách dịch vụ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
        }

        private void LoadOrders()
        {
            try
            {
                tabControlOrders.TabPages.Clear();

                foreach (string orderId in orderIds)
                {
                    // Load order data to get customer info for tab title
                    cnn.Open();
                    string sql = $"SELECT CustomerName, CustomerPhone FROM Orders WHERE id = '{orderId}'";
                    SqlCommand com = new SqlCommand(sql, cnn);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cnn.Close();

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        string customerName = row["CustomerName"]?.ToString() ?? "";
                        string customerPhone = row["CustomerPhone"]?.ToString() ?? "";
                        
                        // Create tab page
                        TabPage tabPage = new TabPage();
                        tabPage.Text = $"{customerName} ({customerPhone})";
                        tabPage.Name = orderId;

                        // Create form edit order as embedded control
                        frmEditOrder orderForm = new frmEditOrder(orderId);
                        orderForm.TopLevel = false;
                        orderForm.FormBorderStyle = FormBorderStyle.None;
                        orderForm.Dock = DockStyle.Fill;
                        orderForm.Parent = tabPage;
                        orderForm.Visible = true;

                        // Hide Save and Cancel buttons after form loads
                        orderForm.Load += (s, args) =>
                        {
                            Button btnSave = orderForm.Controls.Find("btnSave", true).FirstOrDefault() as Button;
                            Button btnCancel = orderForm.Controls.Find("btnCancel", true).FirstOrDefault() as Button;
                            if (btnSave != null) btnSave.Visible = false;
                            if (btnCancel != null) btnCancel.Visible = false;
                        };

                        tabControlOrders.TabPages.Add(tabPage);
                    }
                }

                if (tabControlOrders.TabPages.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu order nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu orders: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            try
            {
                int successCount = 0;
                int totalCount = tabControlOrders.TabPages.Count;

                foreach (TabPage tabPage in tabControlOrders.TabPages)
                {
                    try
                    {
                        // Find frmEditOrder in tab page
                        frmEditOrder orderForm = null;
                        foreach (Control control in tabPage.Controls)
                        {
                            if (control is frmEditOrder)
                            {
                                orderForm = control as frmEditOrder;
                                break;
                            }
                        }

                        if (orderForm != null)
                        {
                            if (orderForm.SaveOrder())
                            {
                                successCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi lưu order trong tab: {tabPage.Text}\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                MessageBox.Show($"Đã lưu thành công {successCount}/{totalCount} order(s)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                if (successCount == totalCount)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
