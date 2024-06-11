using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Google.Protobuf.WellKnownTypes;

namespace scopusprojeuygulaması
{
    public partial class Form5 : Form
    {
       

        MySqlConnection conn;
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataAdapter adapter;
        DataTable dtMakale;
        string makaleTablo;
        


        public Form5(MySqlConnection conn, MySqlDataAdapter adapter)
        {

            InitializeComponent();
            this.conn = conn;
            this.adapter = adapter;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e) // dosya seçme
        {
            if(textBox5 == null)
            {
                MessageBox.Show("Lütfen önce dosyayı yükleyin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // kadro guncelle
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "CSV files(*.csv) | *.csv";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = fileDialog.FileName;
                csvYansitKadro(textBox5.Text);

            }
        }

        // makale kadro yansitmak icin
        private void csvYansitKadro(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.GetEncoding("iso-8859-9"));

            if (lines.Length > 0)
            {
                DataTable dt = new DataTable();

                // header bolmek icin
                string firstLine = lines[0];
                string[] headerLabels = ParseCsvLine(firstLine);

                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                // data icin datayı da boluyo asiri karmasik cikti veriyo
                for (int r = 1; r < lines.Length; r++)
                {
                    string[] dataWords = ParseCsvLine(lines[r]);
                    DataRow dr = dt.NewRow();

                    // kolon sayısı
                    for (int columnIndex = 0; columnIndex < Math.Min(headerLabels.Length, dataWords.Length); columnIndex++)
                    {
                        dr[headerLabels[columnIndex]] = dataWords[columnIndex];
                    }

                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                {
                    dataGridView2.DataSource = dt;
                }
            }
        }

        // ID leri bir arada tutmak icin
        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool insideQuotes = false;
            StringBuilder currentValue = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ';' && !insideQuotes)
                {
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            result.Add(currentValue.ToString()); // satir sonunu getirmek icin

            return result.ToArray();
        }

        

       /*private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                TabloAdlariniYukle(); // Tablo adlarını yükle
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Bağlantıyı kapat
            }

        }*/

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            makaleTablo = comboBox1.SelectedItem.ToString();
        }

