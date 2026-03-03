using System;
using System.Collections.Generic;

namespace DCUniverseLens.Entidades.EF;

public partial class Pelicula
{
    public int Id { get; set; }

    public string? Descripcion { get; set; }

    public string Nombre { get; set; } = null!;

    public string Ano { get; set; } = null!;

    public virtual ICollection<Personaje> IdPersonajes { get; set; } = new List<Personaje>();
}
