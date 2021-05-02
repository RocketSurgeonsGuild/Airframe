using DynamicData;
using FluentAssertions;
using NSubstitute;
using Rocket.Surgery.Airframe.Data;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Xunit;

namespace Airframe.Tests.Data.DuckGo
{
    public class DuckDuckGoServiceTests
    {
        [Fact]
        public void Should_Return_Query_Results()
        {
            // Given
            var client = Substitute.For<IDuckDuckGoApiClient>();
            client.Search(Arg.Any<string>()).Returns(Observable.Return(new List<DuckDuckGoSearchResult>{
                new DuckDuckGoSearchResult
                {
                    RelatedTopics = new List<DuckDuckGoQueryResult>{ 
                        new DuckDuckGoQueryResult
                        {
                            FirstUrl = Guid.NewGuid().ToString(),
                            Result = "result one",
                            Text = "text"
                        }}

                }}));
            DuckDuckGoService sut = new DuckDuckGoServiceFixture().WithClient(client);

            // When
            sut.Query("")
               .Bind(out var results)
               .Subscribe();

            // Then
            results.Should().NotBeEmpty();
        }
    }
}