# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado; continuación detenida por tope HTTP,16/27 tareas. Primera sesión autenticada, sin pareja completa ni efectos documentales.

## Objetivo

Precisar las brechas COORD-02 (dos clientes, antigüedad y pérdida de respuesta durante llamada activa) y COORD-03 (detención en vuelo y consulta posterior), reutilizando COORD-01 histórico. El refinamiento contiene diez decisiones D-01–D-10 y diez requisitos RQ-01–RQ-10; no se cambia producto sin una brecha reproducida.

## Alcance y compatibilidad

Se reutilizan coordinador VB, MySQL, ASMX y JavaScript actuales. Se preservan políticas EXPEDIENTE/LOTE/PARCIAL, PDF, rechazo TIFF/Multi-TIFF, snapshot, inventario original y efectos confirmados. No hay migración, nuevo worker ni endpoint para reanudar. No se modifica almacenamiento compartido/SII ni se introduce aislamiento adicional del XML. Esta documentación se revierte por Git; eso no revierte efectos en datos.


Progreso16/27. La continuación real autorizada autenticó A y consultó lote23; B quedó sin confirmar al agotar C02. Consumo acumulado425/0 HTTP,3 aperturas y0 SELECT de control/efectos. Se cerraron sesiones y borraron secretos. La reducción de tráfico y el guard contra reinicio de contadores pasan120/120 pruebas locales; no sustituyen aceptación real. Solo PREFLIGHT está implementado en la consola. El observador y las fases documentales siguen pendientes. La ampliación acotada1000 HTTP/5 aperturas (925/75) está propuesta, sin aplicar.
