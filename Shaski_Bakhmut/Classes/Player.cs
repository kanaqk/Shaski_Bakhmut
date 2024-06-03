using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaski_Bakhmut
{
    public class Player
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<(int, int)> PossibleCaptures { get; private set; } = new List<(int, int)>();
        public bool IsInMultiCapture { get; set; } = false;
        public (int, int)? MultiCaptureStart { get; set; } = null;

        public Player(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public void ChooseColor(Color color)
        {
            Color = color;
        }

        public static Color RandomColor()
        {
            Random random = new Random();
            Array values = Enum.GetValues(typeof(Color));
            return (Color)values.GetValue(random.Next(values.Length));
        }

        public Piece ChoosePiece(int row, int column, Game game)
        {
            return game.Board[row, column];
        }

        public bool MakeMove(int startRow, int startColumn, int endRow, int endColumn, Game game)
        {
            Piece piece = game.Board[startRow, startColumn];
            if (piece == null || piece.Color != this.Color)
            {
                return false;
            }

            bool isMoveValid = false;
            int rowDiff = Math.Abs(endRow - startRow);
            int colDiff = Math.Abs(endColumn - startColumn);

            // Проверка на выход за пределы поля
            if (endRow < 0 || endRow >= 8 || endColumn < 0 || endColumn >= 8)
            {
                return false;
            }

            // Проверка на использование не пустого поля
            if (game.Board[endRow, endColumn] != null)
            {
                return false;
            }

            // Проверка на обязательные взятия
            List<(int, int)> mandatoryCaptures = game.FindPossibleCaptures(this.Color);

            if (mandatoryCaptures.Count > 0 && !mandatoryCaptures.Contains((endRow, endColumn)))
            {
                return false; // Если взятие обязательно, обычный ход запрещен
            }

            bool isCaptureMove = false;

            if (piece.Type == PieceType.Regular)
            {
                if (rowDiff == 1 && colDiff == 1)
                {
                    // Обычный ход
                    if (mandatoryCaptures.Count == 0)
                    {
                        if ((piece.Color == Color.White && endRow > startRow) || (piece.Color == Color.Black && endRow < startRow))
                        {
                            isMoveValid = true;
                            game.Board[endRow, endColumn] = game.Board[startRow, startColumn];
                            game.Board[startRow, startColumn] = null;
                            game.Board[endRow, endColumn].Position = new List<int> { endRow, endColumn };

                            if ((piece.Color == Color.White && endRow == 7) || (piece.Color == Color.Black && endRow == 0))
                            {
                                game.Board[endRow, endColumn].Type = PieceType.Dam;
                            }
                        }
                    }
                }
                else if (rowDiff == 2 && colDiff == 2)
                {
                    // Ход с захватом
                    int midRow = (startRow + endRow) / 2;
                    int midColumn = (startColumn + endColumn) / 2;
                    Piece capturedPiece = game.Board[midRow, midColumn];

                    if (capturedPiece != null && capturedPiece.Color != this.Color && !capturedPiece.IsTaken)
                    {
                        isMoveValid = true;
                        isCaptureMove = true;
                        capturedPiece.IsTaken = true;
                        game.Board[endRow, endColumn] = game.Board[startRow, startColumn];
                        game.Board[startRow, startColumn] = null;
                        game.Board[endRow, endColumn].Position = new List<int> { endRow, endColumn };

                        if ((piece.Color == Color.White && endRow == 7) || (piece.Color == Color.Black && endRow == 0))
                        {
                            game.Board[endRow, endColumn].Type = PieceType.Dam;
                        }

                        PossibleCaptures.Clear();
                        PossibleCaptures.AddRange(game.FindPossibleCaptures(piece, endRow, endColumn, isCaptureMove));

                        if (PossibleCaptures.Count > 0 && isCaptureMove)
                        {
                            IsInMultiCapture = true;
                            MultiCaptureStart = (endRow, endColumn);
                            return true; // Принудительный захват продолжается
                        }
                    }
                }
            }
            else if (piece.Type == PieceType.Dam)
            {
                if (rowDiff == colDiff)
                {
                    int rowDirection = (endRow - startRow) / rowDiff;
                    int colDirection = (endColumn - startColumn) / colDiff;
                    bool pathClear = true;
                    bool captureMove = false;
                    int captureRow = -1, captureCol = -1;

                    for (int i = 1; i < rowDiff; i++)
                    {
                        int checkRow = startRow + i * rowDirection;
                        int checkCol = startColumn + i * colDirection;
                        Piece checkPiece = game.Board[checkRow, checkCol];
                        if (checkPiece != null)
                        {
                            if (checkPiece.Color != this.Color && !captureMove && !checkPiece.IsTaken)
                            {
                                captureMove = true;
                                captureRow = checkRow;
                                captureCol = checkCol;
                            }
                            else
                            {
                                pathClear = false;
                                break;
                            }
                        }
                    }

                    if (pathClear)
                    {
                        isMoveValid = true;
                        game.Board[endRow, endColumn] = game.Board[startRow, startColumn];
                        game.Board[startRow, startColumn] = null;
                        game.Board[endRow, endColumn].Position = new List<int> { endRow, endColumn };

                        if (captureMove)
                        {
                            isCaptureMove = true;
                            game.Board[captureRow, captureCol].IsTaken = true;
                        }

                        PossibleCaptures.Clear();
                        PossibleCaptures.AddRange(game.FindPossibleCaptures(piece, endRow, endColumn, isCaptureMove));

                        if (PossibleCaptures.Count > 0 && isCaptureMove)
                        {
                            IsInMultiCapture = true;
                            MultiCaptureStart = (endRow, endColumn);
                            return true; // Принудительный захват продолжается
                        }
                    }
                }
            }

            if (isMoveValid)
            {
                IsInMultiCapture = false;
                MultiCaptureStart = (-1, -1);

                // Удаление всех захваченных шашек
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        Piece p = game.Board[row, col];
                        if (p != null && p.IsTaken)
                        {
                            game.Board[row, col] = null;
                        }
                    }
                }

                game.AddTurn(piece, new List<int> { startRow, startColumn }, new List<int> { endRow, endColumn }, new List<int> { endRow, endColumn });
            }

            return isMoveValid;
        }


    }
}