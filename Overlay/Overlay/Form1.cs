using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overlay
{
    public partial class Form1 : Form
    {
        private Configuration config = null;
        private int x=0, y=0, w=0, h=0;
        private string imagePath ="";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                x = Convert.ToInt32(config.AppSettings.Settings["x"].Value);
                y = Convert.ToInt32(config.AppSettings.Settings["y"].Value);
                w = Convert.ToInt32(config.AppSettings.Settings["w"].Value);
                h = Convert.ToInt32(config.AppSettings.Settings["h"].Value);
                imagePath = config.AppSettings.Settings["imagePath"].Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant load config file: " + ex.Message);
                return;
            }
            try
            {
                this.Location = new Point(x, y);
                this.Size = new Size(w, h);
                this.FormBorderStyle = FormBorderStyle.None;
                this.TransparencyKey = Color.White;
                this.BackColor = Color.White;
                this.TopMost = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                if (File.Exists(imagePath))
                    pictureBox1.Image = Image.FromFile(imagePath);
                else
                    MessageBox.Show("Image file not exists!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
