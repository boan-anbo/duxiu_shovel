using System;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;
using DuxiuShovel.Main;

namespace Gui
{
    public partial class Main : Form
    {
        private Service service = new Service();
        public Main()
        {
            InitializeComponent();
            InitializeOpenFileDialog();

            this.service.datagrid = dataGridView1;
            
        }
        private void InitializeOpenFileDialog()
        {
            openFileDialog1.Filter = "Zip (*.ZIP); Rar (*.rar)|*.ZIP;*.zip;*.rar; *.RAR";            //  Allow the user to select multiple images.
            openFileDialog1.Multiselect = true;     
            openFileDialog1.Title = "Choose Zip Files or Directory";
        }





        private async void button1_Click(object sender, EventArgs e)
        {
            processButton.Enabled = false;
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                var files = openFileDialog1.FileNames;
                if (files.Length > 0)
                { 
                    service.AddRows(files);
                    processButton.Enabled = true;
                }
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            service.SaveJson();


        }

        private async void button3_Click(object sender, EventArgs e)
        {
            service.ProcessFiles(openFileDialog1.FileNames);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}