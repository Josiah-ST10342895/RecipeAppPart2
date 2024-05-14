using System;
using System.Collections.Generic;
using System.IO;

namespace Recipe
{
    public class Menu
    {
        private const string recipesDirectory = "Recipes";

        private List<Recipe> recipes = new List<Recipe>();

        //Delegate used 

        public delegate void RecipeCalorieExceededEventHandler(string recipeName, int totalCalories);

        public event RecipeCalorieExceededEventHandler RecipeCalorieExceeded;

        public Menu()
        {
            InitializeRecipesDirectory();
            LoadRecipes();
        }

        //Main Menu uses switch case
        public void LaunchMenu()
        {
            while (true)
            {
                Console.WriteLine("Please select one of the following menu items:");
                Console.WriteLine("(1) Create a new recipe");
                Console.WriteLine("(2) Display list of recipes");
                Console.WriteLine("(3) Exit application");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("CREATE A NEW RECIPE\n************************");
                        CreateRecipe();
                        break;
                    case 2:
                        Console.WriteLine("LIST OF RECIPES\n************************");
                        DisplayRecipes();
                        break;
                    case 3:
                        Console.WriteLine("EXIT APPLICATION\n************************");
                        SaveRecipes();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option.");
                        break;
                }
            }
        }

        private void InitializeRecipesDirectory()
        {
            if (!Directory.Exists(recipesDirectory))
            {
                Directory.CreateDirectory(recipesDirectory);
            }
        }

        public enum FoodGroupEnum
        {
            StarchyFoods = 1,
            VegetablesAndFruits,
            DryBeansPeasLentilsSoya,
            ChickenFishMeatEggs,
            MilkAndDairyProducts,
            FatsAndOil,
            Water
        }

        public class Ingredient
        {
            public string Name { get; }
            public string Quantity { get; }
            public string Unit { get; }
            public int Calorie { get; }
            public FoodGroupEnum FoodGroup { get; }

            public Ingredient(string name, string quantity, string unit, int calorie, FoodGroupEnum foodGroup)
            {
                Name = name;
                Quantity = quantity;
                Unit = unit;
                Calorie = calorie;
                FoodGroup = foodGroup;
            }
        }

        private void CreateRecipe()
        {
            Console.WriteLine("Enter the name of the recipe:");
            string name = Console.ReadLine();

            Console.WriteLine("Enter the number of ingredients:");
            int ingredientCount;
            while (!int.TryParse(Console.ReadLine(), out ingredientCount) || ingredientCount < 1)
            {
                Console.WriteLine("Please enter a valid positive number for the number of ingredients.");
            }

            List<Ingredient> ingredients = new List<Ingredient>();
            for (int i = 0; i < ingredientCount; i++)
            {
                Console.WriteLine($"Enter name for ingredient {i + 1}:");
                string ingredientName = Console.ReadLine();

                Console.WriteLine($"Enter quantity for {ingredientName}:");
                string quantity = Console.ReadLine();

                Console.WriteLine($"Enter unit of measurement for {ingredientName} (e.g., grams, cups):");
                string unit = Console.ReadLine();

                Console.WriteLine($"Enter calorie count for {ingredientName}:");
                int calorie;
                while (!int.TryParse(Console.ReadLine(), out calorie) || calorie < 0)
                {
                    Console.WriteLine("Please enter a valid non-negative integer for the calorie count.");
                }

                Console.WriteLine("Specify the food group for " + ingredientName + ":");
                Console.WriteLine("1 - Starchy foods");
                Console.WriteLine("2 - Vegetables and fruits");
                Console.WriteLine("3 - Dry beans, peas, lentils and soya");
                Console.WriteLine("4 - Chicken, fish, meat and eggs");
                Console.WriteLine("5 - Milk and dairy products");
                Console.WriteLine("6 - Fats and oil");
                Console.WriteLine("7 - Water");
                int foodGroup;
                while (!int.TryParse(Console.ReadLine(), out foodGroup) || foodGroup < 1 || foodGroup > 7)
                {
                    Console.WriteLine("Please enter a valid food group number between 1 and 7.");
                }

                
                FoodGroupEnum foodGroupEnum = (FoodGroupEnum)foodGroup;

                ingredients.Add(new Ingredient(ingredientName, quantity, unit, calorie, foodGroupEnum));
            }

            Console.WriteLine("Enter the number of steps:");
            int stepCount;
            while (!int.TryParse(Console.ReadLine(), out stepCount) || stepCount < 1)
            {
                Console.WriteLine("Please enter a valid positive number.");
            }

            List<string> steps = new List<string>();
            for (int i = 0; i < stepCount; i++)
            {
                Console.WriteLine($"Enter step {i + 1}:");
                steps.Add(Console.ReadLine());
            }

            // Save recipe to file
            SaveRecipeToFile(name, ingredients, steps);
            Console.WriteLine($"\nRecipe '{name}' was created successfully!");
        }

