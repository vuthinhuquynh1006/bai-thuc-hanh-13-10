
namespace WinFormsApp9

{
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string Lop { get; set; }
        public double Diem { get; set; }

        public SinhVien(string maSV, string hoTen,
                        string lop, double diem)
        {
            MaSV = maSV;
            HoTen = hoTen;
            Lop = lop;
            Diem = diem;
        }
    }
}