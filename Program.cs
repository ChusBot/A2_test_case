/*
Copyright 2022 Mikhail Kovtunov

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LinqToDB.Data;

namespace LesagaisParser
{
    enum UpdateType
    {
        All,
        OnlyByLastTwoDay
    }

    class Program
    {
        // количество одновременно запрашиваемых сделок
        private const int DEALS_PER_PAGE = 50_000;

        // время задержки между запросами
        private readonly static TimeSpan DELAY = TimeSpan.FromMinutes(10);

        // если сделки не могут быть добавлены задним чеслом,
        // то рекомендуемая стратегия обновления OnlyByLastTwoDay
        private const UpdateType UPDATE_TYPE = UpdateType.OnlyByLastTwoDay;


#if DEBUG
        static Program()
        {
            // вывод трассировку запросов к БД
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s, s2, tl) => Debug.WriteLine(s, s2);
        }
#endif

        static async Task Main(string[] args)
        {
            var db = new A2DB();

            LesegaisClient client = new LesegaisClient();

            if (UPDATE_TYPE == UpdateType.All)
            {
                while (true)
                {
                    await AddDeals(db, client);

                    await Task.Delay(DELAY);
                }
            }
            else if (UPDATE_TYPE == UpdateType.OnlyByLastTwoDay)
            {
                await AddDeals(db, client);

                while (true)
                {
                    // обновляем только сделками за последние 2 дня
                    var yesterday = DateTime.Now.AddDays(-1).Date;

                    var filter = new
                    {
                        items = new object[]
                        {
                            new Dictionary<string, object>
                            {
                                ["property"] = "dealDate",
                                ["operator"] = "GTE",
                                ["value"] = yesterday

                            }
                        }
                    };

                    await AddDeals(db, client, filter);

                    await Task.Delay(DELAY);
                }
            }
        }

        private static async Task AddDeals(A2DB db, LesegaisClient client, object filter = null)
        {
            var dealsCount = await client.DealsCount(filter);

            int pages = (int)Math.Ceiling((decimal)dealsCount / DEALS_PER_PAGE);

            for (int page = 0; page < pages; page++)
            {
                var deals = await client.Deals(DEALS_PER_PAGE, page, filter);

                db.AddDeals(deals);
            }
        }
    }
}
