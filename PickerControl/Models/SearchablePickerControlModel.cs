using System.Windows.Input;

namespace PickerControl.Models;

public class SearchablePickerControlModel
{
    public bool IsShowLoaderOnItemSelection { get; set; }
    public View HeaderView { get; set; }
    public double HeightRequest { get; set; } = 480;
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Single; // this only when template is Selection
    public string SubmitButtonText { get; set; } = "Submit";
    public bool IsSubmitButtonVisible { get; set; } = false;
    public DataTemplate SearchableItemTemplate { get; set; } 
    public int PageSize { get; set; } = 25;
    public IList<SearchableDataModel> ItemsSource { get; set; }
    public ICommand ItemSelectedCommand { get; set; }

    public bool IsFetchMoreDataFromServer { get; set; }
       /// <summary>
    /// Optional async method to fetch more items from server.
    /// Parameters: (pageIndex, pageSize)
    /// Return: List of SearchableDataModel to append to existing results.
    /// </summary>
    public Func<int, int, Task<IEnumerable<SearchableDataModel>>>? LoadMoreDataCommand { get; set; }
}