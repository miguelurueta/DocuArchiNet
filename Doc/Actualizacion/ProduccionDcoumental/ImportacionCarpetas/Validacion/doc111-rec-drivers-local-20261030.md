# DOC-111 — Drivers y selector REC preparados localmente

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios previos. Tareas 13.4–13.7 completas en implementación local; 118/130. No se ejecutó aceptación autenticada ni efectos reales.

## Componentes y reutilización

`tools/e2e/tests/support/doc111-importacion-carpetas.cjs` aporta capacidades de apertura/preparación/confirmación, páginas completas por ID/revisión, recepción de PDF, fases acotadas y reapertura. Reutiliza `authenticated-workflow-session.cjs` y `doc110-safe-request.cjs`; no copia selectores de login, crea una plataforma o configura otro Playwright. Usa la sesión/contexto existentes y el token operativo solo en memoria. POST sin retries ni redirecciones; transporte/estado incierto detiene la corrida sin reenviar el efecto.

`tools/e2e/tests/doc111-importacion-carpetas.spec.cjs` agrupa REC-01/02 en tres casos del mismo recorrido: completo/EXPEDIENTE, completo/LOTE y PARCIAL_PERMITIDO/NO_APLICA. Un modo por corrida. Recibe un perfil publicado existente y dos nombres descartables autorizados; no publica perfiles ni cambia permisos, esquema, configuración, carga individual o SII. El PDF reutiliza una fixture existente bajo `tools/e2e/fixtures`, con ruta real acotada a ese directorio. Los nombres de archivo son iguales entre raíces para conservar la prueba de correlación.

`tools/e2e/scripts/run-doc111-importacion-carpetas.cjs` registra el selector `test:doc111:rec` en el paquete existente. Reutiliza consola TTY y proceso hijo compartidos. Antes de una corrida real exige confirmaciones separadas de ambiente, cuenta, efectos documentales concretos, recursos descartables y SELECT con cuentas de lectura; excepción TLS solo si se autoriza. Cuenta, contraseña y conexiones RA/DA se capturan enmascaradas y se entregan al hijo en un entorno efímero. No se crea `.env` ni un perfil con secretos.

El perfil ejemplo es exclusivamente estructural: URL inválida para ejecución, IDs, códigos e índice ilustrativos. Hay que sustituirlos por un nivel/perfil/publicación y expectativas reales aprobados. Cada raíz exige al menos un índice de gabinete esperado, con referencia/tipo/valor explícitos. `--check` no comprueba publicación, permisos, disponibilidad del servidor ni existencia de recursos en BD; valida únicamente contrato local y fixture PDF.

## Recorrido preparado

1. Abrir el nivel autorizado y preparar dos raíces con expectativas de código/nombre. Comprobar la versión/modo/ámbito del snapshot antes de confirmar.
2. Confirmar con clave estable y comprobar repetición de confirmación sobre el mismo lote. Emitir solo referencias numéricas de plan antes de confirmar y lote al conocerlo, para recuperación si hay fallo.
3. Recibir el primer PDF. En LOTE intentar crear esa raíz antes del segundo PDF; en EXPEDIENTE/PARCIAL intentar la raíz aún no recibida. Exigir 409/OPERACION_EN_CURSO y controles propios idénticos, sin nueva reserva/efecto.
4. En EXPEDIENTE/PARCIAL crear la raíz completa mientras falta la segunda recepción. En LOTE recibir ambos PDFs antes de crear cualquiera. Crear dos expedientes, conservando el destino.
5. Materializar cada archivo y contrastar por SELECT los valores fijados contra las expectativas antes de almacenar. Si difieren, detener sin enviar ALMACENAR para ese archivo. Ejecutar una fase documental por petición.
6. En parcial conservar un tercer original `.tif` rechazado por el contrato; no enviar sus bytes. Exigir PARCIAL y denominadores originales, no éxito completo.
7. Contrastar resultados mediante SELECT. Repetir exactamente los comandos ya confirmados, comparar también los IDs de sus respuestas y reabrir el lote con la UI existente. Exigir huella de datos propios/relaciones físicas idéntica, sin más intentos/eventos/documentos.

