using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Interfaces
{
    public interface ICommand : IDisposable
    {
        Result Execute(ObjectContext context);
    }

    public interface ICommand<T> : IDisposable
    {
        Result<T> Execute(ObjectContext context);
    }

    public abstract class CommandBase : ICommand
    {
        

        /// <summary>
        /// Validate before execute a command. Base validation does nothing
        /// </summary>
        protected virtual void ValidateCore(ObjectContext context)
        {
        }
        protected virtual void OnExecutingCore(ObjectContext context)
        {
        }
        protected virtual void OnExecutedCore(ObjectContext context, Result result)
        {
        }
        protected abstract Result ExecuteCore(ObjectContext context);
        public Result Execute(ObjectContext context)
        {
            try
            {
                ValidateCore(context);
                OnExecutingCore(context);
                var result = ExecuteCore(context);
                OnExecutedCore(context, result);
                return result;
            }
            catch (Exception ex)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public virtual void Dispose()
        {
        }

        protected Result Success(string message = null)
        {
            return new Result
            {
                IsSuccess = true,
                Message = message ?? "success",
                
            };
        }
    }

    public abstract class CommandBase<T> : ICommand<T>
    {
        /// <summary>
        /// Validate before execute a command. Base validation does nothing
        /// </summary>
        protected virtual void ValidateCore(ObjectContext context)
        {
        }
        protected virtual void OnExecutingCore(ObjectContext context)
        {
        }
        protected virtual void OnExecutedCore(ObjectContext context, Result<T> result)
        {
        }
        protected abstract Result<T> ExecuteCore(ObjectContext context);
        public Result<T> Execute(ObjectContext context)
        {
            try
            {
                ValidateCore(context);
                OnExecutingCore(context);
                var result = ExecuteCore(context);
                OnExecutedCore(context, result);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<T>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public virtual void Dispose()
        {
        }

        protected Result<T> Success(T data, string message = null)
        {
            return new Result<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message ?? "success",
                Code = 200
            };
        }
    }



    public class Result : IResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }

    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; set; }
    }


}
