# DOC-110 — Revisión consolidada de documentación

Fecha: 2026-10-20. Alcance: coherencia de diseño, requisitos, tareas, prompts y SQL preparado. No es verificación de implementación ni garantía de ausencia de regresiones.

## Diagnóstico

**Actualización posterior:** el contrato común `08-origenes-datos.md` registra topología multibase confirmada. Esta revisión no cierra la identidad de gabinete/índices RA/DA. Su caracterización debe completarse antes de aceptación definitiva de ese vínculo y del SQL; no tomar «preparado para revisión» como autorización para aplicar el script.

| Área | Resultado | Evidencia / trabajo posterior |
|---|---|---|
| Alcance del perfil | Delimitado | Configuración/versiones/ensayo/publicación; carpetas, transferencia, expedientes y lotes en otras entregas. |
| Empresa y conexión | Definido con código y SELECT | Empresa de usuario en origen RA. No confundir GA_IDEMPRESA con empresa del catálogo de módulos; implementar y probar adaptador. |
| Administración de permisos | Dependencia externa confirmada | Contrato externo entregado; sin escritor nuevo en este repo. Aceptación conjunta 9.6 antes de habilitación. |
| Esquema del permiso | Inspeccionado | 78 filas/usuarios actuales, índice de usuario no único. Rechazar ambigüedad; default 0 propuesto y no aplicado. |
| Fuentes de expediente | Contrato técnico redactado | Documento común 05, tipos/etapas/casos. Límite inicial 45 y formatos son decisiones técnicas propuestas, no capacidades desplegadas. |
| Coincidencias | Política aprobada | BLOQUEAR/default o REUTILIZAR_VALIDADO; contrato 06. Identidad física/comparación y concurrencia se caracterizan en adaptador 08. |
| Desactivación | Política aprobada y consumidores alineados | Bloquear nuevas selecciones/confirmaciones; continuidad de lotes confirmados con permisos vigentes. Detención explícita separada. |
| HTTP/CSRF | Contrato técnico redactado | Documento 07: operaciones/revisiones, guard local, errores y recuperación. Requiere pruebas reales ASMX/IIS antes de habilitar mutaciones. |
| SQL 01 | Archivo y procedimiento disponibles | Tres CREATE y un ALTER administrativo; tipos/índices contrastados documentalmente con modelo. No ejecutado ni validado por servidor MySQL. |
| E2E | Reutilización y límites identificados | Helper/config/reporteros existentes. Adaptador READY/count=0 no equivale a E2E funcional. PER-01–03 no ejecutados. |
| Pruebas de componentes | Arnés observado | vbc/csc/MSBuild y node:test ya usados en repo; rutas propuestas documentadas, nuevas pruebas no implementadas. |
| Estados documentales obsoletos | Corregidos | Asignación externa, coincidencias/T11, SQL generado y fechas calendario frente a UTC sincronizados en documentos activos. |
| SII y carga individual | Sin cambios funcionales en esta revisión | Preservación contractual; regresión real sigue pendiente de implementación y pruebas, no se declara por revisión estática. |

## Validaciones ejecutadas

| Comprobación | Resultado | Límite |
|---|---|---|
| `openspec.cmd validate doc-110-produccion-importacion-carpeta-perfil --strict` | PASS | Estructura OpenSpec, no comportamiento. |
| `opsxj:prompt-review <ruta absoluta> --no-fix` sobre 02/06/08/10 | PASS en cuatro prompts | Cada informe recomienda revisión semántica; no genera pruebas de aplicación. Se conservó el reporte previo del repo. |
| `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-110` | FAIL solo en `refinement:approved`; demás controles PASS | El refinamiento permanece draft. No cambiar marcador para ocultar la condición. |
| Longitud prompt 03 | 32.351 unidades UTF-16 con CRLF | Dentro de 32.767; archivo no ampliado en esta revisión. |
| Conteo del plan | 67 tareas; 15 documentales completadas | Las restantes incluyen revisión/aprobación final, implementación y validaciones; no son 52 dudas de diseño. |

Reportes de prompts: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-revision-documental/`. La revisión semántica de este documento complementa esos controles.

## Condiciones restantes sin confundirlas con diseño abierto

1. Revisión y aprobación responsable del refinamiento completo y del DDL propuesto: 2.2/9.5 siguen abiertas. Las aprobaciones anteriores fueron de políticas concretas.
2. Implementación de modelos, repositorios, servicios, guard, interfaz y pruebas según dependencias del plan. No iniciada en esta fase documental.
3. Aplicación manual del SQL por el equipo; no se ejecuta por autorizar documentación.
4. Integración de la capacidad en la aplicación externa y aceptación conjunta autorizada.
5. Ambiente/cuentas/datos descartables y autorización para E2E reales. Las consultas SELECT de exploración no autorizan escrituras de prueba.
6. Caracterización de efectos legacy e identidad/reutilización para las entregas operativas posteriores. No ampliar DOC-110 con esos efectos.

El conjunto puede pasar a revisión final del refinamiento. No se necesitan más búsquedas del administrador de permisos dentro de este repo ni nuevas decisiones sobre las dos políticas ya aprobadas.
