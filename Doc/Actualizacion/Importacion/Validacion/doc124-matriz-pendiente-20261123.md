# DOC-124: matriz de aceptación y antecedentes de preparación

Estado final23/23 tareas. La matriz real está completa: CREAR1/1, AGREGAR3/3 y REEMPLAZAR2/2 casos.
[Resultado REEMPLAZAR](doc124-e2e-reemplazar-resultado-20261123.md) y
[revisión final](doc124-verificacion-final-20261123.md). Los cortes de preparación
inferiores se conservan como antecedentes; no autorizan repetir ventanas cerradas.
Este documento no autoriza otra ventana ni modifica la configuración del ambiente.

## Recursos y condición pendiente

Se reutilizan `importacion-excel.spec.cjs`, `--excel-prepare`, las sesiones del
runner DOC110 y los controles SELECT DOC32. No se necesita otra conexión en
Web.config ni crear tablas de negocio. El recorrido de Órdenes conserva la
plantilla27, límite20000 y carga PREPARADA existentes.

El inventario SELECT previo muestra que `imp_02_contratos` dispone de
`NCONTRATO VARCHAR(250) NOT NULL`, latin1_swedish_ci y UNIQUE completo
`ux_doc121_NCONTRATO`. Los otros campos son FECHA, NIT y NOMBRE, también
VARCHAR(250); FECHA debe tratarse como texto. No repetir el SQL08.
El mismo inventario no encontró una política de fuente para CONTRATOS ni una
plantilla Excel compatible. Es un antecedente, no una verificación nueva de la base.

La investigación posterior solicitada por el usuario resolvió el ámbito:
[empresa2 comprobada](doc124-empresa-contratos-verificada-20261123.md), por decisión
previa de empresa única y12 SELECT actuales de empresas/módulos/destino.
CONTRATOS conserva625 filas,0 claves nulas/vacías y0 grupos duplicados. No hay
otra empresa configurada ni módulos de otra empresa compartiendo ese destino.
Se eligió política CONEXION para empresa2 por esa evidencia, no por la ausencia
de columna empresarial. Su alta/activación posterior fue autorizada expresamente
y quedó verificada como fuente5. No alterar los registros para hacer pasar la prueba.

