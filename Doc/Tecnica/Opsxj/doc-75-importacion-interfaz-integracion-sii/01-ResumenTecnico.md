# IMPORTACION-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-75
- Cambio OpenSpec: doc-75-importacion-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Unifica la preparación previa a escritura para una o varias filas SII. La solución agrega estado, colección y cliente frontend sobre B03/B09/B11, sin ejecutar intenciones ni modificar persistencia.

## Alcance y compatibilidad

- [x] Superficies: `Webworkflow.aspx(.vb)`, CSS moderno, UI/adaptador SII y tres módulos canónicos DOC-75.
- [x] Se preservan gate apagado, cliente ASMX único, DOC-74, almacenamiento y mutadores legacy; rollback retira únicamente markup, listeners, estilos y registros aditivos.
