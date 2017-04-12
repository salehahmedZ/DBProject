﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Store_Management_System
{
    public partial class frmOrders : Form
    {
        public frmOrders()
        {
            InitializeComponent();
        }

        private void txtGameID_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
           // {

           // }
        }
        private void frmOrders_Load(object sender, EventArgs e)
        {
            
        }

        private void txtGameID_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (txtGameID.Text.Trim() == "" || int.Parse(txtGameID.Text.Trim()) == 0)
            {

                MessageBox.Show("Please enter a valid Game ID");
                txtGameID.Focus();


            }
            else
            {


                string id = txtGameID.Text;
                string qu = txtQu.Text;


                string GameID = "";
                string GameName = "";
                string GamePrice = "";
                string Platform = "";

                SqlCommand cmd2 = frmLogin.sqlDBConnection.CreateCommand();
                cmd2.CommandText = "SELECT * FROM Game WHERE ID = @GameID;";
                cmd2.Parameters.AddWithValue("GameID", id);


                try
                {
                    SqlDataReader reader2 = cmd2.ExecuteReader();

                    if (reader2.HasRows)
                    {



                        if (reader2.Read())
                        {

                            GameID = reader2[0].ToString();
                            GameName = reader2[1].ToString();
                            GamePrice = reader2[2].ToString();
                            Platform = reader2[4].ToString();

                        }

                        decimal total = Decimal.Parse(GamePrice) * Decimal.Parse(qu);

                        string[] game = { GameID, GameName, Platform, GamePrice, qu, total.ToString() };

                        grdOrderList.Rows.Add(game);

                        reader2.Close();
                        computeTotal();
                    }
                    else
                    {
                        reader2.Close();
                        MessageBox.Show("Ther enterd Game ID does not exist!");
                        txtGameID.Focus();
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }


        }

        private void grdOrderList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (grdOrderList.CurrentRow != null)
            {
                string Qu = grdOrderList.Rows[grdOrderList.CurrentRow.Index].Cells[4].Value.ToString();

                string Price = grdOrderList.Rows[grdOrderList.CurrentRow.Index].Cells[3].Value.ToString();

                decimal total = decimal.Parse(Price) * decimal.Parse(Qu);
                grdOrderList.Rows[grdOrderList.CurrentRow.Index].Cells[5].Value = total;

                computeTotal();
            }
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                if (createInvoice())
                {
                    createOrders();
                }
            }
        }


        public bool Validation()
        {

            if (grdOrderList.Rows.Count == 0)
            {

                MessageBox.Show("There are no games in the grid");
                return false;
            }


            if (txtCusID.Text.Trim() == "")
            {

                MessageBox.Show("Please enter a valid Customer ID");
                return false;
            }

            if (!IsTheCustomerExist(txtCusID.Text.Trim()))
            {

                MessageBox.Show("Ther enterd customer ID does not exist!");
                return false;
            }


            return true;
        }

        public void computeTotal()
        {
            decimal totalF = 0;
            foreach (DataGridViewRow row in grdOrderList.Rows)
            {
                if (row.Cells[5].Value != null)
                {
                    totalF += decimal.Parse(row.Cells[5].Value.ToString());
                }
            }

            txtTotal.Text = totalF.ToString();
        }


        public bool createInvoice()
        {
            string cusID = txtCusID.Text;
            string total = txtTotal.Text;

            SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
            cmd.CommandText = "INSERT INTO Invoice(Customer_ID,Username,Total,Date) OUTPUT INSERTED.Invoice_NO VALUES(@CusID,@Username,@Total,@Date)";

            cmd.Parameters.AddWithValue("CusID", cusID);
            cmd.Parameters.AddWithValue("Username", frmLogin.userUsername);
            cmd.Parameters.AddWithValue("Total", total);
            cmd.Parameters.AddWithValue("Date", DateTime.Now.ToString());
            try
            {
                Int32 i = (Int32)cmd.ExecuteScalar();
                lblInovice.Text = i.ToString();
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void createOrders()
        {
            string Invoice_NO = lblInovice.Text;

            foreach (DataGridViewRow row in grdOrderList.Rows)
            {
                string Game_ID = row.Cells[0].Value.ToString();
                string Quantity = row.Cells[4].Value.ToString();
                string Price = row.Cells[5].Value.ToString();

                SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO Orders(Invoice_NO,Game_ID,Quantity,Price) VALUES (@Invoice_NO,@Game_ID,@Quantity,@Price)";

                cmd.Parameters.AddWithValue("Invoice_NO", Invoice_NO);
                cmd.Parameters.AddWithValue("Game_ID", Game_ID);
                cmd.Parameters.AddWithValue("Quantity", Quantity);
                cmd.Parameters.AddWithValue("Price", Price);
                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool IsTheCustomerExist(string ID)
        {
            SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Customer WHERE ID = @ID";

            cmd.Parameters.AddWithValue("ID", int.Parse(ID));
            try
            {
                int i = (int)cmd.ExecuteScalar();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool IsTheGameExist(string ID)
        {
            SqlCommand cmd = frmLogin.sqlDBConnection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Game WHERE ID = @ID";

            cmd.Parameters.AddWithValue("ID", int.Parse(ID));
            try
            {
                int i = (int)cmd.ExecuteScalar();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void txtCusID_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keypress = e.KeyChar;
            if (!char.IsDigit(keypress) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                e.Handled=true;
            }
        }

        private void txtGameID_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keypress = e.KeyChar;
            if (!char.IsDigit(keypress) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                e.Handled = true;
            }
        }
    }
}
