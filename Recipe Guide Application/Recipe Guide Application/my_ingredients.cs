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

namespace Recipe_Guide_Application
{
    public partial class my_ingredients : Form
    {
        public my_ingredients()
        {
            InitializeComponent();
        }

        SqlConnection baglanti3 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS; Initial Catalog=DbRecipeApplication; Integrated Security=True");
        SqlConnection baglanti4 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS; Initial Catalog=DbRecipeApplication; Integrated Security=True");

        private void my_ingredients_Load(object sender, EventArgs e)
        {
            baglanti3.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti3);
            SqlDataReader reader = cmd2.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.DefaultCellStyle.Font = new Font("Montserrat", 18);
            dataGridView1.Show();

            baglanti3.Close();

            baglanti4.Open();
            SqlCommand cmd3 = new SqlCommand("SELECT * FROM Tbl_Relation", baglanti4);
            SqlDataReader reader2 = cmd3.ExecuteReader();

            DataTable dt2 = new DataTable();
            dt2.Load(reader2);
            dataGridView2.DataSource = dt2;
            dataGridView2.DefaultCellStyle.Font = new Font("Montserrat", 18);
            dataGridView2.Show();

            baglanti4.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
