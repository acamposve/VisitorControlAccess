using BraceletPrinter.Domain;
using System.Windows;
using System.Text.RegularExpressions;
using BraceletPrinter.Services;

namespace BraceletPrinter
{
    public partial class RegistroAdminWindow : Window
    {
        private readonly UserService _userService;

        public AdminUser AdminUser { get; private set; }

        public RegistroAdminWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private async void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarFormulario())
            {
                try
                {
                    AdminUser = new AdminUser
                    {
                        FullName = TxtFullName.Text.Trim(),
                        Username = TxtUsername.Text.Trim(),
                        Email = TxtEmail.Text.Trim(),
                        Password = TxtPassword.Password
                    };

                    await _userService.CreateAdminUserAsync(AdminUser);
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al registrar el usuario: {ex.Message}", 
                                  "Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MostrarError("El nombre completo es requerido.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtUsername.Text))
            {
                MostrarError("El nombre de usuario es requerido.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text) || !IsValidEmail(TxtEmail.Text))
            {
                MostrarError("Por favor, ingrese un correo electrónico válido.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                MostrarError("La contraseña es requerida.");
                return false;
            }

            if (TxtPassword.Password != TxtConfirmPassword.Password)
            {
                MostrarError("Las contraseñas no coinciden.");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error de Validación",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}