import cv2
from pathlib import Path
import os

raiz = Path(__file__).resolve().parent.parent

imagen = cv2.imread(
    str(raiz / "imagenes" / "persona.jpg")
)

if imagen is None:
    raise FileNotFoundError(
        "No se pudo cargar la imagen"
    )

# Coordenadas de la cara
# Ajustar según la imagen utilizada


centro = (375, 140)
radio = 90

# Dibujar círculo
cv2.circle(
    imagen,
    centro,
    radio,
    (0, 255, 0),
    3
)

# Agregar texto
cv2.putText(
    imagen,
    "Persona",
    (250, 320),
    cv2.FONT_HERSHEY_SIMPLEX,
    1,
    (0, 255, 0),
    2
)

os.makedirs(str(raiz / "resultados" / "circuloNombre"), exist_ok=True)

ruta_salida = (
    raiz /
    "resultados" /
    "circuloNombre" /
    "persona_etiquetada.png"
)

cv2.imwrite(
    str(ruta_salida),
    imagen
)

print("Imagen guardada correctamente.")