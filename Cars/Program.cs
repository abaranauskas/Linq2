using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            //Func<int, int> square = x => x * x;
            //Expression<Func<int, int, int>> add = (y, x) => y + x;
            ////Expresion tai duomenu strutura EF pagal tai pavercia i SQL

            //Func<int, int, int> addI = add.Compile();

            //var result = addI(2, 3);
            //Console.WriteLine(result);
            //Console.WriteLine(add);
            //

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
        }

        private static void QueryData()
        {
            var context = new CarDb();
            context.Database.Log = Console.WriteLine;

            var query = context.Cars.Where(c => c.Manufacturer == "BMW")
                                    .OrderByDescending(c => c.Combined)
                                    .ThenBy(c => c.Name)
                                    .Select(c => new { c.Name, c.Combined })
                                    .Take(10);

            query.ToList().ForEach(c => Console.WriteLine($"{c.Name}:{c.Combined}"));

            var query2 = context.Cars.GroupBy(c => c.Manufacturer)
                .Select(g => new
                {
                    Name = g.Key,
                    Cars = g.OrderByDescending(c => c.Combined).Take(2)
                });

            foreach (var group in query2)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}:{car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var context = new CarDb();
            //context.Database.Log = Console.WriteLine;
            if (!context.Cars.Any())
            {
                context.Cars.AddRange(cars);
                context.SaveChanges();
            }
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            return File.ReadAllLines(path)
                            .Skip(1)
                            .Where(l => l.Length > 0)
                            .Select(l =>
                            {
                                var columns = l.Split(',');
                                return new Manufacturer()
                                {
                                    Year = int.Parse(columns[2]),
                                    Name = columns[0],
                                    Headquarters = columns[1]
                                };
                            }).ToList();
        }

        private static List<Car> ProcessCars(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 0)
                .ToCar().ToList();

            //return File.ReadAllLines(path)
            //                .Skip(1)
            //                .Where(l => l.Length > 0)
            //                .Select(Car.ParseFromCsv)
            //                .ToList();

            //return File.ReadAllLines(path)
            //                .Skip(1)
            //                .Where(l => l.Length > 0)
            //                .Select(l=>
            //                {
            //                    var carArray = l.Split(',');
            //                    return new Car()
            //                    {
            //                        Year = int.Parse(carArray[0]),
            //                        Manufacturer = carArray[1],
            //                        Name = carArray[2],
            //                        Displacement = double.Parse(carArray[3]),
            //                        Cylinders = int.Parse(carArray[4]),
            //                        City = int.Parse(carArray[5]),
            //                        Highway = int.Parse(carArray[6]),
            //                        Combined = int.Parse(carArray[7]),
            //                    };
            //                }).ToList();

        }
    }

    public class CarStatistics
    {
        public int Max { get; set; } = int.MinValue;
        public int Min { get; set; } = int.MaxValue;
        public int Total { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }

        public CarStatistics Accmulate(Car car)
        {
            Count++;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);
            return this;
        }

        public CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }
    }
    public static class Extensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Car()
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3], CultureInfo.CreateSpecificCulture("en-us")),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7]),
                };
            }
        }
    }
}
