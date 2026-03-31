using System.Dynamic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Broadcaster.Business.Models.Config;
using Broadcaster.Business.Services.RepoBases;

namespace Broadcaster.Business.Services;

public class WidgetService
{
    private readonly ILogger<WidgetService> _logger;
    private readonly WidgetRepository _repo;

    public WidgetService(
        ILogger<WidgetService> logger,
        WidgetRepository repo
    )
    {
        _logger = logger;
        _repo = repo;
    }

    public Task<IEnumerable<WidgetObject>> GetAsync() => _repo.GetAsync();

    public Task<WidgetObject?> GetByIdAsync(int collectionId) => _repo.GetByIdAsync(collectionId);

    public Task<IEnumerable<WidgetObject>> GetByNameAsync(string name) => _repo.GetByNameAsync(name);

    public Task<WidgetObject> AddAsync(WidgetObject widget) => _repo.InsertAsync(widget);

    public Task UpdateAsync(WidgetObject widget) => _repo.UpdateAsync(widget);

    public Task DeleteAsync(WidgetObject widget) => _repo.DeleteAsync(widget);

    public Task DeleteAsync(int widgetId) => _repo.DeleteByIdAsync(widgetId);
}

public partial class WidgetRepository : BoundTableRepositoryBase<WidgetObject>
{
    public WidgetRepository(
        ILogger<WidgetRepository> logger,
        IOptions<DataAccessSettings> config,
        BoundTableBinder tableBinder) : base(logger, config, tableBinder)
    {
    }

    public async Task<IEnumerable<WidgetObject>> GetByNameAsync(string name)
    {
        await Task.FromResult(0);
        IDictionary<string, object?> expandoObject = new ExpandoObject();
        expandoObject[nameof(WidgetObject.Name)] = name;
        var res = Get((ExpandoObject)expandoObject).OrderBy(x => x.Name);
        return res;
    }
}