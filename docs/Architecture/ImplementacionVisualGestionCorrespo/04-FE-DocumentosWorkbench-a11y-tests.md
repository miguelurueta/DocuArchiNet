# PROMPT ARQUITECTONICO  Ticket 06 FE
# Workbench Documentos (accesibilidad + pruebas)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Garantizar accesibilidad y pruebas del workbench de **Documentos** con foco en el panel colapsable.


CONTEXTO EXISTENTE

- arquitectura de referencia: `docs/Architecture/ImplementacionVisualGestionCorrespo/ImplemetacionContenidoTabsDocumentos.md`
- componentes shared: `AppToolbar`, `AppCollapseRail`


UBICACION (OBLIGATORIA)

```
src/modules/gestionCorrespondencia/components/documentosWorkbench/
```


RESTRICCIONES (OBLIGATORIAS)

- no modificar `AppCollapseRail`
- no introducir logica de negocio
- no crear estilos globales


CONTRATO (OBLIGATORIO)

- toggles con `aria-expanded`
- `aria-controls` apunta al panel
- foco visible y navegacion por teclado


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. asegurar atributos ARIA en toggles del workbench
2. permitir accion con teclado (Enter/Espacio) donde aplique
3. estados de foco visibles en rail y header


PRUEBAS UNITARIAS (OBLIGATORIAS)

- valida `aria-expanded` y `aria-controls`
- toggle por click cambia estado `collapsed`
- contenido del panel permanece montado


PRUEBAS QT (CALIDAD / E2E)

- navegacion por teclado para toggle
- comportamiento accesible en mobile y desktop


CRITERIOS DE ACEPTACION

- accesibilidad validada sin regresiones
- pruebas cubren estado colapsado/expandido y atributos ARIA
