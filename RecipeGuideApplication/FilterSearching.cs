using System;
using System.Windows.Forms;

namespace RecipeGuideApplication
{
    public partial class FilterSearching : Form
    {
        private bool isFromMyIngredients;

        public FilterSearching(bool fromMyIngredients)
        {
            InitializeComponent();
            isFromMyIngredients = fromMyIngredients;
            SetupComboBox();
        }

        private void SetupComboBox()
        {
            comboBox1.Items.Clear();
            if (isFromMyIngredients)
            {
                comboBox1.Items.AddRange(new string[] { "ID'ye göre", "Malzeme adına göre", "Dolabımdaki miktara göre", "Birim fiyata göre" });
            }
            else
            {
                comboBox1.Items.AddRange(new string[] { "ID'ye göre", "Hazırlama süresine göre", "Isme göre", "Kategoriye göre" });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inc_desc = "";
            string filter_style = "";

            if (radioButton1.Checked) { inc_desc = "ASC"; }
            else if (radioButton2.Checked) { inc_desc = "DESC"; }
            else
            {
                MessageBox.Show("Lütfen sıralama yönünü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            filter_style = GetFilterStyle();

            if (string.IsNullOrEmpty(filter_style))
            {
                MessageBox.Show("Lütfen bir filtreleme kriteri seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(inc_desc) && !string.IsNullOrEmpty(filter_style))
            {
                SendFilterValues(inc_desc, filter_style);
                this.Close();
            }
        }

        private string GetFilterStyle()
        {
            if (isFromMyIngredients)
            {
                switch (comboBox1.Text)
                {
                    case "ID'ye göre": return "MaterialID";
                    case "Malzeme adına göre": return "MaterialName";
                    case "Dolabımdaki miktara göre": return "MaterialTotal";
                    case "Birim fiyata göre": return "MaterialUnitPrice";
                    default: return "";
                }
            }
            else
            {
                switch (comboBox1.Text)
                {
                    case "ID'ye göre": return "RecipeID";
                    case "Hazırlama süresine göre": return "RecipeTime";
                    case "Isme göre": return "RecipeName";
                    case "Kategoriye göre": return "RecipeCategory";
                    default: return "";
                }
            }
        }

        private void SendFilterValues(string incDesc, string filterStyle)
        {
            if (Owner is Form1 mainForm)
            {
                mainForm.ApplyFilter_recipe(incDesc, filterStyle);
            }
            else if (Owner is my_ingredients myIngredients)
            {
                myIngredients.ApplyFilter_material(incDesc, filterStyle);
            }
        }
    }
}