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


## Captura y primer intento de autenticación

Se abrió la consola local solicitada y se reutilizó la entrada enmascarada del arnés. Una primera activación se detuvo por un enlace local desactualizado antes de login/HTTP nuevos; se corrigió y verificó antes de reabrir. El siguiente intento llegó al servidor, que devolvió DOC111_LOGIN_CREDENTIALS_REJECTED. Se cerraron contextos/navegador y se limpiaron los secretos; no se probó otra combinación automáticamente.

Consumo acumulado: C02=98/250 HTTP totales, incluyendo el HEAD previo; C03=0/250;1 intento/contexto de los4 autorizados;0 SELECT de control y0 efectos documentales. No se conoce qué dato de entrada causó el rechazo. Evidencia saneada: doc118-login-preflight-20261107.json.

La propuesta execution-continuation-proposal.json conserva el techo global de500 HTTP y4 intentos/contextos, reutiliza una pareja independiente para ambos casos y redistribuye los topes a425 C02/75 C03. No está autorizada ni aplicada. Todos los límites documentales y SELECT permanecen iguales. Se requiere corregir la entrada en una captura nueva y decidir sobre esa continuación; no enviar secretos por chat.
