using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
            Failed
        };

        const string    pathCustomer = @"\\192.168.10.200\Tausch\Intern\temp\Customer",
                        pathReadyCustomer = @"\\192.168.10.200\Tausch\Intern\temp\CustomerReady",
                        pathVideo = @"mp4",
                        pathImage = "test.png";
        const string    imageText = "#";
        Point           imageTextPos = new Point(10, 10);
        const int       imageTextSize = 40;
        const string    userName = "projekthyundai";
        const string    password = "I!4fis=imC";
        const string    ftpBaseUri = "ftp://hyundai.obsession.de/uploads/";
        const string    httpBaseUri = "http://hyundai.obsession.de/video/add";

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
            customer newCustom = new customer(0, "", "", "", "", 0);
            if (WaitForFile(pathCustomer + @"\" + e.Name.ToString(), FileMode.Open, FileAccess.Read))
            {
                string[] lines = System.IO.File.ReadAllLines(pathCustomer + @"\" + e.Name.ToString());
                newCustom.ID = Convert.ToInt32(lines[0]);
                newCustom.Name = lines[1];
                newCustom.LastName = lines[2];
                newCustom.Song = lines[3];
                newCustom.Video = lines[5];
                newCustom.Status = (byte)customerStatus.None;
            }
            else
            {
                newCustom.ID = -1;
                newCustom.Name = e.Name;
                newCustom.Status = (byte)customerStatus.Failed;
            }
            registrationList.Add(newCustom);
            addCustomer();
        }

        private void addCustomer()
        {
            if (listBoxAnmeldung.InvokeRequired)
            {
                listBoxAnmeldung.Invoke(new MethodInvoker(addCustomer));
                return;
            }

            int count = registrationList.Count - 1;
            if (registrationList[count].Status != (byte)customerStatus.Failed)
                listBoxAnmeldung.Items.Add(registrationList[count].ID + "|" + registrationList[count].Name + "|" + registrationList[count].LastName + "|" + registrationList[count].Song + "|" + registrationList[count].Status);
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
                    if (WaitForFile(pathCustomer + @"\" + cust.ID + ".txt", FileMode.Open, FileAccess.Read))
                        File.Copy(pathCustomer + @"\" + cust.ID + ".txt", pathReadyCustomer + @"\" + cust.ID + ".txt");
                    if (WaitForFile(cust.Video, FileMode.Open, FileAccess.Read))
                        File.Copy(cust.Video, pathReadyCustomer + @"\" + cust.ID + ".mp4");
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
                    listBoxReady.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.LastName + "|" + cust.Song + "|" + cust.Status);
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
            FSW_Initialisieren();
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
                    foreach (customer cust in registrationList)
                    {
                        if (cust.ID == ID)
                        {
                            cust.Status = (byte)customerStatus.Failed;
                        }
                    }
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
                    listBoxSuccess.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.LastName + "|" + cust.Song + "|" + cust.Status);
                    var response = Http.Post(httpBaseUri, new NameValueCollection() {
                        { "user_id", cust.ID.ToString() },
                        { "videouploaded", "true"},
                    });
                    string res = System.Text.Encoding.UTF8.GetString(response);
                    cust.Status = (byte)customerStatus.Success;
                    for (int i = 0; i < listBoxUpload.Items.Count; i++)
                    {
                        if (Convert.ToInt32(listBoxUpload.Items[i].ToString().Split('|')[0]) == cust.ID)
                            listBoxUpload.Items.RemoveAt(i);
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
                    listBoxFailed.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.LastName + "|" + cust.Song + "|" + cust.Status);
                    cust.Status = (byte)customerStatus.Failed;
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
                        writeToPNG(imageText + selectedCustomer, pathImage, imageTextPos, imageTextSize);
                    }
                    else if (cust.Status == (byte)customerStatus.Selected)
                    {
                        cust.Status = (byte)customerStatus.None;
                    }
                }
            }
        }


        private void writeToPNG(String text, String file, Point pos, int size)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
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

            b.Save(file, image.RawFormat);

            image.Dispose();
            b.Dispose();
        }


        private void buttonUpload_Click(object sender, EventArgs e)
        {
            foreach (customer cust in registrationList)
            {
                if (cust.Status == (byte)customerStatus.Wait)
                {
                    FtpAsync(userName, password, ftpBaseUri, pathReadyCustomer + @"\" + cust.ID + ".mp4", cust.ID);
                    cust.Status = (byte)customerStatus.Upload;
                    listBoxUpload.Items.Add(cust.ID + "|" + cust.Name + "|" + cust.LastName + "|" + cust.Song + "|" + cust.Status);
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
        private string song;
        private string video;
        private byte status;

        public customer(int ID, string Name,string LastName, string Song, string Video, byte Status)
        {
            this.ID = ID;
            this.Name = Name;
            this.LastName = LastName;
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

        public string LastName
        {
            get { return lastname; }
            set { lastname = value; }
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
