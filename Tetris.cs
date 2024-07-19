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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Media;
using System.Threading;

namespace Tetris
{
    public partial class Tetris : Form
    {
        private readonly bool DEBUG = false;

        private readonly Color[] COLORS =
        {
            Color.Black,                  // 0: background -> black
            Color.FromArgb(231, 255, 0),  // 1: I          -> yellow
            Color.FromArgb(95, 232, 255), // 2: L          -> blue
            Color.FromArgb(100, 255, 0),  // 3: T          -> green
            Color.FromArgb(255, 115, 0),  // 4: cube       -> orange
            Color.FromArgb(252, 73, 171), // 5: Z          -> pink
            Color.FromArgb(197, 35, 235), // 6: reverse Z  -> purple
            Color.White                   // 7: clear row  -> white
        };
        private readonly Font TITLEFONT = new Font("Courier New", 26F, FontStyle.Bold);
        private readonly Font TEXTFONT = new Font("Courier New", 16F, FontStyle.Bold);

        private readonly int BOARD_WIDTH = 10;
        private readonly int BOARD_HEIGHT = 20;
        private readonly int BLOCK_SIZE = 30;
        private readonly int CONTROL_SPACING = 30;
        private readonly int SCORE_PER_ROW = 1000;

        private int[,] board;
        private PictureBox canvas;
        private PictureBox previewCanvas;
        private Bitmap gameBoard;
        private Bitmap activePieceOnBoard;
        private Bitmap previewNextGamePiece;
        private GamePiece activeGamePiece;
        private GamePiece nextGamePiece;
        private System.Windows.Forms.Timer gameSpeed;
        private System.Windows.Forms.Timer elapsedTime;
        private SoundPlayer music;
        private Label labelPause1;
        private Label labelPause2;
        private Label labelPause3;
        private Label labelElapsedTime;
        private Label labelScore;
        private Label labelLevel;
        private Label labelLines;
        private Label labelHiScore;

        private bool pause;
        private DateTime displayTime;
        private int score; // max 9,999,999
        private int hiscore;
        private int level;
        private int rowsCleared;
        private int xPos;
        private int yPos;

        public Tetris()
        {
            InitializeComponent();

            // read high score from file
            if (File.Exists("hiscore.txt"))
            {
                using (StreamReader sr = new StreamReader("hiscore.txt"))
                {
                    hiscore = Convert.ToInt32(sr.ReadLine());
                }
            }

            initGameBoard();
            newGame();
        }

