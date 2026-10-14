# DOC-104 — bloqueo de fixture firmado

## Estado

`ACCEPTED_WITH_RESIDUAL_RISK` desde 2026-10-14 por decisión explícita del responsable.

La implementación y las verificaciones locales están completas. El caso real de un documento descartable registrado como firmado no se ejecutó porque el ambiente autorizado no dispone de ese dato de prueba. Para finalizar DOC-104, el responsable acepta expresamente ese riesgo residual. No se crea un certificado artificial, no se modifica el registro de certificados y no se sustituye la evidencia por un mock.

## Evidencia disponible

- El análisis contractual confirma que `Solicita_registro_certificado_archivo_imagen_gabinete(...)` se ejecuta antes de `add_anotate_image_pdf(...)` y, por tanto, antes del workspace, base64, lock, staged y reemplazo.
- El runner admite `--scenario signed` y solo acepta `DOC104_DOCUMENT_SIGNED` como resultado esperado.
- El escenario calcula SHA-256 y longitud del PDF, exige `Last-Modified`, compara los tres valores antes/después y verifica mediante `SELECT` que la huella de control no cambió.
- Un rechazo diferente, ausencia del metadato, modificación del PDF, más de un POST o cambio del control hacen fallar la corrida.

## Cierre futuro

Cuando exista una tarea con PDF firmado y descartable, copiar el perfil DOC-104 con un nombre `.runtime.json`, completar únicamente los datos no sensibles y ejecutar:

```powershell
npm.cmd --prefix tools/e2e run test:doc104:server-processing -- --mode real --scenario signed --profile profiles/<perfil-firmado.runtime.json>
```

La ejecución requiere nuevamente autorización explícita del ambiente, cuenta, tarea y PDF descartable. Las credenciales se capturan exclusivamente por TTY. Si termina `PASSED`, se registra la evidencia saneada y se retira el riesgo residual histórico; su ausencia no bloquea el cierre actual aprobado y nunca debe reinterpretarse como una prueba ya ejecutada.
