using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Receptserver.Core.Dtos;
using Receptserver.Laegehus.App.Common;
using Receptserver.Laegehus.App.Services;

namespace Receptserver.Laegehus.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly LaegehusApiClient _api;

    public MainWindowViewModel(LaegehusApiClient api)
    {
        _api = api;
        OpretReceptCommand = new RelayCommand(OpretReceptAsync);
        AddOrdinationCommand = new RelayCommand(() => { Ordinationer.Add(new OrdinationInputViewModel()); return Task.CompletedTask; });
        Ordinationer.Add(new OrdinationInputViewModel());
    }

    public ObservableCollection<LaegehusDto> Laegehuse { get; } = new();
    public ObservableCollection<ApotekDto> Apoteker { get; } = new();
    public ObservableCollection<OrdinationInputViewModel> Ordinationer { get; } = new();

    private LaegehusDto? _selectedLaegehus;
    public LaegehusDto? SelectedLaegehus
    {
        get => _selectedLaegehus;
        set => SetField(ref _selectedLaegehus, value);
    }

    private ApotekDto? _selectedApotek;
    public ApotekDto? SelectedApotek
    {
        get => _selectedApotek;
        set => SetField(ref _selectedApotek, value);
    }

    private string _cprNummer = string.Empty;
    public string CprNummer
    {
        get => _cprNummer;
        set => SetField(ref _cprNummer, value);
    }

    private string? _statusMessage;
    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (SetField(ref _statusMessage, value))
                OnPropertyChanged(nameof(ShowStatus));
        }
    }

    private bool _isError;
    public bool IsError
    {
        get => _isError;
        set
        {
            if (SetField(ref _isError, value))
            {
                OnPropertyChanged(nameof(IsSuccess));
            }
        }
    }

    public bool IsSuccess => !IsError;
    public bool ShowStatus => !string.IsNullOrEmpty(StatusMessage);

    public RelayCommand OpretReceptCommand { get; }
    public RelayCommand AddOrdinationCommand { get; }

    public void RemoveOrdination(OrdinationInputViewModel ordination)
    {
        if (Ordinationer.Count > 1)
            Ordinationer.Remove(ordination);
    }

    public async Task LoadInitialDataAsync()
    {
        try
        {
            var nyeLaegehuse = await _api.GetLaegehuseAsync();
            Laegehuse.Clear();
            foreach (var l in nyeLaegehuse) Laegehuse.Add(l);
            SelectedLaegehus = Laegehuse.Count > 0 ? Laegehuse[0] : null;

            var nyeApoteker = await _api.GetApotekerAsync();
            Apoteker.Clear();
            foreach (var a in nyeApoteker) Apoteker.Add(a);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"Kunne ikke hente data fra serveren: {ex.Message}";
        }
    }

    private async Task OpretReceptAsync()
    {
        StatusMessage = null;
        IsError = false;

        if (SelectedLaegehus is null)
        {
            IsError = true;
            StatusMessage = "Vælg et lægehus inden du opretter en recept.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CprNummer))
        {
            IsError = true;
            StatusMessage = "Indtast et CPR-nummer.";
            return;
        }

        if (Ordinationer.Count == 0 || Ordinationer.Any(o => string.IsNullOrWhiteSpace(o.Laegemiddel) || string.IsNullOrWhiteSpace(o.Dosis)))
        {
            IsError = true;
            StatusMessage = "Hver ordination skal have både lægemiddel og dosis.";
            return;
        }

        var request = new OpretReceptRequest(
            SelectedLaegehus.Ydernummer,
            CprNummer,
            SelectedApotek?.Id,
            Ordinationer.Select(o => new OpretOrdinationRequest(
                o.Laegemiddel.Trim(),
                o.Dosis.Trim(),
                o.AntalUdleveringer)).ToList());

        try
        {
            var recept = await _api.OpretReceptAsync(request);
            IsError = false;
            StatusMessage = $"Recept #{recept.Id} oprettet for CPR {recept.CprNummer} ({recept.Ordinationer.Count} ordinationer).";

            CprNummer = string.Empty;
            Ordinationer.Clear();
            Ordinationer.Add(new OrdinationInputViewModel());
            SelectedApotek = null;
        }
        catch (ApiException ex)
        {
            IsError = true;
            StatusMessage = $"Fejl ({ex.StatusCode}): {ex.Message}";
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"Kunne ikke kontakte server: {ex.Message}";
        }
    }
}