        private void TabloAdlariniYukle()
        {
            try
            {
                // Veritabanı bağlantısını aç
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                // Veritabanındaki tablo adlarını almak için sorgu oluştur
                MySqlCommand cmd = new MySqlCommand("SHOW TABLES", conn);

                // Sorguyu çalıştır ve sonuçları al
                MySqlDataReader reader = cmd.ExecuteReader();

                // Tablo adlarını ComboBox'lara eklemek için döngü
                while (reader.Read())
                {
                    // İlk sütunu (tablo adını) al ve ComboBox'lara ekle
                    comboBox1.Items.Add(reader.GetString(0));

                }

                // Veritabanı bağlantısını kapat
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tablo adları yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        // ComboBox'taki seçili tablo adına göre verileri getiren fonksiyon
        private void makaleVerileriniGetir(string tabloAdi)
        {
            // ComboBox'tan seçilen tablo adı ile verileri getir
            if (tabloAdi == "")
            {
                MessageBox.Show("Lütfen bir tablo seçin.");
                return;
            }

            // Seçilen tablo adı ile verileri getir
            makaleVeriGetir(tabloAdi);
        }


        public void makaleVeriGetir(string tabloAdi)
        {
            try
            {
                dtMakale = new DataTable();
                conn.Open();
                adapter = new MySqlDataAdapter($"SELECT * FROM {tabloAdi};", conn);
                adapter.Fill(dtMakale);
                dataGridView1.DataSource = dtMakale;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri getirme hatası: {ex.Message}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            makaleVeriGetir(makaleTablo);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string tableName = comboBox1.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(tableName))
                {
                    DataTable dt = dataGridView2.DataSource as DataTable;

                    // dataGridView2 null mı kontrol et
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // tablo var mı kontrol et
                        using (MySqlCommand checkTableCommand = new MySqlCommand($"SHOW TABLES LIKE '{tableName}'", conn))
                        {
                            conn.Open();

                            if (checkTableCommand.ExecuteScalar() != null)
                            {
                                // kolonları düzenle
                                foreach (DataColumn column in dt.Columns)
                                {
                                    using (MySqlCommand checkColumnCommand = new MySqlCommand($"SHOW COLUMNS FROM {tableName} LIKE '{column.ColumnName}'", conn))
                                    {
                                        if (checkColumnCommand.ExecuteScalar() == null)
                                        {
                                            // kolon yoksa ekle
                                            using (MySqlCommand addColumnCommand = new MySqlCommand($"ALTER TABLE {tableName} ADD COLUMN `{column.ColumnName}` VARCHAR(255)", conn))
                                            {
                                                addColumnCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                // kopya olan satırları sil link'e göre
                                foreach (DataRow row in dt.Rows)
                                {
                                    string linkValue = row["Link"].ToString();
                                    using (MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {tableName} WHERE Link = @link", conn))
                                    {
                                        deleteCommand.Parameters.AddWithValue("@link", linkValue);
                                        deleteCommand.ExecuteNonQuery();
                                    }
                                }

                                // güncelle ya da ekle
                                foreach (DataRow row in dt.Rows)
                                {
                                    // parametre
                                    string linkValue = row["Link"].ToString();

                                    // parametre satırı var mı kontrol et
                                    using (MySqlCommand checkRowCommand = new MySqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE Link = @link", conn))
                                    {
                                        checkRowCommand.Parameters.AddWithValue("@link", linkValue);

                                        int rowCount = Convert.ToInt32(checkRowCommand.ExecuteScalar());

                                        if (rowCount > 0)
                                        {
                                            // satır varsa güncelle
                                            using (MySqlCommand updateRowCommand = new MySqlCommand($"UPDATE {tableName} SET `Author(s) ID` = @authors, Title = @title, Year = @year, `Source title` = @sourcetitle, Publisher = @publisher, `Cited by` = @citedby, Link = @link, ISSN = @issn, ISBN = @isbn,  DOI = @doi, `Document Type` = @documenttype, Source = @source WHERE Link = @link", conn))
                                            {
                                                // her getirilen kolon için ekle
                                                updateRowCommand.Parameters.AddWithValue("@authors", row["Author(s) ID"]);
                                                updateRowCommand.Parameters.AddWithValue("@title", row["Title"]);
                                                updateRowCommand.Parameters.AddWithValue("@year", row["Year"]);
                                                updateRowCommand.Parameters.AddWithValue("@sourcetitle", row["Source title"]);
                                                updateRowCommand.Parameters.AddWithValue("@publisher", row["Publisher"]);
                                                updateRowCommand.Parameters.AddWithValue("@citedby", row["Cited by"]);
                                                updateRowCommand.Parameters.AddWithValue("@issn", row["ISSN"]);
                                                updateRowCommand.Parameters.AddWithValue("@isbn", row["ISBN"]);
                                                updateRowCommand.Parameters.AddWithValue("@doi", row["DOI"]);
                                                updateRowCommand.Parameters.AddWithValue("@documenttype", row["Document Type"]);
                                                updateRowCommand.Parameters.AddWithValue("@source", row["Source"]);

                                                // parametre kolonu
                                                updateRowCommand.Parameters.AddWithValue("@link", linkValue);
                                                updateRowCommand.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            // satır yoksa ekle
                                            using (MySqlCommand insertRowCommand = new MySqlCommand($"INSERT INTO {tableName} (`Author(s) ID`, Title, Year, `Source title`, Publisher, `Cited by`, Link, ISSN, ISBN, DOI, `Document Type`, Source) VALUES (@authors, @title, @year, @sourcetitle, @publisher, @citedby, @link, @issn, @isbn, @doi, @documenttype, @source)", conn))
                                            {
                                                // her getirilen kolon için ekle
                                                insertRowCommand.Parameters.AddWithValue("@authors", row["Author(s) ID"]);
                                                insertRowCommand.Parameters.AddWithValue("@title", row["Title"]);
                                                insertRowCommand.Parameters.AddWithValue("@year", row["Year"]);
                                                insertRowCommand.Parameters.AddWithValue("@sourcetitle", row["Source title"]);
                                                insertRowCommand.Parameters.AddWithValue("@publisher", row["Publisher"]);
                                                insertRowCommand.Parameters.AddWithValue("@citedby", row["Cited by"]);
                                                insertRowCommand.Parameters.AddWithValue("@issn", row["ISSN"]);
                                                insertRowCommand.Parameters.AddWithValue("@isbn", row["ISBN"]);
                                                insertRowCommand.Parameters.AddWithValue("@doi", row["DOI"]);
                                                insertRowCommand.Parameters.AddWithValue("@documenttype", row["Document Type"]);
                                                insertRowCommand.Parameters.AddWithValue("@source", row["Source"]);

                                                // parametre kolonu 
                                                insertRowCommand.Parameters.AddWithValue("@link", linkValue);
                                                insertRowCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                MessageBox.Show("Başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Table '{tableName}' bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("DataGridView boş, güncelleme yapılamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Comboboxtan tablo seçin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HATA: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string tableName = comboBox1.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(tableName))
                {
                    DataTable dt = dataGridView2.DataSource as DataTable;

                    // dataGridView2 null mı kontrol et
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // tablo var mı kontrol et
                        using (MySqlCommand checkTableCommand = new MySqlCommand($"SHOW TABLES LIKE '{tableName}'", conn))
                        {
                            conn.Open();

                            if (checkTableCommand.ExecuteScalar() != null)
                            {
                                // kolonları düzenle
                                foreach (DataColumn column in dt.Columns)
                                {
                                    using (MySqlCommand checkColumnCommand = new MySqlCommand($"SHOW COLUMNS FROM {tableName} LIKE '{column.ColumnName}'", conn))
                                    {
                                        if (checkColumnCommand.ExecuteScalar() == null)
                                        {
                                            // kolon yoksa ekle
                                            using (MySqlCommand addColumnCommand = new MySqlCommand($"ALTER TABLE {tableName} ADD COLUMN `{column.ColumnName}` VARCHAR(255)", conn))
                                            {
                                                addColumnCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                // aynı idli kopya varsa sil 
                                foreach (DataRow row in dt.Rows)
                                {
                                    string authIdValue = row["Auth-ID"].ToString();
                                    using (MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {tableName} WHERE `Auth-ID` = @authid", conn))
                                    {
                                        deleteCommand.Parameters.AddWithValue("@authid", authIdValue);
                                        deleteCommand.ExecuteNonQuery();
                                    }
                                }

                                foreach (DataRow row in dt.Rows)
                                {
                                    string authIdValue = row["Auth-ID"].ToString();

                                    // indentidier satır var mı kontrol et
                                    using (MySqlCommand checkRowCommand = new MySqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE `Auth-ID` = @authid", conn))
                                    {
                                        checkRowCommand.Parameters.AddWithValue("@authid", authIdValue);

                                        int rowCount = Convert.ToInt32(checkRowCommand.ExecuteScalar());

                                        if (rowCount > 0)
                                        {
                                            // satır varsa güncelle
                                            using (MySqlCommand updateRowCommand = new MySqlCommand($"UPDATE {tableName} SET `Author Name` = @author, `Department` = @department, `Faculty` = @faculty, `Date Of Entry` = @dateofentry WHERE `Auth-ID` = @authid", conn))
                                            {
                                                updateRowCommand.Parameters.AddWithValue("@author", row["Author Name"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@department", row["Department"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@faculty", row["Faculty"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@dateofentry", row["Date Of Entry"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@authid", authIdValue);

                                                updateRowCommand.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            // satır yoksa ekle
                                            using (MySqlCommand insertRowCommand = new MySqlCommand($"INSERT INTO {tableName} (`Author Name`, `Auth-ID`, `Department`, `Faculty`, `Date Of Entry`) VALUES (@author, @authid, @department, @faculty, @dateofentry)", conn))
                                            {
                                                insertRowCommand.Parameters.AddWithValue("@author", row["Author Name"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@authid", authIdValue);
                                                insertRowCommand.Parameters.AddWithValue("@department", row["Department"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@faculty", row["Faculty"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@dateofentry", row["Date Of Entry"] ?? DBNull.Value);

                                                insertRowCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                MessageBox.Show("Başarıyla güncellendi!");
                            }
                            else
                            {
                                MessageBox.Show($"Table '{tableName}' bulunamadı");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("DataGridView boş, güncelleme yapılamadı.");
                    }
                }
                else
                {
                    MessageBox.Show("Comboboxtan tablo seçin");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HATA: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Combobox'tan tablo seçin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string tableName = comboBox1.SelectedItem.ToString();

                // satırın indexini al
                int rowIndex = dataGridView1.CurrentCell.RowIndex;

                // gridde satır varmı bak
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    // string idValue = dataGridView1.Rows[rowIndex].Cells["Author(s) ID"].Value.ToString();
                    string linkValue = dataGridView1.Rows[rowIndex].Cells["Link"].Value.ToString();

                    try
                    {
                        conn.Open();

                        // sql delete komutunu oluştur
                        string deleteQuery = $"DELETE FROM {tableName} WHERE  Link = '{linkValue}'";

                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // satırı gridden sil
                        dataGridView1.Rows.RemoveAt(rowIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"HATA: {ex.Message}");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    // satır yoksa
                    MessageBox.Show("Geçerli bir satır seçilmedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Combobox'tan tablo seçin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string tableName = comboBox1.SelectedItem.ToString();

                // satırın indexini al
                int rowIndex = dataGridView1.CurrentCell.RowIndex;

                // gridde satır varmı bak
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    // parametre auth id
                    string authorID = dataGridView1.Rows[rowIndex].Cells["Auth-ID"].Value.ToString();

                    try
                    {
                        conn.Open();

                        // idye göre delete komutu oluştur
                        string deleteQuery = $"DELETE FROM `{tableName}` WHERE `Auth-ID` = '{authorID}'";

                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // gridden satırı sil
                        dataGridView1.Rows.RemoveAt(rowIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"HATA: {ex.Message}");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    // satır indexi bulunamadı
                    MessageBox.Show("Geçerli bir satır seçilmedi.");
                }
            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
               
                TabloAdlariniYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        

        private void button9_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Combobox'tan tablo seçin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string tableName = comboBox1.SelectedItem.ToString();

                // satırın indexini al
                int rowIndex = dataGridView1.CurrentCell.RowIndex;

                // gridde satır varmı bak
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    // parametre auth id
                    string sourceID = dataGridView1.Rows[rowIndex].Cells["Sourceid"].Value.ToString();

                    try
                    {
                        conn.Open();

                        // idye göre delete komutu oluştur
                        string deleteQuery = $"DELETE FROM `{tableName}` WHERE `Sourceid` = '{sourceID}'";

                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // gridden satırı sil
                        dataGridView1.Rows.RemoveAt(rowIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"HATA: {ex.Message}");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    // satır indexi bulunamadı
                    MessageBox.Show("Geçerli bir satır seçilmedi.");
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                string tableName = comboBox1.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(tableName))
                {
                    DataTable dt = dataGridView2.DataSource as DataTable;

                    // dataGridView2 null mı kontrol et
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        using (MySqlCommand checkTableCommand = new MySqlCommand($"SHOW TABLES LIKE '{tableName}'", conn))
                        {
                            conn.Open();

                            if (checkTableCommand.ExecuteScalar() != null)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    using (MySqlCommand checkColumnCommand = new MySqlCommand($"SHOW COLUMNS FROM {tableName} LIKE '{column.ColumnName}'", conn))
                                    {
                                        if (checkColumnCommand.ExecuteScalar() == null)
                                        {
                                            using (MySqlCommand addColumnCommand = new MySqlCommand($"ALTER TABLE {tableName} ADD COLUMN `{column.ColumnName}` VARCHAR(255)", conn))
                                            {
                                                addColumnCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                foreach (DataRow row in dt.Rows)
                                {
                                    string authIdValue = row["Sourceid"].ToString();
                                    using (MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {tableName} WHERE `Sourceid` = @sourceid", conn))
                                    {
                                        deleteCommand.Parameters.AddWithValue("@sourceid", authIdValue);
                                        deleteCommand.ExecuteNonQuery();
                                    }
                                }

                                foreach (DataRow row in dt.Rows)
                                {
                                    string authIdValue = row["Sourceid"].ToString();

                                    using (MySqlCommand checkRowCommand = new MySqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE `Sourceid` = @sourceid", conn))
                                    {
                                        checkRowCommand.Parameters.AddWithValue("@sourceid", authIdValue);

                                        int rowCount = Convert.ToInt32(checkRowCommand.ExecuteScalar());

                                        if (rowCount > 0)
                                        {
                                            using (MySqlCommand updateRowCommand = new MySqlCommand($"UPDATE {tableName} SET `Type` = @type, `Title` = @title, `Issn` = @issn, `Publisher` = @publisher, `Categories` = @categories, `Areas` = @areas WHERE `Sourceid` = @sourceid", conn))
                                            {
                                                updateRowCommand.Parameters.AddWithValue("@title", row["Title"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@type", row["Type"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@issn", row["Issn"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@publisher", row["Publisher"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@categories", row["Categories"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@areas", row["Areas"] ?? DBNull.Value);
                                                updateRowCommand.Parameters.AddWithValue("@sourceid", authIdValue);

                                                updateRowCommand.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            using (MySqlCommand insertRowCommand = new MySqlCommand($"INSERT INTO {tableName} (`Type`, `Title`, `Issn`, `Publisher`, `Categories`, `Areas` , `Sourceid`) VALUES (@type, @title, @issn, @publisher, @categories, @areas, @sourceid)", conn))
                                            {
                                                insertRowCommand.Parameters.AddWithValue("@title", row["Title"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@sourceid", authIdValue);
                                                insertRowCommand.Parameters.AddWithValue("@type", row["Type"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@issn", row["Issn"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@publisher", row["Publisher"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@categories", row["Categories"] ?? DBNull.Value);
                                                insertRowCommand.Parameters.AddWithValue("@areas", row["Areas"] ?? DBNull.Value);

                                                insertRowCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                MessageBox.Show("Başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Tablo '{tableName}' bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("DataGridView boş, güncelleme yapılamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("ComboBox'tan tablo seçin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HATA: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Combobox'tan tablo seçin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tableName = comboBox1.SelectedItem.ToString();
            int rowIndex = dataGridView1.CurrentCell.RowIndex;

            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
            {
                string columnName = ""; // İlgili sütun adını saklamak için bir değişken
                string cellValue = "";  // İlgili hücrenin değerini saklamak için bir değişken

                // Verilerin bulunduğu sütunlarda hücre değerlerine göre silme işlemi yapılıyor
                if (dataGridView1.Columns.Contains("Link") && dataGridView1.Rows[rowIndex].Cells["Link"].Value != null)
                {
                    columnName = "Link";
                    cellValue = dataGridView1.Rows[rowIndex].Cells["Link"].Value.ToString();
                }
                else if (dataGridView1.Columns.Contains("Sourceid") && dataGridView1.Rows[rowIndex].Cells["Sourceid"].Value != null)
                {
                    columnName = "Sourceid";
                    cellValue = dataGridView1.Rows[rowIndex].Cells["Sourceid"].Value.ToString();
                }
                else if (dataGridView1.Columns.Contains("Auth-ID") && dataGridView1.Rows[rowIndex].Cells["Auth-ID"].Value != null)
                {
                    columnName = "Auth-ID";
                    cellValue = dataGridView1.Rows[rowIndex].Cells["Auth-ID"].Value.ToString();
                }
                else
                {
                    MessageBox.Show("Silme işlemi için uygun bir sütun bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    conn.Open();

                    string deleteQuery = $"DELETE FROM `{tableName}` WHERE `{columnName}` = '{cellValue}'";

                    using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    dataGridView1.Rows.RemoveAt(rowIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"HATA: {ex.Message}");
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Geçerli bir satır seçilmedi.");
            }
        }



        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Eklenecek tablo adı
                string tableName = comboBox1.SelectedItem.ToString();

                // Eklenecek sütunlar ve değerler
                Dictionary<string, string> columnValues = new Dictionary<string, string>();
                columnValues.Add("Author Name", textBox2.Text);
                columnValues.Add("Auth-ID", textBox1.Text);
                columnValues.Add("Department", textBox6.Text);
                columnValues.Add("Faculty", textBox4.Text);
                columnValues.Add("Date Of Entry", textBox7.Text);
                // Daha fazla sütun ve değer ekleyebilirsiniz

                // Ekleme işlemi için AddRowToTable metodunu çağırma
                AddRowToTable(tableName, columnValues);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HATA: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddRowToTable(string tableName, Dictionary<string, string> columnValues)
        {
            try
            {
                conn.Open();

                // Eklenecek sütunlar ve değerlerin string olarak birleştirilmesi
                string columns = string.Join(", ", columnValues.Keys.Select(key => $"`{key}`"));
                string values = string.Join(", ", columnValues.Values.Select(value => $"'{value}'"));

                string insertQuery = $"INSERT INTO `{tableName}` ({columns}) VALUES ({values})";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Yeni satır başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // DataGridView'ı güncelleme
                UpdateDataGridView(tableName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HATA: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void UpdateDataGridView(string tableName)
        {
            string selectQuery = $"SELECT * FROM `{tableName}`";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(cmd.ExecuteReader());
                dataGridView1.DataSource = dataTable;
            }
        }
    }
}
