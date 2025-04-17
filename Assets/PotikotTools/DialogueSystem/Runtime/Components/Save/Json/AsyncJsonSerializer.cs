using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
public class AsyncJsonSerializer : JsonSerializer
{
    public AsyncJsonSerializer() : base() { }

    /// <summary>
    /// Асинхронная десериализация из потока.
    /// </summary>
    public async Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        Encoding encoding = Encoding.UTF8;
        using var streamReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true, 1024, true);
        using var jsonReader = new JsonTextReader(streamReader);

        // JsonTextReader работает синхронно, но мы оборачиваем в Task
        return await Task.Run(() => Deserialize<T>(jsonReader), cancellationToken);
    }

    /// <summary>
    /// Асинхронная сериализация в поток.
    /// </summary>
    public async Task SerializeAsync(Stream stream, object? value, CancellationToken cancellationToken = default)
    {
        Encoding encoding = Encoding.UTF8;
        using var streamWriter = new StreamWriter(stream, encoding, 1024, true);
        using var jsonWriter = new JsonTextWriter(streamWriter)
        {
            AutoCompleteOnClose = false // чтобы не писать окончание, если это не нужно
        };

        // JsonTextWriter работает синхронно, но мы оборачиваем в Task
        await Task.Run(() => Serialize(jsonWriter, value), cancellationToken);
        await streamWriter.FlushAsync();
    }

    /// <summary>
    /// Асинхронная десериализация из строки.
    /// </summary>
    public async Task<T?> DeserializeFromStringAsync<T>(string json, CancellationToken cancellationToken = default)
    {
        using var reader = new StringReader(json);
        using var jsonReader = new JsonTextReader(reader);
        return await Task.Run(() => Deserialize<T>(jsonReader), cancellationToken);
    }

    /// <summary>
    /// Асинхронная сериализация в строку.
    /// </summary>
    public async Task<string> SerializeToStringAsync(object? value, CancellationToken cancellationToken = default)
    {
        using var sw = new StringWriter();
        using var writer = new JsonTextWriter(sw);
        await Task.Run(() => Serialize(writer, value), cancellationToken);
        await writer.FlushAsync();
        return sw.ToString();
    }
}
}