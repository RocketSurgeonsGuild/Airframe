using DynamicData;
using FluentAssertions;
using Rocket.Surgery.Airframe.Data;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Xunit;

namespace Airframe.Tests.Data.DuckGo
{
    public class DuckDuckGoFunctionTests
    {
        [Fact]
        public void Should_Return_ChangeSet()
        {
            // Given
            var sourceCache = new SourceCache<DuckDuckGoQueryResult, string>(x => x.FirstUrl);

            // When
            Observable.Return(
                new List<DuckDuckGoQueryResult>
                {
                    new DuckDuckGoQueryResult
                    {
                        FirstUrl = Guid.NewGuid().ToString(),
                        Result = "result",
                        Text = "text"
                    }
                })
               .Cache(sourceCache)
               .Bind(out var result)
               .Subscribe();

            // Then
            result
               .Should()
               .HaveCount(1);
        }
        [Fact]
        public void Should_Return_Cached()
        {
            // Given
            var sourceCache = new SourceCache<DuckDuckGoQueryResult, string>(x => x.FirstUrl);
            sourceCache.AddOrUpdate(
                new DuckDuckGoQueryResult
                {
                    FirstUrl = Guid.NewGuid().ToString(),
                    Result = "result one",
                    Text = "text"
                });

            // When
            Observable.Return(
                    new List<DuckDuckGoQueryResult>
                    {
                        new DuckDuckGoQueryResult
                        {
                            FirstUrl = Guid.NewGuid().ToString(),
                            Result = "result two",
                            Text = "text"
                        }
                    })
               .Cache(sourceCache)
               .Bind(out var result)
               .Subscribe();

            // Then
            result
               .Should()
               .HaveCount(2);
        }
        [Fact]
        public void Should_Clear_Cached()
        {
            // Given
            var sourceCache = new SourceCache<DuckDuckGoQueryResult, string>(x => x.FirstUrl);
            sourceCache.AddOrUpdate(
                new DuckDuckGoQueryResult
                {
                    FirstUrl = Guid.NewGuid().ToString(),
                    Result = "result one",
                    Text = "text"
                });

            // When
            Observable.Return(
                    new List<DuckDuckGoQueryResult>
                    {
                        new DuckDuckGoQueryResult
                        {
                            FirstUrl = Guid.NewGuid().ToString(),
                            Result = "result two",
                            Text = "text"
                        }
                    })
               .Cache(sourceCache, true)
               .Bind(out var result)
               .Subscribe();

            // Then
            result
               .Should()
               .ContainSingle(x => x.Result == "result two");
        }
    }
}