        private void DisplayRecipes()
        {
            if (recipes.Count == 0)
            {
                Console.WriteLine("No recipes available.");
                return;
            }

            Console.WriteLine("List of Recipes:");
            foreach (var recipe in recipes)
            {
                Console.WriteLine($"- {recipe.Name}");
            }

            // Prompt user to select a recipe to display details
            Console.WriteLine("Enter the name of the recipe to display details (or type 'b' to return to the main menu):" + '\n');
            string input = Console.ReadLine();
            if (input.ToLower() == "b")
                return;

            Recipe selectedRecipe = recipes.Find(r => r.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (selectedRecipe != null)
            {
                Console.WriteLine($"Recipe Details for '{selectedRecipe.Name}':" + '\n');
                Console.WriteLine("Ingredients:");
                int totalCalories = 0; 
                foreach (var ingredient in selectedRecipe.Ingredients)
                {
                    Console.WriteLine(ingredient.Name + '\n' + "Quantity: " + ingredient.Quantity + '\n' + "Unit: " + ingredient.Unit + '\n' + "Calorie: " + ingredient.Calorie + '\n' + "Food Group: " + ingredient.FoodGroup);
                    totalCalories += ingredient.Calorie; // Adds the ingredient calorie to total
                }
                Console.WriteLine("Total Calories: " + totalCalories); // Display total calorie count
                Console.WriteLine("Steps:");
                foreach (var step in selectedRecipe.Steps)
                {
                    Console.WriteLine($"- {step}" + '\n');
                }

                // Check if total calories exceed 300 
                if (totalCalories > 300)
                {
                    RecipeCalorieExceeded?.Invoke(selectedRecipe.Name, totalCalories);
                }
            }
            else
            {
                Console.WriteLine($"Recipe '{input}' not found.");
            }
        }

        private void SaveRecipeToFile(string name, List<Ingredient> ingredients, List<string> steps)
        {
            string recipeFileName = $"{recipesDirectory}/{name}.txt";
            using (StreamWriter writer = new StreamWriter(recipeFileName))
            {
                writer.WriteLine(name);
                foreach (var ingredient in ingredients)
                {
                    writer.WriteLine($"{ingredient.Name},{ingredient.Quantity},{ingredient.Unit},{ingredient.Calorie},{(int)ingredient.FoodGroup}");
                }
                writer.WriteLine(string.Join("\n", steps));
            }
        }

        private void LoadRecipes()
        {
            string[] recipeFiles = Directory.GetFiles(recipesDirectory, "*.txt");
            foreach (var recipeFile in recipeFiles)
            {
                List<string> lines = File.ReadAllLines(recipeFile).ToList();
                if (lines.Count < 3)
                {
                    Console.WriteLine($"Skipping malformed recipe file: {recipeFile}");
                    continue;
                }

                string name = lines[0];
                List<Ingredient> ingredients = new List<Ingredient>();
                List<string> steps = new List<string>();
                List<string> malformedLines = new List<string>(); 

                for (int i = 1; i < lines.Count - 1; i++)
                {
                    string[] ingredientDetails = lines[i].Split(',');
                    if (ingredientDetails.Length != 5)
                    {
                        malformedLines.Add(lines[i]); 
                        continue;
                    }

                    string ingredientName = ingredientDetails[0];
                    string quantity = ingredientDetails[1];
                    string unit = ingredientDetails[2];
                    int calorie;
                    if (!int.TryParse(ingredientDetails[3], out calorie) || calorie < 0)
                    {
                        malformedLines.Add(lines[i]); 
                        continue;
                    }
                    FoodGroupEnum foodGroup;
                    if (!Enum.TryParse(ingredientDetails[4], out foodGroup) || !Enum.IsDefined(typeof(FoodGroupEnum), foodGroup))
                    {
                        malformedLines.Add(lines[i]);
                        continue;
                    }

                    ingredients.Add(new Ingredient(ingredientName, quantity, unit, calorie, foodGroup));
                }

           
                for (int i = lines.Count - 1; i < lines.Count; i++)
                {
                    steps.AddRange(lines[i].Split(';'));
                }

              
                foreach (var line in malformedLines)
                {
                    Console.WriteLine($"Skipping malformed line: {line}");
                }

                Recipe recipe = new Recipe(name, ingredients, steps);
                recipes.Add(recipe);
            }
        }

        private void SaveRecipes()
        {
            
        }
    }

    public class Recipe
    {
        public string Name { get; }
        public List<Menu.Ingredient> Ingredients { get; }
        public List<string> Steps { get; }

        public Recipe(string name, List<Menu.Ingredient> ingredients, List<string> steps)
        {
            Name = name;
            Ingredients = ingredients;
            Steps = steps;
        }
    }
}
