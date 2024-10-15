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

namespace RecipeGuideApplication
{
    public partial class my_ingredients : Form
    {
        public my_ingredients()
        {
            InitializeComponent();
        }

        SqlConnection baglanti3 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");
        private void my_ingredients_Load(object sender, EventArgs e)
        {
            baglanti3.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Material", baglanti3);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti3.Close();

            ConfigureDataGridViewColumns();
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count >= 5)
            {
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.05);
                dataGridView1.Columns[0].HeaderText = "ID";

                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.2);
                dataGridView1.Columns[1].HeaderText = "Name";

                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.25);
                dataGridView1.Columns[2].HeaderText = "Category";

                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.1);
                dataGridView1.Columns[3].HeaderText = "Time";

                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.60);
                dataGridView1.Columns[4].HeaderText = "Instruction";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }

    }
}
