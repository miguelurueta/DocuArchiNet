# DOC-117 ALM-03C: coexistencia lista para revisión

La preparación real está **PASS**: plan 39/lote 23, expedientes 668/669, cero documentos y **339/410 SELECT acumulados**. El ejecutor y la candidata de coexistencia están preparados localmente; **la coexistencia real no se ha ejecutado ni autorizado**. DOC-117 conserva 16/18 tareas, con 17.1 y 18.1 abiertas.

La validación local final reúne **355/355 pruebas PASS**, compilación completa sin errores fuera de bin, comprobación `--check` y OpenSpec estricto PASS. El ensayo de restauración con los mismos bytes de candidata pasó en un sitio temporal: guardián independiente tras la salida del propietario, dos respaldos idénticos y transferencia de ACL verificada. No implica una instalación ni una prueba autenticada en el sitio real.

| Concepto | Alcance concreto propuesto |
| --- | --- |
| Ambiente y cuenta | `https://localhost/GestionDocumental-Docuarchi.net/`, usuario 141/empresa 2; dos sesiones independientes de esa cuenta |
| Importación A | Lote 23, raíz 37/archivo 38, expediente 668/relación 1093, nivel 7, versión 38, gabinete 10, tipología 79 e índice 14 TEXTO `EEE` |
| Importación B | Mismo lote, raíz 38/archivo 39, expediente 669/relación 1094, mismo contexto; envío único posterior a los controles de la pareja |
| Individual | Expediente 467/nivel 8/relación 1072/tipología 50; archivo `DOC117-ALM03C-IND-20261106.pdf`, 677 bytes, nombre canónico exacto del catálogo fijado en el setup |
| Efectos máximos | Tres solicitudes documentales y tres documentos: importación A, individual y, solo si sus controles pasan, importación B |
| Nuevos recursos de preparación | Cero planes, lotes, recepciones de importación, expedientes o materializaciones; se usan los dos originales ya recibidos del lote 23 |
| Runtime | Candidata temporal por máximo 600 segundos, doble respaldo, guardián independiente, restauración byte a byte y reapertura con DLL original |
| Control SQL | Cuenta exclusivamente SELECT, máximo 320 solicitudes adicionales desde 339; techo acumulado propuesto 659 |
| Preservación | Todos los recursos previos y nuevos, originales de staging y documentos individuales anteriores; ningún replay, retry, reparación, reconciliación o limpieza |

El techo vigente es **410**. Esta propuesta requiere ampliar expresamente **410 → 659**; las 320 consultas son un máximo adicional y se cargan desde 339, incluidas las solicitudes fallidas. El ejecutor reserva cada consulta antes de acceder a ODBC, rechaza contadores inconsistentes y no permite comenzar otra corrida si ya existe evidencia de ejecución del caso. No se ha creado una concesión real de coexistencia; las banderas públicas de ejecución, instalación y ampliación permanecen en `false`.

La UI oficial debe recibir primero una cabecera de preparación vinculada al SHA del alcance activo y reconocer el valor canónico exacto antes de seleccionar el archivo. Las guardas nativas vinculan los comandos inmutables, contexto, destinos, contenidos, intentos y dos sesiones comparadas en RAM. Se conserva la comparación exacta del nombre entre preparación y entrada; no se normalizan los argumentos del escritor.

A e individual se despachan juntos mediante coordinación del cliente. No se introducen esperas ni barreras entre los escritores del servidor. Se espera la resolución de ambos envíos incluso si uno falla; sus resultados separados y códigos cerrados se conservan. B solo se envía después de confirmar la pareja, sus checkpoints, relaciones e incorporación física. Cualquier fallo o incertidumbre detiene el caso sin volver a enviar. El registro nativo de rechazo, separado de las marcas del método, conserva únicamente protocolo, caso, huella del alcance y un código cerrado; no guarda mensajes, valores del catálogo, sesiones ni secretos.

Tras restaurar y reabrir el runtime original se comprueban preservación, conteos por los tres expedientes, inventario de usuario 141/empresa 2, originales y evidencia física por IDs conocidos. Si hubo un resultado parcial se registra lo observado sin buscar candidatos por nombre ni inferir IDs ausentes. Una verificación parcial no se convierte en aceptación de negocio o concurrencia.

La ruta productiva de `ALMACENAR` confirma archivo e intento, pero conserva el lote `EN_CURSO` y las raíces `EXPEDIENTE_CONFIRMADO`; no ejecuta la consolidación del agregado. El control final exige ese comportamiento, seis intentos confirmados y terminados, ningún intento activo de raíz y los dos checkpoints documentales. No se añade una mutación de cierre. Esta distinción se verificó en `CoordinadorImportacionCarpetas.Documento.vb` y `MySqlImportacionCarpetasRepository.Documento.vb`.

El PASS de coexistencia exige tres incorporaciones completas, preservación posterior, restauración y solapamiento efectivo dentro del método instrumentado. Si los métodos se serializan, se declara **NO_ACREDITADO** aunque los tres documentos estén correctos, sin repetir el caso. La observación corresponde a la copia de laboratorio; no acredita commits simultáneos, rendimiento ordinario ni concurrencia XML/SII.

La DLL instalada, sus cuatro respaldos históricos, las 912 entradas originales de proyecto/compilación/recursos y la evidencia real anterior permanecen intactos. A del caso detenido sigue documentado como 16224/23322/intento 58 confirmado y B/archivo 37 sin iniciar; no se repiten. El diagnóstico posterior demostró un rechazo del literal de descripción frente al catálogo, sin recuperar retrospectivamente el detalle individual perdido de la primera pareja.

Evidencia revisable:

- Preparación real: [informe](doc117-alm03c-setup-pass-20261106.md) y [certificado](doc117-alm03c-setup-pass-20261106.json).
- [Propuesta de coexistencia](doc117-alm03c-coexistencia-propuesto-20261106.json), SHA `4c8e7bc3c03164178249e62e24fc5742699758d044c062468a192871b5e8fe03`.
- Candidata SHA `dc45234fb9ad479e3dc4313b3c06be6bd550d65d5731c5eff7a1acdcf07d95a5`; compilación `.tmp/doc117-alm03c-scoped-reviewed-build-20261106/build-result.json`.
- Restauración aislada `.tmp/doc117-alm03-lease-8ef09398fc83eab71b9881dc638f239a/rehearsal-result.json`.
- Regresión final `.tmp/doc117-alm03c-coexistence-final-regression-20261106.tap` y lista de 41 archivos de prueba asociada.
- [Certificado local de revisión](doc117-alm03c-coexistencia-local-20261106.json), con las huellas del ejecutor, fuentes revisadas, alcance, pruebas e invariancia histórica.

La aprobación de setup cubrió cero documentos y cero cambios de DLL. La ejecución propuesta necesita autorización propia de sus tres documentos, runtime y ampliación del techo; [AGENTS.md](../../../../../AGENTS.md) exige: «No ejecutar E2E real ni carga sin autorización explícita para el ambiente y las cuentas de prueba».
