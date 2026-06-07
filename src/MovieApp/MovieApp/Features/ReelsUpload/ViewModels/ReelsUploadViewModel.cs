using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Features.ReelsUpload;

namespace MovieApp.Features.ReelsUpload.ViewModels
{
    /// <summary>
    /// ViewModel for the Reels Upload page.
    /// </summary>
    public partial class ReelsUploadViewModel : ObservableObject
    {
        private readonly IVideoStorageService videoStorageService;
        private readonly IMovieService movieService;
        private readonly Microsoft.UI.Dispatching.DispatcherQueue? _dispatcherQueue;

        private const string UntitledName = "Untitled Reel";
        private const string VideoFileExtension = ".mp4";

        private List<Movie> _allMovies = new List<Movie>();

        public ObservableCollection<Movie> SuggestedMovies { get; }

        public ReelsUploadViewModel(
            IVideoStorageService videoStorageService,
            IMovieService movieService)
        {
            this.videoStorageService = videoStorageService;
            this.movieService = movieService;
            SuggestedMovies = new ObservableCollection<Movie>();
            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            _ = LoadMoviesAsync();
        }

        private void RunOnUi(Action action)
        {
            var dispatcher = _dispatcherQueue ?? App.MainWindow?.DispatcherQueue ?? Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            if (dispatcher != null && !dispatcher.HasThreadAccess)
            {
                dispatcher.TryEnqueue(() => action());
            }
            else
            {
                action();
            }
        }

        [ObservableProperty]
        private string pageTitle = "Reels Upload";

        [ObservableProperty]
        private string statusMessage = "Ready to upload.";

        private const int CurrentUserID = 1;

        [ObservableProperty]
        private string reelTitle = string.Empty;

        [ObservableProperty]
        private string reelCaption = string.Empty;

        [ObservableProperty]
        private Movie? linkedMovie;

        [ObservableProperty]
        private string localVideoFilePath = string.Empty;

