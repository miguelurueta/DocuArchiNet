# DOC-124 — Plantilla Órdenes2025 y máximo20000

Solicitud expresa del usuario2026-11-23: crear la plantilla del archivo aportado y
aumentar a20000 el número de filas. Resultado: plantilla27 creada y leída de vuelta;
máximo20000 configurado para Excel en Web.config, sin override local que lo reduzca.
El máximo de archivo sigue10 MB y el lector sin configuración conserva su default10000.

## Plantilla creada

| Propiedad | Valor |
|---|---|
| ID / revisión / estado | 27 / 1 / activa |
| Nombre / conexión existente | imp_02_ordenes_2025 / 6 |
| Hoja / formato / intención | Hoja1 / XLSX / CREAR |
| Clave | NROORDEN, VARCHAR(32) |
| Campos | NOMBRE VARCHAR(128), NIT VARCHAR(32), NROORDEN VARCHAR(32), FECHARECEP DATE, FECHAENTREGA DATE, VIN VARCHAR(32), PLACA VARCHAR(16) |

Alta administrativa autorizada, mediante MySqlPlantillaExcelRepository.Guardar del
proyecto y el recurso de módulo ya configurado. No se fabricó una sesión, identidad
o permiso de usuario. Se verificaron identidad del recurso, referencia6, ausencia
previa de nombre/tabla, motor de catálogos y metadatos físicos. Creó una cabecera y
siete mapeos; SELECT posterior comprobó IDs, revisión, estado, tipos y huella.
Los intentos previos con NUMERO_MATRIZ_EXPLITER nulo se rechazaron y revirtieron;
se usó0, como exige el catálogo existente. No se alteró su nulabilidad ni esquema.

La tabla física no se creó y no se importó ninguna orden. La plantilla representa
la intención CREAR; materializar la tabla y confirmar datos pertenecen a otra fase.
EGRESOS/CRÉDITOS y los proveedores/fuentes existentes no se modificaron. El
[SQL13](../Sql/13-plantilla-ordenes-2025-doc124.sql) documenta el alta equivalente:
ya está aplicada por el repositorio, no repetirla como instalación. No hubo DDL.

## Archivo y copia compatible

La fila1 del original es el título INFORME RESUMIDO DE ORDENES; los encabezados
NOMBRE/NIT/NROORDEN/FECHARECEP/FECHAENTREGA/VIN/PLACA están en fila2. Hay12759 órdenes
reales: el conteo anterior12760 incluía esos encabezados. No se elimina una orden.

Se generó una copia privada en
`.tmp/doc124-ordenes-2025/RELACION_ORDENES_2025_importacion.xlsx`, quitando sólo el
título y ajustando coordenadas/rangos y el filtro del libro. Preserva las demás
partes del XLSX y usa Hoja1 con encabezados en fila1, como requiere el lector actual.
El archivo original conserva su SHA256. Ambos quedan fuera de Git; no se publican
filas, NIT, nombres, VIN o placas en evidencia.

La copia pasó ServicioLecturaExcel DOC123 con la plantilla completa y máximo20000:
12759 filas normalizadas, lectura completa,0 incidencias,7 campos. NROORDEN es único
por comparación ordinal en esa lectura; esto no se presenta como collation MySQL
real acreditada. Es prueba local del lector/configuración, no E2E ASHX/ASMX/MySQL.

## Verificaciones

Se amplía la suite reutilizada: acepta20.000 filas completas y rechaza20.001 sin
exponer prefijo. El alcance DOC124 permite12759/20000 y rechaza20001. Se conservan
pruebas del default10000, límite10 MB y suites previas. Resultados y hashes en el
[JSON](doc124-ordenes-plantilla-20000-20261123.json). No cambió producto VB ni DLL IIS;
el cambio funcional del máximo usa la configuración que la DLL existente ya lee.

DOC124 conserva21/23 tareas: el alta administrativa y la lectura local no sustituyen
la E2E real pendiente2.6 ni la revisión final3.2. No se ejecutó autenticación HTTP,
recepción ASHX, Preparar ASMX, confirmación o importación de órdenes.
