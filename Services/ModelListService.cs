using System.Diagnostics;
using System.Text.Json;
using jsonToGemin.Interfaces;
using jsonToGemin.Utilities;

public class ModelListService : IModelListInterface
{
    private readonly string modelListFolder = "./ModelLists/";
    private readonly CreatorController _creatorController;
    private readonly JsonUtility _jsonUtility;

    public ModelListService(CreatorController creatorController)
    {
        _creatorController = creatorController;
        _jsonUtility = new JsonUtility();
    }

    public async Task SaveModelList(string provider, List<string> models)
    {
        string modelListPath = modelListFolder+provider;
        await _jsonUtility.WriteJsonFile(models, modelListPath);
    }

    public async Task<List<string>> GetModelListFromFile(string provider)
    {
        string jsonContent = _jsonUtility.ReadJsonFileString(modelListFolder+provider);
        List<string> deserializedContent = JsonSerializer.Deserialize<List<string>>(jsonContent);
        return deserializedContent;
    }

    public async Task<Dictionary<string,List<string>>> ReturnAllModelsList()
    {
        Console.WriteLine($"ModelListService ReturnAllModelList");
        Dictionary<string,List<string>> fullModelList = [];
        List<string> namesList = _creatorController.ReturnProviderNameList();
        foreach(string name in namesList)
        {
            var provider = _creatorController.FetchProvider(name);
            var providerModelList = await provider.GetModelList();
            fullModelList.Add(name,providerModelList);
            SaveModelList(provider.GetProviderName(), providerModelList);
        }
        Console.WriteLine($"ModelListService ReturnAllModelList fullModelList: {fullModelList.Count}");
        return fullModelList;
    }

}