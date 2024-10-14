using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace RecipeGuideApplication
{
    public partial class arrange_recipe : Form
    {
        public arrange_recipe()
        {
            InitializeComponent();
            FillCategoryComboBox();
        }
        private void FillCategoryComboBox()
        {
                List<string> categories = new List<string>
            {
        
                "Ana Yemek",
                "Çorba",
                "Zeytinyağlı",
                "Tatlı",
                "Meze",
                "Fast Food",
                "Salata",
                "Börek",
                "Kahvaltılık",
                "İçecek"
            };

            comboBox1.DataSource = categories;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = -1;
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count >= 5)
            {
                // Sütun genişliklerini ayarla ve isimlerini güncelle
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.05);
                dataGridView1.Columns[0].HeaderText = "ID";

                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.15);
                dataGridView1.Columns[1].HeaderText = "Name";

                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.15);
                dataGridView1.Columns[2].HeaderText = "Category";

                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.05);
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

        private void arrange_recipe_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'dbRecipeApplicationDataSet.Tbl_Recipes' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.tbl_RecipesTableAdapter.Fill(this.dbRecipeApplicationDataSet.Tbl_Recipes);

            // Sütun genişliklerini ayarla ve isimlerini güncelle
            ConfigureDataGridViewColumns();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Veri bağlama tamamlandıktan sonra sütun genişliklerini ve isimlerini tekrar ayarla
            ConfigureDataGridViewColumns();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns.Count >= 5)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // RecipeName (2. sütun, indeks 1)
                textBox1.Text = row.Cells[1].Value?.ToString() ?? "";

                // RecipeCategory (3. sütun, indeks 2)
                comboBox1.Text = row.Cells[2].Value?.ToString() ?? "";

                // RecipeTime (4. sütun, indeks 3)
                maskedTextBox1.Text = row.Cells[3].Value?.ToString() ?? "";

                // RecipeInstruction (5. sütun, indeks 4)
                richTextBox1.Text = row.Cells[4].Value?.ToString() ?? "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-GU6MGQD\\SQLEXPRESS;Initial Catalog=DbRecipeApplication;Integrated Security=True;Encrypt=False;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Tbl_Recipes (RecipeName, RecipeCategory, RecipeTime, RecipeInstruction) " +
                               "VALUES (@RecipeName, @RecipeCategory, @RecipeTime, @RecipeInstruction)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecipeName", textBox1.Text);
                    command.Parameters.AddWithValue("@RecipeCategory", comboBox1.SelectedItem?.ToString() ?? "");
                    command.Parameters.AddWithValue("@RecipeTime", maskedTextBox1.Text);
                    command.Parameters.AddWithValue("@RecipeInstruction", richTextBox1.Text);

                    try
                    {
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Ekleme başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputs(); // Girdileri temizleyen metod (aşağıda tanımlanacak)
                        }
                        else
                        {
                            MessageBox.Show("Tarif eklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Girdileri temizleyen metod
        private void ClearInputs()
        {
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            maskedTextBox1.Clear();
            richTextBox1.Clear();
        }
    }
}