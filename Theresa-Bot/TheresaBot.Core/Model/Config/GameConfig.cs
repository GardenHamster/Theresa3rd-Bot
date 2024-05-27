namespace TheresaBot.Core.Model.Config
{
    public record GameConfig : BasePluginConfig
    {
        public List<string> JoinCommands { get; set; } = new();

        public List<string> StartCommands { get; set; } = new();

        public List<string> StopCommands { get; set; } = new();

        public UndercoverConfig Undercover { get; set; }

        public override GameConfig FormatConfig()
        {
            if (JoinCommands is null) JoinCommands = new();
            if (StartCommands is null) StartCommands = new();
            if (StopCommands is null) StopCommands = new();
            Undercover?.FormatConfig();
            return this;
        }
    }

    public record UndercoverConfig : BaseGameConfig
    {
        public List<string> CreateCommands { get; set; } = new();

        public List<string> AddWordCommands { get; set; } = new();

        public List<string> SendWordCommands { get; set; } = new();

        public int AddWordLimits { get; set; } = 10;

        public bool SendIdentity { get; set; } = false;

        public bool PrivateVote { get; set; } = false;

        public int FirstRoundNonVoting { get; set; } = 3;

        public decimal MaxSimilarity { get; set; }

        public int MatchSeconds { get; set; } = 120;

        public int PrepareSeconds { get; set; } = 5;

        public int SpeakingSeconds { get; set; } = 120;

        public int VotingSeconds { get; set; } = 60;

        public int FailedMuteSeconds { get; set; } = 0;

        public int ViolatedMuteSeconds { get; set; } = 0;

        public string RuleMsg { get; set; }

        public override BaseConfig FormatConfig()
        {
            if (CreateCommands is null) CreateCommands = new();
            if (AddWordCommands is null) AddWordCommands = new();
            if (SendWordCommands is null) SendWordCommands = new();
            if (AddWordLimits < 0) AddWordLimits = 0;
            if (MatchSeconds < 30) MatchSeconds = 30;
            if (PrepareSeconds < 5) PrepareSeconds = 5;
            if (SpeakingSeconds < 10) SpeakingSeconds = 10;
            if (VotingSeconds < 10) VotingSeconds = 10;
            if (FailedMuteSeconds < 0) FailedMuteSeconds = 0;
            if (ViolatedMuteSeconds < 0) ViolatedMuteSeconds = 0;
            return this;
        }
    }


}
