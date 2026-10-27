# DOC-110: estados, autorización y concurrencia del proveedor

Fecha: 2026-10-27. Cierre de 14.30, D-09/RQ-12. Se concilian pruebas locales con corridas autenticadas existentes; no se ejecuta una nueva sesión ni se modifica el ambiente.

## Matriz de aceptación

| Criterio | Evidencia y alcance |
|---|---|
| Guardado/publicación/nueva versión concurrentes | --provider-concurrent: 1 passed (33.9 s), caso 32.7 s. Dos sesiones ASP.NET independientes, un 200 y un 409 por carrera, incrementos únicos, documento ganador leído por ambas sesiones y primera publicación intacta. |
| Disponibilidad concurrente | --provider-availability: 1 passed (33.3 s), caso 32.3 s. Incluye las tres carreras previas y activación con un ganador/conflicto; desactiva en finally y ambas sesiones confirman INACTIVO. Historial conserva IDs, números, estados, contratos y revisiones de versión. |
| Edición/publicación indebidas | Arnés del servicio rechaza edición/republicación de PUBLICADA y clonación desde BORRADOR. El ciclo real rechaza GuardarProveedor sobre publicación sin cambiarla. |
| Permiso administrativo | Arnés deniega las nueve operaciones antes de repositorio/validación/consulta cuando falta permiso adicional. --isolation previo: 1 passed (33.1 s), caso 31.9 s, segunda cuenta autenticada sin facultad recibe 403 sin datos en listado/detalle/metadatos. No acredita aislamiento empresarial. |
| Protección de endpoints | --provider-guards ampliado: 1 passed (26.7 s), caso 25.7 s; 33 rechazos CSRF/sesión/revisión con documento intacto. |
| Descubrimiento fuera de alcance | Pruebas locales del materializador y metadatos rechazan ámbito, tabla y columnas ajenos antes de resolver origen. --source-scope previo: 1 passed (18.7 s), rechazo real de nombres ajenos y listado con prefijo literal RA/DA. No acredita un proveedor ya persistido fuera del prefijo. |
| Transiciones del catálogo consumidor | Ciclo real fuente/proveedor/perfil: 1 passed (23.8 s). BORRADOR activo excluido; publicado activo seleccionado y resuelto; proveedor desactivado excluido; documentos e historial publicados intactos. Revocar fuente deniega uso sin reescribir consumidores. |
| Diagnóstico de referencias históricas | --provider-diagnostics: 1 passed (17.4 s); publicación inactiva con fuente revocada conserva referencias y muestra aviso, sin escrituras. |

Los tiempos y resultados anteriores proceden de salidas aportadas por el usuario. Detalles: [carreras y autorización](doc110-continuacion-20261024.md), [guard ampliado](doc110-guard-proveedores-ampliado-20261027.md), [metadatos](doc110-descubrimiento-metadatos-20261027.md), [ciclo de consumidores](doc110-fuentes-aceptacion-preparada-20261026.md) y [diagnóstico](doc110-fuente-referencias-guardadas-20261027.md).

## Comprobación local adicional

`doc110-provider-concurrency.test.cjs` añade una ejecución donde la primera sesión se retrasa y gana la segunda. Exige que se conserve el contrato concurrent_b del ganador, se completen las cuatro carreras y la desactivación final deje el proveedor inactivo. Esto comprueba el verificador y evita suponer que siempre gana la sesión inicial; es transporte simulado, no una nueva carrera MySQL.

Los casos existentes detectan doble éxito, doble incremento, reescritura de la primera publicación y alteración de versión al activar. También comprueban desactivación tras fallo de aserción o fallo simulado de transporte posterior al commit. El arnés .NET ejecuta las defensas productivas de estado, facultad y ambas revisiones, con repositorios dobles. El verificador de historial detecta pérdida de referencias, alteración histórica o proveedor inactivo seleccionable.

```powershell
node --test tools/e2e/tests/doc110-provider-concurrency.test.cjs tools/e2e/tests/doc110-provider-history.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs
```

**8 pruebas PASS**. Una compila y ejecuta los grupos .NET Framework. La corrida local no acredita transacciones reales; esas carreras se sostienen en las corridas autenticadas identificadas arriba.

Se completa 14.30 con esta matriz acumulada. No se generaliza a todas las intercalaciones, fallo real de RA, caducidad temporal, revocación externa de permiso o aislamiento empresarial real. 15.19 sigue pendiente porque no existe evidencia del proveedor previo fuera del prefijo; no se crea uno ni se habilitan excepciones para fabricar PASS. Las pruebas transaccionales de fuentes y migración/control SELECT conservan sus tareas abiertas. Sin DDL, cambios SII, permisos ni nuevos accesos a DA.
