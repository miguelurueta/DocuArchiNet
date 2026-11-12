# DOC-119 — seguimiento de importación

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Progreso local: 4/24 tareas verificadas; no aceptación integrada ni cierre.

## Objetivo

Acreditar seguimiento SEG-01/02 antes de reorganizar UI-01–05. Mantener SEG-03 condicionado a decisión expresa. El usuario continuó tras revisar refinamiento/tareas de 94ca9277; aprobación local registrada en refinement.md y auditoría OPSXJ PASS, sin autorización de pruebas reales.

## Alcance y compatibilidad

Verificadas 1.1 (matriz de procedencia), 2.1 (reproducción), 2.2 (filtros) y 2.3 (CSV). La vista ofrece filtros de estado de carpeta y referencia/efecto/recepción de archivos; paginación y exportación conservan parámetros. Denegación no conserva filas/exportación anteriores. Se renuevan recursos modificados en ASPX.

Se reutilizan DTO/servicios/repositorio/autoridad; no cambia backend, esquema, SII, almacenamiento compartido, permisos ni configuración. Accesos/menú no reorganizados: requieren antes aceptación integrada SEG-01/02. Plan real preparado para concretar ambiente/cuentas/lotes; 1.2 sigue abierta.

Reversa de este cambio de cliente: restaurar sus recursos/caché conservando datos/pistas/efectos; no borrar staging ni revertir lotes.
