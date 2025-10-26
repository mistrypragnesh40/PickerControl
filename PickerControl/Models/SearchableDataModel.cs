using CommunityToolkit.Mvvm.ComponentModel;

namespace PickerControl.Models;

public enum SelectionMode
{
    Single,
    Multiple,
}
public class SearchableDataModel : ObservableObject
{
    public object ReferenceObject { get; set; }
    public int Id { get; set; }
    public int SubId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Logo { get; set; }

    private bool _isChecked;

    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }


    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private bool _isSeperatorVisible = true;

    public bool IsSeperatorVisible
    {
        get => _isSeperatorVisible;
        set => SetProperty(ref _isSeperatorVisible, value);
    }
}