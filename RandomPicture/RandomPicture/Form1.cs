using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomPicture
{
    public partial class Form1 : Form
    {
        private Configuration config = null;
        private string inputFolder = "";
        private string inputFileExt = "";
        private string overlayFileWin = "";
        private string overlayFileLose = "";
        private string outputFolder = "";
        private int startRandom = 0;
        private int endRandom = 0;
        private int luckyNumber = 0;
        private int maxWinner = 0, winnerCounter=0;

        private string textStatus = "";
        private string textOutput = "";
        private string textInput = "";
        private string textLastNumber = "";
        
        FileSystemWatcher imageFSW;

        public Form1()
        {
            InitializeComponent();
        }

        private void FSW_Initialisieren()
        {
            // Filesystemwatcher anlegen
            imageFSW = new FileSystemWatcher();
            // Pfad und Filter festlegen
            imageFSW.Path = inputFolder;
            imageFSW.Filter = inputFileExt;

            // Events definieren
            imageFSW.Created += new FileSystemEventHandler(image_Created);
            // Filesystemwatcher aktivieren
            imageFSW.EnableRaisingEvents = true;
        }

        void image_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                Random r = new Random();
                String imageOutput = outputFolder + e.Name;
                textOutput = imageOutput;
                textInput = e.FullPath;
                textStatus = "Copy input to output";
                if (WaitForFile(e.FullPath, FileMode.Open, FileAccess.Read))
                {
                    File.Copy(e.FullPath, imageOutput, true);
                }
                else
                {
                    textStatus = "Cant Copy :" + e.FullPath;
                    MessageBox.Show("Cant Copy :" + e.FullPath);
                }
                textStatus = "Write overlay to output";
                if (WaitForFile(imageOutput, FileMode.Open, FileAccess.Read))
                {
                    FileStream fsIn = new FileStream(imageOutput, FileMode.Open, FileAccess.Read);
                    Image imageIn = Image.FromStream(fsIn);
                    

                    FileStream fsOverlay = null;
                    int randomNum = r.Next(startRandom, endRandom+1);
                    textLastNumber = randomNum.ToString();
                    if (randomNum == luckyNumber && winnerCounter< maxWinner)
                    {
                        fsOverlay = new FileStream(overlayFileWin, FileMode.Open, FileAccess.Read);
                        winnerCounter++;
                    }
                    else
                    {
                        fsOverlay = new FileStream(overlayFileLose, FileMode.Open, FileAccess.Read);
                    }
                    Image imageOverlay = Image.FromStream(fsOverlay);


                    Bitmap OutputImage = MergeTwoImages(imageIn,imageOverlay);
                    fsIn.Close();
                    fsOverlay.Close();
                    imageIn.Dispose();
                    imageOverlay.Dispose();

                    if (WaitForFile(imageOutput, FileMode.Open, FileAccess.Read))
                    {
                        OutputImage.Save(imageOutput, ImageFormat.Jpeg);
                        OutputImage.Dispose();
                    }
                    else
                    {
                        textStatus = "Cant acces :" + imageOutput;
                        MessageBox.Show("Cant acces :" + imageOutput);
                    }
                }
                else
                {
                    textStatus = "Cant acces :" + imageOutput;
                    MessageBox.Show("Cant acces :" + imageOutput);
                }
                textStatus = "Finish";
            }
            catch (Exception ex)
            {
                textStatus = ex.Message;
                MessageBox.Show(ex.Message);
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

        public static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width;

            int outputImageHeight = firstImage.Height;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(0,0), firstImage.Size),new Rectangle(new Point(0,0), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(0, 0), secondImage.Size),new Rectangle(new Point(0,0), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                startRandom = Convert.ToInt32(config.AppSettings.Settings["startRandom"].Value);
                textBox1.Text = startRandom.ToString();
                endRandom = Convert.ToInt32(config.AppSettings.Settings["endRandom"].Value);
                textBox2.Text = endRandom.ToString();
                luckyNumber = Convert.ToInt32(config.AppSettings.Settings["luckyNumber"].Value);
                textBox3.Text = luckyNumber.ToString();
                inputFolder = config.AppSettings.Settings["inputFolder"].Value;
                textBoxInput.Text = inputFolder;
                overlayFileWin = config.AppSettings.Settings["overlayFileWin"].Value;
                textBox7.Text = overlayFileWin;
                overlayFileLose = config.AppSettings.Settings["overlayFileLose"].Value;
                textBox8.Text = overlayFileLose;
                inputFileExt = config.AppSettings.Settings["inputFileExt"].Value;

                outputFolder = config.AppSettings.Settings["outputFolder"].Value;
                textBoxOutput.Text = outputFolder;


                maxWinner = Convert.ToInt32(config.AppSettings.Settings["maxWinner"].Value);

                if (!Directory.Exists(inputFolder))
                {
                    MessageBox.Show("Cant find inputFolder:" + inputFolder);
                    return;
                }
                if (!Directory.Exists(outputFolder))
                {
                    MessageBox.Show("Cant find outputFolder:" + outputFolder);
                    return;
                }
                if (!File.Exists(overlayFileWin))
                {
                    MessageBox.Show("Cant find overlayFileWin:" + overlayFileWin);
                    return;
                }
                if (!File.Exists(overlayFileLose))
                {
                    MessageBox.Show("Cant find overlayFileLose:" + overlayFileLose);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxOutput.Text = textOutput;
            textBoxInput.Text = textInput;
            textBoxStatus.Text = textStatus;
            textBoxLastNumber.Text = textLastNumber;
            textBoxWinner.Text = winnerCounter.ToString() + " / " + maxWinner.ToString();
        }
    }
}
