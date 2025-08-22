using QRCoder;

namespace PresentacionMAUI
{
    public partial class MainPage : ContentPage
    {

        // Datos de contacto - Personaliza estos valores
        private readonly string phoneNumber = "773137656";
        private readonly string whatsappNumber = "7731795187";
        private readonly string address = "Av. Reforma 123, CDMX, México";
        private readonly string website = "https://pixabay.com/es/images/search/dise%C3%B1o%20web/";
        private readonly string facebookUrl = "https://www.facebook.com/profile.php?id=61578821236039";
        private readonly string instagramUrl = "https://www.instagram.com/lomanconsultoria2025/profilecard/?igsh=aGhrbnN3MGY1bnNh";

        public MainPage()
        {
            InitializeComponent();
            LoadCardData();
            GenerateQRCode();
        }

        private void LoadCardData()
        {
            // Aquí puedes cargar los datos desde una base de datos, API, etc.
            NameLabel.Text = "Consultoría Integral SC";
            PositionLabel.Text = "COMPANY";
        }

        private async void OnPhoneClicked(object sender, EventArgs e)
        {
            try
            {
                // Método compatible con versiones anteriores
                PhoneDialer.Open(phoneNumber);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el marcador: {ex.Message}", "OK");
            }
        }

        private async void OnWhatsAppClicked(object sender, EventArgs e)
        {
            try
            {
                var message = Uri.EscapeDataString("¡Hola! Quisiera conocer más acerca de los servicios que ofrece su consultoria.");
                var whatsappUrl = $"https://wa.me/{whatsappNumber.Replace("+", "")}?text={message}";

                await Browser.OpenAsync(whatsappUrl, BrowserLaunchMode.External);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir WhatsApp: {ex.Message}", "OK");
            }
        }

        private async void OnVisitClicked(object sender, EventArgs e)
        {
            try
            {
                // Usa tu enlace específico de Google Maps
                var mapsUrl = "https://maps.app.goo.gl/UFrmh8ur6MkHkGKP9";
                await Browser.OpenAsync(mapsUrl, BrowserLaunchMode.External);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el mapa: {ex.Message}", "OK");
            }
        }

        private async void OnFacebookClicked(object sender, EventArgs e)
        {
            await OpenUrl(facebookUrl);
        }

        private async void OnInstagramClicked(object sender, EventArgs e)
        {
            await OpenUrl(instagramUrl);
        }


        private async void OnShareClicked(object sender, EventArgs e)
        {
            try
            {
                var shareText = $"Conoce a {NameLabel.Text} - {PositionLabel.Text}\n{website}";

                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = "Tarjeta Digital"
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo compartir: {ex.Message}", "OK");
            }
        }

        private async Task OpenUrl(string url)
        {
            try
            {
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el enlace: {ex.Message}", "OK");
            }
        }

        private void GenerateQRCode()
        {
            try
            {
                // Generar vCard con información completa
                var vCard = GenerateVCard();

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(vCard, QRCodeGenerator.ECCLevel.Q);

                // Generar imagen PNG del QR
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20); // 20 pixels por módulo

                // Convertir a ImageSource y asignar a la imagen
                var stream = new MemoryStream(qrCodeBytes);
                QRImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
            }
            catch (Exception ex)
            {
                // Si hay error, mantener imagen placeholder
                System.Diagnostics.Debug.WriteLine($"Error generando QR: {ex.Message}");
            }
        }

        private string GenerateVCard()
        {
            return $@"BEGIN:VCARD
                    VERSION:3.0
                    FN:{NameLabel.Text}
                    ORG:{NameLabel.Text}
                    TITLE:{PositionLabel.Text}
                    TEL;TYPE=WORK,VOICE:{phoneNumber}
                    TEL;TYPE=CELL:{whatsappNumber}
                    URL:{website}
                    ADR;TYPE=WORK:;;{address};;;
                    NOTE:Contacto desde tarjeta digital
                    END:VCARD";
                            }

        // Método alternativo para generar QR con solo la URL del sitio web
        private string GenerateSimpleUrl()
        {
            return website;
        }

        // Método para guardar contacto en el dispositivo
        private async void OnSaveContactClicked(object sender, EventArgs e)
        {
            try
            {
                // Crear vCard y compartir como archivo
                var vCard = GenerateVCard();
                var fileName = $"{NameLabel.Text.Replace(" ", "_")}_contact.vcf";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                await File.WriteAllTextAsync(filePath, vCard);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Guardar Contacto",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo crear el archivo de contacto: {ex.Message}", "OK");
            }
        }

        // Método adicional para cambiar el tipo de QR en tiempo real (opcional)
        private async void OnQRImageTapped(object sender, EventArgs e)
        {
            try
            {
                // Por defecto generar QR del sitio web
                string websiteUrl = GenerateSimpleUrl();
                GenerateQRWithCustomData(websiteUrl);

                // Opcionalmente mostrar mensaje confirmando la acción
                await DisplayAlert("QR Generado", "Código QR del sitio web generado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al generar QR del sitio web: {ex.Message}", "OK");
            }
        }

        private void GenerateQRWithCustomData(string data)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                var stream = new MemoryStream(qrCodeBytes);
                QRImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando QR personalizado: {ex.Message}");
            }
        }
    }

    // Clase helper para el contacto
    public class Contact
    {
        public string Name { get; set; }
        public List<ContactPhone> Phones { get; set; } = new List<ContactPhone>();
        public List<ContactWebsite> Websites { get; set; } = new List<ContactWebsite>();
    }

    public class ContactPhone
    {
        public string Number { get; set; }
        public ContactPhoneKind Type { get; set; }
    }

    public class ContactWebsite
    {
        public string Website { get; set; }
    }

    public enum ContactPhoneKind
    {
        Work,
        Mobile,
        Home
    }
}