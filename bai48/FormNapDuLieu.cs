using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp9
{
    public class FormNapDuLieu : Form
    {
        private Button btnNapDuLieu;
        private ProgressBar progressBar;
        private Label lblPhanTram;
        private Label lblThongBao;
        private DataGridView dgvSinhVien;

        // Su kien gui phan tram ve Form cha MDI
        public event Action<int> CapNhatTienTrinh;

        public FormNapDuLieu()
        {
            TaoGiaoDien();
        }

        private void TaoGiaoDien()
        {
            this.Text = "Nap du lieu sinh vien";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Nut nap du lieu
            btnNapDuLieu = new Button();
            btnNapDuLieu.Text = "Nap du lieu";
            btnNapDuLieu.Location = new Point(20, 20);
            btnNapDuLieu.Size = new Size(150, 40);
            btnNapDuLieu.Click += btnNapDuLieu_Click;

            // ProgressBar
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 80);
            progressBar.Size = new Size(500, 25);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            // Label phan tram
            lblPhanTram = new Label();
            lblPhanTram.Text = "0%";
            lblPhanTram.Location = new Point(540, 80);
            lblPhanTram.Size = new Size(100, 25);

            // Label thong bao
            lblThongBao = new Label();
            lblThongBao.Text = "Nhan nut de nap du lieu";
            lblThongBao.Location = new Point(20, 120);
            lblThongBao.Size = new Size(500, 25);

            // Bang danh sach sinh vien
            dgvSinhVien = new DataGridView();
            dgvSinhVien.Location = new Point(20, 160);
            dgvSinhVien.Size = new Size(740, 270);
            dgvSinhVien.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvSinhVien.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.AllowUserToAddRows = false;
            dgvSinhVien.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            this.Controls.Add(btnNapDuLieu);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblPhanTram);
            this.Controls.Add(lblThongBao);
            this.Controls.Add(dgvSinhVien);
        }

        // Ham nap du lieu bat dong bo
        private async Task<List<SinhVien>> NapDuLieuAsync(
            IProgress<int> tienTrinh)
        {
            return await Task.Run(async () =>
            {
                List<SinhVien> danhSach = new List<SinhVien>();

                // Gia lap 10 dot doc du lieu tu CSDL
                for (int i = 1; i <= 10; i++)
                {
                    // Gia lap moi dot mat 500ms
                    await Task.Delay(500);

                    danhSach.Add(new SinhVien(
                        "SV" + i.ToString("D3"),
                        "Sinh vien " + i,
                        "CNTT" + ((i % 3) + 1),
                        6.0 + (i % 5) * 0.5
                    ));

                    int phanTram = i * 10;

                    // Bao cao tien trinh ve UI Thread
                    tienTrinh.Report(phanTram);
                }

                return danhSach;
            });
        }

        private async void btnNapDuLieu_Click(
            object sender, EventArgs e)
        {
            btnNapDuLieu.Enabled = false;
            progressBar.Value = 0;
            lblPhanTram.Text = "0%";
            lblThongBao.Text = "Dang nap du lieu...";

            try
            {
                // Progress<int> tu dong marshal ve UI Thread
                IProgress<int> tienTrinh = new Progress<int>(
                    phanTram =>
                    {
                        progressBar.Value = phanTram;
                        lblPhanTram.Text = phanTram + "%";

                        // Cap nhat StatusStrip tren Form cha
                        CapNhatTienTrinh?.Invoke(phanTram);
                    });

                // Cho phep UI tiep tuc hoat dong khi dang nap
                List<SinhVien> danhSach =
                    await NapDuLieuAsync(tienTrinh);

                // Hien thi du lieu len bang
                dgvSinhVien.DataSource = danhSach;

                lblThongBao.Text =
                    "Nap du lieu thanh cong!";

                MessageBox.Show(
                    "Da nap thanh cong " + danhSach.Count +
                    " sinh vien.",
                    "Thong bao",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblThongBao.Text = "Nap du lieu that bai!";

                MessageBox.Show(
                    "Loi: " + ex.Message,
                    "Thong bao loi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Luon mo lai nut ke ca khi co loi
                btnNapDuLieu.Enabled = true;
            }
        }
    }
}