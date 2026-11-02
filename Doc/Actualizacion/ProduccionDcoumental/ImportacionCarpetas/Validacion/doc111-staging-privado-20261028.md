# DOC-111 — Staging privado y copia consumible

Fecha: 2026-10-28. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios locales previos. Se completan 8.3, 8.4, 8.5 y 8.11 con implementación y evidencia local: **66/130** tareas. No se acredita recepción integrada o aceptación real.

## Implementación

`Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb` implementa el puerto existente IAlmacenStagingImportacion. Se vincula mediante copias independientes a lote/archivo de servidor y comprueba identidad, empresa, nivel, sello, recurso y fase del intento. La raíz se deriva exclusivamente de la raíz de aplicación proporcionada por servidor: App_Data/ImportacionCarpetas. El nombre retenido es opaco y por intento, independiente del nombre/ruta local del usuario.

Comprueba contención y componentes sin reparse points, nombres ambiguos/dispositivos/ADS de Windows, y no admite una ruta cliente para staging o copia. La referencia deriva de contexto/recurso/intento/token; no expone el token de reclamación. El retorno solo contiene referencia interna, bytes y hash, sin rutas físicas.

Recibir usa una parte creada con CreateNew, bloques de 4096 bytes como máximo y SHA-256 incremental. No requiere seek, Length o una copia completa en memoria del stream entrante. Lee como máximo un byte adicional a los previstos para detectar exceso y no escribe ese byte en la parte. Truncamiento, exceso, límite de perfil o formato inválido impiden promover a completo. Se comprueban cuota de partes/completos retenidos del lote y espacio disponible. Un bloqueo de archivo exclusivo serializa operaciones sobre el mismo almacenamiento físico del lote; no sustituye la reclamación persistida del repositorio ni una exclusión documental distribuida.

El validador reutiliza la referencia itextshare/itextsharp.dll ya declarada en el proyecto. Admite PDF 1.0–1.7 con cabecera/final coherentes, lectura parcial del parser, árbol de páginas y presupuestos positivos explícitos de páginas/objetos xref. Rechaza cifrado, reconstrucción reparadora y otros formatos, incluido TIFF/Multi-TIFF. Los presupuestos se reciben por constructor sin defaults productivos. No se instalaron dependencias ni se configuró el ambiente. Es validación estructural, no análisis antimalware ni verificación de almacenamiento definitivo.

Verificar reabre el completo, vuelve a comprobar tamaño/hash/formato y contrasta referencia/bytes/hash de una fila RECIBIDO cuando existe. Staging perdido o alterado deja RESULTADO_INCIERTO; una parte aislada nunca se considera completa. El componente no modifica filas, intenta promover partes durante reconciliación ni cambia estados documentales.

CrearCopiaTrabajo exige evidencia RECIBIDO, comprueba el original y produce una copia en directorio opaco independiente conservando el nombre exacto. El retorno es interno para el futuro adaptador, no un endpoint de descarga. Borrar/consumir la copia no borra el único contenido retenido. No se renombra para resolver incompatibilidades físicas de nombre/ruta.

## Verificación local

| Comando | Resultado y alcance |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS. Compila y ejecuta código VB real en .NET Framework; conserva familias DOC-110 y DOC-111. Incorpora pruebas locales de staging con streams sin seek y PDFs sintéticos generados por iTextSharp. |
| `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:OutputPath=.tmp/doc111-staging-build/bin/ /p:IntermediateOutputPath=.tmp/doc111-staging-build/obj/ /nologo /verbosity:quiet /consoleLoggerParameters:ErrorsOnly /fileLogger /fileLoggerParameters:LogFile=.tmp/doc111-staging-build.log` | PASS, exit 0. Proyecto/configuración reales, salidas separadas, sin despliegue o reemplazo de DLL del sitio. El proyecto conserva advertencias legacy. |

Casos ejecutados:

- Identidad o archivo ajenos: rechazo antes de leer contenido o crear staging.
- Stream sin seek/Length, entrega en fragmentos pequeños, bytes exactos y SHA-256 iguales al contenido sintético.
- Nombre retenido opaco y distinto del nombre original/token; verificación del completo antes de registrar recepción.
- Repetición del mismo intento con completo o parte previa: rechazo antes de consumir bytes, sin overwrite/replay.
- Truncamiento y exceso: no se crea completo; consumo limitado a previstos más un byte.
- Falso PDF, sobre PDF sin estructura, PDF cifrado, exceso de páginas y de objetos xref: rechazo.
- Nombres reservados, ADS, traversal y sufijos ambiguos: rechazo; TIFF no soportado se rechaza antes de leer.
- Cuota ocupada por partes/completos: rechazo antes de consumir el stream.
- Error de lectura: conserva parte y devuelve código saneado sin detalles internos; la parte aislada deja incertidumbre.
- Bloqueo de archivo desde otro handle: no obtiene acceso concurrente al lote.
- Completo ausente/alterado o hash persistido discordante: RESULTADO_INCIERTO.
- Copia con nombre original Unicode/espacios y bytes idénticos; eliminarla conserva el original verificable.
- Presupuesto PDF ausente/no positivo: capacidad denegada.

Los archivos existen solo bajo un directorio temporal sintético único de la prueba, comprobado antes de eliminarlo. No se leen ni transfieren archivos reales del usuario. No se ejecutó un escenario HTTP/E2E, carga sobre un ambiente de aplicación ni operación contra MySQL.

## Límites e integración pendiente

8.6–8.10 siguen abiertas: obtener archivo esperado del lote autorizado, reservar intento de recepción, confirmar bytes/hash/evento en MySQL, reenvío por contenido, handler multipart y reconciliación de disco/fila. No se conectó el puerto al servicio o a un endpoint ni se declaró RECIBIDO en la base. La pertenencia persistida archivo → raíz → lote y la autoridad vigente corresponden a esa composición; el staging recibe esos recursos exclusivamente de servidor.

La cuota implementada cuenta partes/completos de recepción dentro del lote y comprueba disco antes de escribir. No representa una reserva global entre todos los lotes ni cuotas de copias por todo el servidor. Las copias y las partes se conservan hasta mantenimiento con política habilitante; no hay borrado automático o rollback documental. El ciclo de vida de copias y la retención/limpieza siguen pendientes de coordinador/mantenimiento.

Reparse points se rechazan en el código, incluidos componentes existentes antes de crear y abrir; no se ejecutó una prueba que alterara enlaces/junctions del sistema ni ACL del sitio. La protección de App_Data en el host, permisos físicos, fallo de disco/rename real, cruce de procesos/hosts y comparación con MySQL requieren validación de integración autorizada. La comprobación de espacio no constituye una reserva de capacidad.

PDF validado aquí no acredita el adaptador de almacenamiento/incorporación. Otros formatos y el recorrido Multi-TIFF siguen sin habilitar. No se modificaron SII, carga individual, Web.config, límites IIS, permisos de cuentas o perfiles. No se ejecutaron REC-01/02, efectos documentales ni SQL de aplicación/control. La validación real de este corte permanece **NO_EJECUTADO**.
