using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HotelReso
{
    public partial class Form1 : Form
    {
        //globals
        private SqlConnection conn = null;
        private SqlDataAdapter da = null;
        private DataSet ds = null;
        private int rowIndex = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getData();
            dg1.Click += dg1_Click;
        }


        private void getData()
        {
            string connStr = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=C:\\PROG37721\\HotelReso\\HotelReso\\Reservations.mdf;Integrated Security=True";
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "SELECT * FROM [tReservations]";
                da = new SqlDataAdapter(sql, conn);
                SqlCommandBuilder sb = new SqlCommandBuilder(da);
                ds = new DataSet();
                da.Fill(ds, "tReservations");

                DataView myView = new DataView(ds.Tables["tReservations"]);
                dg1.DataSource = myView;
                dg1.ClearSelection();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error Connecting to Database");
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }


        private void cmdInsert_Click(object sender, EventArgs e)
        {
            if(dataGood())
            {
                if(isValidReservation("i"))
                {
                    DataRow dr = ds.Tables["tReservations"].NewRow();
                    dr["TableNo"] = Convert.ToInt32(txtTableNum.Text);
                    dr["Date"] = datePicker.Text;
                    dr["Time"] = timePicker.Text;
                    dr["Duration"] = Convert.ToDouble(txtDuration.Text);
                    dr["Name"] = txtName.Text;
                    dr["Telephone"] = txtTel.Text;
                    dr["NumberOfGuests"] = txtGuestsNo.Text;

                    ds.Tables["tReservations"].Rows.Add(dr);
                    updateDB();
                    clearText();
                }
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {

        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this reservation?", "Delete Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                ds.Tables["tReservations"].Rows[rowIndex].Delete();
                updateDB();
            }
        }

        private void updateDB()
        {
            try
            {
                conn.Open();
                da.Update(ds, "tReservations");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error Updating Database");

            }
            finally
            {
                if(conn != null)
                {
                    conn.Close();
                }
            }
        }

        private string isLeapYear(string year)
        {
            string whichYear = null;

            if(Convert.ToInt32(year) % 4 != 0)
            {
                whichYear = "c";
            }
            else if(Convert.ToInt32(year) % 100 != 0 )
            {
                whichYear = "l";
            }
            else if(Convert.ToInt32(year) % 400 != 0)
            {
                whichYear = "c";
            }
            else
            {
                whichYear = "l";
            }
            return whichYear;
        }

        private void clearText()
        {
            datePicker.Text = "";
            timePicker.Text = "";
            txtTableNum.Text = "";
            txtDuration.Text = "";
            txtName.Text = "";
            txtTel.Text = "";
            datePicker.Focus();
            dg1.ClearSelection();
        }

        private bool dataGood()
        {
            return true;
        }

        private bool isValidReservation(string state)
        {
            
            if(state.Equals("i"))
            {
                for (int i = 0; i < dg1.Rows.Count; i++)
                {
                    //gets the duration hours of each row
                    int durationHours = Convert.ToInt32(dg1.Rows[i].Cells[3].Value);
                    
                    //make a timespan new timespan struct using the duratino hours for i reservation
                    TimeSpan duration = new TimeSpan(durationHours, 0, 0);

                    //add the duration timespan to reservation i start time
                    DateTime currentResoStartTime = Convert.ToDateTime(dg1.Rows[i].Cells[1].Value.ToString());
                    DateTime currentResoEndTime = currentResoStartTime.Add(duration);

                    //compare the reservation i end time to incoming reservation start time
                    int compareTime = DateTime.Compare(timePicker.Value, currentResoEndTime);

                    if(datePicker.Text.Equals(dg1.Rows[i].Cells[0].Value.ToString()))
                    {
                        if(txtTableNum.Text.Equals(dg1.Rows[i].Cells[2].Value.ToString()))
                        {
                            if(timePicker.Text.Equals(dg1.Rows[i].Cells[1].Value.ToString()))
                            {
                                MessageBox.Show("A reservation already exists for this table at this time. Please select a different time or a different table", "Invalid Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtTableNum.Focus();
                                return false;
                            }
                            // compareTime returns -1 if t1 is earlier than t2
                            else if(compareTime < 0)
                            {
                                string message = "An earlier reservation for this table finishes at " + currentResoEndTime + ". Please select a later time or a different table";
                                MessageBox.Show(message, "Invalid Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtTableNum.Focus();
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void setControlState(string state)
        {
            if (state.Equals("i"))
            {
                cmdInsert.Enabled = true;
                cmdUpdate.Enabled = false;
                cmdDelete.Enabled = false;
                clearText();
            }
            if (state.Equals("u/d"))
            {
                cmdInsert.Enabled = false;
                cmdUpdate.Enabled = true;
                cmdDelete.Enabled = true;
            }
        }
      
        void dg1_Click(object sender, EventArgs e)
        {
            rowIndex = dg1.CurrentRow.Index;

            dg1.CurrentRow.Selected = true;
            datePicker.Text = dg1.CurrentRow.Cells[0].Value.ToString();
            timePicker.Text = dg1.CurrentRow.Cells[1].Value.ToString();
            txtTableNum.Text = dg1.CurrentRow.Cells[2].Value.ToString();
            txtDuration.Text = dg1.CurrentRow.Cells[3].Value.ToString();
            txtName.Text = dg1.CurrentRow.Cells[4].Value.ToString();
            txtTel.Text = dg1.CurrentRow.Cells[5].Value.ToString();
            setControlState("u/d");
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        
        //add form load
    }
}
