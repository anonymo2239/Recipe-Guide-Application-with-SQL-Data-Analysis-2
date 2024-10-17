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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

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
            combobox_guncelle();
        }

        /// DATABASE ISLEMLERI

        SqlConnection baglanti_my_ingredients = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True");

        void listele()
        {
            baglanti_my_ingredients.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Tbl_Material ORDER BY MaterialID ASC", baglanti_my_ingredients);
            SqlDataReader reader = cmd2.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti_my_ingredients.Close();
        }
        void temizle()
        {
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();
        }
        void combobox_guncelle()
        {
            comboBox1.Items.Clear();
            baglanti_my_ingredients.Open();

            SqlCommand cmd2 = new SqlCommand("SELECT MaterialName FROM Tbl_Material", baglanti_my_ingredients);
            SqlDataReader reader2 = cmd2.ExecuteReader();

            while (reader2.Read())
            {
                comboBox1.Items.Add(reader2[0].ToString());
            }
            baglanti_my_ingredients.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string richTextBoxContent = richTextBox1.Text.Trim();
            string[] lines = richTextBoxContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 0)
            {
                Dictionary<string, double> userMaterials = new Dictionary<string, double>();
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && double.TryParse(parts[1], out double quantity))
                    {
                        userMaterials[parts[0]] = quantity;
                    }
                }

                if (userMaterials.Count > 0)
                {
                    string valuesPlaceholders = string.Join(",", userMaterials.Select((_, index) => $"(@p{index}, @q{index})"));
                    string query = $@"
            WITH UserMaterials AS (
                SELECT M.MaterialID, t.MaterialName, t.Quantity
                FROM (VALUES {valuesPlaceholders}) AS t(MaterialName, Quantity)
                JOIN Tbl_Material M ON t.MaterialName = M.MaterialName
            ),
            RecipeSummary AS (
                SELECT R.RecipeID,
                        STRING_AGG(M.MaterialName + ' (' + CAST(R.MaterialQuantity AS VARCHAR) + ')', ', ') AS AllMaterials,
                        COUNT(DISTINCT R.MaterialID) AS TotalMaterialsNeeded,
                        COUNT(DISTINCT UM.MaterialID) AS UserMaterialsCount,
                        SUM(CASE 
                            WHEN UM.MaterialID IS NULL THEN R.MaterialQuantity * M.MaterialUnitPrice
                            WHEN R.MaterialQuantity > UM.Quantity THEN (R.MaterialQuantity - UM.Quantity) * M.MaterialUnitPrice
                            ELSE 0 
                        END) / 1000.0 AS MissingCost
                FROM Tbl_Relation R
                JOIN Tbl_Material M ON R.MaterialID = M.MaterialID
                LEFT JOIN UserMaterials UM ON R.MaterialID = UM.MaterialID
                GROUP BY R.RecipeID
            )
            SELECT Rec.RecipeName AS [Tarif Adı],
                    RS.AllMaterials AS [Tarifteki Malzemeler],
                    CASE 
                        WHEN RS.MissingCost = 0 THEN 'Var'
                        WHEN RS.MissingCost IS NULL THEN 'Bilinmiyor'
                        ELSE 'Eksik (Maliyet: ' + CAST(RS.MissingCost AS VARCHAR) + ')'
                    END AS [Malzeme Durumu],
                    CAST(CAST(RS.UserMaterialsCount AS FLOAT) / RS.TotalMaterialsNeeded * 100 AS DECIMAL(5,2)) AS [Eşleşme Oranı]
            FROM Tbl_Recipes Rec
            JOIN RecipeSummary RS ON Rec.RecipeID = RS.RecipeID
            WHERE RS.UserMaterialsCount > 0
            ORDER BY [Eşleşme Oranı] DESC, Rec.RecipeName";

                    using (SqlConnection connection = new SqlConnection(baglanti_my_ingredients.ConnectionString))
                    {
                        connection.Open();
                        using (SqlCommand cmd3 = new SqlCommand(query, connection))
                        {
                            int index = 0;
                            foreach (var material in userMaterials)
                            {
                                cmd3.Parameters.AddWithValue($"@p{index}", material.Key);
                                cmd3.Parameters.AddWithValue($"@q{index}", material.Value);
                                index++;
                            }

                            using (SqlDataReader reader2 = cmd3.ExecuteReader())
                            {
                                DataTable dt2 = new DataTable();
                                dt2.Load(reader2);

                                dataGridView2.DataSource = dt2;
                                dataGridView2.Show();
                            }
                        }
                    }

                    ConfigureDataGridViewColumns2();

                    // Satır renklerini ayarla
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.Cells["Malzeme Durumu"].Value != null)
                        {
                            string durum = row.Cells["Malzeme Durumu"].Value.ToString();
                            if (durum.StartsWith("Var"))
                            {
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
                            }
                            else if (durum.StartsWith("Eksik"))
                            {
                                row.DefaultCellStyle.BackColor = Color.Salmon;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.Gray;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Geçerli bir malzeme adı ve miktarı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("RichTextBox boş.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

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

        private void ConfigureDataGridViewColumns2()
        {
            if (dataGridView2.Columns.Count >= 4)
            {
                // "Tarif Adı" sütunu
                dataGridView2.Columns[0].Width = (int)(dataGridView2.Width * 0.25);
                dataGridView2.Columns[0].HeaderText = "Tarif Adı";

                // "Tarifteki Malzemeler" sütunu
                dataGridView2.Columns[1].Width = (int)(dataGridView2.Width * 0.25);
                dataGridView2.Columns[1].HeaderText = "Tarifteki Malzemeler";

                // "Malzeme Durumu" sütunu
                dataGridView2.Columns[2].Width = (int)(dataGridView2.Width * 0.25);
                dataGridView2.Columns[2].HeaderText = "Malzeme Durumu";

                // "Eşleşme Oranı" sütunu
                dataGridView2.Columns[3].Width = (int)(dataGridView2.Width * 0.15);
                dataGridView2.Columns[3].HeaderText = "Eşleşme Oranı";
                dataGridView2.Columns[3].DefaultCellStyle.Format = "N2";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox5.Text) || string.IsNullOrWhiteSpace(comboBox2.Text) || string.IsNullOrWhiteSpace(textBox7.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            baglanti_my_ingredients.Open();
            SqlCommand cmd = new SqlCommand("insert into Tbl_Material(MaterialName, MaterialTotal, MaterialUnit, MaterialUnitPrice) values (@p1, @p2, @a3, @a4)", baglanti_my_ingredients);
            cmd.Parameters.AddWithValue("@p1", textBox4.Text);
            cmd.Parameters.AddWithValue("@p2", textBox5.Text);
            cmd.Parameters.AddWithValue("@a3", comboBox2.Text);
            cmd.Parameters.AddWithValue("@a4", textBox7.Text);
            cmd.ExecuteNonQuery();

            baglanti_my_ingredients.Close();
            MessageBox.Show("Malzeme başarıyla eklendi", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            listele();
            temizle();
            combobox_guncelle();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Lütfen silinecek malzemenin üstüne çift tıklayın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            baglanti_my_ingredients.Open();
            SqlCommand cmdDel = new SqlCommand("delete from Tbl_Material where MaterialID = @m1", baglanti_my_ingredients);
            cmdDel.Parameters.AddWithValue("@m1", textBox3.Text);
            cmdDel.ExecuteNonQuery();
            baglanti_my_ingredients.Close();
            MessageBox.Show("Kayıt silindi", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            listele();
            temizle();
            combobox_guncelle();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Lütfen güncellenecek malzemenin üstüne çift tıklayın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            baglanti_my_ingredients.Open();
            SqlCommand cmdUpt = new SqlCommand("Update Tbl_Material set MaterialName = @alp1, MaterialTotal = @alp2, MaterialUnitPrice = @alp3, MaterialUnit = @alp4 where MaterialID = @alp7", baglanti_my_ingredients);
            cmdUpt.Parameters.Add("@alp1", textBox4.Text);
            cmdUpt.Parameters.Add("@alp2", textBox5.Text);
            cmdUpt.Parameters.Add("@alp3", textBox7.Text);
            cmdUpt.Parameters.Add("@alp4", comboBox2.Text);
            cmdUpt.Parameters.Add("@alp7", textBox3.Text);
            cmdUpt.ExecuteNonQuery();
            baglanti_my_ingredients.Close();
            MessageBox.Show("Başarıyla güncellendi", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            listele();
            combobox_guncelle();
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
            comboBox2.Text = dataGridView1.Rows[selected].Cells[3].Value.ToString();
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

        private void button10_Click(object sender, EventArgs e)
        {
            temizle();
        }

    }
}
