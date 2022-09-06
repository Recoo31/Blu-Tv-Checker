using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RecooChecker.Form1;

namespace RecooChecker
{
    public partial class Hitlist : Form
    {
        public Hitlist()
        {
            InitializeComponent();
        }

        private void hit_grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void hit_grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void hit_grid_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
        DataTable table = new DataTable();
        private void Hitlist_Load(object sender, EventArgs e)
        {
            
            // add columns //
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Password", typeof(string));
            table.Columns.Add("Capture", typeof(string));

            hit_grid.DataSource = table;
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // get lines from the text file //
                string[] lines = File.ReadAllLines(@"Result\Hit.txt");
                string[] value;


                for (int super = 0; super < lines.Length; super++)
                {
                    value = lines[super].ToString().Split(':');
                    string[] ekle = new string[value.Length];

                    for (int superim1 = 0; superim1 < value.Length; superim1++)
                    {
                        ekle[superim1] = value[superim1].Trim();
                    }
                    table.Rows.Add(ekle);
                }
            }
            catch (Exception reco)
            {

                MessageBox.Show("Hit Yok!");
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            try
            {
                // get lines from the text file //
                string[] lines = File.ReadAllLines(@"Result\Free.txt");
                string[] value;


                for (int super = 0; super < lines.Length; super++)
                {
                    value = lines[super].ToString().Split(':');
                    string[] ekle = new string[value.Length];

                    for (int superim1 = 0; superim1 < value.Length; superim1++)
                    {
                        ekle[superim1] = value[superim1].Trim();
                    }
                    table.Rows.Add(ekle);
                }
            }
            catch (Exception reco)
            {

                MessageBox.Show("Free Yok!");
            }
        }
    }
}
