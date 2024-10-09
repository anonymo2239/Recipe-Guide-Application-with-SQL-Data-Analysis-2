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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti1 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS; Initial Catalog=DbRecipeApplication; Integrated Security=True");
        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti1.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti1);
            SqlDataReader reader = cmd2.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.DefaultCellStyle.Font = new Font("Montserrat", 18);
            dataGridView1.Show();

            baglanti1.Close();
        }


        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            arrange_recipe arrange_recipe = new arrange_recipe();
            arrange_recipe.Show();
            this.Hide();
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            my_ingredients my_Ingredients = new my_ingredients();
            my_Ingredients.Show();
            this.Hide();
        }
    }
}
