/// <summary>
/// 奖励信息的类
/// </summary>
public class SeasonGood
{
    public int ScoreRequired { get; set; }
    public int Reward { get; set; }

    public SeasonGood()
    {
    }

    public SeasonGood(int score, int reward)
    {
        ScoreRequired = score;
        Reward = reward;
    }
}