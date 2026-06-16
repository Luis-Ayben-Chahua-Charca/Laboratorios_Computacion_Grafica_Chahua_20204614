import cv2
import os
from pathlib import Path

raiz = Path(__file__).resolve().parent.parent

# Cargar imágenes
img1 = cv2.imread(str(raiz / "resultados" / "redimensionadas" / "img_1.jpg"))
img2 = cv2.imread(str(raiz / "resultados" / "redimensionadas" / "img_2.jpg"))
img3 = cv2.imread(str(raiz / "resultados" / "redimensionadas" / "img_3.jpg"))

# Verificación
if img1 is None or img2 is None or img3 is None:
    raise FileNotFoundError("No se pudieron cargar una o más imágenes")

# Extraer canales
rojo = img1[:, :, 2]
verde = img2[:, :, 1]
azul = img3[:, :, 0]

print("Rojo :", rojo.shape)
print("Verde:", verde.shape)
print("Azul :", azul.shape)

# Combinar canales
imagen_combinada = cv2.merge([azul, verde, rojo])

#crear direccion de salida
os.makedirs(str(raiz / "resultados" / "combinar_canales"), exist_ok=True)

# Guardar resultado
ruta_salida = raiz / "resultados" / "combinar_canales" / "imagen_combinada.png"

cv2.imwrite(
    str(ruta_salida),
    imagen_combinada
)

print(f"Imagen guardada en: {ruta_salida}")