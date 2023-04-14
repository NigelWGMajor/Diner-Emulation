using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Models.Common;

namespace RestaurantService
{
    public class Restaurant
    {
        private Faker _faker = new Faker();

        private Faker<Models.Common.Person>
            _personFaker =
                new Faker<Models.Common.Person>()
                    .RuleFor(p => p.Name, f => f.Person.FirstName);

       // public Table[] Tables { get; set; }

        public Server[] Servers { get; set; }

        public Menu Menu;

        public MenuPlan[] Plans { get; set; }

        public ActivityList Activities { get; set; }

        private (int Count, MenuItem[] Items) mains;
        private (int Count, MenuItem[] Items) sides;

        private void Initialize(int tableCount)
        {
            Servers = GetServers(tableCount);
            Activities = GetActivities(); // will be redundant, used for local menu mocking.
            Plans = GetMenuPlans();
            // Tables = AssignTables(tableCount);
            mains = (Plans.Count(a => a.ItemKind == ItemKind.Main), Plans.Where(a => a.ItemKind == ItemKind.Main).ToArray<MenuItem>());
            sides = (Plans.Count(a => a.ItemKind == ItemKind.Side), Plans.Where(a => a.ItemKind == ItemKind.Side).ToArray<MenuItem>());
        }

        private Server[] GetServers(int tableCount)
        {
            Models.Common.Person[] persons =
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

        private MenuPlan[] GetMenuPlans()
        {
            return new MenuPlan[] {
                new MenuPlan(ItemKind.Main,
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
                new MenuPlan(ItemKind.Side,
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
                new MenuPlan(ItemKind.Main,
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
                new MenuPlan(ItemKind.Side,
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
                new MenuPlan(ItemKind.Main,
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
                new MenuPlan(ItemKind.Side,
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

        public Table GetNewTable()
        {
            var table = new Table(_faker.PickRandom(Servers));
            int dinerCount = _faker.Random.Int(1, 8); // 1 to 6 diners per table
            var dinerList = new List<Diner>();
            dinerList.AddRange(
                _personFaker.Generate(dinerCount).Select(p => new Diner(p.Name)));
            foreach (var diner in dinerList)
            {
                table.Seat(diner);
                diner.OrderFood(
                    _faker.PickRandom(mains.Items, 1).ToArray());
                diner.OrderFood(
                    _faker.PickRandom(sides.Items, _faker.Random.Int(1, Math.Min(sides.Count, 4))).ToArray());
            }
            table.Diners = dinerList;
            return table;
        }
        
        //private Table[] AssignTables(int count)
        //{
        //    var result = new Table[count];
        //    for (int i = 0; i < count; i++)
        //    {
        //        Tables[i] = new Table(_faker.PickRandom(Servers));
        //    }
        //    return result;
        //}
    }
}
