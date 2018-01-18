using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace YourSquare1.Models
{
    public class Image
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int AdvertismentID { get; set; }
        public byte[] ImageFile { get; set; }
        public Advertisment Advertisment { get; set; }
    }
}
