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
using System.IO;
using System.Media;

namespace Tetris
{
    public partial class GameOver : Form
    {
        SoundPlayer music;

        public GameOver(DateTime timePlayed, int score, int hiscore, int rowsCleared)
        {
            InitializeComponent();

            Icon = Properties.Resources.tetris;

            if (score > hiscore) // if high score has been achieved
            {
                head.Text = "HIGH SCORE";

                // write out new hiscore and play winner music
                using (StreamWriter sw = new StreamWriter("hiscore.txt"))
                {
                    sw.Write(score);
                }

                music = new SoundPlayer(Properties.Resources.hiscore);
                music.Play();
            }

            else // high score has not been reached, play loser music
            {
                music = new SoundPlayer(Properties.Resources.gameover);
                music.Play();
            }

            // display game stats
            body.Text = String.Format("{0,-12}         {1:00}:{2:00}\n\n{3,-12}     {4,9:#,0}\n\n{5,-12}     {6,9}",
                                      "TIME PLAYED", timePlayed.Minute, timePlayed.Second,
                                      "SCORE", score,
                                      "ROWS CLEARED", rowsCleared);
        }

        // user clicks button to play again
        private void buttonPlayAgain_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Dispose();
        }

        // user clicks button to exit
        private void buttonExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Dispose();
        }

        // stop music on form closing
        private void GameOver_FormClosing(object sender, FormClosingEventArgs e)
        {
            music.Stop();
        }
    }
}
