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
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();

            // bölme işlemini uygula
            genelDataView.CellEndEdit += genelGridView_CellEndEdit;
            genelDataDupesiz.CellEndEdit += genelGridDupesiz_CellEndEdit;
            genelDataMakale.CellEndEdit += genelGridMakale_CellEndEdit;
        }

        

        private void Form6_Load(object sender, EventArgs e)
        {
        
        }

        public void RaporDataKaynagiMakale(DataTable reportData3)
        {
            genelDataMakale.DataSource = reportData3;
        }

        public void RaporDataKaynagiDupesiz(DataTable reportData2)
        {
            genelDataDupesiz.DataSource = reportData2;
        }

        public void RaporDataKaynagi(DataTable reportData)
        {
            genelDataView.DataSource = reportData;
        }

        private void genelGridMakale_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // hoca sayısı girildimi bak
            if (e.ColumnIndex == genelDataMakale.Columns["Hoca Sayısı"].Index && e.RowIndex >= 0)
            {
                // satırlardan değeri al
                int linkCount = Convert.ToInt32(genelDataMakale.Rows[e.RowIndex].Cells["Makale Sayısı"].Value);
                int inputNumbers = Convert.ToInt32(genelDataMakale.Rows[e.RowIndex].Cells["Hoca Sayısı"].Value);

                // böl ve hoca başına makaleyi güncelle
                double result = inputNumbers == 0 ? 0 : (double)linkCount / inputNumbers;
                genelDataMakale.Rows[e.RowIndex].Cells["Hoca Başına Makale"].Value = result;
            }
        }

        private void genelGridDupesiz_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // hoca sayısı girildimi bak
            if (e.ColumnIndex == genelDataDupesiz.Columns["Hoca Sayısı"].Index && e.RowIndex >= 0)
            {
                // satırlardan değeri al
                int linkCount = Convert.ToInt32(genelDataDupesiz.Rows[e.RowIndex].Cells["Makale Sayısı"].Value);
                int inputNumbers = Convert.ToInt32(genelDataDupesiz.Rows[e.RowIndex].Cells["Hoca Sayısı"].Value);

                // böl ve hoca başına makaleyi güncelle
                double result = inputNumbers == 0 ? 0 : (double)linkCount / inputNumbers;
                genelDataDupesiz.Rows[e.RowIndex].Cells["Hoca Başına Makale"].Value = result;
            }
        }

        // textboxa kadro değerini girdikten sonra sonuç boxunu güncellemesi için  (bölme işlemi)
        private void genelGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // hoca sayısı girildimi bak
            if (e.ColumnIndex == genelDataView.Columns["Hoca Sayısı"].Index && e.RowIndex >= 0)
            {
                // satırlardan değeri al
                int linkCount = Convert.ToInt32(genelDataView.Rows[e.RowIndex].Cells["Makale Sayısı"].Value);
                int inputNumbers = Convert.ToInt32(genelDataView.Rows[e.RowIndex].Cells["Hoca Sayısı"].Value);

                // böl ve hoca başına makaleyi güncelle
                double result = inputNumbers == 0 ? 0 : (double)linkCount / inputNumbers;
                genelDataView.Rows[e.RowIndex].Cells["Hoca Başına Makale"].Value = result;
            }
        }

        
    }
}
