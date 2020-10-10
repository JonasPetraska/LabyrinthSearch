using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch.Helpers
{
    /// <summary>
    /// Array helpers
    /// </summary>
    public static class ArrayHelpers
    {
        /// <summary>
        /// Inverses matrix to provide proper matrix for printing
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>Inversed matrix</returns>
        public static int[,] InverseArray(int[,] matrix)
        {
            var res = new int[matrix.GetLength(0), matrix.GetLength(1)];
            res = (int[,]) matrix.Clone();
            return ReverseY(Transpose(res));
        }

        /// <summary>
        /// Reverses Y direction of matrix columns
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>Reversed matrix</returns>
        public static int[,] ReverseY(int[,] matrix)
        {
            int w = matrix.GetLength(1);
            int h = matrix.GetLength(0);

            int[,] result = new int[h, w];

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[h - i - 1, j] = matrix[i, j];

            return result;
        }

        /// <summary>
        /// Transposes matrix (switches columns and rows)
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>Transposed matrix</returns>
        public static int[,] Transpose(int[,] matrix)
        {
            int w = matrix.GetLength(1);
            int h = matrix.GetLength(0);

            int[,] result = new int[h, w];

            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    result[j, i] = matrix[i, j];

            return result;
        }
    }
}
