using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThuVienVip_ASM2
{
    public partial class frmLogin : Form
    {
        string connectionString;
        SqlConnection conn;

        // kết nối Sql server
        public frmLogin()
        {
            InitializeComponent();
            connectionString = @"Data Source=LAPTOP-Q0PAQT9D\SANGHOCSQL;Initial Catalog=ThuVienVip;Integrated Security=True";
            conn = new SqlConnection(connectionString);
            conn.Open();
            
            conn.Close();
        }

        // Phân quyền và đăng nhập
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUser.Text;
                string password = txtPass.Text;
                string query = "Select * from TaiKhoan where nguoidung =@nguoidung and matkhau =@matkhau";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nguoidung", SqlDbType.VarChar);
                cmd.Parameters["@nguoidung"].Value = username;
                cmd.Parameters.AddWithValue("@matkhau", SqlDbType.VarChar);
                cmd.Parameters["@matkhau"].Value = password;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string role = reader["roles"].ToString();
                    if (role.Equals("admin"))
                    {
                        MessageBox.Show("Login successful !!! ", "Notification", MessageBoxButtons.OK, MessageBoxIcon.None);
                        this.Hide();
                        frmAdmin admin = new frmAdmin();
                        admin.ShowDialog();
                        this.Dispose();
                    }
                    else if (role.Equals("user"))
                    {
                        MessageBox.Show("Login successful !!! ", "Notification", MessageBoxButtons.OK, MessageBoxIcon.None);
                        this.Hide();
                        frmNguoiDung user = new frmNguoiDung();  //chuyển form
                        user.ShowDialog(); //show hộp thoại thông báo
                        this.Dispose(); //Xóa bộ nhớ
                    }
                    else
                    {
                        lblthongbao.Text = "You do not have permission to access yet !!! ";
                    }

                }
                else
                {
                    lblthongbao.Text = "You do not have permission to access yet !!! ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error !!! " + ex.Message, "Error !!! ");
                
            }
            finally
            {
                conn.Close();
            }
            

        }

        // Thoát Ứng dụng
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
