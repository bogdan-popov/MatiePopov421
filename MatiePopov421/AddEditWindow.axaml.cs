using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MatiePopov421.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MatiePopov421
{
    public partial class AddEditWindow : Window
    {
        private int serviceId;

        private static MatiedbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MatiedbContext>()
                .UseNpgsql(AppConfig.ConnectionString)
                .Options;
            return new MatiedbContext(options);
        }

        public AddEditWindow(int serviceId, int defaultCollectionId, int defaultTypeId)
        {
            InitializeComponent();
            this.serviceId = serviceId;

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var iconPath = Path.Combine(basePath, "Assets", "Logo.ico");
            if (File.Exists(iconPath))
                this.Icon = new WindowIcon(iconPath);

            var logoPath = Path.Combine(basePath, "Assets", "Logo.png");
            if (File.Exists(logoPath))
                this.FindControl<Image>("titleLogoAdd")!.Source = new Bitmap(logoPath);

            this.FindControl<Border>("titleBarAdd")!.PointerPressed += (_, e) => BeginMoveDrag(e);
            this.FindControl<Button>("cancelBtn")!.Click += (_, _) => Close();
            this.FindControl<Button>("cancelFormBtn")!.Click += (_, _) => Close();
            this.FindControl<Button>("saveBtn")!.Click += async (_, _) => await SaveService();
            this.FindControl<Button>("browseBtn")!.Click += async (_, _) => await BrowsePhoto();

            _ = InitAsync(defaultCollectionId, defaultTypeId);
        }

        private void UpdatePreview(string relativePath)
        {
            var image = this.FindControl<Image>("previewImage")!;
            var placeholder = this.FindControl<TextBlock>("previewPlaceholder")!;

            if (string.IsNullOrEmpty(relativePath))
            {
                image.Source = null;
                placeholder.IsVisible = true;
                return;
            }

            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", relativePath);
            if (File.Exists(fullPath))
            {
                image.Source = new Bitmap(fullPath);
                placeholder.IsVisible = false;
            }
            else
            {
                image.Source = null;
                placeholder.IsVisible = true;
            }
        }

        private async Task BrowsePhoto()
        {
            var typeCombo = this.FindControl<ComboBox>("typeCombo")!;
            string subFolder = "Custom";
            if (typeCombo.SelectedItem is TypeItem sel && sel.Id == 2)
                subFolder = "Cosplay";

            var options = new FilePickerOpenOptions
            {
                Title = "Выберите фотографию",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Изображения")
                    {
                        Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp" }
                    }
                }
            };

            var files = await StorageProvider.OpenFilePickerAsync(options);
            if (files == null || files.Count == 0) return;

            var sourcePath = files[0].TryGetLocalPath();
            if (string.IsNullOrEmpty(sourcePath)) return;

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var destDir = Path.Combine(basePath, "Assets", subFolder);
            Directory.CreateDirectory(destDir);

            var fileName = Path.GetFileName(sourcePath);
            var destPath = Path.Combine(destDir, fileName);

            if (File.Exists(destPath) && destPath != sourcePath)
            {
                var ext = Path.GetExtension(fileName);
                var nameNoExt = Path.GetFileNameWithoutExtension(fileName);
                fileName = $"{nameNoExt}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                destPath = Path.Combine(destDir, fileName);
            }

            File.Copy(sourcePath, destPath, overwrite: true);

            var relativePath = $"{subFolder}/{fileName}";
            this.FindControl<TextBox>("imagePathBox")!.Text = relativePath;
            UpdatePreview(relativePath);
        }

        private async Task InitAsync(int defaultCollectionId, int defaultTypeId)
        {
            await LoadCollections(defaultCollectionId);
            await LoadTypes(defaultTypeId);
            if (serviceId > 0)
            {
                this.FindControl<TextBlock>("windowTitleText")!.Text = "Редактировать услугу";
                await LoadServiceData();
            }
        }

        private async Task LoadCollections(int defaultCollectionId)
        {
            try
            {
                await using var ctx = CreateContext();
                var collections = await ctx.Collections.ToListAsync();

                var combo = this.FindControl<ComboBox>("collectionCombo")!;
                var items = new List<CollectionItem>();
                foreach (var c in collections)
                    items.Add(new CollectionItem { Id = c.Id, Name = c.Name });

                combo.ItemsSource = items;
                combo.SelectedIndex = items.FindIndex(c => c.Id == defaultCollectionId);
                if (combo.SelectedIndex < 0 && items.Count > 0)
                    combo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось загрузить коллекции:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }

        private async Task LoadTypes(int defaultTypeId)
        {
            try
            {
                await using var ctx = CreateContext();
                var types = await ctx.ServiceTypes.ToListAsync();

                var combo = this.FindControl<ComboBox>("typeCombo")!;
                var items = new List<TypeItem>();
                foreach (var t in types)
                    items.Add(new TypeItem { Id = t.Id, Name = t.Name });

                combo.ItemsSource = items;
                combo.SelectedIndex = items.FindIndex(t => t.Id == defaultTypeId);
                if (combo.SelectedIndex < 0 && items.Count > 0)
                    combo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось загрузить типы услуг:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }

        private async Task LoadServiceData()
        {
            try
            {
                await using var ctx = CreateContext();
                var service = await ctx.Services.FindAsync(serviceId);
                if (service != null)
                {
                    this.FindControl<TextBox>("titleBox")!.Text = service.Title;
                    this.FindControl<TextBox>("descriptionBox")!.Text = service.Description ?? "";
                    this.FindControl<TextBox>("imagePathBox")!.Text = service.Imagepath ?? "";
                    this.FindControl<TextBox>("priceBox")!.Text = service.Price.ToString("F2");
                    UpdatePreview(service.Imagepath ?? "");

                    var collCombo = this.FindControl<ComboBox>("collectionCombo")!;
                    if (collCombo.ItemsSource is List<CollectionItem> colList)
                        collCombo.SelectedIndex = colList.FindIndex(c => c.Id == service.Collectionid);

                    var typeCombo = this.FindControl<ComboBox>("typeCombo")!;
                    if (typeCombo.ItemsSource is List<TypeItem> typeList)
                        typeCombo.SelectedIndex = typeList.FindIndex(t => t.Id == service.Typeid);
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось загрузить услугу:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }

        private async Task SaveService()
        {
            string title = this.FindControl<TextBox>("titleBox")!.Text?.Trim() ?? "";
            string description = this.FindControl<TextBox>("descriptionBox")!.Text?.Trim() ?? "";
            string imagePath = this.FindControl<TextBox>("imagePathBox")!.Text?.Trim() ?? "";
            string priceText = this.FindControl<TextBox>("priceBox")!.Text?.Trim() ?? "0";
            var collCombo = this.FindControl<ComboBox>("collectionCombo")!;
            var typeCombo = this.FindControl<ComboBox>("typeCombo")!;

            if (string.IsNullOrEmpty(title))
            {
                var err = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Поле «Название» обязательно.", ButtonEnum.Ok);
                await err.ShowWindowDialogAsync(this);
                return;
            }

            if (!decimal.TryParse(priceText.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                var err = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Введите корректную цену (число ≥ 0).", ButtonEnum.Ok);
                await err.ShowWindowDialogAsync(this);
                return;
            }

            if (collCombo.SelectedItem is not CollectionItem sel)
            {
                var err = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Выберите коллекцию.", ButtonEnum.Ok);
                await err.ShowWindowDialogAsync(this);
                return;
            }

            if (typeCombo.SelectedItem is not TypeItem selType)
            {
                var err = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Выберите тип услуги.", ButtonEnum.Ok);
                await err.ShowWindowDialogAsync(this);
                return;
            }

            try
            {
                await using var ctx = CreateContext();

                if (serviceId == 0)
                {
                    ctx.Services.Add(new Service
                    {
                        Title = title,
                        Description = description,
                        Imagepath = imagePath,
                        Collectionid = sel.Id,
                        Typeid = selType.Id,
                        Price = price,
                        Lastmodifiedat = DateTime.Now
                    });
                }
                else
                {
                    var existing = await ctx.Services.FindAsync(serviceId);
                    if (existing != null)
                    {
                        existing.Title = title;
                        existing.Description = description;
                        existing.Imagepath = imagePath;
                        existing.Collectionid = sel.Id;
                        existing.Typeid = selType.Id;
                        existing.Price = price;
                        existing.Lastmodifiedat = DateTime.Now;
                    }
                }

                await ctx.SaveChangesAsync();
                Close();
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось сохранить:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }
    }

    public class CollectionItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public override string ToString() => Name;
    }

    public class TypeItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public override string ToString() => Name;
    }
}
