using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_LineNotify
{
    public partial class Form1 : Form
    {
        string imgPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //VaryQualityLevel();
            lineNotify_img(textBox1.Text.ToString());
        }

        //壓縮圖片品質，測試傳輸效率
        private void VaryQualityLevel()
        {
            // Get a bitmap. The using statement ensures objects  
            // are automatically disposed from memory after use.
            Bitmap img1 = new Bitmap(pictureBox1.Image);

            //if (System.IO.File.Exists(imgPath))
            //    System.IO.File.Delete(imgPath);

            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID  
            // for the Quality parameter category.  
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.  
            // An EncoderParameters object has an array of EncoderParameter  
            // objects. In this case, there is only one  
            // EncoderParameter object in the array.  
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, long.Parse(Path.GetFileNameWithoutExtension(imgPath)));
            myEncoderParameters.Param[0] = myEncoderParameter;
            img1.Save(imgPath, jpgEncoder, myEncoderParameters);
            img1.Dispose();
            
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void lineNotify_img(string msg)
        {
            try
            {
                var file = imgPath; //圖檔大小必須小於3MB
                var upfilebytes = File.ReadAllBytes(file);
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient Client = new HttpClient(handler);
                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "gTW2tkn50pz3sCrylA4lpmK5cyCTHpPlrg6ot9PZvbF"); //Token
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent baContent = new ByteArrayContent(upfilebytes);
                content.Add(baContent, "imageFile", Path.GetFileName(imgPath));
                string url = @"https://notify-api.line.me/api/notify?message=";         //組訊息
                url = url + msg;
                var response = Client.PostAsync(url, content).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PNG(*.PNG); JPG(*.JPG; *.JPEG); gif文件(*.GIF) | *.png; *.jpg; *.jpeg; *.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imgPath = openFileDialog1.FileName.ToString();
                FileInfo fs = new FileInfo(imgPath);
                long size = fs.Length;
                if (size > 3145728)
                {
                    MessageBox.Show("檔案必須小於3MB");
                    imgPath = "";
                    pictureBox1.Image = null;
                    return;
                }
                FileStream fss = new FileStream(imgPath, FileMode.Open);
                Image img1 = Image.FromStream(fss);
                fss.Close();
                textBox1.Text = imgPath;
                pictureBox1.Image = img1;
            }
        }
    }
}
