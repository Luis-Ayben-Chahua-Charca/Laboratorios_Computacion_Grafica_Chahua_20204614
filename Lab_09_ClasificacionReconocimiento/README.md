# Detector de Objetos en Tiempo Real

Aplicación de visión computacional que detecta y clasifica objetos en tiempo real usando la webcam. Dibuja cajas delimitadoras de colores con etiquetas y porcentaje de confianza, y muestra un panel lateral con la lista de objetos detectados en el frame actual.

Desarrollado para el curso de **Computación Gráfica, Visión Computacional y Multimedia** — UNSA.

---

## Demo

La aplicación abre una ventana con el feed de la webcam. Cada objeto detectado recibe:
- Una caja de color único según su clase
- Una etiqueta con el nombre y porcentaje de confianza
- Un registro en el panel lateral con conteo en tiempo real

---

## Tecnologías

| Librería | Función |
|---|---|
| [YOLOv8 (Ultralytics)](https://github.com/ultralytics/ultralytics) | Detección y clasificación de objetos |
| [OpenCV](https://opencv.org/) | Captura de video y renderizado |
| [cvzone](https://github.com/cvzone/cvzone) | Utilidades de visualización |
| [PyTorch](https://pytorch.org/) | Backend de inferencia con CUDA |

El modelo usado es **YOLOv8l** (large), preentrenado en el dataset COCO (80 clases). La inferencia corre en GPU vía CUDA.

---

## Requisitos

- Python 3.12 o superior
- GPU NVIDIA con drivers actualizados
- CUDA Toolkit 12.x o superior → [descargar](https://developer.nvidia.com/cuda-downloads)
- Webcam conectada

Verificar CUDA:
```bash
nvcc --version
```

---

## Instalación

```bash
# 1. Clonar el repositorio
git clone <url-del-repo>
cd <carpeta>

# 2. Crear entorno virtual
python -m venv venv
venv\Scripts\activate

# 3. Instalar PyTorch con CUDA
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu128

# 4. Instalar dependencias
pip install -r requirements.txt
```

> El paso 3 descarga ~2.5 GB. Requiere conexión estable.

---

## Uso

```bash
python main.py
```

La primera ejecución descarga el modelo `yolov8l.pt` (~83 MB) automáticamente.

Presiona `q` para cerrar.

---

## Configuración

En `main.py` puedes ajustar:

```python
MODEL_PATH = "yolov8l.pt"  # modelo a usar
CONF_THRESH = 0.40          # confianza mínima (0.0 - 1.0)
CAM_INDEX   = 0             # índice de webcam (0, 1, 2...)
```

### Modelos disponibles

| Modelo | Tamaño | Precisión | Recomendado para |
|---|---|---|---|
| `yolov8n.pt` | 6 MB | Básica | CPU o GPU débil |
| `yolov8s.pt` | 22 MB | Buena | GPU con 4 GB VRAM |
| `yolov8m.pt` | 50 MB | Alta | GPU con 6 GB VRAM |
| `yolov8l.pt` | 83 MB | Muy alta | GPU con 8 GB+ VRAM |

---

## Solución de problemas

**La cámara muestra imagen estática o logo**
Cambia `CAM_INDEX = 1` o `2` en `main.py`. Ocurre cuando hay una cámara virtual instalada (ej. NVIDIA Broadcast).

**PyTorch no detecta la GPU**
```bash
python -c "import torch; print(torch.cuda.is_available())"
```
Debe imprimir `True`. Si no, reinstala PyTorch con el comando del paso 3.

**Error de PowerShell al activar el venv**
```bash
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

---

## Referencias

- Ultralytics YOLOv8 — https://docs.ultralytics.com  
- COCO Dataset — https://cocodataset.org  
- Computer Vision Zone — https://www.computervision.zone/projects
