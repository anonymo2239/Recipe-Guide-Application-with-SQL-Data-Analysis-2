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
using System.Xml.Linq;

namespace RecipeGuideApplication
{
    public partial class my_ingredients : Form
    {
        public my_ingredients()
        {
            InitializeComponent();
            textBox2.KeyPress += textBox2_KeyPress;
        }

        private void my_ingredients_Load(object sender, EventArgs e)
        {
            baglanti_my_ingredients.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Material", baglanti_my_ingredients);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti_my_ingredients.Close();

            ConfigureDataGridViewColumns();

            baglanti_my_ingredients.Open();

            SqlCommand cmd2 = new SqlCommand("SELECT MaterialName FROM Tbl_Material", baglanti_my_ingredients);
            SqlDataReader reader2 = cmd2.ExecuteReader();

            while (reader2.Read())
            {
                comboBox1.Items.Add(reader2[0].ToString());
            }
            baglanti_my_ingredients.Close();
        }

        /// DATABASE ISLEMLERI

        SqlConnection baglanti_my_ingredients = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string searched = textBox2.Text.ToLower();

            string query = @"SELECT * FROM Tbl_Material
                WHERE (MaterialName LIKE @SearchPattern)
                OR (MaterialUnit LIKE @SearchPattern)
                OR (MaterialUnitPrice LIKE @SearchPattern)";

            baglanti_my_ingredients.Open();
            SqlCommand cmd2 = new SqlCommand(query, baglanti_my_ingredients);
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
                baglanti_my_ingredients.Close();
            }

            ConfigureDataGridViewColumns();
        }

        public void ApplyFilter_material(string incDesc, string filterStyle)
        {
            string query = $"SELECT * FROM Tbl_Material ORDER BY {filterStyle} {incDesc}";

            baglanti_my_ingredients.Open();
            SqlCommand cmd = new SqlCommand(query, baglanti_my_ingredients);

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
                baglanti_my_ingredients.Close();
            }

            ConfigureDataGridViewColumns();
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count >= 5)
            {
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.15);
                dataGridView1.Columns[0].HeaderText = "ID";

                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.25);
                dataGridView1.Columns[1].HeaderText = "Malzeme ismi";

                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.20);
                dataGridView1.Columns[2].HeaderText = "Dolabımdaki Miktar";

                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.15);
                dataGridView1.Columns[3].HeaderText = "Birim";

                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.60);
                dataGridView1.Columns[4].HeaderText = "Birim Fiyat";
            }
        }

        /// CLICK FONKSIYONLARI

        private void button3_Click(object sender, EventArgs e)
        {
            FilterSearching filterSearching2 = new FilterSearching(true);
            filterSearching2.Owner = this;
            filterSearching2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "Aradığınız malzemeyi buraya girin...")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Marketten yeni ürün aldıysanız veya eski ürünlerinizde eksilme varsa buradan dolabınızdaki malzemeleri düzenlemelisiniz.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int selected = dataGridView1.SelectedCells[0].RowIndex;
            textBox3.Text = dataGridView1.Rows[selected].Cells[0].Value.ToString();
            textBox4.Text = dataGridView1.Rows[selected].Cells[1].Value.ToString();
            textBox5.Text = dataGridView1.Rows[selected].Cells[2].Value.ToString();
            textBox6.Text = dataGridView1.Rows[selected].Cells[3].Value.ToString();
            textBox7.Text = dataGridView1.Rows[selected].Cells[4].Value.ToString();
        }

        /// DIGER FONKSIYONLAR

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Aradığınız malzemeyi buraya girin...";
                textBox2.ForeColor = SystemColors.ScrollBar;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                pictureBox2_Click(this, new EventArgs());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            baglanti_my_ingredients.Open();

            SqlCommand cmd3 = new SqlCommand("SELECT MaterialTotal, MaterialUnit FROM Tbl_Material WHERE MaterialName = @p1", baglanti_my_ingredients);
            cmd3.Parameters.AddWithValue("@p1", comboBox1.Text);
            SqlDataReader reader2 = cmd3.ExecuteReader();

            if (reader2.Read())
            {
                textBox1.Text = reader2[0].ToString();
                textBox8.Text = reader2[1].ToString();
            }

            baglanti_my_ingredients.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string newEntry = comboBox1.Text + "     " + textBox1.Text + " " + textBox8.Text;

            if (!richTextBox1.Text.Contains(comboBox1.Text))
            {
                if (richTextBox1.Text != "")
                {
                    richTextBox1.AppendText("\n" + newEntry);
                }
                else
                {
                    richTextBox1.AppendText(newEntry);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}
