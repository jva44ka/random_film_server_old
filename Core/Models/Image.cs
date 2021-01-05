using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using BCLImage = System.Drawing.Image;

namespace Core.Models
{
    [Table("Images")]
    public class Image : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [Required]
        public string Data { get; set; }
        public string DataCompressed { get; set; }

        public string GetResizedData(int needHeight = 200)
        {
            var splitedData = this.Data.Split(',');
            using (var ms = new MemoryStream(Convert.FromBase64String(splitedData[1])))
            {
                var image = BCLImage.FromStream(ms);

                double factor;
                int newWidth;
                int newHeigth;

                if (image.Height <= needHeight)
                    return Data;

                else
                {
                    factor = image.Height / needHeight;
                    newWidth = (int)Math.Round(image.Width / factor);
                    newHeigth = (int)Math.Round(image.Height / factor);
                }

                var newImage = new Bitmap(image, newWidth, newHeigth);

                return splitedData[0] + "," + Image.ImageToString(newImage);
            }
        }

        private static string ImageToString(BCLImage img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
