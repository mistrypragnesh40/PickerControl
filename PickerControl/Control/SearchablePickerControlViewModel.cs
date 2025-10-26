using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiPopup;
using MvvmHelpers;
using PickerControl.Models;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using SelectionMode = PickerControl.Models.SelectionMode;

namespace PickerControl.Control;

public partial class SearchablePickerControlViewModel : ObservableObject
{
    #region Properties

    [ObservableProperty]
    private bool _isBusy;


    [ObservableProperty]
    private bool _isLoadingMoreRecords;
    private SearchablePickerControlModel _searchModel;

    public SearchablePickerControlModel SearchModel
    {
        get => _searchModel;
        set => SetProperty(ref _searchModel, value);
    }


    public ObservableRangeCollection<SearchableDataModel> SearchResults { get; set; } = new();
    private List<SearchableDataModel> _filterList { get; set; }
    private List<SearchableDataModel> _allResults { get; set; } = new List<SearchableDataModel>();

    private CancellationTokenSource _searchCancellationTokenSource;
    [ObservableProperty]
    private string _searchText;

    #endregion

    #region Methods

    public SearchablePickerControlViewModel(SearchablePickerControlModel model)
    {
        SearchModel = model;
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = DebouncedSearchAsync(value);
    }

    public async Task BindDataToUI()
    {
        IsBusy = true;
        await Task.Delay(100).ConfigureAwait(false);

        if (SearchModel?.ItemsSource?.Count > 0)
        {
            _allResults = SearchModel.ItemsSource.Cast<SearchableDataModel>().ToList();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SearchResults.ReplaceRange(_allResults.Take(SearchModel.PageSize));
            });
        }
        else
        {
            await MainThread.InvokeOnMainThreadAsync(() => SearchResults.Clear());
        }

        IsBusy = false;
    }
    private async Task SearchResultAsync(string? value, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _filterList = null;
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SearchResults.ReplaceRange(_allResults.Take(SearchModel.PageSize));
            });
            return;
        }

        var comparison = StringComparison.OrdinalIgnoreCase;
        // filter on Title safely
        _filterList = _allResults?
            .Where(f => !string.IsNullOrEmpty(f?.Title) && f.Title.IndexOf(value, comparison) >= 0)
            .ToList() ?? new List<SearchableDataModel>();

        if (ct.IsCancellationRequested) return;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            SearchResults.ReplaceRange(_filterList.Take(SearchModel.PageSize));
        });
    }

    private async Task DebouncedSearchAsync(string? value)
    {
        // cancel previous pending search
        try
        {
            _searchCancellationTokenSource?.Cancel();
            _searchCancellationTokenSource?.Dispose();
        }
        catch { /* ignore */ }

        _searchCancellationTokenSource = new CancellationTokenSource();
        var ct = _searchCancellationTokenSource.Token;

        // debounce delay
        try
        {
            await Task.Delay(250, ct).ConfigureAwait(false); // slightly longer debounce
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (ct.IsCancellationRequested) return;

        await SearchResultAsync(value, ct).ConfigureAwait(false);
    }

    #endregion

    #region Commands

    public ICommand SubmitCommand => new Command(async () =>
    {
        if (!SearchModel.IsSubmitButtonVisible) return;

        if (!SearchResults.Any(f => f.IsChecked)) return;

        var selectedItems = SearchResults.Where(f => f.IsChecked).ToList();

        SearchText= selectedItems.Count.ToString();
        // execute if available
        try
        {
            SearchModel.ItemSelectedCommand?.Execute(selectedItems);
        }
        catch { /* swallow execution exceptions; view layer should handle */ }

        await PopupAction.ClosePopup();
    });

    public ICommand ItemSelectionCommand => new Command<SearchableDataModel>(async item =>
    {
        if (item == null) return;

        if (SearchModel.IsSubmitButtonVisible)
        {
            if (SearchModel.SelectionMode == SelectionMode.Single)
            {
                _allResults?.ForEach(f => f.IsChecked = false);
                foreach (var r in SearchResults) r.IsChecked = false;
                item.IsChecked = true;
            }
            else
            {
                item.IsChecked = !item.IsChecked;
            }
        }
        else
        {
            try
            {
                SearchModel.ItemSelectedCommand?.Execute(item);
            }
            catch { /* ignore */ }

            if (!SearchModel.IsShowLoaderOnItemSelection)
                await PopupAction.ClosePopup();
        }
    });
    private bool _isLoadingMoreGuard;

    public ICommand LoadMoreCommand => new Command(async () =>
    {
        if (IsLoadingMoreRecords || _isLoadingMoreGuard) return;
        _isLoadingMoreGuard = true;

        try
        {
            if (SearchModel.IsFetchMoreDataFromServer)
            {
                IsLoadingMoreRecords = true;
                if (_filterList?.Count > 0)
                {
                    await Task.Delay(50).ConfigureAwait(false);
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        SearchResults.AddRange(_filterList.Skip(SearchResults.Count).Take(SearchModel.PageSize).ToList()));
                    IsLoadingMoreRecords = false;
                    return;
                }

                var newData = await SearchModel.LoadMoreDataCommand?.Invoke(SearchResults.Count, SearchModel.PageSize);
                if (newData != null && newData.Any())
                {
                    _allResults.AddRange(newData);
                    await MainThread.InvokeOnMainThreadAsync(() => SearchResults.AddRange(newData));
                }
                IsLoadingMoreRecords = false;
                return;
            }

            IsLoadingMoreRecords = true;
            if (_filterList?.Count > 0)
            {
                await Task.Delay(50).ConfigureAwait(false);
                await MainThread.InvokeOnMainThreadAsync(() =>
                    SearchResults.AddRange(_filterList.Skip(SearchResults.Count).Take(SearchModel.PageSize).ToList()));
            }
            else if (SearchResults.Count != _allResults.Count)
            {
                await Task.Delay(50).ConfigureAwait(false);
                await MainThread.InvokeOnMainThreadAsync(() =>
                    SearchResults.AddRange(_allResults.Skip(SearchResults.Count).Take(SearchModel.PageSize).ToList()));
            }
            IsLoadingMoreRecords = false;
        }
        finally
        {
            _isLoadingMoreGuard = false;
        }
    });


    #endregion
}