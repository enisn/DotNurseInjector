using MultiLayeredService.Repositories.Models;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;

namespace MultiLayeredService.Repositories.Abstraction
{
    public interface IBookRepository
    {
        IList<Book> Get();

        Book GetSingle(string id);
    }
}
