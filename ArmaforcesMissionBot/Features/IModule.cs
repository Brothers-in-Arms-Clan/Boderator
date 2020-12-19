using System;
using System.Threading.Tasks;

namespace ArmaforcesMissionBot.Features {
    public interface IModule {
        Task ReplyWithException<T>(string message = null) where T : Exception, new();
    }
}
