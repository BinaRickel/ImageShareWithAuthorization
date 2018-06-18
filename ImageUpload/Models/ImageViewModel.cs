using ImageUpload.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageUpload.Models
{
    public class ImageViewModel
    {
        public Image Image { get; set; }
        public string Message { get; set; }
        public bool HasPermission { get; set; }
    }
}