using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public class EmailVerifyCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //引入上方Schema，使用資料庫自行建立的ID碼
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public string Email { get; set; }

        public string VerityCode { get; set; }

        public EmailVerifyCode(string Email,String verityCode)
        {
            this.Email = Email;
            this.CreateTime = DateTime.Now;
            this.VerityCode = verityCode;
        }
    }
}
