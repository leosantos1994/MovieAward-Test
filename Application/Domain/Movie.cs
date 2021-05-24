namespace Domain
{
    public class Movie : BaseEntity
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public string Studio { get; set; }
        public string Producer { get; set; }
        public bool Winner { get; set; }
    }
}
