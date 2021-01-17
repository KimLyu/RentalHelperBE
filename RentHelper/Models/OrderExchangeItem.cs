using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class OrderExchangeItem
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //引入上方Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }

        [ForeignKey("OrderId")]//外鍵連結關係 會自動將主鍵([Key]的Id)與類名(Product)關聯
        [Required]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }//反向導航
        public Guid WishItemId { get; set; }
        public WishItem WishItem { get; set; }//反向導覽用
        public int packageQuantity { get; set; } //訂單交易的交換數量 
    }
}
