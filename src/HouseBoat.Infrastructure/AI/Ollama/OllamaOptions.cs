namespace HouseBoat.Infrastructure.AI.Ollama;

public class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string Endpoint { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "phi3:mini";
}