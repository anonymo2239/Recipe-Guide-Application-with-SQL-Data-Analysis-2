﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recipe_Guide_Application
{
    public partial class arrange_recipe : Form
    {

        SqlConnection baglanti2 = new SqlConnection("Data Source=LAPTOP-E82TE7I7\\SQLEXPRESS; Initial Catalog=DbRecipeApplication; Integrated Security=True");
        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti2.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Tbl_Recipes", baglanti2);
            SqlDataReader reader = cmd2.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            dataGridView1.DataSource = dt;
            dataGridView1.Show();

            baglanti2.Close();
        }
        public arrange_recipe()
        {
            InitializeComponent();
        }
    }
}