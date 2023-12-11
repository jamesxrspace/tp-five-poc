namespace TPFive.Game.Record.Entry
{
    public class SessionEndOption
    {
        public bool SaveToFile { get; set; }

        public bool EnableExport { get; set; }

        public RecordData[] Footage { get; set; }

        public static SessionEndOption Drop()
        {
            return new SessionEndOption
            {
                SaveToFile = false,
                EnableExport = false,
            };
        }

        public static SessionEndOption SaveOnly()
        {
            return new SessionEndOption
            {
                SaveToFile = true,
                EnableExport = false,
            };
        }

        public static SessionEndOption ExportOnly()
        {
            return new SessionEndOption
            {
                SaveToFile = false,
                EnableExport = true,
            };
        }

        public static SessionEndOption SaveAndExport()
        {
            return new SessionEndOption
            {
                SaveToFile = true,
                EnableExport = true,
            };
        }
    }
}