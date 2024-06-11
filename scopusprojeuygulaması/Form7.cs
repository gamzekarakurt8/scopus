using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Forms.ComboBox;

namespace scopusprojeuygulaması
{
    public partial class Form7 : Form
    {


        private DataGridView mainDataGridView;



        public Form7(DataGridView mainDataGridView)
        {
            InitializeComponent();
            this.mainDataGridView = mainDataGridView;

            // raporu oluştur
            OlusturVeGosterBolum();



        }
        // data table oluştur
        private void OlusturVeGosterBolum()
        {
            DataTable reportTable = new DataTable();
            reportTable.Columns.Add("Bölüm", typeof(string));
            reportTable.Columns.Add("Yıl", typeof(int));
            reportTable.Columns.Add("Atıf Sayısı", typeof(int));
            reportTable.Columns.Add("Makale Sayısı", typeof(int));

            var groupedData = mainDataGridView.Rows
                .Cast<DataGridViewRow>()
                .GroupBy(row => new { Department = row.Cells["Department"].Value?.ToString(), Year = row.Cells["Year"].Value?.ToString() })
                .Select(group => new
                {
                    Department = group.Key.Department,
                    Year = int.TryParse(group.Key.Year, out int parsedYear) ? parsedYear : 0,
                    TotalCitedBy = group.Sum(row => ParseCitedBy(row.Cells["Cited by"].Value?.ToString())),
                    LinkCount = MakaleSayisi(group.Key.Department, int.TryParse(group.Key.Year, out int parsedYearMakale) ? parsedYearMakale : 0)
                });

            foreach (var item in groupedData)
            {
                // 0 ları ayıkla ama departman boşsa al
                if (!string.IsNullOrEmpty(item.Department) || item.Department == "0" ||
                item.Year != 0 || item.TotalCitedBy != 0 || item.LinkCount != 0)
                {
                    DataRow newRow = reportTable.NewRow();
                    newRow["Bölüm"] = item.Department;
                    newRow["Yıl"] = item.Year;
                    newRow["Atıf Sayısı"] = item.TotalCitedBy;
                    newRow["Makale Sayısı"] = item.LinkCount;
                    reportTable.Rows.Add(newRow);
                }
            }

            detayliBolum.DataSource = reportTable;

            // Distinct bölüm ve yıl değerlerini al
            List<string> distinctDepartments = GetDistinctColumnValues(mainDataGridView, "Department");
            List<int> distinctYears = GetDistinctColumnValues(mainDataGridView, "Year").Select(year => int.Parse(year)).ToList();

            // ComboBox'lara değerleri ata
            comboBox3.DataSource = distinctDepartments;
            comboBox4.DataSource = distinctYears;

            // ComboBox'ların seçim değişikliği olayını ekle
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            comboBox4.SelectedIndexChanged += comboBox4_SelectedIndexChanged;
        }



        // makale sayısını linkleri sayarak al 
        private int MakaleSayisi(string department, int year)
        {
            int linkCount = mainDataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(row =>
                {
                    string rowDepartment = row.Cells["Department"].Value?.ToString();
                    int rowYear;

                    return rowDepartment == department &&
                           int.TryParse(row.Cells["Year"].Value?.ToString(), out rowYear) &&
                           rowYear == year;
                })
                .Count();

            return linkCount;
        }
        //////////////////   
        public void DataGir(DataTable data)
        {
            // datatable oluştur
            DataTable resultTable = CalculateData(data);

            // fakülte ve yılı grup anahtarı olarak ata
            var groupedData = data.AsEnumerable()
                .GroupBy(row => new { Faculty = row.Field<string>("Faculty"), Year = row.Field<int>("Year") })
                .Select(group => new
                {
                    Faculty = group.Key.Faculty,
                    Year = group.Key.Year,
                    TotalCitedBy = group.Sum(row => ParseCitedBy(row.Field<string>("Cited by"))),
                    LinkCount = MakaleFakulte(group.Key.Faculty, group.Key.Year) // makale sayısını işle
                });

            // datatable a değerleri işle
            foreach (var item in groupedData)
            {
                resultTable.Rows.Add(item.Faculty, item.Year, item.TotalCitedBy, item.LinkCount);
            }

            // data kaynağı form3 teki dv3
            detayliFakulte.DataSource = resultTable;
        }


