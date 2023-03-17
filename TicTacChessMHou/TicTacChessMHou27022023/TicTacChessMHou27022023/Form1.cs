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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            selectedPieceColor = "White";
            UpdatePieceColor();

            foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
            {
                item.AllowDrop = true;
            }

            SetupGame();
        }

        private void pcbBoard_MouseDown(object sender, MouseEventArgs e)
        {
            pcbFrom = (PictureBox)sender;

            // Gets the previous piece location
            horizontal = Convert.ToInt32(pcbFrom.Tag.ToString().Substring(0, 1));
            vertical = Convert.ToInt32(pcbFrom.Tag.ToString().Substring(1, 1));

            // Search the board for selected pieces
            activeBoard = boardList.FirstOrDefault(x => x.GetHorizontal() == horizontal && x.GetVertical() == vertical);
            activePiece = activeBoard.GetPiece();

            GetBoardOptions();
            UpdateLocations();
            CheckForIllegalMoves();

            pcbFrom.DoDragDrop(pcbFrom.Image, DragDropEffects.Copy);
        }

        private void CheckMoves()
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

        private void UpdatePieceColor()
        {
            if (selectedPieceColor == "White")
            {
                pcbKnight.Image = Properties.Resources.Chess_Knight_White;
                pcbRook.Image = Properties.Resources.Chess_Rook_White;
                pcbQueen.Image = Properties.Resources.Chess_Queen_White;
            }
            else if (selectedPieceColor == "Black")
            {
                pcbKnight.Image = Properties.Resources.Chess_Knight_Black;
                pcbRook.Image = Properties.Resources.Chess_Rook_Black;
                pcbQueen.Image = Properties.Resources.Chess_Queen_Black;
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

        private void ClearBoardColors()
        {
            foreach (PictureBox pb in gbxBoard.Controls.OfType<PictureBox>())
            {
                pb.BackColor = Color.Transparent;
            }
        }

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
            //Gets the location of all default neighbours
            Board right = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical());
            Board left = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical());
            Board up = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() && x.GetVertical() == activeBoard.GetVertical() - 1);
            Board down = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() && x.GetVertical() == activeBoard.GetVertical() + 1);

            if (activePiece != null)
            {
                if (activePiece.GetName() == "Rook" || activePiece.GetName() == "Queen")
                {
                    CheckForNeighbour(right, "Right");
                    CheckForNeighbour(left, "Left");
                    CheckForNeighbour(up, "Up");
                    CheckForNeighbour(down, "Down");
                }
                if (activePiece.GetName() == "Queen")
                {
                    //Gets the location of all the diagonal neighbours 
                    Board upRight = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical() - 1);
                    Board upLeft = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical() - 1);
                    Board downRight = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() + 1 && x.GetVertical() == activeBoard.GetVertical() + 1);
                    Board downLeft = boardList.FirstOrDefault(x => x.GetHorizontal() == activeBoard.GetHorizontal() - 1 && x.GetVertical() == activeBoard.GetVertical() + 1);

                    CheckForNeighbour(upRight, "UpRight");
                    CheckForNeighbour(upLeft, "UpLeft");
                    CheckForNeighbour(downRight, "DownRight");
                    CheckForNeighbour(downLeft, "DownLeft");
                }
            }
        }

        private void CheckForNeighbour(Board neighbour, string direction)
        {
            if (neighbour != null && neighbour.GetPiece() != null)
            {
                switch (direction)
                {
                    case "Left":
                        //Checks if there is a board on the left (-1) of the neighbour of the selected piece
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
                //If there is a forbidden bord it sets this picturebox backcolor to transparent
                if (forbidden != null)
                {
                    pcbForbidden = (PictureBox)gbxBoard.Controls.Find(forbidden.GetPictureName(), false)[0];
                    pcbForbidden.BackColor = Color.Transparent;
                }
            }
        }

        private void rdbWhite_CheckedChanged(object sender, EventArgs e)
        {
            selectedPieceColor = "White";
            UpdatePieceColor();
        }

        private void rdbBlack_CheckedChanged(object sender, EventArgs e)
        {
            selectedPieceColor = "Black";
            UpdatePieceColor();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            selectedPieceColor = "White";
            UpdatePieceColor();
            rdbWhite.Checked = true;

            foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
            {
                item.AllowDrop = true;
                item.Image = null;
                item.BackColor = SystemColors.Control;
            }
        }

        private void pcbAllPieces_MouseDown(object sender, MouseEventArgs e)
        {
            // Set the board as inactive, check available setup locations
            activeBoard = null;
            pcbFrom = (PictureBox)sender;

            foreach (Piece item in pieceList)
            {
                if (item.GetBasePictureboxName() == pcbFrom.Name && item.GetColor() == selectedPieceColor)
                {
                    activePiece = item;
                }
            }
            CheckMoves();

            pcbFrom.DoDragDrop(pcbFrom.Image, DragDropEffects.Copy);
        }

        private void pcbBoard_DragDrop(object sender, DragEventArgs e)
        {
            // Gets the currently held piece as an image and copies it to the designated picturebox
            pcbTo = (PictureBox)sender;
            Image getPicture = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
            pcbTo.Image = getPicture;

            // Gets the location of the picturebox to put the piece onto
            horizontal = Convert.ToInt32(pcbTo.Tag.ToString().Substring(0, 1));
            vertical = Convert.ToInt32(pcbTo.Tag.ToString().Substring(1, 1));

            if (activeBoard != null)
            {
                activeBoard.SetPiece(null);
                activeBoard = boardList.FirstOrDefault(x => x.GetHorizontal() == horizontal && x.GetVertical() == vertical);
                activePiece.SetCurrentPicturebox(pcbTo.Name);
                activeBoard.SetPiece(activePiece);
                pcbFrom.Image = null;
            }
            else
            {
                activePiece.SetCurrentPicturebox(pcbTo.Name);
                boardList.FirstOrDefault(x => x.GetHorizontal() == horizontal && x.GetVertical() == vertical).SetPiece(activePiece);
            }
            ClearBoardColors();

            foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
            {
                item.BackColor = Color.Transparent;
            }
        }

        private void pcbBoard_DragOver(object sender, DragEventArgs e)
        {
            pcbTo = (PictureBox)sender;

            foreach (PictureBox item in gbxBoard.Controls.OfType<PictureBox>())
            {
                if (item.Image != null)
                {
                    item.BackColor = Color.Red;
                }
            }

            if (pcbTo.BackColor == Color.Green)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
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
            pieceList.Add(new Piece("Rook", "Black"));
            pieceList.Add(new Piece("Knight", "Black"));
            pieceList.Add(new Piece("Queen", "Black"));

            // Board list, declare all available locations by referencing the names & tags
            // in the form of Board(1, 1, "pcbOne"), where pcbOne is the PictureBox name, and the (1, 1)
            // the horizontal and vertical axis in chronological order
            boardList = new List<Board>();
            boardList.Add(new Board(1, 1, "pcbOne"));
            boardList.Add(new Board(2, 1, "pcbTwo"));
            boardList.Add(new Board(3, 1, "pcbThree"));
            boardList.Add(new Board(1, 2, "pcbFour"));
            boardList.Add(new Board(2, 2, "pcbFive"));
            boardList.Add(new Board(3, 2, "pcbSix"));
            boardList.Add(new Board(1, 3, "pcbSeven"));
            boardList.Add(new Board(2, 3, "pcbEight"));
            boardList.Add(new Board(3, 3, "pcbNine"));
        }
    }
}
