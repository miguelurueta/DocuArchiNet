# DOC-111 — Consolidación de raíces y lote

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios locales previos. Tareas 10.7, 10.9 y 10.10 completadas localmente; 109/130. Sin commit, despliegue o efectos reales.

## Cálculo y evidencia

`CoordinadorImportacionCarpetas.Consolidacion.vb` incorpora el cálculo puro desde `VistaConsolidacionImportacion`: lote, plan confirmado, todas las raíces/archivos originales e intentos relevantes. No consulta proveedores, reglas, catálogos o disco, ni ejecuta efectos.

Se comprueban contexto, snapshot/política, cantidades/bytes originales, correlaciones y valores de plan por elemento. La ausencia/duplicación de originales, datos ajenos o valores cambiados impide consolidar. Los rechazados/cancelados permanecen en los totales; la política no se cambia para presentar éxito.

COMPLETADA en una raíz exige identidad/relación del destino y un checkpoint terminal de creación concordante, con ciclo Archivo Gestión, más confirmación positiva de cada archivo original. COMPLETADO de lote exige todas las raíces y archivos completos. Una etiqueta de archivo por sí sola no acredita éxito: se exige recepción coherente, valores fijados, IDs de documento/inventario y resultado canónico coincidente con un único intento ALMACENAR confirmado/finalizado. La asignación del mismo documento o inventario a dos originales produce incertidumbre en las raíces afectadas.

Los estados se calculan conservando indicadores separados de actividad, incertidumbre, rechazos y pendientes:

- Incertidumbre o marcadores/intentos huérfanos: PENDIENTE_VERIFICACION. Los marcadores y la evidencia se conservan.
- Actividad sin incertidumbre: EN_CURSO; el vencimiento de una fecha no da permiso de repetir ni declara DETENIDO.
- Todos los efectos comprobados: COMPLETADO, incluso si llegó una señal de detención al terminar el último efecto; la señal se conserva.
- Detención sin actividad y con trabajo restante: DETENIDO.
- Progreso con rechazados y sin trabajo automático restante: PARCIAL; no se elimina el error ni se convierte el perfil al modo parcial.
- Trabajo pendiente tras efectos previos: EN_CURSO. Antes de efectos, LISTO/RECIBIENDO conforme recepción. FALLIDO se reserva para ausencia de progreso y de trabajo recuperable.

Una raíz ya INCIERTO permanece así: consolidar no reconcilia ni libera su reclamación. Una terminalidad contradictoria se rechaza en vez de reparar estados automáticamente. Las raíces homónimas se distinguen por sus IDs/correlaciones; completar una no completa el lote mientras falte otra.

## Persistencia y conexiones

El parcial `.Consolidacion.vb` pertenece al mismo repositorio y tablas. Bajo el bloqueo de lote lee raíces/archivos completos con límites originales +1 y bloqueos; selecciona intentos confirmados documentales y toda actividad/incertidumbre, con límite derivado del inventario. Excederlo rechaza la vista, no resume una lista truncada.

Solo actualiza estado/revisión/fecha de raíces y lote mediante CAS por identidad, revisión y estado original. No modifica identidades documentales, archivos, intentos, totales, política o señal de detención. Registra eventos de raíz/lote cuando hay cambios; repetir un agregado idéntico no escribe ni genera eventos.

La consolidación se incorpora a la misma conexión/transacción del resultado nuevo de creación, documento y materialización de valores. También se aplica al registrar una detención. Si falla antes del commit, el wrapper transaccional existente aborta el conjunto; no se usa otra conexión para confirmar el agregado por separado. Los checkpoints terminales repetidos y valores ya fijados mantienen su salida sin nueva consolidación/eventos.

`ConsolidarLote` es una capacidad interna con contexto/revisión, sin endpoint. La consulta de estado conserva PENDIENTE_VERIFICACION como incertidumbre aunque exista un marcador huérfano cuya inconsistencia no pueda expresarse en otra dimensión. El lector de valores admite raíces inciertas con identidad conocida para consultar evidencia histórica; las guardas de creación/materialización/almacenamiento siguen impidiendo nuevos efectos sobre ellas.

## Verificación ejecutada

| Verificación | Resultado y alcance |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS, arnés Framework DOC-110/DOC-111 y regresión DDL de UPDATE de intentos. |
| Completitud y alcance | PASS: raíz/lote completos solo con pruebas positivas; dos raíces/archivos homónimos independientes; una raíz lista no oculta la pendiente. |
| Parcial | PASS: rechazados, pendientes, cantidades/bytes y snapshot originales conservados. |
| Incertidumbre | PASS: ausencia/discordancia/duplicidad de prueba, IDs de otro recurso, valores sin fijar, documento/inventario duplicados, actividad huérfana y raíz incierta impiden completitud. |
| Detención | PASS: actividad antigua permanece en curso; incertidumbre tiene prioridad; detención sin actividad conserva expediente; terminar el último efecto conserva señal y completitud. |
| CAS/repetición | PASS: no cambia archivos/IDs/totales/señal, repetición sin eventos, fallo de cada UPDATE sin publicar estado en el doble, terminalidad contradictoria rechazada. |
| Coordinador después de consolidar | PASS: histórico confirmado con raíz/lote completos e histórico incierto con raíz INCIERTO siguen consultables sin abrir disco ni invocar otro writer. |
| MSBuild Debug completo | PASS, salidas/intermedios `.tmp/doc111-consolidacion-build/`; log local `.tmp/doc111-consolidacion-build.log`, sin despliegue. |
| OpenSpec estricto y `git diff --check` | PASS. Funciones legacy protegidas y SII/ImportarServicioWeb sin diferencias. |

Los estados/evidencias y el ejecutor SQL de consolidación son sintéticos; los casos de coordinación reutilizan PDF local y writer/persistencia dobles. La conexión real de los checkpoints y la consolidación se compila, pero no se ejecuta aquí contra MySQL. Commit/rollback/carreras reales, HTTP/IIS autenticado, archivo/XML/efectos documentales reales y E2E: **NO_EJECUTADO**. No se hereda autorización de corridas anteriores.

10.8 sigue pendiente de reconciliación explícita con evidencia; 10.11 de exposición HTTP del coordinador. Almacenamiento legacy, creación legacy, SII e ImportarServicioWeb permanecen intactos. La exclusión adicional del XML frente a otras cargas continúa descartada por instrucción del usuario; no se reintroduce como requisito.
