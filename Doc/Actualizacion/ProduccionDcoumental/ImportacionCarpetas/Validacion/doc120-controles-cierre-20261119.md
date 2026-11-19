# DOC-120: controles por estado y cierre voluntario seguro

Entrega cliente tras «continua con el flujo». [Evidencia JSON](doc120-controles-cierre-20261119.json), tareas3.204–3.211 y CTL-01–06. Sin HTTP/SELECT/efectos reales, DLL o esquema nuevos.

## Cambio y verificación

politicaControles alimenta vista/controlador. Edición, efectos manuales, filtros/páginas/CSV quedan bloqueados durante ejecución/transición. Ayuda/progreso disponibles. Consultas siempre visibles con Pausar y consultar; una sola intención pendiente. X/Escape/Cerrar esperan acuse/lectura antes de retirar la vista; una confirmación terminada durante el cierre no inicia la importación. Se conserva el consentimiento de descarte de preparaciones sin confirmar.

La cola manual pausa admisión sin abortar el PDF actual y devuelve solo recepciones confirmadas. Su opción permanece accesible aunque Continuar ocupe el pie. Preparación segmentada espera la parte iniciada y conserva referencia; el rechazo esperado de pausa no atasca la transición. Pérdida de respuesta conserva aviso/referencia sin afirmar guardado/cancelación ni repetir. Desmontaje forzado conserva cancelación local e invalidación de callbacks; no revierte el servidor. Ayuda tiene nombre accesible en móvil. Tres assets versionados doc120-controles-cierre-20261119-1.

22 casos nuevos (5 política/transporte,17 Chromium): X/Escape en recepción/creación/valores/ALMACENAR, confirmación tardía, consulta repetida, manual parcial, respuesta perdida, desmontaje, ayuda/foco y390/768/1280px, preparación retenida en inicio/validación y fallo de respuesta manual. Observan solicitudes, acuses, abortos y ausencia del siguiente efecto.

Regresión integrada:142 casos,141 aprobados y una expectativa de redacción fallida. Se conserva la redacción habitual al recibir toda la cola y se informa confirmado/total solo si queda parcial. Verificación focal posterior8/8: DOC111 reparado, cinco CTL-P y dos manuales. Acumulado142 únicos cubiertos; siete focales se solapan. No se anuncia otra corrida completa142/142. Fuente local .tmp/doc120-controles-regresion.log (UTF16), hash en JSON; resultado focal en transcripción de herramientas.

Pasaron los recorridos sintéticos de1.000 archivos, lectura focal1/2, presupuesto DOM/memoria, consultas/filtros/CSV, privacidad, mensajes, árbol y reanudación. Sintaxis JS, OpenSpec estricto y diff comprobados. Flujo/anexo/manifiesto actualizados:124 fuentes/20143 líneas. Sin nueva prueba IIS autenticada ni rollback MySQL.

## Vigencia y siguiente comprobación

QA5f7af8f5 cerrada e intacta conserva SHA del cliente anterior; no acredita el nuevo cierre. Videos manuales del19 muestran lote50 con1.000 archivos completados en esa versión: evidencia visual del usuario, no auditoría SQL nueva. No se vuelve a importar ese lote.

Recarga con Ctrl+F5 para obtener los assets nuevos. En una próxima importación de pruebas, X/Escape o Pausar y consultar deben esperar el archivo iniciado, bloquear intenciones adicionales y conservar resultado/referencia. Si falla la respuesta, debe quedar aviso y consulta antes de otra escritura. Otra E2E autenticada necesita recursos/presupuesto/autorización propios; no se lanza por esta entrega.

Ocho tareas locales verificadas;220/231 y11 pendientes previas. Se conservan3.87/3.118/3.126/3.127/3.130/3.131/3.187/5.2/6.4/6.5/6.6. Barra de progreso y políticas de negocio quedan fuera de este ajuste.
