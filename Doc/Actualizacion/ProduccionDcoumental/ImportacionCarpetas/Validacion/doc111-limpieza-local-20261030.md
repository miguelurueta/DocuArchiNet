# DOC-111 — Limpieza controlada del original privado

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios previos. Tarea 11.7 implementada y probada localmente; 119/130. Sin despliegue, limpieza de datos del sistema, SQL real o efectos documentales reales.

## Alcance y política

La capacidad interna `ServicioConsultaImportacion.LimpiarOriginal` recibe lote/archivo y sus dos revisiones esperadas. No hay WebMethod, botón, runner de borrado o job. Consultar/exportar/diagnosticar continúan sin invocarla; el diagnóstico mantiene eliminación deshabilitada y un candidato nunca concede autoridad para borrar.

La política servidor `PoliticaLimpiezaOriginalImportacion` queda deshabilitada por defecto. La composición interna exige `Doc111.Limpieza.Habilitada=true` y `Doc111.Limpieza.RetencionOriginalSegundos` entero positivo explícito. Ausencia, formato inválido, deshabilitación o retención no calculable impiden ejecutar. No se cambió Web.config ni se decidió un período operativo para el usuario.

Cada llamada reconstruye identidad/empresa/destino y exige autoridad vigente sobre nivel/gabinete con la acción LIMPIAR_STAGING, usando los permisos nativos de creación y carga del nivel además de IMPORTAR_CARPETAS. La composición fija los orígenes RA/DA y comprueba que no hayan cambiado. No concede administración ni añade un permiso/columna paralelo.

Solo se consideran lotes COMPLETADO con inventario original completo, raíces/archivos terminados y checkpoints de creación/documento concordantes, sin actividad, incertidumbre o reclamaciones huérfanas. Se reutiliza el cálculo puro de consolidación. Rechazados, parciales, cancelados, pendientes o efectos sin prueba conservan el original. Las fechas deben ser UTC verificables; se aplica la retención a la más reciente entre actualización del lote y finalización de sus intentos confirmados. Vencer una fecha anterior no acorta la retención de un checkpoint reciente.

## Comprobación y retiro

`PrepararComprobacionFinal` construye un comando marcado exclusivamente para lectura desde el archivo COMPLETADO/intento CONFIRMADO finalizado. Conserva estos estados; no fabrica una reserva ejecutable. Los inicios/resultados ordinarios y binding documental rechazan esa marca. La verificación reutiliza el catálogo y lector SELECT de almacenamiento para gabinete, documento, inventario, índice, nombres, formato, campos efectivos y relación/clasificación del expediente. No ejecuta proveedores, creación, almacenamiento o incorporación.

`AlmacenStagingImportacion.AbrirRetiro` reutiliza rutas privadas, contención, comprobación de enlaces, tamaño/hash/PDF y el mutex de recepción del lote. Retiene en exclusiva el original; un lector que ya lo conserve impide el retiro. La comprobación actual de autoridad, política y documento se repite antes de registrar y de eliminar.

Se retira únicamente el `.bin` exacto de la referencia verificada. No hay enumeración/borrado recursivo, ni borrado de directorios, `.part`, copias de trabajo, otros archivos, metadatos, intentos o historia. Identidades, cantidades originales, señal de detención, referencias/hash y valores fijados permanecen intactos. La política implementada se limita a bytes originales de operaciones completas; no habilita otra retención ni poda general de historial/huérfanos.

## Diario durable y recuperación

El parcial Limpieza pertenece al repositorio y tablas existentes. Reutiliza la carga completa de consolidación bajo bloqueo del lote y sus recursos; no añade tablas o fases ejecutoras. Solo inserta eventos específicos en `ra_integracion_evento`:

