namespace App;

internal class RoomUser
{
    private static readonly List<RoomUser> _users = new List<RoomUser>();
    private static readonly List<ConsoleColor> _availableColors = new List<ConsoleColor>();
    public string Username { get; set; }
    public ConsoleColor Color { get; set; }

    public RoomUser(string username)
    {
        Random random = new Random();

        if (!_availableColors.Any())
        {
            for (int i = 1; i < 16; i++)
            {
                _availableColors.Add((ConsoleColor)i);
            }
        }

        Username = username;

        int randomColor = random.Next(0, _availableColors.Count);
        Color = _availableColors[randomColor];

        _availableColors.RemoveAt(randomColor);

        _users.Add(this);
    }

    public static RoomUser? GetFromMessage(string message)
    {
        int startUser = message.IndexOf("<");
        int endUser = message.IndexOf(">");

        if (startUser != -1 && endUser != -1)
        {
            string username = message.Substring(startUser + 1, startUser + endUser - 1);

            RoomUser? user = _users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                user = new RoomUser(username);
            }

            return user;
        }

        return null;
    }
}
