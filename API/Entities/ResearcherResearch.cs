namespace API.Entities
{
    public class ResearcherResearch
    {
        public ResearcherResearch(int researchId, int researcherId) 
        {
            this.researchId = researchId;
            this.researcherId = researcherId;
        }

        public int researchId { get; set; }
        public int researcherId { get; set; }
    }
}