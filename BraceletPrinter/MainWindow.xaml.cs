using BraceletPrinter.Domain;
using BraceletPrinter.Services;
using System.Windows;

namespace BraceletPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UserService _userService;

        public MainWindow()
        {
            InitializeComponent();
            
            // Inicializar el servicio
            var apiSettings = new ApiSettings();
            _userService = new UserService(apiSettings);
            
            // Ejecutar la verificación de usuario al inicio
            Loaded += async (s, e) => await InitializeApplicationAsync();
        }

        private async Task InitializeApplicationAsync()
        {
            try
            {
                bool usersExist = await _userService.CheckIfUsersExistAsync();
                
                if (!usersExist)
                {
                    var registroWindow = new RegistroAdminWindow(_userService);
                    if (registroWindow.ShowDialog() == true)
                    {
                        MessageBox.Show("Usuario administrador creado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la aplicación: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}