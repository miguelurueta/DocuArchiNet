# DOC-117 — Preparación ALM-03 lista para autorización específica

Soporte implementado y probado localmente; preparación real NO_EJECUTADO. Esta fase no instala la candidata ni almacena documentos. El descubrimiento de lectura distingue **importación nivel 7** de **carga individual futura en expediente 467/nivel 8/relación 1072**. No se modifica ese expediente, su clasificación o su relación.

## Alcance revisable

Ambiente `https://localhost/GestionDocumental-Docuarchi.net/`, cuenta de aplicación correspondiente al actor 141/empresa 2 y cuenta MySQL exclusivamente SELECT por los DSN registrados. Ambas se capturan en consola enmascarada, sin archivos de secretos. Ventana exclusiva para evitar carreras con otras importaciones.

Crear como máximo un plan/lote bajo publicación 38/nivel 7/gabinete 10, dos recepciones, dos expedientes y dos materializaciones. **Cero documentos, ALMACENAR, carga individual o cambios de DLL**. Hasta **160 SELECT** propios de esta fase, además de los doce controles históricos ya consumidos; no se reinicia ni se reutiliza el presupuesto anterior. Se conserva todo, incluidos los recursos nuevos.

| Recurso | Valor exacto propuesto |
| --- | --- |
| Raíz A y PDF | `DOC117-ALM03-A-20261106` / `DOC117-ALM03-IMPORT-A-20261106.pdf` |
| Raíz B y PDF | `DOC117-ALM03-B-20261106` / `DOC117-ALM03-IMPORT-B-20261106.pdf` |
| Contenido de recepción | Fixture `doc111-carga-individual.pdf`, 677 bytes cada una, SHA-256 `a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c` |
| Valores de importación | Tipología 79; índice 14 TEXTO `EEE` |
| Destino individual que solo se verificará | Expediente 467, nivel 8, relación 1072, tipología 50, nombre futuro `DOC117-ALM03-IND-20261106.pdf` |

Manifiesto cerrado: `doc117-alm03-recursos-propuestos-20261106.json`. La candidata desactivada anterior conserva su alcance local; no se convierte en activa por esta preparación.

## Comprobaciones y salida

Antes de PrepararPlan: ausencia de nombres/recursos, preservación de lotes previos 18–21, autoridad actual para crear/cargar en nivel 7 y cargar en nivel 8, expediente 467 con su clasificación conocida y relación 1072 intactas, gabinete coincidente con 10, tipología 50 compatible y ausencia del nombre individual futuro. La cuenta de aplicación se verifica mediante el contexto y consulta del lote 21 existente, únicamente de lectura. Esa consulta no reconcilia B ni acredita aún la UI individual.

Se reutilizan sesión/driver/recepción/SELECT y comparador de publicación existentes. El soporte ALM-03 tiene validación y autorización propias; no se cambia el driver histórico ALM-02 ni se heredan sus concesiones. Las huellas de las solicitudes futuras se calculan con el calculador .NET existente, validando ejecutable, serializer y ensamblado.

Después de recibir, crear y materializar: verificar originales, identidades/clasificaciones, estados RECIBIDO/NO_INICIADO, ausencia documental y preservación de los recursos anteriores. Volver a comprobar el destino individual sin cargarlo. Guardar los dos comandos futuros en un archivo privado y emitir el alcance con las identidades reales de importación y la identidad individual de nivel 8. No predecir IDs de intento.

Un error o respuesta incierta detiene el proceso, conserva las referencias conocidas y no reintenta ni prepara otro plan. El ejecutor no expone continuación automática de un plan detenido; se revisará el estado antes de cualquier recuperación separada. Nunca repetir 41/52/53 ni limpiar recursos.

## Revisión local

Regresión conjunta 221/221 PASS y OpenSpec estricto PASS. Las pruebas de preparación cubren los dos expedientes y materializaciones sin almacenamiento, receta publicada con orden persistido, permisos de ambos niveles, clasificación/relación/gabinete individual discordantes, nombre ya utilizado, resultado incierto y fallo del diario antes del efecto. `--check` valida propuesta/huellas sin login, SQL o escrituras de negocio.

`tools/e2e/scripts/run-doc117-alm03-setup.cjs` requiere una nueva concesión ligada a propuesta/código/calculador, con almacenamiento y runtime expresamente deshabilitados. No se ha creado esa concesión. Huellas y detalle en `doc117-alm03-setup-local-20261106.json`.

Después de esta preparación seguirá faltando implementar/revisar las guardas activas de los llamadores con esos IDs, compilar/restaurar una candidata específica y cerrar el ejecutor/permiso de coexistencia. Esta fase no completa 17.1/18.1 ni acredita ALM-03 real.
