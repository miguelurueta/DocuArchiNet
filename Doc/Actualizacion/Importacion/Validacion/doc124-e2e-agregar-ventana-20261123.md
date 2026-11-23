# DOC124: ventana E2E AGREGAR sobre CONTRATOS

Ventana nueva `e3a3f1afc41c46ba85dfffabb537e3e1`, preparada después de la
instrucción «continua» y de aplicar/verificar SQL15/16. Su ejecución requiere
confirmación específica en la consola privada y credenciales efímeras. No se
reabre la ventana cerrada de Órdenes ni se recuperan sus secretos.

Ambiente local ya verificado, empresa2, DA11, conexión6, fuente5/revisión2 y
plantilla28/revisión1/AGREGAR, Hoja1, NCONTRATO y cuatro campos VARCHAR(250).
Backend servido e1767f58a8d784bc0f958b8c141efad7363669a793eb0eb5c164aefed071e730,
conservado desde el despliegue anterior. Esta ventana no despliega otro binario.

| Caso | Filas | Resultado exigido |
|---|---|---|
| Claves nuevas |8000 | OK/PREPARADA; dos sesiones concurrentes, un ganador y un OPERACION_EN_CURSO. |
| Duplicado interno |2 | CLAVES_INVALIDAS/CLAVE_DUPLICADA_ARCHIVO, RECIBIDA. Claves con diferencia de mayúsculas y espacio final, preservadas por el lector. |
| Clave existente |1 | CLAVES_INVALIDAS/CLAVE_EXISTENTE_DESTINO, RECIBIDA. Se conserva privadamente una clave obtenida por SELECT, sin nombres ni NIT reales. |

Se reutiliza el generador Book del harness DOC123 mediante el ejecutable existente,
ajustando los cuatro encabezados y el nombre de hoja. Los tres libros se validaron
con ServicioLecturaExcel real: lectura completa y sin incidencias de formato/tipo.
Esa validación local no afirma que se haya comparado la collation en MySQL.
El prefijo sintético no tenía coincidencias al consultar el destino; la ventana
lo comprueba otra vez antes de enviar los archivos.

La ejecución usa el mismo runner DOC110 `--excel-add`, suite importacion-excel,
autenticación, safePost y controles SELECT DOC32. Las cuentas se capturan en TTY:
propietario141, segunda sesión independiente del mismo propietario, cuenta
restringida y cuenta MySQL de lectura. Capturas, trazas y video desactivados.

Presupuesto:60 solicitudes de servicios Excel,80 controles SELECT externos y20min
desde el ingreso de claves, sin reintentos automáticos. Efectos máximos: tres IDs
nuevos, tres archivos privados completos, dos recibos idénticos por ID y temporales
de preparación. Cero importación/confirmación de datos, DDL permanente, guardado
de plantillas, permisos, activación de fuentes, SQL17 o limpieza.

Huellas completas antes/después de plantillas/campos, catálogo de fuentes/historial
y EGRESOS/CRÉDITOS/CONTRATOS. La suite conserva sus controles de esquema/datos por
caso, recuperación del recibo, CSRF, acceso restringido y consulta de resultado.
Tras la prueba se verifican por SELECT las tres ejecuciones y sus archivos; el
documento efectivo válido debe corresponder a empresa2/DA11/plantilla28 y20000 filas.
No guardar filas de muestra, claves, credenciales, cookies ni solicitudes en el informe.

Preparación local:18/18 focales y scope/archivos íntegros aprobados, sin login ni
E2E ejecutada a este corte. Plan/libros bajo `.tmp/doc124-real/<ventana>/`, excluidos
de Git. El resultado real, cuando exista, se documentará por separado. Esta fase
no sustituye REEMPLAZAR ni completa por sí sola2.6/3.2;21/23 tareas.

Resultado posterior: la ventana inicial se detuvo en el control del propietario,
con1 SELECT y0 solicitudes Excel. Se cerró y eliminó claves. [Parada y continuación
preparada](doc124-e2e-agregar-parada-20261123.md): nueva589a3040, cuenta resuelta por
ID141 y presupuesto previo conservado, sin repetir automáticamente la ventana.
