using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formQuanLy
{
    public partial class formTuyenDuLich : Form
    {

        private List<string> danhSachXoa = new List<string>();

        private List<string> list_di = new List<string> { "Hà Nội", "Đà Nẵng", "Hồ Chí Minh" };
        private List<string> mien_bac = new List<string>
        {
            "Tràng An-Tam Cốc-Bái Đính-Hoa Lư",
            "Mộc Châu-Điện Biên-Sapa",
            "Hà Giang-Cao Bằng",
            "Ninh Bình-Hạ Long-Sapa",
            "Vịnh Hạ Long-Hạ Long Park-Bãi Cháy-Yên Tử"
        };
        private List<string> mien_trung = new List<string>
        {
            "Quy Nhơn-Phú Yên",
            "Pleiku-Kontum- Măng Đen",
            "Hội An-Bà Nà Hills-Núi Thần Tài",
            "Phan Thiết-Mũi Né",
            "Quảng Bình-Suối Moọc- Động Thiên Đường"
        };
        private List<string> mien_nam = new List<string>
        {
            "Tiền Giang-Bến Tre- An Giang-Cần Thơ",
            "Vũng Tàu",
            "Phú Quốc",
            "Tây Ninh"
        };
        public formTuyenDuLich()
        {
            InitializeComponent();

            if (ViewTuyenDuLich.Columns.Count == 0)
            {
                ViewTuyenDuLich.Columns.Add("maTuyen", "Mã Tuyến");
                ViewTuyenDuLich.Columns.Add("ddDi", "Địa Điểm Đi");
                ViewTuyenDuLich.Columns.Add("ddDen", "Địa Điểm Đến");
                ViewTuyenDuLich.Columns.Add("trangThai", "Trạng Thái");
                ViewTuyenDuLich.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                ViewTuyenDuLich.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                ViewTuyenDuLich.ReadOnly = false; 
            }

            cmb_ddDi.Items.AddRange(list_di.ToArray());
            cmb_ddDi.SelectedIndexChanged += Cmb_ddDi_SelectedIndexChanged;
            ViewTuyenDuLich.SelectionChanged += ViewTuyenDuLich_SelectionChanged;
         
            btnThem.Click += BtnThem_Click;
            btnXoa.Click += BtnXoa_Click;
            btnSua.Click += BtnSua_Click;
            btnHuy.Click += BtnHuy_Click;
            btnLuu.Click += BtnLuu_Click;
            btnThoat.Click += BtnThoat_Click;
            btnBack.Click += btnBack_Click;

            LoadData();
            LamMoiForm();
           
        }

        private void PicTuyenDuLich_Click(object sender, EventArgs e)
        {

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
        private void BtnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void BtnThem_Click(object sender, EventArgs e) => Them();
        private void BtnXoa_Click(object sender, EventArgs e) => Xoa();  
        private void BtnSua_Click(object sender, EventArgs e) => Sua();  
        private void BtnHuy_Click(object sender, EventArgs e) => Huy();  
        private void BtnLuu_Click(object sender, EventArgs e) => Luu();   
        private string AutoMaTuyen()
        {
            try
            {
                List<string> maListGrid = new List<string>();
                foreach (DataGridViewRow row in ViewTuyenDuLich.Rows)
                {
                    if (row.IsNewRow) continue;
                    var val = row.Cells["maTuyen"].Value;
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
                    using (SqlCommand cmd = new SqlCommand("SELECT maTuyen FROM TUYENDULICH", conn))
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
                var maList = maListGrid.Union(maListDb).ToList();

                if (maList.Count == 0)
                    return "TD0001";

                var numbers = maList
                    .Where(m => m.Length >= 4 && m.StartsWith("TD"))
                    .Select(m =>
                    {
                        var sub = m.Substring(2);
                        return int.TryParse(sub, out int n) ? n : -1;
                    })
                    .Where(n => n >= 0)
                    .OrderBy(n => n)
                    .ToList();

                int next = (numbers.Count == 0) ? 1 : numbers.Last() + 1;
                return $"TD{next:0000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AutoMaTuyen lỗi: " + ex.Message);
                return "TD0001";
            }
        }
        private void LoadData()
        {
            
            ViewTuyenDuLich.Rows.Clear();

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT maTuyen, ddDi, ddDen, trangThai FROM TUYENDULICH WHERE trangThai = N'Hoạt động'", conn))
                    {
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string ma = r.GetString(0).Trim();
                                string ddDi = r.GetString(1).Trim();
                                string ddDen = r.GetString(2).Trim();
                                string tt = r.IsDBNull(3) ? "" : r.GetString(3).Trim();
                                ViewTuyenDuLich.Rows.Add(ma, ddDi, ddDen, tt);
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
            txtmaTuyen.ReadOnly = true;
            txtmaTuyen.Text = AutoMaTuyen();
            cmb_ddDi.SelectedIndex = -1;
            cmb_ddDen.Items.Clear();
            cmb_ddDen.Text = "";
        }
        private void CapNhat_ddDen()
        {
            string di = cmb_ddDi.Text?.Trim() ?? "";
            List<string> denList;

            if (di == "Hà Nội") denList = new List<string>(mien_bac);
            else if (di == "Đà Nẵng") denList = new List<string>(mien_trung);
            else if (di == "Hồ Chí Minh") denList = new List<string>(mien_nam);
            else denList = new List<string>();

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ddDen FROM TUYENDULICH WHERE ddDi = @di AND trangThai = N'Hoạt động'", conn))
                    {
                        cmd.Parameters.AddWithValue("@di", di);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                var s = r.GetString(0);
                                denList.RemoveAll(d => d.Equals(s, StringComparison.OrdinalIgnoreCase));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CapNhat_ddDen DB read lỗi: " + ex.Message);
            }

            foreach (DataGridViewRow row in ViewTuyenDuLich.Rows)
            {
                if (row.IsNewRow) continue;
                var ddDiCell = row.Cells["ddDi"].Value;
                var ddDenCell = row.Cells["ddDen"].Value;
                if (ddDiCell != null && ddDenCell != null)
                {
                    if (ddDiCell.ToString().Trim().Equals(di, StringComparison.OrdinalIgnoreCase))
                    {
                        denList.RemoveAll(d => d.Equals(ddDenCell.ToString().Trim(), StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            cmb_ddDen.BeginUpdate();
            cmb_ddDen.Items.Clear();
            foreach (var d in denList) cmb_ddDen.Items.Add(d);
            cmb_ddDen.EndUpdate();
            cmb_ddDen.Text = "";
        }
        private void Them()
        {
            string ma = AutoMaTuyen();
            string di = cmb_ddDi.Text?.Trim() ?? "";
            string den = cmb_ddDen.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(di) || string.IsNullOrEmpty(den))
            {
                MessageBox.Show("Vui lòng chọn địa điểm đi và đến", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Thêm vào DataGridView (chưa lưu DB)
            ViewTuyenDuLich.Rows.Add(ma, di, den, "Hoạt động");
            LamMoiForm();
        }
        private void Xoa()
        {
            if (ViewTuyenDuLich.CurrentRow != null)
            {
                string ma = ViewTuyenDuLich.CurrentRow.Cells["maTuyen"].Value?.ToString();

                if (!string.IsNullOrEmpty(ma))
                {
                    danhSachXoa.Add(ma); 
                }

                ViewTuyenDuLich.Rows.Remove(ViewTuyenDuLich.CurrentRow); 
            }
            LamMoiForm();
        }
        private void Sua()
        {
            if (ViewTuyenDuLich.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tuyến để sửa!", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sel = ViewTuyenDuLich.SelectedRows[0];
            string maTuyen = sel.Cells["maTuyen"].Value?.ToString().Trim();
            string ddDi = cmb_ddDi.Text?.Trim() ?? "";
            string ddDen = cmb_ddDen.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(ddDi) || string.IsNullOrEmpty(ddDen))
            {
                MessageBox.Show("Vui lòng chọn đầy đủ địa điểm đi và đến!", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sel.Cells["ddDi"].Value = ddDi;
            sel.Cells["ddDen"].Value = ddDen;
            sel.Cells["trangThai"].Value = "Hoạt động";

            try
            { 
                MessageBox.Show("Đã cập nhật thông tin tuyến du lịch!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Luu()
        {
            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    foreach (string maXoa in danhSachXoa)
                    {
                        using (SqlCommand cmdXoa = new SqlCommand(
                            "UPDATE TUYENDULICH SET trangThai = N'Đã xóa' WHERE maTuyen = @ma", conn))
                        {
                            cmdXoa.Parameters.AddWithValue("@ma", maXoa);
                            cmdXoa.ExecuteNonQuery();
                        }
                    }

                    danhSachXoa.Clear();

                    foreach (DataGridViewRow row in ViewTuyenDuLich.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string maTuyen = row.Cells["maTuyen"].Value?.ToString()?.Trim();
                        string ddDi = row.Cells["ddDi"].Value?.ToString()?.Trim();
                        string ddDen = row.Cells["ddDen"].Value?.ToString()?.Trim();
                        string tt = row.Cells["trangThai"].Value?.ToString()?.Trim() ?? "Hoạt động";

                        using (SqlCommand cmdCheck = new SqlCommand(
                            "SELECT COUNT(1) FROM TUYENDULICH WHERE maTuyen = @ma", conn))
                        {
                            cmdCheck.Parameters.AddWithValue("@ma", maTuyen);
                            int exists = Convert.ToInt32(cmdCheck.ExecuteScalar());

                            if (exists == 0)
                            {
                                using (SqlCommand cmdInsert = new SqlCommand(
                                    "INSERT INTO TUYENDULICH (maTuyen, ddDi, ddDen, trangThai) VALUES (@ma, @ddDi, @ddDen, @tt)", conn))
                                {
                                    cmdInsert.Parameters.AddWithValue("@ma", maTuyen);
                                    cmdInsert.Parameters.AddWithValue("@ddDi", ddDi);
                                    cmdInsert.Parameters.AddWithValue("@ddDen", ddDen);
                                    cmdInsert.Parameters.AddWithValue("@tt", tt);
                                    cmdInsert.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand cmdUpdate = new SqlCommand(
                                    "UPDATE TUYENDULICH SET ddDi = @ddDi, ddDen = @ddDen, trangThai = @tt WHERE maTuyen = @ma", conn))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@ddDi", ddDi);
                                    cmdUpdate.Parameters.AddWithValue("@ddDen", ddDen);
                                    cmdUpdate.Parameters.AddWithValue("@tt", tt);
                                    cmdUpdate.Parameters.AddWithValue("@ma", maTuyen);
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Đã lưu dữ liệu thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }

        private void Huy()
        {
            LamMoiForm();
        }
        private void Cmb_ddDi_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhat_ddDen();
        }

        private void ViewTuyenDuLich_SelectionChanged(object sender, EventArgs e)
        {
            if (ViewTuyenDuLich.SelectedRows.Count > 0)
            {
                var r = ViewTuyenDuLich.SelectedRows[0];
                txtmaTuyen.Text = r.Cells["maTuyen"].Value?.ToString();
                cmb_ddDi.Text = r.Cells["ddDi"].Value?.ToString();
                CapNhat_ddDen();
                cmb_ddDen.Text = r.Cells["ddDen"].Value?.ToString();
            }
            else
            {
                txtmaTuyen.Text = AutoMaTuyen();
            }
        }

    }
}
