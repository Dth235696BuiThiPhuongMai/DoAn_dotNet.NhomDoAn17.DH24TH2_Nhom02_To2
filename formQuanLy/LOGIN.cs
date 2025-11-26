using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static formQuanLy.VeTrangChu;

namespace formQuanLy
{
    public partial class formLOGIN : Form
    {
       
        public formLOGIN()
        {
            InitializeComponent();


            btnOK.Click += btnOK_Click;
            btnThoat.Click +=btnThoat_Click;

            txtUser.Focus();

            txtPassword.UseSystemPasswordChar = true;
            checkHienMK.CheckedChanged += checkHienMK_CheckedChanged;

        }

       
        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có muốn thoát không??!", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = Database.GetConnection())
                try
                {
                    conn.Open();

                    string tk = txtUser.Text.Trim();
                    string mk = txtPassword.Text.Trim();

                    string sql = "SELECT * FROM TaiKhoan WHERE TenDangNhap = @tk AND MatKhau = @mk";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tk", tk);
                    cmd.Parameters.AddWithValue("@mk", mk);

                    SqlDataReader dta = cmd.ExecuteReader();

                    if (dta.Read())
                    {
                        string vtro = dta["VaiTro"].ToString();

                        UserSession.VaiTro = vtro;
                        UserSession.TaiKhoan = tk;


                        if (vtro == "QuanLy")
                        {
                            MessageBox.Show("Đăng nhập thành công - Quyền Quản Lý");
                            formTrangChuQuanLy f = new formTrangChuQuanLy();
                            f.Show();
                            this.Hide();
                        }
                        else if (vtro == "NhanVien")
                        {
                            MessageBox.Show("Đăng nhập thành công - Quyền Nhân Viên");
                            formTrangChuNhanVien f = new formTrangChuNhanVien();
                            f.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sai tên người dùng hoặc mật khẩu");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối\n" + ex.Message);
                }
        }

        private void checkHienMK_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar =!checkHienMK.Checked;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtUser.Clear();
            txtPassword.Clear();
            checkHienMK.Checked = false;  
            txtUser.Focus();
        }
    }
}
