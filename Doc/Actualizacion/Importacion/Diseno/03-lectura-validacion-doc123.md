# DOC-123 — Lector XLS/XLSX y validación

Entrega de fase03 sobre DOC121/DOC122, .NET Framework4.6.1. El servicio interno lee un stream suministrado por servidor y devuelve datos normalizados o incidencias. La recepción, preparación persistente y confirmación se integran en fases posteriores.

## Uso y responsabilidad del consumidor

`ServicioLecturaExcel.Leer(stream, SolicitudLecturaExcel)` recibe formato esperado XLS/XLSX, nombre descriptivo con extensión, hoja exacta, snapshot `PlantillaExcel`, `DestinoPlantillaExcel` y `LimitesLecturaExcel`. El stream debe ser legible y seekable; permanece abierto y recupera su posición incluso ante rechazo. No se abre conexión ni archivo desde una ruta enviada por navegador o desde RUTA_ARCHIVO_FUENTE.

El consumidor resuelve sesión/usuario/empresa/permiso RA, conexión DA del módulo, plantilla/revisión/huella y esquema. Debe volver a comprobarlos al preparar/confirmar. El lector puro no acredita autoridad por sí mismo. Para CREAR, el consumidor aporta descriptor proyectado explícito de columnas, tipos, longitudes, charset y nulabilidad, además de los tipos creables de la plantilla; no se infieren longitudes ilimitadas. No hay DDL en el lector.

## Contrato de salida

`ResultadoLecturaExcel` expone formato real, hoja y catálogo de hojas, encabezados con columna física base1, filas con número físico, filas de datos observadas y estado `LecturaCompleta`. Cada `ValorNormalizadoExcel` indica campo origen/destino, columna, tipo semántico y `EsNulo`; utiliza Texto, NumeroCanonico o Fecha según tipo. Los números canónicos no se limitan a la capacidad Decimal de CLR y admiten validación de DECIMAL65.

Las incidencias contienen código estable, hoja, fila/columna cuando existen y nombres de mapeo. Se retienen como máximo1000 y se conserva `TotalIncidencias`/`IncidenciasTruncadas`. Una lectura interrumpida reporta solo lo observado. `Valido` exige lectura completa, al menos una fila y cero incidencias. Toda salida inválida vacía `Filas`; ningún prefijo se ofrece para escribir. No incluye excepción interna, credenciales, cadena de conexión o ruta física.

## Reglas y límites efectivos

Primera fila física como encabezado, coincidencia exacta por CAMPO_FUENTE y rechazo de nombre ausente/ambiguo. Mantener columnas vacías y primera/última fila. Omitir filas completamente vacías; espacios, cero, false, error o valor solo en columna no mapeada cuentan.

Límites recibidos del servidor: por defecto10000 filas no vacías y10MB decimales (10000000 bytes). Encabezado y vacías no cuentan; filas inválidas sí. Valores positivos y multiplicación comprobada; máximo exacto permitido y siguiente fila/byte rechazado. No se amplía configuración por disponer de un libro mayor. XLSX tiene presupuesto de expansión de40 veces el máximo de bytes configurado para inspección ZIP/XML. Ese presupuesto no equivale a una garantía medida de memoria total del parser.

Transformaciones explícitas: REMPLAZA_COMA=1 quita comas; luego REMPLAZA_PUNTO=1 quita puntos; luego OPCION_SPLITER=1 selecciona segmento literal base0 conservando vacíos. NULL/0 inactivas, bandera inválida o segmento inexistente genera incidencia. Sin limpieza general: preservar apóstrofe, letra n, barras, símbolos y espacios restantes.

Texto usa contenido original y formato visual significativo del valor tipado (123/00000→00123). Validar capacidad física vigente, incluso BENEFICIARIO ampliado. Reutilizar `PerfilImportacionCatalogAdapter.ValorCompatible` y completar longitud por escalares Unicode, utf8 de3bytes y utf8mb4, fechas y NULL sin cambiar el adaptador de carpetas. Charset admitido: ascii, latin1/cp1252, utf8/utf8mb3, utf8mb4; otros se diagnostican. No truncar ni sustituir caracteres.

