# DOC-114: ajuste propuesto del formato B

El intento 2 leyó seis tipologías de la clasificación publicada. La referencia 79 existe y su código es NULL. Ninguna de las seis proporcionó un código seguro y único para el archivo del formato B. No se generaron recursos ni se llamó a PrepararPlan. No se afirma que los seis códigos sean nulos: la observación retenida solo clasifica el de 79.

Se propone crear/publicar **una versión nueva del perfil de pruebas 27**, partiendo de 41. Único cambio: `TIPOLOGIA:TIPO_DOCUMENTAL.TipoValor`, de `TEXTO` a `REFERENCIA`. Configuración y otras cuatro reglas quedan iguales. El formato B válido usaría `000114#03-11-2026/79.pdf`; el A conservaría `000114-20261103/DOC114-79.pdf`. La versión nueva se tomará de la respuesta, sin asumir su ID.

La versión 41 se conserva publicada y sirve de control negativo: `79.pdf` debe producir TIPOLOGIA_NO_UNIVOCA bajo la regla TEXTO, después de comprobar mediante SELECT que el código 79 no es único. Así se verifica explícitamente que un ID válido no se interpreta como código. Se mantienen siete preparaciones; no se modifica el catálogo para fabricar datos válidos. No se acredita un caso positivo por código, porque falta ese recurso en este ambiente.

Efectos de la publicación propuesta: tres operaciones administrativas sobre el perfil 27 (CrearVersion, GuardarBorrador, PublicarVersion), una nueva versión publicada, cero perfiles nuevos, planes, documentos o cambios de permisos. Se conservan 24/38, 25/39, 26/40 y el contenido publicado 27/41. La publicación cambia únicamente la versión vigente del perfil de pruebas 27. La autorización anterior de dos publicaciones ya se consumió; esta publicación adicional requiere autorización específica.

Propuesta de datos: [doc114-format-b-reference-proposal-20261103.json](doc114-format-b-reference-proposal-20261103.json). Estado: propuesta local, sin ejecutar.

Validación estructural local con el ValidadorPerfilImportacion Framework existente: **PASS**. No consulta catálogos reales ni modifica la aplicación.

## Publicación adicional autorizada y preparada — 2026-11-03

Autorización específica del usuario: “autorizado”, en respuesta a la propuesta de una versión adicional del perfil 27. El JSON aprobado queda ligado por SHA256 a la autorización local. Ejecutor en `tools/e2e/tests/support/doc114-reference-test-version.cjs`; reutiliza el cliente administrativo y solo añade CrearVersion a su lista cerrada de operaciones.

Antes de escribir, contrasta las cuatro versiones publicadas, la configuración de 41 frente a la copia revisada y todas sus reglas. Crea una versión, anuncia el ID inmediatamente, guarda únicamente el cambio TipoValor y publica con revisiones devueltas. Lee nuevamente contenido e historial y contrasta el catálogo operativo. Tres mutaciones máximas, una sola vez cada una; sin planes, documentos ni cambios de permisos.

Regresión 73/73 PASS (incluye 5 casos de esta versión): autorización/delta, origen alterado, fallos intermedios sin retry, preservación del historial y ausencia de publicación operativa. Sintaxis de consola PASS. [Manifiesto local](doc114-b-version-ready-20261103.json). La publicación real aún no se ejecutó; no se declara aceptación PLAN-01.

## Versión 42 publicada; PLAN-01 ajustado listo — 2026-11-03

PASS de publicación adicional: perfil 27 / versión 42. Tres mutaciones confirmadas: CrearVersion, GuardarBorrador y PublicarVersion. Historial 41 y publicaciones 38/39/40 conservados por lectura comparada; 42 disponible en catálogo operativo con la única regla cambiada a REFERENCIA. Cero planes, documentos o cambios de permisos. [Resultado saneado](doc114-b-version-result-20261103.json).

PLAN-01 conserva siete intentos autorizados y cero consumidos hasta este punto. Formato A válido: versión 40, 000114-20261103/DOC114-79.pdf. Formato B válido: versión 42, 000114#03-11-2026/79.pdf. Negativo de tipología: versión histórica 41 TEXTO, 79.pdf, exige TIPOLOGIA_NO_UNIVOCA; antes de prepararlo se comprueba por SELECT que el código 79 no es único en la clasificación. Los otros cuatro casos conservan segmento ausente, requerido vacío, fecha inválida y longitud de nombre 121. Se verifican ceros, fecha, nombre, ID de tipología y descriptor diferido índice 14 con valor null.

Recursos locales comprobados con validateSession: PASS. Regresión de selección/generación 3/3 PASS. Consola de intento 3 con archivo de estado propio, sin sobrescribir los intentos previos detenidos antes de preparar. Esta preparación local aún no acredita ejecución PLAN-01; tareas siguen 12/16. El positivo por código de tipología continúa fuera de evidencia por falta de ese dato en este ambiente.
