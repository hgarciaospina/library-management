using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.DTOs
{
    public class BookCreateDto
    {
        public string ?Title { get; set; }
        public string ?Author { get; set; }
        public string ?ISBN { get; set; }
        public int PublicationYear { get; set; }
        public int LibraryId { get; set; }
    }
}
