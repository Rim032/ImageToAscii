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
using System.Threading;

namespace ImageToAscii
{
    public partial class main_form : Form
    {
        public main_form()
        {
            InitializeComponent();
        }

        private static char[] ascii_image_chars = { '@', '&', 'm', 'Q', 't', ']', '?', '~', '"', '^', '\'', '.' };

        public string img_file = "";
        public static int img_width;
        public static int img_height;

        private void file_button_Click(object sender, EventArgs e)
        {
            ascii_conversion_status.Text = "Status: Waiting.";
            file_dialogue.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png";

            if (file_dialogue.ShowDialog() == DialogResult.OK && File.Exists(file_dialogue.FileName))
            {
                file_text_box.Text = file_dialogue.FileName;
                img_file = file_dialogue.FileName;
            }
        }

        private static int[,] pre_image_to_ascii(string image_file)
        {
            if (image_file == null)
            {
                return null;
            }

            try
            {
                Bitmap image_bitmap = new Bitmap(Path.GetFullPath(image_file));
                img_width = image_bitmap.Width;
                img_height = image_bitmap.Height;

                if (image_bitmap.Width > 4096 || image_bitmap.Height > 4096)
                {
                    return null;
                }

                int[,] ascii_image_pre_result = new int[image_bitmap.Width, image_bitmap.Height];

                for (int pix_y = 0; pix_y < image_bitmap.Height; pix_y++)
                {
                    for (int pix_x = 0; pix_x < image_bitmap.Width; pix_x++)
                    {
                        Color pix_color = image_bitmap.GetPixel(pix_x, pix_y);
                        int pix_grey_color_num = (pix_color.R + pix_color.G + pix_color.B) / 3;

                        //Console.WriteLine($"Norm: {pix_color.R + pix_color.G + pix_color.B}, Grey Scale: {pix_grey_color_num}");
                        ascii_image_pre_result[pix_x, pix_y] = pix_grey_color_num;
                    }
                }

                return ascii_image_pre_result;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private void ascii_convert_button_Click(object sender, EventArgs e)
        {
            ascii_conversion_status.Text = "Status: Started.";
            int[,] ascii_image_pre_result = pre_image_to_ascii(img_file);
            if (ascii_image_pre_result == null)
            {
                ascii_conversion_status.Text = "Status: Failed. (File too large, null or invalid)";
                return;
            }

            char[,] ascii_image_result = new char[img_width, img_height];

            try
            {
                using (StreamWriter ascii_image_sw = new StreamWriter(File.Create(img_file + ".txt")))
                {
                    for (int ascii_y = 0; ascii_y < img_height; ascii_y++)
                    {
                        for (int ascii_x = 0; ascii_x < img_width; ascii_x++)
                        {
                            if (ascii_x < img_width - 1)
                            {
                                ascii_image_result[ascii_x, ascii_y] = ascii_image_chars[(int)Math.Round(Math.Sqrt(ascii_image_pre_result[ascii_x, ascii_y]) /2)];
                                ascii_image_sw.Write(ascii_image_result[ascii_x, ascii_y]);
                                //Console.WriteLine((int)Math.Round(Math.Sqrt(ascii_image_pre_result[ascii_x, ascii_y]) / 2));
                            }
                            else
                            {
                                ascii_image_sw.Write(ascii_image_result[ascii_x, ascii_y]);
                                ascii_image_sw.Write("\n");
                            }
                        }
                    }
                }

                ascii_conversion_status.Text = "Status: Completed.";
            }
            catch (Exception error)
            {
                ascii_conversion_status.Text = $"Status: Failed. ({error.Message})";
            }
        }
    }
}