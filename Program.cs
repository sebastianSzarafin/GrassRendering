namespace GrassRendering
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(1280, 720, "GrassRendering"))
            {
                game.Run();
            }
        }
    }
}