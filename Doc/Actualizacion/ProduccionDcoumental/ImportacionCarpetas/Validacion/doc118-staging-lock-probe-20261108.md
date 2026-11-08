# DOC-118 — RV-02-A: primitivas locales de exclusión del staging

La validación aislada solicitada con «continua» pasó13/13 pruebas. La corrida evidenciada compiló un fixture .NET Framework y usó6 procesos hijo locales, todos terminados. Archivo de resultado:`doc118-staging-lock-probe-20261108.json`. La regresión conjunta pasó205/205 en16 archivos; las13 comprobaciones nuevas se cuentan una vez, sin sumar repeticiones.

## Alcance probado

Solo archivos sintéticos bajo un directorio nuevo `.tmp/doc118-staging-lock-probe-<id>`, en un volumen fijo identificado como NTFS. El fixture usa los modos FileShare.Read y FileShare.None para contrastar lectores y exclusión; la guardia abre con FileMode.Open/FileAccess.Read y no crea un original ausente. Verifica identidad obtenida del handle, tamaño y hash y libera el handle si rechaza la prueba.

| Caso | Resultado observado |
| --- | --- |
| Lector activo en otro proceso | La apertura exclusiva se rechazó por sharing violation, sin espera de reintento. |
| Salida normal del lector | La misma identidad original admitió guardia y validación. |
| Terminación del proceso sintético que retenía el handle | Antes de salir bloqueaba; después de confirmar su salida se pudo abrir el mismo original. |
| Guardia exclusiva mientras arrancan otro lector y otro contendiente | Ambos rechazados. También se rechazó abrir para escritura. |
| Copia con los mismos bytes/hash | Rechazada por identidad nativa distinta. |
| Hardlink del original | Rechazado por el recuento de enlaces; no se adoptó el alias. |
| Reemplazo del mismo path conservando los bytes | Rechazado por identidad distinta. Se mantuvo el original anterior en el fixture para evitar confundir esta prueba con reutilización de IDs. |
| Original ausente | Rechazado y no creado. |
| Contenido cambiado | Rechazado; el handle quedó liberado y el rechazo no alteró los bytes. |
| Guardia ya cerrada | No pudo reutilizarse como prueba válida. |
| Lector posterior a liberar la guardia | Pudo abrir:control que demuestra que el lock aislado no garantiza fencing durable. |
| Ruta UNC no admitida | Rechazada antes de intentar apertura o acceso de red. |

La aserción13 valida los límites y metadatos de la corrida:prueba local, puerto de producto no implementado, proveedor de producción no calificado y cero efectos de negocio.

## Identidad y límites

Se consultan GetFileInformationByHandle y GetVolumeInformationW; el fixture compara volumen/índice de archivo/creación junto con longitud y SHA256 del contenido mientras conserva el handle. No publica nombres de máquina, rutas de staging de negocio, IDs nativos o contenido.

Microsoft limita la combinación de volumen e índice al mismo equipo y advierte que los IDs no son permanentes; el identificador de64 bits tampoco garantiza unicidad en ReFS. La implementación de esta prueba admite únicamente NTFS local y no convierte ese identificador en un certificado entre nodos o entre vidas distintas del archivo. [Identidad por handle](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/ns-fileapi-by_handle_file_information), [limitaciones por proveedor](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfileinformationbyhandle).

El caso de alias crea deliberadamente un hardlink dentro del fixture y comprueba que la guardia lo rechaza. No cambia permisos ni enlaces del repositorio o del staging real. [API de hardlinks](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createhardlinkw).

Los escenarios de copia, reemplazo y contenido modificado alteran únicamente archivos sintéticos para comprobar los rechazos. La guardia no elimina ni copia el original. La limpieza de las pruebas se restringe al directorio temporal recién creado y comprobado, después de confirmar la salida de todos sus procesos.

## Reproducción y resultado de la compuerta

`node --test tests/doc118-staging-lock-probe.test.cjs` compila y ejecuta el fixture, conserva un resultado cerrado en `.tmp/doc118-staging-lock-probe-latest.json` y limpia sus archivos propios tras cerrar los procesos. El runner PowerShell permite una corrida aislada con un ProbeId nuevo de32 caracteres hexadecimales. No necesita navegador, IDE, URL, cuenta de aplicación o credenciales SELECT.

RV-02-A queda completada como comprobación de primitivas locales. RV-02-B queda pendiente:integrar una guardia no destructiva en el proveedor real, fijar el origen V2 emitido por el servidor y acreditar confinamiento, compatibilidad y vida útil hasta commit/rollback. Un test sobre archivos del repositorio no acredita el proveedor de staging de producción.

No hubo logins de aplicación, HTTP, SELECT o escrituras de negocio. El acumulado sigue1276 HTTP/113 SELECT/9 aperturas, con127 SELECT y0 aperturas disponibles. Los intentos41/53/69 y los recursos del lote24 permanecen protegidos. El contrato vigente D-07 y la calificación legacy no cambian. Progreso activo17/27; el plan candidato se desglosó en9 entregables, con RV-02-A ejecutada fuera del mapa activo.
