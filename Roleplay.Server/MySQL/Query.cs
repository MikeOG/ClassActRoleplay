using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Server.MySQL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Return type of data being returned from query</typeparam>
    public class Query<T>
    {
        private string initialQueryString;
        private string builtQueryString;
        private bool buildWhereParameters;
        private Dictionary<string, dynamic> initialWhereParameters;
        private Dictionary<string, dynamic> builtWhereParameters;
        private dynamic SQL => Server.Instance.GetExports("GHMattiMySQL");

        public string QueryString
        {
            get
            {
                if (string.IsNullOrEmpty(builtQueryString))
                    BuildQuery();

                return builtQueryString;
            }
        }

        public T QueryData { get; private set; } 

        public Query(string initQuery, Dictionary<string, dynamic> initWhereParms/*, bool buildWhere = true*/)
        {
            initialQueryString = initQuery;
            initialWhereParameters = initWhereParms;

            builtWhereParameters = new Dictionary<string, dynamic>();

            //buildWhereParameters = buildWhere;
        }

        public async Task<T> Execute()
        {
            //if (QueryData != null && typeof(T) != typeof(int)) return QueryData;

            while (SQL == null)
                await BaseScript.Delay(0);

            if (QueryString.Contains("INSERT") || QueryString.Contains("UPDATE") || QueryString.Contains("DELETE"))
            {
                SQL.execute(QueryString, builtWhereParameters, new Action<T>(data => QueryData = data));
            }

            if (QueryString.Contains("SELECT"))
            {
                SQL.execute(QueryString, builtWhereParameters, new Action<T>(data => QueryData = data));
            }

            var ticks = 0;
            while (QueryData == null && ticks < 300)
            {
                await BaseScript.Delay(0);
                ticks++;
            }

            return QueryData;
        }

        public async void Execute(Action<T> onQueryComplete)
        {
            /*if (QueryData != null)
            {
                onQueryComplete(QueryData);
                return;
            }*/

            while (SQL == null)
                await BaseScript.Delay(0);

            if (QueryString.Contains("INSERT") || QueryString.Contains("UPDATE") || QueryString.Contains("DELETE"))
            {
                SQL.execute(QueryString, builtWhereParameters, new Action<T>(onQueryComplete));
            }

            if (QueryString.Contains("SELECT"))
            {
                SQL.execute(QueryString, builtWhereParameters, new Action<T>(onQueryComplete));
            }
        }

        public string BuildQuery()
        {
            var whereString = "WHERE";

            //if(buildWhereParameters)
            {
                builtWhereParameters.Clear();
            }

            var curIdx = 0;
            foreach (var kvp in initialWhereParameters)
            {
                //if(buildWhereParameters) 
                //{
                    whereString += $" {kvp.Key} = @{curIdx}_param AND";

                    builtWhereParameters[$"@{curIdx}_param"] = kvp.Value;

                    curIdx++;
                /*}
                else
                {
                    whereString += $" {kvp.Key} {kvp.Value} AND";
                }*/
            }

            whereString = whereString.Substring(0, whereString.Length - 3);

            return builtQueryString = initialQueryString + $" {whereString}";
        }
    }
}
