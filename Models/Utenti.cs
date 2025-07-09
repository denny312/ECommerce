using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Utenti
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Ordini> Ordinis { get; set; } = new List<Ordini>();

    public virtual ICollection<Recensioni> Recensionis { get; set; } = new List<Recensioni>();
}
