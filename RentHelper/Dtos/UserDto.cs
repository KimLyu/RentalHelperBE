using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        //public string Password { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }

        //<表單關聯-子表>使用者持有多筆產品
        public ICollection<ProductDto> Products { get; set; }

        public ICollection<WishItemDto> WishItems { get; set; }

    }
}
