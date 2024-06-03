using Shaski_Bakhmut;
using System.Collections.Generic;

public class Turn
{
    public Piece Piece { get; set; }
    public string StartPosition { get; set; }
    public List<int> IntermediatePosition { get; set; }
    public string EndPosition { get; set; }

    public Turn(Piece piece, string startPosition, List<int> intermediatePosition, string endPosition)
    {
        Piece = piece;
        StartPosition = startPosition;
        IntermediatePosition = intermediatePosition;
        EndPosition = endPosition;
    }
}
