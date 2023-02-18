using System;

namespace BusTour.Scheduler.Clients
{
    /// <summary>
    /// Базовый класс клиентов.
    /// </summary>
    public abstract class ClientBase : IDisposable
    {
        protected readonly WebApiHttpClient _client;

        protected ClientBase(string baseUri)
        {
            _client = new WebApiHttpClient(baseUri);
        }

        #region Disposable

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _client.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
