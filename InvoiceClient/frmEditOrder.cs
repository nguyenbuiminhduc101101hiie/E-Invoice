using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace InvoiceClient
{
    public partial class frmEditOrder : Form
    {
        private string orderId;
        private SqlConnection cnn = new SqlConnection(@"Data Source=115.78.95.245;Initial Catalog=NPPBaoNgoc@;Integrated Security=false;user id=sa;password=qwerQWER1234!@#$");
        private DataTable dtServices;
        private List<OrderItem> orderItems = new List<OrderItem>();
        private int editingIndex = -1; // -1 means not editing, >= 0 means editing item at that index
        private bool IsConvertL = false;
        private bool IsConvertToLiterFromDB = false; // Lưu giá trị IsConvertToLiter từ database
        public class OrderItem
        {
            public string ServiceId { get; set; }
            public string ServiceName { get; set; }
            public int Quantity { get; set; }
            public int PromotionQuantity { get; set; }
            public double UnitPrice { get; set; }
            public bool IsConvertToLiter { get; set; } = false;

            // Số lượng chính xác để tính tiền = Quantity - PromotionQuantity
            public int ActualQuantity => Quantity - PromotionQuantity;
        }

        public frmEditOrder(string orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            IsConvertL = false;
        }

        private void frmEditOrder_Load(object sender, EventArgs e)
        {
            LoadServices();
            LoadOrderData();
            SetupDataGridView();
            btnAddItem.Text = "Thêm";
            editingIndex = -1;

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

                cboService.DataSource = dtServices;
                cboService.DisplayMember = "Name";
                cboService.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách dịch vụ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
        }

        private void LoadOrderData()
        {
            try
            {
                cnn.Open();
                string sql = $"SELECT * FROM Orders_Viettel WHERE id = '{orderId}'";
                SqlCommand com = new SqlCommand(sql, cnn);
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cnn.Close();

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    
                    // Load customer info
                    txtCustomerName.Text = row["CustomerName"]?.ToString() ?? "";
                    txtCustomerPhone.Text = row["CustomerPhone"]?.ToString() ?? "";
                    txtCustomerAddress.Text = row["CustomerAddress"]?.ToString() ?? "";
                    txtTaxPercent.Text = row["TaxPercent"]?.ToString() ?? "0";
                    // Không set txtTotalPrice từ database, sẽ được tính lại theo ActualQuantity trong CalculateTotalPrice()

                    // Load IsConvertToLiter từ database Orders_Viettel
                    if (row["IsConvertToLiter"] != null && row["IsConvertToLiter"] != DBNull.Value)
                    {
                        IsConvertToLiterFromDB = Convert.ToBoolean(row["IsConvertToLiter"]);
                    }
                    else
                    {
                        IsConvertToLiterFromDB = false;
                    }

                    // Parse arrays
                    string serviceIdsRaw = row["ServiceIds"]?.ToString()?.Trim() ?? "";
                    string serviceNamesRaw = row["ServiceNames"]?.ToString()?.Trim() ?? "";
                    string quantitiesRaw = row["ServiceQuantities"]?.ToString()?.Trim() ?? "";
                    string promotionQuantitiesRaw = row["promotionquantities"]?.ToString()?.Trim() ?? "";
                    string unitPricesRaw = row["UnitPrice"]?.ToString()?.Trim() ?? "";

                    // Remove brackets and quotes
                    serviceIdsRaw = serviceIdsRaw.Replace("[", "").Replace("]", "").Replace("\"", "");
                    serviceNamesRaw = serviceNamesRaw.Replace("[", "").Replace("]", "").Replace("\"", "");
                    quantitiesRaw = quantitiesRaw.Replace("[", "").Replace("]", "");
                    promotionQuantitiesRaw = promotionQuantitiesRaw.Replace("[", "").Replace("]", "");
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
                    var promotionQuantities = promotionQuantitiesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                    .Select(x => int.TryParse(x.Trim(), out var val) ? val : 0)
                                                                    .ToList();
                    var unitPrices = unitPricesRaw.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(x => double.TryParse(x.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val) ? val : 0)
                                                  .ToList();

                    // Create order items
                    orderItems.Clear();
                    int maxCount = Math.Max(Math.Max(Math.Max(serviceIds.Count, serviceNames.Count), Math.Max(quantities.Count, promotionQuantities.Count)), unitPrices.Count);
                    for (int i = 0; i < maxCount; i++)
                    {
                        orderItems.Add(new OrderItem
                        {
                            ServiceId = i < serviceIds.Count ? serviceIds[i] : "",
                            ServiceName = i < serviceNames.Count ? serviceNames[i] : "",
                            Quantity = i < quantities.Count ? quantities[i] : 0,
                            PromotionQuantity = i < promotionQuantities.Count ? promotionQuantities[i] : 0,
                            UnitPrice = i < unitPrices.Count ? unitPrices[i] : 0
                        });
                    }

                    RefreshDataGridView();
                    CalculateTotalPrice();
                    
                    // Tự động convert nếu order chưa được convert
                    if (!IsConvertToLiterFromDB)
                    {
                        ConvertToLiter(false); // Tự động convert, không hiển thị message
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu order: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
        }

        private void SetupDataGridView()
        {
            dgvItems.AutoGenerateColumns = false;
            dgvItems.Columns.Clear();

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ServiceName",
                HeaderText = "Tên dịch vụ",
                DataPropertyName = "ServiceName",
                Width = 300
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Số lượng",
                DataPropertyName = "Quantity",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "#,##0",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PromotionQuantity",
                HeaderText = "Số lượng khuyến mãi",
                DataPropertyName = "PromotionQuantity",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "#,##0",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ActualQuantity",
                HeaderText = "Số lượng thực",
                DataPropertyName = "ActualQuantity",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "#,##0",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UnitPrice",
                HeaderText = "Đơn giá",
                DataPropertyName = "UnitPrice",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "#,##0.################################################################",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Thành tiền",
                DataPropertyName = "Total",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "#,##0.################################################################",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                }
            });
        }

        private void RefreshDataGridView()
        {
            var bindingSource = new BindingSource();
            var displayItems = orderItems.Select(item => new
            {
                ServiceName = item.ServiceName,
                Quantity = item.Quantity,
                PromotionQuantity = item.PromotionQuantity,
                ActualQuantity = item.ActualQuantity,
                UnitPrice = item.UnitPrice,
                Total = item.ActualQuantity * item.UnitPrice
            }).ToList();

            bindingSource.DataSource = displayItems;
            dgvItems.DataSource = bindingSource;
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (cboService.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Remove thousand separators (commas) and parse unit price
            // Format "N0" displays numbers like 380,000, so we need to remove commas
            string unitPriceText = txtUnitPrice.Text.Replace(",", "").Trim();
            if (!double.TryParse(unitPriceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập đơn giá hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string serviceId = cboService.SelectedValue.ToString();
            string serviceName = cboService.Text;

            if (editingIndex >= 0 && editingIndex < orderItems.Count)
            {
                // Update existing item
                orderItems[editingIndex].ServiceId = serviceId;
                orderItems[editingIndex].ServiceName = serviceName;
                orderItems[editingIndex].Quantity = quantity;
                orderItems[editingIndex].UnitPrice = unitPrice;
                editingIndex = -1;
                btnAddItem.Text = "Thêm";
            }
            else
            {
                // Add new item
                orderItems.Add(new OrderItem
                {
                    ServiceId = serviceId,
                    ServiceName = serviceName,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            RefreshDataGridView();
            CalculateTotalPrice();

            // Clear input fields and reset editing mode
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            editingIndex = -1;
            btnAddItem.Text = "Thêm";
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                int selectedIndex = dgvItems.SelectedRows[0].Index;
                if (selectedIndex >= 0 && selectedIndex < orderItems.Count)
                {
                    // Hiển thị hộp thoại xác nhận
                    OrderItem item = orderItems[selectedIndex];
                    DialogResult result = MessageBox.Show(
                        $"Bạn có chắc chắn muốn xóa item '{item.ServiceName}'?\nSố lượng: {item.Quantity}, Đơn giá: {item.UnitPrice:N0}",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2
                    );

                    if (result == DialogResult.Yes)
                    {
                        orderItems.RemoveAt(selectedIndex);
                        
                        // Reset editing mode if the removed item was being edited
                        if (editingIndex == selectedIndex)
                        {
                            editingIndex = -1;
                            btnAddItem.Text = "Thêm";
                            txtQuantity.Text = "";
                            txtUnitPrice.Text = "";
                        }
                        else if (editingIndex > selectedIndex)
                        {
                            // Adjust editing index if item before it was removed
                            editingIndex--;
                        }
                        
                        RefreshDataGridView();
                        CalculateTotalPrice();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditItem_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                int selectedIndex = dgvItems.SelectedRows[0].Index;
                if (selectedIndex >= 0 && selectedIndex < orderItems.Count)
                {
                    OrderItem item = orderItems[selectedIndex];
                    editingIndex = selectedIndex;
                    
                    // Find service in combo
                    for (int i = 0; i < cboService.Items.Count; i++)
                    {
                        DataRowView row = cboService.Items[i] as DataRowView;
                        if (row != null && row["id"].ToString() == item.ServiceId)
                        {
                            cboService.SelectedIndex = i;
                            break;
                        }
                    }

                    // Lấy giá trị trực tiếp từ orderItems (nguồn gốc) để đảm bảo không bị làm tròn hay format
                    // Format với dấu phẩy phân cách hàng nghìn nhưng vẫn giữ nguyên số thập phân không giới hạn
                    txtQuantity.Text = item.Quantity.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
                    txtUnitPrice.Text = item.UnitPrice.ToString("#,##0.################################################################", System.Globalization.CultureInfo.InvariantCulture);

                    // Change button text to indicate editing mode
                    btnAddItem.Text = "Cập nhật";
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < orderItems.Count)
            {
                OrderItem item = orderItems[e.RowIndex];
                editingIndex = e.RowIndex;
                
                // Find service in combo
                for (int i = 0; i < cboService.Items.Count; i++)
                {
                    DataRowView row = cboService.Items[i] as DataRowView;
                    if (row != null && row["id"].ToString() == item.ServiceId)
                    {
                        cboService.SelectedIndex = i;
                        break;
                    }
                }

                // Lấy giá trị trực tiếp từ orderItems (nguồn gốc) để đảm bảo không bị làm tròn hay format
                // Format với dấu phẩy phân cách hàng nghìn nhưng vẫn giữ nguyên số thập phân không giới hạn
                txtQuantity.Text = item.Quantity.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
                txtUnitPrice.Text = item.UnitPrice.ToString("#,##0.################################################################", System.Globalization.CultureInfo.InvariantCulture);

                // Change button text to indicate editing mode
                btnAddItem.Text = "Cập nhật";
            }
        }

        private void txtTaxPercent_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            // Tính tổng tiền từ orderItems
            // QUAN TRỌNG: Sử dụng ActualQuantity (Quantity thực = Quantity - PromotionQuantity) để tính tiền
            // ActualQuantity là số lượng thực tế phải trả tiền (đã trừ số lượng khuyến mãi)
            // Sum() không làm tròn, nhưng double có giới hạn độ chính xác (~15-17 chữ số)
            // Tính toán chính xác từng item để tránh tích lũy sai số
            double totalWithoutTax = 0;
            foreach (var item in orderItems)
            {
                // Tính theo Quantity thực (ActualQuantity = Quantity - PromotionQuantity)
                int actualQty = item.ActualQuantity; // Quantity thực = Quantity - PromotionQuantity
                totalWithoutTax += actualQty * item.UnitPrice;
            }
            
            double taxPercent = 0;
            if (!double.TryParse(txtTaxPercent.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out taxPercent))
            {
                taxPercent = 0;
            }

            double taxAmount = totalWithoutTax * (taxPercent / 100);
            double totalWithTax = totalWithoutTax + taxAmount;

            // Format với dấu phẩy phân cách hàng nghìn nhưng vẫn giữ nguyên số thập phân không giới hạn
            // txtTotalPrice được tính theo Quantity thực (ActualQuantity)
            txtTotalPrice.Text = totalWithTax.ToString("#,##0.################################################################", System.Globalization.CultureInfo.InvariantCulture);
        }

        public bool SaveOrder()
        {
            try
            {
                if (orderItems.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một item!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Build arrays
                string serviceIds = "[" + string.Join(",", orderItems.Select(item => "\"" + item.ServiceId + "\"")) + "]";
                string serviceNames = "[" + string.Join(",", orderItems.Select(item => "\"" + item.ServiceName + "\"")) + "]";
                string quantities = "[" + string.Join(",", orderItems.Select(item => item.Quantity.ToString())) + "]";
                string promotionQuantities = "[" + string.Join(",", orderItems.Select(item => item.PromotionQuantity.ToString())) + "]";
                string unitPrices = "[" + string.Join(",", orderItems.Select(item => item.UnitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]";

                // Remove thousand separators (commas) before parsing
                string totalPriceText = txtTotalPrice.Text.Replace(",", "").Trim();
                double totalPrice = double.Parse(totalPriceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                
                // Parse tax percent (may have comma as decimal separator)
                string taxPercentText = txtTaxPercent.Text.Replace(",", ".").Trim();
                int taxPercent = int.Parse(taxPercentText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

                // Escape single quotes for SQL
                string customerName = txtCustomerName.Text.Replace("'", "''");
                string customerPhone = txtCustomerPhone.Text.Replace("'", "''");
                string customerAddress = txtCustomerAddress.Text.Replace("'", "''");
                serviceIds = serviceIds.Replace("'", "''");
                serviceNames = serviceNames.Replace("'", "''");
                string extraField = IsConvertL ? ", IsConvertToLiter = 1" : "";
                cnn.Open();
                string sql = $@"UPDATE Orders_Viettel SET 
                    CustomerName = N'{customerName}',
                    CustomerPhone = N'{customerPhone}',
                    CustomerAddress = N'{customerAddress}',
                    ServiceIds = '{serviceIds}',
                    ServiceNames = '{serviceNames}',
                    ServiceQuantities = '{quantities}',
                    promotionquantities = '{promotionQuantities}',
                    UnitPrice = '{unitPrices}',
                    TaxPercent = {taxPercent},
                    TotalPrice = {totalPrice}
                    {extraField}
                    WHERE id = '{orderId}'";

                SqlCommand com = new SqlCommand(sql, cnn);
                com.ExecuteNonQuery();
                cnn.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
                return false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveOrder())
            {
                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ConvertToLiter(bool showMessage = true)
        {
            try
            {
                if (orderItems.Count == 0)
                {
                    if (showMessage)
                    {
                        MessageBox.Show("Không có item nào để convert!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }

                // Kiểm tra xem order đã được convert sang Liter chưa (lấy từ database)
                if (IsConvertToLiterFromDB)
                {
                    if (showMessage)
                    {
                        MessageBox.Show("Đã convert sang Liter rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }

                // Chỉ hiển thị message xác nhận nếu showMessage = true
                if (showMessage)
                {
                    DialogResult result = MessageBox.Show(
                        "Bạn có chắc chắn muốn chuyển đổi tất cả items sang đơn vị Lít?\n" +
                        "Quantity sẽ được nhân với Production và UnitPrice sẽ được chia cho Production.",
                        "Xác nhận chuyển đổi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2
                    );

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                bool hasError = false;
                int convertedCount = 0;

                foreach (OrderItem item in orderItems)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(item.ServiceId))
                        {
                            continue;
                        }

                        // Lấy Production từ Services
                        if (cnn.State != ConnectionState.Open)
                        {
                            cnn.Open();
                        }
                        string sqlServices = $"SELECT Production FROM Services WHERE id = '{item.ServiceId}'";
                        SqlCommand comServices = new SqlCommand(sqlServices, cnn);
                        SqlDataAdapter daServices = new SqlDataAdapter(comServices);
                        DataTable dtServicesData = new DataTable();
                        daServices.Fill(dtServicesData);
                        cnn.Close();

                        if (dtServicesData != null && dtServicesData.Rows.Count > 0)
                        {
                            double production = 0;
                            if (dtServicesData.Rows[0]["Production"] != null && dtServicesData.Rows[0]["Production"] != DBNull.Value)
                            {
                                double.TryParse(dtServicesData.Rows[0]["Production"].ToString(), out production);
                            }

                            if (production > 0)
                            {
                                // quantity mới = Production * quantity hiện tại
                                int quantityNew = (int)(production * item.Quantity);
                                
                                // promotionQuantity mới = Production * promotionQuantity hiện tại
                                int promotionQuantityNew = (int)(production * item.PromotionQuantity);

                                // unitPrice mới = unitPrice hiện tại / Production (không làm tròn)
                                double unitPriceNew = item.UnitPrice / production;

                                item.Quantity = quantityNew;
                                item.PromotionQuantity = promotionQuantityNew;
                                item.UnitPrice = unitPriceNew;
                                item.IsConvertToLiter = true; // Đánh dấu đã convert sang Liter
                                convertedCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
                        if (showMessage)
                        {
                            MessageBox.Show($"Lỗi khi convert item '{item.ServiceName}': {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        if (cnn.State == ConnectionState.Open)
                            cnn.Close();
                    }
                }

                if (convertedCount > 0)
                {
                    RefreshDataGridView();
                    CalculateTotalPrice();
                    if (showMessage)
                    {
                        MessageBox.Show($"Đã chuyển đổi thành công {convertedCount} item(s) sang đơn vị Lít!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    IsConvertL = true;
                }
                else if (!hasError && showMessage)
                {
                    MessageBox.Show("Không có item nào được chuyển đổi. Vui lòng kiểm tra lại Production của các Services!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                {
                    MessageBox.Show("Lỗi khi chuyển đổi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
        }

        private void btnConvertToLit_Click(object sender, EventArgs e)
        {
            ConvertToLiter(true); // Gọi với showMessage = true để hiển thị message xác nhận
        }
    }
}

