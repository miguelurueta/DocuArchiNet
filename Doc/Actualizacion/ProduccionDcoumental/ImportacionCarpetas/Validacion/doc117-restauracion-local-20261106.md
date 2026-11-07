# DOC-117 — Respaldo y restauración local, 2026-11-06

**PASS de ensayo local aislado**. Se copiaron los bytes de la DLL original y de la candidata desactivada a un sitio desechable bajo `.tmp`. No se cargó una DLL ni se activó una variante en IIS o en el sitio real.

La mecánica de archivos existente de DOC-116 se extrajo a `tools/e2e/scripts/support/laboratory-runtime-lease.cjs`. El wrapper DOC-116 conserva sus rutas, escenario, autorizaciones y nombres públicos; sus nueve pruebas de regresión pasaron. El wrapper DOC-117 tiene validaciones propias y no convierte una autorización DOC-116 en una DOC-117.

`doc117-runtime-lease.cjs` distingue LIVE y REHEARSAL. El modo de ensayo solo admite el sitio y copias concretos de un directorio nuevo de `.tmp`, con autorizaciones de efectos e instalación en false. LIVE exige los tres permisos propios y el alcance cerrado de recursos; todavía no hay un manifiesto de ejecución real ni una autorización de instalación.

Ensayo ejecutado:

```powershell
node tools/e2e/scripts/rehearse-doc117-runtime.cjs --candidate-build .tmp/doc117-alm02-dormant-20261106
```

Se verificaron dos respaldos byte a byte y hash; preservación de ACL de Windows al sustituir el archivo temporal; guardián en proceso independiente; observación de los bytes candidatos en el sitio temporal; salida deliberada del propietario sin `finally` de restauración; recuperación del original por el guardián después de esa salida. Los bytes finales y ambos respaldos coinciden con el original. El guardián se lanza oculto y sin credenciales en su entorno.

La DLL real mantiene SHA256 `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. La candidata desactivada conserva `d8966b9b4bf420981adde58d916878c1d3a7df1dfa23974774351dcfc70c7b0a`.

Pruebas: `node --test tests/doc116-runtime-lease.test.cjs tests/doc117-runtime-lease.test.cjs`, **16/16 PASS**. Incluyen restauración desde reserva cuando se altera el respaldo principal, rechazo de ambas copias corruptas, negativa a sobrescribir un cambio externo, guardián ausente, candidata alterada, separación de escenarios y permisos. El límite de 600 segundos se prueba con reloj inyectado mientras el dueño sigue vivo; no se esperaron diez minutos reales. La muerte del propietario sí se ensayó con procesos independientes y archivos reales bajo `.tmp`.

Detalle y hashes de soporte: `doc117-restauracion-local-20261106.json`. No se ejecutaron nuevas consultas SELECT, autenticación, cargas, escrituras de base de datos, permisos/configuración, limpieza ni instalación. La tarea 16.1 sigue pendiente de ejecución interna real.

Verificación conjunta posterior: `node --test tests/doc116-internal-fault.test.cjs tests/doc116-runtime-lease.test.cjs tests/doc117-runtime-lease.test.cjs tests/doc117-alm02-candidate.test.cjs tests/doc117-alm02-preflight.test.cjs`, **33/33 PASS**, sin conexión. Esta corrida incluye los 16 casos iniciales; las cuentas no se suman como si fueran pruebas distintas.

El siguiente bloqueo de diseño está descrito en `doc117-alm02-ajuste-alcance-propuesto-20261106.md`: el intento se reserva dentro de `AvanzarLote` y no puede conocerse antes de compilar por una API de reserva aislada. La propuesta conserva el intento exacto mediante fijación durable previa al escritor; **no está aplicada ni autorizada editorialmente**. Los gates LIVE no se relajaron.
