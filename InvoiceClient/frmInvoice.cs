using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using System.Net;
using log4net;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace InvoiceClient
{
    public partial class frmInvoice : Form
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(frmInvoice));
        string strAccessToken = "";
        string urlAPI = "";
        string codeTax = "";
        string proxy = "";
        int port = 0;
        int ssl = 0;
        //khai bao
        string username_vtel;
        string pass_vtel;
        string[] sohoadon;
        string sohoadon_vtel;
        string sohoadon_noibo;
        //string templatecode;
        SqlConnection cnn = new SqlConnection(@"Data Source=115.78.95.245;Initial Catalog=NPPBaoNgoc@;Integrated Security=false;user id=sa;password=qwerQWER1234!@#$");
        // Lưu DataTable gốc để filter
        DataTable dtCustomers = null;
        DataTable dtEmployees = null;
        // Flag để tránh vòng lặp khi set DataSource
        private bool isUpdatingCustomerName = false;
        private bool isUpdatingEmployeeName = false;
        private void Search()
        {

            cnn.Open();
            string sql;
            string cusname ="";
            if (chkcusname.Checked == true)
            {
                cusname = cbocustomername.Text;
            }

            string Employname = "";
            if (chkempoyeename.Checked == true)
            {
                Employname = cboemployeename.Text;
            }
            if (chk_searchby_PaymentDate.Checked == true) {
                // Search theo paymentDate trong PaymentHistory thay vì createdat
                // Format: [{"cashAmount":0,"transferAmount":1160000,"paymentDate":"2025-12-11T16:38:37.875"}]
                // Extract paymentDate từ JSON và so sánh với khoảng thời gian
                // CHỈ LẤY PHẦN NGÀY (10 ký tự: yyyy-MM-dd), BỎ PHẦN GIỜ (T16:38:37.875)
                // Ví dụ: "paymentDate":"2025-12-11T16:38:37.875" -> extract "2025-12-11" (10 ký tự)
                string startDateStr = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string endDateStr = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                string paymentDatePattern = "\"paymentDate\":\"";
                int patternLength = paymentDatePattern.Length; // 15 ký tự
                
                sql = "SELECT CustomerName As [Tên Quán],CustomerPhone as SĐT,CustomerAddress as [Địa Chỉ],ServiceNames as [Items],ServiceQuantities as [Số Lượng],UnitPrice as [Unit Price],TotalPrice as [Total Price],CreatedAt as [Created At],IsPaid,DeliveryStatus as [Delivery Status],IsDraft,InvoiceNoViettel as [Invoice No Viettel] ,PaymentHistory,* FROM Orders_viettel " +
                      "WHERE PaymentHistory IS NOT NULL " +
                      "AND PaymentHistory != '' " +
                      "AND PaymentHistory != '[]' " +
                      "AND CHARINDEX('" + paymentDatePattern + "', PaymentHistory) > 0 " +
                      // Extract 10 ký tự sau "paymentDate":" (chỉ phần ngày yyyy-MM-dd, bỏ phần giờ T16:38:37.875)
                      "AND SUBSTRING(PaymentHistory, CHARINDEX('" + paymentDatePattern + "', PaymentHistory) + " + patternLength + ", 10) >= '" + startDateStr + "' " +
                      "AND SUBSTRING(PaymentHistory, CHARINDEX('" + paymentDatePattern + "', PaymentHistory) + " + patternLength + ", 10) <= '" + endDateStr + "' " +
                      "AND CustomerPhone LIKE N'%" + txtcustomerphone.Text + "%' " +
                      "AND CustomerName LIKE N'%" + cusname + "%' " +
                      "AND EmployeeNames LIKE N'%" + Employname + "%'";
            }
            else
            {
                sql = "SELECT CustomerName As [Tên Quán],CustomerPhone as SĐT,CustomerAddress as [Địa Chỉ],ServiceNames as [Items],ServiceQuantities as [Số Lượng],UnitPrice as [Unit Price],TotalPrice as [Total Price],CreatedAt as [Created At],IsPaid,DeliveryStatus as [Delivery Status],IsDraft,InvoiceNoViettel as [Invoice No Viettel] ,* FROM Orders_viettel " +
                   "WHERE CAST(createdat AS date) BETWEEN '" +
                   dateTimePicker1.Value.ToString("yyyy-MM-dd") + "' AND '" +
                   dateTimePicker2.Value.ToString("yyyy-MM-dd") + "' " +
                   "AND CustomerPhone LIKE N'%" + txtcustomerphone.Text + "%' " +
                   "AND CustomerName LIKE N'%" + cusname + "%' " +
                   "AND EmployeeNames LIKE N'%" + Employname + "%'";

            }





            SqlCommand com = new SqlCommand(sql, cnn); //bat dau truy van
            com.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(com); //chuyen du lieu ve
            DataTable dt = new DataTable(); //tạo một kho ảo để lưu trữ dữ liệu
            da.Fill(dt);  // đổ dữ liệu vào kho
            dataGridView1.DataSource = dt;
            
            // Thêm cột checkbox nếu chưa có (phải thêm TRƯỚC khi ẩn các cột)
            if (!dataGridView1.Columns.Contains("Select"))
            {
                DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                checkBoxColumn.Name = "Select";
                checkBoxColumn.HeaderText = "Chọn";
                checkBoxColumn.Width = 50;
                checkBoxColumn.ReadOnly = false;
                dataGridView1.Columns.Insert(0, checkBoxColumn);
            }
            
            // Ẩn toàn bộ trước (trừ cột Select)
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name != "Select")
                {
                    col.Visible = false;
                }
            }

            // Chỉ hiển thị các cột cần thiết
            dataGridView1.Columns["CustomerName"].Visible = true;
            dataGridView1.Columns["CustomerPhone"].Visible = true;
            dataGridView1.Columns["CustomerAddress"].Visible = true;
            dataGridView1.Columns["ServiceNames"].Visible = true;
            dataGridView1.Columns["ServiceQuantities"].Visible = true;
            
            // Format UnitPrice với dấu phẩy phân cách hàng nghìn, giữ nguyên số thập phân
            // Vì UnitPrice là nvarchar (mảng) nên cần format trong CellFormatting event
            if (dataGridView1.Columns["UnitPrice"] != null)
            {
                dataGridView1.Columns["UnitPrice"].Visible = true;
            }
            
            // Format TotalPrice với dấu phẩy phân cách hàng nghìn, giữ nguyên số thập phân
            if (dataGridView1.Columns["TotalPrice"] != null)
            {
                dataGridView1.Columns["TotalPrice"].Visible = true;
                dataGridView1.Columns["TotalPrice"].DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "#,##0.################################################################",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                };
            }
            
            // Đăng ký event CellFormatting để format UnitPrice (mảng nvarchar)
            dataGridView1.CellFormatting -= dataGridView1_CellFormatting;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            
            dataGridView1.Columns["CreatedAt"].Visible = true;
            dataGridView1.Columns["IsPaid"].Visible = true;
            dataGridView1.Columns["DeliveryStatus"].Visible = true;
            dataGridView1.Columns["IsDraft"].Visible = true;
            dataGridView1.Columns["InvoiceNoViettel"].Visible = true;
            
            // Đảm bảo cột Select luôn hiển thị
            if (dataGridView1.Columns.Contains("Select"))
            {
                dataGridView1.Columns["Select"].Visible = true;
            }
            
            // Đặt màu vàng nhạt cho các dòng có IsConvertToLiter == false
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["id"] != null && row.Cells["id"].Value != null)
                {
                    string orderId = row.Cells["id"].Value.ToString().Trim();
                    if (!string.IsNullOrEmpty(orderId))
                    {
                        try
                        {
                            // Kiểm tra trạng thái connection trước khi mở
                            if (cnn.State != ConnectionState.Open)
                            {
                                cnn.Open();
                            }
                            string sqlCheck = $"SELECT IsConvertToLiter FROM Orders_Viettel WHERE id = '{orderId}'";
                            SqlCommand comCheck = new SqlCommand(sqlCheck, cnn);
                            object result = comCheck.ExecuteScalar();
                            
                            // Chỉ đóng connection nếu chúng ta đã mở nó
                            if (cnn.State == ConnectionState.Open)
                            {
                                cnn.Close();
                            }

                            bool isConvertToLiter = false;
                            if (result != null && result != DBNull.Value)
                            {
                                isConvertToLiter = Convert.ToBoolean(result);
                            }

                            // Nếu IsConvertToLiter == false hoặc NULL, đặt màu cam nhạt
                            if (!isConvertToLiter)
                            {
                                row.DefaultCellStyle.BackColor = System.Drawing.Color.PeachPuff;
                            }
                        }
                        catch
                        {
                            if (cnn.State == ConnectionState.Open)
                                cnn.Close();
                        }
                    }
                }
            }
            
            cnn.Close();  // đóng kết nối
        }

        public frmInvoice()
        {
            InitializeComponent();
            urlAPI = ConfigurationManager.AppSettings["APILink"].ToString();
            codeTax = ConfigurationManager.AppSettings["CodeTax"].ToString();
            proxy = ConfigurationManager.AppSettings["ProxyIP"].ToString();
            port = int.Parse(ConfigurationManager.AppSettings["ProxyPort"].ToString());
            username_vtel = ConfigurationManager.AppSettings["User"].ToString();
            pass_vtel = ConfigurationManager.AppSettings["Pass"].ToString();

            txttaxcode.Text = "3604019464";
            txtmauso.Text = "1/001";
            txtinvseries.Text = "C25TBN";
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Customers", cnn);
            dtCustomers = new DataTable();
            da.Fill(dtCustomers);

            // Lưu lại DataTable gốc để lọc sau
            DataView dv = new DataView(dtCustomers);
            cbocustomername.DropDownStyle = ComboBoxStyle.DropDown;
            cbocustomername.DisplayMember = "Name";
            cbocustomername.ValueMember = "Name";
            cbocustomername.DataSource = dv;
            cbocustomername.TextChanged += new EventHandler(cbocustomername_TextChanged);

            SqlDataAdapter da_employee = new SqlDataAdapter("select * from Employees ", cnn);//SQL là câu truy vấn bảng trong cơ sở dữ liệu, cn là connection đến cơ sở dữ liệu
            dtEmployees = new DataTable();
            da_employee.Fill(dtEmployees);


            // Lưu lại DataTable gốc để lọc sau
            DataView dv_employee = new DataView(dtEmployees);
            cboemployeename.DropDownStyle = ComboBoxStyle.DropDown;
            cboemployeename.DisplayMember = "Name";//Word là tên trường bạn muốn hiển thị trong combobox
            cboemployeename.ValueMember = "Name";
            cboemployeename.DataSource = dv_employee;
            cboemployeename.TextChanged += new EventHandler(cboemployeename_TextChanged);
        }
        private void getAccessToken()
        {
            try
            {
                strAccessToken = "";

                string user = username_vtel;
                string pass = pass_vtel; 
                string apiLink = urlAPI + @"/auth/login";
                string contentType = "application/json";
                string method = "POST";


                string request = @"{""username"":""" + user + @""",""password"":""" + pass + @"""}";
                if (!chkProxy.Checked)
                {
                    proxy = "";
                    port = 0;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtProxy.Text) && !string.IsNullOrEmpty(txtPort.Text) && txtProxy.Text != "Địa chỉ Proxy" && txtPort.Text != "Port")
                    {
                        proxy = txtProxy.Text;
                        port = int.Parse(txtPort.Text);
                    }
                }
                ssl = chkSSL.Checked ? 1 : 0;
 
                string result = CreateRequest.webRequestgetToken(apiLink, request, method, contentType, proxy, port, ssl);

                responseGetAccessToken auth = JsonConvert.DeserializeObject<responseGetAccessToken>(result);
                strAccessToken = @"access_token=" + auth.access_token;
       

            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tạo token: " + ex.Message);
              
            }
        }

        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                getAccessToken();
                lậpHóaĐơnToolStripMenuItem_Click(sender, e);
                Search();
                MessageBox.Show("Finished !", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }

        }

        private void btnCreateInvoiceDraft_Click(object sender, EventArgs e)
        {
            try
            {
                getAccessToken();
                lậpNhápHóaĐơnToolStripMenuItem_Click(sender, e);
                Search();
                MessageBox.Show("Finished !", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(strAccessToken))
                {
                    MessageBox.Show("Chưa lấy giá tị Token");
                    return;
                }
                string apiLink = urlAPI + @"/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/cancelTransactionInvoice";
                string contentType = "application/x-www-form-urlencoded";

                int chk = chkExample.Checked ? 1 : 0;
                string request = "";

                if (chk == 1)
                {
                    request = cancelInvoiceExample();
                }
                else
                {
                    request = txtBody.Text;
                    if (string.IsNullOrEmpty(request))
                    {
                        MessageBox.Show("Chưa nhập giá trị text của body");
                        return;
                    }
                }

                string result = CreateRequest.webRequestV2(apiLink, request, strAccessToken, "POST", contentType, proxy, port, ssl);

                MessageBox.Show("OK " + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }
        }
        private void btnSavePDF_Click(object sender, EventArgs e)
        {
            try
            {
                // getAccessToken();
                if (string.IsNullOrEmpty(strAccessToken))
                {
                    MessageBox.Show("Chưa lấy giá tị Token");
                    return;
                }
                string apiLink = urlAPI + @"/services/einvoiceapplication/api/InvoiceAPI/InvoiceUtilsWS/getInvoiceRepresentationFile";
                string contentType = "application/json";

                int chk = chkExample.Checked ? 1 : 0;
                string request = "";
                int index = dataGridView1.CurrentCell.RowIndex; //lấy ra chỉ số của row đang đc chọn

                string sohoadon = dataGridView1.Rows[index].Cells["InvoiceNoViettel"].Value.ToString().Trim();
                string templatecode_ = dataGridView1.Rows[index].Cells["mauso"].Value.ToString().Trim();

                if (chk == 1)
                {
                    request = getPDFExample(codeTax, sohoadon.Replace("\"", ""), templatecode_);
                }
                else
                {
                    request = txtBody.Text;
                    if (string.IsNullOrEmpty(request))
                    {
                        MessageBox.Show("Chưa nhập giá trị text của body");
                        return;
                    }
                }

                string result = CreateRequest.webRequestV2(apiLink, request, strAccessToken, "POST", contentType, proxy, port, ssl);

                PDFFileResponse objFile = JsonConvert.DeserializeObject<PDFFileResponse>(result);
                string fileName = objFile.fileName;
                string path = "c:\\temp\\" + fileName;
                File.WriteAllBytes(path, objFile.fileToBytes);

                MessageBox.Show("File in " + path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }
        }
        private void btnGetInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(strAccessToken))
                {
                    MessageBox.Show("Chưa lấy giá tị Token");
                    return;
                }
                string apiLink = urlAPI + @"/services/einvoiceapplication/api/InvoiceAPI/InvoiceUtilsWS/getListInvoiceDataControl";
                string contentType = "application/json";

                int chk = chkExample.Checked ? 1 : 0;
                string request = "";

                if (chk == 1)
                {
                    request = getInvoiceExample();
                }
                else
                {
                    request = txtBody.Text;
                    if (string.IsNullOrEmpty(request))
                    {
                        MessageBox.Show("Chưa nhập giá trị text của body");
                        return;
                    }
                }

                string result = CreateRequest.webRequestV2(apiLink, request, strAccessToken, "POST", contentType, proxy, port, ssl);

                MessageBox.Show("OK  " + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }
        }

        private void btnUpdatePaymentStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(strAccessToken))
                {
                    MessageBox.Show("Chưa lấy giá tị Token");
                    return;
                }
                string apiLink = urlAPI + @"/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/updatePaymentStatus";
                string contentType = "application/x-www-form-urlencoded";

                int chk = chkExample.Checked ? 1 : 0;
                string request = "";

                if (chk == 1)
                {
                    request = updatePaymetStatusExample();
                }
                else
                {
                    request = txtBody.Text;
                    if (string.IsNullOrEmpty(request))
                    {
                        MessageBox.Show("Chưa nhập giá trị text của body");
                        return;
                    }
                }
                string result = CreateRequest.webRequestV2(apiLink, request, strAccessToken, "POST", contentType, proxy, port, ssl);
                MessageBox.Show("OK " + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }
        }

     
        public string ddmmmyyyy(DateTime ngay)
        {
            int dd = ngay.Day;
            int MMM = ngay.Month;
            int yyyy = ngay.Year;

            string M = "";
            switch (MMM)
            {
                case 1:
                    // code block
                    M = "JAN";
                    break;
                case 2:
                    // code block
                    M = "FEB";
                    break;

                case 3:
                    // code block
                    M = "MAR";
                    break;
                case 4:
                    // code block
                    M = "APR";
                    break;
                case 5:
                    // code block
                    M = "MAY";
                    break;
                case 6:
                    // code block
                    M = "JUN";
                    break;
                case 7:
                    // code block
                    M = "JUL";
                    break;

                case 8:
                    // code block
                    M = "AUG";
                    break;
                case 9:
                    // code block
                    M = "SEP";
                    break;

                case 10:
                    // code block
                    M = "OCT";
                    break;


                case 11:
                    // code block
                    M = "NOV";
                    break;

                case 12:
                    // code block
                    M = "DEC";
                    break;
                default:
                    // code block
                    break;
            }
            return dd.ToString() + "-" + M + "-" + yyyy.ToString();


        }

        private string createInvoiceExample(List<string> idsForSql)
        {
     
            string invoicetype = "1";
            string templatecode = "";
            string PTTT = "";
            string invoiceseries = "";
            
            string userVT_ = "";
            string passVT_ = "";
            string SerialNo = "";
 
            string Cusid = "";
            string MainCode = "";
            string TaxCode = "";
            string Address = "";
            string Company = "";
            string HotenBuyer = "";
            string Customer_Code = "";
            string cccd = "";
            string Tel = "";
            
            string buyerName_ = "";
            userVT_ = username_vtel;
            passVT_ = pass_vtel;
           
            cnn.Open();
            string idList = string.Join("','", idsForSql);
            
            string sql1 = $"SELECT * FROM Orders_viettel WHERE id IN ('{idList}')";
            
            SqlCommand com1 = new SqlCommand(sql1, cnn); //bat dau truy van
            com1.CommandType = CommandType.Text;
            SqlDataAdapter da1 = new SqlDataAdapter(com1); //chuyen du lieu ve
            DataTable dt1 = new DataTable(); //tạo một kho ảo để lưu trữ dữ liệu
            da1.Fill(dt1);
            var ds = new DataSet();
            ds.Tables.Add(dt1);
            cnn.Close();
            double tax = 0;
            
            if (txtmauso.Text.Length == 0)
            {
                MessageBox.Show("Nhập Mẫu Số !", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }

            if (txtinvseries.Text.Length == 0)
            {
                MessageBox.Show("Nhập INV Series !", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }

            //if (cboPTTT.Text.Length == 0)
            //{
            //    MessageBox.Show("Chọn PTTT !", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return "";
            //}

            templatecode = txtmauso.Text;
            invoiceseries =txtinvseries.Text;
            //PTTT = cboPTTT.Text;
            PTTT = "TM/CK";

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Lấy thông tin từ order đầu tiên
                    Cusid = ds.Tables[0].Rows[0]["CustomerPhone"].ToString();
                    tax = Convert.ToDouble(ds.Tables[0].Rows[0]["TaxPercent"]);
                    
                    // API Viettel cho phép các items có % thuế khác nhau trong cùng 1 hóa đơn
                    // Không cần kiểm tra bắt buộc cùng tax percent
                    
                    if (tax == 0)
                    {
                        var result = MessageBox.Show(
                            "VAT của đơn hiện là 0% , bạn có muốn sửa lại không ?",
                            "Xác nhận VAT",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2
                        );

                        if (result == DialogResult.No)
                        {
                            tax = 0;
                        }
                        else if (result == DialogResult.Yes)
                        {
                            if (double.TryParse(txtvat.Text.Replace(',', '.'), out double newVat))
                            {
                                tax = newVat;
                            }
                            else
                            {
                                MessageBox.Show("Giá trị VAT nhập không hợp lệ. Hệ thống sẽ dùng 0%.",
                                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return "";
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return "";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Kiem tra Hoa don.!");
                    return "";
                }
            }

            cnn.Open();
            string sqlcus = "select * from Customers  where phone='" + Cusid + "'  ";  // lay het du lieu trong bang sinh vien
            SqlCommand comcus = new SqlCommand(sqlcus, cnn); //bat dau truy van
            comcus.CommandType = CommandType.Text;
            SqlDataAdapter dacus = new SqlDataAdapter(comcus); //chuyen du lieu ve
            DataTable dtcus = new DataTable(); //tạo một kho ảo để lưu trữ dữ liệu
            dacus.Fill(dtcus);
            var dscus = new DataSet();
            dscus.Tables.Add(dtcus);
            cnn.Close();  // đóng kết nối
            // đổ dữ liệu vào kho
            if (dscus != null)
            {
                if (dscus.Tables[0].Rows.Count > 0)
                {

                    MainCode = dscus.Tables[0].Rows[0]["Code"].ToString();
                    TaxCode = dscus.Tables[0].Rows[0]["TaxCode"].ToString();

                    if (string.IsNullOrEmpty(TaxCode))
                    {
                        MessageBox.Show("Customer chưa có Tax Code !", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       
                    }
                   
                    if (!string.IsNullOrEmpty(dscus.Tables[0].Rows[0]["InvoiceName"].ToString()))
                    {
                        Company = dscus.Tables[0].Rows[0]["InvoiceName"].ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dscus.Tables[0].Rows[0]["Name"].ToString()))
                        {
                            Company = dscus.Tables[0].Rows[0]["Name"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Nhập Name cho customer !", "Vui thử lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "";
                        }
                            
                    }

                    if (!string.IsNullOrEmpty(dscus.Tables[0].Rows[0]["InvoiceAddress"].ToString()))
                    {
                        Address = dscus.Tables[0].Rows[0]["InvoiceAddress"].ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dscus.Tables[0].Rows[0]["Address"].ToString()))
                        {
                            Address = dscus.Tables[0].Rows[0]["Address"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Nhập Address cho customer !", "Vui thử lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "";
                        }
                        
                    }

                    if (chkcanhan.Checked == true) 
                    {
                        if (!string.IsNullOrEmpty(dscus.Tables[0].Rows[0]["Identification"].ToString()))
                        {
                            cccd = dscus.Tables[0].Rows[0]["Identification"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Nhập CCCD cho customer Để xuất HĐ Cá Nhân!", "Vui thử lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "";
                        }
                    }

                    
                    HotenBuyer = dscus.Tables[0].Rows[0]["CustomerName"]?.ToString() ?? "";
                    //Type = dscus.Tables[0].Rows[0]["Type"].ToString();
                    Tel = dscus.Tables[0].Rows[0]["phone"].ToString();
                    //Email = dscus.Tables[0].Rows[0]["Email"].ToString();
                    buyerName_ = dscus.Tables[0].Rows[0]["Name"].ToString();
                }
            }

            InvoiceInfo objInvoice = new InvoiceInfo();
            objInvoice.uuId = System.Guid.NewGuid().ToString();//ds.Tables[0].Rows[0]["TaxInvoiceID"].ToString()
            objInvoice.invoiceType = invoicetype.Trim();
            objInvoice.templateCode = templatecode.Trim();
            objInvoice.invoiceSeries = invoiceseries.Trim();
            objInvoice.invoiceIssuedDate = ((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds).ToString();//JsonConvert.SerializeObject(DateTime.Now, settings).ToString();
            objInvoice.currencyCode = "VND";
            objInvoice.adjustmentType = "1";
            objInvoice.paymentStatus = "true";
            objInvoice.paymentType = PTTT;
            objInvoice.paymentTypeName = PTTT;
            objInvoice.cusGetInvoiceRight = "true";
            if (chkcanhan.Checked == true)
            {
                objInvoice.buyerIdNo = cccd.ToString();
                objInvoice.buyerIdType = "1";
            }
            else
            {
                objInvoice.buyerIdNo = "";
                objInvoice.buyerIdType = "";
            }
                

            //Buyer Info
            BuyerInfo objBuyer = new BuyerInfo();
            // kiem tra neu personal thi taxcode=''
            //if (MainCode.ToUpper() == "PERSONAL")
            //{
            //    objBuyer.buyerTaxCode = "";
            //}
            //else
            //{
            //    objBuyer.buyerTaxCode = TaxCode;
            //}

            objBuyer.buyerTaxCode = TaxCode;
            objBuyer.buyerName = HotenBuyer;
            objBuyer.buyerLegalName = Company;
            objBuyer.buyerAddressLine = Address.Replace("\n", "").Replace("\r", "");
            if (chkcanhan.Checked == true)
            {
                objBuyer.buyerIdNo = objInvoice.buyerIdNo;
                objBuyer.buyerIdType = "1";
            }
            else
            {
                objBuyer.buyerIdNo = objInvoice.buyerIdNo;
                objBuyer.buyerIdType = "";
            }
          

            objBuyer.buyerPhoneNumber = Tel;


            // -----------
            cnn.Open();
            string sqlseller = "select * from Information";  // lay het du lieu trong bang sinh vien
            SqlCommand comseller = new SqlCommand(sqlseller, cnn); //bat dau truy van
            comcus.CommandType = CommandType.Text;
            SqlDataAdapter daseller = new SqlDataAdapter(comseller); //chuyen du lieu ve
            DataTable dtseller = new DataTable(); //tạo một kho ảo để lưu trữ dữ liệu
            daseller.Fill(dtseller);
            var dsseller = new DataSet();
            dsseller.Tables.Add(dtseller);
            cnn.Close();  // đóng kết nối

            if (dsseller != null)
            {
                if (dsseller.Tables[0].Rows.Count > 0)
                {
                    SellerInfo objSeller = new SellerInfo();
                    objSeller.sellerAddressLine = dsseller.Tables[0].Rows[0]["SalonName"].ToString();
                    objSeller.sellerBankAccount = dsseller.Tables[0].Rows[0]["BankNo"].ToString();
                    objSeller.sellerBankName = dsseller.Tables[0].Rows[0]["BankName"].ToString();
                    objSeller.sellerEmail = dsseller.Tables[0].Rows[0]["Email"].ToString();
                    objSeller.sellerLegalName = "Bên bán";
                    objSeller.sellerPhoneNumber = dsseller.Tables[0].Rows[0]["Phone"].ToString();
                    objSeller.sellerTaxCode = dsseller.Tables[0].Rows[0]["TaxCode"].ToString();
                }
            }

           
            //Item info
            double sumTotalTaxAmount = 0, sumTotalAmountWithoutTax = 0, totalamountwithtax = 0, totaltax = 0,totaldiscount =0;
            List<ItemInfo> lstItem = new List<ItemInfo>();
            int stt = 1; // Đếm lineNumber liên tục qua tất cả Orders_viettel
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    // ===== TÁCH CHUỖI SERVICEID & QUANTITY =====
                    string servicesRaw = ds.Tables[0].Rows[i]["ServiceIds"]?.ToString()?.Trim() ?? "";
                    string quantitiesRaw = ds.Tables[0].Rows[i]["ServiceQuantities"]?.ToString()?.Trim() ?? "";
                    string UnitPriceRaw = ds.Tables[0].Rows[i]["UnitPrice"]?.ToString()?.Trim() ?? "";
                    // Loại bỏ ký tự [ ] và "
                    servicesRaw = servicesRaw.Replace("[", "").Replace("]", "").Replace("\"", "");
                    quantitiesRaw = quantitiesRaw.Replace("[", "").Replace("]", "");
                    UnitPriceRaw = UnitPriceRaw.Replace("[", "").Replace("]", "");
                    // Tách mảng
                    var serviceIds = servicesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(x => x.Trim())
                                                .ToList();

                    var quantities = quantitiesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(x => int.TryParse(x.Trim(), out var val) ? val : 0)
                                                  .ToList();

                    var UnitPrices = UnitPriceRaw
                     .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => double.TryParse(x.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val)
                         ? val
                         : 0.0)
                     .ToList();

                    // Nếu không khớp số lượng thì bỏ qua
                    if (serviceIds.Count != quantities.Count || quantities.Count != UnitPrices.Count)
                        continue;

                    // ===== VÒNG LẶP TẠO ITEM CHO TỪNG SERVICE =====
                    for (int j = 0; j < serviceIds.Count; j++)
                    {
     
                        ItemInfo item = new ItemInfo();

                        item.lineNumber = stt.ToString(); // Sử dụng stt để đếm liên tục
                        //item.discount = ds.Tables[0].Rows[i]["DiscountPercent"].ToString();
                        item.discount = Convert.ToInt32(ds.Tables[0].Rows[i]["DiscountPercent"]).ToString();

                        cnn.Open();
                        string sqlservices = "select * from Services where id = '" + serviceIds[j] +"' ";  // lay het du lieu trong bang sinh vien
                        SqlCommand comservices = new SqlCommand(sqlservices, cnn); //bat dau truy van
                        comcus.CommandType = CommandType.Text;
                        SqlDataAdapter daservices = new SqlDataAdapter(comservices); //chuyen du lieu ve
                        DataTable dtservices = new DataTable(); //tạo một kho ảo để lưu trữ dữ liệu
                        daservices.Fill(dtservices);
                        var dsservices = new DataSet();
                        dsservices.Tables.Add(dtservices);
                        cnn.Close();  // đóng kết nối

                        if (dsservices != null)
                        {
                            if (dsservices.Tables[0].Rows.Count > 0)
                            {
                                item.itemCode = dsservices.Tables[0].Rows[0]["Code"].ToString();
                                item.itemName = dsservices.Tables[0].Rows[0]["Name"].ToString();
                                item.unitName = "Lít"; // Mặc định là "L" (Lít)
                                //// Kiểm tra checkbox chkchuyenL
                                //if (chkchuyenL.Checked)
                                //{
                                //    item.unitName = "Lít"; // Mặc định là "L" (Lít)
                                //}
                                //else
                                //{
                                //    item.unitName = dsservices.Tables[0].Rows[0]["Unit"].ToString();
                                //}
                            }
                        }

                        // Lấy tax riêng của từng order (cho phép % thuế khác nhau)
                        double itemTax = Convert.ToDouble(ds.Tables[0].Rows[i]["TaxPercent"]);
                        double unitPrice = Convert.ToDouble(UnitPrices[j]);
                        int quantity = Convert.ToInt32(quantities[j]);
                        
                        // Nếu checkbox chkchuyenL được tick, tính lại quantity và unitPrice
                        //if (chkchuyenL.Checked && dsservices != null && dsservices.Tables[0].Rows.Count > 0)
                        //{
                        //    // Lấy Production từ Services
                        //    double production = 0;
                        //    if (dsservices.Tables[0].Rows[0]["Production"] != null && dsservices.Tables[0].Rows[0]["Production"] != DBNull.Value)
                        //    {
                        //        double.TryParse(dsservices.Tables[0].Rows[0]["Production"].ToString(), out production);
                        //    }
                            
                        //    if (production > 0)
                        //    {
                        //        // quantity mới = Production * quantities[j]
                        //        int quantityNew = (int)(production * quantity);
                                
                        //        // unitPrice mới = unitPrice hiện tại / Production
                        //        double unitPriceNew = unitPrice / production;
                                
                        //        quantity = quantityNew;
                        //        unitPrice = unitPriceNew;
                        //    }
                        //}

                        // Tính tiền trước thuế
                        //double amountWithoutTax = unitPrice * quantity ; 
                        double amountWithoutTax = Math.Round(unitPrice, 0, MidpointRounding.AwayFromZero) * quantity;
                        // Tính tiền thuế - sử dụng tax riêng của từng order
                        double taxAmount =( amountWithoutTax  - (amountWithoutTax * (double.Parse(item.discount) / 100))) * (itemTax / 100);

                        double discount = amountWithoutTax * (double.Parse(item.discount) / 100); 

                        // Tính tổng tiền sau thuế
                        double amountWithTax = amountWithoutTax + taxAmount;

                        item.itemTotalAmountWithoutTax = Math.Round(amountWithoutTax, 0, MidpointRounding.AwayFromZero).ToString();

                        item.taxAmount = Math.Round(taxAmount, 0, MidpointRounding.AwayFromZero).ToString();

                        //item.taxPercentage = ds.Tables[0].Rows[i]["TaxPercent"].ToString(); 
                        item.taxPercentage = Convert.ToInt32(ds.Tables[0].Rows[i]["TaxPercent"]).ToString();

                        // Sử dụng unitPrice và quantity đã được tính toán (có thể đã thay đổi nếu chkchuyenL.Checked)
                        item.unitPrice = Math.Round(unitPrice, 0, MidpointRounding.AwayFromZero).ToString();

                        item.quantity = quantity.ToString();
                        item.itemDiscount = "0"; 

                        sumTotalAmountWithoutTax += double.Parse(item.itemTotalAmountWithoutTax) ;
                        totalamountwithtax += amountWithTax ;
                        totaltax += double.Parse(item.taxAmount);
                        totaldiscount += discount;
                        lstItem.Add(item);
                        stt++; // Tăng stt cho item tiếp theo
                    }
                }
            }

            SummarizeInfo objSummary = new SummarizeInfo();

            objSummary.discountAmount = totaldiscount.ToString();
            objSummary.sumOfTotalLineAmountWithoutTax = sumTotalAmountWithoutTax.ToString();
            objSummary.totalAmountWithoutTax = (sumTotalAmountWithoutTax- totaldiscount).ToString();
            objSummary.totalAmountWithTax = (totalamountwithtax - totaldiscount).ToString();
            // Tax percentage trong summary - lấy từ order đầu tiên (chỉ để hiển thị, không ảnh hưởng tính toán)
            //objSummary.taxPercentage = ds.Tables[0].Rows[0]["TaxPercent"].ToString();
            objSummary.taxPercentage = Convert.ToInt32(ds.Tables[0].Rows[0]["TaxPercent"]).ToString();
            objSummary.totalAmountWithTaxInWords = NumberUtil.DocSoThanhChu(objSummary.totalAmountWithTax.Replace("-", ""));
            objSummary.totalTaxAmount = totaltax.ToString();
            
            // Tạo danh sách tax breakdowns từ các tax percentage khác nhau trong items
            Dictionary<double, double> taxBreakdowns = new Dictionary<double, double>(); // Key: taxPercentage, Value: taxableAmount
            Dictionary<double, double> taxAmounts = new Dictionary<double, double>(); // Key: taxPercentage, Value: taxAmount
            
            foreach (var itemInfo in lstItem)
            {
                double itemTaxPct = double.Parse(itemInfo.taxPercentage);
                double itemTaxableAmount = double.Parse(itemInfo.itemTotalAmountWithoutTax) - (double.Parse(itemInfo.itemTotalAmountWithoutTax) * (double.Parse(itemInfo.discount) / 100));
                double itemTaxAmt = double.Parse(itemInfo.taxAmount);
                
                if (taxBreakdowns.ContainsKey(itemTaxPct))
                {
                    taxBreakdowns[itemTaxPct] += itemTaxableAmount;
                    taxAmounts[itemTaxPct] += itemTaxAmt;
                }
                else
                {
                    taxBreakdowns[itemTaxPct] = itemTaxableAmount;
                    taxAmounts[itemTaxPct] = itemTaxAmt;
                }
            }


            //-------------------------------
            string str = @"{    ""generalInvoiceInfo"": " +
                   @"{       ""invoiceType"":""" + objInvoice.invoiceType +
                   @""",       ""templateCode"":""" + objInvoice.templateCode +
                   @""", 	""invoiceSeries"":""" + objInvoice.invoiceSeries +
                   @""",       ""transactionUuid"": """ + objInvoice.uuId +
                   @""",       ""invoiceIssuedDate"":" + objInvoice.invoiceIssuedDate +
                   @",       ""currencyCode"":""" + objInvoice.currencyCode + @""", 
                    ""adjustmentType"":""" + objInvoice.adjustmentType + @""",  
                    ""paymentStatus"":" + objInvoice.paymentStatus + @",
                    ""paymentTypeName"": """ + objInvoice.paymentType + @""",
                    ""cusGetInvoiceRight"":" + objInvoice.cusGetInvoiceRight + @",      
                    ""buyerIdNo"":""" + objInvoice.buyerIdNo + @""",        

                    ""buyerIdType"":""" + objInvoice.buyerIdType + @"""    },    
                    ""buyerInfo"":{              
                    ""buyerLegalName"":""" + objBuyer.buyerLegalName + @""",  
                    ""buyerName"":""" + objBuyer.buyerName + @""",
                    ""buyerTaxCode"":""" + objBuyer.buyerTaxCode + @""",      
                    ""buyerAddressLine"":""" + objBuyer.buyerAddressLine + @""",       
                    ""buyerPhoneNumber"":""" + objBuyer.buyerPhoneNumber + @""",      
                    ""buyerEmail"":""" + objBuyer.buyerEmail + @""",      
                    ""buyerIdNo"":""" + objBuyer.buyerIdNo + @""",        
                    ""buyerIdType"":""" + objBuyer.buyerIdType + @"""    },
                    ""payments"":[  {""paymentMethodName"":""" + objInvoice.paymentType + @"""    }    ],    
                    ""deliveryInfo"":{     },
                    ""itemInfo"":[       ";
            int indexItem = 1;

            foreach (var itemInfo in lstItem)
            {
                str += @"{""lineNumber"":" + indexItem + @",          
                    ""itemCode"":""" + itemInfo.itemCode + @""",    
                    ""itemName"":""" + itemInfo.itemName.Replace("\r", "  ").Trim() + @""",
                    ""itemNote"":""" + "xxx" + @""",
                    ""unitName"":""" + itemInfo.unitName + @""",     
                    ""unitPrice"":" + itemInfo.unitPrice + @",             
                    ""quantity"":" + itemInfo.quantity + @",               
                    ""itemTotalAmountWithoutTax"":" + itemInfo.itemTotalAmountWithoutTax + @",               
                    ""taxPercentage"":" + itemInfo.taxPercentage + @",              
                    ""taxAmount"":" + itemInfo.taxAmount + @",              
                    ""discount"":" + itemInfo.discount + @",  
                    
                    ""itemDiscount"":" + itemInfo.itemDiscount;

                // ✅ Chỉ thêm specialInfo nếu có giá trị
                if (!string.IsNullOrEmpty(itemInfo.specialInfo?.value))
                {
                    str += @",
                        ""selection"":" + 6 + @",
                        ""itemType"":" + 2 + @",
                        ""specialInfo"": [
                            {
                                ""name"": """ + itemInfo.specialInfo.name + @""",
                                ""value"": """ + itemInfo.specialInfo.value + @"""
                            }
                        ]";
                }

                str += "}";

                // ✅ Xử lý dấu phẩy cho các phần tử
                if (indexItem != lstItem.Count)
                    str += " , ";
                //if (lstVouchers.Count != 0 && indexItem == lstItem.Count)
                //    str += " , ";

                indexItem++;
            }


            str += @"],    
                    ""discountItemInfo"":[     ],   
                    ""summarizeInfo"":{       
                    ""sumOfTotalLineAmountWithoutTax"":" + objSummary.sumOfTotalLineAmountWithoutTax + @",          
                    ""totalAmountWithoutTax"":" + objSummary.totalAmountWithoutTax + @",   
                    ""totalTaxAmount"":" + objSummary.totalTaxAmount + @",  
                    ""totalAmountWithTax"":" + objSummary.totalAmountWithTax + @",  
                    ""totalAmountWithTaxInWords"":""" + objSummary.totalAmountWithTaxInWords + @""",       
                    ""discountAmount"":" + objSummary.discountAmount + @",  
                    ""taxPercentage"":" + objSummary.taxPercentage + @"  },    
                    ""taxBreakdowns"":[";
            
            // Tạo taxBreakdowns động từ các tax percentage khác nhau
            int taxBreakdownIndex = 0;
            foreach (var kvp in taxBreakdowns)
            {
                if (taxBreakdownIndex > 0)
                    str += ",";
                str += @"       {         
                   ""taxPercentage"":" + kvp.Key.ToString() + @",                    
                    ""taxableAmount"": " + Math.Round(kvp.Value, 0, MidpointRounding.AwayFromZero).ToString() + @",
                    ""taxAmount"": " + Math.Round(taxAmounts[kvp.Key], 0, MidpointRounding.AwayFromZero).ToString() + @"       }";
                taxBreakdownIndex++;
            }
            
            str += @"    ] } ";


            return str;
        }
        private string createInvoiceDraftExample()
        {
            string str = @"{
   ""generalInvoiceInfo"": {
      ""invoiceType"": ""01GTKT"",
      ""templateCode"": ""01GTKT0/002"",
      ""invoiceSeries"": ""AA/20E"",
      ""currencyCode"": ""VND"",
      ""invoiceIssuedDate"": """ + ((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds).ToString() + @""",
      ""adjustmentType"": ""1"",
      ""paymentStatus"": true,
      ""paymentType"": ""CK"",
      ""paymentTypeName"": ""CK"",
      ""cusGetInvoiceRight"": true,
      ""userName"": ""test2"",
      ""transactionUuid"":""" + System.Guid.NewGuid().ToString() + @"""
   },
   ""buyerInfo"": {
      ""buyerName"": ""Lê Thị Thanh Minh_có mã KH_import CoopSmile"",
      ""buyerLegalName"": ""Tên đơn vị"",
      ""buyerTaxCode"": ""8008675383"",
      ""buyerAddressLine"": ""An Khánh Hoài Đức Hà Nội"",
      ""buyerPostalCode"": ""2342324323"",
      ""buyerDistrictName"": ""Số 9, đường 11, VSIP Bắc Ninh, Thị xã Từ Sơn, Tỉnh"",
      ""buyerCityName"": ""Thành Phố Hà Nội"",
      ""buyerCountryCode"": ""84"",
      ""buyerPhoneNumber"": ""987999999"",
      ""buyerFaxNumber"": ""0458954"",
      ""buyerEmail"": ""tamdtt1@viettel.com.vn"",
      ""buyerBankName"": ""Ngân hàng Quân đội MB"",
      ""buyerBankAccount"": ""01578987871236547"",
      ""buyerIdNo"": ""8888899999"",
      ""buyerIdType"": ""3"",
      ""buyerCode"": ""832472343b_b"",
      ""buyerBirthDay"": """"
   },
   ""sellerInfo"": {
      ""sellerCode"": ""MaCNbenban"",
      ""sellerLegalName"": ""TẬP ĐOÀN VIỄN THÔNG QUÂN ĐỘI"",
      ""sellerTaxCode"": ""0100109106-712"",
      ""sellerAddressLine"": ""Số 21 Cát Linh, phường Cát Linh - Quận Ba Đình - Thành Phố Hà Nội - Việt Nam"",
      ""sellerPhoneNumber"": ""57896512"",
      ""sellerEmail"": ""hieuvtm@viettel.com.vn"",
      ""sellerBankName"": ""Ngân hàng Quân Đội MB - CN huyện Yên Sơn; Ngân hàng Vietinbank"",
      ""sellerBankAccount"": ""012564654654; 010057839922""
   },
   ""extAttribute"": [],
   ""payments"": [
      {
         ""paymentMethodName"": ""CK""
      }
   ],
   ""deliveryInfo"": {},
   ""itemInfo"": [
      {
         ""lineNumber"": 1,
         ""itemCode"": ""Thue0"",
         ""itemName"": ""Có tính thuế"",
         ""unitName"": ""KG"",
         ""unitPrice"": 100000,
         ""quantity"": 1,
         ""itemTotalAmountWithoutTax"": 100000,
         ""itemTotalAmountWithTax"": 100000,
         ""itemTotalAmountAfterDiscount"": 100000,
         ""taxPercentage"": 0,
         ""taxAmount"": 0,
         ""discount"": 0,
         ""itemDiscount"": 0,
         ""itemNote"": """",
         ""batchNo"": """",
         ""expDate"": """"
      }
   ],
   ""discountItemInfo"": [],
   ""summarizeInfo"": {
      ""sumOfTotalLineAmountWithoutTax"": 100000,
      ""totalAmountWithoutTax"": 100000,
      ""totalTaxAmount"": 10000,
      ""totalAmountWithTax"": 110000,
      ""totalAmountAfterDiscount"": 110000,
      ""totalAmountWithTaxInWords"": ""Mười hai triệu ba trăm hai mươi bảy nghìn sáu trăm tám mươi mốt phẩy  chín đồng"",
      ""discountAmount"": 0
   },
   ""taxBreakdowns"": [
      {
         ""taxPercentage"": 10,
         ""taxableAmount"": 100000,
         ""taxAmount"": 10000
      }
   ],
   ""metadata"": [],
   ""customFields"": [],
   ""meterReading"": [],
   ""invoiceFile"": {
            ""fileContent"": ""RmlsZSBi4bqjbmcga8OqIMSRxrDhu6NjIGFkZCBsw6puIHThuqFpIHBo4bqnbiBs4bqtcCBow7NhIMSRxqFu"",
            ""fileType"": ""1""
          }
}";

            return str;
        }
        private string cancelInvoiceExample()
        {
            string str = @"supplierTaxCode=0100109106-712&invoiceNo=AA%2F20E0000001&strIssueDate=1600102800000&additionalReferenceDesc=hello&additionalReferenceDate=1600230649604";

            return str;
        }
        private string getPDFExample(string mst, string sohoadon, string templatecode)
        {
            string str = @"{
                ""supplierTaxCode"":""" + mst + @""",
                ""invoiceNo"":""" + sohoadon + @""",
                ""templateCode"": """ + templatecode + @""",
                ""fileType"":""PDF""
            }";

            return str;
        }
        private string getInvoiceExample()
        {
            string str = @"{
	            ""supplierTaxCode"":""0100109106-712"",
	            ""fromDate"":""01/08/2020"",
	            ""toDate"":""18/09/2020""
            }";

            return str;
        }
        private string updatePaymetStatusExample()
        {
            string str = @"supplierTaxCode=0100109106-712&invoiceNo=AA%2F20E0000002&strIssueDate=1600154781000&templateCode=01GTKT0%2F002&buyerEmailAddress=tamdtt1%40viettel.com.vn&paymentType=TM&paymentTypeName=TM&cusGetInvoiceRight=true";

            return str;
        }
        private void txtProxy_Enter(object sender, EventArgs e)
        {
            if (txtProxy.Text == "Địa chỉ Proxy")
            {
                txtProxy.Text = "";
                txtProxy.ForeColor = System.Drawing.Color.Black;
            }
        }
        private void txtProxy_Leave(object sender, EventArgs e)
        {

            if (txtProxy.Text == "")
            {
                txtProxy.Text = "Địa chỉ Proxy";
                txtProxy.ForeColor = System.Drawing.Color.LightGray;
            }
        }
        private void txtPort_Enter(object sender, EventArgs e)
        {
            if (txtPort.Text == "Port")
            {
                txtPort.Text = "";
                txtPort.ForeColor = System.Drawing.Color.Black;
            }
        }
        private void txtPort_Leave(object sender, EventArgs e)
        {

            if (txtPort.Text == "")
            {
                txtPort.Text = "Port";
                txtPort.ForeColor = System.Drawing.Color.LightGray;
            }
        }
        private void chkProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (chkProxy.Checked)
            {
                txtProxy.Visible = true;
                txtPort.Visible = true;

                txtProxy.Text = "Địa chỉ Proxy";
                txtProxy.ForeColor = System.Drawing.Color.LightGray;

                txtPort.Text = "Port";
                txtPort.ForeColor = System.Drawing.Color.LightGray;
            }
            else
            {
                txtProxy.Visible = false;
                txtPort.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdsearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void lậpHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txttaxcode.Text))
                {
                    MessageBox.Show("Input Tax Code.!");
                    return;
                }
                codeTax = txttaxcode.Text;

                if (dataGridView1.Rows.Count <= 0)
                {
                    MessageBox.Show("No Data.!");
                    return;
                }

                // Lấy tất cả các dòng đã được tick checkbox
                List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Select"] != null)
                    {
                        object cellValue = row.Cells["Select"].Value;
                        if (cellValue != null && cellValue != DBNull.Value)
                        {
                            bool isChecked = false;
                            if (cellValue is bool)
                            {
                                isChecked = (bool)cellValue;
                            }
                            else
                            {
                                bool.TryParse(cellValue.ToString(), out isChecked);
                            }
                            
                            if (isChecked)
                            {
                                selectedRows.Add(row);
                            }
                        }
                    }
                }

                if (selectedRows.Count <= 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một dòng dữ liệu!");
                    return;
                }

                // Tạo string ids chứa tất cả các ID đã chọn và kiểm tra điều kiện HoaDonNoiBo
                List<string> listIds = new List<string>();
                string firstHoaDonNoiBo = null;
                bool isFirstRow = true;

                foreach (DataGridViewRow row in selectedRows)
                {
                    if (row.Cells["id"].Value != null)
                    {
                        string id = row.Cells["id"].Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(id))
                        {
                            listIds.Add(id);
                        }
                    }

                    // Kiểm tra HoaDonNoiBo
                    string hoaDonNoiBo = row.Cells["hoadonnoibo"]?.Value?.ToString()?.Trim() ?? "";
                    string invoice_no_viettel = row.Cells["InvoiceNoViettel"]?.Value?.ToString()?.Trim() ?? "";
                    if (isFirstRow)
                    {
                        firstHoaDonNoiBo = hoaDonNoiBo;
                        isFirstRow = false;
                    }
                    else
                    {
                        // Kiểm tra xem có khác với dòng đầu tiên không
                        if (hoaDonNoiBo != firstHoaDonNoiBo)
                        {
                            MessageBox.Show("Các dòng đã chọn phải có chung Hóa Đơn Nội Bộ !");
                            return;
                        }
                    }

                    if (invoice_no_viettel != "" || !string.IsNullOrEmpty(invoice_no_viettel))
                    {
                        MessageBox.Show("Xuất hiện dữ liệu đã có Số Hóa Đơn Viettel");
                        return;
                    }
                }

                // Tự động convert các orders chưa convert và lưu vào database
                if (listIds.Count > 0)
                {
                    List<string> ordersToConvert = new List<string>();
                    cnn.Open();
                    string idsForSqlCheck = string.Join("','", listIds);
                    string sqlCheck = $"SELECT id FROM Orders_Viettel WHERE id IN ('{idsForSqlCheck}') AND (IsConvertToLiter IS NULL OR IsConvertToLiter = 0)";
                    SqlCommand comCheck = new SqlCommand(sqlCheck, cnn);
                    SqlDataReader reader = comCheck.ExecuteReader();
                    while (reader.Read())
                    {
                        ordersToConvert.Add(reader["id"].ToString());
                    }
                    reader.Close();
                    cnn.Close();

                    // Tự động convert các orders chưa convert
                    if (ordersToConvert.Count > 0)
                    {
                        int convertedCount = 0;
                        foreach (string orderId in ordersToConvert)
                        {
                            if (ConvertOrderToLiter(orderId))
                            {
                                convertedCount++;
                            }
                        }
                        
                        if (convertedCount > 0)
                        {
                            MessageBox.Show($"Đã tự động convert {convertedCount} order(s) sang Liter và lưu vào database!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Refresh lại dữ liệu sau khi convert để đảm bảo dữ liệu mới nhất
                            //Search();
                            // Tiếp tục xử lý tạo hóa đơn, không return
                        }
                    }
                }

                // Kiểm tra số lượng HoaDonNoiBo trong database có khớp với số data đã chọn chưa
                if (!string.IsNullOrEmpty(firstHoaDonNoiBo))
                {
                    cnn.Open();
                    string checkcount = $"SELECT Count(*) FROM Orders_viettel WHERE HoaDonNoiBo = '{firstHoaDonNoiBo}'";
                    SqlCommand comCheck = new SqlCommand(checkcount, cnn);
                    int countInDatabase = Convert.ToInt32(comCheck.ExecuteScalar());
                    cnn.Close();

                    if (countInDatabase < selectedRows.Count)
                    {
                        MessageBox.Show($"Số lượng Hóa Đơn Nội Bộ '{firstHoaDonNoiBo}' trong database ({countInDatabase}) ít hơn số dòng đã chọn ({selectedRows.Count}). Vui lòng kiểm tra lại!");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(strAccessToken))
                {
                    MessageBox.Show("Chưa lấy giá tị Token");
                    return;
                }

                string apiLink = urlAPI + @"/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/createInvoice/" + codeTax;
                string contentType = "application/json";

                int chk = chkExample.Checked ? 1 : 0;
                string request = "";

                if (chk == 1)
                {
                    request = createInvoiceExample(listIds);
                    txtjson.Text = request;
                }
                else
                {
                    request = txtBody.Text;
                    if (string.IsNullOrEmpty(request))
                    {
                        MessageBox.Show("Chưa nhập giá trị text của body");
                        return;
                    }
                }

                string result = CreateRequest.webRequestV2(apiLink, request, strAccessToken, "POST", contentType, proxy, port, ssl);
                //txterror.Text = result;

                // Parse invoiceNo từ result
                sohoadon_vtel = "";
                string[] sohoadon = result.Split(',');
                for (int i = 0; i < sohoadon.Length; i++)
                {
                    if (sohoadon[i].ToString().Contains("invoiceNo") == true)
                    {
                        sohoadon_vtel = sohoadon[i].Split(':')[1].ToString().Replace("\"", "").Trim();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(sohoadon_vtel))
                {
                    MessageBox.Show("Không thể lấy số hóa đơn từ kết quả trả về!");
                    return;
                }

                // Update database cho tất cả các ID đã chọn
                string dtNow = DateTime.Today.ToString("dd-MMM-yyyy");
                cnn.Open();
                // Tạo danh sách ID với dấu nháy đơn cho SQL IN clause
                string idsForSql = string.Join("','", listIds);
                string sqlupdate = "update Orders_viettel set DateInvViettel= '" + dtNow + "', InvoiceNoViettel = '" + sohoadon_vtel + "', mauso = '" + txtmauso.Text + "' where id in ('" + idsForSql + "')";
                SqlCommand comupdate = new SqlCommand(sqlupdate, cnn);
                comupdate.ExecuteNonQuery();

                cnn.Close();  // đóng kết nối

                MessageBox.Show("OK " + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("NOK " + ex.Message);
            }
        }

        private bool ConvertOrderToLiter(string orderId)
        {
            try
            {
                // Load order data từ database
                if (cnn.State != ConnectionState.Open)
                {
                    cnn.Open();
                }
                string sql = $"SELECT ServiceIds, ServiceNames, ServiceQuantities, UnitPrice FROM Orders_Viettel WHERE id = '{orderId}'";
                SqlCommand com = new SqlCommand(sql, cnn);
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cnn.Close();

                if (dt.Rows.Count == 0)
                {
                    return false;
                }

                DataRow row = dt.Rows[0];
                
                // Parse arrays
                string serviceIdsRaw = row["ServiceIds"]?.ToString()?.Trim() ?? "";
                string serviceNamesRaw = row["ServiceNames"]?.ToString()?.Trim() ?? "";
                string quantitiesRaw = row["ServiceQuantities"]?.ToString()?.Trim() ?? "";
                string unitPricesRaw = row["UnitPrice"]?.ToString()?.Trim() ?? "";

                // Remove brackets and quotes
                serviceIdsRaw = serviceIdsRaw.Replace("[", "").Replace("]", "").Replace("\"", "");
                serviceNamesRaw = serviceNamesRaw.Replace("[", "").Replace("]", "").Replace("\"", "");
                quantitiesRaw = quantitiesRaw.Replace("[", "").Replace("]", "");
                unitPricesRaw = unitPricesRaw.Replace("[", "").Replace("]", "");

                // Split arrays
                var serviceIds = serviceIdsRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(x => x.Trim())
                                               .ToList();
                var serviceNames = serviceNamesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(x => x.Trim())
                                                  .ToList();
                var quantities = quantitiesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(x => int.TryParse(x.Trim(), out var val) ? val : 0)
                                              .ToList();
                var unitPrices = unitPricesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(x => double.TryParse(x.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val) ? val : 0)
                                              .ToList();

                // Convert từng item sang Liter
                bool hasConverted = false;
                int maxCount = Math.Max(Math.Max(serviceIds.Count, serviceNames.Count), Math.Max(quantities.Count, unitPrices.Count));
                
                for (int i = 0; i < maxCount; i++)
                {
                    if (i < serviceIds.Count && !string.IsNullOrEmpty(serviceIds[i]))
                    {
                        string serviceId = serviceIds[i];
                        
                        // Lấy Production từ Services
                        if (cnn.State != ConnectionState.Open)
                        {
                            cnn.Open();
                        }
                        string sqlServices = $"SELECT Production FROM Services WHERE id = '{serviceId}'";
                        SqlCommand comServices = new SqlCommand(sqlServices, cnn);
                        SqlDataAdapter daServices = new SqlDataAdapter(comServices);
                        DataTable dtServices = new DataTable();
                        daServices.Fill(dtServices);
                        cnn.Close();

                        if (dtServices != null && dtServices.Rows.Count > 0)
                        {
                            double production = 0;
                            if (dtServices.Rows[0]["Production"] != null && dtServices.Rows[0]["Production"] != DBNull.Value)
                            {
                                double.TryParse(dtServices.Rows[0]["Production"].ToString(), out production);
                            }

                            if (production > 0 && i < quantities.Count && i < unitPrices.Count)
                            {
                                // quantity mới = Production * quantity hiện tại
                                int quantityNew = (int)(production * quantities[i]);
                                
                                // unitPrice mới = unitPrice hiện tại / Production (không làm tròn)
                                double unitPriceNew = unitPrices[i] / production;

                                quantities[i] = quantityNew;
                                unitPrices[i] = unitPriceNew;
                                hasConverted = true;
                            }
                        }
                    }
                }

                // Nếu có convert, update lại vào database
                if (hasConverted)
                {
                    // Build arrays mới
                    string newServiceIds = "[" + string.Join(",", serviceIds.Select(id => "\"" + id + "\"")) + "]";
                    string newServiceNames = "[" + string.Join(",", serviceNames.Select(name => "\"" + name + "\"")) + "]";
                    string newQuantities = "[" + string.Join(",", quantities.Select(q => q.ToString())) + "]";
                    string newUnitPrices = "[" + string.Join(",", unitPrices.Select(p => p.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]";

                    // Escape single quotes for SQL
                    newServiceIds = newServiceIds.Replace("'", "''");
                    newServiceNames = newServiceNames.Replace("'", "''");

                    // Update vào database
                    if (cnn.State != ConnectionState.Open)
                    {
                        cnn.Open();
                    }
                    string sqlUpdate = $@"UPDATE Orders_Viettel SET 
                        ServiceIds = '{newServiceIds}',
                        ServiceNames = '{newServiceNames}',
                        ServiceQuantities = '{newQuantities}',
                        UnitPrice = '{newUnitPrices}',
                        IsConvertToLiter = 1
                        WHERE id = '{orderId}'";
                    SqlCommand comUpdate = new SqlCommand(sqlUpdate, cnn);
                    comUpdate.ExecuteNonQuery();
                    cnn.Close();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi convert order {orderId}: {ex.Message}");
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
                return false;
            }
        }

        private void lậpNhápHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if( string.IsNullOrEmpty(txttaxcode.Text))
                {
                    MessageBox.Show("Input Tax Code.!");
                    return;
                }
                codeTax=txttaxcode.Text;

                if (dataGridView1.Rows.Count <= 0)
                {
                    MessageBox.Show("No Data.!");
                    return;
                }

                // Lấy tất cả các dòng đã được tick checkbox
                List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Select"] != null)
                    {
                        object cellValue = row.Cells["Select"].Value;
                        if (cellValue != null && cellValue != DBNull.Value)
                        {
                            bool isChecked = false;
                            if (cellValue is bool)
                            {
                                isChecked = (bool)cellValue;
                            }
                            else
                            {
                                bool.TryParse(cellValue.ToString(), out isChecked);
                            }
                            
                            if (isChecked)
                            {
                                selectedRows.Add(row);
                            }
                        }
                    }
                }

                if (selectedRows.Count <= 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một dòng dữ liệu!");
                    return;
                }

                // Tạo string ids chứa tất cả các ID đã chọn và kiểm tra điều kiện customerphone, customername
                List<string> listIds = new List<string>();
                string firstCustomerPhone = null;
                string firstCustomerName = null;
                bool isFirstRow = true;
                string customername_invoiceno = null;
                foreach (DataGridViewRow row in selectedRows)
                {

                    if (row.Cells["id"].Value != null)
                    {
                        string id = row.Cells["id"].Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(id))
                        {
                            listIds.Add(id);
                        }
                    }

                    // Kiểm tra customerphone và customername
                    string customerPhone = row.Cells["CustomerPhone"]?.Value?.ToString()?.Trim() ?? "";
                    string customerName = row.Cells["CustomerName"]?.Value?.ToString()?.Trim() ?? "";
                    customername_invoiceno = customerName.Trim();
                    string invoice_no_viettel = row.Cells["InvoiceNoViettel"]?.Value?.ToString()?.Trim() ?? "";
                    if (isFirstRow)
                    {
                        firstCustomerPhone = customerPhone;
                        firstCustomerName = customerName;
                        isFirstRow = false;
                    }
                    else
                    {
                        // Kiểm tra xem có khác với dòng đầu tiên không
                        if (customerPhone != firstCustomerPhone || customerName != firstCustomerName)
                        {
                            MessageBox.Show("Các dòng đã chọn phải có chung 1 đối tượng khách hàng !");
                            return;
                        }
                    }

                    if(invoice_no_viettel != "" || !string.IsNullOrEmpty(invoice_no_viettel))
                    {
                        MessageBox.Show("Xuất hiện dữ liệu đã có Số Hóa Đơn Viettel");
                        return;
                    }
                }

                string ids = string.Join(",", listIds); // Tạo string ids từ danh sách

                // Tự động convert các orders chưa convert và lưu vào database
                if (listIds.Count > 0)
                {
                    List<string> ordersToConvert = new List<string>();
                    cnn.Open();
                    string idsForSqlCheck = string.Join("','", listIds);
                    string sqlCheck = $"SELECT id FROM Orders_Viettel WHERE id IN ('{idsForSqlCheck}') AND (IsConvertToLiter IS NULL OR IsConvertToLiter = 0)";
                    SqlCommand comCheck = new SqlCommand(sqlCheck, cnn);
                    SqlDataReader reader = comCheck.ExecuteReader();
                    while (reader.Read())
                    {
                        ordersToConvert.Add(reader["id"].ToString());
                    }
                    reader.Close();
                    cnn.Close();

                    // Tự động convert các orders chưa convert
                    if (ordersToConvert.Count > 0)
                    {
                        int convertedCount = 0;
                        foreach (string orderId in ordersToConvert)
                        {
                            if (ConvertOrderToLiter(orderId))
                            {
                                convertedCount++;
                            }
                        }
                        
                        if (convertedCount > 0)
                        {
                            MessageBox.Show($"Đã tự động convert {convertedCount} order(s) sang Liter và lưu vào database!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Refresh lại dữ liệu sau khi convert để đảm bảo dữ liệu mới nhất
                            //Search();
                            // Tiếp tục xử lý tạo hóa đơn nháp, không return
                        }
                    }
                }

                // Hiển thị form nhập số hóa đơn nội bộ
                frmInputInvoiceNo frmInput = new frmInputInvoiceNo(customername_invoiceno);
                if (frmInput.ShowDialog() != DialogResult.OK)
                {
                    return; // Người dùng hủy, không tiếp tục xử lý
                }

                sohoadon_noibo = frmInput.InvoiceNo; // Lấy số hóa đơn nội bộ từ form

                // Lấy dòng đầu tiên được tick để xử lý
                int index = selectedRows[0].Index;
                String hoadonid = dataGridView1.Rows[index].Cells["id"].Value.ToString().Trim();

                if (string.IsNullOrEmpty(strAccessToken))
                {
                    MessageBox.Show("Chưa lấy giá tị Token");
                    return;
                }
        
                string apiLink = urlAPI + @"/services/einvoiceapplication/api/InvoiceAPI/InvoiceUtilsWS/createInvoiceDraftPreview/" + codeTax;
                string contentType = "application/json";

                int chk = chkExample.Checked ? 1 : 0;
                string request = "";

                if (chk == 1)
                {
                    request = createInvoiceExample(listIds);
                    txtjson.Text = request;
                }
                else
                {
                    request = txtBody.Text;
                    if (string.IsNullOrEmpty(request))
                    {
                        MessageBox.Show("Chưa nhập giá trị text của body");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(request))
                {
                    return;
                }
                    string result = CreateRequest.webRequestV2(apiLink, request, strAccessToken, "POST", contentType, proxy, port, ssl);
                MessageBox.Show( result);
                //txterror.Text = result;
                int startIndex = result.IndexOf(@"fileName"":""") + ((@"""fileName"":""").Length);
                int length = result.IndexOf(@""",""fileToBytes") - startIndex;
                //string fileName = sohoadon_noibo + "_" +
                //                DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year +
                //                DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                string fileName = sohoadon_noibo;
                string[] tach;
                tach = result.Split(',');
                string[] tachok;
                tachok = tach[2].Split(':');
                string tachcuoi = tachok[1].Replace("\"", "");
                using (System.IO.FileStream stream = System.IO.File.Create("c:\\temp\\" + fileName + ".pdf"))
                {
                    System.Byte[] byteArray = System.Convert.FromBase64String(tachcuoi);
                    stream.Write(byteArray, 0, byteArray.Length);
                }
                // insert vao taxinvoice theo so hoa don vtel dieu kien theo hoadonid
                string dtNow = DateTime.Today.ToString("dd-MMM-yyyy");
                cnn.Open();
                // Tạo danh sách ID với dấu nháy đơn cho SQL IN clause
                string idsForSql = string.Join("','", listIds);
                string sqlupdate = "update Orders_viettel set IsDraft=1 , hoadonnoibo ='"+ sohoadon_noibo + "' where id in ('" + idsForSql + "')";  // update tất cả các ID đã chọn
                SqlCommand comupdate = new SqlCommand(sqlupdate, cnn); //bat dau truy van
                comupdate.ExecuteNonQuery();
                
                //update table orders
                string sqlupdate_orders = "update Orders set IsDraft=1 , hoadonnoibo ='" + sohoadon_noibo + "' where id in ('" + idsForSql + "')";  // update tất cả các ID đã chọn
                SqlCommand comupdate_orders = new SqlCommand(sqlupdate_orders, cnn); //bat dau truy van
                comupdate_orders.ExecuteNonQuery();

                cnn.Close();  // đóng kết nối
                // lay files da xuat ve
                MessageBox.Show("OK " + result);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString ());
            }
        }

        private void cbocustomername_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtCustomers == null || isUpdatingCustomerName) return;

                isUpdatingCustomerName = true;

                string searchText = cbocustomername.Text;
                int selectionStart = cbocustomername.SelectionStart;
                int selectionLength = cbocustomername.SelectionLength;

                // Lọc dữ liệu không phân biệt chữ hoa/thường (case-insensitive) với LIKE search
                DataView dv = new DataView(dtCustomers);
                
                if (!string.IsNullOrEmpty(searchText))
                {
                    // Lọc bằng cách duyệt qua rows với case-insensitive và LIKE (Contains)
                    DataTable filteredTable = dtCustomers.Clone();
                    string searchTextLower = searchText.ToLower();
                    
                    foreach (DataRow row in dtCustomers.Rows)
                    {
                        string nameValue = row["Name"] != DBNull.Value ? row["Name"].ToString() : "";
                        if (nameValue.ToLower().Contains(searchTextLower))
                        {
                            filteredTable.ImportRow(row);
                        }
                    }
                    
                    dv = new DataView(filteredTable);
                }
                else
                {
                    dv = new DataView(dtCustomers);
                }

                cbocustomername.DataSource = dv;
                cbocustomername.DisplayMember = "Name";
                cbocustomername.ValueMember = "Name";

                // Khôi phục text và vị trí con trỏ
                cbocustomername.Text = searchText;
                if (selectionStart <= searchText.Length)
                {
                    cbocustomername.SelectionStart = selectionStart;
                    cbocustomername.SelectionLength = selectionLength;
                }

                isUpdatingCustomerName = false;
            }
            catch (Exception ex)
            {
                isUpdatingCustomerName = false;
                // Xử lý lỗi nếu có
            }
        }

        private void cboemployeename_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtEmployees == null || isUpdatingEmployeeName) return;

                isUpdatingEmployeeName = true;

                string searchText = cboemployeename.Text;
                int selectionStart = cboemployeename.SelectionStart;
                int selectionLength = cboemployeename.SelectionLength;

                // Lọc dữ liệu không phân biệt chữ hoa/thường (case-insensitive) với LIKE search
                DataView dv = new DataView(dtEmployees);
                
                if (!string.IsNullOrEmpty(searchText))
                {
                    // Lọc bằng cách duyệt qua rows với case-insensitive và LIKE (Contains)
                    DataTable filteredTable = dtEmployees.Clone();
                    string searchTextLower = searchText.ToLower();
                    
                    foreach (DataRow row in dtEmployees.Rows)
                    {
                        string nameValue = row["Name"] != DBNull.Value ? row["Name"].ToString() : "";
                        if (nameValue.ToLower().Contains(searchTextLower))
                        {
                            filteredTable.ImportRow(row);
                        }
                    }
                    
                    dv = new DataView(filteredTable);
                }
                else
                {
                    dv = new DataView(dtEmployees);
                }

                cboemployeename.DataSource = dv;
                cboemployeename.DisplayMember = "Name";
                cboemployeename.ValueMember = "Name";

                // Khôi phục text và vị trí con trỏ
                cboemployeename.Text = searchText;
                if (selectionStart <= searchText.Length)
                {
                    cboemployeename.SelectionStart = selectionStart;
                    cboemployeename.SelectionLength = selectionLength;
                }

                isUpdatingEmployeeName = false;
            }
            catch (Exception ex)
            {
                isUpdatingEmployeeName = false;
                // Xử lý lỗi nếu có
            }
        }

        private void txtvat_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số, dấu chấm, và phím điều khiển (Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Chỉ cho phép 1 dấu chấm
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void txtvat_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            // Nếu người dùng để trống thì bỏ qua
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "0.00";
                return;
            }

            // Chuyển dấu phẩy thành dấu chấm để tránh lỗi parse
            string input = txt.Text.Replace(',', '.');

            if (double.TryParse(input, out double value))
            {
                txt.Text = value.ToString("0.00"); // format 2 chữ số thập phân
            }
            else
            {
                // Nếu không phải số hợp lệ -> reset
                txt.Text = "0.00";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtmauso_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmInvoice_Load(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
           
                // Lấy các dòng đã được tick checkbox
                List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Select"] != null)
                    {
                        object cellValue = row.Cells["Select"].Value;
                        if (cellValue != null && cellValue != DBNull.Value)
                        {
                            bool isChecked = false;
                            if (cellValue is bool)
                            {
                                isChecked = (bool)cellValue;
                            }
                            else
                            {
                                bool.TryParse(cellValue.ToString(), out isChecked);
                            }
                            
                            if (isChecked)
                            {
                                selectedRows.Add(row);
                            }
                        }
                    }
                }

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một dòng dữ liệu để chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Chỉ cho phép chọn 1 row
                if (selectedRows.Count > 1)
                {
                    MessageBox.Show("Vui lòng chỉ chọn một dòng dữ liệu để chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy ID của order đã chọn
                string orderId = selectedRows[0].Cells["id"]?.Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(orderId))
                {
                    MessageBox.Show("Không thể lấy ID của order!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Mở form chỉnh sửa 1 order
                frmEditOrder frmEdit = new frmEditOrder(orderId);
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    // Refresh lại dữ liệu sau khi lưu thành công
                    Search();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở form chỉnh sửa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Format UnitPrice (nvarchar - mảng) với dấu phẩy phân cách hàng nghìn
            if (dataGridView1.Columns[e.ColumnIndex].Name == "UnitPrice" && e.Value != null)
            {
                string valueStr = e.Value.ToString();
                // Loại bỏ dấu ngoặc vuông nếu có (format array)
                valueStr = valueStr.Replace("[", "").Replace("]", "").Trim();
                
                if (!string.IsNullOrEmpty(valueStr))
                {
                    // Parse tất cả các giá trị trong mảng và format lại
                    string[] values = valueStr.Split(',');
                    List<string> formattedValues = new List<string>();
                    
                    foreach (string val in values)
                    {
                        string trimmedVal = val.Trim().Replace("\"", "");
                        if (double.TryParse(trimmedVal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double numValue))
                        {
                            formattedValues.Add(numValue.ToString("#,##0.################################################################", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            formattedValues.Add(trimmedVal);
                        }
                    }
                    
                    // Hiển thị tất cả các giá trị đã format, cách nhau bằng dấu "/"
                    e.Value = string.Join("/", formattedValues);
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
