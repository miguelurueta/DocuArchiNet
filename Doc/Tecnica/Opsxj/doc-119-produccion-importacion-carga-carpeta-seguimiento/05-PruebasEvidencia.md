# DOC-119 — evidencia del avance local

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Base de implementación: 94ca9277ce234b337b73bdda9bbd881e037b3047, con cambios locales descritos.

## Evidencia requerida

1. Corte previo de refinamiento sobre d7dbf201: 16/16 pruebas cliente PASS, sin cambios de producto en aquel corte.
2. Reproducción DOC-119: dos pruebas nuevas fallan por filtro inexistente antes de cambiar producto.
3. Corrección de filtros/CSV: `node --test tests/doc111-importacion-carpetas-client.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs`: 20/20 PASS (18 cliente y dos del arnés Framework/DDL).
4. Después de añadir Enter sin submit implícito: `node --test tests/doc111-importacion-carpetas-client.test.cjs`: 18/18 PASS. Backend no se modificó y no se repitió; no se suman estas corridas como casos distintos.
5. Auditoría de refinamiento y OpenSpec estricto PASS; comprobación de sintaxis JS y diff sin errores.

Las dos pruebas nuevas usan Chromium/cliente/vista reales y servicios sintéticos; verifican filtros/página/CSV, limpieza, separación de filtros de carpetas/archivos, denegación sin exportación obsoleta, Enter sin postback y viewport de 390 px. El CSV sintético verifica transporte/selección; CeldaCsv productivo se comprueba en el arnés backend existente. No atribuir el CSV simulado a una ejecución real del servicio.

Informes: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-procedencia-20261111.md` y `doc119-filtros-local-20261111.md` en el mismo directorio. Los antecedentes DOC-111/116/117/118 conservan su código/runtime/ambiente y no se reetiquetan como DOC-119.

## QA/E2E WebForms

No ejecutados por DOC-119: autenticación real, HTTP de aplicación, SELECT/MySQL reales, recepción/carga, almacenamiento, inyección de fallos, cambios de permisos, limpieza o despliegue. El plan de aceptación preparado aún requiere ambiente/cuentas/lotes y autorización específica.

Tareas verificadas: 1.1, 2.1, 2.2, 2.3; 4/24. SEG-01/02 siguen sin aceptación integrada; 1.2 abierta para concretar plan. SEG-03 sigue condicionado. No se reorganizan accesos antes de compuerta 2.7, no se solicita cierre ni se marca evidencia manual como aprobada.

## Preparación posterior al «autorizado» del usuario

Ventanas de lectura del plan local autorizadas; cuentas/recursos se capturan por consola enmascarada antes de actuar. Selector específico preparado con sesión/SQL existentes y límites persistentes, sin habilitar efectos. `doc119-selector-lectura-20261111.md` detalla fuentes y límites.

Regresión nueva: **34/34 PASS** (21 cliente/selector y 13 políticas), incluyendo los tres recorridos del selector con servicios simulados; `--check`, sintaxis Node, OpenSpec estricto y refinamiento PASS. No acredita todavía una ejecución real. Progreso permanece 4/24 hasta revisar preflight y resultados con referencias concretas. Estado de ejecución se conserva solo en .tmp, excluido de Git.

## Selección comprensible — 2026-11-12

El selector ahora muestra importaciones existentes de la cuenta autenticada por fecha, destino y estado, sin exigir IDs internos. Descubrimiento parametrizado y limitado a usuario/empresa/facultades actuales; sesión y consultas cuentan dentro de los presupuestos existentes. La captura anterior se reemplazó antes de cualquier sesión/HTTP/SELECT, conservando su estado local.

Regresión actual: **39/39 PASS** (21 cliente/selector y 18 políticas), sin sumar corridas anteriores. Incluye selección por opción, guardia sin recurso elegido, descubrimiento acotado, cuenta ambigua y archivo pendiente. Sigue sin acreditar aceptación integrada real; avance 4/24. Detalle en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-seleccion-importaciones-20261112.md`.

## Primer intento real y corrección del ejecutor — 2026-11-12

Corrida `d09ca8a7cdc547cca79fb53d37af26af`: STOPPED, guardia `DOC119_HTTP_SCOPE_DENIED`, error exterior `DOC111_LOGIN_TRANSPORT_FAILED`. Consumo: 1 sesión, 1 HTTP, 0 SELECT; postback no observado, sin importación elegida ni aceptación SEG-01/02. Secretos eliminados al cerrar. Diagnóstico posterior sin credenciales: 1 GET, 0 sesiones/SELECT; primera solicitud bloqueada `GOOGLE_FONT_STYLESHEET`. Ambos registros se conservan excluidos de Git.

Corrección: abortar localmente la hoja opcional exacta de Google Fonts sin enviar tráfico externo ni detener la guardia; el resto de los límites sigue vigente. Fallos de guardia se informan con su código propio. Regresión **40/40 PASS** (21 cliente/selector, 19 políticas), incluida omisión sin consumo de red y rechazo de POST a esa misma URL. No se suma a los cortes anteriores. El usuario pidió continuar la prueba real; se prepara nueva ventana manual conforme al plan, sin retry automático ni aceptación inferida. Progreso sigue 4/24.

