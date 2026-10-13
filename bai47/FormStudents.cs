using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using System.IO;

namespace code
{
    public partial class FormStudents : Form
    {

        private BindingList<Student> students = new BindingList<Student>();

        private BindingSource bindingSource = new BindingSource();
        public FormStudents()
        {
            InitializeComponent();
            SetupDataBinding();
        }

        private void SetupDataBinding()
        {
            bindingSource.DataSource = students;

            dgvStudents.DataSource = bindingSource;

            dgvStudents.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvStudents.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvStudents.MultiSelect = false;

            dgvStudents.ReadOnly = true;

            dgvStudents.AllowUserToAddRows = false;

            cboGender.Items.Clear();
            cboGender.Items.Add("Nam");
            cboGender.Items.Add("Nữ");

            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
            bindingSource.CurrentChanged += (s, e) =>
            {
                ShowStudentAvatar();
                LoadStudentToInputs();
            };

            dgvStudents.SelectionChanged += (s, e) =>
            {
                LoadStudentToInputs();
            };
        }

        private void LoadStudentToInputs()
        {
            Student student = bindingSource.Current as Student;

            if (student == null)
            {
                txtStudentId.Clear();
                txtFullName.Clear();
                txtAge.Clear();
                cboGender.SelectedIndex = -1;
                return;
            }

            txtStudentId.Text = student.StudentId;
            txtFullName.Text = student.FullName;
            txtAge.Text = student.Age.ToString();
            cboGender.SelectedItem = student.Gender;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string id = txtStudentId.Text.Trim();
            string name = txtFullName.Text.Trim();
            string ageText = txtAge.Text.Trim();
            string gender = cboGender.Text;

            if (id == "" || name == "")
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên và họ tên!");
                return;
            }

            int age;
            if (!int.TryParse(ageText, out age) || age < 1 || age > 120)
            {
                MessageBox.Show("Tuổi phải là số từ 1 đến 120!");
                return;
            }

            if (gender != "Nam" && gender != "Nữ")
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return;
            }

            bool exists = students.Any(s =>
                s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                MessageBox.Show("Mã sinh viên đã tồn tại!");
                return;
            }

            Student student = new Student
            {
                StudentId = id,
                FullName = name,
                Age = age,
                Gender = gender,
                AvatarPath = ""
            };

            students.Add(student);

            bindingSource.Position = students.Count - 1;

            LoadStudentToInputs();

            MessageBox.Show("Thêm sinh viên thành công!");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần cập nhật!");
                return;
            }

            string id = txtStudentId.Text.Trim();
            string name = txtFullName.Text.Trim();
            string ageText = txtAge.Text.Trim();
            string gender = cboGender.Text;

            if (id == "" || name == "")
            {
                MessageBox.Show("Mã sinh viên và họ tên không được để trống!");
                return;
            }

            int age;
            if (!int.TryParse(ageText, out age) || age < 1 || age > 120)
            {
                MessageBox.Show("Tuổi phải là số từ 1 đến 120!");
                return;
            }

            if (gender != "Nam" && gender != "Nữ")
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return;
            }

            Student currentStudent = bindingSource.Current as Student;

            if (currentStudent == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên cần cập nhật!");
                return;
            }

            bool duplicateId = students.Any(s =>
                !object.ReferenceEquals(s, currentStudent) &&
                s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (duplicateId)
            {
                MessageBox.Show("Mã sinh viên này đã được sử dụng!");
                return;
            }

            currentStudent.StudentId = id;
            currentStudent.FullName = name;
            currentStudent.Age = age;
            currentStudent.Gender = gender;

            bindingSource.ResetCurrentItem();

            MessageBox.Show("Cập nhật sinh viên thành công!");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!");
                return;
            }

            Student student = bindingSource.Current as Student;

            if (student == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa sinh viên " + student.FullName + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                students.Remove(student);

                MessageBox.Show("Đã xóa sinh viên thành công!");
            }
        }

        private void btnChooseAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn ảnh đại diện";
                openFileDialog.Filter =
                    "File ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Image tempImage = Image.FromFile(openFileDialog.FileName))
                        {
                            Image oldImage = picAvatar.Image;
                            picAvatar.Image = new Bitmap(tempImage);

                            if (oldImage != null)
                            {
                                oldImage.Dispose();
                            }
                        }

                        Student student = bindingSource.Current as Student;

                        if (student != null)
                        {
                            student.AvatarPath = openFileDialog.FileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Không thể mở ảnh. Chi tiết: " + ex.Message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void ShowStudentAvatar()
        {
            Student student = bindingSource.Current as Student;

            Image oldImage = picAvatar.Image;
            picAvatar.Image = null;

            if (oldImage != null)
            {
                oldImage.Dispose();
            }

            if (student == null ||
                string.IsNullOrWhiteSpace(student.AvatarPath) ||
                !File.Exists(student.AvatarPath))
            {
                return;
            }

            try
            {
                using (Image tempImage = Image.FromFile(student.AvatarPath))
                {
                    picAvatar.Image = new Bitmap(tempImage);
                }
            }
            catch
            {
                picAvatar.Image = null;
            }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show("Danh sách sinh viên đang trống!");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Xuất danh sách sinh viên";
                saveFileDialog.Filter = "Tệp CSV (*.csv)|*.csv";
                saveFileDialog.FileName = "DanhSachSinhVien.csv";
                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(
                        saveFileDialog.FileName,
                        false,
                        new System.Text.UTF8Encoding(true)))
                    {
                        writer.WriteLine(
                            "Mã sinh viên,Họ và tên,Tuổi,Giới tính,Đường dẫn ảnh");

                        foreach (Student student in students)
                        {
                            writer.WriteLine(
                                EscapeCsv(student.StudentId) + "," +
                                EscapeCsv(student.FullName) + "," +
                                student.Age + "," +
                                EscapeCsv(student.Gender) + "," +
                                EscapeCsv(student.AvatarPath));
                        }
                    }

                    MessageBox.Show(
                        "Xuất danh sách sinh viên thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể xuất file CSV: " + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private string EscapeCsv(string value)
        {
            if (value == null)
            {
                value = "";
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
