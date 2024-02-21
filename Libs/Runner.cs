using Schedule.ModelSpace;
using System;
using System.Threading.Tasks;

namespace Schedule
{
  public interface IRunner : IDisposable
  {
    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource<ResponseModel<bool>> Send(Action action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource<ResponseModel<bool>> Send(Task action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource<ResponseModel<T>> Send<T>(Func<T> action);

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    TaskCompletionSource<ResponseModel<T>> Send<T>(Task<T> action);
  }

  public abstract class Runner : IRunner
  {
    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose() { }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<ResponseModel<bool>> Send(Action action)
    {
      var response = new ResponseModel<bool>();
      var source = new TaskCompletionSource<ResponseModel<bool>>();

      Enqueue(new ActionModel
      {
        Error = error =>
        {
          response.Error = error;
          source.TrySetResult(response);
        },
        Success = () =>
        {
          action();
          response.Data = true;
          source.TrySetResult(response);
        }
      });

      return source;
    }

    /// <summary>
    /// Task processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<ResponseModel<bool>> Send(Task action)
    {
      var response = new ResponseModel<bool>();
      var source = new TaskCompletionSource<ResponseModel<bool>>();

      Enqueue(new ActionModel
      {
        Error = error =>
        {
          response.Error = error;
          source.TrySetResult(response);
        },
        Success = async () =>
        {
          await action;
          response.Data = true;
          source.TrySetResult(response);
        }
      });

      return source;
    }

    /// <summary>
    /// Delegate processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<ResponseModel<T>> Send<T>(Func<T> action)
    {
      var response = new ResponseModel<T>();
      var source = new TaskCompletionSource<ResponseModel<T>>();

      Enqueue(new ActionModel
      {
        Error = error =>
        {
          response.Error = error;
          source.TrySetResult(response);
        },
        Success = () =>
        {
          response.Data = action();
          source.TrySetResult(response);
        }
      });

      return source;
    }

    /// <summary>
    /// Task processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<ResponseModel<T>> Send<T>(Task<T> action)
    {
      var response = new ResponseModel<T>();
      var source = new TaskCompletionSource<ResponseModel<T>>();

      Enqueue(new ActionModel
      {
        Error = error =>
        {
          response.Error = error;
          source.TrySetResult(response);
        },
        Success = () =>
        {
          response.Data = action.GetAwaiter().GetResult();
          source.TrySetResult(response);
        }
      });

      return source;
    }

    /// <summary>
    /// Task delegate processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<ResponseModel<T>> Send<T>(Func<Task<T>> action)
    {
      var response = new ResponseModel<T>();
      var source = new TaskCompletionSource<ResponseModel<T>>();

      Enqueue(new ActionModel
      {
        Error = error =>
        {
          response.Error = error;
          source.TrySetResult(response);
        },
        Success = () =>
        {
          response.Data = action().GetAwaiter().GetResult();
          source.TrySetResult(response);
        }
      });

      return source;
    }


    /// <summary>
    /// Enqueue
    /// </summary>
    /// <param name="action"></param>
    protected virtual void Enqueue(ActionModel item)
    {
    }
  }
}
