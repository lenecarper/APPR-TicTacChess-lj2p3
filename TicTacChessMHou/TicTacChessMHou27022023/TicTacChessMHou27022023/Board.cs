﻿using System;
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
        private Piece currentPiece = null;
        private string pictureName = "";

        public Board(int c_horizontal, int c_vertical, string c_pictureName)
        {
            horizontal = c_horizontal;
            vertical = c_vertical;
            pictureName = c_pictureName;
        }

        // Get
        public int GetHorizontal() { return horizontal; }
        public int GetVertical() { return vertical; }
        public Piece GetPiece() { return currentPiece; }
        public string GetPictureName() { return pictureName; }

        // Set
        public void SetPiece(Piece piece) { currentPiece = piece; }
    }

    class oldBoard
    {
        // Variables
        private int arduinoRot;
        private int arduinoHor;
        private int arduinoVer;

        public oldBoard(int c_arduinoRot, int c_arduinoHor, int c_arduinoVer)
        {
            arduinoRot = c_arduinoRot;
            arduinoHor = c_arduinoHor;
            arduinoVer = c_arduinoVer;
        }

        // Get
        public int GetArduinoRot() { return arduinoRot; }
        public int GetArduinoHor() { return arduinoHor; }
        public int GetArduinoVer() { return arduinoVer; }

        // Set

    }

    class newBoard
    {
        // Variables
        private int arduinoRot;
        private int arduinoHor;
        private int arduinoVer;

        public newBoard(int c_arduinoRot, int c_arduinoHor, int c_arduinoVer)
        {
            arduinoRot = c_arduinoRot;
            arduinoHor = c_arduinoHor;
            arduinoVer = c_arduinoVer;
        }

        // Get
        public int GetArduinoRot() { return arduinoRot; }
        public int GetArduinoHor() { return arduinoHor; }
        public int GetArduinoVer() { return arduinoVer; }

        // Set

    }
}
