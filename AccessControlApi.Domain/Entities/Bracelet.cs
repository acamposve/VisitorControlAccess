namespace AccessControlApi.Domain.Entities;

public class Bracelet
{
    public int Id { get; set; }
    public int VisitorId { get; set; }

    // Relación con AccessPoints
    public ICollection<AccessPoint> AccessPoints { get; set; } = new List<AccessPoint>();

    public DateTime CreatedAt { get; set; }

    // Propiedad para almacenar el contenido del QR generado externamente
    public string QrCode { get; set; } = string.Empty;
}