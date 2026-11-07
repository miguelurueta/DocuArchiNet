# DOC-117 — Propuesta técnica, sin ejecución real

Estado vigente posterior: las dos ventanas propuestas se ejecutaron con autorización específica y PASS el 2026-11-06; ver `doc117-alm02-interno-pass-20261106.md/.json`. El texto siguiente conserva la secuencia de propuesta/preparación anterior a esa corrida y no concede nuevas ejecuciones.

Diseño aprobado editorialmente. Esta propuesta no autoriza cargas, credenciales ni instalación de DLL. Conservar todos los recursos; no tocar lote 18/intento 41 ni repetir efectos inciertos. Controles SELECT por los DSN registrados MembershipUsers, sin URL MySQL. Usuario de aplicación y cuenta MySQL de lectura son identidades distintas. Secretos efímeros exclusivamente en consola.

## Dos ventanas documentales exactas

Reutilizar el adaptador y repositorio. La variante de laboratorio instrumenta exclusivamente la composición documental de `_autorizar`, `_iniciar`, `_invocar` y `_registrar`: conserva la autoridad, el inicio transaccional, el escritor y el checkpoint reales; no modifica ClassAlmacenamiento, SII ni añade bandera/endpoint/header productivo. Restringir por actor, empresa, lote, archivo y solicitud exactos. El intento real se fija atómicamente antes del inicio/escritor, según el ajuste aprobado `doc117-alm02-ajuste-alcance-propuesto-20261106.md`; una activación por archivo, con registros/marcadores saneados fuera de la API. El wrapper de `_invocar` registra durablemente una única entrada por archivo antes de llamar al escritor existente.

A: delegar a RegistrarResultadoDocumento real, verificar que retornó su resultado confirmado y lanzar excepción antes de devolverlo al adaptador. El coordinador debe recuperar CONFIRMADO del histórico. Repetir idénticamente una vez: mismos IDs, sin nuevo writer ni evento documental. No describirlo como corte de red o caída del servidor.

B: después de retornar la llamada legacy y verificar su resultado, lanzar antes de invocar RegistrarResultadoDocumento. El adaptador conserva IDs observados y devuelve incertidumbre; no habrá checkpoint documental final. Se espera intento persistido EN_CURSO y resultado público INCIERTO, sin habilitar reconciliación soportada ni otra carga. Registrar documento/inventario conocidos por salida y SELECT. No repetir esta raíz. Esta ventana no equivale a ausencia de incorporación: no se altera la función compartida para fabricar otro fallo.

## Recursos y presupuesto propuestos

Un plan y un lote nuevos bajo nivel 7/versión 38/gabinete 10, dos raíces y dos PDF de 677 bytes, dos recepciones, dos expedientes, dos documentos. Tres solicitudes de almacenamiento: A, replay A y B. No reutilizar raíces de DOC-116. Los IDs de lote/raíz/archivo/expediente y las huellas exactas de las solicitudes deben conocerse antes de construir la candidata cerrada. No se predicen IDs de intento: se fija el primero concordante antes del escritor.

## Requisitos antes de instalar

Construir y revisar fuentes exactas y candidata; comprobar que la candidata cambia solo el binding documental de laboratorio, sin hook distribuido. Validar respaldo original y reserva independientes byte a byte, hash, restauración local y guardián independiente de máximo diez minutos. Ventana exclusiva sin otras importaciones/despliegues/reinicios. El guardián debe restaurar aun si desaparece el cliente; verificar runtime y ambas copias al finalizar y reabrir con original. La candidata, su fuente y autorización tienen SHAs propios y no se atribuyen al binario ordinario. Un fallo de restauración impide cualquier continuación.

## Estado

Ejecución real: NO_EJECUTADO. El 2026-11-06 se construyó una candidata local desactivada con IDs deliberadamente inválidos; no se instaló ni activó. Ver `doc117-alm02-preparacion-local-20261106.md/.json`. El helper de laboratorio y el generador no pertenecen al proyecto productivo; el generador no permite crear un alcance activo. Pruebas locales compilaron copias desechables con IDs sintéticos y dobles, sin MySQL/HTTP/IIS.

La ventana B del prototipo se sitúa exactamente después de verificar el resultado legacy y antes de llamar a `RegistrarResultadoDocumento`; no inyecta un fallo dentro del escritor. El marcador se denomina `AFTER_VERIFICATION_BEFORE_CHECKPOINT`. La ventana A delega al checkpoint, comprueba estado e IDs y falla antes de retornar al adaptador.

El preflight SELECT propuesto se ejecutó después con autorización específica: PASS, 8/8 consultas el 2026-11-06; ver `doc117-alm02-preflight-pass-20261106.md/.json`. Acredita publicación, código/nombre, ausencia de nombres e índices; no acredita permisos, tipología ni ejecución interna de fallos.

El ajuste de fijación previa al escritor fue aprobado e implementado localmente. La mecánica de respaldo/restauración DOC-117 tiene ensayo aislado con guardián independiente y verificaciones propias. Detalle: `doc117-fijacion-intento-local-20261106.md/.json` y `doc117-restauracion-local-20261106.md/.json`. No se atribuyen esos ensayos a una activación real.

Preparación real autorizada posteriormente: PASS, plan 37/lote 21, raíces 33/34, archivos 34/35 y expedientes 664/665; 100/160 SELECT acumulados, sin ALMACENAR. Se generó el alcance cerrado y se compiló la candidata específica `58674b31eedc6cc7d60cc5fe04d731d9273e5ee6fc047ad7938637ce3dfa93ec`, no instalada. Su respaldo/restauración se ensayó como copia de bytes en un sitio temporal con guardián independiente. Ver `doc117-alm02-setup-pass-20261106.md/.json`.

Avance local posterior: ejecutor compuesto y revisado para esas dos ventanas, contador de escritor añadido, candidata específica recompilada (`85824b255a826664a8395754d8903e51218ba1a8303595ee6e181c8c0dc05a95`) y restauración de esos bytes ensayada en sitio temporal. Detalle revisable y presupuesto independiente: `doc117-alm02-interno-listo-20261106.md` y `doc117-alm02-interno-propuesto-20261106.json`. Faltan la autorización específica de instalación/almacenamiento/inyección y los controles previos inmediatos. No se hereda autorización de preparación o DOC-116 ni se usa su lease cerrado como autorización DOC-117. Las ventanas internas documentales reales continúan NO_EJECUTADO.
