using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InvoiceClient
{
    /// <summary>
    /// Quản lý tài khoản đăng nhập của app Hóa đơn (Flutter/Web) — bảng dbo.AppUsers.
    /// Mật khẩu lưu dạng BCrypt, tương thích với backend InvoiceApi.
    /// </summary>
    public partial class frmAppUsers : Form
    {
        private SqlConnection cnn = new SqlConnection(@"Data Source=115.78.95.245;Initial Catalog=NPPBaoNgoc@;Integrated Security=false;user id=sa;password=qwerQWER1234!@#$");
        private int? selectedUserId = null;

        private const string CreateTableSql = @"
IF OBJECT_ID(N'dbo.AppUsers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppUsers (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppUsers PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL,
        PasswordHash NVARCHAR(200) NOT NULL,
        FullName     NVARCHAR(100) NULL,
        Role         NVARCHAR(20)  NOT NULL CONSTRAINT DF_AppUsers_Role DEFAULT (N'User'),
        IsActive     BIT           NOT NULL CONSTRAINT DF_AppUsers_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(0)  NOT NULL CONSTRAINT DF_AppUsers_CreatedAt DEFAULT (SYSDATETIME()),
        LastLoginAt  DATETIME2(0)  NULL,
        CONSTRAINT UQ_AppUsers_Username UNIQUE (Username)
    );
END";

        public frmAppUsers()
        {
            InitializeComponent();
        }

        private void frmAppUsers_Load(object sender, EventArgs e)
        {
            cboRole.Items.Clear();
            cboRole.Items.Add("User");
            cboRole.Items.Add("Admin");
            try
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(CreateTableSql, cnn))
                {
                    cmd.ExecuteNonQuery();
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
                MessageBox.Show("Không tạo/kiểm tra được bảng AppUsers: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadUsers();
            ClearForm();
        }

        private void LoadUsers()
        {
            try
            {
                cnn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, Username AS [Tên đăng nhập], FullName AS [Họ tên], Role AS [Quyền], IsActive AS [Hoạt động], " +
                    "CreatedAt AS [Ngày tạo], LastLoginAt AS [Đăng nhập gần nhất] FROM dbo.AppUsers ORDER BY Username", cnn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cnn.Close();

                dgvUsers.DataSource = dt;
                if (dgvUsers.Columns.Contains("Id")) dgvUsers.Columns["Id"].Visible = false;
                lblCount.Text = dt.Rows.Count + " tài khoản";
            }
            catch (Exception ex)
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
                MessageBox.Show("Lỗi khi tải danh sách tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            selectedUserId = null;
            txtUsername.Text = "";
            txtUsername.ReadOnly = false;
            txtFullName.Text = "";
            txtPassword.Text = "";
            txtConfirm.Text = "";
            cboRole.SelectedItem = "User";
            chkActive.Checked = true;
            chkActive.Enabled = false;
            btnDelete.Enabled = false;
            grpInfo.Text = "Tạo tài khoản mới";
            lblPasswordHint.Text = "Tối thiểu 6 ký tự";
            dgvUsers.ClearSelection();
            txtUsername.Focus();
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

            selectedUserId = Convert.ToInt32(row.Cells["Id"].Value);
            txtUsername.Text = row.Cells["Tên đăng nhập"].Value?.ToString() ?? "";
            txtUsername.ReadOnly = true;
            txtFullName.Text = row.Cells["Họ tên"].Value == DBNull.Value ? "" : row.Cells["Họ tên"].Value?.ToString();
            cboRole.SelectedItem = row.Cells["Quyền"].Value?.ToString() == "Admin" ? "Admin" : "User";
            chkActive.Checked = row.Cells["Hoạt động"].Value is bool active && active;
            chkActive.Enabled = true;
            txtPassword.Text = "";
            txtConfirm.Text = "";
            btnDelete.Enabled = true;
            grpInfo.Text = "Sửa tài khoản: " + txtUsername.Text;
            lblPasswordHint.Text = "Để trống nếu không đổi mật khẩu";
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string password = txtPassword.Text;
            string role = cboRole.SelectedItem?.ToString() == "Admin" ? "Admin" : "User";
            bool isNew = selectedUserId == null;

            if (isNew && username.Length < 3)
            {
                MessageBox.Show("Tên đăng nhập tối thiểu 3 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }
            if ((isNew || password.Length > 0) && password.Length < 6)
            {
                MessageBox.Show("Mật khẩu tối thiểu 6 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }
            if (password != txtConfirm.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirm.Focus();
                return;
            }

            string hash = password.Length > 0 ? BCrypt.Net.BCrypt.HashPassword(password) : null;

            try
            {
                cnn.Open();
                if (isNew)
                {
                    using (SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM dbo.AppUsers WHERE Username = @u", cnn))
                    {
                        check.Parameters.AddWithValue("@u", username);
                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        {
                            cnn.Close();
                            MessageBox.Show("Tên đăng nhập đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO dbo.AppUsers (Username, PasswordHash, FullName, Role) VALUES (@u, @h, @f, @r)", cnn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@h", hash);
                        cmd.Parameters.AddWithValue("@f", string.IsNullOrEmpty(fullName) ? (object)DBNull.Value : fullName);
                        cmd.Parameters.AddWithValue("@r", role);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE dbo.AppUsers SET FullName = @f, Role = @r, IsActive = @a, PasswordHash = COALESCE(@h, PasswordHash) WHERE Id = @id", cnn))
                    {
                        cmd.Parameters.AddWithValue("@f", string.IsNullOrEmpty(fullName) ? (object)DBNull.Value : fullName);
                        cmd.Parameters.AddWithValue("@r", role);
                        cmd.Parameters.AddWithValue("@a", chkActive.Checked);
                        cmd.Parameters.Add("@h", SqlDbType.NVarChar, 200).Value = (object)hash ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@id", selectedUserId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                cnn.Close();

                MessageBox.Show(isNew ? "Đã tạo tài khoản " + username : "Đã cập nhật tài khoản " + username,
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
                MessageBox.Show("Lỗi khi lưu tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId == null) return;
            if (MessageBox.Show($"Xóa tài khoản '{txtUsername.Text}'?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.AppUsers WHERE Id = @id", cnn))
                {
                    cmd.Parameters.AddWithValue("@id", selectedUserId.Value);
                    cmd.ExecuteNonQuery();
                }
                cnn.Close();
                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                if (cnn.State == ConnectionState.Open) cnn.Close();
                MessageBox.Show("Lỗi khi xóa tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
