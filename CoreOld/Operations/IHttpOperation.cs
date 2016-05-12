using System;
using System.Threading.Tasks;

namespace Core.Operations {
    public interface IHttpOperation<T> {
        Uri Uri { get; }
        Task<T> ExecuteAsync();
    }
}