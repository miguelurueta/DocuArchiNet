# DOC-120 — índice cronológico de resultados

## Recepción previa a creación — tarea3.9,2026-11-13

DOC120-REC01: el video de Egresos reproduce creación ofrecida con PDF pendiente en lote31/plan49; cinco SELECT de diagnóstico confirman ausencia de intentos. La consulta proyecta disponibilidad calculada con la barrera existente sobre el ámbito completo EXPEDIENTE/LOTE/PARCIAL, independiente de filtros o páginas. Una lectura de plan y dos SELECT agrupados de metadatos por página; sin reservas ni efectos. El cliente exige true estricto, explica el requisito y muestra Enviar pendientes antes de filtros. La operación conserva su validación inmediata de autoridad, revisión y staging.

**64/64 pruebas Node locales**, incluidos2 wrappers Framework/DDL sin sumar sus casos internos:2 focales de recepción,5 barra,28 cliente,11 navegación,12 editor y4 degradación. Chromium y SQL/HTTP sintéticos, también390px; compilación del proyecto local con0 errores. [Huellas y resultados](doc120-recepcion-creacion-20261113.json). Avance25/29; abiertas5.2,6.4,6.5 y6.6. Sin nueva sesión IIS autenticada ni repetición de recepción/creación/almacenamiento del lote31; aceptación integral pendiente. Los cortes siguientes son históricos; sus huellas anteriores de consulta/UI/arnés no describen esta corrección.

## Barra superior adaptable — implementación3.8,2026-11-13

DOC120-UI08: barra existente con cabeceras indivisibles y búsqueda flexible; colapso Bootstrap bajo1200px y panel estrecho con desplazamiento interno. Se conservan controles,36 manejadores y funciones de enrutamiento originales. Un observador de altura llama a auto_zise sin cambiar su cálculo ni la lógica de negocio; recursos versionados e incluidos en el proyecto.

**60/60 pruebas locales**:5 barra,28 cliente,11 navegación,12 editor y4 degradación. La misma comprobación funcional pasa sobre el HTML original y el nuevo (36 rutas en escritorio/móvil, clic/Enter/restauración); el original falla alineación. Cobertura de10 combinaciones de ancho/texto, foco, altura, Importación y postback sintético. [Huellas y resultados](doc120-barra-20261113.json). Avance24/28; abiertas5.2,6.4,6.5 y6.6. Sin servicios reales, SELECT ni efectos de negocio; no sustituye QA IIS autenticada. Los cortes siguientes permanecen históricos.

## Menú de Importación — corte posterior2026-11-13

DOC120-UI07: cabecera y opciones con iconos; textos del mismo color que Archivos y cabecera con icono azul. Las etiquetas dinámicas de consulta/lote mantienen sus iconos al sincronizar y recargar. Se conservan las opciones según permisos y el indicador de carga sin desplazamiento. Recursos versionados doc120-menu-20261113-2.

**55/55 pruebas locales**:28 cliente,11 navegación,12 editor de perfiles y4 degradación. Dos casos nuevos comparan colores calculados contra Archivos y verifican iconos, permisos y referencias. CSS Bootstrap/de página y fuente de iconos locales; respuestas sintéticas, sin autenticación IIS ni operaciones reales. [Huellas y resultados](doc120-menu-20261113.json). Avance23/27; las cuatro tareas abiertas permanecen. Los siguientes cortes conservan sus versiones históricas.

## Corrección de apertura — 2026-11-13

DOC120-UI06 (D-04/D-10, RQ-04/RQ-10), solicitada con «corrige» tras revisar el video: la carga inicial usaba el aviso amarillo y desplazaba el contenido41px. Ahora muestra un indicador neutro en el menú y conserva el mensaje accesible sin ocupar una franja. Errores, indisponibilidad y confirmación sin acuse conservan sus avisos. CSS/JS usan versión doc120-apertura-20261113-1.

Regresión local nueva: **41/41** (28 cliente,9 navegación,4 degradación), con3 casos nuevos de apertura demorada autorizada/denegada y verificación con aviso conservado. Respuestas sintéticas interceptadas en Chromium; ninguna autenticación, HTTP de aplicación, SELECT ni carga real. [Evidencia y huellas](doc120-apertura-20261113.json). Avance22/26; abiertas5.2,6.4,6.5 y6.6.