        public void AtifGir(DataTable atifyil)
        {
            // datatable oluştur
            DataTable resultTable = AtifData(atifyil);

            // fakülte ve yılı grup anahtarı olarak ata
            var groupedData = atifyil.AsEnumerable()
                .GroupBy(row => new { Author = row.Field<string>("Author Name"), Year = row.Field<int>("Year") })
                .Select(group => new
                {
                    Author = group.Key.Author,
                    Year = group.Key.Year,
                    TotalCitedBy = group.Sum(row => ParseCitedBy(row.Field<string>("Cited by"))),
                    LinkSay = group.Count(),
                    LinkCount = MakaleSayisi2(group.Key.Author, group.Key.Year)

                });

            // datatable a değerleri işle
            foreach (var item in groupedData)
            {
                resultTable.Rows.Add(item.Author, item.Year, item.TotalCitedBy, item.LinkSay);
            }

            // data kaynağı form3 teki dv3
            detayliAtif.DataSource = resultTable;
        }
        public int MakaleSayisi2(string department, int year)
        {
            int linkCount = mainDataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(row =>
                {
                    string rowDepartment = row.Cells["Author Name"].Value?.ToString();
                    int rowYear;

                    return rowDepartment == department &&
                           int.TryParse(row.Cells["Year"].Value?.ToString(), out rowYear) &&
                           rowYear == year;
                })
                .Count();

            return linkCount;
        }

