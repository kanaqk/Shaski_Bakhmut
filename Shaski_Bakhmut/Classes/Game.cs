using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shaski_Bakhmut
{
    public class Game
    {
        public Piece[,] Board { get; set; }
        public List<Player> PlayerList { get; set; }
        public List<Turn> TurnList { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player Winner { get; set; }

        public Game()
        {
            Board = new Piece[8, 8];
            PlayerList = new List<Player>();
            TurnList = new List<Turn>();
            fillBoard();
        }
        public void StartGame(Player player1, Player player2)
        {
            PlayerList.Add(player1);
            PlayerList.Add(player2);
            if (player1.Color == Color.White) { CurrentPlayer = player1; }
            else CurrentPlayer = player2;
        }
        public void fillBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 != 0) 
                    {
                        if (i < 3)
                        {
                            Board[i, j] = new Piece(new List<int> { i, j }, Color.White);
                        }
                        else if (i > 4)
                        {
                            Board[i, j] = new Piece(new List<int> { i, j }, Color.Black);
                        }
                        else
                        {
                            Board[i, j] = null; 
                        }
                    }
                    else
                    {
                        Board[i, j] = null; 
                    }
                }
            }
        }
        private string ConvertToChessNotation(int row, int col)
        {
            return $"{(char)('H' - col)}{1 + row}";
        }

        public void AddTurn(Piece piece, List<int> startPosition, List<int> intermediatePosition, List<int> endPosition)
        {
            string start = ConvertToChessNotation(startPosition[0], startPosition[1]);
            string end = ConvertToChessNotation(endPosition[0], endPosition[1]);
            Turn turn = new Turn(piece, start, intermediatePosition, end);
            TurnList.Add(turn);
        }

        public List<(int, int)> FindPossibleTurns(int startRow, int startColumn)
        {
            List<(int, int)> possibleTurns = new List<(int, int)>();
            Piece piece = Board[startRow, startColumn];

            if (piece == null || piece.Color != CurrentPlayer.Color)
            {
                return possibleTurns;
            }

            List<(int, int)> captures = FindPossibleCaptures(piece, startRow, startColumn);
            if (captures.Count > 0)
            {
                return captures;
            }

            int[] rowDirections = { -1, 1 };
            int[] colDirections = { -1, 1 };

            if (piece.Type == PieceType.Regular)
            {
                foreach (int rowDir in rowDirections)
                {
                    foreach (int colDir in colDirections)
                    {
                        int newRow = startRow + rowDir;
                        int newCol = startColumn + colDir;

                        if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8 &&
                            Board[newRow, newCol] == null &&
                            ((piece.Color == Color.White && rowDir == 1) || (piece.Color == Color.Black && rowDir == -1)))
                        {
                            possibleTurns.Add((newRow, newCol));
                        }
                    }
                }
            }

            if (piece.Type == PieceType.Dam)
            {
                foreach (int rowDir in rowDirections)
                {
                    foreach (int colDir in colDirections)
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            int newRow = startRow + i * rowDir;
                            int newCol = startColumn + i * colDir;

                            if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
                            {
                                if (Board[newRow, newCol] == null)
                                {
                                    possibleTurns.Add((newRow, newCol));
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return possibleTurns;
        }

        public List<(int, int)> FindPossibleCaptures(Piece piece, int startRow, int startColumn, bool isCaptureMove = false, HashSet<(int, int)> visited = null)
        {
            List<(int, int)> captures = new List<(int, int)>();
            int[] rowDirections = { -1, 1 };
            int[] colDirections = { -1, 1 };

            // Инициализация множества посещённых клеток
            if (visited == null)
            {
                visited = new HashSet<(int, int)>();
            }

            if (piece.Type == PieceType.Regular)
            {
                foreach (int rowDir in rowDirections)
                {
                    foreach (int colDir in colDirections)
                    {
                        int newRow = startRow + 2 * rowDir;
                        int newCol = startColumn + 2 * colDir;
                        int midRow = startRow + rowDir;
                        int midCol = startColumn + colDir;

                        if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8 &&
                            Board[newRow, newCol] == null &&
                            Board[midRow, midCol] != null &&
                            Board[midRow, midCol].Color != piece.Color &&
                            !Board[midRow, midCol].IsTaken)
                        {
                            captures.Add((newRow, newCol));
                        }
                    }
                }
            }
            else if (piece.Type == PieceType.Dam)
            {
                List<(int, int)> secondaryCaptures = new List<(int, int)>();
                List<(int, int)> allFreeCells = new List<(int, int)>();

                foreach (int rowDir in rowDirections)
                {
                    foreach (int colDir in colDirections)
                    {
                        int checkRow = startRow + rowDir;
                        int checkCol = startColumn + colDir;

                        while (checkRow >= 0 && checkRow < 8 && checkCol >= 0 && checkCol < 8)
                        {
                            if (Board[checkRow, checkCol] != null)
                            {
                                if (Board[checkRow, checkCol].Color != piece.Color && !Board[checkRow, checkCol].IsTaken)
                                {
                                    int captureRow = checkRow + rowDir;
                                    int captureCol = checkCol + colDir;

                                    // Собираем все свободные поля за захваченной шашкой
                                    while (captureRow >= 0 && captureRow < 8 && captureCol >= 0 && captureCol < 8 && Board[captureRow, captureCol] == null)
                                    {
                                        allFreeCells.Add((captureRow, captureCol));
                                        captureRow += rowDir;
                                        captureCol += colDir;
                                    }
                                }
                                break;
                            }
                            checkRow += rowDir;
                            checkCol += colDir;
                        }
                    }
                }

                // Проверяем, какие из свободных полей ведут к дальнейшим захватам
                foreach (var cell in allFreeCells)
                {
                    if (!visited.Contains(cell))
                    {
                        visited.Add(cell);
                        var furtherCaptures = FindPossibleCaptures(piece, cell.Item1, cell.Item2, true, visited);
                        if (furtherCaptures.Count > 0)
                        {
                            secondaryCaptures.Add(cell);
                        }
                        visited.Remove(cell);
                    }
                }

                // Если есть захваты, ведущие к следующим захватам, добавляем их
                if (secondaryCaptures.Count > 0)
                {
                    captures.AddRange(secondaryCaptures);
                }
                else // Иначе добавляем все свободные поля
                {
                    captures.AddRange(allFreeCells);
                }
            }

            return captures;
        }
        public List<(int, int)> FindPossibleCaptures(Color playerColor)
        {
            List<(int, int)> captures = new List<(int, int)>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = Board[i, j];
                    if (piece != null && piece.Color == playerColor)
                    {
                        captures.AddRange(FindPossibleCaptures(piece, i, j));
                    }
                }
            }

            return captures;
        }

    }
}