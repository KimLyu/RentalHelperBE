using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class PictureDto
    {
        public Guid Id { get; set; }
        public string DateTime { get; set; }
        public string Desc { get; set; }
        public string Path { get; set; }
        public Guid ProductId { get; set; }
    }
}
