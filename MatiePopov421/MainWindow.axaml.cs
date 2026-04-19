using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using MatiePopov421.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MatiePopov421
{
    public partial class MainWindow : Window
    {
        private int currentPage = 1;
        private int itemsPerPage = 3;
        private int totalItems = 0;
        private int currentCollectionId = 0;
        private int currentTypeId = 1;
        private string sortOrder = "ASC";
        private string searchText = "";

        private static MatiedbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MatiedbContext>()
                .UseNpgsql(AppConfig.ConnectionString)
                .Options;
            return new MatiedbContext(options);
        }

        public MainWindow()
        {
            InitializeComponent();

            LoadImages();
            SetupUserInfo();
            _ = LoadCollectionFilter();

            Avalonia.Threading.Dispatcher.UIThread.Post(() => SetTypeFilter(1));

            this.FindControl<Border>("titleBar")!.PointerPressed += (_, e) => BeginMoveDrag(e);

            this.FindControl<Button>("minimizeBtn")!.Click += (_, _) =>
                WindowState = WindowState.Minimized;

            this.FindControl<Button>("closeBtn")!.Click += async (_, _) =>
                await ConfirmExit();

            this.FindControl<Button>("prevBtn")!.Click += (_, _) =>
            {
                if (currentPage > 1) { currentPage--; _ = LoadData(); }
            };

            this.FindControl<Button>("nextBtn")!.Click += (_, _) =>
            {
                int max = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
                if (max < 1) max = 1;
                if (currentPage < max) { currentPage++; _ = LoadData(); }
            };

            var addBtn = this.FindControl<Button>("addServiceBtn")!;
            addBtn.IsVisible = AppConfig.IsModerator;
            addBtn.Click += (_, _) =>
            {
                var win = new AddEditWindow(0,
                    currentCollectionId > 0 ? currentCollectionId : 1,
                    currentTypeId > 0 ? currentTypeId : 1);
                win.ShowDialog(this).ContinueWith(_ =>
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => _ = LoadData()));
            };

            this.FindControl<Button>("exitBtn")!.Click += async (_, _) => await GoToLogin();

            this.FindControl<Button>("tabKustomBtn")!.Click += (_, _) => SetTypeFilter(1);
            this.FindControl<Button>("tabCosplayBtn")!.Click += (_, _) => SetTypeFilter(2);

            var searchBox = this.FindControl<TextBox>("searchBox")!;
            searchBox.TextChanged += (_, _) =>
            {
                searchText = searchBox.Text ?? "";
                currentPage = 1;
                _ = LoadData();
            };

            var sortCombo = this.FindControl<ComboBox>("sortCombo")!;
            sortCombo.SelectedIndex = 0;
            sortCombo.SelectionChanged += (_, _) =>
            {
                sortOrder = sortCombo.SelectedIndex == 0 ? "ASC" : "DESC";
                currentPage = 1;
                _ = LoadData();
            };

            var list = this.FindControl<ListBox>("servicesList")!;
            list.DoubleTapped += (_, _) =>
            {
                if (!AppConfig.IsModerator) return;
                if (list.SelectedItem is ServiceItem item)
                {
                    var win = new AddEditWindow(item.Id, item.CollectionId, item.TypeId);
                    win.ShowDialog(this).ContinueWith(_ =>
                        Avalonia.Threading.Dispatcher.UIThread.Post(() => _ = LoadData()));
                }
            };
        }

        private void SetTypeFilter(int typeId)
        {
            currentTypeId = typeId;
            currentPage = 1;

            var activeColor = new SolidColorBrush(Color.Parse("#7C3AED"));
            var activeFg = new SolidColorBrush(Colors.White);
            var inactiveColor = new SolidColorBrush(Color.Parse("#EDE9FE"));
            var inactiveFg = new SolidColorBrush(Color.Parse("#5B21B6"));

            var tabK = this.FindControl<Button>("tabKustomBtn")!;
            var tabC = this.FindControl<Button>("tabCosplayBtn")!;

            tabK.Background = typeId == 1 ? activeColor : inactiveColor;
            tabK.Foreground = typeId == 1 ? activeFg : inactiveFg;
            tabC.Background = typeId == 2 ? activeColor : inactiveColor;
            tabC.Foreground = typeId == 2 ? activeFg : inactiveFg;

            _ = LoadData();
        }

        private async Task GoToLogin()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Выход из аккаунта", "Вы уверены, что хотите выйти из аккаунта?", ButtonEnum.YesNo);
            var result = await box.ShowWindowDialogAsync(this);
            if (result != ButtonResult.Yes) return;

            AppConfig.CurrentUser = null;
            var login = new LoginWindow();
            if (Avalonia.Application.Current?.ApplicationLifetime
                is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = login;
            login.Show();
            Close();
        }

        private void LoadImages()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var iconPath = Path.Combine(basePath, "Assets", "Logo.ico");
            if (File.Exists(iconPath))
                this.Icon = new WindowIcon(iconPath);

            var logoPath = Path.Combine(basePath, "Assets", "Logo.png");
            if (File.Exists(logoPath))
            {
                var bmp = new Bitmap(logoPath);
                this.FindControl<Image>("logoImage")!.Source = bmp;
                this.FindControl<Image>("titleLogoImage")!.Source = bmp;
            }
        }

        private void SetupUserInfo()
        {
            var user = AppConfig.CurrentUser;
            if (user == null) return;
            this.FindControl<TextBlock>("userNameText")!.Text = user.Fullname ?? user.Username;
            this.FindControl<TextBlock>("userRoleText")!.Text = user.Role?.Name ?? "";
            this.FindControl<TextBlock>("userBalanceText")!.Text = $"Баланс: {user.Balance:N0} ₽";
        }

        private async Task LoadCollectionFilter()
        {
            try
            {
                await using var ctx = CreateContext();
                var collections = await ctx.Collections.OrderBy(c => c.Id).ToListAsync();

                var combo = this.FindControl<ComboBox>("collectionCombo")!;
                var items = new List<CollectionItem>();
                items.Add(new CollectionItem { Id = 0, Name = "Все коллекции" });
                foreach (var c in collections)
                    items.Add(new CollectionItem { Id = c.Id, Name = c.Name });

                combo.ItemsSource = items;
                combo.SelectedIndex = 0;
                combo.SelectionChanged += (_, _) =>
                {
                    if (combo.SelectedItem is CollectionItem sel)
                    {
                        currentCollectionId = sel.Id;
                        currentPage = 1;
                        _ = LoadData();
                    }
                };

                _ = LoadData();
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось загрузить коллекции:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }

        private async Task LoadData()
        {
            try
            {
                await using var ctx = CreateContext();

                var query = ctx.Services.AsQueryable();

                if (currentCollectionId > 0)
                    query = query.Where(s => s.Collectionid == currentCollectionId);

                if (currentTypeId > 0)
                    query = query.Where(s => s.Typeid == currentTypeId);

                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(s => s.Title.ToLower().Contains(searchText.ToLower()));

                totalItems = await query.CountAsync();

                List<Service> dbItems;
                if (sortOrder == "ASC")
                    dbItems = await query.OrderBy(s => s.Title)
                                        .Skip((currentPage - 1) * itemsPerPage)
                                        .Take(itemsPerPage).ToListAsync();
                else
                    dbItems = await query.OrderByDescending(s => s.Title)
                                        .Skip((currentPage - 1) * itemsPerPage)
                                        .Take(itemsPerPage).ToListAsync();

                var items = new List<ServiceItem>();
                foreach (var s in dbItems)
                {
                    var item = new ServiceItem
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description ?? "",
                        ImagePath = s.Imagepath ?? "",
                        CollectionId = s.Collectionid,
                        TypeId = s.Typeid,
                        Price = s.Price,
                        LastModifiedAt = s.Lastmodifiedat
                    };

                    var imgPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "Assets", item.ImagePath);
                    if (File.Exists(imgPath))
                        item.Photo = new Bitmap(imgPath);

                    items.Add(item);
                }

                this.FindControl<ListBox>("servicesList")!.ItemsSource = items;

                int maxPage = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
                if (maxPage < 1) maxPage = 1;
                if (currentPage > maxPage) currentPage = maxPage;

                int first = totalItems == 0 ? 0 : (currentPage - 1) * itemsPerPage + 1;
                int last = Math.Min(currentPage * itemsPerPage, totalItems);

                this.FindControl<TextBlock>("pageInfoText")!.Text =
                    $"{first}–{last} из {totalItems}";
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось загрузить данные:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }

        private async Task ConfirmExit()
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Выход", "Вы уверены, что хотите закрыть приложение?", ButtonEnum.YesNo);
            var result = await box.ShowWindowDialogAsync(this);
            if (result == ButtonResult.Yes)
                Environment.Exit(0);
        }
    }
}
