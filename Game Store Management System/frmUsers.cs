﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Game_Store_Management_System
{
    public partial class frmUsers : Form
    {
        public frmUsers()
        {
            InitializeComponent();
        }


        //Load all users to the data grid 
        private void showAllUsers()
        {
            SqlCommand cmd2 = frmLogin.sqlDBConnection.CreateCommand();
            cmd2.CommandText = "SELECT username AS Username,FName As 'First name', LName As 'Last name',type AS Type FROM Staff WHERE Username != 'sys';";

            try
            {
                SqlDataReader reader2 = cmd2.ExecuteReader();

                DataTable tmpTable = new DataTable();
                tmpTable.Load(reader2);

                dgUsers.DataSource = tmpTable;
                reader2.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void usersForm_Load(object sender, EventArgs e)
        {
            showAllUsers();
        }


        //Validate add new user form
        private bool validateAddNewUserForm()
        {
            string Username = txtUsername.Text;
            string FName = txtName.Text;
            string Password = frmLogin.GetHashString(txtPassword.Text);
            string Type = cmbType.Text;

            if (Username.Trim().Length < 3)
            {

                MessageBox.Show("Please enter more than 3 characters for the Username.");

                return false;

            }

            if (Name.Trim().Length < 3)
            {

                MessageBox.Show("Please enter more than 3 characters for the FName.");

                return false;

            }

                if (txtPassword.Text.Length < 3)
                {

                    MessageBox.Show("Please enter more than 3 characters for the Password.");

                    return false;

                
            }
            return true;

        }


        private bool validateEditUserForm()
        {
            string Username = txtUsername.Text;
            string FName = txtName.Text;
            string Password = frmLogin.GetHashString(txtPassword.Text);
            string Type = cmbType.Text;

            if (Username.Trim().Length < 3)
            {

                MessageBox.Show("Please enter more than 3 characters for the Username.");

                return false;

            }

            if (Name.Trim().Length < 3)
            {

                MessageBox.Show("Please enter more than 3 characters for the FName.");

                return false;

            }
            if (ckPass.Checked)
            {
                if (txtPassword.Text.Length < 3)
                {

                    MessageBox.Show("Please enter more than 3 characters for the Password.");

                    return false;

                }
            }
            return true;

        }


        private void btnSave_Click(object sender, EventArgs e)
        {


            if (validateAddNewUserForm())
            {

                string Username = txtUsername.Text;
                string FName = txtName.Text;
                string LName = txtLName.Text;
                string Password = frmLogin.GetHashString(txtPassword.Text);
                string Type = cmbType.Text;


                SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO Staff VALUES(@Username,@Password,@FName,@LName,@Type)";
                cmd.Parameters.AddWithValue("Username", Username.Trim());
                cmd.Parameters.AddWithValue("FName", FName.Trim());
                cmd.Parameters.AddWithValue("LName", LName.Trim());
                cmd.Parameters.AddWithValue("Password", Password);
                cmd.Parameters.AddWithValue("Type", Type);
                try
                {
                    cmd.ExecuteScalar();

                    txtUsername.Text = "";
                    txtName.Text = "";
                    txtLName.Text = "";
                    txtPassword.Text = "";


                    btnAdd.Visible = true;
                    gbadd.Visible = false;
                    btnEdit.Visible = true;

                    showAllUsers();
                    

                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            string Username = dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[0].Value.ToString();
            string FName =   dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[1].Value.ToString();
            string LName =  dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[2].Value.ToString();
            string Type =   dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[3].Value.ToString();

            if (Username == "sys")
            {

                MessageBox.Show("This account cannot be deleted");
            }
            else if (frmLogin.userUsername == Username)
            {

                MessageBox.Show("You can not delete youself");
            }
            else
            {

                if (MessageBox.Show("Are you sure you want to delete this user?\n\nUsername\t=" + Username + "\nName\t=" + FName + "\nType\t=" + Type, "title", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {

                    SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
                    cmd.CommandText = "DELETE FROM Staff WHERE username = @Username;";
                    cmd.Parameters.AddWithValue("Username", Username);

                    try
                    {
                        cmd.ExecuteScalar();

                        showAllUsers();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {

            string Username = txtUsername.Text;
            string OldUsername = dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[0].Value.ToString();

            string FName = txtName.Text;
            string LName = txtLName.Text;
            string Password = frmLogin.GetHashString(txtPassword.Text);
            string Type = cmbType.Text;



            SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
            if (validateEditUserForm())
            {

                if (ckPass.Checked == true)
                {
                    cmd.CommandText = "Update Staff SET Username=@Username, FName=@FName,LName=@LName, Password=@Password,Type=@Type WHERE Username=@OldUsername;";
                    cmd.Parameters.AddWithValue("Password", Password);
                }
                else if  (ckPass.Checked == false) //txtPassword.Text.Trim()!=""
                {

                    cmd.CommandText = "Update Staff SET Username=@Username, FName=@FName, LName=@LName,Type=@Type WHERE Username=@OldUsername;";
                }


                cmd.Parameters.AddWithValue("OldUsername", OldUsername);
                cmd.Parameters.AddWithValue("Username", Username.Trim());
                cmd.Parameters.AddWithValue("FName", FName.Trim());
                cmd.Parameters.AddWithValue("LName", LName.Trim());
                cmd.Parameters.AddWithValue("Type", Type);
                try
                {
                   cmd.ExecuteNonQuery();


                    txtUsername.Text = "";
                    txtName.Text = "";
                    txtLName.Text = "";
                    txtPassword.Text = "";


                    btnAdd.Visible = true;
                    gbadd.Visible = false;

                    showAllUsers();
                    btnEdit.Visible = true;

                    ckPass.Visible = false;

                    txtUsername.Enabled = true;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        


        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            SqlCommand cmd2 = frmLogin.sqlDBConnection.CreateCommand();
            cmd2.CommandText = @"Select Username AS Username,FName As 'First name',LName As 'Last name',type AS
                                 Type from staff where Username != 'sys'AND (Username LIKE @Search or FName LIKE @Search or
                                LName LIKE @Search or type LIKE @Search);";
       
            cmd2.Parameters.AddWithValue("@Search","%" + search + "%");
            try
            {
                SqlDataReader reader2 = cmd2.ExecuteReader();

                DataTable tmpTable = new DataTable();
                tmpTable.Load(reader2);

                dgUsers.DataSource = tmpTable;
                reader2.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            gbadd.Visible = false;
            btnEdit.Visible = true;
            btnAdd.Visible = true;
            ckPass.Visible = false;
            txtUsername.Enabled = true;
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            btnEdit.Visible = false;
            btnUpdate.Visible = false;
            gbadd.Visible = true;
            btnAdd.Visible = false;
            btnSave.Visible = true;

            txtName.Text = "";
            txtLName.Text = "";
            txtPassword.Text = "";
            txtUsername.Text = "";

            txtPassword.Enabled = true; 
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {

            ckPass.Visible = true;
            btnAdd.Visible = false;
            btnEdit.Visible = false;
            string Username = dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[0].Value.ToString();
            string FName = dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[1].Value.ToString();
            string LName = dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[2].Value.ToString();
            string Type = dgUsers.Rows[dgUsers.CurrentRow.Index].Cells[3].Value.ToString();

            txtUsername.Text = Username;
            txtName.Text = FName;
            txtLName.Text = LName;
            txtPassword.Text = "";
            cmbType.Text = Type;

            gbadd.Visible = true;

            btnSave.Visible = false;
            btnUpdate.Visible = true;

            txtUsername.Enabled = true;
        }

        private void ckPass_CheckedChanged(object sender, EventArgs e)
        {
            if (ckPass.Checked)
            {
                txtPassword.Enabled = true;
            }else
            {
                txtPassword.Enabled = false;

            }
        }

    }
}
