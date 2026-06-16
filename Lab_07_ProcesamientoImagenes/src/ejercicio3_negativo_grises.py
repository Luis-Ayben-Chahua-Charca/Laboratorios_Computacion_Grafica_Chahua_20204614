import cv2
from pathlib import Path
import os

raiz = Path(__file__).resolve().parent.parent

# Imagen creada en el ejercicio 2
imagen = cv2.imread(
    str(raiz / "resultados" / "combinar_canales" / "imagen_combinada.png")
)

if imagen is None:
    raise FileNotFoundError(
        "No se encontró imagen_combinada.png"
    )


#crear direccion de salida
os.makedirs(str(raiz / "resultados" / "negativos"), exist_ok=True)


# Negativo

negativo = 255 - imagen

ruta_negativo = (
    raiz /
    "resultados" /
    "negativos" /
    "imagen_negativa.png"
)

cv2.imwrite(
    str(ruta_negativo),
    negativo
)

# Escala de grises

gris = cv2.cvtColor(
    negativo,
    cv2.COLOR_BGR2GRAY
)

ruta_gris = (
    raiz /
    "resultados" /
    "negativos" /
    "imagen_grises.png"
)

cv2.imwrite(
    str(ruta_gris),
    gris
)

print("Imagen negativa guardada.")
print("Imagen en escala de grises guardada.")