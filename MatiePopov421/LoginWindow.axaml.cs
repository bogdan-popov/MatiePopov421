using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using MatiePopov421.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MatiePopov421
{
    public partial class LoginWindow : Window
    {
        private static MatiedbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MatiedbContext>()
                .UseNpgsql(AppConfig.ConnectionString)
                .Options;
            return new MatiedbContext(options);
        }

        public LoginWindow()
        {
            InitializeComponent();

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

            this.FindControl<Border>("titleBar")!.PointerPressed += (_, e) => BeginMoveDrag(e);
            this.FindControl<Button>("closeBtnLogin")!.Click += async (_, _) => await ConfirmExit();
            this.FindControl<Button>("showLoginBtn")!.Click += (_, _) => ShowLogin();
            this.FindControl<Button>("showRegisterBtn")!.Click += (_, _) => ShowRegister();
            this.FindControl<Button>("loginBtn")!.Click += async (_, _) => await DoLogin();
            this.FindControl<Button>("registerBtn")!.Click += async (_, _) => await DoRegister();
        }

        private void ShowLogin()
        {
            this.FindControl<StackPanel>("loginPanel")!.IsVisible = true;
            this.FindControl<StackPanel>("registerPanel")!.IsVisible = false;
            this.FindControl<Button>("showLoginBtn")!.Background =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#7C3AED"));
            this.FindControl<Button>("showLoginBtn")!.Foreground =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);
            this.FindControl<Button>("showRegisterBtn")!.Background =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#EDE9FE"));
            this.FindControl<Button>("showRegisterBtn")!.Foreground =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#5B21B6"));
        }

        private void ShowRegister()
        {
            this.FindControl<StackPanel>("loginPanel")!.IsVisible = false;
            this.FindControl<StackPanel>("registerPanel")!.IsVisible = true;
            this.FindControl<Button>("showRegisterBtn")!.Background =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#7C3AED"));
            this.FindControl<Button>("showRegisterBtn")!.Foreground =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);
            this.FindControl<Button>("showLoginBtn")!.Background =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#EDE9FE"));
            this.FindControl<Button>("showLoginBtn")!.Foreground =
                new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#5B21B6"));
        }

        private async Task DoLogin()
        {
            string username = this.FindControl<TextBox>("loginUsernameBox")!.Text?.Trim() ?? "";
            string password = this.FindControl<TextBox>("loginPasswordBox")!.Text ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var warn = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Введите логин и пароль.", ButtonEnum.Ok);
                await warn.ShowWindowDialogAsync(this);
                return;
            }

            try
            {
                string hash = AppConfig.HashPassword(password);
                await using var ctx = CreateContext();
                var user = await ctx.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && u.Passwordhash == hash);

                if (user == null)
                {
                    var err = MessageBoxManager.GetMessageBoxStandard(
                        "Ошибка", "Неверный логин или пароль.", ButtonEnum.Ok);
                    await err.ShowWindowDialogAsync(this);
                    return;
                }

                AppConfig.CurrentUser = user;
                OpenMainWindow();
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Ошибка подключения:\n{ex.Message}", ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(this);
            }
        }

        private async Task DoRegister()
        {
            string username = this.FindControl<TextBox>("regUsernameBox")!.Text?.Trim() ?? "";
            string fullName = this.FindControl<TextBox>("regFullNameBox")!.Text?.Trim() ?? "";
            string password = this.FindControl<TextBox>("regPasswordBox")!.Text ?? "";
            string confirm = this.FindControl<TextBox>("regConfirmBox")!.Text ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var warn = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Заполните логин и пароль.", ButtonEnum.Ok);
                await warn.ShowWindowDialogAsync(this);
                return;
            }

            if (password != confirm)
            {
                var warn = MessageBoxManager.GetMessageBoxStandard(
                    "Внимание", "Пароли не совпадают.", ButtonEnum.Ok);
                await warn.ShowWindowDialogAsync(this);
                return;
            }

            try
            {
                await using var ctx = CreateContext();

                if (await ctx.Users.AnyAsync(u => u.Username == username))
                {
                    var warn = MessageBoxManager.GetMessageBoxStandard(
                        "Внимание", "Пользователь с таким логином уже существует.", ButtonEnum.Ok);
                    await warn.ShowWindowDialogAsync(this);
                    return;
                }

                var userRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
                if (userRole == null)
                {
                    var err = MessageBoxManager.GetMessageBoxStandard(
                        "Ошибка", "Роль «Пользователь» не найдена.", ButtonEnum.Ok);
                    await err.ShowWindowDialogAsync(this);
                    return;
                }

                ctx.Users.Add(new User
                {
                    Username = username,
                    Fullname = string.IsNullOrEmpty(fullName) ? username : fullName,
                    Passwordhash = AppConfig.HashPassword(password),
                    Roleid = userRole.Id,
                    Createdat = DateTime.Now
                });
                await ctx.SaveChangesAsync();

                var ok = MessageBoxManager.GetMessageBoxStandard(
                    "Готово", "Аккаунт создан. Войдите с новыми данными.", ButtonEnum.Ok);
                await ok.ShowWindowDialogAsync(this);

                this.FindControl<TextBox>("regUsernameBox")!.Text = "";
                this.FindControl<TextBox>("regFullNameBox")!.Text = "";
                this.FindControl<TextBox>("regPasswordBox")!.Text = "";
                this.FindControl<TextBox>("regConfirmBox")!.Text = "";
                ShowLogin();
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", $"Не удалось зарегистрировать:\n{ex.Message}", ButtonEnum.Ok);
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

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            if (Avalonia.Application.Current?.ApplicationLifetime
                is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = mainWindow;
            mainWindow.Show();
            Close();
        }
    }
}
