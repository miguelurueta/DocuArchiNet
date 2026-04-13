# PROMPT ARQUITECTONICO  Ticket 05 FE
# Workbench Documentos (responsive + UI/UX)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Aplicar el comportamiento responsive y la presentacion visual del workbench de **Documentos** con un look enterprise consistente con el modulo.


CONTEXTO EXISTENTE

- arquitectura de referencia: `docs/Architecture/ImplementacionVisualGestionCorrespo/ImplemetacionContenidoTabsDocumentos.md`
- referencia visual: `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab`
- componentes shared: `AppToolbar`, `AppCollapseRail`


UBICACION (OBLIGATORIA)

```
src/modules/gestionCorrespondencia/components/documentosWorkbench/
```


RESTRICCIONES (OBLIGATORIAS)

- no cambiar el comportamiento de `AppCollapseRail`
- no usar estilos globales
- no modificar el tab **Gestion**
- no agregar logica de negocio


CONTRATO (OBLIGATORIO)

- Desktop: `variant="inline"` y `collapsed=false` por defecto
- Tablet: `collapsed=true` por defecto
- Mobile: `variant="overlay"` con rail visible como chip


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. aplicar bordes redondeados 12px-16px y sombras suaves
2. separar visualmente panel y contenido
3. hover y focus visibles en toggles y acciones
4. rail flotante abajo derecha en mobile
5. panel overlay tipo bottom-sheet con 70-80% de altura


RIESGOS A EVITAR (OBLIGATORIO)

- degradar la legibilidad en mobile
- perder el rail visible en mobile
- estilos que rompan la coherencia del modulo


PRUEBAS UNITARIAS (OBLIGATORIAS)

- estado inicial `collapsed` cambia segun viewport (desktop/tablet)
- en mobile se aplica `variant="overlay"`


PRUEBAS QT (CALIDAD / E2E)

- rail visible en mobile y panel aparece como bottom-sheet
- desktop mantiene layout horizontal completo


CRITERIOS DE ACEPTACION

- comportamiento responsive consistente con la arquitectura
- UI enterprise con separacion clara de panel y contenido
- rail visible y usable en mobile
