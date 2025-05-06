// using System;
// using System.IO;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
//
// namespace Extensions.Newtonsoft.Json
// {
// 	public static class AsyncJsonSerializerExtensions
//     {
//         public static async Task<T> DeserializeJsonAsync<T>(this string data, JsonSerializer serializer)
//         {
//             if (data == null) throw new ArgumentNullException(nameof(data));
//             if (serializer == null) throw new ArgumentNullException(nameof(serializer));
//
//             byte[] bytes = Encoding.UTF8.GetBytes(data);
//             using MemoryStream stream = new(bytes);
//             return await stream.DeserializeJsonAsync<T>(serializer).ConfigureAwait(false);
//         }
//
//         public static async Task<T> DeserializeJsonAsync<T>(this Stream stream, JsonSerializer serializer)
//         {
//             if (stream == null) throw new ArgumentNullException(nameof(stream));
//             if (serializer == null) throw new ArgumentNullException(nameof(serializer));
//
//             using StreamReader reader = new(stream, Encoding.UTF8, true, 1024, true);
//             using JsonTextReader jsonReader = new(reader);
//             return await Task.FromResult(serializer.Deserialize<T>(jsonReader));
//         }
//
//         public static async Task<string> SerializeJsonAsync<T>(this T instance, JsonSerializer serializer)
//         {
//             if (serializer == null) throw new ArgumentNullException(nameof(serializer));
//
//             using MemoryStream stream = new();
//             await instance.SerializeJsonAsync(stream, serializer).ConfigureAwait(false);
//             stream.Position = 0;
//
//             using StreamReader reader = new(stream, Encoding.UTF8);
//             return await reader.ReadToEndAsync().ConfigureAwait(false);
//         }
//
//         public static async Task SerializeJsonAsync<T>(this T instance, Stream toStream, JsonSerializer serializer)
//         {
//             if (toStream == null) throw new ArgumentNullException(nameof(toStream));
//             if (serializer == null) throw new ArgumentNullException(nameof(serializer));
//
//             using StreamWriter writer = new(toStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
//             using JsonTextWriter jsonWriter = new(writer)
//             {
//                 Formatting = serializer.Formatting
//             };
//
//             serializer.Serialize(jsonWriter, instance);
//             await jsonWriter.FlushAsync().ConfigureAwait(false);
//         }
//     }
// }