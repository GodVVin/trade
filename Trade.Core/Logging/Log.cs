using System;
using NLog;

namespace Trade.Core.Logging
{
    /// <summary>
    /// Logger wrapper class.
    /// </summary>
    public class Log
    {
        #region Singleton

        public static Log Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Log("Core");
                return _instance;
            }
        }

        private static Log _instance;

        #endregion

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }

        public void TraceException(string message, Exception exception)
        {
            _logger.TraceException(message, exception);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void DebugException(string message, Exception exception)
        {
            _logger.DebugException(message, exception);
        }

        public void InfoException(string message, Exception exception)
        {
            _logger.InfoException(message, exception);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void WarnException(string message, Exception exception)
        {
            _logger.WarnException(message, exception);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void ErrorException(string message, Exception exception)
        {
            _logger.ErrorException(message, exception);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void FatalException(string message, Exception exception)
        {
            _logger.FatalException(message, exception);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        #region Private members

        private readonly Logger _logger;
        
        private Log(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        #endregion
    }
}
