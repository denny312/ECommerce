using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Rasoi
{
    public int Id { get; set; }

    public string Marca { get; set; } = null!;

    public string Modello { get; set; } = null!;

    public decimal Prezzo { get; set; }

    public string Tipo { get; set; } = null!;

    public string? ImageKey { get; set; }
    public virtual ICollection<OrdiniRasoi> OrdiniRasois { get; set; } = new List<OrdiniRasoi>();

    public virtual ICollection<Recensioni> Recensionis { get; set; } = new List<Recensioni>();
}
