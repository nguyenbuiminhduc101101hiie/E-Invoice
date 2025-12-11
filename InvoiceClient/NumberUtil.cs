using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceClient
{
    public class NumberUtil
    {
        public static string DocSoThanhChu(string number)
        {
            string strReturn = "";
            if (string.IsNullOrWhiteSpace(number))
                return "";

            // Làm sạch dữ liệu đầu vào
            string s = number.Trim()
                             .Replace(",", "")  // loại dấu phẩy
                             .Replace(" ", "")  // loại khoảng trắng
                             .Replace("-", ""); // bỏ dấu âm (xử lý sau)

            // Nếu có dấu chấm thập phân, cắt bỏ phần sau
            int dotIndex = s.IndexOf('.');
            if (dotIndex >= 0)
                s = s.Substring(0, dotIndex);

            while (s.Length > 0 && s.StartsWith("0"))
                s = s.Substring(1);

            string[] so = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = { "", "nghìn", "triệu", "tỷ" };

            int i, j, donvi, chuc, tram;
            bool booAm = false;
            decimal decS = 0;

            try
            {
                decS = Convert.ToDecimal(number);
            }
            catch
            {
                decS = 0;
            }

            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString("0");
                booAm = true;
            }

            i = s.Length;
            if (i == 0)
                strReturn = so[0] + " ";
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0) chuc = Convert.ToInt32(s.Substring(i - 1, 1)); else chuc = -1;
                    i--;
                    if (i > 0) tram = Convert.ToInt32(s.Substring(i - 1, 1)); else tram = -1;
                    i--;

                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        strReturn = hang[j] + " " + strReturn;
                    j++;
                    if (j > 3) j = 1;

                    if ((donvi == 1) && (chuc > 1))
                        strReturn = "mốt " + strReturn;
                    else if ((donvi == 5) && (chuc > 0))
                        strReturn = "lăm " + strReturn;
                    else if (donvi > 0)
                        strReturn = so[donvi] + " " + strReturn;

                    if (chuc < 0) break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) strReturn = "linh " + strReturn;
                        if (chuc == 1) strReturn = "mười " + strReturn;
                        if (chuc > 1) strReturn = so[chuc] + " mươi " + strReturn;
                    }

                    if (tram < 0) break;
                    else if ((tram > 0) || (chuc > 0) || (donvi > 0))
                        strReturn = so[tram] + " trăm " + strReturn;
                }
            }

            if (booAm) strReturn = "Âm " + strReturn;
            string result = strReturn.Trim();
            if (!string.IsNullOrEmpty(result))
                result = char.ToUpper(result[0]) + result.Substring(1) + " đồng chẵn";
            return result;
        }

    }
}