1. PREPARAR_LIMPIEZA/LIMPIEZA_PREPARADA: primera transacción, autorización ligada a lote/archivo, huella del inventario completo, retención exacta y fecha UTC. No se borra antes de que esta llamada confirme el commit.
2. CONCLUIR_LIMPIEZA: segunda transacción. Vuelve a leer/comparar vista y diario, comprobar la retención y reautorizar/verificar relaciones. Solo entonces invoca el retiro exacto y registra STAGING_ELIMINADO o STAGING_AUSENTE_AUTORIZADO.

No se promete atomicidad entre disco y MySQL. Fallo de commit/respuesta después del borrado conserva el evento preparado y devuelve LIMPIEZA_NO_CONFIRMADA/INCIERTO, sin reintento automático. Otra llamada explícita exige la misma evidencia/política y autoridad actuales: si el original ya falta con autorización durable, puede registrar ausencia autorizada; no afirma causalidad del borrado anterior. Si falta sin diario previo, devuelve CONSERVADO/ORIGINAL_NO_VERIFICABLE sin atribuirlo a limpieza.

Una conclusión ya registrada permite repetición sin abrir staging, borrar de nuevo o insertar más eventos, conservando las comprobaciones vigentes. Cambio de revisión, identidad, evidencia, política o permiso impide usar el permiso anterior para otro recurso/estado. Un diario ambiguo/duplicado o de otro actor se rechaza. La referencia preparada no expira para conceder otro efecto.

Los eventos no modifican la fecha/revisión operativa del lote ni reinician su retención. El resultado público interno contiene solo IDs, estado/código y repetición, sin rutas, hashes, autorizaciones internas o errores privados. Consultas e histórico documental siguen funcionando tras retirar los bytes; el replay documental devuelve el checkpoint sin acceder al staging o invocar otro writer. RECIBIDO conserva su sentido histórico y no promete disponibilidad indefinida de los bytes privados.

## Verificación local

- PASS: arnés Framework completo `node --test tests/doc110-perfil-importacion-behavior.test.cjs`, incluyendo familia de limpieza con PDF sintético local y persistencia/lectores SQL dobles. Reutiliza las regresiones DOC-110/DOC-111.
- PASS: política ausente/inválida, retención vigente, fecha inválida/futura, checkpoint más reciente, lote parcial/incierto, prueba terminal ausente, reclamación huérfana y revisión obsoleta conservan recursos.
- PASS: documento no verificable y revocación antes/después de registrar preparación; deshabilitación o ampliación de retención posterior no permiten borrar.
- PASS: fallo y respuesta perdida de preparación; fallo y respuesta perdida de conclusión; recuperación de ausencia autorizada y repetición sin nuevo borrado/evento. Excepciones privadas no se proyectan.
- PASS: original alterado y original retenido por lector se conservan; otro `.part` permanece; solo se retira el `.bin` propio. Metadatos/IDs/totales permanecen iguales y el histórico documental no repite writer.
- PASS: comandos de comprobación final rechazados por inicios/checkpoints ejecutores antes de abrir conexión; no se expone endpoint de limpieza.
- PASS: MSBuild Debug completo, salida/intermedios `.tmp/doc111-limpieza-build/`, log `.tmp/doc111-limpieza-build.log`, sin sustituir la DLL del sitio.
- PASS: registro de los cuatro parciales nuevos en vbproj/arnés, OpenSpec estricto y `git diff --check`. Funciones legacy/SII protegidas sin diferencias.

Los borrados físicos de prueba son exclusivamente de archivos sintéticos regenerables creados por las fixtures locales. No se abrió MySQL ni se ejecutó limpieza sobre una aplicación/usuario/lote real. Commit/rollback/carreras MySQL, habilitación de política operativa, HTTP/IIS autenticado, aceptación real y limpieza real: **NO_EJECUTADO**. No se heredan autorizaciones de corridas anteriores.

La exclusión adicional del XML frente a otras cargas permanece descartada por el usuario. El mutex reutilizado pertenece al staging privado del lote; no se presenta como exclusión de escritores externos del almacenamiento legacy.
