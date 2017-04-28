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

        private Configuration config = null;

        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.CustomFormat = "dd.MM.yyyy";
            dateTimePicker1.Value = DateTime.Now;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.DataSource = MusicDataTable;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                dataGridView1.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Cant open Song-File.\nPlease set it in the config.");
                return;
            }


            // this.TopMost = true;
            this.Location = new Point(0, 0);
            this.Width = Screen.PrimaryScreen.Bounds.Width + 16;
            this.Height = Screen.PrimaryScreen.Bounds.Height - 270;
            this.FormBorderStyle = FormBorderStyle.None;
            label6.Text = CustomerCount.ToString();
            Cursor.Current = Cursors.Default;
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

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(disclaimer, "Disclaimer", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                String User = "";

                if (String.IsNullOrEmpty(textBox2.Text.Trim()) || String.IsNullOrEmpty(textBox3.Text.Trim()) || String.IsNullOrEmpty(textBox4.Text.Trim()))
                {
                    MessageBox.Show("Bitte alle Felder ausfüllen.");
                    return;
                }
                if (!textBox3.Text.Contains('@'))
                {
                    MessageBox.Show("Bitte valide Mail verwenden: mymail@mydomain.com");
                    return;
                }

                String timestamp = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

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
                        { "user_createtimestamp", timestamp}
                    });
                    if (System.Text.Encoding.Default.GetString(response).Contains("existing_user_login"))
                    {
                        MessageBox.Show("Your Mail >" + textBox3.Text + "< is allready in use!");
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
                        MessageBox.Show("Ihre Nummer Lautet: " + (CustomerCount - 1).ToString() + "\nBitte merken!");

                        label6.Text = (CustomerCount).ToString();
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";
                    }
                }
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
    }
}
