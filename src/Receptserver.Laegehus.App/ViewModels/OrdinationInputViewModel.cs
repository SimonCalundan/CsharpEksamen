namespace Receptserver.Laegehus.App.ViewModels;

// Repræsenterer en enkelt ordination-række i UI'et. Hver række kan redigeres
// uafhængigt, og UI binder direkte til disse properties.
public class OrdinationInputViewModel : ViewModelBase
{
    private string _laegemiddel = string.Empty;
    public string Laegemiddel
    {
        get => _laegemiddel;
        set => SetField(ref _laegemiddel, value);
    }

    private string _dosis = string.Empty;
    public string Dosis
    {
        get => _dosis;
        set => SetField(ref _dosis, value);
    }

    private int _antalUdleveringer = 1;
    public int AntalUdleveringer
    {
        get => _antalUdleveringer;
        set => SetField(ref _antalUdleveringer, value);
    }
}
