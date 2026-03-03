# DCUniverseLens

Aplicación ASP.NET Core que permite detectar rostros mediante Emgu CV (OpenCV) e identificar personajes utilizando un modelo de clasificación de imágenes entrenado con ML.NET. La persistencia de datos se maneja con Entity Framework Core y SQL Server.
Requisitos
Visual Studio 2022
.NET 8 (o versión correspondiente del proyecto)
SQL Server / SQL Express
Windows (requerido por Emgu.CV.runtime.windows)

Paquetes principales:
Emgu.CV
Emgu.CV.runtime.windows
Microsoft.ML
Microsoft.ML.Vision
System.Drawing.Common
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
Microsoft.EntityFrameworkCore.SqlServer

Base de datos:
Antes de ejecutar el proyecto, crear la base de datos con las tablas correspondientes.
Comando para regenerar el scaffolding de Entity Framework:
Scaffold-DbContext "Server=.\SQLEXPRESS;Database=PW3-DCUniverseLens;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir EF -Force
Ajustar el nombre del servidor según el entorno local.

Entrenamiento del modelo
El modelo se entrena con ML.NET Model Builder (clasificación multiclase).
Estructura esperada del dataset:
TrainingData/
 ├── 1/
 ├── 2/
 ├── 3/
 ├── ...
 ├── 20/
Cada carpeta representa el Id del actor en la base de datos.

Para reentrenar:
Colocar la carpeta TrainingData en la raíz del proyecto.
Abrir MLModel1.mbconfig.
Ejecutar el entrenamiento.
Verificar que se genere MLModel1.mlnet.

Configurar el archivo .mlnet con:
Copy to Output Directory → Copy if newer.
Flujo de reconocimiento
El usuario carga una imagen.
Emgu detecta rostros mediante Haar Cascade.
Se recorta la cara y se redimensiona a 224x224.
ML.NET clasifica la imagen.
Se muestra el personaje identificado junto con el nivel de confianza y las principales coincidencias.

Notas:
El dataset completo está en: https://drive.google.com/drive/folders/1bgrmepCxt7V897tKDGwBqm4KZv18eG1g?usp=drive_link.
Usar private static string MLNetModelPath = Path.Combine(AppContext.BaseDirectory, "MLModel1.mlnet"); en la ubicacion del pack de imagenes.
