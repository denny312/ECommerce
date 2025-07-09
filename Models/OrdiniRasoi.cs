using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class OrdiniRasoi
{
    public int Id { get; set; }

    public int OrdineId { get; set; }

    public int RasoioId { get; set; }

    public int Quantita { get; set; }

    public virtual Ordini Ordine { get; set; } = null!;

    public virtual Rasoi Rasoio { get; set; } = null!;
}
