using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Recensioni
{
    public int Id { get; set; }

    public int UtenteId { get; set; }

    public int RasoioId { get; set; }

    public sbyte Voto { get; set; }

    public string? Commento { get; set; }

    public virtual Rasoi Rasoio { get; set; } = null!;

    public virtual Utenti Utente { get; set; } = null!;
}
