# DOC-118 — ejecutor por fases y ciclo de vida

Las siete fases del ejecutor están implementadas y verificadas localmente. El observador real pasó disponibilidad, breakpoint de fuente, INSPECT y CLOSE con ack de limpieza y salida0 en sesión supervisada. No se ejecutaron ARM/Freeze/Thaw, fases documentales ni nuevas sesiones autenticadas. Consumo acumulado699 HTTP C02,0 C03,17 SELECT y5 aperturas;0 aperturas disponibles. Progreso16/27: COORD02/03 siguen NO_EJECUTADOS para aceptación.

## Preparación y finalización

case-preparation reutiliza driver, PDF, páginas y controles DOC111. Comprueba nombres nuevos por SELECT antes del plan; confirma una sola vez, recibe ambos PDFs y crea ambos expedientes antes de materializar ambos archivos. Fija inventario, contexto, snapshot, revisiones e identidades antes de ALMACENAR. Una respuesta perdida no provoca repetición automática.

case-completion exige checkpoint positivo concordante antes del único replay exacto de A. C02 termina B solo después de probar A, conteos de escritor e invariancia del replay, y verifica físicamente ambos documentos. C03 conserva A, prueba ausencia documental de B, cierra el contexto A y consulta desde B; el intento posterior usa revisión vigente y debe rechazarse sin escritor. Guarda IDs públicos y deja integrationPass=false para revisión. El corte de retorno es CLIENT_API_RESPONSE_DELIVERY, no una pérdida de red: se retiene la respuesta de Playwright, se prueba checkpoint y se suprime su entrega al driver.

La consulta completa del inventario ocurre antes de la ventana congelada. Dentro de ella, el control del archivo B usa un SELECT acotado con revisión, raíz, contexto y snapshot vigentes; la detención exige otro SELECT que pruebe señal y reclamación todavía activa. Esto evita cinco aperturas ODBC durante la pausa. No habilita takeover ni cambia timestamps.

## Compuertas y cierre

Antes de pedir secretos, la consola exige un único observador del mismo runId con proceso/worker vivos, heartbeat reciente, fuente resuelta y hashes coincidentes de controlador/protocolo/fuente. Repite la comprobación tras la captura y antes de cada apertura del preflight. Un marcador exclusivo bloquea corridas paralelas o abandonadas; solo su propietario lo libera tras cierre confirmado. Cada operación verifica alcance vigente, tiempo de caso y sello de dependencias. Cerrar una consola a la fuerza no se presenta como finally reconocido.

El launcher supervisado start-doc118-console.ps1 conserva su proceso padre esperando la consola enmascarada. El controlador también debe mantenerse en una sesión exec supervisada. La causa de salida de la consola antigua sigue desconocida; esta entrega no la atribuye al host ni al usuario.

## Defecto reproducido del controlador

El nuevo File.Replace del guardado de estado recibió cadena vacía como backup al pasar null desde PowerShell. La sesión supervisada produjo ArgumentException y salida1. Se corrigió con Doc118AtomicStatus.Write: el null se pasa desde C# y el protocolo PowerShell verifica escritura inicial y reemplazo por CLOSED. La última comprobación real resolvió fuente, procesó INSPECT/CLOSE y reconoció cleanupConfirmed=true; el proceso supervisado terminó0 y no quedó el IDE privado. No hubo ARM ni retención de hilo.

## Verificación y límite de evidencia

160/160 pruebas locales:13 preflight,33 enlace,16 coordinación,63 drivers,4 símbolos,4 observador,6 preparación,7 finalización,3 corte de retorno,4 ciclo de vida,6 composición y1 protocolo PowerShell (varios casos internos, no sumados como tests Node). Se conservan18 pruebas previas disjuntas del servidor/cliente. Fuente VB, almacenamiento compartido, SII, binarios, permisos y configuración permanecen fuera de cambios.

El informe doc118-observer-readiness-20261108.json registra dos intentos HEAD asignados: el primero no confirmó resultado y el segundo devolvió200. No se reembolsaron;699 HTTP conservan todos los intentos previos. La disponibilidad del observador no demuestra progresión del peer durante retención. execution-fresh-window-proposal.json propone7 aperturas totales y1200 HTTP (1125/75), conserva699/17/5 consumidos y todavía requiere autorización.
