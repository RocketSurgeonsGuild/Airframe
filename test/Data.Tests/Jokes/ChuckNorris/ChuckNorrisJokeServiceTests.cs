using FluentAssertions;
using NSubstitute;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Rocket.Surgery.Airframe.Data.Tests.Jokes.ChuckNorris;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "Discarded variable.")]
public class ChuckNorrisJokeServiceTests
{
    [Fact]
    public void GivenApiClient_WhenRandom_ThenChuckNorrisJokeReturned()
    {
        // Given
        var client = new ChuckNorrisJokeApiClientMock();
        ChuckNorrisJokeService sut = new ChuckNorrisJokeServiceFixture().WithApiClient(client);
        ChuckNorrisJoke? result = null;
        using var _ = sut.Random().Subscribe(next => result = next);

        // When
        client.Notify(new ChuckNorrisJoke() { Id = Guid.NewGuid().ToString() });

        // Then
        result
           .Should()
           .NotBeNull()
           .And
           .BeOfType<ChuckNorrisJoke>();
    }

    [Theory]
    [ClassData(typeof(CategoryTests))]
    public void GivenApiClient_WhenRandomFromCategory_ThenChuckNorrisJokeReturned(IEnumerable<string> include, IEnumerable<string> exclude)
    {
        // Given
        var client = new ChuckNorrisJokeApiClientMock();
        ChuckNorrisJokeService sut = new ChuckNorrisJokeServiceFixture().WithApiClient(client);
        ChuckNorrisJoke? result = null;
        var categories = include.ToArray();
        using var _ = sut.Random(categories).Subscribe(next => result = next);

        // When
        client.Notify(new ChuckNorrisJoke { Id = Guid.NewGuid().ToString(), Categories = categories });

        // Then
        result
           .Should()
           .NotBeNull()
           .And
           .BeOfType<ChuckNorrisJoke>();
    }

    [Theory]
    [ClassData(typeof(CategoryTests))]
    public void GivenJokeWithCategory_WhenRandomFromCategory_ThenNothingReturned(IEnumerable<string> include, IEnumerable<string> exclude)
    {
        // Given
        var client = new ChuckNorrisJokeApiClientMock();
        ChuckNorrisJokeService sut = new ChuckNorrisJokeServiceFixture().WithApiClient(client);
        ChuckNorrisJoke? result = null;
        var categories = include.ToArray();
        using var _ = sut.Random(exclude.ToArray()).Subscribe(next => result = next);

        // When
        client.Notify(new ChuckNorrisJoke { Id = Guid.NewGuid().ToString(), Categories = categories });

        // Then
        result
           .Should()
           .BeNull();
    }

    [Theory]
    [ClassData(typeof(CategoryTests))]
    public void GivenJokeWithoutCategory_WhenRandomFromCategory_ThenNothingReturned(IEnumerable<string> include, IEnumerable<string> exclude)
    {
        // Given
        var client = new ChuckNorrisJokeApiClientMock();
        ChuckNorrisJokeService sut = new ChuckNorrisJokeServiceFixture().WithApiClient(client);
        ChuckNorrisJoke? result = null;
        var categories = exclude.ToArray();
        using var _ = sut.Random(include.ToArray()).Subscribe(next => result = next);

        // When
        client.Notify(new ChuckNorrisJoke { Id = Guid.NewGuid().ToString(), Categories = categories });

        // Then
        result
           .Should()
           .BeNull();
    }
}

internal sealed class ChuckNorrisJokeServiceFixture : ITestFixtureBuilder
{
    public static implicit operator ChuckNorrisJokeService(ChuckNorrisJokeServiceFixture fixture) => fixture.Build();

    public ChuckNorrisJokeServiceFixture WithApiClient(IChuckNorrisJokeApiClient client) => this.With(ref _chuckNorrisJokeApiClient, client);

    private ChuckNorrisJokeService Build() => new ChuckNorrisJokeService(_chuckNorrisJokeApiClient);

    private IChuckNorrisJokeApiClient _chuckNorrisJokeApiClient = Substitute.For<IChuckNorrisJokeApiClient>();
}

internal class CategoryTests : IEnumerable<object[]>
{
    private readonly IEnumerable<string> _exclude = new[] { "One Fish", "Two Fish" };
    private readonly IEnumerable<string> _include = new[] { "Red Fish", "Blue Fish" };

    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _include, _exclude };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}