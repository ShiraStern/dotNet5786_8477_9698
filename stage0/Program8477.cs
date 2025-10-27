using System;
namespace Stage0 
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Wellcome8477();
            Wellcome9698();

        }

        private static void Wellcome9698()
        {
            Console.ReadKey();
        }

        private static void Wellcome8477()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, wellcome to my first console application", name);

        }


    }
}


