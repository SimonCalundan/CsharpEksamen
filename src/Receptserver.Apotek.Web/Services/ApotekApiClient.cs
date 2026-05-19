using System.Net.Http.Json;
using System.Text.Json;
using Receptserver.Core.Dtos;

namespace Receptserver.Apotek.Web.Services;

public class ApotekApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ApotekApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ReceptDto>> SoegRecepterAsync(string cpr)
    {
        var encoded = Uri.EscapeDataString(cpr);
        var response = await _http.GetAsync($"api/apotek/recepter?cpr={encoded}");
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<List<ReceptDto>>(JsonOptions)
            ?? new List<ReceptDto>();
    }

    public async Task<OrdinationDto> UdleverAsync(int ordinationId)
    {
        var response = await _http.PostAsync($"api/apotek/ordinationer/{ordinationId}/udlever", null);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<OrdinationDto>(JsonOptions)
            ?? throw new ApiException("Tomt svar fra server", (int)response.StatusCode);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        string message;
        try
        {
            var err = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions);
            message = err?.Error ?? response.ReasonPhrase ?? "Ukendt fejl";
        }
        catch
        {
            message = response.ReasonPhrase ?? "Ukendt fejl";
        }
        throw new ApiException(message, (int)response.StatusCode);
    }
}

public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
