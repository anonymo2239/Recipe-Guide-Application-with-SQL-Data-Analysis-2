using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RecipeGuideApplication
{
    public partial class recipe_detail : Form
    {
        public recipe_detail()
        {
            InitializeComponent();
            
        }

        private void recipe_detail_Load(object sender, EventArgs e)
        {
            
        }
        public string TarifAdi
        {
            get { return label5.Text; }
            set { label5.Text = value; }
        }

        public string Kategori
        {
            get { return label6.Text; }
            set { label6.Text = value; }
        }

        public string HazirlamaSuresi
        {
            get { return label7.Text; }
            set { label7.Text = value; }
        }

        public string HazirlanısTalimati
        {
            get { return label8.Text; }
            set { label8.Text = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tarifAdi = label5.Text;

            using (SqlConnection conn = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS;Initial Catalog=DbRecipeApplication2;Integrated Security=True"))
            {
                conn.Open();

                try
                {
                    int recipeId = GetRecipeId(conn, tarifAdi);
                    if (recipeId == -1) return;

                    var ingredients = GetIngredients(conn, recipeId);
                    foreach (var ingredient in ingredients)
                    {
                        if (ingredient.CurrentTotal < ingredient.RequiredQuantity)
                        {
                            MessageBox.Show($"Malzeme ID: {ingredient.MaterialId} için yetersiz miktar. Mevcut: {ingredient.CurrentTotal}, Gerekli: {ingredient.RequiredQuantity}");
                            return;
                        }

                        long newTotal = ingredient.CurrentTotal - ingredient.RequiredQuantity;
                        UpdateMaterialQuantity(conn, ingredient.MaterialId, newTotal);
                    }

                    MessageBox.Show("Tarif pişirildi ve malzemeler güncellendi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}\nStack Trace: {ex.StackTrace}");
                }
            }
        }

        private int GetRecipeId(SqlConnection conn, string recipeName)
        {
            string queryRecipeId = "SELECT RecipeID FROM Tbl_Recipes WHERE RecipeName = @RecipeName";
            using (SqlCommand cmd = new SqlCommand(queryRecipeId, conn))
            {
                cmd.Parameters.AddWithValue("@RecipeName", recipeName);
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    MessageBox.Show("Belirtilen tarif bulunamadı.");
                    return -1;
                }
                return Convert.ToInt32(result);
            }
        }

        private List<Ingredient> GetIngredients(SqlConnection conn, int recipeId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            string queryIngredients = @"
    SELECT rm.MaterialID, rm.MaterialQuantity, m.MaterialTotal, m.MaterialUnit
    FROM Tbl_Relation rm
    JOIN Tbl_Material m ON rm.MaterialID = m.MaterialID
    WHERE rm.RecipeID = @RecipeID";

            using (SqlCommand cmd = new SqlCommand(queryIngredients, conn))
            {
                cmd.Parameters.AddWithValue("@RecipeID", recipeId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredients.Add(new Ingredient
                        {
                            MaterialId = reader.GetInt32(0),
                            RequiredQuantity = Convert.ToInt64(reader.GetValue(1)),
                            CurrentTotal = Convert.ToInt64(reader.GetValue(2)),
                            Unit = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                        });
                    }
                }
            }
            return ingredients;
        }

        private void UpdateMaterialQuantity(SqlConnection conn, int materialId, long newQuantity)
        {
            string updateQuery = "UPDATE Tbl_Material SET MaterialTotal = @NewQuantity WHERE MaterialID = @MaterialID";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@NewQuantity", newQuantity);
                cmd.Parameters.AddWithValue("@MaterialID", materialId);
                cmd.ExecuteNonQuery();
            }
        }

        private class Ingredient
        {
            public int MaterialId { get; set; }
            public long RequiredQuantity { get; set; }
            public long CurrentTotal { get; set; }
            public string Unit { get; set; }
        }

    }
}
