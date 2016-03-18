using System;
using System.Collections.Generic;
using System.Linq;

namespace Win2ch.Common.Html {
    public class Match<TSource, TResult> where TResult : class {
        public Match(Func<TSource, bool> condition, Func<TSource, TResult, TResult> result) {
            Condition = condition;
            Result = result;
        }

        public Func<TSource, bool> Condition { get; }
        public Func<TSource, TResult, TResult> Result { get; }
    }

    public class Selector<TSource, TResult> where TResult : class {

        private List<Match<TSource, TResult>> Matches { get; } = new List<Match<TSource, TResult>>();
        private TResult Base { get; set; }

        public static Selector<TSource, TResult> Begin() {
            return new Selector<TSource, TResult>();
        }

        public Selector<TSource, TResult> AddMatch(Func<TSource, bool> condition,
                                                   Func<TSource, TResult, TResult> result) {
            Matches.Add(new Match<TSource, TResult>(condition, result));
            return this;
        }

        public Selector<TSource, TResult> SetBase(TResult item) {
            Base = item;
            return this;
        }

        public virtual TResult Select(TSource item) {
            IEnumerable<Match<TSource, TResult>> matches = Matches.Where(m => m.Condition(item));

            return Base == null
                ? matches.FirstOrDefault()?.Result?.Invoke(item, Base)
                : matches.Aggregate(Base, (current, match) => match.Result(item, current));
        }

        public IEnumerable<TResult> SelectCollection(IEnumerable<TSource> source) {
            return source.Select(Select);
        }
    }
}