        // initialize the gameboard
        private void initGameBoard()
        {
            Text = "Tetris";
            Icon = Properties.Resources.tetris;
            AutoSize = true;
            Padding = new Padding(CONTROL_SPACING, CONTROL_SPACING, CONTROL_SPACING, CONTROL_SPACING);

            // add gameboard to form
            canvas = new PictureBox();
            canvas.Location = new Point(CONTROL_SPACING, CONTROL_SPACING);
            canvas.Width = BOARD_WIDTH * BLOCK_SIZE;
            canvas.Height = BOARD_HEIGHT * BLOCK_SIZE;
            Controls.Add(canvas);

            // add title
            labelPause1 = new Label();
            labelPause1.Location = new Point(canvas.Left, canvas.Top);
            labelPause1.Size = new Size(canvas.Width, 100);
            labelPause1.TextAlign = ContentAlignment.MiddleCenter;
            labelPause1.Font = TITLEFONT;
            labelPause1.Text = "Tetris";
            labelPause1.BackColor = COLORS[0];
            labelPause1.ForeColor = COLORS[7];
            Controls.Add(labelPause1);

            // add controls description
            labelPause2 = new Label();
            labelPause2.Location = new Point(canvas.Left, labelPause1.Bottom);
            labelPause2.Size = new Size(canvas.Width, canvas.Height - labelPause1.Height - 50);
            labelPause2.TextAlign = ContentAlignment.MiddleCenter;
            labelPause2.Font = TEXTFONT;
            labelPause2.Text = "CONTROLS\n\nLEFT      🠔 or A\nRIGHT     🠖 or D\nDOWN      🠗 or S\nROTATE    🠕 or W\n\nPAUSE   SPACEBAR";
            labelPause2.BackColor = COLORS[0];
            labelPause2.ForeColor = COLORS[7];
            Controls.Add(labelPause2);

            // add press space to continue label
            labelPause3 = new Label();
            labelPause3.Location = new Point(canvas.Left, labelPause2.Bottom);
            labelPause3.Size = new Size(canvas.Width, 50);
            labelPause3.TextAlign = ContentAlignment.MiddleCenter;
            labelPause3.Font = TEXTFONT;
            labelPause3.Text = "Press SPACE to play";
            labelPause3.BackColor = COLORS[0];
            labelPause3.ForeColor = COLORS[7];
            Controls.Add(labelPause3);

            // add preview picturebox to form
            previewCanvas = new PictureBox();
            previewCanvas.Size = new Size(BLOCK_SIZE * 6, BLOCK_SIZE * 6);
            previewCanvas.Location = new Point(canvas.Right + CONTROL_SPACING, canvas.Bottom - previewCanvas.Height);
            Controls.Add(previewCanvas);

            // add preview label to form
            Label labelNext = new Label();
            labelNext.Size = new Size(previewCanvas.Width, 25);
            labelNext.Location = new Point(previewCanvas.Left, previewCanvas.Top - labelNext.Height);
            labelNext.TextAlign = ContentAlignment.BottomCenter;
            labelNext.Font = TEXTFONT;
            labelNext.Text = "UP NEXT";
            labelNext.BackColor = COLORS[0];
            labelNext.ForeColor = COLORS[7];
            Controls.Add(labelNext);

            // add score counter to form
            labelElapsedTime = new Label();
            labelElapsedTime.Location = new Point(previewCanvas.Left, canvas.Top);
            labelElapsedTime.Size = new Size(previewCanvas.Width, 50);
            labelElapsedTime.TextAlign = ContentAlignment.TopCenter;
            labelElapsedTime.Font = TEXTFONT;
            labelElapsedTime.BackColor = COLORS[0];
            labelElapsedTime.ForeColor = COLORS[7];
            Controls.Add(labelElapsedTime);

            // add score counter to form
            labelScore = new Label();
            labelScore.Location = new Point(previewCanvas.Left, labelElapsedTime.Bottom + CONTROL_SPACING);
            labelScore.Size = new Size(previewCanvas.Width, 50);
            labelScore.TextAlign = ContentAlignment.TopCenter;
            labelScore.Font = TEXTFONT;
            labelScore.BackColor = COLORS[0];
            labelScore.ForeColor = COLORS[7];
            Controls.Add(labelScore);

            // add level counter to form
            labelLevel = new Label();
            labelLevel.Location = new Point(previewCanvas.Left, labelScore.Bottom + CONTROL_SPACING);
            labelLevel.Size = new Size(previewCanvas.Width, 50);
            labelLevel.TextAlign = ContentAlignment.TopCenter;
            labelLevel.Font = TEXTFONT;
            labelLevel.BackColor = COLORS[0];
            labelLevel.ForeColor = COLORS[7];
            Controls.Add(labelLevel);

            // add line counter to form
            labelLines = new Label();
            labelLines.Location = new Point(previewCanvas.Left, labelLevel.Bottom + CONTROL_SPACING);
            labelLines.Size = new Size(previewCanvas.Width, 50);
            labelLines.TextAlign = ContentAlignment.TopCenter;
            labelLines.Font = TEXTFONT;
            labelLines.BackColor = COLORS[0];
            labelLines.ForeColor = COLORS[7];
            Controls.Add(labelLines);

            // add highscore to form
            labelHiScore = new Label();
            labelHiScore.Location = new Point(previewCanvas.Left, labelLines.Bottom + CONTROL_SPACING);
            labelHiScore.Size = new Size(previewCanvas.Width, 50);
            labelHiScore.TextAlign = ContentAlignment.TopCenter;
            labelHiScore.Font = TEXTFONT;
            labelHiScore.BackColor = COLORS[0];
            labelHiScore.ForeColor = COLORS[7];
            Controls.Add(labelHiScore);

            // resize form
            Width = previewCanvas.Right + CONTROL_SPACING;
            Height = canvas.Bottom + CONTROL_SPACING;

            // game controls
            KeyDown += Tetris_KeyDown;
        }

