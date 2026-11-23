# DOC-124 — Ventana E2E de Órdenes2025

Autorización del usuario: «lanza la E2E», después de crear plantilla27 y configurar
20000 filas. Ventana nueva para la copia validada de12759 órdenes; no se reabren
ventanas DOC122/DOC120 ni se recuperan sus secretos.

## Alcance concreto

Reutiliza runner DOC110, suite Excel DOC122/DOC124, perfil DOC88, autenticación
oficial, consola privada y controles SELECT DOC32. Añade `--excel-file` y etiqueta
`@doc124-file` en la misma suite. Un caso CREAR de plantilla27 es distinto de la
matriz completa, que mantiene tres modos y cinco casos mínimos. No se declara
completa la tarea2.6 por ejecutar sólo este archivo.

IIS local de la carpeta principal; empresa2, módulo DA11, conexión6, plantilla27/
revisión1, Hoja1 y clave NROORDEN. El propietario se resuelve por SELECT con la
cuenta E2E ingresada, antes de solicitudes de recepción/preparación. Máximo esperado
de servidor20000 filas/10 MB; archivo607575 bytes y12759 filas.

Comprueba permisos/CSRF, ID previo al archivo, recepción, recuperación y replay
del mismo ID en otra sesión, preparación concurrente desde dos sesiones independientes,
PREPARADA/filas previstas, consulta y rechazo de cuenta restringida. SELECT verifica
la ejecución propia y huellas completas de plantillas, mapeos, EGRESOS/CRÉDITOS/
CONTRATOS; el destino Órdenes permanece ausente.

Presupuesto:40 solicitudes de servicios Excel,60 SELECT externos explícitos y20min
desde el ingreso de claves. Hasta una carga, un archivo privado y dos solicitudes
idénticas de recepción. Cero DDL permanente, confirmación/importación, cambios de
plantilla/políticas/permisos o limpieza. Temporales y ejecución pertenecen al flujo
autorizado. Claves sólo en memoria; sin screenshots, vídeo o trace.

## Preparación y lanzamiento

Build net461 aislado aprobado. Mapeo físico IIS verificado y despliegue atómico de
la DLL de aplicación y dos dependencias del lector; respaldo local de la anterior.
No cambió MySql.Data ni Web.config durante el despliegue; permanece20000 previamente
autorizado. WSDL200 con IniciarRecepcionExcel/Preparar/ConsultarCargaExcel además de
DOC122. El healthcheck sin autenticación no acredita PASS de la E2E.

Run `7a82614ded3c44f8b45af3d95ab4e70b`, consola privada PID40432. Abrió en
WAITING_CREDENTIALS y pasó a RUNNING_REAL_FILE_E2E tras verificar cuenta, plantilla
y baseline con11 SELECT. El resultado autoritativo posterior está en
`.tmp/doc124-real/7a82614ded3c44f8b45af3d95ab4e70b/state.json`, excluido de Git.
No reintentar automáticamente si cierra/falla; conservar estado y evidencia.

Focal16/16PASS; Playwright descubre4 casos (dos DOC122, matriz DOC124 y archivo).
Los cambios son de arnés/documentación; no se modificó producto VB para este caso.
DOC124 conserva21/23 tareas: matriz completa y revisión final pendientes. El
resultado real se incorpora después de finalizar, separado de este lanzamiento.

Cierre posterior: PASS_REAL_DOC124_FILE1/1, ventana cerrada y claves eliminadas.
Ver [resultado](doc124-e2e-ordenes-resultado-20261123.md); no relanzar esta ventana.
