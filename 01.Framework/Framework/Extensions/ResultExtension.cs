using Framework.Interfaces;
using System;

namespace Framework
{
    public static class ResultExtensions
    {
        public static IResult ThrowIfFail(this IResult obj, string keyword = "Message")
        {
            if (obj.IsSuccess == false)
                throw new Exception(string.Format("{0} : {1}", keyword, obj.Message));
            return obj;
        }
        public static IResult<T> ThrowIfFail<T>(this IResult<T> obj, string keyword = "Message")
        {
            return ((IResult)obj).ThrowIfFail(keyword) as IResult<T>;
        }
    }
}
