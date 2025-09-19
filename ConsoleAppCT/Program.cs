namespace ConsoleAppCT
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                if (args.Length > 0 && args[0].Equals("--ctrlc", StringComparison.OrdinalIgnoreCase))
                {
                    await RunUntilCtrlCAsync();
                }
                else
                {
                    await RunTimedAsync(TimeSpan.FromMilliseconds(4500));
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex}");
                return 1;
            }
        }

        /* oprire după 4.5s => foloseste CancellationTokenSource.CancelAfter
         dotnet run -- --timed
         */
        private static async Task RunTimedAsync(TimeSpan duration) 
        {
            using var cts = new CancellationTokenSource(duration);
            Console.WriteLine($"\n[Timed] se porneste Fibonacci pentru {duration.TotalSeconds:0.0}s si 1s intre pasi:");
            await FibonacciLoopAsync(cts.Token);
            Console.WriteLine("Unreachable");
        }

        /* Ctrl-C pentru stop => anulare via Console.CancelKeyPress
         dotnet run -- --ctrlc
         */
        private static async Task RunUntilCtrlCAsync()
        {
            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                if (!cts.IsCancellationRequested)
                {
                    Console.WriteLine();
                    Console.WriteLine("apasat Ctrl-C: anulare ceruta si oprire");
                    cts.Cancel();
                }
            };

            Console.WriteLine("\n[Ctrl-C] se porneste Fibonacci; se opreste la apasare Ctrl-C si 1s intre pasi");
            await FibonacciLoopAsync(cts.Token);
            Console.WriteLine("Unreachable");
        }

        private static async Task FibonacciLoopAsync(CancellationToken token)
        {
            long a = 0;
            long b = 1;

            Console.WriteLine(a);
            token.ThrowIfCancellationRequested();

            Console.WriteLine(b);
            token.ThrowIfCancellationRequested();

            while (true)
            {
                checked
                {
                    long next = a + b;
                    a = b;
                    b = next;
                }

                Console.WriteLine(b);

                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
        }
    }
}
