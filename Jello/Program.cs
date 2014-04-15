using OpenTK;

namespace Jello
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var jello = new Jello())
            {
                jello.Run();
            }
        }
    }
}
