# Prompts de implementación de la importación Excel

Los siete archivos describen fases consecutivas del mismo cambio funcional. Cada prompt contiene contexto, dependencias, rutas de las capas, restricciones del alcance, criterios de aceptación, pruebas, documentación y entregable. El prompt 01 formaliza el cambio OpenSpec y sus contratos. Las fases siguientes implementan y verifican la capacidad definida en la exploración.

## Archivos SQL, ejecución manual y revisión posterior

Cada prompt exige entregar las modificaciones SQL como archivos nuevos en Doc/Actualizacion/Importacion/Sql/, con destino RA/DA, orden, precondiciones, efectos previstos y comprobaciones SELECT. El usuario ejecuta manualmente los archivos y confirma el resultado. Después el agente revisa mediante consultas exclusivamente SELECT y registra la evidencia saneada. Una diferencia que requiera SQL produce otro archivo para ejecución manual.

El agente no aplica los cambios SQL ni permite que los tests o scripts de arranque los apliquen automáticamente. La instalación permanece PENDIENTE_EJECUCION_MANUAL hasta la confirmación del usuario y requiere revisión posterior para acreditarse. El trabajo local independiente puede continuar mientras se espera esa ejecución.

La referencia funcional y arquitectónica es [Carga de Excel en tablas de índices](carga-excel-csv-tablas-imp-02.md). Su nombre de archivo conserva la referencia histórica a CSV, que quedó aplazado. La primera entrega comprende XLS y XLSX.

| Orden | Prompt | Depende de | Resultado esperado | Revisión |
|---|---|---|---|---|
| 01 | [Esquema, contratos y configuración](01-esquema-contratos-y-configuracion.md) | Exploración | Cambio OpenSpec, SQL revisable y contratos. | [PASS](Revision/01-esquema-contratos-y-configuracion.review.json) |
| 02 | [Autorización, conexión y plantillas](02-autorizacion-conexion-y-plantillas.md) | 01 | Contexto empresarial, cuenta dedicada y servicios de plantillas. | [PASS](Revision/02-autorizacion-conexion-y-plantillas.review.json) |
| 03 | [Lectura y validación de Excel](03-lectura-y-validacion-excel.md) | 01 y 02 | Lectura de XLS/XLSX y diagnósticos de datos. | [PASS](Revision/03-lectura-y-validacion-excel.review.json) |
| 04 | [Recepción y preparación](04-recepcion-y-preparacion.md) | 02 y 03 | Archivo identificado, vista previa y carga preparada. | [PASS](Revision/04-recepcion-y-preparacion.review.json) |
| 05 | [Confirmación y recuperación](05-confirmacion-y-recuperacion.md) | 04 | Tres modos, transacción, idempotencia y recuperación. | [PASS](Revision/05-confirmacion-y-recuperacion.review.json) |
| 06 | [Interfaz y menú](06-interfaz-y-menu.md) | 02 a 05 | Recorrido desde el menú con permiso propio y teclado. | [PASS](Revision/06-interfaz-y-menu.review.json) |
| 07 | [Verificación integral y cierre](07-verificacion-integral-y-cierre.md) | 01 a 06 | Aceptación, capacidad medida y guía operativa y de despliegue. | [PASS](Revision/07-verificacion-integral-y-cierre.review.json) |

## Ejecución de la revisión

Se ejecutó opsxj:prompt-review para cada archivo con el perfil legacy-webforms-vb. Desde la raíz del repositorio, el comando reproducible para el primero es el siguiente.

```powershell
npm.cmd --prefix tools/opsxj run opsxj:prompt-review -- "Doc/Actualizacion/Importacion/Exploracion/01-esquema-contratos-y-configuracion.md" --tech-profile legacy-webforms-vb --no-fix
```

Para revisar otro prompt, sustituir la ruta por su archivo. La opción no-fix conserva el contenido durante la comprobación. El revisor escribe su informe nativo en .opsxj/reports/prompt-review-report.json. Después de cada ejecución se conservó una copia individual en Revision/.

Los siete resultados finales tienen cero bloqueantes, cero hallazgos mayores y cero menores. Cada informe incluye una observación informativa que recomienda revisión manual. La coherencia de los prompts se contrastó con las decisiones de la exploración, incluidas las dependencias, el permiso exclusivo del cargador y la recuperación del resultado histórico de una ejecución confirmada.

El [resultado consolidado](Revision/resultado-prompt-review.json) incluye el comando, las fechas, el perfil y SHA-256 de cada prompt. Los hashes se calculan sobre el texto UTF-8 con finales de línea LF, para comprobar el contenido revisado entre checkouts de Git con LF o CRLF. El resultado corresponde a la revisión estructural de los prompts. La aceptación del producto se acreditará con las verificaciones previstas en cada fase.
