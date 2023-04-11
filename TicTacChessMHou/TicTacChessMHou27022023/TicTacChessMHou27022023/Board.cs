using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacChessMHou27022023
{
    class Board
    {
        // Variables
        private int horizontal;
        private int vertical;
        private int arduinoRot;
        private int arduinoHor;
        private int arduinoVer;
        private Piece currentPiece = null;
        private string pictureName = "";

        public Board(int c_horizontal, int c_vertical, string c_pictureName, int c_arduinoHor, int c_arduinoVer, int c_arduinoRot)
        {
            horizontal  = c_horizontal;
            vertical    = c_vertical;
            pictureName = c_pictureName;
            arduinoHor  = c_arduinoHor;
            arduinoRot = c_arduinoRot;
            arduinoVer = c_arduinoVer;
        }

        // Get
        public int GetHorizontal() { return horizontal; }
        public int GetVertical() { return vertical; }
        public Piece GetPiece() { return currentPiece; }
        public string GetPictureName() { return pictureName; }
        public int GetArduinoRot() { return arduinoRot; }
        public int GetArduinoHor() { return arduinoHor; }
        public int GetArduinoVer() { return arduinoVer; }

        // Set
        public void SetPiece(Piece piece) { currentPiece = piece; }
    }
}
