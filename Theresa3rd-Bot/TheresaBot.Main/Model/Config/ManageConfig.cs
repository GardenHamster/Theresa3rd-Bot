namespace TheresaBot.Main.Model.Config
{
    public record ManageConfig : BaseConfig
    {
        public List<string> PixivCookieCommands { get; private set; }

        public List<string> SaucenaoCookieCommands { get; private set; }

        public List<string> DisableTagCommands { get; private set; }

        public List<string> EnableTagCommands { get; private set; }

        public List<string> DisableMemberCommands { get; private set; }

        public List<string> EnableMemberCommands { get; private set; }

        public List<string> ListSubCommands { get; private set; }

        public List<string> RemoveSubCommands { get; private set; }

        public List<string> BindTagCommands { get; private set; }

        public List<string> UnBindTagCommands { get; private set; }

        public override ManageConfig FormatConfig()
        {
            if (PixivCookieCommands is null) PixivCookieCommands = new();
            if (SaucenaoCookieCommands is null) SaucenaoCookieCommands = new();
            if (DisableTagCommands is null) DisableTagCommands = new();
            if (EnableTagCommands is null) EnableTagCommands = new();
            if (DisableMemberCommands is null) DisableMemberCommands = new();
            if (EnableMemberCommands is null) EnableMemberCommands = new();
            if (ListSubCommands is null) ListSubCommands = new();
            if (RemoveSubCommands is null) RemoveSubCommands = new();
            if (BindTagCommands is null) BindTagCommands = new();
            if (UnBindTagCommands is null) UnBindTagCommands = new();
            return this;
        }

    }
}
