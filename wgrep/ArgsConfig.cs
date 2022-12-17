namespace wgrep
{
    internal class ArgsConfig
    {
        private const int MAX_ARGS = 6;

        public bool Color { get; private set; } = false;
        public bool Count { get; private set; } = false;
        public string FilePath { get; private set; } = "";
        public int LinesAfter { get; private set; } = 0;
        public int LinesBefore { get; private set; } = 0;
        public string SearchPhrase { get; private set; } = "";

        public bool TrackingLinesBefore { get; private set; } = false;

        public bool ParseArgs(string[] args)
        {
            if (args.Length == 0 || args.Length > MAX_ARGS)
            {
                return false;
            }

            for (int x = 0; x < args.Length; x++)
            {
                switch (args[x])
                {
                    case "-A":
                        if (x + 1 < args.Length && int.TryParse(args[x+1], out int resultA))
                        {
                            LinesAfter = resultA;
                        }
                        else
                        {
                            Console.WriteLine("Invalid/missing option value");
                            return false;
                        }

                        break;
                    case "-B":
                        if (x + 1 < args.Length && int.TryParse(args[x + 1], out int resultB))
                        {
                            LinesBefore = resultB;
                            TrackingLinesBefore = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid/missing option value");
                            return false;
                        }

                        break;
                    case "-c":
                        Count = true;
                        break;
                    case "--color":
                        Color = true;
                        break;
                    default:
                        if (x == args.Length - 2)
                        {
                            SearchPhrase = args[x];
                        }
                        else if (x == args.Length - 1)
                        {
                            FilePath = args[x];
                        }

                        break;
                }
            }

            if (FilePath == "" || SearchPhrase == "")
            {
                return false;
            }

            return true;
        }

        public void PrintArgs()
        {
            Console.WriteLine($"Lines after: {LinesAfter}");
            Console.WriteLine($"Lines before: {LinesBefore}");
            Console.WriteLine($"File path: {FilePath}");
            Console.WriteLine($"Search phrase: {SearchPhrase}");
        }
    }
}
