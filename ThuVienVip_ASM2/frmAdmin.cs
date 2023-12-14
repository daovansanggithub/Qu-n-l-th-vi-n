using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ThuVienVip_ASM2
{
    public partial class frmAdmin : Form
    {
        string connectionString;
        SqlConnection conn;
        SqlCommand cmd;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        

        // kết nối
        public frmAdmin()
        {
            InitializeComponent();
            connectionString = @"Data Source=LAPTOP-Q0PAQT9D\SANGHOCSQL;Initial Catalog=ThuVienVip;Integrated Security=True";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
        }

        // hiện thị dữ liệu
        private void frmAdmin_Load(object sender, EventArgs e)
        {
            // Khởi tạo kết nối và dataAdapter
            // Khởi tạo dataTable
            // Gán dataTable làm nguồn dữ liệu cho DataGridView
            // Đổ dữ liệu từ dataAdapter vào dataTable
            conn = new SqlConnection(connectionString);
            dataAdapter = new SqlDataAdapter("SELECT * FROM Books", conn);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGridViewBook.DataSource = dataTable;    // load dữ liệu khi vào frrom

            

        }


        // load lại dữ liệu
        public void Filldata()
        {
            conn.Open();
            string query = "Select * from Books";
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter(query, conn);
            ad.Fill(dt);
            dataGridViewBook.DataSource = dt;
            conn.Close();
        }


        // trở về form đăng nhập
        private void tsLognout_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmLogin logIn = new frmLogin();
            logIn.ShowDialog();
            this.Dispose();
        }


        
        // thêm dữ liệu
        private void btnthemmoi_Click(object sender, EventArgs e)
        {
            int loi = 0;

            string masach = txtbookid.Text;
            if (masach.Equals(""))
            {
                loi++;
                lblthongbao.Text = "You have left blank in BookID - Please fill in completely !!! ";
            }
            else
            {
                lblthongbao.Text = " ";
            }

            string TitleBook = txtTitle.Text;
            if (TitleBook.Equals(""))
            {
                loi++;
                lblthongbao.Text = "You have left blank in TitleBook - Please fill in completely !!!";
            }
            else //check ID trùng
            {
                try
                {
                    conn.Open();
                    string query = "select * from Books where BookID = @BookID";

                    SqlCommand commcheck = new SqlCommand(query, conn);
                    commcheck.Parameters.AddWithValue("@BookID", SqlDbType.Char); 
                    commcheck.Parameters["@BookID"].Value = masach;  
                    SqlDataReader reader = commcheck.ExecuteReader();

                    if (reader.Read())
                    {
                        loi++;
                        lblthongbao.Text = "BookID had existed";
                    }
                    else
                    {
                        lblthongbao.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.Message, "Error");
                }
                finally
                {
                    conn.Close();
                }

            }

            string Author = txtAuthor.Text;
            string Genre = txtGenre.Text;
            string publishingcompany = txtpublishingcompany.Text;
            string quantity = txtquantity.Text;
            string price = txtprice.Text;

            if (loi == 0)
            {

                string Them = "Insert into Books values (@BookID,@Title,@Author,@Genre,@publishingcompany,@quantity,@price)";
                conn.Open();

                SqlCommand commThem = new SqlCommand(Them, conn);

                commThem.Parameters.AddWithValue("@BookID", SqlDbType.Char);
                commThem.Parameters["@BookID"].Value = masach;

                commThem.Parameters.AddWithValue("@Title", SqlDbType.NVarChar);
                commThem.Parameters["@Title"].Value = TitleBook;

                commThem.Parameters.AddWithValue("@Author", SqlDbType.NVarChar);
                commThem.Parameters["@Author"].Value = Author;

                commThem.Parameters.AddWithValue("@Genre", SqlDbType.VarChar);
                commThem.Parameters["@Genre"].Value = Genre;

                commThem.Parameters.AddWithValue("@publishingcompany", SqlDbType.NVarChar);
                commThem.Parameters["@publishingcompany"].Value = publishingcompany;

                commThem.Parameters.AddWithValue("@quantity", SqlDbType.Int);
                commThem.Parameters["@quantity"].Value = quantity;

                commThem.Parameters.AddWithValue("@price", SqlDbType.Float);
                commThem.Parameters["@price"].Value = price;



                commThem.ExecuteNonQuery();

                conn.Close();

                Filldata();

                lblthongbao.Text = "You have successfully added data to the table!!!";

            }

        }


        // Xóa dữ liệu
        string choosenID;
        private void btnxoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (choosenID != null)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this book : ID =  " + choosenID + "?", "Confirm", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        conn.Open();

                        string sqlQuery = "DELETE FROM Books WHERE BookID = @BookID";

                        SqlCommand comm = new SqlCommand(sqlQuery, conn);
                        comm.Parameters.AddWithValue("@BookID", choosenID);

                        comm.ExecuteNonQuery();

                        conn.Close();

                        Filldata();

                        lblthongbao.Text = " You have successfully deleted data!!! ";
                    }
                }
                else
                {
                    lblthongbao.Text = "To delete successfully, you must click on the data you want to delete.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting data: " + ex.Message, "Error");
            }
            finally
            {
                conn.Close();
            }
        }

        // click vào = hiện lên textbox cho dễ làm việc
        private void dataGridViewBook_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                conn.Open();
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = this.dataGridViewBook.Rows[e.RowIndex];
                    choosenID = row.Cells["BookID"].Value.ToString();
                    txtbookid.Text = row.Cells["BookID"].Value.ToString();
                    txtTitle.Text = row.Cells["Title"].Value.ToString();
                    txtAuthor.Text = row.Cells["Author"].Value.ToString();
                    txtGenre.Text = row.Cells["Genre"].Value.ToString();
                    txtpublishingcompany.Text = row.Cells["publishingcompany"].Value.ToString();
                    txtquantity.Text = row.Cells["quantity"].Value.ToString();
                    txtprice.Text = row.Cells["price"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
            finally { conn.Close(); }
        }

        // Sửa dữ liệu trong bảng
        private void btnSua_Click(object sender, EventArgs e)
        {
           
            try
            {
                if ((MessageBox.Show("Do you want to update?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes))
                {
                    conn.Open();
                    string update = "UPDATE Books SET Title = @title, Author = @author, Genre = @genre, publishingcompany = @publis, quantity = @quantity, price = @price WHERE BookID = @Bookid;";
                    SqlCommand updatee = new SqlCommand(update, conn);
                    updatee.Parameters.Add("@bookid", SqlDbType.Char);
                    updatee.Parameters["@bookid"].Value = txtbookid.Text;

                    updatee.Parameters.Add("@title", SqlDbType.NVarChar);
                    updatee.Parameters["@title"].Value = txtTitle.Text;

                    updatee.Parameters.Add("@author", SqlDbType.NVarChar);
                    updatee.Parameters["@author"].Value = txtAuthor.Text;

                    updatee.Parameters.Add("@genre", SqlDbType.NVarChar);
                    updatee.Parameters["@genre"].Value = txtGenre.Text;

                    updatee.Parameters.Add("@publis", SqlDbType.NVarChar);
                    updatee.Parameters["@publis"].Value = txtpublishingcompany.Text;

                    updatee.Parameters.Add("@quantity", SqlDbType.Int);
                    updatee.Parameters["@quantity"].Value = txtquantity.Text;

                    updatee.Parameters.Add("@price", SqlDbType.Float);
                    updatee.Parameters["@price"].Value = txtprice.Text;

                    int i = updatee.ExecuteNonQuery();

                    // Kiểm tra số hàng bị ảnh hưởng và xử lý kết quả
                    if (i > 0)
                    {

                        MessageBox.Show("You have successfully updated!! ");
                        lblthongbao.Text = "You have just updated successfully !!! ";
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("There was an error during the Update process");
                }
                Filldata();
            }
            catch (Exception ex)
            {
                MessageBox.Show("eror :"+ ex.Message);
            }
            finally
            {
                conn.Close();
            }
                
        }


        // Tìm kiếm dữ liệu

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchValue = txtTim.Text.Trim();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string query = "SELECT * FROM Books WHERE BookID LIKE @SearchValue OR Title LIKE @SearchValue OR Author LIKE @SearchValue OR Genre LIKE @SearchValue OR publishingcompany LIKE @SearchValue";
                conn.Open();
                SqlCommand comm = new SqlCommand(query, conn);
                comm.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();

                if (dt.Rows.Count > 0)
                {
                    dataGridViewBook.DataSource = dt;
                    lblthongbao.Text = "Search completed!";
                }
                else
                {
                    dataGridViewBook.DataSource = null;
                    lblthongbao.Text = "No results found!";
                }
            }
            else
            {
                lblthongbao.Text = "Please enter a search value!";
            }
        }

        // reset dữ liệu trong form 
        public void btnreset_Click(object sender, EventArgs e)
        {
            Filldata();
            lblthongbao.Text = "You have just reset successfully!!!";
            txtbookid.Text = null;
            txtTitle.Text = null;
            txtAuthor.Text = null;
            txtGenre.Text = null;
            txtpublishingcompany.Text = null;
            txtquantity.Text = null;
            txtprice.Text = null;
            txtTim.Text = null;
        }


        // menuStrip chuyển form sang form quản lí người đọc
        private void tsNguoiDoc_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Form Readers Open !!! ", "Notification", MessageBoxButtons.OK, MessageBoxIcon.None);
            this.Hide();
            frmNguoiDoc reader = new frmNguoiDoc();
            reader.ShowDialog();
            this.Dispose();
        }




        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
