namespace MagicSquareSolver;

/// <summary>
/// https://www.youtube.com/watch?v=Kdsj84UdeYg&t=545s
/// </summary>
public class MagicSquare
{
    private MagicSquareRow[] _rows;
    private Square[,] _squares;
    private readonly Random _random;
    private uint? _finder;
    private long _iterations = 0;
    private int _resets = 0;
    private readonly List<uint> _badFinders;
    public MagicSquare(int? seed = null)
    {
        _badFinders = new List<uint>();
        _random = seed != null ? new Random(seed.Value) : new Random();
        _rows = new MagicSquareRow[8];
        _squares = new Square[3, 3];
    }

    public void Build()
    {
        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
            {
                if (_squares[x, y] != null && _squares[x, y].Keep)
                {
                    continue;
                }

                var num = _random.Next(int.MaxValue);
                var sqr = new Square()
                {
                    Num = num,
                    NumSquare = (uint)(num * num),
                    X = x,
                    Y = y
                };
                _squares[x, y] = sqr;
            }
        }

        PopulateRow(0, _squares[0, 0], _squares[1, 0], _squares[2, 0]);
        PopulateRow(1, _squares[0, 1], _squares[1, 1], _squares[2, 1]);
        PopulateRow(2, _squares[0, 2], _squares[1, 2], _squares[2, 2]);
        PopulateRow(3, _squares[0, 0], _squares[0, 1], _squares[0, 2]);
        PopulateRow(4, _squares[1, 0], _squares[1, 1], _squares[1, 2]);
        PopulateRow(5, _squares[2, 0], _squares[2, 1], _squares[2, 2]);
        PopulateRow(6, _squares[2, 0], _squares[1, 1], _squares[0, 2]);
        PopulateRow(7, _squares[0, 0], _squares[1, 1], _squares[2, 2]);
    }

    private void PopulateRow(int rowNum, params Square[] squares)
    {
        if (_rows[rowNum] == null || !_rows[rowNum].Match)
        {
            _rows[rowNum] = new MagicSquareRow(squares);
        }
    }

    public void Evaluate()
    {
        for (var x = 0; x < _rows.Length; x++)
        {
            if (_finder.HasValue && !_rows[x].Match && _rows[x].Sum == _finder)
            {
                _rows[x].MarkMatch();
                Print();
            }
            else
            {
                for (var y = 0; y < _rows.Length; y++)
                {
                    if (_finder == null && y != x && !_rows[x].Match && !_rows[y].Match && _rows[x].Sum == _rows[y].Sum)
                    {
                        if (_badFinders.Contains(_rows[x].Sum))
                        {
                            Reset("Matched Bad Finder");
                            return;
                        }
                        _finder = _rows[x].Sum;
                        _rows[y].MarkMatch();
                        _rows[x].MarkMatch();
                        Print();
                    }
                }
            }
        }
    }

    public bool Checkin(long iterations)
    {
        Console.WriteLine($"Iterations:{iterations} Resets:{_resets} Finder: {_finder?.ToString() ?? "Not Set"} Match Count: {_rows.Count(row => row.Match)}");
        if (_rows.Any(row => !row.Match && row.Squares.All(square => square.Keep)))
        {
            Reset("Dead End");
        }
        else if (_rows.All(row => row.Match))
        {
            return true;
        }
        return false;
    }

    public void Reset(string reason)
    {
        Console.WriteLine($"***** {reason} - Resetting Squares and Rows *****");
        _iterations = 0;
        _resets++;
        if (_finder != null) _badFinders.Add(_finder.Value);
        _finder = null;
        _rows = new MagicSquareRow[8];
        _squares = new Square[3, 3];
    }

    public void Run()
    {
        var isDone = false;
        while (!isDone)
        {
            Build();
            Evaluate();
            _iterations++;
            if (_iterations % 1000000 == 0)
            {
                isDone = Checkin(_iterations);
            }
        }

        Console.WriteLine("********** SOLVED **********");
        Print();
    }

    public void Print()
    {
        for (var x = 0; x < _rows.Length; x++)
        {
            Console.WriteLine($"Row: {x} Sum:{_rows[x].Sum} MATCH:{_rows[x].Match}");
            foreach (var sqr in _rows[x].Squares)
            {
                Console.WriteLine(
                    $"    Square: X:{sqr.X} Y:{sqr.Y} NUM:{sqr.Num} NUM^2:{sqr.NumSquare} MATCH:{sqr.Keep}");
            }
        }
    }
}