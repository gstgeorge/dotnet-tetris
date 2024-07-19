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
using System.Windows.Forms;

namespace Tetris
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Tetris());
        }
    }
}
