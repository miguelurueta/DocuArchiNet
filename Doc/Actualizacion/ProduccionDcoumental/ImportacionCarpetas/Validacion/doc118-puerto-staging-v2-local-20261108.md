# DOC-118 — puerto de staging V2, RV-02-B

Implementada6.2/D-12/RQ-12 tras «continua» posterior a RV-01 y al siguiente paso explícito RV-02-B. El mapa se amplió antes de modificar código y queda19/29:dos tareas locales nuevas completadas respecto de las27 originales; las10 tareas reales/finales anteriores siguen pendientes.

El proveedor real `AlmacenStagingImportacion` incorpora un parcial y una interfaz interna adicional. Ofrece retención compartida del original y guardia exclusiva no destructiva. Abre solo archivos/directorios existentes, usa el lock breve existente del lote y conserva handles del original/ancestros hasta Dispose. No reutiliza AbrirRetiro ni modifica recepción, limpieza o interfaces V1. No se conecta a HTTP, adaptador, repositorio o recuperación.

La adquisición comprueba Windows, NTFS fijo local, ausencia de reparse points, ruta final del handle, un único enlace, identidad física nativa, bytes/hash y vínculos de contexto/recurso. Original ausente, contenido/identidad distintos, hardlink/junction o lock inexistente se rechazan y liberan lo adquirido. No crea originales, locks, copias o directorios, ni borra archivos. Los errores son saneados.

Una época aleatoria por AppDomain evita acreditar otros ejecutores mediante nombre de equipo o hash. **Otra instancia, otro nodo o un reinicio mantienen bloqueo**, incluso con contenido idéntico. La identidad nativa no es permanente. Esto limita deliberadamente el puerto; no resuelve recuperación tras caída completa del proceso ni acredita un despliegue distribuido.

ConservarDurante protege una acción síncrona frente a Dispose concurrente/reentrante y valida antes/después. El futuro consumidor debe mantener commit/rollback dentro de la sección; el puerto no conoce SQL, no concede autorización de invocación ni decide elegibilidad. Origen SERVIDOR y JSON válido siguen siendo datos. La disciplina de retención del adaptador y la procedencia/diario/fencing durable corresponden a RV-03/RV-04/RV-05.

Verificación ejecutada:

- Harness/cliente existentes:18/18 pruebas Node, sin fallos; compilan VB/C# y ejecutan la regresión V1.
- Dentro del harness:21 casos nuevos del proveedor real y los19 casos de contratos RV-01. No se suman al contador Node.
- Pruebas del puerto:lector V1/otro proceso, lector tardío, escritura/borrado/renombrado bloqueados, Dispose, sección concurrente, ausencia/reemplazo/hash/ámbito, enlaces, lock existente, época de otro proceso y privacidad. El renombrado de directorios tiene controles positivos antes/después de adquirir la guardia.
-3 procesos sintéticos cerrados:2 lectores del mismo ejecutable de pruebas y1 helper de junction reutilizando la técnica existente. El hijo lector usa TTL de5 segundos y ambiente mínimo. No se usó IIS ni se detuvieron procesos de aplicación.
- Herramientas locales205/205. Total Node disjunto actual223/223.
- OpenSpec estricto, refinement y diff se verifican sobre la entrega.12 fuentes históricas siguen idénticas y2 archivos de prueba conservan deltas antes/después;10 reportes históricos y16 fuentes de la evaluación permanecen intactos.

No hubo sesiones, HTTP de negocio, SELECT o efectos documentales nuevos. Consumo real conservado1276 HTTP/113 SELECT/9 aperturas; intentos41/53/69 intactos. No hay DLL/PDB, migraciones, permisos, workers o configuración IIS cambiados. RV-02-B está validada localmente; producción y recuperación siguen sin activar/calificar. Sigue RV-03.

Referencias de las primitivas usadas:[CreateFileW](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew), [GetFileInformationByHandle](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfileinformationbyhandle), [GetFinalPathNameByHandleW](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfinalpathnamebyhandlew). Evidencia estructurada:[JSON local](doc118-puerto-staging-v2-local-20261108.json).
