using System.Collections.Generic;

public class SaveData
{
    public const string StartLevel = "L1-1";

    public string CurrentLevel = StartLevel;

    public int Lives = 3;

    public int BlocksHit = 0;

    public List<string> Checkpoints = new List<string>();
}

