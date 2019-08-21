using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Interfaces
{
    public interface IResult
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }
        int Code { get; set; }
    }

    public interface IResult<T> : IResult
    {
        T Data { get; set; }
    }
}
