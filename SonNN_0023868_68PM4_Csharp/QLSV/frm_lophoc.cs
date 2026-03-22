using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLSV
{
    public partial class frm_lophoc : Form
    {
        private DatabaseDataContext db = new DatabaseDataContext();
        private string _selectedMaLop = null;
        public frm_lophoc()
        {
            InitializeComponent();
        }

        private void FrmLopHoc_Load(object sender, EventArgs e)
        {
            LoadLopHoc();
            ClearForm();
        }

        void LoadLopHoc()
        {
            try
            {
                var ds = db.lops.OrderBy(l => l.malop).ToList();
                dgvLopHoc.DataSource = ds;
                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLopTheoTu(string tuKhoa)
        {
            string tk = tuKhoa.Trim();
            var ds = db.lops
                       .Where(l => l.malop.Contains(tk) || l.tenlop.Contains(tk))
                       .OrderBy(l => l.malop)
                       .ToList();
            dgvLopHoc.DataSource = ds;
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvLopHoc.Columns.Count == 0) return;

            if (dgvLopHoc.Columns["malop"] != null) { dgvLopHoc.Columns["malop"].HeaderText = "Mã lớp"; dgvLopHoc.Columns["malop"].Width = 120; }
            if (dgvLopHoc.Columns["tenlop"] != null) { dgvLopHoc.Columns["tenlop"].HeaderText = "Tên lớp"; dgvLopHoc.Columns["tenlop"].Width = 200; }
            if (dgvLopHoc.Columns["siSo"] != null) { dgvLopHoc.Columns["siSo"].HeaderText = "Sĩ số"; dgvLopHoc.Columns["siSo"].Width = 80; }
            if (dgvLopHoc.Columns["ghiChu"] != null) { dgvLopHoc.Columns["ghiChu"].HeaderText = "Ghi chú"; dgvLopHoc.Columns["ghiChu"].Width = 250; }
        }

        private lop GetLopFromForm()
        {
            if (string.IsNullOrWhiteSpace(txtMaLop.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaLop.Focus();
                return null;
            }
            if (string.IsNullOrWhiteSpace(txtTenLop.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên lớp!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenLop.Focus();
                return null;
            }

            int siSo = 0;
            if (!int.TryParse(txtSiSo.Text.Trim(), out siSo))
            {
                MessageBox.Show("Sĩ số phải là số nguyên!", "Sai dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSiSo.Focus();
                return null;
            }

            return new lop
            {
                malop = txtMaLop.Text.Trim(),
                tenlop = txtTenLop.Text.Trim(),
                siSo = siSo,
                ghiChu = txtGhiChu.Text.Trim()
            };
        }

        private void ClearForm()
        {
            txtMaLop.Text = "";
            txtTenLop.Text = "";
            txtSiSo.Text = "";
            txtGhiChu.Text = "";
            txtMaLop.Enabled = true;
            _selectedMaLop = null;
            dgvLopHoc.ClearSelection();
            txtMaLop.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            lop lh = GetLopFromForm();
            if (lh == null) return;

            // Kiểm tra trùng mã lớp
            if (db.lops.Any(x => x.malop == lh.malop))
            {
                MessageBox.Show("Mã lớp đã tồn tại!", "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                db.lops.InsertOnSubmit(lh);
                db.SubmitChanges();
                MessageBox.Show("Thêm lớp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLopHoc();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_selectedMaLop == null)
            {
                MessageBox.Show("Vui lòng chọn lớp cần sửa!", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lh = db.lops.FirstOrDefault(x => x.malop == _selectedMaLop);
            if (lh == null) { MessageBox.Show("Không tìm thấy lớp!"); return; }

            if (string.IsNullOrWhiteSpace(txtTenLop.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên lớp!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenLop.Focus();
                return;
            }

            int siSo = 0;
            if (!int.TryParse(txtSiSo.Text.Trim(), out siSo))
            {
                MessageBox.Show("Sĩ số phải là số nguyên!", "Sai dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSiSo.Focus();
                return;
            }

            lh.tenlop = txtTenLop.Text.Trim();
            lh.siSo = siSo;
            lh.ghiChu = txtGhiChu.Text.Trim();

            try
            {
                db.SubmitChanges();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLopHoc();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedMaLop == null)
            {
                MessageBox.Show("Vui lòng chọn lớp cần xóa!", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lh = db.lops.FirstOrDefault(x => x.malop == _selectedMaLop);
            if (lh == null) { MessageBox.Show("Không tìm thấy lớp!"); return; }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa lớp '{lh.tenlop}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                db.lops.DeleteOnSubmit(lh);
                db.SubmitChanges();
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLopHoc();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvLopHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvLopHoc.Rows[e.RowIndex];

            // Try different column keys depending on mapping
            _selectedMaLop = row.Cells["MaLop"].Value?.ToString() ?? row.Cells["malop"].Value?.ToString();
            txtMaLop.Text = _selectedMaLop;
            txtTenLop.Text = row.Cells["TenLop"].Value?.ToString() ?? row.Cells["tenlop"].Value?.ToString();
            txtSiSo.Text = row.Cells["SiSo"].Value?.ToString() ?? row.Cells["siSo"].Value?.ToString();
            txtGhiChu.Text = row.Cells["GhiChu"].Value?.ToString() ?? row.Cells["ghiChu"].Value?.ToString();
            txtMaLop.Enabled = false;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
            LoadLopHoc();
            ClearForm();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
                LoadLopHoc();
            else
                LoadLopTheoTu(txtTimKiem.Text);
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnTimKiem_Click(sender, e);
        }

        private void dgvLopHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }
    }
}
