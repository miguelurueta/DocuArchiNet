# DOC-111: lote 2 detenido antes del almacenamiento documental

La sesión 10 confirmó **plan 4 / lote 2** y terminó a las 23:39:58 UTC con HTTP 409/RESULTADO_INCIERTO. No se repitió el comando ni se creó otro lote. Evidencias SELECT saneadas: `tools/e2e/artifacts/doc111-lot2-initial-readonly.json` y `doc111-lot2-readonly.json`.

| Recurso | Estado comprobado |
| --- | --- |
| Lote 2 | EN_CURSO, revisión 13, sin detención solicitada |
| Raíz 3 / r1 | Expediente 648, relación 1073, EXPEDIENTE_CONFIRMADO |
| Raíz 4 / r2 | Expediente 649, relación 1074, EXPEDIENTE_CONFIRMADO |
| Archivos 3 y 4 | Ambos RECIBIDO, 677 bytes esperados/recibidos cada uno |
| Archivo 3 | Valores materializados; efecto EN_CURSO, revisión 5, intento activo 5 |
| Archivo 4 | Efecto NO_INICIADO, revisión 3 |
| Intento 5 | ALMACENAR / RESERVADO, sin evidencia de resultado |

Los eventos acreditan ambas recepciones y creaciones, materialización del archivo 3 y reserva del intento 5. No hay evento de inicio documental de ese intento. SELECT adicionales a las 23:52:24 UTC devolvieron cero documentos para expedientes 648/649 en gabinete físico, inventario de producción e incorporación. Esto no se usa para liberar una reserva ni atribuir un resultado por nombre. El driver superó la barrera LOTE con un archivo pendiente antes de crear, pero **REC LOTE no es PASS**: faltan almacenamiento, repetición, reapertura y verificación final.

## Causas y corrección del adaptador nuevo

`DA_EXTENSION.ESTADO_NORMAL` del PDF es **-2**, un código firmado de formato utilizado en DBT. El adaptador lo trataba como clave positiva mediante FilaCreacion/EnteroLegacy. Ahora exige una única fila PDF del catálogo y un Int32 canónico firmado; conserva el valor leído sin fijar -2 como constante. Las claves de entidades siguen siendo positivas.

La columna automática **NOMBRE** no tiene índice visible en el gabinete. El adaptador la exigía en `detalle_gabienete.VISIBLE=1`. Ahora consulta tipo, longitud y charset físicos en information_schema para el gabinete ya autorizado y usa ValorCompatible. Los índices del usuario conservan su metadata/visibilidad; no se crea un índice NOMBRE ni se modifica el perfil.

La validación de solo lectura con datos reales pasa con ambas correcciones: comando, gabinete, tipología, catálogos únicos, datos documentales y clasificación del expediente. Se usó **solo en memoria** una copia del intento con marca de reserva nueva para validar el comando original: no se reclamó propiedad ni invocó escritor, inicio, checkpoint o reconciliación. Reporte `tools/e2e/artifacts/doc111-lot2-pre-effect-readonly.json`.

## Pruebas y despliegue

Arnés Framework y coherencia DDL: 2/2 pruebas Node PASS. Regresiones nuevas: formatos negativos/cero/extremos Int32, rechazo de texto no canónico y overflow, catálogo PDF ambiguo/extensión distinta; nombre automático sin índice visible, longitud insuficiente y conflictos de campos. La suite conserva autoridad, staging, barreras, checkpoints, incertidumbre y no repetición. Build Debug aislado: cero errores, 308 advertencias existentes.

[Manifiesto de 110 archivos](doc111-sha-formato-20261030.json), SHA-256 agregado `f47a8f4d070f4cf257a5fb69d18b71a4d668c19b2e6fbb146d1ec2d17ca88bb3`. Los 27 archivos protegidos coinciden con HEAD normalizando finales de línea. Almacenamiento legacy, SII y Web.config intactos.

DLL desplegada en el sitio previamente autorizado a las 23:50:41 UTC: SHA-256 `7d82c02c68ef04b786413332ae3b832521cbf68287e0e8fec774f928b3b5a977`. Copia anterior `.tmp/doc111-formato-deployment-20261030-235041/previous.dll`, SHA-256 `b5718d01665c4cb1d539e5ef7db8813d3cd37470e756a419979d26cb6ccdb55f`. Login y Producción HTTP 200 sin indicadores de error ASP.NET a las 23:51:21 UTC. Reporte `tools/e2e/artifacts/doc111-formato-deployment.json`. El despliegue no alteró estados del lote.

## Recuperación pendiente

La lectura de las 23:52:27 UTC conserva estados/revisiones del lote, raíces, archivos e intento. El intento 5 sigue RESERVADO y el archivo 3 EN_CURSO. No se marca falsamente RECHAZADO/CONFIRMADO, renueva lease o crea otra reclamación. La reconciliación existente confirma efectos con IDs/evidencia positivos; no convierte una reserva sin inicio en permiso para repetir. Falta resolver explícitamente esa reserva antes de continuar el **mismo lote**, conservando expedientes y originales. No se abrió otra consola de ejecución.

## Prerrequisitos acreditados

Se cierran solo 13.10/13.11: autorización expresa de esta corrida DOC-111 en chat/TTY y `MySqlImportacionCarpetasRepository.VerificarEsquema` PASS mediante SELECT en el destino autorizado. Ambiente HTTPS localhost/GestionDocumental-Docuarchi.net, módulo GESTOR, actor persistido 141/empresa 2, nivel 7, perfil 24/versión 38, modo LOTE; recursos descartables DOC111-20261030-LOTE-01/02, PDF de fixture existente, tipología 79 e índice 14/TEXTO/EEE. Se autorizaron confirmación, recepción, dos creaciones, almacenamiento/incorporación, barrera, idempotencia y controles SELECT. No se infiere autorización de DOC-110; no se guardan credenciales. Progreso **121/130**, sin aceptación final.
