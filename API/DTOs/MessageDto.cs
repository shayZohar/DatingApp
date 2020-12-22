using System;
using System.Text.Json.Serialization;

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderphotoUrl { get; set; }
        public int RecipentId { get; set; }
        public string RecipentUsername { get; set; }
        public string RecipentPhotoUrl { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        [JsonIgnore] // it means the data wont be sent back to the client
        public bool SenderDeleted { get; set; }
        [JsonIgnore] // it means the data wont be sent back to the client
        public bool RecipentDeleted { get; set; }

    }
}