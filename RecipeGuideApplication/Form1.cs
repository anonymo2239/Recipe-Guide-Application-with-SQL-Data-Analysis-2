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
            textBox1.KeyPress += textBox1_KeyPress;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
            textBox1.BorderStyle = BorderStyle.Fixed3D;

            baglanti_form1.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti_form1);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti_form1.Close();

            ConfigureDataGridViewColumns();
        }

        /// DATABASE ISLEMLERI

        SqlConnection baglanti_form1 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            string searched = textBox1.Text.ToLower();

            string query = @"SELECT * FROM Tbl_Recipes
                WHERE (RecipeName LIKE @SearchPattern)
                OR (RecipeCategory LIKE @SearchPattern)
                OR (RecipeInstruction LIKE @SearchPattern)";

            baglanti_form1.Open();
            SqlCommand cmd2 = new SqlCommand(query, baglanti_form1);
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
                baglanti_form1.Close();
            }

            ConfigureDataGridViewColumns();
        }

        public void ApplyFilter_recipe(string incDesc, string filterStyle)
        {
            string query = $"SELECT * FROM Tbl_Recipes ORDER BY {filterStyle} {incDesc}";

            baglanti_form1.Open();
            SqlCommand cmd = new SqlCommand(query, baglanti_form1);

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
                baglanti_form1.Close();
            }

            ConfigureDataGridViewColumns();
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count >= 5)
            {
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.05);
                dataGridView1.Columns[0].HeaderText = "ID";

                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.2);
                dataGridView1.Columns[1].HeaderText = "Tarif Adı";

                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.25);
                dataGridView1.Columns[2].HeaderText = "Kategori";

                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.1);
                dataGridView1.Columns[3].HeaderText = "Süre";

                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.60);
                dataGridView1.Columns[4].HeaderText = "Talimat";
            }
        }

        /// CLICK FONKSIYONLARI

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

        private void button4_Click(object sender, EventArgs e)
        {
            FilterSearching filterSearching = new FilterSearching(false);
            filterSearching.Owner = this;
            filterSearching.Show();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Aradığınız yemek tarifini buraya girin...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        /// DIGER FONKSIYONLAR

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Aradığınız yemek tarifini buraya girin...";
                textBox1.ForeColor = SystemColors.ScrollBar;
            }
        }     

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                pictureBox1_Click(this, new EventArgs());
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                recipe_detail recipe_Detail = new recipe_detail();
                recipe_Detail.Owner = this;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                recipe_Detail.TarifAdi = row.Cells[1].Value.ToString(); // "Tarif Adı" sütunu
                recipe_Detail.Kategori = row.Cells[2].Value.ToString(); // "Kategori" sütunu
                recipe_Detail.HazirlamaSuresi = row.Cells[3].Value.ToString() + " dakika"; // "Süre" sütunu
                recipe_Detail.HazirlanısTalimati = row.Cells[4].Value.ToString(); // "Talimat" sütunu

                recipe_Detail.Show();
            }
        }
    }
}