El manifiesto del2026-11-12 permanece histórico: sus15/15 huellas y diff de producto vacío no describen esta corrección. La vigencia UI se reevalúa con las huellas nuevas; backend, plugin contextual y carga individual no cambian. La prueba local actual no renueva SEG01/02 ni acredita una sesión IIS.

## Corte histórico de consolidación — 2026-11-12

Estado: **consolidación y regresión local; aceptación integral sin completar**. Base funcional8c0773d4, inspección 7310658894305bb203a489f2fb7b9077767dbe2c. [Matriz única](matriz-verificacion.md) y [manifiesto de huellas/primarios](doc120-consolidacion-20261112.json). Ningún resultado real anterior se atribuye a esta versión como corrida nueva.

## Cortes históricos consumidos

| Corte | Caso y fuente | Resultado y límite conservados |
| --- | --- | --- |
| 2026-11-01/02 | [doc111-matriz-final-20261102.md](doc111-matriz-final-20261102.md) | REC completos/parciales, degradación de una tabla y carga individual; sin SII ni limpieza reales. |
| 2026-11-03 | [doc113-verificacion-final-20261103.json](doc113-verificacion-final-20261103.json) | UI01–03 histórica anterior a la reorganización; no sustituye UI definitiva. |
| 2026-11-03 | [doc114-final-manifest-20261103.json](doc114-final-manifest-20261103.json) | PLAN01–03 y restauración; conteos del corte original. |
| 2026-11-04 | [doc115-verificacion-final-20261104.json](doc115-verificacion-final-20261104.json) | RECIB01–03/REPO07A;93Node/2Framework originales, no corrida DOC120. |
| 2026-11-05 | [doc116-entrega-20261105.json](doc116-entrega-20261105.json) | EXP01/02 cliente/02 interno/03 con sus primarios; incertidumbre41 conservada. |
| 2026-11-05/06 | [doc117-matriz-almacenamiento.md](doc117-matriz-almacenamiento.md) | ALM01/02/03C con cortes y laboratorios propios; incertidumbre53 y fallos previos conservados. |
| 2026-11-10 | [doc118-resultados-integracion.md](doc118-resultados-integracion.md) | Archivo administrativo anterior y cierre técnico posterior separados. |
| 2026-11-10 | [doc118-integracion-completa-20261110.json](doc118-integracion-completa-20261110.json) | COORD02/03 y38/38; laboratorio/continuación focal, sin reescribir motor detenido. |
| 2026-11-12 | [doc119-seg01-aceptado-20261112.json](doc119-seg01-aceptado-20261112.json) | SEG01 PASS de la corrida ccb25ec9; STOPPED global sigue por otro segmento. |
| 2026-11-12 | [doc119-recuperacion-verificada-20261112.json](doc119-recuperacion-verificada-20261112.json) | SEG02 real con límites281HTTP/27SELECT/1sesión; sin inserción nueva ni raíces físicas homónimas. |
| 2026-11-12 | [doc119-aceptacion-cobertura-mixta-propuesta-20261112.md](doc119-aceptacion-cobertura-mixta-propuesta-20261112.md) | Aceptación mixta explícita de DOC119, sin modificar runtime PASS_READONLY_WITH_LIMITS. |
| 2026-11-12 | [doc119-ui-verificacion-20261112.md](doc119-ui-verificacion-20261112.md) | UI definitiva local99/99, contratos33/33 y build0errores; sin nueva UI IIS autenticada. |
| 2026-11-12 | [doc119-archivo-20261112.md](doc119-archivo-20261112.md) | Archivo/spec sync previos a merge PR130; main8c0773d4 confirmado en el flujo. |

## Comprobaciones nuevas de DOC120