        private int ParseCitedBy(string citedByValue)
        {

            if (int.TryParse(citedByValue, out int result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        private DataTable AtifData(DataTable atifyil)
        {

            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("İsim", typeof(string));
            resultTable.Columns.Add("Yıl", typeof(int));
            resultTable.Columns.Add("Atıf Sayısı", typeof(int));
            resultTable.Columns.Add("Makale Sayısı", typeof(int));


            return resultTable;
        }


        // makale sayısı hesaplandıktan sonraki data table
        private DataTable CalculateData(DataTable data)
        {

            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Fakülte", typeof(string));
            resultTable.Columns.Add("Yıl", typeof(int));
            resultTable.Columns.Add("Atıf Sayısı", typeof(int));
            resultTable.Columns.Add("Makale Sayısı", typeof(int));

            return resultTable;
        }
        // linkleri say ve yerleştir
        private int MakaleFakulte(string faculty, int year)
        {
            int linkCount = mainDataGridView.Rows
         .Cast<DataGridViewRow>()
         .Where(row =>
         {    // sayılan linkleri  yıl ve fakülteye göre dağıt
             string rowFaculty = row.Cells["Faculty"].Value?.ToString();
             int rowYear;

             return rowFaculty == faculty && int.TryParse(row.Cells["Year"].Value?.ToString(), out rowYear) && rowYear == year;
         })
         .Count();

            return linkCount;
        }



        // datagride veri çekme

        public DataTable raporTable;
        // datagride form3 grid3 ten veri çek 
        public void YazarGir(DataTable sourceTable)
        {
            raporTable = new DataTable();

            raporTable.Columns.Add("Author Name", typeof(string));
            raporTable.Columns.Add("Title", typeof(string));
            raporTable.Columns.Add("Cited by", typeof(string));
            raporTable.Columns.Add("Year", typeof(int));

            detayliSahis.DataSource = raporTable;

            string yearFilter = comboBox1.Text;
            string authorNameFilter = comboBox2.Text;

            foreach (DataRow row in sourceTable.Rows)
            {
                string authorName = row["Author Name"].ToString();
                int year;

                if ((string.IsNullOrEmpty(authorNameFilter) || authorName.Contains(authorNameFilter)) &&
                    (string.IsNullOrEmpty(yearFilter) || (int.TryParse(row["Year"].ToString(), out year) && year == int.Parse(yearFilter))))
                {
                    // filtreli datayı işle
                    DataRow newRow = raporTable.NewRow();
                    newRow["Author Name"] = authorName;
                    newRow["Title"] = row["Title"];
                    newRow["Cited by"] = row["Cited by"];
                    newRow["Year"] = (int.TryParse(row["Year"].ToString(), out year)) ? year : (object)DBNull.Value;
                    raporTable.Rows.Add(newRow);
                }
            }


            detayliSahis.DataSource = raporTable;
        }
        // filter datayı uygula



        // full datadaki kolonlardan değer çek
        private List<string> GetDistinctColumnValues(DataGridView dataGridView, string columnName)
        {
            List<string> distinctValues = new List<string>();

            // kolon varmı bak
            if (dataGridView.Columns.Contains(columnName))
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    object value = row.Cells[columnName].Value;
                    if (value != null && !distinctValues.Contains(value.ToString()))
                    {
                        distinctValues.Add(value.ToString());
                    }
                }
            }

            return distinctValues;
        }
        // grid filtrele


        private void Form7_Load(object sender, EventArgs e)
        {
            // Distinct bölüm ve yıl değerlerini al
            List<string> distinctDepartments = GetDistinctColumnValues(mainDataGridView, "Department").OrderBy(d => d).ToList();
            List<int> distinctYears = GetDistinctColumnValues(mainDataGridView, "Year").Select(year => int.Parse(year)).OrderBy(y => y).ToList();

            // "Seçilmedi" seçeneğini önce ekle
            distinctDepartments.Insert(0, "Seçilmedi");
            distinctYears.Insert(0, 0);

            // ComboBox'lara değerleri ata
            comboBox3.DataSource = distinctDepartments;
            comboBox4.DataSource = distinctYears;

            // ComboBox'ların seçim değişikliği olayını ekle
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            comboBox4.SelectedIndexChanged += comboBox4_SelectedIndexChanged;

            // İsim ve yıl değerlerini al
            List<string> distinctIsim = GetDistinctColumnValues(mainDataGridView, "Author Name").OrderBy(name => name).ToList();
            List<int> distinctYearsIsim = GetDistinctColumnValues(mainDataGridView, "Year").Select(year => int.Parse(year)).OrderBy(y => y).ToList();

            // "Seçilmedi" seçeneğini önce ekle
            distinctIsim.Insert(0, "Seçilmedi");
            distinctYearsIsim.Insert(0, 0);

            // Değerleri ata
            comboBox1.DataSource = distinctIsim;
            comboBox2.DataSource = distinctYearsIsim;

            // Seçim değişikliği
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged_1;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            List<string> distinctFakulte = GetDistinctColumnValues(mainDataGridView, "Faculty").OrderBy(f => f).ToList();
            List<int> distinctFakulteYıl = GetDistinctColumnValues(mainDataGridView, "Year").Select(year => int.Parse(year)).OrderBy(y => y).ToList();

            // "Seçilmedi" seçeneğini önce ekle
            distinctFakulte.Insert(0, "Seçilmedi");
            distinctFakulteYıl.Insert(0, 0);

            comboBox5.DataSource = distinctFakulte;
            comboBox6.DataSource = distinctFakulteYıl;

            comboBox5.SelectedIndexChanged += comboBox5_SelectedIndexChanged;
            comboBox6.SelectedIndexChanged += comboBox6_SelectedIndexChanged;

            List<string> distinctIsim2 = GetDistinctColumnValues(mainDataGridView, "Author Name").OrderBy(name => name).ToList();
            List<int> distinctYearsIsim2 = GetDistinctColumnValues(mainDataGridView, "Year").Select(year => int.Parse(year)).OrderBy(y => y).ToList();

            // "Seçilmedi" seçeneğini önce ekle
            distinctIsim2.Insert(0, "Seçilmedi");
            distinctYearsIsim2.Insert(0, 0);

           




        }



        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() != "Seçilmedi")
            {
                string selectedDepartment = comboBox3.SelectedItem.ToString();

                // Seçilen bölüme göre ilgili author namelerini al
                List<string> filteredAuthors = GetDistinctAuthorsForDepartment(selectedDepartment);

                // Seçim yapılmamışsa veya "Seçilmedi" seçilmişse, tüm author namelerini al
                if (filteredAuthors.Count == 0)
                {
                    filteredAuthors = GetDistinctColumnValues(mainDataGridView, "Author Name").OrderBy(name => name).ToList();
                }

                // "Seçilmedi" seçeneğini ekleyelim
                filteredAuthors.Insert(0, "Seçilmedi");

                // ComboBox1 ve ComboBox7'nin veri kaynağını güncelleyelim
                comboBox1.DataSource = filteredAuthors;
        

                // DetayliBolum DataGridView'ini seçilen bölüme göre filtrele
                (detayliBolum.DataSource as DataTable).DefaultView.RowFilter = string.Format("Bölüm = '{0}'", selectedDepartment);
            }
            else
            {
                // ComboBox3'ten bir bölüm seçilmemişse, tüm author namelerini alıp ComboBox1 ve ComboBox7'nin veri kaynağını güncelleyelim
                List<string> allAuthors = GetDistinctColumnValues(mainDataGridView, "Author Name").OrderBy(name => name).ToList();
                allAuthors.Insert(0, "Seçilmedi");
                comboBox1.DataSource = allAuthors;
               

                // DetayliBolum DataGridView'ini filtresiz göster
                (detayliBolum.DataSource as DataTable).DefaultView.RowFilter = "";
            }

