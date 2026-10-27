# DOC-110: conflictos y fallos de metadatos en el servicio de fuentes

Fecha: 2026-10-27. Avance de 15.5/15.14 (D-09/RQ-12). Ambas tareas permanecen abiertas. Solo pruebas locales; sin nueva autenticación ni acceso al origen.

Actualización de alcance por instrucción del usuario: instalación permanente de empresa única; aceptación real entre empresas NO_APLICA. Se retira la solicitud de otra cuenta/ámbito. [Decisión](../Exploracion/criterio-empresa-unica-doc110.md). Los pendientes transaccionales y de concurrencia reales se mantienen; las notas inferiores sobre dos ámbitos corresponden al criterio anterior.

## Nuevas verificaciones

`FuenteImportacionBehaviorTests` ejecuta ServicioFuenteImportacion productivo con un repositorio doble y un lector de metadatos explícitamente simulado:

| Condición | Comprobación |
|---|---|
| Sin facultad de fuentes | LISTAR/OBTENER/HISTORIAL/ALTA/EDITAR/ACTIVAR/REVOCAR denegados antes de repositorio y metadatos. |
| Revisión obsoleta | EDITAR/ACTIVAR/REVOCAR producen CONFLICTO_REVISION antes de consultar el origen o guardar; snapshot conservado. |
| Metadatos no disponibles | EDITAR/ACTIVAR propagan CAPACIDAD_NO_DISPONIBLE sin llamar a Guardar; snapshot conservado. |
| Carrera después de validar metadatos | El repositorio simula CONFLICTO_REVISION en Guardar después de la lectura y validación. El servicio propaga el conflicto con una sola llamada, sin reintentar ni alterar propuesta/snapshot. |
| Revocación con origen no disponible | REVOCAR llega al repositorio sin invocar el lector de metadatos que falla. No requiere consultar DA para retirar la autorización. |

El contador Writes del doble cuenta llamadas, no commits. No se presenta como un rollback real ni se simula una transacción para atribuirle garantías MySQL.

```powershell
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

PASS: una prueba Node compila y ejecuta el arnés .NET Framework, incluidos los nuevos escenarios. Sin conexión ni HTTP.

## Revisión del repositorio y pendientes

La inspección de MySqlFuenteImportacionRepository confirma una transacción RepeatableRead que engloba cabecera, sustitución de columnas e historial, con Commit únicamente tras completar la operación. Las modificaciones leen la fuente bajo FOR UPDATE y vuelven a comparar revisión. Leer y Listar filtran ámbito parametrizado; Historial verifica primero acceso a la fuente. Los errores MySQL 1062/1205/1213 se traducen a conflicto. Esto es inspección de código, no inyección de fallos transaccionales en servidor.

Las corridas anteriores ya acreditan alta/activación/edición/revocación, columnas retiradas visibles en metadatos, revisión obsoleta rechazada y consumidores históricos preservados. Para completar 15.5/15.14 faltan los criterios reales pendientes: duplicado sin otra fuente, carrera entre sesiones con un ganador, ausencia de columnas/historial parciales tras fallo transaccional y aislamiento autorizado entre ámbitos. Requieren un fixture aislado y condiciones controladas; no se usa la fuente revocada como fixture de una nueva carrera ni se retiran tablas/interrumpe RA para fabricar el fallo.

Para la separación de ámbitos de 11.3/10.19/14.17 y fuentes, la evidencia actual de segunda cuenta sin facultad no sustituye una cuenta con permiso en otro ámbito. Se solicita únicamente identificación de esa cuenta/ámbito, sin credenciales en conversación. Sin ese dato no se elige una cuenta por inferencia ni se conceden permisos.
