# DOC-114 — Recursos propuestos tras consulta real

Fecha: 2026-11-03. Consulta autenticada de solo lectura PASS, HTTP 200, nivel 7. No hubo planes ni cambios de permisos. Publicaciones disponibles: 24/38 COMPLETO_OBLIGATORIO/LOTE y 25/39 PARCIAL_PERMITIDO/NO_APLICA; ambas gabinete 10, cinco reglas. No se observó publicación EXPEDIENTE compatible.

## Reutilización y brecha de datos

Las publicaciones actuales toman código/nombre de carpeta completos y usan constantes para fecha, tipología e índice 14. No pueden acreditar dos formatos de extracción, fecha por nombre, tipología por nombre o fuentes diferidas de PLAN-01. Se conservan para LOTE/PARCIAL de PLAN-02 y como controles compatibles de PLAN-03; no se habilita ni sustituye una versión antigua del perfil 24.

## Dos perfiles nuevos propuestos

| Código nuevo | Política | Fecha/código de carpeta | Tipología de archivo | Índice 14 |
|---|---|---|---|---|
| DOC114-FORMATO-A-20261103 | COMPLETO_OBLIGATORIO/EXPEDIENTE | Segmentos con -, fecha yyyyMMdd | ID interno explícito REFERENCIA en DOC114-<id>.pdf | EXPEDIENTE.ID diferido, sin valor ficticio |
| DOC114-FORMATO-B-20261103 | COMPLETO_OBLIGATORIO/EXPEDIENTE | Segmentos con #, fecha dd-MM-yyyy | Código explícito TEXTO en <codigo>.pdf | EXPEDIENTE.ID diferido, sin valor ficticio |

Se copia la configuración vigente de 24/38, incluida clasificación y gabinete, comprobando primero su huella. Solo cambian NombreVisible, política EXPEDIENTE y las cinco reglas explícitas del archivo [propuesta revisable](doc114-perfiles-propuestos-20261103.json). Todos los ReglaId de las copias son null al guardar: no se reutilizan identidades de reglas ajenas. Los límites de código 45 y nombre funcional 120 permanecen en el servicio; el índice diferido conserva longitud 20. No se modifica el escritor documental ni se crean tablas.

Formato A prueba referencia interna y formato B código de tipología como conceptos distintos. El código real de la tipología autorizada debe obtenerse por un control de lectura existente antes de preparar los datos de PLAN-01; no se presume que sea igual al ID ni se toma 125 del ejemplo sintético como dato real. La propuesta de publicación no contiene un código o ID documental futuro inventado.

## Efectos a aprobar

Como máximo dos perfiles nuevos y dos publicaciones nuevas mediante DOC-110, con lectura posterior y recursos conservados. Activar únicamente los nuevos perfiles si el servicio los crea inactivos. Cero planes, lotes, documentos, recepción, creación de expedientes o cambios de permisos en este paso. Las publicaciones 24/38 y 25/39 deben conservar sus hashes. Cada identidad nueva se registra antes de avanzar; una respuesta incierta detiene la operación sin retry.

Esta propuesta está authorized=false. La autorización vigente cubre exclusivamente lectura de publicaciones/reglas. No se publicó ningún recurso. Las corridas PLAN y la revocación de IMPORTAR_CARPETAS requieren alcances adicionales separados; las tareas de ejecución real permanecen abiertas.

## Validacion local de la propuesta

Las dos reglas/configuraciones materializadas sobre la configuracion local de referencia pasaron ValidadorPerfilImportacion.Validar(publicar=true), cargado desde la DLL Framework compilada en salida aislada. Resultado DOC114_PROPOSED_RULES=PASS; PROFILES=2. No se consultaron catalogos reales ni se comprobo publicabilidad en el ambiente; esa compatibilidad debe contrastarse antes de publicar. Este resultado no autoriza ni acredita creacion de perfiles.


## Publicador preparado — 2026-11-03

Continuación directa del usuario tras la solicitud específica de crear/publicar los dos perfiles: autorización registrada para DOC114-FORMATO-A-20261103 y DOC114-FORMATO-B-20261103, conservando 24/38 y 25/39. El JSON de propuesta aprobado se vincula por SHA256 a la autorización local. Esta autorización no incluye preparaciones, documentos ni cambios de permisos.

Soporte local en `tools/e2e/tests/support/doc114-published-test-profiles.cjs`: reutiliza la sesión, navegación administrativa DOC-110 y transporte existentes; verifica huellas originales, ausencia de códigos, revisiones, contenido publicado y compatibilidad operativa. Cada operación se envía una vez. La consola registra intentos e IDs inmediatamente y conserva el estado ante respuesta incierta. No se crean conexiones ni mecanismos de login nuevos. El lector compartido incorpora modo de solo lectura que rechaza PrepararPlan antes del transporte.

Regresión local 68/68 PASS: driver 54, perfil parcial 9 y publicador 5. Se cubren falta de autorización, origen cambiado, código ocupado, fallo intermedio sin reintento y preservación de referencias. La validación del alcance contra el perfil runtime pasa sin login. Evidencia: [manifiesto del publicador](doc114-publisher-local-20261103.json). La sesión autenticada aún no se ha ejecutado; no se acreditan publicaciones ni PLAN-01/02/03. Corte permanece 12/16.

## Publicación real verificada — 2026-11-03

PASS de la sesión autorizada: formato A perfil 26 / versión 40; formato B perfil 27 / versión 41. Se realizaron seis operaciones, sin reintentos: CrearPerfil, GuardarBorrador y PublicarVersion para cada copia. Ambas quedaron publicadas y disponibles operativamente en nivel 7, gabinete 10, ámbito EXPEDIENTE. Se contrastaron contenido y referencias por lectura administrativa y catálogo operativo; las huellas de 24/38 y 25/39 se conservaron antes y después.

Cero planes, documentos o cambios de permisos. Nuevos recursos conservados. [Evidencia saneada](doc114-publication-result-20261103.json). Este resultado acredita recursos previos; no equivale a aceptación PLAN-01/02/03. Tareas permanecen 12/16. Próximo paso: contrastar la tipología real (ID para A y código para B), preparar y revisar los siete casos PLAN-01 con estas versiones y solicitar autorización específica para esas preparaciones antes de ejecutarlas. No se deriva autorización de planes ni revocación de la publicación ya realizada.
