# DOC127: matriz de plantillas y creación web

**29/33 tareas.** Matriz local de12 combinaciones PASS y seis usos reales actuales (dos formatos × tres modos) reutilizados de la capacidad10000. La creación web XLSX histórica se conserva con su SHA. Dos recorridos del asistente actual están preparados y no ejecutados;5.2 sigue abierta.

## Modelos y compatibilidad local

| Origen | Formato | Modo | Hoja / clave | Revisión | Resultado y nivel |
| --- | --- | --- | --- | --- | --- |
| HISTORICA | XLS | CREAR | Hoja ñ / CLAVE | 1→2 | PASS reutilizado; servicios VB, ADO sintético |
| HISTORICA | XLS | AGREGAR | Hoja ñ / CLAVE | 1→2 | PASS reutilizado; servicios VB, ADO sintético |
| HISTORICA | XLS | REEMPLAZAR | Hoja ñ / CLAVE | 1→2 | PASS reutilizado; servicios VB, ADO sintético |
| HISTORICA | XLSX | CREAR | Hoja ñ / CLAVE | 1→2 | PASS reutilizado; servicios VB, ADO sintético |
| HISTORICA | XLSX | AGREGAR | Hoja ñ / CLAVE | 1→2 | PASS reutilizado; servicios VB, ADO sintético |
| HISTORICA | XLSX | REEMPLAZAR | Hoja ñ / CLAVE | 1→2 | PASS reutilizado; servicios VB, ADO sintético |
| NUEVA | XLS | CREAR | Hoja ñ / CLAVE | 0→1 | PASS reutilizado; servicios VB, ADO sintético |
| NUEVA | XLS | AGREGAR | Hoja ñ / CLAVE | 0→1 | PASS reutilizado; servicios VB, ADO sintético |
| NUEVA | XLS | REEMPLAZAR | Hoja ñ / CLAVE | 0→1 | PASS reutilizado; servicios VB, ADO sintético |
| NUEVA | XLSX | CREAR | Hoja ñ / CLAVE | 0→1 | PASS reutilizado; servicios VB, ADO sintético |
| NUEVA | XLSX | AGREGAR | Hoja ñ / CLAVE | 0→1 | PASS reutilizado; servicios VB, ADO sintético |
| NUEVA | XLSX | REEMPLAZAR | Hoja ñ / CLAVE | 0→1 | PASS reutilizado; servicios VB, ADO sintético |

Estos casos comprueban hoja, clave, revisión, IDs, mapeos y compatibilidad de metadatos históricos. No equivalen a12 altas web ni a12 importaciones reales. Ocho hashes de fuentes siguen coincidiendo con la evidencia doc127-pruebas-preparadas-20261125.json.

## Uso real de plantillas existentes

| Formato | Modo | Plantilla/revisión | Destino | Resultado |
| --- | --- | --- | --- | --- |
| XLS | AGREGAR | 61/r1 | imp_02_doc127_e2e_agregar_xls | PASS real reutilizado;10000 filas |
| XLS | REEMPLAZAR | 62/r1 | imp_02_doc127_e2e_reemplazar_xls | PASS real reutilizado;10000 filas |
| XLS | CREAR | 65/r1 | imp_02_doc127_e2e_crear_xls_10000 | PASS real reutilizado;10000 filas |
| XLSX | AGREGAR | 58/r1 | imp_02_doc126_e2e_agregar | PASS real reutilizado;10000 filas |
| XLSX | REEMPLAZAR | 59/r1 | imp_02_doc126_e2e_reemplazar | PASS real reutilizado;10000 filas |
| XLSX | CREAR | 68/r1 | imp_02_doc127_e2e_crear_xlsx_10000 | PASS real reutilizado;10000 filas |

Los seis casos usan Datos/CLAVE. Conservan los IDs de carga, SHA y evidencia por caso en el JSON. Procedencia de las plantillas: SQL previo confirmado; no se presentan como creadas por el asistente web. Las otras12 celdas de capacidad permanecen en el informe existente: no se repiten cargas.

## Creación desde el asistente

| Caso | Resultado | Vigencia |
| --- | --- | --- |
| XLSX, CREAR, plantilla60,1000 filas | Real histórico PASS DOC126 | Backend/API/CSS coincidentes; Vista y soporte cambiaron después. No es ejecución actual |
| XLS, CREAR, imp_02_doc126_e2e_asistente_fab26227,1000 filas | NO_EJECUTADO | Propuesta actual pendiente de autorización |
| XLSX, CREAR, imp_02_doc126_e2e_asistente_3d2f2ad4,1000 filas | NO_EJECUTADO | Propuesta actual pendiente de autorización |

La diferencia de Vista afecta el propietario de formulario y el manejo de foco/busy. La lectura actual ya verificó aislamiento de form1; esta propuesta verifica el recorrido de creación del asistente con esos cambios y con ambos formatos.

No se ejecuta creación/edición web nueva para AGREGAR/REEMPLAZAR en esta ventana. Su uso con plantillas existentes tiene evidencia real en la tabla anterior. Los cruces de creación web y uso de plantilla se mantienen separados; no se cuenta un caso de capacidad como alta web.

[Alcance revisable](doc127-ventana-asistente-20261126.md) y JSON. La tarea5.2 se revisará al reunir los resultados del asistente y esta matriz; no se marca completa por preparar archivos. EXPLAIN manual y cierre integral continúan pendientes.
