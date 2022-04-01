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
using System.Configuration;
using System.Data.SqlClient;

namespace DB1Lab
{
    public partial class Form1 : Form
    {

        string AusgabeId;
        string issnID;
        DataSet ds1;
        BindingSource bsAusgabe, bsZeitschrift;
        SqlDataAdapter daZeitschrift, daAusgabe;
        SqlCommandBuilder cb;
        public Form1()
        {
            InitializeComponent();
        }

        static string connectionString = ConfigurationManager.AppSettings["connectionString"];
        static string parentName = ConfigurationManager.AppSettings["parentTable"];
        static string childName = ConfigurationManager.AppSettings["childTable"];
        DataSet dynamicDataSet = new DataSet();



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            issnID = dataGridView1.Rows[e.RowIndex].Cells["ISSN"].Value.ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter adapter;
            Console.WriteLine(connectionString);
            string queryForIssnId = "select * from Ausgabe where ZeitschriftISSN="+issnID;
            Console.WriteLine(queryForIssnId);

            SqlCommand command = new SqlCommand(queryForIssnId, connection);
            command.Parameters.AddWithValue("@p", issnID);
            DataSet ds2 = new DataSet();


            //dataGridView2.DataSource = command.ExecuteNonQuery();
            try
            {
                dataGridView2.DataSource = ds1.Tables["Ausgabe"].Select(queryForIssnId);
            }
            catch(Exception ex)
            {
                Console.WriteLine();
            }

           
           
            

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            AusgabeId = dataGridView2.Rows[e.RowIndex].Cells["Id"].Value.ToString();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connectionString);

            if (AusgabeId != "")
            {
                SqlCommand cmd = new SqlCommand("delete Ausgabe where Id=@id", con);
                con.Open();
                cmd.Parameters.AddWithValue("@id", AusgabeId);
          
                cmd.ExecuteNonQuery();
                daAusgabe.Update(ds1, "Ausgabe");

                MessageBox.Show("Record Deleted Successfully!");
                ds1.Tables["Ausgabe"].Clear();
                SqlDataAdapter oneAdapt = new SqlDataAdapter("select * from Ausgabe", con);

                oneAdapt.Fill(ds1, "Ausgabe");
                try
                {
                   dataGridView2.DataSource = ds1.Tables["Ausgabe"].Select("select * from Ausgabe where ZeitschriftISSN=" + issnID);

                }
                catch(Exception ex)
                {
                    Console.WriteLine();
                }

                con.Close();
            }
            else
            {
                MessageBox.Show("Please Select Record to Delete");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            daAusgabe.Update(ds1, "Ausgabe");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            ds1 = new DataSet();

            string AusgabeQuery = "select * from Ausgabe";
            daAusgabe = new SqlDataAdapter(AusgabeQuery, connection);

            string ZeitschriftQuery = "select * from Zeitschrift";
            daZeitschrift = new SqlDataAdapter(ZeitschriftQuery, connection);

            cb = new SqlCommandBuilder(daAusgabe);
            
            daZeitschrift.Fill(ds1, "Zeitschrift");
            daAusgabe.Fill(ds1, "Ausgabe");
            
            ds1.Relations.Add("FK_Zeitschrift_Ausgabe", ds1.Tables["Zeitschrift"].Columns["ISSN"], ds1.Tables["Ausgabe"].Columns["ZeitschriftISSN"]);

            bsZeitschrift = new BindingSource();
            bsAusgabe = new BindingSource();
            
            bsZeitschrift.DataSource = ds1; 
            bsZeitschrift.DataMember = "Zeitschrift";

            bsAusgabe.DataSource = bsZeitschrift;
            bsAusgabe.DataMember = "FK_Zeitschrift_Ausgabe";

            dataGridView1.DataSource = bsZeitschrift;
            dataGridView2.DataSource = bsAusgabe;
        }
    }
}
