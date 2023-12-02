using DynamicData;
using FluentAssertions;
using NSubstitute;
using Rocket.Surgery.Airframe.Data.DuckDuckGo;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Xunit;

namespace Rocket.Surgery.Airframe.Data.Tests.DuckGo
{
    public class DuckDuckGoServiceTests
    {
        [Fact]
        public void Should_Return_Query_Results()
        {
            // Given
            var client = Substitute.For<IDuckDuckGoApiClient>();
            client.Search(Arg.Any<string>()).Returns(
                Observable.Return(
                    new SearchResult
                    {
                        RelatedTopics = new List<RelatedTopic>
                        {
                            new()
                            {
                                FirstUrl = Guid.NewGuid().ToString(),
                                Result = "result one",
                                Text = "text",
                            },
                        },
                    }));
            DuckDuckGoService sut = new DuckDuckGoServiceFixture().WithClient(client);

            // When
            sut.Query(string.Empty)
               .Bind(out var results)
               .Subscribe();

            // Then
            results.Should().NotBeEmpty();
        }
    }
}