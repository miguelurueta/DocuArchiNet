# DOC-118 — coordinación y recuperación

## Corte actual de RV-04

Implementada6.4/D-14/RQ-14:persistencia V2 del repositorio existente, integrada localmente con el adaptador y un doble transaccional.20 casos SQL nuevos;223/223 Node, DDL ampliado válido. Gate de un solo uso, evidencia/diario concordantes y checkpoint/evento/consolidación en la transacción exterior. MySQL real, composición pública y recuperación no acreditados/activados;69 protegido. Progreso21/31. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-persistencia-v2-local-20261108.md`.

## Corte anterior de RV-03

Implementada6.3/D-13/RQ-13:recorrido interno del adaptador con retención antes del inicio, acuses obligatorios y observación en finally.20 casos nuevos con staging real y dobles de persistencia/escritor; regresión223/223 Node. Cancelación/ThreadAbort propagados, sin fallback V1. Composición pública y MySQL transaccional pendientes de RV-04,69 protegido. Progreso20/30, mismas10 tareas anteriores. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-adaptador-v2-local-20261108.md`.

## Corte anterior de RV-02-B

Implementada6.2/D-12/RQ-12:puerto interno no destructivo del proveedor de staging, validado en21 casos sobre PDFs sintéticos y3 procesos propios cerrados. Regresión223/223 Node. Retención, identidad y sección sincronizada frente a Dispose; otro proceso/nodo o reinicio conserva bloqueo. V1 intacto y recuperación sin activar,69 protegido. Progreso19/29, mismas10 tareas anteriores. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-puerto-staging-v2-local-20261108.md`.

## Corte anterior de RV-01

Implementada6.1/D-11/RQ-11: contratos internos de inicio, autorización y retorno V2, con codec cerrado, huellas canónicas y transformaciones sin efectos.19 casos V2 pasan en el harness existente; regresión18/18 y herramientas205/205. JSON válido no concede autoridad ni prueba procedencia durable. Progreso18/28 por una tarea nueva completada; quedan las mismas10 tareas anteriores. Proveedor real y recuperación siguen pendientes,69 protegido. Evidencia:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-contratos-v2-local-20261108.md`.

## Corte anterior de RV-02-A

Las primitivas de exclusión sobre archivos sintéticos NTFS pasaron13/13 pruebas con6 procesos propios cerrados. La regresión conjunta pasa205/205. Se comprobaron lectores activos, salida, competencia, identidad/hash, copias, reemplazos, hardlinks y pérdida de guardia. El control de apertura después de liberar exige fencing durable; no se habilita recuperación con un lock aislado. RV-02-B, integración y calificación del proveedor real, sigue pendiente. Progreso17/27; intento69 protegido. Evidencia:Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-staging-lock-probe-20261108.md.

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: ejecutor de siete fases verificado localmente; disponibilidad/cierre de observador PASS, retención SIN_CALIFICAR. Progreso16/27.

## Objetivo

Precisar las brechas COORD-02 (dos clientes, antigüedad y pérdida de respuesta durante llamada activa) y COORD-03 (detención en vuelo y consulta posterior), reutilizando COORD-01 histórico. El refinamiento contiene diez decisiones D-01–D-10 y diez requisitos RQ-01–RQ-10; no se cambia producto sin una brecha reproducida.

## Alcance y compatibilidad

Se reutilizan coordinador VB, MySQL, ASMX y JavaScript actuales. Se preservan políticas EXPEDIENTE/LOTE/PARCIAL, PDF, rechazo TIFF/Multi-TIFF, snapshot, inventario original y efectos confirmados. No hay migración, nuevo worker ni endpoint para reanudar. No se modifica almacenamiento compartido/SII ni se introduce aislamiento adicional del XML. Esta documentación se revierte por Git; eso no revierte efectos en datos.


Las siete fases del ejecutor están implementadas y verificadas localmente. El observador real pasó disponibilidad, breakpoint de fuente, INSPECT y CLOSE con ack de limpieza y salida0 en sesión supervisada. No se ejecutaron ARM/Freeze/Thaw, fases documentales ni nuevas sesiones autenticadas. Consumo acumulado699 HTTP C02,0 C03,17 SELECT y5 aperturas;0 aperturas disponibles. Progreso16/27: COORD02/03 siguen NO_EJECUTADOS para aceptación.


## Corte posterior a la preparación del lote24

Se conservó la preparación real de C02 (plan40/lote24, expedientes670/671) tras un fallo del observador anterior al primer ALMACENAR. Se corrigió la espera de binding y ARM/RESUME/CLOSE pasaron sin llamadas al escritor. La reconstrucción local relee identidades y ausencia física sin recrear recursos. Véase `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-preparacion-recuperacion-20261108.md`.


## Ventana real y auditoría posterior

RUN02 se detuvo con fallo del depurador. Lote24/archivo40/intento69 permanece EN_CURSO, con reclamación activa y sin resultado durable; archivo41 mantiene NO_INICIADO y ausencia física verificada. No se puede afirmar cero efectos de A ni repetirlo. Acumulado1276 HTTP/95 SELECT/9 aperturas; C03 no iniciado. La candidata local sincroniza Break/Thaw/Go y conserva historial propio. Regresión187/187, SDK compilado, sin prueba documental posterior de esa corrección. Progreso16/27. Detalle:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-ventana-activa-auditoria-20261108.md`.


## Ausencia física y prueba sintética de liberación

La auditoría encontró0 registros físicos de A/B y mantuvo intacto el intento69 EN_CURSO; no se permite replay. La contención4.6 queda acreditada:17/27. La mecánica SDK Freeze/peer/Break/Thaw/Go pasó en un proceso propio con PDB y fuente sintéticos, sin IIS ni efectos documentales; legacy sigue sin calificar. Regresión192/192 y2/2 comportamiento existentes. Acumulado1276 HTTP/113 SELECT/9 aperturas. La evaluación de un diseño de recuperación EN_CURSO está propuesta, sin implementación o mutación autorizadas. Véase `doc118-auditoria-fisica-liberacion-20261108.md`.
