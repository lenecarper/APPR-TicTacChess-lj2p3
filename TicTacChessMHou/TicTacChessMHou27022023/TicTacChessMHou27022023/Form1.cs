using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacChessMHou27022023
{
    public partial class Form1 : Form
    {
        // Check the selected piece color
        string selectedPieceColor = "";

        // The picturebox the piece is coming from and going to
        PictureBox pcbFrom = null;
        PictureBox pcbTo = null;

        // Variables to move the pieces
        List<Piece> pieceList = null;
        Piece activePiece = null;
        List<Board> boardList = null;
        Board activeBoard = null;
        string pieceOptions = "";
        int horizontal, vertical;
        PictureBox pcbForbidden;
        Board forbidden;

        // Player turns
        int onBoardCount = 0;
        bool gameStart = false;
        string turnColor = "";

        // Winner checking
        List<string> winList = null;
        string startingWhite = "";
        string startingBlack = "";

        // Arduino variables
        bool arduinoOn = false;
        bool moveBusy = false;
        string commando;
        Board oldBoard = null;
        Board newBoard = null;
        int moveArduinoCounter = 0;
        int baseDropVertical = 950;
        Form2 arduinoForm = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set selected piece color as white, allow picturebox dropping in the groupbox
            selectedPieceColor = "White";
            UpdatePieceColor();

            foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
            {
                item.AllowDrop = true;
            }

            // Setup/initialize game
            SetupGame();
        }

        // Function to run while the board has a MouseDown event
        private void pcbBoard_MouseDown(object sender, MouseEventArgs e)
        {
            pcbFrom = (PictureBox)sender;

            // Check whether the game has started or not
            if (gameStart == true)
            {
                if (pcbFrom.BackColor == Color.Transparent)
                    {
                    // Gets the previous piece location
                    horizontal = Convert.ToInt32(pcbFrom.Tag.ToString().Substring(0, 1));
                    vertical = Convert.ToInt32(pcbFrom.Tag.ToString().Substring(1, 1));

                    // Search the board for selected pieces
                    activeBoard = boardList.FirstOrDefault(x => x.GetHorizontal() == horizontal && x.GetVertical() == vertical);
                    oldBoard = activeBoard;
                    activePiece = activeBoard.GetPiece();

                    // Get board options, make sure pieces can't move on top- or over eachother
                    GetBoardOptions();
                    UpdateLocations();
                    CheckForIllegalMoves();

                    // Check if the receiving picturebox is not null and a piece can be dropped on the color
                    if (pcbFrom.Image != null && pcbFrom.BackColor == Color.Transparent)
                    {
                        pcbFrom.DoDragDrop(pcbFrom.Image, DragDropEffects.Copy);
                    }

                    // Loop through the board, if a box is checked as green, change it back to transparent
                    foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
                    {
                        if (item.BackColor == Color.Green)
                        {
                            item.BackColor = Color.Transparent;
                        }
                    }
                }
            }
        }

        private void GetStartingOptions()
        {
            // Declare which locations are available during the piece setup
            if (selectedPieceColor == "White")
            {
                pieceOptions = "132333";
            }
            else if (selectedPieceColor == "Black")
            {
                pieceOptions = "112131";
            }
            UpdateLocations();
        }

        // Change the selectable pieces depending on which color the user has checked
        private void UpdatePieceColor()
        {
            if (selectedPieceColor == "White")
            {
                pcbKnight.Image = Properties.Resources.Chess_Knight_White;
                pcbRook.Image = Properties.Resources.Chess_Rook_White;
                pcbQueen.Image = Properties.Resources.Chess_Queen_White;
                pcbKing.Image = Properties.Resources.Chess_King_White;
            }
            else if (selectedPieceColor == "Black")
            {
                pcbKnight.Image = Properties.Resources.Chess_Knight_Black;
                pcbRook.Image = Properties.Resources.Chess_Rook_Black;
                pcbQueen.Image = Properties.Resources.Chess_Queen_Black;
                pcbKing.Image = Properties.Resources.Chess_King_Black;
            }
        }

        private void UpdateLocations()
        {
            // Loop through location options until all pieces have been calculated, show available positions in green
            for (int i = 0; i < pieceOptions.Length; i += 2)
            {
                foreach (PictureBox pb in gbxBoard.Controls.OfType<PictureBox>())
                {
                    if (pb.Tag.ToString() == pieceOptions[i].ToString() + pieceOptions[i + 1].ToString() && pb.Image == null)
                    {
                        pb.BackColor = Color.Green;
                    }
                }
            }
        }

        // Clear the board of any residual colors
        private void ClearBoardColors()
        {
            foreach (PictureBox pb in gbxBoard.Controls.OfType<PictureBox>())
            {
                pb.BackColor = Color.Transparent;
            }
        }

        // Check piece move options before being able to place a piece on the board
        private void GetBoardOptions()
        {
            pieceOptions = "";
            foreach (Board item in boardList)
            {
                if (activePiece != null && item.GetPiece() == null)
                {
                    pieceOptions += activePiece.GetMoveOptions(horizontal, vertical, item.GetHorizontal(), item.GetVertical());
                }
            }
        }

        private void CheckForIllegalMoves()
        {
            // Gets the location of all default neighbours
            Board right = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical());
            Board left = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical());
            Board up = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() && x.GetVertical() == activeBoard.GetVertical() - 1);
            Board down = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() && x.GetVertical() == activeBoard.GetVertical() + 1);

            // If a piece is selected and is not null
            if (activePiece != null)
            {
                // Check for every piece whether they are allowed to be placed
                if (activePiece.GetName() == "Rook" || activePiece.GetName() == "Queen")
                {
                    CheckForNeighbour(right, "Right");
                    CheckForNeighbour(left, "Left");
                    CheckForNeighbour(up, "Up");
                    CheckForNeighbour(down, "Down");
                }
                if (activePiece.GetName() == "Queen")
                {
                    // Gets the location of all the diagonal neighbours 
                    Board upRight = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical() - 1);
                    Board upLeft = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical() - 1);
                    Board downRight = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical() + 1);
                    Board downLeft = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical() + 1);

                    CheckForNeighbour(upRight, "UpRight");
                    CheckForNeighbour(upLeft, "UpLeft");
                    CheckForNeighbour(downRight, "DownRight");
                    CheckForNeighbour(downLeft, "DownLeft");
                }
                if (activePiece.GetName() == "King")
                {
                    Board upRight = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical() - 1);
                    Board upLeft = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical() - 1);
                    Board downRight = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical() + 1);
                    Board downLeft = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical() + 1);

                    CheckForNeighbour(right, "Right");
                    CheckForNeighbour(left, "Left");
                    CheckForNeighbour(up, "Up");
                    CheckForNeighbour(down, "Down");
                    CheckForNeighbour(upRight, "UpRight");
                    CheckForNeighbour(upLeft, "UpLeft");
                    CheckForNeighbour(downRight, "DownRight");
                    CheckForNeighbour(downLeft, "DownLeft");

                }
            }
        }

        // Check the board for neighboring pieces, disallow dropping onto other pieces
        private void CheckForNeighbour(Board neighbour, string direction)
        {
            if (neighbour != null && neighbour.GetPiece() != null)
            {
                switch (direction)
                {
                    case "Left":
                        // Checks if there is a board on the left (-1) of the neighbour of the selected piece
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() - 1 && o.GetVertical() == neighbour.GetVertical());
                        break;
                    case "Right":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() + 1 && o.GetVertical() == neighbour.GetVertical());
                        break;
                    case "Up":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() && o.GetVertical() == neighbour.GetVertical() - 1);
                        break;
                    case "Down":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() && o.GetVertical() == neighbour.GetVertical() + 1);
                        break;
                    case "UpRight":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() + 1 && o.GetVertical() == neighbour.GetVertical() - 1);
                        break;
                    case "UpLeft":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() - 1 && o.GetVertical() == neighbour.GetVertical() - 1);
                        break;
                    case "DownRight":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() + 1 && o.GetVertical() == neighbour.GetVertical() + 1);
                        break;
                    case "DownLeft":
                        forbidden = boardList.FirstOrDefault(o => o.GetHorizontal() == neighbour.GetHorizontal() - 1 && o.GetVertical() == neighbour.GetVertical() + 1);
                        break;
                    default:
                        break;
                }
                // If the board move is forbidden, set it to transparent
                if (forbidden != null)
                {
                    pcbForbidden = (PictureBox)gbxBoard.Controls.Find(forbidden.GetPictureName(), false)[0];
                    pcbForbidden.BackColor = Color.Transparent;
                }
            }
        }

        // Select turn color when checked
        private void rdbWhite_CheckedChanged(object sender, EventArgs e)
        {
            selectedPieceColor = "White";
            UpdatePieceColor();
            UpdatePieceOnBoardColors();
        }

        // Select turn color when checked
        private void rdbBlack_CheckedChanged(object sender, EventArgs e)
        {
            selectedPieceColor = "Black";
            UpdatePieceColor();
            UpdatePieceOnBoardColors();
        }

        // Restart the game when the button is clicked
        private void btnRestart_Click(object sender, EventArgs e)
        {
            // Select white pieces, update (board) colors, allow dropping on the board and reset variables
            selectedPieceColor = "White";
            UpdatePieceColor();
            UpdatePieceOnBoardColors();
            rdbWhite.Checked = true;
            lblGamestate.Text = "Set up pieces";
            onBoardCount = 0;
            startingBlack = "";
            startingWhite = "";

            foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
            {
                item.AllowDrop = true;
                item.Image = null;
                item.BackColor = SystemColors.Control;
            }

            foreach (PictureBox pb in gbxPieces.Controls.OfType<PictureBox>())
            {
                pb.AllowDrop = true;
                pb.BackColor = Color.Transparent;
            }
            SetupGame();
        }

        // Function to run on every picturebox piece when the MouseDown is toggled
        private void pcbAllPieces_MouseDown(object sender, MouseEventArgs e)
        {
            // Set the board as inactive, check available setup locations
            activeBoard = null;
            pcbFrom = (PictureBox)sender;
            if (pcbFrom.BackColor == Color.Transparent)
            {
                // Set the active piece as the current piece that's being held, copy the active piece to the new box
                foreach (Piece item in pieceList)
                {
                    if (item.GetBasePictureboxName() == pcbFrom.Name && item.GetColor() == selectedPieceColor)
                    {
                        activePiece = item;
                    }
                }
                GetStartingOptions();

                pcbFrom.DoDragDrop(pcbFrom.Image, DragDropEffects.Copy);
            }
        }

        // Function to run when the DragDrop event is toggled
        private void pcbBoard_DragDrop(object sender, DragEventArgs e)
        {
            // Gets the currently held piece as an image and copies it to the designated picturebox
            pcbTo = (PictureBox)sender;
            Image getPicture = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
            pcbTo.Image = getPicture;

            // Gets the location of the picturebox to put the piece onto
            horizontal = Convert.ToInt32(pcbTo.Tag.ToString().Substring(0, 1));
            vertical = Convert.ToInt32(pcbTo.Tag.ToString().Substring(1, 1));

            // If there is an active board
            if (activeBoard != null)
            {
                // Set pieces to null, reset variables, check board list
                activeBoard.SetPiece(null);
                activeBoard = boardList.FirstOrDefault(x => x.GetHorizontal() == horizontal && x.GetVertical() == vertical);
                activePiece.SetCurrentPicturebox(pcbTo.Name);
                activeBoard.SetPiece(activePiece);
                pcbFrom.Image = null;
                // If the Arduino checkbox is checked, reset moves to 0, set the new board as the active board, enable Arduino timer
                if (arduinoOn)
                {
                    moveArduinoCounter = 0;
                    newBoard = activeBoard;
                    tmrArduino.Enabled = true;
                }

                // Check turn color and check the winner
                if (turnColor == "White")
                {
                    turnColor = "Black";
                    lblGamestate.Text = "Black's turn";
                }
                else
                {
                    turnColor = "White";
                    lblGamestate.Text = "White's turn";
                }
                CheckWinner();
            }
            else
            {
                // Set active piece if above function gets passed
                activePiece.SetCurrentPicturebox(pcbTo.Name);
                boardList.FirstOrDefault(x => x.GetHorizontal() == horizontal && x.GetVertical() == vertical).SetPiece(activePiece);

                // Increment pieces on board counter
                onBoardCount++;
                activePiece.SetOnBoard(true);
                UpdateStartingPositions();
            }
            // Clear all board colors
            ClearBoardColors();
            UpdatePieceOnBoardColors();

            // If 6 pieces have been placed on the board, start the game and set the turn color to 'White'
            if (onBoardCount == 6)
            {
                onBoardCount++;
                gameStart = true;
                turnColor = "White";
            }
            UpdateAllBoardcolors();
        }

        // Function to run when the DragOver event is toggled
        private void pcbBoard_DragOver(object sender, DragEventArgs e)
        {
            pcbTo = (PictureBox)sender;

            // Check if the picturebox is allowed to be dragged over
            if (pcbTo.BackColor == Color.Green)
            {
                // Place the picturebox
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Pass
                e.Effect = DragDropEffects.None;
            }
        }

        // Setup the pieces and board
        private void SetupGame()
        {
            // Pieces list, declare all available pieces & colors
            pieceList = new List<Piece>();
            pieceList.Add(new Piece("Rook", "White"));
            pieceList.Add(new Piece("Knight", "White"));
            pieceList.Add(new Piece("Queen", "White"));
            pieceList.Add(new Piece("King", "White"));
            pieceList.Add(new Piece("Rook", "Black"));
            pieceList.Add(new Piece("Knight", "Black"));
            pieceList.Add(new Piece("Queen", "Black"));
            pieceList.Add(new Piece("King", "Black"));

            // Board list, declare all available locations by referencing the names & tags
            // in the form of Board(1, 1, "pcbOne"), where pcbOne is the PictureBox name, and the (1, 1)
            // the horizontal and vertical axis in chronological order

            // Add in Arduino coordinate units after the picturebox name (320, 20, 1150 ex.)
            boardList = new List<Board>();
            boardList.Add(new Board(1, 1, "pcbOne", 320, 20));
            boardList.Add(new Board(2, 1, "pcbTwo", 400, 135));
            boardList.Add(new Board(3, 1, "pcbThree", 570, 245));
            boardList.Add(new Board(1, 2, "pcbFour", 850, 0));
            boardList.Add(new Board(2, 2, "pcbFive", 900, 110));
            boardList.Add(new Board(3, 2, "pcbSix", 1050, 200));
            boardList.Add(new Board(1, 3, "pcbSeven", 1330, 0));
            boardList.Add(new Board(2, 3, "pcbEight", 1400, 95));
            boardList.Add(new Board(3, 3, "pcbNine", 1520, 175));
            
            // Declare all possible locations for the player to win from
            winList = new List<string>();
            winList.Add("012");
            winList.Add("345");
            winList.Add("678");
            winList.Add("036");
            winList.Add("147");
            winList.Add("258");
            winList.Add("246");
            winList.Add("048");
        }

        // Check for a game winner
        public void CheckWinner()
        {
            string boardOne = "";
            string boardTwo = "";
            string boardThree = "";
            int locOne, locTwo, locThree;
            // Loop through all possible win locations
            foreach (string item in winList)
            {
                locOne = Convert.ToInt32(item.Substring(0, 1));
                locTwo = Convert.ToInt32(item.Substring(1, 1));
                locThree = Convert.ToInt32(item.Substring(2, 1));

                if (boardList[locOne].GetPiece() != null && boardList[locTwo].GetPiece() != null && boardList[locThree].GetPiece() != null)
                {
                    boardOne = boardList[Convert.ToInt32(item.Substring(0, 1))].GetPiece().GetColor();
                    boardTwo = boardList[Convert.ToInt32(item.Substring(1, 1))].GetPiece().GetColor();
                    boardThree = boardList[Convert.ToInt32(item.Substring(2, 1))].GetPiece().GetColor();

                    string endPositions = $"{locOne}{locTwo}{locThree}";
                    // Checks the winning position pieces
                    if (endPositions == item)
                    {
                        if (boardOne == boardTwo && boardTwo == boardThree && boardOne != "")
                        {
                            if (boardOne == "White")
                            {
                                if (endPositions != startingWhite)
                                {
                                    MessageBox.Show("White wins");
                                    lblGamestate.Text = "White wins. Click restart to play again.";
                                    SetupGame();
                                    // onBoardCount = 0;
                                }
                            }
                            else
                            {
                                if (endPositions != startingBlack)
                                {
                                    MessageBox.Show("Black wins");
                                    lblGamestate.Text = "Black wins. Click restart to play again.";
                                    SetupGame();
                                    // onBoardCount = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Check for allowed starting positions per color
        private void UpdateStartingPositions()
        {
            if (activePiece.GetColor() == "White")
            {
                startingWhite = GetStartingNumber(startingWhite);
            }
            else
            {
                startingBlack = GetStartingNumber(startingBlack);
            }

            if (startingBlack.Length == 3 && startingWhite.Length == 3)
            {
                lblGamestate.Text = "Game started, white begins";

                gameStart = true;
                UpdateAllBoardcolors();
            }
        }

        // Update colors based on whether the piece has been set on the board or not
        private void UpdatePieceOnBoardColors()
        {
            foreach (Piece item in pieceList)
            {
                // Loop through the groupbox and chheck whether the picturebox has been set or not
                foreach (PictureBox picturebox in gbxPieces.Controls.OfType<PictureBox>())
                {
                    // If the selected picturebox has been placed, set it to red and disable drag/dropping
                    if (item.GetBasePictureboxName() == picturebox.Name && item.GetColor() == selectedPieceColor)
                    {
                        if (item.GetIsOnBoard())
                        {
                            picturebox.BackColor = Color.Red;
                        }
                        else
                        {
                            picturebox.BackColor = Color.Transparent;
                        }
                    }
                }
            }
        }

        // Check whether the player's turn color is the same as the pieces
        private void UpdateAllBoardcolors()
        {
            // Loop through the board and check which color's turn it is
            foreach (PictureBox pb in gbxBoard.Controls.OfType<PictureBox>())
            {
                Board b = boardList.FirstOrDefault(x => x.GetPictureName() == pb.Name);
                if (b.GetPiece() != null)
                {
                    // Make the background transparent if it's their turn, otherwise make it gray and disable drag/dropping
                    if (b.GetPiece().GetColor() == turnColor)
                    {
                        pb.BackColor = Color.Transparent;
                    }
                    else
                    {
                        pb.BackColor = Color.Gray;
                    }
                }
            }
        }

        // Confirm whether the checkbox has been checked
        private void ckxArduino_CheckedChanged(object sender, EventArgs e)
        {
            // If the checkbox has been checked, show Form2 (the Arduino form) and update the Gamestate label
            arduinoOn = ckxArduino.Checked;
            if (arduinoOn)
            {
                arduinoForm = new Form2(this);
                arduinoForm.Show();
                lblGamestate.Text = "Arduino is running commands";
            }
            else
            {
                lblGamestate.Text = "Resuming game..";
                arduinoForm.Close();
            }
        }

        // Run these Arduino commands in a row as long as the timer is active
        private void tmrArduino_Tick(object sender, EventArgs e)
        {
            lblGamestate.Text = "Arduino is running commands";
            if (moveArduinoCounter == 0)
            {
                commando = $"RS:{oldBoard.GetArduinoRot()}";
            }
            else if (moveArduinoCounter == 1)
            {
                commando = $"HS:{oldBoard.GetArduinoHor()}";
            }
            else if (moveArduinoCounter == 2)
            {
                commando = $"VS:{oldBoard.GetArduinoVer()}";
            }
            else if (moveArduinoCounter == 3)
            {
                commando = $"CS:1";
            }
            else if (moveArduinoCounter == 4)
            {
                commando = $"SS:1";
            }
            else if (moveArduinoCounter == 5)
            {
                commando = $"VS:{baseDropVertical}";
            }
            else if (moveArduinoCounter == 6)
            {
                commando = $"RS:{newBoard.GetArduinoRot()}";
            }
            else if (moveArduinoCounter == 7)
            {
                commando = $"HS:{newBoard.GetArduinoHor()}";
            }
            else if (moveArduinoCounter == 8)
            {
                commando = $"VS:{baseDropVertical}";
            }
            else if (moveArduinoCounter == 9)
            {
                commando = $"SS:0";
            }
            else if (moveArduinoCounter == 10)
            {
                commando = $"CS:0";
            }
            else if (moveArduinoCounter == 11)
            {
                commando = $"ZS:3";
            }
            else if (moveArduinoCounter == 12)
            {
                commando = $"ZS:2";
            }
            else if (moveArduinoCounter == 13)
            {
                commando = $"ZS:1";
            }
            else if (moveArduinoCounter == 14)
            {
                // Disable timer, start the game again
                tmrArduino.Enabled = false;
                gameStart = true;
            }

            // If the Arduino is not busy yet, activate it and send it a command
            if (moveBusy == false)
            {
                moveBusy = true;
                arduinoForm.WriteArduino(commando);
            }
            else
            {
                lblGamestate.Text = "Arduino is busy.";
            }
        }

        // Configuration for the Arduino coordination (zero before using)
        public string GetStartingNumber(string currentStart)
        {
            int newNumber = boardList.IndexOf(boardList.FirstOrDefault(f => f.GetPictureName() == pcbTo.Name));

            if (currentStart == "")
            {
                currentStart += newNumber;
            }
            else
            {
                if (newNumber.ToString().Length == 1)
                {
                    if (newNumber > Convert.ToInt32(currentStart.Substring(0, 1)))
                    {
                        currentStart += newNumber;
                    }
                    else
                    {
                        currentStart = newNumber + currentStart;
                    }
                }
                else if (newNumber.ToString().Length == 2)
                {
                    if (newNumber > Convert.ToInt32(currentStart.Substring(0, 1)))
                    {
                        if (newNumber > Convert.ToInt32(currentStart.Substring(1, 1)))
                        {
                            currentStart += newNumber;
                        }
                        else
                        {
                            currentStart = currentStart.Substring(0, 1) + newNumber + currentStart.Substring(1, 1);
                        }
                    }
                    else
                    {
                        currentStart = newNumber + currentStart;
                    }
                }
            }
            return currentStart;
        }
        
        // Increment the Arduino command counter, disable while it's not running commands
        public void NextArduinoStep()
        {
            // Increment the Arduino counter to allow for a new command to run
            moveArduinoCounter++;
            moveBusy = false;
        }
    }
}
