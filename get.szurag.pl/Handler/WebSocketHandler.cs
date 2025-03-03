using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace get.szurag.pl.Handler
{
    public class WebSocketHandler
    {
        private const int BufferSize = 4096; // 4KB buffer

        public async Task HandleConnection(HttpContext context, WebSocket webSocket)
        {
            byte[] buffer = new byte[BufferSize];
            string fileName = null;
            FileStream fileStream = null;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result =
                        await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Odbieranie metadanych (np. nazwa pliku)
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var metadata = JObject.Parse(message);
                        fileName = metadata["filename"]?.ToString() ?? $"file_{Guid.NewGuid()}.bin";

                        var currentPath = context.Session.GetString("CurrentPath");

                        
                        var filePath = Path.Combine("public", fileName);
                        if (currentPath != "public")
                        {
                            filePath = Path.Combine("public", context.Session.GetString("CurrentPath")!,
                                fileName);
                        }
                        
                        Directory.CreateDirectory("public"); // Upewniamy się, że katalog istnieje

                        fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                        Console.WriteLine($"Otworzono plik: {fileName}");
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary && fileStream != null)
                    {
                        // Odbieramy binarne dane i zapisujemy je od razu
                        await fileStream.WriteAsync(buffer, 0, result.Count);
                        await fileStream.FlushAsync();
                    }

                    if (result.EndOfMessage) // Jeśli wiadomość jest kompletna
                    {
                        Console.WriteLine($"Częściowy zapis pliku: {fileName}");
                    }

                    if (result.CloseStatus.HasValue)
                    {
                        break; // Zamknięcie WebSocketa
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd WebSocket: {ex.Message}");
            }
            finally
            {
                if (fileStream != null)
                {
                    await fileStream.FlushAsync();
                    fileStream.Close();
                    Console.WriteLine($"Plik zapisany: {fileName}");
                }

                if (webSocket.State != WebSocketState.Closed)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Zamknięto połączenie",
                        CancellationToken.None);
                }
            }
        }
    }
}