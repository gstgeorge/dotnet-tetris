/******************************************************************************
 *                                                                            *
 *  CSCI 473                     Assignment 05                     FALL 2021  *
 *                                                                            *
 *    Created by: Gavin St. George                                            *
 *                                                                            *
 *      Date Due: 2021-11-11                                                  *
 *                                                                            *
 *  Program Name: Tetris                                                      *
 *                                                                            *
 *         Notes:                                                             *
 *                                                                            *
 *****************************************************************************/

using System;

namespace Tetris
{
    class GamePiece
    {
        private int width;       // gamepiece width in blocks
        private int height;      // gamepiece height in blocks
        private int color;       // color as index in COLORS
        private int[,] shape;    // 2d array representing gamepiece's shape
        private int[,] oldShape; // holds gamepiece's shape prior to rotating

        // getters
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int Color { get { return color; } }
        public int[,] Blocks { get { return shape; } }

        // constructor
        public GamePiece()
        {
            int x = new Random().Next(1, 7);
            color = x;

            switch (x) // randomly select one of these gamepieces
            {
                case 1: // I
                    width = 1;
                    height = 4;
                    shape = new int[,]
                    {
                        { x },
                        { x },
                        { x },
                        { x }
                    };
                    break;

                case 2: // L
                    width = 2;
                    height = 3;
                    shape = new int[,]
                    {
                        { x, 0 },
                        { x, 0 },
                        { x, x }
                    };
                    break;

                case 3: // T
                    width = 3;
                    height = 2;
                    shape = new int[,]
                    {
                        { x, x, x },
                        { 0, x, 0 }
                    };
                    break;

                case 4: // cube
                    width = 2;
                    height = 2;
                    shape = new int[,]
                    {
                        { x, x },
                        { x, x }
                    };
                    break;

                case 5: // Z
                    width = 3;
                    height = 2;
                    shape = new int[,]
                    {
                        { 0, x, x },
                        { x, x, 0 }
                    };
                    break;

                case 6: // reverse z
                    width = 3;
                    height = 2;
                    shape = new int[,]
                    {
                        {x, x, 0 },
                        {0, x, x }
                    };
                    break;

                default: throw new Exception("Invalid gamepiece shape");
            }
        }

        // rotate gamepiece
        public void Rotate()
        {
            oldShape = shape;
            shape = new int[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    shape[i, j] = oldShape[(height - 1) - j, i];
                }
            }

            int x = width;
            width = height;
            height = x;
        }

        // restore gamepiece to orientation prior to rotating
        public void HandleIllegalRotate()
        {
            shape = oldShape;
            int x = width;
            width = height;
            height = x;
        }
    }
}
