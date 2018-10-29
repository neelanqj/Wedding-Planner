using System.ComponentModel.DataAnnotations;

namespace Wedding_Planner.Models
{
    public class UserWedding_Xrf
    {
        [Key]
        public int UserWedding_XrfId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } 
        public int WeddingId { get; set; }
        public Wedding Wedding { get; set; }
        public string State { get; set; }
    }
}