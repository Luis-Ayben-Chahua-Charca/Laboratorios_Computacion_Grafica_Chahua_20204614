import cv2
import os
from pathlib import Path

# Obtiene la ruta de la carpeta 'src'
raiz = Path(__file__).resolve().parent.parent

# Cargar imágenes
img1 = cv2.imread(str(raiz / "imagenes" / "perro.jpg"))
img2 = cv2.imread(str(raiz / "imagenes" / "gato.jpg"))
img3 = cv2.imread(str(raiz / "imagenes" / "persona.jpg"))

imagenes = [img1, img2, img3]

# Encontrar dimensiones máximas
max_alto = 0
max_ancho = 0

for img in imagenes:
    alto, ancho = img.shape[:2]

    if alto > max_alto:
        max_alto = alto

    if ancho > max_ancho:
        max_ancho = ancho

print(f"Dimensiones objetivo: {max_ancho}x{max_alto}")

# Crear carpeta si no existe
os.makedirs(str(raiz / "resultados" / "redimensionadas"), exist_ok=True)

# Redimensionar y guardar
for i, img in enumerate(imagenes, start=1):

    nueva = cv2.resize(
        img,
        (max_ancho, max_alto)
    )

    cv2.imwrite(
        str(raiz / "resultados" / "redimensionadas" / f"img_{i}.jpg"),
        nueva
    )

print("Proceso completado.")