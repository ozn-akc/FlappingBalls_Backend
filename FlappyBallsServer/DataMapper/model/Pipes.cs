public class Pipes
{
    private const int PipesAmount = 100;
    public decimal[] MapPipes { get; set; }

    public Pipes()
    {
        MapPipes = new decimal[PipesAmount];
        Random random = new Random();
        for(int i = 0; i<PipesAmount; i++)
        {
            MapPipes[i] = Math.Round((decimal)random.NextDouble(), 2);
        }
    }

}