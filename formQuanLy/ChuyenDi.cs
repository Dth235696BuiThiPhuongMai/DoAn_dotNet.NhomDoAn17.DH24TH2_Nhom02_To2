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
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace formQuanLy
{
    public partial class formChuyenDi : Form
    {
        public formChuyenDi()
        {
            InitializeComponent();

            listNhanVien.SelectionMode = SelectionMode.MultiSimple;

            if (dataChuyenDi.Columns.Count == 0)
            {
                dataChuyenDi.Columns.Add("maCD", "Mã Chuyến Đi");
                dataChuyenDi.Columns.Add("maTuyen", "Mã Tuyến");
                dataChuyenDi.Columns.Add("ngKh", "Ngày Khởi Hành");
                dataChuyenDi.Columns.Add("tgKh", "Thời Gian Khởi Hành");

                dataChuyenDi.Columns.Add("nhanVien", "Nhân Viên");
            }

            dataChuyenDi.CellClick += dataChuyenDi_CellClick;

            btnThem.Click += btnThem_Click;
            btnXoa.Click += btnXoa_Click;
            btnSua.Click += btnSua_Click;
            btnHuy.Click += btnHuy_Click;
            btnLuu.Click += btnLuu_Click;
            btnThoat.Click += btnThoat_Click;
            btnBack.Click += btnBack_Click;

            dtpNgayKH.ValueChanged += (s, e) => LoadNhanVienRanh();

            LoadMaTuyen();
            LoadData();          
            LoadNhanVienRanh();
            LamMoiForm();

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

        private string AutoMaCD()
        {
            try
            {
                List<string> maListGrid = new List<string>();
                foreach (DataGridViewRow row in dataChuyenDi.Rows)
                {
                    if (row.IsNewRow) continue;
                    var val = row.Cells["maCD"].Value;
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
                    using (SqlCommand cmd = new SqlCommand("SELECT maCD FROM CHUYENDI", conn))
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
                    return "CD0001";

                var numbers = maList
                    .Where(m => m.Length >= 4 && m.StartsWith("CD"))
                    .Select(m =>
                    {
                        var sub = m.Substring(2);
                        return int.TryParse(sub, out int n) ? n : -1;
                    })
                    .Where(n => n >= 0)
                    .OrderBy(n => n)
                    .ToList();

                int next = (numbers.Count == 0) ? 1 : numbers.Last() + 1;
                return $"CD{next:0000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AutoMaTuyen lỗi: " + ex.Message);
                return "CD0001";
            }
        }
        private void LoadMaTuyen()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string sql = "SELECT maTuyen FROM TUYENDULICH WHERE trangThai = N'Hoạt động'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmb_maTuyen.Items.Clear();

                    foreach (DataRow r in dt.Rows)
                    {
                        cmb_maTuyen.Items.Add(r["maTuyen"].ToString());
                    }

                    cmb_maTuyen.Text = "Chọn mã tuyến";
                }
            }
        }
       private void LoadData()
        {

            using (SqlConnection conn = Database.GetConnection())
            {

                dataChuyenDi.Rows.Clear();

                SqlDataAdapter da = new SqlDataAdapter(@"
                SELECT maCD, maTuyen, ngKh, tgKh 
                FROM CHUYENDI 
                WHERE trangThai = N'Hoạt động'
                ORDER BY maCD", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    string maCD = r["maCD"].ToString();
                    string maTuyen = r["maTuyen"].ToString();
                    string ngKh = Convert.ToDateTime(r["ngKh"]).ToString("dd/MM/yyyy");
                    string tgKh = r["tgKh"]?.ToString();
                    if (!string.IsNullOrEmpty(tgKh) && tgKh.Length >= 5)
                        tgKh = tgKh.Substring(0, 5);
                    else
                        tgKh = "";
                    SqlDataAdapter daNV = new SqlDataAdapter(
                        "SELECT maNV FROM CHUYENDI_NHANVIEN WHERE maCD='" + maCD + "'", conn);
                    DataTable dtNV = new DataTable();
                    daNV.Fill(dtNV);

                    string nvStr = string.Join(", ",
                        dtNV.Rows.Cast<DataRow>().Select(x => x["maNV"].ToString()));

                    dataChuyenDi.Rows.Add(maCD, maTuyen, ngKh, tgKh, nvStr);

                }
                dataChuyenDi.ClearSelection();
                dataChuyenDi.CurrentCell = null;
            }
        }
        private string LoadNhanVienRanh()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string currentMaCD = "";
                if (dataChuyenDi.CurrentRow != null && dataChuyenDi.CurrentRow.Cells["maCD"].Value != null)
                {
                    currentMaCD = dataChuyenDi.CurrentRow.Cells["maCD"].Value.ToString();
                }

                DateTime selectedDate = dtpNgayKH.Value.Date;

                HashSet<string> assignedInGrid = new HashSet<string>();
                foreach (DataGridViewRow row in dataChuyenDi.Rows)
                {
                    if (row.IsNewRow) continue;

                    var rowMaCD = row.Cells["maCD"].Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(currentMaCD) && rowMaCD == currentMaCD) continue;

                    string rowDateStr = row.Cells["ngKh"].Value?.ToString() ?? "";
                    if (!TryParseDateFromCell(rowDateStr, out DateTime rowDate)) continue;

                    if (rowDate.Date == selectedDate)
                    {
                        var nvCell = row.Cells["nhanVien"].Value?.ToString() ?? "";
                        foreach (var ma in ParseMaNVFromCell(nvCell))
                            assignedInGrid.Add(ma);
                    }
                }

                using (SqlCommand cmd = new SqlCommand("SELECT maNV, hoTen FROM NHANVIEN", conn))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    listNhanVien.Items.Clear();

                    foreach (DataRow r in dt.Rows)
                    {
                        var ma = r["maNV"].ToString();
                        var ht = r["hoTen"].ToString();

                        if (assignedInGrid.Contains(ma)) continue;

                        listNhanVien.Items.Add(ma + " - " + ht);
                    }
                }
            }
            return null;
        }

        private void LamMoiForm()
        {
            txtMaChuyenDi.Text = AutoMaCD();
            cmb_maTuyen.Text = " ";
            dtpNgayKH.Value = DateTime.Now;
            dtp_TgKH.Value = DateTime.Now;
            LoadNhanVienRanh();

            dataChuyenDi.ClearSelection();
            dataChuyenDi.CurrentCell = null;

        }

        private IEnumerable<string> ParseMaNVFromCell(string nvCell)
        {
            if (string.IsNullOrWhiteSpace(nvCell)) yield break;
            var parts = nvCell.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(p => p.Trim())
                              .Where(p => !string.IsNullOrEmpty(p));
            foreach (var p in parts)
            {
                var ma = p.Split('-')[0].Trim();
                if (!string.IsNullOrEmpty(ma)) yield return ma;
            }
        }

        private bool TryParseDateFromCell(string s, out DateTime dt)
        {
            dt = default;
            if (string.IsNullOrWhiteSpace(s)) return false;

            string[] formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "M/d/yyyy", "MM/dd/yyyy" };

            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                return true;

            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            return false;
        }
        private void dataChuyenDi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var r = dataChuyenDi.Rows[e.RowIndex];

            txtMaChuyenDi.Text = r.Cells["maCD"].Value.ToString();
            cmb_maTuyen.Text = r.Cells["maTuyen"].Value.ToString();

            DateTime ngay;
            if (DateTime.TryParseExact(r.Cells["ngKh"].Value.ToString(),
                                       "dd/MM/yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out ngay))
            {
                dtpNgayKH.Value = ngay;
            }

            dtp_TgKH.Text = r.Cells["tgKh"].Value.ToString();

            LoadNhanVienRanh();

            string nvStr = r.Cells["nhanVien"].Value?.ToString() ?? "";
            var nvList = nvStr.Split(',')
                              .Select(x => x.Trim())
                              .Where(x => x != "");

            for (int i = 0; i < listNhanVien.Items.Count; i++)
            {
                string itemMa = listNhanVien.Items[i].ToString().Split('-')[0].Trim();
                listNhanVien.SetSelected(i, nvList.Contains(itemMa));
            }
        }
         private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaChuyenDi.Text) ||
                cmb_maTuyen.Text == "Chọn mã tuyến")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu!");
                return;
            }

            var selectedNV = listNhanVien.SelectedItems;
            if (selectedNV.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!");
                return;
            }

            string nhanVien = string.Join(", ", selectedNV.Cast<string>());
            txtMaChuyenDi.Text = AutoMaCD();
            dataChuyenDi.Rows.Add(
                txtMaChuyenDi.Text,
                cmb_maTuyen.Text,
                dtpNgayKH.Value.ToString("dd/MM/yyyy"),
                dtp_TgKH.Value.ToString("HH:mm"),
                nhanVien
            );

            LoadNhanVienRanh();

            MessageBox.Show("Đã thêm chuyến đi!");
            LamMoiForm();

        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataChuyenDi.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một chuyến đi để sửa.");
                return;
            }

            var r = dataChuyenDi.SelectedRows[0];

            r.Cells["maTuyen"].Value = cmb_maTuyen.Text;
            r.Cells["ngKh"].Value = dtpNgayKH.Value.ToString("dd/MM/yyyy");
            r.Cells["tgKh"].Value = dtp_TgKH.Value.ToString("HH:mm");

            var selectedNV = listNhanVien.SelectedItems;
            string nvStr = string.Join(", ", selectedNV.Cast<string>());
            r.Cells["nhanVien"].Value = nvStr;

            LoadNhanVienRanh(); 

            MessageBox.Show("Đã sửa dữ liệu trên bảng! Nhấn LƯU để cập nhật vào CSDL.");
        }
        private void btnHuy_Click(object sender, EventArgs e)
        {
            LamMoiForm();
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
          
            if (dataChuyenDi.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một chuyến đi để xóa.");
                return;
            }

            var maCD = dataChuyenDi.SelectedRows[0].Cells["maCD"].Value.ToString();

   
            var confirm = MessageBox.Show("Bạn có chắc muốn xóa chuyến đi này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            
            dataChuyenDi.Rows.Remove(dataChuyenDi.CurrentRow);

            
            LoadNhanVienRanh();

            MessageBox.Show("Đã xóa chuyến đi (tạm thời). Nhấn Lưu để cập nhật CSDL.");
        }
        
        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                List<string> dsMaCD = new List<string>();
                foreach (DataGridViewRow row in dataChuyenDi.Rows)
                {
                    if (row.IsNewRow) continue;
                    dsMaCD.Add(row.Cells["maCD"].Value.ToString());
                }

                if (dsMaCD.Count > 0)
                {
                    string placeholders = string.Join(",", dsMaCD.Select((v, i) => "@id" + i));
                    string sqlOff = "UPDATE CHUYENDI SET trangThai=N'Ngưng hoạt động' WHERE maCD NOT IN (" + placeholders + ")";
                    using (SqlCommand cmd = new SqlCommand(sqlOff, conn))
                    {
                        for (int i = 0; i < dsMaCD.Count; i++)
                            cmd.Parameters.AddWithValue("@id" + i, dsMaCD[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string sqlOff = "UPDATE CHUYENDI SET trangThai=N'Ngưng hoạt động'";
                    using (SqlCommand cmd = new SqlCommand(sqlOff, conn))
                        cmd.ExecuteNonQuery();
                }

                foreach (DataGridViewRow row in dataChuyenDi.Rows)
                {
                    if (row.IsNewRow) continue;

                    string maCD = row.Cells["maCD"].Value.ToString();
                    string maTuyen = row.Cells["maTuyen"].Value.ToString();
                    string ngKh = row.Cells["ngKh"].Value.ToString();
                    string tgKh = row.Cells["tgKh"].Value.ToString();
                    string nvStr = row.Cells["nhanVien"].Value?.ToString() ?? "";

                    string qCheck = "SELECT COUNT(*) FROM CHUYENDI WHERE maCD=@maCD";
                    using (SqlCommand cmd = new SqlCommand(qCheck, conn))
                    {
                        cmd.Parameters.AddWithValue("@maCD", maCD);
                        int count = (int)cmd.ExecuteScalar();

                        if (!TryParseDateFromCell(ngKh, out DateTime parsedNgKh))
                        {
                            MessageBox.Show($"Không thể chuyển đổi ngày khởi hành cho chuyến '{maCD}': '{ngKh}'. Vui lòng dùng định dạng dd/MM/yyyy.");
                            return;
                        }

                        if (count > 0)
                        {
                            string qUpdate = "UPDATE CHUYENDI SET maTuyen=@maTuyen, ngKh=@ngKh, tgKh=@tgKh, trangThai=N'Hoạt động' WHERE maCD=@maCD";
                            using (SqlCommand c = new SqlCommand(qUpdate, conn))
                            {
                                c.Parameters.AddWithValue("@maTuyen", maTuyen);
                                c.Parameters.AddWithValue("@ngKh", parsedNgKh);
                                c.Parameters.AddWithValue("@tgKh", tgKh);
                                c.Parameters.AddWithValue("@maCD", maCD);
                                c.ExecuteNonQuery();
                            }

                            using (SqlCommand cDel = new SqlCommand("DELETE FROM CHUYENDI_NHANVIEN WHERE maCD=@maCD", conn))
                            {
                                cDel.Parameters.AddWithValue("@maCD", maCD);
                                cDel.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string qInsert = "INSERT INTO CHUYENDI(maCD, maTuyen, ngKh, tgKh, trangThai) VALUES(@maCD,@maTuyen,@ngKh,@tgKh,N'Hoạt động')";
                            using (SqlCommand c = new SqlCommand(qInsert, conn))
                            {
                                c.Parameters.AddWithValue("@maCD", maCD);
                                c.Parameters.AddWithValue("@maTuyen", maTuyen);
                                c.Parameters.AddWithValue("@ngKh", parsedNgKh);
                                c.Parameters.AddWithValue("@tgKh", tgKh);
                                c.ExecuteNonQuery();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(nvStr))
                    {
                        var nvList = nvStr.Split(',').Select(x => x.Trim());
                        foreach (var nv in nvList)
                        {
                            string maNV = nv.Split('-')[0].Trim();
                            using (SqlCommand cmdNV = new SqlCommand("INSERT INTO CHUYENDI_NHANVIEN(maCD, maNV) VALUES(@maCD,@maNV)", conn))
                            {
                                cmdNV.Parameters.AddWithValue("@maCD", maCD);
                                cmdNV.Parameters.AddWithValue("@maNV", maNV);
                                cmdNV.ExecuteNonQuery();
                            }
                        }
                    }
                }

                conn.Close();
                MessageBox.Show("Đã lưu dữ liệu!");
                LoadData(); 
            }
        }
        private void formChuyenDi_Load(object sender, EventArgs e)
        {

        }

        private void dtp_TgKH_ValueChanged(object sender, EventArgs e)
        {
            LoadNhanVienRanh();
        }
    }
}
