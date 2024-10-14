using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RecipeGuideApplication
{
    public partial class arrange_recipe : Form
    {
        public arrange_recipe()
        {
            InitializeComponent();
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
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // ID sütunundaki bilgiyi textBox1'e yerleştir
                textBox1.Text = row.Cells["ID"].Value.ToString();

                // Name sütunundaki bilgiyi textBox2'ye yerleştir
                comboBox1.Text = row.Cells["Name"].Value.ToString();

                // Category sütunundaki bilgiyi comboBox1'e yerleştir
                comboBox1.Text = row.Cells["Category"].Value.ToString();

                // Time sütunundaki bilgiyi maskedTextBox1'e yerleştir
                maskedTextBox1.Text = row.Cells["Time"].Value.ToString();

                // Instruction sütunundaki bilgiyi richTextBox1'e yerleştir
                richTextBox1.Text = row.Cells["Instruction"].Value.ToString();
            }
        }
    }
}