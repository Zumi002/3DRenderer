namespace GK_3DRendering
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using(Game game = new Game(1500, 800, "GK-3DRendering"))
            {
                game.Run();
            }
        }
    }
}
