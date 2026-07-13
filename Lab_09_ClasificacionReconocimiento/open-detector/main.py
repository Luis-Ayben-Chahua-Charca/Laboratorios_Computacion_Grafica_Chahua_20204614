import cv2
import cvzone
from ultralytics import YOLO
from collections import Counter

# ── Configuración ──────────────────────────────────────────────
MODEL_PATH = "yolov8l.pt"   # se descarga automáticamente la primera vez
CONF_THRESH = 0.40          # confianza mínima para mostrar una detección
CAM_INDEX   = 0             # índice de la webcam (0 = cámara por defecto)

# Colores BGR para las clases (se asignan cíclicamente)
COLORS = [
    (255,  56,  56), (255, 157,  51), ( 51, 255, 153),
    ( 51, 153, 255), (153,  51, 255), (255,  51, 204),
    (  0, 204, 255), (204, 255,   0), (255, 204,   0),
]

def get_color(class_id: int) -> tuple:
    return COLORS[class_id % len(COLORS)]

# ── Carga del modelo ────────────────────────────────────────────
print("Cargando modelo YOLOv8...")
model = YOLO(MODEL_PATH)
model.to("cuda")            # usa la GPU NVIDIA
print(f"Clases disponibles: {len(model.names)}")

# ── Captura de video ────────────────────────────────────────────
cap = cv2.VideoCapture(CAM_INDEX)
cap.set(cv2.CAP_PROP_FRAME_WIDTH,  1280)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT,  720)

PANEL_W = 280   # ancho del panel lateral de lista

print("Presiona 'q' para salir.")

while True:
    ok, frame = cap.read()
    if not ok:
        print("No se pudo leer el frame de la cámara.")
        break

    # ── Inferencia ──────────────────────────────────────────────
    results = model(frame, conf=CONF_THRESH, verbose=False)[0]

    detected = []   # lista de nombres detectados en este frame

    for box in results.boxes:
        cls_id     = int(box.cls[0])
        label      = model.names[cls_id]
        confidence = float(box.conf[0])
        x1, y1, x2, y2 = map(int, box.xyxy[0])
        color = get_color(cls_id)

        # Caja delimitadora
        cv2.rectangle(frame, (x1, y1), (x2, y2), color, 2)

        # Etiqueta con fondo de color
        tag = f"{label} {confidence:.0%}"
        (tw, th), _ = cv2.getTextSize(tag, cv2.FONT_HERSHEY_SIMPLEX, 0.6, 2)
        cv2.rectangle(frame, (x1, y1 - th - 8), (x1 + tw + 6, y1), color, -1)
        cv2.putText(frame, tag, (x1 + 3, y1 - 4),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)

        detected.append(label)

    # ── Panel lateral ───────────────────────────────────────────
    h, w = frame.shape[:2]
    panel = frame.copy()

    # Fondo semitransparente
    overlay = panel.copy()
    cv2.rectangle(overlay, (w - PANEL_W, 0), (w, h), (20, 20, 20), -1)
    cv2.addWeighted(overlay, 0.6, panel, 0.4, 0, panel)

    # Título del panel
    cv2.putText(panel, "Objetos detectados", (w - PANEL_W + 10, 30),
                cv2.FONT_HERSHEY_SIMPLEX, 0.55, (255, 255, 255), 1)
    cv2.line(panel, (w - PANEL_W + 10, 38), (w - 10, 38), (100, 100, 100), 1)

    # Lista con conteo
    counts = Counter(detected)
    for i, (obj, cnt) in enumerate(sorted(counts.items())):
        y_pos = 65 + i * 30
        if y_pos > h - 20:
            break
        dot_color = get_color(list(model.names.values()).index(obj)
                               if obj in model.names.values() else 0)
        cv2.circle(panel, (w - PANEL_W + 16, y_pos - 5), 6, dot_color, -1)
        texto = f"{obj}  x{cnt}"
        cv2.putText(panel, texto, (w - PANEL_W + 30, y_pos),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.55, (220, 220, 220), 1)

    # Total en la parte inferior del panel
    total_txt = f"Total: {len(detected)}"
    cv2.putText(panel, total_txt, (w - PANEL_W + 10, h - 15),
                cv2.FONT_HERSHEY_SIMPLEX, 0.55, (180, 180, 180), 1)

    # FPS aproximado (se muestra en la esquina superior izquierda)
    cv2.putText(panel, "YOLOv8 | GPU CUDA", (10, 25),
                cv2.FONT_HERSHEY_SIMPLEX, 0.55, (200, 200, 200), 1)

    cv2.imshow("Detector de Objetos", panel)

    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()