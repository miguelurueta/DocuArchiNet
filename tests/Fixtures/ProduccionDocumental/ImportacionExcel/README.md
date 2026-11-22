# Libros sintéticos DOC-123

`generar.py` usa exclusivamente la biblioteca estándar de Python para generar BIFF8/CFB y OpenXML. Los libros están versionados; el harness C#/Node los lee sin requerir Python, Office, ACE, conexión o autenticación.

Regenerar: `python tests/Fixtures/ProduccionDocumental/ImportacionExcel/generar.py`. El ZIP tiene fecha fija y `sha256.json` identifica los nueve libros. No contienen información de negocio. El generador admite directorio de salida y número de filas como argumentos para escenarios locales mayores; no modifica el límite del producto.

| Libro | Expectativa independiente |
|---|---|
| valido.xls / valido.xlsx | Hojas Datos/Otra. Datos físicos2/4/5; columnaB vacía. Código123 con formato00000→00123, código textual00007; apóstrofe/n/barras/ñ y espacios conservados. INT signed, DECIMAL, DATE y FECHA textual. |
| fidelidad.xls / fidelidad.xlsx | HojaCasos. B3 error, B4 caché numérica42, B5 fórmula con error, B6 fórmula sin resultado utilizable, B7 fórmula con vacío textual y B8 NULL real. El paquete original oculta B3/B5/B6 como null. El adaptador informa errores en3/5, interrumpe en6 y descarta filas. |
| cache-valido.xls / cache-valido.xlsx | Casos físicos2/4/7/8 sin errores: ceros de formato, caché42, vacío textual y NULL distinguibles; cuatro filas válidas con Valor nullable. |
| variantes -mini.xls | Mismos casos con CFB mini-stream/miniFAT en lugar de FAT regular. |

`tests/ImportacionExcelLecturaTests.cs` aporta expectativas de contenido, crea libros XLSX sintéticos adicionales para fronteras/culturas/reportes y comprueba rechazo de una cadena FAT cíclica. Ningún test escribe SQL ni carga datos de negocio. La compilación y el ejecutable se reutilizan de DOC122/DOC110.
