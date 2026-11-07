# DOC-110: controles SELECT de identidades y concurrencia

Tarea 10.19, D-05/RQ-05, D-02/RQ-02, D-07/RQ-07 y COR-01/COR-02. Completa con carreras autenticadas previamente registradas, rechazos sin facultad y los nuevos controles directos de solo lectura.

## Ejecución directa

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/verify-doc110-identities-readonly.ps1
```

Resultado **PASS**, 14 comprobaciones, finalizado 2026-10-27T16:05:49.2906077Z. Motor observado: MySQL 5.1.37-community. Inventario: **22 perfiles, 35 versiones, 144 reglas**.

El script resuelve RA desde el módulo documental activo del catálogo configurado; exige un módulo inequívoco. Solo ejecuta SELECT, con tiempo acotado, sin escrituras, DDL, cambios de permisos ni exposición de cadenas/credenciales. Los nombres de tablas/columnas son constantes del contrato; no recibe SQL del usuario. El reporte tools/e2e/artifacts/doc110-identities-select-result.json contiene exclusivamente conteos, resultado, versión del motor y fecha.

| Comprobación | Resultado |
|---|---|
| Perfil con ID/revisión cero | 0 |
| Versión con ID/perfil/revisión/número cero | 0 |
| Regla con ID/versión cero | 0 |
| Código duplicado dentro del ámbito | 0 |
| Número de versión duplicado dentro del perfil | 0 |
| Destino de regla duplicado dentro de versión | 0 |
| Perfil con numeración de versiones no consecutiva desde 1 | 0 |
| Versión sin perfil | 0 |
| Regla sin versión | 0 |
| Referencia publicada inexistente, ajena al perfil o no PUBLICADA | 0 |
| Publicación sin hash de 32 bytes o sin actor/fecha | 0 |
| Identidad de perfil INT UNSIGNED AUTO_INCREMENT, InnoDB y siguiente mayor que máximo | PASS |
| Identidad de versión con iguales condiciones | PASS |
| Identidad de regla con iguales condiciones | PASS |

## Correlación con evidencia de concurrencia y autorización

La [evidencia autenticada anterior](doc110-continuacion-20261024.md) distingue sesiones ASP.NET independientes de dos solicitudes de una sola sesión:

- Guardado: 1 passed (33.4 s), caso 32.2 s; un ganador 200 y un conflicto 409, ambas sesiones leen revisión y documento del ganador.
- Creación de versión: 1 passed (38.9 s), caso 37.6 s; una sola versión nueva, revisión única, IDs/números distintos y resultado compartido.
- Publicación concurrente: 1 passed (36.6 s), caso 35.4 s; un ganador 200/conflicto 409 e incremento único, ambas sesiones leen el mismo documento publicado.
- Cuenta autenticada sin facultad: recorridos --isolation anteriores rechazan detalle/historial y acceso administrativo sin datos. La [revocación posterior](doc110-revocacion-permiso-preparacion.md) también confirma rechazo en una sesión vigente y restauración.

La aceptación real entre empresas es **NO_APLICA** por [decisión explícita de empresa única](../Exploracion/criterio-empresa-unica-doc110.md). Se conserva autorización por recurso/permiso.

Los SELECT nuevos verifican invariantes del estado actual de todas las filas después de esas corridas; no constituyen una nueva carrera ni un snapshot SQL anterior/posterior tomado durante cada carrera histórica. La evidencia de ganador/revisión exacta procede de las lecturas HTTP de ambas sesiones en las corridas identificadas. Tampoco se afirma solapamiento interno de todas las transacciones posibles. No se repitieron escrituras para este cierre.

## Identificación del código de control

HEAD del repositorio al verificar: b2e93b726f221868cf2b82d43200a8e568871117. El árbol contiene cambios sin commit; ese SHA por sí solo no representa la implementación completa. SHA-256 del script de control: A77A0A768B37B01D119BE8FE83E47E5B0929FF7DE7896238687CF181CAE3CB13.

Este resultado no acredita instalación nueva/migración ni comparación de JSON/huellas previas de 10.16. No se cierra esa tarea con el mero hecho de observar esquema incremental y motor 5.1.
