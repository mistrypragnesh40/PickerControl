using MauiPopup.Views;
using PickerControl.Models;

namespace PickerControl.Control;

public partial class SearchablePickerControlView: BasePopupPage
{
    private SearchablePickerControlViewModel? _viewModel=> this.BindingContext as SearchablePickerControlViewModel;
    
    public SearchablePickerControlView(SearchablePickerControlModel model)
    {
        InitializeComponent();

        this.BindingContext = new SearchablePickerControlViewModel(model);
    }

    private void ScrollView_OnScrolled(object? sender, ScrolledEventArgs e)
    {
        var scrollView = sender as ScrollView;
        if (scrollView == null) return;

       double totalScrollableHeight = scrollView.ContentSize.Height - scrollView.Height;

    // Check if user has scrolled near the bottom (within 10%)
    if (totalScrollableHeight > 0 && e.ScrollY >= totalScrollableHeight * 0.9)
    {
        if (!_viewModel.IsLoadingMoreRecords)
            _viewModel?.LoadMoreCommand.Execute(null);
    }
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel?.BindDataToUI();
    }
}