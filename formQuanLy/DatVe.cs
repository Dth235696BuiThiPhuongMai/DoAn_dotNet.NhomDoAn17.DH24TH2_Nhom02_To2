using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formQuanLy
{
    public partial class formDatVe : Form
    {
        private List<string> listTrangThai = new List<string>()
        {
            "Đã Thanh Toán",
            "Chưa Thanh Toán"
        };
        public formDatVe()
        {
            InitializeComponent();

            cbTrangThai.DataSource = listTrangThai;

            if (ViewDatVe.Columns.Count == 0)
            {
                ViewDatVe.Columns.Add("maVe", "Mã Vé");
                ViewDatVe.Columns.Add("maKH", "Mã Khách Hàng");
                ViewDatVe.Columns.Add("maCD", "Mã Chuyến Đi");
                ViewDatVe.Columns.Add("ngDat", "Ngày Đặt");  
                ViewDatVe.Columns.Add("trangThai", "Trạng Thái");
                ViewDatVe.Columns.Add("giaVe", "Giá Vé");
                ViewDatVe.Columns.Add("soLuong", "Số Lượng");
                ViewDatVe.Columns.Add("thanhTien", "Thành Tiền");
            }

            ViewDatVe.SelectionChanged += ViewDatVe_SelectionChanged;

            btnThem.Click += btnThem_Click;
            btnXoa.Click += btnXoa_Click;
            btnSua.Click += btnSua_Click;
            btnHuy.Click += btnHuy_Click;
            btnLuu.Click += btnLuu_Click;
            btnThoat.Click += btnThoat_Click;
            btnBack.Click += btnBack_Click;

            loadMaKH();
            LoadMaChuyenDi();
            LoadData();
            LamMoiForm();
            
        }

        public string AutoMaVe()
        {
            try
            {
                List<string> maListGrid = new List<string>();
                foreach (DataGridViewRow row in ViewDatVe.Rows)
                {
                    if (row.IsNewRow) continue;
                    var val = row.Cells["maVe"].Value;
                    if (val != null)
                    {
                        var s = val.ToString().Trim();
                        if (!string.IsNullOrEmpty(s)) maListGrid.Add(s);
                    }
                }

                List<string> maListDb = new List<string>();
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT maVe FROM DATVE", conn))
                    {
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                var s = r.GetString(0).Trim();
                                if (!string.IsNullOrEmpty(s)) maListDb.Add(s);
                            }
                        }
                    }
                }

                var deletedInGrid = maListDb.Except(maListGrid).ToList();
                int parsed;
                if (deletedInGrid.Count > 0)
                {
                    var candidates = new List<int>();
                    foreach (var m in deletedInGrid)
                    {
                        if (m.Length >= 7 && m.StartsWith("VE", StringComparison.OrdinalIgnoreCase))
                        {
                            var sub = m.Substring(2);
                            if (int.TryParse(sub, out parsed) && parsed > 0) candidates.Add(parsed);
                        }
                    }
                    if (candidates.Count > 0)
                    {
                        int reuse = candidates.Min();
                        return $"VE{reuse:00000000}";
                    }
                }

                var maList = maListGrid.Union(maListDb).ToList();

                var used = new HashSet<int>();
                foreach (var m in maList)
                {
                    if (string.IsNullOrWhiteSpace(m)) continue;
                    if (m.Length >= 7 && m.StartsWith("VE", StringComparison.OrdinalIgnoreCase))
                    {
                        var sub = m.Substring(2);
                        if (int.TryParse(sub, out parsed) && parsed > 0) used.Add(parsed);
                    }
                }

                int candidate = 1;
                while (used.Contains(candidate)) candidate++;

                return $"VE{candidate:00000000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AutoMaVe lỗi: " + ex.Message);
                return "VE00000001";
            }
        }
        private void LoadMaChuyenDi()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                
                string sql = "SELECT maCD FROM CHUYENDI WHERE trangThai = N'Hoạt động'";

                
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cbMaCD.Items.Clear();

                    foreach (DataRow r in dt.Rows)
                    {
                        cbMaCD.Items.Add(r["maCD"].ToString());
                    }

                    cbMaCD.Text = " ";
                }
            }
        }
        private void loadMaKH()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql = "SELECT maKH FROM KHACHHANG";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cbMaKH.Items.Clear();

                    foreach (DataRow r in dt.Rows)
                    {
                        cbMaKH.Items.Add(r["maKH"].ToString());
                    }

                    cbMaKH.Text = " ";
                }
            }
        }
        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();
                ViewDatVe.Rows.Clear();

                const string sql = @"
            SELECT maVe, maKH, maCD, ngDat, trangThai, giaVe, SoLuong, ThanhTien 
            FROM DATVE 
            ORDER BY maVe";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        string maVe = r.Table.Columns.Contains("maVe") && r["maVe"] != DBNull.Value ? r["maVe"].ToString() : string.Empty;
                        string maKH = r.Table.Columns.Contains("maKH") && r["maKH"] != DBNull.Value ? r["maKH"].ToString() : string.Empty;
                        string maCD = r.Table.Columns.Contains("maCD") && r["maCD"] != DBNull.Value ? r["maCD"].ToString() : string.Empty;

                        string ngDat = string.Empty;
                        if (r.Table.Columns.Contains("ngDat") && r["ngDat"] != DBNull.Value)
                        {
                            DateTime dtParsed;
                            if (r["ngDat"] is DateTime)
                                dtParsed = (DateTime)r["ngDat"];
                            else if
                                (DateTime.TryParse(r["ngDat"].ToString(), out dtParsed)); 
                            else
                                dtParsed = DateTime.MinValue;

                            if (dtParsed != DateTime.MinValue)
                                ngDat = dtParsed.ToString("yyyy-MM-dd"); 
                        }

                        string trangThai = r.Table.Columns.Contains("trangThai") && r["trangThai"] != DBNull.Value ? r["trangThai"].ToString() : string.Empty;

                        decimal giaVe = 0m;
                        if (r.Table.Columns.Contains("giaVe") && r["giaVe"] != DBNull.Value)
                        {
                            decimal.TryParse(r["giaVe"].ToString(), out giaVe);
                        }

                        int soLuong = 0;
                        if (r.Table.Columns.Contains("soLuong") && r["soLuong"] != DBNull.Value)
                        {
                            int.TryParse(r["soLuong"].ToString(), out soLuong);
                        }

                        decimal thanhTien = 0m;
                        if (r.Table.Columns.Contains("thanhTien") && r["thanhTien"] != DBNull.Value)
                        {
                            decimal.TryParse(r["thanhTien"].ToString(), out thanhTien);
                        }
                        else
                        {
                            thanhTien = giaVe * soLuong;
                        }

                        ViewDatVe.Rows.Add(maVe, maKH, maCD, ngDat, trangThai, giaVe, soLuong, thanhTien);
                    }
                }
            }
        }
    
      private void LamMoiForm()
        {
            txtMaVe.Text = AutoMaVe();

            cbMaKH.SelectedIndex = -1;
            cbMaCD.SelectedIndex = -1;
            cbTrangThai.SelectedIndex = -1;

            dtpNgayDat.Value = DateTime.Now;
            txtGiaVe.Text = "0";
            numSoLuong.Value = 1;

            ViewDatVe.ClearSelection();
            ViewDatVe.CurrentCell = null;
        }



        private void ViewDatVe_SelectionChanged(object sender, EventArgs e)
        {
            if (ViewDatVe.CurrentRow == null || ViewDatVe.CurrentRow.IsNewRow) return;

            txtMaVe.Text = ViewDatVe.CurrentRow.Cells["maVe"].Value?.ToString();
            cbMaKH.Text = ViewDatVe.CurrentRow.Cells["maKH"].Value?.ToString();
            cbMaCD.Text = ViewDatVe.CurrentRow.Cells["maCD"].Value?.ToString();

            string ngDat = ViewDatVe.CurrentRow.Cells["ngDat"].Value?.ToString();
            if (DateTime.TryParse(ngDat, out DateTime d))
                dtpNgayDat.Value = d;

            cbTrangThai.Text = ViewDatVe.CurrentRow.Cells["trangThai"].Value?.ToString();

            txtGiaVe.Text = ViewDatVe.CurrentRow.Cells["giaVe"].Value?.ToString();

            if (int.TryParse(ViewDatVe.CurrentRow.Cells["soLuong"].Value?.ToString(), out int sl))
                numSoLuong.Value = sl;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbMaKH.Text) || string.IsNullOrWhiteSpace(cbMaCD.Text))
            {
                MessageBox.Show("Vui lòng chọn Mã Khách Hàng và Mã Chuyến Đi!");
                return;
            }

            string maVe = AutoMaVe();

            decimal giaVe = 0m;
            decimal.TryParse(txtGiaVe.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out giaVe);
            int sl = (int)numSoLuong.Value;
            decimal thanhTien = giaVe * sl;

            ViewDatVe.Rows.Add(
                maVe,
                cbMaKH.Text,                                 
                cbMaCD.Text,                                 
                dtpNgayDat.Value.ToString("yyyy-MM-dd"),     
                cbTrangThai.Text,                            
                giaVe,                                       
                sl,                                          
                thanhTien                                    
            );

            MessageBox.Show("Đã thêm thông tin đặt vé!");
            LamMoiForm();
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (ViewDatVe.CurrentRow == null)
            {
                MessageBox.Show("Chưa chọn thông tin đặt vé!");
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa vé này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            ViewDatVe.Rows.Remove(ViewDatVe.CurrentRow);
            MessageBox.Show("Đã xóa khỏi giao diện. Nhấn Lưu để cập nhật CSDL.");
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (ViewDatVe.CurrentRow == null)
            {
                MessageBox.Show("Chưa chọn vé!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMaVe.Text) || string.IsNullOrWhiteSpace(cbMaKH.Text) || string.IsNullOrWhiteSpace(cbMaCD.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu!");
                return;
            }

            DataGridViewRow row = ViewDatVe.CurrentRow;

            row.Cells["maVe"].Value = txtMaVe.Text;
            row.Cells["maKH"].Value = cbMaKH.Text;
            row.Cells["maCD"].Value = cbMaCD.Text;
            row.Cells["ngDat"].Value = dtpNgayDat.Value.ToString("yyyy-MM-dd");
            row.Cells["trangThai"].Value = cbTrangThai.Text;

            decimal giaVe = 0m;
            decimal.TryParse(txtGiaVe.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out giaVe);
            row.Cells["giaVe"].Value = giaVe;

            row.Cells["soLuong"].Value = (int)numSoLuong.Value;
            row.Cells["thanhTien"].Value = giaVe * (int)numSoLuong.Value;

            MessageBox.Show("Cập nhật thông tin vé thành công!");
            LamMoiForm();
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                List<string> dsMaVe = new List<string>();
                foreach (DataGridViewRow row in ViewDatVe.Rows)
                {
                    if (row.IsNewRow) continue;
                    var v = row.Cells["maVe"].Value;
                    if (v == null) continue;
                    var s = v.ToString().Trim();
                    if (!string.IsNullOrEmpty(s)) dsMaVe.Add(s);
                }

                if (dsMaVe.Count > 0)
                {
                    string placeholders = string.Join(",", dsMaVe.Select((v, i) => "@id" + i));
                    string sqlDel = "DELETE FROM DATVE WHERE maVe NOT IN (" + placeholders + ")";
                    using (SqlCommand cmd = new SqlCommand(sqlDel, conn))
                    {
                        for (int i = 0; i < dsMaVe.Count; i++)
                            cmd.Parameters.AddWithValue("@id" + i, dsMaVe[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM DATVE", conn))
                        cmd.ExecuteNonQuery();
                }

                string GetCellString(DataGridViewRow r, string col)
                {
                    var val = r.Cells[col].Value;
                    return val?.ToString().Trim() ?? string.Empty;
                }

                bool TryParseDate(string s, out DateTime dt)
                {
                    dt = DateTime.MinValue;
                    if (string.IsNullOrWhiteSpace(s)) return false;
                    
                    string[] fmts = { "dd/MM/yyyy", "yyyy-MM-dd", "M/d/yyyy", "MM/dd/yyyy" };
                    if (DateTime.TryParseExact(s, fmts, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
                        return true;
                    return DateTime.TryParse(s, out dt);
                }

                bool TryParseDecimal(string s, out decimal d)
                {
                    d = 0m;
                    if (string.IsNullOrWhiteSpace(s)) return false;
                    return decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d)
                        || decimal.TryParse(s, out d);
                }

                bool TryParseInt(string s, out int i)
                {
                    i = 0;
                    if (string.IsNullOrWhiteSpace(s)) return false;
                    return int.TryParse(s, out i);
                }

                foreach (DataGridViewRow row in ViewDatVe.Rows)
                {
                    if (row.IsNewRow) continue;

                    string maVe = GetCellString(row, "maVe");
                    if (string.IsNullOrEmpty(maVe)) continue; // skip invalid rows

                    string maKH = GetCellString(row, "maKH");
                    string maCD = GetCellString(row, "maCD");
                    string ngDatRaw = GetCellString(row, "ngDat");
                    string trangThai = GetCellString(row, "trangThai");
                    string giaVeRaw = GetCellString(row, "giaVe");
                    string soLuongRaw = GetCellString(row, "soLuong");
                    string thanhTienRaw = GetCellString(row, "thanhTien");

                    DateTime ngDat;
                    object ngDatParam = DBNull.Value;
                    if (TryParseDate(ngDatRaw, out ngDat))
                        ngDatParam = ngDat;

                    decimal giaVe;
                    if (!TryParseDecimal(giaVeRaw, out giaVe)) giaVe = 0m;

                    int soLuong;
                    if (!TryParseInt(soLuongRaw, out soLuong))
                    {
                        // sometimes numeric cell is of numeric type
                        var cellVal = row.Cells["soLuong"].Value;
                        if (cellVal is int) soLuong = (int)cellVal;
                        else if (cellVal is decimal) soLuong = Convert.ToInt32(cellVal);
                        else soLuong = 0;
                    }

                    decimal thanhTien;
                    if (!TryParseDecimal(thanhTienRaw, out thanhTien))
                    {
                        var cellVal = row.Cells["thanhTien"].Value;
                        if (cellVal is decimal) thanhTien = (decimal)cellVal;
                        else if (cellVal is double) thanhTien = Convert.ToDecimal((double)cellVal);
                        else thanhTien = giaVe * soLuong;
                    }

                    // Check existence
                    string qCheck = "SELECT COUNT(*) FROM DATVE WHERE maVe = @maVe";
                    using (SqlCommand cmd = new SqlCommand(qCheck, conn))
                    {
                        cmd.Parameters.AddWithValue("@maVe", maVe);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            string qUpdate = @"
                        UPDATE DATVE
                        SET maKH = @maKH,
                            maCD = @maCD,
                            ngDat = @ngDat,
                            trangThai = @trangThai,
                            giaVe = @giaVe,
                            soLuong = @soLuong,
                            thanhTien = @thanhTien
                        WHERE maVe = @maVe";
                            using (SqlCommand c = new SqlCommand(qUpdate, conn))
                            {
                                c.Parameters.AddWithValue("@maVe", maVe);
                                c.Parameters.AddWithValue("@maKH", string.IsNullOrEmpty(maKH) ? (object)DBNull.Value : maKH);
                                c.Parameters.AddWithValue("@maCD", string.IsNullOrEmpty(maCD) ? (object)DBNull.Value : maCD);
                                c.Parameters.AddWithValue("@ngDat", ngDatParam);
                                c.Parameters.AddWithValue("@trangThai", string.IsNullOrEmpty(trangThai) ? (object)DBNull.Value : trangThai);
                                c.Parameters.AddWithValue("@giaVe", giaVe);
                                c.Parameters.AddWithValue("@soLuong", soLuong);
                                c.Parameters.AddWithValue("@thanhTien", thanhTien);
                                c.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string qInsert = @"
                        INSERT INTO DATVE(maVe, maKH, maCD, ngDat, trangThai, giaVe, soLuong, thanhTien)
                        VALUES(@maVe, @maKH, @maCD, @ngDat, @trangThai, @giaVe, @soLuong, @thanhTien)";
                            using (SqlCommand c = new SqlCommand(qInsert, conn))
                            {
                                c.Parameters.AddWithValue("@maVe", maVe);
                                c.Parameters.AddWithValue("@maKH", string.IsNullOrEmpty(maKH) ? (object)DBNull.Value : maKH);
                                c.Parameters.AddWithValue("@maCD", string.IsNullOrEmpty(maCD) ? (object)DBNull.Value : maCD);
                                c.Parameters.AddWithValue("@ngDat", ngDatParam);
                                c.Parameters.AddWithValue("@trangThai", string.IsNullOrEmpty(trangThai) ? (object)DBNull.Value : trangThai);
                                c.Parameters.AddWithValue("@giaVe", giaVe);
                                c.Parameters.AddWithValue("@soLuong", soLuong);
                                c.Parameters.AddWithValue("@thanhTien", thanhTien);
                                c.ExecuteNonQuery();
                            }
                        }
                    }
                }

                conn.Close();
                MessageBox.Show("Đã lưu dữ liệu!");
                LoadData();
            }
        }
        private void btnHuy_Click(object sender, EventArgs e)
        {
            LamMoiForm();
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
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

    }
}

