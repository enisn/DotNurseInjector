using DotNurse.Injector.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.Tests.Environment.NamespaceInjectService
{

    public interface IDataProvider<T>
    {
        T Get();
    }

    public interface IMessageDataProvider : IDataProvider<string>
    {
    }

    public class MessageDataProvider : IMessageDataProvider
    {
        public string Get()
        {
            return "Hello World!";
        }
    }

    public interface IDataService<T>
    {
        T Retrieve();
    }

    public interface IMessageDataService : IDataService<string>
    {
    }

    public class MessageDataService : IMessageDataService
    {
        [InjectService]
        public IMessageDataProvider MessageDataProvider { get; set; }
        public string Retrieve()
        {
            return MessageDataProvider.Get();
        }
    }
}
