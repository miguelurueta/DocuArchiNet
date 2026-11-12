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
