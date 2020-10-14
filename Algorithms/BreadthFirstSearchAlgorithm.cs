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
    public class BreadthFirstSearchAlgorithm : IAlgorithm, IDisposable
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

        //The "Front to store opened nodes"
        private int[] _fColumn;
        private int[] _fRow;

        /// <summary>
        /// Number of waves
        /// </summary>
        private int _wave;

        /// <summary>
        /// Move’s number. Starts from 2. Visited positions are marked.
        /// </summary>
        private int _l;

        /// <summary>
        /// Count of closed nodes
        /// </summary>
        private int _close;

        /// <summary>
        /// Count of new nodes
        /// </summary>
        private int _new;

        private ILoggerService _loggerService;

        public BreadthFirstSearchAlgorithm(int[,] labyrinth, int startRow, int startColumn, ILoggerService loggerService)
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
            _fColumn = new int[_labyrinthHeight * _labyrinthWidth];
            _fRow = new int[_labyrinthHeight * _labyrinthWidth];

            //Set-up production rules
            _cRow[0] = 0; _cColumn[0] = -1; //Go West.   4 (left)
            _cRow[1] = -1; _cColumn[1] = 0; //Go South.   1 * 3 (down)
            _cRow[2] = 0; _cColumn[2] = 1; //Go East.      2 (right)
            _cRow[3] = 1; _cColumn[3] = 0; //Go North. (up)

            _operationalLabyrinth = (int[,])_labyrinth.Clone();
            _l = 2;
            _operationalLabyrinth[_startRow, _startColumn] = _l;

            PrintPart1();
            _loggerService.WriteLine("PART 2. Trace");
            _loggerService.WriteLine("");

            _wave = 0;
            _close = 0;
            _new = 0;
            _fColumn[0] = _startColumn;
            _fRow[0] = _startRow;


            _loggerService.WriteLine($"WAVE {_wave}, label L=\"{_l}\". Initial position: X={_startColumn + 1}, Y={_startRow + 1}, NEWN={_new + 1}");

            var result = ExecuteInternal(_startRow, _startColumn);

            //Print results
            _loggerService.WriteLine("PART 3. Results");
            _loggerService.WriteLine("");

            _loggerService.WriteLine($"3.1. {(result ? "Path is found" : "Path is not found")}.");
            _loggerService.WriteLine("");

            _loggerService.WriteLine("3.2. Path graphically:");
            _loggerService.WriteLine("LABCOPY");
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
        /// Iterative breadth first algorithm
        /// </summary>
        /// <param name="row">Row to start at</param>
        /// <param name="column">Column to start at</param>
        /// <returns>True - if path is found, false - if path is not found.</returns>
        private bool ExecuteInternal(int row, int column)
        {
            if (row == 0 || row == _labyrinthHeight - 1 || column == 0 || column == _labyrinthWidth - 1)
                return true;

            var newRow = 0;
            var newColumn = 0;
            var printHeader = false;
            while (_close <= _new)
            {
                row = _fRow[_close];
                column = _fColumn[_close];

                if (printHeader)
                {
                    _wave++;
                    _loggerService.WriteLine($"Wave {_wave}, label L=\"{_operationalLabyrinth[row, column]}\"");
                }

                //Loop over production rules
                for (int i = 0; i < 4; i++)
                {

                    if (printHeader)
                    {
                        _wave++;
                        _loggerService.WriteLine($"Wave {_wave}, label L=\"{_operationalLabyrinth[row, column]}\"");
                    }

                    //Calculate new coordinates
                    newRow = row + _cRow[i];
                    newColumn = column + _cColumn[i];

                    //if cell is free
                    if (_operationalLabyrinth[newRow, newColumn] == 0)
                    {
                        _operationalLabyrinth[newRow, newColumn] = _operationalLabyrinth[row, column] + 1;
                        printHeader = true;
                        if (newRow == 0 || newRow == _labyrinthHeight - 1 || newColumn == 0 || newColumn == _labyrinthWidth - 1)
                        {
                            if (printHeader)
                            {
                                _wave++;
                                _loggerService.WriteLine($"Wave {_wave}, label L=\"{_operationalLabyrinth[newRow, newColumn]}\"");
                            }
                            //var trace = GetTrace(newRow, newColumn);
                            //PrintTrace(newRow, newColumn);
                            return true;
                        }

                        _new++;
                        _fRow[_new] = newRow;
                        _fColumn[_new] = newColumn;
                    }
                    else
                        printHeader = false;
                }

                _close++;
            }

            return false;
        }

        //private string GetTrace(int endRow, int endColumn)
        //{
        //    var trace = new StringBuilder();
        //    var newRow = 0;
        //    var newColumn = 0;

        //    _labyrinth[endRow, endColumn] = _operationalLabyrinth[endRow, endColumn];

        //    var k = 4;

        //    while (_labyrinth[endRow, endColumn] != 2)
        //    {
        //        k--;
        //        //_loggerService.WriteLine($"Wave {_operationalLabyrinth[endRow, endColumn]}, label L=\"{_operationalLabyrinth[endRow, endColumn]}\"");
        //        newRow = endRow + _cRow[k];
        //        newColumn = endColumn + _cColumn[k];

        //        if (newRow > 0 && newRow < _labyrinthHeight - 1 && newColumn > 0 && newColumn < _labyrinthWidth - 1)
        //        {
        //            if (_operationalLabyrinth[newRow, newColumn] == _operationalLabyrinth[endRow, endColumn] - 1)
        //            {
        //                _loggerService.WriteLine($"Wave {_operationalLabyrinth[newRow, newColumn]}, label L=\"{_operationalLabyrinth[newRow, newColumn]}\"");
        //                _labyrinth[newRow, newColumn] = _operationalLabyrinth[newRow, newColumn];
        //                endRow = newRow;
        //                endColumn = newColumn;
        //                k = 4;
        //            }
        //        }

        //    }

        //    return trace.ToString();
        //}

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
            _loggerService.WriteLine($"1.2. Initial position X={_startColumn + 1}, Y={_startRow + 1}, L={_l}");
            _loggerService.WriteLine("");
        }

        public void Dispose()
        {
            _loggerService.Dispose();
        }
    }
}
