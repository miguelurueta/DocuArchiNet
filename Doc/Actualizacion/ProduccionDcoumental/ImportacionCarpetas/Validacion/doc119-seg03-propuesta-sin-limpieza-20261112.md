# DOC-119 — propuesta de operación sin limpieza

Estado: **propuesta pendiente de decisión expresa**. Origen: D-04/RQ-04 y tarea2.6. No acredita ejecución de SEG-03 ni habilita borrados.

## Decisión preparada

Operar DOC-119 manteniendo deshabilitada la limpieza de originales. Conservar originales, partes, copias de trabajo, diarios e historial. No añadir endpoint, botón, job, plazo ni configuración de limpieza. Una política y cualquier borrado futuros requieren una decisión separada con alcance y autorización propios.

Responsable de la decisión: el usuario solicitante de esta sesión, si confirma expresamente esta propuesta. La ausencia de respuesta no se considera aceptación.

## Consumo observado

Medición de solo metadatos sobre `App_Data/ImportacionCarpetas` de la carpeta principal: **57 archivos, 24.371 bytes (23,8 KiB)**; incluye **35 originales .bin y 2 partes .part**. No se leyeron contenidos ni se eliminaron archivos. No se presenta este corte como inventario global de gabinetes, temporales o documentos de otros módulos.

El consumo puede crecer mientras se conserven nuevos originales/partes. Esta propuesta no introduce cuota, garantía de capacidad, monitor automático o retención temporal; su límite operativo es continuar sin limpieza y conservar los datos hasta acordar otra política. No se afirma espacio libre del volumen porque la lectura disponible no lo proporcionó.

La composición actual mantiene `PoliticaLimpiezaOriginalImportacion(False, TimeSpan.Zero)` por defecto. La resolución propuesta documenta esa operación; no cambia el mecanismo ni ejecuta el servicio interno.

## Condiciones de cierre de esta decisión

Con confirmación expresa se puede completar2.6 como decisión de operar sin limpieza, conservando SEG-03 como no ejecutado. Paginación y recuperación continúan pendientes por sus propios criterios; esta decisión no abre la compuerta2.7 ni aprueba reorganización UI.
