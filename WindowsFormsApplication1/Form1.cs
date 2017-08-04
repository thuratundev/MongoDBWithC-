using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {


        public string FilePath { get; set; }


        public Form1()
        {
            InitializeComponent();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        public void SaveData()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("mydb");
            var collection = db.GetCollection<Stock>("stock");

            Stock _stknew = new Stock { name = txt_Name.Text , image = ConvertImageFileToByte(this.FilePath) };
            collection.InsertOne(_stknew);
            this.FilePath = null;
            GetData();

        }

        public  void GetData()
        {

            var credential = MongoCredential.CreateCredential("mydb", "sa", "015427");

            var setting = new MongoClientSettings
            {
                Credentials = new[] { credential },
                Server = new MongoServerAddress("localhost", 27017)

            };

            //MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoClient client = new MongoClient(setting);
            
            var db = client.GetDatabase("mydb");
            
              
            var collection = db.GetCollection<Stock>("stock");
       

            var result =  collection.AsQueryable<Stock>().ToList();


            this.dataGridView1.DataSource = result;
            this.dataGridView1.Columns[0].Visible = false;

        }

        private void btn_Get_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void btn_SaveData_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void btn_ShowImage_Click(object sender, EventArgs e)
        {
           DialogResult result = openFileDialog1.ShowDialog();
            this.FilePath = openFileDialog1.FileName;
        }



        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            
        }
        public Byte[] ConvertImageFileToByte(string Path)
        {
            if (String.IsNullOrEmpty(Path))
            {
                return null;
            }
            else
            {
                return File.ReadAllBytes(Path);
            }

        }
        public Image byteArrayToImage(byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);
            return img;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.Rows[e.RowIndex].Cells["image"].Value is Byte[])
            {
                this.pictureBox1.Image = byteArrayToImage((Byte[])this.dataGridView1.Rows[e.RowIndex].Cells["image"].Value);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                this.pictureBox1.Image = null;
            }
        }
    }

    public class Stock
    {
        public ObjectId Id { get; set; }
        public string name { get; set; }

        public byte[] image { get; set; }
    }
}

