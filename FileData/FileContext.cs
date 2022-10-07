using System.Text.Json;
using Domain.Models;

namespace FileData;

public class FileContext
{
    private const string filePath = "data.json";
        
    private DataContainer? _dataContainer;

    public ICollection<Todo> Todos
    {
        get
        {
            LoadData();
            return _dataContainer!.Todos;
        }
    }

    public ICollection<User> Users
    {
        get
        {
            LoadData();
            return _dataContainer!.Users;
        }
    }

    private void LoadData()
    {
        if (_dataContainer != null) return;
        if (!File.Exists(filePath))
        {
            _dataContainer = new()
            {
                Todos = new List<Todo>(),
                Users = new List<User>()
            };
            return;
        }

        string content = File.ReadAllText(filePath);
        _dataContainer = JsonSerializer.Deserialize<DataContainer>(content);
    }

    public void SaveChanges()
    {
        string serialized = JsonSerializer.Serialize(_dataContainer, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(filePath, serialized);
        _dataContainer = null;
    }
}