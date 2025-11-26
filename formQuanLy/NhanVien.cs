using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formQuanLy
{
    public partial class formNhanVien : Form
    {
        private List<string> listChucVu = new List<string>()
        {
            "Cơ trưởng",
            "Hướng dẫn viên",
            "Nhân viên"
        };

    
        private List<string> danhSachXoa = new List<string>();
        
        public formNhanVien()
        {
            InitializeComponent();
            cmb_ChucVu.DataSource = listChucVu;

            
                ViewNhanVien.Columns.Add("maNV", "Mã NV");
                ViewNhanVien.Columns.Add("so_cccd", "Số CCCD");
                ViewNhanVien.Columns.Add("hoTen", "Họ Tên");
                ViewNhanVien.Columns.Add("chucVu", "Chức Vụ");
                ViewNhanVien.Columns.Add("phai", "Phái");
                ViewNhanVien.Columns.Add("ngSinh", "Ngày Sinh");
                ViewNhanVien.Columns.Add("sdt", "SĐT");
                ViewNhanVien.Columns.Add("dchi", "Địa Chỉ");
                ViewNhanVien.Columns.Add("soChuyen", "Số Chuyến");
                ViewNhanVien.Columns.Add("luong", "Lương");
                ViewNhanVien.Columns["soChuyen"].Visible = false;
                ViewNhanVien.Columns["luong"].Visible = false;



            ViewNhanVien.SelectionChanged += ViewNhanVien_SelectionChanged;


            btnThem.Click += btnThem_Click;
            btnXoa.Click += btnXoa_Click;
            btnSua.Click += btnSua_Click;
            btnLuu.Click += btnLuu_Click;
            btnXemLuong.Click += btnXemLuong_Click;
            btnThoat.Click += btnThoat_Click;
            btnHuy.Click += btnHuy_Click;       
            btnBack.Click += btnBack_Click;

            LamMoiForm();
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void ViewNhanVien_SelectionChanged(object sender, EventArgs e)
        {
            if (ViewNhanVien.SelectedRows.Count == 0)
                return;

            var row = ViewNhanVien.SelectedRows[0];

            if (row.IsNewRow) return; 

            txtMaNV.Text = row.Cells["maNV"].Value?.ToString();
            txtCCCD.Text = row.Cells["so_cccd"].Value?.ToString();
            txtTenNV.Text = row.Cells["hoTen"].Value?.ToString();
            cmb_ChucVu.Text = row.Cells["chucVu"].Value?.ToString();
            txtSDT.Text = row.Cells["sdt"].Value?.ToString();
            txtDiaChiNv.Text = row.Cells["dchi"].Value?.ToString();

            if (DateTime.TryParse(row.Cells["ngSinh"].Value?.ToString(), out DateTime ngSinh))
                dtpNamSinhNV.Value = ngSinh;

            string phai = row.Cells["phai"].Value?.ToString();
            rbNam.Checked = (phai == "Nam");
            rbNu.Checked = (phai == "Nữ");
        }

        private string AutoMaNV()
        {
            try
            {
                List<string> maListGrid = new List<string>();
                foreach (DataGridViewRow row in ViewNhanVien.Rows)
                {
                    if (row.IsNewRow) continue;
                    var val = row.Cells["maNV"].Value;
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
                    using (SqlCommand cmd = new SqlCommand("SELECT maNV FROM NHANVIEN", conn))
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
                        if (m.Length >= 3 && m.StartsWith("NV", StringComparison.OrdinalIgnoreCase))
                        {
                            var sub = m.Substring(2);
                            if (int.TryParse(sub, out parsed) && parsed > 0) candidates.Add(parsed);
                        }
                    }
                    if (candidates.Count > 0)
                    {
                        int reuse = candidates.Min();
                        return $"NV{reuse:0000}";
                    }
                }

                var maList = maListGrid.Union(maListDb).ToList();

                var used = new HashSet<int>();
                foreach (var m in maList)
                {
                    if (string.IsNullOrWhiteSpace(m)) continue;
                    if (m.Length >= 3 && m.StartsWith("NV", StringComparison.OrdinalIgnoreCase))
                    {
                        var sub = m.Substring(2);
                        if (int.TryParse(sub, out parsed) && parsed > 0) used.Add(parsed);
                    }
                }

                int candidate = 1;
                while (used.Contains(candidate)) candidate++;

                return $"NV{candidate:0000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AutoMaNV lỗi: " + ex.Message);
                return "NV0001";
            }
        }
        private void LoadData()
        {
            ViewNhanVien.Rows.Clear();

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT maNV, so_cccd, hoTen, chucVu, phai, ngSinh, sdt, dchi FROM NHANVIEN", conn))
                    {
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string ma = r.GetString(0);
                                string soCCCD = r.GetString(1);
                                string hoTen = r.GetString(2);
                                string chucVu = r.GetString(3);
                                string phai = r.GetString(4);
                                DateTime ngSinh = r.GetDateTime(5);
                                string sdt = r.GetString(6);
                                string dchi = r.GetString(7);
                                ViewNhanVien.Rows.Add(ma, soCCCD, hoTen, chucVu, phai, ngSinh.ToString("yyyy-MM-dd"), sdt, dchi);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadData lỗi: " + ex.Message);
            }
        }
        private void LamMoiForm()
        {
            txtMaNV.ReadOnly = true;
            txtMaNV.Text = AutoMaNV();
            txtCCCD.Text = "";
            txtTenNV.Text = "";
            cmb_ChucVu.SelectedIndex = -1;
            cmb_ChucVu.Text = "";
            rbNam.Checked = true;
            dtpNamSinhNV.Value = DateTime.Now;
            txtSDT.Text = "";
            txtDiaChiNv.Text = "";
          
        }
        private bool KiemTraSoDienThoai(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;
            string pattern = @"^(0\d{9}|(\+84)\d{9})$";
            return Regex.IsMatch(sdt, pattern);
        }
         private bool KiemTraCCCD(string cccd)
         {
             if (string.IsNullOrWhiteSpace(cccd)) return false;
             string pattern = @"^\d{12}$";
             return Regex.IsMatch(cccd, pattern);
         }
        public static bool IsUnique(string value, string columnName, DataGridView dgv)
        {

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                var cellValue = row.Cells[columnName]?.Value?.ToString();
                if (string.Equals(cellValue, value, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            string ma = AutoMaNV();
            string soCCCD = txtCCCD.Text?.Trim() ?? "";
            string hoTen = txtTenNV.Text?.Trim() ?? "";
            string chucVu = cmb_ChucVu.Text?.Trim() ?? "";
            string phai = rbNam.Checked ? "Nam" : "Nữ";
            DateTime ngSinh = dtpNamSinhNV.Value;
            string sdt = txtSDT.Text?.Trim() ?? "";
            string diaChi = txtDiaChiNv.Text?.Trim() ?? "";


            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(soCCCD)|| string.IsNullOrEmpty(sdt))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string sodt = txtSDT.Text.Trim();
                string cccd = txtCCCD.Text.Trim();

                if (!KiemTraSoDienThoai(sodt))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập lại!",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!IsUnique(sodt, "sdt", ViewNhanVien))
                {
                    MessageBox.Show("Số điện thoại đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!KiemTraCCCD(cccd))
                {
                    MessageBox.Show("Số CCCD không hợp lệ. Vui lòng nhập lại!",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; 
                }
                if (!IsUnique(cccd, "soCCCD", ViewNhanVien))
                {
                    MessageBox.Show("Số cccd đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ViewNhanVien.Rows.Add(ma, soCCCD, hoTen, chucVu, phai, ngSinh.ToString("yyyy-MM-dd"), sdt, diaChi);
                MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
            }
             catch(Exception ex) 
            {
                MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
         
            

        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNV.Text))
            {
                MessageBox.Show("Chọn nhân viên để xóa!");

                return;
            }
            if (ViewNhanVien.CurrentRow != null)
            {
                string ma = ViewNhanVien.CurrentRow.Cells["maNV"].Value?.ToString();

                if (!string.IsNullOrEmpty(ma))
                {
                    danhSachXoa.Add(ma); // LƯU LẠI MÃ ĐỂ UPDATE SAU
                }

                ViewNhanVien.Rows.Remove(ViewNhanVien.CurrentRow); // XÓA KHỎI GRID
            }

        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (ViewNhanVien.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để sửa!", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sel = ViewNhanVien.SelectedRows[0];
            string ma = sel.Cells["maNV"].Value?.ToString().Trim();
            string hoTen = txtTenNV.Text?.Trim() ?? "";
            string soCCCD = txtCCCD.Text?.Trim() ?? "";
            string chucVu = cmb_ChucVu.Text?.Trim() ?? "";
            string phai = rbNam.Checked ? "Nam" : "Nữ";
            DateTime ngSinh = dtpNamSinhNV.Value;
            string sdt = txtSDT.Text?.Trim() ?? "";
            string diaChi = txtDiaChiNv.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(soCCCD))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ họ tên và số CCCD!", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sel.Cells["hoTen"].Value = hoTen;
            sel.Cells["so_cccd"].Value = soCCCD;
            sel.Cells["chucVu"].Value = chucVu;
            sel.Cells["phai"].Value = phai;
            sel.Cells["ngSinh"].Value = ngSinh.ToString("yyyy-MM-dd");
            sel.Cells["sdt"].Value = sdt;
            sel.Cells["dchi"].Value = diaChi;

            try
            {
                MessageBox.Show("Đã cập nhật thông tin nhân viên!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm(); // refresh form nếu cần
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    foreach (string maXoa in danhSachXoa)
                    {
                        using (SqlCommand cmdXoa = new SqlCommand(
                            "DELETE FROM NHANVIEN WHERE maNV = @ma", conn))
                        {
                            cmdXoa.Parameters.AddWithValue("@ma", maXoa);
                            cmdXoa.ExecuteNonQuery();
                        }
                    }

                    danhSachXoa.Clear();

                    foreach (DataGridViewRow row in ViewNhanVien.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string maNV = row.Cells["maNV"].Value?.ToString()?.Trim();
                        string soCCCD = row.Cells["so_cccd"].Value?.ToString()?.Trim();
                        string hoTen = row.Cells["hoTen"].Value?.ToString()?.Trim();
                        string chucVu = row.Cells["chucVu"].Value?.ToString()?.Trim();
                        string phai = row.Cells["phai"].Value?.ToString()?.Trim();
                        string ngSinhStr = row.Cells["ngSinh"].Value?.ToString()?.Trim();
                        string sdt = row.Cells["sdt"].Value?.ToString()?.Trim();
                        string dchi = row.Cells["dchi"].Value?.ToString()?.Trim();

                        DateTime ngSinh = DateTime.Now;
                        DateTime.TryParse(ngSinhStr, out ngSinh);

                        using (SqlCommand cmdCheck = new SqlCommand(
                            "SELECT COUNT(1) FROM NHANVIEN WHERE maNV = @ma", conn))
                        {
                            cmdCheck.Parameters.AddWithValue("@ma", maNV);
                            int exists = Convert.ToInt32(cmdCheck.ExecuteScalar());

                            // ---------- INSERT ----------
                            if (exists == 0)
                            {
                                using (SqlCommand cmdInsert = new SqlCommand(
                                    @"INSERT INTO NHANVIEN (maNV, so_cccd, hoTen, chucVu, phai, ngSinh, sdt, dchi)
                              VALUES (@ma, @soCCCD, @hoTen, @chucVu, @phai, @ngSinh, @sdt, @dchi)", conn))
                                {
                                    cmdInsert.Parameters.AddWithValue("@ma", maNV);
                                    cmdInsert.Parameters.AddWithValue("@soCCCD", soCCCD);
                                    cmdInsert.Parameters.AddWithValue("@hoTen", hoTen);
                                    cmdInsert.Parameters.AddWithValue("@chucVu", chucVu);
                                    cmdInsert.Parameters.AddWithValue("@phai", phai);
                                    cmdInsert.Parameters.AddWithValue("@ngSinh", ngSinh);
                                    cmdInsert.Parameters.AddWithValue("@sdt", sdt);
                                    cmdInsert.Parameters.AddWithValue("@dchi", dchi);
                                    cmdInsert.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand cmdUpdate = new SqlCommand(
                                    @"UPDATE NHANVIEN SET  
                                so_cccd = @soCCCD,
                                hoTen = @hoTen,
                                chucVu = @chucVu,
                                phai = @phai,
                                ngSinh = @ngSinh,
                                sdt = @sdt,
                                dchi = @dchi
                              WHERE maNV = @ma", conn))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@soCCCD", soCCCD);
                                    cmdUpdate.Parameters.AddWithValue("@hoTen", hoTen);
                                    cmdUpdate.Parameters.AddWithValue("@chucVu", chucVu);
                                    cmdUpdate.Parameters.AddWithValue("@phai", phai);
                                    cmdUpdate.Parameters.AddWithValue("@ngSinh", ngSinh);
                                    cmdUpdate.Parameters.AddWithValue("@sdt", sdt);
                                    cmdUpdate.Parameters.AddWithValue("@dchi", dchi);
                                    cmdUpdate.Parameters.AddWithValue("@ma", maNV);
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Đã lưu dữ liệu nhân viên thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }
        private void btnXemLuong_Click(object sender, EventArgs e)
        {
            int thang = dtp_xemLuong.Value.Month;
            int nam = dtp_xemLuong.Value.Year;

            if (!ViewNhanVien.Columns.Contains("soChuyen"))
                ViewNhanVien.Columns.Add("soChuyen", "Số Chuyến");
            if (!ViewNhanVien.Columns.Contains("luong"))
                ViewNhanVien.Columns.Add("luong", "Lương");

            ViewNhanVien.Columns["soChuyen"].Visible = true;
            ViewNhanVien.Columns["luong"].Visible = true;

            ViewNhanVien.Rows.Clear();

            List<(string maNV, string soCCCD, string hoTen, string sdt, string phai, DateTime ngSinh, string dchi, string chucVu)> nhanviens
                = new List<(string, string, string, string, string, DateTime, string, string)>();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "SELECT maNV, so_cccd, hoTen, sdt, phai, ngSinh, dchi, chucVu FROM NHANVIEN", conn))
                {
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            nhanviens.Add((
                                r.GetString(0),
                                r.GetString(1),
                                r.GetString(2),
                                r.GetString(3),
                                r.GetString(4),
                                r.GetDateTime(5),
                                r.GetString(6),
                                r.GetString(7)
                            ));
                        }
                    }
                }

                Dictionary<string, int> chuyens = new Dictionary<string, int>();
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT cn.maNV, COUNT(*) 
                    FROM CHUYENDI_NHANVIEN cn
                    JOIN CHUYENDI c ON cn.maCD = c.maCD
                    WHERE MONTH(c.ngKh)=@Thang AND YEAR(c.ngKh)=@Nam
                    GROUP BY cn.maNV", conn))
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            chuyens[r.GetString(0)] = r.GetInt32(1);
                        }
                    }
                }

                foreach (var nv in nhanviens)
                {
                    int soChuyen = chuyens.ContainsKey(nv.maNV) ? chuyens[nv.maNV] : 0;

                    int luong_cb = 1200000; 
                    if (nv.chucVu.Equals("Cơ trưởng", StringComparison.OrdinalIgnoreCase))
                        luong_cb = 2000000;
                    else if (nv.chucVu.Equals("Hướng dẫn viên", StringComparison.OrdinalIgnoreCase))
                        luong_cb = 1800000;

                    int luong_thuc = luong_cb * soChuyen;

                    ViewNhanVien.Rows.Add(
                        nv.maNV,
                        nv.soCCCD.PadLeft(12, '0'),
                        nv.hoTen,
                        nv.chucVu,
                        nv.phai,
                        nv.ngSinh.ToString("yyyy-MM-dd"),
                        nv.sdt.PadLeft(10, '0'),
                        nv.dchi,
                        soChuyen,
                        luong_thuc
                    );
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnHuy_Click(object sender, EventArgs e)
        {
            LamMoiForm();
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
