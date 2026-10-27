# DOC-110: criterio de aceptación para empresa única

El usuario confirma que esta instalación tiene una sola empresa y siempre operará con una sola empresa. Por instrucción explícita, la prueba real entre empresas no es necesaria y se registra **NO_APLICA**, no PASS ni prueba pendiente por falta de cuentas.

Esta decisión elimina la necesidad de preparar otra empresa, cuenta de otro ámbito o concesiones de permisos para simularla. No elimina autorización por usuario/recurso, facultades separadas, sesión vigente, CSRF, revisión esperada ni contexto servidor. Los filtros y defensas existentes del código se conservan; no se ejecuta una migración de ámbito ni se modifica el aislamiento autorizado de fuentes por conexión/columna. El ámbito de integridad EXPEDIENTE/LOTE es un concepto distinto y sigue vigente.

## Consecuencias sobre aceptación

| Tarea | Efecto |
|---|---|
| 11.3 búsqueda asistida | Separación real entre empresas NO_APLICA. Completa con búsquedas/clic/teclado/vacío/limpieza/reapertura autenticadas (1 passed, 19.1 s), respuestas fuera de orden y render seguro locales. |
| 14.17 proveedor integrado | Rechazo real de otra empresa NO_APLICA. Completa con ciclo UI proveedor/perfil/resolución real/versionado ya acreditado (1 passed, 24.1 s; ciclo completo de fuente 23.8 s) y rechazo por falta de facultad con segunda cuenta (33.1 s). |
| 7.4 y 7.8 HTTP | No exigir otra empresa. Mantener comprobaciones pendientes de sesión expirada y demás criterios propios; no confundir contexto anónimo con expiración temporal. |
| 10.19 identidades/concurrencia | No exigir otra empresa. Mantener controles SELECT y criterios pendientes de concurrencia/identidades; conservar rechazo a cuentas sin facultad. |
| 15.5 y 15.14 fuentes | No exigir otra empresa. Duplicados, rollback y concurrencia reales conservan sus verificaciones pendientes, con fixture aislado autorizado. |

Detalles de evidencia: [búsqueda](../Validacion/doc110-busqueda-ampliada-20261027.md), [proveedor/perfil y segunda cuenta](../Validacion/doc110-continuacion-20261024.md), [ciclo completo](../Validacion/doc110-fuentes-aceptacion-preparada-20261026.md) y [conflictos de fuentes](../Validacion/doc110-fuentes-conflictos-servicio-20261027.md).

Las notas históricas que exigían dos ámbitos describen el criterio anterior y quedan superadas por esta decisión en cuanto a aceptación real entre empresas. No se reetiquetan sus resultados de ejecución. Sin nuevas pruebas autenticadas, cambios de permisos, fuentes, SQL, SII ni código productivo.
