# PROMPT ARQUITECTONICO  Ticket 04 FE
# Implementar Workbench de Documentos (core layout)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Implementar el workbench del tab **Documentos** usando `AppToolbar` y `AppCollapseRail`, con area principal scrollable y panel lateral colapsable, respetando el contrato de tabs existente.


CONTEXTO EXISTENTE

- arquitectura de referencia: `docs/Architecture/ImplementacionVisualGestionCorrespo/ImplemetacionContenidoTabsDocumentos.md`
- contrato de `AppTabs`: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- contrato de `AppCollapseRail`: `docs/Architecture/AppCollapseRail/AppCollapseRail-Architecture.md`
- referencia visual del modulo: `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab`


UBICACION (OBLIGATORIA)

```
src/modules/gestionCorrespondencia/components/documentosWorkbench/
```


RESTRICCIONES (OBLIGATORIAS)

- no modificar `AppTabs`, `AppToolbar` ni `AppCollapseRail`
- no introducir logica de negocio ni llamadas a APIs
- no crear estilos globales
- no tocar el tab **Gestion** ni el contrato de `GestionRespuesta.tsx`
- componentes desacoplados y presentacionales


CONTRATO (OBLIGATORIO)

- el tab **Documentos** renderiza `DocumentosWorkbench`
- `DocumentosWorkbench` controla `collapsed` y `onToggle`
- `AppCollapseRail` se usa con:

```tsx
<AppCollapseRail
  title="Visualizar documentos"
  collapsed={collapsed}
  onToggle={toggle}
  placement="right"
  variant="inline"
>
```


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. layout principal en columna con toolbar arriba
2. zona principal en fila con `overflow: hidden`
3. area principal con scroll solo si excede
4. panel lateral con contenido interno scrollable
5. contenido del panel no se desmonta al colapsar


RIESGOS A EVITAR (OBLIGATORIO)

- desmontar el contenido al colapsar
- romper el contrato de tabs en `GestionRespuesta.tsx`
- mezclar estado controlado/no controlado
- acoplar el workbench a la logica de negocio


PRUEBAS UNITARIAS (OBLIGATORIAS)

- renderiza `DocumentosWorkbench` dentro del tab **Documentos**
- `AppCollapseRail` recibe props correctas
- toggle cambia el estado `collapsed`


PRUEBAS QT (CALIDAD / E2E)

- render correcto del layout en desktop
- panel colapsa/expande sin perder contenido


CRITERIOS DE ACEPTACION

- workbench visible en tab **Documentos**
- `AppToolbar` arriba, area principal a la izquierda, rail a la derecha
- `AppCollapseRail` con props exactas y contenido persistente
- sin cambios en el tab **Gestion**
