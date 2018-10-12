namespace Empower_List
{
    public class UserInfo
    {
        public string Name { get; set; }
        public UserGroup Group { get; set; }
        public string Token { get; set; }
        public int AuthType { get; set; }
        public UserStatus Status { get; set; }
        public UserInfo() : this("", UserGroup.analyst, "", 1, UserStatus.enabled) { }
        public UserInfo(string name, UserGroup group, string token, int authType, UserStatus status)
        {
            Name = name;
            Group = group;
            Token = token;
            AuthType = authType;
            Status = status;
        }
    }
}
