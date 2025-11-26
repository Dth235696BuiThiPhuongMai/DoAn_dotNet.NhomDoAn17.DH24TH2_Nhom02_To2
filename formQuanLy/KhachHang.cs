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
    public partial class formKhachHang : Form
    {
        public formKhachHang()
        {
            InitializeComponent();

            if (dgvKH.Columns.Count == 0)
            {
                dgvKH.Columns.Add("maKH", "Mã Khách Hàng");
                dgvKH.Columns.Add("tenKH", "Tên Khách Hàng");
                dgvKH.Columns.Add("gioi", "Giới Tính");   
                dgvKH.Columns.Add("ngsinh", "Ngày Sinh"); 
                dgvKH.Columns.Add("sdtKH", "Số Điện Thoại");
                dgvKH.Columns.Add("diachi", "Địa Chỉ");
            }

            
            dgvKH.SelectionChanged += dgvKH_SelectionChanged;

            btnThem.Click += btnThem_Click;
            btnXoa.Click += btnXoa_Click;
            btnSua.Click += btnSua_Click;
            btnHuy.Click += btnHuy_Click;
            btnLuu.Click += btnLuu_Click;
            btnThoat.Click += btnThoat_Click;
            btnBack.Click += btnBack_Click;

            LamMoiForm();
            LoadData();
        }

        private string AutoMaKH()
        {
            try
            {
                List<string> maListGrid = new List<string>();
                foreach (DataGridViewRow row in dgvKH.Rows)
                {
                    if (row.IsNewRow) continue;
                    var val = row.Cells["maKH"].Value;
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
                    using (SqlCommand cmd = new SqlCommand("SELECT maKH FROM KHACHHANG", conn))
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
                        if (m.Length >= 3 && m.StartsWith("KH", StringComparison.OrdinalIgnoreCase))
                        {
                            var sub = m.Substring(2);
                            if (int.TryParse(sub, out parsed) && parsed > 0) candidates.Add(parsed);
                        }
                    }
                    if (candidates.Count > 0)
                    {
                        int reuse = candidates.Min();
                        return $"KH{reuse:0000}";
                    }
                }

                var maList = maListGrid.Union(maListDb).ToList();

                var used = new HashSet<int>();
                foreach (var m in maList)
                {
                    if (string.IsNullOrWhiteSpace(m)) continue;
                    if (m.Length >= 3 && m.StartsWith("KH", StringComparison.OrdinalIgnoreCase))
                    {
                        var sub = m.Substring(2);
                        if (int.TryParse(sub, out parsed) && parsed > 0) used.Add(parsed);
                    }
                }

                int candidate = 1;
                while (used.Contains(candidate)) candidate++;

                return $"KH{candidate:0000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AutoMaKH lỗi: " + ex.Message);
                return "KH0001";
            }
        }
        private void LoadData()
        {
            using (SqlConnection conn = Database.GetConnection())
            {

                dgvKH.Rows.Clear();

                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT maKH, hoTen, phai, ngsinh, sdt, dchi
                    FROM KHACHHANG", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    string maKH = r["maKH"].ToString();
                    string tenKH = r["hoTen"].ToString();
                    string phai = r["phai"].ToString();
                    string NamSinhKH = Convert.ToDateTime(r["ngsinh"]).ToString("yyyy-MM-dd");
                    string sdt = r["sdt"].ToString();
                    string diaChi = r["dchi"].ToString();
                    
                    dgvKH.Rows.Add(maKH, tenKH, phai, NamSinhKH, sdt, diaChi);
                }
            }
        }

        private void LamMoiForm()
        {
            txtMaKH.Text = AutoMaKH();
            txtTenKH.Text = "";
            string gioi = rbNam.Checked ? "Nam" : "Nữ";
            dtpNamSinhKH.Value = DateTime.Now;
            txtSDTKH.Text = "";
            txtDiaChiKH.Text = "";
        }

        private void dgvKH_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKH.SelectedRows.Count > 0)
            {
                var r = dgvKH.SelectedRows[0];

                txtMaKH.Text = r.Cells["maKH"].Value?.ToString();
                txtTenKH.Text = r.Cells["tenKH"].Value?.ToString();

                var ngVal = r.Cells["ngsinh"].Value?.ToString();
                if (!string.IsNullOrEmpty(ngVal) && DateTime.TryParse(ngVal, out DateTime ngsinh))
                    dtpNamSinhKH.Value = ngsinh;
                else
                    dtpNamSinhKH.Value = DateTime.Now;

                txtSDTKH.Text = r.Cells["sdtKH"].Value?.ToString();
                txtDiaChiKH.Text = r.Cells["diachi"].Value?.ToString();

                var phai = r.Cells["gioi"].Value?.ToString();
                if (string.Equals(phai, "Nam", StringComparison.OrdinalIgnoreCase)) rbNam.Checked = true;
                else if (string.Equals(phai, "Nữ", StringComparison.OrdinalIgnoreCase) || string.Equals(phai, "Nu", StringComparison.OrdinalIgnoreCase)) rbNu.Checked = true;
            }
            else
            {
                txtMaKH.Text = AutoMaKH();
                txtTenKH.Text = "";
                rbNam.Checked = true;
                dtpNamSinhKH.Value = DateTime.Now;
                txtSDTKH.Text = "";
                txtDiaChiKH.Text = "";
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
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
        private bool KiemTraSoDienThoai(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;

            string pattern = @"^(0\d{9}|(\+84)\d{9})$";
            return Regex.IsMatch(sdt, pattern);

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
            if (string.IsNullOrEmpty(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu!");
                return;
            }
            try
            {
                string sodt = txtSDTKH.Text.Trim();


                if (!KiemTraSoDienThoai(sodt))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập lại!",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; 
                }
                if (!IsUnique(sodt, "sdtKH", dgvKH))
                {
                    MessageBox.Show("Số điện thoại đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtMaKH.Text = AutoMaKH();
                string gioi = rbNam.Checked ? "Nam" : "Nữ";
                
                dgvKH.Rows.Add(
                    txtMaKH.Text,
                    txtTenKH.Text,
                    gioi,
                    dtpNamSinhKH.Value.ToString("yyyy-MM-dd"),
                    txtSDTKH.Text,
                    txtDiaChiKH.Text
                    );
                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
            }
            catch (Exception ex)
            {
                    MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            
            if (dgvKH.CurrentRow == null)
            {
                MessageBox.Show("Chưa chọn khách hàng!");
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa hàng này (tạm thời)?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            dgvKH.Rows.Remove(dgvKH.CurrentRow);
            MessageBox.Show("Đã xóa khỏi giao diện. Nhấn Lưu để cập nhật CSDL.");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            
            if (dgvKH.CurrentRow == null)
            {
                MessageBox.Show("Chưa chọn khách hàng để sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                MessageBox.Show("Mã khách hàng không hợp lệ.");
                return;
            }

            DataGridViewRow row = dgvKH.CurrentRow;

            row.Cells["maKH"].Value = txtMaKH.Text.Trim();
            row.Cells["tenKH"].Value = txtTenKH.Text.Trim();
            row.Cells["gioi"].Value = rbNam.Checked ? "Nam" : "Nữ";
            row.Cells["ngsinh"].Value = dtpNamSinhKH.Value.ToString("yyyy-MM-dd");
            row.Cells["sdtKH"].Value = txtSDTKH.Text.Trim();
            row.Cells["diachi"].Value = txtDiaChiKH.Text.Trim();

            MessageBox.Show("Đã cập nhật trên giao diện. Nhấn Lưu để lưu thay đổi vào CSDL.");
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                List<string> dbIds = new List<string>();
                using (SqlCommand cmd = new SqlCommand("SELECT maKH FROM KHACHHANG", conn))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dbIds.Add(rdr.GetString(0));
                    }
                }

                List<string> gridIds = new List<string>();
                foreach (DataGridViewRow row in dgvKH.Rows)
                {
                    if (row.IsNewRow) continue;
                    var id = row.Cells["maKH"].Value?.ToString();
                    if (!string.IsNullOrEmpty(id)) gridIds.Add(id);
                }

                var toDelete = dbIds.Except(gridIds).ToList();
                foreach (var id in toDelete)
                {
                    using (SqlCommand del = new SqlCommand("DELETE FROM KHACHHANG WHERE maKH=@maKH", conn))
                    {
                        del.Parameters.AddWithValue("@maKH", id);
                        del.ExecuteNonQuery();
                    }
                }

                foreach (DataGridViewRow row in dgvKH.Rows)
                {
                    if (row.IsNewRow) continue;

                    string maKH = row.Cells["maKH"].Value?.ToString() ?? "";
                    string tenKH = row.Cells["tenKH"].Value?.ToString() ?? "";
                    string gioi = row.Cells["gioi"].Value?.ToString() ?? "";
                    string sdt = row.Cells["sdtKH"].Value?.ToString() ?? "";
                    string ngsinhStr = row.Cells["ngsinh"].Value?.ToString() ?? "";
                    string dchi = row.Cells["diachi"].Value?.ToString() ?? "";

                    DateTime ngsinh;
                    if (!DateTime.TryParse(ngsinhStr, out ngsinh))
                    {
                        ngsinh = DateTime.Now;
                    }

                    string qCheck = "SELECT COUNT(*) FROM KHACHHANG WHERE maKH=@maKH";
                    using (SqlCommand cmd = new SqlCommand(qCheck, conn))
                    {
                        cmd.Parameters.AddWithValue("@maKH", maKH);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            string qUpdate = "UPDATE KHACHHANG SET hoTen=@hoTen, phai=@phai, ngsinh=@ngsinh, sdt=@sdt, dchi=@dchi WHERE maKH=@maKH";
                            using (SqlCommand c = new SqlCommand(qUpdate, conn))
                            {
                                c.Parameters.AddWithValue("@maKH", maKH);
                                c.Parameters.AddWithValue("@hoTen", tenKH);
                                c.Parameters.AddWithValue("@phai", gioi);
                                c.Parameters.AddWithValue("@ngsinh", ngsinh);
                                c.Parameters.AddWithValue("@sdt", sdt);
                                c.Parameters.AddWithValue("@dchi", dchi);
                                c.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string qInsert = "INSERT INTO KHACHHANG(maKH, hoTen, phai, ngsinh, sdt, dchi) VALUES(@maKH, @hoTen, @phai, @ngsinh, @sdt, @dchi)";
                            using (SqlCommand c = new SqlCommand(qInsert, conn))
                            {
                                c.Parameters.AddWithValue("@maKH", maKH);
                                c.Parameters.AddWithValue("@hoTen", tenKH);
                                c.Parameters.AddWithValue("@phai", gioi);
                                c.Parameters.AddWithValue("@ngsinh", ngsinh);
                                c.Parameters.AddWithValue("@sdt", sdt);
                                c.Parameters.AddWithValue("@dchi", dchi);
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
    }
}