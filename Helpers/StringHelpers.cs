using LabyrinthSearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch.Helpers
{
    /// <summary>
    /// String helpers
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Repeats the symbol amount of times and forms a string
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="amount">Amount of times to repeat symbol</param>
        /// <returns>String composed of symbol repeated amount of times</returns>
        public static string RepeatSymbol(char symbol, int amount)
        {
            var str = new StringBuilder();
            for (int i = 0; i < amount; i++)
                str.Append(symbol);

            return str.ToString();
        }

        /// <summary>
        /// Utility to print matrix
        /// </summary>
        /// <param name="width">Width of matrix (x-axis)</param>
        /// <param name="height">Height of matrix (y-axis)</param>
        /// <param name="matrix">Matrix</param>
        public static void PrintMatrix(int width, int height, int[,] matrix, ILoggerService loggerService)
        {
            //Compute first line
            var firstLine = new StringBuilder();
            firstLine.Append("       ");
            firstLine.Append("Y, V");

            loggerService.WriteLine(firstLine.ToString());

            //Compute second line
            var secondLine = new StringBuilder();
            secondLine.Append("       ");
            secondLine.Append("^");

            for (int i = 8; i < width; i++)
                secondLine.Append(" ");

            loggerService.WriteLine(secondLine);

            //Compute rest of lines
            for (int i = 0; i < height; i++)
            {
                var line = new StringBuilder();
                line.Append("    ");
                line.Append(string.Format("{0,3}", height - i));
                line.Append("|");

                for (int j = 0; j < width; j++)
                    line.Append(string.Format(" {0,3} ", matrix[i, j]));

                loggerService.WriteLine(line);
            }

            //Print last lines
            //Compute second to last line
            var secondToLastLine = new StringBuilder();
            secondToLastLine.Append("     ");

            for (int i = 0; i < width * 5 + 3; i++)
                secondToLastLine.Append("-");

            secondToLastLine.Append(">");
            secondToLastLine.Append(" ");
            secondToLastLine.Append("X,U");

            loggerService.WriteLine(secondToLastLine);

            //Compute last line
            var lastLine = new StringBuilder();
            lastLine.Append("        ");
            for (int i = 1; i <= width; i++)
                lastLine.Append(string.Format(" {0, 3} ", i));

            loggerService.WriteLine(lastLine);
        }
    }
}
