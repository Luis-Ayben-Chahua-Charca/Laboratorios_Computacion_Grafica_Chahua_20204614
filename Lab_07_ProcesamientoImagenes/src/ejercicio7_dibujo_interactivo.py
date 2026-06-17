import cv2
import numpy as np
from pathlib import Path
from tkinter import Tk
from tkinter.filedialog import asksaveasfilename
import math

# ==================================================
# CONFIGURACION
# ==================================================

raiz = Path(__file__).resolve().parent.parent

ANCHO = 1200
ALTO = 700
ALTURA_MENU = 60

canvas = np.ones((ALTO, ANCHO, 3), dtype=np.uint8) * 255

historial = [canvas.copy()]

modo = "circulo"

dibujando = False

punto_inicio = None

preview = None

# ==================================================
# BOTONES
# ==================================================

botones = {
    "circulo": (10, 10, 140, 50),
    "rectangulo": (160, 10, 320, 50),
    "linea": (340, 10, 450, 50),
    "limpiar": (470, 10, 580, 50),
    "guardar": (600, 10, 720, 50),
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
        (800, 35),
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

def guardar_dibujo(canvas):

    ruta_resultados = raiz / "resultados" / "dibujo_interactivo"

    ruta_resultados.mkdir(
        parents=True,
        exist_ok=True
    )
    
    root = Tk()
    root.withdraw()

    ruta = asksaveasfilename(
        title="Guardar dibujo",
        initialdir=str(ruta_resultados),
        initialfile="dibujo.png",
        defaultextension=".png",
        filetypes=[
            ("PNG", "*.png"),
            ("JPEG", "*.jpg"),
            ("Todos los archivos", "*.*")
        ]
    )

    root.destroy()

    if ruta:

        cv2.imwrite(
            ruta,
            canvas
        )

        print(f"Dibujo guardado en: {ruta}")

# ==================================================
# MOUSE
# ==================================================

def mouse_callback(event, x, y, flags, param):

    global modo
    global canvas
    global historial

    global dibujando
    global punto_inicio
    global preview

    # -------------------------
    # CLICK INICIAL
    # -------------------------

    if event == cv2.EVENT_LBUTTONDOWN:

        boton = boton_presionado(x, y)

        if boton is not None:

            if boton == "limpiar":

                historial.append(canvas.copy())

                canvas[:] = 255
                
            elif boton == "guardar":
                guardar_dibujo(canvas)
                
            else:

                modo = boton

            return

        if y <= ALTURA_MENU:
            return

        dibujando = True

        punto_inicio = (x, y)

    # -------------------------
    # MOVIMIENTO
    # -------------------------

    elif event == cv2.EVENT_MOUSEMOVE:

        if dibujando:

            preview = canvas.copy()

            if modo == "rectangulo":

                cv2.rectangle(
                    preview,
                    punto_inicio,
                    (x, y),
                    (255, 0, 0),
                    3
                )
            elif modo == "linea":

                cv2.line(
                    preview,
                    punto_inicio,
                    (x, y),
                    (0, 255, 0),
                    3
                )
            elif modo == "circulo":

                radio = int(
                    math.sqrt((x - punto_inicio[0]) ** 2 + (y - punto_inicio[1]) ** 2)
                )

                cv2.circle(
                    preview,
                    punto_inicio,
                    radio,
                    (0, 0, 255),
                    3
                )        

    # -------------------------
    # SOLTAR MOUSE
    # -------------------------

    elif event == cv2.EVENT_LBUTTONUP:

        if dibujando:

            historial.append(canvas.copy())

            if modo == "rectangulo":

                cv2.rectangle(
                    canvas,
                    punto_inicio,
                    (x, y),
                    (255, 0, 0),
                    3
                )
            elif modo == "linea":
                cv2.line(
                    canvas,
                    punto_inicio,
                    (x, y),
                    (0, 255, 0),
                    3
                )
            elif modo == "circulo":

                radio = int(
                    math.sqrt((x - punto_inicio[0]) ** 2 + (y - punto_inicio[1]) ** 2)
                )

                cv2.circle(
                    canvas,
                    punto_inicio,
                    radio,
                    (0, 0, 255),
                    3
                )

            dibujando = False

            preview = None


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

    
    if dibujando and preview is not None:
        pantalla = preview.copy()
    else:
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
    # SALIR
    # -------------------------

    if tecla == 27:
        break

cv2.destroyAllWindows()