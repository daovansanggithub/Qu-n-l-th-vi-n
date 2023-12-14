using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThuVienVip_ASM2
{
    public partial class frmNguoiDung : Form
    {
        
        string ConnectionString;
        SqlConnection Conn;
        SqlCommand Cmd;
        private SqlDataAdapter DataAdapter;
        private DataTable DataTable;

        // kết nối sql
        public frmNguoiDung()
        {
            InitializeComponent();
            ConnectionString = @"Data Source=LAPTOP-Q0PAQT9D\SANGHOCSQL;Initial Catalog=ThuVienVip;Integrated Security=True";
            Conn = new SqlConnection(ConnectionString);
            Cmd = new SqlCommand();
            Cmd.Connection = Conn;
        }

        // vào form hiển thị dữ liệu
        private void frmNguoiDung_Load(object sender, EventArgs e)
        {
            // Khởi tạo kết nối và dataAdapter
            // Khởi tạo dataTable
            // Gán dataTable làm nguồn dữ liệu cho DataGridView
            // Đổ dữ liệu từ dataAdapter vào dataTable
            Conn = new SqlConnection(ConnectionString);
            DataAdapter = new SqlDataAdapter("SELECT * FROM Books", Conn);
            DataTable = new DataTable();
            DataAdapter.Fill(DataTable);
            dataGridViewTimsach.DataSource = DataTable;    // load dữ liệu khi vào frrom
        }

        // load lại dữ liệu
        public void Filldata()
        {
            Conn.Open();
            string query = "Select * from Books";
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter(query, Conn);
            ad.Fill(dt);
            dataGridViewTimsach.DataSource = dt;
            Conn.Close();
        }

        // tìm kiếm sách
        private void btnTK_Click(object sender, EventArgs e)
        {
            string searchValue = txttim.Text.Trim();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string query = "SELECT * FROM Books WHERE BookID LIKE @SearchValue OR Title LIKE @SearchValue OR Author LIKE @SearchValue OR Genre LIKE @SearchValue OR publishingcompany LIKE @SearchValue";
                Conn.Open();
                SqlCommand comm = new SqlCommand(query, Conn);
                comm.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Conn.Close();

                if (dt.Rows.Count > 0)
                {
                    dataGridViewTimsach.DataSource = dt;
                    
                    MessageBox.Show("Search completed!");
                }
                else
                {
                    dataGridViewTimsach.DataSource = null;
                    MessageBox.Show("No results found!");
                    
                }
            }
            else
            {
                MessageBox.Show( "Please enter a search value!");
            }
        }

        // reset về ban đầu 
        private void reset_Click(object sender, EventArgs e)
        {
            Filldata();
            txttim.Text = "";
            
        }

        // trở lại form đăng nhập
        private void btnTrolai_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmLogin logIn = new frmLogin();
            logIn.ShowDialog();
            this.Dispose();
        }

        // Chuyển form trả sách

        // Chuyển Form Đăng Kí Sách
        private void registerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmDangKi dangki = new frmDangKi();
            dangki.ShowDialog();
            this.Dispose();
        }

        private void bookReturnDateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
