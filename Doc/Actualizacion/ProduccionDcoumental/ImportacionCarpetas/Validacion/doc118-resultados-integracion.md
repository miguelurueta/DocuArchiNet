# DOC-118 — autorización y preflight real

La persona responsable autorizó expresamente la fase real mediante «autorizado», después de revisar el plan de ambiente local, usuario141/empresa2, nivel7/versión38, TLS local, observador externo y límites de recursos. La autorización está registrada en `openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/execution-authorization.json`. No se vuelve a solicitar esa autorización dentro del alcance concedido.

## Resultado observado

Se realizó una petición HEAD a gestor.aspx del ambiente autorizado: HTTP200. La DLL ordinaria y su PDB están presentes; sus huellas figuran en doc118-preflight-20261107.json. Esa presencia no acredita correspondencia de símbolos cargados ni observación del cuerpo de la llamada.

La aplicación usa DefaultAppPool, compartido por seis aplicaciones. Se localizó la instancia de Visual Studio de la carpeta principal en modo diseño, sin depuración activa. Se consultaron configuración/metadatos por lectura, sin attach, breakpoints, congelación de hilos ni cambios de IIS. La lectura automatizada de la API SDK quedó comprobada mediante un helper C# y se identificó el motor administrado .NET Framework local. Attach, breakpoints, Freeze/Thaw y observación interna siguen sin ejecutar ni calificar.

## Presupuesto consumido

| Concepto | Consumo |
| --- | --- |
| HTTP total C02 | 1 de250; HEAD de preparación, cero HTTP de negocio |
| HTTP total C03 | 0 de250 |
| Intentos de login | 0 |
| Controles SELECT | 0 de240 |
| Planes/lotes/raíces/archivos/expedientes/documentos nuevos | 0 |
| Detenciones y mutaciones históricas | 0 |

No hay secretos efímeros de aplicación ni MySQL SELECT en el proceso. Se solicitó elegir entre consola local de captura enmascarada y consola existente; aún no hay respuesta. No se probó un login vacío ni se buscaron credenciales en archivos. La captura de datos de acceso es una dependencia, no una nueva autorización.

## Estado de los casos

COORD02 y COORD03 continúan NO_EJECUTADOS para su aceptación específica. El sitio accesible y la autorización no acreditan exclusión documental, detención en vuelo ni recuperación. Los intentos41/53 siguen intactos. Conservar el HEAD consumido al continuar y calificar la observación antes de iniciar un caso documental.
