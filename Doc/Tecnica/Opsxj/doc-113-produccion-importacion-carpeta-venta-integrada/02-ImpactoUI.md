# DOC-113 — Ventana integrada

- Ticket: DOC-113
- Cambio OpenSpec: doc-113-produccion-importacion-carpeta-venta-integrada
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

La ventana conserva Perfil/Carpetas/Revisión/Resultados, el nivel fijo y la carga individual. Respuestas inválidas dejan un mensaje accesible en lugar de romper la revisión. Al cerrar se devuelve foco al disparador capturado antes de deshabilitarlo, solo si sigue conectado.

## Validacion visual

Chromium sintético y suite cliente: 16/16 PASS. Cubre perfiles/datos nulos, revisión recuperable, foco y retirada del host sin duplicados. No representa capturas IIS autenticadas. UI-01/02/03 reales EJECUTADO_OK. Captura del diálogo tras reapertura revisada; el selector de foco excluye diálogos legacy ocultos.

Evidencia y límites: [Informe de implementación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md).
