using DCUniverseLens.Logica;
using Microsoft.AspNetCore.Mvc;

namespace DCUniverseLens.Controllers
{
    public class PeliculaController : Controller
    {
        private readonly IPeliculaLogica _peliculaLogica;

        public PeliculaController(IPeliculaLogica peliculaLogica)
        {
            _peliculaLogica = peliculaLogica;
        }

        public async Task<IActionResult> Detalles(int id, int? idPersonaje)
        {
            var pelicula = await _peliculaLogica.ObtenerPeliculaPorIdAsync(id);
            if (pelicula == null)
                return NotFound();

            ViewBag.IdPersonaje = idPersonaje;
            return View(pelicula);
        }
    }
}
