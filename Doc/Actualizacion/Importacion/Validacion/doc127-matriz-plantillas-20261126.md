# DOC127: matriz de plantillas y creación web

**31/33 tareas.**5.2 completada:12 combinaciones locales de compatibilidad, seis usos reales por formato/modo y dos altas web actuales XLS/XLSX PASS. La historia XLSX conserva su SHA; el [resultado vigente](doc127-asistente-real-20261126.md) y el [registro de hallazgos](doc127-hallazgos-cierre-20261126.md) identifican niveles y restricciones.

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
| XLS, CREAR, imp_02_doc126_e2e_asistente_fab26227,1000 filas | REAL PASS; plantilla76/r1 | Carga 3671f38d-92d2-4605-8253-b6f317d8904a; Vista actual y huellas conservadas |
| XLSX, CREAR, imp_02_doc126_e2e_asistente_3d2f2ad4,1000 filas | REAL PASS; plantilla77/r1 | Carga 3e2d5b0b-902c-4a99-acaa-817ada56f24f; Vista actual y huellas conservadas |

La diferencia de Vista afecta el propietario de formulario y el manejo de foco/busy. La lectura actual ya verificó aislamiento de form1; esta propuesta verifica el recorrido de creación del asistente con esos cambios y con ambos formatos.

No se ejecuta creación/edición web nueva para AGREGAR/REEMPLAZAR en esta ventana. Su uso con plantillas existentes tiene evidencia real en la tabla anterior. Los cruces de creación web y uso de plantilla se mantienen separados; no se cuenta un caso de capacidad como alta web.

[Alcance ejecutado](doc127-ventana-asistente-20261126.md) y [resultado real](doc127-asistente-real-20261126.md).5.2 completada al reunir los dos resultados actuales y esta matriz; los cruces de edición web fuera de alcance siguen identificados. EXPLAIN manual y gates/QA finales continúan pendientes.
