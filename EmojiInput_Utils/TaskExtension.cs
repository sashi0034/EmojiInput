#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmojiInput_Utils;

public static class TaskExtension
{
    // https://steven-giesel.com/blogPost/d38e70b4-6f36-41ff-8011-b0b0d1f54f6e
    // TODO: 増やす

    public static void Forget(
        this Task task,
        Action<Exception?>? errorHandler = null)
    {
        task.ContinueWith(t =>
            {
                if (t.IsFaulted && errorHandler != null) errorHandler(t.Exception);
            },
            TaskContinuationOptions.OnlyOnFaulted);
    }

    public static async Task RunTaskHandlingErrorAsync(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            if (e is OperationCanceledException oc)
                Console.WriteLine($"{e.Message} [{oc.HResult}]");
            else
                Console.Error.WriteLine(e);
        }
    }

    public static void RunTaskHandlingError(this Task task)
    {
        task.RunTaskHandlingErrorAsync().Forget();
    }

    public static CancellationToken LinkToken(this CancellationToken cancel, CancellationTokenSource otherCancel)
    {
        return LinkToken(cancel, otherCancel.Token);
    }

    public static CancellationToken LinkToken(this CancellationToken cancel, CancellationToken otherCancel)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(cancel, otherCancel).Token;
    }
}