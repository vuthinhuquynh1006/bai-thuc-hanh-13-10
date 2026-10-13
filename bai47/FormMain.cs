namespace code
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

        }

        private void OpenChildForm<T>() where T : Form, new()
        {
            Form childForm = this.MdiChildren.FirstOrDefault(f => f is T);

            if (childForm == null)
            {
                childForm = new T();
                childForm.MdiParent = this;
                childForm.Show();
                UpdateStatus("Đã mở cửa sổ: " + childForm.Text);
            }
            else
            {
                childForm.Activate();
            }
        }

        public void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }

        private void timerSystem_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void mntQLSinhVien_Click(object sender, EventArgs e)
        {
            OpenChildForm<FormStudents>();
        }

        private void btnToolStudent_Click(object sender, EventArgs e)
        {
            OpenChildForm<FormStudents>();
        }

        private void mntQLSanPham_Click(object sender, EventArgs e)
        {
            OpenChildForm<FormProduct>();
        }

        private void btnToolProduct_Click(object sender, EventArgs e)
        {
            OpenChildForm<FormProduct>();
        }

        private void mntCascade_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void mntTileHorizontal_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void mntTileVertical_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void mntThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mntDangXuat_Click(object sender, EventArgs e)
        {
            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }
            UpdateStatus("Đã đăng xuất tài khoản.");
        }
    }
}