- Plan local aprobado por «continua» sobre73106588; opsxj:refine -- DOC-120 --sync PASS15/15. No aprobación de efectos reales.
- Comparación15/15 huellas DOC119 y diff de componentes/pruebas UI vacío: se reutiliza evidencia local vigente, no se reejecuta99/33 por cambiar de ticket.
- Regresión focal inicial:5/5 Node (2 wrappers Framework/DDL y3 políticas de degradación), archivo excluido .tmp/doc120-backend-degradation-local.log. Framework usa disco temporal y dobles SQL/HTTP; ninguna conexión real.
- SUP01: caso nuevo Chromium/cliente real local reproduce Timeout del botón oculto antes del arreglo (.tmp/doc120-degradation-before-fix.log,1fallo). Después del arreglo,4/4 políticas/UI (.tmp/doc120-degradation-after-fix.log). Tres políticas se repiten; no sumar5+4 como9 casos distintos.
- Solo cambia el auxiliar E2E y su prueba; producto, esquema, configuración, permisos y SII sin delta desde8c0773d4. No se ejecuta el runner de degradación real, puente administrativo o DDL.
- Fuentes, logs y resultados primarios enlazados quedan ligados por SHA-256 en doc120-consolidacion-20261112.json. Los JSON históricos no se sobrescriben.

## Contrato de medición INT-03 — NO_EJECUTADO

El solicitante y operador deben definir y autorizar los siguientes campos antes de medir. Esta tabla concreta la información que falta; no elige números por inferencia ni convierte límites técnicos del transporte en capacidad demostrada.

| Campo | Estado / decisión necesaria |
| --- | --- |
| Objetivo | NO_DEFINIDO: volumen normal/pico y resultado que se quiere acreditar. |
| Corpus | NO_DEFINIDO: raíces/archivos, tamaños y distribución; conservar PDF/rechazo TIFF. |
| Navegadores | NO_DEFINIDO: producto/versión; no inferir otros navegadores desde Chromium local. |
| Concurrencia | NO_DEFINIDA: sesiones/importadores simultáneos y fases incluidas. |
| Ambiente y origen | NO_AUTORIZADO para INT03: URL/aplicación, servidor/BD y versión/binarios que se observarán. Localhost de pruebas anteriores es antecedente. |
| Cuentas y recursos | NO_SELECCIONADOS: actor/ámbito/perfiles/destinos/recursos propios y ajenos; secretos solo en consola de ejecución autorizada. |
| Disco temporal/libre | SIN_MEDICION_ACTUAL: definir corpus esperado, reserva libre y consumo permitido; no usar57archivos/24.371bytes históricos como actual. |
| Tiempo y memoria | SIN_UMBRALES: definir duración máxima, percentil/medida por fase, pico de memoria y tolerancia. |
| Topes | SIN_APROBAR: HTTP/SELECT/sesiones/bytes/recibidos/creaciones/documentos; límite por ventana y acumulado. |
| Efectos | NO_AUTORIZADOS: distinguir lectura, preparación, confirmación, recepción, creación, almacenamiento y preservación; sin limpieza ni replay incierto. |
| Parada y recuperación | Acordar códigos/umbrales de detención, preservación, revisión de resultados inciertos y restauración si se autoriza variante. |
| Instrumento | Reutilizar runners, sesiones/SELECT y herramientas existentes; seleccionar el modo después de fijar objetivo y plan, sin plataforma nueva. |
| Decisión solicitada | Definir prueba acotada o mantener NO_EJECUTADO y revisar ese límite al cierre; sin respuesta no hay autorización ni aceptación. |

No se infiere capacidad máxima de MaximoBytesLote=ULong.MaxValue, de un lote de27PDF, un máximo de página o un solapamiento de laboratorio. LimitesImportacionCarpetas deriva restricciones de JSON/transporte/sesión, sin cupo acumulado de negocio nuevo. Sin contrato aprobado no hay medición ni cierre integral por inferencia.

## Estado de aceptación

Los límites de SII real, limpieza real y UI final autenticada se detallan con responsables/impacto en la matriz. No hay carga ni ventana real DOC120 autorizada en este corte. La autorización de capacidad y la revisión final de límites siguen abiertas. Los resultados locales y el avance documental no equivalen a Jira Listo, archivo ni aprobación de salida operativa.
