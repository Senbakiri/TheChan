using Core.Common.Links;
using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public interface IThreadConverter : IConverter<ThreadEntity, Thread> {
         ThreadLink Link { get; set; }
    }
}