            // Önceki seçimi temizleyelim
            comboBox1.SelectedIndex = -1;
           

        }

        // Seçilen bölüme göre ilgili author namelerini al
        private List<string> GetDistinctAuthorsForDepartment(string selectedDepartment)
        {
            List<string> distinctAuthorsForDepartment = new List<string>();

            foreach (DataGridViewRow row in mainDataGridView.Rows)
            {
                string department = row.Cells["Department"].Value?.ToString();
                string authorName = row.Cells["Author Name"].Value?.ToString();

                // Seçilen bölüme ait author namelerini filtreleyelim
                if (department == selectedDepartment && !string.IsNullOrEmpty(authorName) && !distinctAuthorsForDepartment.Contains(authorName))
                {
                    distinctAuthorsForDepartment.Add(authorName);
                }
            }

            return distinctAuthorsForDepartment.OrderBy(a => a).ToList();
        }



        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() != "Seçilmedi")
            {

                string selectedFakulte = comboBox3.SelectedItem.ToString();
                int selectedYear = comboBox4.SelectedItem != null ? Convert.ToInt32(comboBox4.SelectedItem.ToString()) : 0;
                FilterDataGridViewByDepartmentandYear(selectedFakulte, selectedYear);
            }
            else
            {

                DataTable table = (DataTable)detayliBolum.DataSource;
                table.DefaultView.RowFilter = "";
                detayliBolum.Refresh();
            }
        }



        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "Seçilmedi" &&
         comboBox2.SelectedItem != null) // comboBox2.SelectedItem'ı kontrol et
            {
                string selectedIsim = comboBox1.SelectedItem.ToString();
                int selectedYear = Convert.ToInt32(comboBox2.SelectedItem.ToString()); // Burada kontrol etmeye gerek yok, çünkü null olamaz.
                FilterDataGridIsimAndYear(selectedIsim, selectedYear);
            }
            else
            {
                // ComboBox seçilmedi veya "Seçilmedi" seçildiğinde, tam tabloyu göster
                DataTable table1 = (DataTable)detayliSahis.DataSource;
                DataTable table2 = (DataTable)detayliAtif.DataSource;
                if (table1 != null && table2 != null)
                {
                    table1.DefaultView.RowFilter = "";
                    table2.DefaultView.RowFilter = "";
                    detayliSahis.Refresh();
                    detayliAtif.Refresh();
                }
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() != "Seçilmedi")
            {
                string selectedIsim = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : null;
                int selectedYear = Convert.ToInt32(comboBox2.SelectedItem.ToString());
                FilterDataGridIsimAndYear(selectedIsim, selectedYear);
            }
            else
            {
                // ComboBox seçilmedi veya "Seçilmedi" seçildiğinde, tam tabloyu göster
                DataTable table1 = (DataTable)detayliSahis.DataSource;
                DataTable table2 = (DataTable)detayliAtif.DataSource;
                if (table1 != null && table2 != null)
                {
                    table1.DefaultView.RowFilter = "";
                    table2.DefaultView.RowFilter = "";
                    detayliSahis.Refresh();
                    detayliAtif.Refresh();
                }
            }
        }




        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedItem != null && comboBox5.SelectedItem.ToString() != "Seçilmedi")
            {
                string selectedFaculty = comboBox5.SelectedItem.ToString();
                int selectedYear = Convert.ToInt32(comboBox6.SelectedItem.ToString());
                FilterDataGridFakulteAndYear(selectedFaculty, selectedYear);

                // fakülteye göre sıralat
                List<string> filteredDepartments = GetDistinctDepartmentsForFaculty(selectedFaculty);

                // seçilmedi ekle
                filteredDepartments.Insert(0, "Seçilmedi");

                // fakülteye göre bölümü sıralat
                comboBox3.DataSource = filteredDepartments;
            }
            else
            {
                DataTable table = (DataTable)detayliFakulte.DataSource;
                table.DefaultView.RowFilter = "";
                detayliFakulte.Refresh();


                // fakülte seçilmemişse bütün departmanları çak
                List<string> allDepartments = GetDistinctColumnValues(mainDataGridView, "Department").OrderBy(d => d).ToList();

                // seçilmedi ekle
                allDepartments.Insert(0, "Seçilmedi");

                // departman comboboxunu doldur
                comboBox3.DataSource = allDepartments;
            }

            // önceki seçileni boşver
            comboBox3.SelectedIndex = -1;



          
        }

        // fakülte seçince departmanları ayıkla
        private List<string> GetDistinctDepartmentsForFaculty(string selectedFaculty)
        {
            List<string> distinctDepartmentsForFaculty = new List<string>();

            foreach (DataGridViewRow row in mainDataGridView.Rows)
            {
                string faculty = row.Cells["Faculty"].Value?.ToString();
                string department = row.Cells["Department"].Value?.ToString();

                // seçili fakülteye göre departman ekle
                if (faculty == selectedFaculty && !string.IsNullOrEmpty(department) && !distinctDepartmentsForFaculty.Contains(department))
                {
                    distinctDepartmentsForFaculty.Add(department);
                }
            }

            return distinctDepartmentsForFaculty.OrderBy(d => d).ToList();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.SelectedItem != null)
            {
                string selectedFakulte = comboBox5.SelectedItem.ToString();
                int selectedYear = comboBox6.SelectedItem != null ? Convert.ToInt32(comboBox6.SelectedItem.ToString()) : 0;
                FilterDataGridFakulteAndYear(selectedFakulte, selectedYear);
            }
            else
            {
                // ComboBox seçilmedi veya "Seçilmedi" seçildiğinde, tam tabloyu göster
                DataTable table = (DataTable)detayliFakulte.DataSource;
                table.DefaultView.RowFilter = "";
                detayliFakulte.Refresh();
            }
        }

        private void FilterDataGridFakulteAndYear(string fakulte, int year)
        {
            DataTable table = (DataTable)detayliFakulte.DataSource;
            if (fakulte != "Seçilmedi" && year != 0)
            {
                table.DefaultView.RowFilter = $"[Fakülte] = '{fakulte}' AND Yıl = '{year}'";
            }
            else if (fakulte != "Seçilmedi")
            {
                table.DefaultView.RowFilter = $"[Fakülte] = '{fakulte}'";
            }
            else if (year != 0)
            {
                table.DefaultView.RowFilter = $"Yıl = '{year}'";
            }
            else
            {
                // Display all data
                table.DefaultView.RowFilter = "";
            }
            detayliFakulte.Refresh();
        }

        private void FilterDataGridIsimAndYear(string isim, int year)
        {
            DataTable table1 = (DataTable)detayliSahis.DataSource;
            DataTable table2 = (DataTable)detayliAtif.DataSource;
            if (isim != null && year != 0)
            {
                table1.DefaultView.RowFilter = $"[Author Name] = '{isim}' AND Year = '{year}'";
                table2.DefaultView.RowFilter = $"İsim = '{isim}' AND Yıl = '{year}'";
            }
            else if (isim != null)
            {
                table1.DefaultView.RowFilter = $"[Author Name] = '{isim}'";
                table2.DefaultView.RowFilter = $"İsim = '{isim}'";
            }
            else if (year != 0)
            {
                table1.DefaultView.RowFilter = $"Year = '{year}'";
                table2.DefaultView.RowFilter = $"Yıl = '{year}'";
            }
            else
            {
                // Tüm veriyi göster
                table1.DefaultView.RowFilter = "";
                table2.DefaultView.RowFilter = "";
            }
            detayliSahis.Refresh();
            detayliAtif.Refresh();
        }

      
        private void FilterDataGridViewByDepartmentandYear(string department, int year)
        {
            DataTable table = (DataTable)detayliBolum.DataSource;
            if (department != "Seçilmedi" && year != 0)
            {
                table.DefaultView.RowFilter = $"Bölüm = '{department}' AND Yıl = '{year}'";
            }
            else if (department != "Seçilmedi")
            {
                table.DefaultView.RowFilter = $"Bölüm = '{department}'";
            }
            else if (year != 0)
            {
                table.DefaultView.RowFilter = $"Yıl = '{year}'";
            }
            else
            {
                // Display all data
                table.DefaultView.RowFilter = "";
            }
            detayliBolum.Refresh();
        }

     

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

      
        }
    }



        