        private async Task LoadMoviesAsync()
        {
            try
            {
                var movies = await movieService.GetAllMoviesAsync();
                _allMovies = movies.ToList();
            }
            catch (Exception exception)
            {
                RunOnUi(() => {
                    StatusMessage = $"Failed to load movies: {exception.Message}";
                });
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private void WriteLog(string message)
        {
            try
            {
                string logPath = @"d:\Personale\UBB-SE-2026-925-1\UBB-SE-2026-MovieApp\debug_picker.log";
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\r\n");
            }
            catch { }
        }

        [RelayCommand]
        private async Task SelectVideoFileAsync()
        {
            WriteLog("SelectVideoFileAsync started.");
            try
            {
                WriteLog("Creating FileOpenPicker...");
                var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
                filePicker.FileTypeFilter.Add(VideoFileExtension);
                WriteLog("FileOpenPicker created.");

                WriteLog("Getting MainWindow handle...");
                IntPtr windowHandle = IntPtr.Zero;
                try
                {
                    var window = App.MainWindow;
                    if (window != null)
                    {
                        windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        WriteLog($"WindowNative returned handle: {windowHandle}");
                    }
                    else
                    {
                        WriteLog("App.MainWindow is null.");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"WindowNative.GetWindowHandle threw: {ex.Message}");
                }

                if (windowHandle == IntPtr.Zero)
                {
                    try
                    {
                        windowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                        WriteLog($"Process.GetCurrentProcess().MainWindowHandle returned: {windowHandle}");
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"Process.MainWindowHandle threw: {ex.Message}");
                    }
                }

                if (windowHandle == IntPtr.Zero)
                {
                    try
                    {
                        windowHandle = GetActiveWindow();
                        WriteLog($"GetActiveWindow returned: {windowHandle}");
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"GetActiveWindow threw: {ex.Message}");
                    }
                }

                if (windowHandle == IntPtr.Zero)
                {
                    WriteLog("No valid window handle found. Aborting.");
                    RunOnUi(() => {
                        StatusMessage = "Error: Could not retrieve a valid window handle.";
                    });
                    return;
                }

                WriteLog("Initializing FileOpenPicker with window handle...");
                WinRT.Interop.InitializeWithWindow.Initialize(filePicker, windowHandle);
                WriteLog("Initialization completed.");

                WriteLog("Calling PickSingleFileAsync...");
                var selectedFile = await filePicker.PickSingleFileAsync();
                WriteLog($"PickSingleFileAsync completed. selectedFile is null? {selectedFile == null}");

                if (selectedFile != null)
                {
                    WriteLog("Queueing UI update...");
                    RunOnUi(() => {
                        WriteLog("UI update execution started.");
                        try
                        {
                            WriteLog("Accessing selectedFile.Path...");
                            string pickedPath = selectedFile.Path;
                            WriteLog($"selectedFile.Path = {pickedPath}");

                            WriteLog("Accessing selectedFile.Name...");
                            string pickedName = selectedFile.Name;
                            WriteLog($"selectedFile.Name = {pickedName}");

                            WriteLog("Setting LocalVideoFilePath and StatusMessage...");
                            LocalVideoFilePath = pickedPath;
                            StatusMessage = $"Selected: {pickedName}";
                            WriteLog("Properties updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            WriteLog($"Exception inside UI update callback: {ex.Message}");
                            StatusMessage = $"Error retrieving file details: {ex.Message}";
                        }
                    });
                }
                WriteLog("Calling GC.KeepAlive...");
                GC.KeepAlive(filePicker);
                WriteLog("SelectVideoFileAsync finished successfully.");
            }
            catch (Exception ex)
            {
                WriteLog($"Exception caught in SelectVideoFileAsync outer block: {ex.Message}\r\nStack Trace: {ex.StackTrace}");
                RunOnUi(() => {
                    StatusMessage = $"Could not open file picker: {ex.Message}";
                });
            }
        }

        [RelayCommand]
        private async Task UploadReelAsync()
        {
            if (string.IsNullOrWhiteSpace(LocalVideoFilePath))
            {
                StatusMessage = "Please select a video first!";
                return;
            }

            if (string.IsNullOrWhiteSpace(ReelTitle))
            {
                StatusMessage = "Please enter a title for the reel!";
                return;
            }

            if (LinkedMovie == null)
            {
                StatusMessage = "Please link a movie to the reel!";
                return;
            }

            StatusMessage = "Validating video format...";

            try
            {
                // FIX FOR PART 4: Validate locally instead of calling the proxy service
                bool isValid = ValidateVideoLocally(LocalVideoFilePath);

                if (!isValid)
                {
                    StatusMessage = "Invalid file! Must be a non-empty MP4 file.";
                    return;
                }

                StatusMessage = "Uploading to Blob Storage & saving metadata...";

                ReelUploadRequest request = new ReelUploadRequest
                {
                    LocalFilePath = LocalVideoFilePath,
                    Title = ReelTitle,
                    Caption = ReelCaption ?? string.Empty,
                    UploaderUserId = CurrentUserID,
                    MovieId = LinkedMovie.Id
                };

                Reel savedReel = await videoStorageService.UploadVideoAsync(request);

                RunOnUi(() => {
                    StatusMessage = $"Success! Reel uploaded with ID {savedReel.Id}.";
                    LocalVideoFilePath = string.Empty;
                    ReelTitle = string.Empty;
                    ReelCaption = string.Empty;
                    LinkedMovie = null;
                });
            }
            catch (Exception exception)
            {
                RunOnUi(() => {
                    StatusMessage = $"Upload Failed: {exception.Message}";
                });
            }
        }

        private bool ValidateVideoLocally(string localFilePath)
        {
            if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath))
                return false;

            string fileExtension = Path.GetExtension(localFilePath).ToLowerInvariant();
            return fileExtension == VideoFileExtension;
        }

        [RelayCommand]
        private void SelectMovie(Movie movieToSelect)
        {
            LinkedMovie = movieToSelect;
        }

        [RelayCommand]
        private void SearchMovie(string partialMovieName)
        {
            SuggestedMovies.Clear();

            if (string.IsNullOrWhiteSpace(partialMovieName))
            {
                return;
            }

            var filteredMovies = _allMovies
                .Where(movie => movie.Title.Contains(partialMovieName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (Movie movie in filteredMovies)
            {
                SuggestedMovies.Add(movie);
            }
        }
    }
}
