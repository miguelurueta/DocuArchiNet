# NUCLEO-INTRFAZ-INTEGRACION-SII

- Ticket: DOC-72
- Cambio OpenSpec: doc-72-nucleo-intrfaz-integracion-sii
- Clasificación: cross_cutting

## Superficies UI

La entrada moderna es `ctw-document-action-service`. El modal aditivo contiene título, cierre, estado vivo y listado. Con gate autorizado se oculta `btnloadservice`; con gate apagado el script no se carga y el recorrido anterior permanece intacto.

## Validación visual

La estructura contempla escritorio y móvil, scroll interior, foco visible, Escape, ciclo Tab/Shift+Tab y restauración de foco. No se realizó recorrido autenticado ni captura porque faltó autorización explícita de ambiente y cuentas; la estructura quedó cubierta por pruebas focales.
