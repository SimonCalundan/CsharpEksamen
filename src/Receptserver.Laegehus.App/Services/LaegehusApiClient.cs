using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Receptserver.Core.Dtos;

namespace Receptserver.Laegehus.App.Services;

public class LaegehusApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public LaegehusApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<LaegehusDto>> GetLaegehuseAsync()
    {
        return await _http.GetFromJsonAsync<List<LaegehusDto>>("api/laegehus/laegehuse", JsonOptions)
            ?? new List<LaegehusDto>();
    }

    public async Task<IReadOnlyList<ApotekDto>> GetApotekerAsync()
    {
        return await _http.GetFromJsonAsync<List<ApotekDto>>("api/laegehus/apoteker", JsonOptions)
            ?? new List<ApotekDto>();
    }

    public async Task<ReceptDto> OpretReceptAsync(OpretReceptRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/laegehus/recepter", request, JsonOptions);
        if (!response.IsSuccessStatusCode)
        {
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
        return await response.Content.ReadFromJsonAsync<ReceptDto>(JsonOptions)
            ?? throw new ApiException("Tomt svar fra server", (int)response.StatusCode);
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
