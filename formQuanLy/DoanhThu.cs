using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formQuanLy
{
    public partial class formDoanhThu : Form
    {
        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void pnDoanhThu_Paint(object sender, PaintEventArgs e)
        {

        }
        private void rb9_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void rb12_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void rb11_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void rb10_CheckedChanged(object sender, EventArgs e)
        {

        }


        public formDoanhThu()
        {
            InitializeComponent();
            if (viewDoanhThu.Columns.Count == 0)
            {
                //maVe,maCD,sl,ThanhTien
                viewDoanhThu.Columns.Add("maVe", "Mã vé");
                viewDoanhThu.Columns.Add("maCD", "Mã Chuyến Đi");
                viewDoanhThu.Columns.Add("soLuong", "Số Lượng");
                viewDoanhThu.Columns.Add("thanhTien", "Thành Tiền");
            }

            btnXem.Click += btnXem_Click;
            btnBack.Click += btnBack_Click;

            LamMoiForm();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT d.maVe,
                           d.maCD,
                           ISNULL(d.soLuong,0) AS soLuong,
                           ISNULL(d.thanhTien,0) AS thanhTien,
                           ISNULL(k.hoTen, '') AS ngKH
                    FROM DATVE d
                    LEFT JOIN CHUYENDI c ON d.maCD = c.maCD
                    LEFT JOIN KHACHHANG k ON d.maKH = k.maKH
                    ORDER BY d.maVe";

                DataTable dt = new DataTable();
                using (SqlConnection conn = Database.GetConnection())
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                viewDoanhThu.Rows.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    viewDoanhThu.Rows.Add(
                        r["maVe"].ToString(),
                        r["maCD"].ToString(),
                        r["soLuong"].ToString(),
                        Convert.ToDecimal(r["thanhTien"]).ToString("N2"),
                        r["ngKH"].ToString()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LamMoiForm()
        {
            txtNam.Text = DateTime.Now.Year.ToString();
            rbQuy1.Checked = rbQuy2.Checked = rbQuy3.Checked = rbQuy4.Checked = false;
            rb1.Checked = rb2.Checked = rb3.Checked = rb4.Checked = rb5.Checked = rb6.Checked =
            rb7.Checked = rb8.Checked = rb9.Checked = rb10.Checked = rb11.Checked = rb12.Checked = false;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (UserSession.VaiTro == "QuanLy")
            {
                formTrangChuQuanLy f = new formTrangChuQuanLy();
                f.Show();
                this.Hide();
            }
            else if (UserSession.VaiTro == "NhanVien")
            {
                formTrangChuNhanVien f = new formTrangChuNhanVien();
                f.Show();
                this.Hide();
            }
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            try
            {
                // parse year
                if (!int.TryParse(txtNam.Text.Trim(), out int year) || year < 1)
                {
                    MessageBox.Show("Vui lòng nhập Năm hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int monthFrom = 1, monthTo = 12;

                // check quarter radios first
                if (rbQuy1.Checked) { monthFrom = 1; monthTo = 3; }
                else if (rbQuy2.Checked) { monthFrom = 4; monthTo = 6; }
                else if (rbQuy3.Checked) { monthFrom = 7; monthTo = 9; }
                else if (rbQuy4.Checked) { monthFrom = 10; monthTo = 12; }
                else
                {
                    // fallback: check month radios (rb1..rb12)
                    if (rb1.Checked) { monthFrom = monthTo = 1; }
                    else if (rb2.Checked) { monthFrom = monthTo = 2; }
                    else if (rb3.Checked) { monthFrom = monthTo = 3; }
                    else if (rb4.Checked) { monthFrom = monthTo = 4; }
                    else if (rb5.Checked) { monthFrom = monthTo = 5; }
                    else if (rb6.Checked) { monthFrom = monthTo = 6; }
                    else if (rb7.Checked) { monthFrom = monthTo = 7; }
                    else if (rb8.Checked) { monthFrom = monthTo = 8; }
                    else if (rb9.Checked) { monthFrom = monthTo = 9; }
                    else if (rb10.Checked) { monthFrom = monthTo = 10; }
                    else if (rb11.Checked) { monthFrom = monthTo = 11; }
                    else if (rb12.Checked) { monthFrom = monthTo = 12; }
                }

                string sql = @"
                    SELECT d.maVe,
                           d.maCD,
                           ISNULL(d.soLuong,0) AS soLuong,
                           ISNULL(d.thanhTien,0) AS thanhTien,
                           ISNULL(k.hoTen, '') AS ngKH
                    FROM DATVE d
                    LEFT JOIN CHUYENDI c ON d.maCD = c.maCD
                    LEFT JOIN KHACHHANG k ON d.maKH = k.maKH
                    WHERE YEAR(d.ngDat) = @year
                      AND MONTH(d.ngDat) BETWEEN @mfrom AND @mto
                    ORDER BY d.maVe";

                DataTable dt = new DataTable();
                using (SqlConnection conn = Database.GetConnection())
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@mfrom", monthFrom);
                    cmd.Parameters.AddWithValue("@mto", monthTo);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                viewDoanhThu.Rows.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    var maVe = r["maVe"]?.ToString() ?? "";
                    var maCD = r["maCD"]?.ToString() ?? "";
                    var soLuong = 0;
                    try { soLuong = Convert.ToInt32(r["soLuong"]); } catch { soLuong = 0; }
                    var thanhTien = 0m;
                    try { thanhTien = Convert.ToDecimal(r["thanhTien"]); } catch { thanhTien = 0m; }
                    var ngKH = r["ngKH"]?.ToString() ?? "";

                    viewDoanhThu.Rows.Add(maVe, maCD, soLuong, thanhTien.ToString("N2"), ngKH);
                }

                if (viewDoanhThu.Columns["thanhTien"] != null)
                {
                    viewDoanhThu.Columns["thanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
