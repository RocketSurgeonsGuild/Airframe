using DynamicData;
using FluentAssertions;
using Rocket.Surgery.Airframe.Data.DuckDuckGo;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Xunit;

namespace Rocket.Surgery.Airframe.Data.Tests.DuckGo;

public class DuckDuckGoFunctionTests
{
    [Fact]
    public void Should_Return_ChangeSet()
    {
        // Given
        using var sourceCache = new SourceCache<RelatedTopic, string>(topic => topic.FirstUrl);

        // When
        using var disposable =
            Observable.Return(
                    new List<RelatedTopic>
                    {
                        new()
                        {
                            FirstUrl = Guid.NewGuid().ToString(),
                            Result = "result",
                            Text = "text",
                        },
                    })
               .Cache(sourceCache, true)
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
        using var sourceCache = new SourceCache<RelatedTopic, string>(topic => topic.FirstUrl);
        var firstResultGuid = Guid.NewGuid().ToString();
        sourceCache.AddOrUpdate(
            new RelatedTopic
            {
                FirstUrl = firstResultGuid,
                Result = "result one",
                Text = "text",
            });

        // When
        using var disposable =
            Observable.Return(
                    new List<RelatedTopic>
                    {
                        new()
                        {
                            FirstUrl = firstResultGuid,
                            Result = "result one",
                            Text = "text",
                        },
                        new()
                        {
                            FirstUrl = Guid.NewGuid().ToString(),
                            Result = "result two",
                            Text = "text",
                        },
                    })
               .Cache(sourceCache, true)
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
        using var sourceCache = new SourceCache<RelatedTopic, string>(topic => topic.FirstUrl);
        sourceCache.AddOrUpdate(
            new RelatedTopic
            {
                FirstUrl = Guid.NewGuid().ToString(),
                Result = "result one",
                Text = "text",
            });

        // When
        using var disposable =
            Observable.Return(
                    new List<RelatedTopic>
                    {
                        new()
                        {
                            FirstUrl = Guid.NewGuid().ToString(),
                            Result = "result two",
                            Text = "text",
                        },
                    })
               .Cache(sourceCache, true)
               .Bind(out var result)
               .Subscribe();

        // Then
        result
           .Should()
           .ContainSingle(topic => topic.Result == "result two");
    }
}