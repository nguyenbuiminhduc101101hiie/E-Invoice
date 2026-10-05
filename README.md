# InvoiceClient — Công cụ xuất hóa đơn điện tử Viettel (NPP Bảo Ngọc)

> Ứng dụng Windows để kế toán lấy đơn hàng từ hệ thống bán hàng, kiểm tra/chỉnh sửa rồi **phát hành hóa đơn điện tử qua Viettel S-Invoice (vInvoice)** chỉ với vài thao tác — thay cho việc nhập tay từng hóa đơn trên cổng Viettel.

## 1. Bài toán giải quyết

Nhà phân phối ghi nhận đơn hàng hằng ngày (qua app bán hàng, lưu trong SQL Server). Cuối ngày/cuối kỳ, kế toán cần xuất hóa đơn GTGT cho các đơn đó. InvoiceClient:

- Lấy danh sách đơn hàng theo khoảng ngày (hoặc theo **ngày thanh toán**), hiển thị trạng thái đã/chưa xuất hóa đơn, trạng thái thanh toán.
- Cho phép sửa thông tin đơn trước khi xuất: khách hàng, mã số thuế, dịch vụ, số lượng, đơn giá, thuế suất VAT; **quy đổi đơn vị sang Lít**; sửa hàng loạt nhiều đơn.
- Gọi API Viettel để:
  - **Lập hóa đơn nháp / xem trước** (`createInvoiceDraftPreview`)
  - **Phát hành hóa đơn** (`createInvoice`) — hỗ trợ hóa đơn nhiều mức thuế suất, đơn giá đã gồm VAT, xuất cho **cá nhân**
  - **Tải file PDF** hóa đơn (`getInvoiceRepresentationFile`)
  - **Hủy hóa đơn** (`cancelTransactionInvoice`)
  - **Cập nhật trạng thái thanh toán** (`updatePaymentStatus`)
  - Tra cứu danh sách hóa đơn đã phát hành (`getInvoiceList`, `getListInvoiceDataControl`)
- Ghi lại số hóa đơn, ký hiệu, kết quả xuất vào database; gửi email hóa đơn qua SMTP.
- Quản lý **tài khoản đăng nhập App Hóa đơn** (tạo, phân quyền, khóa/mở; mật khẩu băm BCrypt).

## 2. Công nghệ

| Hạng mục | Công nghệ |
|---|---|
| Ứng dụng | C# **Windows Forms**, .NET Framework **4.8** |
| Database | SQL Server (ADO.NET `SqlConnection`) |
| Tích hợp | Viettel S-Invoice REST API (`https://api-vinvoice.viettel.vn`), hỗ trợ đi qua Proxy, bật/tắt TLS |
| Thư viện | Newtonsoft.Json, log4net (log ghi tại `Logs/logfile.txt`), BCrypt.Net-Next — đặt sẵn trong `InvoiceClient/Lib` |

## 3. Dữ liệu sử dụng

| Bảng | Vai trò |
|---|---|
| `Orders_Viettel` | Đơn hàng cần xuất hóa đơn (chi tiết item, lịch sử thanh toán dạng JSON `PaymentHistory`, cờ `IsConvertToLiter`, số HĐ đã xuất) |
| `Orders` | Đơn hàng gốc |
| `Customers`, `Employees`, `Services`, `Information` | Khách hàng, nhân viên, danh mục dịch vụ/hàng hóa, thông tin công ty |
| `AppUsers` | Tài khoản đăng nhập App Hóa đơn |

`test_paymentdate_query.sql` là ví dụ truy vấn ngày thanh toán từ cột JSON `PaymentHistory` (`OPENJSON`).

## 4. Cấu trúc mã nguồn

```text
InvoiceClient/
  frmInvoice.cs              Màn hình chính: lọc đơn, lập nháp, phát hành, hủy, tải PDF, cập nhật thanh toán
  frmEditOrder.cs            Sửa 1 đơn hàng và danh sách item
  frmEditMultipleOrders.cs   Sửa nhiều đơn cùng lúc
  frmInputInvoiceNo.cs       Nhập số hóa đơn
  frmAppUsers.cs             Quản lý tài khoản App Hóa đơn
  CreateRequest.cs           Gửi HTTP request tới API Viettel (proxy, TLS, cookie token)
  InvoiceInfo.cs             Model JSON hóa đơn gửi Viettel
  NumberUtil.cs              Đọc số tiền thành chữ
  App.config                 Cấu hình API Viettel, proxy, SMTP, log4net
```

## 5. Cài đặt & chạy

1. Mở `InvoiceClient.sln` bằng Visual Studio (cần .NET Framework 4.8 Developer Pack).
2. Cấu hình `InvoiceClient/App.config`:
   - `APILink`, `User`, `Pass`, `CodeTax` — tài khoản & mã số thuế trên Viettel S-Invoice
   - `ProxyIP`, `ProxyPort` — chỉ khi mạng phải đi qua proxy
   - `SmtpServer`, `SmtpPort`, `SmtpUser`, `SmtpPassword`, `EnableSsl` — gửi email
3. Kiểm tra chuỗi kết nối SQL Server trong các form (`frmInvoice.cs`, `frmEditOrder.cs`, …).
4. Build & chạy (F5). Bản phát hành lấy từ `InvoiceClient/bin/Release`.

> ⚠️ Không commit mật khẩu Viettel/SMTP/SQL thật lên git. Nên chuyển chuỗi kết nối về `App.config` và dùng giá trị mẫu trong repo.

## 6. Quy trình sử dụng

1. Chọn khoảng ngày (hoặc tick **Lấy theo Payment Date**) → **Search**.
2. Kiểm tra từng đơn (dòng tô màu = chưa quy đổi đơn vị Lít), **Edit** nếu cần.
3. **Lập hóa đơn nháp** để xem trước → **Lập hóa đơn** để phát hành chính thức.
4. **Tải file PDF** hoặc gửi email cho khách; khi khách thanh toán → **Cập nhật trạng thái thanh toán**.
5. Sai sót → **Hủy hóa đơn** và xuất lại.
