using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Takip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        connection conn = new connection();
        SqlDataAdapter da;
        DataSet ds;
        string[] store_information, store_information1;
        double storefuel,sonuc,sonuctanker,storefuel1;
        double alınanyakit;
        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = DateTime.Today;
            progressBar1.Visible = false;
            label7.Text = "";
            label37.Text = "";
            label38.Text = "";
            label4.Text = "";
            label6.Text = "";
            label28.Text = "";
            label29.Text = "";
            txt_kmdifference.Visible = false;
            txt_yakilan.Visible = false;
            groupBox3.Visible = false;
            progressBar2.Visible = false;
            label39.Text = "";
            this.WindowState = FormWindowState.Maximized;

            SqlConnection connection = new SqlConnection(conn.Address);
            connection.Open();
            DataTable dt = connection.GetSchema("Tables");

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                cb_fuel.Items.Add(dt.Rows[i]["TABLE_NAME"]);
                cb_select.Items.Add(dt.Rows[i]["TABLE_NAME"]);
            }

            connection.Close();
        }
        private void Lists()
        {
            SqlConnection connection = new SqlConnection(conn.Address);
            string table = cb_select.Text;
            string command = "SELECT *FROM " + table;
            da = new SqlDataAdapter(command, connection);
            ds = new DataSet();
            connection.Open();
            da.Fill(ds, table);
            dataGridView1.DataSource = ds.Tables[table];
            connection.Close();

        }

        void save()
        {
            try
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                if (cb_fuel.SelectedItem.ToString() == "Depo")
                {
                    store_information = File.ReadAllLines(@"c:\depo.txt");
                    storefuel = Convert.ToDouble(store_information[0]);
                    double alınanyakit, litrefiyati, tutar;
                    double kmdegeri, yakilan, yakit;
                    alınanyakit = Convert.ToDouble(txt_liter.Text);
                    if (storefuel > 0)
                    {

                        if (storefuel >= alınanyakit)
                        {
                            if (txt_plaque.Text == "TANKER" || txt_plaque.Text == "tanker" || txt_plaque.Text == "Tanker")
                            {

                                string insert = "insert into Depo (Plaka,AlinanLitre,Km,LitreFiyat,TarihveSaat,Sofor,Fisno,AracMarka,KayitYapan,Harcanan,Yakilan,KmFark) values ('" + txt_plaque.Text + "','" + txt_liter.Text + "','" + txt_km.Text + "','" + txt_cost.Text + "','" + dateTimePicker1.Text + "','" + txt_driver.Text + "','" + txt_receipt.Text + "','" + txt_brand.Text + "','" + txt_recording.Text + "',@Harcanan,@Yakilan,@KmFark)";
                                SqlCommand command = new SqlCommand(insert, connection);
                                connection.Open();
                                command.Connection = connection;

                                alınanyakit = Convert.ToDouble(txt_liter.Text);
                                litrefiyati = Convert.ToDouble(txt_cost.Text.Replace(".", ","));
                                tutar = litrefiyati * alınanyakit;
                                command.Parameters.AddWithValue("@Harcanan", tutar + "TL");
                                yakit = Convert.ToDouble(txt_liter.Text);
                                if (txt_kmdifference.Text != "")
                                {
                                    kmdegeri = Convert.ToDouble(txt_kmdifference.Text);
                                    yakilan = yakit / kmdegeri;
                                    txt_yakilan.Text = "%" + yakilan.ToString();
                                    command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);

                                }
                                else if (txt_kmdifference.Text == "")
                                {
                                    txt_yakilan.Text = "0";
                                    command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);
                                    txt_kmdifference.Text = "0";
                                }
                                command.Parameters.AddWithValue("@KmFark", txt_kmdifference.Text);
                                command.ExecuteNonQuery();
                                label4.Text = "KM Fark";
                                label6.Text = "Aracın KM Başına Yakıt Tüketimi";
                                label28.Text = ":";
                                label29.Text = ":";
                                txt_kmdifference.Visible = true;
                                txt_yakilan.Visible = true;

                                StreamWriter dosya = new StreamWriter(@"C:\depo.txt");
                                alınanyakit = Convert.ToDouble(txt_liter.Text);
                                sonuc = storefuel - alınanyakit;
                                dosya.WriteLine(sonuc);
                                dosya.Close();
                                progressBar_guncelle();
                                progressBar2.Visible = true;
                                label38.Text = "Kalan Yakıt Miktarı :";
                                label37.Text = "Depo Yakıt Miktarı :";
                                if (DialogResult.OK == MessageBox.Show("Depo Kayıt İşlemi Gerçekleşti."))
                                {
                                    label4.Text = "";
                                    label6.Text = "";
                                    label28.Text = "";
                                    label29.Text = "";
                                    txt_kmdifference.Visible = false;
                                    txt_yakilan.Visible = false;
                                    label38.Text = "";
                                    label39.Text = "";
                                    progressBar2.Visible = false;
                                    store_information1 = File.ReadAllLines(@"c:\tanker.txt");
                                    storefuel1 = Convert.ToDouble(store_information1[0]);
                                    StreamWriter dosya1 = new StreamWriter(@"C:\tanker.txt");

                                    alınanyakit = Convert.ToDouble(txt_liter.Text);
                                    sonuctanker = storefuel1 + alınanyakit;
                                    dosya1.WriteLine(sonuctanker);
                                    MessageBox.Show("Tanker Güncellendi.Tanker Yakıt Miktarı :" + sonuctanker);
                                    dosya1.Close();

                                }
                                Clear();

                            }
                            else
                            {
                                // double alınanyakit, litrefiyati, tutar;
                                //  double kmdegeri, yakilan, yakit;
                                string insert = "insert into Depo (Plaka,AlinanLitre,Km,LitreFiyat,TarihveSaat,Sofor,Fisno,AracMarka,KayitYapan,Harcanan,Yakilan,KmFark) values ('" + txt_plaque.Text + "','" + txt_liter.Text + "','" + txt_km.Text + "','" + txt_cost.Text + "','" + dateTimePicker1.Text + "','" + txt_driver.Text + "','" + txt_receipt.Text + "','" + txt_brand.Text + "','" + txt_recording.Text + "',@Harcanan,@Yakilan,@KmFark)";
                                SqlCommand command = new SqlCommand(insert, connection);
                                connection.Open();
                                command.Connection = connection;

                                alınanyakit = Convert.ToDouble(txt_liter.Text);
                                litrefiyati = Convert.ToDouble(txt_cost.Text.Replace(".", ","));
                                tutar = litrefiyati * alınanyakit;
                                command.Parameters.AddWithValue("@Harcanan", tutar + "TL");
                                yakit = Convert.ToDouble(txt_liter.Text);
                                if (txt_kmdifference.Text != "")
                                {
                                    kmdegeri = Convert.ToDouble(txt_kmdifference.Text);
                                    yakilan = yakit / kmdegeri;
                                    txt_yakilan.Text = "%" + yakilan.ToString();
                                    command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);

                                }
                                else if (txt_kmdifference.Text == "")
                                {
                                    txt_yakilan.Text = "0";
                                    command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);
                                    txt_kmdifference.Text = "0";
                                }
                                command.Parameters.AddWithValue("@KmFark", txt_kmdifference.Text);
                                command.ExecuteNonQuery();
                                label4.Text = "KM Fark";
                                label6.Text = "Aracın KM Başına Yakıt Tüketimi";
                                label28.Text = ":";
                                label29.Text = ":";
                                txt_kmdifference.Visible = true;
                                txt_yakilan.Visible = true;

                                StreamWriter dosya = new StreamWriter(@"C:\depo.txt");
                                alınanyakit = Convert.ToDouble(txt_liter.Text);
                                sonuc = storefuel - alınanyakit;
                                dosya.WriteLine(sonuc);
                                dosya.Close();
                                progressBar_guncelle();
                                progressBar2.Visible = true;
                                label38.Text = "Kalan Yakıt Miktarı :";
                                label37.Text = "Depo Yakıt Miktarı :";
                                if (DialogResult.OK == MessageBox.Show("Kayıt İşlemi Gerçekleşti."))
                                {
                                    label4.Text = "";
                                    label6.Text = "";
                                    label28.Text = "";
                                    label29.Text = "";
                                    txt_kmdifference.Visible = false;
                                    txt_yakilan.Visible = false;
                                    label38.Text = "";
                                    label39.Text = "";
                                    progressBar2.Visible = false;
                                }
                                Clear();
                            }
                        }
                        else if (storefuel < alınanyakit)
                        {
                            MessageBox.Show("Depo'da Yeterli Yakıt Bulunmamaktadır.");
                        }
                    }
                    else if (storefuel <= 0)
                    {
                        MessageBox.Show("Kayıt İşlemi Gerçekleştirilemedi.Depo'da Yakıt Bulunmamaktadır.");
                    }
                }
                else if (cb_fuel.SelectedItem.ToString() == "Tasitmatik")
                {
                    store_information = File.ReadAllLines(@"c:\tasitmatik.txt");
                    storefuel = Convert.ToDouble(store_information[0]);
                    double alınanyakit, litrefiyati, tutar;
                    double kmdegeri, yakilan, yakit;
                    alınanyakit = Convert.ToDouble(txt_liter.Text);
                    if (storefuel > 0)
                    {

                        if (storefuel >= alınanyakit)
                        {

                            string insert = "insert into Tasitmatik (Plaka,AlinanLitre,Km,LitreFiyat,TarihveSaat,Sofor,Fisno,AracMarka,KayitYapan,Harcanan,Yakilan,KmFark) values ('" + txt_plaque.Text + "','" + txt_liter.Text + "','" + txt_km.Text + "','" + txt_cost.Text + "','" + dateTimePicker1.Text + "','" + txt_driver.Text + "','" + txt_receipt.Text + "','" + txt_brand.Text + "','" + txt_recording.Text + "',@Harcanan,@Yakilan,@KmFark)";
                            SqlCommand command = new SqlCommand(insert, connection);
                            connection.Open();
                            command.Connection = connection;

                            alınanyakit = Convert.ToDouble(txt_liter.Text);
                            litrefiyati = Convert.ToDouble(txt_cost.Text.Replace(".", ","));
                            tutar = litrefiyati * alınanyakit;
                            command.Parameters.AddWithValue("@Harcanan", tutar + "TL");
                            yakit = Convert.ToDouble(txt_liter.Text);
                            if (txt_kmdifference.Text != "")
                            {
                                kmdegeri = Convert.ToDouble(txt_kmdifference.Text);
                                yakilan = yakit / kmdegeri;
                                txt_yakilan.Text = "%" + yakilan.ToString();
                                command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);

                            }
                            else if (txt_kmdifference.Text == "")
                            {
                                txt_yakilan.Text = "0";
                                command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);
                                txt_kmdifference.Text = "0";
                            }
                            command.Parameters.AddWithValue("@KmFark", txt_kmdifference.Text);
                            command.ExecuteNonQuery();
                            label4.Text = "KM Fark";
                            label6.Text = "Aracın KM Başına Yakıt Tüketimi";
                            label28.Text = "";
                            label29.Text = "";
                            txt_kmdifference.Visible = true;
                            txt_yakilan.Visible = true;

                            StreamWriter dosya = new StreamWriter(@"C:\tasitmatik.txt");
                            alınanyakit = Convert.ToDouble(txt_liter.Text);
                            sonuc = storefuel - alınanyakit;
                            dosya.WriteLine(sonuc);
                            dosya.Close();
                            progressBar_guncelle();
                            progressBar2.Visible = true;
                            label38.Text = "Kalan Yakıt Miktarı :";
                            label37.Text = "Taşıtmatik Yakıt Miktarı :";
                            if (DialogResult.OK == MessageBox.Show("Kayıt İşlemi Gerçekleşti."))
                            {
                                label4.Text = "";
                                label6.Text = "";
                                label28.Text = "";
                                label29.Text = "";
                                txt_kmdifference.Visible = false;
                                txt_yakilan.Visible = false;
                                label38.Text = "";
                                label39.Text = "";
                                progressBar2.Visible = false;
                            }
                            Clear();
                        }
                        else if (storefuel < alınanyakit)
                        {
                            MessageBox.Show("Taşıtmatik'te Yeterli Yakıt Bulunmamaktadır.");
                        }
                    }
                    else if (storefuel <= 0)
                    {
                        MessageBox.Show("Kayıt İşlemi Gerçekleştirilemedi.Taşıtmatik'te Yakıt Bulunmamaktadır.");
                    }
                }

                else if (cb_fuel.SelectedItem.ToString() == "Tedarikci")
                {
                    store_information = File.ReadAllLines(@"c:\tedarikci.txt");
                    storefuel = Convert.ToDouble(store_information[0]);
                    double alınanyakit, litrefiyati, tutar;
                    double kmdegeri, yakilan, yakit;
                    alınanyakit = Convert.ToDouble(txt_liter.Text);
                    if (storefuel > 0)
                    {

                        if (storefuel >= alınanyakit)
                        {

                            string insert = "insert into Tedarikci (Plaka,AlinanLitre,Km,LitreFiyat,TarihveSaat,Sofor,Fisno,AracMarka,KayitYapan,Harcanan,Yakilan,KmFark) values ('" + txt_plaque.Text + "','" + txt_liter.Text + "','" + txt_km.Text + "','" + txt_cost.Text + "','" + dateTimePicker1.Text + "','" + txt_driver.Text + "','" + txt_receipt.Text + "','" + txt_brand.Text + "','" + txt_recording.Text + "',@Harcanan,@Yakilan,@KmFark)";
                            SqlCommand command = new SqlCommand(insert, connection);
                            connection.Open();
                            command.Connection = connection;

                            alınanyakit = Convert.ToDouble(txt_liter.Text);
                            litrefiyati = Convert.ToDouble(txt_cost.Text.Replace(".", ","));
                            tutar = litrefiyati * alınanyakit;
                            command.Parameters.AddWithValue("@Harcanan", tutar + "TL");
                            yakit = Convert.ToDouble(txt_liter.Text);
                            if (txt_kmdifference.Text != "")
                            {
                                kmdegeri = Convert.ToDouble(txt_kmdifference.Text);
                                yakilan = yakit / kmdegeri;
                                txt_yakilan.Text = "%" + yakilan.ToString();
                                command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);

                            }
                            else if (txt_kmdifference.Text == "")
                            {
                                txt_yakilan.Text = "0";
                                command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);
                                txt_kmdifference.Text = "0";
                            }
                            command.Parameters.AddWithValue("@KmFark", txt_kmdifference.Text);
                            command.ExecuteNonQuery();
                            label4.Text = "KM Fark";
                            label6.Text = "Aracın KM Başına Yakıt Tüketimi";
                            label28.Text = ":";
                            label29.Text = ":";
                            txt_kmdifference.Visible = true;
                            txt_yakilan.Visible = true;

                            StreamWriter dosya = new StreamWriter(@"C:\tedarikci.txt");
                            alınanyakit = Convert.ToDouble(txt_liter.Text);
                            sonuc = storefuel - alınanyakit;
                            dosya.WriteLine(sonuc);
                            dosya.Close();
                            progressBar_guncelle();
                            progressBar2.Visible = true;
                            label38.Text = "Kalan Yakıt Miktarı :";
                            label37.Text = "Tedarikçi Yakıt Miktarı :";
                            if (DialogResult.OK == MessageBox.Show("Kayıt İşlemi Gerçekleşti."))
                            {
                                label4.Text = "";
                                label6.Text = "";
                                label28.Text = "";
                                label29.Text = "";
                                txt_kmdifference.Visible = false;
                                txt_yakilan.Visible = false;
                                label38.Text = "";
                                label39.Text = "";
                                progressBar2.Visible = false;
                            }
                            Clear();
                        }
                        else if (storefuel < alınanyakit)
                        {
                            MessageBox.Show("Tedarikçi'de Yeterli Yakıt Bulunmamaktadır.");
                        }
                    }
                    else if (storefuel <= 0)
                    {
                        MessageBox.Show("Kayıt İşlemi Gerçekleştirilemedi.Tedarikçi'de Yakıt Bulunmamaktadır.");
                    }
                }
                else if (cb_fuel.SelectedItem.ToString() == "Tanker")
                {
                    store_information = File.ReadAllLines(@"c:\tanker.txt");
                    storefuel = Convert.ToDouble(store_information[0]);
                    double alınanyakit, litrefiyati, tutar;
                    double kmdegeri, yakilan, yakit;
                    alınanyakit = Convert.ToDouble(txt_liter.Text);
                    if (storefuel > 0)
                    {

                        if (storefuel >= alınanyakit)
                        {

                            string insert = "insert into Tanker (Plaka,AlinanLitre,Km,LitreFiyat,TarihveSaat,Sofor,Fisno,AracMarka,KayitYapan,Harcanan,Yakilan,KmFark) values ('" + txt_plaque.Text + "','" + txt_liter.Text + "','" + txt_km.Text + "','" + txt_cost.Text + "','" + dateTimePicker1.Text + "','" + txt_driver.Text + "','" + txt_receipt.Text + "','" + txt_brand.Text + "','" + txt_recording.Text + "',@Harcanan,@Yakilan,@KmFark)";
                            SqlCommand command = new SqlCommand(insert, connection);
                            connection.Open();
                            command.Connection = connection;

                            alınanyakit = Convert.ToDouble(txt_liter.Text);
                            litrefiyati = Convert.ToDouble(txt_cost.Text.Replace(".", ","));
                            tutar = litrefiyati * alınanyakit;
                            command.Parameters.AddWithValue("@Harcanan", tutar + "TL");
                            yakit = Convert.ToDouble(txt_liter.Text);
                            if (txt_kmdifference.Text != "")
                            {
                                kmdegeri = Convert.ToDouble(txt_kmdifference.Text);
                                yakilan = yakit / kmdegeri;
                                txt_yakilan.Text = "%" + yakilan.ToString();
                                command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);

                            }
                            else if (txt_kmdifference.Text == "")
                            {
                                txt_yakilan.Text = "0";
                                command.Parameters.AddWithValue("@Yakilan", txt_yakilan.Text);
                                txt_kmdifference.Text = "0";
                            }
                            command.Parameters.AddWithValue("@KmFark", txt_kmdifference.Text);
                            command.ExecuteNonQuery();
                            label4.Text = "KM Fark";
                            label6.Text = "Aracın KM Başına Yakıt Tüketimi";
                            label28.Text = ":";
                            label29.Text = ":";
                            txt_kmdifference.Visible = true;
                            txt_yakilan.Visible = true;

                            StreamWriter dosya = new StreamWriter(@"C:\tanker.txt");
                            alınanyakit = Convert.ToDouble(txt_liter.Text);
                            sonuc = storefuel - alınanyakit;
                            dosya.WriteLine(sonuc);
                            dosya.Close();
                            progressBar_guncelle();
                            progressBar2.Visible = true;
                            label38.Text = "Kalan Yakıt Miktarı :";
                            label37.Text = "Tanker Yakıt Miktarı :";
                            if (DialogResult.OK == MessageBox.Show("Kayıt İşlemi Gerçekleşti."))
                            {
                                label4.Text = "";
                                label6.Text = "";
                                label28.Text = "";
                                label29.Text = "";
                                txt_kmdifference.Visible = false;
                                txt_yakilan.Visible = false;
                                label38.Text = "";
                                label39.Text = "";
                                progressBar2.Visible = false;
                            }
                            Clear();
                        }
                        else if (storefuel < alınanyakit)
                        {
                            MessageBox.Show("Tanker'de Yeterli Yakıt Bulunmamaktadır.");
                        }
                    }
                    else if (storefuel <= 0)
                    {
                        MessageBox.Show("Kayıt İşlemi Gerçekleştirilemedi.Tanker'de Yakıt Bulunmamaktadır.");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Kayıt İşlemi Gerçekleştirilemedi.");
            }
        }
            
        
        void Clear()
        {
            txt_plaque.Clear();
            txt_liter.Clear();
            txt_km.Clear();
            txt_cost.Clear();
            txt_driver.Clear();
            txt_receipt.Clear();
            txt_brand.Clear();
            txt_recording.Clear();
            txt_kmdifference.Clear();
            txt_yakilan.Clear();
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            excel.Visible = true;

            Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

            int StartCol = 1;

            int StartRow = 1;

            for (int j = 0; j < dataGridView1.Columns.Count; j++)
            {
                Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)
                    sheet1.Cells[StartRow, StartCol + j];
                myRange.Value2 = dataGridView1.Columns[j].HeaderText;
            }
            StartRow++; for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)
                            sheet1.Cells[StartRow + i, StartCol + j];
                        myRange.Value2 = dataGridView1[j, i].Value == null ? "" : dataGridView1[j, i].Value;
                    }
                    catch
                    {
                        ;
                    }
                }
            }
        }
        void DeleteRecord(int No)
        {
            SqlConnection connection = new SqlConnection(conn.Address);
            string table = cb_select.Text;
            string sql = "DELETE FROM " + table + " WHERE No=@No";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@No", No);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        private void delete_button_Click(object sender, EventArgs e)
        {
           
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult selection = new DialogResult();
                selection = MessageBox.Show("Seçili Satırı Silmek İstediğinizden Emin Misiniz", "YAKIT TAKİP FORMLARI", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (selection == DialogResult.Yes)
                {
                    foreach (DataGridViewRow drow in dataGridView1.SelectedRows)
                    {
                        int No = Convert.ToInt32(drow.Cells[0].Value);
                        DeleteRecord(No);
                    }
                    MessageBox.Show("Kayıt Başarıyla Silindi.");
                }

                Lists();
            }
        
            else
            {

                MessageBox.Show("Lütfen Silinecek Satırı Seçiniz.");

            }

        }
        void control()
        {

            try
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                connection.Open();
                string table = cb_fuel.Text;
                SqlCommand command = new SqlCommand("select Max(Km) from " + table + " where Plaka=@Plaka", connection);
                command.Parameters.AddWithValue("@Plaka", txt_plaque.Text);
                SqlDataReader dr = command.ExecuteReader();

                if (dr.Read())
                {

                    if (txt_plaque.Text == "")
                    {
                        MessageBox.Show("Plaka Boş Olamaz.");
                    }
                    else if (txt_liter.Text == "")
                    {
                        MessageBox.Show("Alınan Litre Boş Olamaz.");
                    }
                    else if (txt_km.Text == "")
                    {
                        MessageBox.Show("KM Bilgisi Boş Olamaz.");
                    }
                    else if (txt_cost.Text == "")
                    {
                        MessageBox.Show("Litre Fiyat Boş Olamaz.");
                    }
                    else
                    {
                        decimal datakm = dr.GetDecimal(0);
                        decimal km = Convert.ToDecimal(txt_km.Text.Replace(".", ","));

                        if (km == 0)
                        {
                           
                            connection.Close();
                            save();
                        }
                        else if (km <= datakm)
                        {
                            MessageBox.Show("Kayıt Yapılamaz.Km Değeri Önceki Kayıtlardan Büyük Olmalıdır.");
                        }
                        else if (km > datakm)
                        {
                           
                            connection.Close();
                            decimal fark = km - datakm;
                            txt_kmdifference.Text = fark.ToString();
                            save();
                        }                     
                    }
                }
            }
            catch
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                connection.Open();
                string table = cb_fuel.Text;
                SqlCommand command = new SqlCommand("select * from " + table + " WHERE Plaka=@Plaka", connection);
                command.Parameters.AddWithValue("@Plaka", txt_plaque.Text);
                SqlDataReader dr = command.ExecuteReader();
                string plaque = Convert.ToString(txt_plaque.Text);
                if (dr.Read() == false)
                {
                    if (txt_plaque.Text == "")
                    {
                        MessageBox.Show("Plaka Boş Olamaz.");
                    }
                    else if (txt_liter.Text == "")
                    {
                        MessageBox.Show("Alınan Litre Boş Olamaz.");
                    }
                    else if (txt_km.Text == "")
                    {
                        MessageBox.Show("KM Bilgisi Boş Olamaz.");
                    }
                    else if (txt_cost.Text == "")
                    {
                        MessageBox.Show("Litre Fiyat Boş Olamaz.");
                    }
                    else
                    {
                      
                        save();
                    }
                }
                else if (dr.Read() == true)
                {
                    if (txt_plaque.Text == "")
                    {
                        MessageBox.Show("Plaka Boş Olamaz.");
                    }
                    else if (txt_liter.Text == "")
                    {
                        MessageBox.Show("Alınan Litre Boş Olamaz.");
                    }
                    else if (txt_km.Text == "")
                    {
                        MessageBox.Show("KM Bilgisi Boş Olamaz.");
                    }
                    else if (txt_cost.Text == "")
                    {
                        MessageBox.Show("Litre Fiyat Boş Olamaz.");
                    }

                }
            }
        }

        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            if (rdb_plaque.Checked)
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                connection.Open();
                string table = cb_select.Text;
                da = new SqlDataAdapter("Select * from " + table + " where Plaka Like'" + txt_search.Text + "%'", connection);// ½ işareti texbox sa girilen karakterden sonra sonuna hangi harf gelirse gelsin tümünü göster demektir.
                ds = new DataSet();
                da.Fill(ds, table);
                dataGridView1.DataSource = ds.Tables[table];
                connection.Close();
            }
            else if (rdb_driver.Checked)
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                connection.Open();
                string table = cb_select.Text;
                da = new SqlDataAdapter("Select * from " + table + " where Sofor Like'" + txt_search.Text + "%'", connection);// ½ işareti texbox sa girilen karakterden sonra sonuna hangi harf gelirse gelsin tümünü göster demektir.
                ds = new DataSet();
                da.Fill(ds, table);
                dataGridView1.DataSource = ds.Tables[table];
                connection.Close();
            }
            else if (rdb_vehiclebrand.Checked)
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                connection.Open();
                string table = cb_select.Text;
                da = new SqlDataAdapter("Select * from " + table + " where AracMarka Like'" + txt_search.Text + "%'", connection);// ½ işareti texbox sa girilen karakterden sonra sonuna hangi harf gelirse gelsin tümünü göster demektir.
                ds = new DataSet();
                da.Fill(ds, table);
                dataGridView1.DataSource = ds.Tables[table];
                connection.Close();
            }
            else
            {
                SqlConnection connection = new SqlConnection(conn.Address);
            }
        }

        private void cb_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Lists();
        }

        private void cb_fuel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(conn.Address);
            string table = cb_fuel.Text;
            string command = "SELECT *FROM " + table;
            da = new SqlDataAdapter(command, connection);
            ds = new DataSet();
            connection.Open();
            da.Fill(ds, table);
            connection.Close();
        }

        private void btn_ekle_Click(object sender, EventArgs e)
        {
           try
            {
                
                control();
           }
           catch
           {
            
               MessageBox.Show("Kayıt Yapılamadı.Formu Kontrol Ediniz.");
           }
        }

        private void btn_adding_Click(object sender, EventArgs e)
        {
            try
            {
                if (cb_yakit.SelectedItem.ToString() == "Depo")
                {
                    if (rtb_fuel.Text != "")
                    {
                        store_information = File.ReadAllLines(@"c:\depo.txt");
                        storefuel = Convert.ToDouble(store_information[0]);
                        StreamWriter dosya = new StreamWriter(@"C:\depo.txt");

                        alınanyakit = Convert.ToDouble(rtb_fuel.Text);
                        sonuc = storefuel + alınanyakit;
                        dosya.WriteLine(sonuc);
                        MessageBox.Show("Depo Güncellendi.");
                        dosya.Close();               
                        groupBox3.Visible = true;
                        progressBar_guncelle();
                        progressBar1.Visible = true;
                        label37.Text = "Depo Yakıt Miktarı :";
                        label39.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Lütfen Ekleme Yapacak Bir Değer Giriniz.");
                        
                    }
                }
                else if (cb_yakit.SelectedItem.ToString() == "Tedarikçi")
                {
                    if (rtb_fuel.Text != "")
                    {
                        store_information = File.ReadAllLines(@"c:\tedarikci.txt");
                        storefuel = Convert.ToDouble(store_information[0]);
                        StreamWriter dosya = new StreamWriter(@"C:\tedarikci.txt");

                        alınanyakit = Convert.ToDouble(rtb_fuel.Text);
                        sonuc = storefuel + alınanyakit;
                        dosya.WriteLine(sonuc);
                        MessageBox.Show("Tedarikçi Güncellendi.");
                        dosya.Close();
                        groupBox3.Visible = true;
                        progressBar_guncelle();
                        progressBar1.Visible = true;
                        label37.Text = "Tedarikçi Yakıt Miktarı :";
                        label39.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Lütfen Ekleme Yapacak Bir Değer Giriniz.");
                       
                    }
                }
                else if (cb_yakit.SelectedItem.ToString() == "Taşıtmatik")
                {
                    if (rtb_fuel.Text != "")
                    {
                        store_information = File.ReadAllLines(@"c:\tasitmatik.txt");
                        storefuel = Convert.ToDouble(store_information[0]);
                        StreamWriter dosya = new StreamWriter(@"C:\tasitmatik.txt");
                        alınanyakit = Convert.ToDouble(rtb_fuel.Text);
                        sonuc = storefuel + alınanyakit;
                        dosya.WriteLine(sonuc);
                        MessageBox.Show("Taşıtmatik Güncellendi.");
                        dosya.Close();
                        groupBox3.Visible = true;
                        progressBar_guncelle();
                        progressBar1.Visible = true;
                        label37.Text = "Taşıtmatik Yakıt Miktarı :";
                        label39.Text = "";
                    }
                  
                }
                else
                {
                    MessageBox.Show("Lütfen Ekleme Yapmak İstediğiniz Yakıt Deposunu Seçiniz.");
                }
            }
            catch
            {
                MessageBox.Show("Yakıt Eklenemedi.Lütfen Değerleri Kontrol Ediniz.");
            }
        }

        private void rtb_fuel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',';
        }


        private void progressBar_guncelle()
        {
            progressBar1.Maximum = 100000;
            progressBar1.Value = Convert.ToInt32(sonuc);
            progressBar2.Maximum = 100000;
            progressBar2.Value = Convert.ToInt32(sonuc);
            label39.Text = progressBar2.Value.ToString();
            label7.Text = progressBar1.Value.ToString();         
        }


        private void btn_refresh_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                SqlConnection connection = new SqlConnection(conn.Address);
                DialogResult selection = new DialogResult();

                selection = MessageBox.Show("Seçili Satırı Güncellemek İstediğinizden Emin Misiniz", "ARAÇ TAKİP FORMLARI", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (selection == DialogResult.Yes)
                {
                    
                    string table = cb_select.Text;
                    string no, plaque, alinanlitre, km, litrefiyat, tarihvesaat, sofor, fisno, aracmarka, kayityapan, harcanan, yakilan, kmfark;

                    no = dataGridView1.CurrentRow.Cells["No"].Value.ToString();
                    alinanlitre = dataGridView1.CurrentRow.Cells["AlinanLitre"].Value.ToString();
                    plaque = dataGridView1.CurrentRow.Cells["Plaka"].Value.ToString();
                    km = dataGridView1.CurrentRow.Cells["Km"].Value.ToString();
                    litrefiyat = dataGridView1.CurrentRow.Cells["LitreFiyat"].Value.ToString();
                    tarihvesaat = dataGridView1.CurrentRow.Cells["TarihveSaat"].Value.ToString();
                    sofor = dataGridView1.CurrentRow.Cells["Sofor"].Value.ToString();
                    fisno = dataGridView1.CurrentRow.Cells["Fisno"].Value.ToString();
                    aracmarka = dataGridView1.CurrentRow.Cells["AracMarka"].Value.ToString();
                    kayityapan = dataGridView1.CurrentRow.Cells["KayitYapan"].Value.ToString();
                    harcanan = dataGridView1.CurrentRow.Cells["Harcanan"].Value.ToString();
                    yakilan = dataGridView1.CurrentRow.Cells["Yakilan"].Value.ToString();
                    kmfark = dataGridView1.CurrentRow.Cells["KmFark"].Value.ToString();

                    connection.Open();
                    SqlCommand command = new SqlCommand("update " + table + " set AlinanLitre='" + alinanlitre + "',Plaka='" + plaque + "',Km='" + (float)Convert.ToDecimal(km) + "',LitreFiyat='" + litrefiyat + "',TarihveSaat='" + tarihvesaat + "',Sofor='" + sofor + "',Fisno='" + fisno + "',AracMarka='" + aracmarka + "',KayitYapan='" + kayityapan + "',Harcanan='" + harcanan + "',Yakilan='" + yakilan + "',KmFark='" + kmfark + "' where No='" + no + "'", connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    refresh();
                    MessageBox.Show("Güncelleme İşlemi Gerçekleştirildi.");
                }
            }
            else
            {
                MessageBox.Show("Güncelleme Yapılamadı.Lütfen Satır Seçiniz.");
            }
        }

        void refresh()   //refresh line
        {
            SqlConnection connection = new SqlConnection(conn.Address);
            
            string table = cb_select.Text;
            string command = "SELECT *FROM " + table;
            da = new SqlDataAdapter(command, connection);
            ds = new DataSet();
            connection.Open();
            da.Fill(ds, table);
            dataGridView1.DataSource = ds.Tables[table];
            connection.Close();
        }
        private void txt_km_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.';
        }

        private void txt_cost_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',';
        }

        private void txt_plaque_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
            if ((int)e.KeyChar == 32)
            {
                e.Handled = true;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = DateTime.Today;
        }
        private void txt_driver_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                        && !char.IsSeparator(e.KeyChar);
        }

        private void txt_recording_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                        && !char.IsSeparator(e.KeyChar);
        }

        private void txt_liter_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',';
        }


    }
}


