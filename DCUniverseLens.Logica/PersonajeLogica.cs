using DCUniverseLens.Entidades;
using DCUniverseLens.Entidades.EF;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MLModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCUniverseLens.Logica
{
    public interface IPersonajeLogica
    {
        Task<List<Personaje>> ObtenerPersonajesAsync();
        Task<Personaje> ObtenerPersonajePorIdAsync(int id);
        List<Rectangle> DetectFaces(byte[] imageData);
        byte[] ExtractFace(byte[] originalImage, Rectangle faceRect, int padding = 20);
        Task ProcessFacesAsync(List<Rectangle> faces, byte[] imageBytes, float CONFIDENCE_THRESHOLD, List<ActorIdentificationResult> results);
    }

    public class PersonajeLogica : IPersonajeLogica
    {
        private readonly CascadeClassifier _faceCascade;
        private readonly ILogger<PersonajeLogica> _logger;
        private readonly Pw3DcuniverseLensContext _context;

        public PersonajeLogica(ILogger<PersonajeLogica> logger, Pw3DcuniverseLensContext _context)
        {
            _logger = logger;
            this._context = _context;

            // Ruta al clasificador Haar Cascade para detección de caras
            string haarCascadePath = Path.Combine(AppContext.BaseDirectory, "Models", "haarcascade_frontalface_default.xml");

            // Descargar el clasificador si no existe
            if (!File.Exists(haarCascadePath))
            {
                _logger.LogInformation("Descargando clasificador Haar Cascade para detección de caras...");
                Directory.CreateDirectory(Path.GetDirectoryName(haarCascadePath));
                DownloadHaarCascade(haarCascadePath).Wait();
            }

            _faceCascade = new CascadeClassifier(haarCascadePath);
            _logger.LogInformation("Servicio de detección de caras inicializado correctamente");
        }

        public async Task<List<Personaje>> ObtenerPersonajesAsync()
        {
            return await _context.Personajes
                .Include(p => p.IdActorNavigation)
                .ToListAsync();
        }

        public async Task<Personaje> ObtenerPersonajePorIdAsync(int id)
        {
            return await _context.Personajes
                .Include(p => p.IdActorNavigation)
                .Include(p => p.IdPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        private async Task DownloadHaarCascade(string filePath)
        {
            string url = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_default.xml";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            _logger.LogInformation("Clasificador Haar Cascade descargado correctamente");
        }

        public List<Rectangle> DetectFaces(byte[] imageData)
        {
            try
            {
                // Cargar la imagen como Mat (formato nativo de OpenCV)
                Mat mat = new Mat();
                CvInvoke.Imdecode(imageData, ImreadModes.Color, mat);

                // Convertir a escala de grises
                Mat grayMat = new Mat();
                CvInvoke.CvtColor(mat, grayMat, ColorConversion.Bgr2Gray);

                // Detectar caras
                Rectangle[] faces = _faceCascade.DetectMultiScale(
                    grayMat,
                    1.1, // scaleFactor
                    8,   // minNeighbors
                    new Size(30, 30)); // minSize

                // Liberar recursos
                mat.Dispose();
                grayMat.Dispose();

                var result = faces.ToList();
                _logger.LogInformation($"Se detectaron {result.Count} caras en la imagen");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detectar caras en la imagen");
                throw;
            }
        }

        public byte[] ExtractFace(byte[] originalImage, Rectangle faceRect, int padding = 20)
        {
            try
            {
                using var memoryStream = new MemoryStream(originalImage);
                using var bitmap = new Bitmap(memoryStream);

                // Padding
                var x = Math.Max(0, faceRect.X - padding);
                var y = Math.Max(0, faceRect.Y - padding);
                var width = Math.Min(bitmap.Width - x, faceRect.Width + padding * 2);
                var height = Math.Min(bitmap.Height - y, faceRect.Height + padding * 2);

                var paddedRect = new Rectangle(x, y, width, height);

                // Recortar
                using var faceBitmap = bitmap.Clone(paddedRect, bitmap.PixelFormat);

                // Redimensionar al tamaño fijo del modelo (ejemplo: 224x224)
                int targetWidth = 224;
                int targetHeight = 224;
                using var resizedBitmap = new Bitmap(targetWidth, targetHeight);
                using var graphics = Graphics.FromImage(resizedBitmap);
                graphics.DrawImage(faceBitmap, 0, 0, targetWidth, targetHeight);

                // Convertir a byte[]
                using var faceStream = new MemoryStream();
                resizedBitmap.Save(faceStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return faceStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al extraer y redimensionar la cara");
                throw;
            }
        }

        public async Task ProcessFacesAsync(List<Rectangle> faces, byte[] imageBytes, float CONFIDENCE_THRESHOLD, List<ActorIdentificationResult> results)
        {
            foreach (var faceRect in faces)
            {
                try
                {
                    // Extraer la cara de la imagen original
                    byte[] faceImageBytes = ExtractFace(imageBytes, faceRect);

                    //  entrada para el modelo
                    var input = new MLModel1.ModelInput
                    {
                        ImageSource = faceImageBytes,
                        Label = string.Empty
                    };

                    //  predicci?n
                    var prediction = MLModel1.Predict(input);

                    //  mejores predicciones
                    var allPredictions = MLModel1.PredictAllLabels(input)
                        .Take(3)
                        .ToDictionary(kv => kv.Key, kv => kv.Value);

                    // Verificar si la confianza est? por encima del umbral
                    float maxConfidence = prediction.Score.Max();
                    string actorName = prediction.PredictedLabel;

                    _logger.LogInformation("Antes de obtener personajes");
                    var personajes = await ObtenerPersonajesAsync();
                    _logger.LogInformation("Después de obtener personajes");
                    var personajesDict = personajes.ToDictionary(p => p.Id.ToString(), p => p.Apodo);

                    string apodo = personajesDict.ContainsKey(actorName) ? personajesDict[actorName] : string.Empty;


                    // Crear el resultado para esta cara
                    var result = new ActorIdentificationResult
                    {
                        ActorName = actorName,
                        Confidence = maxConfidence,
                        TopPredictions = allPredictions,
                        ImageData = Convert.ToBase64String(faceImageBytes),
                        FaceRectangle = new FaceRectangle
                        {
                            X = faceRect.X,
                            Y = faceRect.Y,
                            Width = faceRect.Width,
                            Height = faceRect.Height
                        },
                        IsRecognized = true,
                        PersonajesNombres = personajesDict,
                        Apodo = apodo

                    };

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al procesar la cara en posici?n {faceRect}");

                }
            }
        }

    }
}
