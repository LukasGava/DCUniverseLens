using DCUniverseLens.Entidades.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCUniverseLens.Logica
{

    public interface IPeliculaLogica
    {
        Task<Pelicula> ObtenerPeliculaPorIdAsync(int id);
    }

    public class PeliculaLogica : IPeliculaLogica
    {
        public async Task<Pelicula> ObtenerPeliculaPorIdAsync(int id)
        {
            using var context = new Pw3DcuniverseLensContext();

            return await context.Peliculas
                .Include(p => p.IdPersonajes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
