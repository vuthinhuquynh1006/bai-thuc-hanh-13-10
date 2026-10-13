

using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp9
{
    public partial class Form1 : Form
    {
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        public Form1()
        {
            // Khoi tao giao dien bang code
            this.IsMdiContainer = true;
            this.Text = "He thong quan ly sinh vien";
            this.WindowState = FormWindowState.Maximized;

            TaoGiaoDien();
        }

        private void TaoGiaoDien()
        {
            // Menu
            menuStrip = new MenuStrip();

            ToolStripMenuItem mnuChucNang =
                new ToolStripMenuItem("Chuc nang");

            ToolStripMenuItem mnuNapDuLieu =
                new ToolStripMenuItem("Nap du lieu sinh vien");

            mnuNapDuLieu.Click += MnuNapDuLieu_Click;

            ToolStripMenuItem mnuThoat =
                new ToolStripMenuItem("Thoat");

            mnuThoat.Click += (s, e) => this.Close();

            mnuChucNang.DropDownItems.Add(mnuNapDuLieu);
            mnuChucNang.DropDownItems.Add(mnuThoat);
            menuStrip.Items.Add(mnuChucNang);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Thanh trang thai
            statusStrip = new StatusStrip();

            lblStatus = new ToolStripStatusLabel();
            lblStatus.Text = "San sang";

            statusStrip.Items.Add(lblStatus);
            this.Controls.Add(statusStrip);
        }

        private void MnuNapDuLieu_Click(object sender, EventArgs e)
        {
            // Tranh mo nhieu cua so nap du lieu cung luc
            foreach (Form form in this.MdiChildren)
            {
                if (form is FormNapDuLieu)
                {
                    form.Activate();
                    return;
                }
            }

            FormNapDuLieu frm = new FormNapDuLieu();
            frm.MdiParent = this;

            // Nhan phan tram tu Form con
            frm.CapNhatTienTrinh += (phanTram) =>
            {
                lblStatus.Text =
                    "Dang nap du lieu: " + phanTram + "%";
            };

            frm.FormClosed += (s, args) =>
            {
                lblStatus.Text = "San sang";
            };

            frm.Show();
        }
    }
}