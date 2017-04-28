using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoUploader
{
    public partial class Form1 : Form
    {
        public enum customerStatus : byte
        {
            None,
            Selected,
            Ready,
            Wait,
            Upload,
            Success,
            Failed,
            Final
        };

        private string pathCustomer = "",
                            pathReadyCustomer = "",
                            pathVideo = "",
                            pathImageIn = "",
                            pathImageOut = "";
        private string imageText = "";
        private Point imageTextPos = new Point(0, 0);
        private int imageTextSize = 0;
        private string ftpUserName = "";
        private string ftpPassword = "";
        private string ftpBaseUri = "";
        private string httpBaseUri = "";
        private Configuration config = null;


        FileSystemWatcher txtFSW, mp4FSW;

        List<customer> registrationList = new List<customer>();

        private void FSW_Initialisieren()
        {
            // Filesystemwatcher anlegen
            txtFSW = new FileSystemWatcher();
            mp4FSW = new FileSystemWatcher();
            // Pfad und Filter festlegen
            txtFSW.Path = pathCustomer;
            txtFSW.Filter = "*.txt";
            mp4FSW.Path = pathVideo;
            mp4FSW.Filter = "*.mp4";
            // Events definieren
            txtFSW.Created += new FileSystemEventHandler(txt_Created);
            mp4FSW.Created += new FileSystemEventHandler(mp4_Created);
            // Filesystemwatcher aktivieren
            txtFSW.EnableRaisingEvents = true;
            mp4FSW.EnableRaisingEvents = true;
        }

        void txt_Created(object sender, FileSystemEventArgs e)
        {
            addCustomer(e.Name);
        }

        void addCustomer(string file)
        {
            customer newCustom = new customer(0, "", "", "", "", 0);
            if (WaitForFile(pathCustomer + @"\" + file, FileMode.Open, FileAccess.Read))
            {
                string[] lines = System.IO.File.ReadAllLines(pathCustomer + @"\" + file);
                newCustom.ID = Convert.ToInt32(lines[0]);
                newCustom.Name = lines[1] + " - " + lines[2];
                newCustom.Mail = lines[4];
                newCustom.Song = lines[5];
                newCustom.Video = "";
                newCustom.Status = (byte)customerStatus.None;
            }
            else
            {
                newCustom.ID = -1;
                newCustom.Name = file;
                newCustom.Status = (byte)customerStatus.Failed;
            }
            registrationList.Add(newCustom);
            addCustomerToListbox();
        }

        void addFailedCustomer(string file)
        {
            customer newCustom = new customer(0, "", "", "", "", 0);
            if (WaitForFile(pathReadyCustomer + @"\" + file, FileMode.Open, FileAccess.Read))
            {
                string[] lines = System.IO.File.ReadAllLines(pathReadyCustomer + @"\" + file);
                if (lines[8].Contains("Failed"))
                {
                    newCustom.ID = Convert.ToInt32(lines[0]);
                    newCustom.Name = lines[1] + " - " + lines[2];
                    newCustom.Mail = lines[4];
                    newCustom.Song = lines[5];
                    if (File.Exists(pathReadyCustomer + @"\" + newCustom.ID + ".mp4"))
                        newCustom.Video = newCustom.ID + ".mp4";
                    else
                        newCustom.Video = lines[7];
                    if (lines[8].Contains("Failed"))
                        newCustom.Status = (byte)customerStatus.Failed;
                    listBoxFailed.Items.Add(newCustom.ID + "|" + newCustom.Name + "|" + newCustom.Mail + "|" + newCustom.Song + "|" + newCustom.Status);
                    registrationList.Add(newCustom);
                }
            }
            else
            {
                MessageBox.Show("Cant read existing File: " + file);
            }
        }

        private void addCustomerToListbox()
        {
            if (listBoxAnmeldung.InvokeRequired)
            {
                listBoxAnmeldung.Invoke(new MethodInvoker(addCustomerToListbox));
                return;
            }

            int count = registrationList.Count - 1;
            if (registrationList[count].Status != (byte)customerStatus.Failed)
                listBoxAnmeldung.Items.Add(registrationList[count].ID + "|" + registrationList[count].Name + "|" + registrationList[count].Mail + "|" + registrationList[count].Song + "|" + registrationList[count].Status);
            else
                listBoxAnmeldung.Items.Add(registrationList[count].Name + "|" + registrationList[count].Status);

        }

        void mp4_Created(object sender, FileSystemEventArgs e)
        {
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Selected)
                {
                    cust.Video = pathVideo + @"\" + e.Name;
                    cust.Status = (byte)customerStatus.Ready;
                    if (WaitForFile(pathCustomer + @"\" + cust.ID + ".txt", FileMode.Open, FileAccess.Read))
                        File.AppendAllText(pathCustomer + @"\" + cust.ID + ".txt", cust.Video + "\n");
                    else
                        MessageBox.Show("Cant access " + pathCustomer + @"\" + cust.ID + ".txt");
                    if (WaitForFile(pathCustomer + @"\" + cust.ID + ".txt", FileMode.Open, FileAccess.Read))
                        File.Move(pathCustomer + @"\" + cust.ID + ".txt", pathReadyCustomer + @"\" + cust.ID + ".txt");
                    else
                        MessageBox.Show("Cant copy :" + pathCustomer + @"\" + cust.ID + ".txt");
                    if (WaitForFile(cust.Video, FileMode.Open, FileAccess.Read))
                        File.Move(cust.Video, pathReadyCustomer + @"\" + cust.ID + ".mp4");
                    else
                        MessageBox.Show("Cant copy :" + pathCustomer + @"\" + cust.ID + ".txt");
                    setReady();
                }
            }
        }

        private void setReady()
        {
            if (listBoxReady.InvokeRequired)
            {
                listBoxReady.Invoke(new MethodInvoker(setReady));
                return;
            }
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Ready)
                {
                    listBoxReady.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.Mail + "|" + cust.Song + "|" + cust.Status);
                    cust.Status = (byte)customerStatus.Wait;
                    for (int i = 0; i < listBoxAnmeldung.Items.Count; i++)
                    {
                        if (Convert.ToInt32(listBoxAnmeldung.Items[i].ToString().Split('|')[0]) == cust.ID)
                            listBoxAnmeldung.Items.RemoveAt(i);
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                pathCustomer = config.AppSettings.Settings["pathCustomer"].Value;
                pathReadyCustomer = config.AppSettings.Settings["pathReadyCustomer"].Value;
                pathVideo = config.AppSettings.Settings["pathVideo"].Value;
                pathImageIn = config.AppSettings.Settings["pathImageIn"].Value;
                pathImageOut = config.AppSettings.Settings["pathImageOut"].Value;
                imageText = config.AppSettings.Settings["imageText"].Value;
                imageTextPos = new Point(Convert.ToInt32(config.AppSettings.Settings["imageTextPosX"].Value), Convert.ToInt32(config.AppSettings.Settings["imageTextPosY"].Value));
                imageTextSize = Convert.ToInt32(config.AppSettings.Settings["imageTextSize"].Value);
                ftpUserName = config.AppSettings.Settings["ftpUserName"].Value;
                ftpPassword = config.AppSettings.Settings["ftpPassword"].Value;
                ftpBaseUri = config.AppSettings.Settings["ftpBaseUri"].Value;
                httpBaseUri = config.AppSettings.Settings["httpBaseUri"].Value;


                if (!Directory.Exists(pathCustomer))
                {
                    MessageBox.Show("Cant find pathCustomer:" + pathCustomer);
                    return;
                }
                if (!Directory.Exists(pathReadyCustomer))
                {
                    MessageBox.Show("Cant find pathReadyCustomer:" + pathReadyCustomer);
                    return;
                }
                if (!Directory.Exists(pathVideo))
                {
                    MessageBox.Show("Cant find customerFolder:" + pathVideo);
                    return;
                }
                if (!File.Exists(pathImageIn))
                {
                    MessageBox.Show("Cant find pathImageIn:" + pathImageIn);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant load config file: " + ex.Message);
                return;
            }

            try
            {
                DirectoryInfo d = new DirectoryInfo(pathCustomer);
                foreach (var file in d.GetFiles("*.txt"))
                {
                    addCustomer(file.Name);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant add existing Files: " + ex.Message);
            }


            try
            {
                DirectoryInfo d = new DirectoryInfo(pathReadyCustomer);
                foreach (var file in d.GetFiles("*.txt"))
                {
                    try
                    {
                        addFailedCustomer(file.Name);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cant read existing File: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant add existing Files: " + ex.Message);
            }


            try
            {
                FSW_Initialisieren();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant init FSW_Initialisieren: " + ex.Message);
                return;
            }
        }

        private async void FtpAsync(string userName, string password, string ftpBaseUri, string fileName, int ID)
        {
            var tasks = new List<Task<byte[]>>();
            FileInfo fileInfo = new FileInfo(fileName);
            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(userName, password);
                tasks.Add(webClient.UploadFileTaskAsync(ftpBaseUri + fileInfo.Name, fileInfo.FullName));
            }

            foreach (var task in tasks)
            {
                try
                {
                    await task;
                    foreach (customer cust in registrationList)
                    {
                        if (cust.ID == ID)
                        {
                            cust.Status = (byte)customerStatus.Success;
                        }
                    }
                    setSuccess();
                }
                catch (Exception ex)
                {
                    int customerID = 0;
                    foreach (customer cust in registrationList)
                    {
                        if (cust.ID == ID)
                        {
                            cust.Status = (byte)customerStatus.Failed;
                            customerID = cust.ID;
                        }
                    }
                    MessageBox.Show("Upload failed for:" + customerID.ToString() + " : " + ex.Message);
                    setFailed();
                }
            }

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

        private void setSuccess()
        {
            if (listBoxSuccess.InvokeRequired)
            {
                listBoxSuccess.Invoke(new MethodInvoker(setSuccess));
                return;
            }
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Success)
                {
                    listBoxSuccess.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.Mail + "|" + cust.Song + "|" + cust.Status);
                    var response = Http.Post(httpBaseUri, new NameValueCollection() {
                        { "user_id", cust.ID.ToString() },
                        { "videouploaded", "true"},
                    });
                    string res = System.Text.Encoding.UTF8.GetString(response);
                    cust.Status = (byte)customerStatus.Final;

                    if (WaitForFile(pathReadyCustomer + @"\" + cust.ID + ".txt", FileMode.Open, FileAccess.Read))
                    {
                        string lines = System.IO.File.ReadAllText(pathReadyCustomer + @"\" + cust.ID + ".txt");
                        if (lines.Contains("Failed"))
                            File.WriteAllText(pathReadyCustomer + @"\" + cust.ID + ".txt", lines.Replace("Failed", "Success"));
                        else
                            File.AppendAllText(pathReadyCustomer + @"\" + cust.ID + ".txt", "Success\n");
                    }
                    else
                        MessageBox.Show("Cant access " + pathReadyCustomer + @"\" + cust.ID + ".txt");

                    for (int i = 0; i < listBoxUpload.Items.Count; i++)
                    {
                        if (Convert.ToInt32(listBoxUpload.Items[i].ToString().Split('|')[0]) == cust.ID)
                            listBoxUpload.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Failed)
                {
                    cust.Status = (byte)customerStatus.Wait;
                    listBoxReady.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.Mail + "|" + cust.Song + "|" + cust.Status);
                    for (int i = 0; i < listBoxReady.Items.Count; i++)
                    {
                        if (Convert.ToInt32(listBoxFailed.Items[i].ToString().Split('|')[0]) == cust.ID)
                            listBoxFailed.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void setFailed()
        {
            if (listBoxFailed.InvokeRequired)
            {
                listBoxFailed.Invoke(new MethodInvoker(setFailed));
                return;
            }
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Failed)
                {
                    listBoxFailed.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.Mail + "|" + cust.Song + "|" + cust.Status);
                    cust.Status = (byte)customerStatus.Failed;
                    if (WaitForFile(pathCustomer + @"\" + cust.ID + ".txt", FileMode.Open, FileAccess.Read))
                        File.AppendAllText(pathCustomer + @"\" + cust.ID + ".txt", "Failed\n");
                    else
                        MessageBox.Show("Cant access " + pathCustomer + @"\" + cust.ID + ".txt");
                    for (int i = 0; i < listBoxUpload.Items.Count; i++)
                    {
                        if (Convert.ToInt32(listBoxUpload.Items[i].ToString().Split('|')[0]) == cust.ID)
                            listBoxUpload.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxAnmeldung.SelectedIndex;
            if (selectedIndex != -1)
            {
                int selectedCustomer = Convert.ToInt32(listBoxAnmeldung.Items[selectedIndex].ToString().Split('|')[0]);
                foreach (customer cust in registrationList)
                {
                    if (cust.ID == selectedCustomer)
                    {
                        cust.Status = (byte)customerStatus.Selected;
                        textBoxCustomer.Text = cust.ID.ToString() + " | " + cust.Name;
                        try
                        {
                            writeToPNG(imageText + selectedCustomer, pathImageIn, pathImageOut, imageTextPos, imageTextSize);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Can't write to Output Image: " + ex.Message);
                        }
                    }
                    else if (cust.Status == (byte)customerStatus.Selected)
                    {
                        cust.Status = (byte)customerStatus.None;
                    }
                }
            }
        }


        private void writeToPNG(String text, String fileIn, String fileOut, Point pos, int size)
        {
            FileStream fs = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
            Image image = Image.FromStream(fs);
            fs.Close();

            Bitmap b = new Bitmap(image);
            Graphics graphics = Graphics.FromImage(b);
            FontFamily fontFamily = new FontFamily("Times New Roman");
            Font font = new Font(fontFamily,
                                   size,
                                   FontStyle.Regular,
                                   GraphicsUnit.Pixel);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(text, font, Brushes.Black, pos.X, pos.Y);

            b.Save(fileOut, image.RawFormat);

            image.Dispose();
            b.Dispose();
        }


        private void buttonUpload_Click(object sender, EventArgs e)
        {
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Wait)
                {
                    FtpAsync(ftpUserName, ftpPassword, ftpBaseUri, pathReadyCustomer + @"\" + cust.ID + ".mp4", cust.ID);
                    cust.Status = (byte)customerStatus.Upload;
                    listBoxUpload.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.Mail + "|" + cust.Song + "|" + cust.Status);
                    for (int i = 0; i < listBoxReady.Items.Count; i++)
                    {
                        if (Convert.ToInt32(listBoxReady.Items[i].ToString().Split('|')[0]) == cust.ID)
                            listBoxReady.Items.RemoveAt(i);
                    }
                }
            }
        }

        bool WaitForFile(string fullPath, FileMode mode, FileAccess access)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access);
                    fs.Dispose();
                    return true;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    System.Threading.Thread.Sleep(50);
                }
            }

            return false;
        }
    }





    public class customer
    {
        private int id;
        private string name;
        private string lastname;
        private string mail;
        private string song;
        private string video;
        private byte status;

        public customer(int ID, string Name, string Mail, string Song, string Video, byte Status)
        {
            this.ID = ID;
            this.Name = Name;
            this.Mail = Mail;
            this.Song = Song;
            this.Video = Video;
            this.Status = Status;
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Mail
        {
            get { return mail; }
            set { mail = value; }
        }

        public string Song
        {
            get { return song; }
            set { song = value; }
        }

        public string Video
        {
            get { return video; }
            set { video = value; }
        }

        public byte Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
