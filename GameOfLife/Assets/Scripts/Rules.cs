using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GM
{
    public class Rules
    {
        bool[,] current, next;
        int sizeX;
        int sizeY;
        public bool[,] updateMatrix(bool[,] main, int x, int y)
        {
            sizeX = x;
            sizeY = y;
            current = new bool[x, y];
            next = new bool[x, y];
            for (int dx = 0; dx < x; dx++)
            {
                for (int dy = 0; dy < y; dy++)
                {
                    next[dx,dy] = false;
                    current[dx,dy] = main[dx,dy];
                }
            }
            // current = (bool[,]) main.Clone();
            // next = (bool[,]) main.Clone();
            // current = main;
            // next = main;
            calculateNext();
            return next;
        }

        private void calculateNext()
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    int cond = calculateAliveAround(x, y);
                    // RULES
                    // Rule No.1
                    // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                    if (cond < 2 && current[x, y])
                    {
                        next[x, y] = false;
                    }
                    // Rule No.2
                    // Any live cell with two or three live neighbours lives on to the next generation.
                    else if((cond == 2 || cond == 3) && current[x, y])
                    {
                        next[x, y] = true;
                    }
                    // Rule No.3
                    // Any live cell with more than three live neighbours dies, as if by overpopulation.
                    else if(cond > 3 && current[x, y])
                    {
                        next[x, y] = false;
                    }
                    // Rule No.4
                    // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                    else if(cond == 3 && !current[x, y])
                    {
                        next[x, y] = true;
                    }
                }
            }
        }

        private int calculateAliveAround(int x, int y)
        {
            int counter = 0;
            for (int xDev = -1; xDev < 2; xDev++)
            {
                for (int yDev = -1; yDev < 2; yDev++)
                {
                    if (xDev == 0 && yDev == 0)
                    {
                        continue;
                    }
                    else if (current[(sizeX + xDev + x) % sizeX, (sizeY + yDev + y) % sizeY])
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
    }
}
