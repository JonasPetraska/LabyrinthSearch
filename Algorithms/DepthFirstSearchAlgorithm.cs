using LabyrinthSearch.Abstractions;
using LabyrinthSearch.Helpers;
using LabyrinthSearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch.Algorithms
{
    public class DepthFirstSearchAlgorithm : IAlgorithm, IDisposable
    {
        //Input data
        //Original labyrinth
        private readonly int[,] _labyrinth;

        //Labyrinth copy, to operate on
        private int[,] _operationalLabyrinth;

        private readonly int _startRow;
        private readonly int _startColumn;

        private readonly int _labyrinthWidth;
        private readonly int _labyrinthHeight;

        //Variables for keeping status

        //4 production –shifts in X and Y.
        private int[] _cRow;
        private int[] _cColumn;

        /// <summary>
        /// Number of trials. To compare effectiveness.
        /// </summary>
        private int _trials;

        /// <summary>
        /// Move’s number. Starts from 2. Visited positions are marked.
        /// </summary>
        private int _l;

        private int _level;

        private ILoggerService _loggerService;

        public DepthFirstSearchAlgorithm(int[,] labyrinth, int startRow, int startColumn, ILoggerService loggerService)
        {
            _labyrinth = labyrinth;
            _labyrinthWidth = labyrinth.GetLength(1);
            _labyrinthHeight = labyrinth.GetLength(0);
            _startRow = startRow;
            _startColumn = startColumn;
            _loggerService = loggerService;
        }

        public void Execute()
        {
            //Initialize variables
            _cRow = new int[4];
            _cColumn = new int[4];

            //Set-up production rules
            _cRow[0] = 0; _cColumn[0] = -1; //Go West.   4 (left)
            _cRow[1] = -1; _cColumn[1] = 0; //Go South.   1 * 3 (down)
            _cRow[2] = 0; _cColumn[2] = 1; //Go East.      2 (right)
            _cRow[3] = 1; _cColumn[3] = 0; //Go North. (up)

            _operationalLabyrinth = (int[,])_labyrinth.Clone();
            _l = 2;

            PrintPart1();
            _loggerService.WriteLine("PART 2. Trace");

            _level = 0;
            _trials = 0;
            _operationalLabyrinth[_startRow, _startColumn] = _l;

            var trace = new StringBuilder();

            var result = ExecuteInternal(_startRow, _startColumn, trace);

            //Add trace of terminal state
            if (result) 
            {
                var strTrace = trace.ToString();
                trace = new StringBuilder().Append(strTrace.Remove(strTrace.LastIndexOf(System.Environment.NewLine))).Append(" Terminal.");
            }

            //Print trace
            _loggerService.WriteLine(trace);
            _loggerService.WriteLine("");

            //Print results
            _loggerService.WriteLine("PART 3. Results");
            _loggerService.WriteLine("");

            _loggerService.WriteLine($"3.1. {(result ? "Path is found" : "Path is not found")}.");
            _loggerService.WriteLine("");

            _loggerService.WriteLine("3.2. Path graphically:");
            _loggerService.WriteLine("");

            StringHelpers.PrintMatrix(_labyrinthWidth, _labyrinthHeight, ArrayHelpers.ReverseY(_operationalLabyrinth), _loggerService);
            _loggerService.WriteLine("");

            //If path is found - print nodes and rules
            if (result)
            {
                //Compute nodes
                var nodes = new List<(int x, int y)>();
                for (int i = 2; i <= _l; i++)
                    for (int u = 0; u < _labyrinthHeight; u++)
                        for (int v = 0; v < _labyrinthWidth; v++)
                            if (_operationalLabyrinth[u, v] == i)
                                nodes.Add((v + 1, u + 1));
                //Print rules
                var rules = new List<string>();
                for (int i = 0; i < nodes.Count; i++)
                    for (int j = 0; j < 4; j++)
                        if (i != nodes.Count - 1 && nodes[i + 1].x == nodes[i].x + _cColumn[j] && nodes[i + 1].y == nodes[i].y + _cRow[j])
                            rules.Add($"R{j + 1}");


                _loggerService.WriteLine($"3.3. Rules: {string.Join(", ", rules)}.");
                _loggerService.WriteLine("");

                _loggerService.WriteLine($"3.4. Nodes: {string.Join(", ", nodes.Select(n => $"[X={n.x},Y={n.y}]"))}.");
            }
        }

        /// <summary>
        /// Internal recursive algorithm
        /// </summary>
        /// <param name="row">Row to start algorithm from</param>
        /// <param name="column">Column to start algorithm from</param>
        /// <returns>True - path found, false - path wasn't found</returns>
        private bool ExecuteInternal(int row, int column, StringBuilder trace)
        {
            if (row == 0 || row == _labyrinthHeight - 1 || column == 0 || column == _labyrinthWidth - 1)
                return true; //TERM(DATA} = true on the boarder.

            //New position
            var newRow = 0;
            var newColumn = 0;

            _level++;

            //Loop over production rules
            for (int i = 0; i < 4; i++)
            {
                //Increase number of trials
                _trials++;

                //Calculate new coordinates
                newRow = row + _cRow[i];
                newColumn = column + _cColumn[i];

                trace.Append($"{string.Format("{0,4}", _trials)}) {StringHelpers.RepeatSymbol('-', _level - 1)}R{i + 1}. U={newColumn + 1}, V={newRow + 1}. ");

                //if cell is free
                if (_operationalLabyrinth[newRow, newColumn] == 0)
                {
                    trace.Append($"Free. L:=L+1={_l + 1}. LAB[{newColumn + 1},{newRow + 1}]:={_l + 1}.");
                    trace.AppendLine();

                    //Mark cell
                    _l++;
                    _operationalLabyrinth[newRow, newColumn] = _l;

                    //Recursive call
                    if (!ExecuteInternal(newRow, newColumn, trace))
                    {
                        //then mark. (0 in case of BACKTRACK).
                        _operationalLabyrinth[newRow, newColumn] = -1;
                        _l--;
                        _level--;
                        trace.AppendLine($"{string.Format("{0,6}", " ")}{StringHelpers.RepeatSymbol('-', _level)}Backtrack from X={newColumn + 1}, Y={newRow + 1}, L={_l + 1}. LAB[{newColumn + 1},{newRow + 1}]:=-1. L:=L-1={_l}");
                    }
                    else
                        return true;
                }
                //We've already visited it (L starts from 2 by definition)
                else if (_operationalLabyrinth[newRow, newColumn] >= 2)
                {
                    trace.Append("Thread. ");
                    trace.AppendLine();
                }
                //We've hit a wall.
                else
                {
                    trace.Append("Wall. ");
                    trace.AppendLine();
                }
            }

            return false;
        }

        /// <summary>
        /// Helper method to print initial data
        /// </summary>
        private void PrintPart1()
        {
            _loggerService.WriteLine("PART 1. Data");
            _loggerService.WriteLine("1.1. Labyrinth");
            _loggerService.WriteLine("");

            StringHelpers.PrintMatrix(_labyrinthWidth, _labyrinthHeight, ArrayHelpers.ReverseY(_operationalLabyrinth), _loggerService);

            _loggerService.WriteLine("");
            _loggerService.WriteLine($"1.2. Initial position X={_startColumn+1}, Y={_startRow+1}, L={_l}");
            _loggerService.WriteLine("");
        }

        public void Dispose()
        {
            _loggerService.Dispose();
        }
    }
}
