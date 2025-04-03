[System.Serializable]
public class TicTacToeData
{
    public int actual;
    public int round;
    public int winner;
    public string score1;
    public string score2;
    public int[] board;
    
    override public string ToString()
    {
        string data = "actual:" + actual + "\nround:" + round + "\nscore1" + score1 + "\nscore2" + score2 + "\nwinner:" + winner +  "\nboard\n";
        foreach (var item in board)
        {
            data += item + "\n";
        }
        return data;
    }
}

