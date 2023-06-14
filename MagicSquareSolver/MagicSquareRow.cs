namespace MagicSquareSolver;

public class MagicSquareRow
{
    public Square[] Squares;

    public MagicSquareRow(params Square[] squares)
    {
        Squares = squares;
        Sum = Squares[0].NumSquare + Squares[1].NumSquare + Squares[2].NumSquare;
    }

    public uint Sum { get; set; }
    public bool Match { get; set; }

    public void MarkMatch()
    {
        Match = true;
        foreach (var square in Squares)
        {
            square.Keep = true;
        }
    }
}