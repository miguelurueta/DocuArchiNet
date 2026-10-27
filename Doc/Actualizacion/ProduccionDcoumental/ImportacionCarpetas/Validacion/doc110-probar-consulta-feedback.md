# DOC-110 — Resultado visible de Probar consulta

Fecha: 2026-10-23. Trazabilidad: D-09/RQ-12, tarea 15.21.

## Diagnóstico

La grabación 135851 muestra la parte inferior del proveedor publicado, clave 44 y clics en Probar consulta sin feedback visible. No contiene captura HTTP: no permite afirmar cuál fue la respuesta exacta de esa petición.

Revisión de ProveedorAdminEditor.js: el estado se escribía en una cabecera fuera de pantalla. Solo las propiedades devueltas se mostraban junto al botón; AMBIGUO y SIN_COINCIDENCIA no tienen propiedades, por lo que esa zona permanecía vacía. Además, un error HTTP podía dejar valores de un ensayo anterior visibles.

SELECT parametrizado de solo lectura en DA: COUNT(*) de imp_02_egresos con CONSECUTIVO=@clave, clave textual 44, devuelve 2. No se imprimieron registros, NIT, beneficiarios ni conexiones. MySqlProveedorDatosIndices usa LIMIT 2 y devuelve AMBIGUO cuando hay más de una coincidencia. Es el resultado esperado para esa fuente/clave si supera las validaciones previas; no se ha reproducido la petición autenticada del video.

## Corrección

- Estado del proveedor visible mediante posición sticky y región accesible.
- Explicación de OK, AMBIGUO, SIN_COINCIDENCIA y fallos definidos, junto al botón y en la cabecera.
- Limpieza de resultados anteriores antes de probar; valores solo para OK.
- Clave vacía rechazada localmente, sin petición; la clave válida se envía intacta.
- No se cambia la unicidad requerida, no se elige una fila arbitraria ni se altera la fuente, el SQL o las versiones publicadas.

## Verificación

- Node: 4 pruebas del editor de proveedores PASS.
- Playwright existente: doc110-perfil-guided.spec.cjs, 2 PASS, transporte simulado. Incluye proveedor publicado, viewport 1000x600, ubicación visible del aviso, consulta correcta, ambigüedad, ausencia, error HTTP sin valores obsoletos y clave vacía sin solicitud.
- Fixture corregido para devolver el código real OK, en lugar de ENCONTRADO.
- Aceptación HTTP autenticada pendiente. Sin despliegue, DDL ni escrituras en las tablas de negocio.
