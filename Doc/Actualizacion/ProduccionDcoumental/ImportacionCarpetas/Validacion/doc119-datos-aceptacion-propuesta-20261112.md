# DOC-119 — datos para completar SEG-01/02

Estado: **preparación local; efectos pendientes de autorización expresa**. No se ha creado ninguna importación, recibido archivos ni creado expedientes con esta preparación. La autorización anterior cubre lectura; D-10 exige autorización específica para efectos. D-09 mantiene cerrada la compuerta UI hasta aceptar SEG-01/02.

## Inspección real previa

Corrida de inspección `eb09fc5981be453b936b2bf1714c43a3`, cerrada el 2026-11-12T19:25:52.198Z: 5 SELECT de un máximo de 6, 0 HTTP y 0 sesiones de aplicación. Secretos eliminados al terminar. Alcance: propietario141/empresa2 de la corrida de privacidad `08e8608d0dd94331864ab958830f2808`; no se reutilizaron sus credenciales.

[Evidencia saneada y verificada contra el estado local](doc119-disponibilidad-datos-20261112.json). La verificación local comprobó además nombre/tamaño/modificación y SHA-256 de las 27 copias preparadas; ninguna fue enviada.

Las 24 importaciones accesibles de ese propietario tienen como máximo tres archivos. Ninguna supera las 25 filas que muestra el cliente por página. Los cinco lotes con recepción pendiente (1,6,8,10,11) carecen de raíces confirmadas para SEG-02. La búsqueda local en `.tmp`, `tools/e2e/artifacts` y `tools/e2e/fixtures` examinó metadatos de 3.569 PDF y no halló coincidencias de nombre, tamaño y modificación para los archivos pendientes examinados. No se alcanzaron los límites de 30 lotes/200 archivos pendientes/200 raíces. Esto acredita la falta de recursos en el alcance inspeccionado, no una ausencia global en el sistema.

## Preparación concreta

Ambiente propuesto: `https://localhost/GestionDocumental-Docuarchi.net/`, módulo GESTOR; propietario141, empresa2, destino7. Segunda cuenta de privacidad: Carolina136/empresa2, ya autorizada para la prueba. Perfil propuesto26/version40/gabinete10: publicado en la inspección, `COMPLETO_OBLIGATORIO/EXPEDIENTE`. No se modifica el perfil. La existencia del destino alterno y sus permisos se comprobarán antes de la recuperación; no se presupone acceso por datos históricos.

Se prepararon originales nuevos bajo `.tmp/doc119-next-preflight/eb09fc5981be453b936b2bf1714c43a3/fixture`, excluidos de Git, con manifiesto local `fixture-manifest.json`. Son copias de la fixture PDF del repositorio, con fechas reales de creación local conservadas, sin cambiar fechas de originales existentes.

| Recurso | Cantidad / identidad |
| --- | --- |
| Nombre visible de ambas raíces | `DOC119-20261112-SEG-eb09fc59` |
| Selecciones / correlaciones distintas | s1/r1 y s2/r2 |
| PDF de r1 | a1, `DOC119-001.pdf`, 677 bytes |
| PDF de r2 | a2–a27, `DOC119-002.pdf` a `DOC119-027.pdf`, 26 × 677 bytes |
| Total local / manifiesto | 27 PDF, 18.279 bytes |
| SHA-256 del contenido PDF de cada copia | a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c |

El manifiesto incluye nombre, ruta relativa, tamaño y modificación efectiva de cada copia. Los homónimos se distinguen por selección/ID; no se simulan coincidencias alterando fechas ni se cambia el tamaño de página del producto.

## Ventana de efectos solicitada

Una corrida nueva y manual, máximo **400 HTTP, 40 SELECT y 1 sesión de propietario** para preparar los datos. Los controles SQL siguen siendo exclusivamente SELECT. Las escrituras pasan por los servicios de la aplicación y quedan limitadas a:

1. Preparar un plan con las dos raíces y los 27 archivos anteriores. Revisar snapshot, integridad EXPEDIENTE, elegibilidad de ambas raíces y de los 27 archivos, valores resueltos y ausencia de conflictos. Si el perfil vigente no admite el manifiesto, detener antes de confirmar; no cambiar reglas ni improvisar otros recursos.
2. Confirmar ese plan una sola vez: máximo un lote, dos raíces y 27 registros de archivo. Conservar las referencias devueltas para cualquier consulta posterior.
3. Recibir únicamente a1: máximo un PDF de 677 bytes. Los otros 26 originales permanecen locales y pendientes de recepción.
4. Ejecutar únicamente CREAR_EXPEDIENTE para r1: máximo un expediente y su relación con destino7. No crear el expediente de r2. La barrera EXPEDIENTE del coordinador permite circunscribir la recepción requerida a r1; la elegibilidad real se valida antes del efecto.
5. Consultar y verificar el estado resultante; detener ante error, respuesta incierta, conflicto, revocación, límite o recurso inesperado. No repetir automáticamente comandos ni reconstruir una segunda importación.

No incluye ALMACENAR, materialización de valores, creación de documentos, reconciliación, inyección de fallos, detención, cambio de permisos, modificación de perfiles, SQL de escritura ni limpieza. Los datos creados se conservarán como evidencia; no se promete rollback de efectos confirmados. El PDF recibido añade al menos 677 bytes de contenido al staging, además del control/diario que produzca la aplicación; no se fija un tamaño exacto del inventario completo posterior.

## Aceptación posterior, separada de la preparación

Reutilizar el selector de lectura DOC-119 con nuevas ventanas e IDs de corrida y con el lote recién confirmado: SEG-01 conserva 600 HTTP/80 SELECT/2 sesiones; SEG-02 conserva 600 HTTP/60 SELECT/2 sesiones. Captura de secretos por consola enmascarada, sin persistencia. El certificado local se acepta solo mediante elección expresa del operador.

El resultado esperado permite paginar un filtro real de 26 archivos pendientes con página de 25, descargar su CSV y volver a comprobar privacidad con Carolina. SEG-02 utiliza el primer PDF pendiente de r2 conservado localmente, la raíz confirmada r1 y un destino alterno permitido para comprobar recarga, reselección sin envío y preservación de selección/rama. La reselección no recibe ese PDF. No se retira una relación del DOM para forzar una supuesta inserción: si la rama ya la contiene, se registra esa limitación real.

Preparar datos no equivale a aceptar SEG-01/02. Las tareas2.4/2.5 y la compuerta2.7 permanecen abiertas hasta revisar la ejecución real y sus huellas. SEG-03 requiere su decisión independiente en `doc119-seg03-propuesta-sin-limpieza-20261112.md`.
