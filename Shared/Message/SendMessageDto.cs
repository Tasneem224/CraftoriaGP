using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Message
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "the receiver should be determined")]
        public string ReceiverId { get; set; } = default!;

        [Required(ErrorMessage = "the content of the message is required")]
        public string Content { get; set; } = default!;
    }
}
