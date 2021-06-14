using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Configuration;

namespace GK_POS
{
    public partial class Form1 : Form
    {
        AutoCompleteStringCollection collCustomerName = new AutoCompleteStringCollection();
        AutoCompleteStringCollection collCustomerCode = new AutoCompleteStringCollection();
        AutoCompleteStringCollection collProductName = new AutoCompleteStringCollection();

        OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        //OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Mangrio\\Documents\\gk.accdb");

        OleDbDataAdapter da;
        OleDbCommand cmd;
        OleDbCommand cmd2;
        public Form1()
        {
            InitializeComponent();
        }


        public void AutoCustomerName()
        {
            da = new OleDbDataAdapter("SELECT CUSTOMER_NAME FROM TBL_CUSTOMERS ORDER BY CUSTOMER_NAME ASC", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    collCustomerName.Add(dt.Rows[i]["CUSTOMER_NAME"].ToString());
                }
            }
            else
            {
                MessageBox.Show("Record not found");
            }

            txtCustName.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtCustName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtCustName.AutoCompleteCustomSource = collCustomerName;

        }

        public void FillCustomerData()
        {
            da = new OleDbDataAdapter("SELECT * FROM TBL_CUSTOMERS WHERE CUSTOMER_NAME='" + txtCustName.Text + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                //dataGridView1.DataSource = dt;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    txtCustName.Text = dt.Rows[i]["CUSTOMER_NAME"].ToString();
                    txtCustCode.Text = dt.Rows[i]["CUSTOMER_CODE"].ToString();
                    txtAddress.Text = dt.Rows[i]["CUSTOMER_ADDRESS"].ToString();
                }
            }
            txtName.Focus();
        }

        public void AutoCustomerCode()
        {
            da = new OleDbDataAdapter("SELECT CUSTOMER_CODE FROM TBL_CUSTOMERS ORDER BY CUSTOMER_CODE ASC", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    collCustomerCode.Add(dt.Rows[i]["CUSTOMER_CODE"].ToString());
                }
            }
            else
            {
                MessageBox.Show("Record not found");
            }

            txtCustCode.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtCustCode.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtCustCode.AutoCompleteCustomSource = collCustomerCode;

        }

        public void FillCustomerCodeData()
        {
            da = new OleDbDataAdapter("SELECT * FROM TBL_CUSTOMERS WHERE CUSTOMER_CODE='" + txtCustCode.Text + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                //dataGridView1.DataSource = dt;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    txtCustName.Text = dt.Rows[i]["CUSTOMER_NAME"].ToString();
                    txtCustCode.Text = dt.Rows[i]["CUSTOMER_CODE"].ToString();
                    txtAddress.Text = dt.Rows[i]["CUSTOMER_ADDRESS"].ToString();
                }
            }

            txtName.Focus();
        }

        public void AutoProductName()
        {
            da = new OleDbDataAdapter("SELECT BRAND_NAME FROM TBL_PRODUCTS ORDER BY BRAND_NAME ASC", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    collProductName.Add(dt.Rows[i]["BRAND_NAME"].ToString());
                }
            }
            else
            {
                MessageBox.Show("Record not found");
            }

            txtName.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtName.AutoCompleteCustomSource = collProductName;

        }

        public void FillProductData()
        {
            da = new OleDbDataAdapter("SELECT * FROM TBL_PRODUCTS WHERE BRAND_NAME='" + txtName.Text + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    txtName.Text = dt.Rows[i]["BRAND_NAME"].ToString();
                    txtPacking.Text = dt.Rows[i]["PACKING"].ToString();
                    txtRate.Text = dt.Rows[i]["RATE"].ToString();
                    lblRetailPrice.Text = dt.Rows[i]["RETAIL_PRICE"].ToString();
                    txtPurchased.Text = dt.Rows[i]["PURCHASED_DISCOUNT"].ToString()+"%";
                    txtDiscount.Text = "0";
                    txtQty.Text = "";
                  

                    if (txtQty.Text != "")
                    {
                        decimal amount = calculateAmount(txtName.Text, decimal.Parse(txtQty.Text), decimal.Parse(txtDiscount.Text));
                        txtTotal.Text = amount.ToString();
                    }
                }
                txtDiscount.Focus();
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            AutoCustomerName(); //calling the auto method. 
            AutoCustomerCode();
            AutoProductName();
            txtName.Focus();
        }

        private void txtCustCode_TextChanged(object sender, EventArgs e)
        {
            //FillCustomerCodeData();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            addRowInGrid();
        }

        private void addRowInGrid()
        {
            if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtRate.Text) && !string.IsNullOrEmpty(txtQty.Text))
            {

                string name = txtName.Text;
                string packing = !string.IsNullOrWhiteSpace(txtPacking.Text) ? txtPacking.Text : "N/A";
                decimal rate = decimal.Parse(txtRate.Text);
                decimal retailPrice = lblRetailPrice.Text!= "" ? decimal.Parse(lblRetailPrice.Text) : 0;
                decimal discounted = txtDiscount.Text != string.Empty ? decimal.Parse(txtDiscount.Text) : 0 ;
                decimal qty = decimal.Parse(txtQty.Text);

                decimal total = rate * qty;
                decimal amount = calculateAmount(name, qty, discounted, rate);

                decimal discount = total - amount;


                if (!string.IsNullOrEmpty(rowIndex.Text))
                {
                    int index = Convert.ToInt32(rowIndex.Text);
                    this.dataGridView1.Rows[index].Cells["PName"].Value = name;
                    this.dataGridView1.Rows[index].Cells["Column1"].Value = packing;
                    this.dataGridView1.Rows[index].Cells["Column2"].Value = rate;
                    this.dataGridView1.Rows[index].Cells["Column3"].Value = qty;
                    this.dataGridView1.Rows[index].Cells["Column4"].Value = amount;
                    this.dataGridView1.Rows[index].Cells["Column5"].Value = discounted + "%"; //discount;
                    this.dataGridView1.Rows[index].Cells["Column6"].Value = retailPrice;
                }
                else
                {
                    this.dataGridView1.Rows.Add(name, packing, rate, qty, discounted + "%", amount, retailPrice);
                }

                decimal gTotal = GetTotal();
                txtTotal.Text = gTotal.ToString();

                txtName.Clear();
                txtPacking.Clear();
                txtRate.Clear();
                txtQty.Clear();
                txtDiscount.Clear();
                txtPurchased.Text = "0";
                txtPayment.Text = "0";
                txtBalance.Text = "0";
                lblRetailPrice.Text = "";
                rowIndex.Text = string.Empty;

                txtName.Focus();
            }
            else
            {
                MessageBox.Show("Please add complete product details");
            }
        }

        private decimal GetTotal()
        {
            decimal grandTotal = 0;
            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                grandTotal = grandTotal + Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column4"].Value);

            }
            return Math.Round(grandTotal, 2);
        }


        private decimal calculateAmount(string name, decimal qty, decimal discount, decimal rate = 0)
        {
            decimal total = 0;
            if (txtName.Text != "" && txtQty.Text != "")
            {
                da = new OleDbDataAdapter("SELECT * FROM TBL_PRODUCTS WHERE BRAND_NAME='" + name + "'", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string brandName = dt.Rows[i]["BRAND_NAME"].ToString();
                        rate = rate > 0 ? rate : Convert.ToDecimal(dt.Rows[i]["RATE"]);
                        //decimal discountAmount = Convert.ToDecimal(dt.Rows[i]["DISCOUNT_AMOUNT"]);
                        //decimal discountPercentage = Convert.ToDecimal(dt.Rows[i]["DISCOUNT_PERCENTAGE"]);

                        rate = rate - (rate * (discount / 100));
                        total = rate * qty;
                    }
                }
                else
                {
                    decimal newRate = decimal.Parse(txtRate.Text) - (decimal.Parse(txtRate.Text) * (discount / 100));
                    total = newRate * qty;
                }
            }

            return Math.Round(total, 2);
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        { 
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }


        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["delete"].Index && e.RowIndex >= 0)
            {
                dataGridView1.Rows.Remove(dataGridView1.Rows[e.RowIndex]);
                decimal grandTotal = 0;
                for (int row = 0; row < dataGridView1.Rows.Count; row++)
                {
                    grandTotal = grandTotal + Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column4"].Value);
                    txtTotal.Text = grandTotal.ToString();
                }
                if (dataGridView1.Rows.Count == 0)
                {
                    txtTotal.Text = "0";
                }
            }

            if (e.ColumnIndex == dataGridView1.Columns["edit"].Index && e.RowIndex >= 0)
            {
                string name = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["PName"].Value);
                string packing = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Column1"].Value);
                string rate = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Column2"].Value);
                string qty = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Column3"].Value);
                string discounted = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Column5"].Value);
                string retailPrice = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value);

                int index = e.RowIndex;
                rowIndex.Text = index.ToString();

                txtName.Text = name;
                txtPacking.Text = packing;
                txtRate.Text = rate;
                txtQty.Text = qty;
                txtDiscount.Text = discounted.Split('%')[0];
                lblRetailPrice.Text = retailPrice;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 && !string.IsNullOrEmpty(txtPayment.Text) && txtPayment.Text != "0")
            {
                DialogResult dialogResult = MessageBox.Show("Do want to save this record", "Confirm", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string customerName = txtCustName.Text;
                    decimal total = decimal.Parse(txtTotal.Text);
                    decimal payment = decimal.Parse(txtPayment.Text);
                    decimal balance = decimal.Parse(txtBalance.Text);
                    decimal grandTotal = 0;
                    decimal grandQty = 0;
                    string total_in_words = words(Convert.ToDouble(total));

                    for (int row = 0; row < dataGridView1.Rows.Count; row++)
                    {
                        grandTotal += (Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column2"].Value) * Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column3"].Value));
                        //grandDiscount += Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column5"].Value);
                    }

                    for (int row = 0; row < dataGridView1.Rows.Count; row++)
                    {
                        grandQty += Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column3"].Value);
                        //grandDiscount += Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column5"].Value);
                    }

                    string sqlQuery = "INSERT INTO TBL_SALES (SUB_TOTAL, PAYMENT, BALANCE, GRAND_TOTAL, CUSTOMER_NAME, TOTAL_ITEMS, TOTAL_QUANTITY, AMOUNT_WORDS ) " +
                                    "VALUES (@subTotal, @payment, @balance, @grandTotal, @customer, @totalItem, @totalQty, @amountInWords)";
                    con.Open();
                    cmd = new OleDbCommand(sqlQuery, con);
                    cmd.Parameters.AddWithValue("@subTotal", OleDbType.VarChar).Value = total.ToString();
                    cmd.Parameters.AddWithValue("@payment", OleDbType.VarChar).Value = payment.ToString();
                    cmd.Parameters.AddWithValue("@balance", OleDbType.VarChar).Value = balance.ToString();
                    cmd.Parameters.AddWithValue("@grandTotal", OleDbType.VarChar).Value = grandTotal.ToString();
                    cmd.Parameters.AddWithValue("@customer", OleDbType.VarChar).Value = customerName.ToString();
                    cmd.Parameters.AddWithValue("@totalItem", OleDbType.VarChar).Value = dataGridView1.Rows.Count.ToString();
                    cmd.Parameters.AddWithValue("@totalQty", OleDbType.VarChar).Value = grandQty.ToString();
                    cmd.Parameters.AddWithValue("@amountInWords", OleDbType.VarChar).Value = total_in_words.ToUpper();
                    
                    int lastId = 0;
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 1)
                    {
                        OleDbCommand cmdId = con.CreateCommand();
                        cmdId.CommandText = "SELECT @@IDENTITY;";
                        lastId = (int)cmdId.ExecuteScalar();
                    }

                    for (int row = 0; row < dataGridView1.Rows.Count; row++)
                    {
                        string name = Convert.ToString(dataGridView1.Rows[row].Cells["PName"].Value);
                        string packing = Convert.ToString(dataGridView1.Rows[row].Cells["Column1"].Value);
                        decimal rate = Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column2"].Value);
                        decimal qty = Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column3"].Value);
                        decimal gTotal = rate * qty;
                        decimal subtotal = Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column4"].Value);
                        string discount = Convert.ToString(dataGridView1.Rows[row].Cells["Column5"].Value);
                        decimal retailPrice = Convert.ToDecimal(dataGridView1.Rows[row].Cells["Column6"].Value);

                        string sqlQuery2 = "INSERT INTO TBL_PRODUCT_SALES (SALES_ID, BRAND_NAME, RATE, QUANTITY, TOTAL, PAYABLE, DISCOUNT, PACKING, RETAIL_PRICE, SN) " +
                                    "VALUES (@salesId, @brandName, @rate, @qty, @total, @payable, @discount, @packing, @retail, @sn)";
                        //con.Open();
                        cmd2 = new OleDbCommand(sqlQuery2, con);
                        cmd2.Parameters.AddWithValue("@salesId", OleDbType.VarChar).Value = lastId.ToString();
                        cmd2.Parameters.AddWithValue("@brandName", OleDbType.VarChar).Value = name.ToString();
                        cmd2.Parameters.AddWithValue("@rate", OleDbType.VarChar).Value = rate.ToString();
                        cmd2.Parameters.AddWithValue("@qty", OleDbType.VarChar).Value = qty.ToString();
                        cmd2.Parameters.AddWithValue("@total", OleDbType.VarChar).Value = gTotal.ToString();
                        cmd2.Parameters.AddWithValue("@payable", OleDbType.VarChar).Value = subtotal.ToString();
                        cmd2.Parameters.AddWithValue("@discount", OleDbType.VarChar).Value = discount.ToString();
                        cmd2.Parameters.AddWithValue("@packing", OleDbType.VarChar).Value = packing.ToString();
                        cmd2.Parameters.AddWithValue("@retail", OleDbType.VarChar).Value = retailPrice.ToString();
                        cmd2.Parameters.AddWithValue("@sn", OleDbType.VarChar).Value = (row+1).ToString();
                        cmd2.ExecuteNonQuery();

                    }
                    con.Close();
                    MessageBox.Show("Success");
                    dataGridView1.Rows.Clear();
                    txtPurchased.Text = "0";
                    txtTotal.Text = "0";
                    txtPayment.Text = "0";
                    txtBalance.Text = "0";
                    txtQty.Text = "0";
                    lblRetailPrice.Text = "";
                    txtCustCode.Text = "CUST-0000";
                    txtCustName.Text = "Cash Counter";
                    txtAddress.Text = "Cash Counter";

                    PrintView p = new PrintView();
                    p.SalesId = lastId;
                    p.CustomerName = customerName;
                    p.Show();
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }

            }
            else
            {
                MessageBox.Show("Please add payments details");
            }
        }

        public static string words(double? numbers, Boolean paisaconversion = false)
        {

            var pointindex = numbers.ToString().IndexOf(".");
            var paisaamt = 0;
            if (pointindex > 0 && paisaconversion)
                paisaamt = Convert.ToInt32(numbers.ToString().Substring(pointindex + 1, 2));

            int number = Convert.ToInt32(numbers);

            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // croress
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }

            if (paisaamt == 0 && paisaconversion == false)
            {
                sb.Append("ruppes only");
            }
            else if (paisaamt > 0)
            {
                var paisatext = words(paisaamt, true);
                sb.AppendFormat("rupees {0} paise only", paisatext);
            }
            return sb.ToString().TrimEnd();
        }

        private void txtDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            decimal gTotal = GetTotal();
            if (e.KeyChar == 13)
            {
              
                txtTotal.Text = gTotal.ToString();

                if (!string.IsNullOrEmpty(txtPayment.Text) || txtPayment.Text != "0")
                {
                    decimal payment = decimal.Parse(txtPayment.Text);
                    txtBalance.Text = (payment - gTotal).ToString("0.##");
                }


                txtPayment.Focus();
            }
        }

        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            if (e.KeyChar == 13)
            {
                decimal total = decimal.Parse(txtTotal.Text);
                decimal payment = decimal.Parse(txtPayment.Text);
                txtBalance.Text = (payment - total).ToString("0.##");
                button2.Focus();
            }
        }


        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillProductData();
            }
        }

        private void txtDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtQty.Focus();
            }
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addRowInGrid();
            }
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtCustCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillCustomerCodeData();
            }
        }

        private void txtCustName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillCustomerData();
            }
        }

    }

}
