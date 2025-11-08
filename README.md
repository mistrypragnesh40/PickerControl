# Plugin.Maui.SearchablePicker

A customizable, modern, and powerful **Searchable Picker Control** for .NET MAUI that supports **multiple selection**, **server-side pagination**, and **custom templates** — all wrapped in a sleek popup interface.

![2025-11-0820-02-53-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/ccabba7e-02b0-42d7-b1a8-810ec2c47e0f)


## ✨ Features

- 🔍 **Searchable picker** with real-time filtering  
- 🧩 **Custom item templates** for full UI flexibility  
- 📃 **Server-side pagination support** (`IsFetchMoreDataFromServer`)  
- ✅ **Single or multiple selection** modes  
- 📦 **Built-in popup display support** for easy usage  
- 🚀 **MVVM-friendly** — fully command-based  
- 🎨 **Easily stylable and extendable**

## 📦 Installation

Add the NuGet package:

```bash
dotnet add package Plugin.Maui.SearchablePicker
```
Or via Visual Studio NuGet Manager:

```bash
Plugin.Maui.SearchablePicker
```

## 🔥 Quick Start — Simple Example
This example shows a single-select searchable picker used to choose a city from a list.

```bash
// 1️⃣ Prepare your item list
var items = new List<SearchableDataModel>();
var cityNames = new[]
{
    new { Id = 1, Title = "New York",     SubTitle = "United States" },
    new { Id = 2, Title = "Toronto",      SubTitle = "Canada" },
    new { Id = 3, Title = "London",       SubTitle = "United Kingdom" },
    new { Id = 4, Title = "Sydney",       SubTitle = "Australia" },
    new { Id = 5, Title = "Tokyo",        SubTitle = "Japan" },
    new { Id = 6, Title = "Dubai",        SubTitle = "United Arab Emirates" },
    new { Id = 7, Title = "Paris",        SubTitle = "France" },
    new { Id = 8, Title = "Singapore",    SubTitle = "Asia" },
    new { Id = 9, Title = "Berlin",       SubTitle = "Germany" },
    new { Id = 10, Title = "Barcelona",   SubTitle = "Spain" }
};

foreach (var city in cityNames)
{
    items.Add(new SearchableDataModel()
    {
        Id = city.Id,
        Title = city.Title,
        SubTitle = city.SubTitle
    });
}

// 2️⃣ Configure the picker model
var model = new SearchablePickerControlModel()
{
    ItemsSource = items,
    PageSize = 25,
    IsSubmitButtonVisible = true,
    SelectionMode = PickerControl.Models.SelectionMode.Single,
    SearchableItemTemplate = new SearchTemplates(),

    ItemSelectedCommand = new Command<List<SearchableDataModel>>(selectedItems =>
    {
        if (selectedItems?.Any() == true)
        {
            var selected = selectedItems.First();
            Console.WriteLine($"🌍 Selected City: {selected.Title} ({selected.SubTitle})");
        }
    })
};

// 3️⃣ Show the picker popup
var pickerControl = new SearchablePickerControlView(model);
await PopupAction.DisplayPopup(pickerControl);
```

## 🎨 Create Your Own Item Template (XAML)
The picker allows you to define your own row UI using a DataTemplate.
Below is an example that shows a check icon, Title, and SubTitle, and handles tap selection.

You must create this template in your own project.

```
Create file:
DataTemplates/SearchTemplates.xaml
```

```
<?xml version="1.0" encoding="utf-8"?>

<DataTemplate xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
              xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
              xmlns:models="clr-namespace:PickerControl.Models;assembly=PickerControl"
              xmlns:controls="clr-namespace:PickerControl.Control;assembly=PickerControl"
              x:DataType="models:SearchableDataModel"
              x:Class="PickerControlDemo.DataTemplates.SearchTemplates">

    <Grid ColumnSpacing="10" 
          ColumnDefinitions="30,*" 
          RowDefinitions="Auto"
          Padding="8">

        <!-- Check / Uncheck Icon -->
        <Image Grid.Column="0" VerticalOptions="Center" HeightRequest="25">
            <Image.Triggers>
                <DataTrigger TargetType="Image" Binding="{Binding IsChecked}" Value="True">
                    <Setter Property="Source" Value="check_circle.png" />
                </DataTrigger>
                <DataTrigger TargetType="Image" Binding="{Binding IsChecked}" Value="False">
                    <Setter Property="Source" Value="uncheck_circle.png" />
                </DataTrigger>
            </Image.Triggers>
        </Image>

        <!-- Text Layout -->
        <VerticalStackLayout Grid.Column="1" Spacing="2">
            <Label Text="{Binding Title}"
                   FontSize="19"
                   TextColor="Black" />

            <Label Text="{Binding SubTitle}"
                   FontSize="14"
                   TextColor="#666666" />
        </VerticalStackLayout>

        <!-- Tap to Select -->
        <Grid.GestureRecognizers>
            <TapGestureRecognizer
                Command="{Binding Source={x:RelativeSource AncestorType={x:Type controls:SearchablePickerControlViewModel}}, Path=ItemSelectionCommand}"
                CommandParameter="{Binding .}" />
        </Grid.GestureRecognizers>

    </Grid>
</DataTemplate>
```

