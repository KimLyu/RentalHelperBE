﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //導入Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }

        //<表單關聯-父表>
        [ForeignKey("UserId")]//外鍵連結關係 會自動將主鍵([Key]的Id)與類名(Product)關聯
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

    }
}
