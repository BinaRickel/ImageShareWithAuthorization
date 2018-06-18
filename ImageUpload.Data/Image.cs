using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUpload.Data
{
    public class Image
    {
        public int Id { get; set; }
        public String FileName { get; set; }
        public String Password { get; set; }
        public int ViewCount { get; set; }
        public int UserId { get; set; }
    }
}
