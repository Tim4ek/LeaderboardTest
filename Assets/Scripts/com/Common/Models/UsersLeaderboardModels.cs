using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace com.Common.Models {
  public class UsersLeaderboardModels {
    public Collection<UserModel> leaderboard;
  }

  public class UserModel {
    public string name;
    public int score;
    public string avatar;
    public string type;
    [JsonIgnore]
    public UserType Type => type.ToEnum(UserType.Default);
    [JsonIgnore]
    public int Index {
      get;
      set;
    }
    
  }

  public enum UserType {
    Default,
    Bronze,
    Silver,
    Gold,
    Diamond
  }
}
