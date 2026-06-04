using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace InvoiceClient
{
    public partial class frmInputInvoiceNo : Form
    {
        public string InvoiceNo { get; private set; }
        private SqlConnection cnn = new SqlConnection(@"Data Source=115.78.95.245;Initial Catalog=NPPBaoNgoc@;Integrated Security=false;user id=sa;password=qwerQWER1234!@#$");
        public string CustomerName { get; private set; }
        public frmInputInvoiceNo(string customerName)
        {
            InitializeComponent();
            CustomerName = customerName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            {
                MessageBox.Show("Vui lòng nhập số hóa đơn nội bộ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInvoiceNo.Focus();
                return;
            }

            InvoiceNo = txtInvoiceNo.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmInputInvoiceNo_Load(object sender, EventArgs e)
        {
            try
            {
                // Tự động tạo số hóa đơn theo format yyMMdd_STT_CustomerName (STT tăng trong tháng)
                string invoiceNo = GenerateInvoiceNo();
                txtInvoiceNo.Text = invoiceNo;
                txtInvoiceNo.SelectAll();
                txtInvoiceNo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo số hóa đơn tự động: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtInvoiceNo.Focus();
            }
        }

        private string GenerateInvoiceNo()
        {
            string today = DateTime.Now.ToString("yyMMdd");
            string monthPrefix = DateTime.Now.ToString("yyMM");
            int nextSTT = 1;

            try
            {
                cnn.Open();
                // Lấy tất cả các HoaDonNoiBo trong tháng hiện tại có format yyMMdd_STT_CustomerName
                string sql = "SELECT HoaDonNoiBo FROM orders WHERE HoaDonNoiBo IS NOT NULL AND HoaDonNoiBo LIKE '" + monthPrefix + "%'";
                SqlCommand com = new SqlCommand(sql, cnn);
                SqlDataReader reader = com.ExecuteReader();

                int maxSTT = 0;
                // Pattern để match format yyMMdd_STT_CustomerName, chỉ kiểm tra phần yyMM đầu tiên
                string pattern = @"^" + Regex.Escape(monthPrefix) + @"\d{2}_(\d+)_";

                while (reader.Read())
                {
                    string hoaDonNoiBo = reader["HoaDonNoiBo"]?.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(hoaDonNoiBo))
                    {
                        Match match = Regex.Match(hoaDonNoiBo, pattern);
                        if (match.Success)
                        {
                            if (int.TryParse(match.Groups[1].Value, out int stt))
                            {
                                if (stt > maxSTT)
                                {
                                    maxSTT = stt;
                                }
                            }
                        }
                    }
                }

                reader.Close();
                cnn.Close();

                // STT tiếp theo = maxSTT + 1
                nextSTT = maxSTT + 1;
            }
            catch (Exception ex)
            {
                if (cnn.State == System.Data.ConnectionState.Open)
                {
                    cnn.Close();
                }
                throw new Exception("Lỗi khi truy vấn database: " + ex.Message);
            }

            // Tạo số hóa đơn theo format yyMMdd_STT_CustomerName (STT tăng trong tháng, reset về 1 khi sang tháng mới)
            string safeCustomerName = CustomerName ?? "";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                safeCustomerName = safeCustomerName.Replace(c, '_');
            }
            safeCustomerName = safeCustomerName.Replace('(', '_').Replace(')', '_').Replace('%', '_').Trim();
            if (string.IsNullOrEmpty(safeCustomerName))
            {
                safeCustomerName = "Customer";
            }

            return today + "_" + nextSTT.ToString() + "_" + safeCustomerName;
        }
    }
}

