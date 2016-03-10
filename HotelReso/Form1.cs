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
            timePicker.CustomFormat = "hh:mm";
            timePicker.Format = DateTimePickerFormat.Custom;
            timePicker.Text = "06:00";
            txtName.KeyPress += txtName_KeyPress;
            txtDuration.KeyPress += txtDuration_KeyPress;
            txtTel.KeyPress += txtTel_KeyPress;
            txtTableNum.KeyPress += txtTableNum_KeyPress;
            //timePicker.KeyDown += timePicker_KeyDown;
            //timePicker.KeyUp += timePicker_KeyUp;
        }

        void txtTableNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            int len = txtTableNum.Text.Length;
            txtTableNum.SelectionStart = len;
            if (c != 8)
            {
                if (c <49 || c > 54)
                {
                    e.Handled = true;
                }
            }
        }

        void txtTel_KeyPress(object sender, KeyPressEventArgs e)    //Phone Number KeyPress 
        {
            char c = e.KeyChar;
            int len = txtTel.Text.Length;
            txtTel.SelectionStart = len;
            if (c != 8)    //Check for backspace
            {
                if(len ==3 || len ==7)    //Check for -'s
                {
                    if (c != 45)
                    {
                        e.Handled = true;
                    }
                }
                else   //Check for numbers
                {
                    if (c<48 || c>57)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        void txtDuration_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (c != 8)
            {
                if (c < 50 || c > 56)
                {
                    e.Handled = true;
                }
            }
        }

        void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;

            if (c != 8) //allows backspace always!!
            {
                if ((c < 65 || c > 90) && (c < 97 || c > 122) && (c != 32))
                {
                    //not letter and not space, dont allow it
                    e.Handled = true;
                }
                if (len < 2)
                {
                    if (c == 32) //a space
                    {
                        e.Handled = true;
                    }
                    else if (len == 0 && (c > 96 && c < 123))
                    {
                        //lower case char
                        e.KeyChar = (char)(c - 32);
                    }
                    else if (len > 0 && (c > 64 && c < 91))
                    {
                        //upper case
                        e.KeyChar = (char)(c + 32);
                    }
                }
                if(len >= 2)
                {
                    if (((TextBox)sender).Text.IndexOf(" ") == -1)
                    {
                        //no space...char for first name
                        if (c > 64 && c < 91)
                        {
                            //upper case char
                            e.KeyChar = (char)(c + 32);
                        }
                    }
                    else if (c == 32) //space
                    {
                        if (((TextBox)sender).Text.IndexOf(" ") > -1)
                        {
                            //space exists
                            e.Handled = true;
                        }
                    }
                    else if (((TextBox)sender).Text.IndexOf(" ") == len - 1)
                    {
                        if (c > 96 && c < 123) //lower case
                        {
                            e.KeyChar = (char)(c - 32);
                        }
                    }
                    else if (((TextBox)sender).Text.IndexOf(" ") < len - 1)
                    {
                        //one or more chars after space
                        if (c > 64 && c < 91)
                        {
                            e.KeyChar = (char)(c + 32);
                        }
                    }
                }
            }
        }

        //void timePicker_KeyUp(object sender, KeyEventArgs e)
        //{            
            
            
        //}

        //void timePicker_KeyDown(object sender, KeyEventArgs e)
        //{
            

        //}        

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
                        if (updateDB())
                        {
                            MessageBox.Show("Reservation succesfully inserted", "Successful Reservation");
                        }
                        clearText();
                        
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
                    if(updateDB())
                    {
                         MessageBox.Show("Reservation succesfully updated", "Successful Reservation");
                    }
                    setControlState("u/d");
                   

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

        private bool updateDB()
        {
            try
            {
                conn.Open();
                da.Update(ds, "tReservations");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error Updating Database");
                return false;

            }
            finally
            {
                if(conn != null)
                {
                    conn.Close();
                }
            }
            return true;
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

                    //add the duration timespan to record being inserted
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

        private bool validateDuration(string durString)
        {
            int duration = Convert.ToInt32(durString);
            //reservationTime
            int resTime = timePicker.Value.Hour;
            if ((resTime + duration) > 12)
            {
                return false;
            }
            return true;
        }
        private bool isValidPhoneNumber ()
        {

            if (txtTel.Text.Length <1) // Empty Phone Number
            {
                MessageBox.Show("Phone number Required!", "Missing Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTel.Focus();
                return false;
            }
            if (txtTel.Text.Length !=12)   //Must be a full phone number
            {
                MessageBox.Show("Phone Numeber must be 12 digits", "Incorrect Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTel.Focus();
                return false;
            }
            return true;
        }
        private bool isValidTable ()
        { 
            if (txtTableNum.Text.Length <1) //Empty Table
            {
                MessageBox.Show("Table Number Required!", "Missing Table Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTableNum.Focus();
                return false;
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

          
            //grab time entered as a string
            string time = timePicker.Text;

            //establish the parameter the string will be split by
            char[] timeDelim = { ':' };

            //create string array to hold what is returned by the split method
            string[] timeValueSplit = { "" };

            //split the time up using : as a delimiter
            timeValueSplit = time.ToString().Split(timeDelim);

            //get the hour value to an int
            int compareHour = timePicker.Value.Hour;

            //compare if hour value is within acceptable range, if not display error message
            if (compareHour < 6 || compareHour > 10)
            {
                string message = "A reservation can only be made between 6:00 and 10:00";
                MessageBox.Show(message, "Invalid Reservation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //get the minute value to an int
            int compareMin = timePicker.Value.Minute;
            if (compareHour != 10)
            {
                if (compareMin < 15)
                {
                    timeValueSplit[1] = "00";
                    timePicker.Text = timeValueSplit[0] + ":" + timeValueSplit[1];
                }
                else if (compareMin >= 15 && compareMin < 30)
                {
                    timeValueSplit[1] = "30";
                    timePicker.Text = timeValueSplit[0] + ":" + timeValueSplit[1];
                }
                else if (compareMin > 30 && compareMin < 45)
                {
                    timeValueSplit[1] = "30";
                    timePicker.Text = timeValueSplit[0] + ":" + timeValueSplit[1];
                }
                else if (compareMin >= 45)
                {
                    timeValueSplit[1] = "00";
                    if (timePicker.Value.Hour < 10)
                    {
                        compareHour += 1;
                        timeValueSplit[0] = compareHour.ToString();
                        timePicker.Text = timeValueSplit[0] + ":" + timeValueSplit[1];
                    }
                }

            }
            else
            {
                timeValueSplit[1] = "00";
                timePicker.Text = timeValueSplit[0] + ":" + timeValueSplit[1];
            }   
                   
        }

        

    }
}
