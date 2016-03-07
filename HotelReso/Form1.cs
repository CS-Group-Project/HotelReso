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
        private DataView myView = null;

        //global variable for date in the date time format
        //if you want to use current date just add today.Date
        //if you want to use current time just add today.TimeOfDay
        private DateTime today = DateTime.Today;

        private int rowIndex = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getData();
            getResosForCurrentDay(today.Date);
            dg1.Click += dg1_Click;
            txtGuestsNo.KeyPress += txtGuestsNo_KeyPress;
        }

        //method which ensures correct input (1-8) for number of people in the reservation
        void txtGuestsNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (c != 8)
            {
                if (c < 49 || c > 56)
                {
                    e.Handled = true;
                }
            }
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

                myView = new DataView(ds.Tables["tReservations"]);

                //adding a filter to only see today's reservations
                string todaysDate = today.ToLongDateString();
                //MessageBox.Show(todaysDate, "Today's Date");
                string filter = "Date = '" + todaysDate + "'";
                myView.RowFilter = filter;

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

        private void getResosForCurrentDay(DateTime today)
        {
            //string filter = "Date = '" + today.ToString() + "'";
            //ds.Tables["tReservations"].DefaultView.RowFilter = filter;

            //for(int i = 0; i < dg1.Rows.Count; i++)
            //{
            //    if(Convert.ToDateTime(dg1.Rows[i].Cells[0].Value).Date.Equals(today))
            //    {
            //        dg1.Rows[i].Visible = true;
            //        MessageBox.Show(today.ToString(), "todays date");
            //    }
            //    else
            //    {
            //        dg1.Rows[i].Visible = false;
            //    }
            //}

            //MessageBox.Show(today.ToString(), "todays date");
        }

        private void cmdInsert_Click(object sender, EventArgs e)
        {
            if(cmdInsert.Text.Equals("Return to Insert Mode"))
            {
                clearText();
                setControlState("i");
            }

            else if (cmdInsert.Text.Equals("Insert"))
            {
                if (dataGood())
                {
                    if (isValidReservation("i"))
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
                        MessageBox.Show("Reservation succesfully inserted", "Successful Reservation");
                    }
                }
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (dataGood())
            {
                if (isValidReservation("u"))
                {
                    DataRow dr = ds.Tables["tReservations"].Rows[rowIndex];
                    dr["TableNo"] = Convert.ToInt32(txtTableNum.Text);
                    dr["Date"] = datePicker.Text;
                    dr["Time"] = timePicker.Text;
                    dr["Duration"] = Convert.ToDouble(txtDuration.Text);
                    dr["Name"] = txtName.Text;
                    dr["Telephone"] = txtTel.Text;
                    dr["NumberOfGuests"] = txtGuestsNo.Text;

                    //ds.Tables["tReservations"].Rows.Add(dr);
                    updateDB();
                    setControlState("u/d");
                    MessageBox.Show("Reservation succesfully updated", "Successful Reservation");

                }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this reservation?", "Delete Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                ds.Tables["tReservations"].Rows[rowIndex].Delete();
                updateDB();
            }
            setControlState("i");
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

        //leap year validation not needed since we're using date pickers

        //private string isLeapYear(string year)
        //{
        //    string whichYear = null;

        //    if(Convert.ToInt32(year) % 4 != 0)
        //    {
        //        whichYear = "c";
        //    }
        //    else if(Convert.ToInt32(year) % 100 != 0 )
        //    {
        //        whichYear = "l";
        //    }
        //    else if(Convert.ToInt32(year) % 400 != 0)
        //    {
        //        whichYear = "c";
        //    }
        //    else
        //    {
        //        whichYear = "l";
        //    }
        //    return whichYear;
        //}

        private void clearText()
        {
           // datePicker.Text = "";
            timePicker.Text = "";
            txtTableNum.Text = "";
            txtDuration.Text = "2";
            txtName.Text = "";
            txtTel.Text = "";
            txtGuestsNo.Text = "";
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
                    //gets the duration hours of each existing reservation
                    int durationHours = Convert.ToInt32(dg1.Rows[i].Cells[3].Value);

                    //gets the duration hours of new reservation
                    int newResoDuration = Convert.ToInt32(txtDuration.Text);
                    
                    //make a timespan new timespan struct using the duratino hours for i reservation
                    TimeSpan duration = new TimeSpan(durationHours, 0, 0);
                    TimeSpan newDuration = new TimeSpan(newResoDuration, 0, 0);

                    //add the duration timespan to reservation i start time
                    DateTime currentResoStartTime = Convert.ToDateTime(dg1.Rows[i].Cells[1].Value.ToString());
                    DateTime currentResoEndTime = currentResoStartTime.Add(duration);

                    //add the duration timespan to rescord being inserted
                    DateTime newResoEndTime = timePicker.Value.Add(newDuration);

                    //compare the reservation i end time to incoming reservation start time
                    //compare the reservation i start time to incoming reservation end time
                    int compareStartTime = DateTime.Compare(timePicker.Value, currentResoEndTime);
                    int compareEndTime = DateTime.Compare(newResoEndTime, currentResoStartTime);

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
                            else if(compareStartTime < 0 && compareEndTime > 0)
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
            if (state.Equals("u"))
            {
                //criteria
                //can't change the start time if an earlier reservation exist for that table
                //can't change table number if a table is alreay occupied
                //can't increase guest number if there are no tables left to allocate OR if increaseing guests will exceed the maximum capacity of 32

                for (int i = 0; i < dg1.Rows.Count; i++)
                {
                    DateTime selectedResoStartTime = timePicker.Value;
                    
                    if (i != dg1.CurrentRow.Index)
                    {
                        //gets the duration hours of each existing reservation
                        int durationHours = Convert.ToInt32(dg1.Rows[i].Cells[3].Value);

                        //gets the duration hours of new reservation
                        int newResoDuration = Convert.ToInt32(txtDuration.Text);

                        //make a timespan new timespan struct using the duratino hours for i reservation
                        TimeSpan duration = new TimeSpan(durationHours, 0, 0);
                        TimeSpan newDuration = new TimeSpan(newResoDuration, 0, 0);

                        //add the duration timespan to reservation i start time
                        DateTime currentResoStartTime = Convert.ToDateTime(dg1.Rows[i].Cells[1].Value.ToString());
                        DateTime currentResoEndTime = currentResoStartTime.Add(duration);

                        //add the duration timespan to rescord being inserted
                        DateTime newResoEndTime = timePicker.Value.Add(newDuration);

                        //compare the reservation i end time to incoming reservation start time
                        //compare the reservation i start time to incoming reservation end time
                        int compareStartTime = DateTime.Compare(timePicker.Value, currentResoEndTime);
                        int compareEndTime = DateTime.Compare(newResoEndTime, currentResoStartTime);

                        if (datePicker.Text.Equals(dg1.Rows[i].Cells[0].Value.ToString()))
                        {
                            if (txtTableNum.Text.Equals(dg1.Rows[i].Cells[2].Value.ToString()))
                            {
                                if (timePicker.Text.Equals(dg1.Rows[i].Cells[1].Value.ToString()))
                                {
                                    MessageBox.Show("A reservation already exists for this table at this time. Please select a different time or a different table", "Invalid Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtTableNum.Focus();
                                    return false;
                                }
                                // compareTime returns -1 if t1 is earlier than t2
                                else if (compareStartTime < 0 && compareEndTime > 0)
                                {

                                    string message = "An reservation alreayd exists for this table that starts at " + currentResoStartTime + " and finishes at " + currentResoEndTime.ToShortTimeString() + ". Please select a later time or a different table";
                                    MessageBox.Show(message, "Invalid Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtTableNum.Focus();
                                    return false;
                                    //if (!txtDuration.Text.Equals("2"))
                                    //{
                                    //    MessageBox.Show(txtDuration.Text, "Duration Changed");
                                    //    TimeSpan newDuration = new TimeSpan(Convert.ToInt32(txtDuration.Text), 0, 0);
                                    //    DateTime newEndTime = selectedResoStartTime.Add(newDuration);
                                    //    int compareNewTime = DateTime.Compare(currentResoStartTime, newEndTime);
                                    //    if (compareNewTime < 0)
                                    //    {
                                    //        MessageBox.Show("A reservation already exists for this table that starts before this reservation ends. Please select another table", "Invalid Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //    }
                                    //    txtTableNum.Focus();
                                    //    return false;

                                    //}
                                    //else
                                    //{
                                        
                                    //}
                                }
                               
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
                cmdInsert.Text = "Insert";
                cmdUpdate.Enabled = false;
                cmdDelete.Enabled = false;
                clearText();
            }
            if (state.Equals("u/d"))
            {
                cmdInsert.Enabled = true;
                cmdInsert.Text = "Return to Insert Mode";
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
            txtGuestsNo.Text = dg1.CurrentRow.Cells[6].Value.ToString();
            setControlState("u/d");
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            //*** DO NOT DELETE***//
            //trying to display reservations for the selected date
            //DateTime selectedDate = datePicker.Value.Date;
            //for (int i = 0; i < dg1.Rows.Count; i++)
            //{

            //    if(selectedDate.CompareTo(Convert.ToDateTime(dg1.Rows[i].Cells[0].Value).Date) == 0)
            //    {
            //        dg1.Rows[i].Visible = true;
            //    }
            //    else
            //    {
            //        dg1.Rows[i].Visible = false;
            //    }
            //}
            
            string selectedDate = datePicker.Value.Date.ToLongDateString();
            string filter = "Date = '" + selectedDate + "'";
            //MessageBox.Show(filter, "Selected Date");
            myView.RowFilter = filter;
            setControlState("i");
            //no need to bind ??
            dg1.DataSource = myView;
            dg1.ClearSelection();

        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        
        //add form load
    }
}
