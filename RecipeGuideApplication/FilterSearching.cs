using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeGuideApplication
{
    public partial class FilterSearching : Form
    {
        public FilterSearching()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inc_desc = "";
            string filter_style = "";

            if (radioButton1.Checked ) { inc_desc = "ASC"; }
            else if (radioButton2.Checked ) { inc_desc = "DESC"; }
            else 
            {
                MessageBox.Show("Lütfen filtreleme bileşenlerini eksiksiz doldurun.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (comboBox1.Text == "ID'ye göre") { filter_style = "RecipeID"; }
            else if (comboBox1.Text == "Hazırlama süresine göre") { filter_style = "RecipeTime"; }
            else if (comboBox1.Text == "Isme göre") { filter_style = "RecipeName"; }
            else if (comboBox1.Text == "Kategoriye göre") { filter_style = "RecipeCategory"; }
            else
            {
                MessageBox.Show("Lütfen filtreleme bileşenlerini eksiksiz doldurun.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (!string.IsNullOrEmpty(inc_desc) && !string.IsNullOrEmpty(filter_style))
            {
                SendFilterValues(inc_desc, filter_style);
                this.Close();
            }

        }

        private void SendFilterValues(string incDesc, string filterStyle)
        {
            if (Owner is Form1 mainForm)
            {
                mainForm.ApplyFilter(incDesc, filterStyle);
            }
        }
    }
}
