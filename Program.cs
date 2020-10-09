using LabyrinthSearch.Abstractions;
using LabyrinthSearch.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var labyrinth = new int[7, 7]
            {
                {1, 1, 1, 1, 1, 0, 1 },
                {0, 0, 0, 0, 0, 0, 0 },
                {1, 1, 1, 1, 0, 1, 1 },
                {1, 0, 0, 0, 0, 1, 1 },
                {1, 0, 1, 0, 1, 1, 1 },
                {1, 0, 0, 0, 1, 1, 1 },
                {1, 1, 1, 1, 1, 1, 1 }
            };

            IAlgorithm algorithm = new DepthFirstSearchAlgorithm(labyrinth, 4, 3);
            algorithm.Execute();
            Console.ReadKey();
        }
    }
}
