using MultiLayeredService.Repositories.Abstraction;
using MultiLayeredService.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNurse.Injector.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace MultiLayeredService.Repositories.Concrete
{
    /*
     * All data is stored in memory.
     * So this service must be singleton.
     */
    [ServiceLifeTime(ServiceLifetime.Singleton)]
    public class BookRepository : IBookRepository
    {
        /* FAKE DATA IS STORED in MEMORY with inital 1000 registry */
        private readonly IList<Book> fakeDataStore = RandomFactory.Repeat(() => new Book
        {
            Author = RandomFactory.GetString(18, 32),
            Title = RandomFactory.GetString(10, 46),
            Description = RandomFactory.GetString(120, 240),
            Id = Guid.NewGuid().ToString(),
            Rate = RandomFactory.GetDouble(1, 5),
            TotalPage = RandomFactory.GetInt32(min: 110, max: 950),
            Year = RandomFactory.GetInt32(min: 1990, max: DateTime.UtcNow.Year),
            CreateDate = RandomFactory.GetDateTime(),
            UpdateDate = RandomFactory.GetDateTime(),
        }, 1000).ToList();

        public IList<Book> Get()
        {
            return fakeDataStore;
        }

        public Book GetSingle(string id)
        {
            return fakeDataStore.FirstOrDefault(x => x.Id == id);
        }
    }
}
