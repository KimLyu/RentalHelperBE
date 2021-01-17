using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentHelper.Models
{
    public class Picture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //引入上方Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }  

        public DateTime DateTime { get; set; }

        [MaxLength(1500)]
        public string Desc { get; set; }

        public string Path { get; set; }

        //<表單關聯-父表>
        [ForeignKey("ProductId")]//外鍵連結關係 會自動將主鍵([Key]的Id)與類名(Product)關聯
        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        
    }
}
