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

            var labyrinth2 = new int[7, 7]
            {
                {1, 1, 1, 1, 1, 1, 1 },
                {1, 0, 0, 0, 1, 1, 1 },
                {1, 0, 1, 0, 1, 1, 1 },
                {1, 0, 0, 0, 0, 1, 1 },
                {1, 1, 1, 1, 0, 1, 1 },
                {0, 0, 0, 0, 0, 0, 0 },
                {1, 1, 1, 1, 1, 0, 1 }
            };

            var labyrinth3 = new int[9, 17]
            {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // 1
                {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1 }, // 2
                {1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1 }, // 3
                {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1 }, // 4
                {1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1 }, // 5
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1 }, // 6
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }, // 7
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 8
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } // 9
            };

            //IAlgorithm algorithm = new DepthFirstSearchAlgorithm(labyrinth2, 3, 4);
            IAlgorithm algorithm = new DepthFirstSearchAlgorithm(labyrinth3, 5, 8);
            algorithm.Execute();
            Console.ReadKey();
        }
    }
}