## Segundo intento y preflight de acceso — 2026-11-12

Corrida `42a55db64044453c8c6cbac63109702a`: STOPPED, 93 HTTP/1 apertura de autenticación/0 SELECT; sin aceptación SEG-01/02. Diagnóstico sin credenciales: mismo bloqueo POST XHR al iniciar la página, 93 HTTP/0 aperturas de autenticación/0 SELECT. La consulta automática ObtenerEstado del cliente 2FA faltaba en la guardia. Se permite solo JSON vacío para esa operación y GET/HEAD al destino local fijo del login; ninguna mutación 2FA se habilita. Se conserva observación de login al priorizar errores de guardia y se añade diagnóstico por categorías fijas sin datos privados.

Preflight real corregido sin credenciales: **PASS_LOGIN_PAGE_ONLY**, HTTP 200, controles presentes, una consulta ObtenerEstado y ninguna violación, con 95 HTTP/0 aperturas de autenticación/0 SELECT. Huella SHA-256 del helper de ese preflight: `53fccc5e80b84a04bea291b683afcf823f6934dbd37a825bce572ef93cda1e73`. Estado en `.tmp/doc119-seguimiento/42a55db64044453c8c6cbac63109702a/login-status-preflight.json`, excluido de Git. No acredita autenticación ni consulta de importaciones.

El presupuesto HTTP original resulta insuficiente para el recorrido completo. El plan propone 600 HTTP por ventana con el resto de límites intactos; pendiente de autorización expresa, sin cambiar todavía los topes del código ni iniciar una nueva corrida. Regresión local actual: **42/42 PASS** (21 cliente/selector y 21 políticas), sin sumar cortes anteriores. Progreso 4/24.

## Ampliación aprobada — 2026-11-12

El usuario autorizó expresamente nueva corrida con 600 HTTP por ventana, 2 sesiones y 80/60 SELECT, sin ampliar efectos. Se configuran los nuevos topes y se registra su revisión en cada estado de ejecución. Los registros previos permanecen intactos. Política del ejecutor: **21/21 PASS**, incluida detención al agotar presupuesto; sintaxis Node, `--check`, diff y OpenSpec estricto PASS. No se repite la suite cliente sin cambios ni se suman corridas previas. No se atribuye resultado real a esta autorización. Progreso 4/24 hasta revisar la ejecución.

## Acceso posterior a ampliación — 2026-11-12

Corrida `c10eeb796fd147ee9742712916622cd2`: STOPPED por servicio POST fuera de alcance durante la página de inicio; 137 HTTP, 1 apertura de autenticación, 0 SELECT. Postback 200/COMPLETE, campos MATCHED y cuerpo BODY_NOT_RETAINED; Producción no observada, sin importación elegida ni huellas de negocio. Secretos eliminados al cerrar. No acredita autenticación al módulo, invariancia documental o SEG-01/02.

Corrección del ejecutor: conservar login compartido/cookie real y resolver localmente solo la página intermedia fija de inicio; abrir después Producción real. Se verifican las seis dependencias CDN exactas del ASPX y se cargan sin encabezados privados, contabilizadas en el mismo presupuesto. Se mantiene rechazo de servicios adicionales, efectos y variantes de las URLs. `access.homeSkipped` y códigos fijos de dependencias se conservan en el estado. El ASPX se incluye en evidencia de código local.

Preflight externo sin credenciales: las seis dependencias responden 200, con SHA-256 en `.tmp/doc119-seguimiento/c10eeb796fd147ee9742712916622cd2/static-preflight-network.json`; 6 HTTP, 0 aperturas de autenticación/SELECT. Se conserva el fallo previo de transporte del sandbox en otro archivo (1 intento contabilizado). Son lecturas públicas estáticas, sin aceptación funcional.

Regresión: **45/45 PASS** (21 cliente/selector y 24 políticas). Una nueva prueba Chromium en modo offline recorre el login compartido, espera la navegación intermedia, conserva la cookie sintética y abre la página sintética de Producción sin ejecutar el panel; otras dos comprueban dependencias exactas, eliminación de encabezados privados, conteo y variantes denegadas. Sintaxis Node, `--check`, diff y OpenSpec estricto PASS. Estas pruebas no se atribuyen al ambiente real.

Preflight final del login real sin credenciales, con el helper corregido: HTTP 200, controles presentes, una consulta ObtenerEstado y ninguna violación; 95 HTTP/0 aperturas de autenticación/0 SELECT. Huella del helper: `2ae98826010297371dc4941a13382b42cdb157d9a3c6afbf07936a1335c0a01e`; registro `login-corrected-preflight.json` en el directorio local de la corrida. Sigue sin acreditar acceso autenticado a Producción o SEG-01/02. Progreso 4/24; nueva consola identificada por runId y bajo control manual del operador para capturar credenciales.
