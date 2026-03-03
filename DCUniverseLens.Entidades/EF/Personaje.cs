using System;
using System.Collections.Generic;

namespace DCUniverseLens.Entidades.EF;

public partial class Personaje
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Apodo { get; set; }

    public string Poderes { get; set; } = null!;

    public int? IdActor { get; set; }

    public virtual Actor? IdActorNavigation { get; set; }

    public virtual ICollection<Pelicula> IdPeliculas { get; set; } = new List<Pelicula>();
}