No se induce ni se reconcilia artificialmente un efecto incierto real en esta suite. Ante incertidumbre se conserva la referencia y se detiene; los casos de pérdida de respuesta/reconciliación ya probados localmente no se presentan como aceptación real de este recorrido.

## Controles de datos

Los únicos comandos de control son SELECT parametrizados, sin sentencias múltiples, DML, bloqueo FOR UPDATE o salida a archivos. Dos conexiones de cuentas de solo lectura suministradas por TTY; `multipleStatements=false`. No se obtiene una conexión leyendo Web.config ni se ejecuta el script previo de persistencia mutante.

Se contrastan lote, raíces/archivos originales, cantidades/bytes, snapshot, actividad pendiente, checkpoints terminales concordantes y eventos. La creación exige correlación del ciclo Archivo Gestión y su ID resuelto por nombre en `ra_de_tipos_ciclos_archivo`, más relación/nivel, actor/empresa, clasificación y código/nombres reales del expediente. No se usa un ID fijo de ciclo.

Documento/inventario/índice se contrastan por los IDs conocidos en el gabinete elegido, nombre original y segundo nombre, expediente, actor/empresa, tipología, clase electrónica y PDF. Los nombres físicos de gabinete/campos se leen de catálogo y se validan como identificadores; referencias duplicadas/ambiguas se rechazan. Los índices se comparan contra valores esperados/fijados, conservando ceros en texto y admitiendo escala física DECIMAL sin redondeo JavaScript. No se atribuye un efecto buscando una coincidencia global por nombre.

La salida del proceso hijo solo conserva referencias numéricas de recuperación y un resumen cerrado de resultados/contadores/SHA. No publica filas, payloads, nombres, cabeceras, cookies, tokens, credenciales ni cadenas de conexión. El runner exige exactamente un resumen completo PASS del modo solicitado: una suite omitida o un proceso con código cero sin ese resultado no aprueban. No hay retries Playwright ni capturas/trazas/videos de sesión.

Se conservan los recursos creados y staging para revisión/recuperación. El finally cierra sesión/conexiones y retira secretos del objeto de entorno efímero; no borra resultados ni ejecuta limpieza.

## Validación ejecutada aquí

- PASS: 18 pruebas Node de contrato, selector/autorización, salida saneada, referencias recuperables, tres modos con dobles, barrera sin mutaciones, índices inesperados antes de almacenamiento, respuestas/huellas de repetición discordantes y controles SELECT sintéticos.
- PASS: 12 regresiones del helper de sesión/uso compartido; 13 pruebas existentes del cliente, incluyendo Chromium local con servicios sintéticos. Total de la corrida conjunta: 43/43.
- PASS: `npm.cmd --prefix tools/e2e run test:doc111:rec -- --check --profile profiles/doc111-importacion-carpetas.profile.example.json --scenario EXPEDIENTE`, sin TTY/login/BD.
- PASS: Playwright `--list` registra exactamente tres casos REC-01/02 en un archivo; no se ejecutaron esos casos reales.
- PASS: `node --check` de cliente, inventario, transporte, vista, script Producción, driver, spec y selector. OpenSpec estricto y `git diff --check`.

No hay cambios productivos VB/SQL en este corte; no se repite MSBuild para cambios exclusivos de herramientas/documentación. Se conserva la compilación local de 13.9. Las funciones legacy protegidas/SII siguen sin diferencias. La exclusión adicional del XML frente a otras cargas permanece descartada por instrucción del usuario.

13.10–13.16 (autorización específica, esquema/control del ambiente y aceptación real por modo/regresión), matriz final 13.17–13.18 y limpieza 11.7 siguen pendientes. HTTP/IIS autenticado, conexiones MySQL reales, efectos documentales/XML y limpieza: **NO_EJECUTADO**. Los PASS de dobles/listado/sintaxis no sustituyen esas tareas.
