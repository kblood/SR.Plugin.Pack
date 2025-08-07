using System;
using System.IO;

namespace SatelliteReignModdingTools.Services
{
    // Simple, reusable file watcher to auto-reload managers when XML changes on disk.
    public sealed class DataWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        private readonly Action _onChanged;
        private readonly string _filter;

        public DataWatcher(string directory, string filter, Action onChanged)
        {
            _filter = string.IsNullOrWhiteSpace(filter) ? "*.xml" : filter;
            _onChanged = onChanged ?? (() => { });

            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return;

            _watcher = new FileSystemWatcher(directory, _filter)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.CreationTime,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };
            _watcher.Changed += OnFsEvent;
            _watcher.Created += OnFsEvent;
            _watcher.Renamed += OnFsEvent;
            _watcher.Deleted += OnFsEvent;
        }

        private void OnFsEvent(object sender, FileSystemEventArgs e)
        {
            try
            {
                // Debounce a bit to avoid partial write reads
                System.Threading.Tasks.Task.Delay(200).ContinueWith(_ => _onChanged());
            }
            catch { }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Changed -= OnFsEvent;
                _watcher.Created -= OnFsEvent;
                _watcher.Renamed -= OnFsEvent;
                _watcher.Deleted -= OnFsEvent;
                _watcher.Dispose();
                _watcher = null;
            }
        }
    }
}
