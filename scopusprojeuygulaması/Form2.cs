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
    public partial class Form2 : Form
    {
        private Form3 form3;
        private Form5 form5;
      
        MySqlConnection conn;
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataAdapter adapter;
        DataTable dtMakale;
        DataTable dtKadro;
        DataTable dtQ;
        DataTable dt3;

        string makaleTablo;
        string kadroTablo;
        string qTablo;

        public Form2()
        { 
            InitializeComponent();
            conn = new MySqlConnection();
            
            form3 = new Form3(dtMakale, dtKadro, conn, adapter, dt3, dtQ);
            form5 = new Form5(conn, adapter);
        }

        private void Form2_Load(object sender, EventArgs e)
        {        
        }

        // SQL BAĞLANTISININ GERÇEKLEŞTİRİLDİĞİ KODLAR
        // SQL BAĞLANTISININ GERÇEKLEŞTİRİLDİĞİ KODLAR
        // SQL BAĞLANTISININ GERÇEKLEŞTİRİLDİĞİ KODLAR
        // **************************************************************************



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
                    comboBox2.Items.Add(reader.GetString(0)); // Yeni eklenen kod
                    comboBox3.Items.Add(reader.GetString(0)); 
                }

                // Veritabanı bağlantısını kapat
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tablo adları yüklenirken bir hata oluştu: " + ex.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ComboBox'taki seçili tablo adına göre verileri getiren fonksiyon
        private void makaleVerileriniGetir(string tabloAdi)
        {
            // ComboBox'tan seçilen tablo adı ile verileri getir
            if (tabloAdi == "")
            {
                MessageBox.Show("Lütfen bir tablo seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen tablo adı ile verileri getir
            form3.makaleVeriGetir(tabloAdi);
        }

        private void kadroVerileriniGetir(string tabloAdi)
        {
            // ComboBox'tan seçilen tablo adı ile verileri getir
            if (tabloAdi == "")
            {
                MessageBox.Show("Lütfen bir tablo seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen tablo adı ile verileri getir
            form3.kadroVeriGetir(tabloAdi);
        }

        private void qVerileriniGetir(string tabloAdi)
        {
            // ComboBox'tan seçilen tablo adı ile verileri getir
            if (tabloAdi == "")
            {
                MessageBox.Show("Lütfen bir tablo seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen tablo adı ile verileri getir
            form3.qVeriGetir(tabloAdi);
        }

        //SQL BAĞLANTISINI YAPAN BUTON
        private void button4_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan sunucu, veritabanı, kullanıcı adı ve şifreyi al
            string server = txtServer.Text;
            string database = txtDatabase.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Bağlantı dizesini oluştur
            string connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};Charset=utf8mb4";

            // MySqlConnection nesnesini oluştur ve bağlantıyı aç
            conn.ConnectionString = connectionString;

            try
            {
                conn.Open();
                MessageBox.Show("Bağlantı başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Bağlantı başarılı olduktan sonra gerekli işlemleri yapabilirsiniz
                // Örneğin, verileri çekme veya işleme işlemleri
                TabloAdlariniYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçili tablo adını al
              makaleTablo = comboBox1.SelectedItem.ToString();

           

          
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçili tablo adını al
            kadroTablo = comboBox2.SelectedItem.ToString();

            
           

          
        }

       /* private void button5_Click(object sender, EventArgs e)
        {
            // Bağlantıyı aç
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

        private void button1_Click(object sender, EventArgs e)
        {
           
            Form3 form3 = new Form3(dtMakale, dtKadro, conn, adapter, dt3, dtQ);
            form3.makaleVeriGetir(makaleTablo);
            form3.kadroVeriGetir(kadroTablo);
            form3.qVeriGetir(qTablo);
            form3.ShowDialog();
        }

       /* private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }*/

        private void button3_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(conn,adapter);
            form5.ShowDialog();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            qTablo = comboBox3.SelectedItem.ToString();
        }
    }
}
