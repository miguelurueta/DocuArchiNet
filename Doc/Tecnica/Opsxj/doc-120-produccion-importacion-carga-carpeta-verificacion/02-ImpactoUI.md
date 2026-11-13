# DOC-120 — ImpactoUI

## Corte de recepción — DOC120-REC01,2026-11-13

Tarea3.9: disponibilidad de creación por consulta, reutilizando la barrera de recepción y el ámbito completo, con dos SELECT de metadatos agrupados por página y lectura del plan. Se añade el booleano público recepcionListaParaCrear (false por defecto), sin endpoint ni esquema nuevo; pista interna excluida de JSON persistido. Cliente exige true, explica la recepción y acerca el envío. Sin cambios de creación/almacenamiento/SII/perfil Egresos ni efectos sobre lote31. Regresión local64/64 Node (incluye2 wrappers Framework/DDL) y compilación0 errores. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-recepcion-creacion-20261113.json. Avance25/29; cuatro tareas de autorización/aceptación/gobierno/entrega abiertas. Sin nueva prueba IIS autenticada; los apartados siguientes son cortes históricos y sus afirmaciones sin delta/DTO no describen esta corrección.

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Barra superior — implementación3.8,2026-11-13

Avance24/28. DOC120-UI08: barra y búsqueda adaptables con acciones/IDs originales, sin cambios de rutas de negocio; baseline funcional36 acciones y regresión60/60 locales con dobles de negocio/postback. Altura ajustada al desplegar, foco y montaje dinámico conservados. CSS/JS aislados, versionados e incluidos como Content; prueba de10 combinaciones de ancho/texto y desplegables largos. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-barra-20261113.json. Sin IIS autenticado ni efectos reales; siguen cuatro tareas abiertas. Los cortes siguientes son antecedentes.

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
