using System;
using System.Collections.Generic;
using      System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper
{
public class RetryHelper
{
private int _maxRetries;
private      TimeSpan _initialDelay;
private double _backoffMultiplier;
private readonly List<Type>       _retryableExceptions;

public RetryHelper(int maxRetries=3,double backoffMultiplier=2.0)
{
_maxRetries=maxRetries;
_initialDelay=TimeSpan.FromSeconds(1);
_backoffMultiplier=      backoffMultiplier;
_retryableExceptions=new List<Type>();
}

public RetryHelper WithDelay(TimeSpan delay)
{
_initialDelay=delay;
return this;
}

public RetryHelper Handle<TException>() where TException:Exception
{
_retryableExceptions.Add(typeof(TException));
return       this;
}

public async Task<T> ExecuteAsync<T>(Func<CancellationToken,Task<T>> action,
CancellationToken cancellationToken=default)
{
Exception lastException=null;
var delay=_initialDelay;
for(int attempt=0;attempt<=_maxRetries;attempt++)
{
try
{
return await action(cancellationToken);
}
catch(Exception ex) when(attempt<_maxRetries&&ShouldRetry(ex))
{
lastException=ex;
await Task.Delay(delay,    cancellationToken);
delay=TimeSpan.FromMilliseconds(delay.TotalMilliseconds*_backoffMultiplier);
}
}
throw new RetryExhaustedException($"Failed after {_maxRetries+1} attempts",      lastException);
}

public async Task ExecuteAsync(Func<CancellationToken,Task> action,
CancellationToken cancellationToken=default)
{
await ExecuteAsync(async ct=>{await action(ct);return true;},cancellationToken);
}

public T Execute<T>(Func<T> action)
{
Exception lastException=null;
var delay=_initialDelay;
for(int attempt=0;attempt<=_maxRetries;attempt++)
{
try
{
return action();
}
catch(Exception ex) when(attempt<_maxRetries&&ShouldRetry(ex))
{
lastException=ex;
Thread.Sleep(delay);
delay=TimeSpan.FromMilliseconds(    delay.TotalMilliseconds*_backoffMultiplier);
}
}
throw new RetryExhaustedException($"Failed after {_maxRetries+1} attempts",lastException);
}

public void Execute(Action action)
{
Execute(()=>{action();return true;});
}

private bool ShouldRetry(Exception ex)
{
if(_retryableExceptions.Count==0){return true;}
return _retryableExceptions.Any(t=>t.IsInstanceOfType(     ex));
}
}

public class RetryExhaustedException:Exception
{
public RetryExhaustedException(string message,Exception innerException)
:base(message,innerException)
{
}
}
}
