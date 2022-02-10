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
using LinqToDB;
using LinqToDB.Mapping;

namespace LesagaisParser
{
    /// <summary>
    /// Запись о совершенной сделке
    /// </summary>
    [Table(Name = "wood_deal")]
    public class WoodDeal : IEquatable<WoodDeal>
    {
        private string sellerName;
        private string buyerName;
        private string sellerInn;
        private string buyerInn;
        private string dealnumber;

        [Column(Name = "number", Length = 29), PrimaryKey, NotNull]
        public string DealNumber
        {
            get => dealnumber;
            set => dealnumber = value?.Trim() ?? string.Empty;
        }


        [Column(Name = "sellerName", Length = 350)]
        public string SellerName
        {
            get => sellerName;
            set => sellerName = value?.Trim() ?? string.Empty;
        }


        [Column(Name = "sellerInn", Length = 14)]
        public string SellerInn
        {
            get => sellerInn;
            set => sellerInn = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "sellAmount"), NotNull]
        public float WoodVolumeSeller { get; set; }

        [Column(Name = "buyerName", Length = 350)]
        public string BuyerName
        {
            get => buyerName;
            set => buyerName = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "buyerInn", Length = 30)]
        public string BuyerInn
        {
            get => buyerInn;
            set => buyerInn = value?.Trim() ?? string.Empty;
        }

        [Column(Name = "buyAmount"), NotNull]
        public float WoodVolumeBuyer { get; set; }

        [Column(Name = "dealDate", DataType = DataType.DateTime2)]
        public DateTime? DealDate { get; set; }

        public bool Equals(WoodDeal other)
        {
            return DealNumber.Equals(other.DealNumber, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return DealNumber.GetHashCode();
        }
    }
}
