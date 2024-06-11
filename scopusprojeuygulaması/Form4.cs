using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace scopusprojeuygulaması
{
    public partial class Form4 : Form
    {
        private DataTable originalDataSource;

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            bilgilerigetir();
            BenzersizDegerleriAta("Year", comboBox1);
            BenzersizDegerleriAta("Department", comboBox2);
            BenzersizDegerleriAta("Faculty", comboBox3);

            UpdateCitedByLabel();
            int toplamCitedBy = ToplamCitedBy();
            DataView dataView = ((DataTable)dataGridView1.DataSource).DefaultView;
            UpdateFilteredCitedByLabel(dataView); // DataView nesnesini gönderelim
            label7.Text = $"Toplam Atıf Sayısı: {toplamCitedBy}";

            // Combobox3'ün ilk elemanını seçelim
            comboBox3.SelectedIndex = 0;
        }

        private void UpdateCitedByLabel()
        {
            if (dataGridView1.DataSource != null)
            {
                int totalCitedBy = ToplamCitedBy();
                

                int totalArticles = originalDataSource.Rows.Count;
                // Filtrelenmiş makale sayısı

                // Filtrelenmiş makale sayısı
                int filteredArticles = ((DataTable)dataGridView1.DataSource).Rows.Count;

               
               


               
                label13.Text = $"Toplam Makale Sayısı: {totalArticles}";
                label14.Text = $"Filtrelenmiş Makale Sayısı: {filteredArticles}";

               
            }
        }

        private int ToplamCitedBy()
        {
            int toplamCitedBy = 0;

            if (originalDataSource != null && originalDataSource.Rows.Count > 0)
            {
                foreach (DataRow row in originalDataSource.Rows)
                {
                    // "Cited by" sütunundaki hücrenin değerini alırken null kontrolü yapmak önemlidir
                    if (row["Cited by"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Cited by"].ToString()))
                    {
                        // Sayıya dönüştürme işlemi güvenli bir şekilde yapılır
                        int citedByValue;
                        if (int.TryParse(row["Cited by"].ToString(), out citedByValue))
                        {
                            toplamCitedBy += citedByValue;
                        }
                    }
                }
            }

            return toplamCitedBy;

        }

       
        private void BenzersizDegerleriAta(string sütunAdı, ComboBox comboBox)
        {
            if (originalDataSource != null)
            {
                List<string> uniqueValues = BenzersizDegerler(sütunAdı);
                foreach (string value in uniqueValues)
                {
                    comboBox.Items.Add(value);
                }
            }
        }

        private List<string> BenzersizDegerler(string sütunAdı)
        {
            List<string> benzersizDegerler = new List<string>();

            if (originalDataSource != null && originalDataSource.Rows.Count > 0)
            {
                foreach (DataRow row in originalDataSource.Rows)
                {
                    string deger = row[sütunAdı].ToString();
                    if (!benzersizDegerler.Contains(deger))
                    {
                        benzersizDegerler.Add(deger);
                    }
                }
            }

            return benzersizDegerler;
        }

        private void bilgilerigetir()
        {
            if (Application.OpenForms["Form3"] is Form3 form3)
            {
                DataGridView dataGridView3 = form3.DataGridView3;
                if (dataGridView3 != null && dataGridView3.Rows.Count > 0)
                {
                    originalDataSource = new DataTable();
                    foreach (DataGridViewColumn col in dataGridView3.Columns)
                    {
                        originalDataSource.Columns.Add(col.Name, typeof(string));
                    }

                    foreach (DataGridViewRow row in dataGridView3.Rows)
                    {
                        DataRow newRow = originalDataSource.Rows.Add();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            newRow[cell.ColumnIndex] = cell.Value;
                        }
                    }

                    dataGridView1.DataSource = originalDataSource;
                }
            }
        }

        private void IdAraması(string searchText)
        {
            DataView dv = originalDataSource.DefaultView;

            if (!string.IsNullOrEmpty(searchText))
            {
                dv.RowFilter = $"[Auth-ID] LIKE '%{searchText}%'";
            }
            else
            {
                dv.RowFilter = string.Empty;
            }

            dataGridView1.DataSource = dv.ToTable();
            UpdateCitedByLabel();
        }

        private void IsimileAramayap(string searchText)
        {
            DataView dv = originalDataSource.DefaultView;

            if (!string.IsNullOrEmpty(searchText))
            {
                dv.RowFilter = $"[Author Name] LIKE '%{searchText}%'";
            }
            else
            {
                dv.RowFilter = string.Empty;
            }

            dataGridView1.DataSource = dv.ToTable();
            UpdateCitedByLabel();
        }


        private void FilterData()
        {
            DataView dv = originalDataSource.DefaultView;

            // Year filtresi
            string selectedYear = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedYear))
            {
                dv.RowFilter = $"Year = '{selectedYear}'";
            }
            else
            {
                dv.RowFilter = string.Empty;
            }

            // Department filtresi
            string selectedDepartment = comboBox2.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDepartment))
            {
                if (!string.IsNullOrEmpty(dv.RowFilter))
                {
                    dv.RowFilter += $" AND Department = '{selectedDepartment}'";
                }
                else
                {
                    dv.RowFilter = $"Department = '{selectedDepartment}'";
                }
            }

            // Faculty filtresi
            string selectedFaculty = comboBox3.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedFaculty))
            {
                if (!string.IsNullOrEmpty(dv.RowFilter))
                {
                    dv.RowFilter += $" AND Faculty = '{selectedFaculty}'";
                }
                else
                {
                    dv.RowFilter = $"Faculty = '{selectedFaculty}'";
                }
            }

            // Auth-ID filtresi
            string idFilter = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(idFilter))
            {
                if (!string.IsNullOrEmpty(dv.RowFilter))
                {
                    dv.RowFilter += $" AND [Auth-ID] LIKE '%{idFilter}%'";
                }
                else
                {
                    dv.RowFilter = $"[Auth-ID] LIKE '%{idFilter}%'";
                }
            }

            // Author Name filtresi
            string nameFilter = textBox2.Text.Trim();
            if (!string.IsNullOrEmpty(nameFilter))
            {
                if (!string.IsNullOrEmpty(dv.RowFilter))
                {
                    dv.RowFilter += $" AND [Author Name] LIKE '%{nameFilter}%'";
                }
                else
                {
                    dv.RowFilter = $"[Author Name] LIKE '%{nameFilter}%'";
                }
            }

            dataGridView1.DataSource = dv.ToTable();
            UpdateCitedByLabel();
            UpdateFilteredCitedByLabel(dv);
        

    }

        private void UpdateFilteredCitedByLabel(DataView filteredDataView)
        {
            int filteredCitedBy = 0;

            foreach (DataRowView rowView in filteredDataView)
            {
                if (rowView["Cited by"] != DBNull.Value && !string.IsNullOrWhiteSpace(rowView["Cited by"].ToString()))
                {
                    int citedByValue;
                    if (int.TryParse(rowView["Cited by"].ToString(), out citedByValue))
                    {
                        filteredCitedBy += citedByValue;
                    }
                }
            }

            label10.Text = $"Filtrelenmiş Atıf Sayısı: {filteredCitedBy}";
        }

        private void BenzersizDegerleriAta(string sütunAdı, ComboBox comboBox, string selectedFaculty = null)
        {
            if (originalDataSource != null)
            {
                List<string> uniqueValues = BenzersizDegerler(sütunAdı, selectedFaculty);
                comboBox.Items.Clear(); // Önceki verileri temizleyelim
                foreach (string value in uniqueValues)
                {
                    comboBox.Items.Add(value);
                }
            }
        }

        private List<string> BenzersizDegerler(string sütunAdı, string selectedFaculty = null)
        {
            List<string> benzersizDegerler = new List<string>();

            if (originalDataSource != null && originalDataSource.Rows.Count > 0)
            {
                foreach (DataRow row in originalDataSource.Rows)
                {
                    // Seçilen fakülteye göre bölüm seçimi yapalım
                    if (selectedFaculty != null && row["Faculty"].ToString() != selectedFaculty)
                    {
                        continue;
                    }

                    string deger = row[sütunAdı].ToString();
                    if (!benzersizDegerler.Contains(deger))
                    {
                        benzersizDegerler.Add(deger);
                    }
                }
            }

            return benzersizDegerler;
        }





        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData(); 
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFaculty = comboBox3.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedFaculty))
            {
                // Combobox3'ten seçilen fakültenin bölümlerini combobox2'ye ata
                BenzersizDegerleriAta("Department", comboBox2, selectedFaculty);
            }

            FilterData();
        }
    }
}
