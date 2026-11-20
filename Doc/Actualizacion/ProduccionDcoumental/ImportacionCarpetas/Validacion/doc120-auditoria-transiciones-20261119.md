# DOC-120 - auditoria de transiciones del importador

Auditoria solicitada por el usuario tras regresiones repetidas. Base077920da; tareas3.235-3.238 y escenarios AUDCTX01-04. [Fuentes y resultados](doc120-auditoria-transiciones-20261119.json).

Se revisaron el controlador cliente, la vista, la coordinacion y el transporte. La regresion inicial existente aprobo127/127 y aun asi se reprodujeron cuatro defectos al cruzar contextos. Esa diferencia demuestra por que el numero de pruebas aisladas no bastaba. Se agregaron siete casos Chromium con cliente/CSS/DOM reales, lotes sinteticos con recursos diferentes y respuestas controladas.

| Defecto reproducido | Causa | Correccion | Evidencia nueva |
|---|---|---|---|
| Cambiar de un lote en pagina2 de carpetas a otro con una sola pagina mostraba el listado vacio | paginaRaicesResultado conservada al reiniciar referencia | Reiniciar pagina de raices junto con filtros y pagina de archivos | AUD-01 |
| Al volver a consultar un lote reaparecia un filtro borrador aunque la consulta era sin filtros | viewStates guardaba borradores por ID, sin distinguir una nueva instancia de filtros aplicados | Asociar borradores con los objetos de filtros del contexto; nueva consulta los descarta, volver al original los conserva | AUD-02/AUD-05; CSV usa solo solicitud aplicada |
| La ayuda se cerraba al llegar el siguiente acuse | render cerraba cualquier dialogo secundario cuando el modelo estaba ocupado | Distinguir ayuda, referencia y expedientes; ayuda no se cierra por avance ni recibe alertas de consulta | AUD-03 |
| Un fallo del lote consultado decia que la preparacion se detuvo y que no se habian enviado PDF, tras un guardado confirmado | Object.assign copiaba progreso y estado transitorio de la revision guardada al modelo de consulta | Crear consulta con modeloInicial propio; conservar ticket/seleccion original aparte y limitar Continuar revision a su contexto | AUD-04; un inicio de preparacion, sin confirmarla |

Los tres primeros fallaron en sus aserciones antes de corregirlos; el cuarto reprodujo literalmente el mensaje falso despues de ajustar la inyeccion del fallo al endpoint focal realmente utilizado. Una primera inyeccion sobre ConsultarLote no afectaba ConsultarRecursoLote y se descarto como evidencia de defecto. Durante el ajuste de AUD-06 se corrigieron texto esperado/selector de prueba: una denegacion403 cierra deliberadamente el transporte. Ese comportamiento de seguridad se mantiene; no se contabiliza como otro defecto corregido.

La matriz revisa los caminos del cliente dentro de este corte; cada fila identifica la cobertura reutilizada y sus limites.