        // reset counters, clear board, and start a new game
        private void newGame()
        {
            pause = true;
            score = 0;
            rowsCleared = 0;
            level = 1;
            displayTime = new DateTime();
            board = new int[BOARD_WIDTH, BOARD_HEIGHT];

            // clear gameboard display
            gameBoard = new Bitmap(canvas.Width, canvas.Height);
            Graphics g = Graphics.FromImage(gameBoard);
            using (Brush b = new SolidBrush(COLORS[0]))
            {
                g.FillRectangle(b, 0, 0, gameBoard.Width, gameBoard.Height);
            }
            canvas.Image = gameBoard;

            // generate first gamepiece
            activeGamePiece = spawnGamePiece();
            getNextGamePiece();

            // initialize elapsed time timer
            elapsedTime = new System.Windows.Forms.Timer();
            elapsedTime.Interval = 1000;
            elapsedTime.Tick += new EventHandler(ElapsedTime_Tick);

            // initialize gamespeed timer
            gameSpeed = new System.Windows.Forms.Timer();
            gameSpeed.Tick += GameSpeed_Tick;
            gameSpeed.Interval = 1000;

            // initialize counters
            labelElapsedTime.Text = String.Format("TIME\n{0:00}:{1:00}", displayTime.Minute, displayTime.Second);
            labelScore.Text = String.Format("SCORE\n{0:#,0}", score);
            labelLevel.Text = String.Format("LEVEL\n{0}", level);
            labelLines.Text = String.Format("LINES\n{0}", rowsCleared);
            labelHiScore.Text = String.Format("HIGH SCORE\n{0:#,0}", hiscore);

            // show pause menu
            canvas.Visible = false;
            labelPause1.Visible = true;
            labelPause2.Visible = true;
            labelPause3.Visible = true;
        }

        // spawn a new gamepiece above the gameboard
        private GamePiece spawnGamePiece()
        {
            GamePiece gp = new GamePiece();
            xPos = (BOARD_WIDTH / 2) - (gp.Width / 2);
            yPos = gp.Height * -1;
            return gp;
        }

