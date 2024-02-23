namespace EFCoreSample.Entities
{
    public class ProbeSNView
    {
        public required string ProbeSN { get; set; }
        public IList<int>? Results { get; set; }
    }
}
