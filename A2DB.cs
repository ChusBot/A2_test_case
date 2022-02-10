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

using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace LesagaisParser
{
    /// <summary>
    /// Инкапсулирует работу с БД
    /// Строка подключения настраивается в App.config
    /// </summary>
    public class A2DB : DataConnection
    {
        public A2DB() : base(ProviderName.SqlServer)
        {
            this.CreateTable<WoodDeal>(tableOptions: TableOptions.CreateIfNotExists);

            TempWoodDeal = this.CreateTempTable<WoodDeal>(tableName: "temp_wood_deal",
                tableOptions: TableOptions.CreateIfNotExists);
        }

        /// <summary>
        /// Основная таблица со сделками
        /// </summary>
        public ITable<WoodDeal> WoodDeals => GetTable<WoodDeal>();

        /// <summary>
        /// Временная таблица со сделками (нужна для ускоренной вставки с помощью MERGE)
        /// </summary>
        public ITable<WoodDeal> TempWoodDeal { get; }

        internal void AddDeals(IEnumerable<WoodDeal> deals)
        {
            TempWoodDeal.Delete();
            TempWoodDeal.BulkCopy(deals.Distinct());

            WoodDeals.Merge().Using(TempWoodDeal)
                .OnTargetKey()
                .InsertWhenNotMatched()
                .Merge();
        }
    }
}
