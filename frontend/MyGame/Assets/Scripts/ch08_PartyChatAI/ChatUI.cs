public static class ChatUI
{
    private static string textArea = "\n\n\n\n\n";
    private static string[] chatRows = new string[5];
    public static void UpdateTextArea(string displayName, string message)
    {
        for (int i = 0; i < 4; i++)
        {
            chatRows[i] = chatRows[i + 1];
        }
        chatRows[4] = displayName + ": " + message;

        string ta = "";

        for (int i = 0; i < 5; i++)
        {
            if (chatRows[i] == null)
            {
                ta += "\n";
            }
            else
            {
                ta += chatRows[i] + "\n";
            }
        }
        textArea = ta;
    }

    public static string GetTextArea()
    {
        return textArea;
    }

    public static void UpdateDisplayName(string entityId, string displayName)
    {
        textArea = textArea.Replace(entityId, displayName);
        chatRows[4] = chatRows[4].Replace(entityId, displayName);
    }

}

