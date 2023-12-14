using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThuVienVip_ASM2
{
    public partial class frmNguoiDoc : Form
    {
        string connectionString;
        SqlConnection conn;
        SqlCommand cmd;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;

        // kết nối
        public frmNguoiDoc()
        {
            InitializeComponent();
            connectionString = @"Data Source=LAPTOP-Q0PAQT9D\SANGHOCSQL;Initial Catalog=ThuVienVip;Integrated Security=True";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
        }

        // trở về form admin chính
        private void tsout_Click(object sender, EventArgs e)
        {
            
            this.Hide();
            frmAdmin admin = new frmAdmin();
            admin.ShowDialog();
            this.Dispose();
        }

        // hiển thị dữ liệu
        private void frmNguoiDoc_Load(object sender, EventArgs e)
        {
            conn = new SqlConnection(connectionString);
            dataAdapter = new SqlDataAdapter("SELECT * FROM Readers", conn);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGridViewNguoiDoc.DataSource = dataTable;
        }

        // load lại dữ liệu
        public void Filldata()
        {
            conn.Open();
            string query = "Select * from Readers";
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter(query, conn);
            ad.Fill(dt);
            dataGridViewNguoiDoc.DataSource = dt;
            conn.Close();
        }

        // thêm dữ liệu
        private void btninsert_Click(object sender, EventArgs e)
        {
            int loi = 0;

            string madoc = txtreaid.Text;
            if (madoc.Equals(""))
            {
                loi++;
                lblbao.Text = "You have left blank in ReaderID - Please fill in completely !!! ";
            }
            else
            {
                lblbao.Text = " ";
            }

            string fullname = txtten.Text;
            if (fullname.Equals(""))
            {
                loi++;
                lblbao.Text = "You have left blank in fullname - Please fill in completely !!!";
            }
            else //check ID trùng
            {
                try
                {
                    conn.Open();
                    string query = "select * from Readers where ReaderID = @ReaderID";

                    SqlCommand commcheck = new SqlCommand(query, conn);
                    commcheck.Parameters.AddWithValue("@ReaderID", SqlDbType.Char);
                    commcheck.Parameters["@ReaderID"].Value = madoc;
                    SqlDataReader reader = commcheck.ExecuteReader();

                    if (reader.Read())
                    {
                        loi++;
                        lblbao.Text = "ReaderID had existed";
                    }
                    else
                    {
                        lblbao.Text = "";
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

            string date = txtdate.Text;
            string sex = txtsex.Text;
            string add = txtadd.Text;
            string phone = txtphone.Text;
            

            if (loi == 0)
            {

                string Them = "Insert into Readers values (@ReaderID,@Fullname,@date,@sex,@Address,@Phone)";
                conn.Open();

                SqlCommand commThem = new SqlCommand(Them, conn);

                commThem.Parameters.AddWithValue("@ReaderID", SqlDbType.Char);
                commThem.Parameters["@ReaderID"].Value = madoc;

                commThem.Parameters.AddWithValue("@Fullname", SqlDbType.NVarChar);
                commThem.Parameters["@Fullname"].Value = fullname;

                commThem.Parameters.AddWithValue("@date", SqlDbType.Date);
                commThem.Parameters["@date"].Value = date;

                commThem.Parameters.AddWithValue("@sex", SqlDbType.TinyInt);
                commThem.Parameters["@sex"].Value = sex;

                commThem.Parameters.AddWithValue("@Address", SqlDbType.NVarChar);
                commThem.Parameters["@Address"].Value = add;

                commThem.Parameters.AddWithValue("@Phone", SqlDbType.NVarChar);
                commThem.Parameters["@Phone"].Value = phone;


                commThem.ExecuteNonQuery();

                conn.Close();

                Filldata();

                lblbao.Text = "You have successfully added data to the table!!!";

            }
        }

        string choosenID;

        // click vào dữ liệu trong bảng
        private void dataGridViewNguoiDoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                conn.Open();
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = this.dataGridViewNguoiDoc.Rows[e.RowIndex];
                    choosenID = row.Cells["ReaderID"].Value.ToString();
                    txtreaid.Text = row.Cells["ReaderID"].Value.ToString();
                    txtten.Text = row.Cells["Fullname"].Value.ToString();
                    txtdate.Text = row.Cells["DateOfBirth"].Value.ToString();
                    txtsex.Text = row.Cells["Sex"].Value.ToString();
                    txtadd.Text = row.Cells["Address"].Value.ToString();
                    txtphone.Text = row.Cells["Phone"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
            finally { conn.Close(); }
        }

        // Update dữ liệu
        private void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                if ((MessageBox.Show("Do you want to update?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes))
                {
                    conn.Open();
                    string update = "UPDATE Readers SET Fullname = @fullname, DateOfBirth = @dateOfBirth, Sex = @sex, Address = @address, Phone = @phone WHERE ReaderID = @readerID;";
                    SqlCommand updatee = new SqlCommand(update, conn);

                    updatee.Parameters.Add("@readerID", SqlDbType.Char);
                    updatee.Parameters["@readerID"].Value = txtreaid.Text;

                    updatee.Parameters.Add("@fullname", SqlDbType.NVarChar);
                    updatee.Parameters["@fullname"].Value = txtten.Text;

                    updatee.Parameters.Add("@dateOfBirth", SqlDbType.Date);
                    updatee.Parameters["@dateOfBirth"].Value = txtdate.Text;

                    updatee.Parameters.Add("@sex", SqlDbType.TinyInt);
                    updatee.Parameters["@sex"].Value = txtsex.Text;

                    updatee.Parameters.Add("@address", SqlDbType.NVarChar);
                    updatee.Parameters["@address"].Value = txtadd.Text;

                    updatee.Parameters.Add("@phone", SqlDbType.NVarChar);
                    updatee.Parameters["@phone"].Value = txtphone.Text;

                    int i = updatee.ExecuteNonQuery();

                    // Kiểm tra số hàng bị ảnh hưởng và xử lý kết quả
                    if (i > 0)
                    {

                        MessageBox.Show("You have successfully updated!! ");
                        lblbao.Text = "You have just updated successfully !!! ";
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

        // Xóa dữ liệu
        private void btndele_Click(object sender, EventArgs e)
        {
            try
            {
                if (choosenID != null)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this reader : ID =  " + choosenID + "?", "Confirm", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        conn.Open();

                        string sqlQuery = "DELETE FROM Readers WHERE ReaderID = @readerID";

                        SqlCommand comm = new SqlCommand(sqlQuery, conn);
                        comm.Parameters.AddWithValue("@readerID", choosenID);

                        comm.ExecuteNonQuery();

                        conn.Close();

                        Filldata();

                        lblbao.Text = " You have successfully deleted data!!! ";
                    }
                }
                else
                {
                    lblbao.Text = "To delete successfully, you must click on the data you want to delete.";
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

        // reset lại form 
        private void btnlammoi_Click(object sender, EventArgs e)
        {
            Filldata();
            lblbao.Text = "You have just reset successfully!!!";
            txtreaid.Text = null;
            txtten.Text = null;
            txtdate.Text = null;
            txtsex.Text = null;
            txtadd.Text = null;
            txtphone.Text = null;
            txtkiem.Text = null;
           
        }

        // tìm kiếm dữ liệu
        private void btnkiem_Click(object sender, EventArgs e)
        {
            string searchValue = txtkiem.Text.Trim();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string query = "SELECT * FROM Readers WHERE readerID LIKE @SearchValue OR Fullname LIKE @SearchValue";
                conn.Open();
                SqlCommand comm = new SqlCommand(query, conn);
                comm.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();

                adapter.Fill(dt);

                conn.Close();

                if (dt.Rows.Count > 0)
                {
                    dataGridViewNguoiDoc.DataSource = dt;
                    lblbao.Text = "Search completed!";
                }
                else
                {
                    dataGridViewNguoiDoc.DataSource = null;
                    lblbao.Text = "No results found!";
                }
            }
            else
            {
                lblbao.Text = "Please enter a search value!";
            }
        }
    }
}
