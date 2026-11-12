# DOC-120 — ImpactoUI

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Superficies UI

La UI de producto no cambia. Menú superior/contextual, destino fijo, consulta/referencia/último lote/verificación y árbol por IDs conservan15 huellas DOC119. El diff de las fuentes y pruebas UI/carga individual respecto a94d57979 está vacío.

## Validacion visual

Se reutiliza la evidencia local DOC119 de navegación, homónimos/denegación, reapertura/recarga, foco/viewport y26 contratos de invocador. No se convierte en nueva sesión IIS ni26 flujos de negocio reales.

DOC120-SUP01: auxiliar de degradación con botón oculto falló en Chromium; se abre el desplegable antes de pulsar. Caso nuevo usa cliente real local, Bootstrap CSS local y respuesta503 interceptada.4/4 políticas/UI pasan después; no hay DDL/login/consulta real. Las fuentes exactas y logs están fijados en doc120-consolidacion-20261112.json.
