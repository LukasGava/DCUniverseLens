using DCUniverseLens.Entidades;
using DCUniverseLens.Entidades.EF;
using DCUniverseLens.Logica;
using DCUniverseLens.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PW3_DCUniverseLens.Controllers
{
    public class PersonajeController : Controller
    {
        private readonly ILogger<PersonajeController> _logger;
        private readonly IPersonajeLogica _personajeLogica;


        public PersonajeController(ILogger<PersonajeController> logger, IPersonajeLogica personajeLogica)
        {
            _logger = logger;
            _personajeLogica = personajeLogica;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult UploadMultiple()
        {
            return View();
        }     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> IdentifyMultipleFaces(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Message"] = "Debe seleccionar una imagen.";
                return RedirectToAction("UploadMultiple");
            }

            if (!imageFile.ContentType.StartsWith("image/"))
            {
                TempData["Message"] = "El archivo debe ser una imagen.";
                return RedirectToAction("UploadMultiple");
            }

            try
            {
                // Leer la imagen 
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                // Detectar caras en la imagen
                var faces = _personajeLogica.DetectFaces(imageBytes);

                if (faces.Count == 0)
                {
                    TempData["Message"] = "No se detectaron caras en la imagen. Intenta con otra imagen.";
                    return RedirectToAction("UploadMultiple");
                }

                // Lista para almacenar los resultados de cada cara
                var results = new List<ActorIdentificationResult>();

                // Porcentaje de confianza m?nimo 
                const float CONFIDENCE_THRESHOLD = 0.60f;

                // Procesar cada cara detectada
                await _personajeLogica.ProcessFacesAsync(faces, imageBytes, CONFIDENCE_THRESHOLD, results);

                return View("MultipleResults", new MultipleActorsResult
                {
                    OriginalImageData = Convert.ToBase64String(imageBytes),
                    DetectedActors = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la imagen");
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    //ErrorMessage = "Error al procesar la imagen: " + ex.Message
                });
            }
        }

        public async Task<IActionResult> VerPersonajes()
        {
            List<Personaje> personajes = await _personajeLogica.ObtenerPersonajesAsync(); 

            return View(personajes);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Personaje personaje = await _personajeLogica.ObtenerPersonajePorIdAsync(id);

            if (personaje == null)
            {
                return NotFound();
            }

            return View(personaje);
        }

    }
}
