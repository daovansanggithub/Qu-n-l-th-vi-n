using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThuVienVip_ASM2
{
    public partial class frmDangKi : Form
    {
        string connectionString;
        SqlConnection conn;
        SqlCommand cmd;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;

        // kết nối Sql
        public frmDangKi()
        {
            InitializeComponent();
            connectionString = @"Data Source=LAPTOP-Q0PAQT9D\SANGHOCSQL;Initial Catalog=ThuVienVip;Integrated Security=True";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
        }


        // Load Dữ liệu từ sql lên bảng
        public void Filldata()
        {
            conn.Open();
            string query = "Select * from BorrowedBooks";
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter(query, conn);
            ad.Fill(dt);
            dataGridViewDangKi.DataSource = dt;
            conn.Close();
        }

        // thoát ra form chính của người dùng
        private void tsout_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmNguoiDung user = new frmNguoiDung();
            user.ShowDialog();
            this.Dispose();
        }


        // BorrowID, ReaderID, BookID, BorrowDate, ReturnDate  === BorrowedBooks
        // hiện thi dữ liệu khi mở form 
        private void frmDangKi_Load(object sender, EventArgs e)
        {
            conn = new SqlConnection(connectionString);
            dataAdapter = new SqlDataAdapter("SELECT * FROM BorrowedBooks", conn);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGridViewDangKi.DataSource = dataTable;
        }

        // thêm dữ liệu vào bảng
        private void btnTHEM_Click(object sender, EventArgs e)
        {
            int loi = 0;

            string madangki = txtBorrowID.Text;
            if (madangki.Equals(""))
            {
                loi++;
                lblbaos.Text = "You have left blank in BorrowID - Please fill in completely !!! ";
            }
            else
            {
                lblbaos.Text = " ";
            }

            string readerID = txtReadID.Text;
            if (readerID.Equals(""))
            {
                loi++;
                lblbaos.Text = "You have left blank in fullname - Please fill in completely !!!";
            }
            else //check ID trùng
            {
                try
                {
                    conn.Open();
                    string query = "select * from BorrowedBooks where BorrowID = @BorrowID";

                    SqlCommand commcheck = new SqlCommand(query, conn);
                    commcheck.Parameters.AddWithValue("@BorrowID", SqlDbType.Char);
                    commcheck.Parameters["@BorrowID"].Value = madangki;
                    SqlDataReader reader = commcheck.ExecuteReader();

                    if (reader.Read())
                    {
                        loi++;
                        lblbaos.Text = "BorrowID had existed";
                    }
                    else
                    {
                        lblbaos.Text = "";
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

            string booid = txtBookID.Text;
            string borrowDate = txtBRDate.Text;
            string retturnDate = txtRTDate.Text;


            if (loi == 0)
            {

                string Them = "Insert into BorrowedBooks values (@BorrowID,@ReaderID,@BookID,@BorrowDate,@ReturnDate)";
                conn.Open();

                SqlCommand commThem = new SqlCommand(Them, conn);

                commThem.Parameters.AddWithValue("@BorrowID", SqlDbType.Char);
                commThem.Parameters["@BorrowID"].Value = madangki;

                commThem.Parameters.AddWithValue("@ReaderID", SqlDbType.Char);
                commThem.Parameters["@ReaderID"].Value = readerID;

                commThem.Parameters.AddWithValue("@BookID", SqlDbType.Char);
                commThem.Parameters["@BookID"].Value = booid;

                commThem.Parameters.AddWithValue("@BorrowDate", SqlDbType.Date);
                commThem.Parameters["@BorrowDate"].Value = borrowDate;

                commThem.Parameters.AddWithValue("@ReturnDate", SqlDbType.Date);
                commThem.Parameters["@ReturnDate"].Value = retturnDate;


                commThem.ExecuteNonQuery();

                conn.Close();

                Filldata();

                lblbaos.Text = "You have successfully added data to the table!!!";

            }
        }


        string choosenID;
        // khi click vào dữ liệu trong bảng thì lập tức hiển thị lên textbox
        private void dataGridViewDangKi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                conn.Open();
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = this.dataGridViewDangKi.Rows[e.RowIndex];
                    choosenID = row.Cells["BorrowID"].Value.ToString();
                    txtBorrowID.Text = row.Cells["BorrowID"].Value.ToString();
                    txtReadID.Text = row.Cells["ReaderID"].Value.ToString();
                    txtBookID.Text = row.Cells["BookID"].Value.ToString();
                    txtBRDate.Text = row.Cells["BorrowDate"].Value.ToString();
                    txtRTDate.Text = row.Cells["ReturnDate"].Value.ToString();
                  

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
            finally { conn.Close(); }
        }

        

        // BorrowID, ReaderID, BookID, BorrowDate, ReturnDate  === BorrowedBooks
        // undate dữ liệu 
        private void btnUPDATE_Click(object sender, EventArgs e)
        {
            try
            {
                if ((MessageBox.Show("Do you want to update?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes))
                {
                    conn.Open();
                    string update = "UPDATE BorrowedBooks SET ReaderID = @ReaderID, BookID = @BookID, BorrowDate = @BorrowDate, ReturnDate = @ReturnDate WHERE BorrowID = @BorrowID;";
                    SqlCommand updatee = new SqlCommand(update, conn);

                    updatee.Parameters.Add("@BorrowID", SqlDbType.Char);
                    updatee.Parameters["@BorrowID"].Value = txtBorrowID.Text;

                    updatee.Parameters.Add("@ReaderID", SqlDbType.Char);
                    updatee.Parameters["@ReaderID"].Value = txtReadID.Text;

                    updatee.Parameters.Add("@BookID", SqlDbType.Char);
                    updatee.Parameters["@BookID"].Value = txtBookID.Text;

                    updatee.Parameters.Add("@BorrowDate", SqlDbType.Date);
                    updatee.Parameters["@BorrowDate"].Value = txtBRDate.Text;

                    updatee.Parameters.Add("@ReturnDate", SqlDbType.Date);
                    updatee.Parameters["@ReturnDate"].Value = txtRTDate.Text;

                    int i = updatee.ExecuteNonQuery();

                    // Kiểm tra số hàng bị ảnh hưởng và xử lý kết quả
                    if (i > 0)
                    {

                        MessageBox.Show("You have successfully updated!! ");
                        lblbaos.Text = "You have just updated successfully !!! ";
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
                MessageBox.Show("eror :" + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // BorrowID, ReaderID, BookID, BorrowDate, ReturnDate  === BorrowedBooks
        // Xóa dữ liệu trong bảng
        private void btnDELE_Click(object sender, EventArgs e)
        {
            try
            {
                if (choosenID != null)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this reader : ID =  " + choosenID + "?", "Confirm", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        conn.Open();

                        string sqlQuery = "DELETE FROM BorrowedBooks WHERE BorrowID = @BorrowID";

                        SqlCommand comm = new SqlCommand(sqlQuery, conn);
                        comm.Parameters.AddWithValue("@BorrowID", choosenID);

                        comm.ExecuteNonQuery();

                        conn.Close();

                        Filldata();

                        lblbaos.Text = " You have successfully deleted data!!! ";
                    }
                }
                else
                {
                    lblbaos.Text = "To delete successfully, you must click on the data you want to delete.";
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

        // reset bảng
        private void btnRESET_Click(object sender, EventArgs e)
        {
            Filldata();
            lblbaos.Text = "You have just reset successfully!!!";
            txtBorrowID.Text = null;
            txtReadID.Text = null;
            txtBookID.Text = null;
            txtBRDate.Text = null;
            txtRTDate.Text = null;
            txtTK.Text = null;
            
        }

        // BorrowID, ReaderID, BookID, BorrowDate, ReturnDate  === BorrowedBooks
        // Tìm kiếm dữ liệu trong bảng
        private void btnK_Click(object sender, EventArgs e)
        {
            string searchValue = txtTK.Text.Trim();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string query = "SELECT * FROM BorrowedBooks WHERE BorrowID LIKE @SearchValue OR ReaderID LIKE @SearchValue OR BookID LIKE @SearchValue";
                conn.Open();
                SqlCommand comm = new SqlCommand(query, conn);
                comm.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();

                adapter.Fill(dt);

                conn.Close();

                if (dt.Rows.Count > 0)
                {
                    dataGridViewDangKi.DataSource = dt;
                    lblbaos.Text = "Search completed!";
                }
                else
                {
                    dataGridViewDangKi.DataSource = null;
                    lblbaos.Text = "No results found!";
                }
            }
            else
            {
                lblbaos.Text = "Please enter a search value!";
            }
        }
    }
}
