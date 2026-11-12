# DOC-119 — seguimiento de importación

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Progreso local: 24/24 tareas verificadas; implementación lista para revisión, sin archivo ni despliegue.

## Objetivo

Acreditar seguimiento SEG-01/02 antes de reorganizar UI-01–05. Mantener SEG-03 condicionado a decisión expresa. El usuario continuó tras revisar refinamiento/tareas de 94ca9277; aprobación local registrada en refinement.md y auditoría OPSXJ PASS, sin autorización de pruebas reales.

## Alcance y compatibilidad

Verificadas 1.1 (matriz de procedencia), 2.1 (reproducción), 2.2 (filtros) y 2.3 (CSV). La vista ofrece filtros de estado de carpeta y referencia/efecto/recepción de archivos; paginación y exportación conservan parámetros. Denegación no conserva filas/exportación anteriores. Se renuevan recursos modificados en ASPX.

Se reutilizan DTO/servicios/repositorio/autoridad. Después de SEG-01 real y aceptación expresa de la cobertura mixta de SEG-02, se implementó el menú operativo independiente de administración, consulta en el asistente y contextual con destino pulsado, viewport y teclado. El servidor conserva la autoridad. No cambia esquema, SII, almacenamiento compartido ni configuración. SEG-03 resuelto como operación sin limpieza; no se ejecutó borrado.

El párrafo anterior de cuatro tareas describe el primer corte. El corte final del2026-11-12 consta en `Validacion/doc119-ui-verificacion-20261112.md`:99/99 pruebas locales,33/33 de carga individual/contratos y compilación sin errores. La UI final se probó localmente con servicios sintéticos; las corridas reales anteriores conservan sus límites y no se atribuyen al cliente nuevo.

Reversa de este cambio de cliente: restaurar sus recursos/caché conservando datos/pistas/efectos; no borrar staging ni revertir lotes.
