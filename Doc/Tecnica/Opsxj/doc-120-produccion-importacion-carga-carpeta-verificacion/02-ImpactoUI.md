# DOC-120 — ImpactoUI

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Menú — corte posterior2026-11-13

DOC120-UI07: iconos de cabecera/opciones y colores coherentes con Archivos, preservando permisos, referencias y carga sin desplazamiento.55/55 pruebas locales con CSS/fuente reales y servicios sintéticos; sin IIS autenticado. Avance23/27, sin aceptación integral. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-menu-20261113.json. Los cortes siguientes son antecedentes.

## Corte vigente2026-11-13

Avance22/26. Corrección DOC120-UI06: carga inicial con indicador neutro accesible en el menú, sin franja amarilla ni desplazamiento41px; errores y confirmación sin acuse siguen visibles. Cambian JS/CSS y las URLs ASPX de esos recursos. Regresión focal41/41 en Chromium/cliente local con respuestas sintéticas (28 cliente,9 navegación,4 degradación); sin IIS autenticado ni cambios de negocio. Huellas nuevas: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-apertura-20261113.json. Las afirmaciones15/15/sin delta siguientes corresponden al corte histórico2026-11-12. Se conservan las cuatro tareas abiertas y los límites de capacidad/aceptación.

## Antecedente2026-11-12

## Superficies UI

La UI de producto no cambia. Menú superior/contextual, destino fijo, consulta/referencia/último lote/verificación y árbol por IDs conservan15 huellas DOC119. El diff de las fuentes y pruebas UI/carga individual respecto a94d57979 está vacío.

## Validacion visual

Se reutiliza la evidencia local DOC119 de navegación, homónimos/denegación, reapertura/recarga, foco/viewport y26 contratos de invocador. No se convierte en nueva sesión IIS ni26 flujos de negocio reales.

DOC120-SUP01: auxiliar de degradación con botón oculto falló en Chromium; se abre el desplegable antes de pulsar. Caso nuevo usa cliente real local, Bootstrap CSS local y respuesta503 interceptada.4/4 políticas/UI pasan después; no hay DDL/login/consulta real. Las fuentes exactas y logs están fijados en doc120-consolidacion-20261112.json.
