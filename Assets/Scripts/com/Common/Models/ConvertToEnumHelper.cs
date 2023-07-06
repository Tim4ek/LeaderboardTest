using System.Collections.Generic;

namespace com.Common.Models {
  public static class ConvertToEnumHelper {

    private static Dictionary<string, UserType> _userType = new Dictionary<string, UserType> {
      {
      UserType.Default.ToString(),
      UserType.Default
      },
      {
      UserType.Bronze.ToString(),
      UserType.Bronze
      },
      {
      UserType.Silver.ToString(),
      UserType.Silver
      },
      {
      UserType.Gold.ToString(),
      UserType.Gold
      },
      {
      UserType.Diamond.ToString(),
      UserType.Diamond
      }
    };

    public static UserType ToEnum(this string value, UserType defaultValue) {
      if (!_userType.TryGetValue(value, out UserType value2)) {
        return defaultValue;
      }
      return value2;
    }
  }
}