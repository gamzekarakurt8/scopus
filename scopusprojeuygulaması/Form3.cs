using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace scopusprojeuygulaması
{
    public partial class Form3 : Form
    {
        
        private DataTable dtMakale;
        private DataTable dtKadro;
        private DataTable dtQ;
        private MySqlConnection conn; 
        private MySqlDataAdapter adapter; 
        private DataTable dt3;

        public Form3(DataTable dtMakale, DataTable dtKadro, MySqlConnection conn, MySqlDataAdapter adapter, DataTable dt3, DataTable dtQ)
        {
            InitializeComponent();

            this.dtMakale = dtMakale;
            this.dtKadro = dtKadro;
            this.conn = conn;
            this.adapter = adapter;
            this.dt3 = dt3;
            this.dtQ = dtQ;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // conn nesnesini BaseForm'dan alınan nesne olarak kullanın
            

            dataGridView1.DataSource = dtMakale;
            dataGridView2.DataSource = dtKadro;

            label7.Text = dtMakale.Rows.Count.ToString();
            label8.Text = dtKadro.Rows.Count.ToString();
            label12.Text = dtQ.Rows.Count.ToString();
            
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
                MessageBox.Show($"Veri getirme hatası: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void kadroVeriGetir(string tabloAdi)
        {
            try
            {
                dtKadro = new DataTable();
                conn.Open();
                adapter = new MySqlDataAdapter($"SELECT * FROM {tabloAdi};", conn);
                adapter.Fill(dtKadro);
                dataGridView2.DataSource = dtKadro;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri getirme hatası: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void qVeriGetir(string tabloAdi)
        {
            try
            {
                dtQ = new DataTable();
                conn.Open();
                adapter = new MySqlDataAdapter($"SELECT * FROM {tabloAdi};", conn);
                adapter.Fill(dtQ);
                dataGridView5.DataSource = dtQ; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri getirme hatası: {ex.Message}");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        // oluşturduğumuz eşleştirme datagridviewine sonuçların getirilmesini sağlar. button içine atamasını yapıyoruz.
        private void EşleştirVeGöster()
        {
            // dtKadro ve dtQ boş olmadığından emin ol
            if (dtKadro == null || dtQ == null)
            {
                MessageBox.Show("Lütfen önce verileri yükleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Birleştirilmiş DataTable oluştur
            dt3 = new DataTable();
            dt3.Columns.Add("Auth-ID", typeof(string));
            dt3.Columns.Add("Author Name", typeof(string));
            dt3.Columns.Add("Department", typeof(string));
            dt3.Columns.Add("Faculty", typeof(string));
            dt3.Columns.Add("Date Of Entry", typeof(string));
            dt3.Columns.Add("Author Status", typeof(int));
            dt3.Columns.Add("Title", typeof(string));
            dt3.Columns.Add("Source Title", typeof(string));
            dt3.Columns.Add("Publisher", typeof(string));
            dt3.Columns.Add("Document Type", typeof(string));
            dt3.Columns.Add("Cited by", typeof(string));
            dt3.Columns.Add("Categories", typeof(string));
            dt3.Columns.Add("Link", typeof(string));
            dt3.Columns.Add("ISSN", typeof(string));
            dt3.Columns.Add("ISBN", typeof(string));
            dt3.Columns.Add("DOI", typeof(string));
            dt3.Columns.Add("Year", typeof(int));
             

            // Paralel işlem sonuçlarını birleştir
            object lockObject = new object(); // lock için bir nesne oluştur
            int rowCount = 0; // eşleştirenleri sayan label için formu açınca fatal verdirme diye 
            Parallel.ForEach(new DataView(dtKadro).ToTable().AsEnumerable(), rowKadro =>
            {
                string authId = rowKadro["Auth-ID"].ToString();

                // dtMakale içindeki eşleşenleri bul
                var matchingRows = dtMakale.AsEnumerable()
                    .Where(item => item.Field<string>("Author(s) ID").Split(';').Contains(authId))
                    .Where(row =>
                    {
                        string authorId = row.Field<string>("Author(s) ID");
                        int semicolonCount = authorId.Split(';').Length - 1;
                        return semicolonCount <= 200 && authorId.Contains(authId); // kaç kişinin yazdığını belirtiyoruz
                    });

                foreach (var matchingRow in matchingRows)
                {
                    lock (lockObject) // Güvenli erişim için lock kullan
                    {
                        DataRow newRow = dt3.NewRow();
                        newRow["Auth-ID"] = authId;

                        // isim ve soyisim ters çevirip yazdırıldı!
                        string[] nameParts = rowKadro["Author Name"].ToString().Split(',');
                        if (nameParts.Length > 1)
                        {
                            newRow["Author Name"] = $"{nameParts[1].Trim()} {nameParts[0].Trim()}";
                        }
                        else
                        {
                            newRow["Author Name"] = rowKadro["Author Name"];
                        }

                        // Kontrol ediyok department - faculty - date of entry var mı diye yoksa boş yazdırıyoz
                        newRow["Department"] = rowKadro.Table.Columns.Contains("Department") ? rowKadro["Department"] : "";
                        newRow["Faculty"] = rowKadro.Table.Columns.Contains("Faculty") ? rowKadro["Faculty"] : "";
                        newRow["Date Of Entry"] = rowKadro.Table.Columns.Contains("Date Of Entry") ? rowKadro["Date Of Entry"] : "";

                        newRow["Cited by"] = matchingRow["Cited by"];
                        newRow["Title"] = matchingRow["Title"];
                        newRow["Source Title"] = matchingRow["Source Title"];
                        newRow["Link"] = matchingRow["Link"];
                        newRow["Year"] = matchingRow["Year"];
                        newRow["Document Type"] = matchingRow["Document Type"];
                        newRow["ISSN"] = matchingRow["ISSN"];
                        newRow["DOI"] = matchingRow["DOI"];
                        newRow["Publisher"] = matchingRow["Publisher"];
                        newRow["ISBN"] = matchingRow["ISBN"];

                        // Eşleştiği satıra olan sırayı temsil eden değeri atanıyor
                        string[] authorIds = matchingRow.Field<string>("Author(s) ID").Split(';');
                        newRow["Author Status"] = Array.IndexOf(authorIds, authId) + 1;

                        // dtQ tablosundan eşleşen ISSN numarasını al
                        string issn = matchingRow["ISSN"].ToString();
                        DataRow[] qMatchingRows = dtQ.Select($"ISSN = '{issn}' OR ISSN LIKE '%{issn}%' OR ISSN LIKE '{issn}%' OR ISSN LIKE '%{issn}'");
                        if (qMatchingRows.Length > 0)
                        {
                            newRow["Categories"] = qMatchingRows[0]["Categories"]; // dtQ'dan Categories değerini al
                        }
                        else
                        {
                            newRow["Categories"] = DBNull.Value; // Eşleşme bulunamadıysa DBNull değerini ata
                        }

                        dt3.Rows.Add(newRow);
                        rowCount++; // satır say
                    }
                }
            });

            
            DataView dv3 = new DataView(dt3);
            dv3.Sort = "Auth-ID";
            dataGridView3.DataSource = dv3.ToTable();

            // satır sayısını göster
            label9.Text = $" {rowCount}";
        }


        // DataGridView3'ün veri kaynağı değiştiğinde eşleşmeyen makaleleri bul ve göster
        private void EşleşmeyenMakaleleriBulVeGöster()
        {
            
            if (dtMakale == null || dt3 == null || dt3.Rows.Count == 0)
            {
                MessageBox.Show("Veri tabanlarından biri boş veya tanımsız.");
                return;
            }

            // Eşleşmeyen makalelerin bulunduğu DataTable oluştur
            DataTable eşleşmeyenMakaleler = dtMakale.Clone();

            // dt3'teki linkleri bir HashSet'e ekleyerek daha hızlı bir arama sağla
            HashSet<string> dt3Linkler = new HashSet<string>();
            foreach (DataRow row in dt3.Rows)
            {
                string link = Uri.EscapeUriString(row["Link"].ToString()); // Linkleri URI formatına uygun olarak kodla
                dt3Linkler.Add(link);
            }

            // dtMakale içindeki her bir makalenin linkini dt3 içinde ara
            foreach (DataRow row in dtMakale.Rows)
            {
                string link = Uri.EscapeUriString(row["Link"].ToString()); // Linkleri URI formatına uygun olarak kodla
                                                                           // Eğer dt3 içinde bu link yoksa, eşleşmeyenMakaleler'e ekle
                if (!dt3Linkler.Contains(link))
                {
                    // Bu makale eşleşmedi, bu yüzden eşleşmeyenMakaleler'e ekle
                    eşleşmeyenMakaleler.ImportRow(row);
                }
            }

            // Eşleşmeyen makalelerin olduğu DataGridView'e atama yap
            dataGridView4.DataSource = eşleşmeyenMakaleler;

            // Display the row count in a label
            label10.Text = $" {eşleşmeyenMakaleler.Rows.Count}";

            if (eşleşmeyenMakaleler.Rows.Count == 0)
            {
                MessageBox.Show("Eşleşmeyen makale bulunamadı.");
            }
        }

        // id ile arama yapacağımız textboxun içine atıyoruz.
        private void IdAramayap()
        {
            // dtMakale ve dtKadro boş olmadığından emin ol
            if (dtMakale == null || dtKadro == null)
            {
                MessageBox.Show("Lütfen önce verileri yükleyin.");
                return;
            }

            string searchText = textBox1.Text.Trim();
            DataView dv1 = new DataView(dtMakale);
            DataView dv2 = new DataView(dtKadro);

            if (!string.IsNullOrEmpty(searchText))
            {
                dv1.RowFilter = $"CONVERT([Author(s) ID], System.String) LIKE '%{searchText}%'";
                dv2.RowFilter = $"CONVERT([Auth-ID], System.String) LIKE '%{searchText}%'";
            }
            else
            {
                dv1.RowFilter = string.Empty;
                dv2.RowFilter = string.Empty;
            }

            // Diğer DataGridView'leri güncelle
            dataGridView1.DataSource = dv1.ToTable();
            dataGridView2.DataSource = dv2.ToTable();
        }

        // isim ile arama yapacağımız textboxun içine atıyoruz.
        private void IsimileAramayap()
        {
            // dtKadro boş olmadığından emin ol
            if (dtKadro == null)
            {
                MessageBox.Show("Lütfen önce verileri yükleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string searchText = textBox2.Text.Trim().ToLower();

            DataView dv = dtKadro.DefaultView;

            if (!string.IsNullOrEmpty(searchText))
            {
                dv.RowFilter = $"[Author Name] LIKE '%{searchText}%'";
            }
            else
            {
                dv.RowFilter = string.Empty;
            }

            dataGridView2.DataSource = dv.ToTable();
        }

        // csvye aktarmayı sağlayan butonun fonksiyonu
        private void csvaktar()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV dosyaları (*.csv)|*.csv|Tüm dosyalar (*.*)|*.*";
            saveFileDialog.Title = "CSV'ye Aktar";

            // Kullanıcı, dosyanın nereye kaydedileceğini ve adını seçene kadar bekler
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen dosya yolunu al
                string dosyaYolu = saveFileDialog.FileName;

                // CSV dosyasına aktarma işlemini gerçekleştir
                ExportToCSV(dataGridView3, dosyaYolu);

                MessageBox.Show("CSV dosyası başarıyla kaydedildi!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("İşlem iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // csvye aktarılacak verilerin kurallarını berirleyen fonksiyon
        private void ExportToCSV(DataGridView dataGridView, string dosyaYolu)
        {
            StringBuilder sb = new StringBuilder();

            // Sütun başlıklarını al ve Author Name sütununu düzelt
            IEnumerable<string> sütunBasliklari = dataGridView.Columns.Cast<DataGridViewColumn>()
                .Select(sütun => sütun.HeaderText == "Author Name" ? FixName(sütun.HeaderText) : sütun.HeaderText);

            sb.AppendLine(string.Join(",", sütunBasliklari));

            // Veri satırlarını al ve hücre verilerini düzelt
            foreach (DataGridViewRow satir in dataGridView.Rows)
            {
                List<string> alanlar = new List<string>();
                foreach (DataGridViewCell hücre in satir.Cells)
                {
                    // Tüm hücre verilerini düzelt
                    alanlar.Add(FixName(hücre.Value != null ? hücre.Value.ToString() : string.Empty));
                }
                sb.AppendLine(string.Join(",", alanlar));
            }

            // Dosyaya yaz
            System.IO.File.WriteAllText(dosyaYolu, sb.ToString());
        }

        private string FixName(string name)
        {
            // İsimdeki virgülü boşluk veya başka bir karakterle değiştir
            return name.Replace(",", " ");
        }

        private DataTable GetDataTable(string query)
        {
            DataTable dataTable = new DataTable();
            conn.Open();
            adapter = new MySqlDataAdapter(query, conn);
            adapter.Fill(dataTable);
            conn.Close();
            return dataTable;
        }

        private void RaporOlusturMakale(Form6 targetForm)
        {
            // dt3 dolumu bak
            if (dt3 == null)
            {
                MessageBox.Show("lütfen öncelikle eşleştirme yapın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // raporlama için database oluştur
            DataTable reportTable3 = new DataTable();
            reportTable3.Columns.Add("Yıl", typeof(int));
            reportTable3.Columns.Add("Makale Sayısı", typeof(int)); // makale sayısını link sayısı üzerinden alıyo
            reportTable3.Columns.Add("Atıf Sayısı", typeof(int));
            reportTable3.Columns.Add("Hoca Sayısı", typeof(int)); // textbox ekle , hoca sayısı elle girilir
            reportTable3.Columns.Add("Hoca Başına Makale", typeof(double)); // sonuç      Makale Sayısı / Hoca sayısı

            // datayı yıla göre grupla
            var groupedData = dtMakale.AsEnumerable()
                .GroupBy(row => row.Field<int>("Year")) // yılı al
                .Select(group =>
                {
                    int year = group.Key;
                    int linkCount = group.Count(); // yıla denk gelen link sayısını say
                    int citedByCount = group.Sum(row => ParseAndConvertToInt(row.Field<string>("Cited by")));  // yıla denk gelen atıf sayısını say

                    return new { Year = year, LinkCount = linkCount, CitedByCount = citedByCount };
                })
                .Where(result => result.Year != 0); // null yada sıfırları filtreleme

            // raporlama datatableını oluştur
            foreach (var item in groupedData)
            {
                DataRow newRow = reportTable3.NewRow();
                newRow["Yıl"] = item.Year;
                newRow["Makale Sayısı"] = item.LinkCount;
                newRow["Atıf Sayısı"] = item.CitedByCount;
                newRow["Hoca Sayısı"] = 0; // 0 = default değer
                newRow["Hoca Başına Makale"] = 0; // 0 = default değer   makale sayısı / hoca sayısı
                reportTable3.Rows.Add(newRow);
            }

            // hedefteki yani form6 daki datatable ın data kaynağı olarak burayı göster
            targetForm.RaporDataKaynagiMakale(reportTable3);
        }


        // form 6 
        private void RaporOlustur(Form6 targetForm)
        {
            // dt3 dolumu bak
            if (dt3 == null)
            {
                MessageBox.Show("lütfen öncelikle eşleştirme yapın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // raporlama için database oluştur
            DataTable reportTable = new DataTable();
            reportTable.Columns.Add("Yıl", typeof(int));
            reportTable.Columns.Add("Makale Sayısı", typeof(int)); // makale sayısını link sayısı üzerinden alıyo
            reportTable.Columns.Add("Atıf Sayısı", typeof(int));
            reportTable.Columns.Add("Hoca Sayısı", typeof(int)); // textbox ekle , hoca sayısı elle girilir
            reportTable.Columns.Add("Hoca Başına Makale", typeof(double)); // sonuç      Makale Sayısı / Hoca sayısı

            // datayı yıla göre grupla
            var groupedData = dt3.AsEnumerable()
                .GroupBy(row => row.Field<int>("Year")) // yılı al
                .Select(group =>
                {
                    int year = group.Key;
                    int linkCount = group.Count(); // yıla denk gelen link sayısını say
                    int citedByCount = group.Sum(row => ParseAndConvertToInt(row.Field<string>("Cited by")));  // yıla denk gelen atıf sayısını say

                    return new { Year = year, LinkCount = linkCount, CitedByCount = citedByCount };
                })
                .Where(result => result.Year != 0); // null yada sıfırları filtreleme

            // raporlama datatableını oluştur
            foreach (var item in groupedData)
            {
                DataRow newRow = reportTable.NewRow();
                newRow["Yıl"] = item.Year;
                newRow["Makale Sayısı"] = item.LinkCount;
                newRow["Atıf Sayısı"] = item.CitedByCount;
                newRow["Hoca Sayısı"] = 0; // 0 = default değer
                newRow["Hoca Başına Makale"] = 0; // 0 = default değer   makale sayısı / hoca sayısı
                reportTable.Rows.Add(newRow);
            }

            // hedefteki yani form6 daki datatable ın data kaynağı olarak burayı göster
            targetForm.RaporDataKaynagi(reportTable);
        }

        private void RaporOlusturDupesiz(Form6 targetForm)
        {
            // dt3 dolumu bak
            if (dt3 == null)
            {
                MessageBox.Show("lütfen öncelikle eşleştirme yapın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // raporlama için database oluştur
            DataTable reportTable2 = new DataTable();
            reportTable2.Columns.Add("Yıl", typeof(int));
            reportTable2.Columns.Add("Makale Sayısı", typeof(int)); // makale sayısını link sayısı üzerinden alıyo
            reportTable2.Columns.Add("Atıf Sayısı", typeof(int));
            reportTable2.Columns.Add("Hoca Sayısı", typeof(int)); // textbox ekle , hoca sayısı elle girilir
            reportTable2.Columns.Add("Hoca Başına Makale", typeof(double)); // sonuç      Makale Sayısı / Hoca sayısı

            // aynı linkleri tek say 
            HashSet<string> uniqueLinks = new HashSet<string>();
            Dictionary<string, int> linkCitedByCounts = new Dictionary<string, int>();

            // datayı yıla göre grupla
            var groupedData = dt3.AsEnumerable()
                .GroupBy(row => row.Field<int>("Year")) // yılı al
                .Select(group =>
                {
                    int year = group.Key;
                    int linkCount = 0;
                    int citedByCount = 0;

                    foreach (var row in group)
                    {
                        string link = row.Field<string>("Link");

                        // linkler eşsizmi bak eşsizse +1 say
                        if (uniqueLinks.Add(link))
                        {
                            linkCount++;

                            // link daha önce sayıldımı bak yoksa ekle varsa atla ve linke  eş gelen atıfı paralel oalrak al 
                            if (!linkCitedByCounts.ContainsKey(link))
                            {
                                linkCitedByCounts[link] = ParseAndConvertToInt(row.Field<string>("Cited by"));
                                citedByCount += linkCitedByCounts[link];
                            }
                        }
                    }

                    return new { Year = year, LinkCount = linkCount, CitedByCount = citedByCount };
                })
                .Where(result => result.Year != 0); // null yada sıfırları filtreleme

            // raporlama datatableını oluştur
            foreach (var item in groupedData)
            {
                DataRow newRow = reportTable2.NewRow();
                newRow["Yıl"] = item.Year;
                newRow["Makale Sayısı"] = item.LinkCount;
                newRow["Atıf Sayısı"] = item.CitedByCount;
                newRow["Hoca Sayısı"] = 0; // 0 = default değer
                newRow["Hoca Başına Makale"] = 0; // 0 = default değer   makale sayısı / hoca sayısı
                reportTable2.Rows.Add(newRow);
            }

            // hedefteki yani form6 daki datatable ın data kaynağı olarak burayı göster
            targetForm.RaporDataKaynagiDupesiz(reportTable2);

            
        }


        /*  private void RaporOlusturDupesiz(Form6 targetForm)
          {
              // dt3 dolumu bak
              if (dt3 == null)
              {
                  MessageBox.Show("lütfen öncelikle eşleştirme yapın");
                  return;
              }

              // raporlama için database oluştur
              DataTable reportTable = new DataTable();
              reportTable.Columns.Add("Yıl", typeof(int));
              reportTable.Columns.Add("Makale Sayısı", typeof(int)); // makale sayısını link sayısı üzerinden alıyo
              reportTable.Columns.Add("Atıf Sayısı", typeof(int));
              reportTable.Columns.Add("Hoca Sayısı", typeof(int)); // textbox ekle , hoca sayısı elle girilir
              reportTable.Columns.Add("Hoca Başına Makale", typeof(double)); // sonuç      Makale Sayısı / Hoca sayısı

              // datayı yıla göre grupla
              var groupedData = dt3.AsEnumerable()
                  .GroupBy(row => row.Field<int>("Year")) // yılı al
                  .Select(group =>
                  {
                      int year = group.Key;
                      int linkCount = group.Count(); // yıla denk gelen link sayısını say
                      int citedByCount = group.Sum(row => ParseAndConvertToInt(row.Field<string>("Cited by")));  // yıla denk gelen atıf sayısını say

                      return new { Year = year, LinkCount = linkCount, CitedByCount = citedByCount };
                  })
                  .Where(result => result.Year != 0); // null yada sıfırları filtreleme

              // raporlama datatableını oluştur
              foreach (var item in groupedData)
              {
                  DataRow newRow = reportTable.NewRow();
                  newRow["Yıl"] = item.Year;
                  newRow["Makale Sayısı"] = item.LinkCount;
                  newRow["Atıf Sayısı"] = item.CitedByCount;
                  newRow["Hoca Sayısı"] = 0; // 0 = default değer
                  newRow["Hoca Başına Makale"] = 0; // 0 = default değer   makale sayısı / hoca sayısı
                  reportTable.Rows.Add(newRow);
              }

              // hedefteki yani form6 daki datatable ın data kaynağı olarak burayı göster
              targetForm.RaporDataKaynagiDupesiz(reportTable);
          }   */

        // atıf sayısını toplayabilmek için gerekti
        private int ParseAndConvertToInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            // boşsa 0 döndür
            return 0;
        }




        // fonksiyonların kullanıldığı alanlar
        // *********************************************************
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IdAramayap();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            IsimileAramayap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Veriler Eşleştiriliyor Lütfen Bekleyiniz..");
            EşleştirVeGöster();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dataGridView3.Rows.Count == 0 || dataGridView3 == null)
            {
                MessageBox.Show("Lütfen önce verileri yükleyiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            csvaktar();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EşleşmeyenMakaleleriBulVeGöster();
        }

        public DataGridView DataGridView3 { get { return dataGridView3; } }

        private void button4_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();

            // raporu form6 da oluştur
            RaporOlustur(form6);
            RaporOlusturDupesiz(form6);
            RaporOlusturMakale(form6);
            

            // formu aç
            form6.Show();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // grid3 dolumu bak
            if (dataGridView3.Rows.Count == 0 || dataGridView3 == null)
            {
                MessageBox.Show("lütfen önce eşleştirme yapınız", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // datagridview 3 ü form 7 ye aktar
            Form7 reportForm = new Form7(dataGridView3);
            reportForm.DataGir((DataTable)dataGridView3.DataSource);
            reportForm.YazarGir((DataTable)dataGridView3.DataSource);
            reportForm.AtifGir((DataTable)dataGridView3.DataSource);
            reportForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView3.Rows.Count == 0 || dataGridView3 == null)
            {
                MessageBox.Show("Lütfen önce verileri yükleyin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                Form4 form4 = new Form4();
                form4.Show();
            }

           
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        
    }
    }

