using ImageUpload.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageUpload.Models
{
    public class IndexViewModel
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
    }
}