ENTERO valida rango físico signed/unsigned; DECIMAL valida precisión/escala sin redondear. Texto numérico sigue signo/dígitos/punto decimal sin grupos o cultura implícita; opciones guardadas pueden quitar coma/punto antes. El valor numérico Excel se convierte de forma invariante, expandiendo exponente. No se recuperan dígitos que Excel ya perdió; códigos largos deben almacenarse como texto.

DATE acepta DateTime o texto exacto yyyy-MM-dd/dd/MM/yyyy, calendario válido y rango1000..9999; conserva fecha sin hora. FECHA VARCHAR sigue texto. NULL requiere nullable no clave. Clave mapeada siempre requerida, no vacía/solo espacios; no se suple con default. La comparación de claves y unicidad según MySQL pertenece a fase04.

## Fidelidad y dependencias

Paquetes fijados: ExcelDataReader3.6.0 net45 y ExcelNumberFormat1.1.0 net20. Referencias CopyLocal y archivos Compile registrados en vbproj. FastExcel y sus consumidores de SII permanecen en su implementación anterior.

La caracterización binaria reprodujo pérdida de errores/caché como null. `InspectorFidelidadExcel` observa metadatos de error/fórmula en XML y registros BIFF de Workbook/Book, con extracción CFB/FAT/miniFAT acotada y detección de ciclos. No reemplaza al lector de valores: ExcelDataReader sigue convirtiendo tipos y fechas, ExcelNumberFormat aporta formato visual. Sin API privada, fork o nueva dependencia.

Error Excel produce ERROR_CELDA_EXCEL con coordenadas. Fórmula sin caché produce FORMULA_SIN_RESULTADO y corta: en XLS la búsqueda de STRING del paquete puede consumir el siguiente registro. Se descarta toda fila parcial. Caché válida se lee sin recalcular, incluso vacío textual separado de NULL. No se ejecutan fórmulas, macros, Office o vínculos externos; un resultado guardado puede estar desactualizado. Fuentes oficiales versionadas se conservan en el diseño OpenSpec.

## Compatibilidad, SQL y operación

Conservar conexión DA del módulo y cuenta compartida aceptada en DOC122, autorización RA y huella v1. No añade ASMX/ASHX, UI, nuevas conexiones Web.config, SQL, permisos o despliegue. Sin cambios en EGRESOS/CONTRATOS, SII o importación de carpetas. No requiere script SQL adicional; si se necesita adecuación en una fase posterior, se entrega un archivo nuevo para ejecución manual y verificación SELECT tras confirmación.

El lector no ofrece pausa/reanudación, PREPARADA, confirmación, rollback MySQL o habilitación como fuente de índices. Fases04/05 deben asociar archivo inmutable/ID/hash, persistir estado y revalidar antes de escribir. Repetir lectura es determinista y no sustituye esa recuperación.

## Verificación y rollback

Reutiliza `tests/doc122-importacion-excel-behavior.test.cjs`, harness DOC110 y mismo ejecutable C#; pruebas del lector separadas en `tests/ImportacionExcelLecturaTests.cs` por legibilidad. Nueve libros sintéticos versionados y generador stdlib. Pruebas de contenido, errores/caché, FAT/mini-stream, fronteras, Unicode, culturas y muestra de incidencias. Evidencia en `../Validacion/doc123-validacion-local-20261122.md/json`.

MSBuild usa salida/intermedios propios en .tmp y no despliega. E2E funcional no aplica a esta fase sin recorrido o endpoint nuevo; tools/e2e queda disponible para integración posterior con autorización específica. Pruebas locales y lectura de archivos sintéticos no acreditan importación real ni rendimiento en producción.

Rollback: revertir referencias/Compile y archivos del lector y sus modelos internos preservando contratos DOC122. No necesita migración de base ni limpieza de datos.
