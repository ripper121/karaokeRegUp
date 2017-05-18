using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Drawing.Text;

namespace Anmeldung
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(int hWnd, uint msg, int wParam, int lParam);

        DataTable MusicDataTable;
        double CustomerCount = 0;
        String customerFolder = "";
        String SongFile = "";
        String ClosePassword = null;
        String registrationURL = "";
        String disclaimer = "";
        PrivateFontCollection fonts;
        FontFamily family;
        Font theFont_Big;
        Font theFont_Small;
        Font theFont_Smaller;

        private Configuration config = null;

        public static FontFamily LoadFontFamily(string fileName, out PrivateFontCollection fontCollection)
        {
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(fileName);
            return fontCollection.Families[0];
        }

        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.CustomFormat = "dd.MM.yyyy";
            dateTimePicker1.Value = DateTime.Now;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FontFamily family = LoadFontFamily(Application.StartupPath + @"\Fonts\HyundaiSansTextOffice-Regular.ttf", out fonts);
            Font theFont_Big = new Font(family, 15.0f);
            Font theFont_Small = new Font(family, 8.0f);
            Font theFont_Bigger = new Font(family, 25.0f);

            try
            {
                config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                CustomerCount = Convert.ToDouble(config.AppSettings.Settings["CustomerCount"].Value);
                SongFile = config.AppSettings.Settings["SongFile"].Value;
                ClosePassword = config.AppSettings.Settings["ClosePassword"].Value;
                customerFolder = config.AppSettings.Settings["CustomerFolder"].Value + @"\";
                registrationURL = config.AppSettings.Settings["registrationURL"].Value;
                disclaimer = config.AppSettings.Settings["Disclaimer"].Value;
                if (!Directory.Exists(customerFolder))
                {
                    MessageBox.Show("Cant find customerFolder:" + customerFolder);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant load config file.");
                return;
            }


            try
            {
                Cursor.Current = Cursors.WaitCursor;
                MusicDataTable = ConvertCSVtoDataTable(SongFile);

                dataGridView1.DefaultCellStyle.Font = theFont_Big;
                dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(0, 44, 95);
                dataGridView1.DefaultCellStyle.ForeColor = Color.White;
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.DataSource = MusicDataTable;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                dataGridView1.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["ID"].Width = 1;
                dataGridView1.Columns["ID"].Visible = false;
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 44, 95);
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.RowHeadersDefaultCellStyle.Font = theFont_Big;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 44, 95);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = theFont_Big;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Cant open Song-File.\nPlease set it in the config.");
                return;
            }

            try
            {
                pictureBox1.Image = Image.FromFile("Anmeldeoverlay.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant open Anmeldeoverlay.png");
                return;
            }


            // this.TopMost = true;
            this.Location = new Point(0, 0);
            this.Width = Screen.PrimaryScreen.Bounds.Width + 16;
            this.Height = Screen.PrimaryScreen.Bounds.Height - 270;
            this.FormBorderStyle = FormBorderStyle.None;
            groupBox1.ForeColor = Color.White;
            groupBox2.ForeColor = Color.White;
            label6.Text = CustomerCount.ToString();
            textBoxDiscl.Font = new Font("Microsoft Sans Serif", 16);
            Cursor.Current = Cursors.Default;

            groupBox1.Font = theFont_Big;
            label11.Font = theFont_Small;
            dateTimePicker1.Font = theFont_Big;
            label9.Font = theFont_Big;
            textBox6.Font = theFont_Big;
            label3.Font = theFont_Big;
            label6.Font = theFont_Bigger;
            label5.Font = theFont_Big;
            textBox4.Font = theFont_Big;
            label4.Font = theFont_Big;
            textBox3.Font = theFont_Big;
            label2.Font = theFont_Big;
            textBox2.Font = theFont_Big;
            label1.Font = theFont_Big;
            button1.Font = theFont_Big;
            groupBox2.Font = theFont_Big;
            textBox1.Font = theFont_Big;
            textBox5.Font = theFont_Big;
            label8.Font = theFont_Big;
            label7.Font = theFont_Big;
            buttonReset.Font = theFont_Big;
            buttonOK.Font = theFont_Big;
            label10.Font = theFont_Bigger;
            buttonID.Font = theFont_Big;
            myCheckBox3.Font = theFont_Small;
            checkBoxAGB.Font = theFont_Small;
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(';');
                dt.Columns.Add(headers[0]);
                dt.Columns.Add(headers[1]);
                dt.Columns.Add(headers[2]);
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < 3; i++)
                    {
                        dr[i] = rows[i].Replace("\"", "");
                    }

                    dt.Rows.Add(dr);

                }

            }
            return dt;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
                textBox4.Text = row.Cells[0].Value.ToString() + " - " + row.Cells[1].Value.ToString() + " - " + row.Cells[2].Value.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //this code is used to search Name on the basis of textBox1.text
                ((DataTable)dataGridView1.DataSource).DefaultView.RowFilter = string.Format("Title like '%{0}%'", textBox1.Text.Trim().Replace("'", "''"));
                dataGridView1.CurrentRow.Selected = true;
            }
            catch (Exception) { }
            Cursor.Current = Cursors.Default;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //this code is used to search Name on the basis of textBox1.text
                ((DataTable)dataGridView1.DataSource).DefaultView.RowFilter = string.Format("Artist like '%{0}%'", textBox5.Text.Trim().Replace("'", "''"));
                dataGridView1.CurrentRow.Selected = true;
            }
            catch (Exception) { }
            Cursor.Current = Cursors.Default;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            ShowTouchKeyboard(true, false);
            if (textBox5.Text != String.Empty)
                textBox5.Text = String.Empty;
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            ShowTouchKeyboard(true, false);
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            ShowTouchKeyboard(true, false);
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            ShowTouchKeyboard(true, false);
        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            ShowTouchKeyboard(true, false);
            if (textBox1.Text != String.Empty)
                textBox1.Text = String.Empty;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShowTouchKeyboard(false, false);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!String.IsNullOrEmpty(ClosePassword))
            {
                this.TopMost = false;
                string input = Microsoft.VisualBasic.Interaction.InputBox("Bitte Passwort eingeben", "Beenden", "", -1, -1);
                e.Cancel = (input != ClosePassword);
                this.TopMost = true;
            }
            else
                e.Cancel = false;
        }

        public static class Http
        {
            public static byte[] Post(string uri, NameValueCollection pairs)
            {
                byte[] response = null;
                using (WebClient client = new WebClient())
                {
                    response = client.UploadValues(uri, pairs);
                }
                return response;
            }
        }

        int GetAge(DateTime bornDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - bornDate.Year;
            if (bornDate > today.AddYears(-age))
                age--;

            return age;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text.Trim()) || String.IsNullOrEmpty(textBox3.Text.Trim()) || String.IsNullOrEmpty(textBox4.Text.Trim()) || String.IsNullOrEmpty(textBox6.Text.Trim()))
            {
                MessageBox.Show("Bitte alle Felder ausfüllen.");
                return;
            }
            if (!textBox3.Text.Contains('@'))
            {
                MessageBox.Show("Bitte valide Mail verwenden: mymail@mydomain.com");
                return;
            }

            if (GetAge(dateTimePicker1.Value.Date) < 18 || !myCheckBox1.Checked)
            {
                MessageBox.Show("Sie müssen min. 18 Jahre alt sein!");
                return;
            }

            if (checkBoxAGB.Checked)
            {
                String User = "";
                String timestamp = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                String newsletter = "false";
                if (myCheckBox3.Checked)
                    newsletter = "true";

                User += CustomerCount.ToString() + "\n";
                User += textBox2.Text + "\n";
                User += textBox6.Text + "\n";
                User += dateTimePicker1.Text + "\n";
                User += textBox3.Text + "\n";
                User += textBox4.Text + "\n";
                User += timestamp + "\n";

                try
                {

                    var response = Http.Post(registrationURL, new NameValueCollection() {
                        { "user_id", CustomerCount.ToString() },
                        { "user_name", textBox2.Text },
                        { "user_lastname", textBox6.Text },
                        { "user_birthdate", dateTimePicker1.Text },
                        { "user_email", textBox3.Text },
                        { "user_createtimestamp", timestamp},
                        { "second_mail", "true"},
                        { "marketing_mail", newsletter}
                    });
                    if (System.Text.Encoding.Default.GetString(response).Contains("existing_user_login"))
                    {
                        MessageBox.Show("Your Mail >" + textBox3.Text + "< is allready in use!");
                        return;
                    }
                    else if (System.Text.Encoding.Default.GetString(response).ToLower().Contains("error"))
                    {
                        MessageBox.Show("WebService Error " + textBox3.Text);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cant request WebService:" + ex.Message);
                }

                try
                {
                    File.WriteAllText(customerFolder + CustomerCount.ToString() + ".txt", User);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't write to CustomerFolder.");
                }
                finally
                {
                    try
                    {
                        CustomerCount++;
                        updateSetting("CustomerCount", CustomerCount.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Can't write to config.");
                    }
                    finally
                    {
                        dataGridView1.Visible = false;
                        label6.Text = (CustomerCount).ToString();
                        label10.Text = "Ihre Nummer Lautet: " + (CustomerCount - 1).ToString() + "\nBitte merken!";
                        label10.Visible = true;
                        buttonID.Visible = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte Akzeptieren sie die AGBs um fortzufahren.");
            }
        }

        private void updateSetting(string key, string value)
        {
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void addSetting(string key, string value)
        {
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void KillTabTip()
        {
            try
            {

            }
            catch (Exception ex) { }
        }

        public void ShowTouchKeyboard(bool isVisible, bool numericKeyboard)
        {
            // if (Process.GetProcessesByName("process_name").Length > 0)
            try
            {

            }
            catch (Exception ex) { }
        }



        private void buttonReset_Click(object sender, EventArgs e)
        {
            label6.Text = (CustomerCount).ToString();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            textBoxDiscl.Text = "";
            textBoxDiscl.Visible = false;
            buttonOK.Visible = false;
            dataGridView1.Visible = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxDiscl.Text = disclaimer;
            textBoxDiscl.Visible = true;
            buttonOK.Visible = true;
            dataGridView1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void buttonID_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBoxDiscl.Visible = false;
            buttonOK.Visible = false;
            dataGridView1.Visible = true;
            checkBoxAGB.Checked = false;
            label10.Visible = false;
            buttonID.Visible = false;
            dateTimePicker1.CustomFormat = "dd.MM.yyyy";
            dateTimePicker1.Value = DateTime.Now;
            myCheckBox1.Checked = false;
            myCheckBox3.Checked = true;
        }

        private void myCheckBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxAGB_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void myCheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
