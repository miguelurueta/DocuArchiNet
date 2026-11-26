# DOC127: asistente actual XLS y XLSX aprobado

**2/2 E2E reales PASS**, sin reintentos. Se completa5.2 con la matriz de plantillas y sus niveles de evidencia. **31/33 tareas** al incorporar la revisión de hallazgos; capacidad18/18 y autoridad6/6 conservadas.

Lote `c659ec0b64d94681bccaaa8e6e75e4d0`, autorizado por el usuario mediante «autorizado». SHA de ejecución `ca8d352fc93497592a9f4333298f4ab9596f268d`. Finalizado 2026-11-26T19:35:19.378Z; consola cerrada naturalmente, sin terminación forzada.

## Casos ejecutados

IIS local HTTPS/GESTOR, empresa2, DA11/RA12, conexión6 y cuenta194 con sólo cargas habilitadas. Una contraseña efímera, dos sesiones secuenciales y segundo caso condicionado al PASS/control posterior del primero.

| Formato | Plantilla/revisión | Destino | Carga | Filas |
| --- | --- | --- | --- | --- |
| XLS | 76/r1 | imp_02_doc126_e2e_asistente_fab26227 | 3671f38d-92d2-4605-8253-b6f317d8904a |1000 |
| XLSX | 77/r1 | imp_02_doc126_e2e_asistente_3d2f2ad4 | 3e2d5b0b-902c-4a99-acaa-817ada56f24f |1000 |

Ambos recorrieron la interfaz actual: analizar, revisar propuesta, guardar una plantilla de dos campos, conservar el archivo, recibir, preparar, confirmar una vez pese al doble clic y consultar el resultado por ID. Hoja Datos y clave CLAVE. Guardar la plantilla no creó aún su tabla ni importó filas. Los tipos analizados, hash del archivo, propietario, conexión y revisión coincidieron con el alcance.

## Conservación y efectos

El caso XLS comparó íntegramente38 tablas protegidas y el XLSX39, incluyendo la tabla XLS recién creada. La configuración anterior y la primera alta se conservaron. SELECT antes/después de cada caso verificaron78 perfiles,41 cargas,38 cabeceras y134 campos anteriores idénticos a la preparación.

Efectos autorizados retenidos: dos plantillas, cuatro campos, dos tablas y2000 filas nuevas. Cero borrados, cambios de filas anteriores, permisos, instalación/limpieza SQL o despliegues. Se derivan40 tablas imp_02_,40 plantillas,138 campos y43 cargas totales de las conservadas y las dos altas verificadas.

Los controles externos de línea base/postflight suman64 SELECT; los controles internos de la suite se contabilizan aparte y no se instrumentaron. Cada test terminó1 passed después del bucle completo de huellas; ambos controles posteriores pasaron. Los20 hashes de runtime/scripts/recursos permanecen iguales a los autorizados.

## Matriz y límites

La creación web actual XLS/XLSX se combina con12 casos locales de compatibilidad histórica/nueva y seis usos reales por formato/modo de la capacidad. Estos últimos usan plantillas existentes y no se anuncian como altas web. La creación/edición web para AGREGAR/REEMPLAZAR permanece explícitamente sin ejecutar; no era efecto autorizado de este lote.

La historia XLSX de DOC126 conserva su SHA/ID y sus diferencias de Vista. Esta ventana acredita el asistente con la Vista actual. No repitió archivos consumidos, confirmaciones ni capacidad para obtener mediciones adicionales.

El [registro de hallazgos](doc127-hallazgos-cierre-20261126.md) completa7.1 con evidencia clasificada. Quedan EXPLAIN manual2.4 y gates/QA de cierre7.5. No se archiva ni cierra Jira. La aceptación automatizada no se convierte en QA humana. Credenciales y cookies no se publicaron ni guardaron; contextos cerrados y variables limpiadas por finally. Datos de ejecución y log saneado permanecen excluidos de Git en .tmp/doc127-assistant-ventana/.

El JSON adjunto conserva autorización, SHA, IDs, hashes, campos, controles y restricciones. Esta autorización quedó consumida y no permite nuevas altas o reintentos.
