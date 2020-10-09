using LabyrinthSearch.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch.Algorithms
{
    public class DepthFirstSearchAlgorithm : IAlgorithm
    {
        //Input data
        //Original labyrinth
        private readonly int[,] _labyrinth;

        //Labyrinth copy, to operate on
        private int[,] _operationalLabyrinth;

        private readonly int _startX;
        private readonly int _startY;

        private readonly int _labyrinthWidth;
        private readonly int _labyrinthHeight;

        //Variables for keeping status

        //4 production –shifts in X and Y.
        private int[] _cx;
        private int[] _cy;

        /// <summary>
        /// umber of trials. To compare effectiveness.
        /// </summary>
        private int _trials;

        /// <summary>
        /// Move’s number. Starts from2. Visited positions are marked.
        /// </summary>
        private int _l;

        public DepthFirstSearchAlgorithm(int[,] labyrinth, int startX, int startY)
        {
            _labyrinth = labyrinth;
            _labyrinthWidth = labyrinth.GetLength(0);
            _labyrinthHeight = labyrinth.GetLength(1);
            _startX = startX;
            _startY = startY;
        }

        public void Execute()
        {
            //Initialize variables
            _cx = new int[4];
            _cy = new int[4];
            _cx[0] = -1; _cy[0] = 0; //Go West.   4
            _cx[1] = 0; _cy[1] = -1; //Go South.   1 * 3
            _cx[2] = 1; _cy[2] = 0; //Go East.      2
            _cx[3] = 0; _cy[3] = 1; //Go North.

            _trials = 0;
            _operationalLabyrinth = (int[,])_labyrinth.Clone();

            var result = ExecuteInternal(_startX, _startY);
            Console.WriteLine(result);

            if (result)
            {
                for(int i = 0; i < _labyrinthWidth; i++)
                {
                    for(int j = 0; j < _labyrinthHeight; j++)
                    {
                        Console.Write(string.Format("{0,2} ", _operationalLabyrinth[i, j]));
                    }

                    Console.WriteLine();
                }
            }
        }

        private bool ExecuteInternal(int x, int y)
        {
            if (x == 0 || x == _labyrinthWidth - 1 || y == 0 || y == _labyrinthHeight - 1)
                return true; //TERM(DATA} = true on the boarder.

            //New position
            var newX = 0;
            var newY = 0;

            //Loop over production rules
            for(int i = 0; i < 4; i++)
            {
                newX = x + _cx[i];
                newY = y + _cy[i];
                
                //if cell is free
                if(_operationalLabyrinth[newX, newY] == 0)
                {
                    //Increase number of trials
                    _trials++;

                    //Mark cell
                    _l++;
                    _operationalLabyrinth[newX, newY] = _l;

                    //Recursive call
                    if(!ExecuteInternal(newX, newY))
                    {
                        //then mark. (0in case of BACKTRACK).
                        _operationalLabyrinth[newX, newY] = -1;
                        _l--;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
