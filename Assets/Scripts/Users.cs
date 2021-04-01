/// <summary>
/// 用户类型的数据
/// </summary>
public class Users
{
    public long Id { get; set; }
    public string Nickname { get; set; }
    public int Score { get; set; }
    public int Coins { get; set; }
    public int RewardsGot { get; set; }

    public Users()
    {
    }

    public Users(long id, string nickname, int score,int coins,int rewardsGot)
    {
        Id = id;
        Nickname = nickname;
        Score = score;
        Coins = coins;
        RewardsGot = rewardsGot;
    }
}