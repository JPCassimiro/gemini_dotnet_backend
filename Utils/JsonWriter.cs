using System.Text.Json;

namespace jsonToGemin.Utilities;

public class JsonUtility
{

    private JsonSerializerOptions jsonConfig;

    public JsonUtility()
    {
        jsonConfig = new JsonSerializerOptions{WriteIndented = true};
    }

    public string SerializeObject(object target)
    {
        try
        {
            if (target != null)
            {
                if (target is string)
                {   
                    return (string)target;
                }
                string jsonString = JsonSerializer.Serialize(target, this.jsonConfig);
                return jsonString;
            }
            else
            {
                throw new Exception("Serialization error: null object");
            }
        } catch (Exception e)
        {
            Console.WriteLine($"JsonWriter SerializeObject error: {e}");
            throw;
        }
    }

    public async Task WriteJsonFile(object jsonData, string filePath)
    {
        try
        {
            if (jsonData != null && filePath != null)
            {
                string? directory = Path.GetDirectoryName(filePath);
                Console.WriteLine($"JsonWriter WriteJsonFile directory: {directory}");

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await using FileStream createStream = File.Create(filePath+".json");
                Console.WriteLine($"JsonWriter WriteJsonFile directory: {createStream.Name}");
                await JsonSerializer.SerializeAsync(createStream, jsonData);
            }
        } catch(Exception e)
        {
            Console.WriteLine($"JsonWriter WriteJsonFile error: {e}");
            throw;
        }
    }

    public string ReadJsonFileString(string filepath)
    {
        return File.ReadAllText(filepath);
    } 

    public async Task ConvertWriteProcess(object target, string filePath)
    {
        string jsonData = SerializeObject(target);
        await WriteJsonFile(jsonData, filePath);
    }

}