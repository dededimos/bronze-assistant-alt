using BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService;
using System.Threading;

namespace BronzeFactoryApplication.ApplicationServices.StockGlassesService
{
    public class GlassMatchingService : IDisposable
    {
        public event EventHandler? MatchingFinished;
        public event EventHandler? MatchingStarted;
        private void RaiseMatchingFinished()
        {
            MatchingFinished?.Invoke(this, EventArgs.Empty);
        }
        private void RaiseMatchingStarted()
        {
            MatchingStarted?.Invoke(this, EventArgs.Empty);
        }

        private readonly GlassesStockService _stockService;
        private readonly IGlassesStockSettingsProvider settingsProvider;

        /// <summary>
        /// A Counter for the active Tasks of the Service (Will be busy if tasks are running)
        /// </summary>
        private int _tasksCounter;
        private readonly object _taskCounterLock = new();

        /// <summary>
        /// A Token to cancel any matching when a new one starts
        /// </summary>
        private CancellationTokenSource? _cancellationTokenSource;
                
        public List<GlassMatches> RecentMatches { get; set; } = new();

        public GlassMatchingService(GlassesStockService stockService, IGlassesStockSettingsProvider settingsProvider)
        {
            _stockService = stockService;
            this.settingsProvider = settingsProvider;
        }

        /// <summary>
        /// Gets any Similar Glasses avaiable in stock that can be Used instead of the Glass that is being compared
        /// </summary>
        /// <param name="glassToCheck">The Glass to check</param>
        /// <param name="identifier">The Cabin Identifier where the glass belongs to</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A GlassMatches Object containing info for the matching</returns>
        private GlassMatches GetSimilarGlassFromStock(Glass glassToCheck, GlassesStockSettings settings, CabinIdentifier identifier, CancellationToken token)
        {
            //Get a copy Enumerable of Current StockList to filter out anything that is not needed
            IEnumerable<StockedGlassRow> filteredStock = _stockService.StockList.Where(r=> r.Glass.Draw == glassToCheck.Draw);

            //Filter out any glasses that do Not Match According to the Provided Settings

            if (settings.ShouldCompareHeight)
            {
                filteredStock = filteredStock.Where(g => Math.Abs(glassToCheck.Height - g.Glass.Height) <= settings.AllowedHeightDifference);
            }

            if (settings.ShouldCompareLength)
            {
                filteredStock = filteredStock.Where(g => Math.Abs(glassToCheck.Length - g.Glass.Length) <= settings.AllowedLengthDifference);
            }

            if (settings.ShouldCompareFinish)
            {
                filteredStock = filteredStock.Where(g => glassToCheck.Finish == g.Glass.Finish);
            }

            if (settings.ShouldCompareThickness)
            {
                filteredStock = filteredStock.Where(g => glassToCheck.Thickness == g.Glass.Thickness);
            }

            //Glass Matches Found
            var matches = new GlassMatches(identifier, glassToCheck, filteredStock);
            
            token.ThrowIfCancellationRequested();
            return matches;
        }
        /// <summary>
        /// Returns all the Glass Matches for the Cabin under Check
        /// </summary>
        /// <param name="cabinToCheck"></param>
        /// <returns></returns>
        private IEnumerable<GlassMatches> MatchCabinGlassesFromStock(Cabin? cabinToCheck,GlassesStockSettings? settings, CancellationToken token)
        {
            if (cabinToCheck?.Model is null || settings?.ConcernsModel is null) return Enumerable.Empty<GlassMatches>();

            List<GlassMatches> matches = new();
            //One match for each Glass
            foreach (var glass in cabinToCheck.Glasses)
            {
                token.ThrowIfCancellationRequested();
                var match = GetSimilarGlassFromStock(glass, settings,cabinToCheck.Identifier(),token);
                if(match.HasMatches) matches.Add(match);
            }
            return matches;
        }
        /// <summary>
        /// Matches Glasses for a List of Cabins
        /// </summary>
        /// <param name="cabinsToCheck">The List to Check for Matches</param>
        /// <returns></returns>
        public async Task<IEnumerable<GlassMatches>> MatchCabinsGlassesFromStock(params (Cabin? cabin,GlassesStockSettings? settings)[] cabinsToCheck)
        {
            // Add to the Tasks Counter
            // lock to prevent the mixing of the Raise of Start and End Matches Events
            lock(_taskCounterLock)
            {
                Interlocked.Increment(ref _tasksCounter);
                RaiseMatchingStarted();
            }
            
            // Cancel any previous Task
            _cancellationTokenSource?.Cancel();
            
            // Create a new Token for the New Task
            _cancellationTokenSource = new();
            CancellationToken token = _cancellationTokenSource.Token;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                RecentMatches.Clear();
            });
            
            List<GlassMatches> matchesTotal = new();
            try
            {
                foreach (var cabin in cabinsToCheck)
                {
                    //Check if a cancellation was requested
                    token.ThrowIfCancellationRequested();
                    var matches = MatchCabinGlassesFromStock(cabin.cabin,cabin.settings,token);
                    if(matches.Any()) matchesTotal.AddRange(matches);
                }
                return matchesTotal;
            }
            catch (OperationCanceledException)
            {
                //Return any matches that managed to Finish if Any
                return matchesTotal;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                lock (_taskCounterLock)
                {
                    Interlocked.Decrement(ref _tasksCounter);
                    if(_tasksCounter == 0) RaiseMatchingFinished();
                }
            }
        }

        /// <summary>
        /// Cancels any running Tasks
        /// </summary>
        public void CancelRunningTask()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            //item already disposed above code , supress finalize
            GC.SuppressFinalize(this);
        }
        
        private bool _disposed;
        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = null;
                RecentMatches.Clear();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }
}
