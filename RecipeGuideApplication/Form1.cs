using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeGuideApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
            textBox1.BorderStyle = BorderStyle.Fixed3D;

            baglanti.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti.Close();

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

        private void button1_Click(object sender, EventArgs e)
        {
            my_ingredients my_Ingredients = new my_ingredients();
            my_Ingredients.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            arrange_recipe arrange_Recipe = new arrange_recipe();
            arrange_Recipe.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            arrange_recipe arrange_Recipe2 = new arrange_recipe();
            arrange_Recipe2.Show();
            this.Hide();
        }

        SqlConnection baglanti2 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");

        private void pictureBox1_Click(object sender, EventArgs e)
        {   
           
            string searched = textBox1.Text.ToLower();

            string query = @"SELECT * FROM Tbl_Recipes
                WHERE (RecipeName LIKE @SearchPattern)
                OR (RecipeCategory LIKE @SearchPattern)
                OR (RecipeInstruction LIKE @SearchPattern)";

            baglanti.Open();
            SqlCommand cmd2 = new SqlCommand(query, baglanti);
            cmd2.Parameters.AddWithValue("@SearchPattern", "%" + searched + "%");

            try
            {
                SqlDataReader reader = cmd2.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
                dataGridView1.Show();
            }
            finally
            {
                baglanti.Close();
            }

            ConfigureDataGridViewColumns();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Aradığınız yemek tarifini buraya girin...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Aradığınız yemek tarifini buraya girin...";
                textBox1.ForeColor = SystemColors.ScrollBar;
            }
        }

        public void ApplyFilter(string incDesc, string filterStyle)
        {
            string query = $"SELECT * FROM Tbl_Recipes ORDER BY {filterStyle} {incDesc}";

            baglanti.Open();
            SqlCommand cmd = new SqlCommand(query, baglanti);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
                dataGridView1.Show();
            }
            finally
            {
                baglanti.Close();
            }

            ConfigureDataGridViewColumns();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FilterSearching filterSearching = new FilterSearching();
            filterSearching.Owner = this;
            filterSearching.Show();
        }
    }
}
