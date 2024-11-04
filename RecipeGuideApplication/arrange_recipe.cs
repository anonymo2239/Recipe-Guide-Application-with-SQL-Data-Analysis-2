using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace RecipeGuideApplication
{
    public partial class arrange_recipe : Form
    {

        List<string> secilenMalzemeler = new List<string>();

        public arrange_recipe()
        {
            InitializeComponent();
            
            combobox1Liste(comboBox1);
            combobox2Liste(comboBox2);
            SetupComboBox();
            textBox4.Text = "miktar giriniz(gr/ml)...";
            textBox4.ForeColor = Color.Gray;
        }

        SqlConnection baglanti = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");

        List<string> listBox2 = new List<string>();
        List<string> listBox4 = new List<string>();
        List<string> listBox1 = new List<string>();

        private void SetupComboBox()
        {
            comboBox2.Items.Insert(0, "Malzemeler");
            comboBox2.SelectedIndex = -1;
            comboBox2.Text = "Malzemeler";
            comboBox2.ForeColor = Color.Gray;
            comboBox2.DropDownHeight = 300;
            comboBox1.SelectedIndex = -1;
            comboBox1.ForeColor = Color.Black;
            comboBox1.DropDownHeight = 300;
        }

        public void combobox1Liste(System.Windows.Forms.ComboBox comboBox)
        {
            string connectionString = "Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True";
            string query = "SELECT DISTINCT RecipeCategory FROM Tbl_Recipes";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            comboBox.Items.Clear();  // Mevcut öğeleri temizle

                            while (reader.Read())
                            {
                                string recipeCategory = reader["RecipeCategory"].ToString();
                                comboBox.Items.Add(recipeCategory);
                            }
                        }
                    }
                }

                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;  // İlk öğeyi seç (isteğe bağlı)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanından veri çekerken bir hata oluştu: " + ex.Message);
            }
        }

        public void combobox2Liste(System.Windows.Forms.ComboBox comboBox)
        {
            string connectionString = "Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True";
            string query = "SELECT MaterialName FROM Tbl_Material";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            comboBox.Items.Clear();  // Mevcut öğeleri temizle

                            while (reader.Read())
                            {
                                string materialName = reader["MaterialName"].ToString();
                                comboBox.Items.Add(materialName);
                            }
                        }
                    }
                }

                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;  // İlk öğeyi seç (isteğe bağlı)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanından veri çekerken bir hata oluştu: " + ex.Message);
            }
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
            baglanti.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti);
            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti.Close();
            // Sütun genişliklerini ayarla ve isimlerini güncelle
            ConfigureDataGridViewColumns();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ConfigureDataGridViewColumns();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns.Count >= 5)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // 1. sütundaki bilgiyi label6'ya yazdır
                textBox2.Text = row.Cells[0].Value?.ToString() ?? "";

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

        private void RefreshDataGridView()
        {
            try
            {
                baglanti.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yenileme hatası: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            baglanti.Open();

            SqlCommand komut = new SqlCommand("UPDATE Tbl_Recipes SET RecipeName = @a1, RecipeCategory = @a2, RecipeTime = @a3,RecipeInstruction = @a4 WHERE RecipeID = @a5", baglanti);
            komut.Parameters.AddWithValue("@a1", textBox1.Text);
            komut.Parameters.AddWithValue("@a2", comboBox1.Text);
            komut.Parameters.AddWithValue("@a3", maskedTextBox1.Text);
            komut.Parameters.AddWithValue("@a4", richTextBox1.Text);
            komut.Parameters.AddWithValue("@a5", textBox2.Text);

            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Güncelleme Tamamlandı!");
            RefreshDataGridView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();

                // Tbl_Relation tablosundaki ilgili kayıtları sil
                SqlCommand deleteRelation = new SqlCommand("DELETE FROM Tbl_Relation WHERE RecipeID = @a5", baglanti);
                deleteRelation.Parameters.AddWithValue("@a5", textBox2.Text);
                deleteRelation.ExecuteNonQuery();

                // Tbl_Recipes tablosundaki kaydı sil
                SqlCommand deleteRecipe = new SqlCommand("DELETE FROM Tbl_Recipes WHERE RecipeID = @a5", baglanti);
                deleteRecipe.Parameters.AddWithValue("@a5", textBox2.Text);
                int result = deleteRecipe.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Silme İşlemi Tamamlandı!");
                }
                else
                {
                    MessageBox.Show("Belirtilen ID ile eşleşen bir kayıt bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }

            RefreshDataGridView();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // ComboBox2'den listBox2'ye aktarma
            if (comboBox2.SelectedItem != null)
            {
                listBox2.Add(comboBox2.SelectedItem.ToString());
            }

            // TextBox4'ten listBox4'e aktarma
            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                listBox4.Add(textBox4.Text);
            }
            // ComboBox2 ve TextBox4 değerlerini RichTextBox2'ye aktarma
            if (comboBox2.SelectedItem != null && !string.IsNullOrWhiteSpace(textBox4.Text))
            {
                string newEntry = $"{comboBox2.SelectedItem} - {textBox4.Text}";

                // Eğer RichTextBox2 boş değilse, yeni satıra geç
                if (!string.IsNullOrEmpty(richTextBox2.Text))
                {
                    richTextBox2.AppendText(Environment.NewLine);
                }

                richTextBox2.AppendText(newEntry);
            }
            // ComboBox2 ve TextBox4'ü temizleme
            comboBox2.SelectedIndex = -1;
            textBox4.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True"))
            {
                try
                {
                    baglanti.Open();

                    // TextBox1'deki değeri listBox1'e ekle
                    if (!listBox1.Contains(textBox1.Text))
                    {
                        listBox1.Add(textBox1.Text);
                    }

                    // Tarif adının veritabanında olup olmadığını kontrol et
                    string checkRecipeQuery = "SELECT COUNT(*) FROM Tbl_Recipes WHERE RecipeName = @recipeName";
                    using (SqlCommand checkCmd = new SqlCommand(checkRecipeQuery, baglanti))
                    {
                        checkCmd.Parameters.AddWithValue("@recipeName", textBox1.Text);
                        int recipeCount = (int)checkCmd.ExecuteScalar();

                        if (recipeCount > 0)
                        {
                            MessageBox.Show("Bu isimde bir tarif zaten mevcut. Lütfen farklı bir isim giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // İşlemi burada sonlandır
                        }
                    }

                    // Tbl_Recipes tablosuna veri ekleme
                    string insertRecipeQuery = "INSERT INTO Tbl_Recipes (RecipeName, RecipeCategory, RecipeTime, RecipeInstruction) VALUES (@p1, @p2, @p3, @p4); SELECT SCOPE_IDENTITY();";
                    int newRecipeId;

                    using (SqlCommand cmd = new SqlCommand(insertRecipeQuery, baglanti))
                    {
                        cmd.Parameters.AddWithValue("@p1", textBox1.Text);
                        cmd.Parameters.AddWithValue("@p2", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@p3", maskedTextBox1.Text);
                        cmd.Parameters.AddWithValue("@p4", richTextBox1.Text);

                        newRecipeId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Malzeme ve miktar verilerini işleme
                    for (int i = 0; i < listBox2.Count; i++)
                    {
                        string materialName = listBox2[i];
                        string quantity = listBox4[i];

                        // MaterialID'yi bulma
                        int materialId = GetMaterialId(baglanti, materialName);

                        if (materialId != -1)
                        {
                            // Tbl_Relation tablosuna veri ekleme
                            string insertRelationQuery = "INSERT INTO Tbl_Relation (MaterialQuantity, RecipeID, MaterialID) VALUES (@p1, @p2, @p3)";
                            using (SqlCommand cmd = new SqlCommand(insertRelationQuery, baglanti))
                            {
                                cmd.Parameters.AddWithValue("@p1", quantity);
                                cmd.Parameters.AddWithValue("@p2", newRecipeId);
                                cmd.Parameters.AddWithValue("@p3", materialId);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show($"'{materialName}' malzemesi bulunamadı.");
                        }
                    }

                    MessageBox.Show("Tarif ve ilişkili malzemeler başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }

        }

        private int GetMaterialId(SqlConnection connection, string materialName)
        {
            string query = "SELECT MaterialID FROM Tbl_Material WHERE MaterialName = @name";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@name", materialName);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private bool isUpdating = false;

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;

            isUpdating = true;
            if (comboBox2.SelectedIndex > 0)
            {
                comboBox2.ForeColor = Color.Black;
            }
            else
            {
                comboBox2.SelectedIndex = -1;
                comboBox2.Text = "Malzemeler";
                comboBox2.ForeColor = Color.Gray;
            }
            isUpdating = false;
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "miktar giriniz(gr/ml)...")
            {
                textBox4.Clear();
                textBox4.ForeColor = Color.Black;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;

            isUpdating = true;
            comboBox1.ForeColor = Color.Black;
            isUpdating = false;
            
        }
    }
}
