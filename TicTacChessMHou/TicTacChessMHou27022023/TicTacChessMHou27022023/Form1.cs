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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ClickTile(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            pic.Image = TicTacChessMHou27022023.Properties.Resources.flappybird;
        }

        private void rdbWhiteMHou_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbWhiteMHou.Checked)
            {
                this.pbxPieceOneMHou.Image = TicTacChessMHou27022023.Properties.Resources.Chess_Queen_White;
                this.pbxPieceTwoMHou.Image = TicTacChessMHou27022023.Properties.Resources.Chess_Rook_White;
                this.pbxPieceThreeMHou.Image = TicTacChessMHou27022023.Properties.Resources.Chess_Knight_White;
            }
            else
            {
                this.pbxPieceOneMHou.Image = TicTacChessMHou27022023.Properties.Resources.Chess_Queen_Black;
                this.pbxPieceTwoMHou.Image = TicTacChessMHou27022023.Properties.Resources.Chess_Rook_Black;
                this.pbxPieceThreeMHou.Image = TicTacChessMHou27022023.Properties.Resources.Chess_Knight_Black;
            }
        }

        private void btnRestartGameMHou_Click(object sender, EventArgs e)
        {
            rdbWhiteMHou.Checked = true;
            // PictureBox pic = sender as PictureBox;

            // pic.Image = null;
        }
    }
}
