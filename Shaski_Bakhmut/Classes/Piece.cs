using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaski_Bakhmut
{
    public class Piece
    {
        public PieceType Type { get; set; } = PieceType.Regular;
        public List<int> Position { get; set; }
        public Color Color { get; set; }
        public bool IsTaken { get; set; } = false;

        public Piece(List<int> position, Color color)
        {
            Type = PieceType.Regular;
            Position = position;
            Color = color;
            IsTaken = false;
        }
    }
}
