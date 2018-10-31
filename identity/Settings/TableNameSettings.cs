namespace identity.Settings
{
    public class TableNameSettings
    {
        public string UsersTableName { get; set; }
        public string RolesTableName { get; set; }
        public string RoleClaimsTableName { get; set; }
        public string UserClaimsTableName { get; set; }
        public string UserLoginsTableName { get; set; }
        public string UserRolesTableName { get; set; }
        public string UserTokensTableName { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }
}