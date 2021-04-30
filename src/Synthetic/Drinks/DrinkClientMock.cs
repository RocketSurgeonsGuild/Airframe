using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// Represents an <see cref="IClient"/> that returns <see cref="DrinkDto"/>.
    /// </summary>
    public class DrinkClientMock : ClientMock<DrinkDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrinkClientMock"/> class.
        /// </summary>
        public DrinkClientMock() => Items = new List<DrinkDto>
        {
            new DrinkDto
            {
                Title = "Americano",
                Type = DrinkType.Americano,
                Description = "Dark. Strong"
            },
            new DrinkDto
            {
                Title = "Blonde Americano",
                Type = DrinkType.Americano,
                Description = "Dark. Strong"
            },
            new DrinkDto
            {
                Title = "Blonde Roast",
                Type = DrinkType.Brewed
            },
            new DrinkDto
            {
                Title = "Caffe Misto",
                Type = DrinkType.Brewed
            },
            new DrinkDto
            {
                Title = "Dark Roast",
                Type = DrinkType.Brewed
            },
            new DrinkDto
            {
                Title = "Medium Roast",
                Type = DrinkType.Brewed
            },
            new DrinkDto
            {
                Title = "Espresso",
                Type = DrinkType.Espresso,
                Description = "Hot. Bold."
            },
            new DrinkDto
            {
                Title = "Cappuccino",
                Type = DrinkType.Cappuccino,
                Description = "Foamy. Light."
            },
            new DrinkDto
            {
                Title = "Blonde Cappuccino",
                Type = DrinkType.Cappuccino,
                Description = "Foamy. Light."
            },
            new DrinkDto
            {
                Title = "Flat White",
                Type = DrinkType.FlatWhite,
                Description = "Heavy. Full."
            },
            new DrinkDto
            {
                Title = "Blonde Flat White",
                Type = DrinkType.FlatWhite,
                Description = "Heavy. Full."
            },
            new DrinkDto
            {
                Title = "Caffe Latte",
                Type = DrinkType.Latte,
                Description = "Smooth. Bold."
            },
            new DrinkDto
            {
                Title = "Vanilla Latte",
                Type = DrinkType.Latte,
                Description = "Smooth. Bold."
            },
            new DrinkDto
            {
                Title = "Burnt Honey Latte",
                Type = DrinkType.Latte,
                Description = "Smooth. Bold."
            },
            new DrinkDto
            {
                Title = "Hazelnut Latte",
                Type = DrinkType.Latte,
                Description = "Smooth. Bold."
            },
            new DrinkDto
            {
                Title = "Macchiato",
                Type = DrinkType.Macchiato,
                Description = "Hot. Bold."
            },
            new DrinkDto
            {
                Title = "Carmel Macchiato",
                Type = DrinkType.Macchiato,
                Description = "Rich. Sweet."
            },
            new DrinkDto
            {
                Title = "Latte Macchiato",
                Type = DrinkType.Macchiato,
                Description = "Hot. Bold."
            },
            new DrinkDto
            {
                Title = "Hazelnut Macchiato",
                Type = DrinkType.Macchiato,
                Description = "Hot. Bold."
            }
        };
    }
}