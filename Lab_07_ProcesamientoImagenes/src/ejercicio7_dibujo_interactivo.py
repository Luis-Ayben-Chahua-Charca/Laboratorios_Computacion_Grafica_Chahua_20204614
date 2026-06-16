import cv2
import numpy as np
from pathlib import Path
from tkinter import Tk
from tkinter.filedialog import asksaveasfilename

# ==================================================
# CONFIGURACION
# ==================================================

raiz = Path(__file__).resolve().parent.parent

ANCHO = 1000
ALTO = 700
ALTURA_MENU = 60

canvas = np.ones((ALTO, ANCHO, 3), dtype=np.uint8) * 255

historial = [canvas.copy()]

modo = "circulo"

# ==================================================
# BOTONES
# ==================================================

botones = {
    "circulo": (10, 10, 140, 50),
    "rectangulo": (160, 10, 320, 50),
    "linea": (340, 10, 450, 50),
    "limpiar": (470, 10, 580, 50),
}


# ==================================================
# DIBUJAR INTERFAZ
# ==================================================

def dibujar_interfaz(img):

    cv2.rectangle(
        img,
        (0, 0),
        (ANCHO, ALTURA_MENU),
        (220, 220, 220),
        -1
    )

    for nombre, (x1, y1, x2, y2) in botones.items():

        cv2.rectangle(
            img,
            (x1, y1),
            (x2, y2),
            (50, 50, 50),
            2
        )

        cv2.putText(
            img,
            nombre.capitalize(),
            (x1 + 10, y1 + 28),
            cv2.FONT_HERSHEY_SIMPLEX,
            0.6,
            (0, 0, 0),
            2
        )

    cv2.putText(
        img,
        f"Modo actual: {modo}",
        (650, 35),
        cv2.FONT_HERSHEY_SIMPLEX,
        0.7,
        (0, 0, 255),
        2
    )


# ==================================================
# DETECTAR BOTON
# ==================================================

def boton_presionado(x, y):

    for nombre, (x1, y1, x2, y2) in botones.items():

        if x1 <= x <= x2 and y1 <= y <= y2:
            return nombre

    return None


# ==================================================
# MOUSE
# ==================================================

def mouse_callback(event, x, y, flags, param):

    global modo
    global canvas
    global historial

    if event != cv2.EVENT_LBUTTONDOWN:
        return

    # ---------------------------------
    # BOTONES
    # ---------------------------------

    boton = boton_presionado(x, y)

    if boton is not None:

        if boton == "limpiar":

            historial.append(canvas.copy())

            canvas[:] = 255

        else:

            modo = boton

        return

    # ---------------------------------
    # AREA DE DIBUJO
    # ---------------------------------

    if y <= ALTURA_MENU:
        return

    historial.append(canvas.copy())

    if modo == "circulo":

        cv2.circle(
            canvas,
            (x, y),
            40,
            (0, 0, 255),
            3
        )

    elif modo == "rectangulo":

        cv2.rectangle(
            canvas,
            (x - 50, y - 30),
            (x + 50, y + 30),
            (255, 0, 0),
            3
        )

    elif modo == "linea":

        cv2.line(
            canvas,
            (x - 50, y - 50),
            (x + 50, y + 50),
            (0, 255, 0),
            3
        )


# ==================================================
# VENTANA
# ==================================================

cv2.namedWindow("Dibujo Interactivo")
cv2.setMouseCallback(
    "Dibujo Interactivo",
    mouse_callback
)

# ==================================================
# BUCLE PRINCIPAL
# ==================================================

while True:

    pantalla = canvas.copy()

    dibujar_interfaz(pantalla)

    cv2.imshow(
        "Dibujo Interactivo",
        pantalla
    )

    tecla = cv2.waitKey(1) & 0xFF

    # -------------------------
    # DESHACER
    # -------------------------

    if tecla == ord("z"):

        if len(historial) > 1:

            historial.pop()

            canvas = historial[-1].copy()

    # -------------------------
    # GUARDAR
    # -------------------------

    elif tecla == ord("s"):

        ruta = (
            raiz /
            "resultados" /
            "dibujo_interactivo"
            "dibujo_interactivo.png"
        )

        cv2.imwrite(
            str(ruta),
            canvas
        )

        print(
            f"Guardado en: {ruta}"
        )

    # -------------------------
    # SALIR
    # -------------------------

    elif tecla == 27:
        break

cv2.destroyAllWindows()