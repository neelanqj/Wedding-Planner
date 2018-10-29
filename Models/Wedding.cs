using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wedding_Planner.Extensions;

namespace Wedding_Planner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Required]
        public string Wedder1 { get; set; }
        [Required]
        public string Wedder2 { get; set; }
        [Required]
        [DataType(DataType.Date)]

        [CurrentDate(ErrorMessage = "Date must be after or equal to current date")]
        public DateTime Date { get; set; }
        [Required]
        public string Address { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public List<UserWedding_Xrf> WeddingGuests {get;set;}
    }
}