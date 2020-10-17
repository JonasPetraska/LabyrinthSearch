using LabyrinthSearch.Abstractions;
using LabyrinthSearch.Helpers;
using LabyrinthSearch.Models;
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

            var result = ExecuteInternal(_startRow, _startColumn);

            //Print results
            _loggerService.WriteLine("PART 3. Results");
            _loggerService.WriteLine("");

            _loggerService.WriteLine($"3.1. {(result.IsSuccess ? "Path is found" : "Path is not found")}.");
            _loggerService.WriteLine("");

            _loggerService.WriteLine("3.2. Path graphically:");
            _loggerService.WriteLine("LABCOPY");
            _loggerService.WriteLine("");

            StringHelpers.PrintMatrix(_labyrinthWidth, _labyrinthHeight, ArrayHelpers.ReverseY(_operationalLabyrinth), _loggerService);
            _loggerService.WriteLine("");

            //If path is found - print nodes and rules
            if (result.IsSuccess)
            {
                _loggerService.WriteLine($"3.3. Rules: {string.Join(", ", result.Rules)}.");
                _loggerService.WriteLine("");

                _loggerService.WriteLine($"3.4. Nodes: {string.Join(", ", result.Path.Select(n => $"[X={n.Value + 1},Y={n.Key + 1}]"))}.");
            }
        }

        /// <summary>
        /// Iterative breadth first algorithm
        /// </summary>
        /// <param name="row">Row to start at</param>
        /// <param name="column">Column to start at</param>
        /// <returns>True - if path is found, false - if path is not found.</returns>
        private BreadthFirstResult ExecuteInternal(int row, int column)
        {
            var result = new BreadthFirstResult();
            if (row == 0 || row == _labyrinthHeight - 1 || column == 0 || column == _labyrinthWidth - 1)
            {
                result.IsSuccess = true;
                GetTrace(row, column, result);
                return result;
            }

            var newRow = 0;
            var newColumn = 0;

            while (_close <= _new)
            {
                row = _fRow[_close];
                column = _fColumn[_close];

                //Loop over production rules
                for (int i = 0; i < 4; i++)
                {
                    //Calculate new coordinates
                    newRow = row + _cRow[i];
                    newColumn = column + _cColumn[i];

                    //if cell is free
                    if (_operationalLabyrinth[newRow, newColumn] == 0)
                    {
                        _operationalLabyrinth[newRow, newColumn] = _operationalLabyrinth[row, column] + 1;
                        if (newRow == 0 || newRow == _labyrinthHeight - 1 || newColumn == 0 || newColumn == _labyrinthWidth - 1)
                        {
                            GetTrace(newRow, newColumn, result);
                            result.IsSuccess = true;
                            return result;
                        }

                        _new++;
                        _fRow[_new] = newRow;
                        _fColumn[_new] = newColumn;
                    }
                }

                _close++;
            }

            return result;
        }

        private void GetTrace(int endRow, int endColumn, BreadthFirstResult result)
        {
            var newRow = 0;
            var newColumn = 0;

            var endRowCopy = endRow;
            var endColumnCopy = endColumn;
            var labCopy = (int[,])_labyrinth.Clone();

            var endRowNotChanged = endRow;
            var endColumnNotChanged = endColumn;

            _labyrinth[endRow, endColumn] = _operationalLabyrinth[endRow, endColumn];

            var path = new List<KeyValuePair<int, int>>();
            path.Add(new KeyValuePair<int, int>(endRow, endColumn));

            //Collect path
            do
            {
                for (int k = 3; k >= 0; k--)
                {
                    newRow = endRow + _cRow[k];
                    newColumn = endColumn + _cColumn[k];

                    //if in labyrinth
                    if (newRow > 0 && newRow < _labyrinthHeight - 1 && newColumn > 0 && newColumn < _labyrinthWidth - 1)
                    {
                        //if previous number
                        if (_operationalLabyrinth[newRow, newColumn] == _operationalLabyrinth[endRow, endColumn] - 1)
                        {
                            //previous step found
                            _labyrinth[newRow, newColumn] = _operationalLabyrinth[newRow, newColumn];
                            endRow = newRow;
                            endColumn = newColumn;
                            path.Add(new KeyValuePair<int, int>(endRow, endColumn));
                        }
                    }
                }

            } while (_labyrinth[endRow, endColumn] != 2);

            //Reverse path, first element = start element
            path.Reverse();

            result.Path = path;

            var newN = 0;
            var close = 0;
            newColumn = 0;
            newRow = 0;

            var processNextIteration = new List<KeyValuePair<int, int>>();
            var processNextIterationCopy = new List<KeyValuePair<int, int>>();

            //Collect trace
            for (int i = 0; i < path.Count; i++)
            {
                processNextIteration = new List<KeyValuePair<int, int>>(processNextIterationCopy);
                processNextIterationCopy.Clear();
                _loggerService.Write($"Wave {i}, label L=\"{_operationalLabyrinth[path[i].Key, path[i].Value]}\"");

                if (i == 0)
                {
                    _loggerService.Write($". Initial position: X={path[i].Value + 1}, Y={path[i].Key + 1}, NEWN={newN + 1}");
                    processNextIterationCopy.Add(new KeyValuePair<int, int>(path[i].Key, path[i].Value));
                    _loggerService.WriteLine("");
                    continue;
                }

                _loggerService.WriteLine("");

                for (int j = 0; j < processNextIteration.Count; j++)
                {
                    _loggerService.WriteLine($"     Close CLOSE={close + 1}, X={processNextIteration[j].Value + 1}, Y={processNextIteration[j].Key + 1}.");
                    close++;

                    for (int k = 0; k < 4; k++)
                    {
                        newRow = processNextIteration[j].Key + _cRow[k];
                        newColumn = processNextIteration[j].Value + _cColumn[k];

                        _loggerService.Write($"         R{k + 1}. X={newColumn + 1}, Y={newRow + 1}.");

                        //if in labyrinth
                        if ((newRow > 0 && newRow < _labyrinthHeight - 1 && newColumn > 0 && newColumn < _labyrinthWidth - 1) || 
                            (newRow == endRowNotChanged && newColumn == endColumnNotChanged))
                        {
                            //if terminal.
                            if (newRow == endRowNotChanged && newColumn == endColumnNotChanged)
                            {
                                endRowCopy = newRow;
                                endColumnCopy = newColumn;
                                newN++;
                                _loggerService.Write($" Free. NEWN={newN + 1}. Terminal.");
                                processNextIterationCopy.Add(new KeyValuePair<int, int>(newRow, newColumn));
                                _loggerService.WriteLine("");
                                goto terminalCheckpoint;

                            }
                            //if next number
                            else if (_operationalLabyrinth[newRow, newColumn] + 1 == _operationalLabyrinth[endRowCopy, endColumnCopy])
                            {
                                endRowCopy = newRow;
                                endColumnCopy = newColumn;
                                newN++;
                                _loggerService.Write($" Free. NEWN={newN + 1}");
                                processNextIterationCopy.Add(new KeyValuePair<int, int>(newRow, newColumn));
                            }
                            //if free
                            else if (labCopy[newRow, newColumn] == 0)
                            {
                                if (_fRow[newN + 1] == newRow && _fColumn[newN + 1] == newColumn)
                                {
                                    newN++;
                                    _loggerService.Write($" Free. NEWN={newN + 1}");
                                    processNextIterationCopy.Add(new KeyValuePair<int, int>(newRow, newColumn));
                                }
                                else
                                {
                                    _loggerService.Write($" CLOSED OR OPEN.");
                                }
                            }
                            //if wall
                            else if(labCopy[newRow, newColumn] == 1)
                            {
                                _loggerService.Write($" Wall.");
                            }
                        }
                        else
                        {
                            _loggerService.Write($" Wall.");
                        }

                        _loggerService.WriteLine("");
                    }
                }
            }

            terminalCheckpoint:

            //Compute rules
            var rules = new List<string>();
            for (int i = 0; i < path.Count; i++)
                for (int j = 0; j < 4; j++)
                    if (i != path.Count - 1 && path[i + 1].Value == path[i].Value + _cColumn[j] && path[i + 1].Key == path[i].Key + _cRow[j])
                        rules.Add($"R{j + 1}");

            result.Rules = rules;
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
            _loggerService.WriteLine($"1.2. Initial position X={_startColumn + 1}, Y={_startRow + 1}, L={_l}");
            _loggerService.WriteLine("");
        }

        public void Dispose()
        {
            _loggerService.Dispose();
        }
    }
}
