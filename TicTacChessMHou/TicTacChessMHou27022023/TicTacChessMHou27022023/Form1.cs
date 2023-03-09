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
        string selectedPieceColor = "";
        bool pieceAvailable = true;

        PictureBox pcbFrom = null;
        PictureBox pcbTo = null;
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
        }

        private void CheckMoves()
        {
            if (selectedPieceColor == "White")
            {
                pcbSeven.BackColor = Color.GreenYellow;
                pcbEight.BackColor = Color.GreenYellow;
                pcbNine.BackColor = Color.GreenYellow;
            }
            else if (selectedPieceColor == "Black")
            {
                pcbOne.BackColor = Color.GreenYellow;
                pcbTwo.BackColor = Color.GreenYellow;
                pcbThree.BackColor = Color.GreenYellow;
            }
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
            CheckMoves();
            pcbFrom = (PictureBox)sender;
            pcbFrom.DoDragDrop(pcbFrom.Image, DragDropEffects.Copy);
        }

        private void pcbBoard_DragDrop(object sender, DragEventArgs e)
        {
            pcbTo = (PictureBox)sender;
            Image getPicture = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
            pcbTo.Image = getPicture;
        }

        private void pcbBoard_DragOver(object sender, DragEventArgs e)
        {
            pcbTo = (PictureBox)sender;
            e.Effect = DragDropEffects.Copy;
        }
    }
}
