# DOC-118 — pérdida de acuses V2 y conductor local

Base de esta entrega: `1a322a5ca8b85ff9b76457512e72a5ee9da91ae8`. Tarea6.11, D-18/RQ-18; continuación autorizada para preparación y pruebas locales. Se conservan los informes anteriores y el plan RV-08.

El decorador `tests/Fixtures/ProduccionDocumental/Doc118PerdidaAcuseV2.vb` solo se compila en el harness. Envuelve los tres callbacks del puerto existente, identifica comando/revisión exactos y respeta el orden. Ejecuta cada callback nativo una vez; únicamente tras recibir y validar su acuse pierde la respuesta seleccionada y consume la capacidad. Excepción nativa (incluso después de commit), rollback, acuse nulo/alterado, orden/revisión/comando distintos, reentrada y cierre no se presentan como una pérdida de acuse demostrada. No llama al checkpoint ni reinicia el control. No agrega activación HTTP/configuración, endpoints o referencias al vbproj productivo.

`tools/e2e/tests/support/doc118-v2-fault-conductor.cjs` exporta una capacidad de un uso y recibe el driver compartido, arm/disarm, assertActive, beforeDispatch y readEvidence del host. No implementa login, SQL, instalación o un CLI autónomo. El lector/enlace nativos del host se deben suministrar y calificar en P4; no están instalados por esta entrega. El callback previo obligatorio conserva el presupuesto exterior; el host cuenta SELECT individuales y tráfico de navegador, define el timeout de sus operaciones y demuestra autoridad del ambiente/cuentas. El marcador runId no concede autoridad.

| Caso | Corte y comprobación local |
| --- | --- |
| Q1 | Pérdida de INICIO tras acuse válido; cero escritor; cierre negativo solo con prueba física concordante; un replay de recuperación confirmada conserva historial. |
| Q2 | Pérdida de AUTORIZAR; cero escritor; recuperación rechazada por CONFLICTO_REVISION, reclamación y huella intactas; sin replay. |
| Q3 | Pérdida de OBSERVAR; un escritor; recuperación confirma los mismos IDs observados y verificados; un replay conserva historial. |
| Q4 | No lo implementa este conductor. Reutilizar el observador existente después de calificación P4. |

Topes por caso: un ALMACENAR, hasta dos recuperaciones y cuatro lecturas de evidencia; Q2 consume1/1/3. No reintenta un transporte incierto. Comprueba actividad y duración de60s entre operaciones, sin interrumpir un callback externo bloqueado: los timeouts efectivos son responsabilidad del driver/host. Intenta disarm siempre que se haya intentado arm, incluso si arm falla; un cierre no confirmado invalida el resultado. Evidencia cerrada de20 campos, acuse de6 y reporte saneado sin IDs, claves, tokens, huellas o filas. Siempre `businessEvidenceVerified=false` y `integrationPass=false`.

Regresión:42/42 Node del harness/cliente/driver/conductor y205/205 herramientas existentes, conjuntos disjuntos. El harness ejecuta157 casos V2 internos (145 previos y12 nuevos), no son157 pruebas Node adicionales. Los12 casos nuevos .NET usan núcleo de adaptador/repositorio/composición y guardia reales de código; SQL/transacción/escritor son dobles. Los12 casos Node nuevos ejercen el driver compartido sobre transporte/evidencia sintéticos. No hay conexión MySQL real ni llamada al almacenamiento compartido. El decorador queda expresamente excluido del proyecto productivo.

Resultado: P3 preparado localmente, sin hook instalado ni calificación de runtime. P1/P2 siguen implementados en candidato. P4/P5 y las10 tareas reales/finales continúan abiertas; DOC-118 no listo para archivo. No desplegado, sin cambios SII/función compartida ni mutación de intentos41/53/69 o lotes18/21/24. Incremento real:0 HTTP,0 SELECT,0 sesiones,0 efectos. Consumo histórico1276 HTTP/113 SELECT/9 sesiones sin ampliar.
