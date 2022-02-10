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
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace LesagaisParser
{
    internal class LesegaisClient
    {
        private const string BaseUrl = "https://www.lesegais.ru";
        private const string Resource = "open-area/graphql";

        /// <summary>
        /// Возвращает количество всех сделок
        /// </summary>
        /// <returns></returns>
        public async Task<int> DealsCount(object filter = null)
        {
            var body = new
            {
                query = "query SearchReportWoodDealCount($size:Int!,$number:Int!,$filter:Filter,$orders:[Order!]){searchReportWoodDeal(filter:$filter,pageable:{number:$number,size:$size},orders:$orders){total}}",
                variables = new
                {
                    size = 0,
                    number = 0,
                    filter
                },
                operationName = "SearchReportWoodDealCount"
            };

            string responseContent = await MakeRequest(body);

            return JObject.Parse(responseContent).SelectToken("$.data.searchReportWoodDeal.total").Value<int>();
        }

        /// <summary>
        /// Возвращает сделки
        /// </summary>
        /// <param name="size">размер выборки</param>
        /// <param name="page">страница выборки</param>
        /// <returns></returns>
        public async Task<IEnumerable<WoodDeal>> Deals(int size, int page, object filter = null)
        {
            var body = new
            {
                query = "query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\n    content {\n      sellerName\n      sellerInn\n      buyerName\n      buyerInn\n      woodVolumeBuyer\n      woodVolumeSeller\n      dealDate\n      dealNumber\n  }\n}\n}\n",
                variables = new
                {
                    size,
                    number = page,
                    filter
                },
                operationName = "SearchReportWoodDeal"
            };

            string responseContent = await MakeRequest(body);

            return JObject.Parse(responseContent)
                .SelectToken("$.data.searchReportWoodDeal.content")
                .ToObject<List<WoodDeal>>(); ;
        }

        private async Task<string> MakeRequest(object body)
        {
            var client = new RestClient(BaseUrl);

            var request = new RestRequest(Resource, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(JsonConvert.SerializeObject(body), DataFormat.Json);

            RestResponse response = await client.ExecutePostAsync(request);

            return response.Content;
        }
    }
}
