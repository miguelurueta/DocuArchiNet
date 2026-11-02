# DOC-111: falso conflicto al confirmar una publicación equivalente

## Evidencia real del fallo

La sesión 09 creó el plan **3** y recibió HTTP **409 / CONFLICTO_REVISION** de `ConfirmarPlan`. Se detuvo sin repetir la operación. SELECT mediante el repositorio existente confirmó: estado REVISABLE, revisión 1, publicación 38, nivel 7, preflight confirmable, cero lotes asociados y cero expedientes con los dos códigos/alias propuestos.

El plan se creó a las 23:26:21 UTC y vencía a las 23:36:21 UTC. La confirmación falló a las 23:26:21 UTC, por lo que ese primer rechazo no fue por vencimiento. No se conservó la clave aleatoria privada del comando ni se intentó reconstruirla para reenviarlo.

## Causa comprobada

`MySqlImportacionCarpetasRepository.PrepararPersistencia` ordena las reglas del snapshot por DestinoTipo/DestinoRef. La publicación leída del repositorio conserva su orden de lectura. `ServicioRecepcionImportacion.Confirmar` comparaba sus JSON completos como texto: el orden distinto de los elementos del array provocaba conflicto aun siendo la misma publicación.

La lectura real del plan y la publicación mostró diferencias exclusivamente en posiciones del array de reglas. Con la comparación canónica corregida, esos dos snapshots son iguales; la comparación textual/estructural sin normalizar sigue dando falso. Reporte saneado de flags, fechas y rutas de propiedades, sin contenidos: `tools/e2e/artifacts/doc111-plan3-readonly.json`.

## Corrección

`PerfilImportacionJson.MismaPublicacion` compara copias tipadas con las reglas ordenadas por los mismos criterios ordinales del repositorio. Conserva la comparación de todos los campos: identidad de perfil/versión, número, huella, configuración y contenido completo de cada regla, incluido su atributo Orden. La comparación textual final es ordinal, por lo que distingue cambios de mayúsculas. No muta ninguno de los snapshots ni modifica huellas publicadas, huellas de plan o datos existentes. El servicio usa esta comparación antes de confirmar.

No se omitieron controles de autoridad, revisión, huella, fecha de vencimiento, coincidencias o transacción. No se alteró el perfil 24/versión 38, el plan 3, sus reglas ni almacenamiento legacy/SII.

## Verificación y despliegue

- Arnés .NET Framework y coherencia DDL: 2/2 pruebas Node PASS; la regresión prepara un plan por el método real de normalización, conserva la publicación en otro orden y confirma con persistencia doble. También rechaza cambios reales de regla, mayúsculas, Orden, configuración, identidad, número y huella; comprueba que las entradas no se muten y que el rechazo anteceda al escritor.
- Build Debug aislado: cero errores, 308 advertencias existentes. Salida `.tmp/doc111-confirmacion-orden-build/bin/`.
- Comprobación SELECT con plan 3 y método corregido: `snapshotCanonicalEqual=true`; cero lotes asociados. Sin reenvío de confirmación.
- [Manifiesto de 110 archivos](doc111-sha-confirmacion-20261030.json): SHA-256 agregado `08a1ce806dbab144243de782c8b921d960c80c8a637856aa66d60ee1abbae939`; los 27 archivos protegidos coinciden con HEAD normalizando finales de línea. Workspace sin commit; HEAD solo no identifica el código actual.
- Despliegue en el mismo sitio de pruebas previamente autorizado a las 23:36:03 UTC. DLL activa SHA-256 `b5718d01665c4cb1d539e5ef7db8813d3cd37470e756a419979d26cb6ccdb55f`. Copia anterior `.tmp/doc111-confirmacion-deployment-20261030-233603/previous.dll`, SHA-256 `c85547f134de48f08eaccbba6e41ca477f8ff660a5f340af2dffa552dd8fcca9`.
- Arranque básico a las 23:36:46 UTC: login y Producción HTTP 200, formulario presente, sin indicadores de error ASP.NET. GET sin autenticar; no acredita importación. Reporte `tools/e2e/artifacts/doc111-confirmacion-deployment.json`.

## Plan conservado y continuación

A las 23:36:54 UTC el plan 3 seguía REVISABLE/revisión 1 y sin lote, pero ya vencido por su fecha original. `PuedeConfirmar=true` es el resultado histórico del preflight y no evita el control de vencimiento. No se resucitó el plan, se amplió su plazo ni se modificó su estado en BD.

Tras comprobar el resultado sin efectos documentales y corregir la causa, se abrió la sesión 10 para una **preparación nueva** del mismo escenario LOTE y recursos autorizados. Estado `tools/e2e/artifacts/doc111-lote-authorized-session-10.json`. No es recuperación de una confirmación incierta ni reutilización del plan vencido. El perfil pasa `--check`; el plan anterior se conserva como evidencia. La aceptación REC permanece pendiente y no se marca PASS por las pruebas locales o el despliegue. Progreso 119/130.
