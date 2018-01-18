using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace YourSquare1.Models
{
    public class Advertisment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string UserID { get; set; }
        public DateTime DateOfPublication { get; set; }
        public bool Accepted { get; set; }
        public bool DecisionMade { get; set; }
        public string Description { get; set; }
        public string AdditionalEquipmentDescription { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public string Address { get; set; }
        public ApplicationUser AdvertismentCreator { get; set; }
        public ICollection<Image> AdvertismentImages { get; set; }
    }
}
