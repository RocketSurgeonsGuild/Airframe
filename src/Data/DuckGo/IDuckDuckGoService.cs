using System;
using System.Threading.Tasks;
using DynamicData;

namespace Data
{
    public interface IDuckDuckGoService
    {
        Task Query(string query);

        IObservable<IChangeSet<DuckDuckGoQueryResult, string>> QueryResults { get; }
    }
}