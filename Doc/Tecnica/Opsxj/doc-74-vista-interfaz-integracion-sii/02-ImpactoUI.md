# VISTA-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-74
- Cambio OpenSpec: doc-74-vista-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

- [x] `workflow/Webworkflow.aspx`: panel de preview dentro del modal existente, con estados, frame, renovación, descarga y retorno.
- [x] `Styles/importar-servicio-web-modern.css`: panel lateral en escritorio y subvista completa en ancho reducido.
- [x] `importar-servicio-web-ui.js`: foco, scroll, cierre, retorno y delegación segura al visor vigente.
- [x] Adaptador SII: acción de preview por fila e identidad interna opcional.

## Estados de interacción

- [x] Foco: entra al título de la subvista y vuelve al disparador al regresar.
- [x] Selección y scroll: la lista no se reconstruye al abrir preview y su posición se restaura al volver.
- [x] Responsive: escritorio conserva lista y panel; móvil muestra subvista con **Volver a la lista**.
- [x] Accesibilidad: nombres accesibles, región `aria-live`, botones nativos y frame titulado.

## Validacion visual

Las invariancias estructurales y pruebas Node están aprobadas. La validación visual en navegador autenticado queda pendiente de autorización explícita para ambiente y cuenta de prueba; no se activó el gate ni se ejecutó E2E real.
