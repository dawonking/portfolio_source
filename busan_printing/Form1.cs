using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
namespace CertificationPrint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PrintFromImage();
        }

        protected void PrintFromImage()
        {
            using (var pd = new System.Drawing.Printing.PrintDocument())
            {
                //가로세로
                pd.DefaultPageSettings.Landscape = false;
                PaperSize pa = new PaperSize("", 1012, 636);
                pd.DefaultPageSettings.PaperSize.RawKind = (int)PaperKind.Custom;
                pd.PrinterSettings.DefaultPageSettings.PaperSize.RawKind = (int)PaperKind.Custom;
                pd.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("1", 100, 100);
                pd.PrintPage += (_, e) =>
                {

                    #region data_read
                    UserData data = null;
                    using (StreamReader sr = new StreamReader(@"./userData.txt"))
                    {
                        string v = sr.ReadToEnd();
                        data = JsonConvert.DeserializeObject<UserData>(v);
                        sr.Close();
                    }
                    string path = @".\config.txt";
                    if (!File.Exists(path))
                    {
                        using (StreamWriter sw = new StreamWriter(path))
                        {
                            var c = new Config();
                            var s = JsonConvert.SerializeObject(c, Formatting.Indented);
                            sw.Write(s);
                            sw.Close();
                        }
                    }
                    Config conf = null;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        var s = sr.ReadToEnd();
                        conf = JsonConvert.DeserializeObject<Config>(s);
                        sr.Close();
                    }

                    string point_save_path = @".\save_point.txt";

                    if (!File.Exists(point_save_path))
                    {
                        using (StreamWriter sw = new StreamWriter(point_save_path))
                        {
                            var c = new Config_card();
                            var s = JsonConvert.SerializeObject(c, Formatting.Indented);
                            sw.Write(s);
                            sw.Close();
                        }
                    }
                    Config_card config_Card = null;
                    using (StreamReader sr = new StreamReader(point_save_path))
                    {
                        var s = sr.ReadToEnd();
                        config_Card = JsonConvert.DeserializeObject<Config_card>(s);
                        sr.Close();
                    }


                    person_info card_data = null;

                    using (StreamReader sr = new StreamReader(@"./save_value.txt"))
                    {
                        string v = sr.ReadToEnd();
                        card_data = JsonConvert.DeserializeObject<person_info>(v);
                        sr.Close();
                    }

                    #endregion
                    

                    #region Font&image
                    Font _fontName = new Font("Noto Sans CJK KR Black", 9, FontStyle.Bold);

                    //----------------------

                    Font _fontcenter = new Font("Noto Sans CJK KR Black", 15, FontStyle.Bold);
                    Font _fontcenter2 = new Font("Noto Sans CJK KR Black", 13, FontStyle.Bold);
                    Font _fontgroup_name = new Font("Noto Sans CJK KR Black", 10, FontStyle.Bold);

                    //----------------------

                    Font _fontCap = new Font("Noto Sans Korean DemiLight", 14, FontStyle.Regular);

                    //Font _fontCenter = new Font("Noto Sans Korean DemiLight", 14, FontStyle.Regular);

                    Font _fontCenter = new Font("Noto Sans CJK KR Black ", 9, FontStyle.Regular);
                    Font _fontNormal = new Font("Noto Sans Korean DemiLight", 8, FontStyle.Regular);

                    SolidBrush _brush = new SolidBrush(Color.Black);
                    SolidBrush _brusu_gray = new SolidBrush(Color.Black);
                    
                    //Image img = Image.FromFile(@"./card_dummy.png");
                    //Image face_cap = Image.FromFile(@"./cap.jpg");
                    
                    //Bitmap bm = ResizeImage(img, 1755, 1241,false);
                    //Bitmap face = ResizeImage(face_cap, face_cap.Width, face_cap.Height,true);
                    
                    #endregion

                    #region draw_setting                    
                    e.PageSettings.PaperSize.RawKind = (int)PaperKind.Custom;
                    e.PageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                    //e.PageSettings.PaperSize.Width = (int)1012;
                    //e.PageSettings.PaperSize.Height = (int)636;                    

                    //e.PageSettings.PrinterResolution.X = pd.PrinterSettings.PrinterResolutions[0].X;
                    //e.PageSettings.PrinterResolution.Y = pd.PrinterSettings.PrinterResolutions[0].Y;

                    int page_w = (int)e.PageSettings.PrintableArea.Width;
                    int page_h = (int)e.PageSettings.PrintableArea.Height;

                    save_log sa = new save_log();

                    sa.print_height = page_h;
                    sa.print_witdh = page_w;

                    string save_path = @".\debug_log.txt";
                    using (StreamWriter sw = new StreamWriter(save_path))
                    {
                        var s = JsonConvert.SerializeObject(sa, Formatting.Indented);
                        sw.Write(s);
                        sw.Close();
                    }


                    //e.Graphics.DrawImage(face, new Rectangle(config_Card.image_x, config_Card.image_y, config_Card.image_witdh_x, config_Card.image_witdh_y));

                    //충주과학관 33 -49 - 19
                    e.Graphics.DrawString("충주어린이과학관", _fontcenter, _brush, new Point(33, 35), StringFormat.GenericTypographic);
                    e.Graphics.DrawString(card_data.center_value, _fontCenter, _brush, new Point(config_Card.group_text_x, config_Card.group_text_y), StringFormat.GenericTypographic);
                    e.Graphics.DrawString("예약자 이름", _fontgroup_name, _brush, new Point(33, 135), StringFormat.GenericTypographic);
                    //e.Graphics.DrawString("과학관 홈페이지 주소 [https://www.naver.com]", _fontgroup_name, _brush, new Point(33, 185), StringFormat.GenericTypographic);
                    e.Graphics.DrawString(card_data.date_value, _fontgroup_name, _brush, new Point(config_Card.time_text_x, config_Card.time_text_y));


                    //------------------------
                    e.Graphics.DrawString(card_data.name_value, _fontgroup_name, _brush, new Point(config_Card.name_text_x, config_Card.name_text_y), StringFormat.GenericTypographic);
                    //e.Graphics.DrawString(card_data.center_value, _fontCenter, _brush, new Point(config_Card.group_text_x, config_Card.group_text_y), StringFormat.GenericTypographic);
                    //e.Graphics.DrawString(string.Format("{0}년 {1}월 {2}일", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Date.Day)
                    //    , _fontNormal, _brush, new Point(config_Card.time_text_x, config_Card.time_text_y));
                    

                    #endregion

                };
                

                pd.Print();
            }
            Close();
        }

        private int MM2Inch(int mm)
        {
            return (int)(mm * 100.0f / 25.4f);
        }

        public static Bitmap ResizeImage(Image image, int width, int height, bool _rotate)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(300, 300);

            if (_rotate)
            {
                image.RotateFlip(RotateFlipType.Rotate270FlipY);
            }

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;


                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            if (_rotate)
            {                
                destImage.Save(@"./destImage_rot.jpg", ImageFormat.Jpeg);
            }
            else
            {
                destImage.Save(@"./destImage.jpg", ImageFormat.Jpeg);
            }
            
           
            return destImage;
        }
        public Dictionary<string, string> dictKioskName = new Dictionary<string, string>()
        {
            { "attendance1","재난안전" },
            { "attendance2","화재안전" },
            { "attendance3","학생안전" },
            { "attendance4","생활안전" },
            { "attendance5","\n교통안전" },
            { "attendance6","\n선박항공안전" }
        };

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class Config
    {
        public int groupY = 0;
        public int capY = 0;
        public int upperDateY = 0;
        public int lowerDateY = 0;
    }

    public class person_info
    {
        public string name_value = "";        
        public string center_value = "";
        public string date_value = "";
    }

    public class Config_card
    {
        public int image_x = 0;
        public int image_y = 0;
        public int image_witdh_x = 0;
        public int image_witdh_y = 0;

        public int name_text_x = 0;
        public int name_text_y = 0;

        public int group_text_x = 0;
        public int group_text_y = 0;

        public int time_text_x = 0;
        public int time_text_y = 0;

    }

    public class save_log
    {
        public int print_witdh = 0;
        public int print_height = 0;
    }

}
