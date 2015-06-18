using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTS
{
    public static class Sudoku
    {
        public static string Solve(string input)
        {
            byte[,] blocks = new byte[9, 9];
            int i = 0;
            foreach (byte block in input.Split(',')
                .Select(x => x == "" ? "0" : x)
                .Select(x => Byte.Parse(x))
                .ToArray())
            {
                int x = i % 9;
                int y = (i - x) / 9;
                blocks[x, y] = block;
                i++;
            }

            bool[, ,] possibilities = Generatepossibilities(blocks);
            Backtrack(blocks, possibilities);
            byte[] solvedBlocks = new byte[9 * 9];

            for (i = 0; i < 9 * 9; i++)
            {
                int x = i % 9;
                int y = (i - x) / 9;
                solvedBlocks[i] = blocks[x, y];
            }

            return String.Join(",", solvedBlocks);
        }

        private static bool Backtrack(byte[,] blocks, bool[, ,] possibilities, int x = 0, int y = 0)
        {
            if (x == 8 && y == 8)
                return true;

            while (blocks[x, y] != 0)
            {
                if (++y >= 9)
                {
                    y = 0;
                    if (++x >= 9)
                        return true;
                }
            }
            int ty, tx;

            for (byte val = 1; val <= 9; val++)
            {
                if (possibilities[x, y, val - 1])
                {
                    bool[, ,] clone = possibilities.Clone() as bool[, ,];
                    SetBlock(blocks, possibilities, x, y, val);
                    ty = y;
                    tx = x;
                    if (++ty >= 9)
                    {
                        ty = 0;
                        if (++tx >= 9)
                            return true;
                    }

                    if (Backtrack(blocks, possibilities, tx, ty))
                        return true;
                    else
                    {
                        possibilities = clone;
                        blocks[x, y] = 0;
                    }
                }
            }

            if (--y < 0)
            {
                y = 8;
                x--;
            }
            return false;
        }

        private static bool[, ,] Generatepossibilities(byte[,] blocks)
        {
            bool[, ,] ret = new bool[9, 9, 9];

            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                    for (byte val = 1; val <= 9; val++)
                    {
                        bool possible = true;
                        possible &= blocks[x, y] == 0;
                        possible &= SolveRow(blocks, x, y, val);
                        possible &= SolveColumn(blocks, x, y, val);
                        possible &= SolveCell(blocks, x, y, val);
                        ret[x, y, val - 1] = possible;
                    }

            return ret;
        }

        private static void SetBlock(byte[,] blocks, bool[, ,] possibilities, int x, int y, byte val)
        {
            blocks[x, y] = val;

            // Rows
            for (int ix = 0; ix < 9; ix++)
                possibilities[ix, y, val - 1] = false;

            // Columns
            for (int iy = 0; iy < 9; iy++)
                possibilities[x, iy, val - 1] = false;

            // Cell
            int startx = x - (x % 3);
            int starty = y - (y % 3);
            for (int ix = startx; ix < startx + 3; ix++)
            {
                for (int iy = starty; iy < starty + 3; iy++)
                    possibilities[ix, iy, val - 1] = false;
            }
        }

        private static bool SolveRow(byte[,] blocks, int x, int y, byte val)
        {
            for (int ix = 0; ix < 9; ix++)
            {
                if (x == ix) continue;
                if (blocks[ix, y] == val) return false;
            }
            return true;
        }

        private static bool SolveColumn(byte[,] blocks, int x, int y, byte val)
        {
            for (int iy = 0; iy < 9; iy++)
            {
                if (y == iy) continue;
                if (blocks[x, iy] == val) return false;
            }
            return true;
        }

        private static bool SolveCell(byte[,] blocks, int x, int y, byte val)
        {
            int startx = x - (x % 3);
            int starty = y - (y % 3);

            for (int ix = startx; ix < startx + 3; ix++)
            {
                for (int iy = starty; iy < starty + 3; iy++)
                {
                    if (y == iy && x == ix) continue;
                    if (blocks[ix, iy] == val) return false;
                }
            }
            return true;
        }
    }
}
