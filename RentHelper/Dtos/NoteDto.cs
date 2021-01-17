using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Dtos
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid SenderId { get; set; }
        public String SenderName { get; set; }
        public String Message { get; set; }
        public string createTime { get; set; }
    }
}