        // gets the next gamepiece and renders a preview of it
        private void getNextGamePiece()
        {
            nextGamePiece = spawnGamePiece(); // create a new gamepiece

            // create a bitmap to display a preview of the next gamepiece
            previewNextGamePiece = new Bitmap(BLOCK_SIZE * 6, BLOCK_SIZE * 6);
            Graphics g = Graphics.FromImage(previewNextGamePiece);
            using (Brush b = new SolidBrush(COLORS[0]))
            {
                g.FillRectangle(Brushes.Black, 0, 0, previewNextGamePiece.Width, previewNextGamePiece.Height);
            }

            // get location of gamepiece in preview image
            int x = (6 - nextGamePiece.Width) / 2;
            int y = (6 - nextGamePiece.Height) / 2;

            // render a preview of the next gamepiece
            for (int i = 0; i < nextGamePiece.Height; i++) // for each row in the preview
            {
                for (int j = 0; j < nextGamePiece.Width; j++) // for each column in the row
                {
                    if (nextGamePiece.Blocks[i, j] > 0) // if coordinates contains a block, render it
                    {
                        using (Brush b = new SolidBrush(COLORS[nextGamePiece.Color]))
                        {
                            g.FillRectangle(b, (x + j) * BLOCK_SIZE, (y + i) * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
                        }
                    }
                }
            }

            // update preview image
            previewCanvas.Image = previewNextGamePiece;
        }

        // moves the active gamepiece
        // returns true if move was successful
        // returns false if destination coordinates are not on the board
        // returns false if destination coordinates overlap with an existing block
        private bool moveGamePiece(int x, int y)
        {
            // find destination coordinates
            int newXPos = xPos + x;
            int newYPos = yPos + y;

            // verify gamepiece is not trying to move out of bounds
            if (newXPos < 0 || newXPos + activeGamePiece.Width > BOARD_WIDTH || newYPos + activeGamePiece.Height > BOARD_HEIGHT)
            {
                return false;
            }

            // check if gamepiece is touching any other blocks 
            for (int i = 0; i < activeGamePiece.Width; i++) // for each column in the gamepiece
            {
                for (int j = 0; j < activeGamePiece.Height; j++) // for each row in the column
                {
                    // if gamepiece is on the board, and overlapping an existing block, return false
                    if (newYPos + j > 0 && board[newXPos + i, newYPos + j] > 0 && activeGamePiece.Blocks[j, i] > 0)
                    {
                        return false;
                    }
                }
            }

            // otherwise move is valid, update x/y position
            xPos = newXPos;
            yPos = newYPos;

            // render gamepiece
            activePieceOnBoard = new Bitmap(gameBoard);
            Graphics g = Graphics.FromImage(activePieceOnBoard);
            using (Brush b = new SolidBrush(COLORS[activeGamePiece.Color]))
            {
                for (int i = 0; i < activeGamePiece.Width; i++) // for each column in the gamepiece
                {
                    for (int j = 0; j < activeGamePiece.Height; j++) // for each row in the column
                    {
                        // if block exists in that location, render it
                        if (activeGamePiece.Blocks[j, i] > 0)
                        {
                            g.FillRectangle(b, (xPos + i) * BLOCK_SIZE, (yPos + j) * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
                        }
                    }
                }
            }

            // update image and return true
            canvas.Image = activePieceOnBoard;
            return true;
        }

        // if a row is full, turn it white and call clearFullRows
        public void findFullRows()
        {
            List<int> rowsToClear = new List<int>();
            gameSpeed.Stop();
            elapsedTime.Stop();

            // search for a full row
            for (int i = 0; i < BOARD_HEIGHT; i++) // for each row
            {
                int j;
                for (j = BOARD_WIDTH - 1; j >= 0; j--) // for each block in the row
                {
                    if (board[j, i] == 0) // if empty space is found, move on to next row
                        break;
                }

                // if row is full
                if (j == -1)
                {
                    rowsToClear.Add(i); // add row index to list of rows to clear

                    // change color of that row to white
                    for (j = 0; j < BOARD_WIDTH; j++)
                    {
                        board[j, i] = 7;
                    }
                }
            }

            renderGameBoard();
            
            if (rowsToClear.Count > 0)
            {
                clearFullRows(rowsToClear);
            }
            
            gameSpeed.Start();
            elapsedTime.Start();
        }

        // removes full rows
        private void clearFullRows(List<int> rowsToClear)
        {
            canvas.Refresh(); // update canvas to display cleared row as white
            Thread.Sleep(200);
            
            // remove full rows
            foreach (int i in rowsToClear)
            {
                // shift all rows above the full row down 1
                for (int j = 0; j < BOARD_WIDTH; j++)
                {
                    for (int k = i; k > 0; k--)
                    {
                        board[j, k] = board[j, k - 1];
                    }

                    // clear top row
                    board[j, 0] = 0;
                }

                // increment rows cleared, update counters, and render board
                rowsCleared = rowsCleared + 1 <= 999 ? rowsCleared + 1 : 999; // max rows cleared 999
                updateScore(SCORE_PER_ROW);
                renderGameBoard();
            }
        }

        // renders gamepieces on the game board
        private void renderGameBoard()
        {
            gameBoard = new Bitmap(gameBoard);
            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                for (int j = 0; j < BOARD_HEIGHT; j++)
                {
                    Graphics g = Graphics.FromImage(gameBoard);
                    using (Brush b = new SolidBrush(COLORS[board[i, j]]))
                    {
                        g.FillRectangle(b, i * BLOCK_SIZE, j * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
                    }
                }
            }
            canvas.Image = gameBoard;
        }

        // update the score and level counters
        private void updateScore(int addPoints = 0)
        {
            // if points are being added, update score and lines cleared counters
            if (addPoints > 0)
            {
                score = score + addPoints <= 9999999 ? score + addPoints : 9999999; // max score 9,999,999
                labelLines.Text = String.Format("LINES\n{0}", rowsCleared);
                labelScore.Text = String.Format("SCORE\n{0:#,0}", score);
            }

            // adjust level in relation to play time/score
            int totalSeconds = (displayTime.Minute * 60) + displayTime.Second;
            if (level < (score / 10000) + 1 || level < (totalSeconds / 20) + 1)
            {
                // if game speed is slower than max, increase speed
                if (gameSpeed.Interval > 200)
                {
                    gameSpeed.Interval -= 100;
                }

                // increment level and counter
                level = level + 1 <= 999 ? level + 1 : 999; // max level 999
                labelLevel.Text = String.Format("LEVEL\n{0}", level);
            }
        }

        // handles game over
        // stops timers, writes out high score if its been reached, prompts user to play again
        private void gameOver()
        {
            // stop music
            music.Stop();

            // stop/dispose timers
            gameSpeed.Stop();
            gameSpeed.Dispose();
            elapsedTime.Stop();
            elapsedTime.Dispose();

            // display game over dialog
            using (GameOver db = new GameOver(displayTime, score, hiscore, rowsCleared))
            {
                DialogResult dr = db.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    hiscore = score > hiscore ? score : hiscore;
                    newGame();
                }
                else Application.Exit();
            }
        }

        // pauses/unpauses the current game
        private void handlePause()
        {
            if (pause) // if game is paused, unpause it
            {
                pause = false;

                // start music
                music = new SoundPlayer(Properties.Resources.gamemusic);
                music.PlayLooping();

                // show board
                labelPause1.Visible = false;
                labelPause2.Visible = false;
                labelPause3.Visible = false;
                canvas.Visible = true;

                // start timers
                elapsedTime.Start();
                gameSpeed.Start();
            }

            else // pause the game
            {
                // stop timers
                gameSpeed.Stop();
                elapsedTime.Stop();

                // hide board
                canvas.Visible = false;
                labelPause1.Visible = true;
                labelPause2.Visible = true;
                labelPause3.Visible = true;

                // stop music
                music.Stop();

                pause = true;
            }
        }

        // update the elapsed time counter
        private void ElapsedTime_Tick(object source, EventArgs e)
        {
            displayTime = displayTime.AddSeconds(1); // increment display time
            labelElapsedTime.Text = String.Format("TIME\n{0:00}:{1:00}", displayTime.Minute, displayTime.Second);

            // max game time = 1 hour
            if (displayTime.Minute == 59 && displayTime.Second == 59)
            {
                gameOver();
            }

            // update score/level every 20 seconds
            if (displayTime.Second % 20 == 0)
            {
                updateScore();
            }
        }

        // move the active gamepiece downward until it can move no further
        // if game is not over, record that gamepiece's final location
        private void GameSpeed_Tick(object source, EventArgs e)
        {
            // attempt to move gamepiece downward, if it can't move any further then update the game board
            if (!moveGamePiece(0, 1))
            {
                // for each block in the active gamepiece
                for (int i = 0; i < activeGamePiece.Width; i++)
                {
                    for (int j = 0; j < activeGamePiece.Height; j++)
                    {
                        if (activeGamePiece.Blocks[j, i] > 0) // if block exists here
                        {
                            if (yPos < 0) // if block exceeds game board's vertical bound
                            {
                                gameOver();
                                return;
                            }

                            // otherwise game is not over, record active piece's final location on the game board
                            else board[xPos + i, yPos + j] = activeGamePiece.Color;
                        }
                    }
                }

                // add points for most recent piece, and get next piece
                updateScore(100);
                activeGamePiece = nextGamePiece;
                getNextGamePiece();
                findFullRows();
            }
        }

        // game controls
        private void Tetris_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left: // move gamepiece left
                case Keys.A:
                    moveGamePiece(-1, 0);
                    break;

                case Keys.Right: // move gamepiece right
                case Keys.D:
                    moveGamePiece(1, 0);
                    break;

                case Keys.Down: // move gamepiece down
                case Keys.S:
                    moveGamePiece(0, 1);
                    break;

                case Keys.Up: // rotate gamepiece
                case Keys.W:
                    activeGamePiece.Rotate();
                    if (!moveGamePiece(0, 0)) // undo if piece cannot be rotated that way
                    {
                        activeGamePiece.HandleIllegalRotate();
                    }
                    break;

                case Keys.Space: // pause game
                    handlePause();
                    break;

                case Keys.X: // add points to score for debugging
                    if (DEBUG)
                    {
                        updateScore(1000);
                    }
                    break;

                case Keys.C: // add time to elapsed time for debugging
                    if (DEBUG)
                    {
                        displayTime = displayTime.AddSeconds(20);
                        labelElapsedTime.Text = String.Format("TIME\n{0:00}:{1:00}", displayTime.Minute, displayTime.Second);
                        updateScore();
                    }
                    break;

                default: // some other key was pressed
                    return;
            }
        }
    }
}