## 🔥 Multi-Selection Example

In this example, the picker allows the user to select **multiple items**, and the result is returned as a list of `SearchableDataModel`.

```csharp
// 1️⃣ Prepare your item list
var items = new List<SearchableDataModel>();
items.Add(new SearchableDataModel { Id = 1, Title = "Football",   SubTitle = "Sports Category" });
items.Add(new SearchableDataModel { Id = 2, Title = "Basketball", SubTitle = "Sports Category" });
items.Add(new SearchableDataModel { Id = 3, Title = "Tennis",     SubTitle = "Sports Category" });
items.Add(new SearchableDataModel { Id = 4, Title = "Cricket",    SubTitle = "Sports Category" });
items.Add(new SearchableDataModel { Id = 5, Title = "Volleyball", SubTitle = "Sports Category" });

// 2️⃣ Configure the picker for multi-selection
var model = new SearchablePickerControlModel()
{
    ItemsSource = items,
    PageSize = 25,
    IsSubmitButtonVisible = true,     // Shows "Submit" button to finalize selection
    SelectionMode = PickerControl.Models.SelectionMode.Multiple,
    SearchableItemTemplate = new SearchTemplates(),

    // Command receives a list of all selected items
    ItemSelectedCommand = new Command<List<SearchableDataModel>>(selectedItems =>
    {
        Console.WriteLine("✅ Selected Items:");
        foreach (var item in selectedItems)
            Console.WriteLine($"• {item.Title}");
    })
};

// 3️⃣ Display the picker popup
var pickerControl = new SearchablePickerControlView(model);
await PopupAction.DisplayPopup(pickerControl);
```

## ✅ Why Create Your Own Template?

| Benefit | Explanation |
|--------|-------------|
| **Full UI Control** | You decide exactly how each row should look and behave. |
| **Supports Any App Theme** | Works seamlessly with Light, Dark, and custom brand styling. |
| **Works for Single & Multiple Selection** | The control handles the logic — your template simply displays state. |
| **Uses Your Own Icons / Styles** | Choose any icons (PNG, SVG, FontIcon) to match your design system. |

## 🏷️ Adding a Custom Header (Optional)

You can display any custom UI at the top of the picker using the `HeaderView` property.
This is useful for showing titles, instructions, filters, etc.

### Example

```csharp
var headerView = new VerticalStackLayout
{
    Padding = new Thickness(12, 8),
    Spacing = 6,
    Children =
    {
        new Label
        {
            Text = "Select Sports",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        },
        new Label
        {
            Text = "You can choose multiple items from the list below.",
            FontSize = 13,
            TextColor = Colors.Gray
        }
    }
};

var model = new SearchablePickerControlModel()
{
    // ✅ Show custom header inside popup
    HeaderView = headerView,
};

// Display popup
var pickerControl = new SearchablePickerControlView(model);
await PopupAction.DisplayPopup(pickerControl);
```


## 🌐 Fetch Data From Server (Load More / Pagination)

If your dataset is large, you can load data **page-by-page** from an API or database
by assigning a `LoadMoreDataCommand`.  
The control will call this function when more items are needed (scroll or search).

To load data from a server while scrolling, **enable** server paging:

```csharp
IsFetchMoreDataFromServer = true
```
### Example
```
```csharp
var model = new SearchablePickerControlModel()
{
    PageSize = 25,                                     // Number of items to load per request
    IsSubmitButtonVisible = true,
    SelectionMode = PickerControl.Models.SelectionMode.Multiple,
    SearchableItemTemplate = new PickerControlDemo.DataTemplates.SearchTemplates(),

    // ✅ Enable dynamic server data loading
    IsFetchMoreDataFromServer = true,

    // Load data from server when picker requests more items
    LoadMoreDataCommand = async (offset, limit) =>
    {
        // Example: simulate delay / API
        await Task.Delay(100);

        // Example: return dynamic items (this is where you'd call your API)
        var newItems = Enumerable.Range(offset + 1, limit)
            .Select(i => new SearchableDataModel
            {
                Id = i,
                Title = $"Item {i}",
                SubTitle = $"Loaded dynamically from server (#{i})"
            })
            .ToList();

        return newItems; // ✅ Must return list of items
    },

    // Returned list of selected items when user taps Submit
    ItemSelectedCommand = new Command<List<SearchableDataModel>>(selectedItems =>
    {
        Console.WriteLine("✅ Selected Items:");
        foreach (var item in selectedItems)
            Console.WriteLine($"• {item.Title}");
    })
};

// Display popup
var pickerControl = new SearchablePickerControlView(model);
await PopupAction.DisplayPopup(pickerControl);
```
### 📜 License
MIT

### ❤️ Author
Pragnesh Mistry

