using LabyrinthSearch.Abstractions;
using LabyrinthSearch.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read file
            Console.WriteLine("Please provide a file name:");
            var fileName = Console.ReadLine();
            if (String.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Please provide a file name.");
                return;
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} is not found.");
                Console.ReadKey();
                return;
            }

            var text = File.ReadAllText(fileName);
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var height = lines.Count() - 1;
            var width = lines[0].Split(' ').Count();

            var labyrinth = new int[height, width];
            for(int i = 0; i < height; i++)
            {
                var line = lines[i].Split(' ');
                for (int j = 0; j < line.Length; j++)
                    labyrinth[i, j] = int.Parse(line[j]);
            }

            var startCoordinates = lines.Last().Split(' ');
            var x = int.Parse(startCoordinates[0]);
            var y = int.Parse(startCoordinates[1]);

            IAlgorithm algorithm = new DepthFirstSearchAlgorithm(labyrinth, y - 1, x - 1);
            //IAlgorithm algorithm = new DepthFirstSearchAlgorithm(labyrinth3, 5, 8);
            algorithm.Execute();
            Console.ReadKey();
        }
    }
}
