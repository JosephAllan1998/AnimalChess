namespace AnimalChess.Models
{
    public enum Level
    {
        Elephant = 8, Lion = 7, Tiger = 6,
        Leopard = 5, Dog = 4, Wolf = 3,
        Cat = 2, Rat = 1, Dangerous = 0
    }

    public enum Swim
    {
        Jump = 2,
        Available = 1,
        NotAvailable = 0
    }

    public enum Player
    {
        Red = 1,
        Blue = 2
    }

    public enum Mode
    {
        OnePlayer = 1,
        TwoPlayers = 2,
        Computer = 0
    }
    public enum Direction
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

}
