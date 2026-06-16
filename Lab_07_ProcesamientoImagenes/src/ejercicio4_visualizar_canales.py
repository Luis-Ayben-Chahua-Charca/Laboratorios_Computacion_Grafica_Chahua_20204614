import cv2
from pathlib import Path

raiz = Path(__file__).resolve().parent.parent

imagen_original = cv2.imread(
    str(raiz / "imagenes" / "gato.jpg")
)

if imagen_original is None:
    raise FileNotFoundError(
        "No se encontró imagen_combinada.png"
    )

mostrar_rojo = True
mostrar_verde = True
mostrar_azul = True

while True:

    imagen = imagen_original.copy()

    if not mostrar_azul:
        imagen[:, :, 0] = 0

    if not mostrar_verde:
        imagen[:, :, 1] = 0

    if not mostrar_rojo:
        imagen[:, :, 2] = 0

    cv2.imshow(
        "Visualizador de Canales RGB",
        imagen
    )

    tecla = cv2.waitKey(1) & 0xFF

    if tecla == ord('r'):
        mostrar_rojo = not mostrar_rojo

    elif tecla == ord('g'):
        mostrar_verde = not mostrar_verde

    elif tecla == ord('b'):
        mostrar_azul = not mostrar_azul

    elif tecla == 27:  # ESC
        break

cv2.destroyAllWindows()