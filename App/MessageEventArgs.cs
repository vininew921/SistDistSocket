namespace App;

internal class MessageEventArgs : EventArgs
{
    public string Message { get; private set; }
    public bool SystemMessage { get; set; }

    public MessageEventArgs(string message, bool systemMessage = false)
    {
        Message = message;
        SystemMessage = systemMessage;
    }
}