| Estado / transicion | Decisiones y errores comprobados | Casos o suites |
|---|---|---|
| Accesos / apertura | Destino, seleccionado/pulsado, referencia conocida, ultimo lote sin referencia, fallo de apertura | doc119-navigation/context-menu, doc120-consultar-ultimo |
| Seleccion / perfil / revision | Invalidez del ticket al cambiar inventario/perfil, preparacion pequena/segmentada, Unicode/tamano, pausa y misma continuacion | doc111-client, doc120-preparacion-segmentada/tamano, REV-UI |
| Confirmacion / respuesta tardia | Confirmacion unica, perdida de acuse, cierre mientras confirma, consulta sin repetir confirmacion | doc111-client, CTL-UI02, REV-UI05 |
| Consulta / volver / otra referencia | Contextos separados, pagina1, filtros/borradores/CSV, referencia denegada, preservacion del ticket original | AUD-01/02/04/05/06, LAST, CTX-UI |
| Lectura antes de continuar | Enumeracion/ambito/revision coherentes, fallo inicial sin efectos, controles liberados para consultar | AUD-07, COORD-08/09/11 |
| Recepcion / creacion / indices / guardado | Barreras reales del cliente, una fase en vuelo, avance solo por acuse, integridad publicada | RUN, COORD, doc120-recepcion-creacion/retomar-creacion |
| Pausar / consultar durante ejecucion | Espera de solicitud iniciada, no abortar ni admitir siguiente; filtros y consultas no reanudan por si mismos | CTL-UI01/03/04/08/09, MNS-UI01/02 |
| Cerrar / cancelar / aceptar / desmontar | Consentimiento previo, cancelar/Escape mantiene ejecucion, aceptar espera, forzado invalida callbacks | CTL-UI, REV-UI, CTX-UI |
| Original ausente / vincular / continuar | Decision explicita, omitir solo con consentimiento, pendientes locales separados de temporal recibido, carpeta valida sin envio automatico | IND-UI, ORI-UI, RUN08 |
| Red perdida / fallo de lectura posterior | Resultado incierto o confirmado conservado, consulta sin replay, no afirmar rollback ni ausencia de efectos | RUN03/04, MNS-UI03, COORD03/04/15/16, AUD-04 |
| Seguridad / concurrencia |401/403 cierra transporte; reabrir reautoriza referencia. Carrera entre dos vistas con un ganador en doble de revision | AUD-06, COORD14 y casos existentes de cliente; no carrera MySQL nueva |
| Ayuda / foco / pantallas pequenas / volumen | Ayuda no se cierra por acuse, teclado/foco/modales, presupuestos DOM/memoria y recorridos1000 sinteticos | AUD-03, CTL/REV/ORI/MNS y segmentada-browser |

La verificacion final, lista exacta de suites y hashes se registran en el JSON companero. No sumar de nuevo la regresion inicial ni los focales solapados. Los dobles no miden consultas SQL, rendimiento nativo, escrituras RA/DA, permisos actuales de cuentas reales o integridad de documentos reales. Cero HTTP/SELECT/autenticacion/efectos de negocio reales; sin compilacion/sustitucion DLL ni cambio de esquema. La QA historica permanece cerrada y no se usa como certificacion del cliente actual. Las once tareas pendientes previas conservan su alcance; no se declara DOC-120 terminado ni ausencia absoluta de errores.

Ejecucion conjunta reutilizable: `node tools/e2e/scripts/test-doc120-transitions.cjs`. Incluye las15 suites de contratos/entradas/recuperacion seleccionadas y todas las suites `doc120-*-browser.test.cjs` (24 en este corte). `--list` permite inspeccionar la seleccion sin ejecutar pruebas; coincide con la lista de la corrida final. No pide claves ni llama a runners de negocio real. Las cinco suites antiguas se actualizaron para aceptar el consentimiento del modulo antes de esperar cierre, usar Pausar y salir durante actividad y comprobar ultimo lote siempre disponible junto con referencias aisladas;38/38 tras esos ajustes. El contraste parcial contra fuentes anteriores reprodujo seis de las expectativas obsoletas; no se presenta como una ejecucion completa de nueve fallos en aquella version.

**Resultado final:223/223 en una corrida completa de24 suites, siete casos nuevos;0 fallos/cancelados/omitidos.** No se suman127 iniciales ni7/38 focales. Los casos1000, acuse perdido, lectura posterior, cierres por fase, originales, filtros/CSV, arbol y reanudacion quedan incluidos. Sintaxis, OpenSpec estricto, diff y flujo vigente124 fuentes/20225 lineas verificados. Assets doc120-auditoria-transiciones-20261119-8;247/258 tareas,44 S/214 M/0 L,107 escenarios y once pendientes previas. Pruebas locales aprobadas; no equivalen a una nueva certificacion de RA/DA o del ambiente autenticado.
