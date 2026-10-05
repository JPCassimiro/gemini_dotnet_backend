namespace jsonToGemin.Interfaces;

public interface IModelListInterface
{
    Task SaveModelList(string provider, List<string> models);
    Task<List<string>> GetModelListFromFile(string provider);
}