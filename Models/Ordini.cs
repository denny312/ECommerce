using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Ordini
{
    public int Id { get; set; }

    public int UtenteId { get; set; }

    public DateTime? Data { get; set; }

    public virtual ICollection<OrdiniRasoi> OrdiniRasois { get; set; } = new List<OrdiniRasoi>();

    public virtual Utenti Utente { get; set; } = null!;
}
