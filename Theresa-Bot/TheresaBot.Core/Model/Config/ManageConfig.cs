namespace TheresaBot.Core.Model.Config
{
    public record ManageConfig : BaseConfig
    {
        public List<string> PixivCookieCommands { get; set; } = new();

        public List<string> PixivTokenCommands { get; set; } = new();

        public List<string> SaucenaoCookieCommands { get; set; } = new();

        public List<string> DisableTagCommands { get; set; } = new();

        public List<string> EnableTagCommands { get; set; } = new();

        public List<string> DisableMemberCommands { get; set; } = new();

        public List<string> EnableMemberCommands { get; set; } = new();

        public List<string> ListSubCommands { get; set; } = new();

        public List<string> RemoveSubCommands { get; set; } = new();

        public List<string> BindTagCommands { get; set; } = new();

        public List<string> UnBindTagCommands { get; set; } = new();

        public List<string> DisablePixiverCommands { get; set; } = new();

        public List<string> EnablePixiverCommands { get; set; } = new();

        public override ManageConfig FormatConfig()
        {
            if (PixivCookieCommands is null) PixivCookieCommands = new();
            if (PixivTokenCommands is null) PixivTokenCommands = new();
            if (SaucenaoCookieCommands is null) SaucenaoCookieCommands = new();
            if (DisableTagCommands is null) DisableTagCommands = new();
            if (EnableTagCommands is null) EnableTagCommands = new();
            if (DisableMemberCommands is null) DisableMemberCommands = new();
            if (EnableMemberCommands is null) EnableMemberCommands = new();
            if (ListSubCommands is null) ListSubCommands = new();
            if (RemoveSubCommands is null) RemoveSubCommands = new();
            if (BindTagCommands is null) BindTagCommands = new();
            if (UnBindTagCommands is null) UnBindTagCommands = new();
            if (DisablePixiverCommands is null) DisablePixiverCommands = new();
            if (EnablePixiverCommands is null) EnablePixiverCommands = new();
            return this;
        }

    }
}
