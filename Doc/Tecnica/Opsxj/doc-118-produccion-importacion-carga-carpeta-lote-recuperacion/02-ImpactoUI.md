# DOC-118 — coordinación y recuperación

## Cierre tecnico:38/38

Integracion aceptada tras revision de evidencia:COORD02 acredita misma solicitud, exclusion del otro archivo por la barrera nativa con revision vigente y retorno perdido despues del checkpoint con replay unico/misma identidad. Lote27 COMPLETADO:documentos16237 y16238, inventarios23346 y23348. COORD03 lote28 DETENIDO:documento16239/inventario23349, segundo archivo sin efecto; stop en vuelo, cierre del primer contexto y consulta por el segundo sin reanudar, todos verificados.4 raices/4 PDFs/3 documentos, sin duplicados; lotes18/21/24/25/26 conservados.285/285 Node; almacenamiento compartido/SII, binario/configuracion y archivo administrativo intactos. Consumo acumulado2793 HTTP/444 SELECT/19 aperturas; se conserva la extension revisada de sesiones. Sesiones/SDK cerrados y secretos eliminados. Los fallos previos son historia:el motor completo detenido no se reetiqueta como PASS. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-integracion-completa-20261110.json`.

## Observador corregido; preflight detenido en acceso

Se reprodujo y corrigió una carrera de reloj en la disponibilidad del observador, conservando rechazo de estados futuros/antiguos. La captura posterior completó las cuatro entradas y pasó a PREFLIGHT, pero la primera autenticación no se confirmó. Cero SELECT o recursos nuevos; SDK/contextos cerrados y credenciales eliminadas. Acumulado1671 HTTP/172 SELECT/12 intentos/aperturas; sin reintento automático.274/274 Node candidatos y32/38 tareas. Se reutiliza el diagnóstico cerrado de DOC117 para conservar códigos en futuras ejecuciones. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-observador-preflight-20261110.json`.

## Captura revisada lanzada — 2026-11-10

El usuario pidió «lanza» después de la parada sin login o efectos. Se preservó el intento anterior y se abrió la consola en el escritorio normal con observador2 enlazado;al registrar espera APPLICATION_USER. Ambas HEAD se conservan dentro del máximo:acumulado1580 HTTP/172 SELECT/11 sesiones.269/269 Node y32/38 tareas;sin nuevo PASS de integración. Lote25 confirmado/protegido y archivo anterior intacto. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-captura-revisada-20261110.json`.

## Lectura durable resuelta;captura de integración por revisar

Lote25:documento16235/inventario23339 físicamente confirmados y archivoB sin efecto, mediante25 SELECT nuevos y sin replay. La consola se hizo visible fuera del escritorio aislado. La ventana nueva consumió solo1 HEAD;se detuvo antes de login/fases, con SDK/secretos cerrados. Acumulado1579 HTTP/172 SELECT/11 sesiones.268/268 Node candidatos;32/38 tareas. Falta aclarar la captura de cuatro entradas antes de otra ejecución. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-integracion-continuacion-20261110.json`.

## Continuación técnica activa — 2026-11-10

Jira En curso y mismo cambio OpenSpec reactivado, conservando el archivo administrativo intacto. Se acreditan5.2 por resultados/procedencia explícitos y5.4 por sincronización comprobada;5.3 pasó OpenSpec estricto/refinement;32/38, con seis comprobaciones reales pendientes. La auditoría SELECT del lote25 se retoma con contadores intactos y sin replay; requiere dos entradas enmascaradas.22/22 pruebas focales; almacenamiento/SII/binarios intactos. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-continuacion-tareas-20261110.json`.

## Cierre administrativo con salvedades — 2026-11-10

Jira confirmó el cierre administrativo en Listo (comentario24490), aceptado por el usuario conservando las nueve tareas pendientes. 29/38;260/260 Node históricos y observador4.1 calificado, sin aceptación completa de COORD02/03 ni QA/E2E. Lote25 retenido con almacenamiento UNKNOWN, sin replay ni limpieza. Cambio archivado en `openspec/changes/archive/2026-11-10-doc-118-produccion-importacion-carga-carpeta-lote-recuperacion`; sin sincronizar specs ni fusionar la rama. Las compuertas ordinarias OPSXJ siguen sin aprobarse; no se presenta como cierre técnico completo. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-cierre-administrativo-20261110.json`.

## Corte local de pérdida de acuses V2

Completada6.11/D-18/RQ-18: decorador de pruebas de pérdida de un acuse nativo válido y conductor acotado Q1/Q2/Q3, fuera del proyecto productivo.247/247 Node (42 comportamiento/cliente/driver/conductor y205 herramientas);157 casos V2 internos del harness, incluidos sin sumarlos a Node. Progreso28/38, mismas10 tareas reales/finales pendientes. P3 preparado y probado como paquete local; enlace/lector del host de pruebas aún sin instalar/calificar. P4/P5 abiertos. Sin modificar almacenamiento compartido/InvocarAlmacenamiento/SII ni ejecutar HTTP/SQL/login reales. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-acuses-v2-local-20261109.json`. Sin cambios visuales o selectores públicos de fallo.

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificacion: cross_cutting
- Estado: enlace/driver V2 implementados localmente;27/37 tareas, aceptación real y calificación del runtime abiertas.

## Superficies UI

RV-01..RV-08 y6.9/6.10 no añaden botón o recuperación automática a la UI. RecuperarDocumentoV2 es una operación HTTP explícita con proyección propia; un cierre de control no debe presentarse como documento confirmado. La composición candidata de intentos nuevos y el driver están integrados localmente, sin despliegue. P3/P4/P5 del plan real continúan abiertos. No hay nueva validación visual de UI de producto en esta continuación.

Se inspeccionó js/gestion/ImportacionCarpetas/ImportacionCarpetas.js: consultar, cerrar y resume. Consultar lote recupera la vista; no ejecuta una fase ni borra la señal de detención. Cerrar libera el cliente y conserva referencias; no prueba cancelación del servidor. La consulta desde otra sesión exige autoridad actual y referencia al lote, conservando snapshot e identidades.

## Validacion visual

Esta entrega no modifica marcado, estilos ni interacciones y no acredita una validación visual nueva. COORD-03 requiere evidenciar, en ejecución posteriormente autorizada, detención durante llamada real, cierre del navegador y consulta desde otra sesión. Los botones o hints del cliente no sustituyen los controles de autoridad, revisión y reclamación del servidor.
