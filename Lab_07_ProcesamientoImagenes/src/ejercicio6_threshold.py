import cv2
from pathlib import Path
import os 

raiz = Path(__file__).resolve().parent.parent

# Cargar imagen en escala de grises
imagen = cv2.imread(
    #str(raiz / "resultados" /"negativos"/ "imagen_grises.png"),
    str(raiz / "imagenes" /"gato.jpg"),
    cv2.IMREAD_GRAYSCALE
)

if imagen is None:
    raise FileNotFoundError(
        "No se encontró imagen_grises.png"
    )

os.makedirs(str(raiz / "resultados" / "threshold"), exist_ok=True)

# Aplicar threshold binario
umbral = 127

_, imagen_binaria = cv2.threshold(
    imagen,
    umbral,
    255,
    cv2.THRESH_BINARY
)

# Guardar resultado
ruta_salida = (
    raiz /
    "resultados" /
    "threshold" /
    "imagen_threshold.png"
)

cv2.imwrite(
    str(ruta_salida),
    imagen_binaria
)

print(f"Imagen guardada en: {ruta_salida}")