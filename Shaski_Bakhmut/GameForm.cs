using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Shaski_Bakhmut
{
    public partial class GameForm : Form
    {
        private Game game;
        private DoubleBufferedPanel boardPanel;
        private int? selectedRow = null;
        private int? selectedColumn = null;
        private List<(int, int)> possibleMoves = new List<(int, int)>();
        private ListBox turnListBox;
        private Label player1Label;
        private Label player2Label;
        private Label currentPlayerLabel;
        private Button surrenderButton1;
        private Button surrenderButton2;
        private Button drawOfferButton1;
        private Button drawOfferButton2;
        private bool drawOffered = false;
        private Player drawOfferingPlayer;

        public GameForm()
        {
            InitializeComponent();
            if (SetupPlayers())
            {
                InitializeBoardPanel();
                InitializePlayerLabels();
                InitializeTurnListBox();
                InitializeSurrenderAndDrawButtons();
            }
            else
            {
                this.Close();
            }
        }

        private void CheckForWinner()
        {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (Piece piece in game.Board)
            {
                if (piece != null)
                {
                    if (piece.Color == Color.White)
                    {
                        whiteCount++;
                    }
                    else
                    {
                        blackCount++;
                    }
                }
            }

            if (whiteCount == 0)
            {
                if (game.PlayerList[0].Color == Color.White) game.Winner = game.PlayerList[1];
                else game.Winner = game.PlayerList[0];
            }
            else if (blackCount == 0)
            {
                if (game.PlayerList[0].Color == Color.White) game.Winner = game.PlayerList[0];
                else game.Winner = game.PlayerList[1];
            }
            else
            {
                bool hasPossibleMoves = false;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Piece piece = game.Board[i, j];
                        if (piece != null && piece.Color == game.CurrentPlayer.Color)
                        {
                            var moves = game.FindPossibleTurns(i, j);
                            if (moves.Count > 0)
                            {
                                hasPossibleMoves = true;
                                break;
                            }
                        }
                    }
                    if (hasPossibleMoves) break;
                }

                if (!hasPossibleMoves)
                {
                    if (game.CurrentPlayer.Color == Color.White)
                    {
                        game.Winner = game.PlayerList[1];
                    }
                    else
                    {
                        game.Winner = game.PlayerList[0];
                    }
                }
            }

            if (game.Winner != null)
            {
                currentPlayerLabel.Text = $"Победитель:{game.Winner.Name}" ;
            }
        }

        private bool SetupPlayers()
        {
            using (SetupForm setupForm = new SetupForm())
            {
                if (setupForm.ShowDialog() == DialogResult.OK)
                {
                    string player1Name = setupForm.Player1Name;
                    string player2Name = setupForm.Player2Name;
                    Color player1Color = setupForm.Player1Color;
                    Color player2Color = setupForm.Player2Color;

                    game = new Game();
                    Player player1 = new Player(player1Name, player1Color);
                    Player player2 = new Player(player2Name, player2Color);
                    game.StartGame(player1, player2);
                    return true;
                }
                return false;
            }
        }

        private void InitializeBoardPanel()
        {
            int cellSize = 50;
            int boardSize = cellSize * 8;
            int panelSize = boardSize + 40; 
            int boardX = (this.ClientSize.Width - panelSize) / 2;

            boardPanel = new DoubleBufferedPanel
            {
                Location = new Point(boardX, 20),
                Name = "boardPanel",
                Size = new Size(panelSize, panelSize)
            };
            boardPanel.Paint += new PaintEventHandler(BoardPanel_Paint);
            boardPanel.MouseClick += new MouseEventHandler(BoardPanel_MouseClick);
            this.Controls.Add(boardPanel);
        }
        private void InitializeSurrenderAndDrawButtons()
        {
            surrenderButton1 = new Button
            {
                Location = new Point(player1Label.Location.X, player1Label.Location.Y + 30),
                Name = "surrenderButton1",
                Size = new Size(75, 23),
                Text = "Сдаться"
            };
            surrenderButton1.Click += new EventHandler(SurrenderButton1_Click);
            this.Controls.Add(surrenderButton1);

            surrenderButton2 = new Button
            {
                Location = new Point(player2Label.Location.X, player2Label.Location.Y + 30),
                Name = "surrenderButton2",
                Size = new Size(75, 23),
                Text = "Сдаться"
            };
            surrenderButton2.Click += new EventHandler(SurrenderButton2_Click);
            this.Controls.Add(surrenderButton2);

            drawOfferButton1 = new Button
            {
                Location = new Point(surrenderButton1.Location.X + 80, surrenderButton1.Location.Y),
                Name = "drawOfferButton1",
                Size = new Size(100, 23),
                Text = "Ничья"
            };
            drawOfferButton1.Click += new EventHandler(DrawOfferButton1_Click);
            this.Controls.Add(drawOfferButton1);

            drawOfferButton2 = new Button
            {
                Location = new Point(surrenderButton2.Location.X + 80, surrenderButton2.Location.Y),
                Name = "drawOfferButton2",
                Size = new Size(100, 23),
                Text = "Ничья"
            };
            drawOfferButton2.Click += new EventHandler(DrawOfferButton2_Click);
            this.Controls.Add(drawOfferButton2);
        }
        private void DrawOfferButton1_Click(object sender, EventArgs e)
        {
            OfferDraw(game.PlayerList[0]);
        }

        private void DrawOfferButton2_Click(object sender, EventArgs e)
        {
            OfferDraw(game.PlayerList[1]);
        }
        private void OfferDraw(Player player)
        {
            if (drawOffered)
{
                if (drawOfferingPlayer != player)
                {
                    game.Winner = null; 
                    currentPlayerLabel.Text = "Ничья";
                    boardPanel.MouseClick -= BoardPanel_MouseClick;
                }
                drawOffered = false;
            }
            else
            {
                drawOffered = true;
                drawOfferingPlayer = player;
            }
        }
        private void SurrenderButton1_Click(object sender, EventArgs e)
        {
            game.Winner = game.PlayerList[1];
            currentPlayerLabel.Text = $"Победитель: {game.Winner.Name}";
            boardPanel.MouseClick -= BoardPanel_MouseClick;
        }

        private void SurrenderButton2_Click(object sender, EventArgs e)
        {
            game.Winner = game.PlayerList[0];
            currentPlayerLabel.Text = $"Победитель: {game.Winner.Name}";
            boardPanel.MouseClick -= BoardPanel_MouseClick;
        }
        private void InitializePlayerLabels()
        {
            if (game.PlayerList[0].Color == Color.White)
            {
                player1Label = new Label
                {
                    Location = new Point(boardPanel.Location.X - 200, 40),
                    Name = "player1Label",
                    Size = new Size(200, 23),
                    Font = new Font(Font.FontFamily, 14),
                    Text = $"{game.PlayerList[0].Name} (Белые)",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                player2Label = new Label
                {
                    Location = new Point(boardPanel.Location.X - 200, 395),
                    Name = "player2Label",
                    Size = new Size(200, 23),
                    Font = new Font(Font.FontFamily, 14),
                    Text = $"{game.PlayerList[1].Name} (Черные)",
                    TextAlign = ContentAlignment.MiddleLeft
                };
            }
            else
            {
                player1Label = new Label
                {
                    Location = new Point(boardPanel.Location.X - 200, 395),
                    Name = "player1Label",
                    Size = new Size(200, 23),
                    Font = new Font(Font.FontFamily, 14),
                    Text = $"{game.PlayerList[0].Name} (Черные)",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                player2Label = new Label
                {
                    Location = new Point(boardPanel.Location.X - 200, 40),
                    Name = "player2Label",
                    Size = new Size(200, 23),
                    Font = new Font(Font.FontFamily, 14),
                    Text = $"{game.PlayerList[1].Name} (Белые)",
                    TextAlign = ContentAlignment.MiddleLeft
                };
            }

            this.Controls.Add(player1Label);
            this.Controls.Add(player2Label);

            currentPlayerLabel = new Label
            {
                Location = new Point(boardPanel.Location.X - 200, 200),
                Name = "currentPlayerLabel",
                Size = new Size(200, 23),
                Font = new Font(Font.FontFamily, 14),
                Text = $"Ходит: {game.CurrentPlayer.Name}",
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(currentPlayerLabel);
        }

        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int cellSize = (boardPanel.Width - 40) / 8;
            int offsetX = 20;
            int offsetY = 20;
            int row = (e.Y - offsetY) / cellSize;
            int col = (e.X - offsetX) / cellSize;

            if (row < 0 || row >= 8 || col < 0 || col >= 8)
            {
                return;
            }

            if (selectedRow == null || selectedColumn == null)
            {
                Piece piece = game.Board[row, col];
                if (piece != null && piece.Color == game.CurrentPlayer.Color)
                {
                    selectedRow = row;
                    selectedColumn = col;
                    possibleMoves = game.FindPossibleTurns(row, col);
                }
            }
            else
            {
                if (possibleMoves.Contains((row, col)))
                {
                    bool moveMade = game.CurrentPlayer.MakeMove((int)selectedRow, (int)selectedColumn, row, col, game);

                    if (moveMade)
                    {
                        if (game.CurrentPlayer.IsInMultiCapture)
                        {
                            selectedRow = row;
                            selectedColumn = col;
                            possibleMoves = game.FindPossibleTurns(row, col);
                        }
                        else
                        {
                            selectedRow = null;
                            selectedColumn = null;
                            possibleMoves.Clear();
                            SwitchPlayer();
                        }
                        UpdateTurnList();
                    }
                }
                else
                {
                    selectedRow = null;
                    selectedColumn = null;
                    possibleMoves.Clear();
                }
            }
            boardPanel.Invalidate();
        }


        private void SwitchPlayer()
        {
            game.CurrentPlayer = game.CurrentPlayer == game.PlayerList[0] ? game.PlayerList[1] : game.PlayerList[0];
            currentPlayerLabel.Text = $"Ходит: {game.CurrentPlayer.Name}";
            CheckForWinner();
            if (game.Winner != null)
            {
                boardPanel.MouseClick -= BoardPanel_MouseClick;
            }
        }

        private void InitializeTurnListBox()
        {
            int boardX = boardPanel.Location.X;
            turnListBox = new ListBox
            {
                Location = new Point(boardX + boardPanel.Width + 20, 40),
                Name = "turnListBox",
                Size = new Size(200, 400),
                Font = new Font(Font.FontFamily, 10),
            };
            this.Controls.Add(turnListBox);
        }

        private void UpdateTurnList()
        {
            turnListBox.Items.Clear();
            foreach (var turn in game.TurnList)
            {
                string turnString = $"{turn.Piece.Color} {turn.Piece.Type} from ({turn.StartPosition[0]}, {turn.StartPosition[1]}) to ({turn.EndPosition[0]}, {turn.EndPosition[1]})";
                turnListBox.Items.Add(turnString);
            }
        }

        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int cellSize = (boardPanel.Width - 40) / 8;
            int offsetX = 20;
            int offsetY = 20;
            Brush whiteBrush = Brushes.White;
            Brush blackBrush = Brushes.Black;
            Brush whitePieceBrush = Brushes.White;
            Brush blackPieceBrush = Brushes.Black;
            Pen selectionPen = new Pen(System.Drawing.Color.Green, 3);
            Pen movePen = new Pen(System.Drawing.Color.Green, 3);
            Pen borderPen = new Pen(System.Drawing.Color.Black, 2);
            Pen blackPieceBorderPen = new Pen(System.Drawing.Color.White, 2);
            Pen whiteCrownPen = new Pen(System.Drawing.Color.White, 2);
            Pen blackCrownPen = new Pen(System.Drawing.Color.Black, 2);

            // Отрисовка номеров строк и столбцов
            for (int i = 0; i < 8; i++)
            {
                g.DrawString((1 + i).ToString(), new Font("Arial", 10), Brushes.Black, new PointF(5, i * cellSize + offsetY + cellSize / 2 - 7));
                g.DrawString((1 + i).ToString(), new Font("Arial", 10), Brushes.Black, new PointF(boardPanel.Width - 20 + 5, i * cellSize + offsetY + cellSize / 2 - 7));

                g.DrawString(((char)('H' - i)).ToString(), new Font("Arial", 10), Brushes.Black, new PointF(i * cellSize + offsetX + cellSize / 2 - 7, 5));
                g.DrawString(((char)('H' - i)).ToString(), new Font("Arial", 10), Brushes.Black, new PointF(i * cellSize + offsetX + cellSize / 2 - 7, boardPanel.Height - 20 + 5));
            }

            // Отрисовка границ доски
            g.DrawRectangle(borderPen, offsetX, offsetY, cellSize * 8, cellSize * 8);

            // Отрисовка клеток и шашек
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Brush brush = (i + j) % 2 == 0 ? whiteBrush : blackBrush;
                    g.FillRectangle(brush, j * cellSize + offsetX, i * cellSize + offsetY, cellSize, cellSize);

                    Piece piece = game.Board[i, j];
                    if (piece != null)
                    {
                        Brush pieceBrush = piece.Color == Color.White ? whitePieceBrush : blackPieceBrush;
                        g.FillEllipse(pieceBrush, j * cellSize + offsetX + 5, i * cellSize + offsetY + 5, cellSize - 10, cellSize - 10);

                        if (piece.Color == Color.Black)
                        {
                            g.DrawEllipse(blackPieceBorderPen, j * cellSize + offsetX + 5, i * cellSize + offsetY + 5, cellSize - 10, cellSize - 10);
                        }

                        if (piece.Type == PieceType.Dam)
                        {
                            if (piece.Color == Color.White)
                            {
                                g.DrawEllipse(blackCrownPen, j * cellSize + offsetX + 15, i * cellSize + offsetY + 15, cellSize - 30, cellSize - 30);
                            }
                            else
                            {
                                g.DrawEllipse(whiteCrownPen, j * cellSize + offsetX + 15, i * cellSize + offsetY + 15, cellSize - 30, cellSize - 30);
                            }
                        }
                    }
                }
            }

            // Получение обязательных взятий для текущего игрока
            List<(int, int)> mandatoryCaptures = game.FindPossibleCaptures(game.CurrentPlayer.Color);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    // Отрисовка выделения выбранной шашки
                    if (selectedRow.HasValue && selectedColumn.HasValue && selectedRow.Value == i && selectedColumn.Value == j)
                    {
                        g.DrawRectangle(selectionPen, j * cellSize + offsetX, i * cellSize + offsetY, cellSize - 2, cellSize - 2);
                    }

                    // Отрисовка возможных ходов
                    if (possibleMoves.Contains((i, j)))
                    {
                        // Отрисовка возможных ходов только если нет обязательных взятий или текущая шашка может выполнить захват
                        if (mandatoryCaptures.Count == 0 || mandatoryCaptures.Contains((selectedRow.Value, selectedColumn.Value)) || mandatoryCaptures.Contains((i, j)))
                        {
                            g.DrawRectangle(movePen, j * cellSize + offsetX, i * cellSize + offsetY, cellSize - 2, cellSize - 2);
                        }
                    }
                }
            }
        }



        public class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                this.UpdateStyles();
            }
        }
    }
}
