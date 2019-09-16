using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");


            #region 5th Module
            var query = cars.Where(c => c.Manufacturer == "BMW").OrderByDescending(c => c.Combined).ThenBy(c => c.Name)
                .Select(c => new
                {
                    c.Manufacturer,
                    c.Name,
                    c.Combined
                });
            var query2 = cars.Where(c => c.Manufacturer == "BMW")
                             .OrderByDescending(c => c.Combined)
                             .ThenBy(c => c.Name)
                             .FirstOrDefault();

            var result = cars.Any(c => c.Manufacturer == "Ford");
            var result2 = cars.All(c => c.Manufacturer == "Ford");

            //Console.WriteLine(result2);

            //cars.SelectMany(c => c.Name).OrderBy(c => c).ToList().ForEach(c => Console.WriteLine(c));

            //Console.WriteLine(character);

            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Name} : {car.Combined} : {car.}");
            //}

            #endregion

            var query3 = cars.Join(manufacturers,
                c => c.Manufacturer,
                m => m.Name,
                (c, m) => new
                {
                    m.Headquarters,
                    c.Name,
                    c.Combined
                })
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .ToList();

            var query4 = cars.Join(manufacturers,
                c => c.Manufacturer,
                m => m.Name,
                (c, m) => new
                {
                    m.Headquarters,
                    c.Name,
                    c.Combined
                })
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .ToList();

            var query5 = cars.Join(manufacturers,
                    c => new { c.Manufacturer, c.Year },
                    m => new { Manufacturer = m.Name, m.Year },
                    (c, m) => new
                    {
                        m.Headquarters,
                        c.Name,
                        c.Combined
                    })
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var car in query5.Take(10))
            {
                //Console.WriteLine($"{car.Name} {car.Headquarters} {car.Combined}");
            }

            var query6 = cars.GroupBy(c => c.Manufacturer.ToUpper())
                             .OrderBy(g => g.Key);

            //foreach (var group in query6)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name}: {car.Combined} MPG");
            //    }
            //}

            var query7 = manufacturers.GroupJoin(cars,
                                            m => m.Name,
                                            c => c.Manufacturer,
                                            (m, g) => new
                                            {
                                                Manufacturer = m,
                                                Cars = g
                                            }).OrderBy(g => g.Manufacturer.Name);

            foreach (var group in query7)
            {
                //Console.WriteLine($"{group.Manufacturer.Name}: {group.Manufacturer.Headquarters}");
                //foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                //{
                //    Console.WriteLine($"\t{car.Name}: {car.Combined} MPG");
                //}
            }

            var query8 = manufacturers.GroupJoin(cars,
                m => m.Name,
                c => c.Manufacturer,
                (m, g) => new
                {
                    Manufacturer = m,
                    Cars = g
                }).GroupBy(x => x.Manufacturer.Headquarters)
                .OrderBy(x => x.Key);

            foreach (var group in query8)
            {
                //Console.WriteLine(group.Key);
                //foreach (var car in group.SelectMany(x=>x.Cars).OrderByDescending(x => x.Combined).Take(3))
                //{
                //    Console.WriteLine($"\t{car.Name}:{car.Combined}");
                //}
            }


            var query9 = cars.GroupBy(c => c.Manufacturer.ToUpper())
                .Select(x => new
                {
                    Name = x.Key,
                    Max = x.Max(c => c.Combined),
                    Min = x.Min(c => c.Combined),
                    Avg = x.Average(c => c.Combined)
                }).OrderByDescending(c => c.Max);

            var query10 = cars.GroupBy(c => c.Manufacturer.ToUpper())
                .Select(x =>
                {
                    var results = x.Aggregate(new CarStatistics(),
                                                (acc, c) => acc.Accmulate(c),
                                                acc => acc.Compute());
                    return new
                    {
                        Name = x.Key,
                        Max = results.Max,
                        Min = results.Min,
                        Avg = results.Average
                    };
                }).OrderByDescending(c => c.Max);

            foreach (var group in query10)
            {
                Console.WriteLine(group.Name);
                Console.WriteLine($"\tMax: {group.Max}");
                Console.WriteLine($"\tMin: {group.Min}");
                Console.WriteLine($"\tAvg: {group.Avg}");
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
