using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoAddOverlay
{
    public partial class Form1 : Form
    {
        FileSystemWatcher FSW;
        String inputPath = "input\\", outputPath = "output\\", overlayPath = "overlay.png";
        String latestFile = "", statusFile = "";
        Color statusColor = Color.White;
        private Configuration config = null;

        private void FSW_Initialisieren()
        {
            // Filesystemwatcher anlegen
            FSW = new FileSystemWatcher();

            // Pfad und Filter festlegen
            FSW.Path = inputPath;
            FSW.Filter = "*.mp4";

            FSW.Created += new FileSystemEventHandler(FSW_Created);


            // Filesystemwatcher aktivieren
            FSW.EnableRaisingEvents = true;
        }

        void FSW_Created(object sender, FileSystemEventArgs e)
        {
            latestFile = e.FullPath;
            statusFile = "New file found!";
            statusColor = Color.Green;
            if (e.FullPath.Contains(" "))
            {
                MessageBox.Show("There are Spaces in the file or Path. Cant add Overlays!!!\n" + e.FullPath);
                statusFile = "There are Spaces in the file or Path. Cant add Overlays!!!\n" + e.FullPath;
                statusColor = Color.Red;
                return;
            }
            if (WaitForFile(e.FullPath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    File.Copy(e.FullPath, outputPath + e.Name, true);
                }
                catch (Exception ex)
                {
                    statusFile = "Cant copy :" + ex.Message;
                    statusColor = Color.Red;
                    return;
                }

                if (WaitForFile(outputPath + e.Name, FileMode.Open, FileAccess.Read))
                {
                    statusFile = "File Copyed!";
                    statusColor = Color.Green;
                    try
                    {
                        // Prepare the process to run
                        ProcessStartInfo start = new ProcessStartInfo();
                        // Enter in the command line arguments, everything you would enter after the executable name itself
                        start.Arguments = "-i .\\" + outputPath + e.Name + " -i " + overlayPath + " -filter_complex \"overlay = (main_w - overlay_w):(main_h - overlay_h)\" \".\\" + outputPath + e.Name + "\" -y";
                        // Enter the executable to run, including the complete path
                        start.FileName = "ffmpeg.exe";
                        // Do you want to show a console window?
                        start.WindowStyle = ProcessWindowStyle.Hidden;
                        start.CreateNoWindow = true;
                        int exitCode;


                        // Run the external process & wait for it to finish
                        using (Process proc = Process.Start(start))
                        {
                            proc.WaitForExit();

                            // Retrieve the app's exit code
                            exitCode = proc.ExitCode;
                        }
                        statusFile = "File Overlay added!";
                        statusColor = Color.Green;
                    }
                    catch (Exception ex)
                    {
                        statusFile = "Cant add Overlay :" + ex.Message;
                        statusColor = Color.Red;
                        return;
                    }
                }
                else
                {
                    statusFile = "Cant add Overlay :" + e.FullPath;
                    statusColor = Color.Red;
                    return;
                }
            }
            else
            {
                statusFile = "Cant copy :" + e.FullPath;
                statusColor = Color.Red;
                return;
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
                    System.Threading.Thread.Sleep(100);
                    return true;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }

            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = latestFile;
            textBox2.Text = statusFile;
            textBox1.BackColor = statusColor;
            textBox2.BackColor = statusColor;
        }


        /*
        FOR %%a in (*.mp4) do ffmpeg -i %%a -i overlay.png -filter_complex "overlay=(main_w-overlay_w):(main_h-overlay_h)" "done\_%%a"
        move *.mp4 "raw\" 
        timeout /t 10 



        @echo off & setlocal 
        set source=C:\Test\video\Test\Videos
        set target=C:\Test\video
        for /f "tokens=* delims=" %%a IN ('dir /B /S /A-D /O-D "%source%\*.mp4"') DO @( 
        echo Kopiere "%%a" nach "%target%" ... 
        copy "%%a" "%target%" & goto :end 
        )  || outputPath.Contains(" ") || overlayPath.Contains(" "))
        :end
        */
        public Form1()
        {
            InitializeComponent();
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                inputPath = config.AppSettings.Settings["inputPath"].Value;
                outputPath = config.AppSettings.Settings["outputPath"].Value;
                overlayPath = config.AppSettings.Settings["overlayPath"].Value;
                if (!Directory.Exists(inputPath))
                {
                    MessageBox.Show("Cant find inputPath:" + inputPath);
                    return;
                }
                if (!Directory.Exists(outputPath))
                {
                    MessageBox.Show("Cant find outputPath:" + outputPath);
                    return;
                }
                if (!File.Exists(overlayPath))
                {
                    MessageBox.Show("Cant find overlayFile:" + overlayPath);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant load config file: " + ex.Message);
                return;
            }
            finally
            {
                try
                {
                    FSW_Initialisieren();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cant init FSW " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (inputPath.Contains(" "))
            {
                MessageBox.Show("There are Spaces in the file or Path. Cant add Overlays!!!\n" + inputPath);
            }
            if (outputPath.Contains(" "))
            {
                MessageBox.Show("There are Spaces in the file or Path. Cant add Overlays!!!\n" + outputPath);
            }
            if (overlayPath.Contains(" "))
            {
                MessageBox.Show("There are Spaces in the file or Path. Cant add Overlays!!!\n" + overlayPath);
            }
        }
    }
}
