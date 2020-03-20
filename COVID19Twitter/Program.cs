using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tweetinvi;

namespace COVID19
{
    class Program
    {
        static void Main(string[] args)
        {
            // Tokens API : [Twitter@Covid19Spainbot]
            Auth.SetUserCredentials(
                "UBdpeVnAbt9ow1QigNXSRPCqt",
                "N5Ow7h0DmK0v20quzDKzlMiwbNBEs3dl7xtCiaFparx9lJZdns",
                "316723929-zVfNXP3KtWQ8gZyGyxewMnrMZMoPcMKrZlZI2Gm9",
                "5vw2LpzDj0FYzdpWWpL53auF14e6ai8rDDtXFqGl6xxlB"
            );

            (List<dynamic>Spain, List<dynamic>China) getData()
            {
                List<dynamic> Spain = new List<dynamic>();
                List<dynamic> China = new List<dynamic>();

                dynamic DataSet;
                try
                {
                    using (WebClient wc = new WebClient())
                        DataSet = JsonConvert.DeserializeObject(wc.DownloadString("https://corona.lmao.ninja/countries"));
                }
                catch { return (Spain, China); }
            
                foreach (dynamic Data in DataSet)
                {
                    if (Data.country == "Spain")
                        Spain.AddRange(new List<dynamic>() {
                            Data.cases,  Data.todayCases, Data.todayDeaths,
                            Data.deaths, Data.recovered,  Data.critical
                        });

                    if (Data.country == "China")
                        China.AddRange(new List<dynamic>() { Data.cases, Data.deaths });
                }
                return (Spain, China);
            }

            int count = 1;
            (List<dynamic> ES, List<dynamic> CH) FirstResult = getData();
            (List<dynamic> ES, List<dynamic> CH) SecondResult = FirstResult;

            Timer Stats = new Timer(UpdateStats, null, 0, 300000); // Each 5 minutes.
            Timer AverageDeaths = new Timer(UpdateAverageDeaths, null, 0, 43200000); // each 12 hours.


            if (FirstResult.ES.Any() && FirstResult.CH.Any())
                publicTweetStatus(FirstResult.ES);
            else
                Error();

            Console.WriteLine("[{0}] Publishing tweet... ", count);
            Console.ReadLine();

            void UpdateStats(Object o)
            {
                SecondResult = getData(); 
                if (!Enumerable.SequenceEqual(FirstResult.ES, SecondResult.ES))
                {
                    count++;
                    if (SecondResult.ES.Any() && SecondResult.CH.Any())
                        publicTweetStatus(SecondResult.ES);
                    else
                        Error();

                    Console.WriteLine("[{0}] An update was found. ---> Publishing tweet...", count);
                    FirstResult = SecondResult;
                }
            }

            void UpdateAverageDeaths(Object o)
            {
                if (FirstResult.ES.Any() && FirstResult.CH.Any())
                    publicTweetRelation(getAverage(FirstResult));
                else
                    Error();

                Console.WriteLine("[+] Publishing relationships between deaths / total cases...");
            }

            List<decimal> getAverage((List<dynamic> Spain, List<dynamic> China) Data) {

                decimal deathsPerThousand_China = (1000 * (decimal)Data.China[1]) / (decimal)Data.China[0];
                decimal deathsPerThousand_Spain = (1000 * (decimal)Data.Spain[3]) / (decimal)Data.Spain[0];

                decimal Mortality_China = ((decimal)Data.China[1] * 100 ) / (decimal)Data.China[0];
                decimal Mortality_Spain = ((decimal)Data.Spain[3] * 100 ) / (decimal)Data.Spain[0];

                return  new List<decimal>() { deathsPerThousand_Spain, deathsPerThousand_China, Mortality_Spain,  Mortality_China };
            }

            void publicTweetRelation(List<decimal> Averages)
            {
                Tweet.PublishTweet(String.Format("Muertes en España por cada 1000 infectados: {0:0.0}\n"              +
                                                 "Muertes en China por cada 1000 infectados: {1:0.0}\n"               +
                                                 "Tasa de mortalidad del COVID-19 en España: {2:0.000000}%\n"         +
                                                 "Tasa de mortalidad del COVID-19 en China: {3:0.000000}%\n"          +
                                                 "Última actualización: {4}",
                                                 Averages[0], Averages[1], Averages[2], Averages[3], DateTime.Now));
            }

            void publicTweetStatus(List<dynamic> Spain_Data)
            {
                Tweet.PublishTweet(String.Format(
                                  "Casos totales: {0}\n"          +
                                  "Casos hoy: {1}\n"              +
                                  "Muertes hoy: {2}\n"            +
                                  "Muertes totales: {3}\n"        +
                                  "Recuperados: {4}\n"            +
                                  "En estado crítico: {5}\n"      +
                                  "Última actualización: {6}"     , 
                                  Spain_Data[0], Spain_Data[1], Spain_Data[2], Spain_Data[3], Spain_Data[4], Spain_Data[5], DateTime.Now));
            }

            void Error() => Console.WriteLine("[-] An exception occurred.");
        }
    }
}
