using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Fusion;
using Microsoft.Extensions.Logging;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace TPFive.Extended.Fusion
{
    public abstract class ObservableNetworkBehaviour : NetworkBehaviour, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs NullEventArgs = new (null);
        private static readonly Dictionary<string, PropertyChangedEventArgs> PropertyEventArgs = new ();
        private readonly object @lock = new ();

        [Inject]
        private ILoggerFactory loggerFactory;
        private ILogger logger;

        private PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (@lock)
                {
                    propertyChanged += value;
                }
            }

            remove
            {
                lock (@lock)
                {
                    propertyChanged -= value;
                }
            }
        }

        private ILogger Logger => logger ??= loggerFactory.CreateLogger<ObservableNetworkBehaviour>();

        protected ILogger CreateLogger<T>() => loggerFactory.CreateLogger<T>();

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            RaisePropertyChanged(GetPropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs">Property changed event.</param>
        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            try
            {
                propertyChanged?.Invoke(this, eventArgs);
            }
            catch (Exception e)
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                {
                    Logger.LogWarning("Set property '{0}', raise PropertyChanged failure.Exception:{1}", eventArgs.PropertyName, e);
                }
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs">Property changed events.</param>
        protected virtual void RaisePropertyChanged(params PropertyChangedEventArgs[] eventArgs)
        {
            foreach (var args in eventArgs)
            {
                try
                {
                    propertyChanged?.Invoke(this, args);
                }
                catch (Exception e)
                {
                    if (Logger.IsEnabled(LogLevel.Warning))
                    {
                        Logger.LogWarning("Set property '{0}', raise PropertyChanged failure.Exception:{1}", args.PropertyName, e);
                    }
                }
            }
        }

        protected virtual string ParserPropertyName(LambdaExpression propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = propertyExpression.Body as MemberExpression ?? throw new ArgumentException("Invalid argument", "propertyExpression");
            var property = body.Member as PropertyInfo ?? throw new ArgumentException("Argument is not a property", "propertyExpression");
            return property.Name;
        }

        private static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (propertyName == null)
            {
                return NullEventArgs;
            }

            if (PropertyEventArgs.TryGetValue(propertyName, out var eventArgs))
            {
                return eventArgs;
            }

            eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyEventArgs[propertyName] = eventArgs;
            return eventArgs;
        }
    }
}
