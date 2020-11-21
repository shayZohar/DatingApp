namespace API.Entities
{
    public class Research
    {
        public Research()
        {
        }

        public Research(int id, string name, string uni) 
        {
            this.Id = id;
            this.Name = name;
            this.University = uni;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string University { get; set; }
    }
}