Se entrega [SQL14](../Sql/14-preflight-contratos-e2e-doc124.sql), solo SELECT,
para comprobar RA/DA y preparar la configuración exacta. No se ha ejecutado.
El aislamiento configurado está confirmado; cualquier alta o adecuación se entrega en otro SQL
manual, con actor/motivo/historial de fuente cuando corresponda. No activar una
fuente ni publicar un proveedor desde la E2E. La configuración concreta ya se
entregó en SQL15/16/17, [con orden y transacciones manuales](../Sql/README.md#configuración-manual-de-contratos-para-doc124);
el usuario autorizó después «aplica los archivos por mi» y se aplicaron/verificaron
15/16: [fuente5 y plantilla28](doc124-sql15-16-aplicados-20261123.md), AGREGAR/revisión1.
17 sigue pendiente hasta cerrar AGREGAR. No repetir las altas ya aplicadas.

## Casos que debe acreditar el recorrido

| Caso | Destino y contenido | Resultado esperado |
|---|---|---|
| CREAR válido | Plantilla27, copia compatible de Órdenes,12759 filas | OK/PREPARADA; tabla física sigue ausente. Ya acreditado individualmente. |
| AGREGAR válido | CONTRATOS,8000 claves nuevas contrastadas por MySQL | PASS real, PREPARADA; ninguna fila insertada. |
| AGREGAR con duplicado interno | Dos claves iguales según tipo/collation físicos | PASS real, CLAVES_INVALIDAS y CLAVE_DUPLICADA_ARCHIVO; RECIBIDA. |
| AGREGAR con clave existente | Clave tomada por SELECT privado del destino autorizado | PASS real, CLAVES_INVALIDAS y CLAVE_EXISTENTE_DESTINO; RECIBIDA. |
| REEMPLAZAR válido |8000 filas incluyendo coincidencia existente, sin duplicados internos | PASS real, PREPARADA; se admite coincidencia, sin sustituir datos. |
| REEMPLAZAR con duplicado interno | Dos claves equivalentes en el archivo | PASS real, CLAVES_INVALIDAS y CLAVE_DUPLICADA_ARCHIVO; RECIBIDA. |

Las claves existentes se usan solo en archivos/evidencia privados excluidos de Git.
Los casos de igualdad deben incluir mayúsculas/espacios cuando sean compatibles
con la collation observada. No dar por acreditada esa semántica con un doble local.

La plantilla controla el modo mediante flags históricos:0/0/1 para AGREGAR,
0/1/0 para REEMPLAZAR. Preparar no permite cambiar esa intención silenciosamente.
Se usan ventanas separadas con cambio manual explícito de modo y nueva revisión/huella:
`--excel-add` exige escenario agregar con al menos tres casos (válido, duplicado
interno y existente); `--excel-replace` exige escenario reemplazar con al menos
dos (válido y duplicado interno). Ambos exigen destino protegido en lectura,
una sola identidad/revisión/huella de plantilla por ventana y un caso válido de
al menos8000 filas para probar contención. El arnés común conserva permisos,
recuperación, CSRF, dos sesiones y huellas antes/después. Los flags de matriz y
archivo original conservan sus alcances; no aceptan estas fases como matriz completa.
No crear dos cabeceras con igual nombre sin revisar los consumidores históricos:
`Gestion/Class_imp01_plantillaimp.vb` también busca plantillas por nombre.

Cada caso usa un ID de carga nuevo y comprueba recepción, recuperación, rechazo
CSRF, estado y huellas completas de destino antes/después. La concurrencia usa
dos sesiones independientes y el caso válido de más de8000 filas. No reutilizar
el ID de Órdenes ni reabrir la ventana cerrada para completar la matriz.

## Revisión parcial anterior de requisitos

Se contrastaron los contratos con `ServicioPreparacionExcel`, el validador MySQL,
el adaptador de política, el repositorio de plantillas y la suite E2E existente.
Las verificaciones iniciales de esta revisión fueron locales. La investigación
posterior del ámbito añadió12 SELECT reales, documentados por separado; no hubo
login web ni otra ejecución E2E.

| Requisitos | Evidencia disponible y límite |
|---|---|
| RQ01–03: autoridad, archivo e identidad | Harness nativo y recorrido real de Órdenes: permisos, CSRF, recuperación/replay e integridad. |
| RQ04: aislamiento | Selección explícita de política cubierta localmente. Falta política verificada para el destino de la matriz. |
| RQ05: lectura/límites | Harness de fronteras y archivo real12759 filas con máximo20000. El PASS real no es una prueba de carga de20000 filas. |
| RQ06–07: esquema/duplicados | Harness con dobles; CREAR real. Falta acreditar los modos existentes y la igualdad del destino en MySQL real. |
| RQ08–10: huellas, estados, consulta | Harness y comprobación real de documento, PREPARADA, consulta y ganador concurrente en Órdenes. |
| RQ11: recursos | Harness de limpieza/fallos; lock real libre después de Órdenes. No se afirma inspección global del pool. |
| RQ12: SQL manual | SQL14 entregado, sin ejecutar. SQL históricos conservados; no se han aplicado cambios permanentes en esta continuación. |
| RQ13: regresión | Mismo harness DOC122/DOC110,246+318+490=1054 comprobaciones nativas sintéticas; no son MySQL/HTTP reales. |
| RQ14: E2E | PASS individual de Órdenes. Matriz completa pendiente; no sustituirla por una simulación. |

La revisión final3.2 depende de completar2.6 y sus compuertas de evidencia.
No cerrar ni archivar DOC124 con este preflight. El resultado real anterior y
sus límites permanecen en [el informe de Órdenes](doc124-e2e-ordenes-resultado-20261123.md).

Verificación de esta continuación, base Git `52c55cd8fde61b58ffec6533d6ec7729d60f3f07`:
`node --test tests/doc122-importacion-excel-behavior.test.cjs tests/doc122-e2e-reuse.test.cjs tests/doc124-e2e-reuse.test.cjs`
aprobó17/17; el ejecutable nativo aprobó1054 comprobaciones. Después de incorporar
SQL14 se repitieron solo las suites focales afectadas,16/16. OpenSpec estricto y
`git diff --check` aprobados. La nueva consulta queda cubierta por la comprobación
existente que exige exclusivamente SELECT en los preflights; no se presenta esa
prueba como ejecución SQL real.

Preparación posterior de configuración:18/18 pruebas focales aprobadas y seis
casos descubiertos por Playwright, sin autenticación. No se modificó producto VB
ni se desplegó una DLL por incorporar las fases. SQL15 conserva el formato canónico
del documento de fuente y las revisiones ALTA/ACTIVAR del repositorio existente;
SQL16/17 conservan los flags, mapeos y revisión de la plantilla. Esta revisión de
archivos no sustituye su ejecución manual ni la comprobación posterior en MySQL5.1.
Los dos documentos JSON de historial, con ID sintético123, fueron deserializados
y recanonizados por PerfilImportacionJson/DocumentoFuenteImportacion reales y
validados por ReglasFuenteImportacion:2/2 compatibles, sin SQL ni HTTP.
OpenSpec estricto, auditoría de refinamiento15/15 y diff sin errores completan la
validación de esta preparación, sobre base Git2e6ad4b1. No se repitió el build VB
ni las1054 comprobaciones nativas de producto porque ese código no cambió.
