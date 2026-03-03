using System;
using System.Collections.Generic;

namespace DCUniverseLens.Entidades.EF;

public partial class Actor
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaNacimiento { get; set; }

    public string PaisOrigen { get; set; } = null!;

    public virtual ICollection<Personaje> Personajes { get; set; } = new List<Personaje>();
}
