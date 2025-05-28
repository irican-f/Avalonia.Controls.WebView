using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.Win.WebView1.Interop;

namespace Avalonia.Controls.Win.WebView1;

internal static class AsyncOperationEx
{
    public static TResult? SetResult<TResult>(
        this TaskCompletionSource<TResult> taskCompletionSource,
        AsyncStatus asyncStatus,
        Func<TResult> resolver)
        where TResult : class, IInspectable
    {
        if (asyncStatus == AsyncStatus.Started)
        {
            throw new InvalidOperationException("Async operation was not completed");
        }
        else if (asyncStatus == AsyncStatus.Canceled)
        {
            taskCompletionSource.TrySetCanceled();
        }
        else
        {
            try
            {
                var value = resolver();
                if (asyncStatus == AsyncStatus.Completed)
                {
                    taskCompletionSource.TrySetResult(value);
                    return value;
                }
                else
                {
                    taskCompletionSource.TrySetException(new InvalidOperationException());
                }
            }
            catch (COMException ex)
            {
                var exWithMessage = Marshal.GetExceptionForHR(ex.ErrorCode)
                                    ?? ex;
                taskCompletionSource.TrySetException(exWithMessage);
            }
        }

        return null;
    }
}
