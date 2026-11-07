# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado y preparación local en curso; sin despliegue ni integración real nueva.

## Objetivo

Precisar las brechas COORD-02 (dos clientes, antigüedad y pérdida de respuesta durante llamada activa) y COORD-03 (detención en vuelo y consulta posterior), reutilizando COORD-01 histórico. El refinamiento contiene diez decisiones D-01–D-10 y diez requisitos RQ-01–RQ-10; no se cambia producto sin una brecha reproducida.

## Alcance y compatibilidad

Se reutilizan coordinador VB, MySQL, ASMX y JavaScript actuales. Se preservan políticas EXPEDIENTE/LOTE/PARCIAL, PDF, rechazo TIFF/Multi-TIFF, snapshot, inventario original y efectos confirmados. No hay migración, nuevo worker ni endpoint para reanudar. No se modifica almacenamiento compartido/SII ni se introduce aislamiento adicional del XML. Esta documentación se revierte por Git; eso no revierte efectos en datos.


El usuario indicó «continua» después del detalle de implementación el 2026-11-07. Se aplicó el mapa de 25 tareas trazables y se prepararon los protocolos de dos sesiones, respuesta perdida y detención; su observación externa sigue sin calificación en runtime.


La única incorporación de código es un helper de coordinación de pruebas en tools/e2e, con callbacks para sesiones autorizadas/observador externo. No cambia código funcional ni concede PASS de integración; tiene 16 pruebas locales propias.
