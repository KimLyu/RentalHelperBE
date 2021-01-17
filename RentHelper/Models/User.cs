using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //導入Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        //[MaxLength(12)]
        public string Password { get; set; }
        public string dTK { get; set; }
        [Required]
        public string Name { get; set; }

        public string NickName { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Address { get; set; }


        //<表單關聯-子表>使用者持有多筆產品
        public ICollection<Product> Products { get; set; }
            = new List<Product>();

        public ICollection<WishItem> WishItems { get; set; }
            = new List<WishItem>();

    }
}
