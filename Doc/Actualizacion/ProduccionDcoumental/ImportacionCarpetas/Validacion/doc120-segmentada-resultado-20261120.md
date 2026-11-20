# DOC-120 — persistencia segmentada verificada en MySQL

**PASS_SEGMENTED_REPOSITORY**, run `1a31695b3e0c4a9c9f559a205d2dcf3c`, ejecutado 2026-11-20 de 15:50:54 a 15:51:12 UTC. Código `43bcff75076061ddd31b8a745ffc60f8f4464e39`. [Resultado portable, consumos y huellas](doc120-segmentada-resultado-20261120.json); [ventana, autorización y antecedente](doc120-segmentada-ventana-20261120.md). Consola FINISHED, claves eliminadas y ventana cerrada; no relanzar.

| Comprobación real | Resultado |
| --- | --- |
| Recursos E2E reutilizados | Propietario 141, empresa 2, nivel 7, perfil 24/publicación 38, gabinete 10. |
| Identidades nuevas conservadas | Preparación **9**, plan **93**, lote **56**. |
| Inventario | Una raíz y dos archivos; **89.570 bytes declarados** desde los dos PDF seleccionados. Correlación por IDs cliente, raíz, bytes y documento de origen completo. |
| Partes | Tres posiciones únicas; archivos 2→1, duplicado idempotente, contenido distinto rechazado y dos conexiones convergentes sobre la misma parte. |
| Borrador y revisión | Rechaza sellado incompleto/sin validar, expiración y revisión antigua; conserva validaciones y sello coherente. |
| Sellado y recuperación | Una preparación sellada y un plan esquema 2 íntegro; dos solicitantes y recuperación devuelven la misma identidad/huella. Seis fragmentos finales. |
| Tres rollbacks MySQL | ANTES_EVENTO, EVENTO_INSERTADO y PLAN_CONFIRMADO, con fila/evento/revisión provisional comprobados dentro de la transacción; después cero lote publicado y plan original conservado. |
| Confirmación concurrente y recuperación | Un lote, una raíz, dos archivos pendientes, un evento LOTE_CONFIRMADO y cero intentos; confirmación repetida conserva el mismo lote. |
| Conservación | **16 grupos de huellas preexistentes iguales**, incluidos perfiles/reglas/permisos, preparación/partes/resultados e inventario previo; conteos de expedientes/documentos sin cambio. |
| Fuentes | **145 fuentes/dependencias**, cero discrepancias al finalizar. DLL conservada, sin instalación. |

Se reutilizaron los ejecutores DOC-115/DOC-111 y el núcleo de rollback existente; la ampliación específica es la preparación persistente segmentada. Las conexiones del repositorio fueron físicas, sin pooling. No se incorporó un ejecutor alternativo.

## Consumo y límites

33/48 llamadas de repositorio; 24/64 SELECT nativos de control. El envoltorio hizo 53 SELECT en esta corrida y conserva los 29 de la anterior: **82/96 acumulados**. Total de controles de ambas ventanas: **106/160**, sin reiniciar el consumo. Los SELECT internos del repositorio, validación de esquema y catálogo no se cuentan como controles externos.

La primera ventana `92158d88c6d94d328c5af9f2cb562d3d` sigue cerrada STOPPED tras sus 29 SELECT, antes de mutaciones. Se conserva su journal por SHA-256 y la corrección local del adaptador de destino. La continuación tuvo identidad y recursos sellados propios, abiertos mediante aprobación de consola; no se reabrió la ventana anterior.

Cero HTTP de aplicación, bytes PDF enviados, recepciones, creación de expedientes, almacenamiento documental, SII, borrados o DDL. Los dos originales y los datos previos se conservan; permanecen los recursos de inventario nuevos del ensayo. Las transacciones revertidas pueden consumir autoincrementos, que no se restauran.

## Alcance de aceptación

Se completa **3.118** en la verificación de persistencia, junto a la cobertura local anterior de contrato/cliente/servicio. La brecha de carrera y fallo/rollback MySQL queda acreditada por esta corrida real. Avance **260/270**, diez tareas abiertas.

La concurrencia real fue entre dos conexiones de repositorio. No se ejecutaron dos pestañas de navegador ni autenticación HTTP en esta ventana. El fixture usa plan/valores preconstruidos y sello de catálogo de prueba; no acredita la resolución operativa de proveedores, envío binario, selección completa de UI, inicio/árbol, capacidad de 1.000 documentos o 3.187. Las evidencias locales/HTTP históricas conservan sus propios SHA y límites. No se declara aceptación integral, archivo o cierre global de DOC-120.
