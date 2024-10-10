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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
    }
}
