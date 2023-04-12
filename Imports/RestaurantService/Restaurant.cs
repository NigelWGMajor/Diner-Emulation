using System;
using System.Collections.Generic;
using Bogus;
using Models;

namespace RestaurantService
{
    public class Restaurant
    {
        private Faker _faker = new Faker();

        private Faker<Models.Person>
            _personFaker =
                new Faker<Models.Person>()
                    .RuleFor(p => p.Name, f => f.Person.FirstName);

        public Table[] Tables { get; set; }

        public Server[] Servers { get; set; }

        public Menu Menu;

        public Plan[] Plans { get; set; }

        public ActivityList Activities { get; set; }

        private void Initialize(int tableCount)
        {
            Servers = GetServers(tableCount);
            Activities = GetActivities();
            Plans = GetMenuPlans();
            Tables = AssignTables(tableCount);
        }

        private Server[] GetServers(int tableCount)
        {
            Models.Person[] persons =
                _personFaker
                    .GenerateBetween(tableCount / 6 + 1, tableCount / 4 + 1)
                    .ToArray();
            Server[] result = new Server[persons.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Server(persons[i].Name);
            }
            return result;
        }

        private ActivityList GetActivities()
        {
            var result = new ActivityList();
            result.Add("Fetch", "Fetch");
            result.Add("Prep", "Prepare");
            result.Add("Cook", "Cook");
            result.Add("Dress", "Dress");
            result.Add("Plate", "Plate");

            return result;
        }

        private Plan[] GetMenuPlans()
        {
            return new Plan[] {
                new Plan(ItemKind.Main,
                    "Catfish",
                    new Stage("Fetch",
                        Activities["Fetch"],
                        1,
                        new ReactFail("Out of fish")),
                    new Stage("Cook",
                        Activities["Fetch"],
                        3,
                        new ReactDelay("Waiting on oil to heat up"),
                        new ReactRestart("Need to recook")),
                    new Stage("Plate",
                        Activities["Plate"],
                        1,
                        new ReactDelay("Waiting on plates"))),
                new Plan(ItemKind.Side,
                    "FrenchFries",
                    new Stage("Get Fries",
                        Activities["Fetch"],
                        1,
                        new ReactDelay("Waiting on Potatos"),
                        new ReactFail("Out of potatoes")),
                    new Stage("Cook Fries",
                        Activities["Prep"],
                        3,
                        new ReactDelay("Waiting on oil to heat up"),
                        new ReactRestart("Need to recook")),
                    new Stage("Plate",
                        Activities["Plate"],
                        1,
                        new ReactDelay("Waiting on plates"))),
                new Plan(ItemKind.Main,
                    "Hamburger",
                    new Stage("Fetch",
                        Activities["Fetch"],
                        1,
                        new ReactFail("Out of hamburger")),
                    new Stage("Cook",
                        Activities["Fetch"],
                        3,
                        new ReactRestart("Need to recook")),
                    new Stage("Plate",
                        Activities["Plate"],
                        1,
                        new ReactDelay("Waiting on plates"))),
                new Plan(ItemKind.Side,
                    "Salad",
                    new Stage("Get salad ingredients",
                        Activities["Fetch"],
                        1,
                        new ReactDelay("Waiting on ingredients"),
                        new ReactFail("issing ungredients for salad")),
                    new Stage("Cut", Activities["Prep"], 3),
                    new Stage("Plate",
                        Activities["Plate"],
                        2,
                        new ReactDelay("Waiting on oil to heat up"),
                        new ReactRestart("Need to recook")),
                    new Stage("Dress",
                        Activities["Dress"],
                        1,
                        new ReactDelay("Waiting on plates"))),
                new Plan(ItemKind.Main,
                    "Chicken",
                    new Stage("Fetch",
                        Activities["Fetch"],
                        1,
                        new ReactFail("Out of chicken")),
                    new Stage("Cook",
                        Activities["Fetch"],
                        3,
                        new ReactRestart("Need to recook")),
                    new Stage("Plate",
                        Activities["Plate"],
                        1,
                        new ReactDelay("Waiting on plates"))),
                new Plan(ItemKind.Side,
                    "Coleslaw",
                    new Stage("Get coleslaw",
                        Activities["Fetch"],
                        1,
                        new ReactDelay("Waiting on ingredients"),
                        new ReactFail("Out of potatoes")),
                    new Stage("Plate",
                        Activities["Plate"],
                        2,
                        new ReactDelay("Waiting on oil to heat up"),
                        new ReactRestart("Need to recook")),
                    new Stage("Dress",
                        Activities["Dress"],
                        1,
                        new ReactDelay("Waiting on plates")))
            };
        }

        private Table[] AssignTables(int count)
        {
            var result = new Table[count];
            for (int i = 0; i < count; i++)
            {
                Tables[i] = new Table(_faker.PickRandom(Servers));
            }
            return result;
        }
    }
}
