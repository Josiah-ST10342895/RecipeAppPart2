using System;
using System.ComponentModel.Design;


namespace Recipe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Gourmet Guide!" + '\n' + "*****************************");
            Console.WriteLine("");
            Console.WriteLine("Enter '1' to launch menu or any other key to exit:");
            string userInput = Console.ReadLine();


            if (userInput == "1")
            {
                Menu menu = new Menu();
                menu.LaunchMenu();
            }
            else
            {
                Console.WriteLine("Exiting the program.");
                Environment.Exit(0);
            }
        }

    }
}

