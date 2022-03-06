using DuxiuShovel.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gui
{
    class Service
    {
        internal DataGridView datagrid;
        private string[] files;

        public async void ProcessFiles(string[] files)
        {
            var shovel = new Shovel();

            await Task.Run(() =>
            {

                for (int i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var parentDirectory = Directory.GetParent(file).FullName;
                    try
                    {
                        var correctPassword = shovel.Zipper.UnzipOne(file, parentDirectory + Path.DirectorySeparatorChar + "unzipped");
                        MarkEntryEnded(i, correctPassword);
                    }
                    catch (Exception)
                    {
                        MarkEntryError(i);
                    }
                }
            });

  
            
            
        }


        public void TestRow()
        {
            UpdateStatus(0, "fuck", "wrong password");
        }

        private void ClearRows()
        {
            datagrid.Rows.Clear();
        }
        public void AddRows(string[] files)
        {
            ClearRows();
            var entries =  new List<string[]>();
            for (int i = 0; i < files.Length; i++)
            {
                var fileName = Path.GetFileName(files[i]);
                var order = i + 1;
                entries.Add(new string[] {
                order.ToString(),
                fileName,
                "-"
                });
            
            }

            entries.ForEach(e =>
            {
                datagrid.Rows.Add(e);

            });
        }

        public void MarkEntryEnded(int rowIndex)
        {
            UpdateStatus(rowIndex, "✔️", null);
        }

        public void MarkEntryEnded(int rowIndex, string correctPassword)
        {
            UpdateStatus(rowIndex, "✔️", correctPassword);
        }

        public void MarkEntryError(int rowIndex)
        {
            UpdateStatus(rowIndex, "X", null);
        }

        public void UpdateStatus(int rowIndex, string status, string correntPassword)
        {

            datagrid.Invoke(() =>
            {
                datagrid.Rows[rowIndex].Cells[2].Value = status;
                if (correntPassword != null)
                {
                datagrid.Rows[rowIndex].Cells[3].Value = correntPassword;
                }

            });
            datagrid.Invoke(() => {
                datagrid.Refresh();
            }); 
        }
        public void SaveJson()
        {
            Debug.WriteLine("test");

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pdf (*.PDF)|*.PDF";            //  Allow the user to select multiple images.
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Choose Zip Files or Directory";
            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                var files = openFileDialog.FileNames;
                if (files.Length > 0)
                {
                    Debug.WriteLine(files);
                    var newRequest = new DuxiuShovel.Models.CabinetExtractRequest();
                    newRequest.filePaths = files.ToList();
                    string jsonString = JsonConvert.SerializeObject(newRequest);
                    Debug.WriteLine(jsonString);
                    File.WriteAllText("cabinet_request_file_paths.json", jsonString, Encoding.UTF8);
                }

            }
        }
    }   
}
