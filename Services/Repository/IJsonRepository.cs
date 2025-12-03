using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Ixjok.Models.Note;
using Ixjok.Tools;

namespace Ixjok.Services.Repository;

public interface IJsonRepository<T> : IRepository<T>
    where T : class
{

}