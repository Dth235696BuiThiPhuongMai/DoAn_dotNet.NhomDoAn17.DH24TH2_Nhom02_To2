using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formQuanLy
{
    public partial class formTrangChuNhanVien : Form
    {
        public formTrangChuNhanVien()
        {
            InitializeComponent();
            btnChuyenDi.Click += btnChuyenDi_Click;
            btnDatVe.Click += btnDatVe_Click;
            btnKhachHang.Click += btnKhachHang_Click;
            btnTuyen.Click += btnTuyen_Click;
            btnThoat.Click += btnThoat_Click;

        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            formKhachHang f = new formKhachHang();
            f.Show();
            this.Hide();
        }

        private void btnChuyenDi_Click(object sender, EventArgs e)
        {
            formChuyenDi f = new formChuyenDi();
            f.Show();
            this.Hide();
        }

        private void btnDatVe_Click(object sender, EventArgs e)
        {
            formDatVe f = new formDatVe();
            f.Show();
            this.Hide();
        }

        private void btnTuyen_Click(object sender, EventArgs e)
        {
            formTuyenDuLich f = new formTuyenDuLich();
            f.Show();
            this.Hide();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có muốn thoát không??!", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
