namespace API.Entities
{
    public class Researcher
    {
        public Researcher()
        {
        }

        public Researcher(int id, string firstname, string lastname, string email, string phone) 
        {
            this.Id = id;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Email = email;
            this.Phone = phone;
        }
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}