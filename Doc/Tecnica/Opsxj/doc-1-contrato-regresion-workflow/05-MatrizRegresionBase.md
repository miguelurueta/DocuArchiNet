# Matriz de regresión base

## Perfil de ejecución requerido

| Elemento | Valor requerido |
| --- | --- |
| Línea base | Inventario y corte aprobados en JIRA-00, commit/versionado desplegado. |
| Navegador | Registrar versión del navegador objetivo; repetir en el estándar del cliente. |
| Cuentas | Una con permiso de transición y una sin permiso. |
| Datos | Sin tarea, tarea sin documento, tarea con título largo, lista extensa, documento válido y selección masiva. |
| Viewports | 1366, 1024, 768 y 375 px. |

| ID | Caso | Datos / cuenta | Resultado esperado | Evidencia |
| --- | --- | --- | --- | --- |
| R-01 | Carga sin tarea | Cuenta válida, sin seleccionar tarea | Centinelas intactos, no hay acción de negocio habilitada indebidamente. | Pendiente de ambiente. |
| R-02 | Selección de tarea | Tarea válida | Sesión y hidden sincronizados; lista relacionada se actualiza. | Pendiente de ambiente. |
| R-03 | Tarea sin documentos | Tarea sin documento | Mensaje/estado existente, visor no muestra un documento erróneo. | Pendiente de ambiente. |
| R-04 | Abrir documento | Documento válido | Descriptor y fila activa alimentan el visor e índice correctos. | Pendiente de ambiente. |
| R-05 | Documento de título largo | Documento con texto largo | Sin solapamiento ni pérdida de selección. | Pendiente de ambiente. |
| R-06 | Selección masiva | Varias filas con checkbox | Acción masiva no cambia el documento activo sin evento de apertura. | Pendiente de ambiente. |
| R-07 | Transición autorizada | Cuenta con permiso | Enviar, devolver, pendiente/cerrar conservan validaciones y resultado. | Pendiente de ambiente. |
| R-08 | Permiso restringido | Cuenta sin permiso | El servidor deniega o limita según la regla actual, sin fuga de acción. | Pendiente de ambiente. |
| R-09 | Tres postbacks parciales | Tarea + documento | Menú, lista, visor e índice quedan coherentes después de tres ciclos consecutivos. | Pendiente de ambiente. |
| R-10 | Cuatro viewports | Estados R-01, R-02, R-04 y menú abierto | Layout sin recorte crítico en 1366/1024/768/375 px. | Pendiente de ambiente. |

## Criterio de aprobación

Cada caso debe indicar resultado, fecha, ambiente, cuenta enmascarada, datos usados, versión/commit y archivos de captura. Un resultado pendiente no equivale a aprobado: DOC-1 deja la matriz preparada, pero su ejecución depende de JIRA-00 y del ambiente de pruebas.

