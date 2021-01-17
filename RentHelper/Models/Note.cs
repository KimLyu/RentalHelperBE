using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //導入Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }

        //<表單關聯-父表>
        [ForeignKey("OrderId")]//外鍵連結關係 會自動將主鍵([Key]的Id)與類名(Order)關聯
        [Required]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public Guid SenderId { get; set; }
        public String SenderName { get; set; }
        public String Message { get; set; }
        public DateTime createTime { get; set; }

    }
}
