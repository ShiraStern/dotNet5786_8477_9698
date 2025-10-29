using System;
namespace stage0
{
    partial class Program
     {
        static void Main(string[] args)
        {
            Wellcome8477();
            Wellcome9698();
            Console.ReadKey();
        }
        static partial void Wellcome9698();
        private static void Wellcome8477()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, wellcome to my first console application", name);

        }
    }
}