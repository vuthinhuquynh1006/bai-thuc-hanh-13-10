using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace bai46
{
    public partial class Form1 : Form
    {
        // Danh sách & BindingSource
        private BindingList<Student> studentList;
        private BindingSource bindingSource;

        // Khai báo các Control trên Giao diện
        private DataGridView dgvStudents;
        private TextBox txtStudentId;
        private TextBox txtFullName;
        private TextBox txtAge;
        private ComboBox cbGender;
        private PictureBox picAvatar;
        private Button btnChooseAvatar;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnExportCsv;

        public Form1()
        {
            // Cấu hình Form
            //this.Title = "QUẢN LÝ SINH VIÊN";
            this.Text = "Quản lý Sinh viên - BindingSource & CSV";
            this.Size = new Size(850, 520);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Khởi tạo các Control bằng Code
            InitializeCustomComponents();

            // Dữ liệu & Binding
            InitData();
            SetupBinding();
        }

        private void InitializeCustomComponents()
        {
            // 1. DataGridView
            dgvStudents = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(480, 420),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };
            this.Controls.Add(dgvStudents);

            // GroupBox Nhập liệu
            GroupBox gbInfo = new GroupBox
            {
                Text = "Thông tin sinh viên",
                Location = new Point(510, 15),
                Size = new Size(300, 260)
            };
            this.Controls.Add(gbInfo);

            // Labels & TextBoxes
            Label lblId = new Label { Text = "Mã SV:", Location = new Point(15, 30), AutoSize = true };
            txtStudentId = new TextBox { Location = new Point(85, 27), Size = new Size(190, 23) };

            Label lblName = new Label { Text = "Họ tên:", Location = new Point(15, 65), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(85, 62), Size = new Size(190, 23) };

            Label lblAge = new Label { Text = "Tuổi:", Location = new Point(15, 100), AutoSize = true };
            txtAge = new TextBox { Location = new Point(85, 97), Size = new Size(190, 23) };

            Label lblGender = new Label { Text = "Giới tính:", Location = new Point(15, 135), AutoSize = true };
            cbGender = new ComboBox { Location = new Point(85, 132), Size = new Size(190, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cbGender.Items.AddRange(new object[] { "Nam", "Nữ" });

            // PictureBox Avatar
            Label lblAvatar = new Label { Text = "Ảnh:", Location = new Point(15, 170), AutoSize = true };
            picAvatar = new PictureBox
            {
                Location = new Point(85, 170),
                Size = new Size(80, 80),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            btnChooseAvatar = new Button { Text = "Chọn ảnh", Location = new Point(175, 170), Size = new Size(100, 30) };
            btnChooseAvatar.Click += BtnChooseAvatar_Click;

            gbInfo.Controls.AddRange(new Control[] { lblId, txtStudentId, lblName, txtFullName, lblAge, txtAge, lblGender, cbGender, lblAvatar, picAvatar, btnChooseAvatar });

            // GroupBox Chức năng
            GroupBox gbActions = new GroupBox
            {
                Text = "Chức năng",
                Location = new Point(510, 285),
                Size = new Size(300, 155)
            };
            this.Controls.Add(gbActions);

            btnAdd = new Button { Text = "Thêm", Location = new Point(20, 30), Size = new Size(115, 35) };
            btnUpdate = new Button { Text = "Cập nhật", Location = new Point(160, 30), Size = new Size(115, 35) };
            btnDelete = new Button { Text = "Xóa", Location = new Point(20, 80), Size = new Size(115, 35) };
            btnExportCsv = new Button { Text = "Xuất CSV", Location = new Point(160, 80), Size = new Size(115, 35), BackColor = Color.LightGreen };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnExportCsv.Click += BtnExportCsv_Click;

            gbActions.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnExportCsv });
        }

        // Khởi tạo dữ liệu mẫu
        private void InitData()
        {
            studentList = new BindingList<Student>
            {
                new Student("SV001", "Nguyễn Văn An", 20, "Nam", ""),
                new Student("SV002", "Trần Thị Bình", 21, "Nữ", "")
            };

            bindingSource = new BindingSource { DataSource = studentList };
        }

        // Đóng gói Binding 2 chiều
        private void SetupBinding()
        {
            dgvStudents.DataSource = bindingSource;

            txtStudentId.DataBindings.Add("Text", bindingSource, "StudentId", true, DataSourceUpdateMode.OnPropertyChanged);
            txtFullName.DataBindings.Add("Text", bindingSource, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAge.DataBindings.Add("Text", bindingSource, "Age", true, DataSourceUpdateMode.OnPropertyChanged);
            cbGender.DataBindings.Add("SelectedItem", bindingSource, "Gender", true, DataSourceUpdateMode.OnPropertyChanged);

            bindingSource.CurrentChanged += (s, e) => LoadCurrentAvatar();
            LoadCurrentAvatar();
        }

        private void LoadCurrentAvatar()
        {
            if (bindingSource.Current is Student student && !string.IsNullOrEmpty(student.AvatarPath) && File.Exists(student.AvatarPath))
            {
                try
                {
                    using (var img = Image.FromFile(student.AvatarPath))
                    {
                        picAvatar.Image = new Bitmap(img);
                    }
                }
                catch { picAvatar.Image = null; }
            }
            else
            {
                picAvatar.Image = null;
            }
        }

        // Xử lý các nút bấm
        private void BtnChooseAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK && bindingSource.Current is Student student)
                {
                    student.AvatarPath = ofd.FileName;
                    LoadCurrentAvatar();
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Student newStudent = new Student
            {
                StudentId = "SV" + (studentList.Count + 1).ToString("D3"),
                FullName = "Sinh viên mới",
                Age = 18,
                Gender = "Nam",
                AvatarPath = ""
            };
            studentList.Add(newStudent);
            bindingSource.Position = studentList.Count - 1;
            txtFullName.Focus();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            bindingSource.EndEdit();
            dgvStudents.Refresh();
            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current is Student student)
            {
                if (MessageBox.Show($"Bạn có muốn xóa sinh viên {student.FullName}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bindingSource.RemoveCurrent();
                }
            }
        }

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV file (*.csv)|*.csv", FileName = "DanhSachSinhVien.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            writer.WriteLine("StudentId,FullName,Age,Gender,AvatarPath");
                            foreach (var s in studentList)
                            {
                                writer.WriteLine($"{EscapeCsv(s.StudentId)},{EscapeCsv(s.FullName)},{s.Age},{EscapeCsv(s.Gender)},{EscapeCsv(s.AvatarPath)}");
                            }
                        }
                        MessageBox.Show("Xuất CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string EscapeCsv(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                return $"\"{val.Replace("\"", "\"\"")}\"";
            return val;
        }
    }
}