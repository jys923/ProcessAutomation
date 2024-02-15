using EFCoreSample.Data;

namespace EFCoreSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            using (var db = new EFCoreSampleDbContext())
            {
            }
        }
    }
}
