# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: preflight autenticado PASS; observador SIN_CALIFICAR y revisión de consola pendiente. Progreso16/27, sin efectos documentales.

## Objetivo

Precisar las brechas COORD-02 (dos clientes, antigüedad y pérdida de respuesta durante llamada activa) y COORD-03 (detención en vuelo y consulta posterior), reutilizando COORD-01 histórico. El refinamiento contiene diez decisiones D-01–D-10 y diez requisitos RQ-01–RQ-10; no se cambia producto sin una brecha reproducida.

## Alcance y compatibilidad

Se reutilizan coordinador VB, MySQL, ASMX y JavaScript actuales. Se preservan políticas EXPEDIENTE/LOTE/PARCIAL, PDF, rechazo TIFF/Multi-TIFF, snapshot, inventario original y efectos confirmados. No hay migración, nuevo worker ni endpoint para reanudar. No se modifica almacenamiento compartido/SII ni se introduce aislamiento adicional del XML. Esta documentación se revierte por Git; eso no revierte efectos en datos.


El preflight de dos sesiones pasó con publicación38/gabinete10 y17 SELECT. Consumo acumulado697 HTTP C02,0 C03 y5 aperturas;0 fases documentales y0 efectos. El worker26244 dejó de estar activo por causa no determinada. La consola terminó sin reconocer la orden CLOSE ni el finally; no quedan consola/shell ni hijos directos detectados. Las instancias privadas de depuración ya no están presentes y los dos IDE originales quedaron sin depuración activa, con12 breakpoints del usuario conservados. La observación interna sigue SIN_CALIFICAR. Evidencia: doc118-authenticated-preflight-20261107.json.
