using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GK_POS
{
    public partial class PrintView : Form
    {
        OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        //OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Mangrio\\Documents\\gk.accdb");

        OleDbDataAdapter da;
        OleDbDataAdapter da2;
        OleDbDataAdapter da3;
        OleDbCommand cmd;
        OleDbCommand cmd2;
        OleDbCommand cmd3;
        public PrintView()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
        public int SalesId { get; set; }
        public string CustomerName { get; set; }
        private void PrintView_Load(object sender, EventArgs e)
        {
            con.Open();

            DataTable dt = new DataTable();
            cmd = new OleDbCommand("select * from TBL_SALES where ID = "+ SalesId, con);
            da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            string customerName = dt.Rows[0]["CUSTOMER_NAME"].ToString();

            DataTable dt2 = new DataTable();
            cmd2 = new OleDbCommand("select * from TBL_PRODUCT_SALES where SALES_ID = '" + SalesId + "'", con);
            da2 = new OleDbDataAdapter(cmd2);
            da2.Fill(dt2);


            DataTable dt3 = new DataTable();
            cmd3 = new OleDbCommand("select * from TBL_CUSTOMERS where CUSTOMER_NAME = '" + customerName + "'", con);
            da3 = new OleDbDataAdapter(cmd3);
            da3.Fill(dt3);

            CrystalReport1 cr = new CrystalReport1();
            cr.Database.Tables["TBL_SALES"].SetDataSource(dt);
            cr.Database.Tables["TBL_PRODUCT_SALES"].SetDataSource(dt2);
            cr.Database.Tables["TBL_CUSTOMERS"].SetDataSource(dt3);

            this.crystalReportViewer1.ReportSource = cr;
            cr.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"C:\GK\Reports\" + SalesId + ".pdf");
            cr.PrintToPrinter(1, false, 0, 0);


            con.Close();

        }
    }
}
